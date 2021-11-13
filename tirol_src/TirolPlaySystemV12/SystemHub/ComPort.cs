using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace XControls.IO
{
    /// <summary>
    /// シリアル通信を行うためのクラスです。
    /// </summary>
    [
        DebuggerNonUserCode,
        Designer(typeof(ComPortDesigner)),
    ]
    public class ComPort : UserControl
    {
        #region インナークラス

        #region ComPortDesignerクラス

        /// <summary>
        /// ComPort用にデザイナをカスタマイズします。
        /// </summary>
        public class ComPortDesigner : System.Windows.Forms.Design.ControlDesigner
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.IO.ComPortDesigner クラスの新しいインスタンスを初期化します。
            /// </summary>
            public ComPortDesigner()
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

        #region SLabelクラス

        /// <summary>
        /// ステータス用ラベルコントロールです。
        /// </summary>
        public class SLabel : Label
        {
            #region イベント

            protected override void OnPaintBackground(PaintEventArgs pevent)
            {
                Redraw(pevent);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Redraw(e);
            }

            #endregion

            #region メソッド

            /// <summary>
            /// 別スレッドからの再描画を行います。
            /// </summary>
            public void Redraw()
            {
                Graphics g = this.CreateGraphics();
                PaintEventArgs e = new PaintEventArgs(g, this.ClientRectangle);
                Redraw(e);
                g.Dispose();
            }
            private void Redraw(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                SizeF siz = g.MeasureString(this.Text, this.Font);
                SolidBrush bc = new SolidBrush(base.BackColor);
                SolidBrush fc = new SolidBrush(this.ForeColor);
                g.FillRectangle(bc, 0, 0, this.Width, this.Height);
                TextFormatFlags tf = TextFormatFlags.Default;
                switch(this.TextAlign)
                {
                    case ContentAlignment.TopCenter:
                        tf = TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
                        break;
                    case ContentAlignment.TopLeft:
                        tf = TextFormatFlags.Top | TextFormatFlags.Left;
                        break;
                    case ContentAlignment.TopRight:
                        tf = TextFormatFlags.Top | TextFormatFlags.Right;
                        break;
                    case ContentAlignment.MiddleCenter:
                        tf = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                        break;
                    case ContentAlignment.MiddleLeft:
                        tf = TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                        break;
                    case ContentAlignment.MiddleRight:
                        tf = TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
                        break;
                    case ContentAlignment.BottomCenter:
                        tf = TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
                        break;
                    case ContentAlignment.BottomLeft:
                        tf = TextFormatFlags.Bottom | TextFormatFlags.Left;
                        break;
                    case ContentAlignment.BottomRight:
                        tf = TextFormatFlags.Bottom | TextFormatFlags.Right;
                        break;
                }
                TextRenderer.DrawText(g, this.Text, this.Font, new Rectangle(0, 0, this.Width, this.Height), this.ForeColor, base.BackColor, tf);
                bc.Dispose();
                fc.Dispose();
            }

            #endregion
        }

        #endregion

        #endregion

        #region 構築・破棄

        /// <summary>
        /// XControls.ComPort クラスの新しいインスタンスを初期化します。
        /// </summary>
        public ComPort()
        {
            #region コンポーネント初期化

            this.labelSend = new SLabel();
            this.labelRecive = new SLabel();
            this.stim = new System.Timers.Timer(50);
            this.rtim = new System.Timers.Timer();
            this.SuspendLayout();

            this.labelSend.AutoSize = false;
            this.labelSend.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelSend.Location = new System.Drawing.Point(3, 3);
            this.labelSend.Name = "labelSend";
            this.labelSend.Size = new System.Drawing.Size(12, 12);
            this.labelSend.TabIndex = 0;
            this.labelSend.Text = "S";
            this.labelSend.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.labelRecive.AutoSize = false;
            this.labelRecive.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelRecive.Location = new System.Drawing.Point(20, 3);
            this.labelRecive.Name = "labelRecive";
            this.labelRecive.Size = new System.Drawing.Size(12, 12);
            this.labelRecive.TabIndex = 1;
            this.labelRecive.Text = "R";
            this.labelRecive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.stim.Interval = 30;
            this.stim.AutoReset = true;
            this.stim.Elapsed += stim_Elapsed;

            this.rtim.Interval = 30;
            this.rtim.AutoReset = true;
            this.rtim.Elapsed += rtim_Elapsed;

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.labelRecive);
            this.Controls.Add(this.labelSend);
            this.DoubleBuffered = true;
            this.Name = "ComPort";
            this.Size = new System.Drawing.Size(36, 18);
            this.ResumeLayout(false);
            
            #endregion

            // 通信バッファサイズ初期設定
            comPort.ReadBufferSize = 4096;
            comPort.WriteBufferSize = 4096;

            // 画面再配置
            ControlRelocation();

            // イベント設定
            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
            backColors.ColorChange += new BackColors.ColorChangeEventHandler(backColors_ColorChange);
        }

         /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            #region 後処理

            if (disposing && (components != null))
            {
                comPort.Close();
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
        private SLabel labelSend;

        /// <summary>
        /// 受信モニタ用ラベルです。
        /// </summary>
        private SLabel labelRecive;

        /// <summary>
        /// 送信モニタ用タイマーです。
        /// </summary>
        private System.Timers.Timer stim;

        /// <summary>
        /// 受信モニタ用タイマーです。
        /// </summary>
        private System.Timers.Timer rtim;

        /// <summary>
        /// 動作ログ出力に使用するLoggingクラスのインスタンスです。
        /// </summary>
        private Logging xlog = null;

        /// <summary>
        /// ステータス背景色です。
        /// </summary>
        private BackColors backColors = new BackColors();

        /// <summary>
        /// ラベル内テキストの描画位置です。
        /// </summary>
        private Point drawpos = new Point(0, 0);

        /// <summary>
        /// 通信に使用するシリアルポートクラスのインスタンスです。
        /// </summary>
        private SerialPort comPort = new SerialPort();

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

        #region AllowVT100Emulationプロパティ

        private bool allowVT100Emulation = false;
        /// <summary>
        /// Recivedイベント時にVT100簡易エミュレーションを有効にするかどうかを示します。trueの場合、エスケープシーケンスが分割されないよう処理します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("Recivedイベント時にVT100簡易エミュレーションを有効にするかどうかを示します。trueの場合、エスケープシーケンスが分割されないよう処理します。")
        ]
        public bool AllowVT100Emulation
        {
            get
            {
                return allowVT100Emulation;
            }
            set
            {
                allowVT100Emulation = value;
            }
        }

        #endregion

        #region ArrowInvokeReciveEventプロパティ

        private bool arrowInvokeReciveEvent = true;
        /// <summary>
        /// データ受信イベント内で、InvokeRequired を処理するかどうかを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(true),
            Description("データ受信イベント内で、InvokeRequired を処理するかどうかを参照または設定します。")
        ]
        public bool ArrowInvokeReciveEvent
        {
            get
            {
                return arrowInvokeReciveEvent;
            }
            set
            {
                arrowInvokeReciveEvent = value;
            }
        }

        #endregion

        #region BaudRateプロパティ

        /// <summary>
        /// この通信ポートで使用するボーレートを参照または設定します。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        [
            Category("動作"),
            DefaultValue(115200),
            Description("この通信ポートで使用するボーレートを参照または設定します。")
        ]
        public int BaudRate
        {
            get
            {
                return comPort.BaudRate;
            }
            set
            {
                comPort.BaudRate = value;
            }
        }

        #endregion

        #region DataBitsプロパティ

        /// <summary>
        /// バイト毎のデータビットの標準の長さを参照または設定します。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        [
            Category("動作"),
            DefaultValue(8),
            Description("バイト毎のデータビットの標準の長さを参照または設定します。")
        ]
        public int DataBits
        {
            get
            {
                return comPort.DataBits;
            }
            set
            {
                comPort.DataBits = value;
            }
        }

        #endregion

        #region DtrEnableプロパティ

        /// <summary>
        /// 通信中にDTR(Data Terminal Ready)シグナルを有効にする値を参照または設定します。
        /// </summary>
        /// <exception cref="System.IO.IOException"></exception>
        [
            Category("動作"),
            DefaultValue(false),
            Description("通信中にDTR(Data Terminal Ready)シグナルを有効にする値を参照または設定します。")
        ]
        public bool DtrEnable
        {
            get
            {
                return comPort.DtrEnable;
            }
            set
            {
                comPort.DtrEnable = value;
            }
        }

        #endregion

        #region Handshakeプロパティ

        /// <summary>
        /// データ交換のフロー制御に使用するハンドシェイクプロトコルを参照または設定します。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        [
            Category("動作"),
            DefaultValue(Handshake.None),
            Description("データ交換のフロー制御に使用するハンドシェイクプロトコルを参照または設定します。")
        ]
        public Handshake Handshake
        {
            get
            {
                return comPort.Handshake;
            }
            set
            {
                comPort.Handshake = value;
            }
        }

        #endregion

        #region Parityプロパティ

        /// <summary>
        /// 各受信バイトのパリティチェックプロトコルを参照または設定します。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        [
            Category("動作"),
            DefaultValue(Parity.None),
            Description("各受信バイトのパリティチェックプロトコルを参照または設定します。")
        ]
        public Parity Parity
        {
            get
            {
                return comPort.Parity;
            }
            set
            {
                comPort.Parity = value;
            }
        }

        #endregion

        #region StopBitsプロパティ

        /// <summary>
        /// 各送受信バイト毎のストップビット数を参照または設定します。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        [
            Category("動作"),
            DefaultValue(StopBits.One),
            Description("各送受信バイト毎のストップビット数を参照または設定します。")
        ]
        public StopBits StopBits
        {
            get
            {
                return comPort.StopBits;
            }
            set
            {
                comPort.StopBits = value;
            }
        }

        #endregion

        #region PortNameプロパティ

        /// <summary>
        /// 使用する通信ポート名を参照または設定します。
        /// </summary>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        [
            Category("動作"),
            DefaultValue("COM1"),
            Description("使用する通信ポート名を参照または設定します。")
        ]
        public string PortName
        {
            get
            {
                return comPort.PortName;
            }
            set
            {
                try
                {
                    string val = value;
                    int n = val.IndexOf("(COM");
                    if (n >= 0)
                    {
                        val = val.Substring(n + 1);
                        n = val.IndexOf(")");
                        val = val.Substring(0, n);
                    }
                    comPort.PortName = val;
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
                if (comPort.IsOpen == true)
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

        /// <summary>
        /// 通信ポートをオープンしているかを参照します。
        /// </summary>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        [
            Category("状態"),
            Description("通信ポートをオープンしているかを参照します。")
        ]
        public bool IsOpen
        {
            get
            {
                return comPort.IsOpen;
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

        #region SendByteWaitプロパティ

        private int sendByteWait = 0;
        /// <summary>
        /// 送信時の文字単位の待ち時間(ミリ秒単位)を参照または設定します。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        [
            Category("動作"),
            DefaultValue(false),
            Description("送信時の文字単位の待ち時間(ミリ秒単位)を参照または設定します。")
        ]
        public int SendByteWait
        {
            get
            {
                return sendByteWait;
            }
            set
            {
                if (value < 0 || value > 32767)
                {
                    throw new ArgumentOutOfRangeException(
                                "SendByteWait",
                                value,
                                "0～32767の範囲で設定してください");
                }
                else
                {
                    sendByteWait = value;
                }
            }
        }

        #endregion

        #region TextAlignプロパティ

        /// <summary>
        /// 送受信表示 S R の配置を参照または設定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(ContentAlignment.MiddleCenter),
            Description("送受信文字 S,R の配置を参照または設定します。")
        ]
        public ContentAlignment TextAlign
        {
            get
            {
                return labelSend.TextAlign;
            }
            set
            {
                labelSend.TextAlign = value;
                labelRecive.TextAlign = value;
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
        private void backColors_ColorChange(object sender, ComPort.BackColors.ColorChangeEventArgs e)
        {
            if (comPort.IsOpen == true)
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

        #region データ受信イベント (comPort_DataReceived)

        /// <summary>
        /// データ受信を通知するイベントです。
        /// </summary>
        /// <param name="sender">発生元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int rcvlen = 0;
            bool esc = false;

            labelRecive.BackColor = backColors.ReciveOn;
            rtim.Enabled = false;
            rtim.Enabled = true;

            // Mutexの所有権を得る - ∞秒待つ
            mutex.WaitOne(-1, false);

            // 受信
            try
            {
                // データ受信 - 受信データサイズを更新
                rcvlen = comPort.Read(reciveBuffer, reciveBufferDataLength, reciveBuffer.Length - reciveBufferDataLength);
                reciveBufferDataLength += rcvlen;

                // VT100シーケンス途中かの判定 - [D [C [s [u [0J に対応
                if (allowVT100Emulation)
                {
                    if (reciveBufferDataLength > 1 &&
                        reciveBuffer[reciveBufferDataLength - 1] == '[')
                    {
                        esc = true;
                    }
                    if (reciveBufferDataLength > 2 && 
                        reciveBuffer[reciveBufferDataLength - 2] == '[' &&
                        reciveBuffer[reciveBufferDataLength - 1] == '0')
                    {
                        esc = true;
                    }
                }
            }
            catch
            {
            }
            
            // Mutexの所有権を解放
            mutex.ReleaseMutex();

            if (reciveBufferDataLength == 0)
            {
                // 受信データなし
                return;
            }

            // VT100シーケンス途中
            if (esc)
            {
                // そのまま続きを受信
                return;
            }

            // 受信通知イベント
            RecivedEventArgs eArgs = new RecivedEventArgs();
            eArgs.DataCount = reciveBufferDataLength;
            eArgs.FreeCount = reciveBuffer.Length - reciveBufferDataLength;
            if (dataDump == true)
            {
                LogHexDump("Status[RCV:" + eArgs.DataCount.ToString() + ",FREE:" + eArgs.FreeCount.ToString() + "]");
            }
            if (Recived != null)
            {
                // ハンドラが設定されていたらイベント発生
                CallRecivedEvent(this, eArgs);
            }

            if (reciveBuffer.Length - reciveBufferDataLength < 1)
            {
                rtim.Enabled = false;
                labelRecive.BackColor = backColors.BufferFull;
                LogPrint("受信バッファがいっぱいです");
            }
        }

        #endregion

        #region データ受信通知イベント (Recived)

        /// <summary>
        /// Recivedイベントのイベントデータを提供します。
        /// </summary>
        public class RecivedEventArgs : EventArgs
        {
            /// <summary>
            /// バッファ内のデータバイト数
            /// </summary>
            public int DataCount;
            /// <summary>
            /// バッファの空きバイト数
            /// </summary>
            public int FreeCount;
        }
        public delegate void RecivedEventHandler(object sender, RecivedEventArgs e);
        /// <summary>
        /// データ受信したことを通知するイベントです。環境により別スレッドで発生します。
        /// </summary>
        [
            Category("動作"),
            Description("データ受信したことを通知するイベントです。環境により別スレッドで発生します。")
        ]
        public event RecivedEventHandler Recived;

        #endregion

        #endregion

        #region メソッド

        #region 通信ポート名取得 (GetPortNames)

        /// <summary>
        /// 現在接続されている通信ポート名の配列を取得します。
        /// ポートがない場合はWin32Exceptionが発生します。
        /// </summary>
        /// <returns>ポート名の配列</returns>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        public string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        #endregion

        #region デバイス名取得 (GetDeviceNames)

        /// <summary>
        /// 通信ポートのデバイス名を取得します。
        /// </summary>
        /// <returns>デバイス名の配列</returns>
        public string[] GetDeviceNames()
        {
            ManagementClass pnpEntity = new ManagementClass("Win32_PnPEntity");
            ManagementObjectCollection pnps = pnpEntity.GetInstances();
            ArrayList nm = new ArrayList(); 
            foreach (ManagementObject pnp in pnps)
            {
                //Nameプロパティを取得
                string nam = pnp.GetPropertyValue("Name") as string;
                if (nam != null)
                {
                    int n = nam.IndexOf("(COM");
                    if (n >= 0)
                    {
                        string no = nam.Substring(n + 4);
                        if (no.Substring(no.Length - 1).Equals(")") && int.TryParse(no.Substring(0, no.Length - 1), out n))
                        {
                            nm.Add(nam);
                        }
                    }
                }
            }
            string[] rt = (String[])nm.ToArray(typeof(string));
            if (rt == null) rt = new string[0];
            return rt;
        }

        #endregion

        #region 通信ポートオープン (Open)

        /// <summary>
        /// 通信ポートをオープンします。
        /// </summary>
        /// <returns>結果(True=成功, False=失敗)</returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public bool Open()
        {
            bool status = false;

            try
            {
                reciveBuffer = new byte[reciveBufferSize];
                reciveBufferDataLength = 0;
                comPort.Open();
                labelRecive.BackColor = backColors.ReciveOff;
                labelSend.BackColor = backColors.SendOff;
                LogPrint("通信ポート" + comPort.PortName + "をオープンしました");
                status = true;
            }
            catch (Exception es)
            {
                reciveBuffer = null;
                LogPrint("通信ポートのオープンに失敗しました - " + es.Message);
            }

            return status;
        }

        #endregion

        #region クローズ (Close)

        /// <summary>
        /// 通信ポートをクローズします。
        /// </summary>
        /// <exception cref="System.IO.IOException"></exception>
        public void Close()
        {
            try
            {
                comPort.Close();
            }
            catch (Exception es)
            {
                LogPrint(es.Message + es.StackTrace);
            }
            reciveBuffer = null;
            labelRecive.BackColor = backColors.Close;
            labelSend.BackColor = backColors.Close;
            LogPrint("通信ポート" + comPort.PortName + "をクローズしました");
        }

        #endregion

        #region バイナリデータ送信 (SendBytes)

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
            if(comPort.IsOpen==true)
            {
                // データダンプ
                if (dataDump == true && xlog!=null)
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
                stim.Enabled = false;
                stim.Enabled = true;

                try
                {
                    if (sendByteWait <= 0)
                    {
                        comPort.Write(data, offset, count);
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            comPort.Write(data, offset + i, 1);
                            Thread.Sleep(new TimeSpan(sendByteWait * 10000));
                        }
                    }
                }
                catch (Exception es)
                {
                    LogPrint("送信に失敗しました - " + es.Message);
                }
            }
            else
            {
                LogPrint("通信ポートをオープンしていないため送信できません");
            }
        }

        #endregion

        #region テキストデータ送信 (SendLine)

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

        #region バイナリデータ取得 (GetReciveBytes)

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
            if (InvokeRequired && arrowInvokeReciveEvent)
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

        #region バイナリデータ取得(終端文字指定) (GetReciveLineBytes)

        delegate byte[] GetReciveLineBytesDelegate(byte[] endchar, int length);

        /// <summary>
        /// 終端文字を指定してバイナリデータを取得します。
        /// </summary>
        /// <returns>１行分のバイナリデータ(データが無いときはnullが返ります)</returns>
        /// <param name="endchar">行末文字配列</param>
        /// <param name="length">行末文字数</param>
        public byte[] GetReciveLineBytes(byte[] endchar, int length)
        {
            if (InvokeRequired && arrowInvokeReciveEvent)
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

        #region テキストデータ取得 (GetReciveLine)

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

        #region 内部処理

        #region コントロール再配置 (ControlRelocation)

        /// <summary>
        /// 各コントロールの位置を調整します。
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

            labelSend.Redraw();

            #endregion

            #region Recive : ラベル

            posX += label_size_w + label_spc_w;

            labelRecive.Top = posY;
            labelRecive.Left = posX;
            labelRecive.Height = label_size_h;
            labelRecive.Width = label_size_w;

            labelRecive.Redraw();

            #endregion

            #endregion
        }

        #endregion

        #region データ取得 (getReciveBytes)

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

            if (comPort.IsOpen == true)
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
                    for (int i = 0; i < reciveBufferDataLength - retlen; i++)
                    {
                        reciveBuffer[i] = reciveBuffer[i + retlen];
                    }

                    // 受信データサイズを更新
                    reciveBufferDataLength -= retlen;

                    // データダンプ
                    if (dataDump == true && xlog != null)
                    {
                        try
                        {
                            xlog.HexDump("受信データ", data, offset, retlen);
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

        #region 行末検索 (findLineLength)

        /// <summary>
        /// 行末を検索します。
        /// </summary>
        /// <returns>行末の区切り文字の終端位置または-1(エラー値)</returns>
        /// <param name="endchar">区切り文字配列</param>
        /// <param name="length">区切り文字数</param>
        private int findLineLength(byte[] endchar, int length)
        {
            int linelength = -1;

            if (comPort.IsOpen == true)
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
                            if (reciveBuffer[i + j] == endchar[j])
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
                    if (comPort.IsOpen == false)
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

        #region データ受信通知 (CallReciveEvent)

        delegate void CallRecivedEventDelegate(object sender, RecivedEventArgs e);

        /// <summary>
        /// データ受信イベントを発生させます。
        /// </summary>
        /// <param name="sender">発生元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void CallRecivedEvent(object sender, RecivedEventArgs e)
        {
            if (InvokeRequired && arrowInvokeReciveEvent)
            {
                // 別スレッドから呼び出された場合
                object[] param = { sender, e };
                Invoke(new CallRecivedEventDelegate(CallRecivedEvent), param);
                return;
            }

            try
            {
                Recived(sender, e);
            }
            catch
            {
            }
        }

        #endregion

        #region 送受信モニタ消灯イベント (stim_Tick/rtim_Tick)

        void stim_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (IsOpen) labelSend.BackColor = backColors.SendOff;
            stim.Enabled = false;
        }
        void rtim_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (IsOpen) labelRecive.BackColor = backColors.ReciveOff;
            rtim.Enabled = false;
        }

        #endregion

        #region 操作ログ出力 (LogPrint)

        /// <summary>
        /// 操作ログを出力します。
        /// </summary>
        /// <param name="msg">出力する内容</param>
        private void LogPrint(string msg)
        {
            if (xlog != null)
            {
                try
                {
                    xlog.Print("[" + this.Name + "] " + msg);
                }
                catch
                {
                }
            }
        }

        #endregion

        #region ダンプ出力 (LogHexDump)

        /// <summary>
        /// データダンプの内容説明を出力します。
        /// </summary>
        /// <param name="msg">出力する内容の説明</param>
        private void LogHexDump(string msg)
        {
            if (xlog != null)
            {
                try
                {
                    xlog.HexDump(msg);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// データダンプを出力します。
        /// </summary>
        /// <param name="msg">出力する内容の説明</param>
        /// <param name="data">出力するバイトデータ配列</param>
        /// <param name="offset">バイト単位の0から始まる開始位置</param>
        /// <param name="count">バイト単位の出力サイズ</param>
        private void LogHexDump(string msg, byte[] data, int offset, int count)
        {
            if (xlog != null)
            {
                try
                {
                    xlog.HexDump("[" + this.Name + "] " + msg, data, offset, count);
                }
                catch
                {
                }
            }
        }

        #endregion

        #endregion
    }
}
