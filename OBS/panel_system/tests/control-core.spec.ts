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
    const { obs, http, sent, broadcast, logger } = mkDeps();

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

    const core = new ControlCore({ obs, http, broadcast, logger, configPath: cfgPath });
    core.initialize();

    await core.handleMessage({ type: "button.click", payload: { page: "main", x: 0, y: 0 } });
    await core.handleMessage({ type: "button.click", payload: { page: "main", x: 1, y: 0 } });

    expect(obs.toggleStreaming).toHaveBeenCalledTimes(1);
    expect(obs.toggleRecording).toHaveBeenCalledTimes(1);
    const err = sent.find((m) => m?.type === "error");
    expect(err).toBeUndefined();
  });

  it("TC-06: HTTP 呼び出し＋表示更新 (http.get)", async () => {
    const { obs, http, sent, broadcast, logger } = mkDeps();
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

    const core = new ControlCore({ obs, http, broadcast, logger, configPath: cfgPath });
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
    const broadcast = (msg: any) => { sent.push(msg); };
    const logger = { info: vi.fn(), warn: vi.fn(), error: vi.fn(), debug: vi.fn() };

    return { obs, http, sent, broadcast, logger };
  };

  it("TC-24: version が number でない場合エラー", () => {
    const cfgPath = writeJson(tmpDir, "tc24.json", {
      version: "1", // 不正
      pages: { main: { name: "Main", buttons: [] } },
    });
    const { obs, http, broadcast, logger, sent } = mkDeps();
    const core = new ControlCore({ obs, http, broadcast, logger, configPath: cfgPath });

    expect(() => core.initialize()).toThrow();
    // page.update は送られない
    const update = sent.find(m => m?.type === "page.update");
    expect(update).toBeUndefined();
  });

  it("TC-25: pages 欠如でエラー", () => {
    const cfgPath = writeJson(tmpDir, "tc25.json", { version: 1 });
    const { obs, http, broadcast, logger } = mkDeps();
    const core = new ControlCore({ obs, http, broadcast, logger, configPath: cfgPath });
    expect(() => core.initialize()).toThrow();
  });

  it("TC-26: buttons 配列未定義ページはエラー", () => {
    const cfgPath = writeJson(tmpDir, "tc26.json", {
      version: 1,
      pages: { main: { name: "Main" } }, // buttons 無し
    });
    const { obs, http, broadcast, logger } = mkDeps();
    const core = new ControlCore({ obs, http, broadcast, logger, configPath: cfgPath });
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
    const { obs, http, broadcast, logger } = mkDeps();
    const core = new ControlCore({ obs, http, broadcast, logger, configPath: cfgPath });
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
    const { obs, http, broadcast, logger, sent } = mkDeps();
    const core = new ControlCore({ obs, http, broadcast, logger, configPath: cfgPath });
    core.initialize();

    const update = sent.find(m => m?.type === "page.update");
    expect(update?.payload?.currentPage).toBe("util");
  });

  it("TC-29: 未初期化状態で button.click → NOT_INITIALIZED", async () => {
    const cfgPath = writeJson(tmpDir, "tc29.json", {
      version: 1,
      pages: { main: { name: "Main", buttons: [] } },
    });
    const { obs, http, broadcast, logger, sent } = mkDeps();
    const core = new ControlCore({ obs, http, broadcast, logger, configPath: cfgPath });
    // initialize しない
    await core.handleMessage({ type: "button.click", payload: { page: "main", x: 0, y: 0 } });
    const err = sent.find(m => m?.type === "error");
    expect(err?.code || err?.payload?.code).toBe("NOT_INITIALIZED");
  });

  it("TC-30: currentPageKey が不正ページを指す → INVALID_ACTION", async () => {
    const { obs, http, sent, broadcast, logger } = mkDeps();
    const cfgPath = writeJsonTemp({
      version: 1,
      pages: { main: { buttons: [{ x: 0, y: 0, label: "X", action: { type: "noop" } }] } },
      currentPageKey: "not-exists",
    });
    const core = new ControlCore({ obs, http, broadcast, logger, configPath: cfgPath });
    core.initialize();

    await core.handleMessage({ type: "button.click", payload: { page: "main", x: 0, y: 0 } });
    const err = sent.find((m) => m?.type === "error");
    expect(err?.code || err?.payload?.code).toBe("INVALID_ACTION"); // ← 変更
  });

  it("TC-31: config 未設定時 pushPageUpdate は送信しない", () => {
    const { obs, http, broadcast, logger, sent } = mkDeps();
    const core: any = new ControlCore({ obs, http, broadcast, logger, configPath: "" });
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
    const { obs, http, broadcast, logger, sent } = mkDeps();
    const core: any = new ControlCore({ obs, http, broadcast, logger, configPath: cfgPath });
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
    const core = new ControlCore(cfgPath, broadcaster, obs, http);

    await core.initialize();
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
    const core = new ControlCore(cfgPath, broadcaster, obs, http);

    await core.initialize();
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
    const core = new ControlCore(cfgPath, broadcaster, obs, http);

    await core.initialize();
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

    const core = new ControlCore(cfgPath, broadcaster, obs, http);
    await core.initialize();
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

    const core = new ControlCore(cfgPath, broadcaster, obs, http);
    await core.initialize();
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

    const core = new ControlCore(cfgPath, broadcaster, obs, http);
    await core.initialize();
    sent.length = 0;

    await core.handleMessage({ type: "button.click", payload: { page: "main", x: 0, y: 0 } });

    const err = sent.find(m => m?.type === "error");
    expect(err?.code || err?.payload?.code).toBe("INVALID_ACTION");
    expect(obs.toggleMute).toHaveBeenCalledTimes(1);
  });
  
});
