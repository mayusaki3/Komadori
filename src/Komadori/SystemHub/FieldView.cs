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
    /// 競技フィールド表示用コントロールです。
    /// </summary>
    [
    //DebuggerNonUserCode,
    Designer(typeof(FieldViewDesigner))]
    public class FieldView : Panel
    {
        #region インナークラス

        #region FieldViewDesignerクラス

        /// <summary>
        /// FieldView用にデザイナをカスタマイズします。
        /// </summary>
        public class FieldViewDesigner : System.Windows.Forms.Design.ControlDesigner
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.UI.FieldView.PushButtonDesigner クラスの新しいインスタンスを初期化します。
            /// </summary>
            public FieldViewDesigner()
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
        /// XControls.UI.FieldView クラスの新しいインスタンスを初期化します。
        /// </summary>
        public FieldView()
            : base()
        {
            // 初期表示調整
            DoubleBuffered = true;
        }

        #endregion

        #region 定数

        /// <summary>
        /// 壁の幅(mm)
        /// </summary>
        private const float WALL_WIDTH = 12.0F;

        /// <summary>
        /// 壁と壁の間隔(mm)
        /// </summary>
        private const float WALL_SPACE = 168.0F;

        #endregion

        #region 変数

        /// <summary>
        /// グリッド状態変更フラグ
        /// </summary>
        private bool changeFlg = false;

        #endregion

        #region プロパティ

        #region 追加のプロパティ

        #region WallColorプロパティ

        private Color wallColor = Color.Red;
        /// <summary>
        /// 壁の色です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(Color), "Red"),
            Description("壁の色です。")
        ]
        public Color WallColor
        {
            get
            {
                return wallColor;
            }
            set
            {
                wallColor = value;
                Invalidate();
            }
        }

        #endregion

        #region PostColorプロパティ

        private Color postColor = Color.DarkSlateGray;
        /// <summary>
        /// 壁のない柱だけの色です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(Color), "DarkSlateGray"),
            Description("壁のない柱だけの色です。")
        ]
        public Color PostColor
        {
            get
            {
                return postColor;
            }
            set
            {
                postColor = value;
                Invalidate();
            }
        }

        #endregion

        #region GridCountXプロパティ

        private int gridCountX = 16;
        /// <summary>
        /// X軸のグリッド数です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(16),
            Description("X軸のグリッド数です。")
        ]
        public int GridCountX
        {
            get
            {
                return gridCountX;
            }
            set
            {
                gridCountX = value;
                wallSettingCheck();
                Invalidate();
            }
        }

        #endregion

        #region GridCountYプロパティ

        private int gridCountY = 16;
        /// <summary>
        /// Y軸のグリッド数です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(16),
            Description("Y軸のグリッド数です。")
        ]
        public int GridCountY
        {
            get
            {
                return gridCountY;
            }
            set
            {
                gridCountY = value;
                wallSettingCheck();
                Invalidate();
            }
        }

        #endregion

        #region WallSettingプロパティ

        private string[] wallSetting = null;
        /// <summary>
        /// 壁の位置をY軸=行・X軸=桁で表し、'+'の場所が壁ありを表します。
        /// </summary>
        [
            Category("表示"),
            Description("壁の位置をY軸=行・X軸=桁で表し、'W'の場所が壁ありを表します。")
        ]
        public string[] WallSetting
        {
            get
            {
                wallSettingCheck();
                return wallSetting;
            }
            set
            {
                wallSetting = value;
                wallSettingCheck();
                Invalidate();
            }
        }

        private void wallSettingCheck()
        {
            int x, y;
            if (wallSetting == null)
            {
                // 初期化
                wallSetting = new string[gridCountY * 2 + 1];
                for (y = 0; y < wallSetting.Length; y++)
                {
                    wallSetting[y] = new string('+', gridCountX + y % 2);
                }
            }
            else
            {
                // 作成
                string[] w = new string[gridCountY * 2 + 1];
                for (y = 0; y < w.Length; y++)
                {
                    w[y] = new string(' ', gridCountX + y % 2);
                }
                // 複製
                for (y = 0; y < wallSetting.Length; y++)
                {
                    if (y < w.Length && y < wallSetting.Length)
                    {
                        for (x = 0; x < wallSetting[y].Length; x++)
                        {
                            if (x < w[y].Length && x < wallSetting[y].Length)
                            {
                                if (wallSetting[y][x] == '+')
                                {
                                    w[y] = w[y].Substring(0, x) + "+" + w[y].Substring(x + 1);
                                }
                            }
                        }
                    }
                }
                wallSetting = w;
            }
        }

        #endregion

        #region GridTextプロパティ

        private string[] gridText = new string[0];
        /// <summary>
        /// Y軸=行・X軸=桁(カンマ区切り)で表し、グリッドに表示するテキストを設定します。
        /// </summary>
        [
            Category("表示"),
            Description("Y軸=行・X軸=桁(カンマ区切り)で表し、グリッドに表示するテキストを設定します。")
        ]
        public new string[] GridText
        {
            get
            {
                return gridText;
            }
            set
            {
                gridText = value;
                Invalidate();
            }
        }

        #endregion

        #region GridOnプロパティ

        private string[] gridOn = null;
        /// <summary>
        /// Y軸=行・X軸=桁でグリッドのON/OFF状態を0,1で表します。
        /// </summary>
        [
            Category("表示"),
            Browsable(true),
            Description("Y軸=行・X軸=桁でグリッドのON/OFF状態を0,1で表します。")
        ]
        public new string[] GridOn
        {
            get
            {
                gridOnCheck();
                return gridOn;
            }
            set
            {
                gridOn = value;
                gridOnCheck();
                Invalidate();
            }
        }

        private void gridOnCheck()
        {
            int x, y;
            if (gridOn == null)
            {
                // 初期化
                gridOn = new string[gridCountY];
                for (y = 0; y < gridOn.Length; y++)
                {
                    gridOn[y] = new string('0', gridCountX);
                }
            }
            else
            {
                // 作成
                string[] w = new string[gridCountY];
                for (y = 0; y < w.Length; y++)
                {
                    w[y] = new string('0', gridCountX);
                }
                // 複製
                for (y = 0; y < gridOn.Length; y++)
                {
                    if (y < w.Length && y < gridOn.Length)
                    {
                        for (x = 0; x < gridOn[y].Length; x++)
                        {
                            if (x < w[y].Length && x < gridOn[y].Length)
                            {
                                if (gridOn[y][x] == '1')
                                {
                                    w[y] = w[y].Substring(0, x) + "1" + w[y].Substring(x + 1);
                                }
                            }
                        }
                    }
                }
                gridOn = w;
            }
        }

        #endregion

        #region OnForeColorプロパティ

        private Color onForeColor = SystemColors.ControlText;
        /// <summary>
        /// グリッドON時の前景色です。
        /// </summary>
        [
            Category("表示"),
            Description("グリッドON時の前景色です。")
        ]
        public Color OnForeColor
        {
            get
            {
                return onForeColor;
            }
            set
            {
                onForeColor = value;
                Invalidate();
            }
        }

        #endregion

        #region OnBackColorプロパティ

        private Color onBackColor = SystemColors.Control;
        /// <summary>
        /// グリッドON時の背景色です。
        /// </summary>
        [
            Category("表示"),
            Description("グリッドON時の背景色です。")
        ]
        public Color OnBackColor
        {
            get
            {
                return onBackColor;
            }
            set
            {
                onBackColor = value;
                Invalidate();
            }
        }

        #endregion

        #region EnterBackColorOffsetプロパティ

        private int enterBackColorOffset = 30;
        /// <summary>
        /// グリッド上にマウスカーソルが重なった時の明るさ補正値です。
        /// </summary>
        [
            Category("表示"),
        DefaultValue(30),
            Description("グリッド上にマウスカーソルが重なった時の明るさ補正値です。")
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
                Invalidate();
            }
        }

        #endregion

        #endregion

        #region 変更のプロパティ

        #region BackColorプロパティ

        private Color backColor = Color.Black;
        /// <summary>
        /// フィールドの背景色です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(Color),"Black"),
            Description("フィールドの背景色です。")
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
                Invalidate();
            }
        }

        #endregion

        #region Paddingプロパティ

        /// <summary>
        /// フィールドの周囲の隙間です。
        /// </summary>
        [
            Category("表示"),
            Description("フィールドの周囲の隙間です。")
        ]
        public new Padding Padding
        {
            get
            {
                return base.Padding;
            }
            set
            {
                base.Padding = value;
                Invalidate();
            }
        }

        #endregion

        #endregion

        #endregion

        #region イベント

        #region OnKeyDownイベント

         protected override void OnKeyDown(KeyEventArgs kevent)
        {
            if (kevent.KeyCode == Keys.Space)
            {
                StateChange();
                Invalidate();
            }
            base.OnKeyDown(kevent);
        }

        #endregion

        #region OnMouseDownイベント

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button.Equals(MouseButtons.Left))
            {
                StateChange();
                Invalidate();
            }
            base.OnMouseDown(mevent);
        }

        #endregion

        #region OnMouseEnterイベント

        private bool isMouseEnter = false;
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isMouseEnter = true;
            Invalidate();
        }

        #endregion

        #region OnMouseMoveイベント

        private float mouseX = 0;
        private float mouseY = 0;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mouseX = (float)e.X;
            mouseY = (float)e.Y;
            Invalidate();
        }

        #endregion

        #region OnMouseLeaveイベント

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isMouseEnter = false;
            Invalidate();
        }

        #endregion

        #region OnPaintイベント

        /// <summary>
        /// XControls.UI.FieldView.Paintイベントを発生させます。
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawField(e);
        }

        #endregion

        #region OnChangeValueイベント

        public void OnChangeValue(object sender, EventArgs e)
        {
            if (ChangeValue != null) ChangeValue(sender, e);
        }

        #endregion

        #region ChangeValueイベント

        public delegate void ChangeValueEventHandler(object sender, EventArgs e);
        /// <summary>
        /// カウントダウンの状態を通知します。
        /// </summary>
        [
            Category("動作"),
            Description("設定内容が変化したことを通知します。")
        ]
        public event ChangeValueEventHandler ChangeValue;

        #endregion

        #endregion

        #region 内部処理

        #region フィールド描画 (DrawField)

        /// <summary>
        /// フィールドを描画します。
        /// </summary>
        /// <param name="e">イベントデータ</param>
        private void DrawField(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // 描画用比率計算
            float rSizeX = (WALL_WIDTH + WALL_SPACE) * gridCountX + WALL_WIDTH;
            float rSizeY = (WALL_WIDTH + WALL_SPACE) * gridCountY + WALL_WIDTH;
            float sizeX = Width - Padding.Left - Padding.Right - 1;
            float sizeY = Height - Padding.Top - Padding.Bottom - 1;
            float rX = sizeX / rSizeX;
            float rY = sizeY / rSizeY;

            // 外周枠
            //g.DrawRectangle(Pens.DarkOrchid, Padding.Left, Padding.Top, sizeX, sizeY);

            Brush wc = new SolidBrush(wallColor);
            Brush pc = new SolidBrush(postColor);
            Brush fc = new SolidBrush(base.ForeColor);

            for (int y = 0; y <= gridCountY; y++)
            {
                string[] tcol = null;
                if (y < gridText.Length) tcol = gridText[y].Split(new char[] { ',' });
                for (int x = 0; x <= gridCountX; x++)
                {
                    // 柱の左上頂点
                    float xp = Padding.Left + x * (WALL_WIDTH + WALL_SPACE) * rX;
                    float yp = Padding.Top + y * (WALL_WIDTH + WALL_SPACE) * rY;

                    #region 柱と壁

                    // 柱描画
                    g.FillRectangle(wc, xp, yp, rX * WALL_WIDTH, rY * WALL_WIDTH);
                    bool d = false;

                    // マス上端
                    if (x < gridCountX)
                    {
                        if(wallSetting[y * 2][x].Equals('+'))
                        {
                            g.FillRectangle(wc, xp + rX * WALL_WIDTH, yp, rX * WALL_SPACE, rY * WALL_WIDTH);
                            d = true;
                        }
                    }

                    // マス左端
                    if (y < gridCountY)
                    {
                        if (wallSetting[y * 2 + 1][x].Equals('+'))
                        {
                            g.FillRectangle(wc, xp, yp + rY * WALL_WIDTH, rX * WALL_WIDTH, rY * WALL_SPACE);
                            d = true;
                        }
                    }

                    // 柱だけ描画
                    if(!d)
                    {
                        if (!(x > 0 && wallSetting[y * 2][x - 1].Equals('+') || y > 0 && wallSetting[y * 2 - 1][x].Equals('+')))
                        {
                            g.FillRectangle(pc, xp, yp, rX * WALL_WIDTH, rY * WALL_WIDTH);
                        }
                    }

                    #endregion

                    #region ON/OFF表示

                    if (x < gridCountX && y < gridCountY)
                    {
                        float gsx = xp + WALL_WIDTH * rX;
                        float gsy = yp + WALL_WIDTH * rY;
                        float gsw = WALL_SPACE * rX;
                        float gsh = WALL_SPACE * rY;
                        Color gr = BackColor;
                        if (isMouseEnter && gsx <= mouseX && gsx + gsw > mouseX && gsy <= mouseY && gsy + gsh > mouseY)
                        {
                            if (changeFlg)
                            {
                                if (gridOn[y].Substring(x, 1).Equals("1"))
                                {
                                    gridOn[y] = gridOn[y].Substring(0, x) + "0" + gridOn[y].Substring(x + 1);
                                }
                                else
                                {
                                    gridOn[y] = gridOn[y].Substring(0, x) + "1" + gridOn[y].Substring(x + 1);
                                }
                                changeFlg = false;
                                OnChangeValue(this, new EventArgs());
                            }
                        }
                        if(gridOn[y].Substring(x,1).Equals("1"))
                        {
                            gr = onBackColor;
                        }
                        if (isMouseEnter && gsx <= mouseX && gsx + gsw > mouseX && gsy <= mouseY && gsy + gsh > mouseY)
                        {
                            gr = GetOffsetColor(gr, enterBackColorOffset);
                        }
                        Brush gb = new SolidBrush(gr);
                        g.FillRectangle(gb, gsx, gsy, gsw, gsh);
                        gb.Dispose();
                    }

                    #endregion

                    #region 文字

                    if (tcol !=null && x < tcol.Length)
                    {
                        SizeF ts = g.MeasureString(tcol[x], base.Font, (int)WALL_SPACE);
                        float xt = xp + WALL_WIDTH * rX + (WALL_SPACE * rX - ts.Width) / 2;
                        float yt = yp + WALL_WIDTH * rY + (WALL_SPACE * rY - ts.Height) / 2;
                        RectangleF tr = new RectangleF(new PointF(xt, yt), ts);
                        g.DrawString(tcol[x], base.Font, fc, tr);
                    }
                    #endregion
                }
            }

            wc.Dispose();
            pc.Dispose();
            fc.Dispose();
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
            if (!isMouseEnter)
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

        #region グリッド状態更新 (StateChange)

        /// <summary>
        /// グリッドの状態を更新します。
        /// </summary>
        private void StateChange()
        {
            changeFlg = true;
        }

        #endregion

        #endregion
    }
}
