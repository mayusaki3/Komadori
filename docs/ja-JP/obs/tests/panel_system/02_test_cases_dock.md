[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../../index.md) > [パネルシステムの単体テスト・結合テスト方針とケース定義](00_test_policy.md) > Dock UI 単体テストケース定義

# Dock UI 単体テストケース定義

## 1. 目的

OBS Dock UI が Control Core のメッセージ仕様に従い、
正しくボタン表示とクリックイベント送信を行うことを確認する。

## 2. テスト範囲

- `page.update` → DOM 反映
- ボタンクリック → `button.click` 送信
- 接続状態表示

## 3. テストケース一覧（抜粋）

### D-01: page.update の反映

- 条件:
  - `currentPage = "main"`
  - buttons に (x=0,y=0,label="LIVE") を含む
- 手順:
  - 疑似 WebSocket から上記 `page.update` を送信
- 確認:
  - (0,0) の要素に "LIVE" が表示され、`.btn` クラスが付与されている。

### D-02: 未定義セルの扱い

- 条件:
  - 3x5 のうち一部のみ buttons 定義
- 確認:
  - 未定義セルは `.btn.disabled` かつラベル空欄のまま。

### D-03: クリック送信

- 条件:
  - (0,0) に有効なボタン
- 手順:
  - 対象要素をクリック
- 確認:
  - WebSocket 送信メッセージに
    - `type = "button.click"`
    - `payload.page = "main"`
    - `payload.x = 0`, `payload.y = 0`
    - `payload.source = "dock"`
    が含まれる。

### D-04: 接続状態表示

- 条件:
  - 接続成功 / 切断 / エラー を順に発生させる
- 確認:
  - `#status` の表示が "CONNECTED" / "DISCONNECTED - RETRYING" / "ERROR" に変化する。

---
[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../../index.md) > [パネルシステムの単体テスト・結合テスト方針とケース定義](00_test_policy.md) > Dock UI 単体テストケース定義
