using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace XControls.UI
{
    /// <summary>
    /// 時計、タイマーを表示するコントロールです。
    /// </summary>
    [Designer(typeof(TimerViewDesigner))]
    public class TimerView : UserControl
    {
        #region インナークラス

        #region TimerViewDesignerクラス

        /// <summary>
        /// TimerView用にデザイナをカスタマイズします。
        /// </summary>
        public class TimerViewDesigner : System.Windows.Forms.Design.ControlDesigner
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.UI.TimerView.TimerViewDesigner クラスの新しいインスタンスを初期化します。
            /// </summary>
            public TimerViewDesigner()
            {
            }

            #endregion

            #region メソッド

            #region PostFilterPropertiesメソッド

            protected override void PostFilterProperties(IDictionary properties)
            {
                // フィルタリングするプロパティ
                properties.Remove("CausesValidation");
                properties.Remove("AutoValidate");
                properties.Remove("AutoScroll");
                properties.Remove("AutoScrollMargin");
                properties.Remove("AutoScrollMinSize");
                properties.Remove("MaximumSize");
                properties.Remove("MinimumSize");
                properties.Remove("RightToLeft");
                properties.Remove("ForeColor");
                properties.Remove("Font");
                properties.Remove("ImeMode");

                base.PostFilterProperties(properties);
            }

            #endregion

            #endregion
        }

        #endregion

        #region TimerNoticeEventArgsクラス

        /// <summary>
        /// Timerイベントデータ
        /// </summary>
        public class TimerNoticeEventArgs : EventArgs
        {
            /// <summary>
            /// スタートの通知です。
            /// </summary>
            public Boolean StartUp = false;

            /// <summary>
            /// タイムアップの通知です。
            /// </summary>
            public Boolean TimeUp = false;

            /// <summary>
            /// 残り時間(秒)です。
            /// </summary>
            public double RemainingTime = 0;

            /// <summary>
            /// 経過時間(秒)です。
            /// </summary>
            public double ElapsedTime = 0;
        }

        #endregion

        #endregion
        
        #region 構築・破棄

        /// <summary>
        /// XControls.UI.TimerView クラスの新しいインスタンスを初期化します。
        /// </summary>
        public TimerView()
            : base()
        {
            // 初期表示調整
            FixedRatio = true;
            FixedRatio = false;
            DoubleBuffered = true;
            Time = DefaultTime;
        }

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            #region 後処理

            Stop();
            base.Dispose(disposing);

            #endregion
        }

        #endregion

        #region 変数

        /// <summary>
        /// 7セグメント表示サイズ(X軸)です。
        /// </summary>
        private float LED_SIZE_X = 80;

        /// <summary>
        /// 7セグメント表示サイズ(Y軸)です。
        /// </summary>
        private float LED_SIZE_Y = 100;

        /// <summary>
        /// 7セグメント表示[8]の上側の左縦線を描画するポリゴン定義です。
        /// </summary>
        private float[] LED_UL = { 11, 14, 15, 10, 18, 10, 22, 14, 18, 40, 14, 44, 11, 44, 7, 40 };
        
        /// <summary>
        /// 7セグメント表示[8]の上側の横線を描画するポリゴン定義です。
        /// </summary>
        private float[] LED_UC = { 18, 4, 22, 0, 67, 0, 71, 4, 71, 7, 67, 11, 22, 11, 18, 7 };
        
        /// <summary>
        /// 7セグメント表示[8]の上側の右縦線を描画するポリゴン定義です。
        /// </summary>
        private float[] LED_UR = { 68, 14, 72, 10, 75, 10, 79, 14, 74, 42, 70, 46, 67, 46, 63, 42 };
        
        /// <summary>
        /// 7セグメント表示[8]の中央の横線を描画するポリゴン定義です。
        /// </summary>
        private float[] LED_CC = { 14, 47, 18, 43, 61, 43, 65, 47, 65, 51, 61, 55, 18, 55, 14, 51 };
        
        /// <summary>
        /// 7セグメント表示[8]の下側の左縦線を描画するポリゴン定義です。
        /// </summary>
        private float[] LED_LL = { 4, 56, 8, 52, 11, 52, 15, 56, 11, 84, 7, 88, 4, 88, 0, 84 };

        /// <summary>
        /// 7セグメント表示[8]の下側の横線を描画するポリゴン定義です。
        /// </summary>
        private float[] LED_LC = { 8, 91, 12, 87, 56, 87, 61, 91, 61, 95, 56, 99, 12, 99, 8, 95 };

        /// <summary>
        /// 7セグメント表示[8]の下側の右縦線を描画するポリゴン定義です。
        /// </summary>
        private float[] LED_LR = { 60, 58, 64, 54, 67, 54, 71, 58, 67, 84, 63, 88, 60, 88, 56, 84 };

        /// <summary>
        /// 7セグメント表示[:]の上の点を描画するポリゴン定義です。
        /// </summary>
        private float[] LED_CU = { 37, 21, 12, 12 };

        /// <summary>
        /// 7セグメント表示[:]の下の点を描画するポリゴン定義です。
        /// </summary>
        private float[] LED_CL = { 31, 65, 12, 12 };

        /// <summary>
        /// 7セグメント表示[.]を描画するポリゴン定義です。
        /// </summary>
        private float[] LED_PI = { 67, 87, 12, 12 };

        /// <summary>
        /// 残り時間です。
        /// </summary>
        private double rtime = 0;

        /// <summary>
        /// 経過時間です。
        /// </summary>
        private double eltime = 0;

        /// <summary>
        /// タイマーIDです。
        /// </summary>
        private int timerId = 0;

        /// <summary>
        /// コールバック関数(アンマネージ)です。
        /// </summary>
        private GCHandle callback;

        #endregion

        #region プロパティ

        #region 追加のプロパティ

        #region DefaultTimeプロパティ

        private double defaultTime = 500;
        /// <summary>
        /// リセット時のタイマー値を参照または設定します。mmss.nnn(分秒.1/100秒) の形式で指定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(500),
            Description("リセット時のタイマー値を参照または設定します。mmss.nnn(分秒.1/100秒) の形式で指定します。")
        ]
        public double DefaultTime
        {
            get
            {
                return defaultTime;
            }
            set
            {
                double wk = value;
                if (defaultTime != wk)
                {
                    if (wk == -1)
                    {
                        defaultTime = -1;   // [-]表示
                    }
                    else
                    {
                        defaultTime = NumTimeCheck(wk, 0);
                    }
                    if (autoReset) Reset();
                    Linkage();
                }
            }
        }

        #endregion

        #region ElapsedTimeプロパティ

        /// <summary>
        /// 経過時間を参照します。mmss.nnn(分秒.1/100秒) の形式です。
        /// </summary>
        [
            Category("状態"),
            Description("経過時間を参照します。mmss.nnn(分秒.1/100秒) の形式です。")
        ]
        public double ElapsedTime
        {
            get
            {
                return eltime;
            }
        }

        #endregion

        #region RemainingTimeプロパティ

        /// <summary>
        /// 残り時間を参照します。mmss.nnn(分秒.1/100秒) の形式です。
        /// </summary>
        [
            Category("状態"),
            Description("残り時間を参照します。mmss.nnn(分秒.1/100秒) の形式です。")
        ]
        public double RemainingTime
        {
            get
            {
                return rtime;
            }
        }

        #endregion

        #region FixedRatioプロパティ

        private bool fixedRatio = false;
        /// <summary>
        /// タイマー表示の縦横比を1:1に固定化するかどうかを参照または設定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(false),
            Description("タイマー表示の縦横比を1:1に固定化するかどうかを参照または設定します。")
        ]
        public bool FixedRatio
        {
            get
            {
                return fixedRatio;
            }
            set
            {
                fixedRatio = value;
                OnLayout(new LayoutEventArgs(this, "Height"));
            }
        }
        
        #endregion

        #region DisplayOffColorプロパティ

        private Color displayOffColor = Color.Gainsboro;
        /// <summary>
        /// タイマー表示部の消灯時の色を参照または設定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(Color), "Gainsboro"),
            Description("タイマー表示部の消灯時の色を参照または設定します。")
        ]
        public Color DisplayOffColor
        {
            get
            {
                return displayOffColor;
            }
            set
            {
                displayOffColor = value;
                // 再描画
                base.Invalidate();
            }
        }

        #endregion

        #region DisplayOnColorプロパティ

        private Color displayOnColor = Color.Gray;
        /// <summary>
        /// タイマー表示部の点灯時の色を参照または設定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(Color), "Gray"),
            Description("タイマー表示部の点灯時の色を参照または設定します。")
        ]
        public Color DisplayOnColor
        {
            get
            {
                return displayOnColor;
            }
            set
            {
                displayOnColor = value;
                // 再描画
                base.Invalidate();
            }
        }

        #endregion

        #region DisplayShadowColorプロパティ

        private Color displayShadowColor = Color.Black;
        /// <summary>
        /// タイマー表示部の点灯時の影の色を参照または設定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(Color), "Black"),
            Description("タイマー表示部の点灯時の影の色を参照または設定します。")
        ]
        public Color DisplayShadowColor
        {
            get
            {
                return displayShadowColor;
            }
            set
            {
                displayShadowColor = value;
                // 再描画
                base.Invalidate();
            }
        }

        #endregion

        #region ShadowOffsetXプロパティ

        private uint shadowOffsetX = 3;
        /// <summary>
        /// タイマー表示部の影の位置(X方向のシフト量)を参照または設定します。ピクセル単位で指定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(3),
            Description("タイマー表示部の影の位置(X方向のシフト量)を参照または設定します。ピクセル単位で指定します。")
        ]
        public uint ShadowOffsetX
        {
            get
            {
                return shadowOffsetX;
            }
            set
            {
                shadowOffsetX = value;
                // 再描画
                base.Invalidate();
            }
        }

        #endregion

        #region ShadowOffsetYプロパティ

        private uint shadowOffsetY = 3;
        /// <summary>
        /// タイマー表示部の影の位置(Y方向のシフト量)を参照または設定します。ピクセル単位で指定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(3),
            Description("タイマー表示部の影の位置(Y方向のシフト量)を参照または設定します。ピクセル単位で指定します。")
        ]
        public uint ShadowOffsetY
        {
            get
            {
                return shadowOffsetY;
            }
            set
            {
                shadowOffsetY = value;
                // 再描画
                base.Invalidate();
            }
        }

        #endregion

        #region NumberSpaceプロパティ

        private uint numberSpace = 5;
        /// <summary>
        /// タイマー表示部の数字の間隔を参照または設定します。ピクセル単位で指定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(5),
            Description("タイマー表示部の数字の間隔を参照または設定します。ピクセル単位で指定します。")
        ]
        public uint NumberSpace
        {
            get
            {
                return numberSpace;
            }
            set
            {
                numberSpace = value;
                // 再描画
                base.Invalidate();
            }
        }

        #endregion

        #region AdjustmentXプロパティ

        private float aax = 1;
        /// <summary>
        /// タイマー表示部の各セグメントの縦横比を調整します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(float), "1"),
            Description("タイマー表示部の各セグメントの縦横比を調整します。")
        ]
        public float AdjustmentX
        {
            get
            {
                return aax;
            }
            set
            {
                aax = value;

                // 再描画
                base.Invalidate();
            }
        }
        
        #endregion

        #region AdjustmentYプロパティ

        private float aay = 1;
        /// <summary>
        /// タイマー表示部の各セグメントの縦横比を調整します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(float), "1"),
            Description("タイマー表示部の各セグメントの縦横比を調整します。")
        ]
        public float AdjustmentY
        {
            get
            {
                return aay;
            }
            set
            {
                aay = value;

                // 再描画
                base.Invalidate();
            }
        }

        #endregion

        #region AdjustmentCプロパティ

        private float aac = 0;
        /// <summary>
        /// タイマー表示部の各セグメントの上下のつながりを調整します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(float), "0"),
            Description("タイマー表示部の各セグメントの上下のつながりを調整します。")
        ]
        public float AdjustmentC
        {
            get
            {
                return aac;
            }
            set
            {
                aac = value;

                // 再描画
                base.Invalidate();
            }
        }

        #endregion

        #region OneMinuteModeプロパティ

        private bool oneMinuteMode = false;
        /// <summary>
        /// タイマー表示部が1分間表示(00-59)かどうかを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("タイマー表示部が1分間表示(00-59)かどうかを参照または設定します。")
        ]
        public bool OneMinuteMode
        {
            get
            {
                return oneMinuteMode;
            }
            set
            {
                bool wk = value;
                if (oneMinuteMode != wk)
                {
                    oneMinuteMode = wk;
                    // 再描画
                    OnLayout(new LayoutEventArgs(this, "Height"));
                    Linkage();
                }
            }
        }

        #endregion

        #region ClockModeプロパティ

        private bool clockMode = false;
        /// <summary>
        /// 時計表示モードかどうかを参照または設定します。(24時間制のみサポート)
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("時計表示モードかどうかを参照または設定します。(24時間制のみサポート)")
        ]
        public bool ClockMode
        {
            get
            {
                return clockMode;
            }
            set
            {
                bool wk = value;
                if (clockMode != wk)
                {
                    Stop();
                    clockMode = wk;
                    // 再描画
                    base.Invalidate();
                    Linkage();
                }
            }
        }

        #endregion

        #region AutoResetプロパティ

        private bool autoReset = true;
        /// <summary>
        /// DefaultTimeプロパティを設定した場合に自動的にResetを実行するかどうかを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(true),
            Description("DefaultTimeプロパティを設定した場合に自動的にResetを実行するかどうかを参照または設定します。")
        ]
        public bool AutoReset
        {
            get
            {
                return autoReset;
            }
            set
            {
                bool wk = value;
                if (autoReset != wk)
                {
                    autoReset = wk;
                    Linkage();
                }
            }
        }

        #endregion

        #region LinkageTimerViewプロパティ

        private TimerView[] linkageTimerView = null;
        /// <summary>
        /// 連携させる TimerView コントロールのインスタンスを参照または設定します。TimerNotice イベントは連携しません。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(typeof(TimerView[]), null),
            Description("連携させる TimerView コントロールのインスタンスを参照または設定します。TimerNotice イベントは連携しません。")
        ]
        public TimerView[] LinkageTimerView
        {
            get
            {
                return linkageTimerView;
            }
            set
            {
                linkageTimerView = value;
            }
        }

        #endregion

        #region ElapsedTimeModeプロパティ

        private bool elapsedTimeMode = false;
        /// <summary>
        /// 残り時間ではなく経過時間を表示するかどうかを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("残り時間ではなく経過時間を表示するかどうかを参照または設定します。")
        ]
        public bool ElapsedTimeMode
        {
            get
            {
                return elapsedTimeMode;
            }
            set
            {
                bool wk = value;
                if (elapsedTimeMode != wk)
                {
                    elapsedTimeMode = wk;
                    // 再描画
                    base.Invalidate();
                }
            }
        }

        #endregion

        #region Timeプロパティ(private)

        /// <summary>
        /// 現在の残り時間を参照または設定します。TimerNotice イベントは発生しません。
        /// </summary>
        [
            Category("状態"),
            DefaultValue(typeof(TimerView[]), null),
            Description("現在の残り時間を参照または設定します。TimerNotice イベントは発生しません。")
        ]
        private double Time
        {
            get
            {
                return rtime;
            }
            set
            {
                double wk = value;
                if (rtime != wk)
                {
                    rtime = wk;
                    eltime = DiffTime(defaultTime, rtime);
                    // 再描画
                    base.Invalidate();
                    Linkage();
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region イベント

        #region OnLayoutイベント

        private bool inLayout = false;
        /// <summary>
        /// XControls.XTimerView.Layoutイベントを発生させます。
        /// </summary>
        protected override void OnLayout(LayoutEventArgs e)
        {
            if (inLayout) return;
            inLayout = true;

            // 縦横比調整
            if(fixedRatio)
            {
                int len;
                if (oneMinuteMode)
                {
                    len = 2;
                }
                else
                {
                    len = 7;
                }
                if (clockMode)
                {
                    len = 7;
                }
                double xw = base.Width - base.Padding.Left - base.Padding.Right - shadowOffsetX;
                double xw1f = (xw + numberSpace) / len;
                double xw1 = xw1f - numberSpace;
                double xz = xw1 / LED_SIZE_X;
                base.Height = base.Padding.Top + base.Padding.Bottom + (int)shadowOffsetY + (int)(xz * LED_SIZE_Y + 1);
            }

            base.OnLayout(e);
            // 再描画
            base.Invalidate();

            inLayout = false;
        }

        #endregion

        #region OnPaintイベント

        /// <summary>
        /// XControls.XTimerView.Paintイベントを発生させます。
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // スムージング
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            // 背景
            Brush backBrush = new SolidBrush(BackColor);
            e.Graphics.FillRectangle(backBrush, 0, 0, Width, Height);

            // カウンタの描画
            Brush OffBrush = new SolidBrush(displayOffColor);
            Brush OnBrush = new SolidBrush(displayOnColor);
            Brush ShadowBrush = new SolidBrush(displayShadowColor);

            // カウンタ値
            string num;
            int len;
            double numtime = rtime;
            if (elapsedTimeMode) numtime = eltime;
            if (oneMinuteMode)
            {
                num = (numtime + 0.99).ToString("#0.00").PadLeft(8);
                num = num.Substring(3, 2);
                if (defaultTime < 0) num = "--";
                len = 2;
            }
            else
            {
                num = numtime.ToString("#0:00.00").PadLeft(8);
                len = 7;
            }
            if (clockMode)
            {
                num = DateTime.Now.ToLongTimeString().PadLeft(8);
                len = 7;
            }

            // 座標計算
            float yw = base.Height - base.Padding.Top - base.Padding.Bottom - shadowOffsetY;
            float yz = yw / LED_SIZE_Y;
            float yo = base.Padding.Top;

            float xw = base.Width - base.Padding.Left - base.Padding.Right - shadowOffsetX;
            float xw1f = (xw + numberSpace) / len;
            float xw1 = xw1f - numberSpace;
            float xz = xw1 / LED_SIZE_X;
            float xo = base.Padding.Left;

            for (int i = 0; i < num.Length; i++)
            {
                switch (num.Substring(i, 1))
                {
                    case " ":
                        // [88 88 88]の影を含む部分(スペース)
                        DrawNumber(-1, e.Graphics, OffBrush, OnBrush, ShadowBrush, xz, xo, yz, yo);
                        xo += xw1f;
                        break;
                    case "0":
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                    case "8":
                    case "9":
                        // [88 88 88]の影を含む部分
                        DrawNumber(int.Parse(num.Substring(i, 1)), e.Graphics, OffBrush, OnBrush, ShadowBrush, xz, xo, yz, yo);
                        xo += xw1f;
                        break;
                    case "-":
                        // [88 88 88]の影を含む部分
                        DrawNumber(-2, e.Graphics, OffBrush, OnBrush, ShadowBrush, xz, xo, yz, yo);
                        xo += xw1f;
                        break;
                    case ":":
                        // [  :     ]の影の部分
                        Rectangle p_CU = new Rectangle();
                        Rectangle p_CL = new Rectangle();
                        DrawDotPoints(xz, xo - xw1 / 4 + shadowOffsetX, yz, yo + shadowOffsetY, ref p_CU, ref p_CL);
                        e.Graphics.FillEllipse(ShadowBrush, p_CU);
                        e.Graphics.FillEllipse(ShadowBrush, p_CL);
                        // [  :     ]の部分
                        DrawDotPoints(xz, xo - xw1 / 4, yz, yo, ref p_CU, ref p_CL);
                        e.Graphics.FillEllipse(OnBrush, p_CU);
                        e.Graphics.FillEllipse(OnBrush, p_CL);
                        xo += xw1f / 2;
                        break;
                    case ".":
                        // [     .  ]の影の部分
                        Rectangle p_PI = new Rectangle();
                        DrawCommaPoints(xz, xo - xw1 / 1.3 + shadowOffsetX, yz, yo + shadowOffsetY, ref p_PI);
                        e.Graphics.FillEllipse(ShadowBrush, p_PI);
                        // [     .  ]の部分
                        DrawCommaPoints(xz, xo - xw1 / 1.3, yz, yo, ref p_PI);
                        e.Graphics.FillEllipse(OnBrush, p_PI);
                        xo += xw1f / 2;
                        break;
                }
            }
        }

        #endregion

        #region timer_Tickイベント

        private int presec = 0;
        private int redrawc = 0;
        private void timer_Tick(int id, int uiNo, IntPtr user, IntPtr reserved1, IntPtr reserved2)
        {
            if (id == timerId)
            {
                TimerNoticeEventArgs eArgs = new TimerNoticeEventArgs();

                if (clockMode)
                {
                    // 時計モード
                    int sec = DateTime.Now.Second;

                    if (presec != sec)
                    {
                        // 再描画
                        base.Invalidate();
                        Linkage();
                    }

                    presec = sec;
                }
                else
                {
                    // カウントダウン
                    rtime = NumTimeCheck(rtime, -0.01);
                    eltime = NumTimeCheck(eltime, 0.01);
                    if (rtime <= 0)
                    {
                        rtime = 0;
                        if (!clockMode) Stop();

                        eArgs.TimeUp = true;
                    }

                    int wtime = (int)rtime;
                    if (wtime == rtime)
                    {
                        if (TimerNotice != null)
                        {
                            // ハンドラが設定されていたらイベント発生
                            eArgs.RemainingTime = rtime;
                            eArgs.ElapsedTime = eltime;
                            CallTimerNotice(this, eArgs);
                        }
                    }
                    // 再描画
                    if (redrawc <= 0)
                    {
                        base.Invalidate();
                        Linkage();
                        redrawc = 9;
                    }
                    redrawc--;
                }
            }
        }

        #endregion

        #region TimerNoticeイベント

        public delegate void TimerNoticeEventHandler(object sender, TimerNoticeEventArgs e);
        /// <summary>
        /// カウントダウンの状態を通知します。
        /// </summary>
        [
            Category("動作"),
            Description("カウントダウンの状態を通知します。")
        ]
        public event TimerNoticeEventHandler TimerNotice;

        #endregion

        #endregion

        #region メソッド

        #region スタート (Start)

        /// <summary>
        /// タイマーをスタートします。
        /// </summary>
        public void Start()
        {
            if ((rtime > 0 || clockMode) && timerId == 0)
            {
                if (TimerNotice != null)
                {
                    // ハンドラが設定されていたらイベント発生
                    TimerNoticeEventArgs eArgs = new TimerNoticeEventArgs();
                    eArgs.StartUp = true;
                    eArgs.RemainingTime = rtime;
                    eArgs.ElapsedTime = eltime;
                    CallTimerNotice(this, eArgs);
                }

                TickedCallback tick = new TickedCallback(timer_Tick);
                callback = GCHandle.Alloc(tick);
                timerId = TimeSetEvent(10, 0, tick, IntPtr.Zero, TimerEventTypes.Periodic);
                if (this.timerId == 0)
                {
                    this.callback.Free();
                    throw new InvalidOperationException("マルチメディアタイマの初期化に失敗しました。");
                }
            }
        }

        #endregion

        #region ストップ (Stop)

        /// <summary>
        /// タイマーをストップするします。
        /// </summary>
        public void Stop()
        {
            // 再描画
            base.Invalidate();
            Linkage();

            if (timerId != 0)
            {
                TimeKillEvent(timerId);
                if (callback.IsAllocated)
                {
                    callback.Free();
                }
            }
            timerId = 0;

            if (TimerNotice != null)
            {
                // ハンドラが設定されていたらイベント発生
                TimerNoticeEventArgs eArgs = new TimerNoticeEventArgs();
                eArgs.TimeUp = true;
                eArgs.RemainingTime = rtime;
                eArgs.ElapsedTime = eltime;
                CallTimerNotice(this, eArgs);
            }

        }

        #endregion

        #region リセット (Reset)

        /// <summary>
        /// タイマーをリセットします。
        /// </summary>
        public void Reset()
        {
            Stop();
            Time = defaultTime;
            eltime = 0;
            // 再描画
            base.Invalidate();
        }

        #endregion

        #region 分調整 (AdjustMinute)

        /// <summary>
        /// 分を調整します。
        /// </summary>
        /// <param name="count">時間(±分)</param>
        public void AdjustMinute(int count)
        {
            Time = NumTimeCheck(rtime, count * 100);
            // 再描画
            base.Invalidate();
        }

        #endregion

        #region 秒調整 (AdjustSecound)

        /// <summary>
        /// 秒を調整します。
        /// </summary>
        /// <param name="count">時間(±秒)</param>
        public void AdjustSecound(int count)
        {
            Time = NumTimeCheck(rtime, count);
            // 再描画
            base.Invalidate();
        }

        #endregion

        #region 時間引き算 (DiffTime)

        /// <summary>
        /// time1 - time2 を計算します。値はmmss.nnn(分秒.1/100秒) の形式です。
        /// </summary>
        /// <param name="time1">時間</param>
        /// <param name="time2">時間</param>
        public double DiffTime(double time1, double time2)
        {
            return NumTimeCheck(time1, -time2);
        }

        #endregion

        #endregion

        #region 内部処理

        #region 数字を描画する (DrawNumber)

        /// <summary>
        /// 数字を描画します。
        /// </summary>
        /// <param name="num">描画する数値0-9,-1でOFF, -2で[-]</param>
        /// <param name="g">描画用Graphics</param>
        /// <param name="OffBrush">消灯部分の描画ブラシ</param>
        /// <param name="OnBrush">点灯部分の描画ブラシ</param>
        /// <param name="ShadowBrush">点灯影部分の描画ブラシ</param>
        /// <param name="xz">X座標倍率</param>
        /// <param name="xo">X座標オフセット</param>
        /// <param name="yz">Y座標倍率</param>
        /// <param name="yo">Y座標オフセット</param>
        private void DrawNumber(int num, Graphics g, Brush OffBrush, Brush OnBrush, Brush ShadowBrush,
                                float xz, float xo, float yz, float yo)
        {
            // 座標計算
            PointF[] s_UL = new PointF[8];
            PointF[] s_UC = new PointF[8];
            PointF[] s_UR = new PointF[8];
            PointF[] s_CC = new PointF[8];
            PointF[] s_LL = new PointF[8];
            PointF[] s_LC = new PointF[8];
            PointF[] s_LR = new PointF[8];
            DrawNumberPoints(xz, xo + shadowOffsetX, yz, yo + shadowOffsetY, ref s_UL, ref s_UC, ref s_UR, ref s_CC, ref s_LL, ref s_LC, ref s_LR);
            PointF[] p_UL = new PointF[8];
            PointF[] p_UC = new PointF[8];
            PointF[] p_UR = new PointF[8];
            PointF[] p_CC = new PointF[8];
            PointF[] p_LL = new PointF[8];
            PointF[] p_LC = new PointF[8];
            PointF[] p_LR = new PointF[8];
            DrawNumberPoints(xz, xo, yz, yo, ref p_UL, ref p_UC, ref p_UR, ref p_CC, ref p_LL, ref p_LC, ref p_LR);

            switch (num)
            {
                #region 0描画
                case 0:
                    g.FillPolygon(OffBrush, p_CC);
                    g.FillPolygon(ShadowBrush, s_UL);
                    g.FillPolygon(ShadowBrush, s_UC);
                    g.FillPolygon(ShadowBrush, s_UR);
                    g.FillPolygon(ShadowBrush, s_LL);
                    g.FillPolygon(ShadowBrush, s_LC);
                    g.FillPolygon(ShadowBrush, s_LR);
                    g.FillPolygon(OnBrush, p_UL);
                    g.FillPolygon(OnBrush, p_UC);
                    g.FillPolygon(OnBrush, p_UR);
                    g.FillPolygon(OnBrush, p_LL);
                    g.FillPolygon(OnBrush, p_LC);
                    g.FillPolygon(OnBrush, p_LR);
                    break;
                #endregion

                #region 1描画
                case 1:
                    g.FillPolygon(OffBrush, p_UL);
                    g.FillPolygon(OffBrush, p_UC);
                    g.FillPolygon(OffBrush, p_CC);
                    g.FillPolygon(OffBrush, p_LL);
                    g.FillPolygon(OffBrush, p_LC);
                    g.FillPolygon(ShadowBrush, s_UR);
                    g.FillPolygon(ShadowBrush, s_LR);
                    g.FillPolygon(OnBrush, p_UR);
                    g.FillPolygon(OnBrush, p_LR);
                    break;
                #endregion

                #region 2描画
                case 2:
                    g.FillPolygon(OffBrush, p_UL);
                    g.FillPolygon(OffBrush, p_LR);
                    g.FillPolygon(ShadowBrush, s_UC);
                    g.FillPolygon(ShadowBrush, s_UR);
                    g.FillPolygon(ShadowBrush, s_CC);
                    g.FillPolygon(ShadowBrush, s_LL);
                    g.FillPolygon(ShadowBrush, s_LC);
                    g.FillPolygon(OnBrush, p_UC);
                    g.FillPolygon(OnBrush, p_UR);
                    g.FillPolygon(OnBrush, p_CC);
                    g.FillPolygon(OnBrush, p_LL);
                    g.FillPolygon(OnBrush, p_LC);
                    break;
                #endregion

                #region 3描画
                case 3:
                    g.FillPolygon(OffBrush, p_UL);
                    g.FillPolygon(OffBrush, p_LL);
                    g.FillPolygon(ShadowBrush, s_UC);
                    g.FillPolygon(ShadowBrush, s_UR);
                    g.FillPolygon(ShadowBrush, s_CC);
                    g.FillPolygon(ShadowBrush, s_LC);
                    g.FillPolygon(ShadowBrush, s_LR);
                    g.FillPolygon(OnBrush, p_UC);
                    g.FillPolygon(OnBrush, p_UR);
                    g.FillPolygon(OnBrush, p_CC);
                    g.FillPolygon(OnBrush, p_LC);
                    g.FillPolygon(OnBrush, p_LR);
                    break;
                #endregion

                #region 4描画
                case 4:
                    g.FillPolygon(OffBrush, p_UC);
                    g.FillPolygon(OffBrush, p_LL);
                    g.FillPolygon(OffBrush, p_LC);
                    g.FillPolygon(ShadowBrush, s_UL);
                    g.FillPolygon(ShadowBrush, s_UR);
                    g.FillPolygon(ShadowBrush, s_CC);
                    g.FillPolygon(ShadowBrush, s_LR);
                    g.FillPolygon(OnBrush, p_UL);
                    g.FillPolygon(OnBrush, p_UR);
                    g.FillPolygon(OnBrush, p_CC);
                    g.FillPolygon(OnBrush, p_LR);
                    break;
                #endregion

                #region 5描画
                case 5:
                    g.FillPolygon(OffBrush, p_UR);
                    g.FillPolygon(OffBrush, p_LL);
                    g.FillPolygon(ShadowBrush, s_UL);
                    g.FillPolygon(ShadowBrush, s_UC);
                    g.FillPolygon(ShadowBrush, s_CC);
                    g.FillPolygon(ShadowBrush, s_LC);
                    g.FillPolygon(ShadowBrush, s_LR);
                    g.FillPolygon(OnBrush, p_UL);
                    g.FillPolygon(OnBrush, p_UC);
                    g.FillPolygon(OnBrush, p_CC);
                    g.FillPolygon(OnBrush, p_LC);
                    g.FillPolygon(OnBrush, p_LR);
                    break;
                #endregion

                #region 6描画
                case 6:
                    g.FillPolygon(OffBrush, p_UR);
                    g.FillPolygon(ShadowBrush, s_UL);
                    g.FillPolygon(ShadowBrush, s_UC);
                    g.FillPolygon(ShadowBrush, s_CC);
                    g.FillPolygon(ShadowBrush, s_LL);
                    g.FillPolygon(ShadowBrush, s_LC);
                    g.FillPolygon(ShadowBrush, s_LR);
                    g.FillPolygon(OnBrush, p_UL);
                    g.FillPolygon(OnBrush, p_UC);
                    g.FillPolygon(OnBrush, p_CC);
                    g.FillPolygon(OnBrush, p_LL);
                    g.FillPolygon(OnBrush, p_LC);
                    g.FillPolygon(OnBrush, p_LR);
                    break;
                #endregion

                #region 7描画
                case 7:
                    g.FillPolygon(OffBrush, p_UL);
                    g.FillPolygon(OffBrush, p_CC);
                    g.FillPolygon(OffBrush, p_LL);
                    g.FillPolygon(OffBrush, p_LC);
                    g.FillPolygon(ShadowBrush, s_UC);
                    g.FillPolygon(ShadowBrush, s_UR);
                    g.FillPolygon(ShadowBrush, s_LR);
                    g.FillPolygon(OnBrush, p_UC);
                    g.FillPolygon(OnBrush, p_UR);
                    g.FillPolygon(OnBrush, p_LR);
                    break;
                #endregion

                #region 8描画
                case 8:
                    g.FillPolygon(ShadowBrush, s_UL);
                    g.FillPolygon(ShadowBrush, s_UC);
                    g.FillPolygon(ShadowBrush, s_UR);
                    g.FillPolygon(ShadowBrush, s_CC);
                    g.FillPolygon(ShadowBrush, s_LL);
                    g.FillPolygon(ShadowBrush, s_LC);
                    g.FillPolygon(ShadowBrush, s_LR);
                    g.FillPolygon(OnBrush, p_UL);
                    g.FillPolygon(OnBrush, p_UC);
                    g.FillPolygon(OnBrush, p_UR);
                    g.FillPolygon(OnBrush, p_CC);
                    g.FillPolygon(OnBrush, p_LL);
                    g.FillPolygon(OnBrush, p_LC);
                    g.FillPolygon(OnBrush, p_LR);
                    break;
                #endregion

                #region 9描画
                case 9:
                    g.FillPolygon(OffBrush, p_LL);
                    g.FillPolygon(ShadowBrush, s_UL);
                    g.FillPolygon(ShadowBrush, s_UC);
                    g.FillPolygon(ShadowBrush, s_UR);
                    g.FillPolygon(ShadowBrush, s_CC);
                    g.FillPolygon(ShadowBrush, s_LC);
                    g.FillPolygon(ShadowBrush, s_LR);
                    g.FillPolygon(OnBrush, p_UL);
                    g.FillPolygon(OnBrush, p_UC);
                    g.FillPolygon(OnBrush, p_UR);
                    g.FillPolygon(OnBrush, p_CC);
                    g.FillPolygon(OnBrush, p_LC);
                    g.FillPolygon(OnBrush, p_LR);
                    break;
                #endregion

                #region OFF描画
                case -1:
                    g.FillPolygon(OffBrush, p_UL);
                    g.FillPolygon(OffBrush, p_UC);
                    g.FillPolygon(OffBrush, p_UR);
                    g.FillPolygon(OffBrush, p_CC);
                    g.FillPolygon(OffBrush, p_LL);
                    g.FillPolygon(OffBrush, p_LC);
                    g.FillPolygon(OffBrush, p_LR);
                    break;
                #endregion

                #region [-]描画
                case -2:
                    g.FillPolygon(OffBrush, p_UL);
                    g.FillPolygon(OffBrush, p_UC);
                    g.FillPolygon(OffBrush, p_UR);
                    g.FillPolygon(OnBrush, p_CC);
                    g.FillPolygon(OffBrush, p_LL);
                    g.FillPolygon(OffBrush, p_LC);
                    g.FillPolygon(OffBrush, p_LR);
                    break;
                #endregion
            }
        }

        #endregion

        #region 描画用に座標を計算する (DrawNumberPoints)

        /// <summary>
        /// 描画のための座標を計算します。
        /// </summary>
        /// <param name="xz">X座標倍率</param>
        /// <param name="xo">X座標オフセット</param>
        /// <param name="yz">Y座標倍率</param>
        /// <param name="yo">Y座標オフセット</param>
        /// <param name="UL">７セグメント・上部左側パタン</param>
        /// <param name="UC">７セグメント・上部中央パタン</param>
        /// <param name="UR">７セグメント・上部右側パタン</param>
        /// <param name="CC">７セグメント・中央部パタン</param>
        /// <param name="LL">７セグメント・下部左側パタン</param>
        /// <param name="LC">７セグメント・下部中央パタン</param>
        /// <param name="LR">７セグメント・下部右側パタン</param>
        private void DrawNumberPoints(float xz, float xo, float yz, float yo,
                                      ref PointF[] p_UL, ref PointF[] p_UC, ref PointF[] p_UR, ref PointF[] p_CC,
                                      ref PointF[] p_LL, ref PointF[] p_LC, ref PointF[] p_LR)
        {
            p_UL[0] = new PointF((LED_UL[0] * xz + xo), (LED_UL[1] * yz + yo));
            p_UL[1] = new PointF((LED_UL[2] * xz + xo), (LED_UL[3] * yz + yo));
            p_UL[2] = new PointF((LED_UL[4] * xz + xo), (LED_UL[5] * yz + yo));
            p_UL[3] = new PointF((LED_UL[6] * xz + xo), (LED_UL[7] * yz + yo));
            p_UL[4] = new PointF((LED_UL[8] * xz + xo), (LED_UL[9] * yz + yo));
            p_UL[5] = new PointF((LED_UL[10] * xz + xo), (LED_UL[11] * yz + yo));
            p_UL[6] = new PointF((LED_UL[12] * xz + xo), (LED_UL[13] * yz + yo));
            p_UL[7] = new PointF((LED_UL[14] * xz + xo), (LED_UL[15] * yz + yo));
            DrawNumberAdjust(ref p_UL, false, 1);

            p_UC[0] = new PointF((LED_UC[0] * xz + xo), (LED_UC[1] * yz + yo));
            p_UC[1] = new PointF((LED_UC[2] * xz + xo), (LED_UC[3] * yz + yo));
            p_UC[2] = new PointF((LED_UC[4] * xz + xo), (LED_UC[5] * yz + yo));
            p_UC[3] = new PointF((LED_UC[6] * xz + xo), (LED_UC[7] * yz + yo));
            p_UC[4] = new PointF((LED_UC[8] * xz + xo), (LED_UC[9] * yz + yo));
            p_UC[5] = new PointF((LED_UC[10] * xz + xo), (LED_UC[11] * yz + yo));
            p_UC[6] = new PointF((LED_UC[12] * xz + xo), (LED_UC[13] * yz + yo));
            p_UC[7] = new PointF((LED_UC[14] * xz + xo), (LED_UC[15] * yz + yo));
            DrawNumberAdjust(ref p_UC, true, 1);

            p_UR[0] = new PointF((LED_UR[0] * xz + xo), (LED_UR[1] * yz + yo));
            p_UR[1] = new PointF((LED_UR[2] * xz + xo), (LED_UR[3] * yz + yo));
            p_UR[2] = new PointF((LED_UR[4] * xz + xo), (LED_UR[5] * yz + yo));
            p_UR[3] = new PointF((LED_UR[6] * xz + xo), (LED_UR[7] * yz + yo));
            p_UR[4] = new PointF((LED_UR[8] * xz + xo), (LED_UR[9] * yz + yo));
            p_UR[5] = new PointF((LED_UR[10] * xz + xo), (LED_UR[11] * yz + yo));
            p_UR[6] = new PointF((LED_UR[12] * xz + xo), (LED_UR[13] * yz + yo));
            p_UR[7] = new PointF((LED_UR[14] * xz + xo), (LED_UR[15] * yz + yo));
            DrawNumberAdjust(ref p_UR, false, 1);

            p_CC[0] = new PointF((LED_CC[0] * xz + xo), (LED_CC[1] * yz + yo));
            p_CC[1] = new PointF((LED_CC[2] * xz + xo), (LED_CC[3] * yz + yo));
            p_CC[2] = new PointF((LED_CC[4] * xz + xo), (LED_CC[5] * yz + yo));
            p_CC[3] = new PointF((LED_CC[6] * xz + xo), (LED_CC[7] * yz + yo));
            p_CC[4] = new PointF((LED_CC[8] * xz + xo), (LED_CC[9] * yz + yo));
            p_CC[5] = new PointF((LED_CC[10] * xz + xo), (LED_CC[11] * yz + yo));
            p_CC[6] = new PointF((LED_CC[12] * xz + xo), (LED_CC[13] * yz + yo));
            p_CC[7] = new PointF((LED_CC[14] * xz + xo), (LED_CC[15] * yz + yo));
            DrawNumberAdjust(ref p_CC, true, 0);

            p_LL[0] = new PointF((LED_LL[0] * xz + xo), (LED_LL[1] * yz + yo));
            p_LL[1] = new PointF((LED_LL[2] * xz + xo), (LED_LL[3] * yz + yo));
            p_LL[2] = new PointF((LED_LL[4] * xz + xo), (LED_LL[5] * yz + yo));
            p_LL[3] = new PointF((LED_LL[6] * xz + xo), (LED_LL[7] * yz + yo));
            p_LL[4] = new PointF((LED_LL[8] * xz + xo), (LED_LL[9] * yz + yo));
            p_LL[5] = new PointF((LED_LL[10] * xz + xo), (LED_LL[11] * yz + yo));
            p_LL[6] = new PointF((LED_LL[12] * xz + xo), (LED_LL[13] * yz + yo));
            p_LL[7] = new PointF((LED_LL[14] * xz + xo), (LED_LL[15] * yz + yo));
            DrawNumberAdjust(ref p_LL, false, -1);

            p_LC[0] = new PointF((LED_LC[0] * xz + xo), (LED_LC[1] * yz + yo));
            p_LC[1] = new PointF((LED_LC[2] * xz + xo), (LED_LC[3] * yz + yo));
            p_LC[2] = new PointF((LED_LC[4] * xz + xo), (LED_LC[5] * yz + yo));
            p_LC[3] = new PointF((LED_LC[6] * xz + xo), (LED_LC[7] * yz + yo));
            p_LC[4] = new PointF((LED_LC[8] * xz + xo), (LED_LC[9] * yz + yo));
            p_LC[5] = new PointF((LED_LC[10] * xz + xo), (LED_LC[11] * yz + yo));
            p_LC[6] = new PointF((LED_LC[12] * xz + xo), (LED_LC[13] * yz + yo));
            p_LC[7] = new PointF((LED_LC[14] * xz + xo), (LED_LC[15] * yz + yo));
            DrawNumberAdjust(ref p_LC, true, -1);

            p_LR[0] = new PointF((LED_LR[0] * xz + xo), (LED_LR[1] * yz + yo));
            p_LR[1] = new PointF((LED_LR[2] * xz + xo), (LED_LR[3] * yz + yo));
            p_LR[2] = new PointF((LED_LR[4] * xz + xo), (LED_LR[5] * yz + yo));
            p_LR[3] = new PointF((LED_LR[6] * xz + xo), (LED_LR[7] * yz + yo));
            p_LR[4] = new PointF((LED_LR[8] * xz + xo), (LED_LR[9] * yz + yo));
            p_LR[5] = new PointF((LED_LR[10] * xz + xo), (LED_LR[11] * yz + yo));
            p_LR[6] = new PointF((LED_LR[12] * xz + xo), (LED_LR[13] * yz + yo));
            p_LR[7] = new PointF((LED_LR[14] * xz + xo), (LED_LR[15] * yz + yo));
            DrawNumberAdjust(ref p_LR, false, -1);
        }

        #endregion

        #region 描画のための座標を計算する (DrawNumberAdjust)

        /// <summary>
        /// 描画のための座標を計算します。
        /// </summary>
        /// <param name="pt">７セグメントパタン</param>
        /// <param name="fixwidth">幅を固定化するならtrue, 高さを固定化するならfalse</param>
        /// <param name="c">上下接続の補正方向(-1=左,+1=右</param>
        private void DrawNumberAdjust(ref PointF[] pt, bool fixwidth, float c)
        {
            float x = 0, y = 0, cx, cy;
            int i;

            // 中心を求める
            for (i = 0; i < pt.Length; i++)
            {
                x += pt[i].X;
                y += pt[i].Y;
            }
            x /= pt.Length;
            y /= pt.Length;

            for (i = 0; i < pt.Length; i++)
            {
                cx = x - pt[i].X + c * aac;
                cy = y - pt[i].Y;
                if (fixwidth)
                {
                    // 幅固定
                    cy *= aay;
                }
                else
                {
                    // 高さ固定
                    cx *= aax;
                }
                pt[i] = new PointF(x + cx, y + cy);
            }
        }

        #endregion

        #region 描画のための座標を計算する (DrawDotPoints)

        /// <summary>
        /// 描画のための座標を計算します。
        /// </summary>
        /// <param name="xz">X座標倍率</param>
        /// <param name="xo">X座標オフセット</param>
        /// <param name="yz">Y座標倍率</param>
        /// <param name="yo">Y座標オフセット</param>
        /// <param name="p_CU">７セグメント・コロン上部パタン</param>
        /// <param name="p_CL">７セグメント・コロン下部パタン</param>
        private void DrawDotPoints(double xz, double xo, double yz, double yo,
                                      ref Rectangle p_CU, ref Rectangle p_CL)
        {
            p_CU = DrawRectAdjust((LED_CU[0] * xz + xo), (LED_CU[1] * yz + yo),
                                 (LED_CU[2] * xz), (LED_CU[3] * yz));
            p_CL = DrawRectAdjust((LED_CL[0] * xz + xo), (LED_CL[1] * yz + yo),
                                 (LED_CL[2] * xz), (LED_CL[3] * yz));
        }

        #endregion

        #region 描画のための座標を計算する (DrawCommaPoints)

        /// <summary>
        /// 描画のための座標を計算します。
        /// </summary>
        /// <param name="xz">X座標倍率</param>
        /// <param name="xo">X座標オフセット</param>
        /// <param name="yz">Y座標倍率</param>
        /// <param name="yo">Y座標オフセット</param>
        /// <param name="p_PI">７セグメント・小数点パタン</param>
        private void DrawCommaPoints(double xz, double xo, double yz, double yo,
                                      ref Rectangle p_PI)
        {
            p_PI = DrawRectAdjust((LED_PI[0] * xz + xo), (LED_PI[1] * yz + yo),
                                 (LED_PI[2] * xz), (LED_PI[3] * yz));
        }

        #endregion

        #region 描画のための座標を計算する (DrawRectAdjust)

        /// <summary>
        /// 描画のための座標を計算します。
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="w">Xサイズ</param>
        /// <param name="h">Yサイズ</param>
        /// <returns>描画範囲</returns>
        private Rectangle DrawRectAdjust(double x, double y, double w, double h)
        {
            double c;

            c = x + w / 2;
            w *= aax;
            x = c - w / 2;

            c = y + h / 2;
            h *= aay;
            y = c - h / 2;

            return new Rectangle((int)x, (int)y, (int)w, (int)h);
        }

        #endregion

        #region 時間チェック (NumTimeCheck)

        /// <summary>
        /// 数値を時間としてチェックして「mmss.nnn(分秒.1/100秒)」にする
        /// </summary>
        /// <param name="num">補正する数値</param>
        /// <param name="cal">加算する調整用数値</param>
        /// <returns>補正後の数値</returns>
        public double NumTimeCheck(double num, double cal)
        {
            double wnum = Math.Abs(num);

            int m = (int)(wnum / 100);
            int s = ((int)wnum) - m * 100;
            int n = ((int)(wnum * 100 + .5)) - m * 10000 - s * 100;
            int mc = (int)(cal / 100);
            int sc = ((int)cal) - mc * 100;
            int nc = ((int)(cal * 100)) - mc * 10000 - sc * 100;

            m += mc;
            s += sc;
            n += nc;

            if (n < 0)
            {
                n += 100;
                s--;
            }
            if (s > 59)
            {
                s -= 60;
                m++;
            }
            if (s < 0)
            {
                s += 60;
                m--;
            }
            while (m > 59)
            {
                m -= 60;
            }
            while (m < 0)
            {
                m += 60;
            }

            wnum = m * 10000 + s * 100 + n;
            return (wnum / 100);
        }

        #endregion

        #region コントロール連携 (Linkage)

        /// <summary>
        /// LinkageTimerView プロパティに設定されたコントロールに、状態を反映します。
        /// </summary>
        private void Linkage()
        {
            if (linkageTimerView == null) return;
            foreach(TimerView tv in linkageTimerView)
            {
                tv.DefaultTime = this.DefaultTime;
                tv.OneMinuteMode = this.OneMinuteMode;
                tv.ClockMode = this.ClockMode;
                if(this.clockMode)
                {
                    tv.Time = DateTime.Now.Second;
                }
                else
                {
                    tv.Time = this.Time;
                }
            }
        }

        #endregion

        #region タイマー通知イベント呼び出し (CallTimerNotice)

        delegate void CallTimerNoticeDelegate(object sender, TimerNoticeEventArgs e);

        private void CallTimerNotice(object sender, TimerNoticeEventArgs e)
        {
            if (InvokeRequired)
            {
                // 別スレッドから呼び出された場合
                object[] param = { sender, e };
                Invoke(new CallTimerNoticeDelegate(CallTimerNotice), param);
                return;
            }

            try
            {
                TimerNotice(sender, e);
            }
            catch
            {
            }
        }

        #endregion

        #region マルチメディアタイマー関連 (WinAPI)

        private delegate void TickedCallback(int id, int uiNo, IntPtr user, IntPtr reserved1, IntPtr reserved2);

        [DllImport("winmm.dll", EntryPoint = "timeSetEvent")]
        private static extern int TimeSetEvent(int delay, int resolution, TickedCallback ticked, IntPtr user, TimerEventTypes type);
        [DllImport("winmm.dll", EntryPoint = "timeKillEvent")]
        private static extern int TimeKillEvent(int id);
        [Flags]
        private enum TimerEventTypes : int
        {
            OneShot = 0x00,
            Periodic = 0x01,
        }

        #endregion

        #endregion
    }
}
