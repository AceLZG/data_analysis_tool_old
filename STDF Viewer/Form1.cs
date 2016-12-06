using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;


namespace STDF_Viewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd1 = new OpenFileDialog();
            ofd1.Filter = "STDF文件|*.std|所有文件|*.*";



            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                Record stdf = new Record();

                DateTime dt_start = DateTime.Now;

                if (Path.GetExtension(ofd1.FileName).ToLower() == ".std")
                {

                    using (FileStream fs = new FileStream(ofd1.FileName, FileMode.Open))
                    {
                        stdf.AnalyzeFile(fs);
                    }
                }
                else if(Path.GetExtension(ofd1.FileName).ToLower() == ".gz")
                {
                    using (FileStream fs = new FileStream(ofd1.FileName, FileMode.Open))
                    {
                        using (GZipStream gzs = new GZipStream(fs, CompressionMode.Decompress))
                        {
                            stdf.AnalyzeFile(gzs);
                            //using (MemoryStream OutputStream = new MemoryStream())
                            //{
                            //    gzs.CopyTo(OutputStream);
                            //    stdf.AnalyzeFile(OutputStream);
                               
                            //}

                        }
                    }
                }
        

                TimeSpan ts = DateTime.Now - dt_start;

                this.Text = ts.TotalMilliseconds.ToString();

                headerTable = stdf.GetHeaderStr();
                dataTable = stdf.GetData();
                dataGridView1.DataSource = headerTable;
                dataGridView2.DataSource = dataTable;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                for (int i = 0; i < 3; i++)
                {
                    dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                }

                for (int i = 0; i < 5; i++)
                {
                    dataGridView2.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }

                this.Text = (ts.TotalSeconds * 1000).ToString();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            CSVFileHelper.SaveCSV(headerTable, dataTable, "C:\\Users\\Ace\\Documents\\test.csv");
        }

        DataTable headerTable;
        DataTable dataTable;
    }
}
