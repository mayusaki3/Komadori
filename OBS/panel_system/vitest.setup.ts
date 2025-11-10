import { afterEach, vi } from "vitest";

/**
 * 共通テスト初期化:
 * - モックのクリア
 * - 必要に応じて環境変数などをセット
 */

afterEach(() => {
  vi.clearAllMocks();
  vi.restoreAllMocks();
});
