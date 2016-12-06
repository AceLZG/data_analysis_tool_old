using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Data;
using System.Windows.Forms;
using System.Data.Common;

namespace STDF_Viewer
{
    public class CSVFileHelper
    {
        public static void SaveCSV(DataTable dt1, DataTable dt2, string fullPath)
        {
            FileInfo fi = new FileInfo(fullPath);

            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            FileStream fs = new FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            //StreamWriter sw = new StreamWriter(fs);
            string data = "";

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt1.Columns.Count; j++)
                {
                    string str = dt1.Rows[i][j].ToString();
                    str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                    if (str.Contains(',') || str.Contains('"')
                        || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                    {
                        str = string.Format("\"{0}\"", str);
                    }

                    data += str;
                    if (j < dt1.Columns.Count - 1)
                    {
                        data += ",";
                    }
                    //sw.Write(dt.Rows[i][j]);
                    //sw.Write(',');
                }
                sw.WriteLine(data);
            }
            sw.WriteLine();
            sw.WriteLine();
            //写出列名称
            data = "";
            for (int i = 0; i < dt2.Columns.Count; i++)
            {
                data += dt2.Columns[i].ColumnName.ToString();
                if (i < dt2.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);
            //写出各行数据
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt2.Columns.Count; j++)
                {
                    string str = dt2.Rows[i][j].ToString();
                    str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                    if (str.Contains(',') || str.Contains('"')
                        || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                    {
                        str = string.Format("\"{0}\"", str);
                    }

                    data += str;
                    if (j < dt2.Columns.Count - 1)
                    {
                        data += ",";
                    }
                    //sw.Write(dt.Rows[i][j]);
                    //sw.Write(',');
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
            DialogResult result = MessageBox.Show("CSV文件保存成功！");
            if (result == DialogResult.OK)
            {
                System.Diagnostics.Process.Start("explorer.exe", fullPath);
            }
        }

        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        //public static DataTable OpenCSV(string filePath)
        //{
        //    Encoding encoding = Common.GetType(filePath); //Encoding.ASCII;//
        //    DataTable dt = new DataTable();
        //    FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

        //    //StreamReader sr = new StreamReader(fs, Encoding.UTF8);
        //    StreamReader sr = new StreamReader(fs, encoding);
        //    //string fileContent = sr.ReadToEnd();
        //    //encoding = sr.CurrentEncoding;
        //    //记录每次读取的一行记录
        //    string strLine = "";
        //    //记录每行记录中的各字段内容
        //    string[] aryLine = null;
        //    string[] tableHead = null;
        //    //标示列数
        //    int columnCount = 0;
        //    //标示是否是读取的第一行
        //    bool IsFirst = true;
        //    //逐行读取CSV中的数据
        //    while ((strLine = sr.ReadLine()) != null)
        //    {
        //        //strLine = Common.ConvertStringUTF8(strLine, encoding);
        //        //strLine = Common.ConvertStringUTF8(strLine);

        //        if (IsFirst == true)
        //        {
        //            tableHead = strLine.Split(',');
        //            IsFirst = false;
        //            columnCount = tableHead.Length;
        //            //创建列
        //            for (int i = 0; i < columnCount; i++)
        //            {
        //                DataColumn dc = new DataColumn(tableHead[i]);
        //                dt.Columns.Add(dc);
        //            }
        //        }
        //        else
        //        {
        //            aryLine = strLine.Split(',');
        //            DataRow dr = dt.NewRow();
        //            for (int j = 0; j < columnCount; j++)
        //            {
        //                dr[j] = aryLine[j];
        //            }
        //            dt.Rows.Add(dr);
        //        }
        //    }
        //    if (aryLine != null && aryLine.Length > 0)
        //    {
        //        dt.DefaultView.Sort = tableHead[0] + " " + "asc";
        //    }

        //    sr.Close();
        //    fs.Close();
        //    return dt;
        //}
    }
}
