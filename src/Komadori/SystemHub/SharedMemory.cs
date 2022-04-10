using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace XControls
{
    /// <summary>
    /// プロセス間共有メモリを操作するためのクラスです。
    /// </summary>
    /// <remarks>
    /// 共有メモリのメモリマップは次の通りです。
    /// Addr byte数 説明
    /// ---- ------ ------------------------------------------
    /// 0000    3   固定文字列'XSM'   --+
    /// 0003    1   参照数(0-255)       | 共有メモリ・ヘッダ部
    /// 0004    2   共有メモリサイズ    |
    /// 0005    1   更新カウンタ      --+
    /// 0007    n   ユーザーデータ
    ///   :     :     :
    ///   :     :     :
    /// </remarks>
    public partial class SharedMemory
    {
        #region 構築・破棄

        /// <summary>
        /// XControls.XSharedMemory クラスの新しいインスタンスを初期化します。
        /// </summary>
        public SharedMemory()
        {
            memInfo = new MemoryInfo(memorySize);
        }

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected void Dispose(bool disposing)
        {
            #region 後処理

            if (!hMap.Equals(IntPtr.Zero))
            {
                // オープン中ならクローズ
                Close();
            }

            #endregion
        }

        #endregion

        #region 定数

        /// <summary>
        /// 共有メモリ・ヘッダ部の長さ(byte)です。
        /// </summary>
        const int SMEM_HEADER_LENGTH = 7;

        /// <summary>
        /// プロセス間排他で使用するミューテックス名です。
        /// </summary>
        const string MUTEX_NAME = "XSharedMemory_Mutex";

        #endregion

        #region 変数

        /// <summary>
        /// 無効なハンドルを示す値です。
        /// </summary>
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        // ヘッダ文字列
        /// <summary>
        /// 共有メモリのヘッダ識別文字列('XSM')です。
        /// </summary>
        private byte[] SMEM_HEADER = { 0x58, 0x53, 0x4D };
                                    
        /// <summary>
        /// 共有メモリへのハンドルです。
        /// </summary>
        private IntPtr hMap = IntPtr.Zero;

        /// <summary>
        /// 共有メモリのアドレスです。
        /// </summary>
        private IntPtr address = IntPtr.Zero;

        /// <summary>
        /// 共有メモリの情報です。
        /// </summary>
        private MemoryInfo memInfo = null;

        // Close時のLoadCount
        /// <summary>
        /// 共有メモリクローズ時のLoadCountです。
        /// </summary>
        private byte lastLoadCount = 0;

        /// <summary>
        /// 共有メモリ更新通知スレッドです。
        /// </summary>
        private Thread memoryCheckthread = null;

        #endregion

        #region プロパティ

        #region SharedNameプロパティ

        private String sharedName = "XSMEMORY";
        /// <summary>
        /// 共有メモリに付ける名前を取得または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue("XSMEMORY"),
            Description("共有メモリに付ける名前を取得または設定します。")
        ]
        public String SharedName
        {
            get
            {
                return sharedName;
            }
            set
            {
                if (hMap.Equals(IntPtr.Zero) == false)
                {
                    throw new ArgumentException("共有メモリオープン中は変更できません。");
                }
                if (value.Length == 0)
                {
                    throw new ArgumentException("SharedNameは省略できません。");
                }
                sharedName = value;
            }
        }

        #endregion

        #region MemorySizeプロパティ

        private Int32 memorySize = 256;
        /// <summary>
        /// 共有メモリのサイズを参照または1～65535の範囲で設定します。実際の共有メモリサイズは、設定したサイズに管理情報のサイズ(7byte)を追加したものです。
        /// </summary>
        [
            Category("共有"),
            DefaultValue(256),
            Description("共有メモリのサイズを参照または1～65535の範囲で設定します。実際の共有メモリサイズは、設定したサイズに管理情報のサイズ(7byte)を追加したものです。")
        ]
        public Int32 MemorySize
        {
            get
            {
                return memorySize;
            }
            set
            {
                if (hMap.Equals(IntPtr.Zero) == false)
                {
                    throw new ArgumentException("共有メモリオープン中は変更できません。");
                }
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("MemorySize", "指定できる共有メモリサイズの下限を超えています。");
                }
                if (value > UInt16.MaxValue)
                {
                    throw new ArgumentOutOfRangeException("MemorySize", "指定できる共有メモリサイズの上限を超えています。");
                }
                Int32 ps = memorySize;
                memorySize = value;
                if(ps != memorySize)
                {
                    memInfo = new MemoryInfo(memorySize);
                }
            }
        }

        #endregion

        #region DumpFilenameプロパティ

        private String dumpFilename = "";
        /// <summary>
        /// BinarySaveメソッドで最後に保存したファイル名を取得します。
        /// </summary>
        [
            Category("動作"),
            Description("BinarySaveメソッドで最後に保存したファイル名を取得します。")
        ]
        public String DumpFilename
        {
            get
            {
                return dumpFilename;
            }
        }

        #endregion

        #endregion

        #region メソッド

        #region 共有メモリオープン (Open)

        /// <summary>
        /// プロパティに設定された条件で共有メモリをオープンします。
        /// </summary>
        public void Open()
        {
            if (hMap.Equals(IntPtr.Zero) == false)
            {
                throw new IOException("既に共有メモリをオープンしています。");
            }

            // 共有メモリオープン
            hMap = CreateFileMapping(INVALID_HANDLE_VALUE, IntPtr.Zero,
                                        ProtectAttributes.PAGE_READWRITE | ProtectAttributes.SEC_COMMIT,
                                        0, SMEM_HEADER_LENGTH + memorySize, sharedName);
            if(hMap.Equals(IntPtr.Zero))
            {
                uint err = GetLastError();
                throw new IOException("共有メモリのオープンに失敗しました。\r\nエラーコード=" +
                                        err.ToString() + "\r\n" + GetFormatMessage(err));
            }

            // マッピングの設定
            address = MapViewOfFile(hMap, DesiredAccess.FILE_MAP_ALL_ACCESS, 0, 0, 0);
            if(address.Equals(IntPtr.Zero))
            {
                uint err = GetLastError();
                string msg = "";
                if (CloseHandle(hMap) == false)
                {
                    uint cerr = GetLastError();
                    msg = "\r\n共有メモリのクローズに失敗しました。\r\nエラーコード=" +
                                            cerr.ToString() + "\r\n" + GetFormatMessage(cerr);
                }
                hMap = IntPtr.Zero;
                throw new IOException("共有メモリのマッピングに失敗しました。\r\nエラーコード=" +
                                        err.ToString() + "\r\n" + GetFormatMessage(err) + msg);
            }

            // ヘッダチェック
            byte[] buf = new byte[SMEM_HEADER_LENGTH];
            int i = 0;
            getBytes(ref buf, 0, 0, SMEM_HEADER_LENGTH);
            for (i = 0; i < 3; i++)
            {
                if (buf[i] != SMEM_HEADER[i])
                {
                    // ヘッダおよび参照カウント初期化
                    buf[i] = SMEM_HEADER[i];
                    buf[3] = 0;
                }
            }

            // 共有メモリ長
            if (buf[3] == 0)
            {
                // 設定
                buf[4] = (byte)(memorySize / 0x100);
                buf[5] = (byte)(memorySize & 0xff);
                buf[6] = 0;
            }
            else
            {
                // チェック
                if (memorySize!=buf[4] * 0x100 + buf[5])
                {
                    CloseFnc(false);
                    throw new IOException("共有メモリサイズが一致しないためクローズしました。");
                }
            }

            // 参照数オーバーチェック
            if (buf[3] == 255)
            {
                CloseFnc(false);
                throw new IOException("規定の参照カウント数に達したためクローズしました。");
            }

            // ヘッダ更新
            buf[3]++;
            putBytes(buf, 0, 0, SMEM_HEADER_LENGTH);

            // 更新カウント初期化
            lastUpdCount = GetUpdCount();
            if (lastUpdCount == 0)
            {
                lastUpdCount = 255;
            }
            else
            {
                lastUpdCount--;
            }

            // 通知スレッド起動
            isMemoryCheckthreadReady = false;
            memoryCheckthread = new Thread(new ThreadStart(MemoryCheckthread));
            memoryCheckthread.Start();
            while (!isMemoryCheckthreadReady)
            {
                Thread.Sleep(50);
            }
        }

        #endregion

        #region 共有メモリクローズ (Close)

        /// <summary>
        /// 共有メモリをクローズします。
        /// </summary>
        public void Close()
        {
            CloseFnc(true);
        }
        private void CloseFnc(bool dec)
        {
            if (hMap.Equals(IntPtr.Zero))
            {
                throw new IOException("共有メモリをオープンしていません。");
            }

            // 通知スレッド停止
            memoryCheckthread.Abort();

            // マッピングの開放
            if (address.Equals(IntPtr.Zero) == false)
            {
                if (dec)
                {
                    // ヘッダ更新
                    byte[] buf = new byte[1];
                    getBytes(ref buf, 3, 0, 1);
                    if (buf[0] > 0)
                    {
                        buf[0]--;
                    }
                    lastLoadCount = buf[0];
                    putBytes(buf, 3, 0, 1);
                }

                if (FlushViewOfFile(address, 0) == false)
                {
                    uint err = GetLastError();
                    throw new IOException("共有メモリへのフラッシュに失敗しました。\r\nエラーコード=" +
                                            err.ToString() + "\r\n" + GetFormatMessage(err));
                }

                if (UnmapViewOfFile(address) == false)
                {
                    uint err = GetLastError();
                    throw new IOException("共有メモリのマッピングの開放に失敗しました。\r\nエラーコード=" +
                                            err.ToString() + "\r\n" + GetFormatMessage(err));
                }
                address = IntPtr.Zero;
            }

            // 共有メモリクローズ
            if (CloseHandle(hMap) == false)
            {
                uint err = GetLastError();
                throw new IOException("共有メモリのクローズに失敗しました。\r\nエラーコード=" +
                                        err.ToString() + "\r\n" + GetFormatMessage(err));
            }
            hMap = IntPtr.Zero;
        }

        #endregion

        #region 共有メモリハンドル取得 (GetHandle)

        /// <summary>
        /// 共有メモリへのハンドルを返します。
        /// </summary>
        /// <returns></returns>
        public IntPtr GetHandle()
        {
            return hMap;
        }

        #endregion

        #region 共有メモリの参照数取得 (GetLoadCount)

        /// <summary>
        /// 共有メモリの参照数を返します。
        /// 共有メモリをオープンしていない場合は、最後にクローズした時の参照数を返します。
        /// </summary>
        /// <returns>参照数</returns>
        public byte GetLoadCount()
        {
            byte[] buf = new byte[1];
            buf[0] = 0;
            if (hMap.Equals(IntPtr.Zero) == false)
            {
                getBytes(ref buf, 3, 0, 1);
            }
            else
            {
                buf[0] = lastLoadCount;
            }
            return buf[0];
        }

        #endregion

        #region 共有メモリ更新チェック (IsUpdate)

        private byte lastUpdCount = 0;
        /// <summary>
        /// 共有メモリ上のデータが更新されているかを調べます。
        /// </summary>
        /// <returns>結果(true=更新あり, false=更新なし)</returns>
        public bool IsUpdate()
        {
            try
            {
                return lastUpdCount != GetUpdCount();
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 共有メモリ書き込み (PutBytes)

        /// <summary>
        /// 共有メモリにbyte配列を書き込みます。
        /// </summary>
        /// <param name="buf">書き込むbyte配列</param>
        /// <param name="addr">書き込み先共有メモリの 0 から始まるアドレス</param>
        /// <param name="start">書き込み元のbyte配列内の 0 から始まるインデックス</param>
        /// <param name="length">書き込む配列要素の数</param>
        public void PutBytes(byte[] buf, int addr, int start, int length)
        {
            // 排他 - 最大1秒待つ
            Mutex mutex = new Mutex(false, MUTEX_NAME + "_" + sharedName);
            mutex.WaitOne(1000, false);
            try
            {
                // ユーザーエリアに書き込み
                putBytes(buf, addr + SMEM_HEADER_LENGTH, start, length);
                UpdCountup();
                if (FlushViewOfFile(address, 0) == false)
                {
                    uint err = GetLastError();
                    throw new IOException("共有メモリへのフラッシュに失敗しました。\r\nエラーコード=" +
                                            err.ToString() + "\r\n" + GetFormatMessage(err));
                }
            }
            catch (Exception es)
            {
                throw es;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        #endregion

        #region 共有メモリ読み込み (GetBytes)

        /// <summary>
        /// 共有メモリからbyte列を読み込みます。
        /// </summary>
        /// <param name="buf">読み込むbyte配列</param>
        /// <param name="addr">読み込み元共有メモリの 0 から始まるアドレス</param>
        /// <param name="start">読み込み先のbyte配列内の 0 から始まるインデックス</param>
        /// <param name="length">読み込む配列要素の数</param>
        public void GetBytes(ref byte[] buf, int addr, int start, int length)
        {
            // ユーザーエリアから読み込み
            getBytes(ref buf, addr + SMEM_HEADER_LENGTH, start, length);

            lastUpdCount = GetUpdCount();
        }

        #endregion

        #region 共有メモリ書き込み (PutObject)

        /// <summary>
        /// 共有メモリにオブジェクトを書き込みます。
        /// </summary>
        /// <param name="obj">書き込むシリアライズ可能なオブジェクト</param>
        /// <param name="addr">書き込み先共有メモリの 0 から始まるアドレス</param>
        public void PutObject(object obj, int addr)
        {
            // 排他 - 最大1秒待つ
            Mutex mutex = new Mutex(false, MUTEX_NAME + "_" + sharedName);
            mutex.WaitOne(1000, false);
            try
            {
                SharedObjectBag bag = new SharedObjectBag();
                bag.Object = obj;
                MemoryStream ms = new MemoryStream();
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, bag);
                byte[] buf = ms.ToArray();
                byte[] siz = new byte[2];
                int wsiz = buf.Length;
                siz[0] = (byte)(wsiz / 0x100);
                siz[1] = (byte)(wsiz & 0xff);
                putBytes(siz, addr + SMEM_HEADER_LENGTH, 0, 2);
                putBytes(buf, addr + SMEM_HEADER_LENGTH + 2, 0, wsiz);
            
                UpdCountup();
            }
            catch (Exception es)
            {
                throw es;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        #endregion

        #region 共有メモリ読み込み (GetObject)

        /// <summary>
        /// 共有メモリからオブジェクトを読み込みます。
        /// </summary>
        /// <param name="addr">読み込み元共有メモリの 0 から始まるアドレス</param>
        /// <returns>読み込むオブジェクト</returns>
        public object GetObject(int addr)
        {
            byte[] siz = new byte[2];
            getBytes(ref siz, addr + SMEM_HEADER_LENGTH, 0, 2);
            int wsiz = siz[0] * 0x100 + siz[1];
            byte[] buf = new byte[wsiz];
            getBytes(ref buf, addr + SMEM_HEADER_LENGTH + 2, 0, wsiz);
            MemoryStream ms = new MemoryStream(buf);
            IFormatter formatter = new BinaryFormatter();
            SharedObjectBag bag = (SharedObjectBag)formatter.Deserialize(ms);
            
            lastUpdCount = GetUpdCount();
            
            return bag.Object;
        }

        #endregion

        #region 共有メモリダンプ (MemoryDump)

        /// <summary>
        /// 共有メモリの内容を{SharedName}[PROCESS-ID].DATファイルに保存します。(障害調査用)
        /// </summary>
        public void MemoryDump()
        {
            int length = SMEM_HEADER_LENGTH + memorySize;
            byte[] buf = new byte[length];
            getBytes(ref buf, 0, 0, length);
            BinarySave(buf);
        }

        #endregion

        #endregion

        #region イベント

        #region 共有メモリ更新通知 (MemoryUpdateEvent)

        public delegate void MemoryUpdateEventHandler(object sender, EventArgs e);
        [
            Category("動作"),
            Description("共有メモリが更新されたことを通知するイベントです。")
        ]
        public event MemoryUpdateEventHandler MemoryUpdateEvent;

        #endregion

        #endregion

        #region その他

        #region システムエラーメッセージ取得 (GetFormatMessage)

        /// <summary>
        /// システムエラーメッセージ取得
        /// </summary>
        /// <param name="err">エラー番号</param>
        /// <returns>メッセージ内容</returns>
        private string GetFormatMessage(uint err)
        {
            StringBuilder buffer = new StringBuilder(256);
            FormatMessage(FormatMessageOptions.FORMAT_MESSAGE_FROM_SYSTEM,
                            IntPtr.Zero, (int)err, 0, buffer, 256, IntPtr.Zero);
            return buffer.ToString();
        }

        #endregion

        #region 共有メモリ書込(全共有メモリ用) (putBytes)

        /// <summary>
        /// 共有メモリにbyte列を書き込む。
        /// </summary>
        /// <param name="buf">byte配列</param>
        /// <param name="ofs">書き込み先共有メモリの 0 から始まるアドレス</param>
        /// <param name="start">書き込み元のbyte配列内の 0 から始まるインデックス</param>
        /// <param name="length">書き込む配列要素の数</param>
        private void putBytes(byte[] buf, int ofs, int start, int length)
        {
            // 共有メモリ上、境界を超えないかのみをチェック
            if (SMEM_HEADER_LENGTH + memorySize < ofs + length)
            {
                int siz = (ofs + length) - (SMEM_HEADER_LENGTH + memorySize);
                throw new IOException("アドレスおよび長さの組み合わせがMemorySizeプロパティの指定を " + siz + " バイト超えています。");
            }

            // 書き込み
            IntPtr ofsadr = new IntPtr(address.ToInt64() + ofs);
            try
            {
                Marshal.Copy(buf, start, ofsadr, length);
            }
            catch (Exception es)
            {
                throw es;
            }
        }

        #endregion

        #region 共有メモリ読込(全共有メモリ用) (getBytes)

        /// <summary>
        /// 共有メモリからbyte列を読み込みます。
        /// </summary>
        /// <param name="buf">読み込むbyte配列</param>
        /// <param name="ofs">読み込み元共有メモリの 0 から始まるアドレス</param>
        /// <param name="start">読み込み先のbyte配列内の 0 から始まるインデックス</param>
        /// <param name="length">読み込む配列要素の数</param>
        private void getBytes(ref byte[] buf, int ofs, int start, int length)
        {
            // 共有メモリ上、境界を超えないかのみをチェック
            if (SMEM_HEADER_LENGTH + memorySize < ofs + length)
            {
                int siz = (ofs + length) - (SMEM_HEADER_LENGTH + memorySize);
                throw new IOException("アドレスが不正、またはアドレスおよび長さの組み合わせがMemorySizeプロパティの指定を " + siz + " バイト超えています。");
            }

            // 読み込み
            IntPtr ofsadr = new IntPtr(address.ToInt64() + ofs);
            buf = new byte[length];
            try
            {
                Marshal.Copy(ofsadr, buf, start, length);
            }
            catch (Exception es)
            {
                throw es;
            }
        }

        #endregion

        #region バイナリ保存(メモリダンプ) (BinarySave)

        /// <summary>
        /// メモリの内容をSMEMORY[PROCESS-ID].DATファイルに保存する。(障害調査用)
        /// </summary>
        /// <param name="mem"></param>
        public void BinarySave(byte[] mem)
        {
            FileStream fs = null;
            try
            {
                dumpFilename = Application.StartupPath + "\\" + SharedName + "[" +
                                    Process.GetCurrentProcess().Id.ToString("d5") + "].DAT";
                dumpFilename = dumpFilename.Replace("C:\\Program Files", "C:\\ProgramData");
                string folder = dumpFilename.Substring(0, dumpFilename.LastIndexOf("\\"));
                Directory.CreateDirectory(folder);
                fs = new FileStream(dumpFilename, FileMode.Create, FileAccess.Write);
                for (int i = 0; i < mem.Length; i++)
                {
                    fs.WriteByte(mem[i]);
                }
            }
            catch
            {
            }
            finally
            {
                try
                {
                    fs.Close();
                }
                catch
                {
                }
            }
        }

        #endregion

        #region 更新カウント取得 (GetUpdCount)

        /// <summary>
        /// 更新カウントを取得します。
        /// </summary>
        private byte GetUpdCount()
        {
            byte[] upd = new byte[1];
            getBytes(ref upd, 6, 0, 1);
            return upd[0];
        }

        #endregion

        #region 更新カウントUP (UpdCountup)

        /// <summary>
        /// 更新カウントを更新します。
        /// </summary>
        private void UpdCountup()
        {
            byte[] upd = new byte[1];
            getBytes(ref upd, 6, 0, 1);
            if(upd[0]==255)
            {
                upd[0] = 0;
            }
            else
            {
                upd[0]++;
            }
            putBytes(upd, 6, 0, 1);
        }

        #endregion

        #region 更新通知スレッド (MemoryCheckthread)

        private bool isMemoryCheckthreadReady = false;
        /// <summary>
        /// メモリ更新をチェックして通知するスレッドです。
        /// </summary>
        private void MemoryCheckthread()
        {
            Debug.Print("メモリ更新通知スレッド開始しました");
            byte lastCount = GetUpdCount();
            byte checkCount = lastCount;
            try
            {
                isMemoryCheckthreadReady = true;
                while (true)
                {
                    // 受信ループ(Abort例外で終了)

                    checkCount = GetUpdCount();
                    if (lastCount != checkCount)
                    {
                        // メモリ更新通知イベント
                        EventArgs eArgs = new EventArgs();
                        if (MemoryUpdateEvent != null)
                        {
                            // ハンドラが設定されていたらイベント発生
                            CallMemoryUpdateEvent(this, eArgs);
                        }
                        lastCount = checkCount;
                    }
                    Thread.Sleep(10);
                }
            }
            catch (ThreadAbortException)
            {
                // スレッド終了
                Debug.Print("メモリ更新通知スレッド停止しました");
            }
            catch (Exception es)
            {
                Debug.Print("メモリ更新通知スレッドで例外が発生しました - " + es.Message);
                throw es;
            }
        }

        #region データ受信通知

        delegate void CallMemoryUpdateEventDelegate(object sender, EventArgs e);

        private void CallMemoryUpdateEvent(object sender, EventArgs e)
        {
            try
            {
                MemoryUpdateEvent(sender, e);
            }
            catch (Exception es)
            {
                Debug.Print("MemoryUpdateEvent内で例外が発生しました - " + es.Message);
            }
        }

        #endregion

        #endregion

        #region WIN32API

        [DllImport("kernel32.dll")]
        extern private static UInt32 GetLastError();

        [Flags]
        private enum FormatMessageOptions : int
        {
            FORMAT_MESSAGE_ALLOCATE_BUFFER  = 0x0100,
            FORMAT_MESSAGE_IGNORE_INSERTS   = 0x0200,
            FORMAT_MESSAGE_FROM_STRING      = 0x0400,
            FORMAT_MESSAGE_FROM_HMODULE     = 0x0800,
            FORMAT_MESSAGE_FROM_SYSTEM      = 0x1000,
            FORMAT_MESSAGE_ARGUMENT_ARRAY   = 0x2000,
        }

        [DllImport("kernel32.dll")]
        extern private static int FormatMessage(
            FormatMessageOptions dwFlags,
            IntPtr lpSource,
            int dwMessageId,
            int dwLanguageId,
            StringBuilder lpBuffer,
            int nSize,
            IntPtr Arguments
        );

        [Flags]
        private enum ProtectAttributes : int
        {
            PAGE_READONLY   = 0x00000002,
            PAGE_READWRITE  = 0x00000004,
            PAGE_WRITECOPY  = 0x00000008,
            SEC_IMAGE       = 0x01000000,
            SEC_RESERVE     = 0x04000000,
            SEC_COMMIT      = 0x08000000,
            SEC_NOCACHE     = 0x10000000
        }
        [DllImport("kernel32.dll")]
        extern private static IntPtr CreateFileMapping(
            IntPtr hFile,
            IntPtr lpAttributes,
            ProtectAttributes flProtect,
            int dwMaximumSizeHigh,
            int dwMaximumSizeLow,
            string lpName 
        );

        [DllImport("kernel32.dll")]
        extern private static bool CloseHandle(IntPtr hObject);

        [Flags]
        private enum DesiredAccess : int
        {
            FILE_MAP_COPY       = 0x00000001,
            FILE_MAP_WRITE      = 0x00000002,
            FILE_MAP_READ       = 0x00000004,
            FILE_MAP_ALL_ACCESS = 0x000F001F
        }
        [DllImport("kernel32.dll")]
        extern private static IntPtr MapViewOfFile(
            IntPtr hFileMappingObject,
            DesiredAccess dwDesiredAccess,
            int dwFileOffsetHigh,
            int dwFileOffsetLow,
            int dwNumberOfBytesToMap
        );

        [DllImport("kernel32.dll")]
        extern private static bool FlushViewOfFile(
            IntPtr lpBaseAddress,
            int dwNumberOfBytesToFlush
        );

        [DllImport("kernel32.dll")]
        extern private static bool UnmapViewOfFile(IntPtr lpBaseAddress);

        #endregion

        #endregion
    }
}