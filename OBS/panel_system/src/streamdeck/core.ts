/**
 * Stream Deck 向けパネルコア実装。
 * page.update メッセージに応じてキー表示を更新し、
 * キー押下時に button.click メッセージをコアへ送信する。
 */

export type PageUpdateButton = {
  x: number;
  y: number;
  label?: string;
  image?: string;
};

export type PageUpdatePayload = {
  currentPage: string;
  buttons: PageUpdateButton[];
};

export type PageUpdateMessage = {
  type: "page.update";
  payload: PageUpdatePayload;
};

export type ConnectionStatus = "disconnected" | "connected" | "error";

export type ButtonClickPayload = {
  page: string;
  x: number;
  y: number;
};

export type ButtonClickMessage = {
  type: "button.click";
  payload: ButtonClickPayload;
};

/**
 * StreamDeck SDK から渡されるキー更新関数。
 * index は 0〜 rows*cols-1 のキー番号。
 * title はキー上に表示するテキスト。
 * image はキーに表示する画像パス（任意）。
 */
export type UpdateKeyFn = (index: number, title: string, image?: string) => void;

/**
 * コア → OBS パネル側への送信関数など、デッキ依存の情報。
 */
export type StreamDeckCoreDeps = {
  rows: number;
  cols: number;
  /**
   * キー更新コールバック。
   * 旧コード/テスト側が onUpdateKey を使っている可能性も考慮して両方許容する。
   */
  updateKey?: UpdateKeyFn;
  onUpdateKey?: UpdateKeyFn;
  sendToCore: (msg: ButtonClickMessage) => void;
};

/**
 * テストが期待する Stream Deck Core の公開インターフェース。
 */
export type StreamDeckCore = {
  /**
   * page.update メッセージを直接適用するユーティリティ。
   * テストではここを直接叩いてキー表示の状態を検証する。
   */
  applyPageUpdate: (msg: PageUpdateMessage) => void;

  /**
   * コアからの汎用メッセージを処理する。
   * 実運用では WebSocket から流れてきたメッセージをここに渡す想定。
   */
  handleCoreMessage: (msg: unknown) => void;

  /**
   * キー押下イベントハンドラ。
   * keyIndex は 0〜 rows*cols-1 のキー番号。
   */
  handleKeyDown: (keyIndex: number) => void;

  /**
   * 現在アクティブなページキー（page.update.currentPage）を返す。
   */
  getCurrentPage: () => string | undefined;

  /**
   * 指定キーの「現在状態」を返す。
   * テストでは title / image を確認する。
   */
  getKeyState: (index: number) => { title: string; image?: string; disabled: boolean } | undefined;

  /**
   * 接続状態（簡易） getter / setter。
   * テストでは get → 初期値 / set → 反映 だけを確認する。
   */
  getConnectionStatus: () => ConnectionStatus;
  setConnectionStatus: (status: ConnectionStatus) => void;
};

/**
 * StreamDeckCore を生成するファクトリ。
 * テストでは createStreamDeckCore(...) を呼んで core を取得する。
 */
export function createStreamDeckCore(
  deps: StreamDeckCoreDeps & { sendButtonClick?: (msg: ButtonClickMessage) => void },
): StreamDeckCore {
  const { rows, cols } = deps;

  // sendToCore / sendButtonClick どちらでも受け付ける
  const sendToCore: (msg: ButtonClickMessage) => void =
    typeof deps.sendToCore === "function"
      ? deps.sendToCore
      : typeof deps.sendButtonClick === "function"
      ? deps.sendButtonClick
      : () => {};

  // deps.updateKey / deps.onUpdateKey のどちらかを使う（どちらも無ければ no-op）
  const rawUpdateKey =
    (deps.updateKey as UpdateKeyFn | undefined) ??
    (deps.onUpdateKey as UpdateKeyFn | undefined);

  const safeUpdateKey: UpdateKeyFn =
    typeof rawUpdateKey === "function"
      ? rawUpdateKey
      : (_index: number, _title: string, _image?: string) => {
          // no-op（テスト側で updateKey を渡していない場合は何も起きない）
        };

  // 現在ページキー（例: "main" / "util"）
  let currentPage: string | undefined;

  // 接続状態（テスト用の簡易状態）
  let connectionStatus: ConnectionStatus = "disconnected";

  // 各キーの現在状態（テストが title/image/disabled を見る）
  const keyStates: { title: string; image?: string; disabled: boolean }[] = Array.from(
    { length: rows * cols },
    () => ({ title: "", image: undefined, disabled: true }),
  );

  // 現在のページに対するボタン定義のグリッド
  const buttonGrid: (PageUpdateButton | undefined)[][] = Array.from(
    { length: rows },
    () => Array.from({ length: cols }, () => undefined),
  );

  /**
   * ペイロード部分を適用する内部ヘルパー。
   * handleCoreMessage / core.applyPageUpdate の両方から利用する。
   */
  const applyPageUpdatePayload = (payload: PageUpdatePayload) => {
    if (!payload || !Array.isArray(payload.buttons)) {
      return;
    }

    const { currentPage: newPage, buttons } = payload;

    if (typeof newPage === "string" && newPage.length > 0) {
      currentPage = newPage;
    }

    // 1) すべてのキー表示をクリア
    for (let y = 0; y < rows; y++) {
      for (let x = 0; x < cols; x++) {
        buttonGrid[y][x] = undefined;
        const index = y * cols + x;
        // 内部状態もクリア（disabled = true）
        keyStates[index] = { title: "", image: undefined, disabled: true };
        // 実際のキー表示もクリア
        safeUpdateKey(index, "", undefined);
      }
    }

    // 2) 有効なボタン定義だけを反映
    for (const def of buttons) {
      if (!def) continue;

      const x = Number(def.x);
      const y = Number(def.y);

      if (
        !Number.isFinite(x) ||
        !Number.isFinite(y) ||
        y < 0 ||
        y >= rows ||
        x < 0 ||
        x >= cols
      ) {
        // グリッド外定義は無視（TC-002 想定）
        continue;
      }

      const label = def.label ?? "";
      const image = def.image;

      buttonGrid[y][x] = { x, y, label, image };

      const index = y * cols + x;
      // 内部状態を更新（有効キーなので disabled = false）
      keyStates[index] = { title: label, image, disabled: false };
      // 実際のキー表示も更新
      safeUpdateKey(index, label, image);
    }
  };

  /**
   * 公開用: PageUpdateMessage を直接適用する。
   * テストではここを叩いて page.update によるキー表示更新・ページ切替を確認する。
   */
  const applyPageUpdate = (msg: PageUpdateMessage) => {
    if (!msg || msg.type !== "page.update") return;
    applyPageUpdatePayload(msg.payload);
  };

  /**
   * コアからの汎用メッセージを処理するハンドラ。
   * 現時点では page.update のみを扱う。
   */
  const handleCoreMessage = (msg: unknown) => {
    const m = msg as { type?: string; payload?: PageUpdatePayload } | null | undefined;
    if (!m || typeof m.type !== "string") return;

    if (m.type === "page.update") {
      if (m.payload) {
        applyPageUpdatePayload(m.payload);
      }
    }
    // 将来の拡張用: 他のメッセージ種別が来た場合は無視
  };

  /**
   * キー押下イベント。
   * rows * cols の 1D インデックスから x,y を求め、該当ボタンがあれば button.click を送信する。
   */
  const handleKeyDown = (keyIndex: number) => {
    if (!Number.isFinite(keyIndex)) return;
    if (keyIndex < 0 || keyIndex >= rows * cols) return;

    const x = keyIndex % cols;
    const y = Math.floor(keyIndex / cols);

    const def = buttonGrid[y]?.[x];
    if (!def) {
      // 定義がないキーは何もしない（TC-002 想定）
      return;
    }

    const page = currentPage ?? "main";

    const msg: ButtonClickMessage = {
      type: "button.click",
      payload: { page, x, y },
    };

    sendToCore(msg);
  };

  /**
   * 現在のページキーを返す。
   */
  const getCurrentPage = (): string | undefined => currentPage;

  /**
   * 指定キーの現在状態を返す。
   * インデックス範囲外なら undefined。
   */
  const getKeyState = (
    index: number,
  ): { title: string; image?: string; disabled: boolean } | undefined => {
    if (!Number.isFinite(index)) return undefined;
    if (index < 0 || index >= rows * cols) return undefined;
    return keyStates[index];
  };

  /**
   * 接続状態 getter / setter。
   * テストでは単純に状態の保存・取得のみ確認する。
   */
  const getConnectionStatus = (): ConnectionStatus => connectionStatus;

  const setConnectionStatus = (status: ConnectionStatus) => {
    connectionStatus = status;
  };

  return {
    applyPageUpdate,
    handleCoreMessage,
    handleKeyDown,
    getCurrentPage,
    getKeyState,
    getConnectionStatus,
    setConnectionStatus,
  };
}
