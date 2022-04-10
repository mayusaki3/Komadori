using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace XControls
{
    public partial class MonitorForm : Form
    {
        public MonitorForm()
        {
            InitializeComponent();

            #region ラベル関連付け

            lbl[00] = lbl00;
            lbl[01] = lbl01;
            lbl[02] = lbl02;
            lbl[03] = lbl03;
            lbl[04] = lbl04;
            lbl[05] = lbl05;
            lbl[06] = lbl06;
            lbl[07] = lbl07;
            lbl[08] = lbl08;
            lbl[09] = lbl09;
            lbl[10] = lbl10;
            lbl[11] = lbl11;
            lbl[12] = lbl12;
            lbl[13] = lbl13;
            lbl[14] = lbl14;
            lbl[15] = lbl15;
            lbl[16] = lbl16;
            lbl[17] = lbl17;
            lbl[18] = lbl18;
            lbl[19] = lbl19;
            lbl[20] = lbl20;
            lbl[21] = lbl21;
            lbl[22] = lbl22;
            lbl[23] = lbl23;
            lbl[24] = lbl24;
            lbl[25] = lbl25;
            lbl[26] = lbl26;
            lbl[27] = lbl27;
            lbl[28] = lbl28;
            lbl[29] = lbl29;
            lbl[30] = lbl30;
            lbl[31] = lbl31;

            #endregion

            #region マップ関連付け

            map[00] = txtM00;
            map[01] = txtM01;
            map[02] = txtM02;
            map[03] = txtM03;
            map[04] = txtM04;
            map[05] = txtM05;
            map[06] = txtM06;
            map[07] = txtM07;
            map[08] = txtM08;
            map[09] = txtM09;
            map[10] = txtM10;
            map[11] = txtM11;
            map[12] = txtM12;
            map[13] = txtM13;
            map[14] = txtM14;
            map[15] = txtM15;
            map[16] = txtM16;
            map[17] = txtM17;
            map[18] = txtM18;
            map[19] = txtM19;
            map[20] = txtM20;
            map[21] = txtM21;
            map[22] = txtM22;
            map[23] = txtM23;
            map[24] = txtM24;
            map[25] = txtM25;
            map[26] = txtM26;
            map[27] = txtM27;
            map[28] = txtM28;
            map[29] = txtM29;
            map[30] = txtM30;
            map[31] = txtM31;

            #endregion

            #region キー関連付け

            key[00] = txtK00;
            key[01] = txtK01;
            key[02] = txtK02;
            key[03] = txtK03;
            key[04] = txtK04;
            key[05] = txtK05;
            key[06] = txtK06;
            key[07] = txtK07;
            key[08] = txtK08;
            key[09] = txtK09;
            key[10] = txtK10;
            key[11] = txtK11;
            key[12] = txtK12;
            key[13] = txtK13;
            key[14] = txtK14;
            key[15] = txtK15;
            key[16] = txtK16;
            key[17] = txtK17;
            key[18] = txtK18;
            key[19] = txtK19;
            key[20] = txtK20;
            key[21] = txtK21;
            key[22] = txtK22;
            key[23] = txtK23;
            key[24] = txtK24;
            key[25] = txtK25;
            key[26] = txtK26;
            key[27] = txtK27;
            key[28] = txtK28;
            key[29] = txtK29;
            key[30] = txtK30;
            key[31] = txtK31;

            key[32] = txtK32;
            key[33] = txtK33;
            key[34] = txtK34;
            key[35] = txtK35;

            key[36] = txtK36;
            key[37] = txtK37;
            key[38] = txtK38;
            key[39] = txtK39;

            #endregion
        }

        public XJoyStickControl joy = null;
        public Label[] lbl = new Label[32];
        public TextBox[] map = new TextBox[32];
        public TextBox[] key = new TextBox[40];
        private XJoyStickControl.KeyMapList kmap = new XJoyStickControl.KeyMapList();

        private void MonitorForm_Deactivate(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void txtM_Validating(object sender, CancelEventArgs e)
        {
            TextBox map = (TextBox)sender;
            int n;
            if (int.TryParse(map.Text, out n) == false)
            {
                e.Cancel = true;
                return;
            }
            if (n < 1 || n > 32)
            {
                e.Cancel = true;
                return;
            }
            map.Text = n.ToString("00");
        }

        private void txtM_Validated(object sender, EventArgs e)
        {
            int i;
            string b = "";
            for (i = 0; i < 32; i++)
            {
                b += map[i].Text;
            }
            joy.ButtonMapping = b;
        }

        private void txtK_Validating(object sender, CancelEventArgs e)
        {
            TextBox key = (TextBox)sender;
            string ks = key.Text.ToUpper().Trim();
            if (ks.Length > 0 && kmap.FindKey(ks) == false)
            {
                e.Cancel = true;
            }
            else
            {
                key.Text = ks;
            }
        }

        private void txtK_Validated(object sender, EventArgs e)
        {
            int i;
            string b = "";
            for (i = 0; i < 40; i++)
            {
                b += (key[i].Text.ToUpper() + "  ").Substring(0, 2);
            }
            joy.KeyMapping = b;
        }

        private void MonitorForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                lblID.Focus();
                e.Handled = true;
            }
        }
    }
}
