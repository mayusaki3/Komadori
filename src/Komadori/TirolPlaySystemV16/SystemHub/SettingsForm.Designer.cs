namespace XControls
{
    partial class SettingsForm
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
            // 後処理
            Terminate();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tabBase = new XControls.UI.Tab();
            this.tabInfo = new System.Windows.Forms.TabPage();
            this.infoBox = new XControls.UI.Console();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.logBox = new XControls.UI.Console();
            this.tabPref = new System.Windows.Forms.TabPage();
            this.conf = new XControls.UI.ConfigGrid();
            this.btnSave = new XControls.UI.PushButton();
            this.btnLoad = new XControls.UI.PushButton();
            this.tabDiag = new System.Windows.Forms.TabPage();
            this.tabDiagBase = new XControls.UI.Tab();
            this.tabUDP = new System.Windows.Forms.TabPage();
            this.console1 = new XControls.UI.Console();
            this.diagBtnList = new XControls.UI.PushButton();
            this.diagLstIP = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.diagPnlSR = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.contextMenu.SuspendLayout();
            this.tabBase.SuspendLayout();
            this.tabInfo.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.tabPref.SuspendLayout();
            this.tabDiag.SuspendLayout();
            this.tabDiagBase.SuspendLayout();
            this.tabUDP.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "SystemHub";
            this.notifyIcon.Visible = true;
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuSettings,
            this.MenuSeparator1,
            this.MenuExit});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(153, 54);
            // 
            // MenuSettings
            // 
            this.MenuSettings.Name = "MenuSettings";
            this.MenuSettings.Size = new System.Drawing.Size(152, 22);
            this.MenuSettings.Text = "動作設定(&S)...";
            this.MenuSettings.Click += new System.EventHandler(this.MenuSettings_Click);
            // 
            // MenuSeparator1
            // 
            this.MenuSeparator1.Name = "MenuSeparator1";
            this.MenuSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // MenuExit
            // 
            this.MenuExit.Name = "MenuExit";
            this.MenuExit.Size = new System.Drawing.Size(152, 22);
            this.MenuExit.Text = "終了(&X)";
            this.MenuExit.Click += new System.EventHandler(this.MenuExit_Click);
            // 
            // tabBase
            // 
            this.tabBase.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabBase.BackColor = System.Drawing.Color.Transparent;
            this.tabBase.Controls.Add(this.tabInfo);
            this.tabBase.Controls.Add(this.tabLog);
            this.tabBase.Controls.Add(this.tabPref);
            this.tabBase.Controls.Add(this.tabDiag);
            this.tabBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabBase.ItemSize = new System.Drawing.Size(80, 24);
            this.tabBase.Location = new System.Drawing.Point(0, 0);
            this.tabBase.Name = "tabBase";
            this.tabBase.PushOffBackColor = System.Drawing.Color.LightSlateGray;
            this.tabBase.PushOffBorderColor = System.Drawing.Color.Black;
            this.tabBase.PushOffForeColor = System.Drawing.Color.Black;
            this.tabBase.PushOnBackColor = System.Drawing.Color.PaleTurquoise;
            this.tabBase.PushOnBorderColor = System.Drawing.Color.DeepSkyBlue;
            this.tabBase.PushOnForeColor = System.Drawing.Color.Black;
            this.tabBase.SelectedIndex = 0;
            this.tabBase.Size = new System.Drawing.Size(622, 283);
            this.tabBase.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabBase.TabIndex = 0;
            // 
            // tabInfo
            // 
            this.tabInfo.Controls.Add(this.infoBox);
            this.tabInfo.Location = new System.Drawing.Point(4, 28);
            this.tabInfo.Name = "tabInfo";
            this.tabInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tabInfo.Size = new System.Drawing.Size(614, 251);
            this.tabInfo.TabIndex = 0;
            this.tabInfo.Text = "情報";
            this.tabInfo.UseVisualStyleBackColor = true;
            // 
            // infoBox
            // 
            this.infoBox.BackColor = System.Drawing.SystemColors.Window;
            this.infoBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.infoBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoBox.ForeColor = System.Drawing.Color.Black;
            this.infoBox.Location = new System.Drawing.Point(3, 3);
            this.infoBox.Name = "infoBox";
            this.infoBox.Size = new System.Drawing.Size(608, 245);
            this.infoBox.TabIndex = 0;
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.logBox);
            this.tabLog.Location = new System.Drawing.Point(4, 28);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(614, 251);
            this.tabLog.TabIndex = 1;
            this.tabLog.Text = "ログ";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // logBox
            // 
            this.logBox.BackColor = System.Drawing.SystemColors.Window;
            this.logBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.logBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logBox.ForeColor = System.Drawing.Color.Black;
            this.logBox.Location = new System.Drawing.Point(3, 3);
            this.logBox.Name = "logBox";
            this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logBox.Size = new System.Drawing.Size(608, 245);
            this.logBox.TabIndex = 0;
            // 
            // tabPref
            // 
            this.tabPref.BackColor = System.Drawing.SystemColors.Control;
            this.tabPref.Controls.Add(this.conf);
            this.tabPref.Controls.Add(this.btnSave);
            this.tabPref.Controls.Add(this.btnLoad);
            this.tabPref.ForeColor = System.Drawing.Color.Black;
            this.tabPref.Location = new System.Drawing.Point(4, 28);
            this.tabPref.Name = "tabPref";
            this.tabPref.Size = new System.Drawing.Size(614, 251);
            this.tabPref.TabIndex = 2;
            this.tabPref.Text = "設定";
            // 
            // conf
            // 
            this.conf.Dock = System.Windows.Forms.DockStyle.Left;
            this.conf.Location = new System.Drawing.Point(0, 0);
            this.conf.Name = "conf";
            this.conf.Size = new System.Drawing.Size(520, 251);
            this.conf.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.ActionMode = XControls.UI.PushButton.ActionModes.Button;
            this.btnSave.GroupName = "";
            this.btnSave.IsPushed = false;
            this.btnSave.Location = new System.Drawing.Point(526, 219);
            this.btnSave.Name = "btnSave";
            this.btnSave.PushOffBackColor = System.Drawing.Color.Pink;
            this.btnSave.PushOffBorderColor = System.Drawing.Color.Black;
            this.btnSave.PushOffForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSave.PushOnBackColor = System.Drawing.Color.Pink;
            this.btnSave.PushOnBorderColor = System.Drawing.Color.Black;
            this.btnSave.PushOnForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSave.Size = new System.Drawing.Size(80, 24);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "保存";
            // 
            // btnLoad
            // 
            this.btnLoad.ActionMode = XControls.UI.PushButton.ActionModes.Button;
            this.btnLoad.GroupName = "";
            this.btnLoad.IsPushed = false;
            this.btnLoad.Location = new System.Drawing.Point(526, 8);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.PushOffBackColor = System.Drawing.Color.Yellow;
            this.btnLoad.PushOffBorderColor = System.Drawing.Color.Black;
            this.btnLoad.PushOffForeColor = System.Drawing.SystemColors.ControlText;
            this.btnLoad.PushOnBackColor = System.Drawing.Color.Yellow;
            this.btnLoad.PushOnBorderColor = System.Drawing.Color.Black;
            this.btnLoad.PushOnForeColor = System.Drawing.SystemColors.ControlText;
            this.btnLoad.Size = new System.Drawing.Size(80, 24);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "読込";
            // 
            // tabDiag
            // 
            this.tabDiag.Controls.Add(this.tabDiagBase);
            this.tabDiag.Location = new System.Drawing.Point(4, 28);
            this.tabDiag.Name = "tabDiag";
            this.tabDiag.Padding = new System.Windows.Forms.Padding(3);
            this.tabDiag.Size = new System.Drawing.Size(614, 251);
            this.tabDiag.TabIndex = 3;
            this.tabDiag.Text = "診断";
            this.tabDiag.UseVisualStyleBackColor = true;
            // 
            // tabDiagBase
            // 
            this.tabDiagBase.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabDiagBase.BackColor = System.Drawing.SystemColors.Control;
            this.tabDiagBase.Controls.Add(this.tabUDP);
            this.tabDiagBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabDiagBase.ItemSize = new System.Drawing.Size(80, 24);
            this.tabDiagBase.Location = new System.Drawing.Point(3, 3);
            this.tabDiagBase.Name = "tabDiagBase";
            this.tabDiagBase.PushOffBackColor = System.Drawing.Color.LightSlateGray;
            this.tabDiagBase.PushOffBorderColor = System.Drawing.Color.Black;
            this.tabDiagBase.PushOffForeColor = System.Drawing.Color.Black;
            this.tabDiagBase.PushOnBackColor = System.Drawing.Color.Gold;
            this.tabDiagBase.PushOnBorderColor = System.Drawing.Color.DarkKhaki;
            this.tabDiagBase.PushOnForeColor = System.Drawing.Color.Black;
            this.tabDiagBase.SelectedIndex = 0;
            this.tabDiagBase.Size = new System.Drawing.Size(608, 245);
            this.tabDiagBase.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabDiagBase.TabIndex = 1;
            // 
            // tabUDP
            // 
            this.tabUDP.Controls.Add(this.label2);
            this.tabUDP.Controls.Add(this.diagPnlSR);
            this.tabUDP.Controls.Add(this.label1);
            this.tabUDP.Controls.Add(this.diagLstIP);
            this.tabUDP.Controls.Add(this.diagBtnList);
            this.tabUDP.Controls.Add(this.console1);
            this.tabUDP.Location = new System.Drawing.Point(4, 28);
            this.tabUDP.Name = "tabUDP";
            this.tabUDP.Padding = new System.Windows.Forms.Padding(3);
            this.tabUDP.Size = new System.Drawing.Size(600, 213);
            this.tabUDP.TabIndex = 0;
            this.tabUDP.Text = "通信テスト";
            this.tabUDP.UseVisualStyleBackColor = true;
            // 
            // console1
            // 
            this.console1.BackColor = System.Drawing.SystemColors.Control;
            this.console1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.console1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.console1.ForeColor = System.Drawing.Color.Black;
            this.console1.Location = new System.Drawing.Point(3, 3);
            this.console1.Name = "console1";
            this.console1.Size = new System.Drawing.Size(594, 207);
            this.console1.TabIndex = 0;
            // 
            // diagBtnList
            // 
            this.diagBtnList.ActionMode = XControls.UI.PushButton.ActionModes.Button;
            this.diagBtnList.GroupName = "";
            this.diagBtnList.IsPushed = false;
            this.diagBtnList.Location = new System.Drawing.Point(6, 25);
            this.diagBtnList.Name = "diagBtnList";
            this.diagBtnList.PushOffBackColor = System.Drawing.Color.Yellow;
            this.diagBtnList.PushOffBorderColor = System.Drawing.Color.Black;
            this.diagBtnList.PushOffForeColor = System.Drawing.SystemColors.ControlText;
            this.diagBtnList.PushOnBackColor = System.Drawing.Color.Yellow;
            this.diagBtnList.PushOnBorderColor = System.Drawing.Color.Black;
            this.diagBtnList.PushOnForeColor = System.Drawing.SystemColors.ControlText;
            this.diagBtnList.Size = new System.Drawing.Size(80, 24);
            this.diagBtnList.TabIndex = 1;
            this.diagBtnList.Text = "リスト更新";
            // 
            // diagLstIP
            // 
            this.diagLstIP.Font = new System.Drawing.Font("MS UI Gothic", 14F);
            this.diagLstIP.FormattingEnabled = true;
            this.diagLstIP.ItemHeight = 19;
            this.diagLstIP.Location = new System.Drawing.Point(92, 25);
            this.diagLstIP.Name = "diagLstIP";
            this.diagLstIP.Size = new System.Drawing.Size(123, 175);
            this.diagLstIP.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.SkyBlue;
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(209, 14);
            this.label1.TabIndex = 3;
            this.label1.Tag = "";
            this.label1.Text = "通信先リスト";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // diagPnlSR
            // 
            this.diagPnlSR.Location = new System.Drawing.Point(221, 25);
            this.diagPnlSR.Name = "diagPnlSR";
            this.diagPnlSR.Size = new System.Drawing.Size(373, 175);
            this.diagPnlSR.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.SkyBlue;
            this.label2.Location = new System.Drawing.Point(221, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(373, 14);
            this.label2.TabIndex = 5;
            this.label2.Tag = "";
            this.label2.Text = "送受信テスト";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SettingsForm
            // 
            this.ClientSize = new System.Drawing.Size(622, 283);
            this.Controls.Add(this.tabBase);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SystemHub 設定";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.contextMenu.ResumeLayout(false);
            this.tabBase.ResumeLayout(false);
            this.tabInfo.ResumeLayout(false);
            this.tabInfo.PerformLayout();
            this.tabLog.ResumeLayout(false);
            this.tabLog.PerformLayout();
            this.tabPref.ResumeLayout(false);
            this.tabDiag.ResumeLayout(false);
            this.tabDiagBase.ResumeLayout(false);
            this.tabUDP.ResumeLayout(false);
            this.tabUDP.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private XControls.UI.Tab tabBase;
        private System.Windows.Forms.TabPage tabInfo;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.TabPage tabPref;
        private XControls.UI.Console infoBox;
        private XControls.UI.Console logBox;
        private XControls.UI.PushButton btnSave;
        private XControls.UI.PushButton btnLoad;
        private XControls.UI.ConfigGrid conf;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem MenuSettings;
        private System.Windows.Forms.ToolStripSeparator MenuSeparator1;
        private System.Windows.Forms.ToolStripMenuItem MenuExit;
        private System.Windows.Forms.TabPage tabDiag;
        private UI.Tab tabDiagBase;
        private System.Windows.Forms.TabPage tabUDP;
        private UI.Console console1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel diagPnlSR;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox diagLstIP;
        private UI.PushButton diagBtnList;
    }
}

