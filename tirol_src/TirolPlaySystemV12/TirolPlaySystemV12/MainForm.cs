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
        #region インナークラス

        #region GameInfoクラス

        /// <summary>
        /// ゲーム情報を扱います。
        /// </summary>
        public class GameInfo
        {
            #region プロパティ

            #region GameNameプロパティ

            private string gameName = "";
            /// <summary>
            /// 競技名を参照または設定します。
            /// </summary>
            public string GameName
            {
                get
                {
                    return gameName;
                }
                set
                {
                    gameName = value;
                }
            }

            #endregion

            #region Timeプロパティ

            private int time = -1;
            /// <summary>
            /// 制限時間(分)を参照または設定します。
            /// </summary>
            public int Time
            {
                get
                {
                    return time;
                }
                set
                {
                    time = value;
                }
            }

            #endregion

            #region UseHandiプロパティ
            
            private bool useHandi = false;
            /// <summary>
            /// ハンデを使用するかどうかを参照または設定します。
            /// </summary>
            public bool UseHandi
            {
                get
                {
                    return useHandi;
                }
                set
                {
                    useHandi = value;
                }
            }

            #endregion

            #region UseDistanceプロパティ
            
            private bool useDistance = false;
            /// <summary>
            /// 距離を使用するかどうかを参照または設定します。
            /// </summary>
            public bool UseDistance
            {
                get
                {
                    return useDistance;
                }
                set
                {
                    useDistance = value;
                }
            }

            #endregion

            #region UseTimeプロパティ
            
            private bool useTime = false;
            /// <summary>
            /// 時間を使用するかどうかを参照または設定します。
            /// </summary>
            public bool UseTime
            {
                get
                {
                    return useTime;
                }
                set
                {
                    useTime = value;
                }
            }

            #endregion

            #region UseGetプロパティ
            
            private bool useGet = false;
            /// <summary>
            /// 獲得を使用するかどうかを参照または設定します。
            /// </summary>
            public bool UseGet
            {
                get
                {
                    return useGet;
                }
                set
                {
                    useGet = value;
                }
            }

            #endregion

            #region Dataプロパティ

            private string[] data = null;
            /// <summary>
            /// 設定データ配列へのアクセスを提供します。
            /// </summary>
            public string[] Data
            {
                get
                {
                    return data;
                }
                set
                {
                    data = value;
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #region GameInfoCollectionクラス

        /// <summary>
        /// GameInfoクラスインスタンスのコレクションを扱います。
        /// </summary>
        public class GameInfoCollection : CollectionBase
        {
            #region メソッド

            #region this[]メソッド

            /// <summary>
            /// GameInfoコレクションへのアクセスを提供します。
            /// </summary>
            /// <param name="index">コレクション内のインデックス</param>
            /// <returns>項目の設定を保持するGameInfoのインスタンス</returns>
            public GameInfo this[int index]
            {
                get
                {
                    return ((GameInfo)List[index]);
                }
                set
                {
                    List[index] = value;
                }
            }

            #endregion

            #region Addメソッド

            /// <summary>
            /// GameInfoコレクションにGameInfoインスタンスを追加します。
            /// </summary>
            /// <param name="value">追加するGameInfoのインスタンス</param>
            /// <returns>追加された位置の0から始まるインデックス</returns>
            public int Add(GameInfo value)
            {
                int index = List.Add(value);
                return (index);
            }

            #endregion

            #region IndexOfメソッド

            /// <summary>
            /// GameInfoコレクション内のインスタンスのインデックスを取得します。
            /// </summary>
            /// <param name="value">検索するGameInfoのインスタンス</param>
            /// <returns>見つかった位置の0から始まるインデックス</returns>
            public int IndexOf(GameInfo value)
            {
                return (List.IndexOf(value));
            }

            #endregion

            #region Insertメソッド

            /// <summary>
            /// GameInfoコレクションにインスタンスを挿入します。
            /// </summary>
            /// <param name="index">挿入する位置の0から始まるインデックス</param>
            /// <param name="value">項目の設定を保持するGameInfoのインスタンス</param>
            public void Insert(int index, GameInfo value)
            {
                List.Insert(index, value);
            }

            #endregion

            #region Removeメソッド

            /// <summary>
            ///  GameInfoコレクションからインスタンスを削除します。
            /// </summary>
            /// <param name="value">削除するGameInfoのインスタンス</param>
            public void Remove(GameInfo value)
            {
                List.Remove(value);
            }

            #endregion

            #region Contains

            /// <summary>
            ///  GameInfoコレクションにインスタンスが存在しているかを返します。
            /// </summary>
            /// <param name="value">検索するGameInfoのインスタンス</param>
            /// <returns>存在していたらtrueを返す</returns>
            public bool Contains(GameInfo value)
            {
                return (List.Contains(value));
            }

            #endregion

            #endregion
        }

        #endregion

        #region PlayerInfoクラス

        /// <summary>
        /// プレイヤー情報を扱います。
        /// </summary>
        public class PlayerInfo
        {
            #region プロパティ

            #region PlayerNameプロパティ

            private string playerName = "";
            /// <summary>
            /// プレイヤー名を参照または設定します。
            /// </summary>
            public string PlayerName
            {
                get
                {
                    return playerName;
                }
                set
                {
                    playerName = value;
                }
            }

            #endregion

            #region PlayerYomiプロパティ

            private string playerYomi = "";
            /// <summary>
            /// プレイヤー名よみを参照または設定します。
            /// </summary>
            public string PlayerYomi
            {
                get
                {
                    return playerYomi;
                }
                set
                {
                    playerYomi = value;
                }
            }

            #endregion

            #region RobotNameプロパティ

            private string robotName = "";
            /// <summary>
            /// ロボット名を参照または設定します。
            /// </summary>
            public string RobotName
            {
                get
                {
                    return robotName;
                }
                set
                {
                    robotName = value;
                }
            }

            #endregion

            #region RobotYomiプロパティ

            private string robotYomi = "";
            /// <summary>
            /// ロボット名よみを参照または設定します。
            /// </summary>
            public string RobotYomi
            {
                get
                {
                    return robotYomi;
                }
                set
                {
                    robotYomi = value;
                }
            }

            #endregion

            #region IsTirolRobotプロパティ
            
            private bool isTirolRobot = true;
            /// <summary>
            /// 距離を使用するかどうかを参照または設定します。
            /// </summary>
            public bool IsTirolRobot
            {
                get
                {
                    return isTirolRobot;
                }
                set
                {
                    isTirolRobot = value;
                }
            }

            #endregion

            #region RobotTypeプロパティ

            /// <summary>
            /// ロボットの種類を参照します。
            /// </summary>
            public string RobotType
            {
                get
                {
                    if (isTirolRobot)
                    {
                        return "チロル";
                    }
                    else
                    {
                        return "Ｕ１Ｋ";
                    }
                }
            }

            #endregion

            #region RobotImageNameプロパティ

            /// <summary>
            /// ロボットのイメージファイル名を参照します。
            /// </summary>
            public string RobotImageName
            {
                get
                {
                    return "PHOTO\\" + robotName + ".jpg";
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #region PlayerInfoCollectionクラス

        /// <summary>
        /// PlayerInfoクラスインスタンスのコレクションを扱います。
        /// </summary>
        public class PlayerInfoCollection : CollectionBase
        {
            #region メソッド

            #region this[]メソッド

            /// <summary>
            /// PlayerInfoコレクションへのアクセスを提供します。
            /// </summary>
            /// <param name="index">コレクション内のインデックス</param>
            /// <returns>項目の設定を保持するPlayerInfoのインスタンス</returns>
            public PlayerInfo this[int index]
            {
                get
                {
                    return ((PlayerInfo)List[index]);
                }
                set
                {
                    List[index] = value;
                }
            }

            #endregion

            #region Addメソッド

            /// <summary>
            /// PlayerInfoコレクションにPlayerInfoインスタンスを追加します。
            /// </summary>
            /// <param name="value">追加するPlayerInfoのインスタンス</param>
            /// <returns>追加された位置の0から始まるインデックス</returns>
            public int Add(PlayerInfo value)
            {
                int index = List.Add(value);
                return (index);
            }

            #endregion

            #region IndexOfメソッド

            /// <summary>
            /// PlayerInfoコレクション内のインスタンスのインデックスを取得します。
            /// </summary>
            /// <param name="value">検索するPlayerInfoのインスタンス</param>
            /// <returns>見つかった位置の0から始まるインデックス</returns>
            public int IndexOf(PlayerInfo value)
            {
                return (List.IndexOf(value));
            }

            #endregion

            #region Insertメソッド

            /// <summary>
            /// PlayerInfoコレクションにインスタンスを挿入します。
            /// </summary>
            /// <param name="index">挿入する位置の0から始まるインデックス</param>
            /// <param name="value">項目の設定を保持するPlayerInfoのインスタンス</param>
            public void Insert(int index, PlayerInfo value)
            {
                List.Insert(index, value);
            }

            #endregion

            #region Removeメソッド

            /// <summary>
            ///  PlayerInfoコレクションからインスタンスを削除します。
            /// </summary>
            /// <param name="value">削除するPlayerInfoのインスタンス</param>
            public void Remove(PlayerInfo value)
            {
                List.Remove(value);
            }

            #endregion

            #region Contains

            /// <summary>
            ///  PlayerInfoコレクションにインスタンスが存在しているかを返します。
            /// </summary>
            /// <param name="value">検索するPlayerInfoのインスタンス</param>
            /// <returns>存在していたらtrueを返す</returns>
            public bool Contains(PlayerInfo value)
            {
                return (List.Contains(value));
            }

            #endregion

            #endregion
        }

        #endregion
        
        #endregion

        #region 構築・破棄

        /// <summary>
        /// TirolPlaySystem.MainForm クラスの新しいインスタンスを初期化します。
        /// </summary>
        public MainForm()
        {
            // ブラウザバージョン設定 - IE11
            int IEVer = 11000;
            XBrowser.SetIEVersion("TirolPlaySystem.exe", IEVer);
            XBrowser.SetIEVersion("TirolPlaySystem.vshost.exe", IEVer);
            
            InitializeComponent();

            // ログ出力設定
            txtLog.MaxLines = LOG_LINE_MAX;
            txtLog.Clear();
            log = new Logging();
            log.LoggingNotice += log_LoggingNotice;
            log.OutputDirectory=".";
            log.OutputFileName="log.txt";
            log.LoggingMode = Logging.LoggingModes.Day;
            log.Print("***** チロルチョコロボット大会システム 起動 *****");

            // 設定読み込み
            try
            {
                xconf.LoadSetting();
            }
            catch
            {
            }

            // Excel連携
            excel = new XControls.ExcelLink.ApplicationClass();
            excel.Visible = false;

            // ニコ生用キャプチャ画面
            nico = new NicoCapForm();

            // 会場モニター画面
            mon = new MonitorForm();
            
            // タイマー連携
            timRem.LinkageTimerView = new XControls.UI.TimerView[] { nico.Timer, mon.Timer, timRap };

            // 設定ページを選択
            Tab.SelectedIndex = 4;

            // 計測状態初期化
            btnMeaReset.Tag = "";

            // STARTメッセージ
            txtLog.PrintLine("");
            txtLog.Print("INFO> システム終了は[ALT+F4]です。", Color.Lime);
            txtLog.PrintLine("");
            txtLog.Print("INFO> [START]ボタンを押してください。", Color.Lime);
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

        /// <summary>
        /// 大会名
        /// </summary>
        private string contest = "";

        #endregion

        #region 画面設定

        /// <summary>
        /// ニコ生用キャプチャ画面
        /// </summary>
        private NicoCapForm nico = null;

        /// <summary>
        /// 会場モニター画面
        /// </summary>
        private MonitorForm mon = null;

        /// <summary>
        /// 会場モニタ―スクリーンのスクリーン番号
        /// </summary>
        private int monitorScreenNo = 0;

        #endregion

        #region Excel連携

        /// <summary>
        /// Excelアプリ
        /// </summary>
        private XControls.ExcelLink.ApplicationClass excel = null;

        /// <summary>
        /// 大会Excelブック
        /// </summary>
        private XControls.ExcelLink.WorkbookClass gamebook = null;

        /// <summary>
        /// シート名リスト
        /// </summary>
        private string[] sheets = null;

        #endregion

        #region ゲーム・プレイヤー情報

        /// <summary>
        /// ゲーム情報リスト
        /// </summary>
        private GameInfoCollection games = new GameInfoCollection();

        /// <summary>
        /// プレイヤー情報リスト
        /// </summary>
        private PlayerInfoCollection players = new PlayerInfoCollection();

        #endregion

        #region ゲーム計測

        /// <summary>
        /// ゲーム番号
        /// </summary>
        private int GameNo = 0;

        /// <summary>
        /// 挑戦回数
        /// </summary>
        private int TryNo = 0;

        /// <summary>
        /// 段・ハンディ数
        /// </summary>
        private int Handi = 0;

        /// <summary>
        /// 獲得数
        /// </summary>
        private int GetCount = 0;

        #endregion

        #endregion

        #region プロパティ

        #endregion

        #region イベント

        #region フォームイベント

        #region OnShownイベント

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // ニコ生用キャプチャの設定
            nico.Show(this);
            nico.Top = this.Top + pnlNico.Top;
            nico.Left = this.Left + pnlNico.Left;

            // 時計スタート
            xtimClock.Start();

            this.Focus();
        }

        #endregion

        #region OnClosingイベント

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            nico.FormClose();
        }

        #endregion

        #region OnKeyDownイベント

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (btnBrowsTune.IsPushed)
            {
                e.Handled = true;
            }
        }

        #endregion

        #endregion

        #region EXCEL PAGE SELECTOR 制御イベント

        #region btnPageShift_Clickイベント

        private void btnPageShift_Click(object sender, EventArgs e)
        {
            int i;
            int s;
            int n;
            XControls.UI.PushButton[] btn = new XControls.UI.PushButton[9];
            btn[0] = btnPage1;
            btn[1] = btnPage2;
            btn[2] = btnPage3;
            btn[3] = btnPage4;
            btn[4] = btnPage5;
            btn[5] = btnPage6;
            btn[6] = btnPage7;
            btn[7] = btnPage8;
            btn[8] = btnPage9;
            for (i = 0; i < 9; i++)
            {
                btn[i].Text = "";
                btn[i].Tag = "";
                btn[i].IsPushed = false;
            }
            if (btnPageShift.IsPushed)
            {
                s = 9;
            }
            else
            {
                s = 0;
            }
            for (i = 0; i < 9; i++)
            {
                n = s + i;
                if (n < sheets.Length)
                {
                    btn[i].Text = sheets[n];
                    btn[i].Tag = n;
                    btn[i].Enabled = true;
                }
                else
                {
                    btn[i].Enabled = false;
                }
            }
        }

        #endregion

        #region btnPage_Clickイベント

        private void btnPage_Click(object sender, EventArgs e)
        {
            XControls.UI.PushButton btn = (XControls.UI.PushButton)sender;
            if (btn.Tag == null) return;
            SetViewSheet((int)btn.Tag);
        }

        #endregion

        #region BGM SELECTOR 制御イベント

        #region btnS_Clickイベント

        private void btnS_Click(object sender, EventArgs e)
        {
            XControls.UI.PushButton btn = (XControls.UI.PushButton)sender;
            int n = int.Parse(btn.Name.Substring(btn.Name.Length - 1, 1));
            xsnd1.Stop();
            xsnd2.Stop();
            xsnd_se1.Stop();
            xsnd_se10.Stop();
            switch (n)
            {
                case 1:
                    xsnd1.Play();
                    break;
                case 2:
                    xsnd2.Play();
                    break;
                case 3:
                    xsnd_se1.Play();
                    break;
                case 4:
                    xsnd_se10.Play();
                    break;
            }
        }

        #endregion

        #region btnSSTOP_Clickイベント

        private void btnSSTOP_Click(object sender, EventArgs e)
        {
            xsnd1.Stop();
            xsnd2.Stop();
            xsnd3.Stop();
            xsnd4.Stop();
            xsnd5.Stop();
            xsnd6.Stop();
            xsnd7.Stop();
            xsnd8.Stop();
        }

        #endregion

        #endregion

        #endregion

        #region 操作パネル切替 制御イベント

        #region Tab_SelectedIndexChangedイベント

        private void Tab_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 会場モニタ起動
            if (Tab.SelectedIndex == 2)
            {
                // 大モニタ
                xscrViewM.Enabled = false;
                xscrViewM.ShowBackColor = true;
                xscrView.Enabled = true;
            }
            else
            {
                // 小モニタ
                xscrViewM.Enabled = true;
                xscrView.Enabled = false;
            }
        }

        #endregion

        #endregion

        #region NICONICO SELECTOR 制御イベント

        #region btnNicoBlue_Clickイベント

        private void btnNicoBlue_Click(object sender, EventArgs e)
        {
            nico.ViewMode = NicoCapForm.ViewModes.None;
        }

        #endregion

        #region btnNicoBar_Clickイベント

        private void btnNicoBar_Click(object sender, EventArgs e)
        {
            nico.ViewMode = NicoCapForm.ViewModes.ColorBar;
        }

        #endregion

        #region btnNicoInfo_Clickイベント

        private void btnNicoInfo_Click(object sender, EventArgs e)
        {
            nico.ViewMode = NicoCapForm.ViewModes.InfoBar;
        }

        #endregion

        #region btnNicoMon_Clickイベント

        private void btnNicoMon_Click(object sender, EventArgs e)
        {
            nico.ViewMode = NicoCapForm.ViewModes.Monitor;
        }

        #endregion

        #region txtTop_Enterイベント

        private void txtTop_Enter(object sender, EventArgs e)
        {
            try
            {
                txtTop.Items.Clear();
                txtTop.Items.AddRange(xconf.GetConfigValue("NicoMsg", "Top").Replace("\r", "").Split(new char[] { '\n' }));
            }
            catch
            {
            }
        }

        #endregion

        #region txtBottom_Enterイベント

        private void txtBottom_Enter(object sender, EventArgs e)
        {
            try
            {
                txtBottom.Items.Clear();
                txtBottom.Items.AddRange(xconf.GetConfigValue("NicoMsg", "Bottom").Replace("\r", "").Split(new char[] { '\n' }));
            }
            catch
            {
            }
        }

        #endregion

        #region txtTop_Validatedイベント

        private void txtTop_Validated(object sender, EventArgs e)
        {
            if (btnNicoText.IsPushed)
            {
                nico.TopMessage = txtTop.Text;
            }
            int i = txtTop.Items.IndexOf(txtTop.Text);
            if (i < 0) txtTop.Items.Add(txtTop.Text);
            try
            {
                StringBuilder sb = new StringBuilder();
                for (i = 0; i < txtTop.Items.Count; i++) if (txtTop.Items[i].ToString().Trim().Length > 0) sb.Append(txtTop.Items[i].ToString() + "\r\n");
                xconf.SetConfigValue("NicoMsg", "Top", sb.ToString());
            }
            catch
            {
            }
        }

        #endregion

        #region txtBottom_Validatedイベント

        private void txtBottom_Validated(object sender, EventArgs e)
        {
            if (btnNicoText.IsPushed)
            {
                nico.BottomMessage = txtBottom.Text;
            }
            int i = txtBottom.Items.IndexOf(txtBottom.Text);
            if (i < 0) txtBottom.Items.Add(txtBottom.Text);
            try
            {
                StringBuilder sb = new StringBuilder();
                for (i = 0; i < txtBottom.Items.Count; i++) if (txtBottom.Items[i].ToString().Trim().Length > 0) sb.Append(txtBottom.Items[i].ToString() + "\r\n");
                xconf.SetConfigValue("NicoMsg", "Bottom", sb.ToString());
            }
            catch
            {
            }
        }

        #endregion

        #region txtTop_KeyDownイベント

        private void txtTop_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter) txtTop_Validated(this, new EventArgs());
        }

        #endregion

        #region txtBottom_KeyDownイベント

        private void txtBottom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter) txtBottom_Validated(this, new EventArgs());
        }

        #endregion

        #region btnNicoText_Clickイベント

        private void btnNicoText_Click(object sender, EventArgs e)
        {
            if (btnNicoText.IsPushed)
            {
                nico.TopMessage = txtTop.Text;
                nico.BottomMessage = txtBottom.Text;
            }
            else
            {
                nico.TopMessage = "";
                nico.BottomMessage = "";
            }
        }
        
        #endregion

        #endregion

        #region MONITOR VIEW SELECTOR 制御イベント

        #region btnMonBlue_Clickイベント

        private void btnMonBlue_Click(object sender, EventArgs e)
        {
            mon.ViewMode = MonitorForm.ViewModes.None;
            mon.Visible = true;
            excel.Visible = false;
        }

        #endregion

        #region btnMonBar_Clickイベント

        private void btnMonBar_Click(object sender, EventArgs e)
        {
            mon.ViewMode = MonitorForm.ViewModes.ColorBar;
            mon.Visible = true;
            try
            {
                excel.Visible = false;
            }
            catch
            {
            }
        }

        #endregion

        #region btnMonInfo_Clickイベント

        private void btnMonInfo_Click(object sender, EventArgs e)
        {
            mon.ViewMode = MonitorForm.ViewModes.InfoView;
            mon.Visible = true;
            excel.Visible = false;
        }

        #endregion

        #region btnMonTimer_Clickイベント

        private void btnMonTimer_Click(object sender, EventArgs e)
        {
            mon.ViewMode = MonitorForm.ViewModes.Monitor;
            mon.Visible = true;
            excel.Visible = false;
        }

        #endregion

        #region btnMonExcel_Clickイベント

        private void btnMonExcel_Click(object sender, EventArgs e)
        {
            excel.Visible = false;
            if (monitorScreenNo < Screen.AllScreens.Length)
            {
                Rectangle b = Screen.AllScreens[monitorScreenNo].Bounds;
                SetBookPos(b.Top, b.Left);
            }
            excel.DisplayFullScreen = true;
            excel.Visible = true;
            mon.Visible = false;
        }

        #endregion

        #region btnEditExcel_Clickイベント

        private void btnEditExcel_Click(object sender, EventArgs e)
        {
            if (btnMonExcel.IsPushed) btnMonBlue.Push();
            SetBookPos(0, 0);
            excel.DisplayFullScreen = false;
            excel.Visible = true;
        }

        #endregion

        #endregion

        #region GAME SELECTOR 制御イベント

        #region btnGame_Clickイベント

        private void btnGame_Click(object sender, EventArgs e)
        {
            XControls.UI.PushButton btn = (XControls.UI.PushButton)sender;
            GameNo = int.Parse(btn.Name.Substring(btn.Name.Length - 1, 1)) - 1;
            SetGame();
        }

        #endregion

        #region btnTry_Clickイベント

        private void btnTry_Click(object sender, EventArgs e)
        {
            XControls.UI.PushButton btn = (XControls.UI.PushButton)sender;
            TryNo = int.Parse(btn.Name.Substring(btn.Name.Length - 1, 1));
            SetGame();
        }

        #endregion

        #endregion

        #region PLAYER SELECTOR 制御イベント

        #region cmbPlayer_SelectedIndexChangedイベント

        private void cmbPlayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            PlayerInfo p = players[cmbPlayer.SelectedIndex];
            txtPlayerInfo.Text = "よみ: " + p.PlayerYomi + "\r\n" +
                                 "ロボ: " + p.RobotName + "\r\n" +
                                 "よみ: " + p.RobotYomi + "\r\n" +
                                 "規格: " + p.RobotType;
            nico.PlayerInfo = p.PlayerName + "  ロボット名:" + p.RobotName;
            mon.PlayerName = p.PlayerName;
            mon.RobotName = p.RobotName;
            mon.Image.Clear();
            try
            {
                mon.Image.LoadImage(datapath + p.RobotImageName);
            }
            catch
            {
            }
            GetScore();
        }

        #endregion

        #endregion

        #region btnMeaReset_Clickイベント

        private GameInfo lastG = null;
        private PlayerInfo lastP = null;
        private void btnMeaReset_Click(object sender, EventArgs e)
        {
            if (lastG != null)
            {
                double dist = 0;
                double.TryParse(txtDist.Text, out dist);
                string ofs = lastG.Data[3];
                if (TryNo == 1) ofs = "";

                log.Print("[RE]" + lastG.GameName + lastG.GameName + " " + TryNo.ToString() + "|" + lastP.PlayerName + "|" + lastP.RobotName + "| [" +
                    Handi.ToString() + "段, " +
                    dist.ToString() + "cm, " +
                    timRem.ElapsedTime.ToString() + "秒, " +
                    GetCount.ToString() + "個]");
            }

            GameInfo g = games[GameNo];
            lastG = g;
            lastP = players[cmbPlayer.SelectedIndex];
            btnMeaReset.Tag = "RESET";
            xsnd3.Stop();
            xsnd4.Stop();
            xsnd5.Stop();
            txtSet.Enabled = true;

            // 時間
            float tz = 1.0F;
            //if (GameNo == 2 && players[cmbPlayer.SelectedIndex].RobotType.Equals("チロル")) tz = 1.3F;
            txtSet.Text = "";
            txtSet.Text = (ToTime(ToSecond(g.Time) * tz) * 100).ToString("0.00");
            timRem.Reset();

            // 段・ハンディ・獲得数
            if (g.UseHandi)
            {
                Handi = 1;
            }
            else
            {
                Handi = -1;
            }
            if (g.UseGet)
            {
                GetCount = 0;
            }
            else
            {
                GetCount = -1;
            }

            // フィールド
            field.GameNumber = GameNo + 1;
            field.Reset();
            field_ChangeValue(field, new EventArgs());
            nico.FieldShow = false;  // (GameNo != 1);

            timHandi.DefaultTime = Handi;
            mon.Handi.DefaultTime = Handi;
            timCount.DefaultTime = GetCount;
            mon.Counter.DefaultTime = GetCount;
            nico.Counter.DefaultTime = GetCount;

            if (g.UseDistance)
            {
                txtDist.Text = "200.0";
                txtDist.Enabled = true;
            }
            else
            {
                txtDist.Text = "---";
                txtDist.Enabled = false;
            }
        }

        private float ToSecond(int time)
        {
            return (int)time * 60;
        }

        private float ToTime(float sec)
        {
            sec /= 60;
            return (int)sec + (sec % 1) * 0.6F;
        }

        #endregion

        #region txtSet_TextChangedイベント

        private void txtSet_TextChanged(object sender, EventArgs e)
        {
            GameInfo g = games[GameNo];
            double t;
            if (double.TryParse(txtSet.Text, out t))
            {
                timRem.DefaultTime = t;
            }
        }

        #endregion

        #region btnMeaStart_Clickイベント

        private void btnMeaStart_Click(object sender, EventArgs e)
        {
            if (btnMeaReset.Tag.Equals("RESET"))
            {
                // リセット状態 → 開始
                txtSet.Enabled = false;
                bool f = (timRem.DefaultTime <= 30);
                xsnd_se1.Play();
                System.Threading.Thread.Sleep(500);
                timRem.Start();
                switch (GameNo)
                {
                    case 0:
                        xsnd3.Stop();
                        xsnd3.FasterRate = f;
                        xsnd3.Play();
                        break;
                    case 1:
                        xsnd4.Stop();
                        xsnd4.FasterRate = f;
                        xsnd4.Play();
                        break;
                    case 2:
                        xsnd5.Stop();
                        xsnd5.FasterRate = f;
                        xsnd5.Play();
                        break;
                }
                btnMeaReset.Tag = "START";
                return;
            }
        }

        #endregion

        #region btnMeaStop_Clickイベント

        private void btnMeaStop_Click(object sender, EventArgs e)
        {
            if (btnMeaReset.Tag.Equals("START") || btnMeaReset.Tag.Equals("PAUSE"))
            {
                // 開始 or 一時停止 → 停止
                timRem.Stop();
                xsnd_se10.Play();
                xsnd3.Stop();
                xsnd4.Stop();
                xsnd5.Stop();
                btnMeaReset.Tag = "STOP";
                return;
            }
        }

        #endregion

        #region btnMeaPause_Clickイベント

        private void btnMeaPause_Click(object sender, EventArgs e)
        {
            if (btnMeaReset.Tag.Equals("PAUSE"))
            {
                // 一時停止 → 開始
                txtSet.Enabled = false;
                timRem.Start();
                switch (GameNo)
                {
                    case 0:
                        xsnd3.Play();
                        break;
                    case 1:
                        xsnd4.Play();
                        break;
                    case 2:
                        xsnd5.Play();
                        break;
                }
                btnMeaReset.Tag = "START";
                return;
            }
            if (btnMeaReset.Tag.Equals("START"))
            {
                // 開始 → 一時停止
                timRem.Stop();
                xsnd3.Pause();
                xsnd4.Pause();
                xsnd5.Pause();
                btnMeaReset.Tag = "PAUSE";
                txtSet.Enabled = true;
                return;
            }
        }

        #endregion

        #region btnMeaHandiUp_Clickイベント

        private void btnMeaHandiUp_Click(object sender, EventArgs e)
        {
            if (Handi == -1) return;
            Handi++;
            timHandi.DefaultTime = Handi;
            mon.Handi.DefaultTime = Handi;
        }

        #endregion

        #region btnMeaHandiDown_Clickイベント

        private void btnMeaHandiDown_Click(object sender, EventArgs e)
        {
            if (Handi == -1) return;
            if (Handi == 0) return;
            Handi--;
            timHandi.DefaultTime = Handi;
            mon.Handi.DefaultTime = Handi;
        }

        #endregion

        #region btnMeaGetUp_Clickイベント

        private void btnMeaGetUp_Click(object sender, EventArgs e)
        {
            if (GetCount == -1) return;
            GetCount++;
            timCount.DefaultTime = GetCount;
            mon.Counter.DefaultTime = GetCount;
            nico.Counter.DefaultTime = GetCount;

        }

        #endregion

        #region btnMeaGetDown_Clickイベント

        private void btnMeaGetDown_Click(object sender, EventArgs e)
        {
            if (GetCount == -1) return;
            if (GetCount == 0) return;
            GetCount--;
            timCount.DefaultTime = GetCount;
            mon.Counter.DefaultTime = GetCount;
            nico.Counter.DefaultTime = GetCount;
        }

        #endregion

        #region field_ChangeValueイベント

        private void field_ChangeValue(object sender, EventArgs e)
        {
            nico.Field.WallSetting = field.WallSetting;
            mon.Field.WallSetting = field.WallSetting;
            nico.Field.GridText = field.GridText;
            mon.Field.GridText = field.GridText;
            nico.Field.GridOn = field.GridOn;
            mon.Field.GridOn = field.GridOn;
            if(GameNo == 0)
            {
             //   txtDist.Text = field.Distance.ToString();
            }
        }

        #endregion

        #region btnMeaWrite_Clickイベント

        private void btnMeaWrite_Click(object sender, EventArgs e)
        {
            double dist = 0;
            double.TryParse(txtDist.Text, out dist);
            GameInfo g = games[GameNo];
            string ofs = g.Data[3];
            if (TryNo == 1) ofs = "";

            PlayerInfo p = players[cmbPlayer.SelectedIndex];
            log.Print("[■]" + g.GameName + " " + TryNo.ToString() + "|" + p.PlayerName + "|" + p.RobotName + "| [" +
                Handi.ToString() + "段, " +
                dist.ToString() + "cm, " +
                timRem.ElapsedTime.ToString() + "秒, " +
                GetCount.ToString() + "個]");

            if (g.UseHandi) WriteMeasData(cmbPlayer.SelectedIndex, g.Data[4], ofs, Handi.ToString());
            if (g.UseDistance) WriteMeasData(cmbPlayer.SelectedIndex, g.Data[5], ofs, dist.ToString());
            if (g.UseTime) WriteMeasData(cmbPlayer.SelectedIndex, g.Data[6], ofs, timRem.ElapsedTime.ToString());
            if (g.UseGet) WriteMeasData(cmbPlayer.SelectedIndex, g.Data[7], ofs, GetCount.ToString());

            GetScore();
        }

        #endregion

        #region btnMeaClear_Clickイベント

        private void btnMeaClear_Click(object sender, EventArgs e)
        {
            GameInfo g = games[GameNo];
            string ofs = g.Data[3];
            if (TryNo == 1) ofs = "";

            PlayerInfo p = players[cmbPlayer.SelectedIndex];
            log.Print("[□]" + g.GameName + " " + TryNo.ToString() + "|" + p.PlayerName + "|" + p.RobotName);

            if (g.UseHandi) WriteMeasData(cmbPlayer.SelectedIndex, g.Data[4], ofs, "");
            if (g.UseDistance) WriteMeasData(cmbPlayer.SelectedIndex, g.Data[5], ofs, "");
            if (g.UseTime) WriteMeasData(cmbPlayer.SelectedIndex, g.Data[6], ofs, "");
            if (g.UseGet) WriteMeasData(cmbPlayer.SelectedIndex, g.Data[7], ofs, "");

            GetScore();
        }

        #endregion

        #region btnMeaSave_Clickイベント

        private void btnMeaSave_Click(object sender, EventArgs e)
        {
            gamebook.Save();
            log.Print("ブックを保存しました");
        }

        #endregion

        #region btnLogOpen_Clickイベント

        private void btnLogOpen_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(log.OutputDirectory);
        }

        #endregion

        #region timRem_TimerNoticeイベント

        private void timRem_TimerNotice(object sender, XControls.UI.TimerView.TimerNoticeEventArgs e)
        {
            if (btnMeaReset.Tag.Equals("START"))
            {
                switch ((int)e.RemainingTime)
                {
                    case 100:
                        xsnd_se2.Play();
                        break;
                    case 30:
                        xsnd_se3.Play();
                        switch (GameNo)
                        {
                            case 0:
                                xsnd3.FasterRate = true;
                                break;
                            case 1:
                                xsnd4.FasterRate = true;
                                break;
                            case 2:
                                xsnd5.FasterRate = true;
                                break;
                        }
                        break;
                    case 10:
                        xsnd_se4.Play();
                        break;
                    case 5:
                        xsnd_se5.Play();
                        break;
                    case 4:
                        xsnd_se6.Play();
                        break;
                    case 3:
                        xsnd_se7.Play();
                        break;
                    case 2:
                        xsnd_se8.Play();
                        break;
                    case 1:
                        xsnd_se9.Play();
                        break;
                    case 0:
                        xsnd_se10.Play();
                        xsnd3.Stop();
                        xsnd4.Stop();
                        xsnd5.Stop();
                        btnMeaReset.Tag = "STOP";
                        break;
                }
            }
        }

        #endregion

        #region CAMERA WEB 制御イベント

        #region TabOpe_SelectedIndexChangedイベント

        private void TabOpe_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnCamOn.IsPushed = true;
            btnCamOn.Push();
            cmbCam.Items.Clear();
            cmbCam.Items.AddRange(xcam.GetCameraNames());
        }

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
            PlayerInfo p = players[cmbPlayer.SelectedIndex];
            try
            {
                xcam.LoadImage(datapath + p.RobotImageName);
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
            PlayerInfo p = players[cmbPlayer.SelectedIndex];
            try
            {
                xcam.SaveImage(datapath + p.RobotImageName);
                mon.Image.LoadImage(datapath + p.RobotImageName);
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
            PlayerInfo p = players[cmbPlayer.SelectedIndex];
            try
            {
                xcam.DeleteImage(datapath + p.RobotImageName);
                mon.Image.Clear();
            }
            catch
            {
            }
        }

        #endregion

        #endregion

        #region BROWSER TURN 制御イベント

        #region btnBrowsDraw_Clickイベント

        private void btnBrowsDraw_Click(object sender, EventArgs e)
        {
            if (btnBrowsDraw.IsPushed)
            {
                mon.Controls.Remove(mon.Browser);
                pnlWeb.Controls.Add(mon.Browser);
                mon.Browser.Visible = true;
            }
            else
            {
                pnlWeb.Controls.Remove(mon.Browser);
                mon.Controls.Add(mon.Browser);
                mon.ViewMode = mon.ViewMode;
            }
        }

        #endregion

        #region btnBrowsTune_Clickイベント

        private void btnBrowsTune_Click(object sender, EventArgs e)
        {
            if (btnBrowsTune.IsPushed)
            {
                trcBraMask.Enabled = true;
                trcBraZoom.Enabled = true;
                pnlBraPos.Enabled = true;
            }
            else
            {
                trcBraMask.Enabled = false;
                trcBraZoom.Enabled = false;
                pnlBraPos.Enabled = false;
            }
        }
        
        #endregion

        #region btnBrowsSave_Clickイベント

        private void btnBrowsSave_Click(object sender, EventArgs e)
        {
            xconf.SetConfigValue("NicoNama", "URL", mon.Browser.CurrentURL.ToString());

            string w;
            w = mon.Browser.ViewNowPosition.Y.ToString() + "," + mon.Browser.ViewNowPosition.X.ToString() + "," + mon.Browser.ZoomNowRate.ToString() + "," + mon.MaskL.ToString() + "," + mon.MaskR.ToString();
            xconf.SetConfigValue("NicoNama", "NicoPosMon", w);
        }

        #endregion

        #region btnBrowsLoad_Clickイベント

        private void btnBrowsLoad_Click(object sender, EventArgs e)
        {
            LoadWeb();
        }

        #endregion

        #region trcBraZoom_MouseDownイベント

        private void trcBraZoom_MouseDown(object sender, MouseEventArgs e)
        {
            timBraMode = 1;
            timBraInt.Enabled = true;
        }

        #endregion

        #region trcBraZoom_MouseUpイベント

        private void trcBraZoom_MouseUp(object sender, MouseEventArgs e)
        {
            timBraInt.Enabled = false;
            timBraMode = 0;
            trcBraZoom.Value = 0;
            mon.Browser.Refresh();
        }

        #endregion

        #region trcBraMask_MouseDownイベント

        private void trcBraMask_MouseDown(object sender, MouseEventArgs e)
        {
            timBraMode = 2;
            timBraInt.Enabled = true;
        }

        #endregion

        #region trcBraMask_MouseUpイベント

        private void trcBraMask_MouseUp(object sender, MouseEventArgs e)
        {
            timBraInt.Enabled = false;
            timBraMode = 0;
            trcBraMask.Value = 0;
        }

        #endregion

        #region pnlBraPos_MouseDownイベント

        private void pnlBraPos_MouseDown(object sender, MouseEventArgs e)
        {
            timBraMode = 3;
            timBraInt.Enabled = true;
            pnlBraSX = e.X;
            pnlBraSY = e.Y;
            pnlBraCX = e.X;
            pnlBraCY = e.Y;
        }

        #endregion

        #region pnlBraPos_MouseMoveイベント

        private void pnlBraPos_MouseMove(object sender, MouseEventArgs e)
        {
            pnlBraCX = e.X;
            pnlBraCY = e.Y;
        }

        #endregion

        #region pnlBraPos_MouseUpイベント

        private void pnlBraPos_MouseUp(object sender, MouseEventArgs e)
        {
            timBraInt.Enabled = false;
            timBraMode = 0;
        }

        #endregion

        #region timBraInt_Tickイベント

        private int timBraMode = 0;
        private int pnlBraSX = 0;
        private int pnlBraSY = 0;
        private int pnlBraCX = 0;
        private int pnlBraCY = 0;
        private void timBraInt_Tick(object sender, EventArgs e)
        {
            switch(timBraMode)
            {
                case 1:
                    double z = mon.Browser.ZoomRate + trcBraZoom.Value / 10;
                    if (z < 30) z = 30;
                    if (z > 800) z = 800;
                    mon.Browser.ZoomRate = z;
                    break;
                case 2:
                    int m = mon.MaskL + trcBraMask.Value;
                    if (m < 0) m = 0;
                    if (m > 500) m = 500;
                    mon.MaskL = m;
                    mon.MaskR = m;
                    break;
                case 3:
                    int x = mon.Browser.ViewPosition.X + (pnlBraCX - pnlBraSX);
                    int y = mon.Browser.ViewPosition.Y + (pnlBraCY - pnlBraSY);
                    mon.Browser.ViewPosition = new Point(x,y);
                    break;
            }
        }

        #endregion

        #endregion

        #region システム設定ページイベント

        #region btnConfSave_Clickイベント

        private void btnConfSave_Click(object sender, EventArgs e)
        {
            try
            {
                log.Print("設定を保存します");
                xconf.SaveSetting();
            }
            catch (Exception es)
            {
                XControls.UI.MessageBox.Show(es.Message);
            }
        }

        #endregion

        #region btnConfLoad_Clickイベント

        private void btnConfLoad_Click(object sender, EventArgs e)
        {
            try
            {
                log.Print("設定を読み込みます");
                xconf.LoadSetting();

                // STARTメッセージ
                txtLog.PrintLine("");
                txtLog.Print("INFO> 設定を反映するため[START]ボタンを押してください。", Color.Lime);
            }
            catch (Exception es)
            {
                XControls.UI.MessageBox.Show(es.Message);
            }
        }

        #endregion

        #region btnSysStart_Clickイベント

        private void btnSysStart_Click(object sender, EventArgs e)
        {
            log.Print("システム起動します");
            SystemInitialize();
        }

        #endregion

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

        #region 内部処理

        #region システム初期化 (SystemInitialize)

        /// <summary>
        /// システム初期化
        /// </summary>
        private void SystemInitialize()
        {
            Application.DoEvents();
            Enabled = false;
            try
            {
                log.Print("システム初期化");

                #region 設定読み込み

                // データパス取得
                datapath = xconf.GetConfigValue("General", "DataFolder");
                log.Print("データパスルート : " + datapath);

                int.TryParse(xconf.GetConfigValue("General", "ScreenNo"), out monitorScreenNo);
                if (monitorScreenNo > Screen.AllScreens.Length - 1)
                {
                    string msg = "指定のスクリーンNo " + monitorScreenNo.ToString() + " に対応するスクリーンが存在しません";
                    log.Print(msg);
                    XControls.UI.MessageBox.Show(this,
                                    msg + "。設定を確認してください。",
                                    "",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    monitorScreenNo = Screen.AllScreens.Length - 1;
                }

                #endregion

                #region モニタスクリーン初期化

                // カラーバー
                Rectangle boun = Screen.AllScreens[monitorScreenNo].Bounds;
                mon.Left = boun.Left;
                mon.Top = boun.Top;
                mon.Show();
                xscrView.ScreenIndex = monitorScreenNo;
                xscrViewM.ScreenIndex = monitorScreenNo;
                nico.ScreenIndex = monitorScreenNo;

                btnNicoBar.Push();
                btnMonBar.Push();

                #endregion

                #region サウンド初期化

                // サウンド読み込み
                LoadSound(xsnd1, datapath);
                LoadSound(xsnd2, datapath);
                LoadSound(xsnd3, datapath);
                LoadSound(xsnd4, datapath);
                LoadSound(xsnd5, datapath);
                LoadSound(xsnd6, datapath);
                LoadSound(xsnd7, datapath);
                LoadSound(xsnd8, datapath);
                LoadSound(xsnd_se1, datapath);
                LoadSound(xsnd_se2, datapath);
                LoadSound(xsnd_se3, datapath);
                LoadSound(xsnd_se4, datapath);
                LoadSound(xsnd_se5, datapath);
                LoadSound(xsnd_se6, datapath);
                LoadSound(xsnd_se7, datapath);
                LoadSound(xsnd_se8, datapath);
                LoadSound(xsnd_se9, datapath);
                LoadSound(xsnd_se10, datapath);

                // BGMパネル
                btnS1.Text = "THEME";
                btnS2.Text = "BRIEFING";
                btnS3.Text = "START";
                btnS4.Text = "END";
                btnSSTOP.Push();

                #endregion

                #region Excel連携

                // 大会ブック読込
                if (!OpenBook())
                {
                    throw new ApplicationException("大会ブックオープンエラー");
                }
                if (!GetInfoData(out sheets, out contest, out games, out players))
                {
                    throw new ApplicationException("大会ブックデータ連携エラー");
                }

                // タイトルページ選択
                btnPageShift.IsPushed = true;
                btnPageShift.Push();
                btnPage1.IsPushed = true;
                btnPage1.Push();

                // モニタ上の大会名
                mon.ContestName = contest;

                #endregion

                #region コンソール初期化

                // ゲーム
                btnGame1.Push();
                btnTry1.Push();

                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                StreamWriter writer = new StreamWriter(this.xconf.GetConfigValue("General", "DataFolder") + "robotnamelist.txt", false, sjisEnc);

                // プレイヤー
                cmbPlayer.Items.Clear();
                for (int i = 0; i < players.Count; i++)
                {
                    cmbPlayer.Items.Add("[" + (i + 1).ToString("00") + "] " + players[i].PlayerName);
                    writer.WriteLine(players[i].RobotName);
                }
                cmbPlayer.SelectedIndex = 0;

                writer.Close();

                // 計測パネル
                btnMeaReset.Push();

                // ブラウザ初期化
                LoadWeb();

                // 操作タブ
                Tab.SelectedIndex = 0;
                TabOpe.SelectedIndex = 0;

                #endregion

                log.Print("起動しました");
            }
            catch(Exception es)
            {
                log.Print(es.ToString());
                log.Print("システム初期化に失敗しました");
                txtLog.PrintLine("");
                txtLog.Print("INFO> [ALT+F4]で終了してください。", Color.Lime);
            }
            Enabled = true;
        }

        #endregion

        #region システム停止 (SystemTerminate)

        /// <summary>
        /// システム停止
        /// </summary>
        private void SystemTerminate()
        {
            CloseBook();
            log.Print("システム終了");
            log.Print("***** チロルチョコロボット大会システム 終了 *****");
            log.LoggingNotice -= log_LoggingNotice;
        }

        #endregion

        #region サウンドを読み込む (LoadSound)

        /// <summary>
        /// サウンドを読み込む
        /// </summary>
        /// <remarks>
        /// ファイル名は読込先コントロールのTagに設定しておくこと。
        /// </remarks>
        /// <param name="ctrl">読込先コントロール</param>
        /// <param name="path">データパス</param>
        private void LoadSound(XSoundPlay ctrl, string path)
        {
            ctrl.FasterRate = false;
            ctrl.Volume = 100;
            ctrl.Loop = !ctrl.MiniPanel;
            string lp = "";
            if (ctrl.Loop) lp = "(loop) ";

            string fil = path + "\\SOUND\\" + ctrl.Tag;
            log.Print("サウンドデータ " + ctrl.Title + lp + ": LOADING... " + fil);
            ctrl.SoundFile = fil;
        }

        #endregion

        #region Excel連携

        #region 大会ブックを開く (OpenBook)

        /// <summary>
        /// 大会ブック取得 (OpenBook)
        /// </summary>
        /// <returns>結果(trie=成功, false=失敗)</returns>
        private bool OpenBook()
        {
            bool rt = false;
            if (gamebook != null)
            {
                CloseBook();
            }
            string book = xconf.GetConfigValue("General", "GameBook");
            string path = datapath + "\\" + book;
            try
            {
                // Excelファイルオープン
                log.Print("大会ブック読み込み: " + path);
                excel.Workbooks.OpenBook(path);
                gamebook = excel.Workbooks.GetBookInstance(excel.Workbooks.Count);
                rt = true;
            }
            catch
            {
                // 失敗
                log.Print("大会ブック連携失敗");
                XControls.UI.MessageBox.Show(this, "大会ブック(" + book + ")が読めません。設定を確認してください。",
                                                "",
                                                MessageBoxButtons.OK);
            }
            return rt;
        }

        #endregion

        #region 大会ブックを開放 (CloseBook)

        /// <summary>
        /// 大会ブックを開放します。
        /// </summary>
        private void CloseBook()
        {
            if (gamebook != null)
            {
                try
                {
                    // インスタンス開放
                    if (!gamebook.IsFailed)
                    {
                        // 保存してから閉じる
                        excel.DisplayAlerts = false;
                        if (!gamebook.Saved)
                        {
                            if (XControls.UI.MessageBox.Show(this, "大会ブックを保存しますか？",
                                                            "大会ブックが更新されています",
                                                            MessageBoxButtons.YesNo,
                                                            MessageBoxIcon.Question,
                                                            MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                            {
                                gamebook.Save();
                            }
                        }
                        gamebook.Close();
                        excel.DisplayAlerts = true;
                    }
                    excel.Workbooks.ReleaseBookInstance(ref gamebook);
                }
                catch
                {
                    // 失敗
                }
            }
        }

        #endregion

        #region 大会ブック情報取得 (GetInfoData)

        /// <summary>
        /// 大会ブックから情報取得を取得します。
        /// </summary>
        /// <param name="sheets">大会ブックシートリスト(エラー時はnull)</param>
        /// <param name="contest">大会名</param>
        /// <param name="games">ゲーム情報リスト</param>
        /// <param name="players">プレイヤー情報リスト</param>
        /// <returns>結果(trie=成功, false=失敗)</returns>
        private bool GetInfoData(out string[] sheets, out string contest, out GameInfoCollection games, out PlayerInfoCollection players)
        {
            bool rt = false;
            int i;
            int w;
            sheets = null;
            contest = "* ERROR *";
            games = new GameInfoCollection();
            players = new PlayerInfoCollection();
            try
            {
                object[] valparm = null;
                object[] refparm = new object[4];
                refparm[0] = "";
                refparm[1] = "";
                refparm[2] = "";
                refparm[3] = "";
                excel.Run(gamebook.Name, "Module2", "GetInfoData", valparm, ref refparm);
                string s = refparm[0].ToString();
                contest = refparm[1].ToString();
                string g = refparm[2].ToString();
                string p = refparm[3].ToString();

                // シート情報
                sheets = s.Split(new char[] { ',' });

                // ゲーム情報
                string[] gl = g.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                for (i = 0; i < gl.Length; i++)
                {
                    GameInfo gi = new GameInfo();
                    string[] gc = gl[i].Split(new char[] { ',' });
                    if (gc.Length >= 7)
                    {
                        gi.GameName = gc[0];
                        int.TryParse(gc[1], out w);
                        gi.Time = w;
                        gi.UseHandi = !gc[4].ToString().Equals("");
                        gi.UseDistance = !gc[5].ToString().Equals("");
                        gi.UseTime = !gc[6].ToString().Equals("");
                        gi.UseGet = !gc[7].ToString().Equals("");
                        gi.Data = gc;
                    }
                    games.Add(gi);
                }

                // プレイヤー情報
                string[] pl = p.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                for (i = 0; i < pl.Length; i++)
                {
                    PlayerInfo pi = new PlayerInfo();
                    string[] pc = pl[i].Split(new char[] { ',' });
                    if (pc.Length >= 6)
                    {
                        pi.PlayerName = pc[0];
                        pi.PlayerYomi = pc[1];
                        pi.RobotName = pc[2];
                        pi.RobotYomi = pc[3];
                        pi.IsTirolRobot = pc[4].ToString().Equals("チロル");
                    }
                    players.Add(pi);
                }
                rt = true;
            }
            catch (Exception es)
            {
                // 失敗
                log.Print("例外: " + es.Message);
                XControls.UI.MessageBox.Show(this,
                                "大会ブックにエラーがあります。",
                                "",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            return rt;
        }

        #endregion

        #region 大会ブックの表示位置設定 (SetBookPos)

        /// <summary>
        /// 大会ブックを指定スクリーンに移動します。
        /// </summary>
        /// <param name="top">表示座標Y</param>
        /// <param name="left">表示座標X</param>
        /// <returns>結果(trie=成功, false=失敗)</returns>
        private bool SetBookPos(int top, int left)
        {
            bool rt = false;
            try
            {
                excel.WindowState = FormWindowState.Normal;
                excel.Top = top;
                excel.Left = left;
                excel.WindowState = FormWindowState.Maximized;
                rt = true;
            }
            catch (Exception es)
            {
                // 失敗
                log.Print("例外: " + es.Message);
                XControls.UI.MessageBox.Show(this,
                                "大会ブックの画面移動に失敗しました。",
                                "",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            return rt;
        }

        #endregion

        #region 大会ブックのシート表示設定 (SetBookPos)

        /// <summary>
        /// 大会ブックの指定シートに切り替えます。
        /// </summary>
        /// <param name="no">0から始まるシートインデックス</param>
        /// <returns>結果(trie=成功, false=失敗)</returns>
        private bool SetViewSheet(int no)
        {
            bool rt = false;
            try
            {
                object[] valparm = new object[1];
                valparm[0] = no + 1;
                excel.Run(gamebook.Name, "Module2", "ViewSheet", valparm);
                rt = true;
            }
            catch (Exception es)
            {
                // 失敗
                log.Print("例外: " + es.Message);
                XControls.UI.MessageBox.Show(this,
                                "大会ブックにエラーがあります。",
                                "",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            return rt;
        }

        #endregion

        #region 大会ブックのシート表示設定 (ResultPos)

        /// <summary>
        /// 大会ブックのリザルトシートをスクロールします。
        /// </summary>
        /// <param name="column">カラム位置</param>
        /// <returns>結果(trie=成功, false=失敗)</returns>
        private bool ResultPos(string column)
        {
            bool rt = false;
            try
            {
                object[] valparm = new object[1];
                valparm[0] = column;
                excel.Run(gamebook.Name, "Module2", "ResultPos", valparm);
                rt = true;
            }
            catch (Exception es)
            {
                // 失敗
                log.Print("例外: " + es.Message);
                XControls.UI.MessageBox.Show(this,
                                "大会ブックにエラーがあります。",
                                "",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            return rt;
        }

        #endregion

        #region 大会ブックの順位を取得 (GetScore)

        /// <summary>
        /// 大会ブックの順位を取得します。
        /// </summary>
        /// <returns>結果(trie=成功, false=失敗)</returns>
        private bool GetScore()
        {
            bool rt = false;
            try
            {
                object[] valparm = new object[1];
                valparm[0] = (int)cmbPlayer.SelectedIndex;
                object[] refparm = new object[1];
                refparm[0] = "0";
                excel.Run(gamebook.Name, "Module2", "GetScore", valparm, ref refparm);
                txtScore.Text = refparm[0].ToString();
                rt = true;
            }
            catch (Exception es)
            {
                // 失敗
                log.Print("例外: " + es.Message);
                XControls.UI.MessageBox.Show(this,
                                "大会ブックにエラーがあります。",
                                "",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            return rt;
        }

        #endregion

        #region 大会ブックへの書き込み (WriteMeasData)

        /// <summary>
        /// 大会ブックに計測結果を書き込みます。
        /// </summary>
        /// <param name="playeridx">0から始まるプレイヤーIndex</param>
        /// <param name="column">カラム位置</param>
        /// <param name="offset">カラムオフセット</param>
        /// <param name="value">値</param>
        /// <returns>結果(trie=成功, false=失敗)</returns>
        private bool WriteMeasData(int playeridx, string column, string offset, string value)
        {
            bool rt = false;
            try
            {
                object[] valparm = new object[4];
                valparm[0] = playeridx;
                valparm[1] = column;
                valparm[2] = offset;
                valparm[3] = value;
                excel.Run(gamebook.Name, "Module2", "WriteData", valparm);
                rt = true;
            }
            catch (Exception es)
            {
                // 失敗
                log.Print("例外: " + es.Message);
                XControls.UI.MessageBox.Show(this,
                                "大会ブックにエラーがあります。",
                                "",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            return rt;
        }

        #endregion

        #endregion

        #region ウェブページを読み込む (LoadWeb)

        /// <summary>
        /// ウェブページを読み込む
        /// </summary>
        private void LoadWeb()
        {
            string url = xconf.GetConfigValue("NicoNama", "URL");
            string[] posmon = xconf.GetConfigValue("NicoNama", "NicoPosMon").Split(new char[] { ',' });
            int x, y, z, l, r;

            mon.Browser.StartURL = url;
            if (posmon.Length == 5)
            {
                x = 0;
                y = 0;
                z = 0;
                l = 0;
                r = 0;
                int.TryParse(posmon[0], out y);
                int.TryParse(posmon[1], out x);
                int.TryParse(posmon[2], out z);
                int.TryParse(posmon[3], out l);
                int.TryParse(posmon[4], out r);
                mon.Browser.ViewPosition = new Point(x, y);
                mon.Browser.ZoomRate = z;
                mon.MaskL = l;
                mon.MaskR = r;

            }
            mon.Browser.Run();
            Application.DoEvents();
            System.Threading.Thread.Sleep(3000);
            mon.Browser.Run();
        }

        #endregion

        #region ゲーム制御

        #region ゲーム設定 (SetGame)

        private void SetGame()
        {
            try
            {
                mon.GameName = games[GameNo].GameName + "  " + TryNo.ToString() + "回目";
                txtTop.Text = mon.GameName;
                ResultPos(games[GameNo].Data[2]);
            }
            catch
            {
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
