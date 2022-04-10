using System;
using System.Net;
using System.Windows.Forms;

namespace XControls
{
    /// <summary>
    /// SystemHubの設定ウィンドウ機能を提供します。
    /// </summary>
    public partial class SettingsForm : Form
    {
        #region 構築・破棄

        /// <summary>
        /// XControls.SettingsForm クラスの新しいインスタンスを初期化します。
        /// </summary>        
        public SettingsForm()
        {
            InitializeComponent();

            SystemHub.Console = this.logBox;
            //hub = SystemHub.GetInstance();

            // 表示位置調整
            Left = Screen.PrimaryScreen.WorkingArea.Right - Width;
            Top = Screen.PrimaryScreen.WorkingArea.Bottom - Height;

            // 画面非表示
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            tabBase.SelectedIndex = 2;
        }

        /// <summary>
        /// 後処理を行います。
        /// </summary>
        private void Terminate()
        {
            //SystemHub.ReleaseInstance(hub);
        }

        #endregion

        #region 変数

        /// <summary>
        /// SystemHubのインスタンスを保持します。
        /// </summary>
        private SystemHub hub = null;

        /// <summary>
        /// 画面を閉じないかどうかを設定します。
        /// </summary>
        private bool isCancelClose = true;

        #endregion

        #region イベント

        #region OnClosingイベント

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            if (isCancelClose) e.Cancel = true;
            base.OnClosing(e);
        }

        #endregion

        #region OnClosedイベント

        protected override void OnClosed(EventArgs e)
        {
            // トレイアイコン消去
            notifyIcon.Visible = false;
            notifyIcon.Dispose();

            base.OnClosed(e);
        }

        #endregion

        #region OnKeyDownイベント

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if(e.KeyCode == Keys.F5)
            {
                // 情報タブ更新
                infoBox.Clear();

                infoBox.PrintLine("システム情報 (F5キーで最新に更新)");
                infoBox.PrintLine("--------------------------------------------------------------------------------");

                //#region コンピュータ名およびネットワークI/F

                //infoBox.PrintLine("◇コンピュータ名:                   " + hub.GetHostName());

                //infoBox.PrintLine("◇ネットワークインターフェース:");
                //infoBox.PrintLine("  IP               MASK             BROADCAST");
                //IPAddress[] iplst = hub.GetLocalIPAddressList();
                //if(iplst==null)
                //{
                //    infoBox.PrintLine("- ネットワークインターフェースがありません。");
                //}
                //else
                //{
                //    foreach(IPAddress ip in iplst)
                //    {
                //        try
                //        {
                //            infoBox.PrintLine("  " + ip.ToString().PadRight(15) +
                //                              "  " + hub.GetLocalSubnetMask(ip).ToString().PadRight(15) +
                //                              "  " + hub.GetBroadcastAddress(ip));
                //        }
                //        catch(Exception es)
                //        {
                //            infoBox.PrintLine(es.ToString());
                //        }
                //    }
                //}

                //#endregion

                //hub.PingTest("192.168.1.1");
                //hub.PingTest("google.com");
                

            }
        }

        #endregion

        #region MenuSettings_Clickイベント

        private void MenuSettings_Click(object sender, EventArgs e)
        {
            // ウィンドウを表示
            tabBase.SelectedIndex = 1;
            ShowWindow();
        }

        #endregion

        #region MenuExit_Clickイベント

        private void MenuExit_Click(object sender, EventArgs e)
        {
            // 終了
            isCancelClose = false;
            Close();
        }

        #endregion



        #region イベント



        #endregion


        #endregion

        #region 内部処理

        #region 画面表示 (ShowWindow)

        /// <summary>
        /// 画面を表示します。
        /// </summary>
        private void ShowWindow()
        {
            // 表示
            if (WindowState == FormWindowState.Minimized) WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            Left = Screen.PrimaryScreen.WorkingArea.Right - Width;
            Top = Screen.PrimaryScreen.WorkingArea.Bottom - Height;
            Show();
            Focus();
        }

        #endregion


        #endregion
    }
}
