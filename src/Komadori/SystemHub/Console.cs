using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace XControls.UI
{
    /// <summary>
    /// コンソールコントロールです。
    /// </summary>
    [
        DebuggerNonUserCode,
		Designer(typeof(ConsoleDesigner))
    ]
    public class Console : Control
    {
        #region インナークラス

        #region ConsoleDesignerクラス

        /// <summary>
        /// Console用にデザイナをカスタマイズします。
        /// </summary>
        public class ConsoleDesigner : System.Windows.Forms.Design.ControlDesigner
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.UI.Console.ConsoleDesigner クラスの新しいインスタンスを初期化します。
            /// </summary>
            public ConsoleDesigner()
            {
            }

            #endregion

            #region メソッド

            #region PostFilterPropertiesメソッド

            protected override void PostFilterProperties(IDictionary properties)
            {
                // フィルタリングするプロパティ
                properties.Remove("BackgroundImage");
                properties.Remove("BackgroundImageLayout");
                properties.Remove("Padding");

                base.PostFilterProperties(properties);
            }

            #endregion

            #endregion
        }

        #endregion

        #region innerTextクラス

        /// <summary>
        /// テキストを一行分保持します。
        /// </summary>
        public class InnerText : Object
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.UI.Console.InnerText クラスの新しいインスタンスを初期化します。
            /// </summary>
            public InnerText()
            {
            }

            /// <summary>
            /// XControls.UI.Console.InnerText クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="text">保持するテキスト</param>
            public InnerText(string text)
            {
                this.text = text;
            }

            /// <summary>
            /// XControls.UI.Console.InnerText クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="text">保持するテキスト</param>
            /// <param name="lineForeColor">行の前景色</param>
            public InnerText(string text, Color lineForeColor)
            {
                this.text = text;
                this.lineForeColor = lineForeColor;
            }

            /// <summary>
            /// XControls.UI.Console.InnerText クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="text">保持するテキスト</param>
            /// <param name="lineForeColor">行の前景色</param>
            /// <param name="lineBackColor">行の背景色</param>
            public InnerText(string text, Color lineForeColor, Color lineBackColor)
            {
                this.text = text;
                this.lineForeColor = lineForeColor;
                this.lineBackColor = lineBackColor;
            }

            #endregion

            #region プロパティ

            #region LineBackColorプロパティ

            private Color lineBackColor = SystemColors.Window;
            /// <summary>
            /// 行のデフォルト背景色を取得または設定します。
            /// </summary>
            public Color LineBackColor
            {
                get
                {
                    return lineBackColor;
                }
                set
                {
                    lineBackColor = value;
                }
            }

            #endregion

            #region LineForeColorプロパティ

            private Color lineForeColor = SystemColors.ControlText;
            /// <summary>
            /// 行のデフォルト前景色を取得または設定します。
            /// </summary>
            public Color LineForeColor
            {
                get
                {
                    return lineForeColor;
                }
                set
                {
                    lineForeColor = value;
                }
            }

            #endregion

            #region Textプロパティ

            private string text = "";
            /// <summary>
            /// テキストを取得または設定します。
            /// </summary>
            public string Text
            {
                get
                {
                    return text;
                }
                set
                {
                    text = value;
                }
            }

            #endregion

            #endregion

            #region メソッド
            
            #region ToStringメソッド
            
            /// <summary>
            /// 内容をテキストに変換します。
            /// </summary>
            /// <returns></returns>
            public new string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(text);
                return sb.ToString();
            }
            
            #endregion

            #endregion
        }

        #endregion

        #endregion

        #region 構築・破棄

        /// <summary>
        /// XControls.UI.Console クラスの新しいインスタンスを初期化します。
        /// </summary>
        public Console()
        {
            // スタイル初期化
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            
            // プロパティ初期化
            base.BackColor = SystemColors.Window;
            base.Cursor = Cursors.IBeam;

            // スクロールバー初期化
            base.Controls.Add(hsbar);
            base.Controls.Add(vsbar);
            ScrollCrossRectangle.Width = vsbar.Width;
            ScrollCrossRectangle.Height = hsbar.Height;
            locationCalc();
            hsbar.Cursor = Cursors.Arrow;
            hsbar.ValueChanged += scroll_ValueChanged;
            vsbar.Cursor = Cursors.Arrow;
            vsbar.ValueChanged += scroll_ValueChanged;

            // テキスト初期化
            sLinIdx = 0;
            eLinIdx = -1;
            MaxLines = maxLines;

            // 表示初期化
            displaySizeCalc();
        }

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region 定数

        /// <summary>
        /// テキスト周囲のスペースです。
        /// </summary>
        private int BORDER_SPACE = 2;

        /// <summary>
        /// 行間のスペースです。
        /// </summary>
        private int ROW_SPACE = 1;

        /// <summary>
        /// キャレットの幅です。
        /// </summary>
        private int CARET_WIDTH = 2;

        #endregion

        #region 変数

        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 横スクロールバーです。
        /// </summary>
        private HScrollBar hsbar = new HScrollBar();

        /// <summary>
        /// 低スクロールバーです。
        /// </summary>
        private VScrollBar vsbar = new VScrollBar();

        /// <summary>
        /// テキスト表示域を示すRecrangleです。
        /// </summary>
        private Rectangle TextRectangle = new Rectangle();

        /// <summary>
        /// スクロールバーで囲まれた表示域を示すRecrangleです。
        /// </summary>
        private Rectangle ScrollCrossRectangle = new Rectangle();

        /// <summary>
        /// 全テキスト表示サイズを示すRecrangleです。
        /// </summary>
        private Rectangle FullTextRectangle = new Rectangle();

        /// <summary>
        /// テキスト左端のスペースです。
        /// </summary>
        private int textLeftSpace = 0;

        /// <summary>
        /// テキストを行ごとに管理します。
        /// </summary>
        private List<InnerText> innerLines;

        /// <summary>
        /// 開始行のインデックスです。
        /// </summary>
        private int sLinIdx;

        /// <summary>
        /// 終了行のインデックスです。
        /// </summary>
        private int eLinIdx;

        /// <summary>
        /// 行高さ(行間あり)です。
        /// </summary>
        private int lineHeight = 0;

        /// <summary>
        /// キャレットの表示を行うかのフラグです。
        /// </summary>
        private bool caretShow = false;

        /// <summary>
        /// キャレットの高さです。
        /// </summary>
        private int caretHeight = 7;

        /// <summary>
        /// エスケープシーケンス未処理データです。
        /// </summary>
        private StringBuilder esc = new StringBuilder();

        /// <summary>
        /// エスケープシーケンス処理中フラグです。
        /// </summary>
        private bool escmode = false;
 
        /// <summary>
        /// 保存されたカーソル桁位置です。
        /// </summary>
        private int savpos = 0;

        /// <summary>
        /// カーソル移動抑制フラグ
        /// </summary>
        private bool offpos = false;

        /// <summary>
        /// 現在のカーソル位置です。
        /// </summary>
        private int lastpos = 0;

        #endregion

        #region Win32API

        [DllImport("user32.dll")]
        static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

        [DllImport("user32.dll")]
        static extern bool DestroyCaret();

        [DllImport("user32.dll")]
        static extern bool SetCaretPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern bool ShowCaret(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        #endregion

        #region プロパティ

        // 動作

        #region AllowVT100Emulationプロパティ

        private bool allowVT100Emulation = false;
        /// <summary>
        /// Printメソッド、PrintLineメソッド内でVT100簡易エミュレーションを有効にするかどうかを示します。trueの場合、一部のエスケープシーケンスを処理します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("Printメソッド、PrintLineメソッド内でVT100簡易エミュレーションを有効にするかどうかを示します。trueの場合、一部のエスケープシーケンスを処理します。")
        ]
        public bool AllowVT100Emulation
        {
            get
            {
                return allowVT100Emulation;
            }
            set
            {
                allowVT100Emulation = value;
            }
        }

        #endregion

        #region AutoScrollプロパティ

        private bool autoScroll = true;
        /// <summary>
        /// キャレット位置に合わせて自動でスクロールするかどうかを示します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(true),
            Description("キャレット位置に合わせて自動でスクロールするかどうかを示します。")
        ]
        public bool AutoScroll
        {
            get
            {
                return autoScroll;
            }
            set
            {
                autoScroll = value;
                caretPosCalc();
            }
        }

        #endregion

        #region CaretLineプロパティ

        private int caretLine = 0;
        private int caretY = 0;
        /// <summary>
        /// キャレットの行位置を取得または設定します。
        /// </summary>
        [
            Category("動作"),
            Description("キャレットの行位置を取得または設定します。")
        ]
        public int CaretLine
        {
            get
            {
                return caretLine;
            }
            set
            {
                caretLine = value;
                caretPosCalc();
            }
        }

        #endregion

        #region CaretColumnプロパティ

        private int caretColumn = 0;
        private int caretColumnNow = 0;
        private int caretX = 0;
        /// <summary>
        /// キャレットの桁位置を取得または設定します。
        /// </summary>
        [
            Category("動作"),
            Description("キャレットの桁位置を取得または設定します。")
        ]
        public int CaretColumn
        {
            get
            {
                return caretColumn;
            }
            set
            {
                caretColumn = value;
                caretPosCalc();
                if (caretColumn > caretColumnNow) CaretColumn = caretColumnNow;
            }
        }

        #endregion

        #region DisplayCaretColumnプロパティ

        /// <summary>
        /// キャレットの表示桁位置を取得します。
        /// </summary>
        [
            Category("動作"),
            Description("キャレットの表示桁位置を取得します。")
        ]
        public int DisplayCaretColumn
        {
            get
            {
                return caretColumnNow;
            }
        }

        #endregion

        #region DisplayLineCountプロパティ

        /// <summary>
        /// 表示可能行数を取得します。
        /// </summary>
        [
            Category("動作"),
            Description("表示可能行数を取得します。")
        ]
        public int DisplayLineCount
        {
            get
            {
                return (TextRectangle.Height - BORDER_SPACE * 2) / lineHeight;
            }
        }

        #endregion

        #region LastLineLengthプロパティ

        /// <summary>
        /// 最終行の文字数を取得します。
        /// </summary>
        [
            Category("動作"),
            Description("最終行の文字数を取得します。")
        ]
        public int LastLineLength
        {
            get
            {
                int rt = 0;
                if (eLinIdx >= 0) rt = innerLines[eLinIdx].Text.Length;
                return rt;
            }
        }

        #endregion

        #region LineCountプロパティ

        /// <summary>
        /// 現在表示されている行数を参照します。
        /// </summary>
        [
            Category("表示"),
            Description("現在表示されている行数を参照します。")
        ]
        public int LineCount
        {
            get
            {
                int n = eLinIdx - sLinIdx + 1;
                if (eLinIdx < 0) n = 1;
                if (n < 1) n = maxLines;
                return n;
            }
        }

        #endregion

        #region MaxLinesプロパティ

        private int maxLines = 999;
        /// <summary>
        /// 表示可能な最大行数を取得または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(999),
            Description("表示可能な最大行数を取得または設定します。")
        ]
        public int MaxLines
        {
            get
            {
                return maxLines;
            }
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException("MaxLinesは 1 以上でなければなりません。");
                if (maxLines != value || innerLines == null)
                {
                    // 新しくリストを作成
                    int sidx = 0;
                    int eidx = -1;
                    int mlin = maxLines;
                    maxLines = value;
                    List<InnerText> nw = new List<InnerText>(maxLines);
                    nw.AddRange(new InnerText[maxLines]);
                    if (eLinIdx >= 0)
                    {
                        // 内容をコピー
                        int cidx = sLinIdx;
                        while (true)
                        {
                            InnerText tx = innerLines[cidx];
                            addLine(nw, ref sidx, ref eidx, tx.Text, tx.LineForeColor, tx.LineBackColor, false);
                            if (cidx == eLinIdx) break;
                            cidx++;
                            if (cidx >= mlin) cidx = 0;
                        }
                    }
                    innerLines = nw;
                    sLinIdx = sidx;
                    eLinIdx = eidx;
                }
            }
        }

        #endregion

        #region MaxLineLengthプロパティ

        private int maxLineLength = 2000;
        /// <summary>
        /// 行の最大文字数を取得または設定します。表示済みのテキストには影響しません。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(2000),
            Description("行の最大文字数を取得または設定します。表示済みのテキストには影響しません。")
        ]
        public int MaxLineLength
        {
            get
            {
                return maxLineLength;
            }
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException("MaxLineLengthは 1 以上でなければなりません。");
                maxLineLength = value;
            }
        }

        #endregion

        #region DisplayUpdateプロパティ

        private bool displayUpdate = true;
        /// <summary>
        /// 画面表示を更新するかどうかを示します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(true),
            Description("画面表示を更新するかどうかを示します。")
        ]
        public bool DisplayUpdate
        {
            get
            {
                return displayUpdate;
            }
            set
            {
                displayUpdate = value;
                if (displayUpdate) Invalidate();
            }
        }

        #endregion

        // 表示

        #region BackColorプロパティ

        /// <summary>
        /// コントロールの背景色を取得または設定します。 
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(Color), "Window"),
            Description("コントロールの背景色を取得または設定します。")
        ]
        public new Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        #endregion

        #region BorderStyleプロパティ

        private BorderStyle borderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        /// <summary>
        /// コントロールの境界線スタイルを取得または設定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(BorderStyle), "Fixed3D"),
            Description("コントロールの境界線スタイルを取得または設定します。")
        ]
        public BorderStyle BorderStyle
        {
            get
            {
                return borderStyle;
            }
            set
            {
                borderStyle = value;
                Invalidate();
            }
        }

        #endregion

        #region Cursorプロパティ

        /// <summary>
        /// マウス ポインターがコントロールの上にあるときに表示されるカーソルを取得または設定します。 
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(Cursor), "IBeam"),
            Description("マウス ポインターがコントロールの上にあるときに表示されるカーソルを取得または設定します。 ")
        ]
        public override Cursor Cursor
        {
            get
            {
                return base.Cursor;
            }
            set
            {
                base.Cursor = value;
            }
        }

        #endregion

        #region Linesプロパティ

        /// <summary>
        /// 文字列配列としてコントロールに表示されるテキストを取得または設定します。
        /// </summary>
        [
            Category("表示"),
            Description("文字列配列としてコントロールに表示されるテキストを取得または設定します。")
        ]
        public string[] Lines
        {
            get
            {
                string[] rt = new string[0];
                lock (innerLines)
                {
                    if (eLinIdx >= 0)
                    {
                        rt = new string[LineCount];
                        int cidx = sLinIdx;
                        int idx = 0;
                        while (true)
                        {
                            rt[idx] = innerLines[cidx].Text;
                            if (cidx == eLinIdx) break;
                            cidx++;
                            idx++;
                            if (cidx >= maxLines) cidx = 0;
                        }
                    }
                }
                return rt;
            }
            set
            {
                lock(innerLines)
                {
                    Clear();
                    foreach (string txt in value)
                    {
                        AddLine(txt, ForeColor, BackColor);
                    }
                }
            } 
        }

        #endregion

        #region ScrollBarsプロパティ

        private ScrollBars scrollBars = ScrollBars.None;
        /// <summary>
        /// コントロールに対して表示するスクロールバーを取得または設定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(ScrollBars),"None"),
            Description("コントロールに対して表示するスクロールバーを取得または設定します。")
        ]
        public ScrollBars ScrollBars
        {
            get
            {
                return scrollBars;
            }
            set
            {
                scrollBars = value;
                locationCalc();
            }
        }

        #endregion

        #region Textプロパティ

        /// <summary>
        /// コントロールに表示されるテキストを取得または設定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(""),
            EditorAttribute(
                typeof(System.ComponentModel.Design.MultilineStringEditor),
                typeof(System.Drawing.Design.UITypeEditor)),
            Description("コントロールに表示されるテキストを取得または設定します。")
        ]
        public new string Text
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach(string txt in Lines)
                {
                    sb.AppendLine(txt);
                }
                if (sb.Length > 2)
                {
                    base.Text = sb.ToString().Remove(sb.Length - 2);
                }
                else
                {
                    base.Text = sb.ToString();
                }
                return base.Text;
            }
            set
            {
                if(base.Text != value)
                {
                    string[] txt = value.Replace("\r", "").Split(new char[] { '\n' });
                    Lines = txt;
                }
            }
        }

        #endregion

        #endregion

        #region イベント

        #region scroll_ValueChangedイベント

        private void scroll_ValueChanged(object sender, EventArgs e)
        {
            moveCaret();
            Invalidate();
        }

        #endregion

        #region OnClickイベント

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.Focus();
        }
        
        #endregion

        #region OnGotFocusイベント

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            CreateCaret(this.Handle, IntPtr.Zero, CARET_WIDTH, caretHeight);
            caretShow = true;
        }

        #endregion

        #region OnLostFocusイベント

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            HideCaret(this.Handle);
            DestroyCaret();
        }
        
        #endregion

        #region OnFontChangedイベント

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            displaySizeCalc();
            if (this.Focused)
            {
                DestroyCaret();
                CreateCaret(this.Handle, IntPtr.Zero, CARET_WIDTH, caretHeight);
                caretShow = true;
            }
            Invalidate();
        }

        #endregion

        #region OnMouseClickイベント

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            caretPosCalc();
        }

        #endregion

        #region OnMouseWheelイベント

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            
            // スクロール量計算
            int ln = (TextRectangle.Height - BORDER_SPACE * 2) / lineHeight;
            int n = (vsbar.Value - (TextRectangle.Height - BORDER_SPACE * 2) / ln * e.Delta / 120);
            if (n > vsbar.Maximum - vsbar.LargeChange) n = vsbar.Maximum - vsbar.LargeChange;
            if (n < vsbar.Minimum) n = vsbar.Minimum;
           
            // スクロールバー反映
            vsbar.Value = n;
        }

        #endregion

        #region OnPaintイベント

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!displayUpdate) return;

            base.OnPaint(e);

            Graphics g = e.Graphics;

            // 背景描画
            Brush bg = new SolidBrush(base.BackColor);
            g.FillRectangle(bg, TextRectangle);
            bg.Dispose();
            if (scrollBars == System.Windows.Forms.ScrollBars.Both)
            {
                g.FillRectangle(SystemBrushes.Control, ScrollCrossRectangle);
            }
            switch (borderStyle)
            {
                case System.Windows.Forms.BorderStyle.Fixed3D:
                    ControlPaint.DrawBorder3D(g, TextRectangle);
                    break;
                case System.Windows.Forms.BorderStyle.FixedSingle:
                    g.DrawRectangle(SystemPens.ControlDarkDark, 0, 0, TextRectangle.Width, TextRectangle.Height);
                    break;
            }

            // テキスト表示
            int ofsx = -hsbar.Value;
            int ofsy = -vsbar.Value;
            Debug.Print(ofsy.ToString());
            if (eLinIdx >= 0)
            {
                Region region = new Region(new Rectangle(BORDER_SPACE,
                                                         BORDER_SPACE,
                                                         TextRectangle.Width - BORDER_SPACE * 2,
                                                         TextRectangle.Height - BORDER_SPACE * 2));
                e.Graphics.SetClip(region, System.Drawing.Drawing2D.CombineMode.Replace);
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                int ofsw = (int)(ofsy / lineHeight) * lineHeight;
                if (ofsy < ofsw) ofsy = ofsw - lineHeight;

                int tl = (-ofsy) / lineHeight;
                int cidx = sLinIdx + tl;
                while (cidx >= maxLines) cidx -= maxLines;
                int y = BORDER_SPACE + tl * lineHeight;
                bool draw = false;

                while (true)
                {
                    InnerText tx = innerLines[cidx];
                    if (tx == null) break;

                    int xt = ofsx + BORDER_SPACE + textLeftSpace;
                    int yt = ofsy + y - 1;
                    if (yt >= 0 && yt < TextRectangle.Bottom)
                    {
                        SizeF sz = g.MeasureString(tx.Text, this.Font);
                        Brush bc = new SolidBrush(tx.LineBackColor);
                        Brush fc = new SolidBrush(tx.LineForeColor);
                        g.FillRectangle(bc, xt, yt, sz.Width - textLeftSpace * 2, sz.Height);
                        g.DrawString(tx.Text, this.Font, fc, ofsx + BORDER_SPACE, ofsy + y + BORDER_SPACE);
                        bc.Dispose();
                        fc.Dispose();
                        draw = true;
                    }
                    else
                    {
                        if (draw) break;
                    }

                    if (cidx == eLinIdx) break;
                    cidx++;
                    if (cidx >= maxLines) cidx = 0;

                    y += lineHeight;
                }

                // キャレット位置DEBUG表示
                //g.DrawRectangle(Pens.Red, ofsx + BORDER_SPACE + caretX, ofsy + BORDER_SPACE + caretY, 5, caretHeight);

            }
        }

        #endregion

        #region OnResizeイベント

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            locationCalc();
            displaySizeCalc();
            int vv = vsbar.Maximum - vsbar.LargeChange;
            int hv = hsbar.Maximum - hsbar.LargeChange;
            if (vsbar.Value > 0 && vv >= 0) vsbar.Value = vv;
            if (hsbar.Value > 0 && hv >= 0) hsbar.Value = hv;
            Invalidate();
        }

        #endregion

        #endregion

        #region メソッド

        #region Clearメソッド

        /// <summary>
        /// コンソール ボックスからすべての内容を削除します。
        /// </summary>
        public void Clear()
        {
            lock(innerLines)
            {
                innerLines.Clear();
                innerLines.AddRange(new InnerText[maxLines]);
                sLinIdx = 0;
                eLinIdx = -1;
            }
            displaySizeCalc();
            scrollCalc();
            Invalidate();
        }
        
        #endregion

        #region AddLineメソッド

        /// <summary>
        /// １行テキストを追加します。VT100簡易エミュレーションは機能しません。
        /// </summary>
        /// <param name="text">出力するテキスト</param>
        /// <param name="lineForeColor">行の前景色</param>
        /// <param name="lineBackColor">行の背景色</param>
        public void AddLine(string text, Color lineForeColor, Color lineBackColor)
        {
            addLine(innerLines, ref sLinIdx, ref eLinIdx, text, lineForeColor, lineBackColor, false);
        }

        /// <summary>
        /// １行テキストを追加します。VT100簡易エミュレーションは機能しません。
        /// </summary>
        /// <param name="text">出力するテキスト</param>
        /// <param name="lineForeColor">行の前景色</param>
        /// <param name="lineBackColor">行の背景色</param>
        /// <param name="noblank">trueの場合は直前の行が空かをチェックして再利用します</param>
        public void AddLine(string text, Color lineForeColor, Color lineBackColor, bool noblank)
        {
            addLine(innerLines, ref sLinIdx, ref eLinIdx, text, lineForeColor, lineBackColor, noblank);
        }

        /// <summary>
        /// １行テキストを追加します。
        /// </summary>
        private void addLine(List<InnerText> innerLines, ref int sLinIdx, ref int eLinIdx, string txt, Color lineForeColor, Color lineBackColor, bool noblank)
        {
            lock(innerLines)
            {
                if (eLinIdx < 0)
                {
                    eLinIdx = 0;
                }
                else
                {
                    if (noblank == false || innerLines[eLinIdx] != null && innerLines[eLinIdx].Text.Length > 0)
                    {
                        int pidx = eLinIdx;
                        eLinIdx++;
                        if (eLinIdx >= maxLines) eLinIdx = 0;
                        if (pidx >= 0 && sLinIdx == eLinIdx)
                        {
                            sLinIdx++;
                            if (sLinIdx >= maxLines) sLinIdx = 0;
                        }
                    }
                }
                innerLines[eLinIdx] = new InnerText(txt, lineForeColor, lineBackColor);
                caretLine = LineCount - 1;
                caretColumn = txt.Length;
                lastpos = 0;
            }
            displaySizeCalc();
            Invalidate();
        }

        #endregion

        #region PrintLineメソッド

        /// <summary>
        /// コンソールにテキストを出力して改行します。
        /// </summary>
        /// <param name="text">出力するテキスト</param>
        public void PrintLine(string text)
        {
            Print(text + "\n");
        }

        /// <summary>
        /// コンソールにテキストを出力して改行します。
        /// </summary>
        /// <param name="text">出力するテキスト</param>
        /// <param name="lineForeColor">行の前景色</param>
        public void PrintLine(string text, Color lineForeColor)
        {
            Print(text + "\n", lineForeColor);
        }

        /// <summary>
        /// コンソールにテキストを出力して改行します。
        /// </summary>
        /// <param name="text">出力するテキスト</param>
        /// <param name="lineForeColor">行の前景色</param>
        /// <param name="lineBackColor">行の背景色</param>
        public void PrintLine(string text, Color lineForeColor, Color lineBackColor)
        {
            Print(text + "\n", lineForeColor, lineBackColor);
        }

        #endregion

        #region Printメソッド

        /// <summary>
        /// コンソールにテキストを出力します。行の前景色および背景色が変更されます。
        /// </summary>
        /// <param name="text">出力するテキスト</param>
        public void Print(string text)
        {
            Color lineForeColor = this.ForeColor;
            Color lineBackColor = this.BackColor;
            if (eLinIdx >= 0)
            {
                InnerText tx = innerLines[eLinIdx];
                lineForeColor = tx.LineForeColor;
                lineBackColor = tx.LineBackColor;
            }
            Print(text, lineForeColor, lineBackColor);
        }

        /// <summary>
        /// コンソールにテキストを出力します。行の前景色および背景色が変更されます。
        /// </summary>
        /// <param name="text">出力するテキスト</param>
        /// <param name="lineForeColor">行の前景色</param>
        public void Print(string text, Color lineForeColor)
        {
            Color lineBackColor = this.BackColor;
            if (eLinIdx >= 0)
            {
                InnerText tx = innerLines[eLinIdx];
                lineBackColor = tx.LineBackColor;
            }
            Print(text, lineForeColor, lineBackColor);
        }

        /// <summary>
        /// コンソールにテキストを出力します。行の前景色および背景色が変更されます。
        /// </summary>
        /// <param name="text">出力するテキスト</param>
        /// <param name="lineForeColor">行の前景色</param>
        /// <param name="lineBackColor">行の背景色</param>
        public void Print(string text, Color lineForeColor, Color lineBackColor)
        {
            StringBuilder linbuf = new StringBuilder();
            bool linFirst = true;
            bool linFeed = false;

            // CRLFのうちCRを除去して、エスケープシーケンス未処理データに連結
            esc.Append(text.Replace("\r", ""));

            lock (innerLines)
            {
                // 先頭行調整
                if (eLinIdx < 0)
                {
                    AddLine("", lineForeColor, lineBackColor);
                }
                innerLines[eLinIdx].LineForeColor = lineForeColor;
                innerLines[eLinIdx].LineBackColor = lineBackColor;

                // 編集データ初期化
                text = esc.ToString();
                esc.Clear();

                // 編集
                for (int i = 0; i < text.Length && esc.Length == 0; i++)
                {
                    // 行編集開始判定
                    if (linFirst)
                    {
                        linbuf.Clear();
                        linbuf.Append(innerLines[eLinIdx].Text);
                        linFirst = false;
                        if (linbuf.Length >= maxLineLength) linFeed = true;
                    }

                    // 文字判定
                    while (true)
                    {
                        // 改行判定
                        if (text[i] == '\n' || linFeed)
                        {
                            innerLines[eLinIdx].Text = linbuf.ToString();
                            lastpos = 0;
                            hsbar.Value = hsbar.Minimum;
                            AddLine("", lineForeColor, lineBackColor);
                            linFirst = true;
                            if (linFeed) i--;
                            linFeed = false;
                            break;
                        }

                        if (allowVT100Emulation)
                        {
                            // エスケープシーケンス開始判定
                            if (text[i] == 0x1b) escmode = true;
                            if (escmode)
                            {
                                #region エスケープシーケンス処理

                                // エスケープシーケンス
                                int elen = text.Length - i;
                                if (elen > 2 && text.Substring(i + 1, 2) == "[D")
                                {
                                    // カーソル左へ移動
                                    escmode = false;
                                    lastpos--;
                                    if (lastpos < 0) lastpos = 0;
                                    i += 2;
                                    break;
                                }
                                if (elen > 2 && text.Substring(i + 1, 2) == "[C")
                                {
                                    // カーソル右へ移動
                                    escmode = false;
                                    lastpos++;
                                    i += 2;
                                    break;
                                }
                                if (elen > 2 && text.Substring(i + 1, 2) == "[s")
                                {
                                    // カーソル位置保存
                                    escmode = false;
                                    savpos = lastpos;
                                    offpos = true;
                                    i += 2;
                                    break;
                                }
                                if (elen > 2 && text.Substring(i + 1, 2) == "[u")
                                {
                                    // カーソル位置復元
                                    escmode = false;
                                    lastpos = savpos;
                                    offpos = false;
                                    i += 2;
                                    break;
                                }
                                if (elen > 3 && text.Substring(i + 1, 3) == "[0J")
                                {
                                    // カーソル位置から行末までを削除
                                    escmode = false;
                                    linbuf.Remove(lastpos, linbuf.Length - lastpos);
                                    i += 3;
                                    break;
                                }

                                // 不正シーケンス判定
                                if (elen > 3)
                                {
                                    escmode = false;
                                    break;
                                }

                                // シーケンス途中
                                esc.Append(text.Substring(i));
                                escmode = false;

                                #endregion

                                break;
                            }
                        }

                        // 通常テキスト
                        if (linbuf.Length > lastpos)
                        {
                            linbuf.Remove(lastpos, 1).Insert(lastpos, text[i].ToString());
                        }
                        else
                        {
                            linbuf.Append(text[i].ToString());
                        }
                        lastpos++;
                        if (linbuf.Length >= maxLineLength) linFeed = true;
                        break;
                    }
                }
                if (!linFirst)
                {
                    innerLines[eLinIdx].Text = linbuf.ToString();
                }
                caretColumn = lastpos;
            }

            displaySizeCalc();
            Invalidate();
        }

        #endregion

        #region ChangeColorメソッド

        /// <summary>
        /// テキストと一致する最後の行に指定した色を設定します。
        /// </summary>
        /// <param name="text">比較用テキスト</param>
        /// <param name="lineForeColor">行の前景色</param>
        /// <param name="lineBackColor">行の背景色</param>
        public void ChangeColor(string text, Color lineForeColor, Color lineBackColor)
        {
            if(eLinIdx < 0) return;
            lock (innerLines)
            {
                int pidx = eLinIdx;
                while(true)
                {
                    if(innerLines[pidx].Text.Equals(text))
                    {
                        innerLines[pidx].LineForeColor = lineForeColor;
                        innerLines[pidx].LineBackColor = lineBackColor;
                        break;
                    }
                    if (pidx == sLinIdx) break;
                    pidx--;
                    if (pidx < 0) pidx = maxLines - 1;
                }
            }
            Invalidate();
        }

        #endregion

        #region MoveCaretFromPointメソッド

        /// <summary>
        /// マウス座標で示された場所にキャレットを移動します。
        /// </summary>
        /// <param name="X">X座標</param>
        /// <param name="Y">Y座標</param>
        public void MoveCaretFromPoint(int X, int Y)
        {
            int ofsx = -hsbar.Value;
            int ofsy = -vsbar.Value;
            int ofsw = (int)(ofsy / lineHeight) * lineHeight;
            if (ofsy < ofsw) ofsy = ofsw - lineHeight;

            caretLine = (Y - ofsy) / lineHeight;
            int n = sLinIdx + caretLine;
            while(n >= maxLines) n -= maxLines;
            if (innerLines[n] != null)
            {
                Graphics g = this.CreateGraphics();
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                SizeF sz = g.MeasureString(innerLines[n].Text, this.Font);
                caretColumn = innerLines[n].Text.Length;
                if ((int)sz.Width - textLeftSpace > X - ofsx)
                {
                    for (; caretColumn > 0; )
                    {
                        caretColumn--;
                        sz = g.MeasureString(innerLines[n].Text.Substring(0, caretColumn), this.Font);
                        if ((int)sz.Width - textLeftSpace < X - ofsx) break;
                    }
                }
                g.Dispose();
            }
            caretPosCalc();
        }

        #endregion

        #endregion

        #region 内部処理

        #region IsInputKey

        protected override bool IsInputKey(System.Windows.Forms.Keys keyData)
        {
            return true;
        }

        #endregion

        #region 内部配置再計算 (locationCalc)

        /// <summary>
        /// 内部の各コントロール配置を再計算します。
        /// </summary>
        private void locationCalc()
        {
            switch (scrollBars)
            {
                case System.Windows.Forms.ScrollBars.Both:
                    TextRectangle.Width = ClientRectangle.Width - vsbar.Width - 1;
                    TextRectangle.Height = ClientRectangle.Height - hsbar.Height - 1;
                    hsbar.Visible = true;
                    hsbar.Left = 0;
                    hsbar.Top = ClientRectangle.Bottom - hsbar.Height;
                    hsbar.Width = TextRectangle.Width+1;
                    vsbar.Visible = true;
                    vsbar.Left = ClientRectangle.Right - vsbar.Width;
                    vsbar.Top = 0;
                    vsbar.Height = TextRectangle.Height+1;
                    break;
                case System.Windows.Forms.ScrollBars.Horizontal:
                    TextRectangle.Width = ClientRectangle.Width - 1;
                    TextRectangle.Height = ClientRectangle.Height - hsbar.Height - 1;
                    hsbar.Visible = true;
                    hsbar.Left = 0;
                    hsbar.Top = ClientRectangle.Bottom - hsbar.Height;
                    hsbar.Width = TextRectangle.Width + 1;
                    vsbar.Visible = false;
                    break;
                case System.Windows.Forms.ScrollBars.None:
                    TextRectangle.Width = ClientRectangle.Width - 1;
                    TextRectangle.Height = ClientRectangle.Height - 1;
                    hsbar.Visible = false;
                    vsbar.Visible = false;
                    break;
                case System.Windows.Forms.ScrollBars.Vertical:
                    TextRectangle.Width = ClientRectangle.Width - vsbar.Width - 1;
                    TextRectangle.Height = ClientRectangle.Height - 1;
                    hsbar.Visible = false;
                    vsbar.Visible = true;
                    vsbar.Left = ClientRectangle.Right - vsbar.Width;
                    vsbar.Top = 0;
                    vsbar.Height = TextRectangle.Height + 1;
                    break;
            }
            ScrollCrossRectangle.Location = new Point(hsbar.Right, vsbar.Bottom);
            Invalidate();
        }

        #endregion

        #region 表示サイズ再計算 (displaySizeCalc)

        /// <summary>
        /// 内部の表示サイズを再計算します。
        /// </summary>
        private void displaySizeCalc()
        {
            // 計算
            StringBuilder sb = new StringBuilder(" ");
            int mxlen = 0;
            if (eLinIdx >= 0)
            {
                foreach (InnerText tx in innerLines)
                {
                    if (tx != null)
                    {
                        if (tx.Text.Length > mxlen)
                        {
                            sb.Clear();
                            sb.Append(tx.Text);
                            mxlen = tx.Text.Length;
                        }
                    }
                }
            }
            Graphics g = this.CreateGraphics();
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            SizeF sz = g.MeasureString(sb.ToString(), this.Font);
            caretHeight = (int)(sz.Height + 0.999);
            lineHeight = caretHeight + ROW_SPACE;
            SizeF s1 = g.MeasureString("H", this.Font);
            SizeF s2 = g.MeasureString("HH", this.Font);
            textLeftSpace = (int)((s1.Width - (s2.Width - s1.Width)) / 2);
            g.Dispose();
            FullTextRectangle.Width = (int)(sz.Width + 0.999) + 1 + CARET_WIDTH;
            FullTextRectangle.Height = lineHeight * LineCount;

            // スクロールバー調整
            scrollCalc();

            // キャレット位置調整
            caretPosCalc();
        }

        #endregion

        #region スクロールバー計算 (scrollCalc)

        /// <summary>
        /// スクロールバーの設定を計算します。
        /// </summary>
        private void scrollCalc()
        {
            if (TextRectangle.Width < 1 || TextRectangle.Height < 1) return;

            hsbar.Minimum = 0;
            hsbar.Maximum = FullTextRectangle.Width;
            hsbar.LargeChange = TextRectangle.Width - BORDER_SPACE;

            vsbar.Minimum = 0;
            vsbar.Maximum = FullTextRectangle.Height;
            vsbar.SmallChange = (int)(lineHeight + 0.5);
            vsbar.LargeChange = TextRectangle.Height - BORDER_SPACE;

            if (vsbar.Value < lineHeight) vsbar.Value = 0;
        }

        #endregion

        #region キャレット位置計算 (caretPosCalc)

        /// <summary>
        /// キャレットの位置を設定します。
        /// </summary>
        private void caretPosCalc()
        {
            // キャレット行桁位置補正
            if (eLinIdx < 0)
            {
                caretLine = 0;
                caretColumn = 0;
            }
            if (caretLine < 0) caretLine = 0;
            if (LineCount <= caretLine) caretLine = LineCount - 1;
            if (caretColumn < 0) caretColumn = 0;
            caretColumnNow = caretColumn;

            // キャレット座標計算
            if (eLinIdx < 0)
            {
                caretX = textLeftSpace;
                caretY = BORDER_SPACE;
            }
            else
            {
                int n = sLinIdx + caretLine;
                if (n >= maxLines) n -= maxLines;
                Graphics g = this.CreateGraphics();
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                if (innerLines[n].Text.Length < caretColumnNow) caretColumnNow = innerLines[n].Text.Length;
                SizeF sz = g.MeasureString(innerLines[n].Text.Substring(0, caretColumnNow), this.Font);
                g.Dispose();
                if(caretColumnNow < 1)
                {
                    caretX = textLeftSpace;
                }
                else
                {
                    caretX = (int)(sz.Width) - textLeftSpace;
                }
                caretY = caretLine * lineHeight;
            }

            // カーソル位置保存中はスクロールおよびキャレット移動をしない
            if (offpos) return;

            // キャレット画面内へスクロール
            if (autoScroll && TextRectangle.Height > 0)
            {
                int ofsx = -hsbar.Value;
                int ofsy = -vsbar.Value;
                int ofsw = (int)(ofsy / lineHeight) * lineHeight;
                if (ofsy < ofsw) ofsy = ofsw - lineHeight;

                int x = ofsx + BORDER_SPACE + caretX;
                int y = ofsy + BORDER_SPACE + caretY;
                int n;

                if (TextRectangle.Top + BORDER_SPACE > y)
                {
                    vsbar.Value = caretY;
                    return;
                }
                if (TextRectangle.Bottom - BORDER_SPACE < y + lineHeight)
                {
                    n = (y + lineHeight) - (TextRectangle.Bottom - BORDER_SPACE) + lineHeight - 1;
                    n = vsbar.Value + ((int)(n / lineHeight) * lineHeight);
                    if (n > vsbar.Maximum - vsbar.LargeChange) n = vsbar.Maximum - vsbar.LargeChange;
                    if (n < vsbar.Minimum) n = vsbar.Minimum;
                    if(vsbar.Value != n)
                    {
                        vsbar.Value = n;
                        return;
                    }
                }
                if (TextRectangle.Left + BORDER_SPACE + textLeftSpace > x)
                {
                    n = hsbar.Value - (TextRectangle.Left - (x - BORDER_SPACE - textLeftSpace));
                    if (n < hsbar.Minimum) n = hsbar.Minimum;
                    hsbar.Value = n;
                    return;
                }
                n = sLinIdx + caretLine;
                while (n >= maxLines) n -= maxLines;
                if (innerLines[n] != null)
                {
                    if (TextRectangle.Right - BORDER_SPACE - textLeftSpace < x + CARET_WIDTH)
                    {
                        int nx = hsbar.Value + ((x + CARET_WIDTH) - (TextRectangle.Right - BORDER_SPACE - textLeftSpace));
                        if (nx > hsbar.Maximum - hsbar.LargeChange) nx = hsbar.Maximum - hsbar.LargeChange;
                        if (nx < hsbar.Minimum) nx = hsbar.Minimum;
                        if (hsbar.Value != nx)
                        {
                            hsbar.Value = nx;
                            return;
                        }
                    }

                }
            }
            Invalidate();

            // キャレット移動
            moveCaret();
        }

        #endregion

        #region キャレット移動 (moveCaret)

        /// <summary>
        /// キャレットを移動します。
        /// </summary>
        private void moveCaret()
        {
            if (this.Focused)
            {
                int ofsx = -hsbar.Value;
                int ofsy = -vsbar.Value;
                int ofsw = (int)(ofsy / lineHeight) * lineHeight;
                if (ofsy < ofsw) ofsy = ofsw - lineHeight;
                SetCaretPos(ofsx + BORDER_SPACE + caretX, ofsy + BORDER_SPACE + caretY);
                if (caretShow)
                {
                    ShowCaret(this.Handle);
                    caretShow = false;
                }
            }
        }

        #endregion

        #endregion
    }
}
