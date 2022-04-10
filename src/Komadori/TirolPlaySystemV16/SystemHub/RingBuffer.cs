using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace XControls
{
    /// <summary>
    /// 共有メモリ上にリングバッファを作成するためのクラスです。
    /// </summary>
    public partial class RingBuffer
    {
        #region 構築・破棄

        /// <summary>
        /// XControls.XRingBuffer クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="xsmem">リングバッファ格納用の XSharedMemory クラスインスタンス</param>
        public RingBuffer(SharedMemory xsmem)
        {
            this.xsmem = xsmem;
        }

        #endregion

        #region 変数

        /// <summary>
        /// 共有メモリでやり取りするリングバッファデータです。
        /// </summary>
        private BufferDataBag databuf = new BufferDataBag();

        /// <summary>
        /// リングバッファ格納用の XSharedMemory クラスインスタンスです。
        /// </summary>
        private SharedMemory xsmem = null;

        #endregion

        #region プロパティ

        #region IsShutDownプロパティ

        /// <summary>
        /// シャットダウン中かどうかを取得または設定します。true にするとバッファ書き込みを無視します。
        /// </summary>
        public bool IsShutDown
        {
            set
            {
                LoadSMem();
                databuf.IsShutDown = value;
                SaveSMem();
            }
            get
            {
                LoadSMem();
                return databuf.IsShutDown;
            }
        }

        #endregion

        #region IsUpdateプロパティ

        /// <summary>
        /// リングバッファの内容が更新されたかどうかを取得または設定します。
        /// </summary>
        public bool IsUpdate
        {
            set
            {
                LoadSMem();
                databuf.IsUpdate = value;
                SaveSMem();
            }
            get
            {
                LoadSMem();
                return databuf.IsUpdate;
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
            bool rt = false;

            LoadSMem();
            rt = databuf.SetObject(obj);
            SaveSMem();

            return rt;
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
            object obj = null;

            LoadSMem();
            obj = databuf.GetObject(out rptr);

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
            LoadSMem();
            return databuf.ToString();
        }

        #endregion

        #endregion

        #region 内部処理

        #region オブジェクト読み込み (LoadSMem)

        /// <summary>
        /// オブジェクトを共有メモリから読み込みます。
        /// </summary>
        public void LoadSMem()
        {
            try
            {
                databuf = xsmem.GetObject(0) as BufferDataBag;
            }
            catch
            {
            }
        }

        #endregion

        #region オブジェクト書き込み (SaveSMem)

        /// <summary>
        /// オブジェクトを共有メモリに書き出します。
        /// </summary>
        public void SaveSMem()
        {
            try
            {
                xsmem.PutObject(databuf, 0);
            }
            catch
            {
            }
        }

        #endregion

        #endregion
    }
}
