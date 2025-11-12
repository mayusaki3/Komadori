/**
 * ControlCore 単体テスト
 *
 * TestID 対応:
 * - TC-01: 正常ロード
 * - TC-02: 重複座標エラー
 * - TC-03: 未定義ページ / 不正ページ文脈
 * - TC-04: OBS シーン切替
 * - TC-05: 配信/録画トグル
 * - TC-06: HTTP 呼び出し＋表示更新
 * - TC-07: ページ切替同期
 * - TC-08: 不明 action.type エラー
 * - TC-09: NO_BUTTON エラー
 * - TC-10: toggleMute 実行
 * - TC-11: setSourceVisibility 実行
 * - TC-12: http.post 実行
 * - TC-13: HTTP エラー時の HTTP_FAILED
 */

import { describe, it, expect, vi, beforeEach } from "vitest";
import { fileURLToPath } from "node:url";
import * as path from "node:path";
import * as fs from "node:fs";
import {
  ControlCore,
  ControlCoreOptions,
  ClientBroadcaster,
  ObsController,
  HttpClient,
  ButtonClickMessage,
} from "../src/control-core";

/**
 * 共通モック生成
 */
function createMocks() {
  const sent: any[] = [];

  const broadcaster: ClientBroadcaster = {
    broadcast: (msg) => {
      sent.push(msg);
    },
  };

  const obs: ObsController = {
    setScene: vi.fn().mockResolvedValue(undefined),
    toggleStream: vi.fn().mockResolvedValue(undefined),
    toggleRecord: vi.fn().mockResolvedValue(undefined),
    toggleMute: vi.fn().mockResolvedValue(undefined),
    setSourceVisibility: vi.fn().mockResolvedValue(undefined),
    getStatus: vi.fn().mockResolvedValue({
      streaming: false,
      recording: false,
    }),
  };

  const http: HttpClient = {
    get: vi.fn().mockResolvedValue({ status: "OK" }),
    post: vi.fn().mockResolvedValue({ status: "OK" }),
  };

  return { broadcaster, obs, http, sent };
}

/**
 * 一時 panel.json を書き出すユーティリティ
 */
function writeTempConfig(name: string, json: unknown): string {
  const baseDir = path.dirname(fileURLToPath(import.meta.url));
  const filePath = path.join(baseDir, name);
  fs.writeFileSync(filePath, JSON.stringify(json), "utf-8");
  return filePath;
}

/**
 * デフォルト panel.json のパス
 */
const defaultConfigPath = fileURLToPath(
  new URL("../config/panel.json", import.meta.url),
);

describe("ControlCore", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("TC-01: 正常ロードで page.update を送信する", () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const core = new ControlCore({
      configPath: defaultConfigPath,
      broadcaster,
      obs,
      http,
    });

    core.initialize();

    const update = sent.find((m) => m.type === "page.update");
    expect(update).toBeTruthy();
    expect(update.payload.currentPage).toBe("main");
  });

  it("TC-02: 重複座標エラーで page.update を送信しない", () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const dupConfigPath = writeTempConfig("panel-dup.json", {
      version: 1,
      pages: {
        main: {
          name: "Main",
          buttons: [
            { x: 0, y: 0, label: "A" },
            { x: 0, y: 0, label: "B" },
          ],
        },
      },
    });

    const core = new ControlCore({
      configPath: dupConfigPath,
      broadcaster,
      obs,
      http,
    });

    expect(() => core.initialize()).toThrow();

    const update = sent.find((m) => m.type === "page.update");
    expect(update).toBeUndefined();
  });

  it("TC-03: 未定義ページ / 不正ページ文脈で INVALID_PAGE_CONTEXT を返す", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const core = new ControlCore({
      configPath: defaultConfigPath,
      broadcaster,
      obs,
      http,
    });
    core.initialize();

    const msg: ButtonClickMessage = {
      type: "button.click",
      payload: {
        page: "unknown", // currentPage と違う値
        x: 0,
        y: 0,
        source: "dock",
      },
    };

    await core.handleButtonClick(msg);

    // OBS/HTTP は呼ばれない
    expect(obs.setScene).not.toHaveBeenCalled();
    expect(http.get).not.toHaveBeenCalled();

    const err = sent.find((m) => m.type === "error");
    expect(err).toBeTruthy();
    expect(err.payload?.code).toBe("INVALID_PAGE_CONTEXT");
  });

  it("TC-04: button.click で obs.setScene が呼ばれる", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const core = new ControlCore({
      configPath: defaultConfigPath,
      broadcaster,
      obs,
      http,
    });
    core.initialize();

    const msg: ButtonClickMessage = {
      type: "button.click",
      payload: { page: "main", x: 2, y: 0, source: "dock" },
    };

    await core.handleButtonClick(msg);

    expect(obs.setScene).toHaveBeenCalledWith("CameraOnly");
    const err = sent.find((m) => m.type === "error");
    expect(err).toBeUndefined();
  });

  it("TC-05: 配信/録画トグルで各 OBS API が呼ばれる", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const core = new ControlCore({
      configPath: defaultConfigPath,
      broadcaster,
      obs,
      http,
    });
    core.initialize();

    // LIVE ボタン
    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0, source: "dock" },
    });

    // REC ボタン
    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 1, y: 0, source: "dock" },
    });

    expect(obs.toggleStream).toHaveBeenCalledTimes(1);
    expect(obs.toggleRecord).toHaveBeenCalledTimes(1);

    const status = sent.filter((m) => m.type === "status.update");
    expect(status.length).toBeGreaterThanOrEqual(1);
  });

  it("TC-06: HTTP 呼び出し＋表示更新 (http.get)", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    // displayKey を利用する定義
    const httpConfigPath = writeTempConfig("panel-http.json", {
      version: 1,
      pages: {
        main: {
          name: "Main",
          buttons: [
            {
              x: 0,
              y: 0,
              label: "STATUS",
              action: {
                type: "http.get",
                url: "https://example.test/status",
                display: "status",
              },
            },
          ],
        },
      },
    });

    const core = new ControlCore({
      configPath: httpConfigPath,
      broadcaster,
      obs,
      http,
    });
    core.initialize();

    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0, source: "dock" },
    });

    expect(http.get).toHaveBeenCalledTimes(1);

    // displayKey "status" によりボタンラベル更新 + page.update が飛ぶ想定
    const updates = sent.filter((m) => m.type === "page.update");
    const serialized = JSON.stringify(updates);
    expect(serialized).toContain("OK");
  });

  it("TC-07: ページ切替同期 main→util", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const core = new ControlCore({
      configPath: defaultConfigPath,
      broadcaster,
      obs,
      http,
    });
    core.initialize();

    sent.length = 0; // 初期更新をクリア

    // main ページ上の page.switch ボタンを押す前提
    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 0, y: 2, source: "dock" },
    });

    const updates = sent.filter((m) => m.type === "page.update");
    expect(updates.length).toBeGreaterThanOrEqual(1);
    const last = updates[updates.length - 1];
    expect(last.payload.currentPage).toBe("util");
  });

  it("TC-08: 不明 action.type で INVALID_ACTION エラー", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const unknownConfigPath = writeTempConfig("panel-unknown-action.json", {
      version: 1,
      pages: {
        main: {
          name: "Main",
          buttons: [
            {
              x: 0,
              y: 0,
              label: "NG",
              action: { type: "unknown.action" },
            },
          ],
        },
      },
    });

    const core = new ControlCore({
      configPath: unknownConfigPath,
      broadcaster,
      obs,
      http,
    });
    core.initialize();

    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0, source: "dock" },
    });

    expect(obs.setScene).not.toHaveBeenCalled();
    expect(http.get).not.toHaveBeenCalled();
    expect(http.post).not.toHaveBeenCalled();

    const err = sent.find((m) => m.type === "error");
    expect(err).toBeTruthy();
    expect(err.payload?.code).toBe("INVALID_ACTION");
  });

  it("TC-09: ボタン未定義座標で NO_BUTTON エラー", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const core = new ControlCore({
      configPath: defaultConfigPath,
      broadcaster,
      obs,
      http,
    });
    core.initialize();

    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 4, y: 4, source: "dock" }, // 想定外座標
    });

    expect(obs.setScene).not.toHaveBeenCalled();
    const err = sent.find((m) => m.type === "error");
    expect(err).toBeTruthy();
    expect(err.payload?.code).toBe("NO_BUTTON");
  });

  it("TC-10: toggleMute アクションで obs.toggleMute が呼ばれる", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const muteConfigPath = writeTempConfig("panel-mute.json", {
      version: 1,
      pages: {
        main: {
          name: "Main",
          buttons: [
            {
              x: 0,
              y: 0,
              label: "MUTE",
              action: { type: "obs.toggleMute", source: "Mic/Aux" },
            },
          ],
        },
      },
    });

    const core = new ControlCore({
      configPath: muteConfigPath,
      broadcaster,
      obs,
      http,
    });
    core.initialize();

    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0, source: "dock" },
    });

    expect(obs.toggleMute).toHaveBeenCalledWith("Mic/Aux");
    const err = sent.find((m) => m.type === "error");
    expect(err).toBeUndefined();
  });

  it("TC-11: setSourceVisibility アクションで obs.setSourceVisibility が呼ばれる", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const visConfigPath = writeTempConfig("panel-vis.json", {
      version: 1,
      pages: {
        main: {
          name: "Main",
          buttons: [
            {
              x: 1,
              y: 1,
              label: "SRC",
              action: {
                type: "obs.setSourceVisibility",
                scene: "SceneA",
                source: "Overlay",
                visible: true,
              },
            },
          ],
        },
      },
    });

    const core = new ControlCore({
      configPath: visConfigPath,
      broadcaster,
      obs,
      http,
    });
    core.initialize();

    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 1, y: 1, source: "dock" },
    });

    expect(obs.setSourceVisibility).toHaveBeenCalledWith(
      "SceneA",
      "Overlay",
      true,
    );
    const err = sent.find((m) => m.type === "error");
    expect(err).toBeUndefined();
  });

  it("TC-12: http.post アクションで http.post が呼ばれる", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const postConfigPath = writeTempConfig("panel-http-post.json", {
      version: 1,
      pages: {
        main: {
          name: "Main",
          buttons: [
            {
              x: 0,
              y: 0,
              label: "SEND",
              action: {
                type: "http.post",
                url: "https://example.test/hook",
              },
            },
          ],
        },
      },
    });

    const core = new ControlCore({
      configPath: postConfigPath,
      broadcaster,
      obs,
      http,
    });
    core.initialize();

    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0, source: "dock" },
    });

    expect(http.post).toHaveBeenCalledTimes(1);
    const err = sent.find((m) => m.type === "error");
    expect(err).toBeUndefined();
  });

  it("TC-13: HTTP 実行エラー時に HTTP_FAILED エラーを返す", async () => {
    const { broadcaster, obs } = createMocks();

    const httpError: HttpClient = {
      get: vi.fn().mockRejectedValue(new Error("network error")),
      post: vi.fn(),
    };

    const httpErrConfigPath = writeTempConfig("panel-http-error.json", {
      version: 1,
      pages: {
        main: {
          name: "Main",
          buttons: [
            {
              x: 0,
              y: 0,
              label: "ERR",
              action: {
                type: "http.get",
                url: "https://example.test/error",
              },
            },
          ],
        },
      },
    });

    const sent: any[] = [];
    const broadcasterErr: ClientBroadcaster = {
      broadcast: (m) => sent.push(m),
    };

    const core = new ControlCore({
      configPath: httpErrConfigPath,
      broadcaster: broadcasterErr,
      obs,
      http: httpError,
    });
    core.initialize();

    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0, source: "dock" },
    });

    const err = sent.find(
      (m) => m.type === "error" && m.payload?.code === "HTTP_FAILED",
    );
    expect(err).toBeTruthy();
  });

  it("TC-14: 不正 version で起動エラーになり page.update を送信しない", () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const invalidVersionPath = writeTempConfig("panel-invalid-version.json", {
      version: 999,
      pages: {
        main: {
          name: "Main",
          buttons: [{ x: 0, y: 0, label: "A" }],
        },
      },
    });

    const core = new ControlCore({
      configPath: invalidVersionPath,
      broadcaster,
      obs,
      http,
    });

    expect(() => core.initialize()).toThrow();
    const update = sent.find((m) => m.type === "page.update");
    expect(update).toBeUndefined();
  });

  it("TC-15: ページ未定義で起動エラーになり page.update を送信しない", () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const noPagesPath = writeTempConfig("panel-no-pages.json", {
      version: 1,
      pages: {},
    });

    const core = new ControlCore({
      configPath: noPagesPath,
      broadcaster,
      obs,
      http,
    });

    expect(() => core.initialize()).toThrow();
    const update = sent.find((m) => m.type === "page.update");
    expect(update).toBeUndefined();
  });

  it("TC-16: 座標範囲不正で起動エラーになり page.update を送信しない", () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const outOfRangePath = writeTempConfig("panel-out-of-range.json", {
      version: 1,
      pages: {
        main: {
          name: "Main",
          buttons: [
            { x: 99, y: 0, label: "A" }, // 実装側の許容範囲外を想定
          ],
        },
      },
    });

    const core = new ControlCore({
      configPath: outOfRangePath,
      broadcaster,
      obs,
      http,
    });

    expect(() => core.initialize()).toThrow();
    const update = sent.find((m) => m.type === "page.update");
    expect(update).toBeUndefined();
  });

  it("TC-17: page.switch で page 未指定の場合 INVALID_ACTION エラー", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const cfg = writeTempConfig("panel-missing-page-switch.json", {
      version: 1,
      pages: {
        main: {
          name: "Main",
          buttons: [
            {
              x: 0,
              y: 0,
              label: "SW",
              action: {
                type: "page.switch"
                // page をあえて指定しない
              }
            }
          ]
        }
      }
    });

    const core = new ControlCore({ configPath: cfg, broadcaster, obs, http });
    core.initialize();

    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0, source: "dock" }
    });

    const err = sent.find((m) => m.type === "error");
    expect(err?.payload?.code).toBe("INVALID_ACTION");
  });

  it("TC-18: http.get で url 未指定の場合 INVALID_ACTION エラー", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const cfg = writeTempConfig("panel-missing-http-get-url.json", {
      version: 1,
      pages: {
        main: {
          name: "Main",
          buttons: [
            {
              x: 0,
              y: 0,
              label: "GET",
              action: {
                type: "http.get",
                // url なし
                display: "status"
              }
            }
          ]
        }
      }
    });

    const core = new ControlCore({ configPath: cfg, broadcaster, obs, http });
    core.initialize();

    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0, source: "dock" }
    });

    const err = sent.find((m) => m.type === "error");
    expect(err?.payload?.code).toBe("INVALID_ACTION");
    expect(http.get).not.toHaveBeenCalled();
  });

  it("TC-19: http.post で url 未指定の場合 INVALID_ACTION エラー", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const cfg = writeTempConfig("panel-missing-http-post-url.json", {
      version: 1,
      pages: {
        main: {
          name: "Main",
          buttons: [
            {
              x: 0,
              y: 0,
              label: "POST",
              action: {
                type: "http.post"
                // url なし
              }
            }
          ]
        }
      }
    });

    const core = new ControlCore({ configPath: cfg, broadcaster, obs, http });
    core.initialize();

    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0, source: "dock" }
    });

    const err = sent.find((m) => m.type === "error");
    expect(err?.payload?.code).toBe("INVALID_ACTION");
    expect(http.post).not.toHaveBeenCalled();
  });

  it("TC-20: switchPage を未初期化状態で呼ぶと NOT_INITIALIZED エラー", async () => {
    const { broadcaster, obs, http, sent } = createMocks();
    const core = new ControlCore({
      // initialize を呼ばない
      configPath: defaultConfigPath,
      broadcaster,
      obs,
      http
    });

    // private メソッドだが runtime では呼べる
    await (core as any).switchPage("main");

    const err = sent.find((m) => m.type === "error");
    expect(err?.payload?.code).toBe("NOT_INITIALIZED");
  });

  it("TC-21: switchPage に未定義ページを指定すると INVALID_PAGE エラー", async () => {
    const { broadcaster, obs, http, sent } = createMocks();
    const core = new ControlCore({ configPath: defaultConfigPath, broadcaster, obs, http });

    core.initialize();
    await (core as any).switchPage("unknown-page");

    const err = sent.find((m) => m.type === "error");
    expect(err?.payload?.code).toBe("INVALID_PAGE");
  });

  it("TC-22: updateStatusFromObs 失敗時に OBS_STATUS_FAILED エラー", async () => {
    const { broadcaster, http, sent } = createMocks();

    const obs: ObsController = {
      setScene: vi.fn(),
      toggleStream: vi.fn(),
      toggleRecord: vi.fn(),
      toggleMute: vi.fn(),
      setSourceVisibility: vi.fn(),
      getStatus: vi.fn().mockRejectedValue(new Error("NG"))
    };

    const core = new ControlCore({
      configPath: defaultConfigPath,
      broadcaster,
      obs,
      http
    });

    core.initialize();
    await (core as any).updateStatusFromObs();

    const err = sent.find((m) => m.type === "error");
    expect(err?.payload?.code).toBe("OBS_STATUS_FAILED");
  });

  it("TC-23: http.get displayKey 指定だがレスポンスにキー無しの場合 label 未更新", async () => {
    const { broadcaster, obs, sent } = createMocks();

    const http: HttpClient = {
      get: vi.fn().mockResolvedValue({ other: "XXX" }),
      post: vi.fn()
    };

    const cfg = writeTempConfig("panel-http-display-missing-key.json", {
      version: 1,
      pages: {
        main: {
          name: "Main",
          buttons: [
            {
              x: 0,
              y: 0,
              label: "STATUS",
              action: { type: "http.get", url: "http://example", display: "status" }
            }
          ]
        }
      }
    });

    const core = new ControlCore({ configPath: cfg, broadcaster, obs, http });
    core.initialize();

    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0, source: "dock" }
    });

    // displayKey が見つからない場合の分岐を踏むことが目的
    const pageUpdate = sent.findLast((m) => m.type === "page.update");
    if (pageUpdate) {
      // ラベルが "STATUS" のままであることを確認
      expect(
        pageUpdate.payload.buttons.some(
          (b: any) => b.x === 0 && b.y === 0 && b.label === "STATUS"
        )
      ).toBe(true);
    }
  });

});
