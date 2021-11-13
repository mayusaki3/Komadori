using System;

namespace XControls
{
    public partial class SharedMemory
    {
        /// <summary>
        /// 共有メモリの情報を扱うためのクラスです。
        /// このクラスのインスタンスは、共有メモリの先頭に配置されます。
        /// </summary>
        public class MemoryInfo
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.XSharedMemory.MemoryInfo クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="MemorySize">共有メモリサイズ(byte)</param>
            public MemoryInfo(Int32 memorySize)
            {
                if (memorySize > Int32.MaxValue)
                {
                    throw new ArgumentOutOfRangeException("memorySize", "指定できる共有メモリサイズの上限を超えています。");
                }
                this.memorySize = memorySize;
            }

            #endregion

            #region プロパティ

            #region LoadCountプロパティ

            private UInt16 loadCount = UInt16.MinValue;
            /// <summary>
            /// 共有メモリの参照カウントを取得します。
            /// </summary>
            public UInt16 LoadCount
            {
                get
                {
                    return loadCount;
                }
            }

            #endregion

            #region MemorySizeプロパティ

            private Int32 memorySize = Int32.MinValue;
            /// <summary>
            /// 共有メモリサイズ(byte)を取得します。
            /// </summary>
            public Int32 MemorySize
            {
                get
                {
                    return memorySize;
                }
            }

            #endregion

            #endregion

            #region メソッド

            #region 参照カウント+1 (LoadCountInc)

            /// <summary>
            /// 共有メモリの参照カウントを+1します。
            /// </summary>
            /// <returns>結果(true=成功, false=失敗)</returns>
            public bool LoadCountInc()
            {
                if (loadCount < UInt16.MaxValue)
                {
                    loadCount++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            #endregion

            #region 参照カウント-1 (LoadCountDec)

            /// <summary>
            /// 共有メモリの参照カウントを-1します。
            /// </summary>
            /// <returns>結果(true=成功, false=失敗)</returns>
            public bool LoadCountDec()
            {
                if (loadCount > UInt16.MinValue)
                {
                    loadCount--;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            #endregion

            #endregion
        }
    }
}
