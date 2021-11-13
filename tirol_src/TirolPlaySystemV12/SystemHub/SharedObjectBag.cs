using System;

namespace XControls
{
    public partial class SharedMemory
    {
        /// <summary>
        /// 共有メモリに配置するオブジェクトを格納します。
        /// </summary>
        [Serializable]
        public class SharedObjectBag
        {
            #region プロパティ

            #region SenderHostプロパティ

            private string senderHost = "";
            /// <summary>
            /// オブジェクト格納元のホスト名を取得します。
            /// </summary>
            public string SenderHost
            {
                get
                {
                    return senderHost;
                }
            }

            #endregion

            #region SenderAppプロパティ

            private string senderApp = "";
            /// <summary>
            /// オブジェクト格納元のアプリケーション名を取得します。
            /// </summary>
            public string SenderApp
            {
                get
                {
                    return senderApp;
                }
            }

            #endregion

            #region Objectプロパティ

            private Object obj = null;
            /// <summary>
            /// 共有メモリに配置するオブジェクトを取得または設定します。
            /// </summary>
            public Object Object
            {
                get
                {
                    return obj;
                }
                set
                {
                    obj = value;
                    senderHost = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
                    senderApp = System.Environment.CommandLine[0].ToString();
                }
            }

            #endregion

            #endregion
        }
    }
}
