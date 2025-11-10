import { afterEach, vi } from "vitest";

/**
 * 共通テスト初期化処理。
 * - 各テストケース後にモック状態をリセットする。
 */
afterEach(() => {
  vi.clearAllMocks();
  vi.restoreAllMocks();
});
