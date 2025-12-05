[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../../index.md) > [パネルシステムの単体テスト・結合テスト方針とケース定義](00_test_policy.md) > パネルシステム テスト環境構築手順

# パネルシステム テスト環境構築手順

## 1. 前提

- Node.js 20 以上
- pnpm インストール済み
- リポジトリ直下をカレントディレクトリとして作業すること

## 2. 依存パッケージインストール

以下を実行して依存パッケージを導入する。  
※ pnpm がインストールされていない場合は、https://pnpm.io/ja/installation よりインストールしてください。
```shell
pnpm -C OBS/panel_system install
```

※ OBS/panel_system/package.json に定義された devDependencies（Vitest, TypeScript 等）を導入する。

## 3. テスト実行コマンド

### 3.1 単体テスト実行

```shell
pnpm -C OBS/panel_system test
```

- vitest.config.mts に従い OBS/panel_system/tests/**/*.spec.ts を実行する。
- Control Core 向けテストは 01_test_cases_core.md の TestID と対応させる。

### 3.2 カバレッジ付き実行

```shell
pnpm -C OBS/panel_system test:coverage
```

- coverage/ ディレクトリに HTML レポートを出力する。
- 対象は OBS/panel_system/src/**/*.ts。

## 4. 運用ルール

- 新機能追加時
  - 対応する TestID を docs/ja-JP/obs/tests/panel_system/ 内に追記する。
  - OBS/panel_system/tests/ 配下にテストコードを追加する。
  - `pnpm -C OBS/panel_system test:coverage` を実行し、エラーなしであることを確認してからマージする。
- Control Core の I/F 変更時
  - 本手順書および関連仕様書も同一ブランチで更新する。

---
[目次](../../目次.md) > [OBS 関連ドキュメント インデックス](../../index.md) > [パネルシステムの単体テスト・結合テスト方針とケース定義](00_test_policy.md) > パネルシステム テスト環境構築手順
