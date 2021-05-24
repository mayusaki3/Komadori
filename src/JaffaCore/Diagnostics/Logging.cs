using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jaffa.Diagnostics
{
    /// <summary>
    /// Jaffaフレームワーク・ロギング機能
    /// </summary>
    public static partial class Logging : Object
    {
        #region イベント

        #region ログ出力イベント (LogWriting)

        /// <summary>
        /// ログ出力の内容を通知します。
        /// </summary>
        public static event EventHandler<LogWritingEventArgs> LogWriting;

        #endregion

        #endregion

        #region メソッド

        #region ログメッセージ書き込み (Write)

        /// <summary>
        /// ログにメッセージを書き込みます。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="type">ログタイプ</param>
        public static void Write(String message, LogType type = LogType.Information)
        {
            Write(new string[] { message }, type);
        }

        /// <summary>
        /// ログにメッセージを書き込みます。
        /// </summary>
        /// <param name="type">ログタイプ</param>
        /// <param name="messages">メッセージリスト</param>
        public static void Write(String[] messages, LogType type = LogType.Information)
        {
            List<string> msgs = new(messages);
            Write(msgs, type);
        }

        /// <summary>
        /// ログにメッセージを書き込みます。
        /// </summary>
        /// <param name="exp">例外</param>
        /// <param name="type">ログタイプ</param>
        public static void Write(Exception exp, LogType type = LogType.Error)
        {
            Write(ExceptionToList(exp), type);
        }

        /// <summary>
        /// ログにメッセージを書き込みます。
        /// </summary>
        /// <param name="messages">メッセージリスト</param>
        /// <param name="type">ログタイプ</param>
        public static void Write(List<string> messages, LogType type = LogType.Information)
        {
            // 開始メッセージ
            if (LoggingSettings.FrameworkMessage == false)
            {
                IsUseStartMessage = false;
            }
            if (IsUseStartMessage == true)
            {
                IsUseStartMessage = false;
                Write(Core.MakeMessage(Core.Jaffa, Messages.JFWI0001, new string[] { Core.Version }));
            }

            LoggingData data = new(Jaffa.DateTime.Now, type, messages);
            DebugWrite(data);
            loggingBuffer.Enqueue(data);
            WriteLogFileAsync();
        }
        private static readonly ConcurrentQueue<LoggingData> loggingBuffer = new();

        #region DebugWrite

        [Conditional("DEBUG")]
        private static void DebugWrite(LoggingData data)
        {
            foreach (string msg in data.ToStrings())
            {
                System.Diagnostics.Debug.WriteLine(msg);
            }
        }

        /// <summary>
        /// デバッグ用にメッセージを出力します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        [Conditional("DEBUG")]
        private static void DebugWrite(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        /// <summary>
        /// デバッグ用にメッセージを出力します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="newname">新しいファイル名</param>
        /// <param name="oldname">古いファイル名</param>
        [Conditional("DEBUG")]
        private static void DebugWrite(string message, string newname, string oldname)
        {
            if (newname.Equals(oldname)) return;
            System.Diagnostics.Debug.WriteLine(message + newname);
        }

        #endregion

        #endregion

        #region ログ16進ダンプ書き込み (WriteDump)

        /// <summary>
        /// ログにバイト配列を16進ダンプとして書き込みます。
        /// </summary>
        /// <param name="bytes">ダンプ対象のバイト配列</param>
        /// <param name="type">ログタイプ</param>
        public static void WriteDump(byte[] bytes, LogType type = LogType.Information)
        {
            uint size = (uint)bytes.Length;
            WriteDump(bytes, 0, size, type);
        }

        /// <summary>
        /// ログにバイト配列を16進ダンプとして書き込みます。
        /// </summary>
        /// <param name="bytes">ダンプ対象のバイト配列</param>
        /// <param name="start">0から始まるダンプの開始位置</param>
        /// <param name="size">出力するバイト数</param>
        /// <param name="type">ログタイプ</param>
        public static void WriteDump(byte[] bytes, uint start, uint size, LogType type = LogType.Information)
        {
            Write(BytesToHexDump(bytes, start, size), type);
        }

        #endregion

        #region バイト配列16進ダンプ変換 (BytesToHexDump)

        /// <summary>
        /// バイト配列を16進ダンプに変換します。
        /// </summary>
        /// <param name="bytes">ダンプ対象のバイト配列</param>
        /// <returns>16進ダンプ</returns>
        public static List<string> BytesToHexDump(byte[] bytes)
        {
            uint size = (uint)bytes.Length;
            return BytesToHexDump(bytes, 0, size);
        }

        /// <summary>
        /// バイト配列の指定した範囲を16進ダンプに変換します。
        /// </summary>
        /// <param name="bytes">ダンプ対象のバイト配列</param>
        /// <param name="start">0から始まるダンプの開始位置</param>
        /// <param name="size">出力するバイト数</param>
        /// <returns></returns>
        public static List<string> BytesToHexDump(byte[] bytes, uint start, uint size)
        {
            List<string> rt = new();
            StringBuilder hexArea;          // 16進表示部編集ワーク
            StringBuilder charArea;         // 文字表示部編集ワーク
            try
            {
                rt.Add(DUMP_HEADER);
                rt.Add(DUMP_LINE);
                for (uint i = start; i < start + size; i += 16)
                {
                    // 1行分編集
                    hexArea = new StringBuilder(48);
                    charArea = new StringBuilder(16);
                    for (int j = 0; j < 16; j++)
                    {
                        if (i + j < start + size)
                        {
                            // 出力範囲内
                            hexArea.Append(bytes[i + j].ToString("X2") + " ");
                            if (bytes[i + j] < 0x20 || bytes[i + j] > 0x7f)
                            {
                                charArea.Append('.');
                            }
                            else
                            {
                                charArea.Append(Convert.ToChar(bytes[i + j]));
                            }
                        }
                        else
                        {
                            // 出力範囲外
                            hexArea.Append("   ");
                            charArea.Append(' ');
                        }
                    }
                    rt.Add(i.ToString("X8") + " " + hexArea.ToString() + charArea.ToString());
                }
            }
            catch (Exception es)
            {
                try
                {
                    rt.Add(Core.MakeMessage(Core.Jaffa, "**{LOGGING_EXCEPTION}: ") + es.Message.Remove('\r').Split(new char[] { '\n' })[0]);
                }
                catch { }
            }

            rt.Add(DUMP_LINE);
            return rt;
        }

        private const string DUMP_HEADER = "ADDR     +0 +1 +2 +3 +4 +5 +6 +7 +8 +9 +A +B +C +D +E +F 0123456789ABCDEF";
        private const string DUMP_LINE = "-------- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- ----------------";

        #endregion

        #region 例外をログ用リストに変換 (ExceptionToList)

        /// <summary>
        /// 例外をログ用リストに変換します。
        /// </summary>
        /// <param name="exp">例外</param>
        /// <returns>ログ用リスト</returns>
        public static List<string> ExceptionToList(Exception exp)
        {
            List<string> rt = new();
            rt.Add(exp.Message);
            if (exp.StackTrace != null)
            {
                foreach (string tr in exp.StackTrace.Split(new char[] { '\r', '\n' }))
                {
                    if (tr.Length > 0)
                    {
                        rt.Add("\t" + tr);
                    }
                }
            }
            return rt;
        }

        #endregion

        #region 非同期ログ出力 (WriteLogFileAsync) [private]

        private static async void WriteLogFileAsync()
        {
            while (logWriteWaiting == false && syslogWriteWaiting == false && !loggingBuffer.IsEmpty)
            {
                await writeisLock.WaitAsync();
                try
                {
                    if (loggingBuffer.TryDequeue(out LoggingData log))
                    {
                        #region ログ通知

                        if (MuteLoggingEvent == false)
                        {
                            try
                            {
                                // イベント通知
                                LogWriting?.Invoke(loggingBuffer, new LogWritingEventArgs(log.DateTime, log.ToShortStrings()));
                            }
                            catch { }
                        }

                        #endregion

                        #region ログ書き込み

                        if (LoggingSettings.LoggingMode != LoggingMode.None)
                        {
                            // ファイル名生成
                            string logName1 = "";
                            string logName2 = "";
                            switch (LoggingSettings.LoggingMode)
                            {
                                case LoggingMode.Size:
                                    logName1 = LoggingSettings.FileName.Replace("[@]", "01");
                                    logName2 = LoggingSettings.FileName.Replace("[@]", "02");
                                    break;
                                case LoggingMode.Day:
                                    logName1 = LoggingSettings.FileName.Replace("[@]", log.DateTime.ToString("dd"));
                                    break;
                                case LoggingMode.Week:
                                    logName1 = LoggingSettings.FileName.Replace("[@]", ((int)(log.DateTime.DayOfWeek + 1)).ToString("D2"));
                                    break;
                                case LoggingMode.Month:
                                    logName1 = LoggingSettings.FileName.Replace("[@]", log.DateTime.ToString("MM"));
                                    break;
                            }

                            // 書き込み
                            WriteTaskQueueCount++;
                            await WriteLogBufferToFileAsync(log, logName1, logName2);
                        }

                        #endregion
                    }
                }
                catch
                {
                    // エラー・無視する
                }
                finally
                {
                    writeisLock.Release();
                }
            }
        }
        private static readonly SemaphoreSlim writeisLock = new(1, 1);

        #endregion

        #region キャッシュされたログの書き込みを完了 (FlushAsync)

        /// <summary>
        /// キャッシュされたログの書き込みを完了します。
        /// </summary>
        public static async Task FlushAsync()
        {
            WriteLogFileAsync();
            while (WriteTaskQueueCount > 0)
            {
                await Task.Delay(10);
            }
        }

        #endregion

        #endregion

        #region プロパティ

        #region ログ出力中断を参照または設定 ([R/W] LogWriteWaiting)

        /// <summary>
        /// ログ出力中断を参照または設定します。trueの場合、メモリ上にバッファリングされます。
        /// </summary>
        public static bool LogWriteWaiting
        {
            get
            {
                return logWriteWaiting;
            }
            set
            {
                logWriteWaiting = value;
                if (!logWriteWaiting)
                {
                    WriteLogFileAsync();
                }
            }
        }
        private static bool logWriteWaiting = true;

        #endregion

        #region Jaffaフレームワーク内ログ出力中断を参照または設定 ([R/W] SysLogWriteWaiting)

        /// <summary>
        /// ログ出力中断を参照または設定します。Jaffaフレームワーク内で使用します。
        /// </summary>
        public static bool SysLogWriteWaiting
        {
            get
            {
                return syslogWriteWaiting;
            }
            set
            {
                syslogWriteWaiting = value;
                if (!syslogWriteWaiting)
                {
                    WriteLogFileAsync();
                }
            }
        }
        private static bool syslogWriteWaiting = false;

        #endregion

        #region ログ出力時のLoggingイベント無効化を参照または設定 ([R/W] MuteLoggingEvent)

        /// <summary>
        /// ログ出力時のLoggingイベント無効化を参照または設定します。
        /// </summary>
        public static bool MuteLoggingEvent { get; set; } = false;

        #endregion

        #region 最後にログ出力したファイル名を参照 ([R] LastFilename)

        /// <summary>
        /// 最後にログ出力したファイル名を参照します。
        /// </summary>
        public static string LastFilename
        {
            get
            {
                return lastFilename;
            }
        }
        private static string lastFilename = "";

        #endregion

        #region ログ書き込みキューイング数を参照 ([R] WriteQueueCount)

        /// <summary>
        /// ログ書き込みキューイング数を参照します。
        /// </summary>
        public static int WriteQueueCount
        {
            get
            {
                return WriteTaskQueueCount;
            }
        }

        #endregion

        #region ログ書き込みタスクキューイング数を参照または設定 ([R/W] WriteQueueCount) [private]

        /// <summary>
        /// ログ書き込みタスクキューイング数を参照または設定します。
        /// </summary>
        private static int WriteTaskQueueCount { get; set; } = 0;

        #endregion

        #region Jaffa起動メッセージ出力使用を参照または設定 ([R/W] IsUseStartMessage) [private]

        /// <summary>
        /// Jaffa起動メッセージ出力使用
        /// </summary>
        private static bool IsUseStartMessage { get; set; } = true;

        #endregion

        #endregion
    }
}
