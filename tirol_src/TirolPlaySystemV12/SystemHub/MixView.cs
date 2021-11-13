using System;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace XControls.UI
{
    /// <summary>
    /// ニコ生キャプチャ用ウィンドウです。色々な情報をミックスして表示します。
    /// </summary>
    [Designer(typeof(MixViewDesigner))]
    public class MixView : Form
    {
        #region インナークラス

        #region MixViewDesignerクラス

        /// <summary>
        /// MixView用にデザイナをカスタマイズします。
        /// </summary>
        public class MixViewDesigner : System.Windows.Forms.Design.ControlDesigner
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.UI.MixView.MixViewDesigner クラスの新しいインスタンスを初期化します。
            /// </summary>
            public MixViewDesigner()
            {
            }

            #endregion

            #region メソッド

            #region PostFilterPropertiesメソッド

            protected override void PostFilterProperties(IDictionary properties)
            {
                // フィルタリングするプロパティ
//                properties.Remove("AutoScroll");
//                properties.Remove("AutoScrollMargin");
//                properties.Remove("AutoScrollMinSize");
//                properties.Remove("ImeMode");

                base.PostFilterProperties(properties);
            }

            #endregion

            #endregion
        }

        #endregion

        


        #endregion

        #region 構築・破棄

        /// <summary>
        /// XControls.UI.MixView クラスの新しいインスタンスを初期化します。
        /// </summary>
        public MixView()
        {
            InitializeComponent();
        }

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
            this.SuspendLayout();
            // 
            // MixView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Blue;
            this.ClientSize = new System.Drawing.Size(556, 323);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("MS UI Gothic", 12F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MixView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MixView";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        #endregion
    }
}
