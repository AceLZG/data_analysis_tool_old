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
    /// Data analysis class
    /// </summary>
    public class Analysis
    {
        #region *** Variable declare ***
        private string[] m_FailureArray;
        Util util = new Util();

        #endregion *** Variable declare ***

        #region *** Properties ***
        public string[] FailureArray
        {
            get
            {
                return m_FailureArray;
            }
            set
            {
                m_FailureArray = value;
            }
        }

        #endregion *** Properties ***

        #region *** Method ***
        public List<FailureMode> GetFailureModeList(DataTable dataTable, int DataStartColumn,AnalysisType failureAnalysisType)
        {
            #region *** Variable define ***
            //m_FailureArray = new string[dataTable.Rows.Count - 3];
            m_FailureArray = new string[100000];
            string[] deviceNoArray;
            Dictionary<string, string> myDic = new Dictionary<string, string>();
            List<FailureMode> failureModeList = new List<FailureMode>();

            #endregion *** Variable define ***

            #region *** Generate failure parameter list for each device ***

            m_FailureArray[0] = "This array contain failure parameter list for each device";

            foreach (DataRow dr in dataTable.Rows) // for datatable rows
            {
                if (dataTable.Rows.IndexOf(dr) > 3 && Convert.ToString(dr[dataTable.Columns.Count - 1]).ToUpper() == "FAIL") //if Datarow
                {
                    for (int j = DataStartColumn; j < dataTable.Columns.Count - 1; j++)  //for Column loop
                    {
                        //if (dr[j] != DBNull.Value) //if DBNull
                        if (util.IsNumeral(dr[j]))
                        {
                            double Value = Convert.ToDouble(dr[j]);
                            double LowLimit = Convert.ToDouble(dataTable.Rows[2][j]);
                            double HighLimit = Convert.ToDouble(dataTable.Rows[3][j]);
                            int DeviceNo = Convert.ToInt32(dr[0]);

                            if (Value < LowLimit || Value > HighLimit)
                            {
                                m_FailureArray[DeviceNo] += dataTable.Rows[0][j].ToString() + " " + '"' + " ";
                            }
                        }//end of if DBNull

                    } //end of for Column loop

                }// end of if Datarow

            }// end of for datatable rows
            #endregion *** Generate failure parameter list for each device ***

            if (failureAnalysisType == AnalysisType.FailureMode)        //if Analysis type = failure mode
            {
                #region *** Caculate failure mode ***
                for (int i = 1; i < m_FailureArray.Length; i++) // for failureArray length
                {
                    string FaulireModeTemp = m_FailureArray[i];
                    if (FaulireModeTemp != null)
                    {
                        if (myDic.ContainsKey(FaulireModeTemp))
                        {//if Contain key, append device no
                            string DeviceNo = myDic[FaulireModeTemp] + "," + i.ToString();
                            myDic[FaulireModeTemp] = DeviceNo;
                        }
                        else
                        { //if not, add the failure mode and device no
                            string DeviceNo = i.ToString();
                            myDic.Add(FaulireModeTemp, DeviceNo);
                        }
                    }
                }// end of for failureArray length

                #endregion *** Caculate failure mode ***
            } //end of if Analysis type = failure mode
            else                                                        //if Analysis type = failure rate
            {
                #region *** Caculate failure rate ***
                for (int i = 1; i < m_FailureArray.Length; i++) // for failureArray length
                {
                    if (m_FailureArray[i] != null)  //if (m_FailureArray[i] != null)
                    {
                        int stringLength = m_FailureArray[i].Length - 2;
                        string[] failureRateTemp = m_FailureArray[i].Substring(0, stringLength).Split('"');
                        for (int j = 0; j < failureRateTemp.Length; j++) //for failureRateTemp.Length
                        {
                            string failureParameter = failureRateTemp[j].Trim();
                            if (myDic.ContainsKey(failureParameter))
                            {//if Contain key, append device no
                                string DeviceNo = myDic[failureParameter] + "," + i.ToString();
                                myDic[failureParameter] = DeviceNo;
                            }
                            else
                            { //if not, add the failure mode and device no
                                string DeviceNo = i.ToString();
                                myDic.Add(failureParameter, DeviceNo);
                            }
                        }//end of for failureRateTemp.Length

                    }//end of if (m_FailureArray[i] != null)

                }// end of for failureArray length

                #endregion *** Caculate failure rate ***

            }//end of if Analysis type = failure rate

            #region *** Sorting failure mode ***
            foreach (string key in myDic.Keys)
            {
                deviceNoArray = myDic[key].Split(',');

                FailureMode FMTemp = new FailureMode();
                FMTemp.Count = deviceNoArray.Length;
                FMTemp.Parameter = key;
                FMTemp.Device = deviceNoArray;

                failureModeList.Add(FMTemp);
            }
            Comparison<FailureMode> cmp = (x, y) =>
            {
                if (x.Count > y.Count)
                {
                    return -1;
                }
                else if (x.Count < y.Count)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            };

            failureModeList.Sort(cmp);

            #endregion *** Sorting failure mode ***

            return failureModeList;
        } //end of GetFailureModeList

        public List<FailureMode> GetFailureModeList(DataTable dataTable, int DataStartColumn, AnalysisType failureAnalysisType, Dictionary<int, string>  ExceptionList)
        {
            #region *** Variable define ***
            //m_FailureArray = new string[dataTable.Rows.Count - 3];
            m_FailureArray = new string[1000000];
            string[] deviceNoArray;
            Dictionary<string, string> myDic = new Dictionary<string, string>();
            List<FailureMode> failureModeList = new List<FailureMode>();

            #endregion *** Variable define ***

            #region *** Generate failure parameter list for each device ***

            m_FailureArray[0] = "This array contain failure parameter list for each device";

            foreach (DataRow dr in dataTable.Rows) // for datatable rows
            {
                if (dataTable.Rows.IndexOf(dr) > 3 && Convert.ToString(dr[dataTable.Columns.Count - 1]).ToUpper() == "FAIL") //if Datarow
                {
                    for (int j = DataStartColumn; j < dataTable.Columns.Count - 1; j++)  //for Column loop
                    {
                        if (ExceptionList.ContainsKey(j)) continue;

                        //if (dr[j] != DBNull.Value) //if DBNull
                        if (util.IsNumeral(dr[j]))
                        {
                            double Value = Convert.ToDouble(dr[j]);
                            double LowLimit = Convert.ToDouble(dataTable.Rows[2][j]);
                            double HighLimit = Convert.ToDouble(dataTable.Rows[3][j]);
                            int DeviceNo = Convert.ToInt32(dr[0]);

                            if (Value < LowLimit || Value > HighLimit)
                            {
                                m_FailureArray[DeviceNo] += dataTable.Rows[0][j].ToString() + " " + '"' + " ";
                            }
                        }//end of if DBNull

                    } //end of for Column loop

                }// end of if Datarow

            }// end of for datatable rows
            #endregion *** Generate failure parameter list for each device ***

            if (failureAnalysisType == AnalysisType.FailureMode)        //if Analysis type = failure mode
            {
                #region *** Caculate failure mode ***
                for (int i = 1; i < m_FailureArray.Length; i++) // for failureArray length
                {
                    string FaulireModeTemp = m_FailureArray[i];
                    if (FaulireModeTemp != null)
                    {
                        if (myDic.ContainsKey(FaulireModeTemp))
                        {//if Contain key, append device no
                            string DeviceNo = myDic[FaulireModeTemp] + "," + i.ToString();
                            myDic[FaulireModeTemp] = DeviceNo;
                        }
                        else
                        { //if not, add the failure mode and device no
                            string DeviceNo = i.ToString();
                            myDic.Add(FaulireModeTemp, DeviceNo);
                        }
                    }
                }// end of for failureArray length

                #endregion *** Caculate failure mode ***
            } //end of if Analysis type = failure mode
            else                                                        //if Analysis type = failure rate
            {
                #region *** Caculate failure rate ***
                for (int i = 1; i < m_FailureArray.Length; i++) // for failureArray length
                {
                    if (m_FailureArray[i] != null)  //if (m_FailureArray[i] != null)
                    {
                        int stringLength = m_FailureArray[i].Length - 2;
                        string[] failureRateTemp = m_FailureArray[i].Substring(0, stringLength).Split('"');
                        for (int j = 0; j < failureRateTemp.Length; j++) //for failureRateTemp.Length
                        {
                            string failureParameter = failureRateTemp[j].Trim();
                            if (myDic.ContainsKey(failureParameter))
                            {//if Contain key, append device no
                                string DeviceNo = myDic[failureParameter] + "," + i.ToString();
                                myDic[failureParameter] = DeviceNo;
                            }
                            else
                            { //if not, add the failure mode and device no
                                string DeviceNo = i.ToString();
                                myDic.Add(failureParameter, DeviceNo);
                            }
                        }//end of for failureRateTemp.Length

                    }//end of if (m_FailureArray[i] != null)

                }// end of for failureArray length

                #endregion *** Caculate failure rate ***

            }//end of if Analysis type = failure rate

            #region *** Sorting failure mode ***
            foreach (string key in myDic.Keys)
            {
                deviceNoArray = myDic[key].Split(',');

                FailureMode FMTemp = new FailureMode();
                FMTemp.Count = deviceNoArray.Length;
                FMTemp.Parameter = key;
                FMTemp.Device = deviceNoArray;

                failureModeList.Add(FMTemp);
            }
            Comparison<FailureMode> cmp = (x, y) =>
            {
                if (x.Count > y.Count)
                {
                    return -1;
                }
                else if (x.Count < y.Count)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            };

            failureModeList.Sort(cmp);

            #endregion *** Sorting failure mode ***

            return failureModeList;
        } //end of GetFailureModeList

        // Caculate Cpk
        public DataTable CaculateCpk(DataTable datatable , int DataStartColumn)
        {
            #region *** Variable Define ***
            bool FirstRun = true;
            int RowsCount = datatable.Rows.Count;
            int ColumnsCount = datatable.Columns.Count;
            int TestParameterQty = ColumnsCount - DataStartColumn - 1;
            int TotalDeviceQty = RowsCount - 4;

            DataTable tblCpk = new DataTable();
            //double[] LSL = new double[TestParameterQty];
            //double[] USL = new double[TestParameterQty];
            //int[] ActualDeviceQty = new int[TestParameterQty];
            double[] sSum = new double[TestParameterQty];
            double[] xSum = new double[TestParameterQty];

            ParameterCpk[] Cpk = new ParameterCpk[TestParameterQty];

            #endregion *** Variable Define ***

            #region *** Get LSL, USL, Average, Count ***
            for (int j = 0; j < TestParameterQty; j++)
            {
                FirstRun = true;
                xSum[j] = 0;
                Cpk[j].LSL = 0;
                Cpk[j].USL = 0;
                Cpk[j].Average = 0;
                Cpk[j].Count = 0;

                for (int i = 0; i < RowsCount; i++)
                {
                    //Low Limit
                    if (i == 2)
                        Cpk[j].LSL = Convert.ToDouble(datatable.Rows[i][j + DataStartColumn]);
                    //Upper Limit
                    if (i == 3)
                        Cpk[j].USL = Convert.ToDouble(datatable.Rows[i][j + DataStartColumn]);
                    // Sum
                    if (i > 3 && datatable.Rows[i][j + 1] != DBNull.Value)
                    {
                        double CellValue = 0;
                        Cpk[j].Count++;
                        try
                        {
                            CellValue = Convert.ToDouble(datatable.Rows[i][j + DataStartColumn]);
                        }
                        catch
                        {
                            CellValue = 0;
                        }

                        xSum[j] += CellValue;

                        if (FirstRun)
                        {
                            Cpk[j].MIN = CellValue;
                            Cpk[j].MAX = CellValue;
                            FirstRun = false;
                        }
                        else
                        {
                            if (CellValue < Cpk[j].MIN)
                                Cpk[j].MIN = CellValue;
                            else if (CellValue > Cpk[j].MAX)
                                Cpk[j].MAX = CellValue;
                        }
                    }
                }//end of for i
                //Average
                if (Cpk[j].Count != 0)
                    Cpk[j].Average = xSum[j] / Cpk[j].Count;
            }//end of for j

            #endregion *** Get LSL, USL, Average ***

            #region *** Caculate Cpk***
            for (int j = 0; j < TestParameterQty; j++)
            {
                sSum[j] = 0;
                for (int i = 0; i < RowsCount; i++)
                {
                    if (i > 3 && datatable.Rows[i][j + 1] != DBNull.Value)
                    {
                        double CellValue = 0;
                        try
                        {
                            CellValue = Convert.ToDouble(datatable.Rows[i][j + DataStartColumn]);
                        }
                        catch
                        {
                            CellValue = 0;
                        }
                        sSum[j] += (Cpk[j].Average - CellValue) * (Cpk[j].Average - CellValue);
                    }
                }
                if (Cpk[j].Count != 0)
                {
                    Cpk[j].Stdev = Math.Sqrt(sSum[j] / (Cpk[j].Count - 1));
                    Cpk[j].Stdevp = Math.Sqrt(sSum[j] / Cpk[j].Count);
                    Cpk[j].U = (Cpk[j].USL + Cpk[j].LSL) / 2;
                    Cpk[j].T = (Cpk[j].USL - Cpk[j].LSL);
                    Cpk[j].Cp = (Cpk[j].USL - Cpk[j].LSL) / (6 * Cpk[j].Stdevp);
                    Cpk[j].Ca = (Cpk[j].Average - Cpk[j].U) / (Cpk[j].T / 2);
                    Cpk[j].Value = Cpk[j].Cp * (1 - Math.Abs(Cpk[j].Ca));
                    Cpk[j].CpkL = (Cpk[j].Average - Cpk[j].LSL) / (3 * Cpk[j].Stdevp);
                    Cpk[j].CpkU = (Cpk[j].USL - Cpk[j].Average) / (3 * Cpk[j].Stdevp);
                }
            }

            #endregion *** Caculate Cpk***

            #region *** Merge to Datatable ***
            tblCpk.Columns.Add("#", typeof(int));
            tblCpk.Columns.Add("Parameter", typeof(string));
            tblCpk.Columns.Add("Unit", typeof(string));
            tblCpk.Columns.Add("Low Limit", typeof(double));
            tblCpk.Columns.Add("High Limit", typeof(double));
            tblCpk.Columns.Add("Cpk", typeof(double));
            tblCpk.Columns.Add("Average", typeof(double));
            tblCpk.Columns.Add("Min", typeof(double));
            tblCpk.Columns.Add("Max", typeof(double));
            tblCpk.Columns.Add("Stdevp", typeof(double));
            tblCpk.Columns.Add("CpkU", typeof(double));
            tblCpk.Columns.Add("CpkL", typeof(double));
            tblCpk.Columns.Add("Cp", typeof(double));
            tblCpk.Columns.Add("Ca", typeof(double));
            //tblCpk.Columns.Add("Yield+", typeof(string));

            for (int j = 0; j < datatable.Columns.Count - DataStartColumn - 1; j++)
            {
                DataRow dr = tblCpk.NewRow();
                dr["#"] = j + 1;
                dr["Parameter"] = datatable.Rows[0][j + DataStartColumn].ToString();
                dr["Unit"] = datatable.Rows[1][j + DataStartColumn].ToString();
                dr["Low Limit"] = Convert.ToDouble(datatable.Rows[2][j + DataStartColumn]);
                dr["High Limit"] = Convert.ToDouble(datatable.Rows[3][j + DataStartColumn]);
                dr["Average"] = Math.Round(Cpk[j].Average, 3);
                dr["Min"] = Math.Round(Cpk[j].MIN, 3);
                dr["Max"] = Math.Round(Cpk[j].MAX, 3);
                dr["Stdevp"] = Math.Round(Cpk[j].Stdevp, 3);
                dr["Cpk"] = Math.Round(Cpk[j].Value, 3);
                dr["CpkU"] = Math.Round(Cpk[j].CpkU, 3);
                dr["CpkL"] = Math.Round(Cpk[j].CpkL, 3);
                dr["Cp"] = Math.Round(Cpk[j].Cp, 3);
                dr["Ca"] = Math.Round(Cpk[j].Ca, 3);
                tblCpk.Rows.Add(dr);
            }

            #endregion *** Merge to Datatable ***

            return tblCpk;
        }

        // Caculate Cpk with updated limit
        public DataTable CaculateCpk(DataTable datatable,int DataStartColumn, double[] LowLimit, double[] HighLimit)
        {
            #region *** Variable Define ***
            bool FirstRun = true;
            int RowsCount = datatable.Rows.Count;
            int ColumnsCount = datatable.Columns.Count;
            int TestParameterQty = ColumnsCount - DataStartColumn - 1;
            int TotalDeviceQty = RowsCount - 4;

            DataTable tblCpk = new DataTable();
            //double[] LSL = new double[TestParameterQty];
            //double[] USL = new double[TestParameterQty];
            //int[] ActualDeviceQty = new int[TestParameterQty];
            double[] sSum = new double[TestParameterQty];
            double[] xSum = new double[TestParameterQty];

            ParameterCpk[] Cpk = new ParameterCpk[TestParameterQty];

            #endregion *** Variable Define ***

            #region *** Get LSL, USL, Average ***
            for (int j = 0; j < TestParameterQty; j++)
            {
                FirstRun = true;
                xSum[j] = 0;
                Cpk[j].LSL = 0;
                Cpk[j].USL = 0;
                Cpk[j].Average = 0;
                Cpk[j].Count = 0;

                for (int i = 0; i < RowsCount; i++)
                {
                    //Low Limit
                    if (i == 2)
                        Cpk[j].LSL = LowLimit[j];
                    //Upper Limit
                    if (i == 3)
                        Cpk[j].USL = HighLimit[j];
                    // Sum
                    if (i > 3 && datatable.Rows[i][j + 1] != DBNull.Value)
                    {
                        //Cpk[j].Count++;
                        //double CellValue = Convert.ToDouble(datatable.Rows[i][j + DataStartColumn]);
                        //xSum[j] += CellValue;

                        double CellValue = 0;
                        Cpk[j].Count++;
                        try
                        {
                            CellValue = Convert.ToDouble(datatable.Rows[i][j + DataStartColumn]);
                        }
                        catch
                        {
                            CellValue = 0;
                        }

                        xSum[j] += CellValue;

                        if (FirstRun)
                        {
                            Cpk[j].MIN = CellValue;
                            Cpk[j].MAX = CellValue;
                            FirstRun = false;
                        }
                        else
                        {
                            if (CellValue < Cpk[j].MIN)
                                Cpk[j].MIN = CellValue;
                            else if (CellValue > Cpk[j].MAX)
                                Cpk[j].MAX = CellValue;
                        }
                    }

                }//end of for i
                //Average
                if (Cpk[j].Count != 0)
                    Cpk[j].Average = xSum[j] / Cpk[j].Count;
            }//end of for j

            #endregion *** Get LSL, USL, Average ***

            #region *** Caculate Cpk***
            for (int j = 0; j < TestParameterQty; j++)
            {
                sSum[j] = 0;
                for (int i = 0; i < RowsCount; i++)
                {
                    if (i > 3 && datatable.Rows[i][j + DataStartColumn] != DBNull.Value)
                    {
                        //double CellValue = Convert.ToDouble(datatable.Rows[i][j + DataStartColumn]);

                        double CellValue = 0;
                        try
                        {
                            CellValue = Convert.ToDouble(datatable.Rows[i][j + DataStartColumn]);
                        }
                        catch
                        {
                            CellValue = 0;
                        }
                        sSum[j] += (Cpk[j].Average - CellValue) * (Cpk[j].Average - CellValue);
                    }
                }
                if (Cpk[j].Count != 0)
                {
                    Cpk[j].Stdev = Math.Sqrt(sSum[j] / (Cpk[j].Count - 1));
                    Cpk[j].Stdevp = Math.Sqrt(sSum[j] / Cpk[j].Count);
                    Cpk[j].U = (Cpk[j].USL + Cpk[j].LSL) / 2;
                    Cpk[j].T = (Cpk[j].USL - Cpk[j].LSL);
                    Cpk[j].Cp = (Cpk[j].USL - Cpk[j].LSL) / (6 * Cpk[j].Stdevp);
                    Cpk[j].Ca = (Cpk[j].Average - Cpk[j].U) / (Cpk[j].T / 2);
                    Cpk[j].Value = Cpk[j].Cp * (1 - Math.Abs(Cpk[j].Ca));
                    Cpk[j].CpkL = (Cpk[j].Average - Cpk[j].LSL) / (3 * Cpk[j].Stdevp);
                    Cpk[j].CpkU = (Cpk[j].USL - Cpk[j].Average) / (3 * Cpk[j].Stdevp);
                }
            }

            #endregion *** Caculate Cpk***

            #region *** Merge to Datatable ***
            tblCpk.Columns.Add("#", typeof(int));
            tblCpk.Columns.Add("Parameter", typeof(string));
            tblCpk.Columns.Add("Unit", typeof(string));
            tblCpk.Columns.Add("Low Limit", typeof(double));
            tblCpk.Columns.Add("High Limit", typeof(double));
            tblCpk.Columns.Add("Cpk", typeof(double));
            tblCpk.Columns.Add("Average", typeof(double));
            tblCpk.Columns.Add("Min", typeof(double));
            tblCpk.Columns.Add("Max", typeof(double));
            tblCpk.Columns.Add("Stdevp", typeof(double));
            tblCpk.Columns.Add("CpkU", typeof(double));
            tblCpk.Columns.Add("CpkL", typeof(double));
            tblCpk.Columns.Add("Cp", typeof(double));
            tblCpk.Columns.Add("Ca", typeof(double));
            //tblCpk.Columns.Add("Yield+", typeof(string));

            for (int j = 0; j < datatable.Columns.Count - DataStartColumn-1; j++)
            {
                DataRow dr = tblCpk.NewRow();
                dr["#"] = j + 1;
                dr["Parameter"] = datatable.Rows[0][j + DataStartColumn].ToString();
                dr["Unit"] = datatable.Rows[1][j + DataStartColumn].ToString();
                dr["Low Limit"] = LowLimit[j];
                dr["High Limit"] = HighLimit[j];
                dr["Average"] = Math.Round(Cpk[j].Average, 3);
                dr["Min"] = Math.Round(Cpk[j].MIN, 3);
                dr["Max"] = Math.Round(Cpk[j].MAX, 3);
                dr["Stdevp"] = Math.Round(Cpk[j].Stdevp, 3);
                dr["Cpk"] = Math.Round(Cpk[j].Value, 3);
                dr["CpkU"] = Math.Round(Cpk[j].CpkU, 3);
                dr["CpkL"] = Math.Round(Cpk[j].CpkL, 3);
                dr["Cp"] = Math.Round(Cpk[j].Cp, 3);
                dr["Ca"] = Math.Round(Cpk[j].Ca, 3);
                tblCpk.Rows.Add(dr);
            }

            #endregion *** Merge to Datatable ***

            return tblCpk;
        }

        // Caculate Yield impact
        public double CaculateYieldImpact(DataTable datatable, int DataStartColumn, double[] NewLowLimit, double[] NewHighLimit)
        {
            int ParameterCount = datatable.Columns.Count - DataStartColumn-1;
            int DeviceCount = datatable.Rows.Count - 4;

            int PassDeviceCount = 0;
            double Yield;

            foreach (DataRow dr in datatable.Rows)
            {
                if (datatable.Rows.IndexOf(dr) > 3)
                {
                    bool Pass = true;
                    for (int i = 0; i < ParameterCount; i++)
                    {
                        if (dr[i + 1] != DBNull.Value)
                        {
                            double TestResult = Convert.ToDouble(dr[i + DataStartColumn]);
                            if (TestResult < NewLowLimit[i] || TestResult > NewHighLimit[i])
                            {
                                Pass = false;
                            }
                        }
                    }
                    if (Pass)
                    {
                        PassDeviceCount++;
                    }
                }
            }

            Yield = Convert.ToDouble(PassDeviceCount) / Convert.ToDouble(DeviceCount);
            return Yield;
        }
        #endregion *** Method ***

        //Build JMP Script
        public void BuildJMPScript(string fileName, object[] objParameter, bool blnBuildOverLay)
        {
            bool blnFirstParameter = true;

            if (File.Exists(fileName)) File.Delete(fileName);

            FileStream _FileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter _StreamWriter = new StreamWriter(_FileStream);
            StringBuilder _StringBuilder = new StringBuilder();

            //Build Distribution JMP Script
            _StringBuilder.AppendLine("Distribution(");
            _StringBuilder.AppendLine("	Stack( 1 ),");
            foreach (object objTemp in objParameter)
            {
                if (objTemp == null) break;
                if (!blnFirstParameter)
                {
                    _StringBuilder.AppendLine(",");
                }

                _StringBuilder.AppendLine("	Continuous Distribution(");
                _StringBuilder.AppendLine("		Column( :Name( \"" + objTemp.ToString() + "\" ) ),");
                _StringBuilder.AppendLine("		Horizontal Layout( 1 ),");
                _StringBuilder.AppendLine("		Vertical( 0 )");
                _StringBuilder.Append("	)");

                blnFirstParameter = false;
            }
            _StringBuilder.AppendLine();
            _StringBuilder.AppendLine(");");

            if (blnBuildOverLay)
            {
                blnFirstParameter = true; ;
                _StringBuilder.AppendLine("Overlay Plot(");
                _StringBuilder.AppendLine("	Y(");
                foreach (object objTemp in objParameter)
                {
                    if (objTemp == null) break;
                    if (!blnFirstParameter)
                    {
                        _StringBuilder.AppendLine(",");
                    }

                    _StringBuilder.Append("    :Name( \"" + objTemp.ToString() + "\" )");

                    blnFirstParameter = false;
                }
                _StringBuilder.AppendLine();
                _StringBuilder.AppendLine("	),");
                _StringBuilder.AppendLine("	Overlay( 0 ),");
                _StringBuilder.AppendLine("	Ungroup Plots( 1 )");
                _StringBuilder.AppendLine(");");
            }

            _StreamWriter.Flush();
            _StreamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
            _StreamWriter.Write(_StringBuilder.ToString());
            _StreamWriter.Flush();
            _StreamWriter.Close();
        }

    } //end of class Vanchip.Data.Analysis

}//end of namespace Vanchip.Data
