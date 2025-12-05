// OBS Dock UI 用エントリーポイント。
// ControlCore からのメッセージを受け取り、DOM に反映し、
// ボタンクリックを ControlCore に送信する役割を持つ。

// Dock UI 初期化オプション
export interface DockUiOptions {
  /** 接続先 WebSocket URL （必須） */
  wsUrl: string;
  /** グリッド行数（省略時 3） */
  rows?: number;
  /** グリッド列数（省略時 5） */
  cols?: number;
  /**
   * DOM を差し替えたい場合の Document（jsdom テスト用）。
   * 省略時はグローバル document を使用する。
   */
  document?: Document;
  /**
   * WebSocket 実装差し替え用（テスト用 FakeWebSocket など）。
   * 省略時はグローバル WebSocket を使用する。
   */
  WebSocketImpl?: typeof WebSocket;
}

// 内部で扱う page.update メッセージの型（必要最低限）
interface PageUpdatePayload {
  currentPage: string;
  buttons: Array<{
    x: number;
    y: number;
    label?: string;
  }>;
}

interface PageUpdateMessage {
  type: "page.update";
  payload: PageUpdatePayload;
}

/**
 * Dock UI を初期化する。
 * - グリッド DOM を構築する
 * - WebSocket に接続し、page.update を受けて描画を更新する
 * - クリックイベントを button.click として送信する
 *
 * @param configOrUrl WebSocket URL 文字列、または DockUiOptions
 * @returns dispose 関数などを含むハンドル
 */
export function initDockUi(
  configOrUrl: string | DockUiOptions
): {
  /** WebSocket インスタンス（テスト用） */
  socket: WebSocket;
  /** イベントハンドラなどを解除するための dispose 関数 */
  dispose: () => void;
} {
  // 引数を正規化
  const config: DockUiOptions =
    typeof configOrUrl === "string"
      ? { wsUrl: configOrUrl }
      : configOrUrl;

  const rows = config.rows ?? 3;
  const cols = config.cols ?? 5;

  // Document / WebSocket 実装の解決
  const doc: Document =
    config.document ?? (globalThis.document as Document);
  if (!doc) {
    throw new Error("Document が利用できません (Dock UI 初期化失敗)");
  }

  const WSImpl: typeof WebSocket =
    config.WebSocketImpl ?? (globalThis.WebSocket as typeof WebSocket);
  if (!WSImpl) {
    throw new Error("WebSocket 実装が利用できません (Dock UI 初期化失敗)");
  }

  // グリッドのルート要素とステータス表示要素
  const gridRoot = doc.getElementById("grid");
  const statusEl = doc.getElementById("status");

  if (!gridRoot) {
    throw new Error("#grid が見つかりません (Dock UI 初期化失敗)");
  }

  // ステータス表示ユーティリティ
  const setStatus = (text: string) => {
    if (statusEl) {
      statusEl.textContent = text;
    }
  };

  // グリッド DOM 構築
  type Cell = {
    x: number;
    y: number;
    button: HTMLButtonElement;
  };

  const cells: Cell[][] = [];
  gridRoot.innerHTML = "";

  for (let y = 0; y < rows; y++) {
    const row: Cell[] = [];
    const rowDiv = doc.createElement("div");
    rowDiv.className = "row";
    gridRoot.appendChild(rowDiv);

    for (let x = 0; x < cols; x++) {
      const btn = doc.createElement("button");
      btn.className = "btn disabled";
      btn.dataset.x = String(x);
      btn.dataset.y = String(y);
      btn.textContent = "";
      rowDiv.appendChild(btn);

      row.push({ x, y, button: btn });
    }

    cells.push(row);
  }

  // 現在ページ名を保持
  let currentPage = "main";

  // ボタン定義を反映するヘルパー
  const applyPageUpdate = (msg: PageUpdateMessage) => {
    const { currentPage: newPage, buttons } = msg.payload;
    currentPage = newPage;

    // 一旦全セルを無効化＆ラベルクリア
    for (const row of cells) {
      for (const cell of row) {
        cell.button.className = "btn disabled";
        cell.button.textContent = "";
      }
    }

    // ボタン定義を適用
    for (const def of buttons) {
      const { x, y, label } = def;
      if (y < 0 || y >= rows || x < 0 || x >= cols) {
        continue;
      }
      const cell = cells[y][x];
      cell.button.className = "btn";
      cell.button.textContent = label ?? "";
    }
  };

  // WebSocket 接続
  const socket: WebSocket = new WSImpl(config.wsUrl) as WebSocket;

  // WebSocket ハンドラ設定（FakeWebSocket を考慮し onXXX プロパティで統一）
  (socket as any).onopen = () => {
    setStatus("CONNECTED");
  };

  (socket as any).onclose = () => {
    setStatus("DISCONNECTED - RETRYING");
  };

  (socket as any).onerror = () => {
    setStatus("ERROR");
  };

  (socket as any).onmessage = (ev: MessageEvent | { data: any }) => {
    let raw = (ev as any).data;
    if (raw == null) return;

    try {
      if (typeof raw !== "string") {
        raw = String(raw);
      }
      const parsed = JSON.parse(raw) as PageUpdateMessage;

      if (parsed && parsed.type === "page.update") {
        applyPageUpdate(parsed);
      }
      // 他のメッセージ種別は現状無視
    } catch {
      // JSON でなければ無視
    }
  };

  // クリックイベントの設定
  const onClick = (ev: Event) => {
    const target = ev.target as HTMLElement | null;
    if (!target) return;
    if (!target.classList.contains("btn")) return;
    if (target.classList.contains("disabled")) return;

    const x = Number(target.dataset.x);
    const y = Number(target.dataset.y);

    const message = {
      type: "button.click",
      payload: {
        page: currentPage,
        x,
        y,
        source: "dock" as const,
      },
    };

    socket.send(JSON.stringify(message));
  };

  gridRoot.addEventListener("click", onClick);

  // テスト等でクリーンアップしたい場合の dispose
  const dispose = () => {
    gridRoot.removeEventListener("click", onClick);
    try {
      socket.close();
    } catch {
      // すでに閉じている等は無視
    }
  };

  return { socket, dispose };
}
