using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DataAnalysisTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            args = new string[2];
            Application.Run(new Main(args));
        }
    }
}
