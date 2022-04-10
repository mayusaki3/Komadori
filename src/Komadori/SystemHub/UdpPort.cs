using System;
using System.Collections;
//using System.Collections.Generic;
using System.ComponentModel;
//using System.ComponentModel.Design;
using System.Drawing;
//using System.Data;
using System.Globalization;
//using System.IO.Ports;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
//using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
//using System.Windows.Forms.Design;

namespace XControls.IO
{
    /// <summary>
    /// UDP通信を行うためのクラスです。
    /// </summary>
    [Designer(typeof(UdpPortDesigner))]
    public class UdpPort : UserControl
    {
        #region インナークラス

        #region UdpPortDesignerクラス

        /// <summary>
        /// UdpPort用にデザイナをカスタマイズします。
        /// </summary>
        public class UdpPortDesigner : System.Windows.Forms.Design.ControlDesigner
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.UdpPort.UdpPortDesigner クラスの新しいインスタンスを初期化します。
            /// </summary>
            public UdpPortDesigner()
            {
            }

            #endregion

            #region メソッド

            #region PostFilterPropertiesメソッド

            protected override void PostFilterProperties(IDictionary properties)
            {
                // フィルタリングするプロパティ
                properties.Remove("CausesValidation");
                properties.Remove("AutoValidate");
                properties.Remove("AutoScroll");
                properties.Remove("AutoScrollMargin");
                properties.Remove("AutoScrollMinSize");
                properties.Remove("AutoSize");
                properties.Remove("AutoSizeMode");
                properties.Remove("BackgroundImage");
                properties.Remove("BackgroundImageLayout");
                properties.Remove("MaximumSize");
                properties.Remove("MinimumSize");
                properties.Remove("Padding");

                base.PostFilterProperties(properties);
            }

            #endregion

            #endregion
        }

        #endregion

        #region BackColorsクラス

        /// <summary>
        /// 通信状態を表す配色を設定するためのクラスです。
        /// </summary>
        [TypeConverter(typeof(BackColorsConverter))]
        public class BackColors
        {
            #region インナークラス

            #region BackColorsConverterコンバータ

            /// <summary>
            /// BackColorsクラスのプロパティに対する型変換を提供します。 
            /// </summary>
            internal class BackColorsConverter : ExpandableObjectConverter
            {
                #region メソッド

                #region CanConvertFromメソッド

                /// <summary>
                /// 指定されたタイプのオブジェクトから変換可能かを判定します。
                /// </summary>
                /// <param name="context">コンテキスト情報</param>
                /// <param name="cvType">タイプ</param>
                /// <returns>判定結果</returns>
                public override bool CanConvertFrom(ITypeDescriptorContext context, Type cvType)
                {
                    if (cvType == typeof(string))
                    {
                        // 文字列から変換不可
                        return false;
                    }
                    return base.CanConvertFrom(context, cvType);
                }

                #endregion

                #region CanConvertToメソッド

                /// <summary>
                /// 指定されたタイプのオブジェクトに変換可能かを判定します。
                /// </summary>
                /// <param name="context">コンテキスト情報</param>
                /// <param name="cvType">タイプ</param>
                /// <returns>判定結果</returns>
                public override bool CanConvertTo(ITypeDescriptorContext context, Type cvType)
                {
                    if (cvType == typeof(string))
                    {
                        // 文字列への変換可
                        return true;
                    }
                    return base.CanConvertTo(context, cvType);
                }

                #endregion

                #region ConvertFromメソッド

                /// <summary>
                /// 
                /// </summary>
                /// <param name="context">コンテキスト情報</param>
                /// <param name="culture">カルチャ―情報</param>
                /// <param name="value">変換するオブジェクト</param>
                /// <returns>変換結果のオブジェクト</returns>
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

                #endregion

                #region ConvertToメソッド

                /// <summary>
                /// 
                /// </summary>
                /// <param name="context">コンテキスト情報</param>
                /// <param name="cultInfo"></param>
                /// <param name="value">変換するオブジェクト</param>
                /// <param name="destType">変換後のタイプ</param>
                /// <returns>変換結果のオブジェクト</returns>
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

                #endregion

                #endregion
            }

            #endregion

            #endregion

            #region 構築・破棄

            public BackColors()
            {
                close = System.Drawing.Color.Gray;
                sendOff = System.Drawing.Color.Olive;
                reciveOff = System.Drawing.Color.Green;
                sendOn = System.Drawing.Color.Yellow;
                reciveOn = System.Drawing.Color.Lime;
                bufferFull = System.Drawing.Color.Red;
            }

            #endregion

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

            /// <summary>
            /// 回線切断中のステータス枠背景色を参照または設定します。
            /// </summary>
            [
                Category("表示"),
                DefaultValue(typeof(Color), "Gray"),
                Description("回線切断中のステータス枠背景色を参照または設定します。")
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

            /// <summary>
            /// データ非送信時のステータス枠背景色を参照または設定します。
            /// </summary>
            [
                Category("表示"),
                DefaultValue(typeof(Color), "Olive"),
                Description("データ非送信時のステータス枠背景色を参照または設定します。")
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

            /// データ受信待機中のステータス枠背景色を参照または設定します。
            [
                Category("表示"),
                DefaultValue(typeof(Color), "Green"),
                Description("データ受信待機中のステータス枠背景色を参照または設定します。")
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

            /// データ送信中のステータス枠背景色を参照または設定します。
            [
                Category("表示"),
                DefaultValue(typeof(Color), "Yellow"),
                Description("データ送信中のステータス枠背景色を参照または設定します。")
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

            /// データ受信中のステータス枠背景色を参照または設定します。
            [
                Category("表示"),
                DefaultValue(typeof(Color), "Lime"),
                Description("データ受信中のステータス枠背景色を参照または設定します。")
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

            /// バッファフル時のステータス枠背景色を参照または設定します。
            [
                Category("表示"),
                DefaultValue(typeof(Color), "Red"),
                Description("バッファフル時のステータス枠背景色を参照または設定します。")
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

            #region イベント

            #region 設定色変更通知 (ColorChange)

            /// <summary>
            /// ColorChangeイベントのイベントデータを提供します。
            /// </summary>
            public class ColorChangeEventArgs : EventArgs
            {
                /// <summary>
                /// イベントが処理されたかどうかを示す値を参照または設定します。
                /// </summary>
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

            #endregion
        }

        #endregion

        #region ReceiveHelperクラス

        /// <summary>
        /// データ受信スレッド処理を行うためのヘルパークラスです。
        /// </summary>
        public class ReceiveHelper
        {
            #region 変数

            private UdpClient client = null;
            private ReceiveEventHandler receive = null;
            private int port = 0;
            private volatile bool tStopFlg = false;
            private UdpPort udp = new UdpPort();

            #endregion

            #region コンストラクタ

            /// <summary>
            /// ReceiveThread クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="client">UDPClient(UdpPortクラス側で、生成済みのもの)</param>
            /// <param name="e">ReceiveEventHandler(Receivedイベント発生に必要)</param>
            /// <param name="port">データ受信に使用するポート番号</param>
            public ReceiveHelper(UdpClient client, ReceiveEventHandler e, int port)
            {
                this.client = client;
                this.receive = e;
                this.port = port;
            }

            #endregion

            #region メソッド

            #region データ受信スレッド (DataReceiveThread)

            /// <summary>
            /// データ受信スレッドです。
            /// </summary>
            /// <exception cref=" ObjectDisposedException">破棄されたオブジェクトで処理を実行しようとした場合</exception>
            /// <exception cref="ArgumentNullException">Null参照された場合</exception>
            /// <exception cref="SocketException">ソケットエラーが発生した場合</exception>
            /// <exception cref="FormatException">引数の書式がメソッドのパラメータの仕様と一致しない場合</exception>
            /// <exception cref="PlatformNotSupportedException">特定のプラットフォームで機能が実行されない場合</exception>
            /// <exception cref="DecoderFallbackException">デコーダフォールバック操作に失敗した場合</exception>
            /// <exception cref="Exception">その他例外</exception>
            public void DataReceiveThread()
            {
                try
                {
                    IPEndPoint remoteEP = null;     //受信データ（送信IPアドレス、送信ポート番号など）
                    byte[] rcvBytes;                //受信データ

                    while (!tStopFlg)
                    {
                        //データ受信
                        rcvBytes = client.Receive(ref remoteEP);

                        //IPアドレス、受信データを設定
                        ReceiveEventArgs e = new ReceiveEventArgs();
                        e.IpAddress = remoteEP.Address;
                        e.ReceiveData = rcvBytes;

                        if (tStopFlg == false)
                        {
                            //イベント発生
                            if (this.receive != null)
                            {
                                this.receive(this, e);
                            }
                        }

                        e = null;
                    }
                }
                catch (SocketException ex)
                {
                    //スレッド終了処理
                    if (ex.ErrorCode.Equals(10004))
                    {
                        return;
                    }
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            #endregion

            #region データ受信スレッド停止 (ReceiveStop)
            /// <summary>
            /// データ受信スレッドを停止して、受信を終了します。
            /// </summary>
            public void ReceiveStop()
            {
                tStopFlg = true;
                client.Close();
                client = null;
            }

            #endregion

            #endregion
        }

        #endregion

        #endregion

        #region 構築・破棄

        /// <summary>
        /// XControls.UdpPort クラスの新しいインスタンスを初期化します。
        /// </summary>
        public UdpPort()
        {
            #region コンポーネント初期化

            this.labelSend = new System.Windows.Forms.Label();
            this.labelRecive = new System.Windows.Forms.Label();
            this.SuspendLayout();

            this.labelSend.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelSend.Location = new System.Drawing.Point(3, 3);
            this.labelSend.Name = "labelSend";
            this.labelSend.Size = new System.Drawing.Size(12, 12);
            this.labelSend.TabIndex = 0;
            this.labelSend.Text = "S";
            this.labelSend.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.labelRecive.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelRecive.Location = new System.Drawing.Point(20, 3);
            this.labelRecive.Name = "labelRecive";
            this.labelRecive.Size = new System.Drawing.Size(12, 12);
            this.labelRecive.TabIndex = 1;
            this.labelRecive.Text = "R";
            this.labelRecive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.labelRecive);
            this.Controls.Add(this.labelSend);
            this.DoubleBuffered = true;
            this.Name = "UdpPort";
            this.Size = new System.Drawing.Size(36, 18);
            this.ResumeLayout(false);

            #endregion

            // 画面再配置
            ControlRelocation();

            // イベント設定
            backColors.ColorChange += new BackColors.ColorChangeEventHandler(backColors_ColorChange);
        }

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            #region 後処理

            // 通信中ならクローズ
            if(isOpen)  Close();
            
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            #endregion
        }

        #endregion

        #region 定数

        /// <summary>
        /// ラベルの最小高さと幅
        /// </summary>
        const int CTRL_LABEL_MINIMUM = 12;

        /// <summary>
        /// ラベルと周囲の間隔
        /// </summary>
        const int CTRL_LABEL_SPC = 4;

        #endregion

        #region 変数

        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 送信モニタ用ラベルです。
        /// </summary>
        private System.Windows.Forms.Label labelSend;

        /// <summary>
        /// 受信モニタ用ラベルです。
        /// </summary>
        private System.Windows.Forms.Label labelRecive;

        /// <summary>
        /// 動作ログ出力に使用するLoggingクラスのインスタンスです。
        /// </summary>
        private Logging xlog = null;

        /// <summary>
        /// ステータス背景色です。
        /// </summary>
        private BackColors backColors = new BackColors();

        /// <summary>
        /// 通信に使用するUDPクライアントクラスのインスタンスです。
        /// </summary>
        private UdpClient udp = null;

        /// <summary>
        /// 通信に使用するUDP受信スレッドです。
        /// </summary>
        private Thread rcvThread = null;

        /// <summary>
        /// 通信に使用するUDP受信ヘルパークラスのインスタンスです。
        /// </summary>
        private ReceiveHelper rcvHelper = null;

        /// <summary>
        /// 作業用受信バッファです。
        /// </summary>
        private byte[] reciveBuffer = null;

        /// <summary>
        /// 作業用受信バッファに格納されているデータのサイズです。
        /// </summary>
        private int reciveBufferDataLength = 0;

        /// <summary>
        /// 送受信に使用するミューテックスオブジェクトです。
        /// </summary>
        private static Mutex mutex = new Mutex(false);

        #endregion

        #region プロパティ

        #region Loggingプロパティ

        /// <summary>
        /// 動作ログを出力する Logging のインスタンスを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(null),
            Description("動作ログを出力する Logging のインスタンスを参照または設定します。")
        ]
        public Logging Logging
        {
            get
            {
                return xlog;
            }
            set
            {
                xlog = value;
            }
        }

        #endregion

        #region PortNoプロパティ

        private int portNo = 1024;
        /// <summary>
        /// この通信で使用するポートを参照または設定します。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        [
            Category("動作"),
            DefaultValue(1024),
            Description("この通信で使用するポートを参照または設定します。")
        ]
        public int PortNo
        {
            get
            {
                return portNo;
            }
            set
            {
                if(isOpen)
                {
                    new InvalidOperationException("オープン中はポートを変更できません。");
                }
                if(value < IPEndPoint.MinPort || value > IPEndPoint.MaxPort)
                {
                    new InvalidOperationException("ポートは" + 
                                                  IPEndPoint.MinPort.ToString() + "～" +
                                                  IPEndPoint.MaxPort.ToString() + 
                                                  "の範囲で指定してください。");
                }
                portNo = value;
            }
        }

        #endregion

        #region ReciveBufferSizeプロパティ

        private int reciveBufferSize = 4096;
        /// <summary>
        /// 受信用バッファのサイズを参照または設定します。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        [
            Category("動作"),
            DefaultValue(4096),
            Description("受信用バッファのサイズを参照または設定します。")
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

        #region SendToAddressプロパティ

        private string sendToAddress = "";
        private IPAddress sendToIP = null;
        /// <summary>
        /// 既定の送信先コンピュータ名またはIPアドレスを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(""),
            Description("既定の送信先コンピュータ名またはIPアドレスを参照または設定します。")
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

        /// <summary>
        /// 通信ポートの状態表示に使用される背景色を参照または設定します。
        /// </summary>
        [
            Category("表示"),
            Description("通信ポートの状態表示に使用される背景色を参照または設定します。")
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
        }

        #endregion

        #region IsOpenプロパティ

        private bool isOpen = false;
        /// <summary>
        /// 通信ポートをオープンしているかを参照します。
        /// </summary>
        [
            Category("状態"),
            Description("通信ポートをオープンしているかを参照します。")
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
        /// <summary>
        /// 送受信データのダンプを動作ログに出力するかを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("送受信データのダンプを動作ログに出力するかを参照または設定します。")
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

        #region イベント

        #region サイズ変更イベント (SizeChanged)

        /// <summary>
        /// サイズ変更を通知するイベントです。
        /// </summary>
        /// <param name="e">イベントデータ</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ControlRelocation();
        }

        #endregion

        #region フォント変更イベント (FontChanged)

        /// <summary>
        /// フォント変更を通知するイベントです。
        /// </summary>
        /// <param name="e">イベントデータ</param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            labelRecive.Font = base.Font;
            labelSend.Font = base.Font;
            Refresh();
        }

        #endregion

        #region 前景色変更イベント (ForeColorChanged)

        /// <summary>
        /// 前景色変更を通知するイベントです。
        /// </summary>
        /// <param name="e">イベントデータ</param>
        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            labelRecive.ForeColor = base.ForeColor;
            labelSend.ForeColor = base.ForeColor;
            Refresh();
        }

        #endregion

        #region 背景色変更イベント (backColors_ColorChange)

        /// <summary>
        /// 背景色変更を通知するイベントです。
        /// </summary>
        /// <param name="sender">発生元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void backColors_ColorChange(object sender, UdpPort.BackColors.ColorChangeEventArgs e)
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

        #region 再描画イベント (Paint)

        /// <summary>
        /// 再描画イベントです。
        /// </summary>
        /// <param name="e">イベントデータ</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlRelocation();
        }

        #endregion

        #region データ受信イベント (Receive)

        /// <summary>
        /// Receiveイベントのイベントデータを提供します。
        /// </summary>
        public class ReceiveEventArgs : EventArgs
        {
            /// <summary>
            /// 送信元IPアドレス
            /// </summary>
            public IPAddress IpAddress;

            /// <summary>
            /// 受信データ
            /// </summary>
            public byte[] ReceiveData;
        }
        public delegate void ReceiveEventHandler(object sender, ReceiveEventArgs e);
        /// <summary>
        /// データを受信したときに発生するイベントです。
        /// </summary>
        [
            Category("動作"),
            Description("データを受信したときに発生するイベントです。")
        ]
        public event ReceiveEventHandler Receive;

        #endregion

        //        #region イベント



        //        #region データ受信イベント (comPort_DataReceived)

        //        /// <summary>
        //        /// データ受信を通知するイベントです。
        //        /// </summary>
        //        /// <param name="sender">発生元オブジェクト</param>
        //        /// <param name="e">イベントデータ</param>
        //        private void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //        {
        //            int rcvlen = 0;

        //            labelRecive.BackColor = backColors.ReciveOn;
        //            Application.DoEvents();

        //            // Mutexの所有権を得る - ∞秒待つ
        //            mutex.WaitOne(-1, false);

        //            // 受信
        //            try
        //            {
        //                rcvlen = comPort.Read(reciveBuffer, reciveBufferDataLength, reciveBuffer.Length - reciveBufferDataLength);

        //                // 受信データサイズを更新
        //                reciveBufferDataLength += rcvlen;
        //            }
        //            catch
        //            {
        //            }

        //            // Mutexの所有権を解放
        //            mutex.ReleaseMutex();

        //            if (reciveBufferDataLength == 0)
        //            {
        //                // 受信データなし
        //                labelRecive.BackColor = backColors.ReciveOff;
        //                Application.DoEvents();
        //                return;
        //            }

        //            // 受信通知イベント
        //            RecivedEventArgs eArgs = new RecivedEventArgs();
        //            eArgs.DataCount = reciveBufferDataLength;
        //            eArgs.FreeCount = reciveBuffer.Length - reciveBufferDataLength;
        //            if (dataDump == true)
        //            {
        //                LogPrint("Status[RCV:" + eArgs.DataCount.ToString() + ",FREE:" + eArgs.FreeCount.ToString() + "]");
        //            }
        //            if (Recived != null)
        //            {
        //                // ハンドラが設定されていたらイベント発生
        //                CallRecivedEvent(this, eArgs);
        //            }

        //            if (reciveBuffer.Length - reciveBufferDataLength < 1)
        //            {
        //                labelRecive.BackColor = backColors.BufferFull;
        //                LogPrint("受信バッファがいっぱいです");
        //            }
        //            else
        //            {
        //                labelRecive.BackColor = backColors.ReciveOff;
        //            }
        //            Application.DoEvents();
        //        }

        //        #endregion

        //        #region データ受信通知イベント (Recived)

        //        /// <summary>
        //        /// Recivedイベントのイベントデータを提供します。
        //        /// </summary>
        //        public class RecivedEventArgs : EventArgs
        //        {
        //            /// <summary>
        //            /// バッファ内のデータバイト数
        //            /// </summary>
        //            public int DataCount;
        //            /// <summary>
        //            /// バッファの空きバイト数
        //            /// </summary>
        //            public int FreeCount;
        //        }
        //        public delegate void RecivedEventHandler(object sender, RecivedEventArgs e);
        //        /// <summary>
        //        /// データ受信したことを通知するイベントです。環境により別スレッドで発生します。
        //        /// </summary>
        //        [
        //            Category("動作"),
        //            Description("データ受信したことを通知するイベントです。環境により別スレッドで発生します。")
        //        ]
        //        public event RecivedEventHandler Recived;

        //        #endregion

        #endregion

        #region メソッド

        #region ホスト名取得 (GetHostName)

        /// <summary>
        /// 自ホスト名を取得します。
        /// </summary>
        /// <returns>結果(成功=ホスト名, 失敗="")</returns>
        public string GetHostName()
        {
            string name = "";
            try
            {
                name = Dns.GetHostName();
            }
            catch
            {
            }
            return name;
        }

        #endregion

        #region ホスト名取得 (GetHostName)

        /// <summary>
        /// 指定したIPアドレスのホスト名を取得します。
        /// </summary>
        /// <param name="IPAddr">対象のIPアドレス</param>
        /// <returns>結果(成功=ホスト名, 失敗="")</returns>
        public string GetHostName(IPAddress IPAddr)
        {
            string name = "";
            try
            {
                System.Net.IPHostEntry iphe = System.Net.Dns.GetHostEntry(IPAddr);
                name = iphe.HostName;
            }
            catch
            {
            }
            return name;
        }

        #endregion

        #region ローカルIPアドレス取得 (GetLocalIPAddressList)

        /// <summary>
        /// このホストのIPアドレス(IPv4)の一覧を取得します。
        /// </summary>
        /// <returns>結果(成功=IPアドレスリスト, 失敗=null)</returns>
        public IPAddress[] GetLocalIPAddressList()
        {
            IPAddress[] localIpAddress = null;
            ArrayList lst = new ArrayList();

            try
            {
                // ネットワークインタフェースリストを取得
                NetworkInterface[] netIfEnt = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface nif in netIfEnt)
                {
                    if (nif.NetworkInterfaceType == NetworkInterfaceType.Ethernet && nif.OperationalStatus == OperationalStatus.Up)
                    {
                        IPInterfaceProperties ipp = nif.GetIPProperties();
                        foreach (UnicastIPAddressInformation uif in ipp.UnicastAddresses)
                        {
                            IPAddress uip = uif.Address;
                            if (uip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                // ローカルIPアドレスを取得
                                lst.Add(uip);
                            }
                        }
                    }
                }
                if(lst.Count > 0)
                {
                    localIpAddress = new IPAddress[lst.Count];
                    for(int i=0;i < lst.Count;i++)
                    {
                        localIpAddress[i] = lst[i] as IPAddress;
                    }
                }
            }
            catch (Exception ex)
            {
                xlog.Print("ローカルIPアドレス取得に失敗しました - " + ex.Message);
            }

            return localIpAddress;
        }

        #endregion

        #region プロードキャストアドレス取得 (GetBroadcastAddress)

        /// <summary>
        /// 指定したIPアドレスのブロードキャストアドレス(IPv4)を取得します。
        /// </summary>
        /// <param name="localIPAddr">対象のIPアドレス</param>
        /// <returns>結果(成功=ブロードキャストアドレス, 失敗=null)</returns>
        public IPAddress GetBroadcastAddress(IPAddress localIPAddr)
        {
            IPAddress broadcastAddress = null;

            // ローカルIPアドレスおよびサブネットを取得
            IPAddress subnetMask = GetLocalSubnetMask(localIPAddr);

            if (localIPAddr == null || subnetMask == null)
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
                    if (strBroadcastAddress.Length > 0)
                    {
                        strBroadcastAddress += ".";
                    }
                    strBroadcastAddress += (ip[i] & subnet[i] | ((Byte)~subnet[i])).ToString();
                }
                broadcastAddress = IPAddress.Parse(strBroadcastAddress);
            }
            catch (Exception ex)
            {
                Print("ブロードキャストアドレス生成に失敗しました - " + ex.Message);
            }

            return broadcastAddress;
        }

        #endregion

        #region ローカルサブネットマスク取得 (GetLocalSubnetMask)

        /// <summary>
        /// このホストのサブネットマスク(IPv4)を取得します。
        /// </summary>
        /// <param name="localIPAddr">対象のIPアドレス</param>
        /// <returns>結果(成功=サブネットマスク, 失敗=null)</returns>
        public IPAddress GetLocalSubnetMask(IPAddress localIPAddr)
        {
            IPAddress localSubnet = null;

            try
            {
                string ip = localIPAddr.ToString();

                // ローカルのサブネットを取得
                ManagementObjectSearcher mos = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled=TRUE");
                ManagementObjectCollection moc = mos.Get();
                foreach (ManagementObject mo in moc)
                {
                    String[] mip = (String[])mo["IPAddress"];
                    if (ip == mip[0])
                    {
                        String[] msn = (String[])mo["IPSubnet"];
                        localSubnet = IPAddress.Parse(msn[0]);
                        break;
                    }
                }
                if (localSubnet == null)
                {
                    throw new Exception("有効なインターフェースが見つかりません。");
                }
            }
            catch (Exception ex)
            {
                Print("サブネットマスク取得に失敗しました - " + ex.Message);
            }

            return localSubnet;
        }

        #endregion

        #region Pingテスト (IsPingReply)

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
                Print("ping " + host);
                System.Net.NetworkInformation.PingReply reply = p.Send(host, toutms);
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    Print("ping 成功 - " + reply.Address.ToString() + " からの応答: バイト数 =" +
                                           reply.Buffer.Length.ToString() + " 時間 =" +
                                           reply.RoundtripTime.ToString() + "ms TTL=" +
                                           reply.Options.Ttl.ToString());
                }
                else
                {
                    Print("ping 失敗 - " + reply.Status.ToString());
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

        #region 通信ポートオープン (Open)

        /// <summary>
        /// 通信ポートをオープンします。
        /// </summary>
        /// <returns>結果(True=成功, False=失敗)</returns>
        /// <exception cref="System.Threading.ThreadStateException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        public bool Open()
        {
            bool status = false;
            
            if (isOpen == false)
            {
                try
                {
                    if (udp == null)        udp = new UdpClient(portNo);
                    if (rcvHelper == null)  rcvHelper = new ReceiveHelper(udp,  Receive, portNo);
                    if (rcvThread == null)  rcvThread = new Thread(rcvHelper.DataReceiveThread);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (rcvThread.IsAlive != true)
                {
                    try
                    {
                        // バックグラウンドでの処理に設定
                        rcvThread.IsBackground = true;

                        // リスナ起動
                        Print("データ受信スレッドを開始します。");

                        rcvThread.Start();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                Print("通信ポートはすでにオープンされています");
            }
            return status;
        }

        #endregion

        #region クローズ (Close)

        /// <summary>
        /// 通信ポートをクローズします。
        /// </summary>
        /// <exception cref="System.Threading.ThreadStateException"></exception>
        /// <exception cref="System.Threading.ThreadInterruptedException"></exception>
        public void Close()
        {
            if (isOpen)
            {
                try
                {
                    //データ受信スレッドを停止(自前)
                    if (rcvThread.ThreadState == ThreadState.Running)
                    {
                        rcvHelper.ReceiveStop();

                        rcvThread.Join();
                        Print("データ受信スレッドを停止しました。");
                    }

                    if (rcvThread.IsAlive)
                    {
                        rcvHelper.ReceiveStop();

                        rcvThread.Join();
                        Print("データ受信スレッドを停止しました。");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    rcvHelper = null;
                    rcvThread = null;
                    udp = null;
                    isOpen = false;
                }
            }
            else
            {
            }
        }

        #endregion

        #region バイナリデータ送信 (SendBytes)

        /// <summary>
        /// SendToAddressプロパティに指定したIPアドレスへバイナリデータを送信します。
        /// </summary>
        /// <param name="data">送信データ</param>
        /// <param name="count">送信バイト数</param>
        public void SendBytes(byte[] data, int count)
        {
            SendBytes(sendToIP, data, 0, count);
        }

        /// <summary>
        /// バイナリデータを送信します。
        /// </summary>
        /// <param name="ipAddress">送信先IPアドレス</param>
        /// <param name="data">送信データ</param>
        /// <param name="count">送信バイト数</param>
        public void SendBytes(IPAddress ipAddress, byte[] data, int count)
        {
            SendBytes(ipAddress, data, 0, count);
        }

        /// <summary>
        /// SendToAddressプロパティに指定したIPアドレスへバイナリデータを送信します。
        /// </summary>
        /// <param name="data">送信データ</param>
        /// <param name="offset">送信開始位置</param>
        /// <param name="count">送信バイト数</param>
        public void SendBytes(byte[] data, int offset, int count)
        {
            SendBytes(sendToIP, data, offset, count);
        }

        /// <summary>
        /// バイナリデータを送信します。
        /// </summary>
        /// <param name="ipAddress">送信先IPアドレス</param>
        /// <param name="data">送信データ</param>
        /// <param name="offset">送信開始位置</param>
        /// <param name="count">送信バイト数</param>
        public void SendBytes(IPAddress ipAddress, byte[] data, int offset, int count)
        {
            if (isOpen == true)
            {
                // データダンプ
                if (dataDump == true && xlog != null)
                {
                    try
                    {
                        xlog.HexDump("送信データ", data, offset, count);
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
                    udp.Send(dataofs, count, ep);
                }
                catch (Exception es)
                {
                    Print("送信に失敗しました - " + es.Message);
                }
                finally
                {
                    labelSend.BackColor = backColors.SendOff;
                    Application.DoEvents();
                }
            }
            else
            {
                Print("通信ポートをオープンしていないため送信できません");
            }
        }

        #endregion

        #region テキストデータ送信 (SendLine)

        /// <summary>
        /// SendToAddressプロパティに指定したIPアドレスへテキストデータを送信します。
        /// </summary>
        /// <param name="text">１行のデータ</param>
        public void SendLine(string text)
        {
            SendLine(sendToIP, text);
        }

        /// <summary>
        /// テキストデータを送信します。
        /// </summary>
        /// <param name="ipAddress">送信先IPアドレス</param>
        /// <param name="text">１行のデータ</param>
        public void SendLine(IPAddress ipAddress, string text)
        {
            byte[] endchar = Encoding.Unicode.GetBytes(System.Environment.NewLine);
            SendLine(ipAddress, text, Encoding.Unicode, endchar, endchar.Length);
        }

        /// <summary>
        /// SendToAddressプロパティに指定したIPアドレスへテキストデータを送信します。
        /// </summary>
        /// <param name="text">１行のデータ</param>
        /// <param name="encoding">テキストエンコーディング</param>
        /// <param name="endchar">行末文字配列</param>
        /// <param name="length">行末文字数</param>
        public void SendLine(string text, Encoding encoding, byte[] endchar, int length)
        {
             SendLine(sendToIP, text, encoding, endchar, length);
        }

        /// <summary>
        /// テキストデータを送信します。
        /// </summary>
        /// <param name="ipAddress">送信先IPアドレス</param>
        /// <param name="text">１行のデータ</param>
        /// <param name="encoding">テキストエンコーディング</param>
        /// <param name="endchar">行末文字配列</param>
        /// <param name="length">行末文字数</param>
        public void SendLine(IPAddress ipAddress, string text, Encoding encoding, byte[] endchar, int length)
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
            SendBytes(ipAddress, buf, buf.Length);
        }

        #endregion

        #endregion

        #region 内部処理

        #region コントロール再配置 (ControlRelocation)

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

        #region ログ出力 (Print)
        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="msg">出力するメッセージ</param>
        private void Print(string msg)
        {
            if (xlog != null)
            {
                try
                {
                    xlog.Print(msg);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion
        
        #endregion
    }
}



//        #region メソッド




//        #region バイナリデータ取得 (GetReciveBytes)

//        delegate int GetReciveBytesDelegate(byte[] data, int offset, int count);

//        /// <summary>
//        /// バイナリデータを取得します。
//        /// </summary>
//        /// <returns>取得バイト数または-1(エラー値)</returns>
//        /// <param name="data">取得データ</param>
//        /// <param name="offset">格納開始位置</param>
//        /// <param name="count">格納可能バイト数</param>
//        public int GetReciveBytes(byte[] data, int offset, int count)
//        {
//            if (InvokeRequired)
//            {
//                // 別スレッドから呼び出された場合
//                object[] param = {data, offset, count};
//                return (int)Invoke(new GetReciveBytesDelegate(GetReciveBytes), param);
//            }
            
//            // データを取得
//            int retlen = getReciveBytes(data, offset, count);

//            return retlen;
//        }

//        #endregion

//        #region バイナリデータ取得(終端文字指定) (GetReciveLineBytes)

//        delegate byte[] GetReciveLineBytesDelegate(byte[] endchar, int length);

//        /// <summary>
//        /// 終端文字を指定してバイナリデータを取得します。
//        /// </summary>
//        /// <returns>１行分のバイナリデータ(データが無いときはnullが返ります)</returns>
//        /// <param name="endchar">行末文字配列</param>
//        /// <param name="length">行末文字数</param>
//        public byte[] GetReciveLineBytes(byte[] endchar, int length)
//        {
//            if (InvokeRequired)
//            {
//                // 別スレッドから呼び出された場合
//                object[] param = { endchar, length };
//                return (byte[])Invoke(new GetReciveLineBytesDelegate(GetReciveLineBytes), param);
//            }

//            // Mutexの所有権を得る - ∞秒待つ
//            mutex.WaitOne(-1, false);

//            // 行の長さを取得
//            int retlen = findLineLength(endchar, length);
//            if (retlen < 0)
//            {
//                // Mutexの所有権を解放
//                mutex.ReleaseMutex();
//                return null;
//            }

//            // データを取得
//            byte[] buf = new byte[retlen];
//            getReciveBytes(buf, 0, retlen);

//            // Mutexの所有権を解放
//            mutex.ReleaseMutex();

//            return buf;
//        }

//        #endregion

//        #region テキストデータ取得 (GetReciveLine)

//        delegate string GetReciveLineDelegate(Encoding encoding, byte[] endchar, int length);

//        /// <summary>
//        /// テキストデータを取得します。
//        /// </summary>
//        /// <returns>１行のデータ</returns>
//        /// <param name="encoding">テキストエンコーディング</param>
//        /// <param name="endchar">行末文字配列</param>
//        /// <param name="length">行末文字数</param>
//        public string GetReciveLine(Encoding encoding, byte[] endchar, int length)
//        {
//            string rcvtext = "";

//            // データを取得
//            byte[] buf = GetReciveLineBytes(endchar, length);

//            // 文字列に変換
//            if (buf != null)
//            {
//                rcvtext = encoding.GetString(buf);
//            }

//            return rcvtext;
//        }

//        #endregion

//        #endregion

//        #region 内部処理


//        #region データ取得 (getReciveBytes)

//        /// <summary>
//        /// データを取得します。
//        /// </summary>
//        /// <returns>取得バイト数または-1(エラー値)</returns>
//        /// <param name="data">取得データ</param>
//        /// <param name="offset">格納開始位置</param>
//        /// <param name="count">格納可能バイト数</param>
//        private int getReciveBytes(byte[] data, int offset, int count)
//        {
//            int retlen = 0;

//            if (comPort.IsOpen == true)
//            {
//                // Mutexの所有権を得る - ∞秒待つ
//                mutex.WaitOne(-1, false);

//                // 返却サイズ計算
//                retlen = count;
//                if (retlen > reciveBufferDataLength)
//                {
//                    retlen = reciveBufferDataLength;
//                }

//                try
//                {
//                    // データ格納
//                    for (int i = 0; i < retlen; i++)
//                    {
//                        data[offset + i] = reciveBuffer[i];
//                    }

//                    // データシフト
//                    for (int i = 0; i < reciveBufferDataLength - retlen; i++)
//                    {
//                        reciveBuffer[i] = reciveBuffer[i + retlen];
//                    }

//                    // 受信データサイズを更新
//                    reciveBufferDataLength -= retlen;

//                    // データダンプ
//                    if (dataDump == true && xlog != null)
//                    {
//                        try
//                        {
//                            xlog.HexDump("受信データ", data, offset, retlen);
//                        }
//                        catch
//                        {
//                        }
//                    }
//                }
//                catch (Exception es)
//                {
//                    LogPrint("データの取得に失敗しました - " + es.Message);
//                    retlen = -1;
//                }
//                finally
//                {
//                }

//                // Mutexの所有権を解放
//                mutex.ReleaseMutex();
//            }
//            else
//            {
//                LogPrint("通信ポートをオープンしていないため受信できません");
//                retlen = -1;
//            }

//            return retlen;
//        }

//        #endregion

//        #region 行末検索 (findLineLength)

//        /// <summary>
//        /// 行末を検索します。
//        /// </summary>
//        /// <returns>行末の区切り文字の終端位置または-1(エラー値)</returns>
//        /// <param name="endchar">区切り文字配列</param>
//        /// <param name="length">区切り文字数</param>
//        private int findLineLength(byte[] endchar, int length)
//        {
//            int linelength = -1;

//            if (comPort.IsOpen == true)
//            {
//                // Mutexの所有権を得る - ∞秒待つ
//                mutex.WaitOne(-1, false);

//                // 検索
//                for (int i = 0; i <= reciveBufferDataLength - length; i++)
//                {
//                    if (reciveBuffer[i] == endchar[0])
//                    {
//                        int match = 0;
//                        for (int j = 0; j < length; j++)
//                        {
//                            if (reciveBuffer[i + j] == endchar[j])
//                            {
//                                match++;
//                            }
//                            else
//                            {
//                                break;
//                            }
//                        }
//                        if (match == length)
//                        {
//                            linelength = i + length;
//                            break;
//                        }
//                    }
//                    if (comPort.IsOpen == false)
//                    {
//                        LogPrint("通信ポートをオープンしていないため受信できません");
//                        linelength = -1;
//                        break;
//                    }
//                }

//                // Mutexの所有権を解放
//                mutex.ReleaseMutex();
//            }
//            else
//            {
//                LogPrint("通信ポートをオープンしていないため受信できません");
//                linelength = -1;
//            }

//            return linelength;
//        }

//        #endregion

//        #region データ受信通知 (CallReciveEvent)

//        delegate void CallRecivedEventDelegate(object sender, RecivedEventArgs e);

//        /// <summary>
//        /// データ受信イベントを発生させます。
//        /// </summary>
//        /// <param name="sender">発生元オブジェクト</param>
//        /// <param name="e">イベントデータ</param>
//        private void CallRecivedEvent(object sender, RecivedEventArgs e)
//        {
//            if (InvokeRequired)
//            {
//                // 別スレッドから呼び出された場合
//                object[] param = { sender, e };
//                Invoke(new CallRecivedEventDelegate(CallRecivedEvent), param);
//                return;
//            }

//            try
//            {
//                Recived(sender, e);
//            }
//            catch
//            {
//            }
//        }

//        #endregion

//        #region 操作ログ出力 (LogPrint)

//        /// <summary>
//        /// 操作ログを出力します。
//        /// </summary>
//        /// <param name="msg">出力する内容</param>
//        private void LogPrint(string msg)
//        {
//            if (xlog != null)
//            {
//                try
//                {
//                    xlog.Print("[" + this.Name + "] " + msg);
//                }
//                catch
//                {
//                }
//            }
//        }

//        #endregion

//        #region ダンプ出力 (LogHexDump)

//        /// <summary>
//        /// データダンプを出力します。
//        /// </summary>
//        /// <param name="msg">出力する内容の説明</param>
//        /// <param name="data">出力するバイトデータ配列</param>
//        /// <param name="offset">バイト単位の0から始まる開始位置</param>
//        /// <param name="count">バイト単位の出力サイズ</param>
//        private void LogHexDump(string msg, byte[] data, int offset, int count)
//        {
//            if (xlog != null)
//            {
//                try
//                {
//                    xlog.HexDump("[" + this.Name + "] " + msg, data, offset, count);
//                }
//                catch
//                {
//                }
//            }
//        }

//        #endregion

//        #endregion
//    }
//}


//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Drawing;
//using System.Data;
//using System.Globalization;
//using System.Management;
//using System.Net;
//using System.Net.Sockets;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading;
//using System.Windows.Forms;




//        #region メソッド






//        #region バイナリデータ送信

//        /// <summary>
//        /// バイナリデータを送信します。
//        /// </summary>
//        /// <param name="data">送信データ</param>
//        /// <param name="count">送信バイト数</param>
//        public void SendBytes(byte[] data, int count)
//        {
//            SendBytes(data, 0, count);
//        }

//        /// <summary>
//        /// バイナリデータを送信します。
//        /// </summary>
//        /// <param name="data">送信データ</param>
//        /// <param name="offset">送信開始位置</param>
//        /// <param name="count">送信バイト数</param>
//        public void SendBytes(byte[] data, int offset, int count)
//        {
//            if (isOpen == true)
//            {
//                // データダンプ
//                if (dataDump == true && xConsoleBox != null)
//                {
//                    try
//                    {
//                        xConsoleBox.LogHexDump("送信データ", data, offset, count);
//                    }
//                    catch
//                    {
//                    }
//                }

//                // 送信
//                labelSend.BackColor = backColors.SendOn;
//                Application.DoEvents();

//                try
//                {
//                    IPEndPoint ep = new IPEndPoint(sendToIP, portNo);
//                    byte[] dataofs = new byte[count];
//                    for (int i = 0; i < dataofs.Length; i++)
//                    {
//                        dataofs[i] = data[offset + i];
//                    }
//                    udp.Send(dataofs, count, ep);
//                }
//                catch (Exception es)
//                {
//                    LogPrint("送信に失敗しました - " + es.Message);
//                }
//                finally
//                {
//                    labelSend.BackColor = backColors.SendOff;
//                    Application.DoEvents();
//                }
//            }
//            else
//            {
//                LogPrint("通信ポートをオープンしていないため送信できません");
//            }
//        }

//        #endregion

//        #region テキストデータ送信

//        /// <summary>
//        /// テキストデータを送信します。
//        /// </summary>
//        /// <param name="text">１行のデータ</param>
//        /// <param name="encoding">テキストエンコーディング</param>
//        /// <param name="endchar">行末文字配列</param>
//        /// <param name="length">行末文字数</param>
//        public void SendLine(string text, Encoding encoding, byte[] endchar, int length)
//        {
//            // テキストをバイト列に変換
//            int size = encoding.GetByteCount(text);
//            int len = endchar.Length;
//            if (len > length)
//            {
//                len = length;
//            }

//            byte[] buf = new byte[size + len];
//            encoding.GetBytes(text, 0, text.Length, buf, 0);

//            // 行末文字を付加
//            for (int i = 0; i < len; i++)
//            {
//                buf[size + i] = endchar[i];
//            }

//            // 送信
//            SendBytes(buf, buf.Length);
//        }

//        #endregion

//        #region バイナリデータ取得

//        delegate int GetReciveBytesDelegate(byte[] data, int offset, int count);

//        /// <summary>
//        /// バイナリデータを取得します。
//        /// </summary>
//        /// <returns>取得バイト数または-1(エラー値)</returns>
//        /// <param name="data">取得データ</param>
//        /// <param name="offset">格納開始位置</param>
//        /// <param name="count">格納可能バイト数</param>
//        public int GetReciveBytes(byte[] data, int offset, int count)
//        {
//            if (InvokeRequired)
//            {
//                // 別スレッドから呼び出された場合
//                object[] param = { data, offset, count };
//                return (int)Invoke(new GetReciveBytesDelegate(GetReciveBytes), param);
//            }

//            // データを取得
//            int retlen = getReciveBytes(data, offset, count);

//            return retlen;
//        }

//        #endregion

//        #region バイナリデータ取得(終端文字指定)

//        delegate byte[] GetReciveLineBytesDelegate(byte[] endchar, int length);

//        /// <summary>
//        /// 終端文字を指定してバイナリデータを取得します。
//        /// </summary>
//        /// <returns>１行分のバイナリデータ(データが無いときはnullが返ります)</returns>
//        /// <param name="endchar">行末文字配列</param>
//        /// <param name="length">行末文字数</param>
//        public byte[] GetReciveLineBytes(byte[] endchar, int length)
//        {
//            if (InvokeRequired)
//            {
//                // 別スレッドから呼び出された場合
//                object[] param = { endchar, length };
//                return (byte[])Invoke(new GetReciveLineBytesDelegate(GetReciveLineBytes), param);
//            }

//            // Mutexの所有権を得る - ∞秒待つ
//            mutex.WaitOne(-1, false);

//            // 行の長さを取得
//            int retlen = findLineLength(endchar, length);
//            if (retlen < 0)
//            {
//                // Mutexの所有権を解放
//                mutex.ReleaseMutex();
//                return null;
//            }

//            // データを取得
//            byte[] buf = new byte[retlen];
//            getReciveBytes(buf, 0, retlen);

//            // Mutexの所有権を解放
//            mutex.ReleaseMutex();

//            return buf;
//        }

//        #endregion

//        #region テキストデータ取得

//        delegate string GetReciveLineDelegate(Encoding encoding, byte[] endchar, int length);

//        /// <summary>
//        /// テキストデータを取得します。
//        /// </summary>
//        /// <returns>１行のデータ</returns>
//        /// <param name="encoding">テキストエンコーディング</param>
//        /// <param name="endchar">行末文字配列</param>
//        /// <param name="length">行末文字数</param>
//        public string GetReciveLine(Encoding encoding, byte[] endchar, int length)
//        {
//            string rcvtext = "";

//            // データを取得
//            byte[] buf = GetReciveLineBytes(endchar, length);

//            // 文字列に変換
//            if (buf != null)
//            {
//                rcvtext = encoding.GetString(buf);
//            }

//            return rcvtext;
//        }

//        #endregion

//        #endregion



//        #region データ受信スレッド

//        /// <summary>
//        /// UDP受信スレッド
//        /// </summary>
//        private void UDPReciveThread()
//        {
//            int rcvlen = 0;
//            int cpylen = 0;
//            IPAddress fromIP = null;
//            IPEndPoint remoteEP = null;     // 受信データ（送信IPアドレス、送信ポート番号など）
//            byte[] rcvBytes = new byte[0];  // 受信データ

//            try
//            {
//                LogPrint("UDP受信スレッド開始しました");
//                while (true)
//                {
//                    // 受信ループ(Abort例外で終了)

//                    // 未コピーデータがなければ受信
//                    if (rcvBytes.Length == cpylen)
//                    {
//                        if (udp.Available == 0)
//                        {
//                            rcvBytes = new byte[0];
//                        }
//                        else
//                        {
//                            rcvBytes = udp.Receive(ref remoteEP);
//                            fromIP = remoteEP.Address;
//                        }
//                        cpylen = 0;
//                    }

//                    // 未コピーデータがあり、バッファが空いていれば格納しイベントを上げる
//                    if ((reciveBuffer.Length != reciveBufferDataLength) && (rcvBytes.Length != cpylen))
//                    {
//                        labelRecive.BackColor = backColors.ReciveOn;
//                        Application.DoEvents();

//                        // Mutexの所有権を得る - ∞秒待つ
//                        mutex.WaitOne(-1, false);

//                        // 今回格納可能なデータ長計算
//                        rcvlen = reciveBuffer.Length - reciveBufferDataLength;
//                        if (rcvlen > rcvBytes.Length - cpylen)
//                        {
//                            rcvlen = rcvBytes.Length - cpylen;
//                        }

//                        // 格納
//                        Array.Copy(rcvBytes, cpylen, reciveBuffer, reciveBufferDataLength, rcvlen);

//                        // 受信データサイズを更新
//                        reciveBufferDataLength += rcvlen;
//                        cpylen += rcvlen;

//                        // Mutexの所有権を解放
//                        mutex.ReleaseMutex();

//                        if (reciveBufferDataLength == 0)
//                        {
//                            // 受信データなし
//                            labelRecive.BackColor = backColors.ReciveOff;
//                            Application.DoEvents();
//                            return;
//                        }

//                        // 受信通知イベント
//                        ReciveEventArgs eArgs = new ReciveEventArgs();
//                        eArgs.FromIP = fromIP;
//                        eArgs.DataCount = reciveBufferDataLength;
//                        eArgs.FreeCount = reciveBuffer.Length - reciveBufferDataLength;
//                        if (dataDump == true)
//                        {
//                            LogPrint("Status[RCV:" + eArgs.DataCount.ToString() + ",FREE:" + eArgs.FreeCount.ToString() + "]");
//                        }
//                        if (ReciveEvent != null)
//                        {
//                            // ハンドラが設定されていたらイベント発生
//                            CallReciveEvent(this, eArgs);
//                        }

//                        if (reciveBuffer.Length - reciveBufferDataLength < 1)
//                        {
//                            labelRecive.BackColor = backColors.BufferFull;
//                            LogPrint("受信バッファがいっぱいです");
//                        }
//                        else
//                        {
//                            labelRecive.BackColor = backColors.ReciveOff;
//                        }

//                        Application.DoEvents();
//                    }
//                }
//            }
//            catch (ThreadAbortException)
//            {
//                // スレッド終了
//                LogPrint("UDP受信スレッド停止しました");
//            }
//            catch (Exception es)
//            {
//                LogPrint("UDP受信スレッドで例外が発生しました - " + es.Message);
//                throw es;
//            }
//        }

//        #endregion

//        #region 追加のイベント

//        #region データ受信通知

//        /// <summary>
//        /// データ受信通知イベント
//        /// </summary>
//        public class ReciveEventArgs : EventArgs
//        {
//            public IPAddress FromIP;    // 最後に受信した送信元IPアドレス
//            public int DataCount;       // 受信データ長
//            public int FreeCount;       // 受信バッファの空き
//        }
//        public delegate void ReciveEventHandler(object sender, ReciveEventArgs e);
//        [
//            Category("動作"),
//            Description("データ受信したことを通知するイベントです。環境により別スレッドで発生します。")
//        ]
//        public event ReciveEventHandler ReciveEvent;

//        #endregion

//        #endregion

//        #endregion

//        #region その他



//        #region データ取得

//        /// <summary>
//        /// データを取得します。
//        /// </summary>
//        /// <returns>取得バイト数または-1(エラー値)</returns>
//        /// <param name="data">取得データ</param>
//        /// <param name="offset">格納開始位置</param>
//        /// <param name="count">格納可能バイト数</param>
//        private int getReciveBytes(byte[] data, int offset, int count)
//        {
//            int retlen = 0;

//            if (isOpen == true)
//            {
//                // Mutexの所有権を得る - ∞秒待つ
//                mutex.WaitOne(-1, false);

//                // 返却サイズ計算
//                retlen = count;
//                if (retlen > reciveBufferDataLength)
//                {
//                    retlen = reciveBufferDataLength;
//                }

//                try
//                {
//                    // データ格納
//                    for (int i = 0; i < retlen; i++)
//                    {
//                        data[offset + i] = reciveBuffer[i];
//                    }

//                    // データシフト
//                    for (int i = 0; i < reciveBufferDataLength - retlen; i++)
//                    {
//                        reciveBuffer[i] = reciveBuffer[i + retlen];
//                    }

//                    // 受信データサイズを更新
//                    reciveBufferDataLength -= retlen;

//                    // データダンプ
//                    if (dataDump == true && xConsoleBox != null)
//                    {
//                        try
//                        {
//                            xConsoleBox.LogHexDump("受信データ", data, offset, retlen);
//                        }
//                        catch
//                        {
//                        }
//                    }
//                }
//                catch (Exception es)
//                {
//                    LogPrint("データの取得に失敗しました - " + es.Message);
//                    retlen = -1;
//                }
//                finally
//                {
//                }

//                // Mutexの所有権を解放
//                mutex.ReleaseMutex();
//            }
//            else
//            {
//                LogPrint("通信ポートをオープンしていないため受信できません");
//                retlen = -1;
//            }

//            return retlen;
//        }

//        #endregion

//        #region 行末検索

//        /// <summary>
//        /// 行末を検索します。
//        /// </summary>
//        /// <returns>行末の区切り文字の終端位置または-1(エラー値)</returns>
//        /// <param name="endchar">区切り文字配列</param>
//        /// <param name="length">区切り文字数</param>
//        private int findLineLength(byte[] endchar, int length)
//        {
//            int linelength = -1;

//            if (isOpen == true)
//            {
//                // Mutexの所有権を得る - ∞秒待つ
//                mutex.WaitOne(-1, false);

//                // 検索
//                for (int i = 0; i <= reciveBufferDataLength - length; i++)
//                {
//                    if (reciveBuffer[i] == endchar[0])
//                    {
//                        int match = 0;
//                        for (int j = 0; j < length; j++)
//                        {
//                            if (reciveBuffer[i + j] == endchar[j])
//                            {
//                                match++;
//                            }
//                            else
//                            {
//                                break;
//                            }
//                        }
//                        if (match == length)
//                        {
//                            linelength = i + length;
//                            break;
//                        }
//                    }
//                    if (isOpen == false)
//                    {
//                        LogPrint("通信ポートをオープンしていないため受信できません");
//                        linelength = -1;
//                        break;
//                    }
//                }

//                // Mutexの所有権を解放
//                mutex.ReleaseMutex();
//            }
//            else
//            {
//                LogPrint("通信ポートをオープンしていないため受信できません");
//                linelength = -1;
//            }

//            return linelength;
//        }

//        #endregion

//        #region データ受信通知

//        delegate void CallReciveEventDelegate(object sender, ReciveEventArgs e);

//        private void CallReciveEvent(object sender, ReciveEventArgs e)
//        {
//            if (InvokeRequired)
//            {
//                // 別スレッドから呼び出された場合
//                object[] param = { sender, e };
//                Invoke(new CallReciveEventDelegate(CallReciveEvent), param);
//                return;
//            }

//            try
//            {
//                ReciveEvent(sender, e);
//            }
//            catch (Exception es)
//            {
//                LogPrint("ReciveEvent内で例外が発生しました - " + es.Message);
//            }
//        }

//        #endregion

//        #region ダンプ出力

//        private void LogHexDump(string msg, byte[] data, int offset, int count)
//        {
//            if (xConsoleBox != null)
//            {
//                try
//                {
//                    xConsoleBox.LogHexDump("[" + this.Name + "] " + msg, data, offset, count);
//                }
//                catch
//                {
//                }
//            }
//        }

//        #endregion






//        #endregion

//    }
//}

