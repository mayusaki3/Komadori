[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../../index.md) > [パネルシステムの単体テスト・結合テスト方針とケース定義](00_test_policy.md) > Control Core 単体テストケース定義

# Control Core 単体テストケース定義

## 1. 目的

panel.json に基づく制御ロジックと、メッセージ入出力の正当性を検証する。

## 2. 前提

- テスト対象: Control Core モジュール
- 依存先はモック化する:
  - OBS WebSocket クライアント
  - 外部 HTTP クライアント
  - Dock / Stream Deck 向け送信インターフェース

## 3. テストケース一覧（抜粋）

### TC-01: 正常ロード

- 条件:
  - 有効な panel.json（version=1, pages/main 定義あり）
- 手順:
  - Control Core 起動
- 確認:
  - エラーなしで起動完了
  - 現在ページが `main` に設定される
  - Dock/Stream Deck 向けに `page.update` が1回送信される

### TC-02: 重複座標エラー

- 条件:
  - 同一 (x,y) を持つボタンが存在する panel.json
- 手順:
  - Control Core 起動
- 確認:
  - 起動エラー扱い（ログ出力）
  - 外部への `page.update` を送信しない

### TC-03: 未定義ページへの page.switch

- 条件:
  - `page.switch` で存在しないページ名を指定
- 手順:
  - `button.click` を送信
- 確認:
  - OBS/HTTP 呼び出しは行わない
  - `error` メッセージを返す（code=`INVALID_PAGE`）

### TC-04: OBS シーン切替アクション

- 条件:
  - main ページ (0,0) に `obs.setScene: SceneA` を定義
- 手順:
  - `button.click` (page=main, x=0, y=0) を送信
- 確認:
  - OBS WebSocket クライアントに `SetCurrentProgramScene(SceneA)` が1回呼ばれる

### TC-05: 配信/録画トグル

- 条件:
  - LIVE ボタン: `obs.toggleStream`
  - REC ボタン: `obs.toggleRecord`
- 手順:
  - 各ボタンに対する `button.click` を送信
- 確認:
  - 対応する OBS API 呼び出しが行われる
  - 結果に応じて `status.update` が送信される（モック応答で検証）

### TC-06: HTTP 呼び出し＋表示

- 条件:
  - `http.get` アクション + `display: "status"` 定義
- 手順:
  - 対象ボタン `button.click`
  - モック HTTP クライアントが `{ "status": "OK" }` を返す
- 確認:
  - HTTP GET が1回呼び出される
  - `page.update` などで該当ボタンラベルが `"OK"` に更新される

### TC-07: ページ切替同期

- 条件:
  - main→util へ切替する `page.switch` ボタンあり
- 手順:
  - `button.click` でページ切替
- 確認:
  - Control Core の現在ページが `util`
  - Dock/Stream Deck 両方に `page.update` が送られる（送信先モックで件数確認）

### TC-08: 不明 action.type

- 条件:
  - `action.type = "unknown.action"`
- 手順:
  - 対象ボタン `button.click`
- 確認:
  - 外部呼び出しを行わない
  - `error` を返す（code=`INVALID_ACTION`）

## 4. 実装上の注意

- 単体テストは `/OBS/panel_system/tests/` 配下に配置し、本ドキュメントの TestID と1対1対応させる。
- 新規アクション

---
[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../../index.md) > [パネルシステムの単体テスト・結合テスト方針とケース定義](00_test_policy.md) > Control Core 単体テストケース定義
