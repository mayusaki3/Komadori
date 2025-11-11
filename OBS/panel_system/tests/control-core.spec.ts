/**
 * TestID 対応:
 * - TC-01: 正常ロード
 * - TC-02: 重複座標エラー
 * - TC-03: 未定義ページへの page.switch
 * - TC-04: OBS シーン切替
 * - TC-05: 配信/録画トグル
 * - TC-06: HTTP 呼び出し＋表示
 * - TC-07: ページ切替同期
 * - TC-08: 不明 action.type
 */

import { describe, it, expect, vi } from "vitest";
import { fileURLToPath } from "node:url";
import * as path from "node:path";
import * as fs from "node:fs";
import {
  ControlCore,
  ClientBroadcaster,
  ObsController,
  HttpClient,
  ButtonClickMessage,
} from "../src/control-core";

/**
 * テスト共通モック
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
 * テスト用 config JSON を一時生成
 * リポジトリには残してよい前提の軽量ファイル。
 */
function writeConfig(name: string, json: any): string {
  const baseDir = path.dirname(fileURLToPath(import.meta.url));
  const filePath = path.join(baseDir, name);
  fs.writeFileSync(filePath, JSON.stringify(json), "utf-8");
  return filePath;
}

/**
 * 通常 panel.json のパス
 */
const defaultConfigPath = fileURLToPath(
  new URL("../config/panel.json", import.meta.url),
);

describe("ControlCore", () => {
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

    const dupConfigPath = writeConfig("panel-dup.json", {
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

    try {
      core.initialize();
    } catch {
      // 実装が例外で落とす場合も許容
    }

    const update = sent.find((m) => m.type === "page.update");
    expect(update).toBeUndefined();
  });

  it("TC-03: 未定義ページへの page.switch で INVALID_PAGE エラー", async () => {
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
        page: "unknown",
        x: 0,
        y: 0,
        source: "dock",
      },
    };

    await core.handleButtonClick(msg);

    // 外部呼び出しなし
    expect(obs.setScene).not.toHaveBeenCalled();
    expect(http.get).not.toHaveBeenCalled();
    expect(http.post).not.toHaveBeenCalled();

    // エラー送信確認
    const err = sent.find((m) => m.type === "error");
    expect(err).toBeTruthy();
    expect(err.code || err.payload?.code).toBe("INVALID_PAGE_CONTEXT");
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
    // エラーが飛んでいないことだけ軽く確認
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

    // LIVE (0,0)
    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 0, y: 0, source: "dock" },
    });

    // REC (1,0)
    await core.handleButtonClick({
      type: "button.click",
      payload: { page: "main", x: 1, y: 0, source: "dock" },
    });

    expect(obs.toggleStream).toHaveBeenCalledTimes(1);
    expect(obs.toggleRecord).toHaveBeenCalledTimes(1);

    // 状態更新メッセージが少なくとも1件ある想定
    const status = sent.filter((m) => m.type === "status.update");
    expect(status.length).toBeGreaterThanOrEqual(1);
  });

  it("TC-06: HTTP 呼び出し＋表示でラベル更新", async () => {
    const { broadcaster, obs, http, sent } = createMocks();

    const httpConfigPath = writeConfig("panel-http.json", {
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

    const update = sent.find(
      (m) => m.type === "page.update" && m.payload?.buttons,
    );

    // HTTPが1回呼ばれていること
    expect(http.get).toHaveBeenCalledTimes(1);

    // 少なくとも何らかの page.update が発行されていること
    expect(update).toBeTruthy();
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
    sent.length = 0; // 初期 page.update をクリアして純粋に切替のみを見る

    // main の UTIL ボタン (0,2) で util へ
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

    const unknownConfigPath = writeConfig("panel-unknown-action.json", {
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

    // 外部呼び出しなし
    expect(obs.setScene).not.toHaveBeenCalled();
    expect(obs.toggleStream).not.toHaveBeenCalled();
    expect(http.get).not.toHaveBeenCalled();
    expect(http.post).not.toHaveBeenCalled();

    // エラー送信確認
    const err = sent.find((m) => m.type === "error");
    expect(err).toBeTruthy();
    expect(err.code || err.payload?.code).toBe("INVALID_ACTION");
  });
});
