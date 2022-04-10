using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XControls.IO;
using XControls.UI;

namespace XControls
{
    /// <summary>
    /// アプリケーション間のハブ機能を提供するクラスです。
    /// 
    /// ◇使い方◇
    /// (1) PC内での多重起動を禁止するプログラムでは、Program.cs内で次の様にしてください。
    ///     if (XControls.SystemHub.ProgramStartCheck())
    ///     {
    ///         // すでに実行中
    ///         XControls.SystemHub.ProgramActivate();
    ///     }
    ///     else
    ///     {
    ///         // 起動
    ///         Application.Run(new Form1());
    ///     }
    /// (2) 各プログラムでは、次の様にインスタンスを取得して利用を開始してください。
    ///     SystemHub hub = SystemHub.GetInstance();
    ///     また、利用終了時には必ずインスタンスへの参照を開放してください。
    ///     SystemHub.ReleaseInstance(hub);
    /// (3) hub.SendObjectメソッドでメッセージ交換を可能にするため、
    ///     次の様にイベントハンドラを設定してください。
    ///     SystemHub.ReciveObjectEvent += new SystemHub.ReciveObjectEventHandler(ReciveObject);
    ///     SystemHub.ReciveObjectEvent -= new SystemHub.ReciveObjectEventHandler(ReciveObject);
    ///     public static void ReciveObject(object sender, ReciveObjectEventArgs e) {}

    

    ///     ※このイベントは別スレッドから呼ばれます
    ///     ※このイベント内でSendSysError/SendSysMessageは利用できません。代わりにe.SysError/e.SysMessageの
    ///       どちらかにメッセージをセットしてください。
    /// (3) bandbs.StartFolderCheckメソッドで受注メールフォルダを監視します。
    ///     次の様にイベントハンドラを設定してください。
    ///     bandbs.MailFolderUpdateEvent += new BanDBShared.MailFolderUpdateEventHandler(MailFolderUpdate_Notice);
    ///     bandbs.MailFolderUpdateEvent -= new BanDBShared.MailFolderUpdateEventHandler(MailFolderUpdate_Notice);
    ///     public void MailFolderUpdate_Notice(object sender, FileSystemEventArgs e) {}
    ///     ※このイベントは別スレッドから呼ばれます
    /// </summary>
    public sealed class SystemHub
    {
        #region 構築・破棄

        /// <summary>
        /// XControls.SystemHub クラスの新しいインスタンスを初期化します。
        /// </summary>
        private SystemHub()
        {
            InitializeHub();
        }

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        private void Dispose(bool disposing)
        {
            TerminateHub();
        }

        #endregion

        #region API定義

        [DllImport("USER32.DLL")]
        private static extern int ShowWindow(System.IntPtr hWnd, int nCmdShow);
        private const int SW_NORMAL = 1;

        [DllImport("USER32.DLL")]
        private static extern bool SetForegroundWindow(System.IntPtr hWnd);

        public enum AnimateWindowFlags : int
        {
            AW_HOR_POSITIVE = 0x0000001,
            AW_HOR_NEGATIVE = 0x0000002,
            AW_VER_POSITIVE = 0x0000003,
            AW_VER_NEGATIVE = 0x0000008,
            AW_CENTER       = 0x0000010,
            AW_HIDE         = 0x0010000,
            AW_ACTIVATE     = 0x0020000,
            AW_SLIDE        = 0x0040000,
            AW_BLEND        = 0x0080000
        }

        [DllImport("USER32.DLL")]
        private static extern bool AnimateWindow(System.IntPtr hWnd, int time, AnimateWindowFlags flags);

        #endregion

        #region 定数

        /// <summary>
        /// SystemHub用共有メモリサイズです。
        /// </summary>
        private const int SMHB_MEM_SIZE = 1024 - 7;

        /// <summary>
        /// 設定用共有メモリサイズです。
        /// </summary>
        private const int SMCF_MEM_SIZE = 1024 - 7;

        /// <summary>
        /// アプリ用共有メモリサイズです。
        /// </summary>
        private const int SMAP_MEM_SIZE = 4096 - 7;

        #endregion

        #region 変数

        /// <summary>
        /// 唯一のインスタンスです。
        /// </summary>
        private static SystemHub instance = new SystemHub();

        /// <summary>
        /// 設定グリッドコントロールのインスタンスです。
        /// </summary>
        private ConfigGrid xconf = new ConfigGrid();

        /// <summary>
        /// ログ出力クラスのインスタンスです。
        /// </summary>
        private Logging xlog = new Logging();

        ///// <summary>
        ///// 設定用共有メモリ管理クラスのインスタンスです。
        ///// </summary>
        //private SharedMemory xsmCf = new SharedMemory();

        ///// <summary>
        ///// SystemHub用共有メモリ管理クラスのインスタンスです。
        ///// </summary>
        //private SharedMemory xsmHb = new SharedMemory();

        ///// <summary>
        ///// アプリ用共有メモリ管理クラスのインスタンスです。
        ///// </summary>
        //private SharedMemory xsmAp = new SharedMemory();

        ///// <summary>
        ///// SystemHub用リングバッファクラスのインスタンスです。
        ///// </summary>
        //private RingBuffer xrbuf = null;

        ///// <summary>
        ///// アプリ用リングバッファクラスのインスタンスです。
        ///// </summary>
        //private RingBuffer xrbAp = null;

        ///// <summary>
        ///// UDP通信用クラスのインスタンスです。
        ///// </summary>
        //private UdpPort udp = new UdpPort(); 

        // 自メッセージ識別用のインスタンスIDです。
        private static byte instanceId = 0;

        /// <summary>
        /// GetInstance呼び出し状態フラグです。
        /// </summary>
        private static bool isGetInstance = false;

        #endregion

        #region プロパティ

        #region ConsleBoxプロパティ

        private UI.Console xcons = null;
        /// <summary>
        /// ログ出力を表示する Console のインスタンスを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            Description("ログ出力を表示する Console のインスタンスを参照または設定します。")
        ]
        public static UI.Console Console
        {
            get
            {
                return instance.xcons;
            }
            set
            {
                instance.xcons = value;
            }
        }

        #endregion

        #region ErrorDialogEnabledプロパティ

        private static bool errorDialogEnabled = true;
        /// <summary>
        /// エラー時にダイアログ表示を行うかを指定します。エラーになるたびにfalseに設定されます。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(true),
            Description("エラー時にダイアログ表示を行うかを指定します。エラーになるたびにfalseに設定されます。")
        ]
        public bool ErrorDialogEnabled
        {
            get
            {
                return errorDialogEnabled;
            }
            set
            {
                errorDialogEnabled = value;
            }
        }

        #endregion





        #region Textプロパティ

        private string text = "SystemHub";
        /// <summary>
        /// SystemHubの名前を参照または設定します。
        /// </summary>
        [
            Category("表示"),
            Description("SystemHubの名前を参照または設定します。")
        ]
        public static string Text
        {
            get
            {
                return instance.text;
            }
            set
            {
                instance.text = value;
            }
        }

        #endregion
        
        #endregion

        #region イベント

        #region xlog_LoggingNoticeイベント

        private void xlog_LoggingNotice(object sender, Logging.LoggingNoticeEventArgs e)
        {
            if (xcons != null) xcons.PrintLine(e.DateTimeProcess.Substring(8) + "| " + e.Message);
        }

        #endregion

        #endregion

        #region メソッド

        // ◇ Staticメソッド

        #region 自プログラムインスタンス起動チェック (ProgramStartCheck) (static)

        /// <summary>
        /// 自プログラムのインスタンスが起動しているかをチェックします。
        /// </summary>
        /// <returns>結果(true=起動している, false=起動していない)</returns>
        public static bool ProgramStartCheck()
        {
            bool rt = false;

            return rt;
        }
                
        #endregion

        #region 起動済み自プログラムインスタンスをアクティブ化 (ProgramActivate) (static)

        /// <summary>
        /// すでに起動している自プログラムのインスタンスをアクティブにします。
        /// </summary>
        public static void ProgramActivate()
        {

        }
        
        #endregion

        //#region インスタンスへの参照を取得 (GetInstance) (static)

        ///// <summary>
        ///// SystemHubのインスタンスへの参照を取得します。終了時には必ずReleaseInstance()を実行してください。
        ///// </summary>
        ///// <returns>SystemHubインスタンス</returns>
        //public static SystemHub GetInstance()
        //{
        //    if (isGetInstance) return instance;

        //    isGetInstance = true;

        //    // SystemHub用共有メモリを開く
        //    try
        //    {
        //        //instance.xsmHb.SharedName = "SYSTEMHUB";
        //        //instance.xsmHb.MemorySize = SMHB_MEM_SIZE;
        //        //instance.xsmHb.Open();
        //        //instance.xrbuf = new RingBuffer(instance.xsmHb);
        //    }
        //    catch (Exception es)
        //    {
        //        if (errorDialogEnabled)
        //        {
        //            MessageBox.Show("原因: " + es.Message, Text + "インスタンス取得に失敗しました", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            errorDialogEnabled = false;
        //        }
        //        throw es;
        //    }
        //    byte loadCount = instance.xsmHb.GetLoadCount();
        //    instanceId = loadCount;

        //    // インスタンス初期化処理
        //    string apppath = Assembly.GetEntryAssembly().Location;
        //    string exename = apppath.Substring(apppath.LastIndexOf("\\") + 1);
        //    apppath = apppath.Substring(0, apppath.LastIndexOf("\\"));
            
        //    instance.xconf.PublicKeyText = "SYSKYHUB";
        //    instance.InitializeConfig();

        //    // ログ及び設定ファイルのパス設定 - ※異なる配置パスの同一プログラムの混在は考慮しない
        //    if (apppath.IndexOf("Program Files") > 0)
        //    {
        //        // 本番 - セットアップ用
        //        apppath = apppath.Replace("Program Files", "ProgramData");
        //        instance.xlog.OutputDirectory = apppath + "\\Log";
        //        instance.xconf.Directory = apppath;
        //    }
        //    else
        //    {
        //        if (apppath.IndexOf("Debug") > 0 || apppath.IndexOf("Release") > 0)
        //        {
        //            // テスト - 開発環境
        //            instance.xlog.OutputDirectory = apppath + "\\Log";
        //            apppath = apppath.Substring(0, apppath.LastIndexOf("\\"));
        //            instance.xconf.Directory = apppath;
        //        }
        //        else
        //        {
        //            // 本番 - 任意フォルダ
        //            instance.xlog.OutputDirectory = apppath + "\\Log";
        //            instance.xconf.Directory = apppath;
        //        }
        //    }
        //    instance.xlog.OutputFileName = "syslog#[@].txt";
        //    instance.xlog.LoggingMode = XControls.Logging.LoggingModes.Week;

        //    // 設定用共有メモリを開く
        //    try
        //    {
        //        instance.xsmCf.SharedName = "SYSTEMCNF";
        //        instance.xsmCf.MemorySize = SMCF_MEM_SIZE;
        //        instance.xsmCf.Open();
        //    }
        //    catch (Exception es)
        //    {
        //        if (errorDialogEnabled)
        //        {
        //            MessageBox.Show("原因: " + es.Message, Text + "インスタンス取得に失敗しました", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            errorDialogEnabled = false;
        //        }
        //        throw es;
        //    }

        //    if (loadCount == 1)
        //    {
        //        // システム起動メッセージ出力
        //        LogPrint("======== " + Text + " 起動 ========");

        //        // 設定ファイル読み込み
        //        try
        //        {
        //            instance.xconf.LoadSetting();
        //            LogPrint("現在のシステム設定値を読み込みました。");
        //        }
        //        catch (Exception es)
        //        {
        //            LogPrint("既定のシステム設定値で起動しました: " + es.Message);
        //        }

        //        // 設定を同期
        //        instance.PutConfig();

        //        // SystemHubメッセージを同期
        //        instance.xrbuf.SaveSMem();
        //    }
        //    else
        //    {
        //        // アプリ用共有メモリを開く
        //        try
        //        {
        //            instance.xsmAp.SharedName = "APP_" + exename.Substring(0, exename.LastIndexOf(".")).ToUpper();
        //            instance.xsmAp.MemorySize = SMAP_MEM_SIZE;
        //            instance.xsmAp.Open();
        //            instance.xrbAp = new RingBuffer(instance.xsmAp);
        //        }
        //        catch (Exception es)
        //        {
        //            if (errorDialogEnabled)
        //            {
        //                MessageBox.Show("原因: " + es.Message, Text + "インスタンス取得に失敗しました", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                errorDialogEnabled = false;
        //            }
        //            throw es;
        //        }

        //        // 設定を同期
        //        instance.GetConfig();

        //        // SystemHubメッセージを同期
        //        instance.xrbuf.LoadSMem();

        //        // アプリメッセージを同期
        //        instance.xrbAp.SaveSMem();
        //    }

        //    // アプリ開始を記録
        //    LogPrint(exename + " を開始しました。");
        //    LogPrint("LoadCount+[" + instanceId.ToString() + "]");

        //    return instance;
        //}

        //#endregion

        //#region インスタンスへの参照を解放 (ReleaseInstance) (static)

        ///// <summary>
        ///// SystemHubのインスタンスへの参照を解放します。
        ///// </summary>
        ///// <param name="instance">解放するインスタンス</param>
        //public static void ReleaseInstance(SystemHub instance)
        //{
        //    if (!isGetInstance) return;

        //    // 少し待つ
        //    System.Threading.Thread.Sleep(10);

        //    LogPrint(Text + "の後処理を開始しました。");

        //    // メッセージを同期
        //    instance.xrbuf.SaveSMem();

        //    // アプリ終了を記録
        //    string apppath = Assembly.GetEntryAssembly().Location;
        //    string exename = apppath.Substring(apppath.LastIndexOf("\\") + 1);
        //    LogPrint(exename + " を終了しました。");

        //    // 共有メモリを閉じる
        //    byte loadCount = instance.xsmHb.GetLoadCount();
        //    LogPrint("LoadCount-[" + (loadCount - 1).ToString() + "]");
        //    try
        //    {
        //        try
        //        {
        //            instance.xsmHb.Close();
        //        }
        //        catch
        //        {
        //        }
        //    }
        //    catch (Exception es)
        //    {
        //        LogPrint(Text + "共有メモリエラー: " + es.Message);
        //    }
        //    try
        //    {
        //        try
        //        {
        //            instance.xsmCf.Close();
        //        }
        //        catch
        //        {
        //        }
        //    }
        //    catch (Exception es)
        //    {
        //        LogPrint("設定共有メモリエラー: " + es.Message);
        //    }
        //    try
        //    {
        //        try
        //        {
        //            if (instance.xsmAp != null) instance.xsmAp.Close();
        //            instance.xsmAp = null;
        //        }
        //        catch
        //        {
        //        }
        //    }
        //    catch (Exception es)
        //    {
        //        LogPrint("アプリ共有メモリエラー: " + es.Message);
        //    }

        //    // インスタンス終了処理
        //    if (instanceId == 1)
        //    {
        //        // システム終了メッセージ出力
        //        LogPrint("======== " + Text + " 終了 ========");
        //        LogPrint("");
        //        LogPrint("");
        //    }
        //    instanceId = 0;
        //    isGetInstance = false;
        //}

        //#endregion

        #region ログメッセージ出力 (LogPrint) (static)

        /// <summary>
        /// ログにメッセージを出力します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public static void LogPrint(string message)
        {
            instance.xlog.Print(message);
        }

        #endregion

        #region フォームアニメーション表示 (AmnimateWindow) (static)

        /// <summary>
        /// フォームをアニメーション表示します。
        /// </summary>
        /// <param name="form">フォーム</param>
        /// <param name="time">時間</param>
        /// <param name="flags">効果の組み合わせフラグ</param>
        public static void AnimateWindow(Form form, int time, AnimateWindowFlags flags)
        {
            AnimateWindow(form.Handle, time, flags);
        }
        
        #endregion

        // ◇ システム情報取得メソッド

        //#region ホスト名取得 (GetHostName)

        ///// <summary>
        ///// 自ホスト名を取得します。
        ///// </summary>
        ///// <returns>ホスト名(失敗時は"")</returns>
        //public string GetHostName()
        //{
        //    return udp.GetHostName();
        //}

        ///// <summary>
        ///// 自ホスト名を取得します。
        ///// </summary>
        ///// <param name="ipAddr">調べるIPアドレス</param>
        ///// <returns>ホスト名(失敗時は"")</returns>
        //public string GetHostName(IPAddress ipAddr)
        //{
        //    return udp.GetHostName(ipAddr);
        //}

        //#endregion

        //#region IPアドレス一覧取得 (GetLocalIPAddressList)

        ///// <summary>
        ///// このホストのIPアドレスの一覧を取得します。
        ///// </summary>
        ///// <returns>IPアドレスのリスト(失敗時はnull)</returns>
        //public IPAddress[] GetLocalIPAddressList()
        //{
        //    return udp.GetLocalIPAddressList();
        //}

        //#endregion

        //#region サブネットマスク取得 (GetLocalSubnetMask)

        ///// <summary>
        ///// 指定したIPアドレスのサブネットマスクを取得します。
        ///// </summary>
        ///// <param name="localIpAddress">IPアドレス</param>
        ///// <returns>サブネットマスク(失敗時はnull)</returns>
        //public IPAddress GetLocalSubnetMask(IPAddress localIpAddress)
        //{
        //    return udp.GetLocalSubnetMask(localIpAddress);
        //}

        //#endregion

        //#region ブロードキャストアドレス取得 (GetBroadcastAddress)

        ///// <summary>
        ///// 指定したIPアドレスのブロードキャストアドレスを取得します。
        ///// </summary>
        ///// <param name="localIpAddress">IPアドレス</param>
        ///// <returns>ブロードキャストアドレス(失敗時はnull)</returns>
        //public IPAddress GetBroadcastAddress(IPAddress localIpAddress)
        //{
        //    return udp.GetBroadcastAddress(localIpAddress);
        //}

        //#endregion

        //// ◇ ネットワーク処理メソッド

        //#region PINGテスト (PingTest)

        ///// <summary>
        ///// 指定したホストに対してPINGを行い、応答があったかどうかを返します。
        ///// </summary>
        ///// <param name="host">ホスト名またはIPアドレス</param>
        ///// <returns>結果(成功=true, 失敗=false</returns>
        //public bool PingTest(string host)
        //{
        //    return udp.IsPingReply(host, 5000);
        //}

        //#endregion


        #endregion

        #region 内部処理

        #region 未処理例外検出用公開関数(static) ※外部から使用しないでください

        public static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ShowErrorMessage(e.Exception, "Application_ThreadExceptionによる例外通知です。");
        }
        public static void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                ShowErrorMessage(ex, "Application_UnhandledExceptionによる例外通知です。");
            }
        }
        public static void ShowErrorMessage(Exception ex, string extraMessage)
        {
            // ログ出力
            LogPrint(extraMessage);
            LogPrint(ex.Message + "\r\nスタックトレース==>\r\n" + ex.StackTrace);

            // ダイアログで通知
            ThreadExceptionDialog ed = new ThreadExceptionDialog(ex);
            ed.Text = extraMessage;
            ed.ShowDialog();

            // この後、アプリケーションエラーで終了
            Environment.Exit(0);
        }

        #endregion

        #region Hub初期化 (InitializeHub)

        /// <summary>
        /// SystemHubの初期化を行います。
        /// </summary>
        private void InitializeHub()
        {
            // ログ初期化
            xlog.LoggingNotice += xlog_LoggingNotice;
            xlog.OutputDirectory = ".\\Log\\";
            xlog.OutputFileName = "log[@].txt";
            xlog.LoggingMode = Logging.LoggingModes.Size;
            xlog.MaxFileSize = 2000;

            // UDP通信初期化
            //udp.Logging = xlog;

        }

        #endregion

        #region Hub後処理 (TerminateHub)

        /// <summary>
        /// SystemHubの後処理を行います。
        /// </summary>
        private void TerminateHub()
        {
            xlog.LoggingNotice -= xlog_LoggingNotice;
        }
        
        #endregion

        #region 設定ファイル項目初期化 (InitializeConfig)

        /// <summary>
        /// 設定ファイル項目の初期化を行います。
        /// </summary>
        private void InitializeConfig()
        {

        
        
        
        }

        #endregion

        #region 共有メモリから設定内容を読む (GetConfig)

        /// <summary>
        /// 共有メモリから設定内容を読み込みます。
        /// </summary>
        public void GetConfig()
        {
            //try
            //{
            //    instance.xconf.SetDataAll(instance.xsmCf.GetObject(0) as ArrayList);
            //}
            //catch (Exception es)
            //{
            //    LogPrint("共有メモリ(CF)読み込みエラー: " + es.Message);
            //    if (errorDialogEnabled)
            //    {
            //        MessageBox.Show("共有メモリ(CF)に異常があります。\r\n" + 
            //                        "処理は続行しますが、速やかに作業を終了してください。\r\n\r\n" +
            //                        es.Message,
            //                        Assembly.GetEntryAssembly().FullName,
            //                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        errorDialogEnabled = false;
            //    }
            //}
        }

        #endregion

        #region 共有メモリに設定内容を書き込む (PutConfig)

        /// <summary>
        /// 共有メモリに設定内容を書き込みます。
        /// </summary>
        public void PutConfig()
        {
            //try
            //{
            //    instance.xsmCf.PutObject(instance.xconf.GetDataAll(), 0);
            //}
            //catch (Exception es)
            //{
            //    LogPrint("共有メモリ(CF)書き込みエラー: " + es.Message);
            //    if (errorDialogEnabled)
            //    {
            //        MessageBox.Show("共有メモリ(CF)に異常があります。\r\n" +
            //                        "処理は続行しますが、速やかに作業を終了してください。\r\n\r\n" +
            //                        es.Message,
            //                        Assembly.GetEntryAssembly().FullName,
            //                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        errorDialogEnabled = false;
            //    }
            //}
        }

        #endregion




        #endregion
    }
}
