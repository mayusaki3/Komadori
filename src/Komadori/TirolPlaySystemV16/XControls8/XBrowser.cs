using Microsoft.Web.WebView2.WinForms;
using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace XControls
{
    /// <summary>
    /// WebBrowserの機能拡張版コントロールです。
    /// </summary>
    /// <remarks>
    /// 次のNuGetパッケージをプロジェクトの「NuGetパッケージの管理」で追加してください。
    ///     Microsoft.Web.WebView2
    /// </remarks>
    [Designer(typeof(XBrowserDesigner))]
    public class XBrowser : Panel
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
                //properties.Remove("Uri");

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
        //        : base()
        {
            webview.Dock = DockStyle.Fill;
            base.Controls.Add(webview);
            base.ParentChanged += XBrowser_ParentChanged;
        }

        /// <summary>
        /// インスタンスを破棄します。
        /// </summary>
        ~XBrowser()
        {
            Stop();
        }

        #endregion


        #region 定数

        #endregion

        #region 変数

        private WebView2 webview = new WebView2();

        #endregion

        #region イベント

        private void XBrowser_ParentChanged(object sender, EventArgs e)
        {
            webview.Dock = DockStyle.None;
            webview.Dock = DockStyle.Fill;
        }

        #endregion

        #region メソッド

        #region ブラウズを開始する (Run)

        /// <summary>
        /// ブラウズを開始します。
        /// </summary>
        /// <returns>結果(true=成功, false=失敗)</returns>
        public bool Run()
        {
            bool rt = false;
            Stop();

            if (StartURL.Length == 0)
            {
                return true;
            }

            // ブラウズを開始
            try
            {
                webview.Source = new Uri(StartURL);
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
        public void Stop()
        {
            // ブラウズを停止
            webview.Stop();
        }

        #endregion

        #endregion

        //    #region 内部処理

        //    #region コンテンツ表示位置設定 (SetPosition)

        //    /// <summary>
        //    /// コンテンツ表示位置を設定する
        //    /// </summary>
        //    /// <param name="pos">左上の座標</param>
        //    private void SetPosition(Point pos)
        //    {
        //        try
        //        {
        //            //TODO: WebView2
        //            //if (Document == null) throw new NullReferenceException();
        //            //Document.Window.Parent.ScrollTo(pos);
        //            //viewNowPosition = pos;
        //        }
        //        catch (NullReferenceException)
        //        {
        //            // OK
        //        }
        //        catch (Exception es)
        //        {
        //            ThreadExceptionDialog ed = new ThreadExceptionDialog(es);
        //            ed.ShowDialog();
        //        }
        //    }

        //    #endregion

        //    #endregion
        //    private Size PixelToHIMETRIC(Size size, IntPtr hdc)
        //    {
        //        const int HIMETRIC_PER_INCH = 2540;

        //        Size newSize = new Size();

        //        newSize.Width = (int)((double)size.Width * HIMETRIC_PER_INCH / GetDeviceCaps(hdc, DEVICECAPS.LOGPIXELSX) + 0.5);
        //        newSize.Height = (int)((double)size.Height * HIMETRIC_PER_INCH / GetDeviceCaps(hdc, DEVICECAPS.LOGPIXELSY) + 0.5);

        //        return newSize;
        //    }
        //    public enum DEVICECAPS
        //    {
        //        LOGPIXELSX = 88,
        //        LOGPIXELSY = 90,
        //    }
        //    [DllImport("gdi32")]
        //    private static extern int GetDeviceCaps(IntPtr hdc, DEVICECAPS caps);
        //}


        #region プロパティ

        #region 追加のプロパティ

        #region StartURLプロパティ

        /// <summary>
        /// ブラウズ開始時のURLを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(""),
            Description("ブラウズ開始時のURLを参照または設定します。")
        ]
        public string StartURL { set; get; } = "";

        #endregion

        //    #region CurrentURLプロパティ

        //    /// <summary>
        //    /// 現在表示しているページのURLを示します。
        //    /// </summary>
        //    [
        //        Category("動作"),
        //        Description("現在表示しているページのURLを示します。")
        //    ]
        public string CurrentURL { set; get; } = "";
        //    public String CurrentURL
        //    {
        //        get
        //        {
        //            if (webview.Source == null)
        //            {
        //                return "";
        //            }
        //            return webview.Source.ToString();
        //        }
        //    }

        //    #endregion

        //    #region ViewPositionプロパティ

        //    private Point viewPosition = new Point(0, 0);
        //    /// <summary>
        //    /// コンテンツの左上座標を参照または設定します。
        //    /// </summary>
        //    [
        //        Category("動作"),
        //        DefaultValue(typeof(Point), "0, 0"),
        //        Description("コンテンツの左上座標を参照または設定します。")
        //    ]
        public Point ViewPosition { set; get; } = new Point(0, 0);
        //    public Point ViewPosition
        //    {
        //        get
        //        {
        //            return viewPosition;
        //        }
        //        set
        //        {
        //            if (viewPosition != value)
        //            {
        //                viewPosition = value;
        //            }
        //        }
        //    }

        //    #endregion

        //    #region ViewNowPositionプロパティ

        //    private Point viewNowPosition = new Point(0, 0);
        //    /// <summary>
        //    /// 実際に表示されているページの左上座標を示します。
        //    /// </summary>
        //    [
        //        Category("動作"),
        //        Description("実際に表示されているページの左上座標を示します。")
        //    ]
        public Point ViewNowPosition { set; get; } = new Point(0, 0);
        //    public Point ViewNowPosition
        //    {
        //        get
        //        {
        //            return new Point(0, 0);// webview.CoreWebView2..;
        //        }
        //    }

        //    #endregion

        //    #region ZoomRateプロパティ

        //    /// <summary>
        //    /// ページ表示の拡大率(%)を参照または設定します。
        //    /// </summary>
        //    [
        //        Category("動作"),
        //        DefaultValue(typeof(Double), "100"),
        //        Description("ページ表示の拡大率(%)を参照または設定します。")
        //    ]
        public double ZoomRate { set; get; } = 100;
        //    public Double ZoomRate
        //    {
        //        get
        //        {
        //            return (Int32)(webview.ZoomFactor * 100); ;
        //        }
        //        set
        //        {
        //            webview.ZoomFactor = value / 100;
        //        }
        //    }

        //    #endregion

        #endregion

        #endregion
    }
}
