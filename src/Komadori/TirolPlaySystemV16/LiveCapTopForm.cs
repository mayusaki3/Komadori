using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TirolPlaySystem
{
    /// <summary>
    /// ライブキャプチャ用ウィンドウです。
    /// </summary>
    public partial class LiveCapTopForm : Form
    {
        #region 構築・破棄

        /// <summary>
        /// TirolPlaySystem.LiveCapForm クラスの新しいインスタンスを初期化します。
        /// </summary>
        public LiveCapTopForm()
        {
            InitializeComponent();
            TopMessage = "";
            BottomMessage = "";
            PlayerInfo = "";
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

        private LiveCapForm.ViewModes viewMode = LiveCapForm.ViewModes.None;
        /// <summary>
        /// 表示モードを参照または設定します。メッセージは初期化されます。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(LiveCapForm.ViewModes.None),
            Description("表示モードを参照または設定します。メッセージは初期化されます。")
        ]
        public LiveCapForm.ViewModes ViewMode
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
                    case LiveCapForm.ViewModes.None:
                        field.Visible = false;
                        if (pnlTime.Visible)
                        {
                            lblMsgB.Top += pnlTime.Height;
                            pnlTime.Visible = false;
                            lblPlayer.Enabled = false;
                        }
                        break;
                    case LiveCapForm.ViewModes.ColorBar:
                        if (pnlTime.Visible)
                        {
                            lblMsgB.Top += pnlTime.Height;
                            pnlTime.Visible = false;
                            lblPlayer.Enabled = false;
                        }
                        field.Visible = false;
                        break;
                    case LiveCapForm.ViewModes.Monitor:
                        field.Visible = false;
                        if (pnlTime.Visible)
                        {
                            lblMsgB.Top += pnlTime.Height;
                            pnlTime.Visible = false;
                            lblPlayer.Enabled = false;
                        }
                        break;
                    case LiveCapForm.ViewModes.InfoBar:
                        if (!pnlTime.Visible)
                        {
                            lblMsgB.Top -= pnlTime.Height;
                            pnlTime.Visible = true;
                            lblPlayer.Enabled = true;
                        }
                        field.Parent = this;
                        field.Visible = fieldShow;
                        break;
                }
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
                return lblMsgT.Text;
            }
            set
            {
                lblMsgT.Text = value;

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
                return lblMsgB.Text;
            }
            set
            {
                lblMsgB.Text = value;

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
                return lblPlayer.Text;
            }
            set
            {
                lblPlayer.Text = value;
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
                return xtime;
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
                return xcount;
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

        #region FieldShowプロパティ

        private bool fieldShow = false;
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
                return fieldShow;
            }
            set
            {
                fieldShow = value;
                if (viewMode == LiveCapForm.ViewModes.InfoBar) field.Visible = fieldShow;
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
            ViewMode = LiveCapForm.ViewModes.ColorBar;
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
            lblPlayer.Enabled = false;
            isCancel = false;
            Close();
        }

        #endregion

        #endregion
    }
}
