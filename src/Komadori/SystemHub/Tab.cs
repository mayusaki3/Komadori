using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace XControls.UI
{
    /// <summary>
    /// TabControlのデザイン拡張版コントロールです。
    /// </summary>
    [
		DebuggerNonUserCode
	]
    public class Tab : TabControl
    {
        #region 構築・破棄

        /// <summary>
        /// XControls.UI.Tab クラスの新しいインスタンスを初期化します。
        /// </summary>
        public Tab()
            : base()
        {
            #region 初期化

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            base.Appearance = TabAppearance.Normal;
            base.Multiline = false;

            #endregion
        }

        #endregion

        #region プロパティ

        #region 追加のプロパティ

        #region PushOnForeColorプロパティ

        private Color pushOnForeColor = SystemColors.ControlText;
        /// <summary>
        /// タブ選択時の前景色です。
        /// </summary>
        [
            Category("表示"),
            Description("タブ選択時の前景色です。")
        ]
        public Color PushOnForeColor
        {
            get
            {
                return pushOnForeColor;
            }
            set
            {
                pushOnForeColor = value;
                Refresh();
            }
        }

        #endregion

        #region PushOnBackColorプロパティ

        private Color pushOnBackColor = SystemColors.Control;
        /// <summary>
        /// タブ選択時の背景色です。
        /// </summary>
        [
            Category("表示"),
            Description("タブ選択時の背景色です。")
        ]
        public Color PushOnBackColor
        {
            get
            {
                return pushOnBackColor;
            }
            set
            {
                pushOnBackColor = value;
                Refresh();
            }
        }

        #endregion

        #region PushOnBorderColorプロパティ

        private Color pushOnBorderColor = Color.Black;
        /// <summary>
        /// タブ選択時の境界色です。
        /// </summary>
        [
            Category("表示"),
            Description("タブ選択時の境界色です。")
        ]
        public Color PushOnBorderColor
        {
            get
            {
                return pushOnBorderColor;
            }
            set
            {
                pushOnBorderColor = value;
                Refresh();
            }
        }

        #endregion

        #region PushOffForeColorプロパティ

        private Color pushOffForeColor = SystemColors.ControlText;
        /// <summary>
        /// タブ非選択時の前景色です。
        /// </summary>
        [
            Category("表示"),
            Description("タブ非選択時の前景色です。")
        ]
        public Color PushOffForeColor
        {
            get
            {
                return pushOffForeColor;
            }
            set
            {
                pushOffForeColor = value;
                Refresh();
            }
        }

        #endregion

        #region PushOffBackColorプロパティ

        private Color pushOffBackColor = SystemColors.Control;
        /// <summary>
        /// タブ非選択時の背景色です。
        /// </summary>
        [
            Category("表示"),
            Description("タブ非選択時の背景色です。")
        ]
        public Color PushOffBackColor
        {
            get
            {
                return pushOffBackColor;
            }
            set
            {
                pushOffBackColor = value;
                Refresh();
            }
        }

        #endregion

        #region PushOffBorderColorプロパティ

        private Color pushOffBorderColor = Color.Black;
        /// <summary>
        /// タブ非選択時の境界色です。
        /// </summary>
        [
            Category("表示"),
            Description("タブ非選択時の境界色です。")
        ]
        public Color PushOffBorderColor
        {
            get
            {
                return pushOffBorderColor;
            }
            set
            {
                pushOffBorderColor = value;
                Refresh();
            }
        }

        #endregion

        #region BackColorプロパティ

        private Color backColor = Color.Transparent;
        /// <summary>
        /// コントロールの背景色です。
        /// </summary>
        [
            Category("表示"),
            Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
            DefaultValue("Transparent"),
            Description("コントロールの背景色です。"),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        ]
        public override Color BackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                backColor = value;
                Refresh();
            }
        }

        #endregion

        #endregion

        #region 既存のプロパティ（上書き）

        #region Appearanceプロパティ

        private TabAppearance _appearance = TabAppearance.Buttons;
        /// <summary>
        /// タブをボタンとして描画するか、通常のタブとして描画するかを示します。
        /// </summary>
        [
            Category("動作"),
            Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
            Description("タブをボタンとして描画するか、通常のタブとして描画するかを示します。"),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        ]
        public new TabAppearance Appearance
        {
            get
            {
                return _appearance;
            }
            set
            {
                _appearance = value;
                Refresh();
            }
        }

        #endregion

        #endregion

        #endregion

        #region イベント

        #region OnPaintイベント

        /// <summary>
        /// XControls.XTab.Paintイベントを発生させます。
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawTab(e);
        }

        #endregion

        #endregion

        #region 内部処理

        #region 画面描画 (DrawTab)

        private void DrawTab(PaintEventArgs e)
        {
            #region 背景描画

            e.Graphics.FillRectangle(new SolidBrush(backColor), this.ClientRectangle);
            if (TabPages.Count < 1) return;

            #endregion

            if (SelectedIndex >= 0)
            {
                #region ページ枠描画

                TabPage tp = TabPages[SelectedIndex];
                Rectangle tr = new Rectangle();
                switch (this.Alignment)
                {
                    case TabAlignment.Top:
                        tr = new Rectangle(tp.Bounds.X - 4, tp.Bounds.Y - 3, tp.Bounds.Width + 6, tp.Bounds.Height + 6);
                        break;
                    case TabAlignment.Bottom:
                        tr = new Rectangle(tp.Bounds.X - 4, tp.Bounds.Y - 4, tp.Bounds.Width + 6, tp.Bounds.Height + 6);
                        break;
                    case TabAlignment.Left:
                        tr = new Rectangle(tp.Bounds.X - 3, tp.Bounds.Y - 4, tp.Bounds.Width + 6, tp.Bounds.Height + 7);
                        break;
                    case TabAlignment.Right:
                        tr = new Rectangle(tp.Bounds.X - 4, tp.Bounds.Y - 4, tp.Bounds.Width + 6, tp.Bounds.Height + 7);
                        break;
                }
                if (Appearance == TabAppearance.Normal)
                {
                    switch (this.Alignment)
                    {
                        case TabAlignment.Top:
                            tr.Width += 3;
                            tr.Height += 1;
                            break;
                        case TabAlignment.Bottom:
                            tr.Width += 3;
                            tr.Height += 2;
                            break;
                        case TabAlignment.Left:
                            tr.Width += 2;
                            tr.Height += 1;
                            break;
                        case TabAlignment.Right:
                            tr.Width += 3;
                            tr.Height += 1;
                            break;
                    }
                    TabRenderer.DrawTabPage(e.Graphics, tr);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(tp.BackColor), tr);
                    e.Graphics.DrawRectangle(new Pen(SystemColors.ControlDark), tr);
                }

            #endregion

                #region タブ描画

            System.Windows.Forms.VisualStyles.TabItemState tstat;
            System.Windows.Forms.VisualStyles.PushButtonState bstat;
            for (int ii = 0; ii <= TabPages.Count; ii++)
            {
                if (ii != SelectedIndex)
                {
                    int i = ii;
                    if (ii == TabPages.Count) i = SelectedIndex;

                    tp = TabPages[i];
                    tr = GetTabRect(i);

                    #region タブ表示状態を決定

                    if (!Enabled)
                    {
                        tstat = System.Windows.Forms.VisualStyles.TabItemState.Disabled;
                        bstat = System.Windows.Forms.VisualStyles.PushButtonState.Disabled;
                    }
                    else if (SelectedIndex == i)
                    {
                        tstat = System.Windows.Forms.VisualStyles.TabItemState.Selected;
                        bstat = System.Windows.Forms.VisualStyles.PushButtonState.Pressed;
                    }
                    else
                    {
                        tstat = System.Windows.Forms.VisualStyles.TabItemState.Normal;
                        bstat = System.Windows.Forms.VisualStyles.PushButtonState.Normal;
                    }

                    #endregion

                    #region タブの調整

                    switch (this.Alignment)
                    {
                        case TabAlignment.Top:
                            tr.Y -= 2;
                            tr.Height += 1;
                            if (Appearance == TabAppearance.Normal)
                            {
                                if (SelectedIndex == i)
                                {
                                    tr.X -= 2;
                                    tr.Width += 4;
                                    tr.Height += 1;
                                }
                                else
                                {
                                    tr.Y += 2;
                                    tr.Height -= 2;
                                }
                            }
                            break;
                        case TabAlignment.Bottom:
                            tr.Y += 1;
                            tr.Height += 1;
                            if (Appearance == TabAppearance.Normal)
                            {
                                if (SelectedIndex == i)
                                {
                                    tr.X -= 2;
                                    tr.Width += 4;
                                    tr.Y -= 1;
                                    tr.Height += 1;
                                }
                                else
                                {
                                    tr.Height -= 2;
                                }
                            }
                            break;
                        case TabAlignment.Left:
                            tr.X -= 2;
                            tr.Width += 1;
                            if (Appearance == TabAppearance.Normal)
                            {
                                if (SelectedIndex == i)
                                {
                                    tr.Width += 1;
                                    tr.Y -= 2;
                                    tr.Height += 4;
                                }
                                else
                                {
                                    tr.X += 2;
                                    tr.Width -= 2;
                                }
                            }
                            break;
                        case TabAlignment.Right:
                            tr.X += 1;
                            tr.Width += 1;
                            if (Appearance == TabAppearance.Normal)
                            {
                                if (SelectedIndex == i)
                                {
                                    tr.X -= 1;
                                    tr.Width += 1;
                                    tr.Y -= 2;
                                    tr.Height += 4;
                                }
                                else
                                {
                                    tr.Width -= 2;
                                }
                            }
                            break;
                    }

                    #endregion

                    #region タブの生成と描画

                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    #region ビットマップ生成

                    Size ims;
                    if (Alignment == TabAlignment.Left || Alignment == TabAlignment.Right)
                    {
                        ims = new Size(tr.Height, tr.Width);
                    }
                    else
                    {
                        ims = tr.Size;
                    }
                    String tabText = tp.Text;
                    if (Alignment == TabAlignment.Bottom) tabText = "";
                    Bitmap bmp = new Bitmap(ims.Width, ims.Height);

                    #endregion

                    #region ビットマップ描画

                    Graphics g = Graphics.FromImage(bmp);
                    Brush tb = (Brush)SystemBrushes.ControlText.Clone();
                    switch (Appearance)
                    {
                        case TabAppearance.Normal:
                            TabRenderer.DrawTabItem(g, new Rectangle(0, 0, bmp.Width, bmp.Height + 1), tabText, tp.Font, this.Focused && tp.Equals(SelectedTab), tstat);
                            break;

                        case TabAppearance.Buttons:
                            ButtonRenderer.DrawButton(g, new Rectangle(0, 0, bmp.Width, bmp.Height), tabText, tp.Font, this.Focused && tp.Equals(SelectedTab), bstat);
                            break;

                        case TabAppearance.FlatButtons:
                            Brush fb, bb;
                            Pen bp;
                            if (SelectedIndex == i)
                            {
                                fb = new SolidBrush(PushOnForeColor);
                                bb = new SolidBrush(PushOnBackColor);
                                bp = new Pen(PushOnBorderColor);
                            }
                            else
                            {
                                fb = new SolidBrush(PushOffForeColor);
                                bb = new SolidBrush(PushOffBackColor);
                                bp = new Pen(PushOffBorderColor);
                            }
                            g.FillRectangle(bb, new Rectangle(2, 2, bmp.Width - 4, bmp.Height - 4));
                            g.DrawRectangle(bp, new Rectangle(1, 1, bmp.Width - 3, bmp.Height - 3));
                            if (this.Focused && tp.Equals(SelectedTab))
                            {
                                Pen focusPen = new Pen(Color.DarkGoldenrod);
                                focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                                g.DrawRectangle(new Pen(Color.Black), new Rectangle(3, 3, bmp.Width - 7, bmp.Height - 7));
                                g.DrawRectangle(focusPen, new Rectangle(3, 3, bmp.Width - 7, bmp.Height - 7));
                            }
                            tb.Dispose();
                            tb = (Brush)fb.Clone();
                            g.DrawString(tabText, tp.Font, tb, new Rectangle(0, 0, bmp.Width, bmp.Height), sf);
                            fb.Dispose();
                            bb.Dispose();
                            break;
                    }
                    g.Dispose();

                    #endregion

                    #region ビットマップの回転

                    switch (this.Alignment)
                    {
                        case TabAlignment.Bottom:
                            bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            g = Graphics.FromImage(bmp);
                            g.DrawString(tp.Text, tp.Font, tb, new RectangleF(0, 0, bmp.Width, bmp.Height), sf);
                            g.Dispose();
                            break;
                        case TabAlignment.Left:
                            bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        case TabAlignment.Right:
                            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                    }
                    e.Graphics.DrawImage(bmp, tr);
                    bmp.Dispose();
                    tb.Dispose();
                    sf.Dispose();

                    #endregion

                    #endregion
                }
            }

            #endregion
            }
        }

        #endregion

        #endregion
    }
}
