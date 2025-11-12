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

### TC-03: 未定義ページ / 不正ページ文脈

- 条件:
  - `currentPage = "main"` の状態で、`page = "unknown"` など現在ページと異なる `page` の `button.click` を受信
- 手順:
  - Control Core を通常起動
  - 上記条件の `button.click` メッセージを入力
- 確認:
  - OBS / HTTP の呼び出しを行わない
  - `error` メッセージ（code=`INVALID_PAGE_CONTEXT`）を送信する

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

### TC-06: HTTP 呼び出し＋表示更新 (http.get)

- 条件:
  - `action.type = "http.get"`, `url` と `display`（表示用キー）が定義されたボタンが存在する panel.json
- 手順:
  - Control Core を起動
  - 対象ボタンに対する `button.click` を送信
- 確認:
  - `http.get` が1回呼び出される
  - 応答内容を反映した `page.update` を送信する（display対象の値がボタン情報に反映されていること）

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

### TC-09: ボタン未定義座標

- 条件:
  - 対象ページ上に定義されていない (x,y) に対する `button.click`
- 手順:
  - Control Core を起動
  - 未定義座標の `button.click` を送信
- 確認:
  - OBS / HTTP の呼び出しを行わない
  - `error` メッセージ（code=`NO_BUTTON`）を送信する

### TC-10: obs.toggleMute アクション

- 条件:
  - `action.type = "obs.toggleMute"`, `source` が定義されたボタンが存在する panel.json
- 手順:
  - Control Core を起動
  - 対象ボタンに対する `button.click` を送信
- 確認:
  - `obs.toggleMute(source)` が1回呼び出される
  - `error` メッセージを送信しない

### TC-11: obs.setSourceVisibility アクション

- 条件:
  - `action.type = "obs.setSourceVisibility"`, `scene`, `source`, `visible` が定義されたボタンが存在する panel.json
- 手順:
  - Control Core を起動
  - 対象ボタンに対する `button.click` を送信
- 確認:
  - `obs.setSourceVisibility(scene, source, visible)` が1回呼び出される
  - `error` メッセージを送信しない

### TC-12: http.post アクション

- 条件:
  - `action.type = "http.post"`, `url` が定義されたボタンが存在する panel.json
- 手順:
  - Control Core を起動
  - 対象ボタンに対する `button.click` を送信
- 確認:
  - `http.post(url, body)` が1回呼び出される（body 内容は実装定義）
  - `error` メッセージを送信しない

### TC-13: HTTP エラー時のハンドリング

- 条件:
  - `http.get` または `http.post` が例外/エラーを返す状況
- 手順:
  - エラーを返す HTTP クライアントを用意した状態で Control Core を起動
  - 対象ボタンに対する `button.click` を送信
- 確認:
  - `error` メッセージ（code=`HTTP_FAILED`）を送信する
  - 例外を呼び出し元へスローしない（プロセスが落ちない）

### TC-14: 不正 version

- 条件:
  - version が未定義、またはサポート外の値を持つ panel.json
- 手順:
  - 不正な version を持つ panel.json を指定して Control Core を起動
- 確認:
  - 起動エラー扱い（例外、または error ログ相当）
  - page.update を送信しない

### TC-15: ページ未定義

- 条件:
  - pages が空、または有効なページ定義を含まない panel.json
- 手順:
  - 該当 panel.json を指定して Control Core を起動
- 確認:
  - 起動エラー扱い
  - page.update を送信しない

### TC-16: 座標範囲不正

- 条件:
  - 許容範囲外の x / y を持つボタン定義を含む panel.json
- 手順:
  - 該当 panel.json を指定して Control Core を起動
- 確認:
  - 起動エラー扱い
  - page.update を送信しない

### TC-17: page.switch 必須パラメータ不足

- 条件:
  - `action.type = "page.switch"` だが `page` 未指定のボタンが存在する panel.json
- 手順:
  - Control Core を起動
  - 対象ボタンに対する `button.click` を送信
- 確認:
  - `error` メッセージ（code=`INVALID_ACTION`）を送信する

### TC-18: http.get 必須パラメータ不足

- 条件:
  - `action.type = "http.get"` だが `url` 未指定のボタン
- 手順:
  - Control Core を起動
  - 対象ボタンに対する `button.click` を送信
- 確認:
  - `error` メッセージ（code=`INVALID_ACTION`）を送信
  - HTTP クライアントは呼び出されない

### TC-19: http.post 必須パラメータ不足

- 条件:
  - `action.type = "http.post"` だが `url` 未指定のボタン
- 手順:
  - Control Core を起動
  - 対象ボタンに対する `button.click` を送信
- 確認:
  - `error` メッセージ（code=`INVALID_ACTION`）を送信
  - HTTP クライアントは呼び出されない

### TC-20: switchPage 未初期化呼び出し

- 条件:
  - Control Core 未初期化状態
- 手順:
  - `switchPage("main")` を直接呼び出し（テスト上の内部呼び出し）
- 確認:
  - `error` メッセージ（code=`NOT_INITIALIZED`）を送信

### TC-21: switchPage 未定義ページ指定

- 条件:
  - panel.json に存在しないページ名を指定
- 手順:
  - Control Core 初期化後、`switchPage("unknown")` を内部的に呼び出し
- 確認:
  - `error` メッセージ（code=`INVALID_PAGE`）を送信

### TC-22: OBS ステータス取得失敗

- 条件:
  - `obs.getStatus()` がエラーを投げる実装
- 手順:
  - Control Core 初期化後、ステータス更新処理（`updateStatusFromObs`）を実行
- 確認:
  - `error` メッセージ（code=`OBS_STATUS_FAILED`）を送信

### TC-23: HTTP displayKey 不一致

- 条件:
  - `action.type = "http.get"`, `display` 指定あり
  - HTTP 応答に `display` キーが存在しない
- 手順:
  - Control Core を起動
  - 対象ボタンに対する `button.click` を送信
- 確認:
  - 例外なく処理完了する
  - ラベル書き換えが行われない（元のラベルのまま）

### TC-24: version が number でない場合エラー

- 条件:
  - version: "1" の panel.json
- 手順:
  - ControlCore を該当 configPath で生成し initialize() 実行
- 確認:
  - initialize() が例外を投げる
  - page.update は送信されない

### TC-25: pages が存在しない場合エラー

- 条件:
  - version: 1 のみ定義し pages 欠如の panel.json
- 手順:
  - initialize()
- 確認:
  - 例外発生（'pages' is required 想定文言）
  - page.update 無し

### TC-26: buttons 配列未定義ページはエラー

- 条件:
  - pages: { main: { name: "Main" } } の panel.json
- 手順:
  - initialize()
- 確認:
  - 例外発生（has no buttons array 想定）
  - page.update 無し

## TC-27: ボタンの label または action 欠如でエラー

- 条件:
  - buttons: [{ x:0, y:0, label:"A" }] のように action欠如 等
- 手順:
  - initialize()
- 確認:
  - 例外発生（missing label or action 想定）
  - page.update 無し

## TC-28: main ページが無い場合、最初のページをデフォルトにする

- 条件:
  - pages: { util: { ... } } のみ定義
- 手順:
  - initialize()
- 確認:
  - 最初の page.update の currentPage が "util"
  - 例外無し

### TC-29: 未初期化状態で button.click → NOT_INITIALIZED

- 条件:
  - initialize() 未呼び出し
- 手順:
  - handleButtonClick({ page:"main", x:0, y:0, ... })
- 確認:
  - error メッセージ送信
  - code が NOT_INITIALIZED

### TC-30: currentPageKey が不正ページを指す場合 → INVALID_PAGE

- 条件:
  - 正常 initialize() 実行後、テスト側で (core as any).currentPageKey = "invalid" に書き換え
- 手順:
  - その状態で handleButtonClick 実行
- 確認:
  - error.code が INVALID_PAGE
  - 他の副作用無し

### TC-31: config 未設定時の pushPageUpdate は何も送信しない

- 条件:
  - const core = new ControlCore(...); のみ。initialize() 未実行
- 手順:
  - (core as any).pushPageUpdate() を直接呼ぶ
- 確認:
  - broadcast 呼び出し無し

### TC-32: currentPageKey 不正時 pushPageUpdate は何も送信しない

- 条件:
  - initialize() 済み
  - (core as any).currentPageKey = "unknown"
- 手順:
  - (core as any).pushPageUpdate() 実行
- 確認:
  - broadcast 呼び出し無し

## 4. 実装上の注意

- 単体テストは `/OBS/panel_system/tests/` 配下に配置し、本ドキュメントの TestID と1対1対応させる。
- 新規アクション

---
[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../../index.md) > [パネルシステムの単体テスト・結合テスト方針とケース定義](00_test_policy.md) > Control Core 単体テストケース定義
