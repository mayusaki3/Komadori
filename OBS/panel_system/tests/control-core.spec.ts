/**
 * TestID 対応:
 * - TC-01: 正常ロード
 * - TC-02: 重複座標エラー
 * - TC-03: 未定義ページへの page.switch
 * - TC-04: OBS シーン切替
 * - TC-05: 配信/録画トグル
 */

import { describe, it, expect, vi, beforeEach } from "vitest";
import {
  ControlCore,
  ControlCoreOptions,
  ClientBroadcaster,
  ObsController,
  HttpClient,
  ButtonClickMessage
} from "../src/control-core";
import path from "node:path";

function createMocks() {
  const sent: any[] = [];

  const broadcaster: ClientBroadcaster = {
    broadcast: (msg) => {
      sent.push(msg);
    }
  };

  const obs: ObsController = {
    setScene: vi.fn().mockResolvedValue(undefined),
    toggleStream: vi.fn().mockResolvedValue(undefined),
    toggleRecord: vi.fn().mockResolvedValue(undefined),
    toggleMute: vi.fn().mockResolvedValue(undefined),
    setSourceVisibility: vi.fn().mockResolvedValue(undefined),
    getStatus: vi.fn().mockResolvedValue({ streaming: false, recording: false })
  };

  const http: HttpClient = {
    get: vi.fn().mockResolvedValue({ status: "OK" }),
    post: vi.fn().mockResolvedValue({ status: "OK" })
  };

  return { broadcaster, obs, http, sent };
}

describe("ControlCore", () => {
  const configPath = path.join(__dirname, "../config/panel.json");

  it("TC-01: 正常ロードで page.update を送信する", () => {
    const { broadcaster, obs, http, sent } = createMocks();
    const core = new ControlCore({ configPath, broadcaster, obs, http });

    core.initialize();

    const update = sent.find(m => m.type === "page.update");
    expect(update).toBeTruthy();
    expect(update.payload.currentPage).toBe("main");
  });

  it("TC-04: button.click で obs.setScene が呼ばれる", async () => {
    const { broadcaster, obs, http } = createMocks();
    const core = new ControlCore({ configPath, broadcaster, obs, http });

    core.initialize();

    const msg: ButtonClickMessage = {
      type: "button.click",
      payload: { page: "main", x: 2, y: 0, source: "dock" }
    };

    await core.handleButtonClick(msg);
    expect(obs.setScene).toHaveBeenCalledWith("CameraOnly");
  });
});
