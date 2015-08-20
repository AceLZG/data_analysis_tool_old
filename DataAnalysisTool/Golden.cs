using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vanchip.Data;
using Vanchip.Common;

namespace DataAnalysisTool
{
    public partial class GoldenSample : Form
    {
        db mydb = new db();

        public GoldenSample()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void importGoldenDatacsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportGold importData = new ImportGold();
            importData.ShowDialog();
        }

        private void viewToleranceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //tvList.Nodes.Add
        }
    }
}
