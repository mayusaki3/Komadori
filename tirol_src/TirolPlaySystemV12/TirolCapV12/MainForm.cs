using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XControls;
using XControls.IO;
using XControls.UI;

namespace TirolPlaySystem
{
    /// <summary>
    /// チロルチョコロボット大会運用システムコンソール用ウィンドウです。
    /// </summary>
    public partial class MainForm : Form
    {
        #region 構築・破棄

        /// <summary>
        /// TirolPlaySystem.MainForm クラスの新しいインスタンスを初期化します。
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            // ログ出力設定
            txtLog.MaxLines = LOG_LINE_MAX;
            txtLog.Clear();
            log = new Logging();
            log.LoggingNotice += log_LoggingNotice;
            log.OutputDirectory=".";
            log.OutputFileName="log.txt";
            log.LoggingMode = Logging.LoggingModes.Day;
            log.Print("***** チロルチョコロボット大会システム 'キャプチャ' 起動 *****");

            // 設定読み込み
            try
            {
                xconf.LoadSetting();
            }
            catch
            {
            }

            // キャプチャページを選択
            Tab.SelectedIndex = 0;
            Tab_SelectedIndexChanged(Tab, new EventArgs());
        }

        #endregion

        #region 定数

        /// <summary>
        /// コンソールのログ表示行数です。
        /// </summary>
        private const int LOG_LINE_MAX = 1000;

        #endregion

        #region 変数

        #region 共通

        /// <summary>
        /// 動作ログ
        /// </summary>
        private Logging log = null;

        /// <summary>
        /// データ格納場所のルート
        /// </summary>
        private string datapath = "";

        #endregion

        #region ロボット情報

        /// <summary>
        /// ロボット名情報リスト
        /// </summary>
        private List<string> robots = new List<string>();

        #endregion

        #endregion

        #region プロパティ

        #endregion

        #region イベント

        #region 操作パネル切替 制御イベント

        #region Tab_SelectedIndexChangedイベント

        private void Tab_SelectedIndexChanged(object sender, EventArgs e)
        {
            // キャプチャパネル切り替え
            if (Tab.SelectedIndex == 0)
            {
                btnCamOn.IsPushed = true;
                btnCamOn.Push();
                cmbCam.Items.Clear();
                cmbCam.Items.AddRange(xcam.GetCameraNames());

                try
                {
                    // ロボット名読み込み
                    Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                    StreamReader reader = new StreamReader(this.xconf.GetConfigValue("General", "DataFolder") + "robotnamelist.txt", sjisEnc);
                    cmbPlayer.Items.Clear();
                    while (!reader.EndOfStream)
                    {
                        cmbPlayer.Items.Add(reader.ReadLine());
                    }
                    reader.Close();
                }
                catch
                {
                }

                datapath = this.xconf.GetConfigValue("General", "DataFolder") + @"PHOTO\";
            }
        }

        #endregion

        #endregion
 
        #region btnCamOn_Clickイベント

        private void btnCamOn_Click(object sender, EventArgs e)
        {
            if (btnCamOn.IsPushed)
            {
                // ON
                if (cmbCam.SelectedIndex >= 0)
                {
                    xcam.CameraName = cmbCam.Items[cmbCam.SelectedIndex].ToString();
                    xcam.CameraOn();
                }
                else
                {
                    btnCamOn.IsPushed = false;
                }
            }
            else
            {
                // OFF
                xcam.CameraOff();
            }
        }

        #endregion

        #region btnCamCapture_Clickイベント

        private void btnCamCapture_Click(object sender, EventArgs e)
        {
            xcam.CameraCapture();
            btnCamOn.Push();
        }

        #endregion

        #region btnCamLoad_Clickイベント

        private void btnCamLoad_Click(object sender, EventArgs e)
        {
            if (cmbPlayer.SelectedIndex < 0) return;
            try
            {
                xcam.LoadImage(datapath + cmbPlayer.SelectedItem + ".jpg");
            }
            catch
            {
            }
        }

        #endregion

        #region btnCamSave_Clickイベント

        private void btnCamSave_Click(object sender, EventArgs e)
        {
            if (cmbPlayer.SelectedIndex < 0) return;
            try
            {
                xcam.SaveImage(datapath + cmbPlayer.SelectedItem + ".jpg");
            }
            catch
            {
            }
        }

        #endregion

        #region btnCamClear_Clickイベント

        private void btnCamClear_Click(object sender, EventArgs e)
        {
            if (cmbPlayer.SelectedIndex < 0) return;
            try
            {
                xcam.DeleteImage(datapath + cmbPlayer.SelectedItem + ".jpg");
            }
            catch
            {
            }
        }

        #endregion

        #region システム設定ページイベント

        #region log_LoggingNoticeイベント

        void log_LoggingNotice(object sender, Logging.LoggingNoticeEventArgs e)
        {
            // コンソールにログを追加
            if (txtLog.Text.Length > 0) txtLog.PrintLine("");
            txtLog.Print(e.Message, txtLog.ForeColor);
        }

        #endregion

        #endregion

        #endregion
    }
}
