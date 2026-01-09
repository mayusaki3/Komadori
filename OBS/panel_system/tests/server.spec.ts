import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";

/**
 * hoisted: vi.mock より前に初期化される必要がある共有状態
 */
const hoisted = vi.hoisted(() => {
  return {
    initializeSpy: vi.fn(),
    handleButtonClickSpy: vi.fn(),

    // ws server instances
    wssInstances: [] as any[],

    // 最後に new ControlCore(opts) された opts を拾えるようにする（必要なら）
    lastCoreOptions: undefined as any,
  };
});

/**
 * ws の WebSocketServer を差し替え
 */
vi.mock("ws", () => {
  class FakeWebSocketServer {
    static instances: FakeWebSocketServer[] = [];

    public port: number;
    private handlers: Record<string, Function | undefined> = {};

    constructor(opts: { port: number }) {
      this.port = opts.port;
      FakeWebSocketServer.instances.push(this);
      hoisted.wssInstances.push(this);
    }

    on(event: "connection", cb: (ws: any) => void) {
      this.handlers[event] = cb;
    }

    emitConnection(ws: any) {
      const cb = this.handlers["connection"];
      if (cb) cb(ws);
    }
  }

  // main/server.ts 側は named import しているので named export で返す
  return {
    WebSocketServer: FakeWebSocketServer,
  };
});

/**
 * ../control-core.js を差し替え
 * ※ server.ts 側が `../control-core.js` から複数の named export を import しているため、
 *   それらを「全部」export する（型だけでも import が value なら実体が必要）。
 */
vi.mock("../src/control-core.js", () => {
  class ControlCore {
    constructor(opts: any) {
      hoisted.lastCoreOptions = opts;
    }

    initialize() {
      hoisted.initializeSpy();
    }

    async handleButtonClick(msg: any) {
      hoisted.handleButtonClickSpy(msg);
    }
  }

  // server.ts が import している named export を揃える（未使用でも必要）
  return {
    ControlCore,

    // 以降は server.ts 側の named import を満たすためのダミー
    ControlCoreOptions: {},
    ClientBroadcaster: {},
    ObsController: {},
    HttpClient: {},
    ButtonClickMessage: {},
  };
});

class FakeWsClient {
  public OPEN = 1;
  public readyState = 1;

  public send = vi.fn();

  private handlers: Record<string, Function | undefined> = {};

  on(event: "close" | "message", cb: Function) {
    this.handlers[event] = cb;
  }

  emitClose() {
    const cb = this.handlers["close"];
    if (cb) cb();
  }

  async emitMessage(data: any) {
    const cb = this.handlers["message"];
    if (cb) {
      // server.ts 側は (data: RawData) で受けて String(data) する想定
      await cb(data);
    }
  }
}

describe("Server bootstrap (tests/server.spec.ts)", () => {
  const originalLog = console.log;

  beforeEach(async () => {
    // spy リセット
    hoisted.initializeSpy.mockClear();
    hoisted.handleButtonClickSpy.mockClear();
    hoisted.lastCoreOptions = undefined;

    // ws instances リセット（FakeWebSocketServer.instances は vi.mock 側の static なので、import 後に参照する）
    hoisted.wssInstances.length = 0;

    // main.ts が import 時に起動するので、毎回 module cache を切る
    vi.resetModules();

    // noisy log を抑制（必要なら）
    console.log = vi.fn() as any;
  });

  afterEach(() => {
    console.log = originalLog;
    vi.restoreAllMocks();
  });

  it("PORT=7345 で WebSocketServer を生成し、ControlCore.initialize を呼ぶ", async () => {
    await import("../src/server/main");

    // WebSocketServer が 1 回生成されていること
    expect(hoisted.wssInstances.length).toBe(1);
    expect(hoisted.wssInstances[0].port).toBe(7345);

    // ControlCore.initialize が呼ばれていること
    expect(hoisted.initializeSpy).toHaveBeenCalledTimes(1);
  });

  it("受信 msg.type===button.click の場合のみ core.handleButtonClick を呼ぶ", async () => {
    await import("../src/server/main");

    const wss = hoisted.wssInstances[0];
    const ws = new FakeWsClient();

    // 接続
    wss.emitConnection(ws);

    // button.click -> forward
    await ws.emitMessage(
      JSON.stringify({ type: "button.click", payload: { page: "main", x: 1, y: 2 } }),
    );
    expect(hoisted.handleButtonClickSpy).toHaveBeenCalledTimes(1);

    // button.click 以外 -> 無視
    await ws.emitMessage(JSON.stringify({ type: "page.switch", payload: { page: "util" } }));
    expect(hoisted.handleButtonClickSpy).toHaveBeenCalledTimes(1);
  });

  it("不正 JSON は warn して落ちない", async () => {
    const warnSpy = vi.spyOn(console, "warn").mockImplementation(() => {});

    await import("../src/server/main");

    const wss = hoisted.wssInstances[0];
    const ws = new FakeWsClient();

    wss.emitConnection(ws);

    await ws.emitMessage("{broken json");

    expect(warnSpy).toHaveBeenCalled();
    expect(hoisted.handleButtonClickSpy).toHaveBeenCalledTimes(0);
  });
});
