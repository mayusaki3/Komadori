using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace XControls
{
    /// <summary>
    /// UDPシリアル通信コントロール
    /// </summary>
    /// <remarks>
    /// 受信イベント内で送り元IPアドレスを取得できますが、あくまでイベント発生時に受信したデータの送り元です。
    /// 受信データは送り元に関係なくバッファに格納されるため、すべてがそのIPからの物とは限りません。
    /// 必要であれば、データ内に送り元の情報を格納してください。
    /// </remarks>
    [
        ComVisible(true), ClassInterface(ClassInterfaceType.None), Guid("5167E0A4-F974-40e0-A549-F1404723625F"),
        Description("LAN接続されたコンピュータ間でUDPによるシリアル通信を行います。"),
        ToolboxBitmap(typeof(XUDPComControl), "XUDPComControl.bmp")
    ]
    public partial class XUDPComControl : UserControl
    {
        #region インナークラス

        #region BackColorsクラス

        [TypeConverter(typeof(BackColorsConverter))]
        public class BackColors
        {
            public BackColors()
            {
                close = System.Drawing.Color.Gray;
                sendOff = System.Drawing.Color.Olive;
                reciveOff = System.Drawing.Color.Green;
                sendOn = System.Drawing.Color.Yellow;
                reciveOn = System.Drawing.Color.Lime;
                bufferFull = System.Drawing.Color.Red;
            }

            #region 変数

            private System.Drawing.Color close = new System.Drawing.Color();
            private System.Drawing.Color sendOff = new System.Drawing.Color();
            private System.Drawing.Color reciveOff = new System.Drawing.Color();
            private System.Drawing.Color sendOn = new System.Drawing.Color();
            private System.Drawing.Color reciveOn = new System.Drawing.Color();
            private System.Drawing.Color bufferFull = new System.Drawing.Color();

            #endregion

            #region プロパティ

            #region Closeプロパティ

            [
                Category("表示"),
                DefaultValue(typeof(Color), "Gray"),
                Description("回線切断中のステータス枠背景色です。")
            ]
            public Color Close
            {
                get
                {
                    return close;
                }
                set
                {
                    close = value;
                    ColorChangeNotice();
                }
            }

            #endregion

            #region SendOffプロパティ

            [
                Category("表示"),
                DefaultValue(typeof(Color), "Olive"),
                Description("データ非送信時のステータス枠背景色です。")
            ]
            public Color SendOff
            {
                get
                {
                    return sendOff;
                }
                set
                {
                    sendOff = value;
                    ColorChangeNotice();
                }
            }

            #endregion

            #region ReciveOffプロパティ

            [
                Category("表示"),
                DefaultValue(typeof(Color), "Green"),
                Description("データ受信待機中のステータス枠背景色です。")
            ]
            public Color ReciveOff
            {
                get
                {
                    return reciveOff;
                }
                set
                {
                    reciveOff = value;
                    ColorChangeNotice();
                }
            }

            #endregion

            #region SendOnプロパティ

            [
                Category("表示"),
                DefaultValue(typeof(Color), "Yellow"),
                Description("データ送信中のステータス枠背景色です。")
            ]
            public Color SendOn
            {
                get
                {
                    return sendOn;
                }
                set
                {
                    sendOn = value;
                    ColorChangeNotice();
                }
            }

            #endregion

            #region ReciveOnプロパティ

            [
                Category("表示"),
                DefaultValue(typeof(Color), "Lime"),
                Description("データ受信中のステータス枠背景色です。")
            ]
            public Color ReciveOn
            {
                get
                {
                    return reciveOn;
                }
                set
                {
                    reciveOn = value;
                    ColorChangeNotice();
                }
            }

            #endregion

            #region BufferFullプロパティ

            [
                Category("表示"),
                DefaultValue(typeof(Color), "Red"),
                Description("バッファフル時のステータス枠背景色です。")
            ]
            public Color BufferFull
            {
                get
                {
                    return bufferFull;
                }
                set
                {
                    bufferFull = value;
                    ColorChangeNotice();
                }
            }

            #endregion

            #endregion

            #region 追加のイベント

            /// <summary>
            /// 設定色変更イベント
            /// </summary>
            public class ColorChangeEventArgs : EventArgs
            {
                public Boolean Handled;
            }
            public delegate void ColorChangeEventHandler(object sender, ColorChangeEventArgs e);
            [
                Category("動作"),
                Description("設定色が変更されたことを通知するイベントです。")
            ]
            public event ColorChangeEventHandler ColorChange;
            private void ColorChangeNotice()
            {
                if (ColorChange != null)
                {
                    // ハンドラが設定されていたらイベント発生
                    ColorChangeEventArgs eArgs = new ColorChangeEventArgs();
                    eArgs.Handled = false;
                    ColorChange(this, eArgs);
                }
            }

            #endregion

            #region BackColorsConverterコンバータ

            internal class BackColorsConverter : ExpandableObjectConverter
            {
                // 変換可否の設定
                public override bool CanConvertFrom(ITypeDescriptorContext context, Type cvType)
                {
                    if (cvType == typeof(string))
                    {
                        // 文字列から変換不可
                        return false;
                    }
                    return base.CanConvertFrom(context, cvType);
                }
                public override bool CanConvertTo(ITypeDescriptorContext context, Type cvType) 
                {
                    if (cvType == typeof(string))
                    {
                        // 文字列への変換可
                        return true;
                    }
                    return base.CanConvertTo(context, cvType);
                }
                
                // 変換処理
                public override object ConvertFrom(ITypeDescriptorContext context,
                                                   CultureInfo culture, object value)
                {
                    if (value is string)
                    {
                        // Textのみの変換不可
                        return null;
                    }
                    return base.ConvertFrom(context, culture, value);
                }
                public override object ConvertTo(ITypeDescriptorContext context,
                                                 CultureInfo cultInfo, object value,
                                                 Type destType)
                {
                    if (destType == typeof(string))
                    {
                        // 空の内容を返す
                        return "";
                    }
                    return base.ConvertTo(context, cultInfo, value, destType);
                }
            }

            #endregion
        }

        #endregion

        #endregion

        public XUDPComControl()
        {
            InitializeComponent();
            ControlRelocation();
            backColors.ColorChange += new BackColors.ColorChangeEventHandler(backColors_ColorChange);
        }

        #region 定数

        // ライセンスキー用GUID {5470CF73-D558-40f8-8412-B0F8A8EBF09F}
        // このGUIDをLocalLicenseKeyプロパティに設定してください。
        Guid LOCAL_LICENSE_KEY = new Guid("5470CF73-D558-40f8-8412-B0F8A8EBF09F");

        // コントロール再配置用情報
        const int CTRL_LABEL_MINIMUM = 12;          // ラベルの最小高さと幅
        const int CTRL_LABEL_SPC = 4;               // ラベルと周囲の間隔

        #endregion

        #region 変数
        
        // ステータス背景色
        private BackColors backColors = new BackColors();

        // UDPクライアント
        private UdpClient udp = null;

        // UDP受信スレッド
        private Thread rcvthread = null;

        // 作業用受信バッファ
        private byte[] reciveBuffer = null;

        // workBuffer に受信済みのサイズ
        private int reciveBufferDataLength = 0;

        // Mutex
        private static Mutex mutex = new Mutex(false);

        #endregion

        #region プロパティ

        #region 追加のプロパティ

        #region LocalLicenseKeyプロパティ

        private String locallicenseKey = "";
        private Boolean useLocalLicense = false;
        [
            Category("ライセンス"),
            DefaultValue(""),
            Description("XComControl利用のためのライセンス情報を設定します。")
        ]
        public String LocalLicenseKey
        {
            get
            {
                return locallicenseKey;
            }
            set
            {
                locallicenseKey = value;
                useLocalLicense = (locallicenseKey.ToUpper().Equals("{" + LOCAL_LICENSE_KEY.ToString().ToUpper() + "}"));
            }
        }

        #endregion

        #region XConsoleBoxプロパティ

        [
            Category("動作"),
            DefaultValue(null),
            Description("動作ログを出力するXConsoleBoxのインスタンスを示します。")
        ]
        public XConsoleBox XConsoleBox
        {
            get
            {
                return xConsoleBox;
            }
            set
            {
                xConsoleBox = value;
            }
        }

        #endregion

        #region PortNoプロパティ

		private ushort portNo = (ushort)1024;
        [
            Category("動作"),
            DefaultValue((ushort)1024),
            Description("使用するポート番号を設定します。")
        ]
        public ushort PortNo
        {
            get
            {
                return portNo;
            }
            set
            {
                try
                {
                    portNo = value;
                }
                catch (Exception es)
                {
                    throw es;
                }
            }
        }

        #endregion

        #region ReciveBufferSizeプロパティ

        private int reciveBufferSize = 4096;
        [
            Category("動作"),
            DefaultValue(4096),
            Description("受信用バッファのサイズを取得または設定します。")
        ]
        public int ReciveBufferSize
        {
            get
            {
                return reciveBufferSize;
            }
            set
            {
                if (value < 1024 || value > 32767)
                {
                    throw new ArgumentOutOfRangeException(
                                "ReciveBufferSize",
                                value,
                                "1024～32767の範囲で設定してください");
                }
                else
                {
                    reciveBufferSize = value;
                }
            }
        }

        #endregion

        #region SendToAddress

        private string sendToAddress = "";
        private IPAddress sendToIP = null;
        [
            Category("動作"),
            DefaultValue(""),
            Description("送信先のコンピュータを設定します。")
        ]
        public string SendToAddress
        {
            get
            {
                return sendToAddress;
            }
            set
            {
                if (value.Length == 0)
                {
                    sendToIP = null;
                    sendToAddress = "";
                }
                else
                {
                    bool ipok = false;
                    try
                    {
                        IPAddress tmp;
                        if (IPAddress.TryParse(value, out tmp))
                        {
                            sendToIP = tmp;
                            sendToAddress = value;
                            ipok = true;
                        }
                        else
                        {
                            System.Net.IPHostEntry iphe = System.Net.Dns.GetHostEntry(value);
                            foreach (IPAddress ip in iphe.AddressList)
                            {
                                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                {
                                    sendToIP = ip;
                                    sendToAddress = value;
                                    ipok = true;
                                    break;
                                }
                            }
                        }
                        if (ipok == false)
                        {
                            throw new Exception("有効なインターフェースが見つかりません。");
                        }
                    }
                    catch (Exception es)
                    {
                        throw es;
                    }
                }
            }
        }
        
        #endregion

        #region StatusColorsプロパティ

        [
            Category("表示"),
            Description("通信ポートの状態表示に使用される背景色です。")
        ]
        public BackColors StatusColors
        {
            get
            {
                return backColors;
            }
            set
            {
                backColors = value;
                if (isOpen==true)
                {
                    labelRecive.BackColor = backColors.ReciveOff;
                    labelSend.BackColor = backColors.SendOff;
                }
                else
                {
                    labelRecive.BackColor = backColors.Close;
                    labelSend.BackColor = backColors.Close;
                }
            }
        }

        #endregion

        #region IsOpenプロパティ

        private bool isOpen = false;
        [
            Category("状態"),
            Description("通信ポートをオープンしているかを示します。")
        ]
        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
        }

        #endregion

        #region DataDumpプロパティ

        private bool dataDump = false;
        [
            Category("動作"),
            DefaultValue(false),
            Description("送受信データのダンプを動作ログに出力するかを示します。")
        ]
        public bool DataDump
        {
            get
            {
                return dataDump;
            }
            set
            {
                dataDump = value;
            }
        }

        #endregion

        #endregion

        #region 既存のプロパティ(削除)

        #region CausesValidationプロパティ

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool CausesValidation
        {
            get
            {
                return base.CausesValidation;
            }
        }

        #endregion

        #region AutoValidateプロパティ

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new AutoValidate AutoValidate
        {
            get
            {
                return base.AutoValidate;
            }
        }

        #endregion

        #region AutoScrollプロパティ

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool AutoScroll
        {
            get
            {
                return base.AutoScroll;
            }
        }

        #endregion

        #region AutoScrollMarginプロパティ

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Size AutoScrollMargin
        {
            get
            {
                return base.AutoScrollMargin;
            }
            set
            {
            }
        }

        #endregion

        #region AutoScrollMinSizeプロパティ

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Size AutoScrollMinSize
        {
            get
            {
                return base.AutoScrollMinSize;
            }
            set
            {
            }
        }

        #endregion

        #region AutoSizeプロパティ

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
        }

        #endregion

        #region AutoSizeModeプロパティ

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new AutoSizeMode AutoSizeMode
        {
            get
            {
                return base.AutoSizeMode;
            }
        }

        #endregion

        #region BackgroundImageプロパティ

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
        }

        #endregion

        #region BackgroundImageLayoutプロパティ

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
        }

        #endregion

        #region MaximumSizeプロパティ

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Size MaximumSize
        {
            get
            {
                return base.MaximumSize;
            }
        }

        #endregion

        #region MinimumSizeプロパティ

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Size MinimumSize
        {
            get
            {
                return base.MinimumSize;
            }
        }

        #endregion

        #region Paddingプロパティ

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Padding Padding
        {
            get
            {
                return base.Padding;
            }
        }

        #endregion

        #endregion

        #endregion

        #region メソッド

        #region プロードキャストアドレス取得

        /// <summary>
        /// このホストのブロードキャストアドレス(IPv4)を取得します。
        /// </summary>
        /// <returns>結果(成功=ブロードキャストアドレス, 失敗=null)</returns>
        public IPAddress GetBroadcastAddress()
        {
            return getBroadcastAddress();
        }

        #endregion

        #region ローカルIPアドレス取得

        /// <summary>
        /// このホストのIPアドレス(IPv4)を取得します。
        /// </summary>
        /// <returns>結果(成功=IPアドレス, False=null)</returns>
        public IPAddress GetLocalIPAddress()
        {
            return getLocalIpAddress();
        }

        #endregion

        #region ローカルサブネットマスク取得

        /// <summary>
        /// このホストのサブネットマスク(IPv4)を取得します。
        /// </summary>
        /// <returns>結果(成功=サブネットマスク, False=null)</returns>
        public IPAddress GetLocalSubnetMask()
        {
            return getLocalSubnetMask();
        }

        #endregion

        #region IsPingReplyメソッド

        /// <summary>
        /// Ping応答があるかを返します。
        /// </summary>
        /// <param name="host">送信先のホスト</param>
        /// <param name="toutms">タイムアウト時間(ms)</param>
        /// <returns>結果(リプライあり=true, リプライなし=false)</returns>
        public bool IsPingReply(String host, int toutms)
        {
            bool rt = true;
            System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
            try
            {
                LogPrint("PING " + host);
                System.Net.NetworkInformation.PingReply reply = p.Send(host, toutms);
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    LogPrint("PING 応答 - From[" + reply.Address.ToString() + "], Size[" +
                        reply.Buffer.Length.ToString() + "], Time[" + 
                        reply.RoundtripTime.ToString() + "ms], TTL[" +
                        reply.Options.Ttl.ToString() + "]");
                }
                else
                {
                    LogPrint("PING 失敗 - " + reply.Status.ToString());
                    rt = false;
                }
            }
            catch
            {
                rt = false;
            }
            p.Dispose();
            return rt;
        }

        #endregion

        #region オープン

        /// <summary>
        /// 通信ポートをオープンします。
        /// </summary>
        /// <returns>結果(True=成功, False=失敗)</returns>
        public bool Open()
        {
            bool status = false;

            if (isOpen == false)
            {

                try
                {
                    reciveBuffer = new byte[reciveBufferSize];
                    reciveBufferDataLength = 0;

                    udp = new UdpClient(portNo);
                    udp.DontFragment = false;           /* データグラムの断片化を禁止   */
                    udp.EnableBroadcast = true;         /* ブロードキャストを許可       */
                   
                    rcvthread = new Thread(new ThreadStart(UDPReciveThread));
                    rcvthread.Start();
                    
                    labelRecive.BackColor = backColors.ReciveOff;
                    labelSend.BackColor = backColors.SendOff;
                    LogPrint("通信ポート[" + portNo.ToString().Trim() + "]をオープンしました");
                    status = true;
                    isOpen = true;
                }
                catch (Exception es)
                {
                    reciveBuffer = null;
                    LogPrint("通信ポートのオープンに失敗しました - " + es.Message);
                }
            }
            else
            {
                LogPrint("通信ポートはすでにオープンされています");
            }

            return status;
        }

        #endregion

        #region クローズ

        /// <summary>
        /// 通信ポートをクローズします。
        /// </summary>
        public void Close()
        {
            if (isOpen)
            {
                rcvthread.Abort();

                udp.Close();
                reciveBuffer = null;
                
                labelRecive.BackColor = backColors.Close;
                labelSend.BackColor = backColors.Close;
                LogPrint("通信ポート[" + portNo.ToString().Trim() + "]をクローズしました");
                isOpen = false;
            }
            else
            {
            }
        }

        #endregion

        #region バイナリデータ送信

        /// <summary>
        /// バイナリデータを送信します。
        /// </summary>
        /// <param name="data">送信データ</param>
        /// <param name="count">送信バイト数</param>
        public void SendBytes(byte[] data, int count)
        {
            SendBytes(data, 0, count);
        }
        
        /// <summary>
        /// バイナリデータを送信します。
        /// </summary>
        /// <param name="data">送信データ</param>
        /// <param name="offset">送信開始位置</param>
        /// <param name="count">送信バイト数</param>
        public void SendBytes(byte[] data, int offset, int count)
        {
            if (isOpen == true)
            {
                // データダンプ
                if (dataDump == true && xConsoleBox!=null)
                {
                    try
                    {
                        xConsoleBox.LogHexDump("送信データ", data, offset, count);
                    }
                    catch
                    {
                    }
                }

                // 送信
                labelSend.BackColor = backColors.SendOn;
                Application.DoEvents();

                try
                {
                    IPEndPoint ep = new IPEndPoint(sendToIP, portNo);
                    byte[] dataofs = new byte[count];
                    for (int i = 0; i < dataofs.Length; i++)
                    {
                        dataofs[i] = data[offset + i];
                    }
                    udp.Send(dataofs , count, ep);
                }
                catch (Exception es)
                {
                    LogPrint("送信に失敗しました - " + es.Message);
                }
                finally
                {
                    labelSend.BackColor = backColors.SendOff;
                    Application.DoEvents();
                }
            }
            else
            {
                LogPrint("通信ポートをオープンしていないため送信できません");
            }
        }

        #endregion

        #region テキストデータ送信

        /// <summary>
        /// テキストデータを送信します。
        /// </summary>
        /// <param name="text">１行のデータ</param>
        /// <param name="encoding">テキストエンコーディング</param>
        /// <param name="endchar">行末文字配列</param>
        /// <param name="length">行末文字数</param>
        public void SendLine(string text, Encoding encoding, byte[] endchar, int length)
        {
            // テキストをバイト列に変換
            int size = encoding.GetByteCount(text);
            int len = endchar.Length;
            if (len > length)
            {
                len = length;
            }

            byte[] buf = new byte[size + len];
            encoding.GetBytes(text, 0, text.Length, buf, 0);

            // 行末文字を付加
            for (int i = 0; i < len; i++)
            {
                buf[size + i] = endchar[i];
            }
            
            // 送信
            SendBytes(buf, buf.Length);
        }

        #endregion

        #region バイナリデータ取得

        delegate int GetReciveBytesDelegate(byte[] data, int offset, int count);

        /// <summary>
        /// バイナリデータを取得します。
        /// </summary>
        /// <returns>取得バイト数または-1(エラー値)</returns>
        /// <param name="data">取得データ</param>
        /// <param name="offset">格納開始位置</param>
        /// <param name="count">格納可能バイト数</param>
        public int GetReciveBytes(byte[] data, int offset, int count)
        {
            if (InvokeRequired)
            {
                // 別スレッドから呼び出された場合
                object[] param = {data, offset, count};
                return (int)Invoke(new GetReciveBytesDelegate(GetReciveBytes), param);
            }
            
            // データを取得
            int retlen = getReciveBytes(data, offset, count);

            return retlen;
        }

        #endregion

        #region バイナリデータ取得(終端文字指定)

        delegate byte[] GetReciveLineBytesDelegate(byte[] endchar, int length);

        /// <summary>
        /// 終端文字を指定してバイナリデータを取得します。
        /// </summary>
        /// <returns>１行分のバイナリデータ(データが無いときはnullが返ります)</returns>
        /// <param name="endchar">行末文字配列</param>
        /// <param name="length">行末文字数</param>
        public byte[] GetReciveLineBytes(byte[] endchar, int length)
        {
            if (InvokeRequired)
            {
                // 別スレッドから呼び出された場合
                object[] param = { endchar, length };
                return (byte[])Invoke(new GetReciveLineBytesDelegate(GetReciveLineBytes), param);
            }

            // Mutexの所有権を得る - ∞秒待つ
            mutex.WaitOne(-1, false);

            // 行の長さを取得
            int retlen = findLineLength(endchar, length);
            if (retlen < 0)
            {
                // Mutexの所有権を解放
                mutex.ReleaseMutex();
                return null;
            }

            // データを取得
            byte[] buf = new byte[retlen];
            getReciveBytes(buf, 0, retlen);

            // Mutexの所有権を解放
            mutex.ReleaseMutex();

            return buf;
        }

        #endregion

        #region テキストデータ取得

        delegate string GetReciveLineDelegate(Encoding encoding, byte[] endchar, int length);

        /// <summary>
        /// テキストデータを取得します。
        /// </summary>
        /// <returns>１行のデータ</returns>
        /// <param name="encoding">テキストエンコーディング</param>
        /// <param name="endchar">行末文字配列</param>
        /// <param name="length">行末文字数</param>
        public string GetReciveLine(Encoding encoding, byte[] endchar, int length)
        {
            string rcvtext = "";

            // データを取得
            byte[] buf = GetReciveLineBytes(endchar, length);

            // 文字列に変換
            if (buf != null)
            {
                rcvtext = encoding.GetString(buf);
            }

            return rcvtext;
        }

        #endregion

        #endregion

        #region イベント

        #region ライセンスチェック

        private void XUDPComControl_Load(object sender, EventArgs e)
        {
            if (DesignMode == false && useLocalLicense == false)
            {
                MessageBox.Show("LocalLicenseKeyを設定してください。", this.Name + "コントロール", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region プロパティ変更(コンテナ)

        private void XUDPComControl_SizeChanged(object sender, EventArgs e)
        {
            ControlRelocation();
        }

        private void XUDPComControl_FontChanged(object sender, EventArgs e)
        {
            labelRecive.Font = base.Font;
            labelSend.Font = base.Font;
            Refresh();
        }

        private void XUDPComControl_ForeColorChanged(object sender, EventArgs e)
        {
            labelRecive.ForeColor = base.ForeColor;
            labelSend.ForeColor = base.ForeColor;
            Refresh();
        }

        private void backColors_ColorChange(object sender, XUDPComControl.BackColors.ColorChangeEventArgs e)
        {
            if (isOpen == true)
            {
                labelRecive.BackColor = backColors.ReciveOff;
                labelSend.BackColor = backColors.SendOff;
            }
            else
            {
                labelRecive.BackColor = backColors.Close;
                labelSend.BackColor = backColors.Close;
            }
        }

        #endregion

        #region 再描画

        /// <summary>
        /// コントロール再描画イベント
        /// </summary>
        private void XUDPComControl_Paint(object sender, PaintEventArgs e)
        {
            ControlRelocation();
        }

        #endregion

        #region データ受信スレッド

        /// <summary>
        /// UDP受信スレッド
        /// </summary>
        private void UDPReciveThread()
        {
            int rcvlen = 0;
            int cpylen = 0;
            IPAddress fromIP = null;
            IPEndPoint remoteEP = null;     // 受信データ（送信IPアドレス、送信ポート番号など）
            byte[] rcvBytes = new byte[0];  // 受信データ
            
            try
            {
                LogPrint("UDP受信スレッド開始しました");
                while(true)
                {
                    // 受信ループ(Abort例外で終了)

                    // 未コピーデータがなければ受信
                    if (rcvBytes.Length == cpylen)
                    {
                        if (udp.Available == 0)
                        {
                            rcvBytes = new byte[0];
                        }
                        else
                        {
                            rcvBytes = udp.Receive(ref remoteEP);
                            fromIP = remoteEP.Address;
                        }
                        cpylen = 0;
                    }

                    // 未コピーデータがあり、バッファが空いていれば格納しイベントを上げる
                    if ((reciveBuffer.Length != reciveBufferDataLength) && (rcvBytes.Length != cpylen))
                    {
                        labelRecive.BackColor = backColors.ReciveOn;
                        Application.DoEvents();

                        // Mutexの所有権を得る - ∞秒待つ
                        mutex.WaitOne(-1, false);

                        // 今回格納可能なデータ長計算
                        rcvlen = reciveBuffer.Length - reciveBufferDataLength;
                        if (rcvlen > rcvBytes.Length - cpylen)
                        {
                            rcvlen = rcvBytes.Length - cpylen;
                        }
                        
                        // 格納
                        Array.Copy(rcvBytes, cpylen, reciveBuffer, reciveBufferDataLength, rcvlen);

                        // 受信データサイズを更新
                        reciveBufferDataLength += rcvlen;
                        cpylen += rcvlen;

                        // Mutexの所有権を解放
                        mutex.ReleaseMutex();

                        if (reciveBufferDataLength == 0)
                        {
                            // 受信データなし
                            labelRecive.BackColor = backColors.ReciveOff;
                            Application.DoEvents();
                            return;
                        }

                        // 受信通知イベント
                        ReciveEventArgs eArgs = new ReciveEventArgs();
                        eArgs.FromIP = fromIP;
                        eArgs.DataCount = reciveBufferDataLength;
                        eArgs.FreeCount = reciveBuffer.Length - reciveBufferDataLength;
                        if (dataDump == true)
                        {
                            LogPrint("Status[RCV:" + eArgs.DataCount.ToString()+",FREE:"+eArgs.FreeCount.ToString()+"]");
                        }
                        if (ReciveEvent != null)
                        {
                            // ハンドラが設定されていたらイベント発生
                            CallReciveEvent(this, eArgs);
                        }

                        if (reciveBuffer.Length - reciveBufferDataLength < 1)
                        {
                            labelRecive.BackColor = backColors.BufferFull;
                            LogPrint("受信バッファがいっぱいです");
                        }
                        else
                        {
                            labelRecive.BackColor = backColors.ReciveOff;
                        }

                        Application.DoEvents();
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // スレッド終了
                LogPrint("UDP受信スレッド停止しました");
            }
            catch (Exception es)
            {
                LogPrint("UDP受信スレッドで例外が発生しました - " + es.Message);
                throw es;
            }
        }

        #endregion

        #region 追加のイベント

        #region データ受信通知

        /// <summary>
        /// データ受信通知イベント
        /// </summary>
        public class ReciveEventArgs : EventArgs
        {
            public IPAddress FromIP;    // 最後に受信した送信元IPアドレス
            public int DataCount;       // 受信データ長
            public int FreeCount;       // 受信バッファの空き
        }
        public delegate void ReciveEventHandler(object sender, ReciveEventArgs e);
        [
            Category("動作"),
            Description("データ受信したことを通知するイベントです。環境により別スレッドで発生します。")
        ]
        public event ReciveEventHandler ReciveEvent;

        #endregion

        #endregion

        #endregion

        #region その他

        #region コントロール再配置

        /// <summary>
        /// 各コントロールの位置を調整する
        /// </summary>
        private void ControlRelocation()
        {

            #region サイズチェックおよび計算

            // 最小サイズ以下をチェック
            int posX = CTRL_LABEL_MINIMUM * 2 +
                       CTRL_LABEL_SPC * 3 +
                       Padding.Left + Padding.Right;
            int posY = CTRL_LABEL_MINIMUM +
                       CTRL_LABEL_SPC * 2 +
                       Padding.Top + Padding.Bottom;
            if (Height < posY)
            {
                Height = posY;
            }
            if (Width < posX)
            {
                Width = posX;
            }

            // サイズ計算
            float powX = (float)Width / posX;
            float powY = (float)Height / posY;

            int label_size_w = Convert.ToInt32(CTRL_LABEL_MINIMUM * powX);
            int label_size_h = Convert.ToInt32(CTRL_LABEL_MINIMUM * powY);
            int label_spc_w = Convert.ToInt32(CTRL_LABEL_SPC * powX);
            int label_spc_h = Convert.ToInt32(CTRL_LABEL_SPC * powY);
            int padX = (Width -
                       (label_size_w * 2 +
                        label_spc_w)) / 2;
            int padY = (Height -
                       (label_size_h)) / 2;

            #endregion

            #region 再配置

            #region Send : ラベル

            posX = padX;
            posY = padY;

            labelSend.Top = posY;
            labelSend.Left = posX;
            labelSend.Height = label_size_h;
            labelSend.Width = label_size_w;

            #endregion

            #region Recive : ラベル

            posX += label_size_w + label_spc_w;

            labelRecive.Top = posY;
            labelRecive.Left = posX;
            labelRecive.Height = label_size_h;
            labelRecive.Width = label_size_w;

            #endregion

            #endregion

        }

        #endregion

        #region データ取得

        /// <summary>
        /// データを取得します。
        /// </summary>
        /// <returns>取得バイト数または-1(エラー値)</returns>
        /// <param name="data">取得データ</param>
        /// <param name="offset">格納開始位置</param>
        /// <param name="count">格納可能バイト数</param>
        private int getReciveBytes(byte[] data, int offset, int count)
        {
            int retlen = 0;

            if (isOpen == true)
            {
                // Mutexの所有権を得る - ∞秒待つ
                mutex.WaitOne(-1, false);

                // 返却サイズ計算
                retlen = count;
                if (retlen > reciveBufferDataLength)
                {
                    retlen = reciveBufferDataLength;
                }

                try
                {
                    // データ格納
                    for (int i = 0; i < retlen; i++)
                    {
                        data[offset + i] = reciveBuffer[i];
                    }

                    // データシフト
                    for(int i=0;i<reciveBufferDataLength - retlen;i++)
                    {
                        reciveBuffer[i] = reciveBuffer[i + retlen];
                    }

                    // 受信データサイズを更新
                    reciveBufferDataLength -= retlen;

                    // データダンプ
                    if (dataDump == true && xConsoleBox != null)
                    {
                        try
                        {
                            xConsoleBox.LogHexDump("受信データ", data, offset, retlen);
                        }
                        catch
                        {
                        }
                    }
                }
                catch (Exception es)
                {
                    LogPrint("データの取得に失敗しました - " + es.Message);
                    retlen = -1;
                }
                finally
                {
                }

                // Mutexの所有権を解放
                mutex.ReleaseMutex();
            }
            else
            {
                LogPrint("通信ポートをオープンしていないため受信できません");
                retlen = -1;
            }

            return retlen;
        }

        #endregion

        #region 行末検索

        /// <summary>
        /// 行末を検索します。
        /// </summary>
        /// <returns>行末の区切り文字の終端位置または-1(エラー値)</returns>
        /// <param name="endchar">区切り文字配列</param>
        /// <param name="length">区切り文字数</param>
        private int findLineLength(byte[] endchar, int length)
        {
            int linelength = -1;

            if (isOpen == true)
            {
                // Mutexの所有権を得る - ∞秒待つ
                mutex.WaitOne(-1, false);

                // 検索
                for (int i = 0; i <= reciveBufferDataLength - length; i++)
                {
                    if (reciveBuffer[i] == endchar[0])
                    {
                        int match = 0;
                        for (int j = 0; j < length; j++)
                        {
                            if (reciveBuffer[i+j] == endchar[j])
                            {
                                match++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (match == length)
                        {
                            linelength = i + length;
                            break;
                        }
                    }
                    if (isOpen == false)
                    {
                        LogPrint("通信ポートをオープンしていないため受信できません");
                        linelength = -1;
                        break;
                    }
                }

                // Mutexの所有権を解放
                mutex.ReleaseMutex();
            }
            else
            {
                LogPrint("通信ポートをオープンしていないため受信できません");
                linelength = -1;
            }

            return linelength;
        }

        #endregion

        #region データ受信通知

        delegate void CallReciveEventDelegate(object sender, ReciveEventArgs e);

        private void CallReciveEvent(object sender, ReciveEventArgs e)
        {
            if (InvokeRequired)
            {
                // 別スレッドから呼び出された場合
                object[] param = { sender, e };
                Invoke(new CallReciveEventDelegate(CallReciveEvent), param);
                return;
            }

            try
            {
                ReciveEvent(sender, e);
            }
            catch(Exception es)
            {
                LogPrint("ReciveEvent内で例外が発生しました - " + es.Message);
            }
        }

        #endregion

        #region 操作ログ出力

        private void LogPrint(string msg)
        {
            if (xConsoleBox != null)
            {
                try
                {
                    xConsoleBox.LogPrint("[" + this.Name + "] " + msg);
                }
                catch
                {
                }
            }
        }

        #endregion

        #region ダンプ出力

        private void LogHexDump(string msg, byte[] data, int offset, int count)
        {
            if (xConsoleBox != null)
            {
                try
                {
                    xConsoleBox.LogHexDump("[" + this.Name + "] " + msg, data, offset, count);
                }
                catch
                {
                }
            }
        }

        #endregion

        #region ブロードキャストアドレス生成

        /// <summary>
        /// このホストのブロードキャストアドレスを取得する
        /// </summary>
        /// <returns>結果(成功=ブロードキャストアドレス, 失敗=null)</returns>
        private IPAddress getBroadcastAddress()
        {
            IPAddress broadcastAddress = null;

            // ローカルIPアドレスおよびサブネットを取得
            IPAddress localIPAddr = getLocalIpAddress();
            IPAddress subnetMask = getLocalSubnetMask();

            if(localIPAddr == null || subnetMask == null)
            {
                return null;
            }

            try
            {
                // ローカルIPアドレスおよびサブネットをbyte列に変換
                Byte[] ip = localIPAddr.GetAddressBytes();
                Byte[] subnet = subnetMask.GetAddressBytes();

                // ブロードキャストアドレスを生成
                String strBroadcastAddress = "";
                for (int i = 0; i < ip.Length; i++)
                {
                    if(strBroadcastAddress.Length > 0)
                    {
                        strBroadcastAddress += ".";
                    }
                    strBroadcastAddress += (ip[i] & subnet[i] | ((Byte)~subnet[i])).ToString();
                }
                broadcastAddress = IPAddress.Parse(strBroadcastAddress);
            }
            catch (Exception ex)
            {
                xConsoleBox.LogPrint("ブロードキャストアドレス生成に失敗しました - " + ex.Message);
            }
            return broadcastAddress;
        }

        #endregion

        #region ホストIPアドレス取得

        /// <summary>
        /// このホストのIPアドレスを取得する
        /// </summary>
        /// <returns>結果(成功=IPアドレス, 失敗=null)</returns>
        private IPAddress getLocalIpAddress()
        {
            IPAddress localIpAddress = null;
            try
            {
                // ローカルIPアドレスを取得
                IPHostEntry ipHstEnt = Dns.GetHostEntry("");
                foreach (IPAddress ip in ipHstEnt.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        localIpAddress = ip;
                        break;
                    }
                }
                if (localIpAddress == null)
                {
                    throw new Exception("有効なインターフェースが見つかりません。");
                }
            }
            catch (Exception ex)
            {
                xConsoleBox.LogPrint("ローカルアドレス取得に失敗しました - " + ex.Message);
            }

            return localIpAddress;
        }

        #endregion

        #region ホストサブネットマスク取得

        /// <summary>
        /// このホストのサブネットマスクを取得する
        /// </summary>
        /// <returns>結果(成功=サブネットマスク, 失敗=null)</returns>
        private IPAddress getLocalSubnetMask()
        {
            IPAddress localSubnet = null;
            try
            {
                // ローカルIPアドレスを取得
                IPAddress ip = getLocalIpAddress();

                // ローカルのサブネットを取得
                ManagementObjectSearcher mos = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled=TRUE");
                ManagementObjectCollection moc = mos.Get();
                foreach (ManagementObject mo in moc)
                {
                    String[] mip = (String[])mo["IPAddress"];
                    if (ip.ToString() == mip[0])
                    {
                        String[] msn = (String[])mo["IPSubnet"];
                        localSubnet = IPAddress.Parse(msn[0]);
                        break;
                    }
                }
                if(localSubnet == null)
                {
                    throw new Exception("有効なインターフェースが見つかりません。");
                }
            }
            catch (Exception ex)
            {
                xConsoleBox.LogPrint("サブネットマスク取得に失敗しました - " + ex.Message);
            }
            return localSubnet;
        }

        #endregion

        #endregion

    }
}
