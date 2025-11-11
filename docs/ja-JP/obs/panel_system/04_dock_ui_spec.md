[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../index.md) > [パネルシステム概要](00_overview.md) > Dock UI 仕様

# Dock UI 仕様

## 1. 目的

OBS の Custom Browser Docks 上に、panel_system の定義と同期した 3x5 パネルを表示する。
Control Core と同一プロトコルで通信し、Stream Deck プラグインと機能・表示を揃える。

## 2. 通信仕様

- 接続先: Control Core が提供する WebSocket
  - 例: `ws://127.0.0.1:7345`
- 受信メッセージ:
  - `page.update` : ページとボタン一覧。UIを再描画。
  - `status.update` : 必要に応じてボタン状態に反映（配信ON/OFFなど）。
  - `error` : コンソール出力のみ。
- 送信メッセージ:
  - `button.click`
    - `payload.page` : 現在ページ名
    - `payload.x`, `payload.y` : ボタン座標
    - `payload.source` : `"dock"`

## 3. レイアウト

- グリッド: 5列 × 3行
  - x: 0〜4
  - y: 0〜2
- `page.update.payload.buttons` の各要素に対応するセルへボタンを生成。
- `state` が `"on"` の場合は強調表示（背景色変更など）。

## 4. 非機能要件

- Dock UI はロジック最小限とし、全ての定義と状態は Control Core に委譲する。
- 接続断時は「DISCONNECTED」を表示し、自動再接続を行う。
- 外部環境への直接アクセスは禁止。常に Control Core 経由とする。

---
[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../index.md) > [パネルシステム概要](00_overview.md) > Dock UI 仕様

