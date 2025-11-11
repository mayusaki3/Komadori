/**
 * WebSocket サーバ実装
 *
 * 役割:
 * - Dock UI / Stream Deck プラグインからの接続を受け付ける
 * - ControlCore への button.click 委譲
 * - ControlCore からの broadcast をクライアントに配信
 *
 * 本実装は最小構成。OBS / HTTP はダミー実装としている。
 */

import { WebSocketServer, WebSocket } from "ws";
import {
  ControlCore,
  ControlCoreOptions,
  ClientBroadcaster,
  ObsController,
  HttpClient,
  ButtonClickMessage
} from "../control-core.js"; // tsc 後のパスに合わせる場合は要調整

const PORT = 7345;

// クライアント管理
const clients = new Set<WebSocket>();

const broadcaster: ClientBroadcaster = {
  broadcast(msg) {
    const data = JSON.stringify(msg);
    for (const ws of clients) {
      if (ws.readyState === WebSocket.OPEN) {
        ws.send(data);
      }
    }
  }
};

// 暫定 OBS 実装（後で本実装に差し替える）
const obs: ObsController = {
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
    // 将来は OBS WebSocket から取得
    return { streaming: false, recording: false };
  }
};

// 暫定 HTTP 実装
const httpClient: HttpClient = {
  async get(url) {
    console.log("[HTTP] GET:", url);
    return {};
  },
  async post(url, body) {
    console.log("[HTTP] POST:", url, body);
    return {};
  }
};

const options: ControlCoreOptions = {
  configPath: new URL("../../config/panel.json", import.meta.url).pathname,
  broadcaster,
  obs,
  http: httpClient
};

const core = new ControlCore(options);
core.initialize();

const wss = new WebSocketServer({ port: PORT });

wss.on("connection", (ws) => {
  clients.add(ws);

  ws.on("close", () => {
    clients.delete(ws);
  });

  ws.on("message", async (data) => {
    try {
      const msg = JSON.parse(String(data));

      if (msg.type === "button.click") {
        await core.handleButtonClick(msg as ButtonClickMessage);
      }
      // 他のメッセージ種別は将来拡張
    } catch (e) {
      console.warn("Invalid message:", e);
    }
  });
});

console.log(`[panel_system] WebSocket server listening on ws://127.0.0.1:${PORT}`);
