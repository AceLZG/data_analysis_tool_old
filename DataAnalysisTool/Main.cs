///     Reversion history log
///     Rev1.0          Initie build                                                                Ace Li      2011-10-21
///     Rev1.1          Add dual site data analysis                                                 Ace Li      2012-02-25
///     Rev1.2          Update dual site KGU analysis and JMP analysis                              Ace Li      2012-03-06
///     Rev1.3          Add JMP script build function
///                     support test time and index time caculation based on the unit               Ace Li      2012-07-04
///     Rev1.3.1        Add support to Advantest test data                                          Ace Li      2014-06-19
///     Rev2.1.0.0      upgrade stdf lib                                                            Ace Li      2016-03-31

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
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using LumenWorks.Framework.IO.Csv;
//using System.Windows.Forms.DataVisualization.Charting;
//using System.Windows.Forms.DataVisualization.Charting.Utilities;
using Microsoft.VisualBasic;

namespace DataAnalysisTool
{
    public partial class Main : Form
    {
        #region *** Variable declare ***
        OpenFileDialog OpenFile;

        Util _Util = new Util();
        DataParse _DataParse = new DataParse();
        Export _Export = new Export();
        Analysis _Analysis = new Analysis();

        DataTable tblData = new DataTable();
        DataTable tblCachedData = new DataTable();
        //DataTable tblFailureModeData = new DataTable();
        //DataTable tblFailureRateData = new DataTable();
        DataTable tblKGU = new DataTable();
        DataTable tblHeader = new DataTable();
        DataTable tblCpk = new DataTable();

        //private System.Windows.Forms.TabControl JMPStarterTabControl;
        private JMP.Application myJMP;
        private string jmpInstallDir;
        private string jmpSampleDataDir;

        //private JMP.Document fmDoc;
        //private JMP.FitModel fm;
        //private JMP.FitLeastSquares fls;
        //private JMP.FitResponse resp;
        
        string strFailureMode = null;
        string strFailureRate = null;
        //string[] arrayFailure = new string[30000];


        string[] Parameter;

        static string strTreeViewTitle = "--- Session List ---";

        static string strTestDataTabName = "Test Data";
        static string strFailureModeTabName = "Failure Mode";
        static string strFailureRateTabName = "Failure Rate";
        static string strDistributionTabName = "Distribution";
        static string strKGUTabName = "KGU";
        static string strCpkTabName = "Cpk_Limit";

        static int TestDeviceColumnIndex = 13;      // !!!important .  used for data add and delete and analysis

        #endregion *** Variable declare ***


        #region  *** Initialize ***
        public Main()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Normal;
            SplitContainer.FixedPanel = FixedPanel.Panel1;
            SplitContainer.SplitterDistance = 300;
            //HScrollBar hs = new HScrollBar();
            //tvDataList.Controls.Remove(hs);
            lblBar.Text = "";
            this.Refresh();
            //Initialize tblHeader
            tblHeader.Columns.Add("Name", typeof(string));
            tblHeader.Columns.Add("Value", typeof(string));
        }
        
        #endregion  *** Initialize ***


        #region *** Sub Functions ***
        // *** Re-Caculate Cpk Limit Impact
        void btnReCaculate_Click(object sender, EventArgs e)
        {
            DataGridView dgvCpk = (DataGridView)FindControl(SplitContainer.Panel2, "dgvCpk");
            Label lblYieldImpact = (Label)FindControl(SplitContainer.Panel2, "YieldImpact");
            if (dgvCpk == null || lblYieldImpact == null) return;

            //bool isLimitUpdated = false;
            lblBar.Text = "ReCaculating the Cpk and yield impact based on new limit";
            this.Refresh();
            Application.DoEvents();
            //Cpk Caculate
            int ParameterQty = tblCpk.Rows.Count;
            double[] LSL = new double[ParameterQty];
            double[] USL = new double[ParameterQty];

            for (int i = 0; i < ParameterQty; i++)
            {
                LSL[i] = Convert.ToDouble(tblCpk.Rows[i][3]);
                USL[i] = Convert.ToDouble(tblCpk.Rows[i][4]);
            }
            tblCpk = _Analysis.CaculateCpk(tblData, _DataParse.FreezeColumn,LSL, USL);
            dgvCpk.DataSource = tblCpk;

            //Yield Impact Analysis
            double dblYield;
            double dblYieldImpact;

            dblYield = _Analysis.CaculateYieldImpact(tblData,_DataParse.FreezeColumn,LSL, USL);
            dblYieldImpact = dblYield - Convert.ToDouble(_DataParse.PassedDevice) / Convert.ToDouble(_DataParse.TestedDevice);

            lblYieldImpact.Text = "Yield Impact = " + Math.Round(dblYieldImpact * 100, 2).ToString() +
                                        "%, New Yield = " + Math.Round(dblYield * 100, 2).ToString() + "%";

            lblBar.Text = "Done";
            this.Refresh();
        }

        // *** SplitContainer Size Change
        void SplitContainer_SizeChanged(object sender, System.EventArgs e)
        {
            DataGridView dgvCpk = (DataGridView)FindControl(SplitContainer.Panel2, "dgvCpk");
            if (dgvCpk == null) return;

            dgvCpk.Height = SplitContainer.Panel2.Height - 65;
        }

        void tvDataList_MouseLeave(object sender, System.EventArgs e)
        {
            this.tvDataList.ContextMenuStrip = null;
        }

        void tabcontrol_MouseLeave(object sender, System.EventArgs e)
        {
            this.tabcontrol.ContextMenuStrip = null;
        }

        // *** FindControl
        private Control FindControl(Control control, string ControlName)
        {
            foreach (Control c in control.Controls)
            {
                if (c.Name == ControlName)
                {
                    return c;
                }
                else if (c.Controls.Count > 0)
                {
                    Control c1 = FindControl(c, ControlName);
                    if (c1 != null)
                    {
                        return c1;
                    }
                }
            }
            return null;
        }

        // *** dgvData GridViewFormat
        private void dgvDataGridViewFormat()
        {
            float currentSize = dgvData.Font.SizeInPoints - 1;
            try
            {
                for (int i = 0; i < dgvData.Columns.Count; i++)
                {
                    dgvData.Columns[i].Width = 70;
                    dgvData.Columns[i].ReadOnly = true;
                    dgvData.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvData.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgvData.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    dgvData.EnableHeadersVisualStyles = true;
                    dgvData.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                    DataGridViewCellStyle style = dgvData.ColumnHeadersDefaultCellStyle;
                    style.Font = new Font(dgvData.Font, FontStyle.Bold);
                    style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                for (int i = 0; i < 4; i++)
                {
                    dgvData.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                    dgvData.Rows[i].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", currentSize, FontStyle.Bold);
                    dgvData.Rows[i].Frozen = true;
                }
                for (int i = 0; i < _DataParse.FreezeColumn; i++)
                {
                    dgvData.Columns[i].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", currentSize, FontStyle.Bold);
                    dgvData.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    dgvData.Columns[i].Width = 45;
                    dgvData.Columns[i].Frozen = true;
                }
                dgvData.Columns[0].Width = 80;
                dgvData.Columns[_DataParse.FreezeColumn - 1].Width = 65;
                dgvData.RowHeadersVisible = false;
                dgvData.ReadOnly = true;
                dgvData.AllowUserToAddRows = false;
                dgvData.AllowUserToResizeRows = false;
            }
            catch (Exception ex)
            {
                //throw new Exception("Format DataGrid Error" + ex.Message);
            }
        }

        // *** dgvData Cell formatting & get failure detail ***
        private void dgvData_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex > 3 && e.ColumnIndex >= _DataParse.FreezeColumn && e.ColumnIndex != dgvData.Columns.Count - 1)
            {
                if (e.Value == DBNull.Value)    //if DBNull
                    e.CellStyle.BackColor = Color.WhiteSmoke;
                else
                {
                    try
                    {
                        double dblCellValue = Convert.ToDouble(e.Value);
                        // format as #.000
                        dblCellValue = Convert.ToInt32(dblCellValue * 1000);
                        dblCellValue = dblCellValue / 1000;
                        e.Value = dblCellValue;

                        double dblLowLimit = Convert.ToDouble(dgvData.Rows[2].Cells[e.ColumnIndex].Value);
                        double dblHighLimit = Convert.ToDouble(dgvData.Rows[3].Cells[e.ColumnIndex].Value);

                        if (dblCellValue < dblLowLimit || dblCellValue > dblHighLimit)
                        {
                            e.CellStyle.BackColor = Color.Red;
                        }
                    }
                    catch
                    {
                        e.CellStyle.BackColor = Color.Red;
                    }
                }// end of if DBNull
            }
        }// end of gvData_CellFormatting

        // *** dgvCpk GridViewFormat
        private void dgvCpkGridViewFormat()
        {
            DataGridView dgvCpk = (DataGridView)FindControl(SplitContainer.Panel2, "dgvCpk");
            // Skip the data format if dgvCpk is not exist
            if (dgvCpk == null) return;

            float currentSize = dgvCpk.Font.SizeInPoints - 1;

            for (int i = 0; i < dgvCpk.Columns.Count; i++)
            {
                dgvCpk.Columns[i].Width = 80;
                dgvCpk.Columns[i].ReadOnly = true;
                dgvCpk.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvCpk.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvCpk.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dgvCpk.EnableHeadersVisualStyles = true;
                dgvCpk.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                DataGridViewCellStyle style = dgvCpk.ColumnHeadersDefaultCellStyle;
                style.Font = new Font(dgvCpk.Font, FontStyle.Bold);
                style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            for (int i = 0; i < 5; i++)
            {
                dgvCpk.Columns[i].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", currentSize, FontStyle.Bold);
                dgvCpk.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
            }
            dgvCpk.Columns[0].Width = 40;
            dgvCpk.Columns[1].Width = 192;
            dgvCpk.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvCpk.RowHeadersVisible = false;
            dgvCpk.Rows[3].Frozen = false;
            dgvCpk.Columns[3].ReadOnly = false;
            dgvCpk.Columns[4].ReadOnly = false;
            dgvCpk.Columns[4].Frozen = true;
            //dgvCpk.ReadOnly = false;
            dgvCpk.AllowUserToAddRows = false;
            dgvCpk.AllowUserToResizeRows = false;

        }        

        // *** dgvCpk Cell formatting ***
        void dgvCpk_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgvCpk = (DataGridView)FindControl(SplitContainer.Panel2, "dgvCpk");
            if (dgvCpk == null) return;

            if (e.ColumnIndex == 3 || e.ColumnIndex == 4)
            {
                // Compare limit change
                if (Convert.ToDouble(e.Value) != Convert.ToDouble(tblData.Rows[e.ColumnIndex - 1][e.RowIndex + _DataParse.FreezeColumn]))
                {
                    e.CellStyle.BackColor = Color.Yellow;
                }
            }
        }

        // *** dgvCpk_CellValueChanged ***
        void dgvCpk_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //DataGridView dgvCpk = (DataGridView)FindControl(SplitContainer.Panel2, "dgvCpk");
            //Label lblYieldImpact = (Label)FindControl(SplitContainer.Panel2, "YieldImpact");
            //if (dgvCpk == null || lblYieldImpact == null) return;

            //int ParameterQty = tblCpk.Rows.Count;
            //double[] LSL = new double[ParameterQty];
            //double[] USL = new double[ParameterQty];

            //for (int i = 0; i < ParameterQty; i++)
            //{
            //    LSL[i] = Convert.ToDouble(tblCpk.Rows[i][3]);
            //    USL[i] = Convert.ToDouble(tblCpk.Rows[i][4]);
            //}
            //tblCpk = _Analysis.CaculateCpk(tblData, LSL, USL);
            //dgvCpk.DataSource = tblCpk;

        }

        // *** SplitContainer.panel1 resize        
        private void Panel1_Resize(object sender, System.EventArgs e)
        {
            DataGridView dgvHeader = (DataGridView)this.FindControl(groupBox1, "dgvHeader");
            if (dgvHeader != null)
                dgvHeader.Columns[1].Width = SplitContainer.Panel1.Width - 100;
        }

        // *** Selected node when move mouse over it ***
        private void tvDataList_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            TreeNode CurrentNode = tvDataList.GetNodeAt(e.X, e.Y);
            int intNodeIndex = tvDataList.Nodes.Count;
            if (CurrentNode != null)
            {
                if (CurrentNode.Text != strTreeViewTitle)
                {
                    tvDataList.SelectedNode = CurrentNode;
                }
                else if (CurrentNode.Text == strTreeViewTitle)
                {
                    tvDataList.SelectedNode = tvDataList.Nodes[1];
                }
            }
        }

        // *** Update Grid when datalist change
        private void tvDataListUpdateData(string safeFileName)
        {
            string TempFolderPath = null;
            string fileName = null;
            string[] arrayName;

            if (safeFileName == strTreeViewTitle) return;
            this.tvDataList.ContextMenuStrip = null;
            dgvData.DataSource = null;
            try
            {
                TempFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\";
                arrayName = safeFileName.Split('}');
                fileName = TempFolderPath + arrayName[1].Trim() + ".csv";

                tblData = _DataParse.GetDataFromCsv(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            this.UpdateSessionInfomation();
            this.updateGrid(tblData);
            //Cache data to tblCacheData
            tblCachedData = tblData.Clone();
            foreach (DataRow dr in tblData.Rows)
            {
                tblCachedData.ImportRow(dr);
            }
            //this.CacheData();
            this.RemoveAllTab();
        }

        // *** Update DataGridView ***
        private void updateGrid(DataTable dataTable)
        {
            dgvData.DataSource = null;
            dgvData.DataSource = dataTable;
            this.dgvDataGridViewFormat();
        }

        // *** Update Header information ***
        private void UpdateSessionInfomation()
        {
            //Clear tabHeader
            tblHeader.Clear();
            
            //int strNameLength = 20;
            bool isHeadNull = false;
            var type = typeof(DataHeader);
            var fields = type.GetFields();
            //Array.ForEach(fields, f =>
            foreach (_FieldInfo fi in fields)
            {
                string name = fi.Name;
                DataRow dr = tblHeader.NewRow();
                dr["Name"] = fi.Name;
                //check if header null
                if (name == "Product")
                {
                    if (fi.GetValue(_DataParse.Header) == null)
                    {
                        isHeadNull = true;
                    }
                }
                //if header null, use Test quantity to caculate yield
                if (isHeadNull)
                {
                    if (name == "TestQuantity")
                    {
                        dr["Value"] = _DataParse.TestedDevice;
                    }
                    else if (name == "PassQuantity")
                    {
                        dr["Value"] = _DataParse.PassedDevice;
                    }
                    else if (name == "FailQuantity")
                    {
                        dr["Value"] = _DataParse.FailedDevice;
                    }
                    else if (name == "Yield")
                    {
                        double pass = Convert.ToDouble(_DataParse.PassedDevice);
                        double total = Convert.ToDouble(_DataParse.TestedDevice);
                        dr["Value"] = Math.Round(pass / total * 100, 3) + "%";
                    }
                }
                    //if header not null, use header info
                else
                {
                    if (name == "Yield")
                    {
                        dr["Value"] = fi.GetValue(_DataParse.Header) + "%";
                    }
                    else
                    {
                        dr["Value"] = fi.GetValue(_DataParse.Header);
                    }
                }
                tblHeader.Rows.Add(dr);
            }
            DataGridView dgvHeader = new DataGridView();
            dgvHeader.Name = "dgvHeader";
            groupBox1.Controls.Add(dgvHeader);
            dgvHeader.Dock = DockStyle.Fill;

            dgvHeader.DataSource = tblHeader;
            //format dgvHeader
            dgvHeader.ReadOnly = true;
            dgvHeader.AllowUserToAddRows = false;
            dgvHeader.AllowUserToResizeColumns = false;
            dgvHeader.AllowUserToResizeRows = false;
            dgvHeader.RowHeadersVisible = false;
            dgvHeader.ColumnHeadersVisible = false;
            dgvHeader.BorderStyle = BorderStyle.None;
            dgvHeader.ScrollBars = ScrollBars.Vertical;


            dgvHeader.Columns[0].Width = 100;
            dgvHeader.Columns[1].Width = SplitContainer.Panel1.Width - 100;
            dgvHeader.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;
            dgvHeader.Columns[1].DefaultCellStyle.BackColor = Color.LightGray;


            //Dispaly Parse time
            lblBar.Text = Math.Round(Convert.ToDouble(_DataParse.ParseTime), 2) + "ms";
                                               //+ Math.Round(Convert.ToDouble(_DataParse.InsertTime), 2) + "ms";
            this.Refresh();
        }

        // *** Cache Data ***
        private void CacheData(string suffix)
        {
            #region *** Cache data ***
            if (tvDataList.Nodes.Count == 0)
                tvDataList.Nodes.Add(strTreeViewTitle);

            //Save datatable to Temp folder
            bool isHeadNull = false;
            if (_DataParse.Header.Product == null)
            {
                isHeadNull = true;
            }
            string cachedSafeFileName = "";

            if (!isHeadNull)
            {
                if (_DataParse.Header.LotID.ToString() != null)
                {
                    cachedSafeFileName += _DataParse.Header.LotID.ToString() + "_";
                }

                if (_DataParse.Header.SubLotID.ToString() != null)
                {
                    cachedSafeFileName += _DataParse.Header.SubLotID.ToString() + "_";
                }

                if (_DataParse.Header.TestSession.ToString() != null)
                {
                    cachedSafeFileName += _DataParse.Header.TestSession.ToString() + "_";
                }

            }
            // add datetime sufix for unique filename
            cachedSafeFileName += DateTime.Now.ToString();
            cachedSafeFileName = cachedSafeFileName.Replace("/", "");
            cachedSafeFileName = cachedSafeFileName.Replace(":", "");
            cachedSafeFileName = cachedSafeFileName.Replace(" ", "");

            if (suffix != "") cachedSafeFileName = cachedSafeFileName + "_" + suffix;
            string TempFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\";
            string tempFileName = TempFolderPath + cachedSafeFileName + ".csv";
            //if temp folder is nor exist, creat it
            if (Directory.Exists(TempFolderPath) == false)
                Directory.CreateDirectory(TempFolderPath);

            _Export.DataTableToCsv(tempFileName, tblData, _DataParse.Header);

            //Cache data to tblCacheData
            tblCachedData = tblData.Clone();
            foreach (DataRow dr in tblData.Rows)
            {
                tblCachedData.ImportRow(dr);
            }

            if (isHeadNull)
            {
                tvDataList.Nodes.Add("{Unknow} " + cachedSafeFileName);
                //tvDataList.Nodes.Insert(1, "{Unknow} " + cachedSafeFileName);
            }
            else
            {
                tvDataList.Nodes.Add("{" + _DataParse.Header.Product + "} " + cachedSafeFileName);
                //tvDataList.Nodes.Insert(1, "{" + _DataParse.Header.Product + "} " + cachedSafeFileName);
            }
            #endregion *** Cache data ***
        }

        // *** Clear cached data ***
        private void DelCachedData()
        {
            int fileCount = 0;
            string tmpDirectory  = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\";
            FileInfo[] FI = _Util.GetFileInfoArray(tmpDirectory, "csv");
            fileCount = _Util.DeleteFiles(FI);
            try
            {
                myJMP.CloseAllWindows();
                _Util.Wait(300);
            }
            catch
            {
                _Util.Wait(300);
            }
        }
        private void deleteCacheDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DelCachedData();
        }
        void Main_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            this.DelCachedData();            
        }

        // *** Exit ***
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // *** Tab Remove ***
        private void RemoveTab(string tabText)
        {
            foreach (TabPage tab in tabcontrol.TabPages)
            {
                if (tabText != strTestDataTabName && tab.Text == tabText)
                {
                    tab.Dispose();
                    tabcontrol.Controls.Remove(tab);
                }
            }
        }

        // *** Remove ll tabs except test data ***
        private void RemoveAllTab()
        {
            foreach (TabPage tab in tabcontrol.TabPages)
            {
                if (tab.Text != strTestDataTabName && tab.Text != strKGUTabName)
                {
                    tab.Dispose();
                    tabcontrol.Controls.Remove(tab);
                }
            }
        }

        // Close JMP
        private void closeJMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuitJMP();
        }

        // Quit JMP
        private void QuitJMP()
        {
            try
            {
                myJMP.Quit();
                _Util.Wait(300);
            }
            catch
            {
                _Util.Wait(300);
            }
        }

        #region *** Hide Hori Scroll Bars ***
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int ShowScrollBar(IntPtr hWnd, int bar, int show);


        private class SubWindow : NativeWindow
        {
            private int m_Horz = 0;
            private int m_Show = 0;

            public SubWindow(int p_Horz, int p_Show)
            {
                m_Horz = p_Horz;
                m_Show = p_Show;
            }
            protected override void WndProc(ref Message m_Msg)
            {
                ShowScrollBar(m_Msg.HWnd, m_Horz, m_Show);
                base.WndProc(ref m_Msg);
            }
        }

        /// <summary>   
        /// 设置滚动条是否显示  zgke@sina.com qq:116149   
        /// </summary>   
        /// <param name="p_ControlHandle">句柄</param>   
        /// <param name="p_Horz">0横 1列 3全部</param>   
        /// <param name="p_Show">0隐 1显</param>   
        public static void SetScrollBar(IntPtr p_ControlHandle, int p_Horz, int p_Show)
        {
            SubWindow _SubWindow = new SubWindow(p_Horz, p_Show);
            _SubWindow.AssignHandle(p_ControlHandle);
        }

        #endregion *** Hide Hori Scroll Bars ***

        #region *** Tab Close ***
        //private void tabcontrol_DrawItem(object sender, DrawItemEventArgs e)
        //        {
        //            try
        //            {
        //                Rectangle myTabRect = this.tabcontrol.GetTabRect(e.Index);
        //                int CLOSE_SIZE = 10;
        //                //先添加TabPage属性  
        //                e.Graphics.DrawString(this.tabcontrol.TabPages[e.Index].Text
        //                , this.Font, SystemBrushes.Highlight, myTabRect.X + 2, myTabRect.Y + 2);

        //                //再画一个矩形框
        //                using (Pen p = new Pen(Color.Black))//自动释放资源
        //                {
        //                    myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
        //                    myTabRect.Width = CLOSE_SIZE;
        //                    myTabRect.Height = CLOSE_SIZE;
        //                    e.Graphics.DrawRectangle(p, myTabRect);
        //                }

        //                //画关闭符号
        //                using (Pen objpen = new Pen(Color.Black))
        //                {
        //                    //"/"线
        //                    Point p1 = new Point(myTabRect.X + 3, myTabRect.Y + 3);
        //                    Point p2 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + myTabRect.Height - 3);
        //                    e.Graphics.DrawLine(objpen, p1, p2);

        //                    //"/"线
        //                    Point p3 = new Point(myTabRect.X + 3, myTabRect.Y + myTabRect.Height - 3);
        //                    Point p4 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + 3);
        //                    e.Graphics.DrawLine(objpen, p3, p4);
        //                }

        //                e.Graphics.Dispose();
        //            }
        //            catch (Exception)
        //            {
        //            }
        //        }

        private void tabcontrol_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = e.X, y = e.Y;

                int CLOSE_SIZE = 10;

                //计算关闭区域  
                Rectangle myTabRect = this.tabcontrol.GetTabRect(this.tabcontrol.SelectedIndex);

                myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                myTabRect.Width = CLOSE_SIZE;
                myTabRect.Height = CLOSE_SIZE;

                //如果鼠标在区域内就关闭选项卡  
                bool isClose = x > myTabRect.X && x < myTabRect.Right && y > myTabRect.Y && y < myTabRect.Bottom;

                if (isClose == true && this.tabcontrol.SelectedTab.Text != strTestDataTabName)
                {
                    //this.tabcontrol.TabPages.Remove(this.tabcontrol.SelectedTab);
                }
            }
            //实现右键选中选项卡 
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < tabcontrol.TabPages.Count; i++)
                {
                    TabPage tp = tabcontrol.TabPages[i];
                    if (tabcontrol.GetTabRect(i).Contains(new Point(e.X, e.Y)))
                    {
                        tabcontrol.SelectedTab = tp;
                        break;
                    }
                }
                //右键选中选项卡
                this.tabcontrol.ContextMenuStrip = this.TabUserMenu;  //弹出菜单
                if (this.tabcontrol.SelectedTab.Text == strTestDataTabName)
                {
                    this.tabcontrol.ContextMenuStrip = null;
                }
            }
        }

        private void tabcontrol_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.RemoveTab(this.tabcontrol.SelectedTab.Text);
            }
        }

        private void CloseTab_Click(object sender, EventArgs e)
        {
            this.RemoveTab(this.tabcontrol.SelectedTab.Text);
        }

        private void CloseAllTab_Click(object sender, EventArgs e)
        {
            this.RemoveAllTab();
        }

        #endregion *** Tab Close ***

        #region *** Remove Data List Node ***
        private void RemoveDataListNode_Click(object sender, EventArgs e)
        {
            if (tvDataList.SelectedNode != null)
            {
                //int nodeIndex = tvDataList.Nodes.IndexOf(tvDataList.SelectedNode);
                tvDataList.SelectedNode.Remove();
                //tvDataList.SelectedNode = tvDataList.Nodes[nodeIndex];                
            }
            if (tvDataList.Nodes.Count == 1)
            {
                tvDataList.Nodes.Clear();
                tblData.Clear();
                dgvData.Columns.Clear();
            }
            else
            {
                string safefileName = tvDataList.SelectedNode.Text;
                this.tvDataListUpdateData(safefileName);
            }
        }

        private void RemoveAllDataListNode_Click(object sender, EventArgs e)
        {
            tvDataList.Nodes.Clear();
            tblData.Clear();
            dgvData.Columns.Clear();
        }

        #endregion *** Remove Data List Node ***


        // *** About
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout about = new frmAbout();
            about.ShowDialog();
        }
        
        #endregion *** Sub Functions ***


        #region *** Import Data ***
        // *** Open file ***
        private void openToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            DateTime dtStart = DateTime.Now;
            TimeSpan ts;

            #region *** Variable define ***
            string strExtension = "";
            string[] strFileName;

            #endregion *** Variable define ***

            #region *** Selected file ***
            OpenFile = new OpenFileDialog();
            OpenFile.RestoreDirectory = true;
            OpenFile.Multiselect = true;
            OpenFile.Filter = "STDF Files(*.std/*.stdf)|*.std;*.stdf|TXT data file(*.txt)|*.txt|Acetech data file(*.csv)|*.csv|All Files|*.*";
            //OpenFile.Filter = "STDF data file(*.std)|*.std|TXT data file(*.txt)|*.txt";
            OpenFile.ReadOnlyChecked = true;

            OpenFile.FileOk += new CancelEventHandler(OpenFile_FileOk);
            //OpenFile.ShowDialog();
            //return;
            if (OpenFile.ShowDialog() != DialogResult.OK) return;

            dtStart = DateTime.Now;
            strFileName = OpenFile.FileNames;
            foreach (string name in strFileName)
            {
                string extTemp = "";
                if (extTemp == "")
                {
                    strExtension = Path.GetExtension(name);
                }
                extTemp = Path.GetExtension(name);

                if (extTemp.ToLower() != strExtension.ToLower())
                    throw new Exception("Selected file has differenct extension");
            }

            #endregion *** Selected file ***

            lblBar.Text = "Parsing data from data file";
            this.Refresh();

            #region *** Parsing data ***
            Application.DoEvents();
            //progressBar.Value = 30;
            try
            {
                tblData.Clear();

                if (strExtension.ToLower() == ".txt")
                {
                    if (strFileName.Length == 1)
                        tblData = _DataParse.GetDataFromTxt(strFileName[0]);
                    else if (strFileName.Length > 1)
                        tblData = _DataParse.GetDataFromTxt(strFileName);
                }
                else if (strExtension.ToLower() == ".std" || strExtension.ToLower() == ".stdf")
                {
                    if (strFileName.Length == 1)
                        //tblData = _DataParse.GetDataFromStd(strFileName[0]);
                        tblData = _DataParse.GetDataFromStdfviewer(strFileName[0]);
                    else if (strFileName.Length > 1)
                        tblData = _DataParse.GetDataFromStdfviewer(strFileName);
                }
                else if (strExtension.ToLower() == ".csv")
                {
                    // Clear Data List Node
                    tvDataList.Nodes.Clear();

                    tblData = FetchAceTechCsvData(strFileName[0]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //progressBar.Value = 70;

            #endregion *** Parsing data ***

            this.UpdateSessionInfomation();
            this.updateGrid(tblData);

            this.CacheData("");
            this.RemoveAllTab();

            ts = DateTime.Now - dtStart;
            lblBar.Text += " / " + Math.Round(ts.TotalMilliseconds, 2).ToString() + "ms";
        }
        private void OpenFile_FileOk(object sender, CancelEventArgs e)
        {
            lblBar.Text = "File OK!";
            this.Refresh();
        }//end of openToolStripMenuItem_Click

        // *** Append Data from file
        private void appendDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region *** Variable define ***
            string strExtension = "";
            string[] strFileName;

            #endregion *** Variable define ***

            #region *** Selected file ***
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.RestoreDirectory = true;
            OpenFile.Multiselect = true;
            OpenFile.Filter = "STDF Files(*.std/*.stdf)|*.std;*.stdf|TXT data file(*.txt)|*.txt|All Files|*.*";
            //OpenFile.Filter = "STDF data file(*.std)|*.std|TXT data file(*.txt)|*.txt";
            if (OpenFile.ShowDialog() != DialogResult.OK) return;
            strFileName = OpenFile.FileNames;
            foreach (string name in strFileName)
            {
                string extTemp = "";
                if (extTemp == "")
                {
                    strExtension = Path.GetExtension(name);
                }
                extTemp = Path.GetExtension(name);

                if (extTemp.ToLower() != strExtension.ToLower())
                    throw new Exception("Selected file has differenct extension");
            }

            #endregion *** Selected file ***

            #region *** Parsing data ***
            lblBar.Text = "Parsing data from txt file";
            this.Refresh();
            Application.DoEvents();
            // Disconnect datagrid will spped up data processing a lot
            dgvData.DataSource = null;
            try
            {
                tblCachedData.Clear();
                if (strExtension.ToLower() == ".txt")
                {
                    if (strFileName.Length == 1)
                        tblCachedData = _DataParse.GetDataFromTxt(strFileName[0]);
                    else if (strFileName.Length > 1)
                        tblCachedData = _DataParse.GetDataFromTxt(strFileName);
                }
                else if (strExtension.ToLower() == ".std")
                {
                    if (strFileName.Length == 1)
                        tblCachedData = _DataParse.GetDataFromStdfviewer(strFileName[0]);
                    else if (strFileName.Length > 1)
                        tblCachedData = _DataParse.GetDataFromStdfviewer(strFileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //progressBar.Value = 70;

            #endregion *** Parsing data ***

            #region *** Appending Data ***
            int DeviceAdded = 1;
            tblCachedData.PrimaryKey = null;
            foreach (DataRow dr in tblCachedData.Rows)
            {
                if (tblCachedData.Rows.IndexOf(dr) > 3)
                {
                    dr[0] = tblData.Rows.Count - 3;
                    tblData.ImportRow(dr);
                    DeviceAdded++;
                }
            }

            #endregion *** Appending Data ***

            #region *** Update session information ***
            int OriginalTestedDevice = Convert.ToInt32(tblHeader.Rows[TestDeviceColumnIndex][1]);
            int OriginalPassedDevice = Convert.ToInt32(tblHeader.Rows[TestDeviceColumnIndex + 1][1]);
            int OriginalFailedDevice = Convert.ToInt32(tblHeader.Rows[TestDeviceColumnIndex + 2][1]);
            this.UpdateSessionInfomation();
            tblHeader.Rows[TestDeviceColumnIndex][1] = Convert.ToInt32(tblHeader.Rows[TestDeviceColumnIndex][1]) + OriginalTestedDevice;
            tblHeader.Rows[TestDeviceColumnIndex + 1][1] = Convert.ToInt32(tblHeader.Rows[TestDeviceColumnIndex + 1][1]) + OriginalPassedDevice;
            tblHeader.Rows[TestDeviceColumnIndex + 2][1] = Convert.ToInt32(tblHeader.Rows[TestDeviceColumnIndex + 2][1]) + OriginalFailedDevice;
            tblHeader.Rows[TestDeviceColumnIndex + 3][1] = Math.Round(100 * Convert.ToDouble(tblHeader.Rows[TestDeviceColumnIndex + 1][1]) /
                                                         Convert.ToDouble(tblHeader.Rows[TestDeviceColumnIndex][1]), 2).ToString() + "%";

            #endregion *** Update session information ***

            this.updateGrid(tblData);

            this.CacheData("");
            this.RemoveAllTab();

            lblBar.Text = "Total " + DeviceAdded.ToString() + " has been added";
        }

        // *** Import test data from formatted csv file ***
        private void ImportfromCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            #region *** Selected file ***
            OpenFileDialog openfiledialog1 = new OpenFileDialog();
            openfiledialog1.Filter = "CSV datat file(*.csv)|*.csv";
            if (openfiledialog1.ShowDialog() != DialogResult.OK) return;
            string strfilename = openfiledialog1.FileName;

            #endregion *** Selected file ***

            #region *** Parsing data ***
            lblBar.Text = "Parsing data from csv file";
            Application.DoEvents();
            dgvData.DataSource = null;
            //this.Refresh();
            try
            {
                tblData = _DataParse.GetDataFromCsv(strfilename);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            #endregion *** Parsing data ***

            this.UpdateSessionInfomation();
            this.updateGrid(tblData);

            this.CacheData("");
            this.RemoveAllTab();

        } //end of ImportfromCSVToolStripMenuItem_Click

        // *** Display select failure mode data in gridview ***
        private void lblFailureMode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            lblBar.Text = "Displaying selected failure mode data";
            Application.DoEvents();

            int i = 0;
            tblData.Clear();
            string[] Device = (string[])e.Link.LinkData;

            tblData = tblCachedData.Clone();
            tblData.PrimaryKey = new DataColumn[] { tblData.Columns[0] };
            tblCachedData.PrimaryKey = new DataColumn[] { tblCachedData.Columns[0] };

            tblData.ImportRow(tblCachedData.Rows[0]);
            tblData.ImportRow(tblCachedData.Rows[1]);
            tblData.ImportRow(tblCachedData.Rows[2]);
            tblData.ImportRow(tblCachedData.Rows[3]);

            while (i < Device.Length)
            {
                int j = Convert.ToInt32(Device[i]);
                //tblData.ImportRow(tblCachedData.Rows[j + 3]);
                tblData.ImportRow(tblCachedData.Rows.Find(j));
                i++;
            }

            this.updateGrid(tblData);
            tabcontrol.SelectedTab = tabTestData;
            lblBar.Text = "Done";

        } //end of lblFailureMode_LinkClicked

        // *** Restore cached data ***
        private void tvDataList_Click(object sender, System.EventArgs e)
        {
            lblBar.Text = "Restoring data from cache";
            Application.DoEvents();
            //EventArgs继承自MouseEventArgs,所以可以强转  
            MouseEventArgs Mouse_e = (MouseEventArgs)e;
            if (Mouse_e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.tvDataList.ContextMenuStrip = this.TreeViewUserMenu;
                return;
            }

            string safefileName = ((TreeView)sender).SelectedNode.Text;

            this.tvDataListUpdateData(safefileName);
            lblBar.Text = "Done";

        } //end of tvDataList_Click


        // *** Open and Cache Test Data from AceTech
        private DataTable FetchAceTechCsvData(string strFileName)
        {
            DataHeader m_Header = new DataHeader();
            DataTable tblFinal = new DataTable();
            DataTable[] tblTestData = new DataTable[6];
            string[] ArrayKGU = null;

            tblTestData = _DataParse.GetDataFromAceTechCsv(strFileName);

            if (_DataParse.a_Header.KGU)
                ArrayKGU = _DataParse.a_Header.KGU_Number.Split(',');

            #region --- Cache Data ---

            #region --- Common Header ---
            m_Header.Handler = _DataParse.a_Header.Handler;
            m_Header.LotFinishDateTime = _DataParse.a_Header.LotEndDateTime;
            m_Header.LotID = _DataParse.a_Header.LotNumber;
            m_Header.LotStartDateTime = _DataParse.a_Header.LotStartDateTime;
            m_Header.OperatorID = _DataParse.a_Header.OperatorID;
            m_Header.Product = _DataParse.a_Header.Product;
            m_Header.ProgramRev = _DataParse.a_Header.ProgramRev;
            m_Header.SubLotID = _DataParse.a_Header.SubLotNumber;
            m_Header.TestBoard = _DataParse.a_Header.TestBoard;
            m_Header.Tester = _DataParse.a_Header.Tester;
            m_Header.LotQuantity = _DataParse.a_Header.LotQty;

            m_Header.TestSession = _DataParse.a_Header.TestSession;
            m_Header.TestQuantity = _DataParse.TestedDevice = _DataParse.a_Header.TestedQty_FT;
            m_Header.PassQuantity = _DataParse.PassedDevice = _DataParse.a_Header.PassedQty_FT;
            m_Header.FailQuantity = _DataParse.FailedDevice = _DataParse.a_Header.TestedQty_FT - _DataParse.a_Header.PassedQty_FT;
            m_Header.Yield = _DataParse.a_Header.Yield_FT;

            _DataParse.Header = m_Header;

            #endregion --- Common Header ---

            #region --- KGU ---
            if (_DataParse.a_Header.KGU)
            {
                m_Header.TestSession = "KGU";
                m_Header.TestQuantity = _DataParse.TestedDevice = 1;
                m_Header.PassQuantity = _DataParse.PassedDevice = 1;
                m_Header.FailQuantity = _DataParse.FailedDevice = 0;
                m_Header.Yield = 100;

                _DataParse.Header = m_Header;

                tblData = tblTestData[0];
                this.CacheData("KGU");
            }
            #endregion --- KGU ---

            #region --- FT ---
            if (_DataParse.a_Header.FT)
            {
                m_Header.TestSession = "FT";
                m_Header.TestQuantity = _DataParse.TestedDevice = _DataParse.a_Header.TestedQty_FT;
                m_Header.PassQuantity = _DataParse.PassedDevice = _DataParse.a_Header.PassedQty_FT;
                m_Header.FailQuantity = _DataParse.FailedDevice = _DataParse.a_Header.TestedQty_FT - _DataParse.a_Header.PassedQty_FT;
                m_Header.Yield = _DataParse.a_Header.Yield_FT;

                _DataParse.Header = m_Header;

                tblData = tblTestData[1];
                this.CacheData("FT");
            }
            #endregion --- FT ---

            #region --- RT1 ---
            if (_DataParse.a_Header.RT1)
            {
                m_Header.TestSession = "RT1";
                m_Header.TestQuantity = _DataParse.TestedDevice = _DataParse.a_Header.TestedQty_RT1;
                m_Header.PassQuantity = _DataParse.PassedDevice = _DataParse.a_Header.PassedQty_RT1;
                m_Header.FailQuantity = _DataParse.FailedDevice = _DataParse.a_Header.TestedQty_RT1 - _DataParse.a_Header.PassedQty_RT1;
                m_Header.Yield = _DataParse.a_Header.Yield_RT1;

                _DataParse.Header = m_Header;

                tblData = tblTestData[2];
                this.CacheData("RT1");
            }
            #endregion --- RT1 ---

            #region --- RT2 ---
            if (_DataParse.a_Header.RT2)
            {
                m_Header.TestSession = "RT2";
                m_Header.TestQuantity = _DataParse.TestedDevice = _DataParse.a_Header.TestedQty_RT2;
                m_Header.PassQuantity = _DataParse.PassedDevice = _DataParse.a_Header.PassedQty_RT2;
                m_Header.FailQuantity = _DataParse.FailedDevice = _DataParse.a_Header.TestedQty_RT2 - _DataParse.a_Header.PassedQty_RT2;
                m_Header.Yield = _DataParse.a_Header.Yield_RT2;

                _DataParse.Header = m_Header;

                tblData = tblTestData[3];
                this.CacheData("RT2");
            }
            #endregion --- RT1 ---

            #region --- EQC ---
            if (_DataParse.a_Header.EQC)
            {
                m_Header.TestSession = "EQC";
                m_Header.TestQuantity = _DataParse.TestedDevice = _DataParse.a_Header.TestedQty_EQC;
                m_Header.PassQuantity = _DataParse.PassedDevice = _DataParse.a_Header.PassedQty_EQC;
                m_Header.FailQuantity = _DataParse.FailedDevice = _DataParse.a_Header.TestedQty_EQC - _DataParse.a_Header.PassedQty_EQC;
                m_Header.Yield = _DataParse.a_Header.Yield_EQC;

                _DataParse.Header = m_Header;

                tblData = tblTestData[4];
                this.CacheData("EQC");
            }
            #endregion --- EQC ---

            #region --- EQCV ---
            if (_DataParse.a_Header.EQCV)
            {
                m_Header.TestSession = "EQCV";
                m_Header.TestQuantity = _DataParse.TestedDevice = _DataParse.a_Header.TestedQty_EQCV;
                m_Header.PassQuantity = _DataParse.PassedDevice = _DataParse.a_Header.PassedQty_EQCV;
                m_Header.FailQuantity = _DataParse.FailedDevice = _DataParse.a_Header.TestedQty_EQCV - _DataParse.a_Header.PassedQty_EQCV;
                m_Header.Yield = _DataParse.a_Header.Yield_EQCV;

                _DataParse.Header = m_Header;

                tblData = tblTestData[5];
                this.CacheData("EQCV");
            }
            #endregion --- EQCV ---

            #endregion --- Cache Data ---

            #region --- Merge Data ---

            m_Header.TestSession = _DataParse.a_Header.TestSession;
            m_Header.TestQuantity = _DataParse.TestedDevice = _DataParse.a_Header.TestedQty_FT;
            m_Header.PassQuantity = _DataParse.PassedDevice = _DataParse.a_Header.PassedQty_FT + _DataParse.a_Header.PassedQty_RT1 + _DataParse.a_Header.PassedQty_RT2;
            m_Header.FailQuantity = _DataParse.FailedDevice = _DataParse.TestedDevice - _DataParse.PassedDevice;
            m_Header.Yield = Math.Round((Convert.ToDouble(_DataParse.PassedDevice) / Convert.ToDouble(_DataParse.TestedDevice)) * 100, 2);

            _DataParse.Header = m_Header;

            int RowIndex = 1;

            if (_DataParse.a_Header.FT && _DataParse.a_Header.RT1 && _DataParse.a_Header.RT2)
            {
                #region --- Merge Data FT & RT1 & RT2 ---
                tblFinal = tblTestData[1].Clone();
                // FT Data
                tblTestData[1].PrimaryKey = null;
                foreach (DataRow dr in tblTestData[1].Rows)
                {
                    // Header Limit
                    if (tblTestData[1].Rows.IndexOf(dr) <= 3) tblFinal.ImportRow(dr);

                    int intCount = tblTestData[1].Columns.Count;
                    if (dr[intCount - 1].ToString().ToLower() == "pass")
                    {
                        dr[0] = RowIndex;
                        tblFinal.ImportRow(dr);
                        RowIndex++;
                    }
                }

                // RT1 Data
                tblTestData[2].PrimaryKey = null;
                foreach (DataRow dr in tblTestData[2].Rows)
                {
                    int intCount = tblTestData[2].Columns.Count;
                    if (dr[intCount - 1].ToString().ToLower() == "pass")
                    {
                        dr[0] = RowIndex;
                        tblFinal.ImportRow(dr);
                        RowIndex++;
                    }
                }

                // RT2 Data
                tblTestData[3].PrimaryKey = null;
                foreach (DataRow dr in tblTestData[3].Rows)
                {
                    if (tblTestData[3].Rows.IndexOf(dr) > 3)
                    {
                        dr[0] = RowIndex;
                        tblFinal.ImportRow(dr);
                        RowIndex++;
                    }
                }
                #endregion --- Merge Data FT & RT1 & RT2 ---
            }
            else if (_DataParse.a_Header.FT && _DataParse.a_Header.RT1)
            {
                #region --- Merge Data FT & RT1 ---
                // FT Data
                tblFinal = tblTestData[1].Clone();
                tblTestData[1].PrimaryKey = null;
                foreach (DataRow dr in tblTestData[1].Rows)
                {
                    // Header Limit
                    if (tblTestData[1].Rows.IndexOf(dr) <= 3) tblFinal.ImportRow(dr);

                    int intCount = tblTestData[1].Columns.Count;
                    if (dr[intCount - 1].ToString().ToLower() == "pass")
                    {
                        dr[0] = RowIndex;
                        tblFinal.ImportRow(dr);
                        RowIndex++;
                    }
                }

                // RT1 Data
                tblTestData[3].PrimaryKey = null;
                foreach (DataRow dr in tblTestData[2].Rows)
                {
                    if (tblTestData[2].Rows.IndexOf(dr) > 3)
                    {
                        dr[0] = RowIndex;
                        tblFinal.ImportRow(dr);
                        RowIndex++;
                    }
                }
                #endregion --- Merge Data FT & RT1 ---
            }
            else
            {
                #region --- No Merge ---
                if (_DataParse.a_Header.FT)
                {
                    m_Header.TestSession = "FT";
                    m_Header.TestQuantity = _DataParse.TestedDevice = _DataParse.a_Header.TestedQty_FT;
                    m_Header.PassQuantity = _DataParse.PassedDevice = _DataParse.a_Header.PassedQty_FT;
                    m_Header.FailQuantity = _DataParse.FailedDevice = _DataParse.a_Header.TestedQty_FT - _DataParse.a_Header.PassedQty_FT;
                    m_Header.Yield = _DataParse.a_Header.Yield_FT;

                    _DataParse.Header = m_Header;

                    tblFinal = tblTestData[1];
                }
                else if (_DataParse.a_Header.RT1)
                {
                    m_Header.TestSession = "RT1";
                    m_Header.TestQuantity = _DataParse.TestedDevice = _DataParse.a_Header.TestedQty_RT1;
                    m_Header.PassQuantity = _DataParse.PassedDevice = _DataParse.a_Header.PassedQty_RT1;
                    m_Header.FailQuantity = _DataParse.FailedDevice = _DataParse.a_Header.TestedQty_RT1 - _DataParse.a_Header.PassedQty_RT1;
                    m_Header.Yield = _DataParse.a_Header.Yield_RT1;

                    _DataParse.Header = m_Header;

                    tblFinal = tblTestData[2];
                }
                else if (_DataParse.a_Header.RT2)
                {
                    m_Header.TestSession = "RT2";
                    m_Header.TestQuantity = _DataParse.TestedDevice = _DataParse.a_Header.TestedQty_RT2;
                    m_Header.PassQuantity = _DataParse.PassedDevice = _DataParse.a_Header.PassedQty_RT2;
                    m_Header.FailQuantity = _DataParse.FailedDevice = _DataParse.a_Header.TestedQty_RT2 - _DataParse.a_Header.PassedQty_RT2;
                    m_Header.Yield = _DataParse.a_Header.Yield_RT2;

                    _DataParse.Header = m_Header;

                    tblFinal = tblTestData[3];
                }
                else if (_DataParse.a_Header.EQC)
                {
                    m_Header.TestSession = "EQC";
                    m_Header.TestQuantity = _DataParse.TestedDevice = _DataParse.a_Header.TestedQty_EQC;
                    m_Header.PassQuantity = _DataParse.PassedDevice = _DataParse.a_Header.PassedQty_EQC;
                    m_Header.FailQuantity = _DataParse.FailedDevice = _DataParse.a_Header.TestedQty_EQC - _DataParse.a_Header.PassedQty_EQC;
                    m_Header.Yield = _DataParse.a_Header.Yield_EQC;

                    _DataParse.Header = m_Header;

                    tblFinal = tblTestData[4];
                }
                else if (_DataParse.a_Header.EQCV)
                {
                    m_Header.TestSession = "EQCV";
                    m_Header.TestQuantity = _DataParse.TestedDevice = _DataParse.a_Header.TestedQty_EQCV;
                    m_Header.PassQuantity = _DataParse.PassedDevice = _DataParse.a_Header.PassedQty_EQCV;
                    m_Header.FailQuantity = _DataParse.FailedDevice = _DataParse.a_Header.TestedQty_EQCV - _DataParse.a_Header.PassedQty_EQCV;
                    m_Header.Yield = _DataParse.a_Header.Yield_EQCV;

                    _DataParse.Header = m_Header;

                    tblFinal = tblTestData[5];
                }
                #endregion --- No Merge ---
            }

            #endregion --- Merge Data ---

            #region --- KGU Analysis ---
            if (_DataParse.a_Header.KGU)
            {
                m_Header = _DataParse.Header;
                AceTechKGUVerify(tblTestData[0], ArrayKGU, 1);
                _DataParse.Header = m_Header;
            }
            #endregion --- KGU Analysis ---

            //tblData.Clear();
            return tblFinal;
        }



        #endregion *** Import Data ***


        #region *** Delete Data ***
        // Show All data
        private void showAllDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int DeviceCount = -4;
            int PassDeviceCount = 0;
            lblBar.Text = "Restoring Data";
            Application.DoEvents();
            dgvData.DataSource = null;
            tblData.Clear();

            tblCachedData.PrimaryKey = null;
            tblData.PrimaryKey = null;

            foreach (DataRow dr in tblCachedData.Rows)
            {
                tblData.ImportRow(dr);
                DeviceCount++;
                if (dr[tblCachedData.Columns.Count - 1].ToString().ToLower() == "pass")
                {
                    PassDeviceCount++;
                }

            }
            // Tested quantity
            tblHeader.Rows[TestDeviceColumnIndex][1] = DeviceCount;
            // Passed quantity
            tblHeader.Rows[TestDeviceColumnIndex + 1][1] = PassDeviceCount;
            // Failed quantity
            tblHeader.Rows[TestDeviceColumnIndex + 2][1] = DeviceCount - PassDeviceCount;
            // Yield
            if (DeviceCount != 0)
            {
                double dblYield = Convert.ToDouble(PassDeviceCount) / Convert.ToDouble(DeviceCount) * 100;
                tblHeader.Rows[TestDeviceColumnIndex + 3][1] = (Math.Round(dblYield, 3)).ToString() + "%";
            }
            else
            {
                tblHeader.Rows[TestDeviceColumnIndex + 3][1] = "0%";
            }

            dgvData.DataSource = tblData;
            this.dgvDataGridViewFormat();
            lblBar.Text += "...Done";

        }

        // Show Pass data
        private void showPassDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int DeviceCount = 0;
            lblBar.Text = "Deleting Fail data";
            Application.DoEvents();
            dgvData.DataSource = null;
            tblData.Clear();
            tblData = tblCachedData.Copy();

            tblData.PrimaryKey = null;

            for (int i = 4; i < tblData.Rows.Count; i++)
            {
                // delete pass data
                if (tblData.Rows[i][tblData.Columns.Count - 1].ToString().ToLower() == "fail")
                {
                    tblData.Rows.RemoveAt(i);
                    i--;
                }
                else
                {
                    tblData.Rows[i][0] = DeviceCount + 1;
                    DeviceCount++;
                }
            }
            // Tested quantity
            tblHeader.Rows[TestDeviceColumnIndex][1] = DeviceCount;
            // Passed quantity
            tblHeader.Rows[TestDeviceColumnIndex + 1][1] = DeviceCount;
            // Failed quantity
            tblHeader.Rows[TestDeviceColumnIndex + 2][1] = 0;
            // Yield
            tblHeader.Rows[TestDeviceColumnIndex + 3][1] = "100%";

            tblData.PrimaryKey = new DataColumn[] { tblData.Columns[0] };
            tblCachedData.PrimaryKey = new DataColumn[] { tblCachedData.Columns[0] };
            dgvData.DataSource = tblData;
            this.dgvDataGridViewFormat();
            lblBar.Text = "Total " + (tblCachedData.Rows.Count - tblData.Rows.Count).ToString() + " fail data has been deleted";
        }

        // Show Fail data
        private void showFailDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int DeviceCount = 0;
            lblBar.Text = "Deleting pass data";
            Application.DoEvents();
            dgvData.DataSource = null;
            tblData.Clear();
            tblData = tblCachedData.Copy();

            tblData.PrimaryKey = null;

            for (int i = 4; i < tblData.Rows.Count; i++)
            {
                // delete pass data
                if (tblData.Rows[i][tblData.Columns.Count - 1].ToString().ToLower() == "pass")
                {
                    tblData.Rows.RemoveAt(i);
                    i--;
                }
                else
                {
                    tblData.Rows[i][0] = DeviceCount + 1;
                    DeviceCount++;
                }
            }
            // Tested quantity
            tblHeader.Rows[TestDeviceColumnIndex][1] = DeviceCount;
            // Passed quantity
            tblHeader.Rows[TestDeviceColumnIndex + 1][1] = 0;
            // Failed quantity
            tblHeader.Rows[TestDeviceColumnIndex + 2][1] = DeviceCount;
            // Yield
            tblHeader.Rows[TestDeviceColumnIndex + 3][1] = "0%";

            tblData.PrimaryKey = new DataColumn[] { tblData.Columns[0] };
            tblCachedData.PrimaryKey = new DataColumn[] { tblCachedData.Columns[0] };
            dgvData.DataSource = tblData;
            this.dgvDataGridViewFormat();
            lblBar.Text = "Total " + (tblCachedData.Rows.Count - tblData.Rows.Count).ToString() + " pass data has been deleted";
        }

        //Show site#1 data
        private void onlyShowSite1DataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int DeviceCount = 0;
            int PassDeviceCount = 0;
            lblBar.Text = "Deleting site#2 data";
            this.Refresh();
            Application.DoEvents();
            dgvData.DataSource = null;
            tblData.Clear();
            tblData = tblCachedData.Copy();

            tblData.PrimaryKey = null;

            for (int i = 4; i < tblData.Rows.Count; i++)
            {
                // delete site#2 data
                if (tblData.Rows[i][1].ToString().ToLower() == "2")
                {
                    tblData.Rows.RemoveAt(i);
                    i--;
                }
                else
                {
                    tblData.Rows[i][0] = DeviceCount + 1;
                    //caculate pass device count
                    if (tblData.Rows[i][tblData.Columns.Count - 1].ToString().ToLower() == "pass")
                    {
                        PassDeviceCount++;
                    }
                    DeviceCount++;
                }
            }
            // Tested quantity
            tblHeader.Rows[TestDeviceColumnIndex][1] = DeviceCount;
            // Passed quantity
            tblHeader.Rows[TestDeviceColumnIndex + 1][1] = PassDeviceCount;
            // Failed quantity
            tblHeader.Rows[TestDeviceColumnIndex + 2][1] = DeviceCount - PassDeviceCount;
            // Yield
            if (DeviceCount != 0)
            {
                double dblYield = Convert.ToDouble(PassDeviceCount) / Convert.ToDouble(DeviceCount) * 100;
                tblHeader.Rows[TestDeviceColumnIndex + 3][1] = (Math.Round(dblYield, 2)).ToString() + "%";
            }
            else
            {
                tblHeader.Rows[TestDeviceColumnIndex + 3][1] = "0%";
            }

            tblData.PrimaryKey = new DataColumn[] { tblData.Columns[0] };
            dgvData.DataSource = tblData;
            this.dgvDataGridViewFormat();
            lblBar.Text = "Total " + (tblCachedData.Rows.Count - tblData.Rows.Count).ToString() + " site#2 data has been deleted";
            this.Refresh();
        }

        //Show site#2 data
        private void onlyShowSite2DataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int DeviceCount = 0;
            int PassDeviceCount = 0;
            lblBar.Text = "Deleting site#1 data";
            this.Refresh();
            Application.DoEvents();
            dgvData.DataSource = null;
            tblData.Clear();
            tblData = tblCachedData.Copy();

            tblData.PrimaryKey = null;

            for (int i = 4; i < tblData.Rows.Count; i++)
            {
                // delete site#1 data
                if (tblData.Rows[i][1].ToString().ToLower() == "1")
                {
                    tblData.Rows.RemoveAt(i);
                    i--;
                }
                else
                {
                    tblData.Rows[i][0] = DeviceCount + 1;
                    //caculate pass device count
                    if (tblData.Rows[i][tblData.Columns.Count - 1].ToString().ToLower() == "pass")
                    {
                        PassDeviceCount++;
                    }
                    DeviceCount++;
                }
            }
            // Tested quantity
            tblHeader.Rows[TestDeviceColumnIndex][1] = DeviceCount;
            // Passed quantity
            tblHeader.Rows[TestDeviceColumnIndex + 1][1] = PassDeviceCount;
            // Failed quantity
            tblHeader.Rows[TestDeviceColumnIndex + 2][1] = DeviceCount - PassDeviceCount;
            // Yield
            if (DeviceCount != 0)
            {
                double dblYield = Convert.ToDouble(PassDeviceCount) / Convert.ToDouble(DeviceCount) * 100;
                tblHeader.Rows[TestDeviceColumnIndex + 3][1] = (Math.Round(dblYield, 2)).ToString() + "%";
            }
            else
            {
                tblHeader.Rows[TestDeviceColumnIndex + 3][1] = "0%";
            }

            tblData.PrimaryKey = new DataColumn[] { tblData.Columns[0] };
            tblCachedData.PrimaryKey = new DataColumn[] { tblCachedData.Columns[0] };
            dgvData.DataSource = tblData;
            this.dgvDataGridViewFormat();
            lblBar.Text = "Total " + (tblCachedData.Rows.Count - tblData.Rows.Count).ToString() + " site#1 data has been deleted";
            this.Refresh();
        }

        //Custom View
        private void customViewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion *** Delete Data ***

        
        #region *** Export Data ***
        // *** Test data export ***
        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "CSV File|*.csv";
                if (!saveFileDialog1.CheckFileExists)
                    saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                //Save Test data
                if (tabcontrol.SelectedTab.Text == strTestDataTabName)
                {
                    if (tblData.Rows.Count == 0)
                    {
                        MessageBox.Show("No Test data need to be saved", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    saveFileDialog1.Title = "Save Test Data as";
                    saveFileDialog1.FileName = "Data.csv";
                    if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;
                    _Export.DataTableToCsv(saveFileDialog1.FileName, tblData, _DataParse.Header);
                }
                //Save Failure Mode
                else if (tabcontrol.SelectedTab.Text == strFailureModeTabName)
                {
                    if (strFailureMode == "")
                    {
                        MessageBox.Show("No Failure Mode data need to be saved", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    saveFileDialog1.Title = "Save Failure Mode as";
                    saveFileDialog1.FileName = "FM.csv";
                    if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;
                    _Export.StringToCsv(saveFileDialog1.FileName, strFailureMode, string.Empty, false);
                }
                //Save Failure rate
                else if (tabcontrol.SelectedTab.Text == strFailureRateTabName)
                {
                    if (strFailureRate == "")
                    {
                        MessageBox.Show("No Failure Rate data need to be saved", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    saveFileDialog1.Title = "Save Failure Rate as";
                    saveFileDialog1.FileName = "FR.csv";
                    if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;
                    _Export.StringToCsv(saveFileDialog1.FileName, strFailureRate, string.Empty, false);
                }
                //Save KGU verifyData
                else if (tabcontrol.SelectedTab.Text == strKGUTabName)
                {
                    if (tblKGU.Rows.Count == 0)
                    {
                        MessageBox.Show("No data need to be saved", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    saveFileDialog1.Title = "Save KGU Verify Data as";
                    saveFileDialog1.FileName = "KGU.csv";
                    if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;
                    _Export.DataTableToCsv(saveFileDialog1.FileName, tblKGU);
                }
                //MessageBox.Show("File has been saved to " + saveFileDialog1.FileName, "File Saved",
                                                                    //MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblBar.Text = "File has been saved to " + saveFileDialog1.FileName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }// end of exportToCSVToolStripMenuItem_Click

        #endregion *** Export Data***


        #region *** Distribution Sub Functions ***
        // *** Start JMP ***
        private void startJMP()
        {
            try
            {
                myJMP.Activate();
                myJMP.Visible = true;
            }
            catch
            {
                this.myJMP = new JMP.Application();
                myJMP.Visible = true;

                //RegistryKey rk = Registry.LocalMachine.OpenSubKey("Software\\SAS Institute Inc.\\JMP\\9.0", true);

                //jmpInstallDir = (string) rk.GetValue("InstallDir") + "\\";
                jmpInstallDir = @"C:\Program Files (x86)\SAS\JMP\9";
                jmpSampleDataDir = jmpInstallDir + "\\Support Files English\\Sample Data\\";
            }
        }

        // *** Quit JMP ***
        void btnQuitJMP_Click(object sender, EventArgs e)
        {
            QuitJMP();
        }

        // *** Generate Distribution ***
        void btnGenerate_Click(object sender, EventArgs e)
        {
            #region *** Variable define ***
            int ColumnsCount = tblData.Columns.Count;
            int RowCount = tblData.Rows.Count;

            int[] SelectedColumns;

            string Directory;
            string fileName;

            bool isStack = false;

            int handler;
            JMP.Document doc;
            JMP.DataTable dt;
            JMP.DataTable subdt;
            JMP.DataTable stackdt;
            JMP.TextImport ti;
            JMP.Distribution dist;

            object[] objParameter = new object[400];
            StringBuilder sbParameter = new StringBuilder();

            TabPage tabDistribution = (TabPage)this.FindControl(tabcontrol, strDistributionTabName);
            ListBox lbxSelected = (ListBox)this.FindControl(tabDistribution, "lbxSelected");

            #endregion *** Variable define ***

            #region *** Build JMP DataTable ***
            QuitJMP();

            lblBar.Text = "Caculating the Distribution ...";
            Directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            fileName = Directory + @"\Temp\JMP.csv";

            //if (lbxSelected.Items.Count > 1) isStack = true;

            SelectedColumns = new int[lbxSelected.Items.Count];
            for (int i = 0; i < lbxSelected.Items.Count;i++ )
            {
                SelectedColumns[i] = Array.IndexOf(Parameter, lbxSelected.Items[i].ToString()) + 3;
            }
            _Export.DataTableToCsvForJMP(fileName, tblData, SelectedColumns);

            startJMP();

            ti = myJMP.CreateTextImportObject(fileName, RowCount - 3);
            ti.SetEndOfFieldOptions(JMP.jmpTIEndOfFieldConstants.tiComma);
            ti.SetEndOfLineOptions(JMP.jmpTIEndOfLineConstants.tiCRLF);
            //ti.ColumnNamesStart(0);
            ti.FirstLineIsData(false);
            doc = ti.OpenFile();
            doc.Name = "Raw";

            dt = doc.GetDataTable();

            //if (isStack)
            //{
            //    dt = doc.GetDataTable();

            //    foreach (object objParameter in lbxSelected.Items)
            //    {
            //        dt.AddToStackList(objParameter.ToString());
            //    }
            //    dt.SetStackOptions(false, true, true);
            //    stackdt = dt.Stack("Parameter", "Data", "Stack");

            //    dt.Visible = true;
            //    stackdt.Visible = true;

            //    stackdt.Activate();
            //}
            //else
            //{
            //    stackdt = doc.GetDataTable();
            //}

            #endregion *** Build JMP DataTable ***

            #region *** Build Distribution Graph ***

            dt.Visible = false;
            dt.Activate();

            lbxSelected.Items.CopyTo(objParameter, 0);

            fileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\dist.jsl";
            
            _Analysis.BuildJMPScript(fileName, objParameter, false);

            myJMP.RunJSLFile(fileName);

            #endregion *** Build Distribution Graph ***

            lblBar.Text = "Done";
            //myJMP.GetActiveJournal();
            //this.WindowState = FormWindowState.Minimized;
            //myJMP.CloseWindowsOfType(JMP.jmpWindowTypeConstants.jmpDatatables);
            //myJMP.CloseWindowsOfType(JMP.jmpWindowTypeConstants.jmpJSLOutputFiles);
        }

        // *** Get Parameter Index ***
        private void ListBoxInsert(ListBox listBox, object objInsertItem)
        {
            string strPatameter = objInsertItem.ToString();
            int Index = Array.IndexOf(Parameter, strPatameter);
            int ItemCount = listBox.Items.Count;
            int IndexMax = 0;

            if (ItemCount != 0)
                IndexMax = Array.IndexOf(Parameter, listBox.Items[ItemCount - 1]);

            if (ItemCount == 0 || Index > IndexMax)
            {
                listBox.Items.Add(objInsertItem);
            }
            else
            {
                for (int i = 0; i < ItemCount; i++)
                {
                    int CurrentIndex = Array.IndexOf(Parameter, listBox.Items[i]);
                    if (Index < CurrentIndex)
                    {
                        listBox.Items.Insert(i, objInsertItem);
                        return;
                    }
                } // end of for
            } //end of if else
        }

        void btnRemoveAll_Click(object sender, EventArgs e)
        {
            TabPage tabDistribution = (TabPage)this.FindControl(tabcontrol, strDistributionTabName);
            ListBox lbxParameter = (ListBox)this.FindControl(tabDistribution, "lbxParameter");
            ListBox lbxSelected = (ListBox)this.FindControl(tabDistribution, "lbxSelected");
            //Button btnAdd = (Button)this.FindControl(tabDistribution, "btnAdd");

            int intCount = lbxSelected.Items.Count;
            string[] strParameter = new string[intCount];

            for (int i = 0; i < intCount; i++)
            {
                object objItem = lbxSelected.Items[i];
                ListBoxInsert(lbxParameter, objItem);
                strParameter[i] = objItem.ToString();
                //lbxSelected.Items.Remove(objItem);
            }
            for (int i = 0; i < intCount; i++)
            {
                lbxSelected.Items.Remove(strParameter[i]);
            }
        }

        void btnAddAll_Click(object sender, EventArgs e)
        {
            TabPage tabDistribution = (TabPage)this.FindControl(tabcontrol, strDistributionTabName);
            ListBox lbxParameter = (ListBox)this.FindControl(tabDistribution, "lbxParameter");
            ListBox lbxSelected = (ListBox)this.FindControl(tabDistribution, "lbxSelected");
            //Button btnAdd = (Button)this.FindControl(tabDistribution, "btnAdd");

            int intCount = lbxParameter.Items.Count;
            string[] strParameter = new string[intCount];
            
            for (int i = 0; i < intCount; i++)
            {
                object objItem = lbxParameter.Items[i];
                ListBoxInsert(lbxSelected, objItem);
                strParameter[i] = objItem.ToString();
                //lbxSelected.Items.Remove(objItem);
            }
            for (int i = 0; i < intCount; i++)
            {
                lbxParameter.Items.Remove(strParameter[i]);
            }
        }

        void btnRemove_Click(object sender, EventArgs e)
        {
            TabPage tabDistribution = (TabPage)this.FindControl(tabcontrol, strDistributionTabName);
            ListBox lbxParameter = (ListBox)this.FindControl(tabDistribution, "lbxParameter");
            ListBox lbxSelected = (ListBox)this.FindControl(tabDistribution, "lbxSelected");
            //Button btnAdd = (Button)this.FindControl(tabDistribution, "btnAdd");
            int intSelectCount = lbxSelected.SelectedItems.Count;
            string[] strParameter = new string[intSelectCount];

            for (int i = 0; i < intSelectCount; i++)
            {
                object objItem = lbxSelected.SelectedItems[i];
                ListBoxInsert(lbxParameter, objItem);
                strParameter[i] = objItem.ToString();
                //lbxSelected.Items.Remove(objItem);
            }
            for (int i = 0; i < intSelectCount; i++)
            {
                lbxSelected.Items.Remove(strParameter[i]);
            }
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            TabPage tabDistribution = (TabPage)this.FindControl(tabcontrol, strDistributionTabName);
            ListBox lbxParameter = (ListBox)this.FindControl(tabDistribution, "lbxParameter");
            ListBox lbxSelected = (ListBox)this.FindControl(tabDistribution, "lbxSelected");
            //Button btnAdd = (Button)this.FindControl(tabDistribution, "btnAdd");
            int intSelectCount = lbxParameter.SelectedItems.Count;
            string[] strParameter = new string[intSelectCount];

            for (int i = 0; i < intSelectCount; i++)
            {
                object objItem = lbxParameter.SelectedItems[i];
                ListBoxInsert(lbxSelected, objItem);
                strParameter[i] = objItem.ToString();
                //lbxParameter.Items.Remove(objItem);
            }
            for (int i = 0; i < intSelectCount; i++)
            {
                lbxParameter.Items.Remove(strParameter[i]);
            }
        } 
        #endregion *** Distribution Sub Functions ***


        #region *** Data Analysis ***
        // *** Distribution ***
        private void distributionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Return if no any data
            if (tblData.Rows.Count == 0)
            {
                MessageBox.Show("No any data need to be analysis!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.DoEvents();

            #region *** Initialize controls ***

            this.RemoveTab(strDistributionTabName);
            TabPage tabDistribution = new TabPage(strDistributionTabName);
            tabDistribution.Name = strDistributionTabName;
            tabcontrol.Controls.Add(tabDistribution);
            tabcontrol.SelectedTab = tabDistribution;
            tabDistribution.AutoScroll = true;

            //Panel panelDistribution = new Panel();
            //panelDistribution.Dock = DockStyle.Fill;

            GroupBox gbParameter = new GroupBox();
            gbParameter.Name = "gbParameter";
            gbParameter.Text = "All Parameter";
            gbParameter.Location = new Point(12, 12);
            gbParameter.Size = new System.Drawing.Size(194, 373);

            ListBox lbxParameter = new ListBox();
            lbxParameter.Name = "lbxParameter";
            lbxParameter.SelectionMode = SelectionMode.MultiExtended;
            lbxParameter.Dock = DockStyle.Fill;
            gbParameter.Controls.Add(lbxParameter);

            GroupBox gbSelected = new GroupBox();
            gbSelected.Name = "gbSelected";
            gbSelected.Text = "Selected Parameter";
            gbSelected.Location = new Point(270, 12);
            gbSelected.Size = new System.Drawing.Size(194, 373);

            ListBox lbxSelected = new ListBox();
            lbxSelected.Name = "lbxSelected";
            lbxSelected.SelectionMode = SelectionMode.MultiExtended;
            lbxSelected.Dock = DockStyle.Fill;
            gbSelected.Controls.Add(lbxSelected);

            Button btnAdd = new Button();
            btnAdd.Name = "btnAdd";
            btnAdd.Text = ">>";
            btnAdd.Location = new Point(219, 43);
            btnAdd.Size = new System.Drawing.Size(39, 23);
            btnAdd.Click += new EventHandler(btnAdd_Click);

            Button btnRemove = new Button();
            btnRemove.Name = "btnRemove";
            btnRemove.Text = "<<";
            btnRemove.Location = new Point(219, 73);
            btnRemove.Size = new System.Drawing.Size(39, 23);
            btnRemove.Click += new EventHandler(btnRemove_Click);

            Button btnAddAll = new Button();
            btnAddAll.Name = "btnAddAll";
            btnAddAll.Text = ">>>";
            btnAddAll.Location = new Point(219, 113);
            btnAddAll.Size = new System.Drawing.Size(39, 23);
            btnAddAll.Click += new EventHandler(btnAddAll_Click);

            Button btnRemoveAll = new Button();
            btnRemoveAll.Name = "btnRemoveAll";
            btnRemoveAll.Text = "<<<";
            btnRemoveAll.Location = new Point(219, 143);
            btnRemoveAll.Size = new System.Drawing.Size(39, 23);
            btnRemoveAll.Click += new EventHandler(btnRemoveAll_Click);

            Button btnGenerate = new Button();
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Text = "Generate";
            btnGenerate.Location = new Point(476, 43);
            btnGenerate.Size = new System.Drawing.Size(75, 23);
            btnGenerate.Click += new EventHandler(btnGenerate_Click);

            Button btnQuitJMP = new Button();
            btnQuitJMP.Name = "btnQuitJMP";
            btnQuitJMP.Text = "Quit JMP";
            btnQuitJMP.Location = new Point(476, 73);
            btnQuitJMP.Size = new System.Drawing.Size(75, 23);
            btnQuitJMP.Click += new EventHandler(btnQuitJMP_Click);

            //tabDistribution.Controls.Add(panelDistribution);
            tabDistribution.Controls.Add(gbParameter);
            tabDistribution.Controls.Add(btnAdd);
            tabDistribution.Controls.Add(btnRemove);
            tabDistribution.Controls.Add(btnAddAll);
            tabDistribution.Controls.Add(btnRemoveAll);
            tabDistribution.Controls.Add(gbSelected);
            tabDistribution.Controls.Add(btnGenerate);
            tabDistribution.Controls.Add(btnQuitJMP);

            #endregion *** Initialize controls ***

            #region *** Fill Parameter ***
            int ParameterCount = tblData.Columns.Count - 4;
            Parameter = new string[ParameterCount];

            for (int i = 0; i < ParameterCount;i++ )
            {
                Parameter[i] = tblData.Rows[0][i + 3].ToString();
                lbxParameter.Items.Add(Parameter[i]);
            }
            //lbxParameter.DataSource = strParameter;
            Application.DoEvents();
            #endregion *** Fill Parameter ***

        }// end of distributionToolStripMenuItem_Click

        // *** JMP distribution for all parameter ***
        private void distributionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //// Return if no any data
            //if (tblData.Rows.Count == 0)
            //{
            //    MessageBox.Show("No any data need to be analysis!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            // Select JMP Script

            Application.DoEvents();
            frmJMPScriptSelect JMPScriptSelect = new frmJMPScriptSelect();
            JMPScriptSelect.ReturnMsg += new frmJMPScriptSelect.ReturnDataEventHandler(JMPScriptSelect_ReturnMsg);
            JMPScriptSelect.ShowDialog();
        }
        private void JMPScriptSelect_ReturnMsg(object sender, ReturnEventArgs e)
        {
            if (e.intMsg == 0) return;

            #region *** Variable define ***
            int ColumnsCount = tblData.Columns.Count;
            int RowCount = tblData.Rows.Count;

            int[] SelectedColumns;

            string Directory;
            string fileName;
            string scriptPath = "Normal";

            int handler;
            JMP.Document doc;
            JMP.DataTable dt;
            JMP.DataTable subdt;
            JMP.DataTable stackdt;
            JMP.TextImport ti;
            JMP.Distribution dist;

            object[] objParameter = new object[400];

            //TabPage tabDistribution = (TabPage)this.FindControl(tabcontrol, strDistributionTabName);
            //ListBox lbxSelected = (ListBox)this.FindControl(tabDistribution, "lbxSelected");

            #endregion *** Variable define ***

            #region *** Fill Parameter ***
            int ParameterCount = tblData.Columns.Count - _DataParse.FreezeColumn - 1;
            Parameter = new string[ParameterCount + 1];

            for (int i = 0; i <= ParameterCount; i++)
            {
                Parameter[i] = tblData.Rows[0][i + _DataParse.FreezeColumn].ToString();
            }

            #endregion *** Fill Parameter ***

            #region *** Build JMP DataTable ***

            QuitJMP();

            lblBar.Text = "Caculating the Distribution ...";
            Directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            fileName = Directory + @"\Temp\JMP.csv";

            SelectedColumns = new int[Parameter.Length];
            for (int i = 0; i < Parameter.Length; i++)
            {
                SelectedColumns[i] = Array.IndexOf(Parameter, Parameter[i]) + _DataParse.FreezeColumn;
            }
            _Export.DataTableToCsvForJMP(fileName, tblData, SelectedColumns);

            startJMP();

            ti = myJMP.CreateTextImportObject(fileName, RowCount - 3);
            ti.SetEndOfFieldOptions(JMP.jmpTIEndOfFieldConstants.tiComma);
            ti.SetEndOfLineOptions(JMP.jmpTIEndOfLineConstants.tiCRLF);
            //ti.ColumnNamesStart(0);
            ti.FirstLineIsData(false);
            doc = ti.OpenFile();
            doc.Name = "Raw";
            dt = doc.GetDataTable();

            ColumnsCount = dt.NumberColumns;
            RowCount = dt.NumberRows;

            dt.Visible = false;
            dt.Activate();

            if (e.strMsg.ToLower().Contains("stack"))
            {
                foreach (string strParameter in Parameter)
                {
                    if (strParameter != "Status")
                        dt.AddToStackList(strParameter);
                }
                dt.SetStackOptions(false, true, false);
                stackdt = dt.Stack("Parameter", "Data", "Stack");

                dt.Visible = false;
                stackdt.Visible = false;
                stackdt.Activate();
            }

            #endregion *** Build JMP DataTable ***

            #region *** Build Distribution Graph ***

            if (e.strMsg.ToLower() == "normal")
            {
                //dt.Visible = true;
                scriptPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\dist.jsl";

                Array.Copy(Parameter, objParameter, Parameter.Length - 1);
                _Analysis.BuildJMPScript(scriptPath, objParameter, true);
            }
            else
            {
                scriptPath = Application.StartupPath + @".\Script\" + e.strMsg + ".jsl";
                scriptPath = Path.GetFullPath(scriptPath);
            }

            myJMP.RunJSLFile(scriptPath);

            #endregion *** Build Distribution Graph ***

            lblBar.Text = "Done";
            this.WindowState = FormWindowState.Minimized;
            //myJMP.CloseWindowsOfType(JMP.jmpWindowTypeConstants.jmpDatatables);
            //myJMP.CloseWindowsOfType(JMP.jmpWindowTypeConstants.jmpJSLOutputFiles);
        }

        // *** Failmode analysis ***
        private void failureModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Return if no any data
            if (tblData.Rows.Count == 0 || _DataParse.FailedDevice == 0)
            {
                if (MessageBox.Show("No failure parts data, do you want to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
            }
            lblBar.Text = "Failure mode analysising";
            Application.DoEvents();
            List<FailureMode> failureModeList = new List<FailureMode>();
            failureModeList = _Analysis.GetFailureModeList(tblData, _DataParse.FreezeColumn, AnalysisType.FailureMode);

            this.RemoveTab(strFailureModeTabName);

            #region *** Display the result ***
            strFailureMode = "#,Failure device count, % of failures,% of total devices, Failure mode" + "\r\n";
            TabPage tabFailureMode = new TabPage(strFailureModeTabName);
            tabcontrol.Controls.Add(tabFailureMode);
            tabcontrol.SelectedTab = tabFailureMode;
            tabFailureMode.AutoScroll = true;

            int intFailureModeCount = 1;
            double TestDeviceCount = Convert.ToInt32(tblHeader.Rows[TestDeviceColumnIndex][1]);
            double PassDeviceCount = Convert.ToInt32(tblHeader.Rows[TestDeviceColumnIndex + 1][1]);
            double FailDeviceCount = Convert.ToInt32(tblHeader.Rows[TestDeviceColumnIndex + 2][1]);

            foreach (FailureMode failureMode in failureModeList) //for failureModeList
            {
                StringBuilder sbFailureMode = new StringBuilder();
                //#
                sbFailureMode.Append("#" + intFailureModeCount.ToString("00") + " , ");
                //Failure Device Count
                sbFailureMode.Append(failureMode.Count.ToString() + " , ");
                //% of failures
                double FailureMode_Percentage = (Convert.ToDouble(failureMode.Count) / FailDeviceCount) * 100;
                sbFailureMode.Append(FailureMode_Percentage.ToString("00.00") + "% , ");
                //% of total devices
                double FailureMode_Percentage_Total = (Convert.ToDouble(failureMode.Count) / TestDeviceCount) * 100;
                sbFailureMode.Append(FailureMode_Percentage_Total.ToString("00.00") + "% , ");
                //Failure mode
                sbFailureMode.AppendLine(failureMode.Parameter);

                LinkLabel lblFailureMode = new LinkLabel();
                lblFailureMode.AutoSize = true;
                if (!Convert.ToBoolean(intFailureModeCount % 2))
                    lblFailureMode.LinkColor = Color.Black;
                lblFailureMode.LinkBehavior = LinkBehavior.NeverUnderline;
                lblFailureMode.Text = sbFailureMode.ToString();
                lblFailureMode.Links.Add(0, lblFailureMode.Text.Length, failureMode.Device);    //Transfer device no
                //lblFailureMode.AutoSize = false;
                //lblFailureMode.Width = SplitContainer.Panel2.Bounds.Width - 30;
                //lblFailureMode.UseCompatibleTextRendering = true;
                lblFailureMode.Location = new Point(10, 25 * intFailureModeCount);
                lblFailureMode.LinkClicked += new LinkLabelLinkClickedEventHandler(lblFailureMode_LinkClicked);
                lblFailureMode.Name = "lblFailureMode" + intFailureModeCount.ToString();
                tabFailureMode.Controls.Add(lblFailureMode);

                //Save to global variable string for exprot
                strFailureMode += sbFailureMode.ToString();

                intFailureModeCount++;
            }//end of for failureModeList

            #endregion *** Display the result ***
            lblBar.Text = "Done";
        }//end of topFailModeToolStripMenuItem_Click

        // *** Failure rate analysis ***
        private void topFailureRateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Return if no any data
            if (tblData.Rows.Count == 0 || _DataParse.FailedDevice == 0)
            {
                if (MessageBox.Show("No failure parts data, do you want to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
            }
            lblBar.Text = "Failure rate analysising";
            Application.DoEvents();
            List<FailureMode> failureRateList = new List<FailureMode>();
            failureRateList = _Analysis.GetFailureModeList(tblData, _DataParse.FreezeColumn, AnalysisType.FailureRate);

            this.RemoveTab(strFailureRateTabName);

            int intFailureModeCount = 1;
            double TestDeviceCount = Convert.ToInt32(tblHeader.Rows[TestDeviceColumnIndex][1]);
            double PassDeviceCount = Convert.ToInt32(tblHeader.Rows[TestDeviceColumnIndex + 1][1]);
            double FailDeviceCount = Convert.ToInt32(tblHeader.Rows[TestDeviceColumnIndex + 2][1]);

            #region *** Display the result ***
            strFailureRate = "#,Failure device count, % of failures,% of total devices, Failure Parameter" + "\r\n";
            TabPage tabFailureRate = new TabPage(strFailureRateTabName);
            tabcontrol.Controls.Add(tabFailureRate);
            tabcontrol.SelectedTab = tabFailureRate;
            tabFailureRate.AutoScroll = true;

            foreach (FailureMode failureMode in failureRateList) //for failureRateList
            {
                StringBuilder sbFailureMode = new StringBuilder();
                //#
                sbFailureMode.Append("#" + intFailureModeCount.ToString("00") + " , ");
                //Failure Device Count
                sbFailureMode.Append(failureMode.Count.ToString() + " , ");
                //% of failures
                double FailureMode_Percentage = (Convert.ToDouble(failureMode.Count) / FailDeviceCount) * 100;
                sbFailureMode.Append(FailureMode_Percentage.ToString("00.00") + "% , ");
                //% of total devices
                double FailureMode_Percentage_Total = (Convert.ToDouble(failureMode.Count) / TestDeviceCount) * 100;
                sbFailureMode.Append(FailureMode_Percentage_Total.ToString("00.00") + "% , ");
                //Failure mode
                sbFailureMode.AppendLine(failureMode.Parameter);

                LinkLabel lblFailureMode = new LinkLabel();
                lblFailureMode.AutoSize = true;
                if (!Convert.ToBoolean(intFailureModeCount % 2))
                    lblFailureMode.LinkColor = Color.Black;
                lblFailureMode.LinkBehavior = LinkBehavior.NeverUnderline;
                lblFailureMode.Text = sbFailureMode.ToString();
                lblFailureMode.Links.Add(0, lblFailureMode.Text.Length, failureMode.Device);    //Transfer device no
                //lblFailureMode.AutoSize = false;
                //lblFailureMode.Width = SplitContainer.Panel2.Bounds.Width - 30;
                //lblFailureMode.UseCompatibleTextRendering = true;
                lblFailureMode.Location = new Point(10, 25 * intFailureModeCount);
                lblFailureMode.LinkClicked += new LinkLabelLinkClickedEventHandler(lblFailureMode_LinkClicked);
                lblFailureMode.Name = "lblFailureMode" + intFailureModeCount.ToString();
                tabFailureRate.Controls.Add(lblFailureMode);

                //Save to global variable string for exprot
                strFailureRate += sbFailureMode.ToString();

                intFailureModeCount++;
            }//end of for failureRateList

            #endregion *** Display the result ***
            lblBar.Text = "Done";

        } //end of topFailParetoToolStripMenuItem_Click

        // *** KGU verify
        private void kGUVerifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region *** Variable Define ***

            db mydb = new db();

            DataTable tblGolden = new DataTable();
            DataTable tblKGURaw = new DataTable();

            DataRow drTemp;

            string strGoldenPath = "";
            string strKGUPath = "";
            string strKGUNumber = "";
            string strKGUNumberTemp = "";
            string strExtension = "";

            string[] strSafeFileName; ;
            string[] arrayKGU;
            string[] arrayKGUTemp;

            int intTotalKGU = 0;
            //int intSiteNumber = 0;

            bool cancelKGU = false;

            #endregion *** Variable Define ***

            #region *** Selected file ***
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.RestoreDirectory = true;
            OpenFile.Filter = "STDF Files(*.std/*.stdf)|*.std|All Files|*.*";
            //OpenFile.Filter = "STDF data file(*.std)|*.std|TXT data file(*.txt)|*.txt";
            if (OpenFile.ShowDialog() != DialogResult.OK) return;
            strKGUPath = OpenFile.FileName;
            strExtension = Path.GetExtension(strKGUPath);
            //Import KGU data
            if (strExtension.ToLower() == ".txt")
                tblKGURaw = _DataParse.GetDataFromTxt(strKGUPath);
            else if (strExtension.ToLower() == ".std" || strExtension.ToLower() == ".stdf")
                tblKGURaw = _DataParse.GetDataFromStdfviewer(strKGUPath);

            string[] tmp = _DataParse.Header.ProgramRev.Split('.');
            //strGoldenPath = Application.StartupPath + @".\GoldenSample\" + _DataParse.Header.Product.ToString() + "_" + _DataParse.Header.ProgramRev.Substring(0, 2)
            strGoldenPath = Application.StartupPath + @".\GoldenSample\" + _DataParse.Header.Product.ToString() + "_" + tmp[0].ToString();

            string vStrPath = Path.GetFullPath(strGoldenPath);
            intTotalKGU = _DataParse.Header.TestQuantity;

            strSafeFileName = OpenFile.SafeFileName.Split('_');
            for (int i = 0; i < strSafeFileName.Length; i++)
            {
                if (strSafeFileName[i].ToUpper() == "KGU")
                {
                    strKGUNumber = strSafeFileName[i + 1].ToString();
                }
            }
            strKGUNumberTemp = Microsoft.VisualBasic.Interaction.InputBox(
                "Please verify the KGU number is correct,                   if not, input the correct KGU number", "KGU Number",
                 strKGUNumber, 100, 100);
            if (strKGUNumberTemp == "") return;
            strKGUNumber = strKGUNumberTemp.Replace(" ", "");
            arrayKGU = strKGUNumber.Split('.');

            //intSiteNumber = _DataParse.GetSiteNumber(strKGUPath);

            if (arrayKGU.Length == intTotalKGU / 2)
            {
                arrayKGUTemp = strKGUNumber.Split('.');
                arrayKGU = new string[intTotalKGU];
                arrayKGU[0] = arrayKGUTemp[0];
                arrayKGU[1] = arrayKGUTemp[1];
                arrayKGU[2] = arrayKGUTemp[1];
                arrayKGU[3] = arrayKGUTemp[0];
            }
            else if (arrayKGU.Length != intTotalKGU)
            {
                while (arrayKGU.Length != intTotalKGU && !cancelKGU)
                {
                    //MessageBox.Show("KGU number is not in correct format", "KGU Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    strKGUNumberTemp = Microsoft.VisualBasic.Interaction.InputBox(
                        "KGU test data does not match with the KGU quantity , please check the KGU number or test data. \r\neach number must be seperated by dot (.)", "KGU Number", strKGUNumberTemp, 100, 100);
                    strKGUNumber = strKGUNumberTemp.Replace(" ", "");
                    arrayKGU = strKGUNumber.Split('.');
                    if (strKGUNumberTemp == "") cancelKGU = true;
                }
            }
            if (cancelKGU == true) return;

            #endregion *** Selected file ***

            lblBar.Text = "KGU analysising";
            this.Refresh();
            Application.DoEvents();
            #region *** Import KGU***
            //tblKGURaw = _DataParse.GetDataFromTxt(strKGUPath);
            //strGoldenPath = @".\GoldenSample\" + _DataParse.Header.Product.ToString() + "_" + _DataParse.Header.ProgramRev + ".raw";
            //string vStrPath = Path.GetFullPath(strGoldenPath);
            #endregion *** Import KGU***

            #region *** Import Golden data ***

            string t_Product = _DataParse.Header.Product.ToString();
            string t_Rev = tmp[0].Substring(1).ToString();
            try
            {
                tblGolden = mydb.GetGoldenData(t_Product, t_Rev);

                foreach (string strKGUSN in arrayKGU)
                {
                    string filterCriteria = "[..]='" + strKGUSN + "'";
                    DataRow[] dataRows = tblGolden.Select(filterCriteria);
                    if (dataRows.Length != 1) throw new Exception("KGU " + strKGUSN + " is not is database");
                }
            }
            catch //DataBase is not available, use csv file instead
            {
                lblBar.Text = "Can not find Golden Data in database, use ascii file instead.";
                tblGolden = _DataParse.GetDataFromCsv(strGoldenPath);
            }

            //Update header information
            if (strExtension.ToLower() == "txt")
                tblKGURaw = _DataParse.GetDataFromTxt(strKGUPath);
            else if (strExtension.ToLower() == "std")
                tblKGURaw = _DataParse.GetDataFromStd(strKGUPath);
            #endregion *** Import Golden data ***

            #region *** Merge DataTable ***

            #region Import used golden data into tblCaculate
            tblKGU = tblGolden.Clone();

            tblKGU.PrimaryKey = null;
            tblKGURaw.PrimaryKey = null;

            bool isHeaderAdded = false;
            foreach (string sKGU in arrayKGU)
            {
                foreach (DataRow dr in tblGolden.Rows)
                {
                    if (!isHeaderAdded)
                    {
                        if (tblGolden.Rows.IndexOf(dr) < 4)
                        {
                            tblKGU.ImportRow(dr);
                        }
                        if (tblGolden.Rows.IndexOf(dr) == 4)
                        {
                            drTemp = tblKGU.NewRow();
                            drTemp[0] = "Golden Sample Data";
                            tblKGU.Rows.Add(drTemp);
                            isHeaderAdded = true;
                        }
                    }
                    if (tblGolden.Rows.IndexOf(dr) > 3 && Convert.ToInt32(dr[0]) == Convert.ToInt32(sKGU))
                    {
                        dr[dr.ItemArray.Length - 1] = "";
                        tblKGU.ImportRow(dr);

                    }//end of if else
                }//end of foreach tblGolden
            }// end of foreach arrayKGU 

            #endregion *** Import used golden data into tblCaculate

            #region Import KGU data into tblCaculate and verify
            drTemp = tblKGU.NewRow();
            drTemp[0] = "KGU Test Data";
            tblKGU.Rows.Add(drTemp);
            int indexKGU = 0;

            // delete the soft bin and hard bin column
            if (tblKGU.Columns.Count == tblKGURaw.Columns.Count - 2)
            {
                tblKGURaw.Columns.RemoveAt(3);
                tblKGURaw.Columns.RemoveAt(2);
            }

            foreach (DataRow dr in tblKGURaw.Rows)
            {
                if (tblKGURaw.Rows.IndexOf(dr) > 3)
                {
                    dr[0] = arrayKGU[indexKGU];
                    dr[tblKGURaw.Columns.Count - 1] = "";

                    DataRow newRow = tblKGU.NewRow();
                    for (int i = 0; i < tblKGURaw.Columns.Count; i++)
                    {
                        newRow[i] = dr[i];
                    }
                    tblKGU.Rows.Add(newRow);
                    indexKGU++;
                }
            }
            #endregion Import KGU data into tblCaculate and verify

            #region Caculate delta
            drTemp = tblKGU.NewRow();
            drTemp[0] = "Delta";
            tblKGU.Rows.Add(drTemp);
            for (int i = 0; i < intTotalKGU; i++)
            {
                drTemp = tblKGU.NewRow();
                drTemp[0] = arrayKGU[i];
                drTemp[1] = Convert.ToDouble(tblKGU.Rows[i + intTotalKGU + 6][1]);
                drTemp[2] = arrayKGU[i];
                for (int j = 3; j < tblKGU.Columns.Count - 1; j++)
                {
                    double dblKGU = Convert.ToDouble(tblKGU.Rows[i + intTotalKGU + 6][j]);
                    double dblGolden = Convert.ToDouble(tblKGU.Rows[i + 5][j]);
                    drTemp[j] = Math.Round(dblKGU - dblGolden, 3);
                }
                tblKGU.Rows.Add(drTemp);
            }
            #endregion Caculate delta

            #endregion *** Merge DataTable ***

            #region *** Display ***
            //Display
            this.UpdateSessionInfomation();
            this.RemoveTab(strKGUTabName);
            TabPage tabKGU = new TabPage(strKGUTabName);
            tabcontrol.Controls.Add(tabKGU);
            tabcontrol.SelectedTab = tabKGU;

            DataGridView dgvKGU = new DataGridView();
            tabKGU.Controls.Add(dgvKGU);
            dgvKGU.Dock = DockStyle.Fill;

            dgvKGU.DataSource = tblKGU;

            float currentSize = dgvKGU.Font.SizeInPoints - 1;
            for (int i = 0; i < dgvKGU.Columns.Count; i++)
            {
                //dgvKGU.Columns[i].Width = 60;
                dgvKGU.Columns[i].ReadOnly = true;
                dgvKGU.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvKGU.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvKGU.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dgvKGU.EnableHeadersVisualStyles = true;
                dgvKGU.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                DataGridViewCellStyle style = dgvKGU.ColumnHeadersDefaultCellStyle;
                style.Font = new Font(dgvKGU.Font, FontStyle.Bold);
                style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            for (int i = 0; i < 4; i++)
            {
                dgvKGU.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                dgvKGU.Rows[i].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", currentSize, FontStyle.Bold);
            }
            dgvKGU.Columns[0].Width = 130;
            dgvKGU.Columns[1].Width = 50;
            dgvKGU.Columns[2].Width = 60;
            dgvKGU.Columns[0].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", currentSize, FontStyle.Bold);
            dgvKGU.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;
            dgvKGU.Columns[1].DefaultCellStyle.BackColor = Color.LightGray;
            dgvKGU.Columns[2].DefaultCellStyle.BackColor = Color.LightGray;
            dgvKGU.RowHeadersVisible = false;
            dgvKGU.Rows[3].Frozen = true;
            dgvKGU.Columns[2].Frozen = true;
            dgvKGU.ReadOnly = true;
            dgvKGU.AllowUserToAddRows = false;
            dgvKGU.AllowUserToResizeRows = false;

            //return;
            #endregion *** Display ***

            #region *** Caculate Pass Fail ***
            foreach (DataGridViewRow dgvr in dgvKGU.Rows)
            {
                if (dgvKGU.Rows.IndexOf(dgvr) > 2 * intTotalKGU + 6)
                {
                    bool isKGUFail = false;
                    foreach (DataGridViewCell dgvc in dgvr.Cells)
                    {
                        string enable = dgvKGU.Rows[3].Cells[dgvc.ColumnIndex].Value.ToString();
                        if (enable == "1")
                        {
                            double lowTolerance = 0 - Convert.ToDouble(dgvKGU.Rows[1].Cells[dgvc.ColumnIndex].Value);
                            double UpperTolerance = Convert.ToDouble(dgvKGU.Rows[2].Cells[dgvc.ColumnIndex].Value);
                            double delta = Convert.ToDouble(dgvc.Value);

                            if (delta < lowTolerance || delta > UpperTolerance)
                            {
                                //dgvr.DefaultCellStyle.BackColor = Color.Red;
                                dgvc.Style.BackColor = Color.Red;
                                isKGUFail = true;
                            }
                        }
                    } // end of foreach DataGridViewCell dgvc
                    if (isKGUFail)
                    {
                        dgvr.Cells[dgvKGU.ColumnCount - 1].Value = "Fail";
                        dgvr.Cells[0].Style.BackColor = Color.Red;
                    }
                    else
                    {
                        dgvr.Cells[dgvKGU.ColumnCount - 1].Value = "Pass";
                        dgvr.Cells[0].Style.BackColor = Color.Green;
                    }
                }//end of if (dgvKGU.Rows.IndexOf(dgvr) > 16)
            }//end of  foreach (DataGridViewRow dgvr

            #endregion *** Caculate Pass Fail ***
            lblBar.Text = "Done";
            this.Refresh();

        } // end of kGUToolStripMenuItem_Click

        // *** AceTech AceTech Verify
        private void AceTechKGUVerify(DataTable tblKGURaw, string[] arrayKGU, int intTotalKGU)
        {
            #region *** Variable Define ***
            DataTable tblGolden = new DataTable();

            DataRow drTemp;

            string strGoldenPath = "";            

            #endregion *** Variable Define ***

            strGoldenPath = Application.StartupPath + @".\GoldenSample\" + _DataParse.Header.Product.ToString() + "_AceTech";
            lblBar.Text = "KGU analysising";
            this.Refresh();
            Application.DoEvents();
 
            #region *** Import Golden data ***

            tblGolden = _DataParse.GetDataFromCsv(strGoldenPath);

            #endregion *** Import Golden data ***

            try
            {
                #region *** Merge DataTable ***

                #region Import used golden data into tblCaculate
                tblKGU = tblGolden.Clone();

                tblKGU.PrimaryKey = null;
                tblKGURaw.PrimaryKey = null;

                bool isHeaderAdded = false;
                foreach (string sKGU in arrayKGU)
                {
                    foreach (DataRow dr in tblGolden.Rows)
                    {
                        if (!isHeaderAdded)
                        {
                            if (tblGolden.Rows.IndexOf(dr) < 4)
                            {
                                tblKGU.ImportRow(dr);
                            }
                            if (tblGolden.Rows.IndexOf(dr) == 4)
                            {
                                drTemp = tblKGU.NewRow();
                                drTemp[0] = "Golden Sample Data";
                                tblKGU.Rows.Add(drTemp);
                                isHeaderAdded = true;
                            }
                        }
                        if (tblGolden.Rows.IndexOf(dr) > 3 && Convert.ToInt32(dr[0]) == Convert.ToInt32(sKGU))
                        {
                            dr[dr.ItemArray.Length - 1] = "";
                            tblKGU.ImportRow(dr);

                        }//end of if else
                    }//end of foreach tblGolden
                }// end of foreach arrayKGU 

                #endregion *** Import used golden data into tblCaculate

                #region Import KGU data into tblCaculate and verify
                drTemp = tblKGU.NewRow();
                drTemp[0] = "KGU Test Data";
                tblKGU.Rows.Add(drTemp);
                int indexKGU = 0;

                // delete the soft bin and hard bin column
                if (tblKGU.Columns.Count == tblKGURaw.Columns.Count - 2)
                {
                    tblKGURaw.Columns.RemoveAt(3);
                    tblKGURaw.Columns.RemoveAt(2);
                }

                foreach (DataRow dr in tblKGURaw.Rows)
                {
                    if (tblKGURaw.Rows.IndexOf(dr) > 3)
                    {
                        dr[0] = arrayKGU[indexKGU];
                        dr[tblKGURaw.Columns.Count - 1] = "";

                        DataRow newRow = tblKGU.NewRow();
                        for (int i = 0; i < tblKGURaw.Columns.Count; i++)
                        {
                            newRow[i] = dr[i];
                        }
                        tblKGU.Rows.Add(newRow);
                        indexKGU++;
                    }
                }
                #endregion Import KGU data into tblCaculate and verify

                #region Caculate delta
                try
                {
                    drTemp = tblKGU.NewRow();
                    drTemp[0] = "Delta";
                    tblKGU.Rows.Add(drTemp);
                    for (int i = 0; i < intTotalKGU; i++)
                    {
                        drTemp = tblKGU.NewRow();
                        drTemp[0] = arrayKGU[i];
                        drTemp[1] = Convert.ToDouble(tblKGU.Rows[i + intTotalKGU + 6][1]);                       
                        drTemp[2] = arrayKGU[i];
                        for (int j = 3; j < tblKGU.Columns.Count - 1; j++)
                        {
                            double dblKGU = Convert.ToDouble(tblKGU.Rows[i + intTotalKGU + 6][j]);
                            double dblGolden = Convert.ToDouble(tblKGU.Rows[i + 5][j]);
                            drTemp[j] = Math.Round(dblKGU - dblGolden, 3);
                        }
                        tblKGU.Rows.Add(drTemp);
                    }
                }
                catch { }

                #endregion Caculate delta

                #endregion *** Merge DataTable ***
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            #region *** Display ***
            //Display
            this.UpdateSessionInfomation();
            this.RemoveTab(strKGUTabName);
            TabPage tabKGU = new TabPage(strKGUTabName);
            tabcontrol.Controls.Add(tabKGU);
            tabcontrol.SelectedTab = tabKGU;

            DataGridView dgvKGU = new DataGridView();
            tabKGU.Controls.Add(dgvKGU);
            dgvKGU.Dock = DockStyle.Fill;

            dgvKGU.DataSource = tblKGU;

            float currentSize = dgvKGU.Font.SizeInPoints - 1;
            for (int i = 0; i < dgvKGU.Columns.Count; i++)
            {
                //dgvKGU.Columns[i].Width = 60;
                dgvKGU.Columns[i].ReadOnly = true;
                dgvKGU.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvKGU.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvKGU.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dgvKGU.EnableHeadersVisualStyles = true;
                dgvKGU.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                DataGridViewCellStyle style = dgvKGU.ColumnHeadersDefaultCellStyle;
                style.Font = new Font(dgvKGU.Font, FontStyle.Bold);
                style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            for (int i = 0; i < 4; i++)
            {
                dgvKGU.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                dgvKGU.Rows[i].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", currentSize, FontStyle.Bold);
            }
            dgvKGU.Columns[0].Width = 130;
            dgvKGU.Columns[1].Width = 50;
            dgvKGU.Columns[2].Width = 60;
            dgvKGU.Columns[0].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", currentSize, FontStyle.Bold);
            dgvKGU.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;
            dgvKGU.Columns[1].DefaultCellStyle.BackColor = Color.LightGray;
            dgvKGU.Columns[2].DefaultCellStyle.BackColor = Color.LightGray;
            dgvKGU.RowHeadersVisible = false;
            dgvKGU.Rows[3].Frozen = true;
            dgvKGU.Columns[2].Frozen = true;
            dgvKGU.ReadOnly = true;
            dgvKGU.AllowUserToAddRows = false;
            dgvKGU.AllowUserToResizeRows = false;

            //return;
            #endregion *** Display ***

            #region *** Caculate Pass Fail ***
            foreach (DataGridViewRow dgvr in dgvKGU.Rows)
            {
                if (dgvKGU.Rows.IndexOf(dgvr) > 2 * intTotalKGU + 6)
                {
                    bool isKGUFail = false;
                    foreach (DataGridViewCell dgvc in dgvr.Cells)
                    {
                        string enable = dgvKGU.Rows[3].Cells[dgvc.ColumnIndex].Value.ToString();
                        if (enable == "1")
                        {
                            double lowTolerance = 0 - Convert.ToDouble(dgvKGU.Rows[1].Cells[dgvc.ColumnIndex].Value);
                            double UpperTolerance = Convert.ToDouble(dgvKGU.Rows[2].Cells[dgvc.ColumnIndex].Value);
                            double delta = Convert.ToDouble(dgvc.Value);

                            if (delta < lowTolerance || delta > UpperTolerance)
                            {
                                //dgvr.DefaultCellStyle.BackColor = Color.Red;
                                dgvc.Style.BackColor = Color.Red;
                                isKGUFail = true;
                            }
                        }
                    } // end of foreach DataGridViewCell dgvc
                    if (isKGUFail)
                    {
                        dgvr.Cells[dgvKGU.ColumnCount - 1].Value = "Fail";
                        dgvr.Cells[0].Style.BackColor = Color.Red;
                    }
                    else
                    {
                        dgvr.Cells[dgvKGU.ColumnCount - 1].Value = "Pass";
                        dgvr.Cells[0].Style.BackColor = Color.Green;
                    }
                }//end of if (dgvKGU.Rows.IndexOf(dgvr) > 16)
            }//end of  foreach (DataGridViewRow dgvr

            #endregion *** Caculate Pass Fail ***
            lblBar.Text = "Done";
            this.Refresh();
        }
        // *** Cpk & Limit
        private void cpkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lblBar.Text = "Cpk Caculating";
            this.Refresh();
            Application.DoEvents();
            tblCpk = _Analysis.CaculateCpk(tblData, _DataParse.FreezeColumn);
            
            #region *** Display ***
            this.RemoveTab(strCpkTabName);
            TabPage tabCpk = new TabPage(strCpkTabName);
            tabCpk.Name = strCpkTabName;
            tabcontrol.Controls.Add(tabCpk);
            tabcontrol.SelectedTab = tabCpk;
            tabCpk.AutoScroll = true;

            Panel CpkPanel2 = new Panel();
            DataGridView dgvCpk = new DataGridView();
            dgvCpk.Name = "dgvCpk";
            dgvCpk.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;// | AnchorStyles.Bottom;
            dgvCpk.Dock = DockStyle.Top;
            dgvCpk.Height = SplitContainer.Panel2.Height - 65;
            dgvCpk.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvCpk_CellFormatting);
            dgvCpk.CellValueChanged += new DataGridViewCellEventHandler(dgvCpk_CellValueChanged);

            tabCpk.Controls.Add(dgvCpk);
            dgvCpk.DataSource = tblCpk;
            this.dgvCpkGridViewFormat();

            Panel CpkPanel1 = new Panel();
            CpkPanel1.Dock = DockStyle.Top;
            CpkPanel1.Height = 35;
            tabCpk.Controls.Add(CpkPanel1);

            Button btnReCaculate = new Button();
            btnReCaculate.Top = 5;
            btnReCaculate.Text = "ReCaculate";
            CpkPanel1.Controls.Add(btnReCaculate);
            btnReCaculate.Click += new EventHandler(btnReCaculate_Click);

            Label lblYieldImpact = new Label();
            double dblYield = Convert.ToDouble(_DataParse.PassedDevice) / Convert.ToDouble(_DataParse.TestedDevice);
            lblYieldImpact.Text = "Yield = " + Math.Round(dblYield * 100, 2) + "%";
            lblYieldImpact.Name = "YieldImpact";
            lblYieldImpact.Left = 90;
            lblYieldImpact.Top = 10;
            lblYieldImpact.AutoSize = true;
            CpkPanel1.Controls.Add(lblYieldImpact);

            #endregion *** Display ***
            lblBar.Text = "Done";
            this.Refresh();
        }

        #endregion *** Data Analysis ***

        #region *** Golden Sample ***
        private void importKGUDataToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion *** Golden Sample ***



        private void buildKGUFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportGold bk = new ImportGold();
            bk.ShowDialog();

            #region ---
            //DataTable tblKGUData = new DataTable();
            //DataHeader tHeader = new DataHeader();
            //tHeader = _DataParse.Header;

            //#region *** Selected file ***
            //OpenFileDialog openfiledialog1 = new OpenFileDialog();
            //openfiledialog1.Title = "Open original KGU file";
            //if (openfiledialog1.ShowDialog() != DialogResult.OK) return;
            //string strfilename = openfiledialog1.FileName;

            //#endregion *** Selected file ***

            //#region *** Parsing data ***
            //lblBar.Text = "Building KGU file";
            //Application.DoEvents();
            //dgvData.DataSource = null;
            ////this.Refresh();
            //try
            //{
            //    tblKGUData = _DataParse.GetDataFromCsv(strfilename);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}

            //#endregion *** Parsing data ***

            //#region *** Save file ***
            //SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //saveFileDialog1.Title = "Build KGU file";
            ////saveFileDialog1.Filter = "CSV File|*.csv";

            //saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //if (tblKGUData.Rows.Count == 0)
            //{
            //    MessageBox.Show("No Test data need to be saved", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            //saveFileDialog1.Title = "Save Test Data as";
            //saveFileDialog1.FileName = "";
            //if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            //StreamWriter swData = new StreamWriter(saveFileDialog1.FileName);
            //StringBuilder sbContent = new StringBuilder();
            ////Header
            ////sbContent.Append(_DataParse.Header.

            //foreach (DataRow drTemp in tblKGUData.Rows)
            //{
            //    sbContent = new StringBuilder();
            //    int RowIndex = tblKGUData.Rows.IndexOf(drTemp);

            //    if (RowIndex == 0)
            //    {
            //        sbContent.Append("Test_Num ");
            //        for (int i = 3; i < tblKGUData.Columns.Count - 3; i++)
            //        {
            //            sbContent.Append(i - 2);
            //            sbContent.Append(" ");
            //        }
            //        swData.WriteLine(sbContent.ToString());

            //        sbContent = new StringBuilder();
            //        drTemp[2] = "Test_Item";
            //    }
            //    else if (RowIndex == 1)
            //    {
            //        drTemp[2] = "L_Tol";
            //    }
            //    else if (RowIndex == 2)
            //    {
            //        drTemp[2] = "H_Tol";
            //    }
            //    else if (RowIndex == 3)
            //    {
            //        drTemp[2] = "Enable";
            //    }

            //    for (int i = 2; i < tblKGUData.Columns.Count - 3; i++)
            //    {
            //        string strTemp;
            //        if (RowIndex == 0)
            //        {
            //            strTemp = drTemp[i].ToString().Replace(" ", "_");
            //        }
            //        else if (RowIndex == 1 || RowIndex == 2)
            //        {
            //            if (drTemp[i].ToString() == "")
            //                strTemp = "0";
            //            else
            //                strTemp = drTemp[i].ToString();
            //        }
            //        else
            //        {
            //            strTemp = drTemp[i].ToString();
            //        }

            //        sbContent.Append(strTemp);
            //        sbContent.Append(" ");
            //    }
            //    //sbContent.AppendLine(drTemp[tblKGUData.Columns.Count - 2].ToString());
            //    swData.WriteLine(sbContent.ToString());
            //}
            //swData.Close();
            //lblBar.Text = lblBar.Text + "...Done";

            //#endregion *** Save file ***

            //_DataParse.Header = tHeader;
            #endregion ---

        }

        private void dBTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            db mydb = new db();

            dgvData.DataSource = mydb.GetGoldenData("5348","1");
        }

        private void kGUToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GoldenSample gs = new GoldenSample();
            gs.ShowDialog();
        }






    }
}