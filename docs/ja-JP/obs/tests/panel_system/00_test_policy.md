[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../../index.md) > パネルシステムの単体テスト・結合テスト方針とケース定義

# パネルシステムの単体テスト・結合テスト方針とケース定義

本ドキュメントは、OBS パネルシステム（panel_system）における  
**テストレイヤ構造・テスト分類・変更可否ルール・カバレッジ要件** を定義する。

外部仕様として保証すべき項目と、  
実装依存かつカバレッジ維持を目的とした補助テストとを厳密に区分し、  
長期的に破壊されないテスト構造を確立する。

---

## 1. テストレイヤ

OBS パネルシステムは次の 3 レイヤに分けてテストする。

### 1.1 Control Core（コア制御ロジック）
- JSON パース／バリデーション  
- ページ状態管理  
- アクション → OBS / HTTP / WebSocket 呼び出しへのマッピング  
- 外部公開動作の保証が必要

→ テストID体系：  
**OBS-PANEL-ControlCore-TC-001〜（外部仕様テスト・変更禁止）**  
**OBS-PANEL-ControlCore-TC-Impl_001〜（実装依存テスト・変更可）**

### 1.2 Dock UI
- page.update による DOM 表示制御  
- ボタンクリック → button.click イベント送出  
- 表示状態（接続／エラー）の反映  

→ テストID体系：  
**OBS-PANEL-Dock-TC-001〜（外部仕様テスト・変更禁止）**  
**OBS-PANEL-Dock-TC-Impl_001〜（実装依存テスト・変更可）**

### 1.3 Stream Deck プラグイン
- page.update → キー表示更新  
- キー押下 → Control Core 連携  
- 接続状態ハンドリング  

→ テストID体系：  
**OBS-PANEL-StreamDeck-TC-001〜（外部仕様テスト・変更禁止）**  
**OBS-PANEL-StreamDeck-TC-Impl_001〜（実装依存テスト・変更可）**

---

## 2. テスト分類（最重要）※ Control Core テストを例に説明する

### 2.1 外部仕様テスト（変更禁止）
Control Core に対する **OBS-PANEL-ControlCore-TC-001〜** は  
パネルシステムの **外部仕様そのものであり、改変・削除は禁止する。**

これらは以下を含む：

- JSON 必須項目および構造の検証  
- ページ遷移（page.switch）の仕様  
- 各種アクション（obs.*, http.*, page.*）の結果  
- 外部向けエラーコードの動作  
  - INVALID_ACTION  
  - INVALID_PAGE  
  - NOT_INITIALIZED  
  - HTTP_FAILED  
  - OBS_STATUS_FAILED  

**外部仕様テストは仕様書と同格であり、このドキュメントと連動してのみ変更される。**

---

### 2.2 実装依存テスト（変更可）
Control Core に対する **OBS-PANEL-ControlCore-TC-Impl_001〜** は  
**実装依存の枝分岐網羅・例外経路・内部条件分岐を対象とした補助テスト** とする。

特徴：
- control-core.spec.ts を **一次ソース** として扱う  
- 実装変更に合わせて自由に差し替えてよい  
- ドキュメント化しない（保守コスト削減）

ただし以下の動作保証は必須（削除禁止）：

- HTTP/OBS の例外時 → 正しいエラーコードを返す  
- ラベル更新の成否判断が正しい  
- ページ状態遷移の整合性が破壊されていない  

---

## 3. カバレッジ要件

### 3.1 Control Core
- **行（Stmt）／分岐（Branch）ともに 100% 必須**
- カバレッジ達成のため TC-Impl_001 以降を柔軟に構成してよい
- **カバレッジ目的のテスト弱体化は禁止**

### 3.2 Dock UI / Stream Deck

#### 3.2.1 Dock UI（例外扱い）
- ブラウザ / DOM 依存のため、行（Stmt）/分岐（Branch）100% を必須とはしない
- ただし外部仕様テスト（OBS-PANEL-Dock-TC-xxx 系）は固定・維持する
- 重要な分岐やガードを中心に、合理的な範囲でカバーする

#### 3.2.2 Stream Deck（層分離）
Stream Deck は「コアロジック」と「外部 I/F」を分離して扱う。

- Stream Deck Core（コアロジック）
  - page.update → 状態反映、キー押下 → button.click 生成などの純ロジック領域
  - 原則として行（Stmt）/分岐（Branch）100% 必須（保証対象）

- Stream Deck 外部 I/F（SDK / WebSocket / ホストアプリ依存）
  - 実行環境依存のため、行（Stmt）/分岐（Branch）100% を必須とはしない
  - UT はモック中心とし、必要に応じてスモーク / E2E テストで補完する

---

## 4. テスト戦略

### 4.1 Control Core
- OBS クライアント → モック  
- HTTP → axios モック  
- Dock → メッセージオブジェクトで疑似通信  
- 入力（JSON／ボタン押下）→ 状態遷移／エラーコード／送信メッセージを検証

### 4.2 Dock UI
- ヘッドレスブラウザ or DOM テスト  
- page.update の描画  
- クリックイベント → button.click の送信内容確認

### 4.3 Stream Deck
- Stream Deck は Core（コアロジック）と外部 I/F（SDK / WebSocket）を分離してテストする
  - Core は純ロジックとしてカバレッジ100%を目標（保証対象）
  - 外部 I/F は SDK モック中心とし、カバレッジ100%は必須としない
- SDK モックによる setTitle / setImage 呼び出し確認  
- button.click の送出内容  
- 接続状態のハンドリング

---

## 5. 実装配置（推奨）

- [パネルシステム テスト環境構築手順](10_test_env_setup.md)  
- 実コードのテスト配置：  
  `/OBS/panel_system/tests/`
- 本ディレクトリ：  
  `docs/ja-JP/obs/tests/panel_system/`  
  → **外部仕様テスト（TC-001〜032）と運用方針のみ管理**  
  → 実装依存テストは仕様書に含めない

---
[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../../index.md) > パネルシステムの単体テスト・結合テスト方針とケース定義
