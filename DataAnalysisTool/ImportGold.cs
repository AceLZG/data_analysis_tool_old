using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Vanchip.Data;
using Vanchip.Common;

namespace DataAnalysisTool
{
    public partial class ImportGold : Form
    {
        private string strGoldenPath = @".\GoldenSample\";
        DataParse _DataParse = new DataParse();
        db mydb = new db();

        public ImportGold()
        {
            InitializeComponent();
            lbxProduct.SelectedIndexChanged += new EventHandler(lbxProduct_SelectedIndexChanged);
        }

        private void lbxProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fileName = strGoldenPath + lbxProduct.SelectedItem.ToString();
            string lineConent = "";
            tbxComment.Text = "";

            #region *** Read to memory ***
            //string content = string.Empty;
            //using (StreamReader ms_StreamReader = new StreamReader(fileName))
            //{
            //    content = ms_StreamReader.ReadToEnd();//一次性读入内存
            //}
            //MemoryStream _MemoryStream = new MemoryStream(Encoding.GetEncoding("GB2312").GetBytes(content));//放入内存流，以便逐行读取
            //StreamReader _StreamReader = new StreamReader(_MemoryStream);
            #endregion *** Read to memory ***

            #region *** Direct Read ***
            FileStream _FileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader _StreamReader = new StreamReader(_FileStream);
            _StreamReader.BaseStream.Seek(0, SeekOrigin.Begin);

            #endregion *** Diresct Read ***

            lineConent = _StreamReader.ReadLine();
            StringBuilder sbHeader = new StringBuilder();
            while (lineConent != null && !lineConent.Contains("Data"))
            {
                if (lineConent.Contains("Header")) lineConent = _StreamReader.ReadLine();
                sbHeader.Append(lineConent);

                string[] strTemp = lineConent.Split(',');
                if (strTemp[0] == "Product")
                    tbxProduct.Text = strTemp[1].ToString();
                else if (strTemp[0] == "Release Date")
                {
                    try
                    {
                        DateTime dtTemp = Convert.ToDateTime(strTemp[1]);
                        tbxDateTime.Text = dtTemp.Year.ToString() + "/" + dtTemp.Month.ToString() + "/" + dtTemp.Day.ToString() + " " + dtTemp.ToLongTimeString();
                    }
                    catch
                    {
                        tbxDateTime.Text = datetime_now();
                    }
                }
                else if (strTemp[0] == "Comment")
                    tbxComment.Text = strTemp[1].ToString();

                lineConent = _StreamReader.ReadLine();
            }

            string[] strTempRev = fileName.Split('_');
            tbxRev.Text = strTempRev[1].Substring(1).ToString();

            tbxLastDateTime.Text = datetime_now();

            btnImport.BackColor = Color.Orange;
            btnImport.Text = "Import Data";
            btnImport.Enabled = true;
        }

        private void ImportGold_Load(object sender, EventArgs e)
        {
            DirectoryInfo DI = new DirectoryInfo(strGoldenPath);
            FileInfo[] FI = DI.GetFiles();

            foreach (FileInfo fi in FI)
            {
                if (fi.Extension == ".csv" || fi.Extension == "")
                {
                    lbxProduct.Items.Add(fi.Name);
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            btnImport.BackColor = Color.Yellow;
            btnImport.Text = "Importing...";
            btnImport.Enabled = false;

            #region *** Variables ***
            lblStatus.Text = "Importing Gold Data!";
            this.Refresh();

            string fileName = strGoldenPath + lbxProduct.SelectedItem.ToString();

            DataTable tblKGUData = new DataTable();
            DataHeader tHeader = new DataHeader();
            tHeader = _DataParse.Header;
            string[] strCriteria = new string[6];

            strCriteria[0] = tbxProduct.Text;
            strCriteria[1] = tbxRev.Text;
            strCriteria[2] = tbxDateTime.Text;
            strCriteria[3] = tbxLastDateTime.Text;
            strCriteria[4] = tbxComment.Text;
            strCriteria[5] = "ACE";
            #endregion *** Variables ***

            #region *** Parsing data ***
            try
            {
                tblKGUData = _DataParse.GetDataFromCsv(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            #endregion *** Parsing data ***

            //Insert golden data and return the golden data which device id is already in database
            #region *** Insert Database ***
            DataTable tblExist = mydb.InsertGoldenData(tblKGUData, strCriteria, false);
            lblStatus.Text = tblKGUData.Rows.Count - tblExist.Rows.Count + " Golden data has been imported Successfully!";
            #endregion *** Insert Database ***

            //Update(Overwrite) these data
            #region *** Update database ***
            if (tblExist.Rows.Count > 4)
            {
                string DeviceID = "";
                for (int i = 4; i < tblExist.Rows.Count; i++)
                {
                    DeviceID += tblExist.Rows[i][2].ToString() + ", ";
                }

                if (MessageBox.Show("The following DeviceID is already in the database, do you want to update(Overwrite)?\r\n" + DeviceID, "Update", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    string strPW_o = "Vanchip@2013";
                    string strPW = "";
                    string strDesc = "Password is needed for procced";

                    while (true)
                    {
                        if (InputBox("Password", strDesc, ref strPW) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            lblStatus.Text += " / " + (tblExist.Rows.Count - 4) + " golden data has been ignored!";
                            break;
                        }

                        if (strPW == strPW_o)
                        {
                            this.Refresh();
                            bool blnSuccess = mydb.InsertGoldenData(tblKGUData, strCriteria);
                            lblStatus.Text = tblKGUData.Rows.Count - 4 + " Golden Data has been imported Successfully!";
                            break;
                        }
                        else
                        {
                            strDesc = "Wrong password, try again!";
                        }
                    }

                }

            }
            #endregion *** Update database ***

            MessageBox.Show(lblStatus.Text);

            btnImport.BackColor = Color.Green;
            btnImport.Text = "Imported";
            btnImport.Enabled = false;
            // Comment Out
            #region *** Save datatable to ascii file ***
            //SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //if (tblKGUData.Rows.Count == 0)
            //{
            //    MessageBox.Show("No Test data need to be saved", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            //saveFileDialog1.Title = "Save KGU file as";
            //saveFileDialog1.FileName = lbxProduct.SelectedItem.ToString() + "_LTX";
            //saveFileDialog1.Filter = "ASCII File|*.*";
            //if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            //StreamWriter swData = new StreamWriter(saveFileDialog1.FileName);
            //StringBuilder sbContent = new StringBuilder();
            ////Header
            //swData.Write(sbHeader);

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
            //    if (RowIndex == tblKGUData.Rows.Count - 1)
            //        swData.Write(sbContent.ToString());
            //    else
            //        swData.WriteLine(sbContent.ToString());
            //}
            //swData.Close();

            //MessageBox.Show("KGU file build successfully, new KGU file has been saved to " + saveFileDialog1.FileName, "Build KGU", MessageBoxButtons.OK);

            #endregion *** Save file ***

        }

        #region *** Sub Function ***
        private void btnFillNow_Click(object sender, EventArgs e)
        {
            tbxDateTime.Text = datetime_now();
        }

        private string datetime_now()
        {
            return DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + " " + DateTime.Now.ToLongTimeString();
        }

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            textBox.UseSystemPasswordChar = true;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    
        #endregion *** Sub Function ***


    }
}
