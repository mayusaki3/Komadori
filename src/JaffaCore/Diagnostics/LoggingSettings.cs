using System;

namespace Jaffa.Diagnostics
{
    /// <summary>
    /// ロギング設定クラス
    /// </summary>
    public class LoggingSettings : Object
    {
        #region プロパティ

        #region ロギングモードを参照または設定 ([R/W] LoggingMode)

        /// <summary>
        /// ロギングモードを参照または設定します。
        /// </summary>
        public static LoggingMode LoggingMode { get; set; } = LoggingMode.None;

        #endregion

        #region ログ出力先フォルダを参照または設定 ([R/W] Folder)

        /// <summary>
        /// ログ出力先フォルダを参照または設定します。
        /// </summary>
        public static string Folder
        {
            get
            {
                return folder;
            }
            set
            {
                folder = value;
                if (folder.Length > 0)
                {
                    if (!folder.Substring(folder.Length - 1, 1).Equals(@"\"))
                    {
                        folder += @"\";
                    }
                }
            }
        }
        private static string folder = @"Logs\";

        #endregion

        #region ログファイル名を参照または設定 ([R/W] FileName)

        /// <summary>
        /// ログファイル名を参照または設定します。
        /// ファイル名の'[@]'の部分はLoggingModeに合わせて変化し、
        /// Sizeの場合は1～2, 
        /// Dayの場合は1～31, 
        /// Weekの場合は1～7,
        /// Monthの場合は1～12の値を取ります。
        /// </summary>
        public static string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
                fileName = fileName.Replace(@"\", "");
                if (fileName.LastIndexOf(".") < 1)
                {
                    fileName += ".txt";
                }
                if (fileName.LastIndexOf("[@]") < 1)
                {
                    int pos = fileName.LastIndexOf(".");
                    fileName = fileName.Substring(0, pos) +
                        "[@]" +
                        fileName[pos..];
                }
            }
        }
        private static string fileName = "syslog[@].txt";

        #endregion

        #region ログファイルサイズ上限をKByte単位で参照または設定 ([R/W] MaxFileSizeKB)

        /// <summary>
        /// ログファイルサイズ上限を参照または設定します。
        /// 2～32767の範囲(単位KByte)で指定し、範囲外の場合は自動調整します。
        /// LoggingModeがSizeの場合に有効で、サイズを超えると1世代のみバックアップを作成します。
        /// </summary>
        public static int MaxFileSizeKB
        {
            get
            {
                return maxFileSizeKB;
            }
            set
            {
                if (value < 2)
                {
                    maxFileSizeKB = 2;
                }
                else if (value > 32767)
                {
                    maxFileSizeKB = 32767;
                }
                else
                {
                    maxFileSizeKB = value;
                }
            }
        }
        private static int maxFileSizeKB = 2048;

        #endregion

        #region フレームワークメッセージのログ出力が有効かどうかを参照または設定 ([R/W] FrameworkMessage)

        /// <summary>
        /// フレームワークログ出力が有効かどうかを参照または設定します。
        /// </summary>
        public static bool FrameworkMessage { get; set; } = false;

        #endregion

        #endregion
    }
}
