import { describe, it, expect, vi } from "vitest";
import { fileURLToPath } from "node:url";
import {
  ControlCore,
  ClientBroadcaster,
  ObsController,
  HttpClient,
  ButtonClickMessage,
} from "../src/control-core";

/**
 * モック生成:
 * - broadcaster: broadcast の送信内容検証用
 * - obs: 各メソッドを vi.fn でモック
 * - http: HTTP アクション用モック
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
 * panel.json の絶対パス
 * - fileURLToPath を使うことで Windows / POSIX 両対応
 * - ここで余計な文字列結合は禁止（"C:" を足さない）
 */
const configPath = fileURLToPath(
  new URL("../config/panel.json", import.meta.url),
);

describe("ControlCore", () => {
  it("TC-01: 正常ロードで page.update を送信する", () => {
    const { broadcaster, obs, http, sent } = createMocks();
    const core = new ControlCore({ configPath, broadcaster, obs, http });

    core.initialize();

    const update = sent.find((m) => m.type === "page.update");
    expect(update).toBeTruthy();
    expect(update.payload.currentPage).toBe("main");
  });

  it("TC-04: button.click で obs.setScene が呼ばれる", async () => {
    const { broadcaster, obs, http } = createMocks();
    const core = new ControlCore({ configPath, broadcaster, obs, http });

    core.initialize();

    const msg: ButtonClickMessage = {
      type: "button.click",
      payload: {
        page: "main",
        x: 2,
        y: 0,
        source: "dock",
      },
    };

    await core.handleButtonClick(msg);

    expect(obs.setScene).toHaveBeenCalledWith("CameraOnly");
  });
});
