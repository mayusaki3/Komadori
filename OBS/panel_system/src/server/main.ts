import { WebSocketServer } from "ws";
import type { WebSocket, RawData } from "ws";
import { fileURLToPath } from "node:url";
import {
  ControlCore,
  ControlCoreOptions,
  ClientBroadcaster,
  ObsController,
  HttpClient,
  ButtonClickMessage,
} from "../control-core.js";

const PORT = 7345;

const clients = new Set<WebSocket>();

const broadcaster: ClientBroadcaster = {
  broadcast(msg) {
    const data = JSON.stringify(msg);
    for (const ws of clients) {
      if (ws.readyState === ws.OPEN) {
        ws.send(data);
      }
    }
  },
};

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
    return { streaming: false, recording: false };
  },
};

const httpClient: HttpClient = {
  async get(url) {
    console.log("[HTTP] GET:", url);
    return {};
  },
  async post(url, body) {
    console.log("[HTTP] POST:", url, body);
    return {};
  },
};

const configPath = fileURLToPath(
  new URL("../../config/panel.json", import.meta.url),
);

const options: ControlCoreOptions = {
  configPath,
  broadcaster,
  obs,
  http: httpClient,
};

const core = new ControlCore(options);
core.initialize();

const wss = new WebSocketServer({ port: PORT });

wss.on("connection", (ws: WebSocket) => {
  clients.add(ws);

  ws.on("close", () => {
    clients.delete(ws);
  });

  ws.on("message", async (data: RawData) => {
    try {
      const msg = JSON.parse(String(data));
      if (msg.type === "button.click") {
        await core.handleButtonClick(msg as ButtonClickMessage);
      }
    } catch (e) {
      console.warn("Invalid message:", e);
    }
  });
});

console.log(
  `[panel_system] WebSocket server listening on ws://127.0.0.1:${PORT}`,
);
