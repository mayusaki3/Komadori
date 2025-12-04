[目次](./00_test_policy.md) > Core ロジック単体テスト > 01_test_cases_core

# Control Core 単体テストケース定義（抜粋）

本ドキュメントは、OBS パネルシステムの Core ロジック（ControlCore）に対する単体テストケースのうち、
代表的なものを記載する。
実際のテストコードは OBS/panel_system/tests/control-core.spec.ts に実装されており、
ここに記載していないテストケースも存在する（TC-33 以降など）。

---

## 1. 対象モジュール

- モジュール: src/control-core.ts
- クラス: ControlCore
- 主な公開メソッド:
  - initialize
  - handleMessage
  - handleButtonClick
  - updateStatusFromObs
  - ほか内部ユーティリティ（emit, getPage, getButton, execHttpAndReflectLabel など）

---

## 2. 前提

### 2.1 共通前提

- 設定ファイル（config.json 相当）はテスト用 JSON を使用する。
- Dock 側とはメッセージ（オブジェクト）で双方向通信する想定。
- OBS 側とはモッククライアントを利用する。
- HTTP 呼び出しは axios をモックし、副作用を検証する。

### 2.2 テストデータの前提

- main / util など複数ページ構成を使用。
- 各ボタンには x, y, label, action が設定される。
- action.type の例:
  - obs.setScene
  - obs.toggleStreaming
  - obs.toggleRecording
  - obs.toggleMute
  - obs.setSourceVisibility
  - http.get
  - http.post
  - page.switch
  - それ以外（未知 action.type）もテスト対象。

---

## 3. テストケース一覧（抜粋）

### 3.1 正常系・基本動作

- **TC-01: 正常ロードで page.update を送信する**  
  initialize 正常完了後、現在ページ（main）のレイアウトを Dock に送信。

- **TC-02: 重複座標エラーで page.update を送信しない**  
  ボタン座標が重複した設定の場合、起動エラーとなり page.update は送信されない。

- **TC-03: 未定義ページ / 不正ページ文脈で INVALID_PAGE_CONTEXT**  
  メッセージ中 page が currentPageKey と一致しない場合に INVALID_PAGE_CONTEXT。

- **TC-04: button.click で obs.setScene が呼ばれる**

- **TC-05: 配信/録画トグルで OBS API が呼ばれる**

- **TC-06: http.get 成功時 displayKey をラベルに反映し page.update**

- **TC-07: page.switch で main → util に切替して page.update**

- **TC-08: 未知 action.type で INVALID_ACTION**

- **TC-09: ボタン未定義座標で NO_BUTTON**

---

### 3.2 OBS / HTTP 連携・エラー処理

- **TC-10: toggleMute で obs.toggleMute が呼ばれる**

- **TC-11: setSourceVisibility で obs.setSourceVisibility が呼ばれる**

- **TC-12: http.post で axios.post が呼ばれる**

- **TC-13: HTTP 実行エラーで HTTP_FAILED**

- **TC-14: config.version 不正で initialize エラー → page.update を送信しない**

- **TC-15: pages 未定義で initialize エラー**

- **TC-16: ボタン座標範囲不正で initialize エラー**

- **TC-17: page.switch で page 未指定なら INVALID_ACTION**

- **TC-18: http.get で url 未指定なら INVALID_ACTION**

- **TC-19: http.post で url 未指定なら INVALID_ACTION**

- **TC-20: initialize 前に switchPage 呼び出し → NOT_INITIALIZED**

- **TC-21: switchPage に未定義ページ指定 → INVALID_PAGE**

- **TC-22: updateStatusFromObs の OBS 例外時 → OBS_STATUS_FAILED**

- **TC-23: http.get displayKey 指定だがレスポンスにキー無し → ラベル更新なし**

- **TC-24: version が number でない場合エラー**

- **TC-25: pages プロパティ欠如で initialize エラー**

- **TC-26: buttons 配列が無いページは initialize エラー**

- **TC-27: ボタンの label または action 欠如で initialize エラー**

- **TC-28: main が無い場合は pages の先頭を currentPageKey に採用**

- **TC-29: initialize 前の button.click → NOT_INITIALIZED**

---

### 3.3 **TC-30: currentPageKey が不正ページなら INVALID_ACTION**（仕様修正済）

条件:
- initialize 正常完了後、テスト側で (core as any).currentPageKey = "invalid" に書き換える。

手順:
- handleButtonClick を呼び出す。

確認:
- 返却エラー code が INVALID_ACTION。
- currentPageKey は書き換わらない。
- 他の副作用なし。

---

### 3.4 pushPageUpdate 関連

- **TC-31: config 未設定時 pushPageUpdate は送信しない**  
  config が falsy の場合は即 return。

- **TC-32: currentPageKey 不正時 pushPageUpdate は送信しない**

---

## 4. 備考

- 本ドキュメントに記載する代表的テストケース（TC-01〜TC-32）は  
  **ControlCore の外部仕様に対応するため、仕様変更がない限り内容の変更・削除を禁止する。**

- 追加された細粒度テスト（TC-33 以降）は、エラーハンドリングや分岐網羅を目的とした  
  **実装依存テスト**であり、詳細仕様は control-core.spec.ts を一次ソースとして扱う。  
  実装変更に伴ってテスト内容が差し替わることを許容するが、  
  次の観点をカバーするテストが常に維持されていることを条件とする：  
  - HTTP/OBS 例外発生時に正しいエラーコード（HTTP_FAILED, OBS_STATUS_FAILED 等）が送信されること  
  - INVALID_ACTION / INVALID_PAGE / NOT_INITIALIZED などのエラー種別の制御が正しく行われること  
  - ラベル更新が行われるべき場合・行われてはならない場合の判定が正しいこと  

- カバレッジ 100%（行・分岐）は、細粒度テストを再構成する際の目安とする。  
  ただし **カバレッジ達成のみを目的としたテスト弱体化は禁止する。**

- エラーコード（INVALID_ACTION, INVALID_PAGE, NOT_INITIALIZED, HTTP_FAILED, OBS_STATUS_FAILED など）は  
  ControlCore の実装仕様に準拠する。

---

[目次](./00_test_policy.md) > Core ロジック単体テスト > 01_test_cases_core
