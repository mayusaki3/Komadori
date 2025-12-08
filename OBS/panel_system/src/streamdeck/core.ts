// OBS/panel_system/src/streamdeck/core.ts

/**
 * Stream Deck プラグイン側のコアロジック。
 *
 * 役割:
 * - Control Core からの page.update を受けて、Stream Deck のキー表示（ラベル / 画像）を更新する。
 * - Stream Deck のボタン押下を Control Core の button.click メッセージに変換して送信する。
 *
 * 注意:
 * - Elgato 純正 SDK / WebSocket とは切り離し、テストでモック可能な I/F のみを提供する。
 * - 実際の SDK ラッパー側からは、本モジュールの公開関数を呼び出すだけにする。
 */

/**
 * Control Core とのメッセージプロトコル（必要最小限）。
 */
export type CoreMessage =
  | {
      type: "page.update";
      payload: {
        currentPage: string;
        buttons: {
          x: number;
          y: number;
          label: string;
          image?: string;
        }[];
      };
    }
  | {
      type: "status";
      payload: {
        status: "CONNECTED" | "DISCONNECTED" | "ERROR";
      };
    }
  // その他のメッセージ型は必要に応じて追加
  | {
      type: string;
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      payload?: any;
    };

/**
 * Stream Deck コアに注入する依存関係。
 * すべてテスト時にモック可能とする。
 */
export type StreamDeckCoreDeps = {
  /**
   * Control Core へメッセージを送信するコールバック。
   * 実際には WebSocket や Stream Deck の sendToPlugin などに対応させる。
   */
  sendToCore: (message: CoreMessage | { type: "button.click"; payload: { page: string | null; x: number; y: number } }) => void;

  /**
   * 指定座標のキー表示を更新する。
   * テストではモック化し、呼び出し回数や引数を検証する。
   */
  updateKey: (args: { x: number; y: number; label: string; image?: string }) => void;

  /**
   * 行数・列数。
   * レイアウトチェックや範囲外防止に使用する。
   */
  rows: number;
  cols: number;
};

/**
 * Stream Deck コアが外部に提供する I/F。
 */
export type StreamDeckCore = {
  /**
   * Control Core からのメッセージを処理する。
   * JSON 文字列またはオブジェクトのどちらでも受け付ける。
   */
  handleCoreMessage: (message: string | CoreMessage) => void;

  /**
   * Stream Deck のキー押下イベントを処理する。
   * SDK ラッパー側から呼び出す想定。
   */
  handleKeyDown: (x: number, y: number) => void;

  /**
   * 現在のページキーを取得する（テスト / デバッグ用）。
   */
  getCurrentPage: () => string | null;
};

/**
 * Stream Deck コアを生成するファクトリ関数。
 */
export function createStreamDeckCore(deps: StreamDeckCoreDeps): StreamDeckCore {
  const { sendToCore, updateKey, rows, cols } = deps;

  // 現在のページキーと、直近の page.update のボタン一覧を保持する。
  let currentPage: string | null = null;
  let lastButtons: { x: number; y: number; label: string; image?: string }[] = [];

  /**
   * 座標が Stream Deck の範囲内か判定する。
   */
  function isValidCoord(x: number, y: number): boolean {
    if (!Number.isInteger(x) || !Number.isInteger(y)) return false;
    if (x < 0 || y < 0) return false;
    if (x >= cols || y >= rows) return false;
    return true;
  }

  /**
   * page.update を受け取ったときの処理。
   * 全キーの表示を更新する。
   */
  function applyPageUpdate(payload: CoreMessage["payload"] & { currentPage: string; buttons: { x: number; y: number; label: string; image?: string }[] }) {
    currentPage = payload.currentPage;
    lastButtons = payload.buttons ?? [];

    // まず全キーをクリア
    for (let y = 0; y < rows; y += 1) {
      for (let x = 0; x < cols; x += 1) {
        updateKey({ x, y, label: "", image: undefined });
      }
    }

    // 受け取ったボタン定義を反映
    for (const btn of lastButtons) {
      const { x, y, label, image } = btn;
      if (!isValidCoord(x, y)) continue;

      updateKey({ x, y, label, image });
    }
  }

  /**
   * Control Core から受信したメッセージを処理する。
   */
  function handleCoreMessage(message: string | CoreMessage): void {
    let msg: CoreMessage;

    if (typeof message === "string") {
      try {
        msg = JSON.parse(message) as CoreMessage;
      } catch {
        // パースできないメッセージは無視
        return;
      }
    } else {
      msg = message;
    }

    if (!msg || typeof msg.type !== "string") return;

    switch (msg.type) {
      case "page.update":
        applyPageUpdate(msg.payload as any);
        break;
      case "status":
        // 必要であればステータスに応じてキー表示を変えるなどの処理を追加
        break;
      default:
        // その他のメッセージは現状何もしない
        break;
    }
  }

  /**
   * Stream Deck のキー押下を Control Core の button.click に変換する。
   */
  function handleKeyDown(x: number, y: number): void {
    if (!isValidCoord(x, y)) {
      return;
    }

    // currentPage が無ければ押下は無視（まだ page.update を受け取っていない状態）
    const page = currentPage;

    sendToCore({
      type: "button.click",
      payload: {
        page,
        x,
        y,
      },
    });
  }

  function getCurrentPage(): string | null {
    return currentPage;
  }

  return {
    handleCoreMessage,
    handleKeyDown,
    getCurrentPage,
  };
}
