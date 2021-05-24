using System;

namespace Jaffa.Diagnostics
{
    /// <summary>
    /// ログタイプ
    /// </summary>
    public enum LogType : byte
    {
        /// <summary>
        /// 情報メッセージ
        /// </summary>
        Information,
        /// <summary>
        /// 警告メッセージ
        /// </summary>
        Warning,
        /// <summary>
        /// エラーメッセージ
        /// </summary>
        Error
    }
}
