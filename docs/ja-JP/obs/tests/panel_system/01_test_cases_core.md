[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../../index.md) > Control Core 単体テストケース定義

# Control Core 単体テストケース定義（外部仕様テスト）

本ドキュメントは、OBS パネルシステムの Core ロジック（ControlCore）に対する  
**外部仕様として保証すべきテストケース** を記載する。

以下に示す **OBS-PANEL-ControlCore-TC-001〜033** は  
ControlCore の外部公開仕様であり、**削除・弱体化・省略を禁止**する。

TC-Impl_001 以降は実装依存テストのため本ドキュメントには記載しない。

---

## 1. 対象モジュール

- モジュール: `src/control-core.ts`
- クラス: `ControlCore`
- 主な公開メソッド:
  - `initialize()`
  - `handleMessage()`
  - `handleButtonClick()`
  - `updateStatusFromObs()`
  - 内部ユーティリティ  
    (`emit`, `getPage`, `getButton`, `execHttpAndReflectLabel` など)

---

## 2. 前提条件

### 2.1 共通前提

- 設定ファイルにはテスト専用 JSON を使用する。
- Dock 側とはメッセージ（オブジェクト）で疑似通信する。
- OBS 側はモッククライアントを使用する。
- HTTP 呼び出しは axios をモックする。

### 2.2 テストデータの前提

- ページ構成：`main` / `util`
- 各ボタンは以下を必須とする：
  - `x`, `y`
  - `label`
  - `action`
- `action.type` の例：
  - `obs.setScene`
  - `obs.toggleStreaming`
  - `obs.toggleRecording`
  - `obs.toggleMute`
  - `obs.setSourceVisibility`
  - `http.get`
  - `http.post`
  - `page.switch`
  - 未知の action.type（INVALID_ACTION を確認するため）

---

## 3. テストケース（外部仕様として保持すべき項目）

---

# 3.1 正常系・基本動作

### **OBS-PANEL-ControlCore-TC-001: 正常ロードで page.update を送信する**
- initialize 完了後、currentPage のレイアウトを Dock に通知する。

### **OBS-PANEL-ControlCore-TC-002: 重複座標エラーで page.update を送信しない**
- ボタン座標の衝突時、起動はエラーとなり page.update は送信されない。

### **OBS-PANEL-ControlCore-TC-003: 未定義ページ / 不正ページ文脈で INVALID_PAGE_CONTEXT**
- handleMessage 受信時、payload.page が currentPage と不一致なら INVALID_PAGE_CONTEXT。

### **OBS-PANEL-ControlCore-TC-004: button.click → obs.setScene が呼ばれる**

### **OBS-PANEL-ControlCore-TC-005: 配信/録画トグルで OBS API が呼ばれる**

### **OBS-PANEL-ControlCore-TC-006: http.get 成功時 displayKey がラベルに反映される**

### **OBS-PANEL-ControlCore-TC-007: page.switch で main → util に切替する**

### **OBS-PANEL-ControlCore-TC-008: 未知 action.type → INVALID_ACTION を返す**

### **OBS-PANEL-ControlCore-TC-009: ボタン未定義座標 → NO_BUTTON**

---

# 3.2 OBS / HTTP 連携・エラー処理

### **OBS-PANEL-ControlCore-TC-010: obs.toggleMute が呼ばれる**

### **OBS-PANEL-ControlCore-TC-011: obs.setSourceVisibility が呼ばれる**

### **OBS-PANEL-ControlCore-TC-012: http.post → axios.post が呼ばれる**

### **OBS-PANEL-ControlCore-TC-013: HTTP 実行エラー → HTTP_FAILED**

### **OBS-PANEL-ControlCore-TC-014: config.version 不正 → initialize エラー**

### **OBS-PANEL-ControlCore-TC-015: pages 未定義 → initialize エラー**

### **OBS-PANEL-ControlCore-TC-016: ボタン座標範囲不正 → initialize エラー**

### **OBS-PANEL-ControlCore-TC-017: page.switch で page 未指定 → INVALID_ACTION**

### **OBS-PANEL-ControlCore-TC-018: http.get url 未指定 → INVALID_ACTION**

### **OBS-PANEL-ControlCore-TC-019: http.post url 未指定 → INVALID_ACTION**

### **OBS-PANEL-ControlCore-TC-020: initialize 前の switchPage → NOT_INITIALIZED**

### **OBS-PANEL-ControlCore-TC-021: switchPage に未定義ページ → INVALID_PAGE**

### **OBS-PANEL-ControlCore-TC-022: updateStatusFromObs の OBS 例外 → OBS_STATUS_FAILED**

### **OBS-PANEL-ControlCore-TC-023: http.get displayKey 指定あり／レスポンスにキー無し → ラベル更新なし**

### **OBS-PANEL-ControlCore-TC-024: version が number でない → エラー**

### **OBS-PANEL-ControlCore-TC-025: pages プロパティ欠如 → エラー**

### **OBS-PANEL-ControlCore-TC-026: buttons 配列欠如 → エラー**

### **OBS-PANEL-ControlCore-TC-027: ボタンの label / action 欠如 → エラー**

### **OBS-PANEL-ControlCore-TC-028: main が無い場合 pages の先頭を currentPage とする**

### **OBS-PANEL-ControlCore-TC-029: initialize 前の button.click → NOT_INITIALIZED**

---

# 3.3 currentPageKey の異常系

### **OBS-PANEL-ControlCore-TC-030: currentPageKey が不正 → INVALID_ACTION**
- initialize 後に強制的に不正ページへ書換え → button.click → INVALID_ACTION を返す。

---

# 3.4 pushPageUpdate 関連

### **OBS-PANEL-ControlCore-TC-031: config 未設定 → pushPageUpdate は送信しない**

### **OBS-PANEL-ControlCore-TC-032: currentPageKey 不正 → pushPageUpdate は送信しない**

### **OBS-PANEL-ControlCore-TC-033: ボタン定義の image を page.update に含める**

- 種別  
  - 外部仕様テスト

- 目的  
  - ページ定義に `image` フィールドが存在する場合、Control Core が送出する `page.update` の `buttons[*].image` に正しく反映されることを確認する。

- 前提  
  - 設定ファイルの `pages.main.buttons` に、以下のようなボタン定義を含める。  
    - 例: `{ x: 0, y: 0, label: "LIVE", image: "live.png", action: { ... } }`
  - Control Core の `initialize()` が正常終了している。

- 手順  
  1. `initialize()` を実行する。  
  2. 初回、または `pushPageUpdate()` により送出される `page.update` メッセージを取得する。

- 期待結果  
  - 取得した `page.update` の `payload.buttons` のうち、`x=0`, `y=0` に対応する要素が以下を満たす。  
    - `label: "LIVE"`  
    - `image: "live.png"`  
  - `image` の値が設定ファイルのボタン定義と一致している。

---

## 4. 保守ポリシー（重要）

### 4.1 テスト ID の区分

- **外部仕様テスト**
  - テスト ID: `OBS-PANEL-ControlCore-TC-***`
  - Control Core の外部 I/F（メッセージ仕様・エラーコード・状態遷移）として保証すべきテスト。
  - 仕様変更以外の理由で「削除・弱体化・省略」してはならない。

- **実装依存テスト**
  - テスト ID: `OBS-PANEL-ControlCore-TC-Impl_***`
  - 実装詳細（最適化・ガード分岐・内部キャッシュなど）を対象とするテスト。
  - 実装変更に合わせた差し替えを許容するが、テストコード側が一次ソースとなる。

### 4.2 常に保証すべき動作

外部仕様テストでは、少なくとも以下を保証する。

- メッセージ種別ごとの基本動作
  - 正常系: `page.update` / `button.click` / `status.update` などが仕様どおり送出されること
  - エラー系: 無効なページ / アクションに対して `INVALID_PAGE` / `INVALID_ACTION` / `NOT_INITIALIZED` 等のエラーが返ること
- メッセージ形式
  - 必須フィールド（`type`, `payload`, `payload.page`, `payload.buttons` 等）が欠落しないこと
  - 追加されたフィールド（例: `buttons[*].image`）も含め、仕様で定めた型・キー名を守ること

### 4.3 カバレッジ方針

- Control Core は、行・分岐ともに **100% カバレッジを目標**とする。
- ただし、**カバレッジ達成を目的としたテスト弱体化は禁止**とし、外部仕様テストを優先する。
- 実装依存テスト（`…-TC-Impl_***`）でガード分岐や最適化を補完する。

---
[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../../index.md) > Control Core 単体テストケース定義
