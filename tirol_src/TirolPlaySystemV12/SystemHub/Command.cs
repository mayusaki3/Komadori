using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace XControls
{
    /// <summary>
    /// コマンド操作用クラスです。
    /// </summary>
    [DebuggerNonUserCode]
    public class Command
    {
        #region 構築・破棄

        ~Command()
        {
            CloseConsole(true);
        }
        
        #endregion

        #region Win32API

        [DllImport("Kernel32.dll")]
        private static extern bool AllocConsole();
        
        [DllImport("Kernel32.dll")]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow); 
        
        #endregion
        
        #region 変数

        /// <summary>
        /// 標準出力モニタ―スレッドです。
        /// </summary>
        Thread stdOutMon = null;

        /// <summary>
        /// 標準エラー出力モニタ―スレッドです。
        /// </summary>
        Thread stdErrMon = null;

        /// <summary>
        /// 排他ロック用オブジェクトです。
        /// </summary>
        Object procLock = new object();

        /// <summary>
        /// コンソールがオープンしているかのフラグです。
        /// </summary>
        private bool isOpen = false;

        /// <summary>
        /// コンソール既定の標準入力ハンドル退避エリアです。
        /// </summary>
        private IntPtr acStdIn;

        /// <summary>
        /// コンソール既定の標準出力ハンドル退避エリアです。
        /// </summary>
        private IntPtr acStdOut;

        /// <summary>
        /// コンソール既定の標準エラー出力ハンドル退避エリアです。
        /// </summary>
        private IntPtr acStdErr; 

        #endregion

        #region プロパティ

        #region Timeoutプロパティ

        private long timeout = -1;
        /// <summary>
        /// コマンド実行可能時間をミリ秒で参照または設定します。
        /// 開始からの経過時間が指定時間を過ぎると強制終了します。
        /// マイナスの時間を設定した場合は、時間を監視しません。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(-1),
            Description("コマンド実行可能時間をミリ秒で参照または設定します。開始からの経過時間が指定時間を過ぎると強制終了します。マイナスの時間を設定した場合は、時間を監視しません。")
        ]
        public long Timeout
        {
            get
            {
                return timeout;
            }
            set
            {
                timeout = value;
            }
        }

        #endregion

        #region ResponseTimeoutプロパティ

        private long responseTimeout = -1;
        /// <summary>
        /// レスポンス待ち時間をミリ秒で参照または設定します。
        /// 開始または前回レスポンスからの経過時間が指定時間を過ぎると強制終了します。
        /// マイナスの時間を設定した場合は、時間を監視しません。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(-1),
            Description("レスポンス待ち時間をミリ秒で参照または設定します。開始または前回レスポンスからの経過時間が指定時間を過ぎると強制終了します。マイナスの時間を設定した場合は、時間を監視しません。")
        ]
        public long ResponseTimeout
        {
            get
            {
                return responseTimeout;
            }
            set
            {
                responseTimeout = value;
            }
        }

        #endregion

        #region BackColorプロパティ

        private ConsoleColor backColor = System.ConsoleColor.Black;
        /// <summary>
        /// コンソールの背景色です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(ConsoleColor), "Black"),
            Description("コンソールの背景色です。")
        ]
        public ConsoleColor BackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                backColor = value;
                Console.BackgroundColor = backColor;
            }
        }

        #endregion

        #region ForeColorプロパティ

        private ConsoleColor foreColor = System.ConsoleColor.White;
        /// <summary>
        /// コンソールの前景色です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(ConsoleColor), "White"),
            Description("コンソールの前景色です。")
        ]
        public ConsoleColor ForeColor
        {
            get
            {
                return foreColor;
            }
            set
            {
                foreColor = value;
            }
        }

        #endregion

        #region CommandColorプロパティ

        private ConsoleColor commandColor = System.ConsoleColor.Cyan;
        /// <summary>
        /// コンソールのコマンド表示色です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(ConsoleColor), "Cyan"),
            Description("コンソールのコマンド表示色です。")
        ]
        public ConsoleColor CommandColor
        {
            get
            {
                return commandColor;
            }
            set
            {
                commandColor = value;
            }
        }

        #endregion

        #region ParamColorプロパティ

        private ConsoleColor paramColor = System.ConsoleColor.Yellow;
        /// <summary>
        /// コンソールのパラメータ入力表示色です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(ConsoleColor), "Yellow"),
            Description("コンソールのパラメータ入力表示色です。")
        ]
        public ConsoleColor ParamColor
        {
            get
            {
                return paramColor;
            }
            set
            {
                paramColor = value;
            }
        }

        #endregion

        #region ErrorColorプロパティ

        private ConsoleColor errorColor = System.ConsoleColor.Red;
        /// <summary>
        /// コンソールのエラー出力時前景色です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(ConsoleColor), "Red"),
            Description("コンソールのエラー出力時前景色です。")
        ]
        public ConsoleColor ErrorColor
        {
            get
            {
                return errorColor;
            }
            set
            {
                errorColor = value;
            }
        }

        #endregion

        #region ErrorLevelColorプロパティ

        private ConsoleColor errorLevelColor = System.ConsoleColor.Green;
        /// <summary>
        /// ERRORLEVEL出力時の前景色です。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(typeof(ConsoleColor), "Green"),
            Description("ERRORLEVEL出力時の前景色です。")
        ]
        public ConsoleColor ErrorLevelColor
        {
            get
            {
                return errorLevelColor;
            }
            set
            {
                errorLevelColor = value;
            }
        }

        #endregion

        #region EchoStdInプロパティ

        private bool echoStdIn = true;
        /// <summary>
        /// 標準入力をエコーバックするかどうかを参照または設定します。
        /// cmd.exe環境など、エコーバックされる場合は false に設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(true),
            Description("標準入力をエコーバックするかどうかを参照または設定します。cmd.exe環境など、エコーバックされる場合は false に設定します。")
        ]
        public bool EchoStdIn
        {
            get
            {
                return echoStdIn;
            }
            set
            {
                echoStdIn = value;
            }
        }

        #endregion

        #endregion

        #region イベント

        #region OutputValueイベント

        #region OutputValueEventArgsクラス

        /// <summary>
        /// OutputValueイベントのイベントデータを提供します。
        /// </summary>
        public class OutputValueEventArgs : EventArgs
        {
            /// <summary>
            /// STDOUTの場合は false、STDERRの場合は true になります。
            /// </summary>
            public bool IsError;

            /// <summary>
            /// 起動時に指定した標準入力データを出力中かを示します。出力が終わると false になります。
            /// </summary>
            public bool IsStdInWriting;

            /// <summary>
            /// STDOUT または STDERR の出力内容が設定されます。
            /// </summary>
            public string OutputText;

            /// <summary>
            /// true にすると強制終了し、System.Threading.ThreadAbortException を発生させます。
            /// </summary>
            public bool Abort;

            /// <summary>
            /// STDIN に渡す内容を設定します。
            /// </summary>
            public ArrayList InputLines;
        }

        #endregion

        public delegate void OutputValueEventHandler(object sender, OutputValueEventArgs e);
        /// <summary>
        /// 標準出力または標準エラー出力のレスポンスを通知するイベントです。
        /// </summary>
        public event OutputValueEventHandler OutputValueEvent;

        #endregion

        #region Loggingイベント

        #region LoggingEventArgsクラス

        /// <summary>
        /// Loggingイベントのイベントデータを提供します。
        /// </summary>
        public class LoggingEventArgs : EventArgs
        {
            /// <summary>
            /// 起動からの経過時間（ミリ秒）です。
            /// </summary>
            public long Time;

            /// <summary>
            /// テキストのタイプです。C=コマンドライン, I=入力, O=出力, E=エラー出力, L=ERRORLEVEL を表します。
            /// </summary>
            public string Type;

            /// <summary>
            /// ログ出力用のテキストです。
            /// </summary>
            public string Text;
        }

        #endregion

        public delegate void LoggingEventHandler(object sender, LoggingEventArgs e);
        /// <summary>
        /// 実行内容をログ用に通知するイベントです。
        /// </summary>
        public event LoggingEventHandler LoggingEvent;

        #endregion

        #endregion

        #region メソッド

        #region コンソール開始 (OpenConsole)

        /// <summary>
        /// コンソールを開きます。
        /// 使用する場合は、コマンド初回実行前に呼び出してください。
        /// </summary>
        public void OpenConsole()
        {
            if(!isOpen)
            {
                IntPtr hwnd = GetConsoleWindow();
                if (hwnd == IntPtr.Zero) AllocConsole();
                Console.Clear();
                ShowWindow(hwnd, 8);
                isOpen = true;
            }
        }

        #endregion

        #region コンソール終了 (CloseConsole)

        /// <summary>
        /// コンソールを閉じます。
        /// </summary>
        public void CloseConsole()
        {
            CloseConsole(false);
        }
        private void CloseConsole(bool free)
        {
            if (isOpen)
            {
                IntPtr hwnd = GetConsoleWindow();
                if (hwnd != IntPtr.Zero) ShowWindow(hwnd, 0);
                isOpen = false;
            }
            if (free) FreeConsole();
        }

        #endregion

        #region コマンド実行 (Execute)

        /// <summary>
        /// コマンドを実行します。
        /// ウィンドウアプリの場合は、OpenConsoleを呼び出してから実行してください。
        /// </summary>
        /// <param name="commandline">コマンドライン</param>
        /// <returns>結果コード(実行するプログラムに依存します)</returns>
        /// <remarks>
        /// レスポンスに応じた応答を行うには、OutputValueEventを登録してください。
        /// 動作内容を記録したい場合は、LoggingEventを登録してください。
        /// </remarks>
        public int Execute(string commandline)
        {
            return ProcessControl(commandline, new ArrayList());
        }

        /// <summary>
        /// コマンドを実行します。
        /// レスポンスに応じた応答を行うには、OutputValueEventを登録してください。
        /// </summary>
        /// <param name="commandline">コマンドライン</param>
        /// <param name="stdinData">標準入力データ</param>
        /// <returns>結果コード(実行するプログラムに依存します)</returns>
        /// <remarks>
        /// レスポンスに応じた応答を行うには、OutputValueEventを登録してください。
        /// 動作内容を記録したい場合は、LoggingEventを登録してください。
        /// </remarks>
        public int Execute(string commandline, ArrayList stdinData)
        {
            return ProcessControl(commandline, stdinData);
        }

        #endregion

        #region 標準入力書き込み (STDInWrite)

        /// <summary>
        /// 標準入力にデータを書き込みます。
        /// </summary>
        /// <param name="data">データ</param>
        public void STDInWrite(string data)
        {
            // コンソールに出力
            if (echoStdIn) ConsoleWrite("I", data);
            
            // 標準入力に書き込み
            proc.StandardInput.Write(data);
        }

        #endregion

        #region 強制終了 (Abort)

        /// <summary>
        /// コマンドの実行を中止します。
        /// </summary>
        public void Abort()
        {
            if (proc == null) return;
            if (proc.HasExited) return;
            proc.Kill();
        }

        #endregion

        #endregion

        #region 内部処理

        #region プロセス実行 (ProcessControl)

        /// <summary>
        /// プロセスを実行します。
        /// </summary>
        /// <param name="args">実行プログラムを含むパラメータリスト</param>
        /// <returns></returns>
        private int ProcessControl(string commandline, ArrayList stdinData)
        {
            int errlvl = 0;
            
            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo();
            procStartInfo.CreateNoWindow = true;

            // 入出力を読み書きできるようにする
            procStartInfo.RedirectStandardInput = true;
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.UseShellExecute = false;

            // ComSpecのパスを取得する
            procStartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");

            // コマンドラインを指定（"/c"は実行後閉じるための設定）
            procStartInfo.Arguments = "/c " + commandline;

            // 標準入力データ初期設定
            dtix = 0;
            noRes = true;
            dts = stdinData;

            // 別プロセスで起動
            string cmdout = System.IO.Directory.GetCurrentDirectory() + ">" + commandline + System.Environment.NewLine;
            ConsoleWrite("C", cmdout);
            proc = System.Diagnostics.Process.Start(procStartInfo);

            // 標準出力・標準エラー出力モニタスレッド起動
            stdOutMon = new Thread(new ThreadStart(stdOutMonitor));
            stdOutMon.Start();
            stdErrMon = new Thread(new ThreadStart(stdErrMonitor));
            stdErrMon.Start();

            // タイムアウトモニタ開始
            timall.Reset();
            timall.Start();
            timres.Reset();
            timres.Start();
            try
            {
                noRes = true;
                while (true)
                {
                    System.Threading.Thread.Sleep(10);
                    lock(procLock)
                    {
                        if (noRes)
                        {
                            if (dtix < dts.Count)
                            {
                                // 標準入力処理
                                STDInWrite(stdinData[dtix++].ToString());
                            }
                            else
                            {
                                if (proc.HasExited) break;
                            }
                        }
                        else
                        {
                            noRes = true;
                            timres.Reset();
                            timres.Start();
                        }

                        // タイムアウト判定
                        if (timeout >= 0 && timeout < timall.ElapsedMilliseconds)
                        {
                            float m = ((float)timeout) / 1000;
                            string errmsg = "Timeout時間(" + m.ToString() + "秒)を超えました。" + System.Environment.NewLine;
                            ConsoleWrite("O", System.Environment.NewLine);
                            ConsoleWrite("E", errmsg);
                            proc.Kill();
                            break;
                        }

                        if (responseTimeout >= 0 && responseTimeout < timres.ElapsedMilliseconds)
                        {
                            float m = ((float)responseTimeout) / 1000;
                            string errmsg = "ResponseTimeout時間(" + m.ToString() + "秒)を超えました。" + System.Environment.NewLine;
                            ConsoleWrite("O", System.Environment.NewLine);
                            ConsoleWrite("E", errmsg);
                            Console.ForegroundColor = foreColor;
                            proc.Kill();
                            break;
                        }
                    }
                }
            }
            finally
            {
                //終了
                errlvl = proc.ExitCode;
                string lvlmsg = "ERRORLEVEL " + errlvl.ToString() + System.Environment.NewLine;
                ConsoleWrite("O", System.Environment.NewLine);
                ConsoleWrite("L", lvlmsg);
                stdOutMon.Abort();
                stdErrMon.Abort();
                proc.Dispose();
            }

            return errlvl;
        }

        private System.Diagnostics.Process proc = null;
        private ArrayList dts = new ArrayList();
        private int dtix = 0;
        private bool noRes = true;
        private bool onErr = false;
        private Stopwatch timall = new System.Diagnostics.Stopwatch();
        private Stopwatch timres = new System.Diagnostics.Stopwatch();

        #endregion

        #region 標準出力モニタスレッド (stdOutMonitor)

        /// <summary>
        /// 標準出力モニタスレッドです。
        /// </summary>
        private void stdOutMonitor()
        {
            char[] charbuf = new char[4096];
            int len;
            string linbuf = "";
            try
            {
                while (true)
                {
                    // 標準出力を読む
                    if (linbuf.Length == 0)
                    {
                        len = proc.StandardOutput.Read(charbuf, 0, charbuf.Length);
                        if (len > 0) linbuf += new string(charbuf, 0, len);
                    }
                    if (linbuf.Length > 0 && onErr == false)
                    {
                        lock (procLock)
                        {
                            string buf = "";
                            int pos = linbuf.IndexOf(System.Environment.NewLine);
                            if (pos < 0)
                            {
                                buf = linbuf;
                                linbuf = "";
                            }
                            else
                            {
                                pos += System.Environment.NewLine.Length;
                                buf = linbuf.Substring(0, pos);
                                linbuf = linbuf.Substring(pos);
                            }
                            STDInOutErr(buf, false, dtix < dts.Count);
                            noRes = false;
                        }
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                }
            }
            catch(ThreadAbortException)
            {
                // スレッド終了
            }
        }

        #endregion

        #region 標準エラー出力モニタスレッド (stdErrMonitor)

        /// <summary>
        /// 標準エラー出力モニタスレッドです。
        /// </summary>
        private void stdErrMonitor()
        {
            char[] charbuf = new char[4096];
            int len;
            string linbuf = "";
            try
            {
                while (true)
                {
                    if (linbuf.Length == 0)
                    {
                        len = proc.StandardError.Read(charbuf, 0, charbuf.Length);
                        if (len > 0) linbuf += new string(charbuf, 0, len);
                    }
                    if (linbuf.Length > 0)
                    {
                        onErr = true;
                        lock (procLock)
                        {
                            string buf = "";
                            int pos = linbuf.IndexOf(System.Environment.NewLine);
                            if (pos < 0)
                            {
                                buf = linbuf;
                                linbuf = "";
                            }
                            else
                            {
                                pos += System.Environment.NewLine.Length;
                                buf = linbuf.Substring(0, pos);
                                linbuf = linbuf.Substring(pos);
                            }
                            STDInOutErr(buf, true, dtix < dts.Count);
                            noRes = false;
                        }
                        if (linbuf.Length < 1) onErr = false;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // スレッド終了
            }
        }

        #endregion

        #region 標準入出力処理 (STDInOutErr)

        /// <summary>
        /// 標準出力および標準エラー出力を受け取り応答を処理します。
        /// </summary>
        /// <param name="data">出力内容</param>
        /// <param name="isError">出力種別(true=標準エラー出力)</param>
        /// <param name="isStdinWriting">起動時の標準入力データ(true=出力中)</param>
        /// <returns>処理したら true</returns>
        private bool STDInOutErr(string data, bool isError, bool isStdinWriting)
        {
            // コンソールに出力
            if (isError)
            {
                ConsoleWrite("E", data);
            }
            else
            {
                ConsoleWrite("O", data);
            }

            // イベント通知
            OutputValueEventArgs e = new OutputValueEventArgs();
            e.Abort = false;
            e.InputLines = new ArrayList();
            e.IsError = isError;
            e.IsStdInWriting = isStdinWriting;
            e.OutputText = data;
            if (OutputValueEvent != null)
            {
                try
                {
                    OutputValueEvent(this, e);
                }
                catch(Exception es)
                {
                    ConsoleWrite("E", es.ToString());
                }
            }

            // 応答処理
            if (e.Abort) Abort();
            dts.AddRange(e.InputLines);

            return true;
        }

        #endregion

        #region コンソール出力処理 (ConsoleWrite)

        private string logBuffer = "";
        /// <summary>
        /// コンソール出力を行います。
        /// </summary>
        /// <param name="type">テキストタイプ(C=CommandLine, I=STDIN, O=STDOUT, E=STDERR, L=ERRORLEVEL)</param>
        /// <param name="text">コンソール出力用テキスト(改行を含みます)</param>
        private void ConsoleWrite(string type, string text)
        {
            // コンソール出力
            switch (type)
            {
                case "C":
                    Console.ForegroundColor = commandColor;
                    break;
                case "I":
                    Console.ForegroundColor = paramColor;
                    break;
                case "O":
                    Console.ForegroundColor = foreColor;
                    break;
                case "E":
                    Console.ForegroundColor = errorColor;
                    break;
                case "L":
                    Console.ForegroundColor = errorLevelColor;
                    break;
            }
            Console.Write(text);
            Console.ForegroundColor = foreColor;

            // ログ出力用編集
            logBuffer += text;
            int s = logBuffer.IndexOf(System.Environment.NewLine);
            if (s < 0) return;

            // ロギングイベント通知
            LoggingEventArgs e = new LoggingEventArgs();
            e.Time = timall.ElapsedMilliseconds;
            e.Type = type;
            e.Text = logBuffer.Substring(0, s);
            logBuffer = logBuffer.Substring(s + System.Environment.NewLine.Length);            
            if (LoggingEvent != null)
            {
                try
                {
                    LoggingEvent(this, e);
                }
                catch (Exception es)
                {
                    Console.ForegroundColor = errorColor;
                    Console.WriteLine(es.ToString());
                }
            }
        }

        #endregion

        #endregion
    }
}
