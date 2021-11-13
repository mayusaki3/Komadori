namespace XControls
{
    partial class XSoundPlay
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                Terminate();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblMusic = new System.Windows.Forms.Label();
            this.lblPause = new System.Windows.Forms.Label();
            this.picTime = new System.Windows.Forms.PictureBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.pnlView = new System.Windows.Forms.Panel();
            this.lblSoundName = new XControls.UI.ScrollLabel();
            this.lblLoop = new System.Windows.Forms.Label();
            this.lbl0 = new System.Windows.Forms.Label();
            this.lblFasterRate = new System.Windows.Forms.Label();
            this.lbl100 = new System.Windows.Forms.Label();
            this.lblVol = new System.Windows.Forms.Label();
            this.pnlOpe = new System.Windows.Forms.Panel();
            this.btnPause = new XControls.UI.PushButton();
            this.btnRewind = new XControls.UI.PushButton();
            this.btnStop = new XControls.UI.PushButton();
            this.btnPlay = new XControls.UI.PushButton();
            ((System.ComponentModel.ISupportInitialize)(this.picTime)).BeginInit();
            this.pnlView.SuspendLayout();
            this.pnlOpe.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMusic
            // 
            this.lblMusic.BackColor = System.Drawing.Color.Sienna;
            this.lblMusic.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblMusic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblMusic.Font = new System.Drawing.Font("Webdings", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.lblMusic.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblMusic.Location = new System.Drawing.Point(0, 0);
            this.lblMusic.MinimumSize = new System.Drawing.Size(45, 45);
            this.lblMusic.Name = "lblMusic";
            this.lblMusic.Size = new System.Drawing.Size(47, 62);
            this.lblMusic.TabIndex = 1;
            this.lblMusic.Text = "¯";
            this.lblMusic.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPause
            // 
            this.lblPause.BackColor = System.Drawing.Color.Sienna;
            this.lblPause.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblPause.Font = new System.Drawing.Font("Tw Cen MT Condensed", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPause.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblPause.Location = new System.Drawing.Point(484, 0);
            this.lblPause.MinimumSize = new System.Drawing.Size(45, 45);
            this.lblPause.Name = "lblPause";
            this.lblPause.Size = new System.Drawing.Size(47, 62);
            this.lblPause.TabIndex = 11;
            this.lblPause.Text = "PAUSE";
            this.lblPause.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPause.Visible = false;
            // 
            // picTime
            // 
            this.picTime.BackColor = System.Drawing.Color.Yellow;
            this.picTime.Location = new System.Drawing.Point(3, 69);
            this.picTime.Name = "picTime";
            this.picTime.Size = new System.Drawing.Size(522, 3);
            this.picTime.TabIndex = 12;
            this.picTime.TabStop = false;
            this.picTime.Paint += new System.Windows.Forms.PaintEventHandler(this.picTime_Paint);
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // pnlView
            // 
            this.pnlView.Controls.Add(this.lblPause);
            this.pnlView.Controls.Add(this.lblMusic);
            this.pnlView.Controls.Add(this.lblSoundName);
            this.pnlView.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlView.Location = new System.Drawing.Point(0, 0);
            this.pnlView.Name = "pnlView";
            this.pnlView.Size = new System.Drawing.Size(531, 62);
            this.pnlView.TabIndex = 14;
            // 
            // lblSoundName
            // 
            this.lblSoundName.BackColor = System.Drawing.Color.Sienna;
            this.lblSoundName.Font = new System.Drawing.Font("メイリオ", 20F, System.Drawing.FontStyle.Bold);
            this.lblSoundName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblSoundName.Location = new System.Drawing.Point(201, 10);
            this.lblSoundName.MoveInterval = 3000;
            this.lblSoundName.MoveSpeed = 50;
            this.lblSoundName.Name = "lblSoundName";
            this.lblSoundName.Size = new System.Drawing.Size(129, 40);
            this.lblSoundName.TabIndex = 13;
            this.lblSoundName.Text = "SoundName";
            this.lblSoundName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblSoundName_MouseDown);
            this.lblSoundName.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblSoundName_MouseMove);
            this.lblSoundName.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblSoundName_MouseUp);
            // 
            // lblLoop
            // 
            this.lblLoop.BackColor = System.Drawing.Color.ForestGreen;
            this.lblLoop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLoop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblLoop.Font = new System.Drawing.Font("Webdings", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.lblLoop.ForeColor = System.Drawing.Color.Black;
            this.lblLoop.Location = new System.Drawing.Point(213, 3);
            this.lblLoop.Name = "lblLoop";
            this.lblLoop.Size = new System.Drawing.Size(46, 46);
            this.lblLoop.TabIndex = 6;
            this.lblLoop.Text = "q";
            this.lblLoop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLoop.Click += new System.EventHandler(this.lblLoop_Click);
            // 
            // lbl0
            // 
            this.lbl0.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.lbl0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbl0.Font = new System.Drawing.Font("Tw Cen MT Condensed", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl0.ForeColor = System.Drawing.Color.Black;
            this.lbl0.Location = new System.Drawing.Point(317, 3);
            this.lbl0.Name = "lbl0";
            this.lbl0.Size = new System.Drawing.Size(46, 46);
            this.lbl0.TabIndex = 8;
            this.lbl0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl0.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbl0_MouseDown);
            this.lbl0.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbl0_MouseMove);
            this.lbl0.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbl0_MouseUp);
            // 
            // lblFasterRate
            // 
            this.lblFasterRate.BackColor = System.Drawing.Color.ForestGreen;
            this.lblFasterRate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFasterRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblFasterRate.Font = new System.Drawing.Font("Tw Cen MT Condensed", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFasterRate.ForeColor = System.Drawing.Color.Black;
            this.lblFasterRate.Location = new System.Drawing.Point(265, 3);
            this.lblFasterRate.Name = "lblFasterRate";
            this.lblFasterRate.Size = new System.Drawing.Size(46, 46);
            this.lblFasterRate.TabIndex = 7;
            this.lblFasterRate.Text = "x1.5";
            this.lblFasterRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFasterRate.Click += new System.EventHandler(this.lblFasterRate_Click);
            // 
            // lbl100
            // 
            this.lbl100.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lbl100.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl100.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbl100.Font = new System.Drawing.Font("Tw Cen MT Condensed", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl100.ForeColor = System.Drawing.Color.Black;
            this.lbl100.Location = new System.Drawing.Point(467, 3);
            this.lbl100.Name = "lbl100";
            this.lbl100.Size = new System.Drawing.Size(46, 46);
            this.lbl100.TabIndex = 10;
            this.lbl100.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl100.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbl100_MouseDown);
            this.lbl100.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbl100_MouseMove);
            this.lbl100.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbl100_MouseUp);
            // 
            // lblVol
            // 
            this.lblVol.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lblVol.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblVol.Font = new System.Drawing.Font("Tw Cen MT Condensed", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVol.ForeColor = System.Drawing.Color.Black;
            this.lblVol.Location = new System.Drawing.Point(369, 3);
            this.lblVol.Name = "lblVol";
            this.lblVol.Size = new System.Drawing.Size(92, 46);
            this.lblVol.TabIndex = 9;
            this.lblVol.Text = "Vol 100";
            this.lblVol.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblVol.Paint += new System.Windows.Forms.PaintEventHandler(this.lblVol_Paint);
            this.lblVol.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblVol_MouseDown);
            this.lblVol.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblVol_MouseMove);
            this.lblVol.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblVol_MouseUp);
            // 
            // pnlOpe
            // 
            this.pnlOpe.Controls.Add(this.btnPause);
            this.pnlOpe.Controls.Add(this.btnRewind);
            this.pnlOpe.Controls.Add(this.btnStop);
            this.pnlOpe.Controls.Add(this.lblVol);
            this.pnlOpe.Controls.Add(this.lbl100);
            this.pnlOpe.Controls.Add(this.lblFasterRate);
            this.pnlOpe.Controls.Add(this.btnPlay);
            this.pnlOpe.Controls.Add(this.lbl0);
            this.pnlOpe.Controls.Add(this.lblLoop);
            this.pnlOpe.Location = new System.Drawing.Point(3, 77);
            this.pnlOpe.Name = "pnlOpe";
            this.pnlOpe.Size = new System.Drawing.Size(521, 54);
            this.pnlOpe.TabIndex = 15;
            // 
            // btnPause
            // 
            this.btnPause.ActionMode = XControls.UI.PushButton.ActionModes.PushButton;
            this.btnPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPause.Font = new System.Drawing.Font("Webdings", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPause.GroupName = "";
            this.btnPause.IsPushed = false;
            this.btnPause.Location = new System.Drawing.Point(5, 3);
            this.btnPause.MaximumSize = new System.Drawing.Size(60, 60);
            this.btnPause.MinimumSize = new System.Drawing.Size(46, 46);
            this.btnPause.Name = "btnPause";
            this.btnPause.PushOffBackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnPause.PushOffBorderColor = System.Drawing.Color.Black;
            this.btnPause.PushOffForeColor = System.Drawing.SystemColors.ControlText;
            this.btnPause.PushOnBackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnPause.PushOnBorderColor = System.Drawing.Color.Black;
            this.btnPause.PushOnForeColor = System.Drawing.SystemColors.ControlText;
            this.btnPause.Size = new System.Drawing.Size(46, 46);
            this.btnPause.TabIndex = 2;
            this.btnPause.Text = ";";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnRewind
            // 
            this.btnRewind.ActionMode = XControls.UI.PushButton.ActionModes.Button;
            this.btnRewind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRewind.Font = new System.Drawing.Font("Webdings", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnRewind.GroupName = "";
            this.btnRewind.IsPushed = false;
            this.btnRewind.Location = new System.Drawing.Point(57, 3);
            this.btnRewind.Name = "btnRewind";
            this.btnRewind.PushOffBackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnRewind.PushOffBorderColor = System.Drawing.Color.Black;
            this.btnRewind.PushOffForeColor = System.Drawing.SystemColors.ControlText;
            this.btnRewind.PushOnBackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnRewind.PushOnBorderColor = System.Drawing.Color.Black;
            this.btnRewind.PushOnForeColor = System.Drawing.SystemColors.ControlText;
            this.btnRewind.Size = new System.Drawing.Size(46, 46);
            this.btnRewind.TabIndex = 3;
            this.btnRewind.Text = "9";
            this.btnRewind.UseVisualStyleBackColor = true;
            this.btnRewind.Click += new System.EventHandler(this.btnRewind_Click);
            // 
            // btnStop
            // 
            this.btnStop.ActionMode = XControls.UI.PushButton.ActionModes.Button;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Font = new System.Drawing.Font("Webdings", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnStop.GroupName = "";
            this.btnStop.IsPushed = false;
            this.btnStop.Location = new System.Drawing.Point(109, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.PushOffBackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnStop.PushOffBorderColor = System.Drawing.Color.Black;
            this.btnStop.PushOffForeColor = System.Drawing.SystemColors.ControlText;
            this.btnStop.PushOnBackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnStop.PushOnBorderColor = System.Drawing.Color.Black;
            this.btnStop.PushOnForeColor = System.Drawing.SystemColors.ControlText;
            this.btnStop.Size = new System.Drawing.Size(46, 46);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "<";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.ActionMode = XControls.UI.PushButton.ActionModes.PushOnButton;
            this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay.Font = new System.Drawing.Font("Webdings", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPlay.GroupName = "";
            this.btnPlay.IsPushed = false;
            this.btnPlay.Location = new System.Drawing.Point(161, 3);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.PushOffBackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnPlay.PushOffBorderColor = System.Drawing.Color.Black;
            this.btnPlay.PushOffForeColor = System.Drawing.SystemColors.ControlText;
            this.btnPlay.PushOnBackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnPlay.PushOnBorderColor = System.Drawing.Color.Black;
            this.btnPlay.PushOnForeColor = System.Drawing.SystemColors.ControlText;
            this.btnPlay.Size = new System.Drawing.Size(46, 46);
            this.btnPlay.TabIndex = 5;
            this.btnPlay.Text = "4";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // XSoundPlay
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.pnlView);
            this.Controls.Add(this.picTime);
            this.Controls.Add(this.pnlOpe);
            this.DoubleBuffered = true;
            this.Name = "XSoundPlay";
            this.Size = new System.Drawing.Size(531, 137);
            ((System.ComponentModel.ISupportInitialize)(this.picTime)).EndInit();
            this.pnlView.ResumeLayout(false);
            this.pnlOpe.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMusic;
        private System.Windows.Forms.Label lblPause;
        private System.Windows.Forms.PictureBox picTime;
        private System.Windows.Forms.Timer timer;
        private XControls.UI.ScrollLabel lblSoundName;
        private System.Windows.Forms.Panel pnlView;
        private System.Windows.Forms.Panel pnlOpe;
        private XControls.UI.PushButton btnPause;
        private XControls.UI.PushButton btnRewind;
        private XControls.UI.PushButton btnStop;
        private System.Windows.Forms.Label lblVol;
        private System.Windows.Forms.Label lbl100;
        private System.Windows.Forms.Label lblFasterRate;
        private XControls.UI.PushButton btnPlay;
        private System.Windows.Forms.Label lbl0;
        private System.Windows.Forms.Label lblLoop;

    }
}
