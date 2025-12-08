/* @vitest-environment jsdom */

import { describe, it, expect, beforeEach, afterEach, vi } from "vitest";
import { initDockUi } from "../src/dock/panel";

// Dock UI テスト用の簡易 FakeWebSocket 実装
class FakeWebSocket {
  static instances: FakeWebSocket[] = [];

  url: string;
  // 送信されたメッセージ
  sent: string[] = [];

  // イベントハンドラ
  onopen: (() => void) | null = null;
  onclose: (() => void) | null = null;
  onerror: ((ev: any) => void) | null = null;
  onmessage: ((ev: { data: any }) => void) | null = null;

  constructor(url: string) {
    this.url = url;
    FakeWebSocket.instances.push(this);
  }

  send(data: string) {
    this.sent.push(data);
  }

  close() {
    // 必要なら onclose を呼び出す
    if (this.onclose) {
      this.onclose();
    }
  }

  // テスト用ヘルパー: メッセージを受信させる
  receive(data: any) {
    if (this.onmessage) {
      this.onmessage({ data });
    }
  }

  // テスト用ヘルパー: すべてリセット
  static reset() {
    FakeWebSocket.instances.length = 0;
  }
}

describe("Dock UI (OBS-PANEL-Dock-TC-001〜005 / TC-Impl_xxx)", () => {
  let dispose: () => void;

  beforeEach(() => {
    // FakeWebSocket の状態リセット
    FakeWebSocket.reset();

    // jsdom 上の DOM を初期化
    document.body.innerHTML = `
      <div id="status"></div>
      <div id="grid"></div>
    `;

    // Dock UI 初期化
    const handle = initDockUi({
      wsUrl: "ws://example.com/dock",
      document,
      WebSocketImpl: FakeWebSocket as any,
      rows: 3,
      cols: 5,
    });

    dispose = handle.dispose;
  });

  afterEach(() => {
    if (dispose) {
      dispose();
    }
  });

  function getSocket(): FakeWebSocket {
    if (FakeWebSocket.instances.length === 0) {
      throw new Error("FakeWebSocket インスタンスが生成されていません");
    }
    return FakeWebSocket.instances[0];
  }

  it("OBS-PANEL-Dock-TC-001: page.update → DOM ボタン反映", () => {
    const ws = getSocket();

    // page.update メッセージを受信させる
    ws.receive(
      JSON.stringify({
        type: "page.update",
        payload: {
          currentPage: "main",
          buttons: [
            { x: 0, y: 0, label: "LIVE" },
            { x: 1, y: 0, label: "REC" },
          ],
        },
      })
    );

    const btn00 = document.querySelector(
      'button[data-x="0"][data-y="0"]'
    ) as HTMLButtonElement | null;
    const btn10 = document.querySelector(
      'button[data-x="1"][data-y="0"]'
    ) as HTMLButtonElement | null;

    expect(btn00).not.toBeNull();
    expect(btn10).not.toBeNull();

    expect(btn00!.textContent).toBe("LIVE");
    expect(btn00!.classList.contains("disabled")).toBe(false);

    expect(btn10!.textContent).toBe("REC");
    expect(btn10!.classList.contains("disabled")).toBe(false);
  });

  it("OBS-PANEL-Dock-TC-002: 未定義セルは disabled かつクリックしても送信しない", () => {
    const ws = getSocket();

    // (0,0) のみ定義された page.update
    ws.receive(
      JSON.stringify({
        type: "page.update",
        payload: {
          currentPage: "main",
          buttons: [{ x: 0, y: 0, label: "LIVE" }],
        },
      })
    );

    const definedBtn = document.querySelector(
      'button[data-x="0"][data-y="0"]'
    ) as HTMLButtonElement;
    const undefinedBtn = document.querySelector(
      'button[data-x="1"][data-y="0"]'
    ) as HTMLButtonElement;

    // 定義済みボタンは enabled
    expect(definedBtn.classList.contains("disabled")).toBe(false);

    // 未定義セルは disabled
    expect(undefinedBtn.classList.contains("disabled")).toBe(true);
    expect(undefinedBtn.textContent).toBe("");

    // 未定義セルをクリックしても send は呼ばれない
    const sendSpy = vi.spyOn(ws, "send");
    undefinedBtn.click();
    expect(sendSpy).not.toHaveBeenCalled();
  });

  it("OBS-PANEL-Dock-TC-003: クリック送信内容の検証 (button.click)", () => {
    const ws = getSocket();

    ws.receive(
      JSON.stringify({
        type: "page.update",
        payload: {
          currentPage: "main",
          buttons: [{ x: 2, y: 1, label: "SWITCH" }],
        },
      })
    );

    const targetBtn = document.querySelector(
      'button[data-x="2"][data-y="1"]'
    ) as HTMLButtonElement;

    const sendSpy = vi.spyOn(ws, "send");

    // ボタンクリック
    targetBtn.click();

    expect(sendSpy).toHaveBeenCalledTimes(1);

    const sent = sendSpy.mock.calls[0][0] as string;
    const parsed = JSON.parse(sent);

    expect(parsed.type).toBe("button.click");
    expect(parsed.payload).toMatchObject({
      page: "main",
      x: 2,
      y: 1,
      source: "dock",
    });
  });

  it("OBS-PANEL-Dock-TC-004: 接続状態表示 (CONNECTED / DISCONNECTED / ERROR)", () => {
    const ws = getSocket();
    const statusEl = document.getElementById("status")!;

    // 接続成功
    ws.onopen && ws.onopen();
    expect(statusEl.textContent).toBe("CONNECTED");

    // 切断
    ws.onclose && ws.onclose();
    expect(statusEl.textContent).toBe("DISCONNECTED - RETRYING");

    // エラー
    ws.onerror && ws.onerror(new Event("error") as any);
    expect(statusEl.textContent).toBe("ERROR");
  });

  it("OBS-PANEL-Dock-TC-005: page.update の image をボタンに反映する", () => {
    const ws = getSocket();

    ws.receive(
      JSON.stringify({
        type: "page.update",
        payload: {
          currentPage: "main",
          buttons: [
            { x: 0, y: 0, label: "LIVE", image: "live.png" },
            { x: 1, y: 0, label: "REC" }, // image 未指定
          ],
        },
      }),
    );

    const btn00 = document.querySelector(
      'button[data-x="0"][data-y="0"]',
    ) as HTMLButtonElement | null;
    const btn10 = document.querySelector(
      'button[data-x="1"][data-y="0"]',
    ) as HTMLButtonElement | null;

    expect(btn00).not.toBeNull();
    expect(btn10).not.toBeNull();

    // image 指定あり → data-image に反映される
    expect(btn00!.dataset.image).toBe("live.png");

    // image 指定なし → data-image は未設定
    expect(btn10!.dataset.image).toBeUndefined();
  });

  it("OBS-PANEL-Dock-TC-Impl_001: page.update 以外のメッセージは無視される", () => {
    const ws = getSocket();

    // まず page.update でボタンを LIVE にしておく
    ws.receive(
      JSON.stringify({
        type: "page.update",
        payload: {
          currentPage: "main",
          buttons: [{ x: 0, y: 0, label: "LIVE" }],
        },
      })
    );

    const btn00 = document.querySelector(
      'button[data-x="0"][data-y="0"]'
    ) as HTMLButtonElement;
    expect(btn00.textContent).toBe("LIVE");

    // type が異なるメッセージを送る（page.update ではない）
    ws.receive(
      JSON.stringify({
        type: "something-else",
        payload: {
          currentPage: "main",
          buttons: [{ x: 0, y: 0, label: "CHANGED" }],
        },
      })
    );

    // ラベルは変更されない（page.update 以外は Dock UI が無視する）
    expect(btn00.textContent).toBe("LIVE");
  });

  it("OBS-PANEL-Dock-TC-Impl_002: data が null のメッセージは無視される", () => {
    const ws = getSocket();

    // null メッセージを送っても例外なく無視されること
    expect(() => {
      ws.receive(null as any);
    }).not.toThrow();

    // grid が空のままであることを軽く確認（全ボタン disabled のまま）
    const anyBtn = document.querySelector(
      'button[data-x="0"][data-y="0"]'
    ) as HTMLButtonElement;
    expect(anyBtn.classList.contains("disabled")).toBe(true);
    expect(anyBtn.textContent).toBe("");
  });

  it("OBS-PANEL-Dock-TC-Impl_003: 非文字列メッセージ & JSON パース失敗は無視される", () => {
    const ws = getSocket();

    // 非文字列（オブジェクト）を送り、toString() → JSON.parse 失敗の catch 経路を通す
    expect(() => {
      ws.receive({ foo: "bar" } as any);
    }).not.toThrow();

    // その結果として DOM に変化がない（全ボタン disabled のまま）ことを確認
    const anyBtn = document.querySelector(
      'button[data-x="0"][data-y="0"]'
    ) as HTMLButtonElement;
    expect(anyBtn.classList.contains("disabled")).toBe(true);
    expect(anyBtn.textContent).toBe("");
  });

  it("OBS-PANEL-Dock-TC-Impl_004: .btn 以外のクリックでは button.click を送信しない", () => {
    const ws = getSocket();
    const grid = document.getElementById("grid")!;

    // グリッド内に .btn ではない要素を追加
    const other = document.createElement("div");
    other.id = "not-a-button";
    grid.appendChild(other);

    const sendSpy = vi.spyOn(ws, "send");

    // .btn でない要素をクリック
    other.click();

    // button.click は送信されない
    expect(sendSpy).not.toHaveBeenCalled();
  });
});
