[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../../index.md) > Stream Deck プラグイン 単体テストケース定義

# Stream Deck プラグイン 単体テストケース定義

## 1. 目的

Stream Deck プラグインが Control Core のメッセージ仕様に従い、
正しくキー表示と button.click 送信を行うことを確認する。

## 2. テスト範囲

- `page.update` の反映ロジック
- キー押下時の `button.click` 送信内容
- 接続状態ハンドリング（簡易）

## 3. テストケース一覧

### SD-01: page.update によるキー表示更新

- 入力:
  - `type: "page.update"`
  - `currentPage: "main"`
  - `buttons: [{ "x":0, "y":0, "label":"LIVE" }]`
- 前提:
  - 対象キーが (0,0) の座標設定を持つ。
- 確認:
  - 該当キーのタイトルが "LIVE" に更新される。

### SD-02: 未定義キーの扱い

- 入力:
  - 一部座標のみ `buttons` に含まれる。
- 確認:
  - 対応する定義が無いキーは空表示（タイトル/アイコンなし）。

### SD-03: キー押下 → button.click 送信

- 状態:
  - 直前の `page.update.currentPage = "main"`.
  - (1,0) キーに定義あり。
- 操作:
  - (1,0) キーを押下。
- 確認:
  - Control Core 向け送信メッセージが
    - `type: "button.click"`
    - `payload.page = "main"`
    - `payload.x = 1`
    - `payload.y = 0`
    - `payload.source = "streamdeck"`
    を満たす。

### SD-04: ページ切替の同期

- 入力:
  - `page.update` (currentPage="main") を受信後、
  - page.switch アクションが設定されたキーを押下。
- 挙動:
  - Control Core 側でページ切替が行われ、新しい `page.update` が配信される（別テスト対象）。
- 確認（本テスト観点）:
  - プラグインは新しい `page.update.currentPage` に従いキー表示を更新する。

### SD-05: 接続エラー時の挙動

- 条件:
  - WebSocket 接続失敗 or 切断。
- 確認:
  - プラグイン内部ステータスが「未接続」を示す状態に遷移する。
  - 自動再接続を試行する（試行間隔は実装依存）。

---
[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../../index.md) > Stream Deck プラグイン 単体テストケース定義
