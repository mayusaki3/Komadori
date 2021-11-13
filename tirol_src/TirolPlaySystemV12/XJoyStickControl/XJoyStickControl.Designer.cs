using System;

namespace XControls
{
    partial class XJoyStickControl
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
            // 監視タイマーストップ
            timerStop();

            if (disposing && (components != null))
            {
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
            this.timerLicenseCheck = new System.Windows.Forms.Timer(this.components);
            // 
            // timerLicenseCheck
            // 
            this.timerLicenseCheck.Interval = 5000;
            this.timerLicenseCheck.Tick += new System.EventHandler(this.timerLicenseCheck_Tick);

        }

        #endregion

        private System.Windows.Forms.Timer timerLicenseCheck;
    }
}
