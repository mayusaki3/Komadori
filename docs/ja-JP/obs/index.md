[目次](../目次.md) > OBS 関連ドキュメント インデックス

# OBS 関連ドキュメント インデックス

本ディレクトリは、OBS を用いた配信・録画運用と、その拡張機能に関する仕様と手順を管理する。

## 1. 共通

- OBS 本体のバージョン前提
- OBS WebSocket 利用方針
- ディレクトリ構成と命名ルール
- 外部ツール（Stream Deck 等）との連携ポリシー

## 2. モジュール別ドキュメント

### 2.1 パネルシステム（panel_system）

共通 JSON 定義に基づき、以下を同期制御する仕組み。

- OBS の Custom Browser Dock 上の仮想パネル
- Stream Deck（3x5）上の物理ボタン
- OBS WebSocket / 外部 HTTP API 呼び出し

詳細:
- [パネルシステム概要](panel_system/00_overview.md)
- [パネルシステム機能仕様](panel_system/01_spec.md)
- [panel.json 仕様](panel_system/02_json_format.md)
- [実装コンポーネント仕様（要約）](panel_system/03_components.md)

### 2.2 テスト関連

- `tests/panel_system/`:
  - パネルシステムの単体テスト・結合テスト方針とケース定義

## 3. 更新ルール

- 実装変更時は、対応する仕様・テストドキュメントを同一ブランチで更新する。
- JSON 仕様変更時は、バージョンと互換性影響を `panel_system/02_json_format.md` に明記する。

---
[目次](../目次.md) > OBS 関連ドキュメント インデックス
