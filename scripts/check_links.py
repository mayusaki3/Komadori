#!/usr/bin/env python3
"""
Markdown リンク切れチェック（ファイル + アンカー）

- アンカー生成規則（2系統）
  1) gfm   : GitHub Flavored Markdown（GitHub見出しID）を近似（重複見出しは "-1" 等）
  2) local : ローカルMarkdownレンダラ向け（簡易ルール）

- 実行モード
  --mode gfm|local|both   : どの規則でチェックするか
  --gate gfm|local|both   : CIの合否判定に使う結果（--mode と整合必須）

- 出力
  1) 標準出力: 人間向けレポート
  2) --output json: JSON（CIアーティファクト向け）
     ※JSONには gfm/local 両方の結果をまとめて出力できる

使い方例:
  # 両方チェックして、CIはGitHub(gfm)基準で落とす（localは参考表示）
  python md_link_check.py --root docs --mode both --gate gfm --output json --output-path artifacts/md-links.json

  # 両方チェックして、両方OKでないとCIを落とす
  python md_link_check.py --root . --mode both --gate both --output json

終了コード:
  0: gate対象がOK
  1: gate対象がNG / 設定不整合 / root不正
"""

from __future__ import annotations

import argparse
import json
import pathlib
import re
import sys
from collections import defaultdict
from dataclasses import dataclass, asdict
from datetime import datetime, timezone
from typing import Iterable
from urllib.parse import unquote


# ----------------------------
# 正規表現（見出し / リンク）
# ----------------------------

# 見出し: "# タイトル"
HEADING_RE = re.compile(r"^(#{1,6})\s+(.+?)\s*$", re.MULTILINE)

# インラインリンク: [text](href)
# - できるだけ "href の ')' まで" を安全に拾うため、括弧をある程度許容
# - 画像リンク ![alt](href) は除外（直前が '!' でないこと）
INLINE_LINK_RE = re.compile(
    r"(?<!\!)\[[^\]]+\]\((?P<href>(?:[^()\\]|\\.|(?:\([^()]*\)))+)\)"
)

# 参照形式リンク: [text][ref]
REF_LINK_RE = re.compile(r"(?<!\!)\[[^\]]+\]\[(?P<ref>[^\]]*)\]")

# 参照定義: [ref]: href "title"
REF_DEF_RE = re.compile(
    r"^\[(?P<ref>[^\]]+)\]:\s*(?P<href>\S+)(?:\s+\"[^\"]*\")?\s*$",
    re.MULTILINE,
)


# ----------------------------
# データ構造
# ----------------------------

@dataclass(frozen=True)
class Broken:
    # 種別: BROKEN_FILE / BROKEN_ANCHOR / ERROR
    kind: str
    # リンクが記載されているMarkdown
    md_path: str
    # 問題のhref（元文字列）
    href: str
    # 追加情報
    detail: str = ""


@dataclass(frozen=True)
class RunSummary:
    root: str
    mode: str
    gate: str
    started_at: str
    finished_at: str
    results: dict
    gated_ok: bool


# ----------------------------
# 見出しテキストの簡易クリーンアップ
# ----------------------------

def _strip_md_inline(text: str) -> str:
    """
    見出しのアンカー生成用に、最低限の整形をする。
    - 前後空白除去
    - 末尾の "###" などを除去
    - インラインコード記号 ` を除去
    """
    t = text.strip()
    t = re.sub(r"\s#+\s*$", "", t).strip()
    t = t.replace("`", "")
    return t


# ----------------------------
# スラッグ（アンカー）生成ルール
# ----------------------------

def slugify_gfm(text: str) -> str:
    """
    GitHub（GFM）の見出しアンカーを近似する。
    目的は「実務でのリンク切れ検知」であり、完全一致の保証ではない。

    - lower
    - '_' は除去（GitHub寄り）
    - 文字（Unicode含む）/数字/空白/ハイフン以外の多くを除去
    - 空白 -> '-'
    - 連続ハイフンを1つに
    - 前後のハイフンを除去
    """
    t = _strip_md_inline(text).lower()
    t = t.replace("_", "")
    t = re.sub(r"[^\w\s\-]+", "", t, flags=re.UNICODE)
    t = re.sub(r"\s+", "-", t)
    t = re.sub(r"\-+", "-", t)
    t = t.strip("-")
    return t


def slugify_local(text: str) -> str:
    """
    ローカルMarkdownレンダラ向けの簡易アンカー生成。
    GitHubと違い、'_' を残すなど、緩めのルールにしている。
    """
    t = _strip_md_inline(text).strip().lower()
    t = re.sub(r"[^\w\s\-]+", "", t, flags=re.UNICODE)
    t = re.sub(r"\s+", "-", t)
    t = re.sub(r"\-+", "-", t)
    t = t.strip("-")
    return t


def collect_anchors(md_path: pathlib.Path, mode: str) -> set[str]:
    """
    Markdownファイルから見出しアンカー一覧を収集する。
    - 重複見出しは GitHub 互換で "-1", "-2" を付与する
    """
    text = md_path.read_text(encoding="utf-8", errors="ignore")

    slugify = slugify_gfm if mode == "gfm" else slugify_local

    anchors: set[str] = set()
    counts: defaultdict[str, int] = defaultdict(int)

    for m in HEADING_RE.finditer(text):
        heading_text = m.group(2)
        base = slugify(heading_text)
        if not base:
            continue

        n = counts[base]
        if n == 0:
            anchors.add(base)
        else:
            anchors.add(f"{base}-{n}")
        counts[base] += 1

    return anchors


# ----------------------------
# リンク抽出
# ----------------------------

def extract_links(md_path: pathlib.Path) -> list[str]:
    """
    1ファイルからリンクhrefを抽出する。
    - インラインリンク
    - 参照形式リンク（参照定義があるもののみ）
    """
    text = md_path.read_text(encoding="utf-8", errors="ignore")

    # 参照定義を収集
    ref_defs: dict[str, str] = {}
    for m in REF_DEF_RE.finditer(text):
        ref = m.group("ref").strip().lower()
        href = m.group("href").strip()
        ref_defs[ref] = href

    hrefs: list[str] = []

    # インラインリンク
    for m in INLINE_LINK_RE.finditer(text):
        hrefs.append(m.group("href").strip())

    # 参照形式リンク
    for m in REF_LINK_RE.finditer(text):
        ref = (m.group("ref") or "").strip().lower()
        if not ref:
            # [text][] の扱いなどは簡易化のためスキップ
            continue
        href = ref_defs.get(ref)
        if href:
            hrefs.append(href)

    return hrefs


# ----------------------------
# チェック本体
# ----------------------------

def _is_remote(href: str) -> bool:
    h = href.strip()
    return h.startswith(("http://", "https://", "mailto:", "tel:", "sandbox:"))


def _split_href(href: str) -> tuple[str, str | None]:
    """
    href を "path" と "fragment" に分離する。
    - (path "title") のようなtitle付きは、最初の空白以降を落とす（簡易）
    - URLデコードも行う
    """
    s = href.strip()

    # タイトル等の余計な部分を簡易に除去
    if " " in s:
        s = s.split(" ", 1)[0].strip()

    s = unquote(s)

    if "#" in s:
        path_part, fragment = s.split("#", 1)
        return path_part, fragment
    return s, None


def check_link(md: pathlib.Path, href: str, anchor_mode: str) -> list[Broken]:
    broken: list[Broken] = []

    # 外部リンク類は対象外（要望があれば別オプションで追加可能）
    if _is_remote(href):
        return broken

    path_part, fragment = _split_href(href)

    # "#anchor" のような同一ファイル内参照
    if path_part == "" and fragment is not None:
        target = md
    else:
        # 空リンク "()" はファイル不明として扱う
        if path_part == "" and fragment is None:
            broken.append(Broken("BROKEN_FILE", str(md), href, "empty href"))
            return broken

        target = (md.parent / path_part).resolve()
        if not target.exists():
            broken.append(Broken("BROKEN_FILE", str(md), href, "target file not found"))
            return broken

    # フラグメントが無ければファイル存在チェックで終了
    if not fragment:
        return broken

    # Markdown以外のファイルに対する #fragment はチェックしない
    if target.suffix.lower() != ".md":
        return broken

    try:
        anchors = collect_anchors(target, anchor_mode)
    except OSError as e:
        broken.append(Broken("ERROR", str(md), href, f"failed to read anchor targets: {e}"))
        return broken

    # 利便性のため、大小文字違いの fragment は lower も試す
    frag = fragment
    if frag not in anchors and frag.lower() in anchors:
        frag = frag.lower()

    if frag not in anchors:
        broken.append(
            Broken(
                "BROKEN_ANCHOR",
                str(md),
                href,
                f"anchor '{fragment}' not found in {target.name} [{anchor_mode}]",
            )
        )

    return broken


def run_check(root: pathlib.Path, anchor_mode: str) -> list[Broken]:
    broken_all: list[Broken] = []

    for md in root.rglob("*.md"):
        try:
            hrefs = extract_links(md)
        except OSError as e:
            broken_all.append(Broken("ERROR", str(md), "", f"failed to read: {e}"))
            continue

        for href in hrefs:
            broken_all.extend(check_link(md, href, anchor_mode))

    return broken_all


# ----------------------------
# レポート / JSON出力
# ----------------------------

def _sorted_broken(items: list[Broken]) -> list[Broken]:
    return sorted(items, key=lambda b: (b.kind, b.md_path, b.href, b.detail))


def format_report(title: str, broken: list[Broken]) -> str:
    lines: list[str] = []
    lines.append(f"== {title} ==")

    if not broken:
        lines.append("OK: broken link は見つかりませんでした。")
        return "\n".join(lines)

    items = _sorted_broken(broken)
    for b in items:
        if b.detail:
            lines.append(f"[{b.kind}] {b.md_path}: {b.href} :: {b.detail}")
        else:
            lines.append(f"[{b.kind}] {b.md_path}: {b.href}")
    lines.append(f"Broken links: {len(items)}")
    return "\n".join(lines)


def write_json(path: pathlib.Path, payload: dict) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(payload, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


# ----------------------------
# CLI
# ----------------------------

def main() -> int:
    p = argparse.ArgumentParser()
    p.add_argument("--root", default=".", help="探索ルート（default: .）")
    p.add_argument("--mode", choices=["gfm", "local", "both"], default="both", help="実行するチェック")
    p.add_argument("--gate", choices=["gfm", "local", "both"], default="both", help="CI合否判定に使う結果")
    p.add_argument("--output", choices=["none", "json"], default="none", help="追加出力（default: none）")
    p.add_argument(
        "--output-path",
        default="md-link-check.json",
        help="--output json の出力先（default: md-link-check.json）",
    )
    args = p.parse_args()

    started = datetime.now(timezone.utc)

    root = pathlib.Path(args.root).resolve()
    if not root.exists():
        print(f"[ERROR] root が存在しません: {root}", file=sys.stderr)
        return 1

    results: dict[str, list[Broken]] = {}

    if args.mode in ("gfm", "both"):
        results["gfm"] = run_check(root, "gfm")
    if args.mode in ("local", "both"):
        results["local"] = run_check(root, "local")

    # 人間向けレポート出力
    if "gfm" in results:
        print(format_report("GFM（GitHub）アンカーチェック", results["gfm"]))
        print()
    if "local" in results:
        print(format_report("ローカル向けアンカーチェック", results["local"]))
        print()

    # gate判定
    def has_errors(key: str) -> bool:
        return bool(results.get(key, []))

    gated_ok = True

    if args.gate == "both":
        # gate対象を要求しているのに、modeで実行されていない場合は設定ミスとしてNG
        for k in ("gfm", "local"):
            if k not in results:
                print(f"[ERROR] --gate both には '{k}' の結果が必要ですが、--mode {args.mode} では実行されません。", file=sys.stderr)
                gated_ok = False
                break
            if has_errors(k):
                gated_ok = False
    else:
        if args.gate not in results:
            print(f"[ERROR] --gate {args.gate} には '{args.gate}' の結果が必要ですが、--mode {args.mode} では実行されません。", file=sys.stderr)
            gated_ok = False
        elif has_errors(args.gate):
            gated_ok = False

    finished = datetime.now(timezone.utc)

    # JSON出力（CIアーティファクト）
    if args.output == "json":
        payload = RunSummary(
            root=str(root),
            mode=args.mode,
            gate=args.gate,
            started_at=started.isoformat(),
            finished_at=finished.isoformat(),
            results={
                k: {
                    "broken_count": len(v),
                    "items": [asdict(x) for x in _sorted_broken(v)],
                }
                for k, v in results.items()
            },
            gated_ok=gated_ok,
        )
        out_path = pathlib.Path(args.output_path).resolve()
        write_json(out_path, asdict(payload))
        print(f"[INFO] JSON を出力しました: {out_path}")
        print()

    if not gated_ok:
        print("FAILED（gate対象で broken link が検出されました / 設定不整合）")
        return 1

    print("SUCCESS（gate対象OK）")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
