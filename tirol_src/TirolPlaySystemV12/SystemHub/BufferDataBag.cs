using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XControls
{
    public partial class RingBuffer
    {
        /// <summary>
        /// リングバッファに格納するデータオブジェクトを格納します。
        /// </summary>
        [Serializable]
        public class BufferDataBag
        {
            #region 定数

            /// <summary>
            /// 最大バッファ数です。
            /// </summary>
            private const int MBUF_MAX = 256;

            #endregion

            #region 変数

            /// <summary>
            /// 次のバッファ書き込み位置を表すポインタです。
            /// </summary>
            private UInt16 w_ptr = 0;

            /// <summary>
            /// 次のバッファ読み込み位置を表すポインタです。
            /// </summary>
            private UInt16 r_ptr = 0;

            /// <summary>
            /// オブジェクトを格納するバッファです。
            /// </summary>
            private object[] m_buf = new string[MBUF_MAX];

            #endregion

            #region プロパティ

            #region IsShutDownプロパティ

            private bool isShutDown = false;
            /// <summary>
            /// シャットダウン中かどうかを取得または設定します。true にするとバッファ書き込みを無視します。
            /// </summary>
            public bool IsShutDown
            {
                get
                {
                    return isShutDown;
                }
                set
                {
                    isShutDown = value;
                }
            }

            #endregion

            #region IsUpdateプロパティ

            private bool isUpdate = false;
            /// <summary>
            /// リングバッファの内容が更新されたかどうかを取得または設定します。
            /// </summary>
            public bool IsUpdate
            {
                set
                {
                    isUpdate = value;
                }
                get
                {
                    return isUpdate;
                }
            }

            #endregion

            #endregion

            #region メソッド

            #region オブジェクト格納 (SetObject)

            /// <summary>
            /// オブジェクトをバッファに格納します。
            /// </summary>
            /// <param name="obj">格納するオブジェクト</param>
            /// <returns>結果(true=成功, false=バッファフル)</returns>
            public bool SetObject(object obj)
            {
                // シャットダウン中なら無視
                if (isShutDown)
                {
                    isUpdate = true;
                    return true;
                }

                // ポインタ計算
                UInt16 n_ptr = w_ptr;
                n_ptr++;
                if (n_ptr >= MBUF_MAX) n_ptr = 0;

                // バッファ確認
                if (n_ptr == r_ptr)
                {
                    // バッファFULL
                    isUpdate = true;
                    return false;
                }

                // バッファ格納
                m_buf[w_ptr] = obj;
                w_ptr = n_ptr;

                isUpdate = true;
                return true;
            }

            #endregion

            #region オブジェクト取り出し (GetObject)

            /// <summary>
            /// バッファからオブジェクトを取り出します。
            /// </summary>
            /// <returns>オブジェクト(null=オブジェクトなし)</returns>
            public object GetObject()
            {
                UInt16 rptr;
                return GetObject(out rptr);
            }

            /// <summary>
            /// バッファからオブジェクトを取り出します。
            /// </summary>
            /// <param name="rptr">シーケンス</param>
            /// <returns>オブジェクト(null=オブジェクトなし)</returns>
            public object GetObject(out UInt16 rptr)
            {
                rptr = r_ptr;

                // バッファ確認
                if (w_ptr == r_ptr)
                {
                    // バッファEMPTY
                    return null;
                }

                // ポインタ計算
                UInt16 n_ptr = r_ptr;
                n_ptr++;
                if (n_ptr >= MBUF_MAX) n_ptr = 0;

                // バッファ取り出し
                object obj = m_buf[r_ptr];
                m_buf[r_ptr] = "";
                r_ptr = n_ptr;

                isUpdate = true;
                return obj;
            }

            #endregion

            #region 内容をテキストで取得 (ToString)

            /// <summary>
            /// 内容をテキストで返します。
            /// </summary>
            /// <returns>内容</returns>
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("---- BufferData Dump ----");
                sb.AppendLine("[IsShutDown]=" + isShutDown);
                sb.AppendLine("[w_ptr]=" + w_ptr);
                sb.AppendLine("[r_ptr]=" + r_ptr);
                for (int i = 0; i < MBUF_MAX; i++)
                {
                    sb.AppendLine("[" + i.ToString() + "]=" + m_buf[i].ToString() + "<<");
                }
                return sb.ToString();
            }

            #endregion

            #endregion
        }
    }
}
