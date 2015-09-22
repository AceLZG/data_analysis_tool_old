using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using Vanchip.Common;
using System.IO;

namespace Vanchip.Data
{
    public class db
    {
        public int available = 0;   // None is available

        //private string strPrefix = "Provider=Microsoft.Jet.OleDb.4.0;Jet OLEDB:Database Password=vc7810Eu_A6_01@VC5318_e1.8.1;Data Source=";
        private string strPrefix = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=";
        private string strMasterUrl = Util.MasterDB;
        private string strSlaveUrl = Util.SlaveDB;
        private string strPin = Util.Pin;

        OleDbConnection oleDbCon_Master = null;
        OleDbConnection oleDbCon = null;
        OleDbDataReader oleReader = null;

        public db()
        {
            string strMasterCon = strPrefix + strMasterUrl + strPin;
            string strSlaveCon = strPrefix + strSlaveUrl + strPin;

            //try
            //{
            //    File.Copy(strMasterUrl, strSlaveUrl, true);
            //    oleDbCon = new OleDbConnection(strMasterCon);
            //    available = 1;  // Master DB is abvailable
            //}
            //catch
            //{
            //    if (File.Exists(strSlaveUrl))
            //    {
            //        oleDbCon = new OleDbConnection(strSlaveCon);
            //        available = 2;  // Slave DB is abvailable
            //    }
            //    else
            //    {
            //        available = 0;   // None is available
            //    }
            //}

            available = 0;
        }

        public DataTable GetGoldenData(string Product, string TestRev)
        {
            #region *** Variable Define ***
            DataTable tblParseResult = new DataTable();
            DataRow dr;

            string strTestID = null;
            string strTestDesc = null;
            string strLowTol = null;
            string strHighTol = null;
            string strEnable = null;

            string Rev = "";

            OleDbDataReader oleDR = null;
            OleDbCommand olecmd = null;
            OleDbParameter p_Rev = null;
            OleDbParameter p_Product = null;

            #endregion *** Variable Define ***

            try
            {
                #region *** Tolerance ***
                //Get Latest Tolerance Rev
                Rev = GetGoldToleranceRev_Latest(Product);

                //Get Tolerance Info
                olecmd = new OleDbCommand("GetToleranceInfo_short", oleDbCon);
                olecmd.CommandType = CommandType.StoredProcedure;

                p_Product = new OleDbParameter("@Product", Product);
                olecmd.Parameters.Add(p_Product);
                p_Rev = new OleDbParameter("@Rev", Rev);
                olecmd.Parameters.Add(p_Rev);

                if (olecmd.Connection.State == ConnectionState.Closed) olecmd.Connection.Open();
                oleDR = olecmd.ExecuteReader();
                if (oleDR.HasRows)
                {
                    #region *** Generate DataTable Columns ***
                    tblParseResult.Columns.Add("Device#", typeof(string));
                    tblParseResult.Columns.Add(".", typeof(string));
                    tblParseResult.Columns.Add("..", typeof(string));

                    strTestDesc = "TestItem,Site No,Device ID";
                    strLowTol = "LowTolerance,,";
                    strHighTol = "HighTolerance,,";
                    strEnable = "Enable,,";

                    while (oleDR.Read())
                    {
                        strTestID = oleDR.GetInt16(1).ToString();
                        strTestDesc += "," + oleDR.GetString(2);
                        strLowTol += "," + oleDR.GetDouble(3).ToString();
                        strHighTol += "," + oleDR.GetDouble(4).ToString();
                        strEnable += "," + oleDR.GetInt16(5).ToString();

                        //generate columns
                        tblParseResult.Columns.Add(strTestID, typeof(string));
                    }
                    tblParseResult.Columns.Add("...", typeof(string));  //Status Columns

                    #endregion *** Generate DataTable Columns ***

                    #region *** Add Tolerance Info ***
                    //Add DataRow TestItem
                    int i = 0;
                    dr = tblParseResult.NewRow();
                    string[] strTemp = strTestDesc.Split(',');
                    for (i = 0; i < strTemp.Length; i++)
                    {
                        dr[i] = strTemp[i];
                    }
                    dr[i] = "Status";
                    tblParseResult.Rows.Add(dr);

                    //Add DataRow LowTolerance
                    dr = tblParseResult.NewRow();
                    strTemp = strLowTol.Split(',');
                    for (i = 0; i < strTemp.Length; i++)
                    {
                        dr[i] = strTemp[i];
                    }
                    dr[i] = "";
                    tblParseResult.Rows.Add(dr);

                    //Add DataRow HighTolerance
                    dr = tblParseResult.NewRow();
                    strTemp = strHighTol.Split(',');
                    for (i = 0; i < strTemp.Length; i++)
                    {
                        dr[i] = strTemp[i];
                    }
                    dr[i] = "";
                    tblParseResult.Rows.Add(dr);

                    //Add DataRow LowTolerance
                    dr = tblParseResult.NewRow();
                    strTemp = strEnable.Split(',');
                    for (i = 0; i < strTemp.Length; i++)
                    {
                        dr[i] = strTemp[i];
                    }
                    dr[i] = "";
                    tblParseResult.Rows.Add(dr);

                    #endregion *** Add Tolerance Info ***
                }
                #endregion *** Tolerance ***

                #region *** Gold Data ***
                olecmd = new OleDbCommand("GetGoldData_Crosstab", oleDbCon);
                olecmd.CommandType = CommandType.StoredProcedure;

                p_Product = new OleDbParameter("@Product", Product);
                olecmd.Parameters.Add(p_Product);
                OleDbParameter p_GoldRev = new OleDbParameter("@Rev", TestRev);
                olecmd.Parameters.Add(p_GoldRev);

                if (olecmd.Connection.State == ConnectionState.Closed) olecmd.Connection.Open();

                oleDR = olecmd.ExecuteReader();

                if (oleDR.HasRows)
                {
                    while (oleDR.Read())
                    {
                        int i = 0;
                        dr = tblParseResult.NewRow();
                        for (i = 0; i < oleDR.FieldCount; i++)
                        {
                            if (i == 0)
                            {
                                dr[i] = oleDR.GetValue(2).ToString();
                                i++;
                            }
                            dr[i] = oleDR.GetValue(i).ToString();
                        }
                        //Add DataRow GoldData
                        dr[i] = "1";
                        tblParseResult.Rows.Add(dr);
                    }
                }
                #endregion *** Gold Data ***
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                oleDR.Close();
                olecmd.Connection.Close();
            }

            return tblParseResult;
        }

        public DataTable GetGoldTolerane(string Product, string TestRev)
        {
            DataTable dtTemp = new DataTable();

            OleDbDataReader oleDR = null;
            OleDbCommand olecmd = null;
            

            #region *** Tolerance ***
            // Generate datatable columns
            dtTemp.Columns.Add("TestID", typeof(string));
            dtTemp.Columns.Add("Test Description", typeof(string));
            dtTemp.Columns.Add("Low Tolerance", typeof(string));
            dtTemp.Columns.Add("High Tolerance", typeof(string));
            dtTemp.Columns.Add("Enable", typeof(string));

            //Get Tolerance Info
            olecmd = new OleDbCommand("GetToleranceInfo", oleDbCon);
            olecmd.CommandType = CommandType.StoredProcedure;

            olecmd.Parameters.Add("@Product", Product);
            olecmd.Parameters.Add("@Rev", TestRev);

            if (olecmd.Connection.State == ConnectionState.Closed) olecmd.Connection.Open();
            oleDR = olecmd.ExecuteReader();
            if (oleDR.HasRows)
            {
                while (oleDR.Read())
                {
                    DataRow dr = dtTemp.NewRow();
                    dr[0] = oleDR.GetValue(0).ToString();
                    dr[1] = oleDR.GetValue(1).ToString();
                    dr[2] = oleDR.GetValue(2).ToString();
                    dr[3] = oleDR.GetValue(3).ToString();
                    dr[4] = oleDR.GetValue(4).ToString();
                    dtTemp.Rows.Add(dr);
                }
            }
            #endregion *** Tolerance ***

            return dtTemp;
        }

        //Get tolerance Rev list
        public string[] GetGoldToleranceRev(string Product)
        {

            #region *** Variable Define ***
            string[] RevList = null;
            string Rev = "";

            OleDbDataReader oleDR = null;
            OleDbCommand olecmd = null;

            #endregion *** Variable Define ***

            //Get Latest Tolerance Rev
            olecmd = new OleDbCommand("GetToleranceRev", oleDbCon);
            olecmd.CommandType = CommandType.StoredProcedure;

            olecmd.Parameters.Add("@Product", Product);

            if (olecmd.Connection.State == ConnectionState.Closed) olecmd.Connection.Open();
            oleDR = olecmd.ExecuteReader();
            if (oleDR.HasRows)
            {
                bool first = true;
                while (oleDR.Read())
                {
                    if (first)
                        Rev = oleDR.GetValue(0).ToString();
                    else
                    {
                        Rev += "," + oleDR.GetValue(0).ToString();
                    }
                }
            }
            RevList = Rev.Split(',');

            if (olecmd.Connection.State == ConnectionState.Open) olecmd.Connection.Close();
            return RevList;
        }

        //Get Latest tolerance rev
        public string GetGoldToleranceRev_Latest(string Product)
        {
            #region *** Variable Define ***
            string Rev = "";

            OleDbDataReader oleDR = null;
            OleDbCommand olecmd = null;

            #endregion *** Variable Define ***

            //Get Latest Tolerance Rev
            olecmd = new OleDbCommand("GetToleranceRev_Latest", oleDbCon);
            olecmd.CommandType = CommandType.StoredProcedure;

            olecmd.Parameters.Add("@Product", Product);

            if (olecmd.Connection.State == ConnectionState.Closed) olecmd.Connection.Open();
            oleDR = olecmd.ExecuteReader();
            if (oleDR.HasRows)
            {
                while (oleDR.Read())
                {
                    Rev = oleDR.GetValue(0).ToString();
                }
            }

            if (olecmd.Connection.State == ConnectionState.Open) olecmd.Connection.Close();
            return Rev;
        }

        public string GetToleranceProduct(string Product, bool Latest)
        {
            #region *** Variable Define ***
            string Rev = "";

            OleDbDataReader oleDR = null;
            OleDbCommand olecmd = null;

            #endregion *** Variable Define ***

            //Get Latest Tolerance Rev
            olecmd = new OleDbCommand("GetToleranceRev", oleDbCon);
            olecmd.CommandType = CommandType.StoredProcedure;

            olecmd.Parameters.Add("@Product", Product);

            if (olecmd.Connection.State == ConnectionState.Closed) olecmd.Connection.Open();
            oleDR = olecmd.ExecuteReader();
            if (oleDR.HasRows)
            {
                while (oleDR.Read())
                {
                    Rev = oleDR.GetValue(0).ToString();
                }
            }

            if (olecmd.Connection.State == ConnectionState.Open) olecmd.Connection.Close();
            return Rev;
        }

        public DataTable InsertGoldenData(DataTable dtGolden, string[] strCriteria, bool Update)
        {
            int i = 0;
            int DeviceID = 0;
            int TestID = 0;
            string Test_Desc = null;
            double Result;

            DataTable dtExist = new DataTable();
            dtExist = dtGolden.Clone();

            #region *** OLE Command Define ***
            // If Exist Command
            OleDbCommand olecmd_Exist = new OleDbCommand("GetDistinctSN", oleDbCon);
            olecmd_Exist.CommandType = CommandType.StoredProcedure;
            olecmd_Exist.Parameters.Add("@Product", strCriteria[0]);
            olecmd_Exist.Parameters.Add("@Rev", strCriteria[1]);
            olecmd_Exist.Parameters.Add("@SN", strCriteria[1]);     //Temp

            // Inser GoldData Command
            OleDbCommand olecmd_InsertData = new OleDbCommand("InsertGoldData", oleDbCon);
            olecmd_InsertData.CommandType = CommandType.StoredProcedure;
            olecmd_InsertData.Parameters.Add("@Product", strCriteria[0]);
            olecmd_InsertData.Parameters.Add("@Rev", strCriteria[1]);
            olecmd_InsertData.Parameters.Add("@SN", strCriteria[1]);            //Temp
            olecmd_InsertData.Parameters.Add("@TestID", strCriteria[1]);        //Temp
            olecmd_InsertData.Parameters.Add("@Test_Desc", strCriteria[1]);     //Temp
            olecmd_InsertData.Parameters.Add("@Result", strCriteria[1]);        //Temp

            // Inser GoldSummary Command
            OleDbCommand olecmd_InsertSummary = new OleDbCommand("InsertGoldDataSummary", oleDbCon);
            olecmd_InsertSummary.CommandType = CommandType.StoredProcedure;
            olecmd_InsertSummary.Parameters.Add("@Product", strCriteria[0]);
            olecmd_InsertSummary.Parameters.Add("@Rev", strCriteria[1]);
            olecmd_InsertSummary.Parameters.Add("@ReleaseDate", strCriteria[2]);
            olecmd_InsertSummary.Parameters.Add("@LastDate", strCriteria[3]);
            olecmd_InsertSummary.Parameters.Add("@Comment", strCriteria[4]);
            olecmd_InsertSummary.Parameters.Add("@OperatorID", strCriteria[5]);
            olecmd_InsertSummary.Parameters.Add("@SN", strCriteria[1]);         //Temp

            // Update GoldData Command
            OleDbCommand olecmd_UpdateData = new OleDbCommand("UpdateGoldData", oleDbCon);
            olecmd_UpdateData.CommandType = CommandType.StoredProcedure;
            olecmd_UpdateData.Parameters.Add("@Product", strCriteria[0]);
            olecmd_UpdateData.Parameters.Add("@Rev", strCriteria[1]);
            olecmd_UpdateData.Parameters.Add("@SN", strCriteria[0]);            //Temp
            olecmd_UpdateData.Parameters.Add("@TestID", strCriteria[1]);        //Temp
            olecmd_UpdateData.Parameters.Add("@Test_Desc", strCriteria[1]);     //Temp
            olecmd_UpdateData.Parameters.Add("@Result", strCriteria[1]);        //Temp

            // Update GoldSummary Command
            OleDbCommand olecmd_UpdateSummary = new OleDbCommand("UpdateGoldSummary", oleDbCon);
            olecmd_UpdateSummary.CommandType = CommandType.StoredProcedure;
            olecmd_UpdateSummary.Parameters.Add("@ReleaseDate", strCriteria[2]);
            olecmd_UpdateSummary.Parameters.Add("@LastDate", strCriteria[3]);
            olecmd_UpdateSummary.Parameters.Add("@Comment", strCriteria[4]);
            olecmd_UpdateSummary.Parameters.Add("@OperatorID", strCriteria[5]);
            olecmd_UpdateSummary.Parameters.Add("@Product", strCriteria[0]);
            olecmd_UpdateSummary.Parameters.Add("@Rev", strCriteria[1]);
            olecmd_UpdateSummary.Parameters.Add("@SN", strCriteria[1]);         //Temp

            #endregion *** OLE Command Define ***

            foreach (DataRow dr in dtGolden.Rows)
            {
                if (dtGolden.Rows.IndexOf(dr) < 4)
                {
                    dtExist.ImportRow(dr);
                    continue;
                }

                DeviceID = Convert.ToInt16(dr[2].ToString());

                olecmd_Exist.Parameters["@SN"].Value = DeviceID;

                if (olecmd_Exist.Connection.State == ConnectionState.Closed) olecmd_Exist.Connection.Open();
                OleDbDataReader oleDR;
                try
                {
                    oleDR = olecmd_Exist.ExecuteReader();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                // If exist then update, else insert into
                if (oleDR.HasRows)
                {
                    if (Update)
                    {
                        #region --- Update ---

                        OleDbTransaction trans = oleDbCon.BeginTransaction();
                        try
                        {
                            olecmd_UpdateSummary.Parameters["@SN"].Value = DeviceID;
                            olecmd_UpdateSummary.Transaction = trans;

                            olecmd_UpdateSummary.ExecuteNonQuery();

                            for (i = 3; i < dr.ItemArray.Count() - 1; i++)
                            {
                                TestID = i - 2;
                                Test_Desc = dtGolden.Rows[2][i].ToString();
                                Result = Convert.ToDouble(dr[i]);

                                olecmd_UpdateData.Parameters["@SN"].Value = DeviceID;
                                olecmd_UpdateData.Parameters["@TestID"].Value = TestID;
                                olecmd_UpdateData.Parameters["@Test_Desc"].Value = Test_Desc;
                                olecmd_UpdateData.Parameters["@Result"].Value = Result;
                                olecmd_UpdateData.Transaction = trans;

                                olecmd_UpdateData.ExecuteNonQuery();
                            }
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                        }

                        trans.Dispose();

                        #endregion --- Update ---
                    }
                    else //return none update data as datatable
                    {
                        dtExist.ImportRow(dr);
                    }
                }

                else
                {
                    #region --- Insert ---
                    OleDbTransaction trans = oleDbCon.BeginTransaction();
                    try
                    {
                        olecmd_InsertSummary.Parameters["@SN"].Value = DeviceID;
                        olecmd_InsertSummary.Transaction = trans;

                        olecmd_InsertSummary.ExecuteNonQuery();

                        for (i = 3; i < dr.ItemArray.Count() - 1; i++)
                        {
                            TestID = i - 2;
                            Test_Desc = dtGolden.Rows[0][i].ToString();
                            Result = Convert.ToDouble(dr[i]);

                            olecmd_InsertData.Parameters["@SN"].Value = DeviceID;
                            olecmd_InsertData.Parameters["@TestID"].Value = TestID;
                            olecmd_InsertData.Parameters["@Test_Desc"].Value = Test_Desc;
                            olecmd_InsertData.Parameters["@Result"].Value = Result;
                            olecmd_InsertData.Transaction = trans;

                            olecmd_InsertData.ExecuteNonQuery();

                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                    }

                    trans.Dispose();
                    #endregion --- Insert ---
                }

                if (!oleDR.IsClosed) oleDR.Close();


            }

            oleDbCon.Close();
            return dtExist;
        }
        
        public bool InsertGoldenData(DataTable dtGolden, string[] strCriteria)
        {
            int i = 0;
            int DeviceID = 0;
            int TestID = 0;
            string Test_Desc = null;
            double Result;

            #region *** OLE Command Define ***
            // If Exist Command
            OleDbCommand olecmd_Exist = new OleDbCommand("GetDistinctSN", oleDbCon);
            olecmd_Exist.CommandType = CommandType.StoredProcedure;
            olecmd_Exist.Parameters.Add("@Product", strCriteria[0]);
            olecmd_Exist.Parameters.Add("@Rev", strCriteria[1]);
            olecmd_Exist.Parameters.Add("@SN", strCriteria[1]);     //Temp

            // Inser GoldData Command
            OleDbCommand olecmd_InsertData = new OleDbCommand("InsertGoldData", oleDbCon);
            olecmd_InsertData.CommandType = CommandType.StoredProcedure;
            olecmd_InsertData.Parameters.Add("@Product", strCriteria[0]);
            olecmd_InsertData.Parameters.Add("@Rev", strCriteria[1]);
            olecmd_InsertData.Parameters.Add("@SN", strCriteria[1]);            //Temp
            olecmd_InsertData.Parameters.Add("@TestID", strCriteria[1]);        //Temp
            olecmd_InsertData.Parameters.Add("@Test_Desc", strCriteria[1]);     //Temp
            olecmd_InsertData.Parameters.Add("@Result", strCriteria[1]);        //Temp

            // Inser GoldSummary Command
            OleDbCommand olecmd_InsertSummary = new OleDbCommand("InsertGoldDataSummary", oleDbCon);
            olecmd_InsertSummary.CommandType = CommandType.StoredProcedure;
            olecmd_InsertSummary.Parameters.Add("@Product", strCriteria[0]);
            olecmd_InsertSummary.Parameters.Add("@Rev", strCriteria[1]);
            olecmd_InsertSummary.Parameters.Add("@ReleaseDate", strCriteria[2]);
            olecmd_InsertSummary.Parameters.Add("@LastDate", strCriteria[3]);
            olecmd_InsertSummary.Parameters.Add("@Comment", strCriteria[4]);
            olecmd_InsertSummary.Parameters.Add("@OperatorID", strCriteria[5]);
            olecmd_InsertSummary.Parameters.Add("@SN", strCriteria[1]);         //Temp

            // Update GoldData Command
            OleDbCommand olecmd_UpdateData = new OleDbCommand("UpdateGoldData", oleDbCon);
            olecmd_UpdateData.CommandType = CommandType.StoredProcedure;
            olecmd_UpdateData.Parameters.Add("@Product", strCriteria[0]);
            olecmd_UpdateData.Parameters.Add("@Rev", strCriteria[1]);
            olecmd_UpdateData.Parameters.Add("@SN", strCriteria[0]);            //Temp
            olecmd_UpdateData.Parameters.Add("@TestID", strCriteria[1]);        //Temp
            olecmd_UpdateData.Parameters.Add("@Test_Desc", strCriteria[1]);     //Temp
            olecmd_UpdateData.Parameters.Add("@Result", strCriteria[1]);        //Temp

            // Update GoldSummary Command
            OleDbCommand olecmd_UpdateSummary = new OleDbCommand("UpdateGoldSummary", oleDbCon);
            olecmd_UpdateSummary.CommandType = CommandType.StoredProcedure;
            olecmd_UpdateSummary.Parameters.Add("@ReleaseDate", strCriteria[2]);
            olecmd_UpdateSummary.Parameters.Add("@LastDate", strCriteria[3]);
            olecmd_UpdateSummary.Parameters.Add("@Comment", strCriteria[4]);
            olecmd_UpdateSummary.Parameters.Add("@OperatorID", strCriteria[5]);
            olecmd_UpdateSummary.Parameters.Add("@Product", strCriteria[0]);
            olecmd_UpdateSummary.Parameters.Add("@Rev", strCriteria[1]);
            olecmd_UpdateSummary.Parameters.Add("@SN", strCriteria[1]);         //Temp

            #endregion *** OLE Command Define ***

            foreach (DataRow dr in dtGolden.Rows)
            {
                if (dtGolden.Rows.IndexOf(dr) < 4) continue;

                DeviceID = Convert.ToInt16(dr[2].ToString());

                olecmd_Exist.Parameters["@SN"].Value = DeviceID;

                if (olecmd_Exist.Connection.State == ConnectionState.Closed) olecmd_Exist.Connection.Open();
                OleDbDataReader oleDR = olecmd_Exist.ExecuteReader();

                // If exist then update, else insert into
                if (oleDR.HasRows)
                {
                    #region --- Update ---
 
                    OleDbTransaction trans = oleDbCon.BeginTransaction();
                    try
                    {
                        olecmd_UpdateSummary.Parameters["@SN"].Value = DeviceID;
                        olecmd_UpdateSummary.Transaction = trans;

                        olecmd_UpdateSummary.ExecuteNonQuery();

                        for (i = 3; i < dr.ItemArray.Count() - 1; i++)
                        {
                            TestID = i - 2;
                            Test_Desc = dtGolden.Rows[2][i].ToString();
                            Result = Convert.ToDouble(dr[i]);

                            olecmd_UpdateData.Parameters["@SN"].Value = DeviceID;
                            olecmd_UpdateData.Parameters["@TestID"].Value = TestID;
                            olecmd_UpdateData.Parameters["@Test_Desc"].Value = Test_Desc;
                            olecmd_UpdateData.Parameters["@Result"].Value = Result;
                            olecmd_UpdateData.Transaction = trans;

                            olecmd_UpdateData.ExecuteNonQuery();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                    }

                    trans.Dispose();
                }
                    #endregion --- Update ---
                else
                {
                    #region --- Insert ---
                    OleDbTransaction trans = oleDbCon.BeginTransaction();
                    try
                    {
                        olecmd_InsertSummary.Parameters["@SN"].Value = DeviceID;
                        olecmd_InsertSummary.Transaction = trans;

                        olecmd_InsertSummary.ExecuteNonQuery();

                        for (i = 3; i < dr.ItemArray.Count() - 1; i++)
                        {
                            TestID = i - 2;
                            Test_Desc = dtGolden.Rows[0][i].ToString();
                            Result = Convert.ToDouble(dr[i]);

                            olecmd_InsertData.Parameters["@SN"].Value = DeviceID;
                            olecmd_InsertData.Parameters["@TestID"].Value = TestID;
                            olecmd_InsertData.Parameters["@Test_Desc"].Value = Test_Desc;
                            olecmd_InsertData.Parameters["@Result"].Value = Result;
                            olecmd_InsertData.Transaction = trans;

                            olecmd_InsertData.ExecuteNonQuery();

                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                    }

                    trans.Dispose();
                    #endregion --- Insert ---
                }

                if (!oleDR.IsClosed) oleDR.Close();


            }

            oleDbCon.Close();
            return true;
        }
        
        public bool InsertGoldenTolerance(DataTable dtGolden, string[] strCriteria)
        {
            return true;

        }
    }
}


