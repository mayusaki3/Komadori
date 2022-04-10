namespace TirolPlaySystem
{
    partial class LiveCapForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LiveCapForm));
            this.pnlColorBar = new System.Windows.Forms.Panel();
            this.xscrview = new XControls.UI.XScreenView();
            this.SuspendLayout();
            // 
            // pnlColorBar
            // 
            this.pnlColorBar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlColorBar.BackgroundImage")));
            this.pnlColorBar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlColorBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlColorBar.Location = new System.Drawing.Point(0, 0);
            this.pnlColorBar.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.pnlColorBar.Name = "pnlColorBar";
            this.pnlColorBar.Size = new System.Drawing.Size(1109, 576);
            this.pnlColorBar.TabIndex = 4;
            // 
            // xscrview
            // 
            this.xscrview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xscrview.Enabled = true;
            this.xscrview.Location = new System.Drawing.Point(0, 0);
            this.xscrview.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.xscrview.MouseZoom = true;
            this.xscrview.Name = "xscrview";
            this.xscrview.ScreenIndex = 0;
            this.xscrview.Size = new System.Drawing.Size(1109, 576);
            this.xscrview.TabIndex = 3;
            this.xscrview.Visible = false;
            this.xscrview.Zoom = ((ushort)(100));
            // 
            // LiveCapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Blue;
            this.ClientSize = new System.Drawing.Size(1109, 576);
            this.Controls.Add(this.pnlColorBar);
            this.Controls.Add(this.xscrview);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Name = "LiveCapForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "LiveCapForm";
            this.ResumeLayout(false);

        }

        #endregion
        private XControls.UI.XScreenView xscrview;
        private System.Windows.Forms.Panel pnlColorBar;
    }
}