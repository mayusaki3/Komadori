using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace XControls
{
    /// <summary>
    /// ウェブカメラからの画像を操作するコントロールです。
    /// </summary>
   /// <remarks>
    /// 次のCOMコンポーネントへの参照を追加してください。
    ///     Windows Media Player (wmp.dll)
    /// 追加後、参照設定でWMPLibのプロパティ「相互運用機能型の埋め込み」をflaseに設定してください。
    /// </remarks>
    [Designer(typeof(XSoundPlayDesigner))]
    public partial class XSoundPlay : UserControl
    {
        #region インナークラス

        #region XSoundPlayDesignerクラス

        /// <summary>
        /// XSoundPlay用にデザイナをカスタマイズします。
        /// </summary>
        public class XSoundPlayDesigner : System.Windows.Forms.Design.ControlDesigner
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.XSoundPlay.XSoundPlayDesigner クラスの新しいインスタンスを初期化します。
            /// </summary>
            public XSoundPlayDesigner()
            {
            }

            #endregion

            #region メソッド

            #region PostFilterPropertiesメソッド

            protected override void PostFilterProperties(IDictionary properties)
            {
                // フィルタリングするプロパティ
                properties.Remove("AutoScroll");
                properties.Remove("AutoScrollMargin");
                properties.Remove("AutoScrollMinSize");
                properties.Remove("ImeMode");
                properties.Remove("RightToLeft");
                properties.Remove("ForeColor");

                base.PostFilterProperties(properties);
            }

            #endregion

            #endregion
        }

        #endregion

        #endregion

        #region 構築・破棄

        /// <summary>
        /// XControls.XSoundPlay クラスの新しいインスタンスを初期化します。
        /// </summary>
        public XSoundPlay()
        {
            wmp.settings.autoStart = false;
            wmp.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(wmp_PlayStateChange);
            wmp.PositionChange += new WMPLib._WMPOCXEvents_PositionChangeEventHandler(wmp_PositionChange);
            InitializeComponent();
            lblSoundName.Enabled = false;
            ControlRelocation();
        }

        /// <summary>
        /// 後処理を行います。
        /// </summary>
        void Terminate()
        {
            wmp.stop();
            wmp.close();
            Thread.Sleep(100);
            wmp.PlayStateChange -= new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(wmp_PlayStateChange);
            wmp.PositionChange -= new WMPLib._WMPOCXEvents_PositionChangeEventHandler(wmp_PositionChange);
        }

        #endregion

        #region 定数

        /// <summary>
        /// ボタン間隔です。
        /// </summary>
        const int SPACE = 2;

        #endregion

        #region 変数

        /// <summary>
        /// Volume操作中かを表します。
        /// </summary>
        private bool isVolDrag = false;

        /// <summary>
        /// Windows Media Playerです。
        /// </summary>
        WMPLib.WindowsMediaPlayerClass wmp = new WMPLib.WindowsMediaPlayerClass();

        /// <summary>
        /// シーク操作までの時間待ちカウンタです。
        /// </summary>
        private int seekmodect = -1;

        /// <summary>
        /// シーク操作中かを表します。
        /// </summary>
        private bool isSeekOn = false;
 
        /// <summary>
        /// シーク表示中にサウンドのタイトルを退避します。
        /// </summary>
        private string savTitle = "";

        #endregion

        #region プロパティ

        #region 追加のプロパティ

        #region SoundFileプロパティ

        private string soundFile = "";
        /// <summary>
        /// 再生するサウンドファイル名を参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(""),
            Description("再生するサウンドファイル名を参照または設定します。"),
            EditorAttribute(
                typeof(System.Windows.Forms.Design.FileNameEditor),
                typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string SoundFile
        {
            get
            {
                return soundFile;
            }
            set
            {
                wmp.stop();
                wmp.close();
                PauseShow = false;
                PlayShow = false;
                if (value.Length > 0)
                {
                    Uri appdir = new Uri(Application.StartupPath + "\\");
                    Uri reldir = new Uri(appdir, value);
                    soundFile = System.Uri.UnescapeDataString(appdir.MakeRelativeUri(reldir).ToString());
                    soundFile = soundFile.Replace("/", "\\");
                    Uri fulldir = new Uri(appdir, System.Environment.ExpandEnvironmentVariables(soundFile));
                    if (title.Length == 0)
                    {
                        string[] st = fulldir.Segments;
                        title = Uri.UnescapeDataString(st[st.Length - 1]);
                    }
                    WMPLib.IWMPMedia media = wmp.newMedia(fulldir.LocalPath);
                    wmp.currentMedia = media;
                    wmp.controls.play();
                    Thread.Sleep(100);
                    wmp.controls.stop();
                }
                else
                {
                    soundFile = "";
                }
            }
        }

        #endregion

        #region IsPlayShowColorプロパティ

        private bool isPlayShowColor = false;
        /// <summary>
        /// 確認用にサウンド再生時の表示に切り替えます。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(false),
            Description("確認用にサウンド再生時の表示に切り替えます。")
        ]
        public bool IsPlayShowColor
        {
            get
            {
                return isPlayShowColor;
            }
            set
            {
                isPlayShowColor = value;
                PlayShow = playShow;
                btnPlay.IsPushShowColor = isPlayShowColor;
                picTime.Invalidate();
                lblVol.Invalidate();
                FasterRate = fasterRate;
                Loop = loop;
            }
        }

        #endregion

        #region Titleプロパティ

        private string title = "";
        /// <summary>
        /// サウンドのタイトルを参照または設定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(""),
            Description("サウンドのタイトルを参照または設定します。"),
        ]
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                if (title.Length==0)
                {
                    if (soundFile.Length > 0)
                    {
                        Uri appdir = new Uri(Application.StartupPath);
                        Uri fulldir = new Uri(appdir, System.Environment.ExpandEnvironmentVariables(soundFile));
                        string[] st = fulldir.Segments;
                        title = Uri.UnescapeDataString(st[st.Length - 1]);
                    }
                }
                string dsc = description;
                if (dsc.Length > 0) dsc = " … " + dsc;
                lblSoundName.Text = Title + dsc;
            }
        }

        #endregion

        #region Descriptionプロパティ

        private string description = "";
        /// <summary>
        /// サウンドの説明を参照または設定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(""),
            Description("サウンドの説明を参照または設定します。"),
        ]
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                string dsc = description;
                if (dsc.Length > 0) dsc = " … " + dsc;
                lblSoundName.Text = Title + dsc;
            }
        }

        #endregion

        #region Volumeプロパティ

        private uint volume = 100;
        /// <summary>
        /// サウンドの音量を参照または設定します。範囲は0～100です。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(100),
            Description("サウンドの音量を参照または設定します。範囲は0～100です。"),
        ]
        public uint Volume
        {
            get
            {
                return volume;
            }
            set
            {
                volume = value;
                if (volume > 100)
                {
                    volume = 100;
                }

                // Volume表示
                lblVol.Text = "Vol " + volume.ToString("000");
                int r = (VolumeMaxBackColor.R - VolumeMinBackColor.R) * (int)volume / 100 + VolumeMinBackColor.R;
                int g = (VolumeMaxBackColor.G - VolumeMinBackColor.G) * (int)volume / 100 + VolumeMinBackColor.G;
                int b = (VolumeMaxBackColor.B - VolumeMinBackColor.B) * (int)volume / 100 + VolumeMinBackColor.B;
                lblVol.BackColor = Color.FromArgb(r, g, b);

                wmp.settings.volume = (int)volume;
            }
        }

        #endregion

        #region Loopプロパティ

        private bool loop = false;
        /// <summary>
        /// ループ再生を行うかどうかを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("ループ再生を行うかどうかを参照または設定します。"),
        ]
        public bool Loop
        {
            get
            {
                return loop;
            }
            set
            {
                loop = value;
                if (loop || isPlayShowColor)
                {
                    lblLoop.BackColor = modeOnBackColor;
                }
                else
                {
                    lblLoop.BackColor = modeOffBackColor;
                }
            }
        }

        #endregion

        #region FasterRateプロパティ

        private bool fasterRate = false;
        /// <summary>
        /// サウンドを高速(1.5倍速)で再生するかどうかを参照または設定します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("サウンドを高速(1.5倍速)で再生するかどうかを参照または設定します。"),
        ]
        public bool FasterRate
        {
            get
            {
                return fasterRate;
            }
            set
            {
                fasterRate = value;
                if (fasterRate)
                {
                    lblFasterRate.BackColor = modeOnBackColor;
                    wmp.settings.rate = 1.5;
                }
                else
                {
                    lblFasterRate.BackColor = modeOffBackColor;
                    wmp.settings.rate = 1;
                }
            }
        }

        #endregion

        #region ButtonForeColorプロパティ

        private Color buttonForeColor = Color.Black;
        /// <summary>
        /// 操作ボタンの前景色です。
        /// </summary>
        [
            Category("表示"),
            Description("操作ボタンの前景色です。")
        ]
        public Color ButtonForeColor
        {
            get
            {
                return buttonForeColor;
            }
            set
            {
                buttonForeColor = value;
                btnPause.PushOffForeColor = buttonForeColor;
                btnRewind.PushOffForeColor = buttonForeColor;
                btnStop.PushOffForeColor = buttonForeColor;
                btnPlay.PushOffForeColor = buttonForeColor;
                btnPause.PushOnForeColor = buttonForeColor;
                btnRewind.PushOnForeColor = buttonForeColor;
                btnStop.PushOnForeColor = buttonForeColor;
                btnPlay.PushOnForeColor = buttonForeColor;
            }
        }

        #endregion

        #region ButtonOnBackColorプロパティ

        private Color buttonOnBackColor = Color.WhiteSmoke;
        /// <summary>
        /// 操作ボタンの背景色です。
        /// </summary>
        [
            Category("表示"),
            Description("操作ボタンの背景色です。")
        ]
        public Color ButtonOnBackColor
        {
            get
            {
                return buttonOnBackColor;
            }
            set
            {
                buttonOnBackColor = value;
                btnPause.PushOnBackColor = buttonOnBackColor;
                btnRewind.PushOnBackColor = buttonOnBackColor;
                btnStop.PushOnBackColor = buttonOnBackColor;
                btnPlay.PushOnBackColor = buttonOnBackColor;
            }
        }

        #endregion

        #region ButtonOffBackColorプロパティ

        private Color buttonOffBackColor = Color.WhiteSmoke;
        /// <summary>
        /// 操作ボタンの背景色です。
        /// </summary>
        [
            Category("表示"),
            Description("操作ボタンの背景色です。")
        ]
        public Color ButtonOffBackColor
        {
            get
            {
                return buttonOffBackColor;
            }
            set
            {
                buttonOffBackColor = value;
                btnPause.PushOffBackColor = buttonOffBackColor;
                btnRewind.PushOffBackColor = buttonOffBackColor;
                btnStop.PushOffBackColor = buttonOffBackColor;
                btnPlay.PushOffBackColor = buttonOffBackColor;
            }
        }

        #endregion

        #region VolumeMaxBackColorプロパティ

        /// <summary>
        /// ボリューム100の時の表示色です。
        /// </summary>
        [
            Category("表示"),
            Description("ボリューム100の時の表示色です。")
        ]
        public Color VolumeMaxBackColor
        {
            get
            {
                return lbl100.BackColor;
            }
            set
            {
                lbl100.BackColor = value;
                lblVol.BackColor = lbl100.BackColor;
                lblVol.Invalidate();
            }
        }

        #endregion

        #region VolumeMinBackColorプロパティ

        /// <summary>
        /// ボリューム0の時の表示色です。
        /// </summary>
        [
            Category("表示"),
            Description("ボリューム0の時の表示色です。")
        ]
        public Color VolumeMinBackColor
        {
            get
            {
                return lbl0.BackColor;
            }
            set
            {
                lbl0.BackColor = value;
                lblVol.Invalidate();
            }
        }

        #endregion

        #region VolumeForeColorプロパティ

        /// <summary>
        /// ボリュームの前景色です。
        /// </summary>
        [
            Category("表示"),
            Description("ボリュームの前景色です。")
        ]
        public Color VolumeForeColor
        {
            get
            {
                return lblVol.ForeColor;
            }
            set
            {
                lblVol.ForeColor = value;
            }
        }

        #endregion

        #region ModeOnBackColorプロパティ

        private Color modeOnBackColor = Color.SpringGreen;
        /// <summary>
        /// 操作ボタンの前景色です。
        /// </summary>
        [
            Category("表示"),
            Description("操作ボタンの前景色です。")
        ]
        public Color ModeOnBackColor
        {
            get
            {
                return modeOnBackColor;
            }
            set
            {
                modeOnBackColor = value;
                FasterRate = fasterRate;
                Loop = loop;
            }
        }

        #endregion

        #region ModeOffBackColorプロパティ

        private Color modeOffBackColor = Color.ForestGreen;
        /// <summary>
        /// 操作ボタンの背景色です。
        /// </summary>
        [
            Category("表示"),
            Description("操作ボタンの背景色です。")
        ]
        public Color ModeOffBackColor
        {
            get
            {
                return modeOffBackColor;
            }
            set
            {
                modeOffBackColor = value;
                FasterRate = fasterRate;
                Loop = loop;
            }
        }

        #endregion

        #region ModeForeColorプロパティ

        /// <summary>
        /// ボリュームの前景色です。
        /// </summary>
        [
            Category("表示"),
            Description("ボリュームの前景色です。")
        ]
        public Color ModeForeColor
        {
            get
            {
                return lblFasterRate.ForeColor;
            }
            set
            {
                lblFasterRate.ForeColor = value;
                lblLoop.ForeColor = lblFasterRate.ForeColor;
            }
        }

        #endregion

        #region PlayForeColorプロパティ

        private Color playForeColor = Color.FromArgb(128, 64, 0);
        /// <summary>
        /// サウンド再生時の前景色です。
        /// </summary>
        [
            Category("表示"),
            Description("サウンド再生時の前景色です。")
        ]
        public Color PlayForeColor
        {
            get
            {
                return playForeColor;
            }
            set
            {
                playForeColor = value;
                PlayShow = playShow;
                Refresh();
            }
        }

        #endregion

        #region PlayBackColorプロパティ

        private Color playBackColor = Color.DarkOrange;
        /// <summary>
        /// サウンド再生時の背景色です。
        /// </summary>
        [
            Category("表示"),
            Description("サウンド再生時の背景色です。")
        ]
        public Color PlayBackColor
        {
            get
            {
                return playBackColor;
            }
            set
            {
                playBackColor = value;
                PlayShow = playShow;
                Refresh();
            }
        }

        #endregion

        #region StopForeColorプロパティ

        private Color stopForeColor = Color.FromArgb(128, 64, 0);
        /// <summary>
        /// サウンド停止時の前景色です。
        /// </summary>
        [
            Category("表示"),
            Description("サウンド停止時の前景色です。")
        ]
        public Color StopForeColor
        {
            get
            {
                return stopForeColor;
            }
            set
            {
                stopForeColor = value;
                PlayShow = playShow;
                Refresh();
            }
        }

        #endregion

        #region StopBackColorプロパティ

        private Color stopBackColor = Color.Sienna;
        /// <summary>
        /// サウンド停止時の背景色です。
        /// </summary>
        [
            Category("表示"),
            Description("サウンド停止時の背景色です。")
        ]
        public Color StopBackColor
        {
            get
            {
                return stopBackColor;
            }
            set
            {
                stopBackColor = value;
                PlayShow = playShow;
                Refresh();
            }
        }

        #endregion

        #region PositionForeColorプロパティ

        private Color positionForeColor = Color.Yellow;
        /// <summary>
        /// サウンド再生位置バーの前景色です。
        /// </summary>
        [
            Category("表示"),
            Description("サウンド再生位置バーの前景色です。")
        ]
        public Color PositionForeColor
        {
            get
            {
                return positionForeColor;
            }
            set
            {
                positionForeColor = value;
                Refresh();
            }
        }

        #endregion

        #region PositionBackColorプロパティ

        private Color positionBackColor = Color.FromArgb(64, 64, 32);
        /// <summary>
        /// サウンド再生位置バーの背景色です。
        /// </summary>
        [
            Category("表示"),
            Description("サウンド再生位置バーの背景色です。")
        ]
        public Color PositionBackColor
        {
            get
            {
                return positionBackColor;
            }
            set
            {
                positionBackColor = value;
                Refresh();
            }
        }

        #endregion

        #region MiniPanelプロパティ

        private bool miniPanel = false;
        /// <summary>
        /// ミニパネルモードかどうかを参照または設定します。
        /// </summary>
        [
            Category("表示"),
            DefaultValue(false),
            Description("ミニパネルモードかどうかを参照または設定します。"),
        ]
        public bool MiniPanel
        {
            get
            {
                return miniPanel;
            }
            set
            {
                miniPanel = value;
                if (miniPanel)
                {
                    btnPause.Visible = false;
                    btnRewind.Visible = false;
                    btnStop.Visible = false;
                    lblLoop.Visible = false;
                    lblFasterRate.Visible = false;
                }
                else
                {
                    btnPause.Visible = true;
                    btnRewind.Visible = true;
                    btnStop.Visible = true;
                    lblLoop.Visible = true;
                    lblFasterRate.Visible = true;
                }
                ControlRelocation();
            }
        }

        #endregion

        #region PlayShowプロパティ(Private)

        private bool playShow = false;
        private bool PlayShow
        {
            get
            {
                return playShow;
            }
            set
            {
                playShow = value;
                if (playShow || isPlayShowColor)
                {
                    lblSoundName.BackColor = playBackColor;
                    lblSoundName.ForeColor = playForeColor;
                }
                else
                {
                    lblSoundName.BackColor = stopBackColor;
                    lblSoundName.ForeColor = stopForeColor;
                }
                pnlView.BackColor = lblSoundName.BackColor;
                lblMusic.BackColor = lblSoundName.BackColor;
                lblPause.BackColor = lblSoundName.BackColor;
                lblMusic.ForeColor = lblSoundName.ForeColor;
                lblPause.ForeColor = lblSoundName.ForeColor;
                Application.DoEvents();
            }
        }

        #endregion

        #region PauseShowプロパティ(Private)

        private bool pauseShow = false;
        private bool PauseShow
        {
            get
            {
                return pauseShow;
            }
            set
            {
                pauseShow = value;
                if (pauseShow)
                {
                    lblPause.Visible = true;
                }
                else
                {
                    lblPause.Visible = false;
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region イベント

        #region OnLayoutイベント

        /// <summary>
        /// XControls.XSoundPlay.Layoutイベントを発生させます。
        /// </summary>
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            ControlRelocation();
        }

        #endregion

        #region 操作系イベント

        #region btnPause_Clickイベント

        private void btnPause_Click(object sender, EventArgs e)
        {
            Pause();
            if (!PlayShow) btnPause.IsPushed = false;
        }

        #endregion

        #region btnRewind_Clickイベント

        private void btnRewind_Click(object sender, EventArgs e)
        {
            Rewind();
        }

        #endregion

        #region btnStop_Clickイベント

        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        #endregion

        #region btnPlay_Clickイベント

        private void btnPlay_Click(object sender, EventArgs e)
        {
            Play();
        }

        #endregion

        #region lblLoop_Clickイベント

        private void lblLoop_Click(object sender, EventArgs e)
        {
            Loop = (Loop == false);
        }

        #endregion

        #region lblFasterRate_Clickイベント

        private void lblFasterRate_Click(object sender, EventArgs e)
        {
            FasterRate = (FasterRate == false);
        }

        #endregion

        #region lbl0_MouseDownイベント

        private void lbl0_MouseDown(object sender, MouseEventArgs e)
        {
            isVolDrag = true;
            Volume = 0;
        }

        #endregion

        #region lbl0_MouseMoveイベント

        private void lbl0_MouseMove(object sender, MouseEventArgs e)
        {
            if (isVolDrag)
            {
                if (e.X > lbl0.Width)
                {
                    Volume = (uint)((e.X - lbl0.Width) * 100 / lblVol.Width);
                }
                else
                {
                    Volume = 0;
                }
            }
        }

        #endregion

        #region lbl0_MouseUpイベント

        private void lbl0_MouseUp(object sender, MouseEventArgs e)
        {
            isVolDrag = false;
        }

        #endregion

        #region lbl100_MouseDownイベント

        private void lbl100_MouseDown(object sender, MouseEventArgs e)
        {
            isVolDrag = true;
            Volume = 100;
        }

        #endregion

        #region lbl100_MouseMoveイベント

        private void lbl100_MouseMove(object sender, MouseEventArgs e)
        {
            if (isVolDrag)
            {
                int v = e.X + lbl100.Left - lblVol.Left;
                if (v < 0)
                {
                    v = 0;
                }
                Volume = (uint)(v * 100 / lblVol.Width);
            }
        }

        #endregion

        #region lbl100_MouseUpイベント

        private void lbl100_MouseUp(object sender, MouseEventArgs e)
        {
            isVolDrag = false;
        }

        #endregion

        #region lblVol_MouseDownイベント

        private void lblVol_MouseDown(object sender, MouseEventArgs e)
        {
            isVolDrag = true;
            Volume = (uint)(e.X * 100 / lblVol.Width);
        }

        #endregion

        #region lblVol_MouseMoveイベント

        private void lblVol_MouseMove(object sender, MouseEventArgs e)
        {
            if (isVolDrag)
            {
                if (e.X >= 0)
                {
                    Volume = (uint)(e.X * 100 / lblVol.Width);
                }
                else
                {
                    Volume = 0;
                }
            }
        }

        #endregion

        #region lblVol_MouseUpイベント

        private void lblVol_MouseUp(object sender, MouseEventArgs e)
        {
            isVolDrag = false;
        }

        #endregion

        #region lblSoundName_MouseDownイベント

        private void lblSoundName_MouseDown(object sender, MouseEventArgs e)
        {
            savTitle = Title;
            isSeekOn = false;
            seekmodect = 5;
        }

        #endregion

        #region lblSoundName_MouseMoveイベント

        private void lblSoundName_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSeekOn)
            {
                try
                {
                    double pos = (double)(e.X * 100) / (double)lblSoundName.Width;
                    if (pos < 0) pos = 0;
                    if (pos > 100) pos = 100;
                    lblSoundName.Text = pos.ToString("000") + "%";
                    wmp.controls.currentPosition = pos / 100 * wmp.currentMedia.duration;
                }
                catch
                {
                }
            }
        }

        #endregion

        #region lblSoundName_MouseUpイベント

        private void lblSoundName_MouseUp(object sender, MouseEventArgs e)
        {
            Title = savTitle;
            seekmodect = -1;
            isSeekOn = false;
        }

        #endregion

        #endregion

        #region wmp_PlayStateChangeイベント

        private void wmp_PlayStateChange(int NewState)
        {
            picTime.Refresh();
            if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                // 再生
                PauseShow = false;
                PlayShow = true;
                if(!lblSoundName.Enabled) lblSoundName.Enabled = true;
            }
            if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsPaused)
            {
                // 一時停止
                PauseShow = true;
            }
            if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
            {
                // 停止またはループ
                if (loop && playShow)
                {
                    Stop();
                    Play();
                    PlayShow = true;
                }
                else
                {
                    Stop();
                    lblSoundName.Enabled = false;
                }
            }

            Thread.Sleep(100);
            if (wmp.playState == WMPLib.WMPPlayState.wmppsReady || wmp.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                // 停止
                btnPlay.IsPushed = false;
                btnPause.IsPushed = false;
            }
        }

        #endregion

        #region wmp_PositionChangeイベント

        private void wmp_PositionChange(double oldPosition, double newPosition)
        {
            picTime.Refresh();
        }

        #endregion

        #region lblVol_Paintイベント

        private void lblVol_Paint(object sender, PaintEventArgs e)
        {
            uint v = volume;
            if (isPlayShowColor) volume = 65;
            int n = (int)(lblVol.Width * (100 - volume) / 100);
            volume = v;
            e.Graphics.FillRectangle(new SolidBrush(lbl0.BackColor), 0, 0, lblVol.Width, lblVol.Height);
            e.Graphics.FillRectangle(new SolidBrush(lblVol.BackColor), 0, 0, lblVol.Width - n, lblVol.Height);
            SizeF siz = e.Graphics.MeasureString(lblVol.Text, lblVol.Font);
            e.Graphics.DrawString(lblVol.Text, lblVol.Font, new SolidBrush(lblVol.ForeColor),
                                    (lblVol.Width - siz.Width) / 2,
                                    (lblVol.Height - siz.Height) / 2);
        }

        #endregion

        #region picTime_Paintイベント

        private void picTime_Paint(object sender, PaintEventArgs e)
        {
            int n = 0;
            if (playShow)
            {
                try
                {
                    if (wmp.currentMedia.duration > 0)
                    {
                        n = (int)(picTime.Width * wmp.controls.currentPosition / wmp.currentMedia.duration);
                    }
                }
                catch
                {
                }
            }
            if (isPlayShowColor)
            {
                n = (int)(picTime.Width / 1.5);
            }
            e.Graphics.FillRectangle(new SolidBrush(positionBackColor), 0, 0, picTime.Width, picTime.Height);
            e.Graphics.FillRectangle(new SolidBrush(positionForeColor), 0, 0, n, picTime.Height);
        }

        #endregion

        #region timer_Tickイベント

        private void timer_Tick(object sender, EventArgs e)
        {
            picTime.Refresh();
            if (seekmodect > -1) seekmodect--;
            if (seekmodect == 0)
            {
                isSeekOn = true;
            }
        }

        #endregion

        #endregion

        #region メソッド

        #region 一時停止 (Pause)

        /// <summary>
        /// サウンドの再生を一時停止します。
        /// </summary>
        public void Pause()
        {
            if (DesignMode) return;
            if (PauseShow)
            {
                wmp.controls.play();
                timer.Enabled = true;
            }
            else
            {
                wmp.controls.pause();
                timer.Enabled = false;
            }
        }

        #endregion

        #region 先頭に戻る (Rewind)

        /// <summary>
        /// サウンドの再生位置を先頭に戻します。
        /// </summary>
        public void Rewind()
        {
            if (DesignMode) return;
            wmp.controls.currentPosition = 0;
        }

        #endregion

        #region 停止 (Stop)

        /// <summary>
        /// サウンドの再生を停止します。
        /// </summary>
        public void Stop()
        {
            if (DesignMode) return;
            if (playShow)
            {
                PauseShow = false;
                PlayShow = false;
                wmp.controls.stop();
                timer.Enabled = false;
                picTime.Refresh();
            }
        }
        
        #endregion

        #region 再生 (Play)

        /// <summary>
        /// サウンドを再生します。
        /// </summary>
        public void Play()
        {
            if (DesignMode) return;
            if (!playShow || pauseShow)
            {
                if (wmp.playState != WMPLib.WMPPlayState.wmppsUndefined)
                {
                    if (!pauseShow)
                    {
                        wmp.controls.currentPosition = 0;
                    }
                    wmp.controls.play();
                    FasterRate = fasterRate;
                    timer.Enabled = true;
                }
            }
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
            #region 操作部

            int hiden = 0;
            if (miniPanel) hiden = 5;
            int min = (btnPause.MinimumSize.Width + SPACE) * (10 - hiden) - SPACE * 4;
            pnlOpe.MinimumSize = new Size(min, btnPause.MinimumSize.Height);
            pnlOpe.MaximumSize = new Size(Width, btnPause.MaximumSize.Height);
            pnlOpe.Left = 0;
            pnlOpe.Top = Height - pnlOpe.Height;
            pnlOpe.Width = Width;
            MinimumSize = new Size(pnlOpe.MinimumSize.Width, 100);

            int h = btnPause.Height;
            int w = pnlView.ClientSize.Width / (10 - hiden) - SPACE;
            if (w > 60) w = 60;

            btnPause.Top = 0;
            btnRewind.Top = 0;
            btnStop.Top = 0;
            btnPlay.Top = 0;
            lblLoop.Top = 0;
            lblFasterRate.Top = 0;
            lbl0.Top = 0;
            lblVol.Top = 1;
            lbl100.Top = 0;

            btnPause.Height = h;
            btnRewind.Height = h;
            btnStop.Height = h;
            btnPlay.Height = h;
            lblLoop.Height = h;
            lblFasterRate.Height = h;
            lbl0.Height = h;
            lblVol.Height = h - 2;
            lbl100.Height = h;
            pnlOpe.Height = h;

            btnPause.Width = w;
            btnRewind.Width = w;
            btnStop.Width = w;
            btnPlay.Width = w;
            lblLoop.Width = w;
            lblFasterRate.Width = w;
            lbl0.Width = w;
            lblVol.Width = pnlOpe.Width - (w + SPACE) * (8 - hiden) - SPACE * 3;
            lbl100.Width = w;

            if (miniPanel)
            {
                btnPlay.Left = 0;
            }
            else
            {
                btnPause.Left = 0;
                btnRewind.Left = btnPause.Right + SPACE + 1;
                btnStop.Left = btnRewind.Right + SPACE + 1;
                btnPlay.Left = btnStop.Right + SPACE + 1;
                lblLoop.Left = btnPlay.Right + SPACE + 1;
                lblFasterRate.Left = lblLoop.Right + SPACE + 1;
            }

            lbl0.Left = pnlView.ClientSize.Width - lbl0.Width - lblVol.Width - lbl100.Width - 1;
            lblVol.Left = lbl0.Right + 1;
            lbl100.Left = lblVol.Right + 1;

            #endregion

            #region 表示部

            picTime.Top = pnlOpe.Top - picTime.Height;
            picTime.Left = 0;
            picTime.Width = Width;

            pnlView.Height = picTime.Top;
            lblMusic.Width = lblMusic.Height / 2;
            lblPause.Width = lblPause.Height / 2;
            lblSoundName.Left = lblMusic.Width + 1;
            lblSoundName.Width = pnlView.Width - lblMusic.Width - lblPause.Width;
            lblSoundName.Top = 0;
            lblSoundName.Height = lblMusic.Height;

            #endregion
        }

        #endregion

        #endregion
    }
}
