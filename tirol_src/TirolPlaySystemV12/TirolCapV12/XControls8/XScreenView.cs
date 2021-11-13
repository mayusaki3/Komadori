using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace XControls.UI
{
    /// <summary>
    /// 指定したスクリーンのイメージを表示するコントロールです。
    /// </summary>
    [Designer(typeof(XScreenViewDesigner))]
    public class XScreenView : Panel
    {
        #region インナークラス

        #region XScreenViewDesignerクラス

        /// <summary>
        /// XScreenView用にデザイナをカスタマイズします。
        /// </summary>
        public class XScreenViewDesigner : System.Windows.Forms.Design.ControlDesigner
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.XScreenView.XScreenViewDesigner クラスの新しいインスタンスを初期化します。
            /// </summary>
            public XScreenViewDesigner()
            {
            }

            #endregion

            #region メソッド

            #region PostFilterPropertiesメソッド

            protected override void PostFilterProperties(IDictionary properties)
            {
                // フィルタリングするプロパティ
                //properties.Remove("PropertyName");

                base.PostFilterProperties(properties);
            }

            #endregion

            #endregion
        }

        #endregion

        #endregion

        #region 構築・破棄

        /// <summary>
        /// XControls.XScreenView クラスの新しいインスタンスを初期化します。
        /// </summary>
        public XScreenView()
            : base()
        {
            #region 初期化

            // 描画のダブルバッファを有効にする
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            components = new System.ComponentModel.Container();
            timDraw = new System.Windows.Forms.Timer(this.components);
            timDraw.Interval = 500;
            timDraw.Tick += new System.EventHandler(this.timDraw_Tick);

            moveCursor = CreateCursor(XControls.XResource.XIconMove.ToBitmap(), 13, 13);
            zoomCursor = CreateCursor(XControls.XResource.XIconZoom.ToBitmap(), 13, 13);

            #endregion
        }

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            #region 後処理

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            #endregion
        }

        #endregion

        #region 定数

        /// <summary>
        /// ズーム最大値
        /// </summary>
        const int ZOOM_MAX = 1600;

        #endregion

        #region 変数

        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 表示更新用のタイマーです。
        /// </summary>
        private System.Windows.Forms.Timer timDraw = null;

        /// <summary>
        /// スクリーン描画用のビットマップ
        /// </summary>
        private Bitmap bitmap = null;

        #region ズーム・スクロール操作用

        /// <summary>
        /// ズーム操作時のカーソルです。
        /// </summary>
        private Cursor zoomCursor = null;

        /// <summary>
        /// スクロール操作時のカーソルです。
        /// </summary>
        private Cursor moveCursor = null;

        /// <summary>
        /// カーソルを退避します。
        /// </summary>
        System.Windows.Forms.Cursor cursor = null;

        /// <summary>
        /// スクロール操作中かを表します。
        /// </summary>
        bool isMove = false;

        /// <summary>
        /// ホバー中のマウスカーソル位置のX座標です。
        /// </summary>
        int hoverX = 0;

        /// <summary>
        /// ホバー中のマウスカーソル位置のY座標です。
        /// </summary>
        int hoverY = 0;

        /// <summary>
        /// 最後に移動したマウスカーソル位置のY座標です。
        /// </summary>
        int lastX = 0;

        /// <summary>
        /// 最後に移動したマウスカーソル位置のY座標です。
        /// </summary>
        int lastY = 0;

        /// <summary>
        /// 現在のビットマップのサイズXです。
        /// </summary>
        int sizeX = 0;

        /// <summary>
        /// 現在のビットマップのサイズYです。
        /// </summary>
        int sizeY = 0;

        #endregion

        #endregion

        #region プロパティ

        #region 追加のプロパティ

        #region ScreenIndexプロパティ

        private int screenIndex = 0;
        /// <summary>
        /// 表示するスクリーンのインデックスを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            Description("表示するスクリーンのインデックスを参照または設定します。")
        ]
        public int ScreenIndex
        {
            get
            {
                return screenIndex;
            }
            set
            {
                int scrix = value;
                if (scrix < 0) scrix = 0;
                if (Screen.AllScreens.Length <= scrix) scrix = 0;
                bool chg = (screenIndex != scrix);
                if (chg || bitmap == null)
                {
                    screenIndex = scrix;
                    Screen scr = Screen.AllScreens[screenIndex];
                    bitmap = new Bitmap(scr.Bounds.Width, scr.Bounds.Height);
                }
            }
        }

        #endregion

        #region MouseZoomプロパティ

        private bool mouseZoom = false;
        /// <summary>
        /// マウス操作でビットマップの拡大縮小表示を行うかを参照または設定します。BackgroundImageLayoutがZoomの場合に有効ですが、カメラ画像には影響しません。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
                    Description("マウス操作でビットマップの拡大縮小表示を行うかを参照または設定します。BackgroundImageLayoutがZoomの場合に有効ですが、カメラ画像には影響しません。")
        ]
        public bool MouseZoom
        {
            get
            {
                return mouseZoom;
            }
            set
            {
                mouseZoom = value;
            }
        }

        #endregion

        #region Zoomプロパティ

        private UInt16 zoom = 100;
        /// <summary>
        /// 表示するビットマップの拡大率を参照または設定します。BackgroundImageLayoutがZoomの場合に有効ですが、カメラ画像には影響しません。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(100),
                    Description("表示するビットマップの拡大率を参照または設定します。BackgroundImageLayoutがZoomの場合に有効ですが、カメラ画像には影響しません。")
        ]
        public UInt16 Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                if (value < 100 || value > ZOOM_MAX)
                {
                    throw new ArgumentOutOfRangeException(
                                "Zoom",
                                value,
                                "1～" + ZOOM_MAX.ToString() + "の範囲で設定してください");
                }
                UInt16 prezoom = zoom;
                zoom = value;

                if (bitmap != null)
                {

                    float zX = (float)ClientRectangle.Width / (float)bitmap.Width / 100 * zoom;
                    float zY = (float)ClientRectangle.Height / (float)bitmap.Height / 100 * zoom;
                    if (zX > zY)
                    {
                        zX = zY;
                    }
                    float dW = (float)bitmap.Width * zX;
                    float dH = (float)bitmap.Height * zX;
                    sizeX = (int)dW;
                    sizeY = (int)dH;
                }
                if (this.Cursor.Equals(zoomCursor))
                {
                    int ofsX = hoverX - ClientRectangle.Width / 2;
                    int ofsY = hoverY - ClientRectangle.Height / 2;

                    PositionX = (PositionX - ofsX) * zoom / prezoom + ofsX;
                    PositionY = (PositionY - ofsY) * zoom / prezoom + ofsY;
                }
                else
                {
                    PositionX = PositionX * zoom / prezoom;
                    PositionY = PositionY * zoom / prezoom;
                }
            }
        }

        #endregion

        #region PositionXプロパティ

        private int positionX = 0;
        /// <summary>
        /// 表示されているビットマップの左端座標Xを参照または設定します。表示範囲になるよう補正されます。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(0),
            Description("表示されているビットマップの左端座標Xを参照または設定します。表示範囲になるよう補正されます。")
        ]
        public int PositionX
        {
            get
            {
                return positionX;
            }
            set
            {
                if (bitmap == null || (ClientSize.Width - sizeX >= 0))
                {
                    positionX = 0;
                    return;
                }
                int n = value;
                int nm = (sizeX - ClientSize.Width) / 2;
                if (nm < n) n = nm;
                if (-nm > n) n = -nm;
                positionX = n;
            }
        }

        #endregion

        #region PositionYプロパティ

        private int positionY = 0;
        /// <summary>
        /// 表示されているビットマップの上端座標Yを参照または設定します。表示範囲になるよう補正されます。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(0),
                    Description("表示されているビットマップの上端座標Yを参照または設定します。表示範囲になるよう補正されます。")
        ]
        public int PositionY
        {
            get
            {
                return positionY;
            }
            set
            {
                if (bitmap == null || (ClientSize.Height - sizeY >= 0))
                {
                    positionY = 0;
                    return;
                }
                int n = value;
                int nm = (sizeY - ClientSize.Height) / 2;
                if (nm < n) n = nm;
                if (-nm > n) n = -nm;
                positionY = n;
            }
        }

        #endregion

        #region Intervalプロパティ

        /// <summary>
        /// スクリーンイメージの更新間隔を参照または設定します。単位はミリ秒です。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(500),
            Description("スクリーンイメージの更新間隔を参照または設定します。単位はミリ秒です。")
        ]
        public int Interval
        {
            get
            {
                return timDraw.Interval;
            }
            set
            {
                timDraw.Interval = value;
            }
        }

        #endregion

        #region ShowBackColorプロパティ

        private bool showBackColor = true;
        /// <summary>
        /// スクリーンイメージではなく背景色を表示するかどうかを参照または設定します。Enabledプロパティをtrueにすると、falseに設定されます。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(true),
            Description("スクリーンイメージではなく背景色を表示するかどうかを参照または設定します。Enabledプロパティをtrueにすると、falseに設定されます。")
        ]
        public bool ShowBackColor
        {
            get
            {
                return showBackColor;
            }
            set
            {
                showBackColor = value;
                Invalidate();
            }
        }

        #endregion

        #endregion

        #region 既存のプロパティ（上書き）

        #region Enabledプロパティ

        /// <summary>
        /// スクリーンイメージ更新を有効にするかどうかを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("スクリーンイメージ更新を有効にするかどうかを参照または設定します。")
        ]
        public new bool Enabled
        {
            get
            {
                return timDraw.Enabled;
            }
            set
            {
                if (value) showBackColor = false;
                timDraw.Enabled = value;
            }
        }

        #endregion

        #endregion

        #endregion

        #region イベント

        #region ズーム・スクロール操作イベント

        #region OnMouseEnterイベント

        /// <summary>
        /// XControls.XScreenView.MouseEnterイベントを発生させます。
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            cursor = this.Cursor;
            if (mouseZoom && bitmap != null)
            {
                this.Focus();
                this.Cursor = zoomCursor;
            }
        }

        #endregion

        #region OnMouseLeaveイベント

        /// <summary>
        /// XControls.XScreenView.MouseLeaveイベントを発生させます。
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.Cursor = cursor;
        }

        #endregion

        #region OnMouseDownイベント

        /// <summary>
        /// XControls.XScreenView.MouseDownイベントを発生させます。
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && zoom > 100 && mouseZoom && bitmap != null)
            {
                lastX = e.X;
                lastY = e.Y;
                isMove = true;
            }
        }

        #endregion

        #region OnMouseMoveイベント

        /// <summary>
        /// XControls.XScreenView.MouseMoveイベントを発生させます。
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            hoverX = e.X;
            hoverY = e.Y;
            if (!(mouseZoom && bitmap != null))
            {
                this.Cursor = cursor;
                isMove = false;
            }
            if (isMove)
            {
                this.Cursor = moveCursor;
                PositionX += e.X - lastX;
                PositionY += e.Y - lastY;
                lastX = e.X;
                lastY = e.Y;
            }
        }

        #endregion

        #region OnMouseUpイベント

        /// <summary>
        /// XControls.XScreenView.MouseUpイベントを発生させます。
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (isMove)
            {
                if (e.X != lastX && e.Y != lastY)
                {
                    PositionX += e.X - lastX;
                    PositionY += e.Y - lastY;
                }
                this.Cursor = zoomCursor;
                isMove = false;
            }
        }

        #endregion

        #region OnMouseDoubleClickイベント

        /// <summary>
        /// XControls.XScreenView.MouseDoubleClickイベントを発生させます。
        /// </summary>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (mouseZoom)
            {
                PositionX += (ClientRectangle.Width / 2) - e.X;
                PositionY += (ClientRectangle.Height / 2) - e.Y;
            }
        }

        #endregion

        #region OnMouseWheelイベント

        /// <summary>
        /// XControls.XScreenView.MouseWheelイベントを発生させます。
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (isMove == false && mouseZoom)
            {
                int zw;
                zw = (int)zoom + e.Delta / 4;
                if (zw < 100) zw = 100;
                if (zw > ZOOM_MAX) zw = ZOOM_MAX;
                if (zoom != zw)
                {
                    Zoom = (UInt16)zw;
                }
            }
        }

        #endregion

        #endregion

        #region OnResizeイベント

        /// <summary>
        /// XControls.XScreenView.Resizeイベントを発生させます。
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            Zoom = 100;
        }

        #endregion

        #region OnPaintイベント

        /// <summary>
        /// XControls.XScreenView.Paintイベントを発生させます。
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.Clear(BackColor);

            if (!showBackColor)
            {
                // 画像表示
                if (bitmap != null)
                {
                    // 拡縮はZoomプロパティ内で計算
                    g.DrawImage(bitmap, (ClientRectangle.Width - sizeX) / 2 + positionX,
                                        (ClientRectangle.Height - sizeY) / 2 + positionY, sizeX, sizeY);
                }
            }
        }

        #endregion

        #region timDraw_Tickイベント

        private void timDraw_Tick(object sender, EventArgs e)
        {
            if (bitmap != null && screenIndex <= Screen.AllScreens.Length - 1)
            {
                Graphics g = Graphics.FromImage(bitmap);
                Screen scr = Screen.AllScreens[screenIndex];
                try
                {
                    g.CopyFromScreen(new Point(scr.Bounds.X, scr.Bounds.Y), new Point(0, 0), bitmap.Size);
                }
                catch(Exception es)
                {
                    Enabled = false;
                    MessageBox.Show(FindForm(), es.Message,
                                    "XScreenView - Screen copy error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
                g.Dispose();
            }

            Invalidate();
        }

        #endregion

        #endregion

        #region 内部処理

        #region マウスカーソル生成 (CreateCursor)

        /// <summary>
        /// マウスカーソルを生成します。
        /// </summary>
        /// <param name="bmp">マイスカーソルイメージ</param>
        /// <param name="xHotSpot">ホットスポット座標X</param>
        /// <param name="yHotSpot">ホットスポット座標Y</param>
        /// <returns>生成されたマウスカーソル</returns>
        private Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            IntPtr ptr = bmp.GetHicon();
            IconInfo tmp = new IconInfo();
            GetIconInfo(ptr, ref tmp);
            tmp.xHotspot = xHotSpot;
            tmp.yHotspot = yHotSpot;
            tmp.fIcon = false;
            ptr = CreateIconIndirect(ref tmp);
            return new Cursor(ptr);
        }
        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        #endregion

        #endregion
    }
}
