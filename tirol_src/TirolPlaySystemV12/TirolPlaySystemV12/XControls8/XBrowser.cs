using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace XControls
{
    /// <summary>
    /// WebBrowserの機能拡張版コントロールです。
    /// </summary>
    /// <remarks>
    /// 次のアセンブリへの参照を追加してください。
    ///     SHDocVw.dll
    /// </remarks>
    [Designer(typeof(XBrowserDesigner))]
    public class XBrowser : WebBrowser
    {
        #region インナークラス

        #region XBrowserDesignerクラス

        /// <summary>
        /// XBrowser用にデザイナをカスタマイズします。
        /// </summary>
        public class XBrowserDesigner : System.Windows.Forms.Design.ControlDesigner
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.XBrowser.XBrowserDesigner クラスの新しいインスタンスを初期化します。
            /// </summary>
            public XBrowserDesigner()
            {
            }

            #endregion

            #region メソッド

            #region PostFilterPropertiesメソッド

            protected override void PostFilterProperties(IDictionary properties)
            {
                // フィルタリングするプロパティ
                properties.Remove("Uri");

                base.PostFilterProperties(properties);
            }

            #endregion

            #endregion
        }

        #endregion

        #endregion

        #region 構築・破棄

        /// <summary>
        /// XControls.XBrowser クラスの新しいインスタンスを初期化します。
        /// </summary>
        public XBrowser()
            : base()
        {
            #region 初期化

            // 空白のページを表示
            Clear();

            #endregion
        }

        /// <summary>
        /// インスタンスを破棄します。
        /// </summary>
        ~XBrowser()
        {
            if (isRun) Stop();
            if (imageOrigin != null) imageOrigin.Dispose();
        }

        #endregion

        #region 定数

        /// <summary>
        /// ページの表示モードを指定する定数です。
        /// </summary>
        public enum ViewModes : int
        {
            /// <summary>
            /// 任意の倍率、位置で表示します。
            /// </summary>
            Normal,
            /// <summary>
            /// 全体が表示されるようにサイズ調整します。
            /// </summary>
            Fit
        }

        /// <summary>
        /// キャプチャ時の描画方法を指定する定数です。
        /// </summary>
        public enum CaptureDrawModes : int
        {
            /// <summary>
            /// PrintWindowで描画します。
            /// </summary>
            PrintWindow,
            /// <summary>
            /// OleDrawで描画します。
            /// </summary>
            OleDraw
        }

        #endregion

        #region 変数

        /// <summary>
        /// キャプチャ用ビットマップです。
        /// </summary>
        private Bitmap imageOrigin = null;

        /// <summary>
        /// GetCaptureImage処理中かを表します。
        /// </summary>
        private bool isCapture = false;

        /// <summary>
        /// 遷移元のURLです。
        /// </summary>
        private String previousUrl = "";

        /// <summary>
        /// Navigate禁止回避用のフラグです。
        /// </summary>
        private bool isGoNavigate = false;

        /// <summary>
        /// スクロールバーを表示するかどうかを表します。
        /// </summary>
        private bool isSetScrollBar = true;

        /// <summary>
        /// SetZoom処理中かを表します。
        /// </summary>
        private bool isZoomUpdate = false;

        #endregion

        #region インタフェース

        #region IOleClientSiteインタフェース

        [
            ComImport,
            Guid("00000118-0000-0000-C000-000000000046"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IOleClientSite
        {
            void SaveObject();
            void GetMoniker(uint dwAssign, uint dwWhichMoniker, ref object ppmk);
            void GetContainer(ref object ppContainer);
            void ShowObject();
            void OnShowWindow(bool fShow);
            void RequestNewObjectLayout();
        }

        #endregion

        #region IOleObjectインタフェース

        [
            ComImport,
            Guid("00000112-0000-0000-C000-000000000046"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IOleObject
        {
            void SetClientSite(IOleClientSite pClientSite);
            void GetClientSite(ref IOleClientSite ppClientSite);
            void SetHostNames(object szContainerApp, object szContainerObj);
            void Close(uint dwSaveOption);
            void SetMoniker(uint dwWhichMoniker, object pmk);
            void GetMoniker(uint dwAssign, uint dwWhichMoniker, object ppmk);
            void InitFromData(System.Runtime.InteropServices.ComTypes.IDataObject pDataObject, bool fCreation, uint dwReserved);
            void GetClipboardData(uint dwReserved, ref System.Runtime.InteropServices.ComTypes.IDataObject ppDataObject);
            void DoVerb(uint iVerb, uint lpmsg, object pActiveSite, uint lindex, uint hwndParent, uint lprcPosRect);
            void EnumVerbs(ref object ppEnumOleVerb);
            void Update();
            void IsUpToDate();
            void GetUserClassID(uint pClsid);
            void GetUserType(uint dwFormOfType, uint pszUserType);
            void SetExtent(DVASPECT dwDrawAspect, ref Size psizel);
            void GetExtent(DVASPECT dwDrawAspect, out Size psizel);
            void Advise(object pAdvSink, uint pdwConnection);
            void Unadvise(uint dwConnection);
            void EnumAdvise(ref object ppenumAdvise);
            void GetMiscStatus(uint dwAspect, uint pdwStatus);
            void SetColorScheme(object pLogpal);
        };

        #endregion

        #endregion

        #region プロパティ

        #region 追加のプロパティ

        #region StartURLプロパティ

        private String startURL = "";
        /// <summary>
        /// ブラウズ開始時のURLを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(""),
            Description("ブラウズ開始時のURLを参照または設定します。")
        ]
        public String StartURL
        {
            get
            {
                return startURL;
            }
            set
            {
                startURL = value;
            }
        }

        #endregion

        #region CurrentURLプロパティ

        /// <summary>
        /// 現在表示しているページのURLを示します。
        /// </summary>
        [
            Category("動作"),
            Description("現在表示しているページのURLを示します。")
        ]
        public String CurrentURL
        {
            get
            {
                if (base.Url == null)
                {
                    return "";
                }
                return base.Url.ToString();
            }
        }

        #endregion

        #region ViewModeプロパティ

        private ViewModes viewMode = ViewModes.Normal;
        /// <summary>
        /// 表示モードを参照または設定します。Fitにするとスクロール可能範囲全体をクライアント領域内に表示します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(typeof(ViewModes), "Normal"),
            Description("表示モードを参照または設定します。Fitにするとスクロール可能範囲全体をクライアント領域内に表示します。")
        ]
        public ViewModes ViewMode
        {
            get
            {
                return viewMode;
            }
            set
            {
                if (viewMode != value)
                {
                    viewMode = value;
                    SetZoom();
                }
            }
        }

        #endregion

        #region CaptureDrawModeプロパティ

        private CaptureDrawModes captureDrawMode = CaptureDrawModes.OleDraw;
        /// <summary>
        /// キャプチャ時の描画方法を参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(typeof(CaptureDrawModes), "OleDraw"),
            Description("キャプチャ時の描画方法を参照または設定します。")
        ]
        public CaptureDrawModes CaptureDrawMode
        {
            get
            {
                return captureDrawMode;
            }
            set
            {
                captureDrawMode = value;
            }
        }

        #endregion

        #region IsRunプロパティ

        private bool isRun = false;
        /// <summary>
        /// ブラウズ中かどうかを示します。
        /// </summary>
        [
            Category("動作"),
            Description("ブラウズ中かどうかを示します。")
        ]
        public bool IsRun
        {
            get
            {
                return isRun;
            }
        }

        #endregion

        #region ViewPositionプロパティ

        private Point viewPosition = new Point(0, 0);
        /// <summary>
        /// コンテンツの左上座標を参照または設定します。ViewModeがNormalの場合に有効です。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(typeof(Point), "0, 0"),
            Description("コンテンツの左上座標を参照または設定します。ViewModeがNormalの場合に有効です。")
        ]
        public Point ViewPosition
        {
            get
            {
                return viewPosition;
            }
            set
            {
                if (viewPosition != value)
                {
                    viewPosition = value;
                }
                if (viewMode != ViewModes.Fit) SetPosition(viewPosition);
            }
        }

        #endregion

        #region ViewNowPositionプロパティ

        private Point viewNowPosition = new Point(0, 0);
        /// <summary>
        /// 実際に表示されているページの左上座標を示します。
        /// </summary>
        [
            Category("動作"),
            Description("実際に表示されているページの左上座標を示します。")
        ]
        public Point ViewNowPosition
        {
            get
            {
                return viewNowPosition;
            }
        }

        #endregion

        #region CaptureZoomRateプロパティ

        private Double captureZoomRate = 100;
        /// <summary>
        /// キャプチャ時の拡大率(%)を参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(typeof(Double), "100"),
            Description("キャプチャ時の拡大率(%)を参照または設定します。")
        ]
        public Double CaptureZoomRate
        {
            get
            {
                return captureZoomRate;
            }
            set
            {
                captureZoomRate = value;
            }
        }

        #endregion

        #region ZoomRateプロパティ

        private Double zoomRate = 100;
        /// <summary>
        /// ページ表示の拡大率(%)を参照または設定します。ViewModeがNormalの場合に有効です。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(typeof(Double), "100"),
            Description("ページ表示の拡大率(%)を参照または設定します。ViewModeがNormalの場合に有効です。")
        ]
        public Double ZoomRate
        {
            get
            {
                return zoomRate;
            }
            set
            {
                if (zoomRate != value)
                {
                    zoomRate = value;
                    if (viewMode != ViewModes.Fit) SetZoom();
                    SetContentsSize(scrollBarsEnabled);
                }
            }
        }

        #endregion

        #region ZoomNowRateプロパティ

        private Int32 zoomNowRate = 100;
        /// <summary>
        /// 実際に表示されているページの拡大率(%)を示します。
        /// </summary>
        [
            Category("動作"),
            Description("実際に表示されているページの拡大率(%)を示します。")
        ]
        public Int32 ZoomNowRate
        {
            get
            {
                return zoomNowRate;
            }
        }

        #endregion

        #region ZoomAdjustmentプロパティ

        private bool zoomAdjustment = true;
        /// <summary>
        /// ViewModeがFitまたはZoomの場合の実際の拡大率を補正するかを示します。falseにした場合は再描画されない領域が発生する場合があります。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(typeof(bool), "true"),
            Description("ViewModeがFitまたはZoomの場合の実際の拡大率を補正するかを示します。falseにした場合は再描画されない領域が発生する場合があります。")
        ]
        public bool ZoomAdjustment
        {
            get
            {
                return zoomAdjustment;
            }
            set
            {
                if (zoomAdjustment != value)
                {
                    zoomAdjustment = value;
                    if (viewMode != ViewModes.Normal) SetZoom();
                }
            }
        }

        #endregion

        #region ReadOnlyプロパティ

        private bool readOnly = false;
        /// <summary>
        /// ブラウザコントロールが表示専用かどうかを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(typeof(bool), "false"),
            Description("ブラウザコントロールが表示専用かどうかを参照または設定します。")
        ]
        public bool ReadOnly
        {
            get
            {
                return readOnly;
            }
            set
            {
                if (readOnly != value)
                {
                    readOnly = value;
                    SetReadOnly(readOnly);
                }
            }
        }

        #endregion

        #region BlankColorプロパティ

        private Color blankColor = System.Drawing.Color.Black;
        /// <summary>
        /// Clearメソッド実行時に表示されるページの色を参照または設定します。
        /// </summary>
        [
            Category("表示"),
            Description("Clearメソッド実行時に表示されるページの色を参照または設定します。")
        ]
        public Color BlankColor
        {
            get
            {
                return blankColor;
            }
            set
            {
                blankColor = value;
                if (!isRun) Clear();
            }
        }

        #endregion

        #endregion

        #region 既存のプロパティ(変更)

        #region ScrollBarsEnabledプロパティ

        private bool scrollBarsEnabled = true;
        [
            Category("動作"),
            Description("スクロールバーを表示するかどうかを参照または設定します。")
        ]
        /// <summary>
        /// スクロールバーを表示するかどうかを参照または設定します。
        /// </summary>
        public new bool ScrollBarsEnabled
        {
            get
            {
                return scrollBarsEnabled;
            }
            set
            {
                if (scrollBarsEnabled != value)
                {
                    scrollBarsEnabled = value;
                    SetContentsSize(scrollBarsEnabled);
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region イベント

        #region OnDocumentCompletedイベント

        /// <summary>
        /// XControls.XBrowser.DocumentCompletedイベントを発生させます。
        /// </summary>
        protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            isGoNavigate = false;
            base.OnDocumentCompleted(e);

            string nowUrl = base.Url.ToString();
            if (previousUrl != nowUrl)
            {
                // サイズ・位置調整
                SetZoom();
                switch (viewMode)
                {
                    case ViewModes.Fit:
                        SetPosition(new Point(0, 0));
                        break;
                    default:
                        SetPosition(viewPosition);
                        break;
                }
                SetContentsSize(scrollBarsEnabled);
                SetReadOnly(readOnly);
                previousUrl = nowUrl;
            }
        }

        #endregion

        #region OnNavigatingイベント

        /// <summary>
        /// XControls.XBrowser.Navigatingイベントを発生させます。
        /// </summary>
        protected override void OnNavigating(WebBrowserNavigatingEventArgs e)
        {
            if (readOnly && !isGoNavigate)
            {
                e.Cancel = true;
            }
            else
            {
                base.OnNavigating(e);
            }
        }

        #endregion

        #region OnResizeイベント

        /// <summary>
        /// XControls.XBrowser.Resizeイベントを発生させます。
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (!isCapture)
            {
                HtmlDocument htmlDoc = Document;
                if (htmlDoc == null) return;
                HtmlElement element = htmlDoc.Body;
                if (element == null) return;

                base.UpdateBounds();
            }
        }

        #endregion

        #endregion

        #region メソッド

        #region インターネットオプションを開く (OpenInternetOption)

        /// <summary>
        /// インターネットオプションを開きます。
        /// </summary>
        /// <returns>結果(true=成功, false=失敗)</returns>
        public bool OpenInternetOption()
        {
            return LaunchInternetControlPanel(Handle);
        }
        [DllImport("inetcpl.cpl")]
        private static extern bool LaunchInternetControlPanel(IntPtr hWnd);

        #endregion

        #region ブラウズを開始する (Run)

        /// <summary>
        /// ブラウズを開始します。
        /// </summary>
        /// <returns>結果(true=成功, false=失敗)</returns>
        public bool Run()
        {
            bool rt = false;
            Stop();

            // ブラウズを開始
            try
            {
                isGoNavigate = true;
                Navigate(StartURL);
                previousUrl = "";
                isRun = true;
                rt = true;
            }
            catch
            {
            }

            return rt;
        }

        #endregion

        #region ブラウズを終了する (Stop)

        /// <summary>
        /// ブラウズを終了します。
        /// </summary>
        public new void Stop()
        {
            // ブラウズを停止
            base.Stop();
            isRun = false;
        }

        #endregion

        #region コンテンツのキャプチャ画像を取得する (GetCaptureImage)

        /// <summary>
        /// コンテンツのキャプチャ画像を取得します。
        /// キャプチャイメージの破棄は本コントロールが行います。
        /// </summary>
        /// <returns>キャプチャイメージ</returns>
        public Bitmap GetCaptureImage()
        {
            isCapture = true;
            GetCaptureImageInternal();
            isCapture = false;
            return imageOrigin;
        }

        #endregion

        #region コンテンツをクリアする (Clear)

        /// <summary>
        /// コンテンツをクリアします。
        /// </summary>
        public void Clear()
        {
            Stop();
            DocumentText = "<html><head><body bgcolor=\"#" + blankColor.R.ToString("X2") +
                           blankColor.G.ToString("X2") + blankColor.B.ToString("X2") +
                           "\"></body></html>";
        }

        #endregion

        #region 現在のページのFrameリストを返す (GetFrameList)
        /// <summary>
        /// 現在のページのFrameリストを返します
        /// </summary>
        /// <returns></returns>
        public String[] GetFrameList()
        {
            String[] rt = null;

            try
            {
                // ボディー部を取得
                HtmlDocument htmlDoc = Document;
                if (htmlDoc == null) throw new NullReferenceException();
                HtmlElement element = htmlDoc.Body;
                if (element == null) throw new NullReferenceException();

                // FRAMESETを検索
                HtmlElementCollection elc = element.GetElementsByTagName("frame");
                if (elc.Count > 0)
                {
                    String host = Document.Url.ToString();
                    host = host.Substring(0, host.IndexOf("/", 7));
                    rt = new String[elc.Count];
                    int i = 0;
                    foreach (HtmlElement el in elc)
                    {
                        String src = el.GetAttribute("src");
                        if ((src + " ").Substring(0, 1) != "/") src = "/" + src;
                        rt[i] = host + src;
                        i++;
                    }
                }
            }
            catch (NullReferenceException)
            {
                // OK
            }
            catch (Exception es)
            {
                ThreadExceptionDialog ed = new ThreadExceptionDialog(es);
                ed.ShowDialog();
            }

            return rt;
        }

        #endregion

        #region アプリで使用するIEのバージョンを設定します。

        /// <summary>
        /// アプリで使用するIEのバージョンを設定します。
        /// </summary>
        /// <param name="appname">アプリ名("xxx.exe")</param>
        /// <param name="ver">バージョン(IE7=7000,IE8=8000,IE9=9000,IE10=10000,IE11=11000...)</param>
        public static void SetIEVersion(string appname, int ver)
        {
            RegistryKey regkey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION");
            regkey.SetValue(appname, ver, RegistryValueKind.DWord);
            regkey.Close();
        }

        #endregion

        #endregion

        #region 内部処理

        #region ズーム率設定 (SetZoom)

        /// <summary>
        /// ズーム率を設定します。
        /// </summary>
        private void SetZoom()
        {
            try
            {
                // ボディー部を取得
                HtmlDocument htmlDoc = Document;
                if (htmlDoc == null) throw new NullReferenceException();
                HtmlElement element = htmlDoc.Body;
                if (element == null) throw new NullReferenceException();
                IOleObject oleObj = (IOleObject)htmlDoc.DomDocument;
                if (oleObj == null) throw new NullReferenceException();

                double z = 100;
                switch (viewMode)
                {
                    case ViewModes.Normal:
                        z = zoomRate;
                        break;
                    case ViewModes.Fit:
                        // コンテンツサイズ
                        Rectangle conRect = new Rectangle(new Point(0, 0), element.ScrollRectangle.Size);
                        if (ScrollBarsEnabled) conRect.Width += SystemInformation.VerticalScrollBarWidth;

                        // 倍率を決定
                        z = ClientRectangle.Width * 100 / conRect.Width;
                        double zy = ClientRectangle.Height * 100 / conRect.Height;
                        if (z > zy) z = zy;
                        break;
                }

                // 倍率変更
                if (zoomAdjustment)
                {
                    z = (double)(int)((z + 12.5) / 25) * 25;
                    if (z < 25) z = 25;
                }
                object izoom = (Int32)z;
                object ozoom = (Int32)0;
                ((SHDocVw.IWebBrowser2)ActiveXInstance).ExecWB(SHDocVw.OLECMDID.OLECMDID_OPTICAL_ZOOM,
                                                                SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT,
                                                                ref izoom, ref ozoom);
                zoomNowRate = (Int32)ozoom;

                // 表示域変更
                isZoomUpdate = true;
                SetContentsSize(scrollBarsEnabled);
                isZoomUpdate = false;
            }
            catch (NullReferenceException)
            {
                // OK
            }
            catch (Exception es)
            {
                ThreadExceptionDialog ed = new ThreadExceptionDialog(es);
                ed.ShowDialog();
            }
        }

        #endregion

        #region コンテンツ表示位置設定 (SetPosition)

        /// <summary>
        /// コンテンツ表示位置を設定する
        /// </summary>
        /// <param name="pos">左上の座標</param>
        private void SetPosition(Point pos)
        {
            try
            {
                if (Document == null) throw new NullReferenceException();
                Document.Window.Parent.ScrollTo(pos);
                viewNowPosition = pos;
            }
            catch (NullReferenceException)
            {
                // OK
            }
            catch (Exception es)
            {
                ThreadExceptionDialog ed = new ThreadExceptionDialog(es);
                ed.ShowDialog();
            }
        }

        #endregion

        #region スクロールバー非表示に関連してコンテンツサイズを調整 (SetContentsSize)

        /// <summary>
        /// コンテンツサイズを調整する
        /// スクロールバーを非表示にするとスクロールしなくなるので表示域の外に追い出す
        /// </summary>
        /// <param name="isShowBar">スクロールバー有無</param>
        private void SetContentsSize(bool isShowBar)
        {
            if (this.FindForm() == null) return;
            if (this.FindForm().WindowState == FormWindowState.Minimized) return;
            if (isSetScrollBar != isShowBar || isZoomUpdate)
            {
                isSetScrollBar = isShowBar;
                try
                {
                    // ボディー部を取得
                    HtmlDocument htmlDoc = Document;
                    if (htmlDoc == null) throw new NullReferenceException();
                    HtmlElement element = htmlDoc.Body;
                    if (element == null) throw new NullReferenceException();
                    IOleObject oleObj = (IOleObject)htmlDoc.DomDocument;
                    if (oleObj == null) throw new NullReferenceException();

                    // スクロール位置を記憶
                    Point scrOrg = new Point();
                    scrOrg.X = element.Parent.ScrollLeft;
                    scrOrg.Y = element.Parent.ScrollTop;

                    Graphics g = CreateGraphics();
                    IntPtr dc = g.GetHdc();

                    double z = zoomRate;
                    if (zoomAdjustment)
                    {
                        z = (double)(int)((z + 12.5) / 25) * 25;
                        if (z < 25) z = 25;
                    }

                    Size ctlSize = new Size((int)(((double)(this.Size.Width * 100)) / zoomRate),
                                            (int)(((double)(this.Size.Height * 100)) / zoomRate));
                    Size curSize = PixelToHIMETRIC(ctlSize, dc);
                    Size barSize = new Size((int)(((double)(SystemInformation.VerticalScrollBarWidth * 1.5) * 100) / zoomRate),
                                            (int)(((double)(SystemInformation.HorizontalScrollBarHeight * 1.5) * 100) / zoomRate));
                    Size ofsSize = PixelToHIMETRIC(barSize, dc);
                    if (!scrollBarsEnabled)
                    {
                        curSize.Width += ofsSize.Width;
                        curSize.Height += ofsSize.Height;
                    }

                    g.ReleaseHdc(dc);
                    g.Dispose();

                    try
                    {
                        // コンテンツサイズ設定
                        oleObj.SetExtent(DVASPECT.DVASPECT_CONTENT, ref curSize);

                        // スクロール位置を戻す
                        SetPosition(scrOrg);
                    }
                    catch (System.Runtime.InteropServices.COMException)
                    {
                        // 失敗 - 何もしない
                    }
                }
                catch (NullReferenceException)
                {
                    // OK
                }
                catch (Exception es)
                {
                    ThreadExceptionDialog ed = new ThreadExceptionDialog(es);
                    ed.ShowDialog();
                }
            }
        }

        #endregion

        #region コンテンツのユーザー操作の可否を設定する (SetReadOnly)

        /// <summary>
        /// コンテンツのユーザー操作の可否を設定します。
        /// </summary>
        /// <param name="isReadOnly">読み取り専用</param>
        private void SetReadOnly(bool isReadOnly)
        {
            try
            {
                // ボディー部を取得
                HtmlDocument htmlDoc = Document;
                if (htmlDoc == null) throw new NullReferenceException();
                HtmlElement element = htmlDoc.Body;
                if (element == null) throw new NullReferenceException();
                IOleObject oleObj = (IOleObject)htmlDoc.DomDocument;
                if (oleObj == null) throw new NullReferenceException();

                // 入力フィールド
                element.Parent.Enabled = !isReadOnly;
            }
            catch (NullReferenceException)
            {
                // OK
            }
            catch (Exception es)
            {
                ThreadExceptionDialog ed = new ThreadExceptionDialog(es);
                ed.ShowDialog();
            }
            return;
        }

        #endregion

        #region コンテンツキャプチャ処理 (GetCaptureImageInternal)

        delegate void GetCaptureImageInternalDelegate();

        /// <summary>
        /// コンテンツキャプチャ
        /// </summary>
        private void GetCaptureImageInternal()
        {
            if (InvokeRequired)
            {
                // 別スレッドから呼び出された場合
                Invoke(new GetCaptureImageInternalDelegate(GetCaptureImageInternal));
                return;
            }

            Bitmap rt = null;
            Size curSize;
            try
            {
                #region コンテンツのボディー部を取得

                HtmlDocument htmlDoc = Document;
                if (htmlDoc == null) throw new NullReferenceException();
                HtmlElement element = htmlDoc.Body;
                if (element == null) throw new NullReferenceException();
                IOleObject oleObj = (IOleObject)htmlDoc.DomDocument;
                if (oleObj == null) throw new NullReferenceException();

                #endregion

                #region コンテンツの情報を記憶

                // スクロール位置を記憶
                Point scrOrg = new Point();
                scrOrg.X = element.Parent.ScrollLeft;
                scrOrg.Y = element.Parent.ScrollTop;

                #endregion

                #region コンテンツをキャプチャ用に調整

                // 一時的に拡大率を変更する
                if (zoomNowRate != captureZoomRate)
                {
                    SHDocVw.IWebBrowser2 browser = (SHDocVw.IWebBrowser2)ActiveXInstance;
                    object izoom = (Int32)captureZoomRate;
                    object ozoom = (Int32)0;
                    browser.ExecWB(SHDocVw.OLECMDID.OLECMDID_OPTICAL_ZOOM,
                                   SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT,
                                   ref izoom, ref ozoom);
                }

                // コンテンツサイズ計算(スクロールバー分大きくする)
                oleObj.GetExtent(DVASPECT.DVASPECT_CONTENT, out curSize);
                Rectangle conRect = new Rectangle(0, 0,
                                                (int)(element.ScrollRectangle.Size.Width * captureZoomRate / 100 + 0.5),
                                                (int)(element.ScrollRectangle.Size.Height * captureZoomRate / 100 + 0.5)
                                                );
                conRect.Width += SystemInformation.VerticalScrollBarWidth;
                conRect.Height += SystemInformation.HorizontalScrollBarHeight;

                #endregion

                if (captureDrawMode == CaptureDrawModes.OleDraw)
                {
                    rt = new Bitmap(conRect.Width, conRect.Height);
                }
                else
                {
                    rt = new Bitmap(this.Size.Width, this.Size.Height);
                }
                Graphics g = Graphics.FromImage(rt);
                IntPtr dc = g.GetHdc();
                try
                {
                    #region コンテンツ全体をキャプチャ

                    switch (captureDrawMode)
                    {
                        case CaptureDrawModes.OleDraw:
                            #region OleDrawによるキャプチャ
                            {
                                IntPtr pUnk = Marshal.GetIUnknownForObject(ActiveXInstance);
                                try
                                {
                                    // コンテンツ描画範囲をコンテンツサイズに設定
                                    Size drawSize = PixelToHIMETRIC(conRect.Size, dc);
                                    oleObj.SetExtent(DVASPECT.DVASPECT_CONTENT, ref drawSize);

                                    // 描画
                                    OleDraw(pUnk, DVASPECT.DVASPECT_CONTENT, dc, ref conRect);
                                }
                                catch (Exception es)
                                {
                                    ThreadExceptionDialog ed = new ThreadExceptionDialog(es);
                                    ed.ShowDialog();
                                }
                                finally
                                {
                                    Marshal.Release(pUnk);
                                }
                            }
                            #endregion
                            break;

                        case CaptureDrawModes.PrintWindow:
                            #region PrintWindowによるキャプチャ
                            {
                                // 表示サイズで取得
                                PrintWindow(Handle, dc, 0);
                            }
                            #endregion
                            break;
                    }

                    #endregion
                }
                catch (Exception es)
                {
                    ThreadExceptionDialog ed = new ThreadExceptionDialog(es);
                    ed.ShowDialog();
                }
                finally
                {
                    g.ReleaseHdc(dc);
                    g.Dispose();
                }

                #region コンテンツを元に戻す

                // 拡大率とコンテンツサイズを戻す
                if (zoomNowRate != captureZoomRate)
                {
                    SHDocVw.IWebBrowser2 browser = (SHDocVw.IWebBrowser2)ActiveXInstance;
                    object izoom = (Int32)zoomNowRate;
                    object ozoom = (Int32)0;
                    browser.ExecWB(SHDocVw.OLECMDID.OLECMDID_OPTICAL_ZOOM,
                                   SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT,
                                   ref izoom, ref ozoom);
                }
                oleObj.SetExtent(DVASPECT.DVASPECT_CONTENT, ref curSize);

                // スクロール位置を戻す
                SetPosition(scrOrg);

                #endregion
            }
            catch (NullReferenceException)
            {
                if (rt == null) rt = new Bitmap(1, 1);
            }
            catch (Exception es)
            {
                ThreadExceptionDialog ed = new ThreadExceptionDialog(es);
                ed.ShowDialog();
            }
            imageOrigin = rt;
        }
        private Size PixelToHIMETRIC(Size size, IntPtr hdc)
        {
            const int HIMETRIC_PER_INCH = 2540;

            Size newSize = new Size();

            newSize.Width = (int)((double)size.Width * HIMETRIC_PER_INCH / GetDeviceCaps(hdc, DEVICECAPS.LOGPIXELSX) + 0.5);
            newSize.Height = (int)((double)size.Height * HIMETRIC_PER_INCH / GetDeviceCaps(hdc, DEVICECAPS.LOGPIXELSY) + 0.5);

            return newSize;
        }
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private extern static bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);
        [DllImport("ole32")]
        private static extern int OleDraw(IntPtr pUnk, DVASPECT dwAspect, IntPtr hdcDraw, ref Rectangle lprcBounds);
        public enum DEVICECAPS
        {
            LOGPIXELSX = 88,
            LOGPIXELSY = 90,
        }
        [DllImport("gdi32")]
        private static extern int GetDeviceCaps(IntPtr hdc, DEVICECAPS caps);

        #endregion

        #endregion
    }
}
