using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace XControls.UI
{
    /// <summary>
    /// Labelの自動スクロール拡張版コントロールです。
    /// </summary>
    public class ScrollLabel : Label
    {
        #region 構築・破棄

        /// <summary>
        /// XControls.UI.ScrollLabel クラスの新しいインスタンスを初期化します。
        /// </summary>
        public ScrollLabel()
            : base()
        {
            this.Enabled = this.Enabled;
        }

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            #region 後処理

            this.Enabled = false;
            base.Dispose(disposing);

            #endregion
        }

        #endregion

        #region 変数

        /// <summary>
        /// タイマーIDです。
        /// </summary>
        private int timerId = 0;

        /// <summary>
        /// 経過時間をカウントします。
        /// </summary>
        private float timecount = 0;

        /// <summary>
        /// 最大カウント値を保持します。
        /// </summary>
        private float timemax = 0;

        /// <summary>
        /// 描画位置(Y座標)です。
        /// </summary>
        private float drawY = 0;

        /// <summary>
        /// コールバック関数(アンマネージ)です。
        /// </summary>
        private GCHandle callback;

        #endregion

        #region プロパティ

        #region 追加のプロパティ
 
        #region PauseTimeプロパティ

        private float pauseTime = 3000;
        /// <summary>
        /// スクロール開始までの時間を参照または設定します。単位はミリ秒です。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(3000F),
            Description("スクロール開始までの時間を参照または設定します。単位はミリ秒です。")
        ]
        public float PauseTime
        {
            get
            {
                return pauseTime;
            }
            set
            {
                pauseTime = value;
                if (pauseTime < 0) PauseTime = 0;
                SetMove();
            }
        }

        #endregion

        #region MoveIntervalプロパティ

        private int moveInterval = 50;
        /// <summary>
        /// スクロール時の描画間隔を参照または設定します。単位はミリ秒です。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(50),
            Description("スクロール時の描画間隔を参照または設定します。単位はミリ秒です。")
        ]
        public int MoveInterval
        {
            get
            {
                return moveInterval;
            }
            set
            {
                moveInterval = value;
                if (moveInterval < 1) MoveInterval = 1;
                if (base.Enabled)
                {
                    Stop();
                    Start();
                }
            }
        }

        #endregion

        #region MoveSpeedプロパティ

        private float moveSpeed = 50;
        /// <summary>
        /// スクロール速度を参照または設定します。単位はピクセル数/秒です。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(50F),
            Description("スクロール速度を参照または設定します。単位はピクセル数/秒です。")
        ]
        public float MoveSpeed
        {
            get
            {
                return moveSpeed;
            }
            set
            {
                moveSpeed = value;
                if (moveSpeed < 0) moveSpeed = 0;
                SetMove();
            }
        }

        #endregion

        #endregion

        #region 既存のプロパティ（上書き）

        #region Enabledプロパティ

        /// <summary>
        /// テキストをスクロールさせるかどうかを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(true),
            Description("テキストをスクロールさせるかどうかを参照または設定します。")
        ]
        public new bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                bool wk = value;
                if (base.Enabled != wk)
                {
                    base.Enabled = wk;
                    if (!DesignMode)
                    {
                        if (base.Enabled)
                        {
                            Start();
                        }
                        else
                        {
                            Stop();
                            timecount = 0;
                        }
                        SetMove();
                        Invalidate();
                    }
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region イベント

        #region OnLayoutイベント

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            SetMove();
        }

        #endregion

        #region OnTextChangedイベント

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            SetMove();
        }

        #endregion

        #region OnPaintイベント

        protected override void OnPaint(PaintEventArgs e)
        {
            DrawLabel(e);
        }

        #endregion

        #region timer_Tickイベント

        private void timer_Tick(int id, int uiNo, IntPtr user, IntPtr reserved1, IntPtr reserved2)
        {
            if (id == timerId)
            {
                timecount += moveInterval;
                if (timecount > timemax + 1000 / moveInterval) timecount = 0;
                Invalidate();
            }
        }

        #endregion

        #endregion

        #region 内部処理

        #region スタート (Start)

        /// <summary>
        /// タイマーをスタートする
        /// </summary>
        private void Start()
        {
            if (timerId == 0)
            {
                TickedCallback tick = new TickedCallback(timer_Tick);
                callback = GCHandle.Alloc(tick);
                timerId = TimeSetEvent(moveInterval, 0, tick, IntPtr.Zero, TimerEventTypes.Periodic);
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
        /// タイマーをストップする
        /// </summary>
        private void Stop()
        {
            if (timerId != 0)
            {
                TimeKillEvent(timerId);
                if (callback.IsAllocated)
                {
                    callback.Free();
                }
            }
            timerId = 0;
        }

        #endregion

        #region 移動を設定 (SetMove)

        /// <summary>
        /// 移動を設定します。
        /// </summary>
        private void SetMove()
        {
            // カウントリセット
            timecount = 0;

            // 最大カウント値計算
            timemax = 0;

            try
            {
                Graphics g = this.CreateGraphics();

                SizeF tsize = g.MeasureString(this.Text, this.Font);
                if (tsize.Width > this.Width)
                {
                    string tx = this.Text + "  " + this.Text.Substring(0, 1);
                    StringFormat sf = new StringFormat();
                    sf.FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.NoWrap;
                    sf.SetMeasurableCharacterRanges(new CharacterRange[] { new CharacterRange(0, tx.Length - 1) });
                    tsize = g.MeasureString(tx, this.Font);
                    Region[] rg = g.MeasureCharacterRanges(tx, this.Font, new RectangleF(new PointF(0F, 0F), tsize), sf);
                    RectangleF rc = rg[rg.Length - 1].GetBounds(g);
                    timemax = pauseTime + (int)(rc.Width * 1000F / (float)moveSpeed);
                }
                drawY = (this.Height - (int)tsize.Height) / 2;

                g.Dispose();
            }
            catch
            {
            }
        }

        #endregion

        #region ラベルを描画 (DrawLabel)

        /// <summary>
        /// ラベルを描画します。
        /// </summary>
        private void DrawLabel(PaintEventArgs e)
        {
            if (IsDisposed) return;

            Graphics g = e.Graphics;

            g.FillRectangle(new SolidBrush(this.BackColor), this.Bounds);
            if (timemax == 0)
            {
                g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), 0, drawY);
            }
            else
            {
                int x = 0;
                if (timecount >= pauseTime)
                {
                    x = (int)Math.Round(-(timecount - pauseTime) * moveSpeed / 1000);
                }
                g.DrawString(this.Text + "  " + this.Text, this.Font, new SolidBrush(this.ForeColor), x, drawY);
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
