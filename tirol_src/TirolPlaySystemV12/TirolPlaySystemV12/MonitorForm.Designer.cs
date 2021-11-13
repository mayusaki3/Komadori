namespace TirolPlaySystem
{
    partial class MonitorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitorForm));
            this.pnlPlayer = new System.Windows.Forms.Panel();
            this.lblPlayer = new System.Windows.Forms.Label();
            this.lblRobot = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.xpicRobot = new XControls.XCamView();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlScore = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.field = new XControls.UI.FieldView();
            this.label3 = new System.Windows.Forms.Label();
            this.xtimCap = new XControls.UI.TimerView();
            this.lblGameInfo = new XControls.UI.ScrollLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.xtimer = new XControls.UI.TimerView();
            this.label1 = new System.Windows.Forms.Label();
            this.xtimHandi = new XControls.UI.TimerView();
            this.xweb = new XControls.XBrowser();
            this.picMaskR = new System.Windows.Forms.PictureBox();
            this.picMaskL = new System.Windows.Forms.PictureBox();
            this.pnlColorBar = new System.Windows.Forms.Panel();
            this.pnlPlayer.SuspendLayout();
            this.pnlScore.SuspendLayout();
            this.panel1.SuspendLayout();
            this.xweb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMaskR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMaskL)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlPlayer
            // 
            this.pnlPlayer.BackColor = System.Drawing.Color.LightSlateGray;
            this.pnlPlayer.Controls.Add(this.lblPlayer);
            this.pnlPlayer.Controls.Add(this.lblRobot);
            this.pnlPlayer.Controls.Add(this.label5);
            this.pnlPlayer.Controls.Add(this.xpicRobot);
            this.pnlPlayer.Controls.Add(this.label4);
            this.pnlPlayer.Location = new System.Drawing.Point(40, 188);
            this.pnlPlayer.Name = "pnlPlayer";
            this.pnlPlayer.Size = new System.Drawing.Size(1024, 600);
            this.pnlPlayer.TabIndex = 1;
            // 
            // lblPlayer
            // 
            this.lblPlayer.BackColor = System.Drawing.Color.AliceBlue;
            this.lblPlayer.Font = new System.Drawing.Font("HG丸ｺﾞｼｯｸM-PRO", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblPlayer.ForeColor = System.Drawing.Color.Black;
            this.lblPlayer.Location = new System.Drawing.Point(227, 65);
            this.lblPlayer.Name = "lblPlayer";
            this.lblPlayer.Size = new System.Drawing.Size(553, 27);
            this.lblPlayer.TabIndex = 13;
            this.lblPlayer.Text = "プレイヤー名";
            // 
            // lblRobot
            // 
            this.lblRobot.BackColor = System.Drawing.Color.AliceBlue;
            this.lblRobot.Font = new System.Drawing.Font("HG丸ｺﾞｼｯｸM-PRO", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblRobot.ForeColor = System.Drawing.Color.Black;
            this.lblRobot.Location = new System.Drawing.Point(227, 16);
            this.lblRobot.Name = "lblRobot";
            this.lblRobot.Size = new System.Drawing.Size(553, 27);
            this.lblRobot.TabIndex = 12;
            this.lblRobot.Text = "ロボット名";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("HG丸ｺﾞｼｯｸM-PRO", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.label5.Location = new System.Drawing.Point(18, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(180, 27);
            this.label5.TabIndex = 11;
            this.label5.Text = "プレイヤー名";
            // 
            // xpicRobot
            // 
            this.xpicRobot.BackColor = System.Drawing.Color.SteelBlue;
            this.xpicRobot.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.xpicRobot.Location = new System.Drawing.Point(200, 105);
            this.xpicRobot.Name = "xpicRobot";
            this.xpicRobot.Size = new System.Drawing.Size(640, 480);
            this.xpicRobot.TabIndex = 10;
            this.xpicRobot.Zoom = ((ushort)(100));
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("HG丸ｺﾞｼｯｸM-PRO", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.label4.Location = new System.Drawing.Point(18, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(152, 27);
            this.label4.TabIndex = 9;
            this.label4.Text = "ロボット名";
            // 
            // pnlScore
            // 
            this.pnlScore.Controls.Add(this.panel1);
            this.pnlScore.Controls.Add(this.xweb);
            this.pnlScore.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlScore.Location = new System.Drawing.Point(0, 0);
            this.pnlScore.Name = "pnlScore";
            this.pnlScore.Size = new System.Drawing.Size(1280, 768);
            this.pnlScore.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.panel1.Controls.Add(this.pnlPlayer);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.field);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.xtimCap);
            this.panel1.Controls.Add(this.lblGameInfo);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.xtimer);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.xtimHandi);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1280, 230);
            this.panel1.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("HG丸ｺﾞｼｯｸM-PRO", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.label6.Location = new System.Drawing.Point(165, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 27);
            this.label6.TabIndex = 10;
            this.label6.Text = "エリア";
            this.label6.Visible = false;
            // 
            // field
            // 
            this.field.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.field.ForeColor = System.Drawing.Color.White;
            this.field.GridCountX = 4;
            this.field.GridCountY = 4;
            this.field.GridOn = new string[] {
        "1000",
        "0000",
        "0100",
        "0010"};
            this.field.GridText = new string[] {
        "1,2,3,4",
        "5,6,7,8",
        "9,10,11,12",
        "13,14,15,16"};
            this.field.Location = new System.Drawing.Point(15, 81);
            this.field.Name = "field";
            this.field.OnBackColor = System.Drawing.Color.LightSeaGreen;
            this.field.OnForeColor = System.Drawing.SystemColors.ControlText;
            this.field.Size = new System.Drawing.Size(144, 144);
            this.field.TabIndex = 9;
            this.field.Visible = false;
            this.field.WallSetting = new string[] {
        "++++",
        "+++++",
        "++++",
        "+++++",
        "++++",
        "+++++",
        "++++",
        "+++++",
        "++++"};
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("HG丸ｺﾞｼｯｸM-PRO", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.label3.Location = new System.Drawing.Point(3, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 27);
            this.label3.TabIndex = 8;
            this.label3.Text = "残り時間";
            // 
            // xtimCap
            // 
            this.xtimCap.AdjustmentC = -0.5F;
            this.xtimCap.AdjustmentY = 0.9F;
            this.xtimCap.DefaultTime = 500D;
            this.xtimCap.DisplayOffColor = System.Drawing.Color.Black;
            this.xtimCap.DisplayOnColor = System.Drawing.Color.Lime;
            this.xtimCap.DisplayShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.xtimCap.Location = new System.Drawing.Point(905, 88);
            this.xtimCap.Name = "xtimCap";
            this.xtimCap.NumberSpace = ((uint)(5u));
            this.xtimCap.OneMinuteMode = true;
            this.xtimCap.ShadowOffsetX = ((uint)(3u));
            this.xtimCap.ShadowOffsetY = ((uint)(3u));
            this.xtimCap.Size = new System.Drawing.Size(195, 128);
            this.xtimCap.TabIndex = 5;
            // 
            // lblGameInfo
            // 
            this.lblGameInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblGameInfo.Font = new System.Drawing.Font("MS UI Gothic", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblGameInfo.ForeColor = System.Drawing.Color.Yellow;
            this.lblGameInfo.Location = new System.Drawing.Point(0, 0);
            this.lblGameInfo.MoveInterval = 20;
            this.lblGameInfo.Name = "lblGameInfo";
            this.lblGameInfo.PauseTime = 2000F;
            this.lblGameInfo.Size = new System.Drawing.Size(1280, 36);
            this.lblGameInfo.TabIndex = 4;
            this.lblGameInfo.Text = "*** GAME INFOMATION - GAME INFOMATION - GAME INFOMATION - GAME INFOMATION - GAME " +
    "INFOMATION ***";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("HG丸ｺﾞｼｯｸM-PRO", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.label2.Location = new System.Drawing.Point(900, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 27);
            this.label2.TabIndex = 7;
            this.label2.Text = "獲得数";
            // 
            // xtimer
            // 
            this.xtimer.AdjustmentC = -0.4F;
            this.xtimer.AdjustmentY = 0.8F;
            this.xtimer.DefaultTime = 500D;
            this.xtimer.DisplayOffColor = System.Drawing.Color.Black;
            this.xtimer.DisplayOnColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.xtimer.DisplayShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.xtimer.Location = new System.Drawing.Point(8, 42);
            this.xtimer.Name = "xtimer";
            this.xtimer.NumberSpace = ((uint)(5u));
            this.xtimer.ShadowOffsetX = ((uint)(3u));
            this.xtimer.ShadowOffsetY = ((uint)(3u));
            this.xtimer.Size = new System.Drawing.Size(881, 183);
            this.xtimer.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("HG丸ｺﾞｼｯｸM-PRO", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.label1.Location = new System.Drawing.Point(1114, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 27);
            this.label1.TabIndex = 7;
            this.label1.Text = "段数";
            // 
            // xtimHandi
            // 
            this.xtimHandi.AdjustmentC = -0.2F;
            this.xtimHandi.AdjustmentY = 0.9F;
            this.xtimHandi.DefaultTime = 500D;
            this.xtimHandi.DisplayOffColor = System.Drawing.Color.Black;
            this.xtimHandi.DisplayOnColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.xtimHandi.DisplayShadowColor = System.Drawing.Color.Red;
            this.xtimHandi.Location = new System.Drawing.Point(1119, 88);
            this.xtimHandi.Name = "xtimHandi";
            this.xtimHandi.NumberSpace = ((uint)(5u));
            this.xtimHandi.OneMinuteMode = true;
            this.xtimHandi.ShadowOffsetX = ((uint)(3u));
            this.xtimHandi.ShadowOffsetY = ((uint)(3u));
            this.xtimHandi.Size = new System.Drawing.Size(128, 87);
            this.xtimHandi.TabIndex = 5;
            // 
            // xweb
            // 
            this.xweb.BlankColor = System.Drawing.Color.Black;
            this.xweb.Controls.Add(this.picMaskR);
            this.xweb.Controls.Add(this.picMaskL);
            this.xweb.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.xweb.Location = new System.Drawing.Point(0, 231);
            this.xweb.Name = "xweb";
            this.xweb.ScriptErrorsSuppressed = true;
            this.xweb.Size = new System.Drawing.Size(1280, 537);
            this.xweb.TabIndex = 8;
            this.xweb.Url = new System.Uri("about:blank", System.UriKind.Absolute);
            // 
            // picMaskR
            // 
            this.picMaskR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.picMaskR.Dock = System.Windows.Forms.DockStyle.Right;
            this.picMaskR.Location = new System.Drawing.Point(1215, 0);
            this.picMaskR.Name = "picMaskR";
            this.picMaskR.Size = new System.Drawing.Size(65, 537);
            this.picMaskR.TabIndex = 10;
            this.picMaskR.TabStop = false;
            // 
            // picMaskL
            // 
            this.picMaskL.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.picMaskL.Dock = System.Windows.Forms.DockStyle.Left;
            this.picMaskL.Location = new System.Drawing.Point(0, 0);
            this.picMaskL.Name = "picMaskL";
            this.picMaskL.Size = new System.Drawing.Size(71, 537);
            this.picMaskL.TabIndex = 9;
            this.picMaskL.TabStop = false;
            // 
            // pnlColorBar
            // 
            this.pnlColorBar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlColorBar.BackgroundImage")));
            this.pnlColorBar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlColorBar.Location = new System.Drawing.Point(12, 12);
            this.pnlColorBar.Name = "pnlColorBar";
            this.pnlColorBar.Size = new System.Drawing.Size(347, 248);
            this.pnlColorBar.TabIndex = 5;
            // 
            // MonitorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Blue;
            this.ClientSize = new System.Drawing.Size(1280, 768);
            this.Controls.Add(this.pnlScore);
            this.Controls.Add(this.pnlColorBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MonitorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MonitorForm";
            this.pnlPlayer.ResumeLayout(false);
            this.pnlPlayer.PerformLayout();
            this.pnlScore.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.xweb.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picMaskR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMaskL)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlPlayer;
        private System.Windows.Forms.Panel pnlScore;
        private System.Windows.Forms.Panel pnlColorBar;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private XControls.UI.TimerView xtimCap;
        private XControls.UI.ScrollLabel lblGameInfo;
        private System.Windows.Forms.Label label2;
        private XControls.UI.TimerView xtimer;
        private System.Windows.Forms.Label label1;
        private XControls.UI.TimerView xtimHandi;
        private System.Windows.Forms.Label lblPlayer;
        private System.Windows.Forms.Label lblRobot;
        private System.Windows.Forms.Label label5;
        private XControls.XCamView xpicRobot;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox picMaskR;
        private System.Windows.Forms.PictureBox picMaskL;
        private System.Windows.Forms.Label label6;
        private XControls.UI.FieldView field;
        private XControls.XBrowser xweb;
    }
}