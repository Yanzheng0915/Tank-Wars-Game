//Author: Yanzheng Wu and Qingwen Bao
//University of Utah
//Date: 2021/04/09
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TankWars
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GameController c = new GameController();
            Form1 tankWars = new Form1(c);
            tankWars.AutoSize = true;
            Application.Run(tankWars);
        }
    }
}
