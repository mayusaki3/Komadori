import { createPanelServer } from "./server.js";

const server = createPanelServer({ port: 7345 });
server.start();
