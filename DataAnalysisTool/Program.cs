using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

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
            // Check for update
            ProcessStartInfo psi = new ProcessStartInfo("Update.exe");
            psi.Arguments = "--update";
            Process p_update = new Process();
            p_update.StartInfo = psi;
            p_update.Start();
            p_update.WaitForExit();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //args = new string[2];
            Application.Run(new frmMain(args));

        }
    }
}
