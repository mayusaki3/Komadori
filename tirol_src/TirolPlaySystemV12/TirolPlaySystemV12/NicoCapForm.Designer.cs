namespace TirolPlaySystem
{
    partial class NicoCapForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NicoCapForm));
            this.label1 = new System.Windows.Forms.Label();
            this.pnlTime = new System.Windows.Forms.Panel();
            this.lblPlayer = new XControls.UI.ScrollLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.xtime = new XControls.UI.TimerView();
            this.pnlColorBar = new System.Windows.Forms.Panel();
            this.field = new XControls.UI.FieldView();
            this.lblMsgB = new System.Windows.Forms.Label();
            this.lblMsgT = new System.Windows.Forms.Label();
            this.xscrview = new XControls.UI.XScreenView();
            this.label3 = new System.Windows.Forms.Label();
            this.xcount = new XControls.UI.TimerView();
            this.pnlTime.SuspendLayout();
            this.pnlColorBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("DAVID", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.ForeColor = System.Drawing.Color.Yellow;
            this.label1.Location = new System.Drawing.Point(279, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "TIME:";
            // 
            // pnlTime
            // 
            this.pnlTime.BackColor = System.Drawing.Color.Black;
            this.pnlTime.Controls.Add(this.xcount);
            this.pnlTime.Controls.Add(this.label3);
            this.pnlTime.Controls.Add(this.lblPlayer);
            this.pnlTime.Controls.Add(this.label2);
            this.pnlTime.Controls.Add(this.label1);
            this.pnlTime.Controls.Add(this.xtime);
            this.pnlTime.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTime.Location = new System.Drawing.Point(0, 248);
            this.pnlTime.Name = "pnlTime";
            this.pnlTime.Size = new System.Drawing.Size(512, 40);
            this.pnlTime.TabIndex = 2;
            // 
            // lblPlayer
            // 
            this.lblPlayer.Font = new System.Drawing.Font("MS UI Gothic", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblPlayer.ForeColor = System.Drawing.Color.Yellow;
            this.lblPlayer.Location = new System.Drawing.Point(64, 2);
            this.lblPlayer.MoveSpeed = 16F;
            this.lblPlayer.Name = "lblPlayer";
            this.lblPlayer.PauseTime = 2000F;
            this.lblPlayer.Size = new System.Drawing.Size(206, 38);
            this.lblPlayer.TabIndex = 3;
            this.lblPlayer.Text = "ROBOT-NAME (USER-NAME)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("DAVID", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.ForeColor = System.Drawing.Color.Yellow;
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 14);
            this.label2.TabIndex = 2;
            this.label2.Text = "PLAYER:";
            // 
            // xtime
            // 
            this.xtime.AdjustmentY = 0.8F;
            this.xtime.DefaultTime = 500D;
            this.xtime.DisplayOffColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(47)))), ((int)(((byte)(46)))));
            this.xtime.DisplayOnColor = System.Drawing.Color.Yellow;
            this.xtime.DisplayShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(125)))), ((int)(((byte)(136)))));
            this.xtime.Location = new System.Drawing.Point(296, 7);
            this.xtime.Name = "xtime";
            this.xtime.NumberSpace = ((uint)(5u));
            this.xtime.ShadowOffsetX = ((uint)(1u));
            this.xtime.ShadowOffsetY = ((uint)(1u));
            this.xtime.Size = new System.Drawing.Size(135, 31);
            this.xtime.TabIndex = 1;
            // 
            // pnlColorBar
            // 
            this.pnlColorBar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlColorBar.BackgroundImage")));
            this.pnlColorBar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlColorBar.Controls.Add(this.field);
            this.pnlColorBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlColorBar.Location = new System.Drawing.Point(0, 0);
            this.pnlColorBar.Name = "pnlColorBar";
            this.pnlColorBar.Size = new System.Drawing.Size(512, 288);
            this.pnlColorBar.TabIndex = 4;
            // 
            // field
            // 
            this.field.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.field.Location = new System.Drawing.Point(426, 6);
            this.field.Name = "field";
            this.field.OnBackColor = System.Drawing.Color.LightSeaGreen;
            this.field.OnForeColor = System.Drawing.SystemColors.ControlText;
            this.field.Size = new System.Drawing.Size(80, 80);
            this.field.TabIndex = 11;
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
            // lblMsgB
            // 
            this.lblMsgB.AutoSize = true;
            this.lblMsgB.BackColor = System.Drawing.Color.MidnightBlue;
            this.lblMsgB.Font = new System.Drawing.Font("小塚ゴシック Pro R", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblMsgB.ForeColor = System.Drawing.Color.White;
            this.lblMsgB.Location = new System.Drawing.Point(5, 218);
            this.lblMsgB.Name = "lblMsgB";
            this.lblMsgB.Size = new System.Drawing.Size(93, 26);
            this.lblMsgB.TabIndex = 8;
            this.lblMsgB.Text = "MESSAGE";
            // 
            // lblMsgT
            // 
            this.lblMsgT.AutoSize = true;
            this.lblMsgT.BackColor = System.Drawing.Color.MidnightBlue;
            this.lblMsgT.Font = new System.Drawing.Font("小塚ゴシック Pro R", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblMsgT.ForeColor = System.Drawing.Color.White;
            this.lblMsgT.Location = new System.Drawing.Point(5, 6);
            this.lblMsgT.Name = "lblMsgT";
            this.lblMsgT.Size = new System.Drawing.Size(93, 26);
            this.lblMsgT.TabIndex = 7;
            this.lblMsgT.Text = "MESSAGE";
            // 
            // xscrview
            // 
            this.xscrview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xscrview.Enabled = true;
            this.xscrview.Location = new System.Drawing.Point(0, 0);
            this.xscrview.MouseZoom = true;
            this.xscrview.Name = "xscrview";
            this.xscrview.ScreenIndex = 0;
            this.xscrview.Size = new System.Drawing.Size(512, 288);
            this.xscrview.TabIndex = 3;
            this.xscrview.Visible = false;
            this.xscrview.Zoom = ((ushort)(100));
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("DAVID", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.ForeColor = System.Drawing.Color.Lime;
            this.label3.Location = new System.Drawing.Point(437, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 14);
            this.label3.TabIndex = 4;
            this.label3.Text = "GET:";
            // 
            // xcount
            // 
            this.xcount.AdjustmentY = 0.8F;
            this.xcount.DefaultTime = 500D;
            this.xcount.DisplayOffColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(47)))), ((int)(((byte)(46)))));
            this.xcount.DisplayOnColor = System.Drawing.Color.Lime;
            this.xcount.DisplayShadowColor = System.Drawing.Color.Green;
            this.xcount.Location = new System.Drawing.Point(472, 7);
            this.xcount.Name = "xcount";
            this.xcount.NumberSpace = ((uint)(5u));
            this.xcount.OneMinuteMode = true;
            this.xcount.ShadowOffsetX = ((uint)(1u));
            this.xcount.ShadowOffsetY = ((uint)(1u));
            this.xcount.Size = new System.Drawing.Size(34, 31);
            this.xcount.TabIndex = 5;
            // 
            // NicoCapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Blue;
            this.ClientSize = new System.Drawing.Size(512, 288);
            this.Controls.Add(this.lblMsgB);
            this.Controls.Add(this.lblMsgT);
            this.Controls.Add(this.pnlTime);
            this.Controls.Add(this.pnlColorBar);
            this.Controls.Add(this.xscrview);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NicoCapForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "NicoCapForm";
            this.TopMost = true;
            this.pnlTime.ResumeLayout(false);
            this.pnlTime.PerformLayout();
            this.pnlColorBar.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private XControls.UI.TimerView xtime;
        private System.Windows.Forms.Panel pnlTime;
        private System.Windows.Forms.Label label2;
        private XControls.UI.ScrollLabel lblPlayer;
        private XControls.UI.XScreenView xscrview;
        private System.Windows.Forms.Panel pnlColorBar;
        private System.Windows.Forms.Label lblMsgB;
        private System.Windows.Forms.Label lblMsgT;
        private XControls.UI.FieldView field;
        private XControls.UI.TimerView xcount;
        private System.Windows.Forms.Label label3;
    }
}