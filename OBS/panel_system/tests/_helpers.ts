// tests/_helpers.ts
import { vi } from "vitest";
import fs from "node:fs";
import os from "node:os";
import path from "node:path";

export function createMocks() {
  const sent: any[] = [];
  const broadcaster = {
    send: vi.fn((msg: any) => { sent.push(msg); })
  };
  const obs = {
    setScene: vi.fn(),
    toggleStreaming: vi.fn(),
    toggleRecording: vi.fn(),
    toggleMute: vi.fn(),
    setSourceVisibility: vi.fn(),
    getStatus: vi.fn().mockResolvedValue({ streaming: false, recording: false })
  };
  const http = {
    get: vi.fn(),
    post: vi.fn()
  };
  const logger = {
    info: vi.fn(),
    warn: vi.fn(),
    error: vi.fn(),
    debug: vi.fn()
  };
  return { broadcaster, obs, http, logger, sent };
}

const TMP_DIR = fs.mkdtempSync(path.join(os.tmpdir(), "komadori-tests-"));
export function writeTempConfig(fileName: string, json: any) {
  const p = path.join(TMP_DIR, fileName);
  fs.writeFileSync(p, JSON.stringify(json, null, 2), "utf-8");
  return p;
}

// mkDeps: createMocks と同一形状を返すユーティリティ（spec内の両表記に対応）
export const mkDeps = createMocks;

export function createLoggerMock() {
  return {
    info: vi.fn(),
    warn: vi.fn(),
    error: vi.fn(),
    debug: vi.fn(),
  };
}
