[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../index.md) > [パネルシステム概要](00_overview.md) > Stream Deck プラグイン仕様

# Stream Deck プラグイン仕様

## 1. 目的

Elgato Stream Deck（3×5 = 15キー）上に、panel_system で定義された
3×5 パネルを同期表示し、Control Core と同一プロトコルで連携する。

OBS Dock UI と同じ JSON 定義・メッセージ仕様を使用し、
どちらから操作しても同じページ・同じアクションが発生することを目的とする。

## 2. 接続先

- Control Core WebSocket サーバ
  - 例: `ws://127.0.0.1:7345`
- プラグインは起動時にサーバへ接続を試行し、接続断時は自動再接続する。

## 3. キーと座標の対応

- レイアウト:
  - x: 0〜4（左から右）
  - y: 0〜2（上から下）
- 各キーは (x,y) を 1つ持つ。
  - 例: 左上キー = (0,0), 右下キー = (4,2)
- (x,y) は Stream Deck プロファイルまたはプラグイン設定に保持し、
  Control Core から受信する `page.update` と組み合わせて表示を決定する。

## 4. メッセージ仕様

### 4.1 受信（Control Core → プラグイン）

1. `page.update`

    ```json
    {
    "type": "page.update",
    "payload": {
        "currentPage": "main",
        "buttons": [
        { "x": 0, "y": 0, "label": "LIVE", "state": "on" },
        { "x": 1, "y": 0, "label": "REC" }
        ]
    }
    }
    ```

- プラグインは、自身が担当する (x,y) に一致するボタン定義を探し、該当キーのタイトル／アイコンを更新する。
- 定義が無いキーは「空」（タイトルなし、アイコンなし）として表示する。

2. status.update

    ```json
    {
    "type": "status.update",
    "payload": {
        "streaming": true,
        "recording": false
    }
    }
    ```

- 任意機能。
- 対応するボタン（例: LIVE/REC）に状態を反映してもよい。

3. error

    ```json
    {
    "type": "error",
    "payload": {
        "code": "INVALID_ACTION",
        "message": "..."
    }
    }
    ```

- デバッグ用。UI表示は任意（ログ出力推奨）。

### 4.2 送信（プラグイン → Control Core）

1. キー押下（ボタン実行）

    ```json
    {
    "type": "button.click",
    "payload": {
        "page": "<現在のページ名>",
        "x": <keyX>,
        "y": <keyY>,
        "source": "streamdeck"
    }
    }
    ```

- page は最後に受信した page.update.currentPage。
- (x,y) はそのキーに紐づく座標。

## 5. 動作要件

- Dock UI が受け取る page.update と同一の情報で、キー表示が更新されること。
- page.switch アクションを含むキーを押した場合:
    - Control Core 側でページが切り替わり、
    - 新たな page.update が Dock / Stream Deck 双方に配信される。

## 6. 非機能要件

- Control Core 非起動時:
    - プラグインはキーに「NO CORE」等を表示しておく（挙動は実装側で定義）。

- WebSocket 切断時:
    - 自動リトライ。

- 設定:
    - WebSocket URL（ホスト・ポート）はプラグイン設定で変更可能とする。

---
[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../index.md) > [パネルシステム概要](00_overview.md) > Stream Deck プラグイン仕様
