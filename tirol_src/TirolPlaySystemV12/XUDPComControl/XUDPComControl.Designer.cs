namespace XControls
{
    partial class XUDPComControl
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
                components.Dispose();
            }
            if (isOpen == true)
            {
                // 未クローズ時はクローズする
                Close();
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
            this.labelRecive = new System.Windows.Forms.Label();
            this.labelSend = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelRecive
            // 
            this.labelRecive.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelRecive.Location = new System.Drawing.Point(20, 3);
            this.labelRecive.Name = "labelRecive";
            this.labelRecive.Size = new System.Drawing.Size(12, 12);
            this.labelRecive.TabIndex = 3;
            this.labelRecive.Text = "R";
            this.labelRecive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelSend
            // 
            this.labelSend.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelSend.Location = new System.Drawing.Point(3, 3);
            this.labelSend.Name = "labelSend";
            this.labelSend.Size = new System.Drawing.Size(12, 12);
            this.labelSend.TabIndex = 2;
            this.labelSend.Text = "S";
            this.labelSend.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // XUDPComControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.labelRecive);
            this.Controls.Add(this.labelSend);
            this.Name = "XUDPComControl";
            this.Size = new System.Drawing.Size(36, 18);
            this.Load += new System.EventHandler(this.XUDPComControl_Load);
            this.FontChanged += new System.EventHandler(this.XUDPComControl_FontChanged);
            this.ForeColorChanged += new System.EventHandler(this.XUDPComControl_ForeColorChanged);
            this.SizeChanged += new System.EventHandler(this.XUDPComControl_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelRecive;
        private System.Windows.Forms.Label labelSend;
    }
}
