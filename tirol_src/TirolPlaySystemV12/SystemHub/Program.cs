using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XControls
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (XControls.SystemHub.ProgramStartCheck())
            {
                // すでに実行中
                XControls.SystemHub.ProgramActivate();
            }
            else
            {
                // 起動
                Application.Run(new SettingsForm());
            }
        }
    }
}
