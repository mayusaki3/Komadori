import { describe, it, expect, vi } from "vitest";
import { createStreamDeckCore } from "../src/streamdeck/core";

describe("Stream Deck Core / OBS-PANEL-StreamDeck-TC-001〜006", () => {
  // 共通ヘルパー: 3x5 キー配置で Core を作る
  function createCore() {
    const labels: string[] = new Array(15).fill("");
    const images: (string | undefined)[] = new Array(15).fill(undefined);

    const updateKey: UpdateKeyFn = (index, title, image) => {
      labels[index] = title;
      images[index] = image;
    };

    const sendToCore = vi.fn();

    const core = createStreamDeckCore({
      rows: 3,
      cols: 5,
      updateKey,     // または onUpdateKey
      sendToCore,
    });

    return { core, labels, images, sendToCore };
  }

  it("OBS-PANEL-StreamDeck-TC-001: page.update によるキー表示更新", () => {
    const { core } = createCore();

    core.applyPageUpdate({
      type: "page.update",
      payload: {
        currentPage: "main",
        buttons: [
          { x: 0, y: 0, label: "LIVE" },
          { x: 1, y: 0, label: "REC" },
        ],
      },
    });

    // index = y * cols + x という一般的なマッピングを想定
    const s00 = core.getKeyState(0) as any; // (0,0)
    const s10 = core.getKeyState(1) as any; // (1,0)

    expect(s00.title).toBe("LIVE");
    expect(s00.disabled).toBe(false);

    expect(s10.title).toBe("REC");
    expect(s10.disabled).toBe(false);
  });

  it("OBS-PANEL-StreamDeck-TC-002: グリッド外の定義は無視される", () => {
    const { core } = createCore();

    core.applyPageUpdate({
      type: "page.update",
      payload: {
        currentPage: "main",
        buttons: [
          // 正常なボタン
          { x: 0, y: 0, label: "OK" },
          // 明らかにグリッド外のボタン
          { x: 99, y: 0, label: "NG_X" },
          { x: 0, y: 99, label: "NG_Y" },
        ],
      },
    });

    // どのキーを見ても "NG_X" / "NG_Y" がタイトルとして現れないことを確認
    for (let idx = 0; idx < 15; idx++) {
      const st = core.getKeyState(idx) as any;
      expect(st.title).not.toBe("NG_X");
      expect(st.title).not.toBe("NG_Y");
    }
  });

  it("OBS-PANEL-StreamDeck-TC-003: キー押下時の button.click 送信内容", () => {
    const { core, sendButtonClick } = createCore();

    core.applyPageUpdate({
      type: "page.update",
      payload: {
        currentPage: "main",
        buttons: [
          { x: 1, y: 1, label: "REC" }, // cols=5 のとき index = 1 + 1*5 = 6
        ],
      },
    });

    core.handleKeyDown(6);

    expect(sendButtonClick).toHaveBeenCalledTimes(1);
    const msg = sendButtonClick.mock.calls[0][0] as any;

    expect(msg.type).toBe("button.click");
    expect(msg.payload).toMatchObject({
      page: "main",
      x: 1,
      y: 1,
      source: "streamdeck",
    });
  });

  it("OBS-PANEL-StreamDeck-TC-004: page.update によるページ切替とキー表示", () => {
    const { core, sendButtonClick } = createCore();

    // main ページ
    core.applyPageUpdate({
      type: "page.update",
      payload: {
        currentPage: "main",
        buttons: [{ x: 0, y: 0, label: "MAIN" }],
      },
    });

    // util ページ
    core.applyPageUpdate({
      type: "page.update",
      payload: {
        currentPage: "util",
        buttons: [{ x: 0, y: 0, label: "UTIL" }],
      },
    });

    // 直近のページ (util) の定義が反映されているはず
    const s00 = core.getKeyState(0) as any;
    expect(s00.title).toBe("UTIL");

    // キー押下時の page も util で送られる
    core.handleKeyDown(0);
    const msg = sendButtonClick.mock.calls[0][0] as any;
    expect(msg.payload.page).toBe("util");
  });

  it("OBS-PANEL-StreamDeck-TC-005: 接続状態ハンドリング（簡易）", () => {
    const { core } = createCore();

    // デフォルトは未接続を想定
    expect(core.getConnectionStatus()).toBe("disconnected");

    core.setConnectionStatus("connected");
    expect(core.getConnectionStatus()).toBe("connected");

    core.setConnectionStatus("disconnected");
    expect(core.getConnectionStatus()).toBe("disconnected");

    core.setConnectionStatus("error");
    expect(core.getConnectionStatus()).toBe("error");
  });

  it("OBS-PANEL-StreamDeck-TC-006: image を setImage 対象キーに反映できる状態にする", () => {
    const { core } = createCore();

    core.applyPageUpdate({
      type: "page.update",
      payload: {
        currentPage: "main",
        buttons: [
          { x: 0, y: 0, label: "LIVE", image: "live.png" },
          { x: 1, y: 0, label: "REC" }, // image なし
        ],
      },
    });

    const s00 = core.getKeyState(0) as any;
    const s10 = core.getKeyState(1) as any;

    expect(s00.title).toBe("LIVE");
    expect(s00.image).toBe("live.png");

    expect(s10.title).toBe("REC");
    expect(s10.image).toBeUndefined();
  });
});
