using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UIFormARM
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // take the xlsx file as parameter
            string[] testt = new string[] { "C:\\coop\\project\\test_RITA220917_001.xlsx", "two1", "three1" };
            testt = args;

            // test if the file xlsx is available
            if (args.Length == 0)
            {
                string[] test = new string[] { "C:\\coop\\project\\test_RITA220917_001.xlsx", "two1", "three1" };
                testt = test;
            }

            // run ORM console
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(testt));



        }
    }
}
