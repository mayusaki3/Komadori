[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../../index.md) > パネルシステムの単体テスト・結合テスト方針とケース定義

# パネルシステムの単体テスト・結合テスト方針とケース定義

## 1. 目的

panel_system の JSON 駆動制御が、仕様通りに動作することを自動テストで保証する。

## 2. テスト対象レイヤ

1. [Control Core 単体テストケース定義](01_test_cases_core.md)
   - JSON パース／バリデーション
   - ページ状態管理
   - アクション→OBS/WebSocket/HTTP 呼び出しへのマッピング

2. OBS Dock UI
   - 表示ロジック（受信データ→ボタン表示）
   - イベント送出（押下→Control Core 通知）

3. Stream Deck プラグイン
   - イベント連携（押下→Control Core）
   - 表示連携（Control Core→Key Title/Icon）

## 3. テスト戦略

- Control Core:
  - 単体テスト: モック OBS/WebSocket/HTTP を使用。
  - 入力: panel.json, ボタン押下イベント
  - 出力: 発行されるコマンド列が期待通りかを検証。

- Dock UI:
  - 単体テスト: ヘッドレスブラウザまたは DOM テストで、
    - 指定ページ定義から正しい座標・ラベルが描画されること。
    - クリックイベントが Control Core 用のメッセージ形式で送出されること。

- Stream Deck プラグイン:
  - 単体テスト相当: SDK 提供のシミュレータまたはモックで、
    - Control Core からの更新指示に対し setTitle 等が正しく呼ばれること。
    - 押下時に規定メッセージが送信されること。

## 4. 実装配置（推奨）

- 実コードのテストは `/OBS/panel_system/tests/` に配置。
- 本ディレクトリ（docs/ja-JP/obs/tests/panel_system/）はテスト仕様書のみを管理する。

---
[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../../index.md) > パネルシステムの単体テスト・結合テスト方針とケース定義
