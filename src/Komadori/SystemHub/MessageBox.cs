using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace XControls.UI
{
    /// <summary>
    /// このクラスは、標準のMessageBoxを置き換えます。
    /// </summary>
    public class MessageBox : Form
    {
        #region 構築・破棄

        /// <summary>
        /// XControls.MessageBox クラスの新しいインスタンスを初期化します。
        /// </summary>
        public MessageBox()
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
            this.lblMessage = new System.Windows.Forms.Label();
            this.pnlBtn = new System.Windows.Forms.Panel();
            this.lblIcon = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnAbort = new XControls.UI.PushButton();
            this.btnNo = new XControls.UI.PushButton();
            this.btnRetry = new XControls.UI.PushButton();
            this.btnYes = new XControls.UI.PushButton();
            this.btnIgnore = new XControls.UI.PushButton();
            this.btnCancel = new XControls.UI.PushButton();
            this.btnOk = new XControls.UI.PushButton();
            this.pnlBtn.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblMessage.Font = new System.Drawing.Font("ＭＳ ゴシック", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblMessage.ForeColor = System.Drawing.Color.White;
            this.lblMessage.Location = new System.Drawing.Point(152, 69);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(177, 38);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "終了します。\r\nよろしいですか？";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlBtn
            // 
            this.pnlBtn.BackColor = System.Drawing.Color.Transparent;
            this.pnlBtn.Controls.Add(this.btnAbort);
            this.pnlBtn.Controls.Add(this.btnNo);
            this.pnlBtn.Controls.Add(this.btnRetry);
            this.pnlBtn.Controls.Add(this.btnYes);
            this.pnlBtn.Controls.Add(this.btnIgnore);
            this.pnlBtn.Controls.Add(this.btnCancel);
            this.pnlBtn.Controls.Add(this.btnOk);
            this.pnlBtn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBtn.Location = new System.Drawing.Point(0, 166);
            this.pnlBtn.Name = "pnlBtn";
            this.pnlBtn.Size = new System.Drawing.Size(556, 157);
            this.pnlBtn.TabIndex = 0;
            // 
            // lblIcon
            // 
            this.lblIcon.AutoSize = true;
            this.lblIcon.BackColor = System.Drawing.Color.Transparent;
            this.lblIcon.Font = new System.Drawing.Font("ＭＳ ゴシック", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblIcon.ForeColor = System.Drawing.Color.White;
            this.lblIcon.Location = new System.Drawing.Point(46, 69);
            this.lblIcon.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIcon.Name = "lblIcon";
            this.lblIcon.Size = new System.Drawing.Size(72, 19);
            this.lblIcon.TabIndex = 1;
            this.lblIcon.Text = "警告：";
            this.lblIcon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("ＭＳ ゴシック", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(556, 35);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "TITLE";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // btnAbort
            // 
            this.btnAbort.ActionMode = XControls.UI.PushButton.ActionModes.PushButton;
            this.btnAbort.BorderSize = 4;
            this.btnAbort.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAbort.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnAbort.FlatAppearance.BorderSize = 2;
            this.btnAbort.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnAbort.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnAbort.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkViolet;
            this.btnAbort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbort.Font = new System.Drawing.Font("MS UI Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.btnAbort.GroupName = "";
            this.btnAbort.IsPushed = false;
            this.btnAbort.Location = new System.Drawing.Point(18, 17);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.PushOnBackColor = System.Drawing.SystemColors.Control;
            this.btnAbort.Size = new System.Drawing.Size(120, 40);
            this.btnAbort.TabIndex = 1;
            this.btnAbort.Text = "中止(&A)\r\n";
            this.btnAbort.UseVisualStyleBackColor = false;
            this.btnAbort.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnNo
            // 
            this.btnNo.ActionMode = XControls.UI.PushButton.ActionModes.PushButton;
            this.btnNo.BorderSize = 4;
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNo.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnNo.FlatAppearance.BorderSize = 2;
            this.btnNo.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnNo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnNo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkViolet;
            this.btnNo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNo.Font = new System.Drawing.Font("MS UI Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.btnNo.GroupName = "";
            this.btnNo.IsPushed = false;
            this.btnNo.Location = new System.Drawing.Point(270, 63);
            this.btnNo.Name = "btnNo";
            this.btnNo.PushOnBackColor = System.Drawing.SystemColors.Control;
            this.btnNo.Size = new System.Drawing.Size(120, 40);
            this.btnNo.TabIndex = 6;
            this.btnNo.Text = "いいえ(&N)\r\n";
            this.btnNo.UseVisualStyleBackColor = false;
            this.btnNo.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnRetry
            // 
            this.btnRetry.ActionMode = XControls.UI.PushButton.ActionModes.PushButton;
            this.btnRetry.BorderSize = 4;
            this.btnRetry.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnRetry.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnRetry.FlatAppearance.BorderSize = 2;
            this.btnRetry.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnRetry.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnRetry.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkViolet;
            this.btnRetry.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRetry.Font = new System.Drawing.Font("MS UI Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.btnRetry.GroupName = "";
            this.btnRetry.IsPushed = false;
            this.btnRetry.Location = new System.Drawing.Point(18, 63);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.PushOnBackColor = System.Drawing.SystemColors.Control;
            this.btnRetry.Size = new System.Drawing.Size(120, 40);
            this.btnRetry.TabIndex = 2;
            this.btnRetry.Text = "再試行(&R)\r\n";
            this.btnRetry.UseVisualStyleBackColor = false;
            this.btnRetry.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnYes
            // 
            this.btnYes.ActionMode = XControls.UI.PushButton.ActionModes.PushButton;
            this.btnYes.BorderSize = 4;
            this.btnYes.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnYes.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnYes.FlatAppearance.BorderSize = 2;
            this.btnYes.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnYes.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnYes.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkViolet;
            this.btnYes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnYes.Font = new System.Drawing.Font("MS UI Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.btnYes.GroupName = "";
            this.btnYes.IsPushed = false;
            this.btnYes.Location = new System.Drawing.Point(270, 17);
            this.btnYes.Name = "btnYes";
            this.btnYes.PushOnBackColor = System.Drawing.SystemColors.Control;
            this.btnYes.Size = new System.Drawing.Size(120, 40);
            this.btnYes.TabIndex = 5;
            this.btnYes.Text = "はい(&Y)\r\n";
            this.btnYes.UseVisualStyleBackColor = false;
            this.btnYes.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnIgnore
            // 
            this.btnIgnore.ActionMode = XControls.UI.PushButton.ActionModes.PushButton;
            this.btnIgnore.BorderSize = 4;
            this.btnIgnore.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnIgnore.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnIgnore.FlatAppearance.BorderSize = 2;
            this.btnIgnore.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnIgnore.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnIgnore.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkViolet;
            this.btnIgnore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIgnore.Font = new System.Drawing.Font("MS UI Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.btnIgnore.GroupName = "";
            this.btnIgnore.IsPushed = false;
            this.btnIgnore.Location = new System.Drawing.Point(18, 109);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.PushOnBackColor = System.Drawing.SystemColors.Control;
            this.btnIgnore.Size = new System.Drawing.Size(120, 40);
            this.btnIgnore.TabIndex = 3;
            this.btnIgnore.Text = "無視(&I)\r\n";
            this.btnIgnore.UseVisualStyleBackColor = false;
            this.btnIgnore.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.ActionMode = XControls.UI.PushButton.ActionModes.PushButton;
            this.btnCancel.BorderSize = 4;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnCancel.FlatAppearance.BorderSize = 2;
            this.btnCancel.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkViolet;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("MS UI Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.btnCancel.GroupName = "";
            this.btnCancel.IsPushed = false;
            this.btnCancel.Location = new System.Drawing.Point(396, 17);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.PushOnBackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Size = new System.Drawing.Size(120, 40);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "キャンセル\r\n";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnOk
            // 
            this.btnOk.ActionMode = XControls.UI.PushButton.ActionModes.PushButton;
            this.btnOk.BorderSize = 4;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnOk.FlatAppearance.BorderSize = 2;
            this.btnOk.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnOk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkViolet;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Font = new System.Drawing.Font("MS UI Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.btnOk.GroupName = "";
            this.btnOk.IsPushed = false;
            this.btnOk.Location = new System.Drawing.Point(144, 17);
            this.btnOk.Name = "btnOk";
            this.btnOk.PushOnBackColor = System.Drawing.SystemColors.Control;
            this.btnOk.Size = new System.Drawing.Size(120, 40);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK\r\n";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.Button_Click);
            // 
            // MessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.ClientSize = new System.Drawing.Size(556, 323);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblIcon);
            this.Controls.Add(this.pnlBtn);
            this.Controls.Add(this.lblMessage);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("MS UI Gothic", 12F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MessageBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MessageBox";
            this.TopMost = true;
            this.pnlBtn.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private PushButton btnAbort;
        private PushButton btnRetry;
        private PushButton btnIgnore;
        private PushButton btnOk;
        private PushButton btnCancel;
        private PushButton btnYes;
        private PushButton btnNo;
        private System.Windows.Forms.Panel pnlBtn;
        private System.Windows.Forms.Label lblIcon;
        private System.Windows.Forms.Label lblTitle;

        #endregion

        #region 変数

        private static MessageBox msgbox = new MessageBox();
        private static MessageBoxButtons savbuttons;
        private static DialogResult result = DialogResult.None;
        private static IWin32Window savowner;
        private static Object msglock = new Object();

        #endregion

        #region プロパティ

        #region CreateParamsプロパティ

        /// <summary>
        /// コントロールの作成時に必要な情報を返します。
        /// </summary>
        [
            Description("コントロールの作成時に必要な情報を返します。")
        ]
        protected override CreateParams CreateParams
        {
            [
                System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.SecurityAction.LinkDemand,
                Flags = System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)
            ]
            get
            {
                const int CS_NOCLOSE = 0x200;
                CreateParams cp = base.CreateParams;

                // クローズボタンの有効/無効を切り替える
                switch (savbuttons)
                {
                    case MessageBoxButtons.AbortRetryIgnore:
                    case MessageBoxButtons.YesNo:
                        cp.ClassStyle = cp.ClassStyle | CS_NOCLOSE;
                        break;
                }

                return cp;
            }
        }

        #endregion

        #endregion

        #region メソッド

        #region Showメソッド(static)

        /// <summary>
        /// メッセージボックスを表示します。
        /// </summary>
        /// <param name="text">メッセージボックスに表示するテキスト</param>
        /// <param name="caption">メッセージボックスのタイトルバーに表示するテキスト</param>
        /// <param name="buttons">メッセージボックスに表示されるボタンを指定する MessageBoxButtons 値の1つ</param>
        /// <param name="icon">メッセージボックスに表示されるアイコンを指定する MessageBoxIcon 値の1つ</param>
        /// <param name="defaultButton">メッセージボックスの既定のボタンを指定する MessageBoxDefaultButton 値の1つ</param>
        /// <returns>DialogResult 値のいずれか</returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon,
                                        MessageBoxDefaultButton defaultButton)
        {
            return ShowCore(null, text, caption, buttons, icon, defaultButton);
        }

        /// <summary>
        /// メッセージボックスを表示します。
        /// </summary>
        /// <param name="text">メッセージボックスに表示するテキスト</param>
        /// <param name="caption">メッセージボックスのタイトルバーに表示するテキスト</param>
        /// <param name="buttons">メッセージボックスに表示されるボタンを指定する MessageBoxButtons 値の1つ</param>
        /// <param name="icon">メッセージボックスに表示されるアイコンを指定する MessageBoxIcon 値の1つ</param>
        /// <returns>DialogResult 値のいずれか</returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon)
        {
            return ShowCore(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// メッセージボックスを表示します。
        /// </summary>
        /// <param name="text">メッセージボックスに表示するテキスト</param>
        /// <param name="caption">メッセージボックスのタイトルバーに表示するテキスト</param>
        /// <param name="buttons">メッセージボックスに表示されるボタンを指定する MessageBoxButtons 値の1つ</param>
        /// <returns>DialogResult 値のいずれか</returns>
        public static DialogResult Show(string text,
                                        string caption,
                                        MessageBoxButtons buttons)
        {
            return ShowCore(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// メッセージボックスを表示します。
        /// </summary>
        /// <param name="text">メッセージボックスに表示するテキスト</param>
        /// <param name="caption">メッセージボックスのタイトルバーに表示するテキスト</param>
        /// <returns>DialogResult 値のいずれか</returns>
        public static DialogResult Show(string text,
                                        string caption)
        {
            return ShowCore(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// メッセージボックスを表示します。
        /// </summary>
        /// <param name="text">メッセージボックスに表示するテキスト</param>
        /// <returns>DialogResult 値のいずれか</returns>
        public static DialogResult Show(string text)
        {
            return ShowCore(null, text, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// メッセージボックスを表示します。
        /// </summary>
        /// <param name="owner">モーダル ダイアログ ボックスを所有する IWin32Window の実装</param>
        /// <param name="text">メッセージボックスに表示するテキスト</param>
        /// <param name="caption">メッセージボックスのタイトルバーに表示するテキスト</param>
        /// <param name="buttons">メッセージボックスに表示されるボタンを指定する MessageBoxButtons 値の1つ</param>
        /// <param name="icon">メッセージボックスに表示されるアイコンを指定する MessageBoxIcon 値の1つ</param>
        /// <param name="defaultButton">メッセージボックスの既定のボタンを指定する MessageBoxDefaultButton 値の1つ</param>
        /// <returns>DialogResult 値のいずれか</returns>
        public static DialogResult Show(IWin32Window owner,
                                        string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon,
                                        MessageBoxDefaultButton defaultButton) {
            return ShowCore(owner, text, caption, buttons, icon, defaultButton);
        }
 
        /// <summary>
        /// メッセージボックスを表示します。
        /// </summary>
        /// <param name="owner">モーダル ダイアログ ボックスを所有する IWin32Window の実装</param>
        /// <param name="text">メッセージボックスに表示するテキスト</param>
        /// <param name="caption">メッセージボックスのタイトルバーに表示するテキスト</param>
        /// <param name="buttons">メッセージボックスに表示されるボタンを指定する MessageBoxButtons 値の1つ</param>
        /// <param name="icon">メッセージボックスに表示されるアイコンを指定する MessageBoxIcon 値の1つ</param>
        /// <returns>DialogResult 値のいずれか</returns>
        public static DialogResult Show(IWin32Window owner,
                                        string text,
                                        string caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon) {
            return ShowCore(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }
 
        /// <summary>
        /// メッセージボックスを表示します。
        /// </summary>
        /// <param name="owner">モーダル ダイアログ ボックスを所有する IWin32Window の実装</param>
        /// <param name="text">メッセージボックスに表示するテキスト</param>
        /// <param name="caption">メッセージボックスのタイトルバーに表示するテキスト</param>
        /// <param name="buttons">メッセージボックスに表示されるボタンを指定する MessageBoxButtons 値の1つ</param>
        /// <returns>DialogResult 値のいずれか</returns>
        public static DialogResult Show(IWin32Window owner,
                                        string text,
                                        string caption,
                                        MessageBoxButtons buttons) {
            return ShowCore(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }
  
        /// <summary>
        /// メッセージボックスを表示します。
        /// </summary>
        /// <param name="owner">モーダル ダイアログ ボックスを所有する IWin32Window の実装</param>
        /// <param name="text">メッセージボックスに表示するテキスト</param>
        /// <param name="caption">メッセージボックスのタイトルバーに表示するテキスト</param>
        /// <returns>DialogResult 値のいずれか</returns>
        public static DialogResult Show(IWin32Window owner,
                                        string text,
                                        string caption) {
            return ShowCore(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }
 
        /// <summary>
        /// メッセージボックスを表示します。
        /// </summary>
        /// <param name="owner">モーダル ダイアログ ボックスを所有する IWin32Window の実装</param>
        /// <param name="text">メッセージボックスに表示するテキスト</param>
        /// <returns>DialogResult 値のいずれか</returns>
        public static DialogResult Show(IWin32Window owner,
                                        string text) {
            return ShowCore(owner, text, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        #endregion

        #endregion

        #region イベント

        #region OnLayoutイベント

        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (msgbox == null) return;

            base.OnLayout(levent);

            // オーナーが指定していなければ、プライマリスクリーンの中央に表示
            msgbox.Top = (Screen.PrimaryScreen.WorkingArea.Height - msgbox.Height) / 2;
            msgbox.Left = (Screen.PrimaryScreen.WorkingArea.Width - msgbox.Width) / 2;
            if (savowner == null)
            {
            }
            else
            {
                // オーナーが指定したあれば、そのウィンドウの中央に表示
                try
                {
                    Rectangle or = Form.FromHandle(savowner.Handle).Bounds;
                    int x = or.Width / 2 + or.Left;
                    int y = or.Height / 2 + or.Top;
                    for (int i = 0; i < Screen.AllScreens.Length; i++)
                    {
                        Rectangle sr = Screen.AllScreens[i].WorkingArea;
                        if (sr.Left <= x && sr.Right >= x && sr.Top <= y && sr.Bottom >= y)
                        {
                            msgbox.Left = x - msgbox.Width / 2;
                            msgbox.Top = sr.Top + sr.Height / 2 - msgbox.Height / 2;
                            if (msgbox.Top < sr.Top) msgbox.Top = sr.Top;
                            if (msgbox.Left < sr.Left) msgbox.Left = sr.Left;
                            if (msgbox.Top > sr.Top + sr.Height - msgbox.Height) msgbox.Top = sr.Top + sr.Height - msgbox.Height;
                            if (msgbox.Left > sr.Left + sr.Width - msgbox.Width) msgbox.Left = sr.Left + sr.Width - msgbox.Width;
                            break;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        #endregion

        #region Button_Clickイベント

        private void Button_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Equals(btnAbort))   result = DialogResult.Abort;
            if (btn.Equals(btnCancel))  result = DialogResult.Cancel;
            if (btn.Equals(btnIgnore))  result = DialogResult.Ignore;
            if (btn.Equals(btnNo))      result = DialogResult.No;
            if (btn.Equals(btnOk))      result = DialogResult.OK;
            if (btn.Equals(btnRetry))   result = DialogResult.Retry;
            if (btn.Equals(btnYes))     result = DialogResult.Yes;
        }

        #endregion

        #region OnClosingイベント

        protected override void OnClosing(CancelEventArgs e)
        {
            if (result == DialogResult.None && savbuttons == MessageBoxButtons.OK) result = DialogResult.OK;
            if (result == DialogResult.None && btnCancel.Visible) result = DialogResult.Cancel;
            if (result == DialogResult.None)
            {
                e.Cancel = true;
                return;
            }
            base.OnClosing(e);
        }

        #endregion

        #endregion

        #region 内部処理

        #region ShowCore処理
        
        private static DialogResult ShowCore(IWin32Window owner,
                                             string text,
                                             string caption,
                                             MessageBoxButtons buttons,
                                             MessageBoxIcon icon,
                                             MessageBoxDefaultButton defaultButton)
        {
            #region 表示調整

            msgbox = new MessageBox();
            msgbox.SuspendLayout();

            // 指定値を退避
            savbuttons = buttons;
            savowner = owner;
            if (savowner == null)
            {
                msgbox.Owner = null;
            }
            else
            {
                try
                {
                    msgbox.Owner = (Form)Form.FromHandle(savowner.Handle);
                }
                catch
                {
                }
            }

            // ダイアログ幅と位置
            msgbox.Width = Screen.PrimaryScreen.Bounds.Width;
            msgbox.Left = 0;
            int hofs = 0;

            // タイトル
            msgbox.lblTitle.Text = caption;

            // メッセージ
            msgbox.lblMessage.Text = text;
            msgbox.lblMessage.Top = msgbox.lblTitle.Height + 16;
            msgbox.lblMessage.Left = (msgbox.ClientSize.Width - msgbox.lblMessage.Width) / 2;

            // アイコン設定
            msgbox.lblIcon.Visible = false;
            int colorofs = 0;
            switch (icon)
            {
                case MessageBoxIcon.Error:
                    msgbox.BackColor = Color.Red;
                    colorofs = 90;
                    msgbox.ForeColor = Color.White;
                    msgbox.lblIcon.Text = "エラー : ";
                    msgbox.lblIcon.Visible = true;
                    break;
                case MessageBoxIcon.Information:
                    msgbox.BackColor = Color.DodgerBlue;
                    colorofs = 30;
                    msgbox.ForeColor = Color.White;
                    msgbox.lblIcon.Text = "情報 : ";
                    msgbox.lblIcon.Visible = true;
                    break;
                case MessageBoxIcon.Question:
                    msgbox.BackColor = Color.Yellow;
                    colorofs = 150;
                    msgbox.ForeColor = Color.Black;
                    msgbox.lblIcon.Text = "確認 : ";
                    msgbox.lblIcon.Visible = true;
                    break;
                case MessageBoxIcon.Warning:
                    msgbox.BackColor = Color.Yellow;
                    colorofs = 150;
                    msgbox.ForeColor = Color.Black;
                    msgbox.lblIcon.Text = "警告 : ";
                    msgbox.lblIcon.Visible = true;
                    break;
                default:
                    msgbox.BackColor = Color.DodgerBlue;
                    colorofs = 30;
                    msgbox.ForeColor = Color.White;
                    break;
            }
            msgbox.lblIcon.Top = msgbox.lblTitle.Height + 16;
            msgbox.lblIcon.Left = msgbox.lblMessage.Left - 16 - msgbox.lblIcon.Width; 

            // ボタンパネル
            msgbox.pnlBtn.Height = msgbox.btnOk.Height + 16 + 8;
            msgbox.pnlBtn.Width = msgbox.ClientSize.Width;

            // ダイアログ高と位置
            hofs = 42 - msgbox.lblMessage.Height;
            if (hofs < 0) hofs = 0;
            msgbox.Height = msgbox.lblTitle.Height + 16 + msgbox.lblMessage.Height + hofs + 8 + msgbox.pnlBtn.Height;
            msgbox.Top = (Screen.PrimaryScreen.Bounds.Height - msgbox.Height) / 2;

            // ボタン
            msgbox.btnAbort.Visible = false;
            msgbox.btnRetry.Visible = false;
            msgbox.btnIgnore.Visible = false;
            msgbox.btnOk.Visible = false;
            msgbox.btnYes.Visible = false;
            msgbox.btnNo.Visible = false;
            msgbox.btnCancel.Visible = false;

            // カラー調整
            msgbox.btnAbort.BackColor = msgbox.BackColor;
            msgbox.btnCancel.BackColor = msgbox.BackColor;
            msgbox.btnIgnore.BackColor = msgbox.BackColor;
            msgbox.btnNo.BackColor = msgbox.BackColor;
            msgbox.btnOk.BackColor = msgbox.BackColor;
            msgbox.btnRetry.BackColor = msgbox.BackColor;
            msgbox.btnYes.BackColor = msgbox.BackColor;
            msgbox.lblTitle.BackColor = msgbox.BackColor;
            msgbox.lblIcon.BackColor = msgbox.BackColor;
            msgbox.lblMessage.BackColor = msgbox.BackColor;
            msgbox.btnAbort.ForeColor = msgbox.ForeColor;
            msgbox.btnCancel.ForeColor = msgbox.ForeColor;
            msgbox.btnIgnore.ForeColor = msgbox.ForeColor;
            msgbox.btnNo.ForeColor = msgbox.ForeColor;
            msgbox.btnOk.ForeColor = msgbox.ForeColor;
            msgbox.btnRetry.ForeColor = msgbox.ForeColor;
            msgbox.btnYes.ForeColor = msgbox.ForeColor;
            msgbox.lblTitle.ForeColor = msgbox.ForeColor;
            msgbox.lblIcon.ForeColor = msgbox.ForeColor;
            msgbox.lblMessage.ForeColor = msgbox.ForeColor;
            msgbox.btnAbort.EnterBackColorOffset = colorofs;
            msgbox.btnCancel.EnterBackColorOffset = colorofs;
            msgbox.btnIgnore.EnterBackColorOffset = colorofs;
            msgbox.btnNo.EnterBackColorOffset = colorofs;
            msgbox.btnOk.EnterBackColorOffset = colorofs;
            msgbox.btnRetry.EnterBackColorOffset = colorofs;
            msgbox.btnYes.EnterBackColorOffset = colorofs;

            switch (buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    msgbox.btnAbort.Visible = true;
                    msgbox.btnRetry.Visible = true;
                    msgbox.btnIgnore.Visible = true;
                    msgbox.btnAbort.Top = msgbox.btnRetry.Top = msgbox.btnIgnore.Top = 8;
                    msgbox.btnAbort.Left = (msgbox.pnlBtn.Width - msgbox.btnAbort.Width - msgbox.btnRetry.Width - msgbox.btnIgnore.Width - 32 * 2) / 2;
                    msgbox.btnRetry.Left = msgbox.btnAbort.Right + 32;
                    msgbox.btnIgnore.Left = msgbox.btnRetry.Right + 32;
                    break;
                case MessageBoxButtons.OK:
                    msgbox.btnOk.Visible = true;
                    msgbox.btnOk.Top = 8;
                    msgbox.btnOk.Left = (msgbox.pnlBtn.Width - msgbox.btnOk.Width)/2;
                    break;
                case MessageBoxButtons.OKCancel:
                    msgbox.btnOk.Visible = true;
                    msgbox.btnCancel.Visible = true;
                    msgbox.btnOk.Top = msgbox.btnCancel.Top = 8;
                    msgbox.btnOk.Left = (msgbox.pnlBtn.Width - msgbox.btnOk.Width - msgbox.btnCancel.Width - 32)/ 2;
                    msgbox.btnCancel.Left = msgbox.btnOk.Right + 32;
                    break;
                case MessageBoxButtons.RetryCancel:
                    msgbox.btnRetry.Visible = true;
                    msgbox.btnCancel.Visible = true;
                    msgbox.btnRetry.Top = msgbox.btnCancel.Top = 8;
                    msgbox.btnRetry.Left = (msgbox.pnlBtn.Width - msgbox.btnRetry.Width - msgbox.btnCancel.Width - 32)/ 2;
                    msgbox.btnCancel.Left = msgbox.btnRetry.Right + 32;
                    break;
                case MessageBoxButtons.YesNo:
                    msgbox.btnYes.Visible = true;
                    msgbox.btnNo.Visible = true;
                    msgbox.btnYes.Top = msgbox.btnNo.Top = 8;
                    msgbox.btnYes.Left = (msgbox.pnlBtn.Width - msgbox.btnYes.Width - msgbox.btnNo.Width - 32)/ 2;
                    msgbox.btnNo.Left = msgbox.btnYes.Right + 32;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    msgbox.btnYes.Visible = true;
                    msgbox.btnNo.Visible = true;
                    msgbox.btnCancel.Visible = true;
                    msgbox.btnYes.Top = msgbox.btnNo.Top = msgbox.btnCancel.Top = 8;
                    msgbox.btnYes.Left = (msgbox.pnlBtn.Width - msgbox.btnYes.Width - msgbox.btnNo.Width - msgbox.btnCancel.Width - 32 * 2)/ 2;
                    msgbox.btnNo.Left = msgbox.btnYes.Right + 32;
                    msgbox.btnCancel.Left = msgbox.btnNo.Right + 32;
                    break;
            }

            msgbox.ResumeLayout();

            #endregion

            lock(msglock)
            {
                #region 選択

                // リザルト
                result = DialogResult.None;
                msgbox.Show();

                // フォーカス
                switch (buttons)
                {
                    case MessageBoxButtons.AbortRetryIgnore:
                        msgbox.btnAbort.Focus();
                        if (defaultButton == MessageBoxDefaultButton.Button2) msgbox.btnRetry.Focus();
                        if (defaultButton == MessageBoxDefaultButton.Button3) msgbox.btnIgnore.Focus();
                        break;
                    case MessageBoxButtons.OK:
                        msgbox.btnOk.Focus();
                        break;
                    case MessageBoxButtons.OKCancel:
                        msgbox.btnOk.Focus();
                        if (defaultButton == MessageBoxDefaultButton.Button2) msgbox.btnCancel.Focus();
                        break;
                    case MessageBoxButtons.RetryCancel:
                        msgbox.btnRetry.Focus();
                        if (defaultButton == MessageBoxDefaultButton.Button2) msgbox.btnCancel.Focus();
                        break;
                    case MessageBoxButtons.YesNo:
                        msgbox.btnYes.Focus();
                        if (defaultButton == MessageBoxDefaultButton.Button2) msgbox.btnNo.Focus();
                        break;
                    case MessageBoxButtons.YesNoCancel:
                        msgbox.btnYes.Focus();
                        if (defaultButton == MessageBoxDefaultButton.Button2) msgbox.btnNo.Focus();
                        if (defaultButton == MessageBoxDefaultButton.Button3) msgbox.btnCancel.Focus();
                        break;
                }
                Application.DoEvents();

                // 音
                switch (icon)
                {
                    case MessageBoxIcon.Error:
                        System.Media.SystemSounds.Hand.Play();
                        break;
                    case MessageBoxIcon.Information:
                        System.Media.SystemSounds.Asterisk.Play();
                        break;
                    case MessageBoxIcon.Question:
                        System.Media.SystemSounds.Question.Play();
                        break;
                    case MessageBoxIcon.Warning:
                        System.Media.SystemSounds.Exclamation.Play();
                        break;
                    default:
                        System.Media.SystemSounds.Asterisk.Play();
                        break;
                }

                if (msgbox.Owner != null) msgbox.Owner.Enabled = false;
                while (result == DialogResult.None && !msgbox.IsDisposed)
                {
                    Application.DoEvents();
                    Thread.Sleep(100);
                }
                if (msgbox.Owner != null) msgbox.Owner.Enabled = true;
                msgbox.Hide();

                #endregion
            }

            return result;
        }

        #endregion

        #endregion
    }
}
