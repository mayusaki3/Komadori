using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TirolPlaySystem
{
    /// <summary>
    /// ライブキャプチャ用ウィンドウです。
    /// </summary>
    public partial class LiveCapForm : Form
    {
        #region 構築・破棄

        /// <summary>
        /// TirolPlaySystem.LiveCapForm クラスの新しいインスタンスを初期化します。
        /// </summary>
        public LiveCapForm()
        {
            InitializeComponent();
            topForm = new LiveCapTopForm();
            TopMessage = "";
            BottomMessage = "";
            PlayerInfo = "";
        }

        #endregion

        #region 定数

        /// <summary>
        /// 画面のモードを指定する定数です。
        /// </summary>
        public enum ViewModes : int
        {
            /// <summary>
            /// ブルースクリーンを表示します。
            /// </summary>
            None,
            /// <summary>
            /// カラーバーを表示します。
            /// </summary>
            ColorBar,
            /// <summary>
            /// 会場モニターを表示します。
            /// </summary>
            Monitor,
            /// <summary>
            /// 大会情報を表示します。
            /// </summary>
            InfoBar
        }

        #endregion

        #region 変数

        /// <summary>
        /// フォームを閉じなくするかどうかを表します。
        /// </summary>
        private bool isCancel = true;

        /// <summary>
        /// ライブキャプチャ用ウィンドウ（最前面用）
        /// </summary>
        private LiveCapTopForm topForm;


        #endregion

        #region プロパティ

        #region ViewModeプロパティ

        private ViewModes viewMode = ViewModes.None;
        /// <summary>
        /// 表示モードを参照または設定します。メッセージは初期化されます。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(ViewModes.None),
            Description("表示モードを参照または設定します。メッセージは初期化されます。")
        ]
        public ViewModes ViewMode
        {
            get
            {
                return viewMode;
            }
            set
            {
                viewMode = value;
                switch (viewMode)
                {
                    case ViewModes.None:
                        pnlColorBar.Visible = false;
                        xscrview.Enabled = false;
                        xscrview.Visible = false;
                        break;
                    case ViewModes.ColorBar:
                        pnlColorBar.Visible = true;
                        xscrview.Enabled = false;
                        xscrview.Visible = false;
                        break;
                    case ViewModes.Monitor:
                        xscrview.Enabled = true;
                        xscrview.Visible = true;
                        pnlColorBar.Visible = false;
                         break;
                    case ViewModes.InfoBar:
                        pnlColorBar.Visible = false;
                        xscrview.Enabled = false;
                        xscrview.Visible = false;
                        break;
                }
                topForm.ViewMode = viewMode;
            }
        }

        #endregion

        #region TopMessageプロパティ

        /// <summary>
        /// 画面上部に表示するメッセージを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(""),
            Description("画面上部に表示するメッセージを参照または設定します。")
        ]
        public string TopMessage
        {
            get
            {
                return topForm.TopMessage;
            }
            set
            {
                topForm.TopMessage = value;

            }
        }

        #endregion

        #region BottomMessageプロパティ

        /// <summary>
        /// 画面下部に表示するメッセージを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(""),
            Description("画面下部に表示するメッセージを参照または設定します。")
        ]
        public string BottomMessage
        {
            get
            {
                return topForm.BottomMessage;
            }
            set
            {
                topForm.BottomMessage = value;

            }
        }

        #endregion

        #region PlayerInfoプロパティ

        /// <summary>
        /// 画面下部に表示するプレイヤー情報を参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(""),
            Description("画面下部に表示するプレイヤー情報を参照または設定します。")
        ]
        public string PlayerInfo
        {
            get
            {
                return topForm.PlayerInfo;
            }
            set
            {
                topForm.PlayerInfo = value;
            }
        }

        #endregion

        #region Timerプロパティ

        /// <summary>
        /// 画面下部に表示する時間へのアクセスを提供します。
        /// </summary>
        [
            Category("動作"),
            Description("画面下部に表示する時間へのアクセスを提供します。")
        ]
        public XControls.UI.TimerView Timer
        {
            get
            {
                return topForm.Timer;
            }
        }

        #endregion

        #region Counterプロパティ

        /// <summary>
        /// 画面下部に表示するカウンタへのアクセスを提供します。
        /// </summary>
        [
            Category("動作"),
            Description("画面下部に表示するカウンタへのアクセスを提供します。")
        ]
        public XControls.UI.TimerView Counter
        {
            get
            {
                return topForm.Counter;
            }
        }

        #endregion

        #region ScreenIndexプロパティ

        /// <summary>
        /// 会場モニターのスクリーンのインデックスを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            Description("会場モニターのスクリーンのインデックスを参照または設定します。")
        ]
        public int ScreenIndex
        {
            get
            {
                return xscrview.ScreenIndex;
            }
            set
            {
                xscrview.ScreenIndex= value;
            }
        }

        #endregion

        #region Fieldプロパティ

        /// <summary>
        /// 画面右上に表示する競技フィールドへのアクセスを提供します。
        /// </summary>
        [
            Category("動作"),
            Description("画面右上に表示する競技フィールドへのアクセスを提供します。")
        ]
        public XControls.UI.FieldView Field
        {
            get
            {
                return topForm.Field;
            }
        }

        #endregion

        #region FieldShowプロパティ

        /// <summary>
        /// 画面右上に表示する競技フィールドへのアクセスを提供します。
        /// </summary>
        [
            Category("動作"),
            Description("画面右上に表示する競技フィールドへのアクセスを提供します。")
        ]
        public bool FieldShow
        {
            get
            {
                return topForm.Field.Visible;
            }
            set
            {
                topForm.Field.Visible = value;
            }
        }

        #endregion

        #endregion

        #region イベント

        #region OnShownイベント

        /// <summary>
        /// TirolPlaySystem.LiveCapForm.Shownイベントを発生させます。
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            topForm.Top = this.Top;
            topForm.Left = this.Left + this.Width;
            topForm.Show();
            ViewMode = ViewModes.ColorBar;
        }

        #endregion

        #region OnClosingイベント

        /// <summary>
        /// TirolPlaySystem.LiveCapForm.Closingイベントを発生させます。
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (isCancel) e.Cancel = true;
        }

        #endregion

        #endregion

        #region メソッド

        #region FormCloseメソッド

        /// <summary>
        /// フォームを閉じます。
        /// </summary>
        public void FormClose()
        {
            xscrview.Enabled = false;
            isCancel = false;
            topForm.FormClose();
            Close();
        }

        #endregion

        #endregion
    }
}
