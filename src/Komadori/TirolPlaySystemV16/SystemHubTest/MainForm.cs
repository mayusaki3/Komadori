using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystemHubTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            //hub = XControls.SystemHub.GetInstance();

            Application.Idle += Application_Idle;

            p2prop1.SelectedObject = p2log;
            p2log.LoggingNotice += p2log_LoggingNotice;
            p3prop1.SelectedObject = p3tab;
            p5prop1.SelectedObject = p5cmd;
            p5cmd.LoggingEvent += p5cmd_LoggingEvent;
            p5cmd.OutputValueEvent += p5cmd_OutputValueEvent;
            p5cmd.ResponseTimeout = 3000;
            p5cmd.Timeout = 10000;
            p5cmd.EchoStdIn = false;
            p6lstBtn.Items.Add(MessageBoxButtons.AbortRetryIgnore);
            p6lstBtn.Items.Add(MessageBoxButtons.OK);
            p6lstBtn.Items.Add(MessageBoxButtons.OKCancel);
            p6lstBtn.Items.Add(MessageBoxButtons.RetryCancel);
            p6lstBtn.Items.Add(MessageBoxButtons.YesNo);
            p6lstBtn.Items.Add(MessageBoxButtons.YesNoCancel);
            p6lstIcon.Items.Add(MessageBoxIcon.Asterisk);
            p6lstIcon.Items.Add(MessageBoxIcon.Error);
            p6lstIcon.Items.Add(MessageBoxIcon.Exclamation);
            p6lstIcon.Items.Add(MessageBoxIcon.Hand);
            p6lstIcon.Items.Add(MessageBoxIcon.Information);
            p6lstIcon.Items.Add(MessageBoxIcon.None);
            p6lstIcon.Items.Add(MessageBoxIcon.Question);
            p6lstIcon.Items.Add(MessageBoxIcon.Stop);
            p6lstIcon.Items.Add(MessageBoxIcon.Warning);
            p6lstDefault.Items.Add(MessageBoxDefaultButton.Button1);
            p6lstDefault.Items.Add(MessageBoxDefaultButton.Button2);
            p6lstDefault.Items.Add(MessageBoxDefaultButton.Button3);

            p9tim1.LinkageTimerView = new XControls.UI.TimerView[] { p9tim2, p9tim3 };
            p9prop1.SelectedObject = p9tim1;

            p14prop1.SelectedObject = p14mxv;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            Control ctl = ActiveControl;
            switch(tab1.SelectedIndex + 1)
            {
                case 3: p3_SelectObject(ctl); break;
            }
        }

        private XControls.SystemHub hub = null;
        private XControls.IO.Logging p2log = new XControls.IO.Logging();
        private XControls.Command p5cmd = new XControls.Command();
        private XControls.ExcelLink p7excel = new XControls.ExcelLink();
        private XControls.ExcelLink.ApplicationClass p7app = null;
        private XControls.ExcelLink.WorkbookClass p7book = null;
        private XControls.ExcelLink.WorksheetClass p7sheet = null;
        private XControls.MixViewController p14mxv = new XControls.MixViewController();

        #region PAGE1 - ConsoleBox

        private void p1btnClear_Click(object sender, EventArgs e)
        {
            console.Clear();
        }

        private void p1btnRefresh_Click(object sender, EventArgs e)
        {
            console.Refresh();
        }

        private void p1btnPrint_Click(object sender, EventArgs e)
        {
            console.Print(p1txtPrint.Text);
        }

        private void p1btnPrintLine_Click(object sender, EventArgs e)
        {
            console.PrintLine(p1txtPrintLine.Text);
        }

        private void p1btnReplace_Click(object sender, EventArgs e)
        {
            if (console.Lines.Length > 0)
            {
                string[] w = console.Lines;
                w[w.Length - 1] = p1txtReplace.Text;
                console.Lines = w;
            }
        }

        #endregion

        #region PAGE2 - Logging

        void p2log_LoggingNotice(object sender, XControls.IO.Logging.LoggingNoticeEventArgs e)
        {
            console.PrintLine("P2> " + e.Message);
        }

        private void p2btnPrint_Click(object sender, EventArgs e)
        {
            p2log.Print(p2txtPrint.Text);
        }

        private void p2btnDump_Click(object sender, EventArgs e)
        {
            byte[] hx = Encoding.Unicode.GetBytes(p2txtDump.Text);
            p2log.HexDump("テキスト", hx, hx.Length);
        }

        private void p2btnFile_Click(object sender, EventArgs e)
        {
            p2txtName.Text = p2log.GetCurrentLogFile();
        }

        private void p2btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(p2txtName.Text);
            }
            catch
            {
            }
        }
    
        #endregion

        #region PAGE3 - Tab / PushButton

        private void p3_SelectObject(Control ctl)
        {
            if (ctl.Name.ToLower().IndexOf("tab") >= 0)
            {
                p3prop1.SelectedObject = p3tab;
            }
            if (ctl.Name.ToLower().IndexOf("btn") >= 0)
            {
                p3prop1.SelectedObject = ctl;
            }
        }

        #endregion

        #region PAGE4 - ConfigGrid

        private void p4_Enter(object sender, EventArgs e)
        {
            p4prop1.SelectedObject = sender;
            p4lblTarget.Text = (sender as XControls.UI.ConfigGrid).Name;
            p4lblProp.Text = p4lblTarget.Text;
        }

        private XControls.UI.ConfigGrid p4Conf(object sender)
        {
            Control ctl = sender as Control;
            if(ctl.Name.Substring(0,3) == "p4a")
            {
                return p4conf1;
            }
            else
            {
                return p4conf2;
            }
        }

        private void p4conf_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            Control ctl = sender as Control;
            string val = "(なし)";
            if (e.OldValue != null) val = e.OldValue.ToString();
            console.PrintLine("P4> 変更[" + ctl.Name + "] " + e.ChangedItem.Label + " = " + val + " → " + e.ChangedItem.Value.ToString());
        }

        private void p4conf_SelectedGridItem(object sender, SelectedGridItemChangedEventArgs e)
        {
            Control ctl = sender as Control;
            string from = "";
            try
            {
                from = e.OldSelection.Label;
            }
            catch
            {
            }
            console.PrintLine("P4> 選択[" + ctl.Name + "] " + from + " → " + e.NewSelection.Label.ToString());
        }

        private void p4aCmbCat_Enter(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            ComboBox cmbnam = p4aCmbNam;
            if (cmb.Name.Substring(0, 3) == "p4b") cmbnam = p4bCmbNam;
            XControls.UI.ConfigGrid conf = p4Conf(sender);
            cmb.Items.Clear();
            cmbnam.Items.Clear();
            for (int i = 0; i < conf.ItemSettings.Count; i++)
            {
                string cat = conf.ItemSettings[i].Category + " | " + conf.ItemSettings[i].DisplayCategory;
                if (cmb.Items.IndexOf(cat) < 0) cmb.Items.Add(cat);
            }
        }

        private void p4aCmbCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            ComboBox cmbnam = p4aCmbNam;
            if (cmb.Name.Substring(0, 3) == "p4b") cmbnam = p4bCmbNam;
            XControls.UI.ConfigGrid conf = p4Conf(sender);
            cmbnam.Items.Clear();
            for (int i = 0; i < conf.ItemSettings.Count; i++)
            {
                string cat = conf.ItemSettings[i].Category + " | " + conf.ItemSettings[i].DisplayCategory;
                if (cat == cmb.Text)
                {
                    string nam = conf.ItemSettings[i].Name + " | " + conf.ItemSettings[i].DisplayName;
                    if (cmbnam.Items.IndexOf(nam) < 0) cmbnam.Items.Add(nam);
                }
            }

        }

        private void p4aBtnCryTo_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            TextBox txt = p4aTxtVal;
            if (btn.Name.Substring(0, 3) == "p4b") txt = p4bTxtVal;
            XControls.UI.ConfigGrid conf = p4Conf(sender);
            try
            {
                txt.Text = conf.ToCrypt(txt.Text);
            }
            catch(Exception es)
            {
                console.PrintLine("P4> 暗号[" + conf.Name + "] " + es.Message);
            }
        }

        private void p4aBtnCryFrom_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            TextBox txt = p4aTxtVal;
            if (btn.Name.Substring(0, 3) == "p4b") txt = p4bTxtVal;
            XControls.UI.ConfigGrid conf = p4Conf(sender);
            try
            {
                txt.Text = conf.FromCrypt(txt.Text);
            }
            catch (Exception es)
            {
                console.PrintLine("P4> 復号[" + conf.Name + "] " + es.Message);
            }
        }

        private void p4aBtnGet_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            ComboBox cmbc = p4aCmbCat;
            ComboBox cmbn = p4aCmbNam;
            TextBox txt = p4aTxtVal;
            XControls.UI.ConfigGrid conf = p4Conf(sender);
            if (btn.Name.Substring(0, 3) == "p4b")
            {
                cmbc = p4bCmbCat;
                cmbn = p4bCmbNam;
                txt = p4bTxtVal;
            }
            try
            {
                txt.Text = conf.GetConfigValue(cmbc.Text.Split(new char[] { ' ' })[0], cmbn.Text.Split(new char[] { ' ' })[0]);
            }
            catch(Exception es)
            {
                console.PrintLine("P4> GET[" + conf.Name + "] " + es.Message);
            }
        }

        private void p4aBtnPut_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            ComboBox cmbc = p4aCmbCat;
            ComboBox cmbn = p4aCmbNam;
            TextBox txt = p4aTxtVal;
            XControls.UI.ConfigGrid conf = p4Conf(sender);
            if (btn.Name.Substring(0, 3) == "p4b")
            {
                cmbc = p4bCmbCat;
                cmbn = p4bCmbNam;
                txt = p4bTxtVal;
            }
            try
            {
                conf.SetConfigValue(cmbc.Text.Split(new char[] { ' ' })[0], cmbn.Text.Split(new char[] { ' ' })[0], txt.Text);
            }
            catch (Exception es)
            {
                console.PrintLine("P4> SET[" + conf.Name + "] " + es.Message);
            }
        }

        private void p4aBtnGetTmp_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            ComboBox cmbn = p4aCmbNam;
            TextBox txt = p4aTxtVal;
            XControls.UI.ConfigGrid conf = p4Conf(sender);
            if (btn.Name.Substring(0, 3) == "p4b")
            {
                cmbn = p4bCmbNam;
                txt = p4bTxtVal;
            }
            try
            {
                txt.Text = conf.GetTempValue(cmbn.Text);
            }
            catch (Exception es)
            {
                console.PrintLine("P4> GETTEMP[" + conf.Name + "] " + es.Message);
            }
        }

        private void p4aBtnPutTmp_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            ComboBox cmbn = p4aCmbNam;
            TextBox txt = p4aTxtVal;
            XControls.UI.ConfigGrid conf = p4Conf(sender);
            if (btn.Name.Substring(0, 3) == "p4b")
            {
                cmbn = p4bCmbNam;
                txt = p4bTxtVal;
            }
            try
            {
                conf.SetTempValue(cmbn.Text, txt.Text);
            }
            catch (Exception es)
            {
                console.PrintLine("P4> SETTEMP[" + conf.Name + "] " + es.Message);
            }
        }

        private void p4aBtnLoad_Click(object sender, EventArgs e)
        {
            XControls.UI.ConfigGrid conf = p4Conf(sender);
            try
            {
                conf.LoadSetting();
            }
            catch (Exception es)
            {
                console.PrintLine("P4> LOAD[" + conf.Name + "] " + es.Message);
            }
        }

        private void p4aBtnSave_Click(object sender, EventArgs e)
        {
            XControls.UI.ConfigGrid conf = p4Conf(sender);
            try
            {
                conf.SaveSetting();
            }
            catch(Exception es)
            {
                console.PrintLine("P4> SAVE[" + conf.Name + "] " + es.Message);
            }
        }

        private void p4btnMerge_Click(object sender, EventArgs e)
        {
            p4conf2.ItemSettingMerge(p4conf1, false);
        }

        private void p4btnCopy_Click(object sender, EventArgs e)
        {
            p4conf2.ItemSettingMerge(p4conf1, true);
        }

        private void p4btnRefTo_Click(object sender, EventArgs e)
        {
            p4conf2.RefrectConfigValue(p4conf1, p4chkTDup.Checked);
        }

        private void p4btnRefFrom_Click(object sender, EventArgs e)
        {
            p4conf1.RefrectConfigValue(p4conf2, p4chkTDup.Checked);
        }

        private void p4btnGetAll_Click(object sender, EventArgs e)
        {
            XControls.UI.ConfigGrid conf = p4prop1.SelectedObject as XControls.UI.ConfigGrid;
            p4array = conf.GetDataAll();
            p4lstData.SelectedObject = p4array[1];
        }

        private void p4btnSetAll_Click(object sender, EventArgs e)
        {
            XControls.UI.ConfigGrid conf = p4prop1.SelectedObject as XControls.UI.ConfigGrid;
            try
            {
                conf.SetDataAll(p4array);
            }
            catch (Exception es)
            {
                console.PrintLine("P4> SETALL " + es.Message);
            }
        }

        private ArrayList p4array = new ArrayList();
        private void p4btnClr_Click(object sender, EventArgs e)
        {
            p4array = new ArrayList();
            p4lstData.SelectedObject = null;
        }

        #endregion

        #region PAGE5 - Command

        private bool p5consOpen = false;
        private void p5btnCons_Click(object sender, EventArgs e)
        {
            try
            {
                if (p5consOpen)
                {
                    p5cmd.CloseConsole();
                }
                else
                {
                    p5cmd.OpenConsole();
                }
                p5consOpen = !p5consOpen;
            }
            catch (Exception es)
            {
                console.PrintLine(es.ToString());
            }
        }

        private void p5btnExec_Click(object sender, EventArgs e)
        {
            int i;
            p5res = new ArrayList(p5txtRes.Lines);
            for (i = 0; i < p5res.Count; i++) p5res[i] = p5res[i] + "\r\n";

            try
            {
                if (p5txtParam.Text.Length == 0)
                {
                    p5cmd.Execute(p5txtCmd.Text);
                }
                else
                {
                    ArrayList param = new ArrayList(p5txtParam.Lines);
                    for (i = 0; i < param.Count; i++) param[i] = param[i] + "\r\n";
                    p5cmd.Execute(p5txtCmd.Text, param);
                }
            }
            catch(Exception es)
            {
                console.PrintLine(es.ToString());
            }
            MessageBox.Show("コマンド終了しました");
        }

        private ArrayList p5res = new ArrayList();
        void p5cmd_OutputValueEvent(object sender, XControls.Command.OutputValueEventArgs e)
        {
            // 応答
            if (p5res.Count > 0)
            {
                string[] inp = p5res[0].ToString().Split(new char[] { '|' });
                if (inp.Length > 1 && e.OutputText.IndexOf(inp[0]) >= 0)
                {
                    e.InputLines.Add(inp[1]);
                    p5res.RemoveAt(0);
                }
            }
        }

        void p5cmd_LoggingEvent(object sender, XControls.Command.LoggingEventArgs e)
        {
            console.PrintLine("P5> " + e.Type + "| " + e.Text);
        }

        #endregion

        #region PAGE6 - MessageBox / ScrollLabel

        private void p6btnShow_Click(object sender, EventArgs e)
        {
            DialogResult result;
            if(p6lstBtn.SelectedItem == null)
            {
                result = XControls.UI.MessageBox.Show(this, p6txtMsg.Text, p6txtTitle.Text);
            }
            else
            {
                MessageBoxButtons btn;
                Enum.TryParse(p6lstBtn.SelectedItem.ToString(), out btn);
                if (p6lstIcon.SelectedItem == null)
                {
                    result = XControls.UI.MessageBox.Show(this, p6txtMsg.Text, p6txtTitle.Text, btn);
                }
                else
                {
                    MessageBoxIcon icn;
                    Enum.TryParse(p6lstIcon.SelectedItem.ToString(), out icn);
                    if (p6lstDefault.SelectedItem == null)
                    {
                        result = XControls.UI.MessageBox.Show(this, p6txtMsg.Text, p6txtTitle.Text, btn, icn);
                    }
                    else
                    {
                        MessageBoxDefaultButton def;
                        Enum.TryParse(p6lstDefault.SelectedItem.ToString(), out def);
                        result = XControls.UI.MessageBox.Show(this, p6txtMsg.Text, p6txtTitle.Text, btn, icn, def);
                    }
                }
            }
            p6lblResult.Text = "結果: " + result.ToString();
        }
        
        #endregion

        #region PAGE7 - ExcelLink

        private void p7btnOpen_Click(object sender, EventArgs e)
        {
            if (p7app == null)
            {
                try
                {
                    p7app = p7excel.GetInstance();
                    p7propApp.SelectedObject = p7app;
                    p7propWorkbooks.SelectedObject = p7app.Workbooks;
                }
                catch (Exception es)
                {
                    console.PrintLine("P7> " + es.ToString());
                }
            }
            else
            {
                console.PrintLine("P7> ExcelはOpen済みです");
            }
        }

        private void p7btnClose_Click(object sender, EventArgs e)
        {
            if (p7app != null)
            {
                try
                {
                    p7excel.ReleaseInstance(ref p7app);
                    p7app = null;
                    p7propApp.SelectedObject = null;
                    p7propWorkbooks.SelectedObject = null;
                }
                catch (Exception es)
                {
                    console.PrintLine("P7> " + es.ToString());
                }
            }
            else
            {
                console.PrintLine("P7> ExcelはOpenしていません");
            }
        }

        private void p7btnBookOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "オープンするExcelブックを選択してください";
            dlg.Filter = "Excelファイル(*.xls;*.xlsx)|*.xls;*.xlsx|すべてのファイル(*.*)|*.*";
            dlg.CheckFileExists = false;
            dlg.CheckPathExists = false;
            dlg.Multiselect = false;
            dlg.FileName = "*.xls";
            dlg.ShowDialog();
            try
            {
                p7app.Workbooks.OpenBook(dlg.FileName);
            }
            catch (Exception es)
            {
                console.PrintLine("P7> " + es.ToString());
            }
        }

        private void p7btnBookNew_Click(object sender, EventArgs e)
        {
            p7app.Workbooks.NewBook();
        }

        private void p7btnBookGet_Click(object sender, EventArgs e)
        {
            if (p7book != null)
            {
                try
                {
                    p7app.Workbooks.ReleaseBookInstance(ref p7book);
                }
                catch (Exception es)
                {
                    console.PrintLine("P7> " + es.ToString());
                }
            }
            int idx = 1;
            int.TryParse(p7txtBookIdx.Text, out idx);
            p7txtBookIdx.Text = idx.ToString();
            try
            {
                p7book = p7app.Workbooks.GetBookInstance(idx);
                p7propBook.SelectedObject = p7book;
                p7propSheets.SelectedObject = p7book.Worksheets;
   //           txtXExcelMacBook.Text = p7book.Name;
            }
            catch (Exception es)
            {
                console.PrintLine("P7> " + es.ToString());
            }
        }

        private void p7btnBookRelease_Click(object sender, EventArgs e)
        {
            if (p7book != null)
            {
                try
                {
                    p7app.Workbooks.ReleaseBookInstance(ref p7book);
                    p7propBook.SelectedObject = null;
                    p7propSheets.SelectedObject = null;
                }
                catch (Exception es)
                {
                    console.PrintLine("P7> " + es.ToString());
                }
            }
            else
            {
                console.PrintLine("P7> WorkbookはRelease済です");
            }
        }

        private void p7btnBookSave_Click(object sender, EventArgs e)
        {
            if(p7book != null)
            {
                try
                {
                    p7book.Save();
                }
                catch (Exception es)
                {
                    console.PrintLine(es.ToString());
                }
            }
            else
            {
                console.PrintLine("P7> WorkbookはRelease済です");
            }
        }

        private void p7btnBookSaveAs_Click(object sender, EventArgs e)
        {
            if (p7book != null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Title = "保存するExcelブックを指定してください";
                dlg.DefaultExt = "xls";
                dlg.CheckFileExists = false;
                dlg.CheckPathExists = false;
                dlg.FileName = p7book.FullName;
                dlg.ShowDialog();
                try
                {
                    p7book.SaveAs(dlg.FileName);
//                    txtXExcelMacBook.Text = p7book.Name;
                }
                catch (Exception es)
                {
                    console.PrintLine(es.ToString());
                }
            }
            else
            {
                console.PrintLine("P7> WorkbookはRelease済です");
            }
        }

        private void p7btnBookClose_Click(object sender, EventArgs e)
        {
            if (p7book != null)
            {
                try
                {
                    p7book.Close();
//                    txtXExcelMacBook.Text = "";
                }
                catch (Exception es)
                {
                    console.PrintLine(es.ToString());
                }
            }
            else
            {
                console.PrintLine("P7> WorkbookはRelease済です");
            }
        }

        private void p7btnSheetGet_Click(object sender, EventArgs e)
        {
            if (p7sheet != null)
            {
                try
                {
                    p7book.Worksheets.ReleaseSheetInstance(ref p7sheet);
                }
                catch (Exception es)
                {
                    console.PrintLine("P7> " + es.ToString());
                }
            }
            int idx = 1;
            int.TryParse(p7txtSheetIdx.Text, out idx);
            p7txtSheetIdx.Text = idx.ToString();
            try
            {
                if (p7txtSheetNam.Text.Length > 0)
                {
                    p7sheet = p7book.Worksheets.GetSheetInstance(p7txtSheetNam.Text);
                }
                else
                {
                    p7sheet = p7book.Worksheets.GetSheetInstance(idx);
                }
                p7propSheet.SelectedObject = p7sheet;
                p7propCells.SelectedObject = p7sheet.Cells;
            }
            catch (Exception es)
            {
                console.PrintLine("P7> " + es.ToString());
            }

        }

        private void p7btnSheetRelease_Click(object sender, EventArgs e)
        {
            if (p7sheet != null)
            {
                try
                {
                    p7book.Worksheets.ReleaseSheetInstance(ref p7sheet);
                    p7propSheet.SelectedObject = null;
                    p7propCells.SelectedObject = null;
                }
                catch (Exception es)
                {
                    console.PrintLine("P7> " + es.ToString());
                }
            }
            else
            {
                console.PrintLine("P7> WorksheetはRelease済です");
            }
        }

        private void p7btnSheetNew_Click(object sender, EventArgs e)
        {
            if (p7book != null)
            {
                try
                {
                    p7book.Worksheets.NewSheet();
                }
                catch (Exception es)
                {
                    console.PrintLine("P7> " + es.ToString());
                }
            }
            else
            {
                console.PrintLine("P7> WorkbookはRelease済です");
            }
        }

        private void p7btnSheetDelete_Click(object sender, EventArgs e)
        {
            if (p7book != null)
            {
                try
                {
                    int idx = 1;
                    int.TryParse(p7txtFromIdx.Text, out idx);
                    p7txtFromIdx.Text = idx.ToString();
                    p7book.Worksheets.Delete(idx);
                }
                catch (Exception es)
                {
                    console.PrintLine("P7> " + es.ToString());
                }
            }
            else
            {
                console.PrintLine("P7> WorkbookはRelease済です");
            }
        }

        private void p7btnSheetCopy_Click(object sender, EventArgs e)
        {
            if (p7book != null)
            {
                try
                {
                    int idx = 1;
                    int.TryParse(p7txtFromIdx.Text, out idx);
                    p7txtFromIdx.Text = idx.ToString();
                    int idx2 = 0;
                    int.TryParse(p7txtToIdx.Text, out idx2);
                    p7txtToIdx.Text = idx2.ToString();
                    p7book.Worksheets.Copy(idx, idx2);
                }
                catch (Exception es)
                {
                    console.PrintLine("P7> " + es.ToString());
                }
            }
            else
            {
                console.PrintLine("P7> WorkbookはRelease済です");
            }
        }

        private void p7btnSheetMove_Click(object sender, EventArgs e)
        {
            if (p7book != null)
            {
                try
                {
                    int idx = 1;
                    int.TryParse(p7txtFromIdx.Text, out idx);
                    p7txtFromIdx.Text = idx.ToString();
                    int idx2 = 0;
                    int.TryParse(p7txtToIdx.Text, out idx2);
                    p7txtToIdx.Text = idx2.ToString();
                    p7book.Worksheets.Move(idx, idx2);
                }
                catch (Exception es)
                {
                    console.PrintLine("P7> " + es.ToString());
                }
            }
            else
            {
                console.PrintLine("P7> WorkbookはRelease済です");
            }
        }

        private void p7btnCell_Click(object sender, EventArgs e)
        {
            if (p7propCells.SelectedObject != null)
            {
                try
                {
                    int idx = 1;
                    int.TryParse(p7txtRow.Text, out idx);
                    p7txtRow.Text = idx.ToString();
                    int idx2 = 0;
                    int.TryParse(p7txtCol.Text, out idx2);
                    p7txtCol.Text = idx2.ToString();
                    XControls.ExcelLink.CellsClass cells = p7propCells.SelectedObject as XControls.ExcelLink.CellsClass;
                    p7propCell.SelectedObject = cells[idx, idx2];
                }
                catch (Exception es)
                {
                    console.PrintLine("P7> " + es.ToString());
                }
            }
            else
            {
                console.PrintLine("P7> CellsはRelease済です");
            }

        }

        #endregion

        #region PAGE8 - XX
        #endregion

        #region PAGE9 - TimerView
        
        private void p9tim_Enter(object sender, EventArgs e)
        {
            p9prop1.SelectedObject = sender;
        }

        private void p9btnStart_Click(object sender, EventArgs e)
        {
            ((XControls.UI.TimerView)p9prop1.SelectedObject).Start();
        }

        private void p9btnStop_Click(object sender, EventArgs e)
        {
            ((XControls.UI.TimerView)p9prop1.SelectedObject).Stop();
        }

        private void p9btnReset_Click(object sender, EventArgs e)
        {
            ((XControls.UI.TimerView)p9prop1.SelectedObject).Reset();
        }

        private void p9btnAdjM_Click(object sender, EventArgs e)
        {
            int n = 0;
            int.TryParse(p9txtM.Text, out n);
            ((XControls.UI.TimerView)p9prop1.SelectedObject).AdjustMinute(n);
        }

        private void p9btnAdjS_Click(object sender, EventArgs e)
        {
            int n = 0;
            int.TryParse(p9txtS.Text, out n);
            ((XControls.UI.TimerView)p9prop1.SelectedObject).AdjustSecound(n);
        }

        private void p9btnCalc_Click(object sender, EventArgs e)
        {
            double p1 = 0;
            double.TryParse(p9txtP1.Text, out p1);
            double p2 = 0;
            double.TryParse(p9txtP2.Text, out p2);
            p9txtAns.Text = ((XControls.UI.TimerView)p9prop1.SelectedObject).DiffTime(p1, p2).ToString("#0.00");
        }

        private void p9tim_TimerNotice(object sender, XControls.UI.TimerView.TimerNoticeEventArgs e)
        {
            XControls.UI.TimerView tv = (XControls.UI.TimerView)sender;
            console.PrintLine(tv.Name +
                        " 経過[" + e.ElapsedTime.ToString("#0.00").PadLeft(7) +
                        "] 残り[" + e.RemainingTime.ToString("#0.00").PadLeft(7) +
                        "] 開始[" + e.StartUp.ToString().PadRight(5) + 
                        "] 終了[" + e.TimeUp.ToString().PadRight(5) + "]");
        }

        #endregion

        #region PAGE10 - XX
        #endregion

        #region PAGE11 - XX
        #endregion

        #region PAGE12 - XX
        #endregion

        #region PAGE13 - XX
        #endregion

        #region PAGE14 - MixView

        
        
        #endregion

    }
}
