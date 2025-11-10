/**
 * Control Core
 *
 * 役割:
 * - panel.json を読み込み・検証し内部モデルに展開する
 * - 現在ページとボタン状態を管理する
 * - クライアント(Dock UI / Stream Deck)からの button.click を受けてアクションを実行する
 * - OBS WebSocket / 外部HTTP への呼び出しを行う（現時点ではIFのみ定義）
 * - クライアントへ page.update / status.update / error を送信する
 *
 * 注意:
 * - このファイルはコアロジックのみを定義する。
 * - WebSocketサーバ起動や実プロセスエントリポイントは別ファイル(main.ts等)で実装する。
 */

import fs from "node:fs";
import path from "node:path";

//
// 型定義
//

export type ActionType =
  | "obs.setScene"
  | "obs.toggleStream"
  | "obs.toggleRecord"
  | "obs.toggleMute"
  | "obs.setSourceVisibility"
  | "page.switch"
  | "http.get"
  | "http.post";

export interface PanelAction {
  type: ActionType | string; // 未定義typeはバリデーションで弾く
  scene?: string;
  source?: string;
  visible?: boolean;
  page?: string;
  url?: string;
  display?: string;
}

export interface PanelButton {
  x: number;
  y: number;
  label: string;
  action: PanelAction;
}

export interface PanelPage {
  name: string;
  buttons: PanelButton[];
}

export interface PanelConfig {
  version: number;
  pages: Record<string, PanelPage>;
}

export type ClientKind = "dock" | "streamdeck";

export interface ButtonClickMessage {
  type: "button.click";
  payload: {
    page: string;
    x: number;
    y: number;
    source: ClientKind;
  };
}

export interface PageUpdateButtonState {
  x: number;
  y: number;
  label: string;
  state?: "on" | "off" | "disabled";
}

export interface PageUpdateMessage {
  type: "page.update";
  payload: {
    currentPage: string;
    buttons: PageUpdateButtonState[];
  };
}

export interface StatusUpdateMessage {
  type: "status.update";
  payload: {
    streaming?: boolean;
    recording?: boolean;
    [key: string]: unknown;
  };
}

export interface ErrorMessage {
  type: "error";
  requestId?: string;
  payload: {
    code: string;
    message: string;
  };
}

/**
 * クライアント送信インターフェース
 * 実装側で WebSocket 等に接続して利用する。
 */
export interface ClientBroadcaster {
  // Dock / StreamDeck を意識せず全クライアントに送る
  broadcast(msg: PageUpdateMessage | StatusUpdateMessage | ErrorMessage): void;
}

/**
 * OBS制御インターフェース
 * 実装側で obs-websocket-js 等を使って実装する。
 */
export interface ObsController {
  setScene(scene: string): Promise<void>;
  toggleStream(): Promise<void>;
  toggleRecord(): Promise<void>;
  toggleMute(source: string): Promise<void>;
  setSourceVisibility(scene: string, source: string, visible: boolean): Promise<void>;
  getStatus(): Promise<{ streaming: boolean; recording: boolean }>;
}

/**
 * HTTPクライアントインターフェース
 * 外部I/F呼び出し用。
 */
export interface HttpClient {
  get(url: string): Promise<unknown>;
  post(url: string, body: unknown): Promise<unknown>;
}

/**
 * ControlCoreOptions:
 * - configPath: panel.json のパス
 * - broadcaster: クライアント通知用
 * - obs: OBS制御用
 * - http: HTTP呼び出し用
 */
export interface ControlCoreOptions {
  configPath: string;
  broadcaster: ClientBroadcaster;
  obs: ObsController;
  http: HttpClient;
}

export class ControlCore {
  private readonly configPath: string;
  private readonly broadcaster: ClientBroadcaster;
  private readonly obs: ObsController;
  private readonly http: HttpClient;

  private config: PanelConfig | null = null;
  private currentPageKey: string | null = null;

  constructor(options: ControlCoreOptions) {
    this.configPath = options.configPath;
    this.broadcaster = options.broadcaster;
    this.obs = options.obs;
    this.http = options.http;
  }

  /**
   * 初期化:
   * - panel.json 読み込み
   * - バリデーション
   * - currentPage 設定
   * - 初期 page.update 送信
   */
  initialize(): void {
    const raw = fs.readFileSync(this.configPath, "utf-8");
    const json = JSON.parse(raw) as PanelConfig;
    this.validateConfig(json);
    this.config = json;

    // デフォルトページ: "main" 優先、なければ最初のキー
    const pageKeys = Object.keys(json.pages);
    if (pageKeys.length === 0) {
      throw new Error("No pages defined in panel.json");
    }
    this.currentPageKey = json.pages["main"] ? "main" : pageKeys[0];

    this.pushPageUpdate();
  }

  /**
   * button.click メッセージ入口
   * テスト側はここを直接呼び出す。
   */
  async handleButtonClick(msg: ButtonClickMessage): Promise<void> {
    if (!this.config || !this.currentPageKey) {
      this.sendError("NOT_INITIALIZED", "ControlCore is not initialized");
      return;
    }

    const { page, x, y } = msg.payload;

    // 現在ページ以外からの座標クリックは無視（明示ルール）
    if (page !== this.currentPageKey) {
      this.sendError("INVALID_PAGE_CONTEXT", `Click from non-current page: ${page}`);
      return;
    }

    const pageDef = this.config.pages[this.currentPageKey];
    if (!pageDef) {
      this.sendError("INVALID_PAGE", `Page not found: ${this.currentPageKey}`);
      return;
    }

    const btn = pageDef.buttons.find(b => b.x === x && b.y === y);
    if (!btn) {
      this.sendError("NO_BUTTON", `No button at (${x},${y}) on page ${page}`);
      return;
    }

    await this.executeAction(btn.action);
  }

  /**
   * panel.json バリデーション:
   * - version
   * - pages存在
   * - buttons座標重複チェック
   * - 必須フィールドの最低限チェック
   */
  private validateConfig(config: PanelConfig): void {
    if (typeof config.version !== "number") {
      throw new Error("panel.json: 'version' must be number");
    }
    if (!config.pages || typeof config.pages !== "object") {
      throw new Error("panel.json: 'pages' is required");
    }

    for (const [pageKey, page] of Object.entries(config.pages)) {
      if (!page.buttons || !Array.isArray(page.buttons)) {
        throw new Error(`panel.json: page '${pageKey}' has no buttons array`);
      }
      const used = new Set<string>();
      for (const btn of page.buttons) {
        const key = `${btn.x},${btn.y}`;
        if (used.has(key)) {
          throw new Error(`panel.json: duplicate button position (${key}) in page '${pageKey}'`);
        }
        used.add(key);
        if (!btn.label || !btn.action) {
          throw new Error(`panel.json: button at (${key}) in page '${pageKey}' missing label or action`);
        }
      }
    }
  }

  /**
   * アクション実行。
   * ここではIFのみ実装し、副作用は ObsController / HttpClient に委譲する。
   */
  private async executeAction(action: PanelAction): Promise<void> {
    switch (action.type) {
      case "obs.setScene":
        if (!action.scene) {
          this.sendError("INVALID_ACTION", "obs.setScene requires 'scene'");
          return;
        }
        await this.obs.setScene(action.scene);
        await this.updateStatusFromObs();
        return;

      case "obs.toggleStream":
        await this.obs.toggleStream();
        await this.updateStatusFromObs();
        return;

      case "obs.toggleRecord":
        await this.obs.toggleRecord();
        await this.updateStatusFromObs();
        return;

      case "obs.toggleMute":
        if (!action.source) {
          this.sendError("INVALID_ACTION", "obs.toggleMute requires 'source'");
          return;
        }
        await this.obs.toggleMute(action.source);
        await this.updateStatusFromObs();
        return;

      case "obs.setSourceVisibility":
        if (!action.scene || !action.source || typeof action.visible !== "boolean") {
          this.sendError("INVALID_ACTION", "obs.setSourceVisibility requires 'scene', 'source', 'visible'");
          return;
        }
        await this.obs.setSourceVisibility(action.scene, action.source, action.visible);
        return;

      case "page.switch":
        if (!action.page) {
          this.sendError("INVALID_ACTION", "page.switch requires 'page'");
          return;
        }
        this.switchPage(action.page);
        return;

      case "http.get":
        if (!action.url) {
          this.sendError("INVALID_ACTION", "http.get requires 'url'");
          return;
        }
        await this.handleHttpAction("GET", action.url, action.display);
        return;

      case "http.post":
        if (!action.url) {
          this.sendError("INVALID_ACTION", "http.post requires 'url'");
          return;
        }
        await this.handleHttpAction("POST", action.url, action.display);
        return;

      default:
        this.sendError("INVALID_ACTION", `Unknown action type: ${action.type}`);
        return;
    }
  }

  private switchPage(page: string): void {
    if (!this.config) {
      this.sendError("NOT_INITIALIZED", "Config not loaded");
      return;
    }
    if (!this.config.pages[page]) {
      this.sendError("INVALID_PAGE", `Unknown page: ${page}`);
      return;
    }
    this.currentPageKey = page;
    this.pushPageUpdate();
  }

  /**
   * 現在ページ情報を page.update として全クライアントに通知。
   */
  private pushPageUpdate(): void {
    if (!this.config || !this.currentPageKey) return;
    const page = this.config.pages[this.currentPageKey];
    if (!page) return;

    const buttons: PageUpdateButtonState[] = page.buttons.map(b => ({
      x: b.x,
      y: b.y,
      label: b.label
      // state は将来拡張（配信中など）
    }));

    const msg: PageUpdateMessage = {
      type: "page.update",
      payload: {
        currentPage: this.currentPageKey,
        buttons
      }
    };
    this.broadcaster.broadcast(msg);
  }

  private async updateStatusFromObs(): Promise<void> {
    try {
      const status = await this.obs.getStatus();
      const msg: StatusUpdateMessage = {
        type: "status.update",
        payload: {
          streaming: status.streaming,
          recording: status.recording
        }
      };
      this.broadcaster.broadcast(msg);
    } catch (e) {
      this.sendError("OBS_STATUS_FAILED", `Failed to fetch OBS status: ${(e as Error).message}`);
    }
  }

  private async handleHttpAction(
    method: "GET" | "POST",
    url: string,
    displayKey?: string
  ): Promise<void> {
    try {
      const res =
        method === "GET"
          ? await this.http.get(url)
          : await this.http.post(url, {});

      if (displayKey && this.config && this.currentPageKey) {
        const page = this.config.pages[this.currentPageKey];
        if (page) {
          const value =
            res && typeof res === "object" && displayKey in (res as any)
              ? String((res as any)[displayKey])
              : undefined;
          if (value) {
            // 最初のボタンラベルを書き換えるなどの簡易サンプル
            page.buttons[0].label = value;
            this.pushPageUpdate();
          }
        }
      }
    } catch (e) {
      this.sendError("HTTP_FAILED", `HTTP ${method} failed: ${(e as Error).message}`);
    }
  }

  private sendError(code: string, message: string): void {
    const msg: ErrorMessage = {
      type: "error",
      payload: { code, message }
    };
    this.broadcaster.broadcast(msg);
  }
}
