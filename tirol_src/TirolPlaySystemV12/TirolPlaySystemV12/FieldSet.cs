using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TirolPlaySystem
{
    public class FieldSet : XControls.UI.FieldView
    {
        public FieldSet()
        {
            base.GridCountX = 4;
            base.GridCountY = 4;
        }

        private int gameNumber = 1;
        public int GameNumber
        {
            get
            {
                return gameNumber;
            }
            set
            {
                int w = value;
                if (w < 1) w = 1;
                if (w > 3) w = 3;
                bool r = (w != gameNumber);
                gameNumber = w;
                if (r) Reset();
            }
        }

        private int dist = 0;
        public int Distance
        {
            get
            {
                if (GameNumber != 1) return 0;
                return dist;
            }
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs mevent)
        {
            if (gameNumber != 1)
            {
                base.OnMouseDown(mevent);
            }
            else
            {
                // 変化した場所を検索
                int i, j;
                string[] pval = new string[GridOn.Length];
                for (i = 0; i < pval.Length; i++)
                {
                    pval[i] = GridOn[i];
                }
                base.OnMouseDown(mevent);
                Refresh();
                dist = 0;
                for (i = 0; i < pval.Length; i++)
                {
                    string[] tx = GridText[i].Split(new char[] { ',' });
                    for(j=0;j<tx.Length;j++)
                    {
                        if(!pval[i].Substring(j,1).Equals(GridOn[i].Substring(j,1)))
                        {
                            int.TryParse(tx[j], out dist);
                            break;
                        }
                    }
                }

                // 指定位置までをONにする
                string [] non = new string[GridOn.Length];
                for (i = 0; i < GridOn.Length; i++)
                {
                    string[] tx = GridText[i].Split(new char[] { ',' });
                    for (j = 0; j < tx.Length; j++)
                    {
                        int no = 0;
                        int.TryParse(tx[j], out no);
                        if (no <= dist)
                        {
                            non[i] += "1";
                        }
                        else
                        {
                            non[i] += "0";
                        }
                    }
                }
                GridOn = non;
                base.OnChangeValue(this, new EventArgs());                
            }
        }

        public void Reset()
        {
            switch (gameNumber)
            {
                case 1:
                    Enabled = true;
                    WallSetting = "++++|+   +| ++ |++ ++|  + |++  +| +++|+".Split(new char[] { '|' });
                    GridText = "6,7,8,9|5,14,15,10|4,13,12,11|3,2,1,0".Split(new char[] { '|' });
                    GridOn = "|||0001".Split(new char[] { '|' });
                    break;
                case 2:
                    Enabled = false;
                    WallSetting = "   +|    +||    +||    +||   +|  +".Split(new char[] { '|' });
                    GridText = new string[0];
                    GridOn = new string[0];
                    break;
                case 3:
                    Enabled = true;
                    WallSetting = "++++|+   +|+   |+   +||+   +||+   +|++++".Split(new char[] { '|' });
                    GridText = ",1,2,3|3,2,3,4|4,3,4,5|5,4,5,6".Split(new char[] { '|' });
                    GridOn = "0111|1111|1111|1111".Split(new char[] { '|' });
                    break;
            }
        }
    }
}
