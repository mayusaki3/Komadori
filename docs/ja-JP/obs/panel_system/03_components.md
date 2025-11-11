[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../index.md) > [パネルシステム概要](00_overview.md) > 実装コンポーネント仕様（要約）

# 実装コンポーネント仕様（要約）

## 1. Control Core

- 起動時に panel.json を読込・検証。
- OBS WebSocket への接続管理。
- Dock UI / Stream Deck プラグインとの WebSocket/HTTP 通信。
- すべてのボタン押下イベントの入口となる。

## 2. OBS Dock UI

- Control Core から受信したページ・ボタン定義を描画。
- ボタン押下で Control Core にイベント通知。
- DOM 側にはロジックを持たず、薄いクライアントとする。

## 3. Stream Deck プラグイン

- キー押下イベントを Control Core にフォワード。
- Control Core からの更新通知で Key Title/Icon を更新。
- 設定は panel.json に寄せ、プラグイン側には持たない。

---
[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../index.md) > [パネルシステム概要](00_overview.md) > 実装コンポーネント仕様（要約）
