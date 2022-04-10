using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TirolPlaySystem
{
    /// <summary>
    /// 会場モニタ用ウィンドウです。
    /// </summary>
    public partial class MonitorForm : Form
    {
        #region 構築・破棄

        /// <summary>
        /// TirolPlaySystem.MonitorForm クラスの新しいインスタンスを初期化します。
        /// </summary>
        public MonitorForm()
        {
            InitializeComponent();
            ContestName = "";
            GameName = "";
            RobotName = "";
            PlayerName = "";
            pnlColorBar.Dock = DockStyle.Fill;
            pnlMonitor.Dock = DockStyle.Fill;
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
            /// スコアモニターを表示します。
            /// </summary>
            Monitor,
            /// <summary>
            /// スコアモニターとプレイヤー情報を表示します。
            /// </summary>
            InfoView
        }

        #endregion

        #region 変数

        /// <summary>
        /// フォームを閉じなくするかどうかを表します。
        /// </summary>
        private bool isCancel = true;

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
                        pnlMonitor.Visible = false;
                        pnlPlayer.Visible = false;
                        xweb.Visible = false;
                        lblGameInfo.Enabled = false;
                        break;
                    case ViewModes.ColorBar:
                        pnlColorBar.Visible = true;
                        pnlMonitor.Visible = false;
                        pnlPlayer.Visible = false;
                        xweb.Visible = false;
                        lblGameInfo.Enabled = false;
                        break;
                    case ViewModes.Monitor:
                        pnlColorBar.Visible = false;
                        pnlMonitor.Visible = true;
                        pnlPlayer.Visible = false;
                        xweb.Visible = true;
                        lblGameInfo.Enabled = true;
                        break;
                    case ViewModes.InfoView:
                        pnlColorBar.Visible = false;
                        pnlMonitor.Visible = true;
                        pnlPlayer.Parent = this;
                        pnlPlayer.Visible = true;
                        xweb.Visible = true;
                        pnlPlayer.BringToFront();
                        lblGameInfo.Enabled = true;
                        break;
                }
            }
        }

        #endregion

        #region ContestNameプロパティ

        private string contestName = "";
        /// <summary>
        /// 画面上部に表示する大会名を参照または設定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(""),
            Description("画面上部に表示する大会名を参照または設定します。")
        ]
        public string ContestName
        {
            get
            {
                return contestName;
            }
            set
            {
                contestName = value;
                SetText();
            }
        }

        #endregion

        #region GameNameプロパティ

        private string gameName = "";
        /// <summary>
        /// 画面上部に表示するゲーム名を参照または設定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(""),
            Description("画面上部に表示するゲーム名を参照または設定します。")
        ]
        public string GameName
        {
            get
            {
                return gameName;
            }
            set
            {
                gameName = value;
                SetText();
            }
        }

        #endregion

        #region RobotNameプロパティ

        private string robotName = "";
        /// <summary>
        /// 画面上部に表示するロボット名を参照または設定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(""),
            Description("画面上部に表示するロボット名を参照または設定します。")
        ]
        public string RobotName
        {
            get
            {
                return robotName;
            }
            set
            {
                robotName = value;
                SetText();
            }
        }

        #endregion

        #region PlayerNameプロパティ

        private string playerName = "";
        /// <summary>
        /// 画面上部に表示するプレイヤー名を参照または設定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(""),
            Description("画面上部に表示するプレイヤー名を参照または設定します。")
        ]
        public string PlayerName
        {
            get
            {
                return playerName;
            }
            set
            {
                playerName = value;
                SetText();
            }
        }

        #endregion

        #region Counterプロパティ

        /// <summary>
        /// 画面に表示する獲得数へのアクセスを提供します。
        /// </summary>
        [
            Category("動作"),
            Description("画面に表示する獲得数へのアクセスを提供します。")
        ]
        public XControls.UI.TimerView Counter
        {
            get
            {
                return xtimCap;
            }
        }

        #endregion

        #region Handiプロパティ

        /// <summary>
        /// 画面に表示する段数へのアクセスを提供します。
        /// </summary>
        [
            Category("動作"),
            Description("画面に表示する段数へのアクセスを提供します。")
        ]
        public XControls.UI.TimerView Handi
        {
            get
            {
                return xtimHandi;
            }
        }

        #endregion

        #region Timerプロパティ

        /// <summary>
        /// 画面に表示する残り時間へのアクセスを提供します。
        /// </summary>
        [
            Category("動作"),
            Description("画面に表示する残り時間へのアクセスを提供します。")
        ]
        public XControls.UI.TimerView Timer
        {
            get
            {
                return xtimer;
            }
        }

        #endregion

        #region Imageプロパティ

        /// <summary>
        /// 画面に表示するロボット写真へのアクセスを提供します。
        /// </summary>
        [
            Category("動作"),
            Description("画面に表示するロボット写真へのアクセスを提供します。")
        ]
        public XControls.XCamView Image
        {
            get
            {
                return xpicRobot;
            }
        }

        #endregion

        #region Browserプロパティ

        /// <summary>
        /// 画面に表示するブラウザへのアクセスを提供します。
        /// </summary>
        [
            Category("動作"),
            Description("画面に表示するブラウザへのアクセスを提供します。")
        ]
        public XControls.XBrowser Browser
        {
            get
            {
                return xweb;
            }
        }

        #endregion

        #region MaskLプロパティ

        /// <summary>
        /// 画面に表示するブラウザへの右側マスキング幅へのアクセスを提供します。
        /// </summary>
        [
            Category("動作"),
            Description("画面に表示するブラウザへの右側マスキング幅へのアクセスを提供します。")
        ]
        public int MaskL
        {
            get
            {
                return picMaskL.Width;
            }
            set
            {
                picMaskL.Width = value;
                picMaskL.Left = 0;
            }
        }

        #endregion

        #region MaskRプロパティ

        /// <summary>
        /// 画面に表示するブラウザへの右側マスキング幅へのアクセスを提供します。
        /// </summary>
        [
            Category("動作"),
            Description("画面に表示するブラウザへの右側マスキング幅へのアクセスを提供します。")
        ]
        public int MaskR
        {
            get
            {
                return picMaskR.Width;
            }
            set
            {
                picMaskR.Width = value;
                picMaskR.Left = this.Width - picMaskR.Width;
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
                return field;
            }
        }

        #endregion

        #endregion

        #region イベント

        #region OnShownイベント

        /// <summary>
        /// TirolPlaySystem.MonitorForm.Shownイベントを発生させます。
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ViewMode = ViewModes.ColorBar;
        }

        #endregion

        #region OnClosingイベント

        /// <summary>
        /// TirolPlaySystem.MonitorForm.Closingイベントを発生させます。
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (isCancel) e.Cancel = true;
        }

        #endregion

        #region OnResizeイベント

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            pnlPlayer.Top = (Height - pnlPlayer.Height) / 2;
            pnlPlayer.Left = (Width - pnlPlayer.Width) / 2;
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
            isCancel = false;
            Close();
        }

        #endregion

        #endregion

        #region 内部処理

        #region 各種テキストを設定 (SetText)

        /// <summary>
        /// 各種テキストを設定します。
        /// </summary>
        private void SetText()
        {
            lblPlayer.Text = playerName;
            lblRobot.Text = robotName;
            lblGameInfo.Text = contestName + " " +
                               gameName + "  " +
                               "ロボット名:" + robotName + " " +
                               "プレイヤー名:" + playerName + "  ";
        }

        #endregion

        #endregion
    }
}
