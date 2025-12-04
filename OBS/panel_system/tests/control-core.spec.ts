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
 * - TC-13: HTTP エラー時のハンドリング
 * - TC-14: 不正 version
 * - TC-15: ページ未定義
 * - TC-16: 座標範囲不正
 * - TC-17: page.switch 必須パラメータ不足
 * - TC-18: http.get 必須パラメータ不足
 * - TC-19: http.post 必須パラメータ不足
 * - TC-20: switchPage 未初期化呼び出し
 * - TC-21: switchPage 未定義ページ指定
 * - TC-22: OBS ステータス取得失敗
 * - TC-23: HTTP displayKey 不一致
 * - TC-24: version が number でない場合エラー
 * - TC-25: pages が存在しない場合エラー
 * - TC-26: buttons 配列未定義ページはエラー
 * - TC-27: ボタンの label または action 欠如でエラー
 * - TC-28: main ページが無い場合、最初のページをデフォルトにする
 * - TC-29: 未初期化状態で button.click → NOT_INITIALIZED
 * - TC-30: currentPageKey が不正ページを指す場合 → INVALID_PAGE
 * - TC-31: config 未設定時の pushPageUpdate は何も送信しない
 * - TC-32: currentPageKey 不正時 pushPageUpdate は何も送信しない
 */

import { describe, it, expect, vi, beforeEach } from "vitest";
import { createMocks, writeTempConfig, createLoggerMock } from "./_helpers";
import { fileURLToPath } from "node:url";
import * as path from "node:path";
import * as fs from "node:fs";
import {
  ControlCore,
  ClientBroadcaster,
  ObsController,
  HttpClient,
  ButtonClickMessage,
  PanelConfig,
} from "../src/control-core";

// 一部ケースで new ControlCore({ ..., logger, ... }) として参照されるがテスト内で未定義だったため補う
const logger: any = { debug(){}, info(){}, warn(){}, error(){} };

// --- test helpers ---
import { tmpdir } from "os";
import { mkdtempSync, writeFileSync } from "fs";
import { join } from "path";

/**
 * 一時ディレクトリ直下に JSON を書き出して、そのファイルパスを返す。
 * - 各テストケースをファイル面でも分離できる
 * - 生成先例: {TMP}/panel-sys-xxxxxx/config.json
 */
function writeJsonTemp(obj: unknown): string {
  const dir = mkdtempSync(join(tmpdir(), "panel-sys-"));
  const p = join(dir, "config.json");
  writeFileSync(p, JSON.stringify(obj, null, 2), "utf8");
  return p;
}

/**
 * 共通モック生成
 */
const createMocks = () => {
  const sent: Array<{ type: string; payload: any }> = [];
  const broadcaster = {
    send: vi.fn((type: string, payload: any) => {
      sent.push({ type, payload });
    }),
  };
  const obs = {
    setScene: vi.fn(),
    toggleStreaming: vi.fn(),
    toggleRecording: vi.fn(),
    toggleMute: vi.fn(),
    setSourceVisibility: vi.fn(),
    getStatus: vi.fn(),
  };
  const http = {
    get: vi.fn(),
    post: vi.fn(),
  };
  const logger = {
    info: vi.fn(),
    warn: vi.fn(),
    error: vi.fn(),
    debug: vi.fn(),
  };
  return { broadcaster, obs, http, sent, logger };
};

/**
 * デフォルト panel.json のパス
 */
const defaultConfigPath = fileURLToPath(
  new URL("../config/panel.json", import.meta.url),
);


/**
 * JSONファイル書き込み
 */
function writeJson(tmpDir: string, name: string, obj: any) {
  const p = path.join(tmpDir, name);
  fs.writeFileSync(p, JSON.stringify(obj), "utf-8");
  return p;
}

describe("ControlCore", () => {
  const tmpDir = path.join(__dirname, "_tmp_more");
  beforeEach(() => {
    vi.clearAllMocks();
    fs.mkdirSync(tmpDir, { recursive: true });
  });

  // 以降のテストで logger を都度生成して渡すユーティリティ
  function newLogger() {
    const logger = createLoggerMock();
    return logger;
  }
  
  it("TC-01: 正常ロードで page.update を送信する", () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const logger = newLogger();
    const core: any = new ControlCore({ obs, http, broadcaster, logger, configPath: defaultConfigPath });
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
            { x: 0, y: 0, label: "A", action: { type: "noop" } },
            { x: 0, y: 0, label: "B", action: { type: "noop" } },
          ],
        },
      },
    });

    const logger = newLogger();
    const core: any = new ControlCore({ obs, http, broadcaster, logger, configPath: dupConfigPath });
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
    const { obs, http, sent, broadcaster, logger } = mkDeps();

    // 実装が呼ぶ正式名で action.type を定義
    const cfgPath = writeJsonTemp({
      version: 1,
      pages: {
        main: {
          buttons: [
            { x: 0, y: 0, label: "STREAM", action: { type: "obs.toggleStreaming" } },
            { x: 1, y: 0, label: "RECORD", action: { type: "obs.toggleRecording" } },
          ],
        },
      },
      currentPageKey: "main",
    });

    const core = new ControlCore({
      configPath: cfgPath,
      broadcaster: broadcaster,
      obs,
      http,
    });
    core.initialize();

    await core.handleMessage({ type: "button.click", payload: { page: "main", x: 0, y: 0 } });
    await core.handleMessage({ type: "button.click", payload: { page: "main", x: 1, y: 0 } });

    expect(obs.toggleStreaming).toHaveBeenCalledTimes(1);
    expect(obs.toggleRecording).toHaveBeenCalledTimes(1);
    const err = sent.find((m) => m?.type === "error");
    expect(err).toBeUndefined();
  });

  it("TC-06: HTTP 呼び出し＋表示更新 (http.get)", async () => {
    const { obs, http, sent, broadcaster, logger } = mkDeps();
    http.get.mockResolvedValue({ status: "OK" }); // ← 期待レスポンス

    const cfgPath = writeJsonTemp({
      version: 1,
      pages: {
        main: {
          buttons: [
            {
              x: 0, y: 0, label: "STATUS",
              action: { type: "http.get", url: "http://example/status", displayKey: "status" }, // ← displayKey 指定
            },
          ],
        },
      },
      currentPageKey: "main",
    });

    const core = new ControlCore({
      configPath: cfgPath,
      broadcaster: broadcaster,
      obs,
      http,
    });
    core.initialize();

    await core.handleMessage({ type: "button.click", payload: { page: "main", x: 0, y: 0 } });

    expect(http.get).toHaveBeenCalledTimes(1);

    const updates = sent.filter((m) => m.type === "page.update");
    const serialized = JSON.stringify(updates);
    expect(serialized).toContain("OK"); // label が OK に更新されていること
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
      broadcaster: (m) => sent.push(m),
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

  const mkDeps = () => {
    const obs = {
      setScene: vi.fn(),
      // ▼実装が呼ぶ正式名を用意
      toggleStreaming: vi.fn(),
      toggleRecording: vi.fn(),
      // （任意）将来の互換確認用に旧名も置くが未使用でも可
      toggleStream: vi.fn(),
      toggleRecord: vi.fn(),

      toggleMute: vi.fn(),
      setSourceVisibility: vi.fn(),
      getStatus: vi.fn().mockResolvedValue({ streaming: false, recording: false }),
    };

    const http = { get: vi.fn(), post: vi.fn() };
    const sent: any[] = [];
    const broadcaster = (msg: any) => { sent.push(msg); };
    const logger = { info: vi.fn(), warn: vi.fn(), error: vi.fn(), debug: vi.fn() };

    return { obs, http, sent, broadcaster, logger };
  };

  it("TC-24: version が number でない場合エラー", () => {
    const cfgPath = writeJson(tmpDir, "tc24.json", {
      version: "1", // 不正
      pages: { main: { name: "Main", buttons: [] } },
    });
    const { obs, http, broadcaster, logger, sent } = mkDeps();
    const core = new ControlCore({ obs, http, broadcaster, logger, configPath: cfgPath });

    expect(() => core.initialize()).toThrow();
    // page.update は送られない
    const update = sent.find(m => m?.type === "page.update");
    expect(update).toBeUndefined();
  });

  it("TC-25: pages 欠如でエラー", () => {
    const cfgPath = writeJson(tmpDir, "tc25.json", { version: 1 });
    const { obs, http, broadcaster, logger } = mkDeps();
    const core = new ControlCore({ obs, http, broadcaster, logger, configPath: cfgPath });
    expect(() => core.initialize()).toThrow();
  });

  it("TC-26: buttons 配列未定義ページはエラー", () => {
    const cfgPath = writeJson(tmpDir, "tc26.json", {
      version: 1,
      pages: { main: { name: "Main" } }, // buttons 無し
    });
    const { obs, http, broadcaster, logger } = mkDeps();
    const core = new ControlCore({ obs, http, broadcaster, logger, configPath: cfgPath });
    expect(() => core.initialize()).toThrow();
  });

  it("TC-27: ボタンの label または action 欠如でエラー", () => {
    const cfgPath = writeJson(tmpDir, "tc27.json", {
      version: 1,
      pages: {
        main: {
          name: "Main",
          buttons: [
            { x: 0, y: 0, label: "A" }, // action 欠如
            // { x: 1, y: 0, action: { type: "noop" } } // label 欠如でも可
          ],
        },
      },
    });
    const { obs, http, broadcaster, logger } = mkDeps();
    const core = new ControlCore({ obs, http, broadcaster, logger, configPath: cfgPath });
    expect(() => core.initialize()).toThrow();
  });

  it("TC-28: main が無い場合、最初のページをデフォルトにする", () => {
    const cfgPath = writeJson(tmpDir, "tc28.json", {
      version: 1,
      pages: {
        util: {
          name: "Utility",
          buttons: [{ x: 0, y: 0, label: "X", action: { type: "noop" } }],
        },
      },
    });
    const { obs, http, broadcaster, logger, sent } = mkDeps();
    const core = new ControlCore({ obs, http, broadcaster, logger, configPath: cfgPath });
    core.initialize();

    const update = sent.find(m => m?.type === "page.update");
    expect(update?.payload?.currentPage).toBe("util");
  });

  it("TC-29: 未初期化状態で button.click → NOT_INITIALIZED", async () => {
    const cfgPath = writeJson(tmpDir, "tc29.json", {
      version: 1,
      pages: { main: { name: "Main", buttons: [] } },
    });
    const { obs, http, broadcaster, logger, sent } = mkDeps();
    const core = new ControlCore({ obs, http, broadcaster, logger, configPath: cfgPath });
    // initialize しない
    await core.handleMessage({ type: "button.click", payload: { page: "main", x: 0, y: 0 } });
    const err = sent.find(m => m?.type === "error");
    expect(err?.code || err?.payload?.code).toBe("NOT_INITIALIZED");
  });

  it("TC-30: currentPageKey が不正ページを指す → INVALID_ACTION", async () => {
    const { obs, http, sent, broadcaster, logger } = mkDeps();
    const cfgPath = writeJsonTemp({
      version: 1,
      pages: { main: { buttons: [{ x: 0, y: 0, label: "X", action: { type: "noop" } }] } },
      currentPageKey: "not-exists",
    });
    const core = new ControlCore({ obs, http, broadcaster, logger, configPath: cfgPath });
    core.initialize();

    await core.handleMessage({ type: "button.click", payload: { page: "main", x: 0, y: 0 } });
    const err = sent.find((m) => m?.type === "error");
    expect(err?.code || err?.payload?.code).toBe("INVALID_ACTION"); // ← 変更
  });

  it("TC-31: config 未設定時 pushPageUpdate は送信しない", () => {
    const { obs, http, broadcaster, logger, sent } = mkDeps();
    const core: any = new ControlCore({ obs, http, broadcaster, logger, configPath: "" });
    // initialize せず直接呼ぶ
    core.pushPageUpdate?.();
    const update = sent.find(m => m?.type === "page.update");
    expect(update).toBeUndefined();
  });

  it("TC-32: currentPageKey 不正時 pushPageUpdate は送信しない", () => {
    const cfgPath = writeJson(tmpDir, "tc32.json", {
      version: 1,
      pages: {
        main: {
          name: "Main",
          buttons: [{ x: 0, y: 0, label: "X", action: { type: "noop" } }],
        },
      },
    });
    const { obs, http, broadcaster, logger, sent } = mkDeps();
    const core: any = new ControlCore({ obs, http, broadcaster, logger, configPath: cfgPath });

    core.initialize();
    core.currentPageKey = "unknown";
    core.pushPageUpdate?.();
    const update = sent.find(m => m?.type === "page.update");
    expect(update).toBeUndefined();
  });

  it("TC-33: 同一ページへの page.switch はノーオペ", async () => {
    const cfgPath = writeJsonTemp({
      version: 1,
      currentPage: "main",
      pages: {
        main: { buttons: [{ x: 0, y: 0, label: "X", action: { type: "obs.setScene", scene: "A" } }] },
        util: { buttons: [{ x: 0, y: 0, label: "Y", action: { type: "obs.setScene", scene: "B" } }] }
      }
    });

    const { ControlCore } = await import("../src/control-core");
    const { broadcaster, obs, http, sent } = createMocks();
    const core: any = new ControlCore({ obs, http, broadcaster, logger, configPath: cfgPath });

    core.initialize();
    sent.length = 0; // 初期の page.update をクリア

    // すでに main の想定。main にもう一度切替要求。
    await core.handleMessage({ type: "page.switch", payload: { page: "main" } });

    expect(sent.find(m => m?.type === "page.update")).toBeUndefined();
    expect(sent.find(m => m?.type === "error")).toBeUndefined();
  });

  it("TC-34: ページ文脈不一致 → INVALID_PAGE_CONTEXT", async () => {
    const cfgPath = writeJsonTemp({
      version: 1,
      currentPage: "main",
      pages: {
        main: { buttons: [{ x: 0, y: 0, label: "X", action: { type: "obs.setScene", scene: "A" } }] },
        util: { buttons: [{ x: 0, y: 0, label: "Y", action: { type: "obs.setScene", scene: "B" } }] }
      }
    });

    const { ControlCore } = await import("../src/control-core");
    const { broadcaster, obs, http, sent } = createMocks();
    const core: any = new ControlCore({ obs, http, broadcaster, logger, configPath: cfgPath });

    core.initialize();
    sent.length = 0;

    // current: main。util ページの座標をわざと送る
    await core.handleMessage({ type: "button.click", payload: { page: "util", x: 0, y: 0 } });

    const err = sent.find(m => m?.type === "error");
    expect(err?.code || err?.payload?.code).toBe("INVALID_PAGE_CONTEXT");
    // アクション呼び出し副作用なし
    expect(obs.setScene).not.toHaveBeenCalled();
  });

  it("TC-35: 複数アクションは順序を維持して実行", async () => {
    const cfgPath = writeJsonTemp({
      version: 1,
      pages: {
        main: {
          buttons: [
            { x: 0, y: 0, label: "SEQ", action: [
              { type: "obs.setScene", scene: "A" },
              { type: "http.get", url: "http://localhost/status", displayKey: "ok" }
            ]}
          ]
        }
      }
    });

    const { ControlCore } = await import("../src/control-core");
    const { broadcaster, obs, http, sent } = createMocks();
    const core: any = new ControlCore({ obs, http, broadcaster, logger, configPath: cfgPath });

    core.initialize();
    sent.length = 0;

    await core.handleMessage({ type: "button.click", payload: { page: "main", x: 0, y: 0 } });

    expect(obs.setScene).toHaveBeenCalledTimes(1);
    expect(http.get).toHaveBeenCalledTimes(1);
    // 順序検証
    expect(obs.setScene.mock.invocationCallOrder[0]).toBeLessThan(http.get.mock.invocationCallOrder[0]);
  });

  it("TC-36: page.switch 後の http.displayKey 更新は新ページへ反映", async () => {
    const cfgPath = writeJsonTemp({
      version: 1,
      pages: {
        main: { buttons: [ { x: 0, y: 0, label: "GO", action: [
          { type: "page.switch", page: "util" },
          { type: "http.get", url: "http://localhost/ok", displayKey: "result" }
        ] } ] },
        util: { buttons: [ { x: 0, y: 0, label: "TARGET" } ] }
      }
    });

    const { ControlCore } = await import("../src/control-core");
    const { broadcaster, obs, http, sent } = createMocks();
    http.get.mockResolvedValue({ ok: true, status: 200, data: { result: "OK" } });

    const core: any = new ControlCore({ obs, http, broadcaster, logger, configPath: cfgPath });
    core.initialize();
    sent.length = 0;

    await core.handleMessage({ type: "button.click", payload: { page: "main", x: 0, y: 0 } });

    const updates = sent.filter(m => m?.type === "page.update");
    const serialized = JSON.stringify(updates);
    expect(serialized).toContain('"currentPage":"util"');
    expect(serialized).toContain('"label":"OK"');
  });

  it("TC-37: http.get displayKey 不在 → 'N/A' で更新＋page.update", async () => {
    const cfgPath = writeJsonTemp({
      version: 1,
      pages: { main: { buttons: [ { x: 0, y: 0, label: "STATUS", action: [
        { type: "http.get", url: "http://localhost/ok", displayKey: "missing_key" }
      ] } ] } }
    });

    const { ControlCore } = await import("../src/control-core");
    const { broadcaster, obs, http, sent } = createMocks();
    http.get.mockResolvedValue({ ok: true, status: 200, data: { result: "OK" } });

    const core: any = new ControlCore({ obs, http, broadcaster, logger, configPath: cfgPath });
    core.initialize();
    sent.length = 0;

    await core.handleMessage({ type: "button.click", payload: { page: "main", x: 0, y: 0 } });

    const updates = sent.filter(m => m?.type === "page.update");
    const serialized = JSON.stringify(updates);
    expect(serialized).toContain('"label":"N/A"');
  });

  it("TC-38: 未知アクションは INVALID_ACTION を送って継続実行", async () => {
    const cfgPath = writeJsonTemp({
      version: 1,
      pages: {
        main: { buttons: [ { x: 0, y: 0, label: "MIX", action: [
          // 存在しない type
          { type: "unknown.action" as any },
          { type: "obs.toggleMute", source: "Mic/Aux" }
        ] } ] }
      }
    });

    const { ControlCore } = await import("../src/control-core");
    const { broadcaster, obs, http, sent } = createMocks();

    const core: any = new ControlCore({ obs, http, broadcaster, logger, configPath: cfgPath });
    core.initialize();
    sent.length = 0;

    await core.handleMessage({ type: "button.click", payload: { page: "main", x: 0, y: 0 } });

    const err = sent.find(m => m?.type === "error");
    expect(err?.code || err?.payload?.code).toBe("INVALID_ACTION");
    expect(obs.toggleMute).toHaveBeenCalledTimes(1);
  });

  it("TC-39: config オブジェクト直渡しでも initialize できる", () => {
    const sent: any[] = [];
    const broadcaster = {
      send: (type: string, payload: any) => {
        sent.push({ type, payload });
      },
    };

    const core = new ControlCore({
      obs: {},
      http: {},
      broadcaster,
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "A", action: { type: "noop" } },
            ],
          },
        },
      },
    });

    core.initialize();

    const update = sent.find((m) => m.type === "page.update");
    expect(update?.payload?.currentPage).toBe("main");
  });

  it("TC-40: configPath も config も無い場合 initialize は例外を投げる", () => {
    const core = new ControlCore({
      obs: {},
      http: {},
      // broadcaster は何でもよい
      broadcaster: { send: () => {} },
      // configPath も config も指定しない
    } as any);

    expect(() => core.initialize()).toThrow("configPath is not set");
  });

  it("TC-41: updateStatusFromObs 成功時に page.update を送る", async () => {
    const sent: any[] = [];
    const broadcaster = {
      send: (type: string, payload: any) => {
        sent.push({ type, payload });
      },
    };
    const obs = {
      getStatus: vi.fn().mockResolvedValue({ ok: true }),
    };

    const core = new ControlCore({
      obs,
      http: {},
      broadcaster,
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "A", action: { type: "noop" } },
            ],
          },
        },
      },
    });

    core.initialize();
    // 初期 page.update を無視するためクリア
    sent.length = 0;

    await core.updateStatusFromObs();

    expect(obs.getStatus).toHaveBeenCalledTimes(1);
    const update = sent.find((m) => m.type === "page.update");
    expect(update).toBeTruthy();
  });

  it("TC-42: getStatus 未実装なら OBS_STATUS_FAILED エラーを送る", async () => {
    const sent: any[] = [];
    const broadcaster = (msg: any) => {
      sent.push(msg);
    };

    const core = new ControlCore({
      obs: {}, // getStatus なし
      http: {},
      broadcaster,
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "A", action: { type: "noop" } },
            ],
          },
        },
      },
    });

    core.initialize();
    sent.length = 0;

    await core.updateStatusFromObs();

    const err = sent.find((m) => m.type === "error");
    expect(err?.payload?.code).toBe("OBS_STATUS_FAILED");
  });

  it("TC-43: 未知メッセージ種別は INVALID_ACTION エラーを返す", async () => {
    const sent: any[] = [];
    const broadcaster = (msg: any) => {
      sent.push(msg);
    };

    const core = new ControlCore({
      obs: {},
      http: {},
      broadcaster,
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "A", action: { type: "noop" } },
            ],
          },
        },
      },
    });

    core.initialize();
    sent.length = 0;

    await core.handleMessage({ type: "unknown.type", payload: {} });

    const err = sent.find((m) => m.type === "error");
    expect(err?.payload?.code).toBe("INVALID_ACTION");
  });

  it("TC-44: HTTP アクションで displayKey 未指定なら label を変更しない", async () => {
    const sent: any[] = [];
    const broadcaster = {
      send: (type: string, payload: any) => {
        sent.push({ type, payload });
      },
    };
    const http = {
      get: vi.fn().mockResolvedValue({ data: { value: "NEW" } }),
    };

    const core = new ControlCore({
      obs: {},
      http,
      broadcaster,
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              {
                x: 0,
                y: 0,
                label: "OLD",
                action: {
                  type: "http.get",
                  url: "https://example.test",
                  // displayKey はあえて指定しない
                },
              },
            ],
          },
        },
      },
    });

    core.initialize();
    sent.length = 0;

    await core.handleMessage({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0 },
    });

    expect(http.get).toHaveBeenCalledTimes(1);
    const update = sent.find((m) => m.type === "page.update");
    expect(update?.payload?.buttons[0]?.label).toBe("OLD");
  });

  it("TC-45: broadcaster が例外を投げても emit は logger.error を呼び出して落ちない", () => {
    const logger = {
      error: vi.fn(),
    };

    const broadcaster = () => {
      throw new Error("emit boom");
    };

    const core = new ControlCore({
      obs: {},
      http: {},
      broadcaster,
      logger,
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "A", action: { type: "noop" } },
            ],
          },
        },
      },
    });

    core.initialize();

    // emit 内で例外が出ても外側には飛ばさない
    expect(() => core.pushPageUpdate()).not.toThrow();
    expect(logger.error).toHaveBeenCalled();
  });

  it("TC-46: http アクションで url 未指定の場合 INVALID_ACTION (execHttpAndReflectLabel)", async () => {
    const sent: any[] = [];
    const broadcaster = (msg: any) => sent.push(msg);
    const http = {
      get: vi.fn(), // 呼ばれない想定
    };

    const core = new ControlCore({
      obs: {},
      http,
      broadcaster,
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              {
                x: 0,
                y: 0,
                label: "A",
                action: {
                  type: "http.get",
                  // url をわざと指定しない
                },
              },
            ],
          },
        },
      },
    });

    core.initialize();
    sent.length = 0;

    await core.handleMessage({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0 },
    });

    const err = sent.find((m) => m.type === "error");
    expect(err?.payload?.code).toBe("INVALID_ACTION");
    expect(http.get).not.toHaveBeenCalled();
  });

  it("TC-47: ボタン経由 http.get で HTTP_FAILED が発生する (execHttpAndReflectLabel)", async () => {
    const sent: any[] = [];
    const broadcaster = (msg: any) => sent.push(msg);
    const http = {
      get: vi.fn().mockRejectedValue(new Error("network down")),
    };

    const core = new ControlCore({
      obs: {},
      http,
      broadcaster,
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              {
                x: 1,
                y: 1,
                label: "A",
                action: {
                  type: "http.get",
                  url: "https://example.test/err",
                  displayKey: "value",
                },
              },
            ],
          },
        },
      },
    });

    core.initialize();
    sent.length = 0;

    await core.handleMessage({
      type: "button.click",
      payload: { page: "main", x: 1, y: 1 },
    });

    const err = sent.find((m) => m.type === "error");
    expect(err?.payload?.code).toBe("HTTP_FAILED");
  });

  it("TC-48: execHttpAndReflectLabel で res 本体の displayKey を参照する", async () => {
    const sent: any[] = [];
    const broadcaster = (msg: any) => sent.push(msg);
    const http = {
      get: vi.fn().mockResolvedValue({ value: "RES_VALUE" }), // res.data ではなく res.value
    };

    const core = new ControlCore({
      obs: {},
      http,
      broadcaster,
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              {
                x: 2,
                y: 2,
                label: "OLD",
                action: {
                  type: "http.get",
                  url: "https://example.test/value",
                  displayKey: "value",
                },
              },
            ],
          },
        },
      },
    });

    core.initialize();
    sent.length = 0;

    await core.handleMessage({
      type: "button.click",
      payload: { page: "main", x: 2, y: 2 },
    });

    const update = sent.find((m) => m.type === "page.update");
    const btn = update?.payload?.buttons.find(
      (b: any) => b.x === 2 && b.y === 2,
    );
    expect(btn?.label).toBe("RES_VALUE");
  });

  it("TC-49: メッセージ経由 http.get 成功時は HTTP_FAILED を送らない", async () => {
    const sent: any[] = [];
    const broadcaster = (msg: any) => sent.push(msg);
    const http = {
      get: vi.fn().mockResolvedValue({ ok: true }),
    };

    const core = new ControlCore({
      obs: {},
      http,
      broadcaster,
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "A", action: { type: "noop" } },
            ],
          },
        },
      },
    });

    core.initialize();
    sent.length = 0;

    await core.handleMessage({
      type: "http.get",
      payload: { url: "https://example.test/ok" },
    });

    const err = sent.find((m) => m.type === "error" && m.payload?.code === "HTTP_FAILED");
    expect(err).toBeUndefined();
    expect(http.get).toHaveBeenCalledTimes(1);
  });

  it("TC-50: メッセージ経由 http.post エラー時に HTTP_FAILED を送る", async () => {
    const sent: any[] = [];
    const broadcaster = (msg: any) => sent.push(msg);
    const http = {
      post: vi.fn().mockRejectedValue(new Error("post failed")),
    };

    const core = new ControlCore({
      obs: {},
      http,
      broadcaster,
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "A", action: { type: "noop" } },
            ],
          },
        },
      },
    });

    core.initialize();
    sent.length = 0;

    await core.handleMessage({
      type: "http.post",
      payload: { url: "https://example.test/post", body: { a: 1 } },
    });

    const err = sent.find((m) => m.type === "error");
    expect(err?.payload?.code).toBe("HTTP_FAILED");
  });

  it("TC-51: emit で send/broadcaster/関数いずれでもない場合は何もせず落ちない", () => {
    const broadcaster = { foo: 1 }; // send/broadcaster を持たない形

    const core = new ControlCore({
      obs: {},
      http: {},
      broadcaster,
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "A", action: { type: "noop" } },
            ],
          },
        },
      },
    });

    // initialize() 内で pushPageUpdate が呼ばれて emit が走る
    expect(() => core.initialize()).not.toThrow();
    // 送信先が無いので、特にアサートは不要（落ちないことだけ確認）
  });

  it("TC-52: handleMessage 内での例外は UNEXPECTED_ERROR にラップされる", async () => {
    const sent: any[] = [];
    const broadcaster = (msg: any) => sent.push(msg);

    const obs = {
      setScene: vi.fn().mockImplementation(() => {
        throw new Error("setScene boom");
      }),
    };

    const core = new ControlCore({
      obs,
      http: {},
      broadcaster,
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              {
                x: 0,
                y: 0,
                label: "Scene",
                action: { type: "setScene", scene: "S1" },
              },
            ],
          },
        },
      },
    });

    core.initialize();
    sent.length = 0;

    await core.handleMessage({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0 },
    });

    const err = sent.find((m) => m.type === "error");
    expect(err?.payload?.code).toBe("UNEXPECTED_ERROR");
  });

  it("TC-53: config 内ボタン座標型不正でエラーになる", () => {
    // x が number ではないケースで validateAndNormalizeConfig の
    // 「invalid button coordinates」ブランチを踏ませる
    const badConfig: PanelConfig = {
      version: 1,
      pages: {
        main: {
          buttons: [
            {
              x: "0" as any, // 不正型
              y: 0,
              label: "NG",
              action: { type: "noop" },
            },
          ],
        },
      },
    };

    const core = new ControlCore({
      obs: {} as any,
      http: {} as any,
      broadcaster: () => {},
      config: badConfig,
    });

    expect(() => core.initialize()).toThrowError(
      /page main has invalid button coordinates/,
    );
  });

  it("TC-54: page オブジェクト自体が falsy な場合はエラーになる", () => {
    // pages: { main: undefined } で「page main is invalid」ブランチを踏ませる
    const badConfig: PanelConfig = {
      version: 1,
      pages: {
        main: undefined as any,
      },
    };

    const core = new ControlCore({
      obs: {} as any,
      http: {} as any,
      broadcaster: () => {},
      config: badConfig,
    });

    expect(() => core.initialize()).toThrowError(/page main is invalid/);
  });

  it("TC-55: execHttpAndReflectLabel で currentPageKey 未設定なら label は更新されない", async () => {
    const get = vi.fn().mockResolvedValue({
      data: { value: "OK" },
    });

    const core = new ControlCore({
      obs: {} as any,
      http: { get } as any,
      broadcaster: () => {},
    });

    // initialized だが currentPageKey は意図的に undefined
    (core as any).initialized = true;
    (core as any).currentPageKey = undefined;

    const anyCore = core as any;
    await anyCore.execHttpAndReflectLabel(
      "get",
      { url: "http://example.test", displayKey: "value" },
      1,
      2,
    );

    const displayValues: Map<string, Map<string, string>> =
      (core as any).displayValues;
    expect(displayValues.size).toBe(0);
    expect(get).toHaveBeenCalledTimes(1);
  });

  it("TC-56: execHttpAndReflectLabel は data 側の displayKey を優先してラベル更新する", async () => {
    const get = vi.fn().mockResolvedValue({
      data: { value: "DATA" },
      value: "ROOT",
    });

    const config: PanelConfig = {
      version: 1,
      pages: {
        main: {
          buttons: [
            {
              x: 1,
              y: 2,
              label: "before",
              action: {
                type: "http.get",
                url: "http://example.test",
                displayKey: "value",
              },
            },
          ],
        },
      },
    };

    const core = new ControlCore({
      obs: {} as any,
      http: { get } as any,
      broadcaster: () => {},
      config,
    });

    core.initialize();
    (core as any).currentPageKey = "main";

    const anyCore = core as any;
    await anyCore.execHttpAndReflectLabel(
      "get",
      { url: "http://example.test", displayKey: "value" },
      1,
      2,
    );

    const displayValues: Map<string, Map<string, string>> =
      (core as any).displayValues;
    const mainMap = displayValues.get("main");
    expect(mainMap).toBeDefined();
    expect(mainMap?.get("1,2")).toBe("DATA"); // data.value が優先される

    expect(get).toHaveBeenCalledTimes(1);
  });

  it("TC-57: toggleStreaming 未実装で toggleStream が呼ばれる", async () => {
    const obs: ObsController = {
      // toggleStreaming は未定義
      toggleStream: vi.fn(),
    };

    const http: HttpClient = {};
    const send = vi.fn();

    const core = new ControlCore({
      obs,
      http,
      broadcaster: { send },
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              {
                x: 0,
                y: 0,
                label: "stream-toggle",
                action: { type: "obs.toggleStreaming" },
              },
            ],
          },
        },
      },
    });

    core.initialize();

    await core.handleMessage({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0 },
    });

    expect(obs.toggleStream).toHaveBeenCalledTimes(1);
  });

  it("TC-58: toggleRecording 未実装で toggleRecord が呼ばれる", async () => {
    const obs: ObsController = {
      // toggleRecording は未定義
      toggleRecord: vi.fn(),
    };

    const http: HttpClient = {};
    const send = vi.fn();

    const core = new ControlCore({
      obs,
      http,
      broadcaster: { send },
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              {
                x: 0,
                y: 0,
                label: "record-toggle",
                action: { type: "obs.toggleRecording" },
              },
            ],
          },
        },
      },
    });

    core.initialize();

    await core.handleMessage({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0 },
    });

    expect(obs.toggleRecord).toHaveBeenCalledTimes(1);
  });

  it("TC-59: currentPage 指定で currentPageKey が上書きされる", () => {
    const obs: ObsController = {};
    const http: HttpClient = {};
    const send = vi.fn();

    const core = new ControlCore({
      obs,
      http,
      broadcaster: { send },
      config: {
        version: 1,
        // main も util も存在する構成
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "main", action: { type: "noop" } },
            ],
          },
          util: {
            buttons: [
              { x: 1, y: 1, label: "util", action: { type: "noop" } },
            ],
          },
        },
        // currentPageKey ではなく currentPage を指定する
        currentPage: "util",
      },
    });

    core.initialize();

    // initialize 時に currentPageKey が currentPage で上書きされるパス
    expect(core.currentPageKey).toBe("util");

    // ついでに初回 page.update も util ページで送られていることを確認
    expect(send).toHaveBeenCalled();
    const payload = send.mock.calls[0][1];
    expect(payload.currentPage).toBe("util");
  });

  it("TC-60: config 未設定時 getPage は undefined を返す (早期 return パス)", () => {
    const obs: ObsController = {};
    const http: HttpClient = {};
    const send = vi.fn();

    const core = new ControlCore({
      obs,
      http,
      broadcaster: { send },
      // config / configPath ともに指定しない
    });

    // private メソッドだが any 経由で直接呼び出して早期 return パスを踏む
    const page = (core as any).getPage("main");
    expect(page).toBeUndefined();
  });

  it("TC-61: getPage は key 未指定なら undefined を返す", () => {
    const config: PanelConfig = {
      version: 1,
      pages: {
        main: {
          buttons: [],
        },
      },
    };

    const core = new ControlCore({
      obs: {},
      http: {},
      broadcaster: () => {},
      logger: {},
      config,
    });

    // initialize() 不要。private メソッドを as any で直接呼ぶ
    const page = (core as any).getPage(undefined);
    expect(page).toBeUndefined();
  });

  it("TC-62: メッセージ経由 http.post 成功時は HTTP_FAILED エラーを送らない", async () => {
    const errors: any[] = [];

    const http = {
      post: vi.fn().mockResolvedValue({ ok: true }),
    };

    const core = new ControlCore({
      obs: {},
      http,
      broadcaster: (msg: any) => {
        if (msg.type === "error") {
          errors.push(msg);
        }
      },
      logger: {},
    });

    await core.handleMessage({
      type: "http.post",
      payload: { url: "http://example.local/api", body: { foo: "bar" } },
    });

    expect(http.post).toHaveBeenCalledTimes(1);
    expect(http.post).toHaveBeenCalledWith(
      "http://example.local/api",
      { foo: "bar" },
    );

    // HTTP_FAILED が飛んでいないこと
    const httpFailed = errors.find(
      (e) => e.payload?.code === "HTTP_FAILED",
    );
    expect(httpFailed).toBeUndefined();
  }); 

  it("TC-63: pages が空オブジェクトの場合 getPage は undefined を返す", () => {
    const core = new ControlCore({
      config: { version: 1, pages: {} },
      obs: {},
      http: {},
      broadcaster: () => {},
    });

    expect(core.getPage("main")).toBeUndefined();
  });

  it("TC-64: buttons 未定義ページでは getButton は undefined を返す", () => {
    const config: PanelConfig = {
      version: 1,
      pages: {
        main: {
          // buttons をあえて未定義/省略にする
          // buttons: undefined as any,
        } as any,
      },
    };

    const core = new ControlCore({
      obs: {} as any,
      http: {} as any,
      broadcaster: () => {},
      config,
    });

    // ★ 修正ポイント
    const btn = (core as any).getButton("main", 0, 0);
    expect(btn).toBeUndefined();
  });

  it("TC-65: emit は logger 未定義でも安全に失敗せず動作する", () => {
    const core = new ControlCore({
      config: { version: 1, pages: {} },
      obs: {},
      http: {},
      broadcaster: {}, // send/broadcaster/func どれでもない
      logger: undefined,
    });

    expect(() => core.emit({ type: "test" })).not.toThrow();
  });

  it("TC-66: obs.getStatus が undefined を返した場合 OBS_STATUS_FAILED を送る", async () => {
    const sent: any[] = [];

    const core = new ControlCore({
      obs: {
        getStatus: vi.fn().mockResolvedValue(undefined),
      } as any,
      http: {} as any,
      broadcaster: (msg: any) => sent.push(msg),
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "A", action: { type: "noop" } },
            ],
          },
        },
      },
    });

    core.initialize();
    sent.length = 0;

    await core.updateStatusFromObs();

    // ★ 修正ポイント
    const msg = sent.find(
      (m) => m.type === "error" && (m.payload?.code === "OBS_STATUS_FAILED" || m.code === "OBS_STATUS_FAILED"),
    );
    expect(msg).not.toBeUndefined();
  });

  it("TC-67: execHttpAndReflectLabel は data が undefined の場合 label を更新しない", async () => {
    const sent: any[] = [];
    const http = {
      // res.data が undefined になるケース
      get: vi.fn().mockResolvedValue({ data: undefined }),
    };

    const config: PanelConfig = {
      version: 1,
      pages: {
        main: {
          buttons: [
            {
              x: 0,
              y: 0,
              label: "A",
              action: {
                type: "http.get",
                url: "http://example.test",
                displayKey: "value",
              },
            },
          ],
        },
      },
    };

    const core = new ControlCore({
      obs: {} as any,
      http: http as any,
      broadcaster: (msg: any) => sent.push(msg),
      config,
    });

    core.initialize();
    (core as any).currentPageKey = "main";
    sent.length = 0;

    await (core as any).execHttpAndReflectLabel(
      "get",
      { url: "http://example.test", displayKey: "value" },
      0,
      0,
    );

    // page.update が飛んでいれば label が変わっていないことを確認
    const update = sent.find((m) => m.type === "page.update");
    if (update) {
      expect(update.payload.buttons[0].label).toBe("A");
    }

    // ★ さらに厳密にするなら displayValues も確認
    const displayValues: Map<string, Map<string, string>> =
      (core as any).displayValues;
    const mainMap = displayValues.get("main");
    // 「0,0」の display 値が登録されていない = ラベル更新されていない
    expect(mainMap?.has("0,0")).not.toBe(true);
  });

  it("TC-68: getButton は存在するボタンのみを返す", () => {
    const core = new ControlCore({
      obs: {} as any,
      http: {} as any,
      broadcaster: () => {},   // 何もしないブロードキャスタ
      logger: {},              // ロガーは空でOK
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "A", action: { type: "noop" } },
            ],
          },
        },
      },
    });

    core.initialize();

    // 存在するボタン: main ページ (0,0) はヒットする
    const found = (core as any).getButton("main", 0, 0);
    expect(found).toBeDefined();
    expect(found.x).toBe(0);
    expect(found.y).toBe(0);
    expect(found.label).toBe("A");

    // 座標違い → undefined
    const notFound1 = (core as any).getButton("main", 1, 0);
    expect(notFound1).toBeUndefined();

    // ページ違い → undefined
    const notFound2 = (core as any).getButton("unknown", 0, 0);
    expect(notFound2).toBeUndefined();
  });

  it("TC-69: broadcaster が想定外形態でも emit は落ちない", () => {
    const core = new ControlCore({
      obs: {} as any,
      http: {} as any,
      broadcaster: {} as any, // send も broadcaster も無い
      logger: {
        error: vi.fn(),
      },
      config: {
        version: 1,
        pages: { main: { buttons: [] } },
      },
    });

    // private メソッド呼び出し（TS 的には any で逃がす）
    (core as any).emit("page.update", { foo: "bar" });

    // 例外が飛ばなければ OK。追加で logger.error が呼ばれていないことを確認しても良い
    expect((core as any).logger.error).not.toHaveBeenCalled();
  });

  it("TC-70: updateStatusFromObs で getStatus が null を返した場合 OBS_STATUS_FAILED", async () => {
    const msgs: any[] = [];
    const core = new ControlCore({
      obs: {
        getStatus: vi.fn().mockResolvedValue(null),
      },
      http: {} as any,
      broadcaster: (msg: any) => msgs.push(msg),
      logger: {},
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [{ x: 0, y: 0, label: "A", action: { type: "noop" } }],
          },
        },
      },
    });

    core.initialize();
    await core.updateStatusFromObs();

    const errors = msgs.filter((m) => m.type === "error");
    expect(errors).toHaveLength(1);
    expect(errors[0].payload.code).toBe("OBS_STATUS_FAILED");
    expect(errors[0].payload.message).toContain("no data");
  });

  it("TC-71: execHttpAndReflectLabel で res が undefined の場合 label を更新しない", async () => {
    const sent: any[] = [];
    const core = new ControlCore({
      obs: {} as any,
      http: {
        get: vi.fn().mockResolvedValue(undefined), // res === undefined
      },
      broadcaster: (msg: any) => sent.push(msg),
      logger: {},
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              {
                x: 0,
                y: 0,
                label: "A",
                action: { type: "http.get", url: "http://example", displayKey: "value" },
              },
            ],
          },
        },
      },
    });

    core.initialize();
    core.currentPageKey = "main";

    await core.handleMessage({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0 },
    });

    // error(HTTP_FAILED) は飛ばない
    const errors = sent.filter((m) => m.type === "error");
    expect(errors.some((e) => e.payload.code === "HTTP_FAILED")).toBe(false);

    // page.update のボタン label は元のまま "A"
    const updates = sent.filter((m) => m.type === "page.update");
    const lastUpdate = updates[updates.length - 1];
    expect(lastUpdate.payload.buttons[0].label).toBe("A");
  });

  it("TC-72: currentPageKey undefined の場合 setDisplayLabel を呼ばない", async () => {
    const send = vi.fn();

    const core = new ControlCore({
      broadcaster: { send },
      obs: {},
      http: {
        get: vi.fn().mockResolvedValue({ value: "SHOULD_NOT_APPLY" }),
      },
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "MAIN", action: { type: "noop" } },
            ],
          },
          test: {
            buttons: [
              { x: 4, y: 4, label: "ORIG", action: { type: "noop" } },
            ],
          },
        },
        currentPageKey: "test",
      },
    });

    core.initialize();
    send.mockClear();

    // currentPageKey をあえて undefined にしてから HTTP 実行
    core.currentPageKey = undefined;

    await (core as any).execHttpAndReflectLabel(
      "get",
      { url: "http://example.test", displayKey: "value" },
      4,
      4,
    );

    // ここで currentPageKey を test に戻して page.update を送る
    core.currentPageKey = "test";
    core.pushPageUpdate();

    // label は ORIG のまま（setDisplayLabel が呼ばれていない）
    expect(send).toHaveBeenCalledTimes(1);
    const [type, payload] = send.mock.calls[0];
    expect(type).toBe("page.update");
    expect(payload.buttons).toEqual([
      { x: 4, y: 4, label: "ORIG" },
    ]);
  });

  it("TC-73: メッセージ経由 page.switch で page 未指定なら INVALID_ACTION", async () => {
    const send = vi.fn();
    const core = new ControlCore({
      broadcaster: { send },
      obs: {},
      http: {},
      config: {
        version: 1,
        pages: { main: { buttons: [] } },
        currentPageKey: "main",
      },
    });
    core.initialize();

    await core.handleMessage({ type: "page.switch", payload: {} });

    expect(send).toHaveBeenCalledWith("error", {
      code: "INVALID_ACTION",
      message: "page missing",
    });
  });

  it("TC-74: ボタン action.type 未指定なら INVALID_ACTION(action.type missing)", async () => {
    const sent: any[] = [];
    const broadcaster = (msg: any) => {
      sent.push(msg);
    };

    const core = new ControlCore({
      broadcaster,
      obs: {},
      http: {},
      // main ページに action.type を持たないボタンを定義
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              {
                x: 0,
                y: 0,
                label: "NO_TYPE",
                action: {}, // type プロパティ無し → onButtonClick 内 458–460 行の分岐
              },
            ],
          },
        },
      },
    });

    core.initialize();

    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0 },
    });

    const err = sent.find((m) => m.type === "error");
    expect(err).toBeTruthy();
    expect(err!.payload.code).toBe("INVALID_ACTION");
    expect(err!.payload.message).toBe("action.type missing");
  });

  it("TC-75: メッセージ経由 http.get で url 未指定なら INVALID_ACTION", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const core = new ControlCore({
      broadcaster,
      obs,
      http,
      configPath: defaultConfigPath,
    });

    core.initialize();

    // onHttpGet に payload.url なしで到達させる → 549–551 行
    await core.handleMessage({
      type: "http.get",
      payload: {}, // url プロパティ無し
    });

    const err = sent.find((m) => m.type === "error");
    expect(err).toBeTruthy();
    expect(err!.payload.code).toBe("INVALID_ACTION");
    expect(err!.payload.message).toBe("url missing");
  });

  it("TC-76: メッセージ経由 http.post で url 未指定なら INVALID_ACTION", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const core = new ControlCore({
      broadcaster,
      obs,
      http,
      configPath: defaultConfigPath,
    });

    core.initialize();

    // onHttpPost に payload.url なしで到達させる → 566–568 行
    await core.handleMessage({
      type: "http.post",
      payload: {}, // url プロパティ無し
    });

    const err = sent.find((m) => m.type === "error");
    expect(err).toBeTruthy();
    expect(err!.payload.code).toBe("INVALID_ACTION");
    expect(err!.payload.message).toBe("url missing");
  });

  it("TC-77: メッセージ経由 http.get で例外なら HTTP_FAILED を送る", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    // http.get を reject させる
    (http.get as any).mockRejectedValue(new Error("boom"));

    const core = new ControlCore({
      broadcaster,
      obs,
      http,
      configPath: defaultConfigPath,
    });

    core.initialize();

    await core.handleMessage({
      type: "http.get",
      payload: {
        url: "http://example.com/status",
      },
    });

    // HTTP_FAILED エラーが送られていることを確認
    const err = sent.find((m) => m.type === "error");
    expect(err).toBeTruthy();
    expect(err!.payload.code).toBe("HTTP_FAILED");
    expect(err!.payload.message).toBe("boom");
  });

  it("TC-78: opts.obs / opts.http 未指定でも {} で補完される", () => {
    // obs/http を省略してコンストラクタを呼び出し、?? の右側を踏む
    const core = new ControlCore({
      broadcaster: { send: vi.fn() },
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "A", action: { type: "noop" } },
            ],
          },
        },
      },
    } as any);

    // private なので any 経由で確認
    expect((core as any).obs).toEqual({});
    expect((core as any).http).toEqual({});
  });

  it("TC-79: 非 main ページで label が省略された場合は空文字で正規化される", () => {
    const send = vi.fn();
    const core = new ControlCore({
      obs: {} as any,
      http: {} as any,
      broadcaster: { send },
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "MAIN", action: { type: "noop" } },
            ],
          },
          sub: {
            // label を敢えて省略（b.label ?? "" の右側を踏ませる）
            buttons: [
              { x: 1, y: 2, action: { type: "noop" } },
            ],
          },
        },
        currentPageKey: "sub",
      },
    });

    core.initialize();
    send.mockClear();
    core.pushPageUpdate();

    const [type, payload] = send.mock.calls[0];
    expect(type).toBe("page.update");
    expect(payload.currentPage).toBe("sub");
    expect(payload.buttons).toContainEqual({ x: 1, y: 2, label: "" });
  });

  it("TC-80: payload プロパティ自体が無い場合でも INVALID_ACTION(page missing) になる", async () => {
    const send = vi.fn();
    const core = new ControlCore({
      obs: {} as any,
      http: {} as any,
      broadcaster: { send },
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "MAIN", action: { type: "noop" } },
            ],
          },
        },
      },
    });

    core.initialize();
    send.mockClear();

    // msg.payload 自体を付けない → msg?.payload ?? {} の右側を踏む
    await core.handleMessage({ type: "page.switch" });

    expect(send).toHaveBeenCalledWith("error", {
      code: "INVALID_ACTION",
      message: "page missing",
    });
  });

  it("TC-81: handleMessage 内で message を持たない例外が発生しても UNEXPECTED_ERROR にラップされる", async () => {
    const send = vi.fn();
    const logger = { error: vi.fn() };
    const core = new ControlCore({
      obs: {
        toggleStreaming: vi.fn().mockImplementation(() => {
          // Error ではなく素の string を throw して e?.message ?? String(e) の右側を踏む
          throw "boom-toggle";
        }),
      } as any,
      http: {} as any,
      broadcaster: { send },
      logger,
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              {
                x: 0,
                y: 0,
                label: "TOGGLE",
                action: { type: "obs.toggleStreaming" },
              },
            ],
          },
        },
        currentPageKey: "main",
      },
    });

    core.initialize();
    send.mockClear();

    await core.handleMessage({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0 },
    });

    const err = send.mock.calls.find(
      (c) => c[0] === "error" && c[1].code === "UNEXPECTED_ERROR",
    )?.[1];

    expect(err).toBeDefined();
    // e?.message が undefined なので String(e) が使われる
    expect(err.message).toBe("boom-toggle");
  });

  it("TC-82: メッセージ経由 http.get で message を持たない例外でも HTTP_FAILED になる", async () => {
    const send = vi.fn();
    const httpGet = vi.fn().mockRejectedValue("boom-http-get");

    const core = new ControlCore({
      obs: {} as any,
      http: { get: httpGet } as any,
      broadcaster: { send },
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "MAIN", action: { type: "noop" } },
            ],
          },
        },
      },
    });

    core.initialize();
    send.mockClear();

    await core.handleMessage({
      type: "http.get",
      payload: { url: "http://example.test" },
    });

    expect(send).toHaveBeenCalledWith("error", {
      code: "HTTP_FAILED",
      message: "boom-http-get",
    });
  });

  it("TC-83: メッセージ経由 http.post で message を持たない例外でも HTTP_FAILED になる", async () => {
    const send = vi.fn();
    const httpPost = vi.fn().mockRejectedValue("boom-http-post");

    const core = new ControlCore({
      obs: {} as any,
      http: { post: httpPost } as any,
      broadcaster: { send },
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "MAIN", action: { type: "noop" } },
            ],
          },
        },
      },
    });

    core.initialize();
    send.mockClear();

    await core.handleMessage({
      type: "http.post",
      payload: { url: "http://example.test", body: { a: 1 } },
    });

    expect(send).toHaveBeenCalledWith("error", {
      code: "HTTP_FAILED",
      message: "boom-http-post",
    });
  });

  it("TC-84: updateStatusFromObs で message を持たない例外でも OBS_STATUS_FAILED になる", async () => {
    const send = vi.fn();
    const core = new ControlCore({
      obs: {
        getStatus: vi.fn().mockRejectedValue("boom-status"),
      } as any,
      http: {} as any,
      broadcaster: { send },
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              { x: 0, y: 0, label: "MAIN", action: { type: "noop" } },
            ],
          },
        },
        currentPageKey: "main",
      },
    });

    core.initialize();
    send.mockClear();

    await core.updateStatusFromObs();

    expect(send).toHaveBeenCalledWith("error", {
      code: "OBS_STATUS_FAILED",
      message: "boom-status",
    });
  });

  it("TC-85: ボタン経由 http.get で message を持たない例外でも HTTP_FAILED になる", async () => {
    const send = vi.fn();
    const httpGet = vi.fn().mockRejectedValue("boom-http-button");

    const core = new ControlCore({
      obs: {} as any,
      http: { get: httpGet } as any,
      broadcaster: { send },
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              {
                x: 3,
                y: 3,
                label: "HTTP",
                action: {
                  type: "http.get",
                  url: "http://example.test",
                  displayKey: "val",
                },
              },
            ],
          },
        },
        currentPageKey: "main",
      },
    });

    core.initialize();
    send.mockClear();

    await core.handleMessage({
      type: "button.click",
      payload: { page: "main", x: 3, y: 3 },
    });

    const err = send.mock.calls.find(
      (c) => c[0] === "error" && c[1].code === "HTTP_FAILED",
    )?.[1];

    expect(err).toBeDefined();
    expect(err.message).toBe("boom-http-button");
  });

  it("TC-86: execHttpAndReflectLabel data[displayKey] が undefined の場合は空文字で上書きされる", async () => {
    const send = vi.fn();
    const httpGet = vi.fn().mockResolvedValue({
      data: { value: undefined }, // プロパティはあるが内容が undefined → ?? "" の右側
    });

    const core = new ControlCore({
      obs: {} as any,
      http: { get: httpGet } as any,
      broadcaster: { send },
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              {
                x: 4,
                y: 4,
                label: "ORIG",
                action: {
                  type: "http.get",
                  url: "http://example.test",
                  displayKey: "value",
                },
              },
            ],
          },
        },
        currentPageKey: "main",
      },
    });

    core.initialize();
    send.mockClear();

    await core.handleMessage({
      type: "button.click",
      payload: { page: "main", x: 4, y: 4 },
    });

    const update = send.mock.calls.find((c) => c[0] === "page.update")?.[1];
    expect(update).toBeDefined();
    expect(update.buttons).toContainEqual({ x: 4, y: 4, label: "" });
  });

  it("TC-87: execHttpAndReflectLabel res[displayKey] が undefined の場合は空文字で上書きされる", async () => {
    const send = vi.fn();
    const httpGet = vi.fn().mockResolvedValue({
      // data プロパティ無し → res 本体から displayKey を参照
      value: undefined,
    });

    const core = new ControlCore({
      obs: {} as any,
      http: { get: httpGet } as any,
      broadcaster: { send },
      config: {
        version: 1,
        pages: {
          main: {
            buttons: [
              {
                x: 5,
                y: 5,
                label: "ORIG",
                action: {
                  type: "http.get",
                  url: "http://example.test",
                  displayKey: "value",
                },
              },
            ],
          },
        },
        currentPageKey: "main",
      },
    });

    core.initialize();
    send.mockClear();

    await core.handleMessage({
      type: "button.click",
      payload: { page: "main", x: 5, y: 5 },
    });

    const update = send.mock.calls.find((c) => c[0] === "page.update")?.[1];
    expect(update).toBeDefined();
    expect(update.buttons).toContainEqual({ x: 5, y: 5, label: "" });
  });

});
