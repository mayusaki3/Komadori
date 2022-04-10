using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.DirectX.DirectInput;
using DInput = Microsoft.DirectX.DirectInput;

namespace XControls
{
    /// <summary>
    /// ジョイスティックコントロール
    /// キーボードおよびリモート操作をサポート
    /// 【制限事項】本バージョンでは、キーボードの割り当ては固定となっています
    ///             フォースフィードバックは固定の振動のみサポート
    ///             アナログスティック有効時の方向キー取得はボタン29-32に割り当て
    /// </summary>
    /// <remarks>
    /// UDPとジョイスティックは排他関係で、ジョイスティックを接続するとUDPを無視する
    /// 通信フォーマットは次の通り
    /// (1) ジョイスティック状態(Send側→Recive側)
    ///     書式: JSbbbbbbbbxxxxyyyyzzzzrrrr\r
    ///     説明: JS = ヘッダ部
    ///           b  = ボタン状態(16進32bit、左からボタン1-32)
    ///           x  = 左スティックのx座標(0000-FFFF)
    ///           y  = 左スティックのy座標(0000-FFFF)
    ///           z  = 右スティックのx座標(0000-FFFF)
    ///           r  = 右スティックのy座標(0000-FFFF)
    ///           \r = 終端文字
    /// (2) 通知情報(Recive側→Send側)
    ///     書式: JNx\r            
    ///     説明: JN = ヘッダ部
    ///           x  = 通知文字列(可変長)
    ///           \r = 終端文字
    /// </remarks>
    [
        Description("ジョイスティックによる操作を可能にします。"),
        ToolboxBitmap(typeof(XJoyStickControl), "XJoyStickControl.bmp")
    ]
    public partial class XJoyStickControl : Component
    {
        #region インナークラス

        #region InputStateクラス

        /// <summary>
        /// ジョイスティックの入力状態を表します
        /// </summary>
        public class InputState
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public InputState()
            {
                Buttons = new Byte[32];
                X = 32767;
                Y = 32767;
                Z = 32767;
                Rz = 32767;
                Notice = "";
                Break = false;
            }
            public InputState(InputState inpstat)
            {
                Buttons = new Byte[32];
                inpstat.Buttons.CopyTo(Buttons, 0);
                X = inpstat.X;
                Y = inpstat.Y;
                Z = inpstat.Z;
                Rz = inpstat.Rz;
                Notice = inpstat.Notice;
                Break = inpstat.Break;
            }

            // x,y軸の傾き
            public long X;
            public long Y;
            public long Z;

            // z軸回転
            public long Rz;

            // ボタン[32]
            public byte[] Buttons;

            // 通知[n]
            public string Notice;

            // 途絶フラグ
            public bool Break;
        }

        #endregion

        #region RemoteStateクラス

        /// <summary>
        /// リモート(UDP)のデータを扱います
        /// </summary>
        public class RemoteState
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public RemoteState()
            {
                _inpstat = new InputState();
                _pdata = "";
            }

            #region 変数

            // 現在の内容
            private InputState _inpstat;

            // 前回の受信データ
            private string _pdata;

            #endregion

            #region メソッド

            #region SetInputStatusメソッド

            /// <summary>
            /// 入力状態を記憶する
            /// </summary>
            /// <param name="value">入力状態(InputState)</param>
            public void SetInputStatus(InputState value)
            {
                _inpstat = value;
            }

            #endregion

            #region GetInputStatusメソッド

            /// <summary>
            /// 入力状態を返す
            /// </summary>
            /// <returns>入力状態(InputState)</returns>
            public InputState GetInputStatus()
            {
                return _inpstat;
            }

            #endregion

            #region ToUDPStringメソッド

            /// <summary>
            /// UDP送信データを作成する
            /// </summary>
            /// <returns>送信データ</returns>
            public string ToUDPString()
            {
                int i;
                int b;
                string ds = "JS";                   // ヘッダ

                b = 0;
                for (i = 0; i < 32; i++)
                {
                    b = b << 1;
                    b += ((_inpstat.Buttons[i] == 0) ? 0 : 1);
                }
                ds += b.ToString("X8");             // ボタン[0-31]

                ds += _inpstat.X.ToString("X4");    // スティックL - X          
                ds += _inpstat.Y.ToString("X4");    //               Y          
                ds += _inpstat.Z.ToString("X4");    // スティックR - X          
                ds += _inpstat.Rz.ToString("X4");   //               Y          

                return ds;
            }

            #endregion

            #region FromUDPStringメソッド

            /// <summary>
            /// UDP受信データから状態を取り込む
            /// </summary>
            /// <param name="value">UDP受信データ</param>
            public void FromUDPString(string value)
            {
                if (value.Length == 27)
                {
                    if (value.Substring(0, 2) == "JS" && _pdata != value)               // ヘッダ
                    {
                        try
                        {
                            int i;
                            int b;

                            b = Convert.ToInt32(value.Substring(2, 8), 16);             // ボタン
                            for (i = 31; i >= 0; i--)
                            {
                                if ((b & 1) != 0)
                                {
                                    _inpstat.Buttons[i] = 128;
                                }
                                else
                                {
                                    _inpstat.Buttons[i] = 0;
                                }
                                b = b >> 1;
                            }

                            _inpstat.X = Convert.ToInt32(value.Substring(10, 4), 16);   // スティックL - X          
                            _inpstat.Y = Convert.ToInt32(value.Substring(14, 4), 16);   //               Y          
                            _inpstat.Z = Convert.ToInt32(value.Substring(18, 4), 16);   // スティックR - X          
                            _inpstat.Rz = Convert.ToInt32(value.Substring(22, 4), 16);  //               Y
                        }
                        catch
                        {
                        }
                    }
                    _pdata = value;
                }
            }

            #endregion

            #region Resetメソッド

            /// <summary>
            /// 途絶復帰時のリセットを行う
            /// </summary>
            public void Reset()
            {
                _pdata = "";
            }

            #endregion

            #endregion

        }

        #endregion

        #region KeyMapDataクラス

        /// <summary>
        /// キーマップのデータを扱います
        /// </summary>
        public class KeyMapData
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public KeyMapData()
            {
                _dkey = new Key();
                _key = "";
            }
            public KeyMapData(Microsoft.DirectX.DirectInput.Key dkey, string key)
            {
                _dkey = dkey;
                _key = key;
            }

            #region 変数

            private Microsoft.DirectX.DirectInput.Key _dkey;
            private string _key;

            #endregion

            #region メソッド

            #region KeyMapDataメソッド

            /// <summary>
            /// データを設定する
            /// </summary>
            /// <param name="dkey">DirectInputキー</param>
            /// <param name="key">キー文字列</param>
            public void SetKeyMapData(Microsoft.DirectX.DirectInput.Key dkey, string key)
            {
                _dkey = dkey;
                _key = key;
            }

            #endregion

            #region GetDInputKeyメソッド

            /// <summary>
            /// DirectInputキーを返す
            /// </summary>
            /// <returns>DirectInputキー</returns>
            public Microsoft.DirectX.DirectInput.Key GetDInputKey()
            {
                return _dkey;
            }

            #endregion

            #region GetKeyメソッド

            /// <summary>
            /// キー文字列を返す
            /// </summary>
            /// <returns>キー文字列</returns>
            public string GetKey()
            {
                return _key;
            }

            #endregion

            #endregion

        }

        #endregion

        #region KeyMapListクラス

        /// <summary>
        /// キーマップデータのリストを扱います
        /// </summary>
        public class KeyMapList
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public KeyMapList()
            {
                #region 英数

                _kmlist.Add(new KeyMapData(DInput.Key.A, "A"));
                _kmlist.Add(new KeyMapData(DInput.Key.B, "B"));
                _kmlist.Add(new KeyMapData(DInput.Key.C, "C"));
                _kmlist.Add(new KeyMapData(DInput.Key.D, "D"));
                _kmlist.Add(new KeyMapData(DInput.Key.E, "E"));
                _kmlist.Add(new KeyMapData(DInput.Key.F, "F"));
                _kmlist.Add(new KeyMapData(DInput.Key.G, "G"));
                _kmlist.Add(new KeyMapData(DInput.Key.H, "H"));
                _kmlist.Add(new KeyMapData(DInput.Key.I, "I"));
                _kmlist.Add(new KeyMapData(DInput.Key.J, "J"));
                _kmlist.Add(new KeyMapData(DInput.Key.K, "K"));
                _kmlist.Add(new KeyMapData(DInput.Key.L, "L"));
                _kmlist.Add(new KeyMapData(DInput.Key.M, "M"));
                _kmlist.Add(new KeyMapData(DInput.Key.N, "N"));
                _kmlist.Add(new KeyMapData(DInput.Key.O, "O"));
                _kmlist.Add(new KeyMapData(DInput.Key.P, "P"));
                _kmlist.Add(new KeyMapData(DInput.Key.Q, "Q"));
                _kmlist.Add(new KeyMapData(DInput.Key.R, "R"));
                _kmlist.Add(new KeyMapData(DInput.Key.S, "S"));
                _kmlist.Add(new KeyMapData(DInput.Key.T, "T"));
                _kmlist.Add(new KeyMapData(DInput.Key.U, "U"));
                _kmlist.Add(new KeyMapData(DInput.Key.V, "V"));
                _kmlist.Add(new KeyMapData(DInput.Key.W, "W"));
                _kmlist.Add(new KeyMapData(DInput.Key.X, "X"));
                _kmlist.Add(new KeyMapData(DInput.Key.Y, "Y"));
                _kmlist.Add(new KeyMapData(DInput.Key.Z, "Z"));

                _kmlist.Add(new KeyMapData(DInput.Key.D1, "1"));
                _kmlist.Add(new KeyMapData(DInput.Key.D2, "2"));
                _kmlist.Add(new KeyMapData(DInput.Key.D3, "3"));
                _kmlist.Add(new KeyMapData(DInput.Key.D4, "4"));
                _kmlist.Add(new KeyMapData(DInput.Key.D5, "5"));
                _kmlist.Add(new KeyMapData(DInput.Key.D6, "6"));
                _kmlist.Add(new KeyMapData(DInput.Key.D7, "7"));
                _kmlist.Add(new KeyMapData(DInput.Key.D8, "8"));
                _kmlist.Add(new KeyMapData(DInput.Key.D9, "9"));
                _kmlist.Add(new KeyMapData(DInput.Key.D0, "0"));

                _kmlist.Add(new KeyMapData(DInput.Key.NumPad1, "N1"));
                _kmlist.Add(new KeyMapData(DInput.Key.NumPad2, "N2"));
                _kmlist.Add(new KeyMapData(DInput.Key.NumPad3, "N3"));
                _kmlist.Add(new KeyMapData(DInput.Key.NumPad4, "N4"));
                _kmlist.Add(new KeyMapData(DInput.Key.NumPad5, "N5"));
                _kmlist.Add(new KeyMapData(DInput.Key.NumPad6, "N6"));
                _kmlist.Add(new KeyMapData(DInput.Key.NumPad7, "N7"));
                _kmlist.Add(new KeyMapData(DInput.Key.NumPad8, "N8"));
                _kmlist.Add(new KeyMapData(DInput.Key.NumPad9, "N9"));
                _kmlist.Add(new KeyMapData(DInput.Key.NumPad0, "N0"));

                #endregion

                #region 英記号

                _kmlist.Add(new KeyMapData(DInput.Key.Minus, "-"));
                _kmlist.Add(new KeyMapData(DInput.Key.Circumflex, "^"));
                _kmlist.Add(new KeyMapData(DInput.Key.Yen, "\\\\"));
                _kmlist.Add(new KeyMapData(DInput.Key.At, "@"));
                _kmlist.Add(new KeyMapData(DInput.Key.LeftBracket, "["));
                _kmlist.Add(new KeyMapData(DInput.Key.SemiColon, ";"));
                _kmlist.Add(new KeyMapData(DInput.Key.Colon, ":"));
                _kmlist.Add(new KeyMapData(DInput.Key.RightBracket, "]"));
                _kmlist.Add(new KeyMapData(DInput.Key.Period, "."));
                _kmlist.Add(new KeyMapData(DInput.Key.Comma, ","));
                _kmlist.Add(new KeyMapData(DInput.Key.Slash, "/"));
                _kmlist.Add(new KeyMapData(DInput.Key.BackSlash, "\\"));
                _kmlist.Add(new KeyMapData(DInput.Key.Space, "SP"));

                _kmlist.Add(new KeyMapData(DInput.Key.Divide, "N/"));
                _kmlist.Add(new KeyMapData(DInput.Key.Multiply, "N*"));
                _kmlist.Add(new KeyMapData(DInput.Key.Subtract, "N-"));
                _kmlist.Add(new KeyMapData(DInput.Key.Add, "N+"));
                _kmlist.Add(new KeyMapData(DInput.Key.NumPadEnter, "NE"));
                _kmlist.Add(new KeyMapData(DInput.Key.NumPadPeriod, "N."));

                #endregion

                #region カーソルキー

                _kmlist.Add(new KeyMapData(DInput.Key.UpArrow, "^U"));
                _kmlist.Add(new KeyMapData(DInput.Key.DownArrow, "^D"));
                _kmlist.Add(new KeyMapData(DInput.Key.LeftArrow, "^L"));
                _kmlist.Add(new KeyMapData(DInput.Key.RightArrow, "^R"));

                #endregion

                #region 特殊キー

                _kmlist.Add(new KeyMapData(DInput.Key.F1, "F1"));
                _kmlist.Add(new KeyMapData(DInput.Key.F2, "F2"));
                _kmlist.Add(new KeyMapData(DInput.Key.F3, "F3"));
                _kmlist.Add(new KeyMapData(DInput.Key.F4, "F4"));
                _kmlist.Add(new KeyMapData(DInput.Key.F5, "F5"));
                _kmlist.Add(new KeyMapData(DInput.Key.F6, "F6"));
                _kmlist.Add(new KeyMapData(DInput.Key.F7, "F7"));
                _kmlist.Add(new KeyMapData(DInput.Key.F8, "F8"));
                _kmlist.Add(new KeyMapData(DInput.Key.F9, "F9"));
                _kmlist.Add(new KeyMapData(DInput.Key.F10, "10"));
                _kmlist.Add(new KeyMapData(DInput.Key.F11, "11"));
                _kmlist.Add(new KeyMapData(DInput.Key.F12, "12"));

                _kmlist.Add(new KeyMapData(DInput.Key.LeftAlt, "LA"));
                _kmlist.Add(new KeyMapData(DInput.Key.LeftControl, "LC"));
                _kmlist.Add(new KeyMapData(DInput.Key.LeftShift, "LS"));
                _kmlist.Add(new KeyMapData(DInput.Key.RightAlt, "RA"));
                _kmlist.Add(new KeyMapData(DInput.Key.RightControl, "RC"));
                _kmlist.Add(new KeyMapData(DInput.Key.RightShift, "RS"));

                _kmlist.Add(new KeyMapData(DInput.Key.Escape, "ES"));
                _kmlist.Add(new KeyMapData(DInput.Key.Tab, "TB"));
                _kmlist.Add(new KeyMapData(DInput.Key.BackSpace, "BS"));
                _kmlist.Add(new KeyMapData(DInput.Key.Return, "RT"));
                _kmlist.Add(new KeyMapData(DInput.Key.NoConvert, "NC"));
                _kmlist.Add(new KeyMapData(DInput.Key.Convert, "CN"));
                _kmlist.Add(new KeyMapData(DInput.Key.Kana, "KA"));
                
                _kmlist.Add(new KeyMapData(DInput.Key.Insert, "IN"));
                _kmlist.Add(new KeyMapData(DInput.Key.Delete, "DL"));
                _kmlist.Add(new KeyMapData(DInput.Key.Home, "HO"));
                _kmlist.Add(new KeyMapData(DInput.Key.End, "ED"));
                _kmlist.Add(new KeyMapData(DInput.Key.PageUp, "PU"));
                _kmlist.Add(new KeyMapData(DInput.Key.PageDown, "PD"));

                _kmlist.Add(new KeyMapData(DInput.Key.Scroll, "SC"));
                _kmlist.Add(new KeyMapData(DInput.Key.Pause, "PA"));

                #endregion
            }

            #region 変数

            // キーマップリスト
            private ArrayList _kmlist = new ArrayList();

            #endregion

            #region メソッド

            #region FindKeyメソッド

            /// <summary>
            /// キー文字列が登録されているかを検索する
            /// </summary>
            /// <param name="key">キー文字列</param>
            /// <returns>結果(true=あり, false=なし)</returns>
            public bool FindKey(string key)
            {
                int i;
                for (i = 0; i < _kmlist.Count; i++)
                {
                    if (((KeyMapData)_kmlist[i]).GetKey() == key)
                    {
                        return true;
                    }
                }
                return false;
            }

            #endregion

            #region GetDInputKeyメソッド

            /// <summary>
            /// キー文字列に対応するDirectInputキーを返す
            /// </summary>
            /// <param name="key">キー文字列</param>
            /// <returns>対応するDirectInputキー</returns>
            public Microsoft.DirectX.DirectInput.Key GetDInputKey(string key)
            {
                int i;
                for (i = 0; i < _kmlist.Count; i++)
                {
                    if (((KeyMapData)_kmlist[i]).GetKey() == key)
                    {
                        return ((KeyMapData)_kmlist[i]).GetDInputKey();
                    }
                }
                return (Microsoft.DirectX.DirectInput.Key)0;
            }

            #endregion

            #endregion

        }

        #endregion

        #region InputStateBuilderクラス

        /// <summary>
        /// ジョイスティックの入力状態を組み立てます
        /// </summary>
        public class InputStateBuilder
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="KbdActive">キーボード有効(true/false)</param>
            /// <param name="DKeyState">キーボード状態</param>
            /// <param name="JoyActive">ジョイスティック有効(true/false)</param>
            /// <param name="DJoyState">ジョイスティック状態</param>
            /// <param name="RmtActive">リモート有効(true/false)</param>
            /// <param name="DRmtState">リモート状態</param>
            /// <param name="map">ボタンマッピング指定(0-31のインデックス配列)</param>
            /// <param name="key">キーマッピング指定(0-31の文字配列)</param>
            /// <param name="kmap">キーマッピングリスト</param>
            public InputStateBuilder(bool KbdActive, KeyboardState DKeyState, bool JoyActive, JoystickState DJoyState, bool RmtActive, RemoteState DRmtState, int[] map, string[] key, KeyMapList kmap)
            {
                int i;

                // 初期化
                _inpstat = new InputState();

                if (JoyActive)
                {
                    // ジョイスティックより設定 ------------------------

                    // x,y軸の傾き
                    _inpstat.X = DJoyState.X;
                    _inpstat.Y = DJoyState.Y;
                    _inpstat.Z = DJoyState.Z;

                    // z軸回転
                    _inpstat.Rz = DJoyState.Rz;

                    // ボタン[1-32]
                    byte[] btns = DJoyState.GetButtons();
                    for (i = 0; i < _inpstat.Buttons.Length; i++)
                    {
                        _inpstat.Buttons[i] = 0;
                    }
                    for (i = 0; i < _inpstat.Buttons.Length; i++)
                    {
                        if (i < btns.Length)
                        {
                            if (_inpstat.Buttons[map[i]] == 0) _inpstat.Buttons[map[i]] = btns[i];
                        }
                    }

                    // アナログスティック有効時の方向キー(POV)
                    int [] pov = DJoyState.GetPointOfView();
                    if (pov[0] >=31500 || pov[0] >= 0 && pov[0] <= 4500)
                    {
                        _inpstat.Buttons[map[28]] = 128;
                    }
                    if (pov[0] >= 4500 && pov[0] <= 13500)
                    {
                        _inpstat.Buttons[map[29]] = 128;
                    }
                    if (pov[0] >= 13500 && pov[0] <= 22500)
                    {
                        _inpstat.Buttons[map[30]] = 128;
                    }
                    if (pov[0] >= 22500 && pov[0] <= 31500)
                    {
                        _inpstat.Buttons[map[31]] = 128;
                    }
                }
                else
                {
                    // UDP受信内容より設定 -----------------------------
                    if(RmtActive)   _inpstat = DRmtState.GetInputStatus();
                }

                if (KbdActive)
                {
                    // キーボードより設定 ------------------------------

                    // ボタン[1-32]
                    for (i = 0; i < 32; i++)
                    {
                        if (DKeyState[kmap.GetDInputKey(key[i])] == true) _inpstat.Buttons[map[i]] = 128;
                    }

                    if (DKeyState[kmap.GetDInputKey(key[32])] == true) _inpstat.Y = 0;
                    if (DKeyState[kmap.GetDInputKey(key[33])] == true) _inpstat.Y = 65535;

                    if (DKeyState[kmap.GetDInputKey(key[34])] == true) _inpstat.X = 0;
                    if (DKeyState[kmap.GetDInputKey(key[35])] == true) _inpstat.X = 65535;

                    if (DKeyState[kmap.GetDInputKey(key[36])] == true) _inpstat.Rz = 0;
                    if (DKeyState[kmap.GetDInputKey(key[37])] == true) _inpstat.Rz = 65535;

                    if (DKeyState[kmap.GetDInputKey(key[38])] == true) _inpstat.Z = 0;
                    if (DKeyState[kmap.GetDInputKey(key[39])] == true) _inpstat.Z = 65535;
                }

                // 途絶
                _inpstat.Break = (RmtActive == false);
            }

            #region 変数

            // 現在のステータス
            private InputState _inpstat;

            #endregion

            #region メソッド
            
            #region GetInputStateメソッド

            /// <summary>
            /// ジョイスティック入力状態を返す
            /// </summary>
            /// <returns></returns>
            public InputState GetInputState()
            {
                return new InputState(_inpstat);
            }

            #endregion

            #endregion
        }

        #endregion

        #endregion

        public XJoyStickControl()
        {
            InitializeComponent();

            #region 初期化

            // キーボード初期化
            DInDevKBD = new Microsoft.DirectX.DirectInput.Device(SystemGuid.Keyboard);

            // キーボード入力制御開始
            DInDevKBD.Acquire();

            // UDP通信終端文字[CR]設定
            eclen = 1;
            ec[0] = 0x0d;

            // UDPリモートデータ初期化
            DRmtState = new RemoteState();

            // マッピング初期化
            ButtonMapping = buttonMapping;
            KeyMapping = keyMapping;

            // ライセンスチェック(5秒後)
            timerLicenseCheck.Enabled = true;

            #endregion
        }

        #region 定数

        // ライセンスキー用GUID {52E788FC-91D6-46c4-9049-7808F56FCE31}
        // このGUIDをLocalLicenseKeyプロパティに設定してください。
        Guid LOCAL_LICENSE_KEY = new Guid("52E788FC-91D6-46c4-9049-7808F56FCE31");

        #endregion

        #region 変数

        // DirectInputキーボードデバイス
        private Device DInDevKBD = null;

        // DirectInputジョイスティックデバイス
        private Device DInDevJOY = null;
        private EffectObject DInEffect = null;
        private int[] DInAxis = null;

        // リモート操作(UDP受信)ステータス
        private RemoteState DRmtState = null;

        // マルチメディアタイマーID
        private int timerId = 0;

        // マルチメディアタイマーコールバック関数(アンマネージ)
        private GCHandle callback;

        // 終端文字
        int eclen = 0;
        byte[] ec = new byte[1];

        // 受信側送信フラグ
        private bool rsendflg = false;

        // 前回ジョイスティック状態
        private string pJoyStatus = "";

        // ジョイスティック状態強制待機カウンタ
        private int pForceSendWaitCount = 0;

        // UDP通信途絶カウンタ
        private int udpstopcount = 0;

        // モニタ画面
        private MonitorForm mform = null;

        // キーマップ
        private KeyMapList kmap = new KeyMapList();

        #endregion

        #region プロパティ

        #region 追加のプロパティ

        #region LocalLicenseKeyプロパティ

        private String locallicenseKey = "";
        private Boolean useLocalLicense = false;
        [
            Category("ライセンス"),
            DefaultValue(""),
            Description("XJoyStickControl利用のためのライセンス情報を設定します。")
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

                // ライセンスチェック(3秒後)
                timerLicenseCheck.Enabled = true;
            }
        }

        #endregion

        #region SelectedFormプロパティ

        private Form formObject = null;
        [
            Category("動作"),
            Description("イベント通知先のフォームを設定します。設定しない場合はフォーム側でInvokeを実行する必要があります。")
        ]
        public Form SelectedForm
        {
            get
            {
                return formObject;
            }
            set
            {
                formObject = value;
            }
        }

        #endregion

        #region XUDPComControlプロパティ

        private XUDPComControl xUDPComControl = null;
        private XUDPComControl.ReciveEventHandler rcvEv = null;
        [
            Category("リモート"),
            Description("リモートジョイスティックで使用するXUDPComControlを指定します。")
        ]
        public XUDPComControl XUDPComControl
        {
            get
            {
                return xUDPComControl;
            }
            set
            {
                if (xUDPComControl != null)
                {
                    // イベント取消
                    xUDPComControl.ReciveEvent -= rcvEv;
                }
                xUDPComControl = value;
                if (xUDPComControl != null)
                {
                    // イベント登録
                    rcvEv = new XUDPComControl.ReciveEventHandler(this.xudpComControl_ReciveEvent);
                    xUDPComControl.ReciveEvent += rcvEv;
                }
            }
        }

        #endregion

        #region RemoteModeプロパティ
        public enum enumRemoteMode : int
        {
            Send,
            Recive
        }
        private enumRemoteMode remoteMode = enumRemoteMode.Recive;
        [
            Category("リモート"),
            Description("リモート操作時に送信側となるか受信側となるかを指定します。")
        ]
        public enumRemoteMode RemoteMode
        {
            get
            {
                return remoteMode;
            }
            set
            {
                remoteMode = value;
            }
        }

        #endregion

        #region NoticeTextプロパティ

        private string noticeText = "";
        [
            Category("動作"),
            Description("リモート操作時に相手側に通知するテキストを指定します。")
        ]
        public string NoticeText
        {
            get
            {
                return noticeText;
            }
            set
            {
                noticeText = value;
                rsendflg = true;
            }
        }

        #endregion

        #region UseKeyboardプロパティ

        private bool useKeyboard = false;
        [
            Category("動作"),
            Description("キーボード操作を使用するかを設定します。")
        ]
        public bool UseKeyboard
        {
            get
            {
                return useKeyboard;
            }
            set
            {
                useKeyboard = value;
            }
        }

        #endregion

        #region SamplingIntervalプロパティ

        private int samplingInterval = 50;
        [
            Category("動作"),
            DefaultValue(5),
            Description("ジョイスティックの状態をサンプリングする間隔(ミリ秒)を10～500の範囲で指定します。")
        ]
        public int SamplingInterval
        {
            get
            {
                return samplingInterval;
            }
            set
            {
                if (value < 10 || value > 500)
                {
                    throw new ArgumentOutOfRangeException(
                                "SamplingInterval",
                                value,
                                "10～500の範囲で設定してください");
                }
                else
                {
                    samplingInterval = value;

                    // タイマー再起動
                    if (timerId != 0)
                    {
                        timerStop();
                        timerStart();
                    }
                }
            }
        }

        #endregion

        #region ForceSendIntervalプロパティ

        private int forceSendInterval = 1000;
        [
            Category("動作"),
            DefaultValue(5),
            Description("ジョイスティックの状態に変化がない場合でも強制的に送信する間隔(ミリ秒)を50～5000の範囲で指定します。")
        ]
        public int ForceSendInterval
        {
            get
            {
                return forceSendInterval;
            }
            set
            {
                if (value < 50 || value > 5000)
                {
                    throw new ArgumentOutOfRangeException(
                                "ForceSendInterval",
                                value,
                                "50～5000の範囲で設定してください");
                }
                else
                {
                    forceSendInterval = value;
                }
            }
        }

        #endregion

        #region ReciveTimeoutIntervalプロパティ

        private int reciveTimeoutInterval = 1500;
        [
            Category("動作"),
            DefaultValue(5),
            Description("ジョイスティックの状態が受信できない場合に、途絶したと判断するまでの時間(ミリ秒)を200～60000の範囲で指定します。送信側のForceSendIntervalより長く設定します。")
        ]
        public int ReciveTimeoutInterval
        {
            get
            {
                return reciveTimeoutInterval;
            }
            set
            {
                if (value < 200 || value > 60000)
                {
                    throw new ArgumentOutOfRangeException(
                                "ReciveTimeoutInterval",
                                value,
                                "200～60000の範囲で設定してください");
                }
                else
                {
                    reciveTimeoutInterval = value;
                }
            }
        }

        #endregion

        #region IsForceFeedbackプロパティ

        private bool isForceFeedback = false;
        [
            Category("動作"),
            Description("接続されたジョイスティックがフォースフィードバックに対応しているかを返します。")
        ]
        public bool IsForceFeedback
        {
            get
            {
                return isForceFeedback;
            }
        }

        #endregion

        #region ButtonMappingプロパティ

        private string buttonMapping = "0102030405060708091011121314151617181920212223242526272829303132";
        private int[] map = new int[32];
        [
            Category("動作"),
            DefaultValue("0102030405060708091011121314151617181920212223242526272829303132"),
            Description("ボタン通知時の番号の付け替えを行います。先頭から01～32に対応する番号を各2桁で32ボタン分指定します。")
        ]
        public string ButtonMapping
        {
            get
            {
                return buttonMapping;
            }
            set
            {
                int i, n;
                if (value.Length != 64)
                {
                    throw new ArgumentOutOfRangeException(
                                "ButtonMapping",
                                value,
                                "32ボタン分の設定を行ってください");
                }
                for (i = 0; i < 32; i++)
                {
                    if (int.TryParse(value.Substring(i * 2, 2), out n) == false)
                    {
                        throw new ArgumentOutOfRangeException(
                                    "ButtonMapping",
                                    value,
                                    "各ボタンは01～32の範囲でのみ設定できます");
                    }
                    if (n < 1 || n > 32)
                    {
                        throw new ArgumentOutOfRangeException(
                                    "ButtonMapping",
                                    value,
                                    "各ボタンは01～32の範囲でのみ設定できます");
                    }
                }
                for (i = 0; i < 32; i++)
                {
                    int.TryParse(value.Substring(i * 2, 2), out n);
                    map[i] = n - 1;
                }

                buttonMapping = value;
            }
        }

        #endregion

        #region KeyMappingプロパティ

        private string keyMapping = "K . , M A ; S L F J V N                                 3 E W Q D X Z C K , M . ";
        private String[] key = new String[40];
        [
            Category("動作"),
            DefaultValue("K . , M A ; S L F J V N                                 3 E W Q D X Z C K , M . "),
            Description("ボタンおよびスティックに対応するキーボード文字を設定します。先頭から対応する文字を各2桁で32ボタン分＋左右スティック分(上下左右)、計40キー分指定します。")
        ]
        public string KeyMapping
        {
            get
            {
                return keyMapping;
            }
            set
            {
                int i;
                if (value.Length != 80)
                {
                    throw new ArgumentOutOfRangeException(
                                "KeyMapping",
                                value,
                                "32ボタン分＋左右スティック分、計40キーの設定を行ってください");
                }
                for (i = 0; i < 40; i++)
                {
                    string ks = value.Substring(i * 2, 2).Trim().ToUpper();
                    if (ks.Length > 0 && kmap.FindKey(ks) == false)
                    {
                        throw new ArgumentOutOfRangeException(
                                    "KeyMapping",
                                    value,
                                    "指定の文字はサポートしていません");
                    }
                }
                keyMapping = value.ToUpper();
                for (i = 0; i < 40; i++)
                {
                    key[i] = keyMapping.Substring(i * 2, 2).Trim().ToUpper();
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region メソッド

        #region 開始

        /// <summary>
        /// ジョイスティックの監視を開始
        /// </summary>
        public void Start()
        {
            // 監視タイマー起動
            timerStart();
        }

        #endregion

        #region 停止

        /// <summary>
        /// ジョイスティックの監視を停止
        /// </summary>
        public void Stop()
        {
            // 監視タイマー停止
            timerStop();
        }

        #endregion

        #region フォースフィードバックエフェクトSTART/STOP

        /// <summary>
        /// フォースフィードバックエフェクトを開始
        /// </summary>
        public void EffectStart()
        {
            if (DInEffect != null)
            {
                try
                {
                    DInEffect.Start(1, EffectStartFlags.Solo);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// フォースフィードバックエフェクトを停止
        /// </summary>
        public void EffectEnd()
        {
            if (DInEffect != null)
            {
                try
                {
                    DInEffect.Stop();
                }
                catch
                {
                }
            }
        }

        #endregion

        #region モニタ画面表示

        delegate void ShowMonitorDelegate();

        /// <summary>
        /// モニタ画面表示
        /// </summary>
        public void ShowMonitor()
        {
            int i;
            if (mform == null)
            {
                // モニター初期化
                mform = new MonitorForm();
                mform.joy = this;
                mform.lblID.Text = monID;
                mform.lblStatus.BackColor = monCNCol;
            }
            if (mform.InvokeRequired)
            {
                // 別スレッドから呼び出された場合
                try
                {
                    mform.Invoke(new ShowMonitorDelegate(ShowMonitor));
                }
                catch (ObjectDisposedException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                return;
            }
            
            if (mform.Visible == false)
            {
                // ボタンマッピング/キーマッピング反映
                for (i = 0; i < 32; i++)
                {
                    mform.map[i].Text = buttonMapping.Substring(i * 2, 2);
                }
                for (i = 0; i < 40; i++)
                {
                    mform.key[i].Text = keyMapping.Substring(i * 2, 2).Trim();
                }

                // 表示調整
                if (SelectedForm == null)
                {
                    mform.StartPosition = FormStartPosition.CenterScreen;
                }
                else
                {
                    mform.StartPosition = FormStartPosition.Manual;
                    mform.Top = SelectedForm.Top + (SelectedForm.Height - mform.Height) / 2;
                    mform.Left = SelectedForm.Left + (SelectedForm.Width - mform.Width) / 2;
                }
                mform.lblID.Focus();
                mform.Show();
            }
        }

        #endregion

        #endregion

        #region イベント

        #region マルチメディアタイマーイベント

        private void timer_Tick(int id, int uiNo, IntPtr user, IntPtr reserved1, IntPtr reserved2)
        {
            if (id != timerId)
            {
                return;
            }

            KeyboardState DKeyState;        //キーボードデバイス状態
            JoystickState DJoyState;        //ジョイスティックデバイス状態

            #region ジョイスティック接続チェック

            // ジョイスティック接続チェック
            if (DInDevJOY == null)
            {
                // ジョイスティック検出(列挙した1台目のみ対応)
                foreach (
                    DeviceInstance di in
                    Manager.GetDevices(
                    DeviceClass.GameControl,
                    EnumDevicesFlags.AttachedOnly))
                {
                    if (di.DeviceType == DeviceType.Joystick)
                    {
                        // デバイスの作成と設定
                        DInDevJOY = new Device(di.InstanceGuid);

                        // 各軸の設定
                        foreach (DeviceObjectInstance doi in DInDevJOY.Objects)
                        {
                            if ((doi.ObjectId & (int)DeviceObjectTypeFlags.Axis) != 0)
                            {
                                // レンジを設定
                                DInDevJOY.Properties.SetRange(ParameterHow.ById, doi.ObjectId, new InputRange(0, 65535));
                            }

                            // フォースフィードバックのある軸情報を記憶
                            int[] temp;
                            if ((doi.Flags & (int)ObjectInstanceFlags.Actuator) != 0)
                            {
                                // テーブルの確保
                                if (DInAxis != null)
                                {
                                    temp = new int[DInAxis.Length + 1];
                                    DInAxis.CopyTo(temp, 0);
                                    DInAxis = temp;
                                }
                                else
                                {
                                    DInAxis = new int[1];
                                }
                                // 情報の記憶
                                DInAxis[DInAxis.Length - 1] = doi.Offset;
                            }
                        }
                        isForceFeedback = DInDevJOY.Caps.ForceFeedback;

                        // ジョイスティックの設定と入力制御開始
                        SetJoyStickProperties();

                        if (ConnectNotice != null)
                        {
                            // ハンドラが設定されていたらイベント発生
                            ConnectNoticeEventArgs eArgs = new ConnectNoticeEventArgs();
                            eArgs.Id = 0;
                            eArgs.IsConnect = true;
                            eArgs.IsForceFeedback = isForceFeedback;
                            CallConnectMonitor(eArgs);
                            CallConnectNotice(this, eArgs);
                        }
                        break;
                    }
                }
            }

            #endregion

            #region ジョイスティック読取

            // ジョイスティック読取
            DJoyState = new JoystickState();
            if (DInDevJOY != null)
            {
                try
                {
                    DJoyState = DInDevJOY.CurrentJoystickState;
                }
                catch (Exception)
                {
                    // 途中でジョイスティックを抜いた
                    DInEffect = null;
                    DInDevJOY = null;
                    DInAxis = null;
                    isForceFeedback = false;

                    if (ConnectNotice != null)
                    {
                        // ハンドラが設定されていたらイベント発生
                        ConnectNoticeEventArgs eArgs = new ConnectNoticeEventArgs();
                        eArgs.Id = -1;
                        eArgs.IsConnect = false;
                        eArgs.IsForceFeedback = isForceFeedback;
                        CallConnectMonitor(eArgs);
                        CallConnectNotice(this, eArgs);
                    }
                }
            }

            #endregion

            #region キーボード読取

            // キーボード読取
            DKeyState = DInDevKBD.GetCurrentKeyboardState();

            #endregion

            #region 通知データ作成

            // 通知データ作成
            if (udpstopcount > 0) udpstopcount--;
            InputStateBuilder inpstat = new InputStateBuilder(useKeyboard, DKeyState, DInDevJOY != null, DJoyState, udpstopcount > 0, DRmtState, map, key, kmap);

            #endregion

            #region UDP送信(Noticeプロパティの内容を同期)

            if (rsendflg == true)
            {
                // 送信(双方向)
                string sd = "JN" + noticeText;
                if (xUDPComControl != null) xUDPComControl.SendLine(sd, Encoding.Default, ec, eclen);
                rsendflg = false;
            }

            #endregion

            if (inpstat != null)
            {
                // 通知内容チェック
                DRmtState.SetInputStatus(inpstat.GetInputState());
                string sd = DRmtState.ToUDPString();
                if ((pJoyStatus != sd) || (pForceSendWaitCount == 0))
                {
                    pForceSendWaitCount = forceSendInterval / samplingInterval;

                    #region UDP送信(ジョイスティック状態を同期)

                    if (remoteMode == enumRemoteMode.Send)
                    {
                        // 送信(Send側→Recive側)
                        if (xUDPComControl != null) xUDPComControl.SendLine(sd, Encoding.Default, ec, eclen);
                    }

                    #endregion

                    #region イベント通知

                    if (InputNotice != null)
                    {
                        // ハンドラが設定されていたらイベント発生
                        InputNoticeEventArgs eArgs = new InputNoticeEventArgs();
                        eArgs.Status = inpstat.GetInputState();
                        eArgs.Status.Notice = noticeText;
                        eArgs.SendData = sd;
                        CallInputMonitor(eArgs);
                        CallInputNotice(this, eArgs);
                    }

                    #endregion

                }
                pForceSendWaitCount--;
                pJoyStatus = sd;
            }
        }

        #endregion

        #region UDP受信イベント

        private void xudpComControl_ReciveEvent(object sender, XControls.XUDPComControl.ReciveEventArgs e)
        {
            // 受信
            string rd = xUDPComControl.GetReciveLine(Encoding.Default, ec, eclen);
            if(rd.Length>2)
            {
                // 状態メッセージ(Send側→Recive側)
                if (rd.Substring(0, 2) == "JS" && remoteMode == enumRemoteMode.Recive)
                {
                    // リモート操作(UDP受信)ステータスを記憶
                    DRmtState.FromUDPString(rd);

                    // UDP通信途絶状態を更新
                    if (udpstopcount == 0) DRmtState.Reset();
                    udpstopcount = reciveTimeoutInterval / samplingInterval;
                    return;
                }

                // 通知メッセージ(双方向)
                if (rd.Substring(0, 2) == "JN")
                {
                    noticeText = rd.Substring(2, rd.Length - 3);    // 通知文字列
                    return;
                }
            }
        }

        #endregion

        #endregion

        #region 追加のイベント

        #region ConnectNoticeイベント

        public class ConnectNoticeEventArgs : EventArgs
        {
            public int Id = -1;
            public Boolean IsConnect = false;
            public Boolean IsForceFeedback = false;
        }
        public delegate void ConnectNoticeEventHandler(object sender, ConnectNoticeEventArgs e);
        [
            Category("動作"),
            Description("ジョイスティックの接続状態を通知します。")
        ]
        public event ConnectNoticeEventHandler ConnectNotice;

        #endregion

        #region InputNoticeイベント

        /// <summary>
        /// InputNoticeイベント
        /// </summary>
        public class InputNoticeEventArgs : EventArgs
        {
            public InputState Status;
            public string SendData;
        }
        public delegate void InputNoticeEventHandler(object sender, InputNoticeEventArgs e);
        [
            Category("動作"),
            Description("ジョイスティックおよびキーボードの操作状態を通知します。")
        ]
        public event InputNoticeEventHandler InputNotice;

        #endregion

        #endregion

        #region その他

        #region マルチメディアタイマー関連

        #region 監視タイマースタート

        /// <summary>
        /// 監視タイマーをスタートする
        /// </summary>
        /// <remarks>
        /// SAMPLECOUNT/1000秒間隔でイベント発生
        /// </remarks>
        private void timerStart()
        {
            if (timerId == 0)
            {
                TickedCallback tick = new TickedCallback(timer_Tick);
                callback = GCHandle.Alloc(tick);
                timerId = TimeSetEvent(samplingInterval, 0, tick, IntPtr.Zero, TimerEventTypes.Periodic);
                if (timerId == 0)
                {
                    this.callback.Free();
                    throw new InvalidOperationException("マルチメディアタイマの初期化に失敗しました。");
                }
            }
        }

        #endregion

        #region 監視タイマーストップ

        /// <summary>
        /// 監視タイマーをストップする
        /// </summary>
        private void timerStop()
        {
            if (timerId != 0)
            {
                TimeKillEvent(timerId);
                if (callback.IsAllocated)
                {
                    callback.Free();
                }
            }
            timerId = 0;
        }

        #endregion

        #region Win32API

        private delegate void TickedCallback(int id, int uiNo, IntPtr user, IntPtr reserved1, IntPtr reserved2);

        [DllImport("winmm.dll", EntryPoint = "timeSetEvent")]
        private static extern int TimeSetEvent(int delay, int resolution, TickedCallback ticked, IntPtr user, TimerEventTypes type);
        [DllImport("winmm.dll", EntryPoint = "timeKillEvent")]
        private static extern int TimeKillEvent(int id);
        [Flags]
        private enum TimerEventTypes : int
        {
            OneShot = 0x00,
            Periodic = 0x01,
        }

        #endregion

        #endregion

        #region ライセンスチェック

        private void timerLicenseCheck_Tick(object sender, EventArgs e)
        {
            timerLicenseCheck.Enabled = false;

            // ライセンスチェック
            if (DesignMode == false && useLocalLicense == false)
            {
                MessageBox.Show("LocalLicenseKeyを設定してください。", "XJoyStickControlコントロール", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region ジョイスティックの設定と入力制御開始

        delegate void  SetJoyStickPropertiesDelegate();

        private void SetJoyStickProperties()
        {
            if (formObject != null)
            {
                if (formObject.InvokeRequired)
                {
                    // 別スレッドから呼び出された場合
                    try
                    {
                        formObject.Invoke(new SetJoyStickPropertiesDelegate(SetJoyStickProperties));
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    catch (UnsupportedException)
                    {
                    }
                    return;
                }
            }

            // 協調モードおよび自動センタ機能の設定
            if (formObject != null && formObject.IsDisposed == false)
            {
                DInDevJOY.SetCooperativeLevel(formObject.Handle, CooperativeLevelFlags.Exclusive | CooperativeLevelFlags.Background);
            }

            try
            {
                DInDevJOY.Properties.AutoCenter = false;
            }
            catch (ObjectDisposedException)
            {
            }
            catch (UnsupportedException)
            {
            }

            // ジョイスティック入力制御開始
            DInDevJOY.Acquire();

            // フォースフィードバック設定
            if (isForceFeedback)
            {
                Effect ef;
                int[] axis = new int[1];
                foreach (EffectInformation ei in DInDevJOY.GetEffects(EffectType.All))
                {
                    if (DInputHelper.GetTypeCode(ei.EffectType) == (int)EffectType.ConstantForce)
                    {
                        ef = new Effect();
                        ef.SetDirection(new int[DInAxis.Length]);
                        ef.SetAxes(new int[DInAxis.Length]);
                        ef.ConditionStruct = new Condition[DInAxis.Length];
                        ef.EffectType = EffectType.ConstantForce;
                        ef.Duration = (int)DI.Infinite;
                        ef.Gain = 10000;
                        ef.Constant = new ConstantForce();
                        ef.Constant.Magnitude = 10000;
                        ef.SamplePeriod = 0;
                        ef.TriggerButton = (int)Microsoft.DirectX.DirectInput.Button.NoTrigger;
                        ef.TriggerRepeatInterval = (int)DI.Infinite;
                        ef.Flags = EffectFlags.ObjectOffsets | EffectFlags.Cartesian;
                        ef.SetAxes(DInAxis);

                        try
                        {
                            DInEffect = new EffectObject(ei.EffectGuid, ef, DInDevJOY);
                        }
                        catch (Exception es)
                        {
                            //ThreadExceptionDialog ed = new ThreadExceptionDialog(es);
                            //ed.Text = "フォースフィードバック効果が設定できません";
                            //ed.ShowDialog();
                            MessageBox.Show(es.Message + "\n" + es.StackTrace, "フォースフィードバック効果が設定できません", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            isForceFeedback = false;
                        }
                        break;
                    }
                }
            }
            return;
        }

        #endregion

        #region 接続通知

        delegate void CallConnectNoticeDelegate(object sender, ConnectNoticeEventArgs e);

        private void CallConnectNotice(object sender, ConnectNoticeEventArgs e)
        {
            if (formObject != null)
            {
                if (formObject.InvokeRequired)
                {
                    // 別スレッドから呼び出された場合
                    object[] param = { sender, e };
                    try
                    {
                        formObject.Invoke(new CallConnectNoticeDelegate(CallConnectNotice), param);
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    return;
                }
            }

            try
            {
                ConnectNotice(sender, e);
            }
            catch
            {
            }
        }

        #endregion

        #region 接続モニタ

        delegate void CallConnectMonitorDelegate(ConnectNoticeEventArgs e);
        private String monID = "";
        private Color monCNCol = Color.Gray;

        private void CallConnectMonitor(ConnectNoticeEventArgs e)
        {
            if (mform == null)
            {
                if (e.IsConnect)
                {
                    monID = "ID" + e.Id.ToString();
                    if (e.IsForceFeedback)
                    {
                        monCNCol = Color.Orange;
                    }
                    else
                    {
                        monCNCol = Color.LimeGreen;
                    }
                }
                else
                {
                    monID = "---";
                    monCNCol = Color.Gray;
                }
                return;
            }
            if (mform.InvokeRequired)
            {
                // 別スレッドから呼び出された場合
                object[] param = { e };
                try
                {
                    mform.Invoke(new CallConnectMonitorDelegate(CallConnectMonitor), param);
                }
                catch (ObjectDisposedException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                return;
            }

            // モニタ画面
            if (e.IsConnect)
            {
                mform.lblID.Text = "ID" + e.Id.ToString();
                if (e.IsForceFeedback)
                {
                    mform.lblStatus.BackColor = Color.Orange;
                }
                else
                {
                    mform.lblStatus.BackColor = Color.LimeGreen;
                }
            }
            else
            {
                mform.lblID.Text = "---";
                mform.lblStatus.BackColor = Color.Gray;
            }
        }

        #endregion

        #region 入力通知

        delegate void CallInputNoticeDelegate(object sender, InputNoticeEventArgs e);

        private void CallInputNotice(object sender, InputNoticeEventArgs e)
        {
            if (formObject != null)
            {
                if (formObject.InvokeRequired)
                {
                    // 別スレッドから呼び出された場合
                    object[] param = { sender, e };
                    try
                    {
                        formObject.Invoke(new CallInputNoticeDelegate(CallInputNotice), param);
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    return;
                }
            }

            try
            {
                InputNotice(sender, e);
            }
            catch
            {
            }
        }

        #endregion

        #region 入力モニタ

        delegate void CallInputMonitorDelegate(InputNoticeEventArgs e);

        private void CallInputMonitor(InputNoticeEventArgs e)
        {
            int i;

            if (mform == null) return;
            if (mform.InvokeRequired)
            {
                // 別スレッドから呼び出された場合
                object[] param = { e };
                try
                {
                    mform.Invoke(new CallInputMonitorDelegate(CallInputMonitor), param);
                }
                catch (ObjectDisposedException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                return;
            }

            // モニタ画面
            for (i = 0; i < 32; i++)
            {
                // ボタン1-32 (29-32はPOV)
                if (e.Status.Buttons[i] == 0)
                {
                    mform.lbl[i].BackColor = Color.Gray;
                }
                else
                {
                    mform.lbl[i].BackColor = Color.Red;
                }
            }

            // x,y,z,Rz軸
            Int64 x = e.Status.X;
            Int64 y = e.Status.Y;
            Int64 z = e.Status.Z;
            Int64 rz = e.Status.Rz;
            mform.lblALx.Text = x.ToString();
            mform.lblALy.Text = y.ToString();
            mform.lblARx.Text = z.ToString();
            mform.lblARy.Text = rz.ToString();

            // x,y軸
            mform.picL.Refresh();
            Graphics gl = mform.picL.CreateGraphics();
            int gw = mform.picL.ClientRectangle.Width;
            int gh = mform.picL.ClientRectangle.Height;
            Brush back = new SolidBrush(Color.Transparent);
            Brush jpos = new SolidBrush(Color.FromArgb(127, 255, 255));
            float zz = (float)(gw - 7) / 65536;
            gl.FillRectangle(back, 0, 0, gw, gh);
            gl.FillRectangle(jpos, (int)(x * zz), (int)(y * zz), 8, 8);

            // z,Rz軸
            mform.picR.Refresh();
            Graphics gr = mform.picR.CreateGraphics();
            gr.FillRectangle(back, 0, 0, gw, gh);
            gr.FillRectangle(jpos, (int)(z * zz), (int)(rz * zz), 8, 8);
        }

        #endregion

        #endregion
    }
}
