using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;

namespace XControls
{
    /// <summary>
    /// ウェブカメラからの画像を操作するコントロールです。
    /// </summary>
    /// <remarks>
    /// プロジェクトプロパティ、ビルドページの「アンセーフコードの許可」をチェックしてください。
    /// </remarks>
    [Designer(typeof(XCamViewDesigner))]
    public partial class XCamView : Panel
    {
        #region インナークラス

        #region XCamViewDesignerクラス

        /// <summary>
        /// XCamView用にデザイナをカスタマイズします。
        /// </summary>
        public class XCamViewDesigner : System.Windows.Forms.Design.ControlDesigner
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.XCamView.XCamViewDesigner クラスの新しいインスタンスを初期化します。
            /// </summary>
            public XCamViewDesigner()
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
                properties.Remove("RightToLeft");
                properties.Remove("SelectedObject");

                base.PostFilterProperties(properties);
            }

            #endregion

            #endregion
        }

        #endregion

        #endregion

        #region 構築・破棄

        /// <summary>
        /// XControls.XCamView クラスの新しいインスタンスを初期化します。
        /// </summary>
        public XCamView()
        {
            #region 初期化

            // 描画のダブルバッファを有効にする
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            components = new System.ComponentModel.Container();
            timDraw = new System.Windows.Forms.Timer(this.components);
            timDraw.Interval = 50;
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
        /// SampleGrabberのフィルタ名
        /// </summary>
        const string FILTERNAME_SAMPLE_GRABBER = "SampleGrabber";

        /// <summary>
        /// ビデオレンダラのフィルタ名
        /// </summary>
        const string FILTERNAME_VIDEO_RENDERER = "Video Renderer";

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
        /// イメージ表示用のビットマップです。
        /// </summary>
        private Bitmap bitmap = null;

        /// <summary>
        /// カメラがONになっているかを表します。
        /// </summary>
        private bool isCameraOn = false;

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

        #region キャプチャ用

        /// <summary>
        /// フィルタグラフを組み立てるグラフビルダです。
        /// </summary>
        private IGraphBuilder graphBuilder = null;

        /// <summary>
        /// キャプチャに使用するサンプルピルダです。
        /// </summary>
        private ISampleGrabber sampleGrabber = null;
        
        /// <summary>
        /// サンプルビルダを使用するかを表します。
        /// </summary>
        private bool useSampleGrabber = false;

        #endregion

        #endregion

        #region プロパティ

        #region 追加のプロパティ

        #region CameraNameプロパティ

        private string cameraName = "";
        /// <summary>
        /// 使用するカメラ名または0から始まるカメラのインデックスを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(""),
            Description("使用するカメラ名または0から始まるカメラのインデックスを参照または設定します。")
        ]
        public string CameraName
        {
            get
            {
                return cameraName;
            }
            set
            {
                cameraName = value;
            }
        }

        #endregion

        #region IsCameraOnプロパティ

        /// <summary>
        /// カメラが有効になっているかどうかを参照します。
        /// </summary>
        [
            Category("動作"),
            Description("カメラが有効になっているかどうかを参照します。")
        ]
        public bool IsCameraOn
        {
            get
            {
                return isCameraOn;
            }
        }

        #endregion

        #region Bitmapプロパティ

        /// <summary>
        /// 表示するビットマップを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(null),
            Description("表示するビットマップを設定します。")
        ]
        public Bitmap Bitmap
        {
            get
            {
                return bitmap;
            }
            set
            {
                bitmap = value;
            }
        }

        #endregion

        #region LocationInfoプロパティ

        private string locationInfo = "";
        /// <summary>
        /// 現在表示している画像に緯度経度情報があれば、それを参照します。
        /// </summary>
        [
            Category("Exif情報"),
            Description("現在表示している画像に緯度経度情報があれば、それを参照します。")
        ]
        public String LocationInfo
        {
            get
            {
                return locationInfo;
            }
        }

        #endregion

        #region DateTimeプロパティ

        private string dateTime = "";
        /// <summary>
        /// 現在表示している画像に撮影日時情報があれば、それを参照します。
        /// </summary>
        [
            Category("Exif情報"),
            Description("現在表示している画像に撮影日時情報があれば、それを参照します。")
        ]
        public String DateTime
        {
            get
            {
                return dateTime;
            }
        }

        #endregion

        #region FileNameプロパティ

        private string fileName = "";
        /// <summary>
        /// 現在表示している画像のファイル名を参照します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(""),
            Description("現在表示している画像のファイル名を参照します。")
        ]
        public string FileName
        {
            get
            {
                return fileName;
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
                if (value < 1 || value > ZOOM_MAX)
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
                    timDraw.Enabled = true;
                    return;
                }
                int n = value;
                int nm = (sizeX - ClientSize.Width) / 2;
                if (nm < n) n = nm;
                if (-nm > n) n = -nm;
                positionX = n;
                timDraw.Enabled = true;
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
                    timDraw.Enabled = true;
                    return;
                }
                int n = value;
                int nm = (sizeY - ClientSize.Height) / 2;
                if (nm < n) n = nm;
                if (-nm > n) n = -nm;
                positionY = n;
                timDraw.Enabled = true;
            }
        }

        #endregion

        #endregion

        #endregion

        #region イベント

        #region ズーム・スクロール操作イベント

        #region OnMouseEnterイベント

        /// <summary>
        /// XControls.XCamView.MouseEnterイベントを発生させます。
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            cursor = this.Cursor;
            if (mouseZoom && this.BackgroundImageLayout == ImageLayout.Zoom && !isCameraOn && bitmap != null)
            {
                this.Focus();
                this.Cursor = zoomCursor;
            }
        }

        #endregion

        #region OnMouseLeaveイベント

        /// <summary>
        /// XControls.XCamView.MouseLeaveイベントを発生させます。
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.Cursor = cursor;
        }

        #endregion

        #region OnMouseDownイベント

        /// <summary>
        /// XControls.XCamView.MouseDownイベントを発生させます。
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && this.BackgroundImageLayout == ImageLayout.Zoom && zoom > 100 && mouseZoom && !isCameraOn && bitmap != null)
            {
                lastX = e.X;
                lastY = e.Y;
                isMove = true;
            }
        }

        #endregion

        #region OnMouseMoveイベント

        /// <summary>
        /// XControls.XCamView.MouseMoveイベントを発生させます。
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            hoverX = e.X;
            hoverY = e.Y;
            if (!(this.BackgroundImageLayout == ImageLayout.Zoom && mouseZoom && !isCameraOn && bitmap != null))
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
        /// XControls.XCamView.MouseUpイベントを発生させます。
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
        /// XControls.XCamView.MouseDoubleClickイベントを発生させます。
        /// </summary>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (BackgroundImageLayout == ImageLayout.Zoom && mouseZoom)
            {
                PositionX += (ClientRectangle.Width / 2) - e.X;
                PositionY += (ClientRectangle.Height / 2) - e.Y;
            }
        }

        #endregion

        #region OnMouseWheelイベント

        /// <summary>
        /// XControls.XCamView.MouseWheelイベントを発生させます。
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (isMove == false && BackgroundImageLayout == ImageLayout.Zoom && mouseZoom)
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

        #region OnPaintイベント

        /// <summary>
        /// XControls.XCamView.Paintイベントを発生させます。
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.Clear(BackColor);
            if (isCameraOn == true)
            {
                // DirectShowによるカメラ画像表示
            }
            else
            {
                // 画像表示
                if (bitmap != null)
                {
                    float sX = 0;   // 左上座標
                    float sY = 0;   // 
                    switch (base.BackgroundImageLayout)
                    {
                        case ImageLayout.Center:
                            sX = ((float)ClientRectangle.Width - bitmap.Width) / 2;
                            sY = ((float)ClientRectangle.Height - bitmap.Height) / 2;
                            g.DrawImageUnscaled(bitmap, (int)sX, (int)sY,
                                                        ClientRectangle.Width - (int)sX,
                                                        ClientRectangle.Height - (int)sY);
                            break;
                        case ImageLayout.None:
                            g.DrawImageUnscaled(bitmap, ClientRectangle);
                            break;
                        case ImageLayout.Stretch:
                            g.DrawImage(bitmap, ClientRectangle);
                            break;
                        case ImageLayout.Tile:
                            for (sY = 0; sY < (float)ClientRectangle.Height; sY += (float)bitmap.Height)
                            {
                                for (sX = 0; sX < (float)ClientRectangle.Width; sX += (float)bitmap.Width)
                                {
                                    g.DrawImageUnscaled(bitmap, (int)sX, (int)sY,
                                                                ClientRectangle.Width - (int)sX,
                                                                ClientRectangle.Height - (int)sY);
                                }
                            }
                            break;
                        case ImageLayout.Zoom:
                            // 拡縮はZoomプロパティ内で計算
                            g.DrawImage(bitmap, (ClientRectangle.Width - sizeX) / 2 + positionX,
                                                (ClientRectangle.Height - sizeY) / 2 + positionY, sizeX, sizeY);
                            break;
                    }
                }
            }
        }

        #endregion

        #region OnBackgroundImageLayoutChangedイベント

        /// <summary>
        /// XControls.XCamView.BackgroundImageLayoutChangedイベントを発生させます。
        /// </summary>
        protected override void OnBackgroundImageLayoutChanged(EventArgs e)
        {
            base.OnBackgroundImageLayoutChanged(e);

            zoom = 100;
            positionX = 0;
            positionY = 0;
        }

        #endregion

        #region OnResizeイベント

        /// <summary>
        /// XControls.XCamView.Resizeイベントを発生させます。
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            Zoom = 100;
        }

        #endregion

        #region timDraw_Tickイベント

        private void timDraw_Tick(object sender, EventArgs e)
        {
            Invalidate();
            timDraw.Enabled = false;
        }

        #endregion

        #endregion

        #region メソッド

        #region カメラ名の一覧を取得 (GetCameraNames)

        /// <summary>
        /// 現在接続されているカメラ名のリストを取得します。
        /// ポートがない場合はWin32Exceptionが発生します。
        /// </summary>
        /// <returns>カメラ名とクラスIDの配列</returns>
        public string[] GetCameraNames()
        {
            ArrayList devlst = new ArrayList();

            Guid systemDeviceEnum = new Guid(CLSID_SystemDeviceEnum);
            ICreateDevEnum cdevEnum = (ICreateDevEnum)GetFromClsid(systemDeviceEnum);
            IEnumMoniker monikers = null;
            cdevEnum.CreateClassEnumerator(FilterCategory.VideoInputDeviceCategory, out monikers, CDef.None);
            if (monikers == null)
            {
                throw new Win32Exception("カメラ名の照会に失敗しました。");
            }
            monikers.Reset();

            IntPtr count = new IntPtr();
            IMoniker[] moniker = new IMoniker[1];
            while (monikers.Next(1, moniker, count) == S_OK)
            {
                object pbagObj;
                Guid propertyBagId = typeof(IPropertyBag).GUID;
                moniker[0].BindToStorage(null, null, ref propertyBagId, out pbagObj);
                IPropertyBag pbag = (IPropertyBag)pbagObj;
                object nameObj;
                pbag.Read("FriendlyName", out nameObj, null);
                devlst.Add((string)nameObj);
            }

            string[] devname = new string[devlst.Count];
            for (int i = 0; i < devlst.Count; i++)
            {
                devname[i] = devlst[i].ToString();
            }

            return devname;
        }

        #endregion

        #region 画像を初期化 (Clear)

        /// <summary>
        /// 表示している画像を初期化します。
        /// </summary>
        public void Clear()
        {
            if (isCameraOn == false)
            {
                bitmap = null;
                fileName = "";
                Zoom = 100;
                PositionX = 0;
                PositionY = 0;
                Invalidate();
            }
        }

        #endregion

        #region 画像をファイルから読む (LoadImage)

        /// <summary>
        /// 画像をファイルから読んで表示します。
        /// </summary>
        /// <param name="imageName">読み込む画像ファイル</param>
        public void LoadImage(string imageName)
        {
            if (isCameraOn == false)
            {
                // 画像読み込み
                Uri appdir = new Uri(Application.StartupPath + "\\");
                Uri reldir = new Uri(appdir, imageName);
                String fnam = System.Uri.UnescapeDataString(appdir.MakeRelativeUri(reldir).ToString());
                fnam = fnam.Replace("/", "\\");
                Uri fulldir = new Uri(appdir, System.Environment.ExpandEnvironmentVariables(fnam));
                fnam = fulldir.LocalPath;
                FileStream fs = File.OpenRead(fnam);
                using (Image img = Image.FromStream(fs, false, false))
                {
                    System.Drawing.Imaging.PropertyItem[] imgProp = img.PropertyItems;

                    // 複製
                    bitmap = new Bitmap(img);
                    for (int i = 0; i < imgProp.Length; i++)
                    {
                        bitmap.SetPropertyItem(imgProp[i]);
                    }
                    img.Dispose();
                }
                fs.Close();
                fs.Dispose();
                System.GC.Collect();

                // 向きを調整
                while (true)
                {
                    int[] idlst = bitmap.PropertyIdList;
                    int ix;
                    System.Drawing.Imaging.PropertyItem item;
                    ix = Array.IndexOf(idlst, 0x0112);
                    if (ix < 0) break;
                    item = bitmap.PropertyItems[ix];
                    UInt16 ore = BitConverter.ToUInt16(item.Value, 0);
                    switch (ore)
                    {
                        case 2:
                            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            break;
                        case 3:
                            bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 4:
                            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                            break;
                        case 5:
                            bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
                            break;
                        case 6:
                            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case 7:
                            bitmap.RotateFlip(RotateFlipType.Rotate270FlipX);
                            break;
                        case 8:
                            bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        default:    // 0 or 1.
                            break;
                    }
                    PropertyItem pi = bitmap.PropertyItems[ix];
                    pi.Value[0] = 0;
                    pi.Value[1] = 0;
                    pi.Len = 2;
                    bitmap.SetPropertyItem(pi);
                    break;
                }
                fileName = fnam;

                // 撮影日時情報
                dateTime = "";
                while (true)
                {
                    int[] idlst = bitmap.PropertyIdList;
                    int ix;
                    System.Drawing.Imaging.PropertyItem item;
                    String dt;

                    // 撮影日時を取得
                    ix = Array.IndexOf(idlst, 0x9003);
                    if (ix < 0) break;
                    item = bitmap.PropertyItems[ix];
                    dt = System.Text.Encoding.ASCII.GetString(item.Value);
                    dt = dt.Trim(new char[] { '\0' });
                    dateTime = dt.Substring(0, 4) + "/" + dt.Substring(5, 2) + "/" + dt.Substring(8);
                    break;
                }

                // 位置情報取得
                locationInfo = "";
                while (true)
                {
                    int[] idlst = bitmap.PropertyIdList;
                    string locs = "";
                    int ix;
                    UInt32 d, m;
                    double dmm, s;
                    System.Drawing.Imaging.PropertyItem item;

                    // 緯度の南北を取得 - TagNo 1.GPSLatitudeRef
                    ix = Array.IndexOf(idlst, 0x0001);
                    if (ix < 0) break;
                    item = bitmap.PropertyItems[ix];
                    //locs += BitConverter.ToChar(item.Value, 0).ToString();

                    // 緯度を取得       - TagNo 2.GPSLatitude
                    ix = Array.IndexOf(idlst, 0x0002);
                    if (ix < 0) break;
                    item = bitmap.PropertyItems[ix];
                    d = BitConverter.ToUInt32(item.Value, 0) / BitConverter.ToUInt32(item.Value, 4);
                    if (BitConverter.ToUInt32(item.Value, 12) == 100)
                    {
                        // DMM形式からDMS形式に変換
                        dmm = (double)BitConverter.ToUInt32(item.Value, 8) / (double)BitConverter.ToUInt32(item.Value, 12);
                        m = (UInt32)dmm;
                        s = (dmm % 1) * 60;
                    }
                    else
                    {
                        // DMS形式
                        m = BitConverter.ToUInt32(item.Value, 8) / BitConverter.ToUInt32(item.Value, 12);
                        s = (double)BitConverter.ToUInt32(item.Value, 16) / (double)BitConverter.ToUInt32(item.Value, 20);
                    }
                    //locs += d.ToString() +"."+ m.ToString() +"."+ s.ToString("#0.0") + ",";
                    // 10進数から60進数に変換
                    locs += ((double)(d + ((double)m) / 60 + s / 3600)).ToString() + ",";

                    // 経度の東西を取得 - TagNo 3.GPSLongitudeRef
                    ix = Array.IndexOf(idlst, 0x0003);
                    if (ix < 0) break;
                    item = bitmap.PropertyItems[ix];
                    //locs += BitConverter.ToChar(item.Value, 0).ToString();

                    // 経度を取得       - TagNo 4.GPSLongitude
                    ix = Array.IndexOf(idlst, 0x0004);
                    if (ix < 0) break;
                    item = bitmap.PropertyItems[ix];
                    d = BitConverter.ToUInt32(item.Value, 0) / BitConverter.ToUInt32(item.Value, 4);
                    if (BitConverter.ToUInt32(item.Value, 12) == 100)
                    {
                        // DMM形式からDMS形式に変換
                        dmm = (double)BitConverter.ToUInt32(item.Value, 8) / (double)BitConverter.ToUInt32(item.Value, 12);
                        m = (UInt32)dmm;
                        s = (dmm % 1) * 60;
                    }
                    else
                    {
                        // DMS形式
                        m = BitConverter.ToUInt32(item.Value, 8) / BitConverter.ToUInt32(item.Value, 12);
                        s = (double)BitConverter.ToUInt32(item.Value, 16) / (double)BitConverter.ToUInt32(item.Value, 20);
                    }
                    //locs += d.ToString() + "." + m.ToString() + "." + s.ToString("#0.0");
                    // 10進数から60進数に変換
                    locs += ((double)(d + ((double)m) / 60 + s / 3600)).ToString();

                    locationInfo = locs;
                    break;
                }

                // 拡大率リセット
                Zoom = 100;

                // 再描画
                timDraw.Enabled = true;
            }
        }

        #endregion

        #region 画像をファイルに保存 (SaveImage)

        /// <summary>
        /// 表示されている画像をファイルに保存します。
        /// </summary>
        /// <param name="imageName">保存先の画像ファイル</param>
        public void SaveImage(string imageName)
        {
            if (bitmap != null)
            {
                // ビットマップが有効なら保存
                ImageFormat fmt = ImageFormat.Png;
                string ext = imageName.Substring(imageName.LastIndexOf(".") + 1).ToLower();
                bool useext = false;

                // 拡張子からフォーマットを設定
                if (ext.Equals("jpeg") || ext.Equals("jpg"))
                {
                    fmt = ImageFormat.Jpeg;
                    useext = true;
                }
                if (ext.Equals("bmp"))
                {
                    fmt = ImageFormat.Bmp;
                    useext = true;
                }
                if (ext.Equals("gif"))
                {
                    fmt = ImageFormat.Gif;
                    useext = true;
                }
                if (ext.Equals("tiff"))
                {
                    fmt = ImageFormat.Tiff;
                    useext = true;
                }
                if (ext.Equals("png"))
                {
                    fmt = ImageFormat.Png;
                    useext = true;
                }

                // 拡張子を再チェック
                if (useext == false)
                {
                    imageName += "." + fmt.ToString().ToLower();
                }
                Uri appdir = new Uri(Application.StartupPath + "\\");
                Uri reldir = new Uri(appdir, imageName);
                String fnam = System.Uri.UnescapeDataString(appdir.MakeRelativeUri(reldir).ToString());
                fnam = fnam.Replace("/", "\\");
                Uri fulldir = new Uri(appdir, System.Environment.ExpandEnvironmentVariables(fnam));
                fnam = fulldir.LocalPath;
                bitmap.Save(fnam, fmt);
                fileName = fnam;
            }
        }

        #endregion

        #region 画像ファイルを削除 (DeleteImage)

        /// <summary>
        /// 画像ファイルを削除します。
        /// </summary>
        /// <param name="imageName">削除する画像ファイル</param>
        public void DeleteImage(string imageName)
        {
            Uri appdir = new Uri(Application.StartupPath + "\\");
            Uri reldir = new Uri(appdir, imageName);
            String fnam = System.Uri.UnescapeDataString(appdir.MakeRelativeUri(reldir).ToString());
            fnam = fnam.Replace("/", "\\");
            Uri fulldir = new Uri(appdir, System.Environment.ExpandEnvironmentVariables(fnam));
            fnam = fulldir.LocalPath;
            File.Delete(fnam);
        }

        #endregion

        #region カメラ画像を表示 (CameraOn)

        //<summary>
        //カメラ画像を連続で表示します。
        //</summary>
        public void CameraOn()
        {
            if (isCameraOn == false && cameraName.Length > 0)
            {
                // フィルタグラフを準備
                Guid filterGraph = new Guid(CLSID_FilterGraph);
                graphBuilder = (IGraphBuilder)GetFromClsid(filterGraph);

                // SampleGrabberを準備
                Guid SampleGrabber = new Guid(CLSID_SampleGrabber);
                sampleGrabber = (ISampleGrabber)GetFromClsid(SampleGrabber);
                sampleGrabber.SetBufferSamples(true);
                IBaseFilter smplGrab = (IBaseFilter)sampleGrabber;

                // カメラ名と一致するデバイスを検索
                Guid systemDeviceEnum = new Guid(CLSID_SystemDeviceEnum);
                ICreateDevEnum cdevEnum = (ICreateDevEnum)GetFromClsid(systemDeviceEnum);

                IEnumMoniker monikers = null;
                cdevEnum.CreateClassEnumerator(FilterCategory.VideoInputDeviceCategory, out monikers, CDef.None);
                if (monikers == null)
                {
                    throw new Win32Exception("カメラ名を照会できませんでした。");
                }
                monikers.Reset();

                object camobj = null;
                IBaseFilter camflt = null;

                IntPtr count = new IntPtr();
                IMoniker[] moniker = new IMoniker[1];
                int no = 0;
                while (monikers.Next(1, moniker, count) == S_OK)
                {
                    object pbagObj;
                    Guid propertyBagId = typeof(IPropertyBag).GUID;
                    moniker[0].BindToStorage(null, null, ref propertyBagId, out pbagObj);
                    IPropertyBag pbag = (IPropertyBag)pbagObj;
                    object nameObj;
                    pbag.Read("FriendlyName", out nameObj, null);

                    if (((string)nameObj).Equals(cameraName) || no.ToString().Equals(cameraName))
                    {
                        // デバイスをフィルタに対応付ける
                        Guid baseFilter = typeof(IBaseFilter).GUID;
                        moniker[0].BindToObject(null, null, ref baseFilter, out camobj);
                        camflt = (IBaseFilter)camobj;
                        break;
                    }
                    no++;
                }

                if (camflt == null)
                {
                    throw new Win32Exception("カメラ '" + cameraName + "' が見つかりません。");
                }

                // カメラフィルタをフィルタグラフに登録
                graphBuilder.AddFilter(camflt, cameraName);

                // SampleGrabberをフィルタグラフに登録
                AMMediaType amMediaType = new AMMediaType();
                amMediaType.majorType = MediaType.Video;
                amMediaType.subType = MediaSubType.RGB24;
                amMediaType.formatType = FormatType.VideoInfo;
                int hresult = sampleGrabber.SetMediaType(amMediaType);
                if (hresult != S_OK)
                {
                    throw new Win32Exception("接続フォーマットの設定に失敗しました。");
                }
                graphBuilder.AddFilter(smplGrab, FILTERNAME_SAMPLE_GRABBER);

                // キャプチャ用ピンを取得
                IEnumPins pinEnum;
                camflt.EnumPins(out pinEnum);

                IPin[] pin = new IPin[1];
                IMediaControl mctrl = null;
                IVideoWindow vwin = null;
                while (pinEnum.Next(1, pin, count) == S_OK)
                {
                    // ピン情報を取得
                    PinInfo pinfo;
                    pin[0].QueryPinInfo(out pinfo);

                    if(pinfo.dir.Equals(PinDirection.Output))
                    {
                        // 出力ピンからグラフを構築
                        mctrl = (IMediaControl)graphBuilder;
                        if (graphBuilder.Render(pin[0]) == S_OK)
                        {
                            // 成功
                            vwin = (IVideoWindow)graphBuilder;
                            vwin.put_Owner(Handle);
                            vwin.put_WindowStyle(WindowStyle.Child);
                            vwin.put_Top(0);
                            vwin.put_Left(0);
                            vwin.put_Height(base.Height);
                            vwin.put_Width(base.Width);
                            isCameraOn = true;
                            break;
                        }
                    }
                }

                // キャプチャ可能チェック
                useSampleGrabber = true;
                hresult = sampleGrabber.GetConnectedMediaType(amMediaType);
                if (hresult != S_OK)
                {
                    // SampleGrabberを使用しない
                    useSampleGrabber = false;
                }
                if ((amMediaType.formatType != FormatType.VideoInfo) ||
                    (amMediaType.formatPtr == IntPtr.Zero))
                {
                    // SampleGrabberを使用しない
                    useSampleGrabber = false;
                }

                if (isCameraOn == false)
                {
                    throw new Win32Exception("カメラ画像の表示に失敗しました。");
                }
                else
                {
                    // 画像サイズ取得
                    Application.DoEvents();
                    vwin.put_Visible(OABool.False);
                    mctrl.Run();
                    System.Threading.Thread.Sleep(700);
                    mctrl.Pause();
                    BITMAPFILEHEADER bmphdr = new BITMAPFILEHEADER();
                    BITMAPINFOHEADER bmpinfo = new BITMAPINFOHEADER();
                    int bsize = 0;
                    int bhsize = Marshal.SizeOf(bmphdr);
                    byte[] bmap = null;
                    hresult = S_OK;

                    if (useSampleGrabber)
                    {
                        // イメージサイズを取得
                        sampleGrabber.GetCurrentBuffer(ref bsize, IntPtr.Zero);

                        // イメージバッファを確保
                        IntPtr pimg;
                        pimg = Marshal.AllocHGlobal(bsize);

                        // ビットマップ情報を取得
                        AMMediaType ammediaType = new AMMediaType();
                        hresult = sampleGrabber.GetConnectedMediaType(ammediaType);
                        if (hresult != S_OK)
                        {
                            throw new Win32Exception("接続フォーマットの取得に失敗しました。");
                        }
                        if ((ammediaType.formatType != FormatType.VideoInfo) ||
                            (ammediaType.formatPtr == IntPtr.Zero))
                        {
                            throw new Win32Exception("不明なフォーマットです。");
                        }
                        VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(ammediaType.formatPtr,
                                                            typeof(VideoInfoHeader));
                        DsBITMAPINFOHEADER dsbmpinfo = videoInfoHeader.BmiHeader;

                        // イメージを取得
                        bhsize = Marshal.SizeOf(bmphdr) + Marshal.SizeOf(bmpinfo);
                        hresult = sampleGrabber.GetCurrentBuffer(ref bsize, pimg);
                        bmap = new Byte[bhsize + bsize];
                        Marshal.Copy(pimg, bmap, bhsize, bsize);

                        // イメージバッファを解放
                        Marshal.FreeHGlobal(pimg);

                        unsafe
                        {
                            // SampleGrabber使用時はDsBITMAPINFOHEADERをマージ
                            fixed (byte* ptr = bmap)
                            {
                                byte* bptr = ptr + Marshal.SizeOf(bmphdr);
                                Marshal.StructureToPtr(dsbmpinfo, (IntPtr)bptr, false);
                            }
                        }
                    }
                    else
                    {
                        // イメージサイズを取得
                        IBasicVideo bvideo = (IBasicVideo)graphBuilder;
                        bvideo.GetCurrentImage(ref bsize, IntPtr.Zero);

                        // イメージバッファを確保
                        IntPtr pimg;
                        pimg = Marshal.AllocHGlobal(bsize);

                        // イメージを取得
                        bvideo.GetCurrentImage(ref bsize, pimg);
                        bmap = new Byte[bhsize + bsize];
                        Marshal.Copy(pimg, bmap, bhsize, bsize);

                        // イメージバッファを解放
                        Marshal.FreeHGlobal(pimg);
                    }

                    // ビットマップ用ヘッダを編集
                    bmphdr.bfType = ('M' << 8) | 'B';
                    bmphdr.bfSize = (uint)(Marshal.SizeOf(bmphdr) + Marshal.SizeOf(bmpinfo) + bsize);
                    bmphdr.bfReserved1 = 0;
                    bmphdr.bfReserved2 = 0;
                    bmphdr.bfOffBits = (uint)(Marshal.SizeOf(bmphdr) + Marshal.SizeOf(bmpinfo));
                    unsafe
                    {
                        fixed (byte* ptr = bmap)
                        {
                            Marshal.StructureToPtr(bmphdr, (IntPtr)ptr, false);
                        }
                    }

                    // ビットマップに変換
                    ImageConverter imcnv = new ImageConverter();
                    Bitmap wbitmap = null;
                    if (hresult == S_OK)
                    {
                        try
                        {
                            Image img = (Image)imcnv.ConvertFrom(bmap);
                            wbitmap = new Bitmap(img);
                        }
                        catch
                        {
                        }
                    }

                    // プレビュー表示
                    switch (base.BackgroundImageLayout)
                    {
                        case ImageLayout.Center:
                        case ImageLayout.None:
                        case ImageLayout.Tile:
                        case ImageLayout.Zoom:
                            if (wbitmap == null)
                            {
                                vwin.put_Top(0);
                                vwin.put_Left(0);
                                vwin.put_Height(base.Height);
                                vwin.put_Width(base.Width);
                            }
                            else
                            {
                                Graphics g = CreateGraphics();
                                g.Clear(BackColor);
                                float zX = 0;   // 拡縮率
                                float zY = 0;   // 
                                float dW = 0;   // 拡縮後描画サイズ
                                float dH = 0;   //
                                zX = (float)base.Width / (float)wbitmap.Width;
                                zY = (float)base.Height / (float)wbitmap.Height;
                                if (zX > zY)
                                {
                                    zX = zY;
                                }
                                dW = (float)wbitmap.Width * zX;
                                dH = (float)wbitmap.Height * zX;
                                vwin.put_Top((base.Height - (int)dH) / 2);
                                vwin.put_Left((base.Width - (int)dW) / 2);
                                vwin.put_Height((int)dH);
                                vwin.put_Width((int)dW);
                            }
                            break;
                        case ImageLayout.Stretch:
                            vwin.put_Top(0);
                            vwin.put_Left(0);
                            vwin.put_Height(base.Height);
                            vwin.put_Width(base.Width);
                            break;
                    }
                    Application.DoEvents();
                    mctrl.Run();
                    sampleGrabber.SetBufferSamples(true);
                    vwin.put_Visible(OABool.True);
                }
            }
        }

        #endregion

        #region カメラ画像を取り込む (CameraCapture)

        /// <summary>
        /// カメラ画像を取り込みます。
        /// </summary>
        public void CameraCapture()
        {
            if (isCameraOn == true)
            {
                // 一時停止
                IMediaControl mctrl = (IMediaControl)graphBuilder;
                mctrl.Pause();

                BITMAPFILEHEADER bmphdr = new BITMAPFILEHEADER();
                BITMAPINFOHEADER bmpinfo = new BITMAPINFOHEADER();
                int bsize = 0;
                int bhsize = Marshal.SizeOf(bmphdr);
                byte[] bmap = null;
                int hresult = S_OK;

                if (useSampleGrabber)
                {
                    // イメージサイズを取得
                    sampleGrabber.GetCurrentBuffer(ref bsize, IntPtr.Zero);

                    // イメージバッファを確保
                    IntPtr pimg;
                    pimg = Marshal.AllocHGlobal(bsize);

                    // ビットマップ情報を取得
                    AMMediaType ammediaType = new AMMediaType();
                    hresult = sampleGrabber.GetConnectedMediaType(ammediaType);
                    if (hresult != S_OK)
                    {
                        throw new Win32Exception("接続フォーマットの取得に失敗しました。");
                    }
                    if ((ammediaType.formatType != FormatType.VideoInfo) ||
                        (ammediaType.formatPtr == IntPtr.Zero))
                    {
                        throw new Win32Exception("不明なフォーマットです。");
                    }
                    VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(ammediaType.formatPtr,
                                                        typeof(VideoInfoHeader));
                    DsBITMAPINFOHEADER dsbmpinfo = videoInfoHeader.BmiHeader;

                    // イメージを取得
                    bhsize = Marshal.SizeOf(bmphdr) + Marshal.SizeOf(bmpinfo);
                    hresult = sampleGrabber.GetCurrentBuffer(ref bsize, pimg);
                    bmap = new Byte[bhsize + bsize];
                    Marshal.Copy(pimg, bmap, bhsize, bsize);

                    // イメージバッファを解放
                    Marshal.FreeHGlobal(pimg);

                    unsafe
                    {
                        // SampleGrabber使用時はDsBITMAPINFOHEADERをマージ
                        fixed (byte* ptr = bmap)
                        {
                            byte* bptr = ptr + Marshal.SizeOf(bmphdr);
                            Marshal.StructureToPtr(dsbmpinfo, (IntPtr)bptr, false);
                        }
                    }
                }
                else
                {
                    // イメージサイズを取得
                    IBasicVideo bvideo = (IBasicVideo)graphBuilder;
                    bvideo.GetCurrentImage(ref bsize, IntPtr.Zero);

                    // イメージバッファを確保
                    IntPtr pimg;
                    pimg = Marshal.AllocHGlobal(bsize);

                    // イメージを取得
                    bvideo.GetCurrentImage(ref bsize, pimg);
                    bmap = new Byte[bhsize + bsize];
                    Marshal.Copy(pimg, bmap, bhsize, bsize);

                    // イメージバッファを解放
                    Marshal.FreeHGlobal(pimg);
                }

                // ビットマップ用ヘッダを編集
                bmphdr.bfType = ('M' << 8) | 'B';
                bmphdr.bfSize = (uint)(Marshal.SizeOf(bmphdr) + Marshal.SizeOf(bmpinfo) + bsize);
                bmphdr.bfReserved1 = 0;
                bmphdr.bfReserved2 = 0;
                bmphdr.bfOffBits = (uint)(Marshal.SizeOf(bmphdr) + Marshal.SizeOf(bmpinfo));
                unsafe
                {
                    fixed (byte* ptr = bmap)
                    {
                        Marshal.StructureToPtr(bmphdr, (IntPtr)ptr, false);
                    }
                }

                // ビットマップに変換
                ImageConverter imcnv = new ImageConverter();
                try
                {
                    if (hresult != S_OK)
                    {
                        throw new Win32Exception("GetCurrentBuffer失敗\n   エラーコード - 0x" + hresult.ToString("X8"));
                    }
                    Image img = (Image)imcnv.ConvertFrom(bmap);
                    bitmap = new Bitmap(img);
                }
                catch (Exception es)
                {
                    BinarySave(bmap);
                    throw new Win32Exception("ビットマップが取得できませんでした。\n\n理由:\n   " + es.Message + "\n   取得結果をIMAGEDUMP.DATに保存しました。\nスタックトレース:\n" + es.StackTrace);
                }
                finally
                {
                    // 一時停止を解除
                    mctrl.Run();

                    // 拡大率リセット/再描画
                    fileName = "";
                    Zoom = 100;
                }
            }
        }

        #endregion

        #region カメラ画像表示を終了 (CameraOff)

        /// <summary>
        /// カメラ画像の表示を終了し、保持している画像を表示します。
        /// </summary>
                public void CameraOff()
        {
            if (isCameraOn == true)
            {
                // カメラ撮影停止
                IMediaControl mctrl = (IMediaControl)graphBuilder;
                mctrl.Stop();
                Marshal.ReleaseComObject(graphBuilder);

                // カメラOFFとする
                isCameraOn = false;
                timDraw.Enabled = true;
            }
        }

        #endregion

        #region ストリーム設定ダイアログ呼び出し (ShowStreamDialog)

        /// <summary>
        /// ストリーム設定のダイアログを呼び出します。
        /// </summary>
        public void ShowStreamDialog()
        {
            if (isCameraOn == true)
            {
                MessageBox.Show("現在この機能はサポートされていません。", "ShowStreamDialog");
            }
        }

        #endregion

        #region カメラプロパティダイアログ呼び出し (ShowPropertyDialog)

        /// <summary>
        /// カメラプロパティのダイアログを呼び出します。
        /// </summary>
        public void ShowPropertyDialog()
        {
            if (isCameraOn == true)
            {
                MessageBox.Show("現在この機能はサポートされていません。", "ShowPropertyDialog");
            }
        }

        #endregion

        #endregion

        #region 内部処理

        #region CLSIDからインスタンスを取得 (GetFromClsid)

        /// <summary>
        /// CLSIDからインスタンスを取得します。
        /// </summary>
        /// <param name="clsid">CLSID</param>
        /// <returns>インスタンス</returns>
        private static object GetFromClsid(Guid clsid)
        {
            Type comType = Type.GetTypeFromCLSID(clsid);
            return Activator.CreateInstance(comType);
        }

        #endregion

        #region バイナリメモリダンプ (BinarySave)

        /// <summary>
        /// メモリの内容をIMAGEDUMP.DATファイルに保存します。(障害調査用)
        /// </summary>
        /// <param name="mem"></param>
        public void BinarySave(byte[] mem)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(Application.StartupPath + "\\IMAGEDUMP.DAT", FileMode.Create, FileAccess.Write);
                for (int i = 0; i < mem.Length; i++)
                {
                    fs.WriteByte(mem[i]);
                }
            }
            catch
            {
            }
            finally
            {
                try
                {
                    fs.Close();
                }
                catch
                {
                }
            }
        }

        #endregion

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
