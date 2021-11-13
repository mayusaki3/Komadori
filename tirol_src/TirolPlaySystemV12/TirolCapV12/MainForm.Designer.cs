namespace TirolPlaySystem
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
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
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            XControls.UI.ConfigGrid.ItemSetting itemSetting1 = new XControls.UI.ConfigGrid.ItemSetting();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabSetting = new System.Windows.Forms.TabPage();
            this.txtLog = new XControls.UI.Console();
            this.xconf = new XControls.UI.ConfigGrid();
            this.tabGame = new System.Windows.Forms.TabPage();
            this.btnCamClear = new XControls.UI.PushButton();
            this.btnCamCapture = new XControls.UI.PushButton();
            this.cmbPlayer = new System.Windows.Forms.ComboBox();
            this.btnCamOn = new XControls.UI.PushButton();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbCam = new System.Windows.Forms.ComboBox();
            this.xcam = new XControls.XCamView();
            this.btnCamSave = new XControls.UI.PushButton();
            this.btnCamLoad = new XControls.UI.PushButton();
            this.label12 = new System.Windows.Forms.Label();
            this.Tab = new XControls.UI.Tab();
            this.tabSetting.SuspendLayout();
            this.tabGame.SuspendLayout();
            this.Tab.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabSetting
            // 
            this.tabSetting.BackColor = System.Drawing.Color.DarkSlateGray;
            this.tabSetting.Controls.Add(this.txtLog);
            this.tabSetting.Controls.Add(this.xconf);
            this.tabSetting.Location = new System.Drawing.Point(4, 64);
            this.tabSetting.Name = "tabSetting";
            this.tabSetting.Padding = new System.Windows.Forms.Padding(3);
            this.tabSetting.Size = new System.Drawing.Size(822, 417);
            this.tabSetting.TabIndex = 1;
            this.tabSetting.Text = "SETTINGS";
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.txtLog.CaretColumn = 3;
            this.txtLog.CaretLine = 0;
            this.txtLog.Font = new System.Drawing.Font("MS Gothic", 13F, System.Drawing.FontStyle.Bold);
            this.txtLog.ForeColor = System.Drawing.Color.Ivory;
            this.txtLog.Lines = new string[] {
        "XXX"};
            this.txtLog.Location = new System.Drawing.Point(6, 167);
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(810, 244);
            this.txtLog.TabIndex = 68;
            this.txtLog.Text = "XXX";
            // 
            // xconf
            // 
            this.xconf.Directory = ".\\";
            itemSetting1.DefaultValue = "DATA\\";
            itemSetting1.Description = "データのルートフォルダの場所を設定します。";
            itemSetting1.DisplayCategory = "";
            itemSetting1.DisplayName = "データフォルダ";
            itemSetting1.Name = "DataFolder";
            itemSetting1.ValueType = "Folder";
            this.xconf.ItemSettings.Add(itemSetting1);
            this.xconf.Location = new System.Drawing.Point(6, 6);
            this.xconf.Name = "xconf";
            this.xconf.Size = new System.Drawing.Size(810, 155);
            this.xconf.TabIndex = 4;
            // 
            // tabGame
            // 
            this.tabGame.BackColor = System.Drawing.Color.DarkSlateGray;
            this.tabGame.Controls.Add(this.btnCamClear);
            this.tabGame.Controls.Add(this.btnCamCapture);
            this.tabGame.Controls.Add(this.cmbPlayer);
            this.tabGame.Controls.Add(this.btnCamOn);
            this.tabGame.Controls.Add(this.label6);
            this.tabGame.Controls.Add(this.cmbCam);
            this.tabGame.Controls.Add(this.xcam);
            this.tabGame.Controls.Add(this.btnCamSave);
            this.tabGame.Controls.Add(this.btnCamLoad);
            this.tabGame.Controls.Add(this.label12);
            this.tabGame.Location = new System.Drawing.Point(4, 64);
            this.tabGame.Name = "tabGame";
            this.tabGame.Size = new System.Drawing.Size(822, 417);
            this.tabGame.TabIndex = 2;
            this.tabGame.Text = "CAPTURE";
            // 
            // btnCamClear
            // 
            this.btnCamClear.ActionMode = XControls.UI.PushButton.ActionModes.Button;
            this.btnCamClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCamClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.btnCamClear.GroupName = "";
            this.btnCamClear.IsPushed = false;
            this.btnCamClear.Location = new System.Drawing.Point(720, 360);
            this.btnCamClear.Name = "btnCamClear";
            this.btnCamClear.PushOffBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnCamClear.PushOffBorderColor = System.Drawing.Color.Red;
            this.btnCamClear.PushOnBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnCamClear.PushOnBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnCamClear.Size = new System.Drawing.Size(95, 49);
            this.btnCamClear.TabIndex = 80;
            this.btnCamClear.Text = "CLEAR";
            this.btnCamClear.UseVisualStyleBackColor = false;
            this.btnCamClear.Click += new System.EventHandler(this.btnCamClear_Click);
            // 
            // btnCamCapture
            // 
            this.btnCamCapture.ActionMode = XControls.UI.PushButton.ActionModes.Button;
            this.btnCamCapture.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCamCapture.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.btnCamCapture.GroupName = "";
            this.btnCamCapture.IsPushed = false;
            this.btnCamCapture.Location = new System.Drawing.Point(720, 77);
            this.btnCamCapture.Name = "btnCamCapture";
            this.btnCamCapture.PushOffBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnCamCapture.PushOffBorderColor = System.Drawing.Color.Yellow;
            this.btnCamCapture.PushOnBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnCamCapture.PushOnBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnCamCapture.Size = new System.Drawing.Size(95, 49);
            this.btnCamCapture.TabIndex = 64;
            this.btnCamCapture.Text = "CAPTURE";
            this.btnCamCapture.UseVisualStyleBackColor = false;
            this.btnCamCapture.Click += new System.EventHandler(this.btnCamCapture_Click);
            // 
            // cmbPlayer
            // 
            this.cmbPlayer.BackColor = System.Drawing.Color.CadetBlue;
            this.cmbPlayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPlayer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbPlayer.Font = new System.Drawing.Font("MS UI Gothic", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbPlayer.FormattingEnabled = true;
            this.cmbPlayer.Location = new System.Drawing.Point(6, 20);
            this.cmbPlayer.Name = "cmbPlayer";
            this.cmbPlayer.Size = new System.Drawing.Size(439, 51);
            this.cmbPlayer.TabIndex = 46;
            // 
            // btnCamOn
            // 
            this.btnCamOn.ActionMode = XControls.UI.PushButton.ActionModes.PushButton;
            this.btnCamOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCamOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.btnCamOn.GroupName = "";
            this.btnCamOn.IsPushed = false;
            this.btnCamOn.Location = new System.Drawing.Point(451, 80);
            this.btnCamOn.Name = "btnCamOn";
            this.btnCamOn.PushOffBackColor = System.Drawing.SystemColors.ControlDark;
            this.btnCamOn.PushOffBorderColor = System.Drawing.Color.DimGray;
            this.btnCamOn.PushOnBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnCamOn.PushOnBorderColor = System.Drawing.Color.Lime;
            this.btnCamOn.Size = new System.Drawing.Size(95, 49);
            this.btnCamOn.TabIndex = 67;
            this.btnCamOn.Text = "CAMERA ON";
            this.btnCamOn.UseVisualStyleBackColor = false;
            this.btnCamOn.Click += new System.EventHandler(this.btnCamOn_Click);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Green;
            this.label6.Font = new System.Drawing.Font("Impact", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.ForeColor = System.Drawing.Color.Lime;
            this.label6.Location = new System.Drawing.Point(4, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(441, 12);
            this.label6.TabIndex = 32;
            this.label6.Text = "ROBOT SELECTOR";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmbCam
            // 
            this.cmbCam.BackColor = System.Drawing.Color.CadetBlue;
            this.cmbCam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCam.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbCam.Font = new System.Drawing.Font("MS UI Gothic", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbCam.FormattingEnabled = true;
            this.cmbCam.Location = new System.Drawing.Point(451, 20);
            this.cmbCam.Name = "cmbCam";
            this.cmbCam.Size = new System.Drawing.Size(364, 51);
            this.cmbCam.TabIndex = 62;
            // 
            // xcam
            // 
            this.xcam.BackColor = System.Drawing.Color.Teal;
            this.xcam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.xcam.Location = new System.Drawing.Point(6, 80);
            this.xcam.Name = "xcam";
            this.xcam.Size = new System.Drawing.Size(439, 329);
            this.xcam.TabIndex = 49;
            this.xcam.Zoom = ((ushort)(100));
            // 
            // btnCamSave
            // 
            this.btnCamSave.ActionMode = XControls.UI.PushButton.ActionModes.Button;
            this.btnCamSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCamSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.btnCamSave.GroupName = "";
            this.btnCamSave.IsPushed = false;
            this.btnCamSave.Location = new System.Drawing.Point(720, 227);
            this.btnCamSave.Name = "btnCamSave";
            this.btnCamSave.PushOffBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnCamSave.PushOffBorderColor = System.Drawing.Color.Blue;
            this.btnCamSave.PushOnBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnCamSave.PushOnBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnCamSave.Size = new System.Drawing.Size(95, 49);
            this.btnCamSave.TabIndex = 63;
            this.btnCamSave.Text = "SAVE";
            this.btnCamSave.UseVisualStyleBackColor = false;
            this.btnCamSave.Click += new System.EventHandler(this.btnCamSave_Click);
            // 
            // btnCamLoad
            // 
            this.btnCamLoad.ActionMode = XControls.UI.PushButton.ActionModes.Button;
            this.btnCamLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCamLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.btnCamLoad.GroupName = "";
            this.btnCamLoad.IsPushed = false;
            this.btnCamLoad.Location = new System.Drawing.Point(451, 227);
            this.btnCamLoad.Name = "btnCamLoad";
            this.btnCamLoad.PushOffBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnCamLoad.PushOffBorderColor = System.Drawing.Color.Blue;
            this.btnCamLoad.PushOnBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnCamLoad.PushOnBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnCamLoad.Size = new System.Drawing.Size(95, 49);
            this.btnCamLoad.TabIndex = 64;
            this.btnCamLoad.Text = "LOAD";
            this.btnCamLoad.UseVisualStyleBackColor = false;
            this.btnCamLoad.Click += new System.EventHandler(this.btnCamLoad_Click);
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.Green;
            this.label12.Font = new System.Drawing.Font("Impact", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label12.ForeColor = System.Drawing.Color.Lime;
            this.label12.Location = new System.Drawing.Point(449, 5);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(366, 12);
            this.label12.TabIndex = 61;
            this.label12.Text = "ROBOT IMAGE CAPTURE";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Tab
            // 
            this.Tab.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.Tab.BackColor = System.Drawing.Color.Transparent;
            this.Tab.Controls.Add(this.tabGame);
            this.Tab.Controls.Add(this.tabSetting);
            this.Tab.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.Tab.ItemSize = new System.Drawing.Size(100, 60);
            this.Tab.Location = new System.Drawing.Point(10, 12);
            this.Tab.Name = "Tab";
            this.Tab.PushOffBackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Tab.PushOffBorderColor = System.Drawing.Color.Black;
            this.Tab.PushOffForeColor = System.Drawing.SystemColors.ControlText;
            this.Tab.PushOnBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.Tab.PushOnBorderColor = System.Drawing.Color.Black;
            this.Tab.PushOnForeColor = System.Drawing.SystemColors.ControlText;
            this.Tab.SelectedIndex = 0;
            this.Tab.Size = new System.Drawing.Size(830, 485);
            this.Tab.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.Tab.TabIndex = 0;
            this.Tab.SelectedIndexChanged += new System.EventHandler(this.Tab_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(847, 505);
            this.Controls.Add(this.Tab);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TIROL ROBOT IMAGE CAPTURE";
            this.tabSetting.ResumeLayout(false);
            this.tabGame.ResumeLayout(false);
            this.Tab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabPage tabSetting;
        private XControls.UI.Console txtLog;
        private XControls.UI.ConfigGrid xconf;
        private System.Windows.Forms.TabPage tabGame;
        private XControls.UI.PushButton btnCamClear;
        private XControls.UI.PushButton btnCamCapture;
        private System.Windows.Forms.ComboBox cmbPlayer;
        private XControls.UI.PushButton btnCamOn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbCam;
        private XControls.XCamView xcam;
        private XControls.UI.PushButton btnCamSave;
        private XControls.UI.PushButton btnCamLoad;
        private System.Windows.Forms.Label label12;
        private XControls.UI.Tab Tab;
    }
}

