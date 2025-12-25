import { describe, it, expect, vi } from "vitest";
import { createStreamDeckCore, type UpdateKeyFn } from "../src/streamdeck/core";

describe("Stream Deck Core / OBS-PANEL-StreamDeck-TC-001〜006", () => {
  // 共通ヘルパー: 3x5 キー配置で Core を作る
  function createCore(overrides?: Partial<Parameters<typeof createStreamDeckCore>[0]>) {
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
      updateKey,
      sendToCore,
      ...(overrides ?? {}),
    } as any);

    return { core, labels, images, sendToCore, sendButtonClick: sendToCore };
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

  it("OBS-PANEL-StreamDeck-TC-007: applyPageUpdate は type 不一致を無視する", () => {
    const { core } = createCore();

    // 事前に状態を作る
    core.applyPageUpdate({
      type: "page.update",
      payload: { currentPage: "main", buttons: [{ x: 0, y: 0, label: "A" }] },
    });

    // type 不一致 → 無視
    core.applyPageUpdate({ type: "unknown" } as any);

    const s00 = core.getKeyState(0) as any;
    expect(s00.title).toBe("A");
  });

  it("OBS-PANEL-StreamDeck-TC-008: handleCoreMessage は null / type 非文字列を無視する", () => {
    const { core } = createCore();

    core.handleCoreMessage(null as any);
    core.handleCoreMessage({} as any);
    core.handleCoreMessage({ type: 123 } as any);

    // 何も起きないこと（初期状態）
    const s00 = core.getKeyState(0) as any;
    expect(s00.title).toBe("");
    expect(s00.disabled).toBe(true);
  });

  it("OBS-PANEL-StreamDeck-TC-009: handleCoreMessage は未知 type を無視する", () => {
    const { core } = createCore();

    core.handleCoreMessage({ type: "unknown", payload: {} } as any);

    const s00 = core.getKeyState(0) as any;
    expect(s00.title).toBe("");
    expect(s00.disabled).toBe(true);
  });

  it("OBS-PANEL-StreamDeck-TC-010: handleCoreMessage(page.update) でも payload 無しは無視する", () => {
    const { core } = createCore();

    core.handleCoreMessage({ type: "page.update" } as any);

    const s00 = core.getKeyState(0) as any;
    expect(s00.title).toBe("");
    expect(s00.disabled).toBe(true);
  });

  it("OBS-PANEL-StreamDeck-TC-011: page.update payload.buttons が配列でない場合は無視する", () => {
    const { core } = createCore();

    core.applyPageUpdate({
      type: "page.update",
      payload: { currentPage: "main", buttons: null as any },
    } as any);

    const s00 = core.getKeyState(0) as any;
    expect(s00.title).toBe("");
    expect(s00.disabled).toBe(true);
  });

  it("OBS-PANEL-StreamDeck-TC-012: currentPage が空文字の場合は currentPage を更新しない", () => {
    const { core } = createCore();

    core.applyPageUpdate({
      type: "page.update",
      payload: { currentPage: "main", buttons: [] },
    });

    core.applyPageUpdate({
      type: "page.update",
      payload: { currentPage: "", buttons: [] },
    } as any);

    expect(core.getCurrentPage()).toBe("main");
  });

  it("OBS-PANEL-StreamDeck-TC-013: buttons に falsy が混ざっていても無視して落ちない", () => {
    const { core } = createCore();

    core.applyPageUpdate({
      type: "page.update",
      payload: {
        currentPage: "main",
        buttons: [null as any, { x: 0, y: 0, label: "OK" }],
      },
    } as any);

    const s00 = core.getKeyState(0) as any;
    expect(s00.title).toBe("OK");
    expect(s00.disabled).toBe(false);
  });

  it("OBS-PANEL-StreamDeck-TC-014: handleKeyDown は NaN / 範囲外を無視する", () => {
    const { core, sendToCore } = createCore();

    core.handleKeyDown(Number.NaN as any);
    core.handleKeyDown(-1);
    core.handleKeyDown(15); // rows*cols = 15

    expect(sendToCore).toHaveBeenCalledTimes(0);
  });

  it("OBS-PANEL-StreamDeck-TC-015: getKeyState は NaN / 範囲外で undefined", () => {
    const { core } = createCore();

    expect(core.getKeyState(Number.NaN as any)).toBeUndefined();
    expect(core.getKeyState(-1)).toBeUndefined();
    expect(core.getKeyState(15)).toBeUndefined();
  });

  it("OBS-PANEL-StreamDeck-TC-016: updateKey/onUpdateKey 未指定でも page.update を適用できる", () => {
    const sendToCore = vi.fn();
    const core = createStreamDeckCore({
      rows: 3,
      cols: 5,
      sendToCore,
      // updateKey も onUpdateKey も渡さない
    } as any);

    // no-op fallback が効いて落ちないこと
    core.applyPageUpdate({
      type: "page.update",
      payload: { currentPage: "main", buttons: [{ x: 0, y: 0, label: "A" }] },
    });
  });

  it("OBS-PANEL-StreamDeck-TC-017: sendToCore が無い場合は sendButtonClick を使う", () => {
    const sendButtonClick = vi.fn();

    const core = createStreamDeckCore({
      rows: 3,
      cols: 5,
      updateKey: vi.fn(),
      sendToCore: null as any,
      sendButtonClick,
    } as any);

    core.applyPageUpdate({
      type: "page.update",
      payload: { currentPage: "main", buttons: [{ x: 0, y: 0, label: "A" }] },
    });

    core.handleKeyDown(0);

    expect(sendButtonClick).toHaveBeenCalledTimes(1);
    const msg = sendButtonClick.mock.calls[0][0] as any;
    expect(msg.type).toBe("button.click");
    expect(msg.payload).toMatchObject({ page: "main", x: 0, y: 0, source: "streamdeck" });
  });

  it("OBS-PANEL-StreamDeck-TC-018: sendToCore / sendButtonClick が無い場合でもキー押下で落ちない", () => {
    const core = createStreamDeckCore({
      rows: 3,
      cols: 5,
      updateKey: vi.fn(),
      sendToCore: null as any,
      // sendButtonClick も無し
    } as any);

    core.applyPageUpdate({
      type: "page.update",
      payload: { currentPage: "main", buttons: [{ x: 0, y: 0, label: "A" }] },
    });

    // no-op send が効いて落ちないこと
    core.handleKeyDown(0);
  });

  it("OBS-PANEL-StreamDeck-TC-019: 文字列 x/y は Number() で変換され、範囲内なら反映される", () => {
    const { core } = createCore();

    core.applyPageUpdate({
      type: "page.update",
      payload: {
        currentPage: "main",
        buttons: [{ x: "1" as any, y: "0" as any, label: "S" }],
      },
    } as any);

    // (1,0) => index 1
    const s10 = core.getKeyState(1) as any;
    expect(s10.title).toBe("S");
    expect(s10.disabled).toBe(false);
  });

  it("OBS-PANEL-StreamDeck-TC-020: x/y が非finiteになる値は無視される", () => {
    const { core } = createCore();

    core.applyPageUpdate({
      type: "page.update",
      payload: {
        currentPage: "main",
        buttons: [{ x: "NaN" as any, y: 0 as any, label: "NG" }],
      },
    } as any);

    // どこにも反映されない（例: index0）
    const s00 = core.getKeyState(0) as any;
    expect(s00.title).toBe("");
    expect(s00.disabled).toBe(true);
  });

  it("OBS-PANEL-StreamDeck-TC-021: handleKeyDown は Infinity / -Infinity を無視する", () => {
    const { core, sendToCore } = createCore();

    core.handleKeyDown(Infinity as any);
    core.handleKeyDown(-Infinity as any);

    expect(sendToCore).toHaveBeenCalledTimes(0);
  });

  it("OBS-PANEL-StreamDeck-TC-022: getKeyState は Infinity / -Infinity で undefined", () => {
    const { core } = createCore();

    expect(core.getKeyState(Infinity as any)).toBeUndefined();
    expect(core.getKeyState(-Infinity as any)).toBeUndefined();
  });

  it("OBS-PANEL-StreamDeck-TC-023: updateKey/onUpdateKey が無い場合でも page.update で no-op safeUpdateKey が呼ばれて落ちない", () => {
    const core = createStreamDeckCore({
      rows: 3,
      cols: 5,
      // updateKey / onUpdateKey を渡さない（safeUpdateKey の no-op 分岐を確実に作る）
      sendToCore: vi.fn(),
    } as any);

    core.applyPageUpdate({
      type: "page.update",
      payload: {
        currentPage: "main",
        buttons: [{ x: 0, y: 0, label: "X" }],
      },
    });

    // 表示コールバックは no-op でも、内部状態は更新されているはず
    const s00 = core.getKeyState(0) as any;
    expect(s00.title).toBe("X");
    expect(s00.disabled).toBe(false);
  });

  it("OBS-PANEL-StreamDeck-TC-024: sendToCore/sendButtonClick が両方無効でも、ボタン定義キー押下で no-op sendToCore が呼ばれて落ちない", () => {
    // updateKey は渡しておく（このテストでは sendToCore 側の no-op 分岐だけを狙う）
    const updateKey = vi.fn();

    const core = createStreamDeckCore({
      rows: 3,
      cols: 5,
      updateKey,
      // sendToCore を関数にしない（フォールバック分岐を確実に作る）
      sendToCore: 123 as any,
      // sendButtonClick も関数にしない（最終 no-op 分岐へ）
      sendButtonClick: "x" as any,
    } as any);

    // 重要：handleKeyDown が早期 return しないよう、押すキーにボタンを定義する
    // (x=0,y=0) → index 0
    core.applyPageUpdate({
      type: "page.update",
      payload: {
        currentPage: "main",
        buttons: [{ x: 0, y: 0, label: "X" }],
      },
    });

    // ここで no-op sendToCore が“実際に呼ばれる”経路に入る
    expect(() => core.handleKeyDown(0)).not.toThrow();
  });

  it("OBS-PANEL-StreamDeck-TC-025: handleCoreMessage(page.update) で payload が適用される", () => {
    const { core } = createCore();

    core.handleCoreMessage({
      type: "page.update",
      payload: {
        currentPage: "main",
        buttons: [{ x: 0, y: 0, label: "OK" }],
      },
    });

    const s00 = core.getKeyState(0) as any;
    expect(s00.title).toBe("OK");
    expect(s00.disabled).toBe(false);
  });

  it("OBS-PANEL-StreamDeck-TC-026: ボタン未定義キー押下では sendToCore しない", () => {
    const { core, sendToCore } = createCore();

    core.applyPageUpdate({
      type: "page.update",
      payload: {
        currentPage: "main",
        buttons: [{ x: 0, y: 0, label: "ONLY" }], // index 0 のみ有効
      },
    });

    core.handleKeyDown(1); // (1,0) は未定義
    expect(sendToCore).toHaveBeenCalledTimes(0);
  });

  it("OBS-PANEL-StreamDeck-TC-027: label 未指定でも title は空文字に正規化され disabled=false", () => {
    const { core } = createCore();

    core.applyPageUpdate({
      type: "page.update",
      payload: {
        currentPage: "main",
        buttons: [
          { x: 0, y: 0 }, // label 省略 → (def.label ?? "") の '' 側を確実に踏む
        ],
      },
    } as any);

    const s00 = core.getKeyState(0) as any;
    expect(s00.title).toBe("");
    expect(s00.disabled).toBe(false);
  });

  it("OBS-PANEL-StreamDeck-TC-028: currentPage 未設定状態のキー押下は page='main' になる", () => {
    // currentPage が更新されない状態（空文字）を作り、(currentPage ?? 'main') の 'main' 側を踏む
    const sendButtonClick = vi.fn();

    const core = createStreamDeckCore({
      rows: 3,
      cols: 5,
      updateKey: vi.fn(),
      sendToCore: null as any,
      sendButtonClick,
    } as any);

    core.applyPageUpdate({
      type: "page.update",
      payload: {
        currentPage: "", // 空文字 → core 側の条件で currentPage を更新しない
        buttons: [{ x: 0, y: 0, label: "X" }],
      },
    } as any);

    core.handleKeyDown(0);

    expect(sendButtonClick).toHaveBeenCalledTimes(1);
    const msg = sendButtonClick.mock.calls[0][0] as any;
    expect(msg.payload.page).toBe("main");
    expect(msg.payload).toMatchObject({ x: 0, y: 0, source: "streamdeck" });
  });

});
