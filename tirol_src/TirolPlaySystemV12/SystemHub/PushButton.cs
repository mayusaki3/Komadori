using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace XControls.UI
{
    /// <summary>
    /// Buttonの押しボタン用拡張版コントロールです。
    /// </summary>
    [
        DebuggerNonUserCode,
        Designer(typeof(PushButtonDesigner))
    ]
    public class PushButton : Button
    {
        #region インナークラス

        #region PushButtonDesignerクラス

        /// <summary>
        /// PushButton用にデザイナをカスタマイズします。
        /// </summary>
        public class PushButtonDesigner : System.Windows.Forms.Design.ControlDesigner
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.UI.PushButton.PushButtonDesigner クラスの新しいインスタンスを初期化します。
            /// </summary>
            public PushButtonDesigner()
            {
            }

            #endregion

            #region メソッド

            #region PostFilterPropertiesメソッド

            protected override void PostFilterProperties(IDictionary properties)
            {
                // フィルタリングするプロパティ
                properties.Remove("AutoScroll");
                properties.Remove("AutoScrollMargin");
                properties.Remove("AutoScrollMinSize");
                properties.Remove("ImeMode");

                base.PostFilterProperties(properties);
            }

            #endregion

            #endregion
        }

        #endregion

        #endregion

        #region 構築・破棄

        /// <summary>
        /// XControls.UI.PushButton クラスの新しいインスタンスを初期化します。
        /// </summary>
        public PushButton()
            : base()
        {
        }

        #endregion

        #region 定数

        /// <summary>
        /// ボタンの動作を指定する定数です。
        /// </summary>
        public enum ActionModes : int
        {
            /// <summary>
            /// 通常のボタンです。
            /// </summary>
            Button,
            /// <summary>
            /// プッシュボタンです。押すたびにON/OFFを切り替えます。
            /// </summary>
            PushButton,
            /// <summary>
            /// プッシュボタンです。押すとONになり、OFFにするにはプログラムで制御する必要があります。
            /// </summary>
            PushOnButton,
            /// <summary>
            /// グループプッシュボタンです。押すたびにON/OFFを切り替え、同じGroupNameの他のボタンはOFFになります。
            /// </summary>
            GroupPushButton,
            /// <summary>
            /// グループプッシュボタンです。押すとONになり、同じGroupNameの他のボタンはOFFになります。
            /// </summary>
            GroupPushOnButton,
            /// <summary>
            /// 通常のボタンです。押すと同じGroupNameのボタンはOFFになります。
            /// </summary>
            GroupResetButton
        }

        #endregion

        #region 変数

        /// <summary>
        /// ボタンが押されているかを表します。
        /// </summary>
        private bool isPush = false;

        #endregion

        #region プロパティ

        #region 追加のプロパティ

        #region IsPushShowColorプロパティ

        private bool isPushShowColor = false;
        /// <summary>
        /// 確認用にボタン押下時の表示に切り替えます。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(false),
            Description("確認用にボタン押下時の表示に切り替えます。")
        ]
        public bool IsPushShowColor
        {
            get
            {
                return isPushShowColor;
            }
            set
            {
                isPushShowColor = value;
                Refresh();
            }
        }

        #endregion

        #region GroupNameプロパティ

        private string groupName = "";
        /// <summary>
        /// 複数のボタン間で連動して動作するためのグループ名を参照または設定します。
        /// </summary>
        [
            Category("動作"),
            Description("複数のボタン間で連動して動作するためのグループ名を参照または設定します。")
        ]
        public string GroupName
        {
            get
            {
                return groupName;
            }
            set
            {
                groupName = value;
            }
        }

        #endregion

        #region ActionModeプロパティ

        private ActionModes actionMode = ActionModes.Button;
        /// <summary>
        /// ボタンの動作モードを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            Description("ボタンの動作モードを参照または設定します。")
        ]
        public ActionModes ActionMode
        {
            get
            {
                return actionMode;
            }
            set
            {
                actionMode = value;
            }
        }

        #endregion

        #region PushOnForeColorプロパティ

        private Color pushOnForeColor = SystemColors.ControlText;
        /// <summary>
        /// ボタンON時の前景色です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(Color), "ControlText"),
            Description("ボタンON時の前景色です。")
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

        private Color pushOnBackColor = SystemColors.ActiveCaption;
        /// <summary>
        /// ボタンON時の背景色です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(Color), "ActiveCaption"),
            Description("ボタンON時の背景色です。")
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
        /// ボタンON時の境界色です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(Color), "Black"),
            Description("ボタンON時の境界色です。")
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
        /// ボタンOFF時の前景色です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(Color), "ControlText"),
            Description("ボタンOFF時の前景色です。")
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
        /// ボタンOFF時の背景色です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(Color), "Control"),
            Description("ボタンOFF時の背景色です。")
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
        /// ボタンOFF時の境界色です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(Color), "Black"),
            Description("ボタンOFF時の境界色です。")
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

        #region EnterBackColorOffsetプロパティ

        private int enterBackColorOffset = 30;
        /// <summary>
        /// ボタン上にマウスカーソルが重なった時の明るさ補正値です。
        /// </summary>
        [
            Category("表示"),
        DefaultValue(30),
            Description("ボタン上にマウスカーソルが重なった時の明るさ補正値です。")
        ]
        public int EnterBackColorOffset
        {
            get
            {
                return enterBackColorOffset;
            }
            set
            {
                enterBackColorOffset = value;
                Refresh();
            }
        }

        #endregion

        #region BorderSizeプロパティ

        private int borderSize = 1;
        /// <summary>
        /// ボタンの境界線のピクセル数を参照または設定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(1),
            Description("ボタンの境界線のピクセル数を参照または設定します。")
        ]
        public int BorderSize
        {
            get
            {
                return borderSize;
            }
            set
            {
                borderSize = value;
                if (borderSize < 0) borderSize = 0;
                if (borderSize > 8) borderSize = 8;
                Refresh();
            }
        }

        #endregion

        #region IsPushedプロパティ

        private bool isPushed = false;
        /// <summary>
        /// ボタンが押されているかどうかを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            Description("ボタンが押されているかどうかを参照または設定します。")
        ]
        public bool IsPushed
        {
            get
            {
                return isPushed;
            }
            set
            {
                isPushed = value;
                Refresh();
            }
        }

        #endregion

        #endregion

        #region 変更のプロパティ

        #region ForeColorプロパティ

        private Color foreColor = SystemColors.ControlText;
        /// <summary>
        /// ボタンの前景色をPushOn/Offに関係なく一括設定します。
        /// </summary>
        [
            Category("表示"),
            Description("ボタンの前景色をPushOn/Offに関係なく一括設定します。")
        ]
        public override Color ForeColor
        {
            get
            {
                return foreColor;
            }
            set
            {
                foreColor = value;
                pushOffBorderColor = foreColor;
                pushOffForeColor = foreColor;
                pushOnBorderColor = foreColor;
                pushOnForeColor = foreColor;
                Refresh();
            }
        }

        #endregion

        #region BackColorプロパティ

        private Color backColor = SystemColors.ControlText;
        /// <summary>
        /// ボタンの背景色をPushOn/Offに関係なく一括設定します。
        /// </summary>
        [
            Category("表示"),
            Description("ボタンの背景色をPushOn/Offに関係なく一括設定します。")
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
                pushOffBackColor = backColor;
                pushOnBackColor = backColor;
                Refresh();
            }
        }

        #endregion

        #endregion

        #endregion

        #region イベント

        #region OnKeyDownイベント

        /// <summary>
        /// XControls.PushButton.KeyDownイベントを発生させます。
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            if (kevent.KeyCode == Keys.Space)
            {
                isPush = true;
                StateChange();
                Refresh();
            }
            base.OnKeyDown(kevent);
        }

        #endregion

        #region OnKeyUpイベント

        /// <summary>
        /// XControls.PushButton.KeyUpイベントを発生させます。
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            base.OnKeyUp(kevent);
            if (kevent.KeyCode == Keys.Space)
            {
                isPush = false;
                Refresh();
            }
        }

        #endregion

        #region OnMouseDownイベント

        /// <summary>
        /// XControls.PushButton.MouseDownイベントを発生させます。
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button.Equals(MouseButtons.Left))
            {
            	isPush = true;
            	StateChange();
            	Refresh();
            }
            base.OnMouseDown(mevent);
        }

        #endregion

        #region OnMouseUpイベント

        /// <summary>
        /// XControls.PushButton.MouseUpイベントを発生させます。
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (IsDisposed) return;
            base.OnMouseUp(mevent);
            isPush = false;
            Refresh();
        }

        #endregion

        #region OnMouseEnterイベント

        private bool isMouseEnter = false;
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isMouseEnter = true;
            Refresh();
        }

        #endregion

        #region OnMouseLeaveイベント

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isMouseEnter = false;
            Refresh();
        }

        #endregion

        #region OnPaintイベント

        /// <summary>
        /// XControls.PushButton.Paintイベントを発生させます。
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawButton(e);
        }

        #endregion

        #endregion

        #region メソッド

        #region Pushメソッド

        /// <summary>
        /// ボタンを押します。
        /// </summary>
        public void Push()
        {
            OnMouseDown(new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));
            OnClick(new EventArgs());
            OnMouseUp(new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));
        }

        #endregion

        #endregion

        #region 内部処理

        #region 画面描画 (DrawButton)

        /// <summary>
        /// ボタンを描画します。
        /// </summary>
        /// <param name="e">イベントデータ</param>
        private void DrawButton(PaintEventArgs e)
        {
            #region ボタン描画

            Graphics g = e.Graphics;

            Brush fb, bb, bp;
            if (isPush || isPushed || isPushShowColor)
            {
                fb = new SolidBrush(PushOnForeColor);
                bb = new SolidBrush(GetOffsetColor(PushOnBackColor, enterBackColorOffset));
                bp = new SolidBrush(PushOnBorderColor);
            }
            else
            {
                fb = new SolidBrush(PushOffForeColor);
                bb = new SolidBrush(GetOffsetColor(PushOffBackColor, enterBackColorOffset));
                bp = new SolidBrush(PushOffBorderColor);
            }
            g.FillRectangle(bp, new Rectangle(0, 0, Width, Height));
            g.FillRectangle(bb, new Rectangle(borderSize, borderSize, Width - borderSize * 2, Height - borderSize * 2));
            if (this.Focused)
            {
                Pen focusPen = new Pen(Color.DarkGoldenrod);
                focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                g.DrawRectangle(new Pen(Color.Black), new Rectangle(borderSize + 1, borderSize + 1, Width - borderSize * 2 - 3, Height - borderSize * 2 - 3));
                g.DrawRectangle(focusPen, new Rectangle(borderSize + 1, borderSize + 1, Width - borderSize * 2 - 3, Height - borderSize * 2 - 3));
            }
            StringFormat sf = new StringFormat();
            switch (this.TextAlign)
            {
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopRight:
                    sf.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomRight:
                    sf.LineAlignment = StringAlignment.Far;
                    break;
                default:
                    sf.LineAlignment = StringAlignment.Center;
                    break;
            }
            switch (this.TextAlign)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    sf.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    sf.Alignment = StringAlignment.Far;
                    break;
                default:
                    sf.Alignment = StringAlignment.Center;
                    break;
            }
            g.DrawString(Text, Font, fb, new Rectangle(0, 0, Width, Height), sf);
            fb.Dispose();
            bb.Dispose();

            #endregion
        }

        #endregion

        #region オフセット色取得 (GetOffsetColor)

        /// <summary>
        /// ベース色に対してRGB値を増減した色を取得します。
        /// </summary>
        /// <param name="baseColor">ベース色</param>
        /// <param name="offset">増減値</param>
        /// <returns>オフセット色</returns>
        private Color GetOffsetColor(Color baseColor, int offset)
        {
            if(!isMouseEnter)
            {
                return baseColor;
            }
            int R = baseColor.R + enterBackColorOffset;
            int G = baseColor.G + enterBackColorOffset;
            int B = baseColor.B + enterBackColorOffset;
            if (R < 0) R = 0;
            if (G < 0) G = 0;
            if (B < 0) B = 0;
            if (R > 255) R = 255;
            if (G > 255) G = 255;
            if (B > 255) B = 255;
            return Color.FromArgb(R, G, B);
        }

        #endregion

        #region ボタン状態更新 (StateChange)

        /// <summary>
        /// ボタンの状態を更新します。
        /// </summary>
        private void StateChange()
        {
            #region 自ボタン状態

            switch (actionMode)
            {
                case ActionModes.Button:
                case ActionModes.GroupResetButton:
                    IsPushed = false;
                    break;
                case ActionModes.PushButton:
                case ActionModes.GroupPushButton:
                    IsPushed = !IsPushed;
                    break;
                case ActionModes.PushOnButton:
                case ActionModes.GroupPushOnButton:
                    IsPushed = true;
                    break;
            }

            #endregion

            #region グループ内ボタン状態

            if (groupName.Length > 0)
            {
                switch (actionMode)
                {
                    case ActionModes.GroupResetButton:
                    case ActionModes.GroupPushButton:
                    case ActionModes.GroupPushOnButton:
                        Control par = base.Parent;
                        if (par != null)
                        {
                            foreach (Control ctrl in par.Controls)
                            {
                                PushButton btn = ctrl as PushButton;
                                if (null != btn)
                                {
                                    if (btn != this && btn.groupName.Equals(this.groupName))
                                    {
                                        btn.IsPushed = false;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            #endregion
        }

        #endregion

        #endregion
    }
}
