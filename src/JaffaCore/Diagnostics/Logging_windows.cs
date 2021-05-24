using System;
using System.IO;
using System.Threading.Tasks;

namespace Jaffa.Diagnostics
{
    /// <summary>
    /// Jaffaフレームワーク・ロギング機能（for Windows）
    /// </summary>
    public static partial class Logging : Object
    {
        #region メソッド

        #region ログ書き込み・ログバッファ消去 (WriteLogBufferToFileAsync) [private]

        /// <summary>
        /// ログ書き込み・ログバッファ消去
        /// </summary>
        /// <param name="log">ログデータ</param>
        /// <param name="logName1">ログファイル名</param>
        /// <param name="logName2">ログファイル名（リネーム用）</param>
#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
        private static async Task WriteLogBufferToFileAsync(LoggingData log, string logName1, string logName2)
#pragma warning restore CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
        {
            Uri appUri = new(Path.GetDirectoryName(Core.StartupPath));
            Uri outUri = new(appUri, Environment.ExpandEnvironmentVariables(LoggingSettings.Folder));
            string logFolder = outUri.LocalPath;

            // フォルダー作成
            if (Directory.Exists(logFolder) == false)
            {
                Directory.CreateDirectory(logFolder);
            }

            if (LoggingSettings.LoggingMode == LoggingMode.Size)
            {
                // サイズチェック
                try
                {
                    FileInfo fi = new(logFolder + @"\" + logName1);
                    if (fi.Length > (long)LoggingSettings.MaxFileSizeKB * 1024)
                    {
                        // サイズオーバー
                        DebugWrite("]]>!! SIZE OVER !!");
                        try
                        {
                            File.Delete(logFolder + @"\" + logName2);
                        }
                        catch (FileNotFoundException)
                        {
                            // ファイルなし
                        }
                        File.Move(logFolder + @"\" + logName1, logFolder + @"\" + logName2);
                    }
                }
                catch (FileNotFoundException)
                {
                    // ファイルなし
                }
            }
            else
            {
                // ファイル日付チェック
                try
                {
                    System.DateTime fd = System.IO.File.GetLastWriteTime(logFolder + @"\" + logName1);
                    TimeSpan dtdiff = fd.Subtract(log.DateTime);
                    bool fileChear = false;
                    switch (LoggingSettings.LoggingMode)
                    {
                        case LoggingMode.Day:
                        case LoggingMode.Week:
                            // ログデータ日時とログファイル更新日時差が１日越で１周したとみなす
                            if (dtdiff.Days > 1) fileChear = true;
                            break;
                        case LoggingMode.Month:
                            // ログデータ日時とログファイル更新日時差が３１日越で１周したとみなす
                            if (dtdiff.Days > 31) fileChear = true;
                            break;
                    }
                    if (fileChear)
                    {
                        DebugWrite("]]>!! FILE CLEAR !!");
                        File.Delete(logFolder + @"\" + logName1);
                    }
                }
                catch (FileNotFoundException)
                {
                    // ファイルなし
                }
            }

            using (var fs = new FileStream(logFolder + logName1, FileMode.Append))
            {
                DebugWrite("]]>", fs.Name, lastFilename);

                // 書き込み
                using (var sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    foreach (var txt in log.ToStrings())
                    {
                        sw.WriteLine(txt);
                    }
                }

                lastFilename = fs.Name;
            }

            // タイムスタンプ変更
            if (Jaffa.DateTime.DifferenceNow.Ticks != 0)
            {
                File.SetLastWriteTime(lastFilename, Jaffa.DateTime.Now);
            }

            WriteTaskQueueCount--;
        }

        #endregion

        #endregion
    }
}
