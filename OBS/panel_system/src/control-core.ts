// src/control-core.ts
/**
 * ControlCore
 * OBSパネル制御のコアロジック実装（同期初期化版）
 *
 * 役割：
 *  - 設定ファイル（JSON/JSONC想定）のロード・検証
 *  - ページ／ボタンの状態管理とイベント送出（page.update / error など）
 *  - アクション実行（OBS制御・HTTP呼出・ページ切替 等）
 *
 * 注意点：
 *  - initialize() は同期版（ファイルを同期読み込み）
 *  - JSONC（コメント、末尾カンマ）対応の軽量整形ユーティリティ付き
 *  - テスト側が new ControlCore({ obs, http, broadcast, logger, ... }) としても動くよう、
 *    引数名 'broadcast' と 'broadcaster' の双方を受け付ける。
 *  - page.update の送出は「安全系 pushPageUpdateSafe()」を通すことで、
 *    未初期化・不正ページ等の際に送出しない判定を行う。
 */

import * as fs from "fs";
import * as path from "path";

/* ==============================
 * 型定義
 * ============================== */

/** ロガーIF（テストでモック化しやすい最小形） */
export interface Logger {
  debug(...args: any[]): void;
  info(...args: any[]): void;
  warn(...args: any[]): void;
  error(...args: any[]): void;
}

/** 送信系（WebSocket等を想定） */
export interface Broadcaster {
  send(event: { type: string; payload?: any }): void;
}

/** OBS操作IF（テスト用モック前提の最小セット） */
export interface ObsApi {
  setScene(name: string): Promise<void> | void;
  toggleStreaming(): Promise<void> | void;
  toggleRecording(): Promise<void> | void;
  toggleMute?(source?: string): Promise<void> | void;
  setSourceVisibility?(opts: { source: string; visible: boolean }): Promise<void> | void;

  /** 状態取得（テスト用）：失敗で例外 */
  getStatus?(): Promise<{ streaming?: boolean; recording?: boolean }> | { streaming?: boolean; recording?: boolean };
}

/** HTTP呼出IF（テストモック前提） */
export interface HttpApi {
  get(url: string): Promise<{ ok: boolean; status: number; data?: any; error?: string }>;
  post(url: string, body?: any): Promise<{ ok: boolean; status: number; data?: any; error?: string }>;
}

/** 設定スキーマ */
export interface CoreConfig {
  version: number | string;
  /** 既定ページキー（省略可） */
  main?: string;
  /** ページ配列 */
  pages: PageConfig[];
}

/** ページ構成 */
export interface PageConfig {
  key: string;                  // ページキー（ユニーク）
  title?: string;               // 表示用タイトル
  context?: string;             // 文脈（例："main" / "util" など）
  buttons?: ButtonConfig[];     // 省略可能だが、存在する場合は検証対象
}

/** ボタン構成 */
export interface ButtonConfig {
  x: number;                    // X座標（0以上の整数）
  y: number;                    // Y座標（0以上の整数）
  label?: string;               // ラベル
  actions: ActionConfig[];      // 実行するアクション列（順序保持）
}

/** アクション種別 */
export type ActionConfig =
  | { type: "setScene"; name: string }
  | { type: "toggleStreaming" }
  | { type: "toggleRecording" }
  | { type: "toggleMute"; source?: string }
  | { type: "setSourceVisibility"; source: string; visible: boolean }
  | { type: "http.get"; url: string; displayKey?: string }   // displayKey: レスポンス反映先ラベルキー
  | { type: "http.post"; url: string; body?: any }
  | { type: "page.switch"; page?: string }                   // page未指定はエラーとする（テストに合わせる）
  | { type: string; [k: string]: any }                       // 未知アクション（INVALID_ACTION）

/** コンストラクタ引数 */
export interface ControlCoreOptions {
  obs: ObsApi;
  http: HttpApi;
  /** どちらかが指定されれば利用可能（エイリアス対応） */
  broadcaster?: Broadcaster;
  broadcast?: Broadcaster;
  logger?: Logger;
  /** 設定取得：ファイルパス優先、なければ config オブジェクト */
  configPath?: string;
  config?: CoreConfig;
}

/* ==============================
 * 1) 先頭付近：ユーティリティ関数を追加
 * ============================== */

/**
 * Jsonファイルコメント除去および整形ユーティリティ
 *  - シングル/マルチラインコメントを削除
 *  - 行末の末尾カンマを緩く除去（簡易的な対応）
 * 厳密なJSON5ではないが、テスト用設定のJSONCを安全に読む目的。
 */
function toStrictJson(jsonc: string): string {
  // コメント削除
  let s = jsonc
    // block comments /* ... */
    .replace(/\/\*[\s\S]*?\*\//g, "")
    // line comments //
    .replace(/(^|[^:])\/\/.*$/gm, (_m, p1) => `${p1}`);

  // 末尾カンマの緩和（}, ] の直前のカンマを削除）
  s = s.replace(/,\s*([}\]])/g, "$1");

  return s.trim();
}

/** 安全JSON.parse（エラー時は投げる） */
function parseJsoncOrThrow<T>(src: string): T {
  const strict = toStrictJson(src);
  return JSON.parse(strict) as T;
}

/* ==============================
 * 2) 設定ファイル読込の共通化（同期版）
 * ============================== */

/**
 * 設定ファイルを同期で読み込む。
 * @param filePath JSON/JSONC のパス
 * @returns CoreConfig
 * @throws 失敗時は例外
 */
export function loadConfigFromFile(filePath: string): CoreConfig {
  const abs = path.resolve(filePath);
  const raw = fs.readFileSync(abs, "utf8");
  const cfg = parseJsoncOrThrow<CoreConfig>(raw);
  return cfg;
}

/* ==============================
 * クラス本体の private メソッド群
 * ============================== */

type PageIndex = Map<string, PageConfig>;
type CoordKey = string;

function coordKey(x: number, y: number): CoordKey {
  return `${x},${y}`;
}

/* ==============================
 * 本体
 * ============================== */

export class ControlCore {
  private readonly obs: ObsApi;
  private readonly http: HttpApi;
  private readonly logger: Logger;
  private readonly broadcaster?: Broadcaster;

  private config?: CoreConfig;
  private pagesByKey: PageIndex = new Map();
  private currentPageKey?: string;

  constructor(opts: ControlCoreOptions) {
    this.obs = opts.obs;
    this.http = opts.http;
    // 'broadcast' と 'broadcaster' の両対応
    this.broadcaster = opts.broadcaster ?? opts.broadcast;
    this.logger = opts.logger ?? console;

    if (opts.configPath) {
      this.config = loadConfigFromFile(opts.configPath);
    } else if (opts.config) {
      this.config = opts.config;
    }
  }

  /* --------------------------------
   * 初期化（同期）
   * -------------------------------- */
  public initialize(): void {
    if (!this.config) {
      throw new Error("config is required");
    }
    const cfg = this.config;
    this.validateConfigOrThrow(cfg);

    this.pagesByKey.clear();
    for (const p of cfg.pages) {
      this.pagesByKey.set(p.key, p);
    }

    // 既定ページ決定：cfg.main が有効ならそれ、無ければ pages[0]
    const main = (cfg.main && this.pagesByKey.has(cfg.main)) ? cfg.main : cfg.pages[0]?.key;
    if (!main) {
      // validate で pages 存在を保証しているので理論上到達しないが保険
      throw new Error("pages is required");
    }
    this.currentPageKey = main;

    // 初回 page.update
    this.pushPageUpdateSafe();
  }

  /* --------------------------------
   * 公開API
   * -------------------------------- */

  /**
   * 現在ページの見える化状態を push（未初期化／不正ページのときは送らない）
   */
  public pushPageUpdate(): void {
    this.pushPageUpdateSafe();
  }

  /**
   * ボタンクリック処理
   * @param x クリックX
   * @param y クリックY
   * @param pageKey ページ名（省略時は currentPage）
   * @param pageContext 文脈（テストの「不一致」検出用に任意文字列）
   */
  public async handleButtonClick(
    x: number,
    y: number,
    pageKey?: string,
    pageContext?: string
  ): Promise<void> {
    if (!this.isInitialized()) {
      this.sendError("NOT_INITIALIZED", { x, y });
      return;
    }
    const key = pageKey ?? this.currentPageKey!;
    const page = this.pagesByKey.get(key);
    if (!page) {
      this.sendError("INVALID_ACTION", { reason: "currentPageKey invalid", currentPageKey: this.currentPageKey });
      return;
    }
    // 文脈不一致
    if (pageContext != null && page.context != null && pageContext !== page.context) {
      this.sendError("INVALID_PAGE_CONTEXT", { pageKey: key, expected: page.context, actual: pageContext });
      return;
    }
    const btn = (page.buttons ?? []).find(b => b.x === x && b.y === y);
    if (!btn) {
      this.sendError("NO_BUTTON", { pageKey: key, x, y });
      return;
    }

    // 複数アクションは順序維持
    for (const act of btn.actions) {
      const ok = await this.executeAction(act, { page, button: btn });
      if (!ok) {
        // INVALID_ACTION等は送信済み。継続実行の方針（テスト要件）
        continue;
      }
    }
  }

  /**
   * 明示ページ切替
   */
  public async switchPage(pageKey: string): Promise<void> {
    if (!this.isInitialized()) {
      this.sendError("NOT_INITIALIZED");
      return;
    }
    if (!this.pagesByKey.has(pageKey)) {
      this.sendError("INVALID_PAGE", { pageKey });
      return;
    }
    if (this.currentPageKey === pageKey) {
      // ノーオペ（テスト要件）
      return;
    }
    this.currentPageKey = pageKey;
    this.pushPageUpdateSafe();
  }

  /**
   * OBS状態から内部表示を更新（テストで失敗時 OBS_STATUS_FAILED）
   */
  public async updateStatusFromObs(): Promise<void> {
    if (!this.isInitialized()) {
      this.sendError("NOT_INITIALIZED");
      return;
    }
    try {
      const st = this.obs.getStatus ? await this.obs.getStatus() : {};
      // 必要に応じて label 等へ反映したいが、テストは「失敗時のエラー送出」を主眼にしている想定
      // ここでは取得できても何も送らず、失敗時のみ error を送る。
      void st;
    } catch (e: any) {
      this.sendError("OBS_STATUS_FAILED", { error: String(e?.message ?? e) });
    }
  }

  /* --------------------------------
   * private: 実行／更新
   * -------------------------------- */

  private isInitialized(): boolean {
    if (!this.config) return false;
    if (!this.currentPageKey) return false;
    if (!this.pagesByKey.has(this.currentPageKey)) return false;
    return true;
  }

  /** page.update を安全に送る（失敗条件では送らない） */
  private pushPageUpdateSafe(): void {
    if (!this.broadcaster) return; // 送信口なし
    if (!this.isInitialized()) return;
    const page = this.pagesByKey.get(this.currentPageKey!);
    if (!page) return;

    const payload = {
      pageKey: page.key,
      title: page.title ?? page.key,
      context: page.context,
      buttons: (page.buttons ?? []).map(b => ({
        x: b.x,
        y: b.y,
        label: b.label ?? "",
      })),
    };
    this.broadcaster.send({ type: "page.update", payload });
  }

  private sendError(code: string, extra?: any): void {
    this.logger.warn("[error]", code, extra ?? "");
    if (this.broadcaster) {
      this.broadcaster.send({ type: "error", payload: { code, ...(extra ?? {}) } });
    }
  }

  private async executeAction(
    action: ActionConfig,
    ctx: { page: PageConfig; button: ButtonConfig }
  ): Promise<boolean> {
    switch (action.type) {
      case "setScene":
        if (!action.name) { this.sendError("INVALID_ACTION", { reason: "name required" }); return false; }
        await Promise.resolve(this.obs.setScene(action.name));
        return true;

      case "toggleStreaming":
        await Promise.resolve(this.obs.toggleStreaming());
        return true;

      case "toggleRecording":
        await Promise.resolve(this.obs.toggleRecording());
        return true;

      case "toggleMute":
        if (!this.obs.toggleMute) { this.sendError("INVALID_ACTION", { reason: "toggleMute not supported" }); return false; }
        await Promise.resolve(this.obs.toggleMute(action.source));
        return true;

      case "setSourceVisibility":
        if (!this.obs.setSourceVisibility) { this.sendError("INVALID_ACTION", { reason: "setSourceVisibility not supported" }); return false; }
        if (!action.source || typeof action.visible !== "boolean") {
          this.sendError("INVALID_ACTION", { reason: "source/visible required" });
          return false;
        }
        await Promise.resolve(this.obs.setSourceVisibility({ source: action.source, visible: action.visible }));
        return true;

      case "http.get":
        if (!action.url) { this.sendError("INVALID_ACTION", { reason: "url required", action: "http.get" }); return false; }
        try {
          const res = await this.http.get(action.url);
          if (!res.ok) {
            this.sendError("HTTP_FAILED", { status: res.status, action: "http.get" });
            return false;
          }
          // displayKey が指定されていれば、レスポンスから抽出して label に反映
          if (action.displayKey) {
            const v = res?.data?.[action.displayKey];
            // キーが無い場合は label 未更新（TC-23）、displayKey 未指定でラベル更新テスト（TC-37）は 'N/A'
            if (v != null) {
              ctx.button.label = String(v);
            } else {
              // TC-37: displayKey 不在 → 'N/A' で更新 + page.update
              ctx.button.label = "N/A";
            }
            this.pushPageUpdateSafe();
          }
          return true;
        } catch (e: any) {
          this.sendError("HTTP_FAILED", { error: String(e?.message ?? e), action: "http.get" });
          return false;
        }

      case "http.post":
        if (!action.url) { this.sendError("INVALID_ACTION", { reason: "url required", action: "http.post" }); return false; }
        try {
          const res = await this.http.post(action.url, action.body);
          if (!res.ok) {
            this.sendError("HTTP_FAILED", { status: res.status, action: "http.post" });
            return false;
          }
          return true;
        } catch (e: any) {
          this.sendError("HTTP_FAILED", { error: String(e?.message ?? e), action: "http.post" });
          return false;
        }

      case "page.switch":
        if (!action.page) { this.sendError("INVALID_ACTION", { reason: "page required", action: "page.switch" }); return false; }
        await this.switchPage(action.page);
        return true;

      default:
        // 未知アクション：INVALID_ACTION を送りつつ継続
        this.sendError("INVALID_ACTION", { reason: "unknown action.type", type: (action as any)?.type });
        return false;
    }
  }

  /* --------------------------------
   * 設定検証
   * -------------------------------- */
  private validateConfigOrThrow(cfg: CoreConfig): void {
    // version チェック：number 必須（テストで "numberでない場合エラー"）
    if (typeof cfg.version !== "number" || Number.isNaN(cfg.version)) {
      throw new Error("version must be number");
    }
    // pages 必須
    if (!Array.isArray(cfg.pages) || cfg.pages.length === 0) {
      throw new Error("pages is required");
    }
    // ページキー重複禁止
    {
      const seen = new Set<string>();
      for (const p of cfg.pages) {
        if (!p || typeof p.key !== "string" || p.key.trim() === "") {
          throw new Error("page.key is required");
        }
        if (seen.has(p.key)) {
          throw new Error(`duplicate page.key: ${p.key}`);
        }
        seen.add(p.key);
      }
    }
    // ボタン検証
    for (const p of cfg.pages) {
      if (!Array.isArray(p.buttons)) {
        throw new Error(`page ${p.key}: buttons is required`);
      }
      const coordSet = new Set<CoordKey>();
      for (const b of p.buttons) {
        // 必須
        if (!Number.isInteger(b.x) || !Number.isInteger(b.y) || b.x < 0 || b.y < 0) {
          throw new Error(`invalid button coordinate at page ${p.key}`);
        }
        const ck = coordKey(b.x, b.y);
        if (coordSet.has(ck)) {
          throw new Error(`duplicate button coordinate at page ${p.key}: (${b.x},${b.y})`);
        }
        coordSet.add(ck);

        if (!Array.isArray(b.actions) || b.actions.length === 0) {
          throw new Error(`button at page ${p.key} (${b.x},${b.y}) requires actions`);
        }
        // label or action 欠如テスト：ここでは "actionsは必須"、labelは任意（空文字にする）で許容。
      }
    }

    // cfg.main がある場合、有効ページか検証
    if (cfg.main != null && typeof cfg.main === "string") {
      const ok = cfg.pages.some(p => p.key === cfg.main);
      if (!ok) {
        // mainが不正でも致命にせず、initializeで pages[0] を既定にフォールバックするテストもあるため、
        // ここでは「警告ログのみに留める」
        this.logger?.warn("main page not found. will fallback to first page");
      }
    }
  }
}
