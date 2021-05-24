using System;

namespace Jaffa.Diagnostics
{
    /// <summary>
    /// ログ出力イベント引数クラス
    /// </summary>
    public class LogWritingEventArgs : EventArgs
    {
        #region コンストラクター

        /// <summary>
        /// ログ出力イベント引数を初期化します。
        /// </summary>
        /// <param name="dateTime">発生日時</param>
        /// <param name="messages">メッセージリスト</param>
        public LogWritingEventArgs(System.DateTime dateTime, string[] messages)
        {
            this.DateTime = dateTime;
            this.Messages = messages;
        }

        #endregion

        #region プロパティ

        #region メッセージの発生日時を参照 ([R] DateTime)

        /// <summary>
        /// メッセージの発生日時を参照します。
        /// </summary>
        public System.DateTime DateTime { get; }

        #endregion

        #region メッセージリストを参照 ([R] Messages)

        /// <summary>
        /// メッセージリストを参照します。
        /// </summary>
        public string[] Messages { get; }

        #endregion

        #endregion
    }
}
