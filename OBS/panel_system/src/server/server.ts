import { WebSocketServer, WebSocket } from "ws";
import type { RawData } from "ws";
import { fileURLToPath } from "node:url";
import {
  ControlCore,
  type ControlCoreOptions,
  type ClientBroadcaster,
  type ObsController,
  type HttpClient,
} from "../control-core.js";

export type PanelServerOptions = {
  port?: number;
  /**
   * config/panel.json のパスを外から差し替えたい場合に指定。
   * 未指定なら既定の ../../config/panel.json を使う。
   */
  configPath?: string;

  /**
   * 実運用では実OBS/実HTTPに差し替えるためのフック。
   * 未指定なら console.log ベースのダミーを使用。
   */
  obs?: ObsController;
  http?: HttpClient;
};

export type PanelServerHandle = {
  port: number;
  start: () => void;
  stop: () => Promise<void>;
  core: ControlCore;
};

export function createPanelServer(opts: PanelServerOptions = {}): PanelServerHandle {
  const port = opts.port ?? 7345;

  const clients = new Set<WebSocket>();

  // ControlCore が期待する broadcaster 形態（関数型）でブロードキャストする
  const broadcaster: ClientBroadcaster = (msg: any) => {
    const data = JSON.stringify(msg);
    for (const ws of clients) {
      if (ws.readyState === WebSocket.OPEN) {
        ws.send(data);
      }
    }
  };

  const obs: ObsController =
    opts.obs ??
    ({
      async setScene(scene) {
        console.log("[OBS] setScene:", scene);
      },
      async toggleStream() {
        console.log("[OBS] toggleStream");
      },
      async toggleRecord() {
        console.log("[OBS] toggleRecord");
      },
      async toggleMute(source) {
        console.log("[OBS] toggleMute:", source);
      },
      async setSourceVisibility(scene, source, visible) {
        console.log("[OBS] setSourceVisibility:", scene, source, visible);
      },
      async getStatus() {
        return { streaming: false, recording: false };
      },
    } satisfies ObsController);

  const http: HttpClient =
    opts.http ??
    ({
      async get(url) {
        console.log("[HTTP] GET:", url);
        return {};
      },
      async post(url, body) {
        console.log("[HTTP] POST:", url, body);
        return {};
      },
    } satisfies HttpClient);

  const configPath =
    opts.configPath ??
    fileURLToPath(new URL("../../config/panel.json", import.meta.url));

  const coreOptions: ControlCoreOptions = {
    configPath,
    broadcaster,
    obs,
    http,
  };

  const core = new ControlCore(coreOptions);
  core.initialize();

  const wss = new WebSocketServer({ port });

  wss.on("connection", (ws: WebSocket) => {
    clients.add(ws);

    // 新規接続時に現状ページを送る（broadcaster形態に依存せずUI初期化できる）
    core.pushPageUpdate?.();

    ws.on("close", () => {
      clients.delete(ws);
    });

    ws.on("message", async (data: RawData) => {
      try {
        const msg = JSON.parse(String(data));
        // 将来拡張（page.switch / http.get など）も含めて全部コアに渡す
        await core.handleMessage?.(msg);
      } catch (e) {
        console.warn("Invalid message:", e);
      }
    });
  });

  const start = () => {
    // ws は生成時点で listen 済み。ここではログのみ。
    console.log(`[panel_system] WebSocket server listening on ws://127.0.0.1:${port}`);
  };

  const stop = async () => {
    // ws close はコールバック型なので Promise 化
    await new Promise<void>((resolve) => {
      wss.close(() => resolve());
    });

    for (const ws of clients) {
      try {
        ws.close();
      } catch {
        // ignore
      }
    }
    clients.clear();
  };

  return { port, start, stop, core };
}
