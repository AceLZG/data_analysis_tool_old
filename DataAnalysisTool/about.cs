using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DataAnalysisTool
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            string w_file = "DataAnalysisTool.exe";
            string w_directory = Directory.GetCurrentDirectory();

            DateTime c3 = File.GetLastWriteTime(System.IO.Path.Combine(w_directory, w_file));

            label1.Text = "Product: " + this.ProductName + "\r\n \r\n" +
                            "Version: " + this.ProductVersion + "\r\n   \r\n" +
                            "Created: " + c3.ToString();
            
        }
    }
}
