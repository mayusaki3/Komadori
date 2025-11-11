"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var vitest_1 = require("vitest");
/**
 * 共通テスト初期化処理。
 * - 各テストケース後にモック状態をリセットする。
 */
(0, vitest_1.afterEach)(function () {
    vitest_1.vi.clearAllMocks();
    vitest_1.vi.restoreAllMocks();
});
