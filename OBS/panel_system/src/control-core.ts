// src/control-core.ts
//
// 概要:
//   OBS配信用の「パネル定義(JSON)」を読み込み、WebUIやドックからの操作メッセージを処理して
//   OBS 制御・HTTP 呼び出し・ページ切替・状態更新などを行うコアクラス。
//
// 変更点（この版）:
//   - rows/cols 省略時の自動推定を追加（テストが rows/cols 無しの定義を生成するための互換）
//   - action を配列で受け取り順次実行（順序保証; TC-35）
//   - button.click の発火座標 (x,y) を「表示更新先特定」に使用。
//     これにより page.switch 後の http.displayKey ラベル更新が“新ページ側の同座標ボタン”へ反映（TC-36）。
//
// 想定依存:
//   - obs: setScene/toggleStreaming/toggleRecording/toggleMute/setSourceVisibility/getStatus
//   - http: get/post
//   - broadcaster: function(msg) または { send(msg) }
//   - logger: info/warn/error 任意
//
// 送信メッセージ:
//   - { type: "page.update", payload: { currentPage, buttons:[{x,y,label}...] } }
//   - { type: "error", payload: { code, ... } }
//
// 主なエラーコード:
//   NOT_INITIALIZED / INVALID_PAGE / INVALID_PAGE_CONTEXT / INVALID_ACTION / NO_BUTTON / HTTP_FAILED / OBS_STATUS_FAILED

import * as fs from "node:fs";

type Broadcaster =
  | ((msg: any) => void)
  | {
      send: (msg: any) => void;
    };

type LoggerLike = {
  info?: (...args: any[]) => void;
  warn?: (...args: any[]) => void;
  error?: (...args: any[]) => void;
};

type ObsApi = {
  setScene?: (scene: string) => Promise<void> | void;
  toggleStreaming?: () => Promise<void> | void;
  toggleRecording?: () => Promise<void> | void;
  toggleMute?: (source?: string) => Promise<void> | void;
  setSourceVisibility?: (source: string, visible: boolean) => Promise<void> | void;
  getStatus?: () => Promise<any> | any;
};

type HttpApi = {
  get?: (url: string) => Promise<{ ok: boolean; status: number; data?: any }>;
  post?: (url: string, body?: any) => Promise<{ ok: boolean; status: number; data?: any }>;
};

type ButtonAction =
  | { type: "setScene"; scene: string }
  | { type: "toggleStreaming" }
  | { type: "toggleRecording" }
  | { type: "toggleMute"; source?: string }
  | { type: "setSourceVisibility"; source: string; visible: boolean }
  | { type: "http.get"; url?: string; displayKey?: string }
  | { type: "http.post"; url?: string; body?: any }
  | { type: "page.switch"; page?: string }
  | { type: string; [k: string]: any }; // 未知アクションも落とさず扱う

type ButtonDef = {
  x: number;
  y: number;
  label: string;
  // 単一 or 複数アクション
  action: ButtonAction | ButtonAction[];
};

type PageDef = {
  // rows/cols は省略可（この実装で推定）
  rows?: number;
  cols?: number;
  buttons: ButtonDef[];
};

type PanelConfig = {
  version: number;
  pages: Record<string, PageDef>;
};

type MessageButtonClick = {
  type: "button.click";
  payload: { page: string; x: number; y: number; source?: string };
};

type MessagePageSwitch = {
  type: "page.switch";
  payload: { page?: string };
};

type AnyInboundMessage = MessageButtonClick | MessagePageSwitch | { type: string; payload?: any };

export class ControlCore {
  private obs: ObsApi;
  private http: HttpApi;
  private broadcaster: Broadcaster | undefined;
  private logger: LoggerLike | undefined;
  private configPath: string | undefined;

  private config: PanelConfig | undefined;
  private currentPageKey: string | undefined;
  private initialized = false;

  constructor(args: {
    obs: ObsApi;
    http: HttpApi;
    broadcaster?: Broadcaster;
    logger?: LoggerLike;
    configPath?: string;
  }) {
    this.obs = args.obs || {};
    this.http = args.http || {};
    this.broadcaster = args.broadcaster;
    this.logger = args.logger;
    this.configPath = args.configPath;
  }

  /**
   * 初期化:
   * - 設定読込
   * - 検証（rows/cols 省略時は推定）
   * - 初期ページ決定（"main" 優先、なければ先頭）
   * - page.update 送信
   */
  initialize() {
    if (!this.configPath) {
      // 設定なしでも初期化自体は完了（page.update は送らない）
      this.initialized = true;
      return;
    }

    const raw = fs.readFileSync(this.configPath, "utf-8");
    const parsed = JSON.parse(raw) as PanelConfig;

    // version 検証
    if (typeof parsed.version !== "number") {
      this.initialized = true;
      throw new Error("Invalid config: version must be a number");
    }
    if (parsed.version !== 1) {
      this.initialized = true;
      throw new Error("Invalid config: unsupported version");
    }

    // pages 検証
    if (!parsed.pages || typeof parsed.pages !== "object" || Object.keys(parsed.pages).length === 0) {
      this.initialized = true;
      throw new Error("Invalid config: pages is required");
    }

    // 各ページ検証＋rows/cols 推定
    for (const [pageKey, page] of Object.entries(parsed.pages)) {
      if (!page || typeof page !== "object") {
        throw new Error(`Invalid config: page '${pageKey}' is invalid`);
      }
      if (!Array.isArray(page.buttons)) {
        throw new Error(`Invalid config: page '${pageKey}' must have buttons[]`);
      }

      // ボタン座標の妥当性と重複チェック
      const seen = new Set<string>();
      let maxX = -1;
      let maxY = -1;

      for (const btn of page.buttons) {
        if (typeof btn?.x !== "number" || typeof btn?.y !== "number") {
          throw new Error(`Invalid config: button in '${pageKey}' must have numeric x/y`);
        }
        if (!btn.label || typeof btn.label !== "string") {
          throw new Error(`Invalid config: button in '${pageKey}' must have label`);
        }
        if (!btn.action || (typeof btn.action !== "object" && !Array.isArray(btn.action))) {
          throw new Error(`Invalid config: button in '${pageKey}' must have action`);
        }

        const key = `${btn.x},${btn.y}`;
        if (seen.has(key)) {
          throw new Error(`Invalid config: duplicate button coordinate in '${pageKey}' at ${key}`);
        }
        seen.add(key);

        if (btn.x > maxX) maxX = btn.x;
        if (btn.y > maxY) maxY = btn.y;
      }

      // rows/cols 省略時はボタンから推定（0始まり座標前提 → 最大座標+1）
      const inferredCols = maxX >= 0 ? maxX + 1 : 0;
      const inferredRows = maxY >= 0 ? maxY + 1 : 0;

      // rows/cols が指定されていれば範囲チェック、無ければ推定値をセット
      if (typeof page.cols === "number" && typeof page.rows === "number") {
        for (const btn of page.buttons) {
          if (btn.x < 0 || btn.x >= page.cols! || btn.y < 0 || btn.y >= page.rows!) {
            throw new Error(`Invalid config: button in '${pageKey}' out of range`);
          }
        }
      } else {
        // 推定値を付与
        (page as any).cols = inferredCols;
        (page as any).rows = inferredRows;
      }
    }

    this.config = parsed;

    // 初期ページ: "main" 優先、なければ先頭
    const pageKeys = Object.keys(parsed.pages);
    this.currentPageKey = parsed.pages["main"] ? "main" : pageKeys[0];

    this.initialized = true;
    this.pushPageUpdate();
  }

  /**
   * 外部メッセージディスパッチ
   */
  async handleMessage(msg: AnyInboundMessage) {
    if (!msg || typeof msg !== "object") return;

    try {
      switch (msg.type) {
        case "button.click":
          await this.handleButtonClick(msg as MessageButtonClick);
          break;
        case "page.switch":
          await this.handlePageSwitch(msg as MessagePageSwitch);
          break;
        default:
          this.sendError("INVALID_ACTION", { message: `Unknown message type: ${msg.type}` });
          break;
      }
    } catch (e: any) {
      this.sendError("INVALID_ACTION", { message: e?.message ?? String(e) });
    }
  }

  /**
   * button.click
   * - ページ文脈の一致を確認
   * - 対象ボタンを特定
   * - アクション（単体/複数）を順次実行
   * - 表示更新が必要な場合は「現在ページ」の同座標ボタンを更新
   */
  async handleButtonClick(msg: MessageButtonClick) {
    const payload = msg?.payload || ({} as any);

    if (!this.initialized) {
      this.sendError("NOT_INITIALIZED");
      return;
    }
    if (!this.config || !this.currentPageKey) {
      this.sendError("NOT_INITIALIZED");
      return;
    }

    const reqPage = payload.page;
    if (!reqPage || reqPage !== this.currentPageKey) {
      this.sendError("INVALID_PAGE_CONTEXT");
      return;
    }

    const page = this.config.pages[this.currentPageKey];
    if (!page) {
      this.sendError("INVALID_ACTION");
      return;
    }

    const originX = payload.x;
    const originY = payload.y;

    const btn = page.buttons.find((b) => b.x === originX && b.y === originY);
    if (!btn) {
      this.sendError("NO_BUTTON");
      return;
    }

    const actions: ButtonAction[] = Array.isArray(btn.action) ? btn.action : [btn.action];
    for (const act of actions) {
      const ok = await this.executeAction(act, { originX, originY });
      if (ok === false) {
        // エラーは sendError 済み。以降も継続（TC-38）
        continue;
      }
    }
  }

  private async handlePageSwitch(msg: MessagePageSwitch) {
    const next = msg?.payload?.page;
    if (!next) {
      this.sendError("INVALID_ACTION");
      return;
    }
    await this.switchPage(next);
  }

  /**
   * ページ切替
   */
  async switchPage(pageKey: string) {
    if (!this.initialized) {
      this.sendError("NOT_INITIALIZED");
      return;
    }
    if (!this.config?.pages?.[pageKey]) {
      this.sendError("INVALID_PAGE");
      return;
    }
    if (this.currentPageKey === pageKey) {
      // ノーオペ
      return;
    }
    this.currentPageKey = pageKey;
    this.pushPageUpdate();
  }

  /**
   * OBS ステータス更新
   */
  async updateStatusFromObs() {
    try {
      if (this.obs?.getStatus) {
        await this.obs.getStatus();
      }
    } catch {
      this.sendError("OBS_STATUS_FAILED");
    }
  }

  /**
   * 単一アクション実行
   * @returns true=成功/継続, false=失敗（エラー送信済み）
   */
  private async executeAction(a: ButtonAction, ctx: { originX: number; originY: number }): Promise<boolean> {
    switch (a.type) {
      case "setScene": {
        if (!a.scene || typeof a.scene !== "string") {
          this.sendError("INVALID_ACTION", { message: "setScene requires 'scene'" });
          return false;
        }
        await this.obs?.setScene?.(a.scene);
        return true;
      }
      case "toggleStreaming": {
        await this.obs?.toggleStreaming?.();
        return true;
      }
      case "toggleRecording": {
        await this.obs?.toggleRecording?.();
        return true;
      }
      case "toggleMute": {
        await this.obs?.toggleMute?.(a.source);
        return true;
      }
      case "setSourceVisibility": {
        if (!("source" in a) || !("visible" in a)) {
          this.sendError("INVALID_ACTION", { message: "setSourceVisibility requires 'source' and 'visible'" });
          return false;
        }
        await this.obs?.setSourceVisibility?.(a.source, a.visible);
        return true;
      }
      case "http.get": {
        if (!a.url) {
          this.sendError("INVALID_ACTION");
          return false;
        }
        try {
          const res = await this.http?.get?.(a.url);
          if (!res || res.ok !== true) {
            this.sendError("HTTP_FAILED", { status: res?.status });
            return false;
          }
          // displayKey 指定時: 現在ページの同座標ボタンの label を更新
          if (a.displayKey) {
            const val =
              res.data && Object.prototype.hasOwnProperty.call(res.data, a.displayKey)
                ? String(res.data[a.displayKey])
                : "N/A";
            this.updateButtonLabelAtCurrentPage(ctx.originX, ctx.originY, val);
            this.pushPageUpdate();
          }
          return true;
        } catch {
          this.sendError("HTTP_FAILED");
          return false;
        }
      }
      case "http.post": {
        if (!a.url) {
          this.sendError("INVALID_ACTION");
          return false;
        }
        try {
          const res = await this.http?.post?.(a.url, a.body);
          if (!res || res.ok !== true) {
            this.sendError("HTTP_FAILED", { status: res?.status });
            return false;
          }
          return true;
        } catch {
          this.sendError("HTTP_FAILED");
          return false;
        }
      }
      case "page.switch": {
        if (!a.page) {
          this.sendError("INVALID_ACTION");
          return false;
        }
        await this.switchPage(a.page);
        return true;
      }
      default: {
        this.sendError("INVALID_ACTION", { message: `Unknown action: ${a.type}` });
        return false;
      }
    }
  }

  /**
   * 現在ページの同座標ボタンの label を更新
   */
  private updateButtonLabelAtCurrentPage(x: number, y: number, label: string) {
    if (!this.config || !this.currentPageKey) return;
    const page = this.config.pages[this.currentPageKey];
    if (!page) return;
    const btn = page.buttons.find((b) => b.x === x && b.y === y);
    if (btn) btn.label = label;
  }

  /**
   * ページ更新通知
   */
  private pushPageUpdate() {
    if (!this.config || !this.currentPageKey) return;
    const page = this.config.pages[this.currentPageKey];
    if (!page) return;

    const buttons = page.buttons.map((b) => ({ x: b.x, y: b.y, label: b.label }));
    this.send({
      type: "page.update",
      payload: { currentPage: this.currentPageKey, buttons },
    });
  }

  /**
   * エラー送信
   */
  private sendError(code: string, extra?: { [k: string]: any }) {
    const payload: any = { code, ...(extra || {}) };
    this.send({ type: "error", payload });
    this.logger?.warn?.("error:", payload);
  }

  /**
   * メッセージ送信
   */
  private send(msg: any) {
    const b = this.broadcaster;
    try {
      if (!b) return;
      if (typeof b === "function") {
        b(msg);
      } else if (typeof (b as any).send === "function") {
        (b as any).send(msg);
      }
    } catch (e: any) {
      this.logger?.warn?.("send failed:", e?.message ?? String(e));
    }
  }
}

export default ControlCore;
