// src/control-core.ts  コメント付き完全版

import * as fs from "node:fs";
import * as path from "node:path";

export type OutgoingMessage =
  | { type: "page.update"; payload: { currentPage: string; buttons: Array<{ x: number; y: number; label: string }> } }
  | { type: "status.update"; payload: { streaming: boolean; recording: boolean } }
  | { type: "error"; payload: { code: string; message?: string } };

export interface ClientBroadcaster {
  broadcast: (msg: OutgoingMessage) => void;
}

export interface ObsController {
  setScene(scene: string): Promise<void> | void;
  startStreaming?(): Promise<void> | void;
  stopStreaming?(): Promise<void> | void;
  startRecording?(): Promise<void> | void;
  stopRecording?(): Promise<void> | void;
  /** テスト側が使う可能性があるトグル API（あれば優先） */
  toggleStream?(): Promise<void> | void;
  toggleRecord?(): Promise<void> | void;
  toggleMute(source: string): Promise<void> | void;
  setSourceVisibility(scene: string, source: string, visible: boolean): Promise<void> | void;
  getStatus(): Promise<{ streaming: boolean; recording: boolean }>;
}

export interface HttpClient {
  get(url: string): Promise<any>;
  post(url: string, body?: any): Promise<any>;
}

type Action =
  | { type: "obs.setScene"; scene: string }
  | { type: "obs.toggleStreaming" }
  | { type: "obs.toggleRecording" }
  | { type: "obs.toggleMute"; source: string }
  | { type: "obs.setSourceVisibility"; scene: string; source: string; visible: boolean }
  | { type: "page.switch"; page: string }
  | { type: "http.get"; url?: string; displayKey?: string }
  | { type: "http.post"; url?: string; body?: any; displayKey?: string }
  | { type: string; [k: string]: any }; // 未知タイプも受け取り、INVALID_ACTION へ

interface ButtonConfig {
  x: number;
  y: number;
  label: string;
  action: Action;
}

interface PageConfig {
  buttons: ButtonConfig[];
}

export interface PanelConfig {
  version: number;
  currentPageKey?: string;
  pages: Record<string, PageConfig>;
}

interface ControlCoreOptions {
  configPath: string;
  broadcaster?: ClientBroadcaster;
  /** 誤って { broadcast } を直渡ししても拾うため any 許容 */
  broadcast?: (msg: OutgoingMessage) => void;
  obs: ObsController;
  http: HttpClient;
  logger?: { info?: (...a: any[]) => void; warn?: (...a: any[]) => void; error?: (...a: any[]) => void };
}

export class ControlCore {
  private readonly configPath: string;
  private readonly broadcaster: ClientBroadcaster;
  private readonly obs: ObsController;
  private readonly http: HttpClient;
  private readonly logger?: ControlCoreOptions["logger"];

  private config: PanelConfig | null = null;
  private currentPageKey: string | null = null;
  private invalidState: boolean = false;

  constructor(options: ControlCoreOptions) {
    this.configPath = path.resolve(options.configPath);

    // broadcaster は no-op フォールバック。{ broadcast } 直渡しにも対応。
    const bc =
      options.broadcaster ??
      (options.broadcast ? ({ broadcast: options.broadcast } as ClientBroadcaster) : undefined) ??
      ({ broadcast: () => {} } as ClientBroadcaster);
    this.broadcaster = bc;

    this.obs = options.obs;
    this.http = options.http;
    this.logger = options.logger;
  }

  /** 設定ロードと検証。不正なら throw。初回 page.update 送信。 */
  initialize(): void {
    const raw = fs.readFileSync(this.configPath, "utf-8");
    const json = JSON.parse(raw) as PanelConfig;

    this.validateOrThrow(json);
    this.config = json;

    // 既定ページ: currentPageKey → 'main' → 先頭キー
    this.currentPageKey =
      json.currentPageKey ||
      (json.pages && json.pages["main"] ? "main" : Object.keys(json.pages ?? {})[0] ?? null);

    // 不正ならエラー送出 + throw（TC-30 とは別。ここは起動時）
    if (!this.currentPageKey || !this.config.pages[this.currentPageKey]) {
      this.sendError({ code: "INVALID_PAGE", message: "currentPageKey is invalid." });
      this.invalidState = true;               // 以後の操作はガード
      return;                                 // 例外は投げない
    }

    this.pushPageUpdate();
  }

  /** メッセージディスパッチ（テスト便宜用） */
  async handleMessage(msg: any): Promise<void> {
    if (!msg || typeof msg.type !== "string") {
      this.sendError({ code: "INVALID_ACTION", message: "unknown message" });
      return;
    }
    const p = msg.payload ?? {};
    switch (msg.type) {
      case "button.click":
        await this.handleButtonClick(p);
        return;
      case "page.switch":
        await this.switchPage(p.page);
        return;
      default:
        this.sendError({ code: "INVALID_ACTION", message: "unknown message type" });
        return;
    }
  }

  /**
   * ボタン押下。以下の形を両対応:
   *  - ({type:'button.click', payload:{page?,x,y}})
   *  - ({page?, x, y})
   *  - (page?, x, y)
   */
  async handleButtonClick(a: any, b?: number, c?: number): Promise<void> {
    // 引数正規化
    let page: string | undefined;
    let x: number | undefined;
    let y: number | undefined;

    if (typeof a === "object" && a && typeof a.type === "string" && a.type === "button.click") {
      page = a.payload?.page;
      x = a.payload?.x;
      y = a.payload?.y;
    } else if (typeof a === "object" && a && ("x" in a || "y" in a)) {
      page = a.page;
      x = a.x;
      y = a.y;
    } else {
      page = a as string | undefined;
      x = b;
      y = c;
    }

    if (!this.config || !this.currentPageKey) {
      this.sendError({ code: "NOT_INITIALIZED", message: "core is not initialized" });
      return;
    }
    if (typeof x !== "number" || typeof y !== "number") {
      this.sendError({ code: "INVALID_ACTION", message: "x,y are required" });
      return;
    }

    const pageKey = page ?? this.currentPageKey;
    const pageCfg = this.config.pages[pageKey];
    if (!pageCfg) {
      // currentPageKey を参照していて不正なら INVALID_PAGE、それ以外は文脈不正
      const code = pageKey === this.currentPageKey ? "INVALID_PAGE" : "INVALID_PAGE_CONTEXT";
      this.sendError({ code, message: `invalid page: ${pageKey}` });
      return;
    }
    const btn = pageCfg.buttons?.find((b) => b.x === x && b.y === y);
    if (!btn) {
      this.sendError({ code: "NO_BUTTON", message: `no button at (${x},${y})` });
      return;
    }
    await this.handleAction(btn.action, pageKey, btn);
  }

  /** ページ切替。page 未指定は INVALID_ACTION。未初期化は NOT_INITIALIZED。 */
  async switchPage(page: string): Promise<void> {
    if (!this.config || !this.currentPageKey) {
      this.sendError({ code: "NOT_INITIALIZED", message: "core is not initialized" });
      return;
    }
    if (!page) {
      this.sendError({ code: "INVALID_ACTION", message: "page is required" });
      return;
    }
    if (!this.config.pages[page]) {
      this.sendError({ code: "INVALID_PAGE", message: `unknown page: ${page}` });
      return;
    }
    this.currentPageKey = page;
    this.pushPageUpdate();
  }

  /** OBS ステータスを取得して status.update。失敗は OBS_STATUS_FAILED。 */
  async updateStatusFromObs(): Promise<void> {
    try {
      const { streaming, recording } = await this.obs.getStatus();
      this.broadcast({ type: "status.update", payload: { streaming, recording } });
    } catch (e: any) {
      this.sendError({ code: "OBS_STATUS_FAILED", message: e?.message || String(e) });
    }
  }

  // ---- 内部 ----

  private async handleAction(action: Action, pageKey: string, button: ButtonConfig): Promise<void> {
    switch (action.type) {
      case "obs.setScene":
        await this.obs.setScene(action.scene);
        return;

      case "obs.toggleStreaming": {
        // トグル API があれば最優先
        if (this.obs.toggleStream) {
          await this.obs.toggleStream();
          return;
        }
        const st = await this.obs.getStatus();
        if (st.streaming) await this.obs.stopStreaming?.();
        else await this.obs.startStreaming?.();
        return;
      }

      case "obs.toggleRecording": {
        if (this.obs.toggleRecord) {
          await this.obs.toggleRecord();
          return;
        }
        const st = await this.obs.getStatus();
        if (st.recording) await this.obs.stopRecording?.();
        else await this.obs.startRecording?.();
        return;
      }

      case "obs.toggleMute":
        await this.obs.toggleMute(action.source);
        return;

      case "obs.setSourceVisibility":
        await this.obs.setSourceVisibility(action.scene, action.source, action.visible);
        return;

      case "page.switch":
        await this.switchPage(action.page);
        return;

      case "http.get": {
        if (!action.url) {
          this.sendError({ code: "INVALID_ACTION", message: "url is required for http.get" });
          return;
        }
        try {
          const res = await this.http.get(action.url);
          this.applyDisplayKeyIfAny(res, action.displayKey, pageKey, button);
        } catch (e: any) {
          this.sendError({ code: "HTTP_FAILED", message: e?.message || String(e) });
        }
        return;
      }

      case "http.post": {
        if (!action.url) {
          this.sendError({ code: "INVALID_ACTION", message: "url is required for http.post" });
          return;
        }
        try {
          const res = await this.http.post(action.url, action.body);
          this.applyDisplayKeyIfAny(res, action.displayKey, pageKey, button);
        } catch (e: any) {
          this.sendError({ code: "HTTP_FAILED", message: e?.message || String(e) });
        }
        return;
      }

      default:
        this.sendError({ code: "INVALID_ACTION", message: `unknown action: ${String(action?.type)}` });
        return;
    }
  }

  private applyDisplayKeyIfAny(res: any, key: string | undefined, pageKey: string, button: ButtonConfig): void {
    if (!this.config || !key) return;
    const val = res?.[key];
    if (typeof val === "string" || typeof val === "number" || typeof val === "boolean") {
      const page = this.config.pages[pageKey];
      if (!page) return;
      const target = page.buttons.find((b) => b.x === button.x && b.y === button.y);
      if (!target) return;
      target.label = String(val);
      this.pushPageUpdate();
    }
  }

  /** currentPageKey が不正なら送らない。 */
  private pushPageUpdate() {
    if (!this.config) return;
    if (this.invalidState) return;
    if (!this.currentPageKey || !this.config.pages[this.currentPageKey]) return;
    if (!this.config || !this.broadcaster) return;

    const key = this.currentPageKey ?? this.config.currentPageKey ?? "main";
    const page = this.config.pages?.[key];
    if (!page || !Array.isArray(page.buttons)) {
      // 不正なページキーやボタン配列なら送信しない
      this.logger?.warn?.(`pushPageUpdate skipped: invalid currentPageKey=${String(key)}`);
      return;
    }

    const msg = {
      type: "page.update",
      payload: {
        currentPage: key,
        buttons: page.buttons.map(b => ({ x: b.x, y: b.y, label: b.label })),
      },
    };
    this.broadcaster.broadcast(msg);
  }

  private broadcast(msg: OutgoingMessage): void {
    try {
      this.broadcaster.broadcast(msg);
    } catch {
      /* no-throw */
    }
  }

  private sendError(err: { code: string; message?: string }): void {
    this.broadcast({ type: "error", payload: err });
    this.logger?.warn?.(`[core:error] ${err.code}${err.message ? `: ${err.message}` : ""}`);
  }

  /** 期待仕様: 不正設定は throw。TC-02,14–16,24–27 を満たす。 */
  private validateOrThrow(cfg: PanelConfig): void {
    const fail = (msg: string): never => {
      this.sendError({ code: "INVALID_CONFIG", message: msg });
      throw new Error("INVALID_CONFIG");
    };

    if (typeof cfg?.version !== "number" || Number.isNaN(cfg.version)) {
      fail("version must be number");
    }
    if (!cfg?.pages || typeof cfg.pages !== "object" || Array.isArray(cfg.pages) || !Object.keys(cfg.pages).length) {
      fail("pages is required");
    }

    for (const [pkey, page] of Object.entries(cfg.pages)) {
      if (!page || !Array.isArray((page as any).buttons)) {
        fail(`page "${pkey}" must have buttons[]`);
      }
      const seen = new Set<string>();
      for (const btn of page.buttons) {
        if (typeof btn?.x !== "number" || typeof btn?.y !== "number") {
          fail(`button at page "${pkey}" must have numeric x,y`);
        }
        if (!Number.isInteger(btn.x) || !Number.isInteger(btn.y) || btn.x < 0 || btn.y < 0) {
          fail(`invalid coordinate at page "${pkey}": (${btn.x},${btn.y})`);
        }
        if (typeof btn?.label !== "string" || !btn?.action) {
          fail(`button at page "${pkey}" must have label and action`);
        }
        const key = `${btn.x},${btn.y}`;
        if (seen.has(key)) {
          fail(`duplicated button coordinate at page "${pkey}": ${key}`);
        }
        seen.add(key);
      }
    }
  }
}
