[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../index.md) > パネルシステム機能仕様

# パネルシステム機能仕様

## 1. 構成要素

1. Control Core
   - ローカル常駐プロセス（Node.js または Python）。
   - 役割:
     - panel.json の読込と検証
     - 現在ページ・ボタン状態の管理
     - OBS WebSocket との接続・コマンド送出
     - 外部 HTTP/API 呼び出し
     - Dock UI / Stream Deck プラグインとの双方向通信

2. OBS Dock UI
   - Browser Dock で表示する Web ページ。
   - Control Core と WebSocket 通信。
   - 3x5 ボタンレイアウトを描画し、押下イベントを Control Core に通知。
   - Control Core からの状態更新に応じてラベルやスタイルを更新。

3. Stream Deck プラグイン
   - Elgato Stream Deck SDK を利用したカスタムプラグイン。
   - キー押下を Control Core に転送。
   - Control Core から受信したボタン定義で Key Title / Icon を更新。

## 2. 機能要件

### F1: ページ管理

- 対応:
  - 固定長 3x5 (x:0-4, y:0-2) のボタン座標。
  - 「ページ」単位でボタン定義を保持。
- 要件:
  - 任意ボタンに `page.switch` アクションを割当可能。
  - ページ変更時、Dock UI と Stream Deck の表示が同期更新される。

### F2: OBS 制御アクション

- サポート種別（例）:
  - `obs.setScene`
  - `obs.toggleStream`
  - `obs.toggleRecord`
  - `obs.toggleMute`
  - `obs.setSourceVisibility`
- Control Core は JSON 定義に従い OBS WebSocket v5 の Request を送信。

### F3: 外部 I/F アクション

- サポート種別（例）:
  - `http.get`
  - `http.post`
- オプション:
  - `display` キーにより、レスポンスの一部をボタンラベルや状態として反映。

### F4: 状態反映

- 最低限:
  - ページ状態の同期。
- 拡張（将来）:
  - OBS の配信/録画状態を取得し、該当ボタンのスタイル変更（ON/OFF表示）。
  - 外部API結果に応じた色分け。

## 3. 通信プロトコル（Control Core ↔ クライアント）

本システム内のやり取りは、JSON メッセージに統一する。輸送層は WebSocket または HTTP を使用する。

### 3.1 共通フィールド

- `type`: メッセージ種別文字列
- `requestId`: 任意。応答が必要な場合に使用。
- `payload`: メッセージ固有のデータ

### 3.2 クライアント → Control Core

1. ボタン押下通知

    ```json
    {
      "type": "button.click",
      "payload": {
        "page": "main",
        "x": 0,
        "y": 1,
        "source": "dock"       // "dock" or "streamdeck"
      }
    }
    ```
    Control Core は panel.json に従いアクションを解決し、OBS/HTTP 等を実行する。

### 3.3 Control Core → クライアント

1. ページ定義・更新

    ```json
    {
      "type": "page.update",
      "payload": {
        "currentPage": "main",
        "buttons": [
          { "x":0, "y":0, "label":"LIVE", "state":"on" },
          { "x":1, "y":0, "label":"REC",  "state":"off" }
        ]
      }
    }
    ```
    - Dock UI はこれを元にボタン表示を再描画。
    - Stream Deck プラグインは対応キーのタイトル・アイコンを更新。

2. 状態更新通知（例）

    ```json
    {
      "type": "status.update",
      "payload": {
        "streaming": true,
        "recording": false
      }
    }
    ```
    クライアント側は該当ボタンのスタイル等を更新する。

## 3.4 エラー応答
```json
{
  "type": "error",
  "requestId": "xxxxx",
  "payload": {
    "code": "INVALID_ACTION",
    "message": "Unknown action type: obs.unknown"
  }
}
```
Control Core は panel.json の不備や実行不能アクションを明示する。

## 4. 非機能要件

- ローカル限定通信（127.0.0.1）を基本とする。
- 設定ファイル不正時は安全側（何もしない）で動作。
- Control Core 停止時でも OBS 単体操作は継続可能。

---
[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../index.md) > パネルシステム機能仕様
