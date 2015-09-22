///     Reversion history log
///     Rev1.0      Initie build                                                                AceLi       2011-10-21
///     Rev1.1      Add dual site data fetch function                                           AceLi       2012-02-25
///     Rev1.2      Fixed some bug in dual site data fetch                                      Ace Li      2012-03-06
///     Rev1.2.1    Fixed some bug when test data file is incompelete                           Ace Li      2012-03-24
///     Rev1.9.0.0  Add STDF file parse solution


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using LumenWorks.Framework.IO.Csv;
using Vanchip.Common;
using System.ComponentModel;
using STDF_V4;


namespace Vanchip.Data
{
    /// <summary>
    /// Export data class
    /// </summary>
    public class Export
    {
        #region *** Method ***
        /// <summary>
        /// Save all contents of datatable into csv file
        /// </summary>
        /// <param name="fileName">Full file path to write to. (include file name)</param>
        /// <param name="dataTable">datatable to be writed</param>
        public void DataTableToCsv(string fileName, DataTable dataTable)
        {
            StreamWriter swData = new StreamWriter(fileName);
            StringBuilder sbTitle = new StringBuilder();

            try  //Try #1
            {
                //Save test data title
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    if (i != 0)
                        sbTitle.Append(',');
                    sbTitle.Append(dataTable.Columns[i].ColumnName);
                }
                swData.WriteLine(sbTitle.ToString());

                //Build and save datatable
                foreach (DataRow drTemp in dataTable.Rows)
                {
                    StringBuilder sbData = new StringBuilder();

                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        if (i != 0)
                            sbData.Append(',');
                        sbData.Append(drTemp[i].ToString());
                    }
                    swData.WriteLine(sbData.ToString());
                }
                swData.Close();
            }   //end of Try #1
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Save all contents of datatable into csv file for import in JMP
        /// Without header, limit, units
        /// </summary>
        /// <param name="fileName">Full file path to write to. (include file name)</param>
        /// <param name="dataTable">datatable to be writed</param>
        public void DataTableToCsvForJMP(string fileName, DataTable dataTable, int[] Columns)
        {
            StreamWriter swData = new StreamWriter(fileName);
            StringBuilder sbTitle = new StringBuilder();

            try  //Try #1
            {
                ////Save test data title
                //for (int i = 0; i < dataTable.Columns.Count; i++)
                //{
                //    if (i != 0)
                //        sbTitle.Append(',');
                //    sbTitle.Append(dataTable.Columns[i].ColumnName);
                //}
                //swData.WriteLine(sbTitle.ToString());

                //Build and save datatable
                foreach (DataRow drTemp in dataTable.Rows)
                {
                    StringBuilder sbData = new StringBuilder();
                    int RowIndex = dataTable.Rows.IndexOf(drTemp);
                    if (RowIndex < 1 || RowIndex > 3)
                    {
                        //for (int i = 0; i < dataTable.Columns.Count; i++)
                        //{
                        //    if (i != 0)
                        //        sbData.Append(',');
                        //    sbData.Append(drTemp[i].ToString());
                        //}
                        sbData.Append(drTemp[2].ToString());
                        foreach (int i in Columns)
                        {
                            sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                }
                swData.Close();
            }   //end of Try #1
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Save all contents of datatable into csv file with header information
        /// </summary>
        /// <param name="fileName">Full file path to write to. (include file name)</param>
        /// <param name="dataTable">datatable to be writed</param>
        /// <param name="Header">String header information</param>
        /// <param name="isHasHeader">true or false</param>
        private void DataTableToCsv(string fileName, DataTable dataTable, string Header)
        {
            StreamWriter swData = new StreamWriter(fileName);
            StringBuilder sbHeader = new StringBuilder();
            StringBuilder sbTitle = new StringBuilder();

            try  //Try #1
            {
                //Save test data title
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    if (i != 0)
                        sbTitle.Append(',');
                    sbTitle.Append(dataTable.Columns[i].ColumnName);
                }
                swData.WriteLine(sbTitle.ToString());

                //Build and save datatable
                foreach (DataRow drTemp in dataTable.Rows)
                {
                    StringBuilder sbData = new StringBuilder();

                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        if (i != 0)
                            sbData.Append(',');
                        sbData.Append(drTemp[i].ToString());
                    }
                    swData.WriteLine(sbData.ToString());
                }

                //Save header
                swData.WriteLine("Header Section,");
                sbTitle.AppendLine(Header);
                sbTitle.AppendLine();
                swData.WriteLine(sbHeader.ToString());

                swData.Close();
            }   //end of Try #1
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }   //有问题

        /// <summary>
        /// Save all contents of datatable into csv file
        /// </summary>
        /// <param name="fileName">Full file path to write to. (include file name)</param>
        /// <param name="dataTable">datatable to be writed</param>
        /// <param name="Header">Struct Header header information</param>
        /// <param name="isHasHeader">true or false</param>
        public void DataTableToCsv(string fileName, DataTable dataTable, DataHeader Header)
        {
            if (fileName.Length > 200)
                fileName = fileName.Substring(0, 200);

            StreamWriter swData = new StreamWriter(fileName);
            StringBuilder sbHeader = new StringBuilder();
            StringBuilder sbTitle = new StringBuilder();

            try  //Try #1
            {
                //Build test data header
                sbHeader.Append("Header Section,");
                for (int i = 0; i < dataTable.Columns.Count - 2; i++)
                {
                    sbHeader.Append(",");   //补足”，“，否则import会报错
                }
                sbHeader.AppendLine();

                var type = typeof(DataHeader);
                var fields = type.GetFields();
                Array.ForEach(fields, f =>
                {
                    sbHeader.Append(f.Name);
                    sbHeader.Append(",");
                    sbHeader.Append(f.GetValue(Header));
                    for (int i = 0; i < dataTable.Columns.Count - 2; i++)
                    {
                        sbHeader.Append(",");   //补足”，“，否则import会报错
                    }
                    sbHeader.AppendLine();
                });
                swData.WriteLine(sbHeader.ToString());

                //Save test data title
                sbTitle.Append("Data Section,");
                for (int i = 0; i < dataTable.Columns.Count - 2; i++)
                {
                    sbTitle.Append(",");   //补足”，“，否则import会报错
                }
                sbTitle.AppendLine();

                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    if (i != 0)
                        sbTitle.Append(',');
                    sbTitle.Append(dataTable.Columns[i].ColumnName);
                }
                swData.WriteLine(sbTitle.ToString());

                //Build and save datatable
                foreach (DataRow drTemp in dataTable.Rows)
                {
                    StringBuilder sbData = new StringBuilder();

                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        if (i != 0)
                            sbData.Append(',');
                        sbData.Append(drTemp[i].ToString());
                    }
                    swData.WriteLine(sbData.ToString());
                }
                swData.Close();
            }   //end of Try #1
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Save all contents of string into csv file
        /// </summary>
        /// <param name="fileName">Full file path to write to. (include file name)</param>
        /// <param name="dataTable">string to be writed</param>
        /// <param name="Header">String header information</param>
        /// <param name="isHasHeader">true or false</param>
        public void StringToCsv(string fileName, string Content, string Header, bool isHasHeader)
        {
            FileStream _FileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter _StreamWriter = new StreamWriter(_FileStream);
            StringBuilder _StringBuilder = new StringBuilder();

            if (isHasHeader)
            {
                _StringBuilder.AppendLine(Header);
            }
            _StringBuilder.AppendLine(Content);

            _StreamWriter.Flush();
            _StreamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
            _StreamWriter.Write(_StringBuilder.ToString());
            _StreamWriter.Flush();
            _StreamWriter.Close();
        }

        #endregion *** Method ***

    }//end of Export


}//end of namespace Vanchip.Data
