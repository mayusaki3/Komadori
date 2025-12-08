// src/control-core.ts
// 役割:
//   OBS 操作・HTTP 呼び出し・ページ/ボタン定義を統合し、
//   クライアントへ page.update / error を送信するコアロジック。
// 注意点（テスト仕様準拠）:
//   - initialize() 正常完了時に 1 回 page.update を送る（ただし broadcaster 形態によって分岐; TC-01, 28, 32）
//   - コンフィグ不正時は initialize() が例外を投げる（TC-02, 14–16, 24–27）
//   - currentPageKey が不正ページを指していても起動は成功する（TC-30）
//   - currentPageKey 不正時の button.click は INVALID_ACTION（TC-30）
//   - ページ文脈不一致は INVALID_PAGE_CONTEXT（TC-03, 34）
//   - 未初期化での button.click / switchPage は NOT_INITIALIZED（TC-20, 29）
//   - 未定義ページへの switchPage は INVALID_PAGE（TC-21）
//   - HTTP displayKey 反映や 'N/A' 更新などをサポート（TC-06, 23, 36, 37）
//   - ボタン配列や座標・ラベル/アクション有無などを厳密に検証する（TC-02, 15–17, 24–27）
//   - 未知アクションは INVALID_ACTION を送っても残りは継続実行（TC-08, 38）
//   - broadcaster は複数形態に対応する（send(type,payload) / fn(msg) / { broadcaster: fn }）

import * as fs from "node:fs";

// ==== 型定義 ====

// パネル構成ファイルの生の形
export type RawButtonConfig = {
  x: number;
  y: number;
  label?: string;
  action?: any; // ボタンアクション（単体 or 配列）
  image?: string;
};

export type RawPageConfig = {
  name?: string;
  buttons?: RawButtonConfig[];
};

export type PanelConfig = {
  version: unknown;
  pages?: Record<string, RawPageConfig>;
  currentPage?: string;
  currentPageKey?: string;
};

// 正規化後に内部で扱うボタン
export type Button = {
  x: number;
  y: number;
  label: string;
  action: ButtonAction;
  /**
   * オプションのボタン画像。
   * main ページなどのコンフィグで固定画像を指定する用途。
   */
  image?: string;
};

// broadcaster は 3 パターンを許容
export type ClientBroadcaster =
  | ((msg: any) => void)
  | { send: (type: string, payload: any) => void }
  | { broadcaster: (msg: any) => void };

// OBS 側 I/F（テストの ObsController に合わせて定義）
export type ObsController = {
  setScene?: (scene: string) => Promise<void> | void;
  toggleStreaming?: () => Promise<void> | void;
  toggleRecording?: () => Promise<void> | void;
  // 旧名も許容（TC-22 の mock）
  toggleStream?: () => Promise<void> | void;
  toggleRecord?: () => Promise<void> | void;
  toggleMute?: (source: string) => Promise<void> | void;
  setSourceVisibility?: (
    scene: string,
    source: string,
    visible: boolean,
  ) => Promise<void> | void;
  getStatus?: () => Promise<any>;
};

// HTTP クライアント I/F（テストの HttpClient に合わせる）
export type HttpClient = {
  get?: (url: string) => Promise<any>;
  post?: (url: string, body?: any) => Promise<any>;
};

// ロガー
export type LoggerLike = {
  debug?: (...a: any[]) => void;
  info?: (...a: any[]) => void;
  warn?: (...a: any[]) => void;
  error?: (...a: any[]) => void;
};

// ControlCore 生成オプション
export type ControlCoreOptions = {
  obs: ObsController;
  http: HttpClient;
  broadcaster: ClientBroadcaster;
  logger?: LoggerLike;
  configPath?: string;
  // 将来の差し替え用に直接 config を渡すことも許可（テストでは未使用）
  config?: PanelConfig;
};

// button.click メッセージ型（テストで使用）
export type ButtonClickMessage = {
  type: "button.click";
  payload: {
    page: string;
    x: number;
    y: number;
    source?: string;
  };
};

export class ControlCore {
  private obs: ObsController;
  private http: HttpClient;
  private broadcaster: ClientBroadcaster;
  private logger: LoggerLike;
  private configPath?: string;

  private config?: PanelConfig;
  private initialized = false;

  // テストから直接書き換えるケースがあるので public（TC-32）
  public currentPageKey?: string;

  // ページごとの表示上書き: pageKey -> "x,y" -> label
  private displayValues = new Map<string, Map<string, string>>();

  constructor(opts: ControlCoreOptions) {
    this.obs = opts.obs ?? {};
    this.http = opts.http ?? {};
    this.broadcaster = opts.broadcaster;
    this.logger = opts.logger ?? {
      debug: () => {},
      info: () => {},
      warn: () => {},
      error: () => {},
    };
    this.configPath = opts.configPath;
    this.config = opts.config;
  }

  // ==== 初期化 & コンフィグ処理 ====

  /**
   * コンフィグを読み込み・検証して初期化する。
   * - 正常時: currentPageKey をセットし、必要に応じ 1 回 page.update を送る
   *   - broadcaster が { send(type,payload) } の場合は常に初回 page.update（TC-01, 28 他）
   *   - 関数/ { broadcaster: fn } の場合は:
   *       - main ページが存在しない構成のみ初回 page.update（TC-28）
   *       - main ページが存在する構成では送らない（TC-32）
   * - 異常時: 例外を投げる（TC-02, 14–16, 24–27）
   */
  public initialize(): void {
    const raw = this.loadConfig();
    const normalized = this.validateAndNormalizeConfig(raw); // ここで不正なら throw

    this.config = normalized;

    // currentPageKey の決定ロジック:
    //   1) raw.currentPageKey が文字列ならそのまま（存在確認はしない）(TC-30)
    //   2) raw.currentPage が文字列ならそれを使う
    //   3) pages.main があれば "main"
    //   4) それ以外は先頭のキー（TC-28）
    const pages = normalized.pages!;
    if (typeof raw.currentPageKey === "string") {
      this.currentPageKey = raw.currentPageKey;
    } else if (typeof raw.currentPage === "string") {
      this.currentPageKey = raw.currentPage;
    } else if (Object.prototype.hasOwnProperty.call(pages, "main")) {
      this.currentPageKey = "main";
    } else {
      const keys = Object.keys(pages);
      this.currentPageKey = keys[0];
    }

    this.initialized = true;

    // 初期 page.update は broadcaster が存在する場合は必ず 1 回送る
    if (this.broadcaster) {
      this.pushPageUpdate();
    }
  }

  /**
   * コンフィグファイル読込（configPath が無い/空で initialize された場合はエラー扱い）
   */
  private loadConfig(): PanelConfig {
    if (this.config) {
      return this.config;
    }
    if (!this.configPath) {
      throw new Error("configPath is not set");
    }
    const json = fs.readFileSync(this.configPath, "utf8");
    const parsed = JSON.parse(json) as PanelConfig;
    return parsed;
  }

  /**
   * コンフィグ検証 + 正規化。
   * - version: number かつ 1 固定（TC-14, 24）
   * - pages: オブジェクト & 1 ページ以上（TC-15, 25）
   * - 各ページ: buttons 配列必須（TC-26）
   * - 各ボタン:
   *    - x, y: number かつ 0 <= x,y < 16 程度の範囲（TC-16）
   *    - page === "main" のボタンのみ label と action 必須（TC-27）
   *    - 同一ページ内で (x,y) 重複禁止（TC-02）
   */
  private validateAndNormalizeConfig(raw: PanelConfig): PanelConfig {
    // version
    if (typeof raw.version !== "number") {
      throw new Error("version must be a number");
    }
    if (raw.version !== 1) {
      // テストは version=999 を不正とみなす
      throw new Error("unsupported version");
    }

    // pages
    if (!raw.pages || typeof raw.pages !== "object") {
      throw new Error("pages is required");
    }
    const pageKeys = Object.keys(raw.pages);
    if (pageKeys.length === 0) {
      throw new Error("pages must not be empty");
    }

    // 各ページの検証
    for (const key of pageKeys) {
      const page = raw.pages[key];
      if (!page) {
        throw new Error(`page ${key} is invalid`);
      }
      if (!Array.isArray(page.buttons)) {
        throw new Error(`page ${key} has no buttons`);
      }

      const seenCoords = new Set<string>();

      for (const btn of page.buttons) {
        if (typeof btn?.x !== "number" || typeof btn?.y !== "number") {
          throw new Error(`page ${key} has invalid button coordinates`);
        }
        // 座標範囲（ざっくり 0〜15 程度。TC-16 の x=99 は確実に弾く）
        if (
          btn.x < 0 ||
          btn.y < 0 ||
          !Number.isFinite(btn.x) ||
          !Number.isFinite(btn.y) ||
          btn.x >= 16 ||
          btn.y >= 16
        ) {
          throw new Error(`page ${key} has out-of-range button coordinate`);
        }

        const coordKey = `${btn.x},${btn.y}`;
        if (seenCoords.has(coordKey)) {
          // TC-02
          throw new Error(`page ${key} has duplicate button coordinates`);
        }
        seenCoords.add(coordKey);

        // main ページだけは label + action 必須（TC-27）
        if (key === "main") {
          if (!btn.label || btn.action == null) {
            throw new Error(`page ${key} button requires label and action`);
          }
        }
      }
    }

    return raw;
  }

  // ==== 内部ヘルパ ====

  /**
   * broadcaster への送信を 3 パターンに対応させる:
   *   1) { send: (type, payload) => ... }  (createMocks)
   *   2) (msg) => ...                     (mkDeps)
   *   3) { broadcaster: (msg) => ... }    (TC-13)
   */
  private emit(type: string, payload: any): void {
    const b = this.broadcaster;
    try {
      if (typeof b === "function") {
        // fn({ type, payload })
        b({ type, payload });
      } else if (b && typeof (b as any).send === "function") {
        // send(type, payload)
        (b as any).send(type, payload);
      } else if (b && typeof (b as any).broadcaster === "function") {
        // broadcaster({ type, payload })
        (b as any).broadcaster({ type, payload });
      } else {
        // 何もしない（想定外形態）
      }
    } catch (e) {
      this.logger.error?.("emit failed", e);
    }
  }

  private getPage(key?: string): { key: string; buttons: Button[] } | undefined {
    if (!key || !this.config || !this.config.pages) return undefined;
    const rawPage = this.config.pages[key];
    if (!rawPage || !Array.isArray(rawPage.buttons)) return undefined;

    // initialize 時に検証しているのでここでは基本そのまま返す
    const buttons: Button[] = rawPage.buttons.map((b) => ({
      x: b.x,
      y: b.y,
      label: b.label ?? "",
      action: b.action,
      image: (b as any).image,
    }));
    return { key, buttons };
  }

  private getButton(pageKey: string, x: number, y: number): Button | undefined {
    const page = this.getPage(pageKey);
    if (!page) return undefined;
    return page.buttons.find((b) => b.x === x && b.y === y);
  }

  private getCurrentPage(): { key: string; buttons: Button[] } | undefined {
    return this.getPage(this.currentPageKey);
  }

  private setDisplayLabel(pageKey: string, x: number, y: number, label: string): void {
    const map = this.displayValues.get(pageKey) ?? new Map<string, string>();
    map.set(`${x},${y}`, label);
    this.displayValues.set(pageKey, map);
  }

  private sendError(code: string, message?: string): void {
    this.emit("error", { code, message });
  }

  // ==== 状態送信 ====

  /**
   * 現在ページの状態を page.update として送信する。
   * - 未初期化 or config 不在 → 何もしない（TC-31）
   * - currentPageKey が不正でページが見つからない → 何もしない（TC-32）
   */
  public pushPageUpdate(): void {
    if (!this.initialized || !this.config || !this.config.pages) return;

    const page = this.getCurrentPage();
    if (!page) {
      // currentPageKey 不正（TC-32）
      return;
    }

    const overrides = this.displayValues.get(page.key) ?? new Map<string, string>();
    const buttons = page.buttons.map((b) => {
      const k = `${b.x},${b.y}`;
      const label = overrides.get(k) ?? b.label;
      // image はコンフィグ固定の値をそのまま載せる。
      // undefined の場合はプロパティ自体を付けない（Dock UI 側の dataset.image は未定義になる）。
      return {
        x: b.x,
        y: b.y,
        label,
        ...(b.image ? { image: b.image } : {}),
      };
    });

    this.emit("page.update", {
      currentPage: page.key,
      buttons,
    });
  }

  // ==== メッセージ入口 ====

  /**
   * button.click 用のショートカット（テスト互換用）
   */
  public async handleButtonClick(msg: ButtonClickMessage): Promise<void> {
    await this.handleMessage(msg);
  }

  /**
   * メインのメッセージ入口
   */
  public async handleMessage(msg: any): Promise<void> {
    const type = msg?.type;
    const payload = msg?.payload ?? {};

    try {
      switch (type) {
        case "button.click":
          await this.onButtonClick(payload);
          return;
        case "page.switch":
          await this.onPageSwitch(payload);
          return;
        case "http.get":
          await this.onHttpGet(payload);
          return;
        case "http.post":
          await this.onHttpPost(payload);
          return;
        default:
          // 未知メッセージ種別
          this.sendError("INVALID_ACTION", `unknown message type: ${type}`);
          return;
      }
    } catch (e: any) {
      this.logger.error?.("handleMessage failed", e);
      this.sendError("UNEXPECTED_ERROR", e?.message ?? String(e));
    }
  }

  // ==== 個別ハンドラ ====

  private async onButtonClick(payload: any): Promise<void> {
    if (!this.initialized) {
      // TC-29
      this.sendError("NOT_INITIALIZED");
      return;
    }

    const reqPage: string | undefined = payload?.page;
    const x: number | undefined = payload?.x;
    const y: number | undefined = payload?.y;

    const cur = this.getCurrentPage();
    if (!cur) {
      // currentPageKey が存在しないページを指しているケース（TC-30）
      this.sendError("INVALID_ACTION", "currentPageKey is invalid");
      return;
    }

    if (!reqPage || cur.key !== reqPage) {
      // ページ文脈不一致（TC-03, 34）
      this.sendError("INVALID_PAGE_CONTEXT");
      return;
    }

    const btn = cur.buttons.find((b) => b.x === x && b.y === y);
    if (!btn) {
      // ボタン未定義座標（TC-09）
      this.sendError("NO_BUTTON");
      return;
    }

    const action = btn.action;
    const actions: any[] = Array.isArray(action) ? action : [action];

    for (const a of actions) {
      const at: string | undefined = a?.type;
      if (!at) {
        this.sendError("INVALID_ACTION", "action.type missing");
        continue;
      }

      // 名前空間付き / なし両対応
      switch (at) {
        // OBS: setScene
        case "obs.setScene":
        case "setScene":
          await this.obs.setScene?.(a.scene);
          break;

        // OBS: toggleStreaming
        case "obs.toggleStreaming":
        case "toggleStreaming":
        case "obs.toggleStream":
        case "toggleStream":
          // toggleStreaming 優先、無ければ旧名 toggleStream
          if (this.obs.toggleStreaming) {
            await this.obs.toggleStreaming();
          } else if (this.obs.toggleStream) {
            await this.obs.toggleStream();
          }
          break;

        // OBS: toggleRecording
        case "obs.toggleRecording":
        case "toggleRecording":
        case "obs.toggleRecord":
        case "toggleRecord":
          if (this.obs.toggleRecording) {
            await this.obs.toggleRecording();
          } else if (this.obs.toggleRecord) {
            await this.obs.toggleRecord();
          }
          break;

        // OBS: toggleMute
        case "obs.toggleMute":
        case "toggleMute":
          await this.obs.toggleMute?.(a.source);
          break;

        // OBS: setSourceVisibility
        case "obs.setSourceVisibility":
        case "setSourceVisibility":
          await this.obs.setSourceVisibility?.(
            a.scene,
            a.source,
            !!a.visible,
          );
          break;

        // ページ切替
        case "page.switch":
          await this.switchPage(a.page);
          break;

        // HTTP: get / post（ボタン経由で displayKey 反映あり）
        case "http.get":
          await this.execHttpAndReflectLabel("get", a, x, y);
          break;
        case "http.post":
          await this.execHttpAndReflectLabel("post", a, x, y);
          break;

        default:
          // 未知アクション: INVALID_ACTION を送りつつ継続（TC-08, 38）
          this.sendError("INVALID_ACTION", `unknown action type: ${at}`);
          break;
      }
    }

    // HTTP による label 更新などを反映するため、常に更新を送る（TC-06, 23, 36, 37）
    this.pushPageUpdate();
  }

  private async onPageSwitch(payload: any): Promise<void> {
    const page = payload?.page;
    if (!page) {
      // page 指定なしは INVALID_ACTION（TC-17）
      this.sendError("INVALID_ACTION", "page missing");
      return;
    }
    await this.switchPage(page);
  }

  private async onHttpGet(payload: any): Promise<void> {
    const url = payload?.url;
    if (!url) {
      // TC-18
      this.sendError("INVALID_ACTION", "url missing");
      return;
    }
    try {
      const res = await this.http.get?.(url);
      // ここではラベル更新はせず、成功/失敗のみ扱う。
      // 失敗条件は「例外が投げられた場合」のみとする（TC-13 は reject 経路）。
      void res;
    } catch (e: any) {
      this.sendError("HTTP_FAILED", e?.message ?? String(e));
    }
  }

  private async onHttpPost(payload: any): Promise<void> {
    const url = payload?.url;
    if (!url) {
      // TC-19
      this.sendError("INVALID_ACTION", "url missing");
      return;
    }
    try {
      const res = await this.http.post?.(url, payload?.body);
      void res;
    } catch (e: any) {
      this.sendError("HTTP_FAILED", e?.message ?? String(e));
    }
  }

  /**
   * ページ切替ロジック
   * - 未初期化: NOT_INITIALIZED（TC-20）
   * - pageKey 未指定: INVALID_ACTION（TC-17）
   * - 同一ページ指定: ノーオペ（TC-33）
   * - 未定義ページ: INVALID_PAGE（TC-21）
   * - 正常時: currentPageKey 更新 + page.update 送信
   */
  private async switchPage(pageKey?: string): Promise<void> {
    if (!this.initialized) {
      this.sendError("NOT_INITIALIZED");
      return;
    }
    if (!pageKey) {
      this.sendError("INVALID_ACTION", "page missing");
      return;
    }
    if (this.currentPageKey === pageKey) {
      // 同一ページ指定は何もしない（TC-33）
      return;
    }
    const next = this.getPage(pageKey);
    if (!next) {
      this.sendError("INVALID_PAGE");
      return;
    }

    this.currentPageKey = pageKey;
    this.pushPageUpdate();
  }

  /**
   * OBS のステータスを取得して UI 更新するフック。
   * - getStatus() が reject → OBS_STATUS_FAILED（TC-22）
   * - getStatus() が undefined/null → OBS_STATUS_FAILED（TC-68）
   * - 成功時は page.update を送る（詳細な反映は今は行わない）
   */
  public async updateStatusFromObs(): Promise<void> {
    try {
      if (!this.obs.getStatus) {
        throw new Error("getStatus not implemented");
      }
      const status = await this.obs.getStatus();

      // 戻り値が空なら失敗扱い（TC-68）
      if (status === undefined || status === null) {
        this.sendError("OBS_STATUS_FAILED", "getStatus returned no data");
        return;
      }

      this.pushPageUpdate();
    } catch (e: any) {
      this.sendError("OBS_STATUS_FAILED", e?.message ?? String(e));
    }
  }

  /**
   * HTTP 実行 → displayKey に従ってボタン label を更新。
   * - method: "get" | "post"
   * - action.url が無ければ INVALID_ACTION（TC-18, 19）
   * - displayKey:
   *    - res.data[displayKey] 優先
   *    - なければ res[displayKey]
   *    - 両方無ければ "N/A"（TC-37）
   * - displayKey 未指定なら label は変更しない（TC-23）
   * - page.switch 後に http.get を打つ場合、新しい currentPageKey に対して更新（TC-36）
   * - HTTP_FAILED は「Promise が reject した場合のみ」（TC-12, 13）
   * - res.data が undefined の場合は label を更新しない（TC-69）
   * - res 自体が undefined/null の場合も label を更新しない（TC-70）
   */
  private async execHttpAndReflectLabel(
    method: "get" | "post",
    action: any,
    x: number,
    y: number,
  ): Promise<void> {
    const url: string | undefined = action?.url;
    const displayKey: string | undefined = action?.displayKey;

    if (!url) {
      this.sendError("INVALID_ACTION", "url missing");
      return;
    }

    let res: any;
    try {
      const client = this.http;
      res =
        method === "get"
          ? await client.get?.(url)
          : await client.post?.(url, action?.body);
    } catch (e: any) {
      this.sendError("HTTP_FAILED", e?.message ?? String(e));
      return;
    }

    // displayKey 未指定なら label 更新は行わない（TC-23）
    if (!displayKey) {
      return;
    }

    // res が undefined/null の場合は何もしない（TC-70）
    if (res === undefined || res === null) {
      return;
    }

    const hasDataProp =
      typeof res === "object" && Object.prototype.hasOwnProperty.call(res, "data");
    const data = (res as any).data;

    // data プロパティが存在していて、中身が undefined/null の場合は
    // 「data が undefined」ケースとして label を更新しない（TC-69）
    if (hasDataProp && (data === undefined || data === null)) {
      return;
    }

    let value: string;

    if (
      hasDataProp &&
      data &&
      typeof data === "object" &&
      displayKey in (data as any)
    ) {
      // data 側に displayKey がある場合はそちらを優先（TC-56）
      value = String((data as any)[displayKey] ?? "");
    } else if (!hasDataProp && typeof res === "object" && displayKey in (res as any)) {
      // data プロパティが無い場合は res 本体から参照（TC-06, 48）
      value = String((res as any)[displayKey] ?? "");
    } else {
      // data がオブジェクトだが displayKey 無し → "N/A"（TC-37）
      // または data プロパティが無く res 本体にも displayKey 無し → "N/A"
      value = "N/A";
    }

    const pageKey = this.currentPageKey;
    if (pageKey) {
      this.setDisplayLabel(pageKey, x, y, value);
    }
  }
}
