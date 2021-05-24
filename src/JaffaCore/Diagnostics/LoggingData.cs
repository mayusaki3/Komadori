using System;
using System.Collections.Generic;
using System.Text;

namespace Jaffa.Diagnostics
{
    /// <summary>
    /// ロギングデータクラス
    /// </summary>
    public class LoggingData : Object
    {
        #region コンストラクター

        /// <summary>
        /// ロギングデータを初期化します。
        /// </summary>
        /// <param name="dateTime">発生日時</param>
        /// <param name="logType">ログタイプ</param>
        /// <param name="messages">メッセージリスト</param>
        public LoggingData(System.DateTime dateTime, LogType logType, List<string> messages)
        {
            this.DateTime = dateTime;
            this.LogType = logType;
            this.Messages = messages.ToArray();
        }

        #endregion

        #region プロパティ

        #region 発生時刻を参照 ([R] DateTime)

        /// <summary>
        /// 発生時刻を参照します。
        /// </summary>
        public System.DateTime DateTime { get; }

        #endregion

        #region ログタイプを参照 ([R] LogType)

        /// <summary>
        /// ログタイプを参照します。
        /// </summary>
        public LogType LogType { get; }

        #endregion

        #region ロギングメッセージリストを参照 ([R] Messages)

        /// <summary>
        /// ロギングメッセージリストを参照します。
        /// </summary>
        public string[] Messages { get; }

        #endregion

        #endregion

        #region メソッド

        #region 発生日時付きメッセージ配列を参照 (ToStrings)

        /// <summary>
        /// 発生日時付きでメッセージ配列を参照します。
        /// </summary>
        /// <returns>メッセージ配列</returns>
        public string[] ToStrings()
        {
            List<string> rt = new();
            foreach (string msg in this.Messages)
            {
                rt.Add(ToString(msg));
            }
            return rt.ToArray();
        }

        #endregion

        #region メッセージ配列を参照 (ToShortStrings)

        /// <summary>
        /// メッセージ配列を参照します。
        /// </summary>
        /// <returns>メッセージ配列</returns>
        public string[] ToShortStrings()
        {
            List<string> rt = new();
            foreach (string msg in this.Messages)
            {
                rt.Add(ToShortString(msg));
            }
            return rt.ToArray();
        }

        #endregion

        #region 発生日時付きメッセージリストを参照 (ToString) [private]

        /// <summary>
        /// 発生日時付きでメッセージを参照します。
        /// </summary>
        /// <returns>メッセージ</returns>
        private string ToString(string message)
        {
            StringBuilder sb = new(ToShortString(message));
            sb.Insert(0, this.DateTime.ToString("yyyyMMddHHmmssfff|"));
            return sb.ToString();
        }

        #endregion

        #region タイプ付きメッセージを編集 (ToShortString) [private]

        /// <summary>
        /// メッセージを編集します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <returns>編集したメッセージ</returns>
        private string ToShortString(string message)
        {
            StringBuilder sb = new();
            switch (this.LogType)
            {
                case LogType.Information:
                    sb.Append('I');
                    break;
                case LogType.Warning:
                    sb.Append('W');
                    break;
                case LogType.Error:
                    sb.Append('E');
                    break;
            }
            sb.Append('|');
            sb.Append(message);
            return sb.ToString();
        }

        #endregion

        #endregion
    }
}
