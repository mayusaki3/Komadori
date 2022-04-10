using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace XControls.IO
{
    /// <summary>
    /// ログ出力を行うためのクラスです。
    /// </summary>
    [DebuggerNonUserCode]
    public class Logging : Object
    {
        #region 構築・破棄

        /// <summary>
        /// XControls.IO.Logging クラスの新しいインスタンスを初期化します。
        /// </summary>
        public Logging()
        {
        }

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected void Dispose(bool disposing)
        {
            #region 後処理


            #endregion
        }

        #endregion

        #region 定数

        /// <summary>
        /// プロセス・スレッド間排他に使用するMutex名
        /// </summary>
        const string MUTEX_NAME = "Mutex_XControls_IO_Logging";

        /// <summary>
        /// ダンプ用メッセージ
        /// </summary>
        const string DUMP_HEADER1 = "HexDump: ";
        const string DUMP_HEADER2 = "ADDR     +0 +1 +2 +3 +4 +5 +6 +7 +8 +9 +A +B +C +D +E +F 0123456789ABCDEF";
        const string DUMP_LINE = "-------- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- ----------------";

        /// <summary>
        /// 例外メッセージ
        /// </summary>
        const string EXCEPTION_NOW = "*** 例外: ";
        const string EXCEPTION_MESSAGE = "*** 操作ログ出力中に例外が発生しました: ";

        /// <summary>
        /// ログの出力モードを指定する定数です。
        /// </summary>
        public enum LoggingModes : int
        {
            /// <summary>
            /// ファイルに出力しません。
            /// </summary>
            None,
            /// <summary>
            /// 指定したサイズを超えたらファイルを切り替えます。ファイル名には(1-2)が入ります。常に1が新しいファイルです。
            /// </summary>
            Size,
            /// <summary>
            /// 日単位でファイルを切り替えます。ファイル名には日(1-31)が入ります。
            /// </summary>
            Day,
            /// <summary>
            /// 週単位でファイルを切り替えます。ファイル名には週(1-7)が入ります。
            /// </summary>
            Week,
            /// <summary>
            /// 月単位でファイルを切り替えます。ファイル名には月(1-12)が入ります。
            /// </summary>
            Month
        }

        #endregion

        #region 変数

        /// <summary>
        /// ログ出力に使用するミューテックスオブジェクトです。
        /// </summary>
        private Mutex mutex = null;

        /// <summary>
        /// 現在のログファイル名です。
        /// </summary>
        private string currentLogFile = "";

        /// <summary>
        /// ログ出力に使用するFileStreamクラスのインスタンスです。
        /// </summary>
        private FileStream logfile = null;

        /// <summary>
        /// ログ出力に使用するFileStreamクラスのインスタンスです。
        /// </summary>
        private StreamWriter logWriter = null;

        /// <summary>
        /// ログの行終端文字です。
        /// </summary>
        private static string[] SPLIT_CRLF = new string[] { "\r\n" };

        #endregion

        #region プロパティ

        #region LoggingModeプロパティ

        private LoggingModes loggingMode = LoggingModes.None;
        /// <summary>
        /// ログの出力モードを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("ログの出力モードを参照または設定します。")
        ]
        public LoggingModes LoggingMode
        {
            get
            {
                return loggingMode;
            }
            set
            {
                loggingMode = value;
            }
        }

        #endregion

        #region OutputDirectoryプロパティ

        private string outputDirectory = "";
        /// <summary>
        /// 操作ログの出力先ディレクトリを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("操作ログの出力先ディレクトリを参照または設定します。")
        ]
        public string OutputDirectory
        {
            get
            {
                return outputDirectory;
            }
            set
            {
                outputDirectory = value;
                if (outputDirectory.Length > 0)
                {
                    if (outputDirectory.Substring(outputDirectory.Length - 1, 1).Equals("\\"))
                    {
                    }
                    else
                    {
                        outputDirectory = outputDirectory + "\\";
                    }
                }
            }
        }

        #endregion

        #region OutputFileNameプロパティ

        private string outputFileName = "log[@].txt";
        /// <summary>
        /// 操作ログの出力ファイル名を参照または設定します。ファイル名の'[@]'の部分はLoggingModeに合わせて変化し、Sizeの場合は1～2, Dayの場合は1～31, Weekの場合は1～7, Monthの場合は1～12の値を取ります。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("操作ログの出力ファイル名を参照または設定します。ファイル名の'[@]'の部分はLoggingModeに合わせて変化し、Sizeの場合は1～2, Dayの場合は1～31, Weekの場合は1～7, Monthの場合は1～12の値を取ります。")
        ]
        public string OutputFileName
        {
            get
            {
                return outputFileName;
            }
            set
            {
                outputFileName = value;
                outputFileName.Replace("\\", "");
                if (outputFileName.LastIndexOf(".") < 1)
                {
                    outputFileName = outputFileName + ".txt";
                }
                if (outputFileName.LastIndexOf("[@]") < 1)
                {
                    int pos = outputFileName.LastIndexOf(".");
                    outputFileName = outputFileName.Substring(0, pos) +
                                     "[@]" +
                                     outputFileName.Substring(pos);
                }
            }
        }

        #endregion

        #region MaxFileSizeプロパティ

        private int maxFileSize = 2;
        /// <summary>
        /// 操作ログのファイルサイズ上限を参照または設定します。2～32767の範囲(単位KByte)で指定します。LoggingModeがSizeの場合に有効で、サイズを超えると1世代のみバックアップを作成します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("操作ログのファイルサイズ上限を参照または設定します。2～32767の範囲(単位KByte)で指定します。LoggingModeがSizeの場合に有効で、サイズを超えると1世代のみバックアップを作成します。")
        ]
        public int MaxFileSize
        {
            get
            {
                return maxFileSize;
            }
            set
            {
                if (value < 2)
                {
                    maxFileSize = 2;
                }
                else if (value > 32767)
                {
                    maxFileSize = 32767;
                }
                else
                {
                    maxFileSize = value;
                }
            }
        }

        #endregion

        #region TraceTriggerTextプロパティ

        private string traceTriggerText = "";
        /// <summary>
        /// トリガー用の文字列を参照または設定します。設定した文字列がログ出力内容に含まれていた場合、ログにスタックトレースを出力します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("トリガー用の文字列を参照または設定します。設定した文字列がログ出力内容に含まれていた場合、ログにスタックトレースを出力します。")
        ]
        public string TraceTriggerText
        {
            get
            {
                return traceTriggerText;
            }
            set
            {
                traceTriggerText = value;
            }
        }

        #endregion

        #region NoCriticalControlプロパティ

        private bool noCriticalControl = false;
        /// <summary>
        /// ログ出力時のクリティカルセクション制御を無効にするかどうかを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("ログ出力時のクリティカルセクション制御を無効にするかどうかを参照または設定します。")
        ]
        public bool NoCriticalControl
        {
            get
            {
                return noCriticalControl;
            }
            set
            {
                noCriticalControl = value;
            }
        }

        #endregion

        #region MuteLoggingNoticeプロパティ

        private bool muteLoggingNotice = false;
        /// <summary>
        /// ログ出力時のLoggingNoticeイベントを無効化するかどうかを示します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("ログ出力時のLoggingNoticeイベントを無効化するかどうかを示します。")
        ]
        public bool MuteLoggingNotice
        {
            get
            {
                return muteLoggingNotice;
            }
            set
            {
                muteLoggingNotice = value;
            }
        }

        #endregion

        #region MuteHexDumpLoggingNoticeプロパティ

        private bool muteHexDumpLoggingNotice = false;
        /// <summary>
        /// ログ16進出力時のLoggingNoticeイベントを無効化するかどうかを示します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("ログ16進出力時のLoggingNoticeイベントを無効化するかどうかを示します。")
        ]
        public bool MuteHexDumpLoggingNotice
        {
            get
            {
                return muteHexDumpLoggingNotice;
            }
            set
            {
                muteHexDumpLoggingNotice = value;
            }
        }

        #endregion

        #endregion

        #region イベント

        #region メッセージ通知 (LoggingNotice)

        /// <summary>
        /// LoggingNoticeイベントのイベントデータを提供します。
        /// </summary>
        public class LoggingNoticeEventArgs : EventArgs
        {
            /// <summary>
            /// ログの発生日時とプロセス情報
            /// </summary>
            public string DateTimeProcess;

            /// <summary>
            /// 出力されるログ内容
            /// </summary>
            public string Message;
        }
        public delegate void LoggingNoticeEventHandler(object sender, LoggingNoticeEventArgs e);
        /// <summary>
        /// ログ出力を通知するイベントです。
        /// </summary>
        [
            Category("動作"),
            Description("ログ出力を通知するイベントです。")
        ]
        public event LoggingNoticeEventHandler LoggingNotice;

        #endregion

        #endregion

        #region メソッド

        #region ログ出力 (Print)

        /// <summary>
        /// 指定された内容をログに出力します。
        /// </summary>
        /// <param name="msg">ログに出力する内容</param>
        public void Print(String msg)
        {
            try
            {
                FileOpen();
                FileWrite(msg);

                if (TraceTriggerText.Length > 0)
                {
                    if (msg.IndexOf(TraceTriggerText) >= 0)
                    {
                        // 情報
                        FileWrite("Track trace ==>");
                        System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                        for (int i = 0; i < st.FrameCount; i++)
                        {
                            System.Diagnostics.StackFrame sf = st.GetFrame(i);
                            FileWrite("   - " + sf.ToString().Replace("\r\n", ""));
                        }
                    }
                }
            }
            catch (Exception es)
            {
                try
                {
                    FileWrite(EXCEPTION_MESSAGE + es.Message);
                }
                catch
                {
                }
            }
            finally
            {
                FileClose();
            }
        }

        #endregion

        #region ログ16進出力 (HexDump)

        /// <summary>
        /// 指定された内容を16進でログに出力します。
        /// </summary>
        /// <param name="msg">ダンプの説明</param>
        public void HexDump(string msg)
        {
            try
            {
                FileOpen();
                FileWrite(msg, true);
            }
            catch (Exception es)
            {
                try
                {
                    FileWrite(EXCEPTION_MESSAGE + es.Message);
                }
                catch
                {
                }
            }
            finally
            {
                FileClose();
            }
        }

        /// <summary>
        /// 指定された内容を16進でログに出力します。
        /// </summary>
        /// <param name="nam">ダンプの名前</param>
        /// <param name="mem">ログに出力する内容</param>
        /// <param name="size">出力するバイト数</param>
        public void HexDump(string nam, byte[] mem, int size)
        {
            HexDump(nam, mem, 0, size);
        }

        /// <summary>
        /// 指定された内容を16進でログに出力します。
        /// </summary>
        /// <param name="nam">ダンプの名前</param>
        /// <param name="mem">ログに出力する内容</param>
        /// <param name="start">0から始まる出力開始位置</param>
        /// <param name="size">出力するバイト数</param>
        public void HexDump(string nam, byte[] mem, int start, int size)
        {
            StringBuilder hexArea;          // 16進表示部編集ワーク
            StringBuilder charArea;         // 文字表示部編集ワーク
            int i = 0;
            int j = 0;

            try
            {
                FileOpen();
                FileWrite(DUMP_HEADER1 + nam, true);
                FileWrite(DUMP_HEADER2, true);
                FileWrite(DUMP_LINE, true);

                for (i = start; i < start + size; i += 16)
                {
                    // 1行分編集
                    hexArea = new StringBuilder(48);
                    charArea = new StringBuilder(16);
                    for (j = 0; j < 16; j++)
                    {
                        if (i + j < start + size)
                        {
                            // 出力範囲内
                            hexArea.Append(mem[i + j].ToString("X2") + " ");
                            if (mem[i + j] < 0x20 || mem[i + j] > 0x7f)
                            {
                                charArea.Append(".");
                            }
                            else
                            {
                                charArea.Append(Convert.ToChar(mem[i + j]));
                            }
                        }
                        else
                        {
                            // 出力範囲外
                            hexArea.Append("   ");
                            charArea.Append(" ");
                        }
                    }
                    FileWrite(i.ToString("X8") + " " + hexArea.ToString() + charArea.ToString(), true);
                }

                FileWrite(DUMP_LINE, true);
            }
            catch (Exception es)
            {
                try
                {
                    FileWrite(EXCEPTION_MESSAGE + es.Message);
                }
                catch
                {
                }
            }
            finally
            {
                FileClose();
            }
        }

        #endregion

        #region 現在のログファイルを取得 (GetCurrentLogFile)

        /// <summary>
        /// 現在のログファイル名をフルパスで取得します。
        /// </summary>
        public string GetCurrentLogFile()
        {
            FileOpen();
            FileClose();
            return currentLogFile;
        }

        #endregion

        #endregion

        #region 内部処理

        #region ログファイルオープン (FileOpen)

        /// <summary>
        /// ログファイルを開く
        /// 必要に応じてログファイルを切り替える
        /// </summary>
        private void FileOpen()
        {
            DateTime dt = DateTime.Now;
            string crrFile = "";
            string bakFile = "";

            // ディレクトリを絶対パスに変換
            Uri appUri = new Uri(Application.StartupPath + "\\");
            Uri outUri = new Uri(appUri, System.Environment.ExpandEnvironmentVariables(outputDirectory));
            string outPath = outUri.LocalPath;

            // ログ出力モードチェック
            if(loggingMode== LoggingModes.None) {
                return;
            }
            
            // Mutexの所有権を得る - 最大3秒待つ
            mutex = new Mutex(false, MUTEX_NAME); 
            string exception_msg = "";
            try
            {
                if (!noCriticalControl) mutex.WaitOne(3000, false);
            }
            catch (Exception es)
            {
                exception_msg = EXCEPTION_NOW + "[OPEN]:" + es.Message + "\r\n" + es.StackTrace;
            }

            // ディレクトリを作成
            Directory.CreateDirectory(outPath);

            // ファイル名を編集
            switch (loggingMode)
            {
                case LoggingModes.Day:
                    crrFile = outputFileName.Replace("[@]", dt.Day.ToString("d2"));
                    break;
                case LoggingModes.Month:
                    crrFile = outputFileName.Replace("[@]", dt.Month.ToString("d2"));
                    break;
                case LoggingModes.Size:
                    crrFile = outputFileName.Replace("[@]", "1");
                    bakFile = outputFileName.Replace("[@]", "2");
                    break;
                case LoggingModes.Week:
                    crrFile = outputFileName.Replace("[@]", ((int)(dt.DayOfWeek+1)).ToString());
                    break;
            }
            currentLogFile = outPath + crrFile;
            FileInfo fi = new FileInfo(currentLogFile);

            if (loggingMode == LoggingModes.Size)
            {
                // ファイルサイズをチェック
                if (fi.Exists==true)
                {
                    if (fi.Length > maxFileSize * 1024)
                    {
                        try
                        {
                            File.Delete(outPath + bakFile);
                            File.Move(currentLogFile, outPath + bakFile);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            else
            {
                // ファイル日付チェック
                if((fi.LastWriteTime.Year  != dt.Year)  ||
                   (fi.LastWriteTime.Month != dt.Month) ||
                   (fi.LastWriteTime.Day   != dt.Day))
                {
                    File.Delete(currentLogFile);
                }
            }

            // ファイルを開く
            try
            {
                logfile = new FileStream(this.currentLogFile, FileMode.Append);
                logWriter = new StreamWriter(logfile, Encoding.GetEncoding(932));
            }
            catch
            {
            }

            // 例外メッセージ
            if (exception_msg.Length > 0)
            {
                FileWrite(exception_msg);
            }
        }

        #endregion

        #region ログファイルクローズ (FileClose)

        /// <summary>
        /// ログファイルを閉じる
        /// </summary>
        private void FileClose()
        {
            // ログ出力モードチェック
            if (loggingMode == LoggingModes.None)
            {
                return;
            }

            // ファイルを閉じる
            try
            {
                logWriter.Close();
                logfile.Close();
            }
            catch (Exception es)
            {
                FileWrite(EXCEPTION_NOW + "[CLOSE]:" + es.Message + "\r\n" + es.StackTrace);
            }

            // Mutexの所有権を解放
            try
            {
                if (!noCriticalControl) mutex.ReleaseMutex();
            }
            catch (ApplicationException)
            {
                // 無視 - 本当は良くないが、悪影響はないと判断
            }
            catch (Exception es)
            {
                FileWrite(EXCEPTION_NOW + "[CLOSE]:" + es.Message + "\r\n" + es.StackTrace);
            }
        }

        #endregion

        #region ログファイル出力 (FileWrite)

        /// <summary>
        /// 指定された内容に情報を付加してコンソールおよびログファイルに出力する
        /// </summary>
        /// <param name="msg">ログに出力する内容</param>
        private void FileWrite(String msg)
        {
            FileWrite(msg, false);
        }

        /// <summary>
        /// 指定された内容に情報を付加してコンソールおよびログファイルに出力する
        /// </summary>
        /// <param name="msg">ログに出力する内容</param>
        /// <param name="hexdump">HexDumpの時true</param>
        private void FileWrite(String msg, bool hexdump)
        {
            if (LoggingNotice != null && !(hexdump && muteHexDumpLoggingNotice) && !(!hexdump && muteLoggingNotice))
            {
                // ハンドラが設定されており、ミュートでなければイベント発生
                LoggingNoticeEventArgs eArgs = new LoggingNoticeEventArgs();
                eArgs.Message = msg;
                try
                {
                    LoggingNotice(this, eArgs);
                }
                catch
                {
                }
            }            

            // ログ出力モードチェック
            if (loggingMode == LoggingModes.None)
            {
                return;
            }

            // ログファイル出力
            try
            {
                StringBuilder sb = new StringBuilder();
                DateTime dt = DateTime.Now;

                sb.Append(dt.Year.ToString("d4"));
                sb.Append(dt.Month.ToString("d2"));
                sb.Append(dt.Day.ToString("d2"));
                sb.Append(dt.Hour.ToString("d2"));
                sb.Append(dt.Minute.ToString("d2"));
                sb.Append(dt.Second.ToString("d2"));
                sb.Append(".");
                sb.Append(dt.Millisecond.ToString("d3"));
                sb.Append(" ");
                sb.Append(Process.GetCurrentProcess().Id.ToString("d5"));
                sb.Append("-");
                sb.Append(Thread.CurrentThread.ManagedThreadId.ToString("d5"));
                sb.Append("| " + msg);
                logWriter.WriteLine(sb.ToString());
            }
            catch
            {
            }
        }

        #endregion

        #endregion
    }
}
