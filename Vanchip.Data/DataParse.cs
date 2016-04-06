///     Reversion history log
///     Rev1.0      Initie build                                                                AceLi       2011-10-21
///     Rev1.1      Add dual site data fetch function                                           AceLi       2012-02-25
///     Rev1.2      Fixed some bug in dual site data fetch                                      Ace Li      2012-03-06
///     Rev1.2.1    Fixed some bug when test data file is incompelete                           Ace Li      2012-03-24
///     Rev1.9.0.0  Add STDF file parse solution                                                Ace Li      2015-01-01
///     Rev2.0.0.0  rewrite stdf lib, using stdf viewer for std/stdf file 
///                 add function GetDataFromStdfviewer()                                        Ace Li      2016-03-31


using System;
using System.Threading.Tasks;
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
using STDF_Viewer;


namespace Vanchip.Data
{
    /// <summary>
    /// Parse data class
    /// </summary>
    public class DataParse
    {
        #region *** Variable declare ***
        private Bin m_Bin;
        private DataHeader m_Header;
        public AceTechDataHeader a_Header;
        private int m_FreezeColumn = 0;
        private double m_ParseTime = 0;
        private double m_InsertTime = 0;
        private int m_TestedDevice = 0;
        private int m_PassedDevice = 0;
        private int m_FailedDevice = 0;

        #endregion *** Variable declare ***

        #region *** Properties ***
        public Bin Bin
        {
            get
            {
                return m_Bin;
            }
            set
            {
                m_Bin = value;
            }
        }

        public DataHeader Header
        {
            get
            {
                return m_Header;
            }
            set
            {
                m_Header = value;
            }
        }

        public int FreezeColumn
        {
            get
            {
                return m_FreezeColumn;
            }
            set
            {
                m_FreezeColumn = value;
            }
        }

        public double ParseTime
        {
            get
            {
                return m_ParseTime;
            }
            set
            {
                m_ParseTime = value;
            }
        }

        public double InsertTime
        {
            get
            {
                return m_InsertTime;
            }
            set
            {
                m_InsertTime = value;
            }
        }
               
        public int TestedDevice
        {
            get
            {
                return m_TestedDevice;
            }
            set
            {
                m_TestedDevice = value;
            }
        }

        public int PassedDevice
        {
            get
            {
                return m_PassedDevice;
            }
            set
            {
                m_PassedDevice = value;
            }
        }
        
        public int FailedDevice
        {
            get
            {
                return m_FailedDevice;
            }
            set
            {
                m_FailedDevice = value;
            }
        }

        #endregion *** Properties ***

        #region *** Method ***
        ///<summary>
        ///<para>Parse test data from LTX txt test result in to datatable</para>
        ///<seealso cref="DataParse.GetDataFromTxt"/>
        ///</summary>
        /// <param name="fileName">Full patch with file name of the txt file</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataFromTxt(string fileName)
        {
            #region *** Variable define ***
            DateTime dtStart;
            TimeSpan ts;

            DataRow dr;
            DataTable DataParseResult = new DataTable();
            StringBuilder sbDataString = new StringBuilder();

            bool isDataHeaderGet = false;
            //bool isTestModeGet = false;
            bool isParseHeaderFinish = false;

            string lineConent = null;

            int intSiteNumber;
            int intSubStringStart = 18; 
            int intDeviceCounter = 0;
            int intParameterCounter = 1;
            int intFixedParameterCounter = 1;
            int intisDataHeaderGetCount = 0;
            int intLineCount = 1;
            int intDeviceID = 0;

            string strParameterBuffer = "Test Item";
            string strLowLimitBuffer = "Low Limit";
            string strHighLimitBuffer = "High Limit";
            string strUnitsBuffer = "Units";

            string strParameterString = "";
            string strLowLimitString = "";
            string strHighLimitString = "";
            string strUnitsString = "";
            string strDataString = "";

            string[] strOriginalData;
            string[] strTrimedData;

            #endregion *** Variable define ***

            #region *** Initialize properties ***
            m_FreezeColumn = 3;
            m_ParseTime = 0;
            m_InsertTime = 0;
            m_TestedDevice = 0;
            m_PassedDevice = 0;
            m_FailedDevice = 0;
            m_Header = new DataHeader();
            m_Bin = new Bin();
            m_Bin.DeviceCount = new int[8];
            m_Bin.Name = new string[8];
            m_Bin.Number = new int[8];
            m_Bin.Percent = new double[8];

            m_Header.SessionCount = 1;

            #endregion *** Initialize properties ***

            intSiteNumber = GetSiteNumber(fileName);
            //Single site 
            if (intSiteNumber == 1)
            {
                #region *** Parse test data from txt file ***
                try
                {
                    //Timer session
                    dtStart = DateTime.Now;

                    #region *** Read to memory ***
                    //string content = string.Empty;
                    //using (StreamReader ms_StreamReader = new StreamReader(fileName))
                    //{
                    //    content = ms_StreamReader.ReadToEnd();//一次性读入内存
                    //}
                    //MemoryStream _MemoryStream = new MemoryStream(Encoding.GetEncoding("GB2312").GetBytes(content));//放入内存流，以便逐行读取
                    //StreamReader _StreamReader = new StreamReader(_MemoryStream);

                    //ts = DateTime.Now - dtStart;
                    //ParseTime = ts.TotalMilliseconds;
                    #endregion *** Read to memory ***

                    #region *** Direct Read ***
                    FileStream _FileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    StreamReader _StreamReader = new StreamReader(_FileStream);
                    _StreamReader.BaseStream.Seek(0, SeekOrigin.Begin);

                    #endregion *** Diresct Read ***

                    lineConent = _StreamReader.ReadLine();   intLineCount++;
                    while (lineConent != null)
                    {
                        #region *** Header section ***

                        #region *** Product & ProgramRev ***
                        //Get product
                        if (!isParseHeaderFinish && lineConent.Contains("Program Name"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            string[] tempArray = temp.Split('_');
                            m_Header.Product = tempArray[0].Trim();
                            try 
                            {
                                m_Header.ProgramRev = tempArray[1].Trim() + "." + tempArray[2].Trim() + "." + tempArray[3].Trim();
                            }
                            catch
                            {
                                try
                                {
                                    m_Header.ProgramRev = tempArray[1].Trim() + "." + tempArray[2].Trim();
                                }
                                catch
                                {
                                    m_Header.ProgramRev = tempArray[1].Trim();
                                }
                            }

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** Product & ProgramRev ***

                        #region *** StartTime ***
                        //Get StartTime
                        if (!isParseHeaderFinish && lineConent.Contains("Start Time"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            string[] tempArray = temp.Split(' ');
                            string[] tempArrayTrimed = new string[5];
                            StringBuilder _StringBuilder = new StringBuilder();
                            int j = 0;

                            for (int i = 0; i < tempArray.Length; i++)
                            {
                                if (tempArray[i] != "")
                                {
                                    tempArrayTrimed[j] = tempArray[i];
                                    j++;
                                }
                            }
                            _StringBuilder.Append(tempArrayTrimed[4]);  //Year
                            _StringBuilder.Append("/");
                            _StringBuilder.Append(tempArrayTrimed[1]);  //Month
                            _StringBuilder.Append("/");
                            _StringBuilder.Append(tempArrayTrimed[2]);  //Day
                            _StringBuilder.Append(" ");
                            _StringBuilder.Append(tempArrayTrimed[3]);  //Time
                            m_Header.LotStartDateTime = Convert.ToDateTime(_StringBuilder.ToString());

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        } //end of is contains
                        #endregion *** StartTime ***

                        #region *** Device Name ***
                        //Get Device Name
                        if (!isParseHeaderFinish && lineConent.Contains("Device Name"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);

                            //m_Header.LotStartDateTime = temp.ToString();

                            m_Header.DeviceName = temp.Trim();
                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        } //end of is contains
                        #endregion *** Device Name ***

                        #region *** Lot Retest ***
                        //Get Lot Retest
                        if (!isParseHeaderFinish && lineConent.Contains("Lot Retest"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);

                            //m_Header.LotStartDateTime = temp.ToString();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        } //end of is contains
                        #endregion *** Lot Retest ***

                        #region *** LotID ***
                        //Get LotID
                        if (!isParseHeaderFinish && lineConent.Contains("Lot ID"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            //string[] tempArray = temp.Split('_');
                            m_Header.LotID = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** LotID ***

                        #region *** Sub LotID ***
                        //Get Sub LotID
                        if (!isParseHeaderFinish && lineConent.Contains("Sublot ID"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            //string[] tempArray = temp.Split('_');
                            m_Header.SubLotID = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** SubLotID ***

                        #region *** Tester ***
                        //Get Tester
                        if (!isParseHeaderFinish && lineConent.Contains("Tester Node Name"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            m_Header.Tester = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** Tester ***

                        #region *** Test Head ***
                        //Get Test Head
                        if (!isParseHeaderFinish && lineConent.Contains("Test Head"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            //m_Header.Tester = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** Test Head ***

                        #region *** TesterType ***
                        //Get TesterType
                        if (!isParseHeaderFinish && lineConent.Contains("Tester Type"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            m_Header.TesterType = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** TesterType ***

                        #region *** Serial Number ***
                        //Get Serial Number
                        if (!isParseHeaderFinish && lineConent.Contains("Serial Number"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            //m_Header.Tester = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** Serial Number ***

                        #region *** Operator ID ***
                        //Get Operator ID
                        if (!isParseHeaderFinish && lineConent.Contains("Operator ID"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            m_Header.OperatorID = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** Operator ID ***

                        #region *** enVision Version ***
                        //Get enVision Version
                        if (!isParseHeaderFinish && lineConent.Contains("enVision Version"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            m_Header.enVision_Version = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** enVision Version ***

                        #region *** FAB ID ***
                        //Get FAB ID
                        if (!isParseHeaderFinish && lineConent.Contains("FAB ID"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            //m_Header.TesterType = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** FAB ID ***

                        #region *** Active Flow ***
                        //Get Active Flow
                        if (!isParseHeaderFinish && lineConent.Contains("Active Flow"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            //m_Header.Tester = temp.Trim();
                            isParseHeaderFinish = true;
                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** Active Flow ***

                        #region *** Software Bin Summary ***
                        ////Get Software Bin Summary
                        //if (lineConent.Contains("Software Bin Summary"))
                        //{
                        //    int j = 0;
                        //    int BinCount = 0;
                        //    string[] tempArrayTrim = new string[5];
                        //    lineConent = _StreamReader.ReadLine();   intLineCount++;
                        //    lineConent = _StreamReader.ReadLine();   intLineCount++;
                        //    lineConent = _StreamReader.ReadLine();   intLineCount++;
                        //    while (lineConent != "" && BinCount < 8)
                        //    {
                        //        string[] tempArray = lineConent.Split(' ');
                        //        for (int i = 0; i < tempArray.Length; i++)
                        //        {
                        //            if (tempArray[i] != "")
                        //            {
                        //                tempArrayTrim[j] = tempArray[i];
                        //                j++;
                        //            }
                        //        }
                        //        m_Bin.Number[BinCount] = Convert.ToInt32(tempArrayTrim[0]);
                        //        m_Bin.DeviceCount[BinCount] = Convert.ToInt32(tempArrayTrim[2]);
                        //        m_Bin.Percent[BinCount] = Convert.ToDouble(tempArrayTrim[3]);
                        //        m_Bin.Name[BinCount] = tempArrayTrim[4];

                        //        lineConent = _StreamReader.ReadLine();   intLineCount++;
                        //        BinCount++;
                        //        j = 0;
                        //    } //end of while (strline != "")

                        //    lineConent = _StreamReader.ReadLine();   intLineCount++;
                        //    lineConent = _StreamReader.ReadLine();   intLineCount++; //jump to Device Count Summary
                        //}//end of if contains
                        #endregion *** Software Bin Summary *** //not calc

                        #region *** Device Count Summary ***
                        //Get Device Count Summary
                        if (lineConent.Contains("Device Count Summary"))
                        {
                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                            lineConent = _StreamReader.ReadLine();   intLineCount++;

                            string temp = lineConent.Substring(intSubStringStart);
                            string[] tempArray = temp.Split(' ');
                            string[] tempArrayTrimed = new string[5];
                            int j = 0;

                            for (int i = 0; i < tempArray.Length; i++)
                            {
                                if (tempArray[i] != "")
                                {
                                    tempArrayTrimed[j] = tempArray[i];
                                    j++;
                                }
                            }
                            m_TestedDevice = m_Header.TestQuantity = Convert.ToInt32(tempArrayTrimed[0]);
                            m_PassedDevice = m_Header.PassQuantity = Convert.ToInt32(tempArrayTrimed[1]);
                            m_FailedDevice = m_Header.FailQuantity = m_Header.TestQuantity - m_Header.PassQuantity;
                            //Yield
                            double yield = Convert.ToDouble(m_Header.PassQuantity) / Convert.ToDouble(m_Header.TestQuantity) * 100;
                            m_Header.Yield = Math.Round(yield, 2);

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                            lineConent = _StreamReader.ReadLine();   intLineCount++; // jump to lot finish time

                        }//end of if strline.Contains
                        #endregion *** Device Count Summary ***

                        #region *** Finish Time ***
                        //Get Finish Time
                        if (lineConent.Contains("Finish Time"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            string[] tempArray = temp.Split(' ');
                            string[] tempArrayTrimed = new string[5];
                            StringBuilder _StringBuilder = new StringBuilder();
                            int j = 0;

                            for (int i = 0; i < tempArray.Length; i++)
                            {
                                if (tempArray[i] != "")
                                {
                                    tempArrayTrimed[j] = tempArray[i];
                                    j++;
                                }
                            }
                            _StringBuilder.Append(tempArrayTrimed[4]);  //Year
                            _StringBuilder.Append("/");
                            _StringBuilder.Append(tempArrayTrimed[1]);  //Month
                            _StringBuilder.Append("/");
                            _StringBuilder.Append(tempArrayTrimed[2]);  //Day
                            _StringBuilder.Append(" ");
                            _StringBuilder.Append(tempArrayTrimed[3]);  //Time
                            m_Header.LotFinishDateTime = Convert.ToDateTime(_StringBuilder.ToString());

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        } //end of if contains
                        #endregion *** StartTime ***

                        #region *** Lot Description ***
                        //Get FAB ID
                        if (!isParseHeaderFinish && lineConent.Contains("Lot Description"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            //m_Header.TesterType = temp.Trim();

                            m_Header.LotDesc = temp.Trim();
                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** FAB ID ***

                        #region *** Test Mode ***
                        ////Get Test
                        //if (isParseHeaderFinish && !isTestModeGet)
                        //{
                        //    string lotName = "";
                        //    if (m_Header.LotID != null)
                        //        lotName = m_Header.LotID;
                        //    else
                        //        lotName = m_Header.SubLotID;

                        //    string[] temp = lotName.Split('_');
                        //    m_Header.TestMode = temp[1].Trim();
                        //    isTestModeGet = true;
                        //}
                        #endregion *** Test Mode ***

                        #endregion *** Header section ***

                        #region *** Device section ***

                        if (lineConent.Contains("Test Description"))
                        {
                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                            lineConent = _StreamReader.ReadLine();   intLineCount++; //Jump to GSM idle Current
                            //accumulative parameter counter
                            intDeviceCounter++;
                            //Reset parameter counter
                            intParameterCounter = 0;
                            //Reset Header buffer
                            strParameterBuffer = "Test Item,Site No,Device ID";
                            strLowLimitBuffer = "Low Limit,,";
                            strHighLimitBuffer = "High Limit,,";
                            strUnitsBuffer = "Units,,";

                            if (intDeviceCounter == 1)
                                sbDataString.Append(intDeviceCounter);
                            else
                            {
                                sbDataString.Append("&");
                                sbDataString.Append(intDeviceCounter);
                            }

                            // identify enVision
                            string enVer = m_Header.enVision_Version.Substring(0, 5);
                            if (enVer == "R12.5")    // CX R12.5.5
                            {
                                #region *** Single device data ***

                                //Get test result and get data header at fisrt device                   
                                while (lineConent != "")
                                {
                                    //strline = strline.Trim();
                                    strOriginalData = lineConent.Split(' ');
                                    int i, j = 0;
                                    strTrimedData = new string[15];
                                    //trim data string
                                    for (i = 0; i < strOriginalData.Length; i++)
                                    {
                                        if (strOriginalData[i] != "")
                                        {
                                            strTrimedData[j] = strOriginalData[i];
                                            j++;
                                        }
                                    }
                                    //Get data header
                                    if (!isDataHeaderGet)
                                    {
                                        if (strTrimedData[1] == "F")
                                        {
                                            strParameterBuffer += "," + strTrimedData[9];
                                            strLowLimitBuffer += "," + strTrimedData[3];
                                            strHighLimitBuffer += "," + strTrimedData[7];
                                            strUnitsBuffer += "," + strTrimedData[4];
                                        }
                                        else
                                        {
                                            strParameterBuffer += "," + strTrimedData[8];
                                            strLowLimitBuffer += "," + strTrimedData[2];
                                            strHighLimitBuffer += "," + strTrimedData[6];
                                            strUnitsBuffer += "," + strTrimedData[3];                                      
                                        }
                                    }
                                    //Get device test result
                                    if (strTrimedData[1] == "F")
                                    {
                                        sbDataString.Append(",");
                                        sbDataString.Append(strTrimedData[5]);
                                        //strDataString += "," + strTrimedData[5];
                                    }
                                    else
                                    {
                                        sbDataString.Append(",");
                                        sbDataString.Append(strTrimedData[4]);
                                        //strDataString += "," + strTrimedData[4];
                                    }//Read next parameter
                                    lineConent = _StreamReader.ReadLine();   intLineCount++;
                                    //accumulative parameter counter
                                    intParameterCounter++;
                                }

                                #region Get site & actual device number
                                //Get site & actual device number
                                lineConent = _StreamReader.ReadLine();   intLineCount++;
                                lineConent = _StreamReader.ReadLine();   intLineCount++;
                                lineConent = _StreamReader.ReadLine();   intLineCount++;
                                lineConent = _StreamReader.ReadLine();   intLineCount++;
                                lineConent = _StreamReader.ReadLine();   intLineCount++;//Jump to Site & actual device number
                                strOriginalData = lineConent.Split(' ');
                                sbDataString.Append(",");
                                sbDataString.Append(strOriginalData[5]); // Site number
                                sbDataString.Append(",");
                                sbDataString.Append(strOriginalData[7]); // Actual device number

                                #endregion Get site & actual device number

                                #endregion *** Single device data ***
                            }
                            else if (enVer == "R15.7")   // PAx R15.7.1
                            {
                                #region *** Single device data ***

                                //Get test result and get data header at fisrt device                   
                                while (lineConent != "")
                                {
                                    //strline = strline.Trim();
                                    strOriginalData = new string[8];
                                    strTrimedData = new string[7];
                                    int i, j = 0;

                                    strTrimedData[0] = lineConent.Substring(11, 10).Trim();         //Low limt
                                    strTrimedData[1] = lineConent.Substring(28, 10).Trim();         //High limit
                                    strTrimedData[2] = lineConent.Substring(38, 5).Trim();          //Units
                                    strTrimedData[3] = lineConent.Substring(48, 10).Trim();         //Site#1 result
                                    strTrimedData[4] = lineConent.Substring(68).Trim();             //Parameter name
                                    strTrimedData[6] = lineConent.Substring(58, 3).Trim();          //Site#1 unit

                                    ////trim data string
                                    //for (i = 0; i < strOriginalData.Length; i++)
                                    //{
                                    //    strTrimedData[i] = strOriginalData[i].Trim();
                                    //}

                                    //Get data header
                                    if (!isDataHeaderGet)
                                    {
                                        strParameterBuffer += "," + strTrimedData[4];
                                        strLowLimitBuffer += "," + strTrimedData[0];

                                        if (strTrimedData[2].ToLower() == "ks")
                                        {
                                            strTrimedData[1] = (Convert.ToDouble(strTrimedData[1]) * 1000 * 1000).ToString();
                                            strHighLimitBuffer += "," + strTrimedData[1];
                                            strUnitsBuffer += "," + "ms";
                                        }
                                        else
                                        {
                                            strHighLimitBuffer += "," + strTrimedData[1];
                                            strUnitsBuffer += "," + strTrimedData[2];
                                        }
                                    }
                                    //Get site#1 device test result
                                    sbDataString.Append(",");
                                    if (strTrimedData[6].ToLower() == "s")
                                    {
                                        sbDataString.Append(Convert.ToDouble(strTrimedData[3]) * 1000);
                                    }
                                    else if (strTrimedData[6].ToLower() == "ks")
                                    {
                                        sbDataString.Append(Convert.ToDouble(strTrimedData[3]) * 1000 * 1000);
                                    }
                                    else
                                    {
                                        sbDataString.Append(strTrimedData[3]);  
                                    }

                                    //Read next parameter
                                    lineConent = _StreamReader.ReadLine();   intLineCount++;
                                    //accumulative parameter counter
                                    intParameterCounter++;
                                }

                                #region Get site & actual device number
                                //Get site & actual device number
                                bool isSiteInfoGet = false;
                                while (!isSiteInfoGet)
                                {
                                    lineConent = _StreamReader.ReadLine();   intLineCount++;
                                    if (lineConent.Contains("Device Results:"))
                                    {
                                        lineConent = _StreamReader.ReadLine();   intLineCount++;
                                        lineConent = _StreamReader.ReadLine();   intLineCount++;
                                        lineConent = _StreamReader.ReadLine();   intLineCount++;//Jump to Site#1 & actual device number
                                        strOriginalData = new string[15];
                                        strOriginalData = lineConent.Split(' ');
                                        sbDataString.Append(",");
                                        sbDataString.Append(strOriginalData[5]); // Site number
                                        sbDataString.Append(",");
                                        sbDataString.Append(strOriginalData[7]); // Actual device number
                                        
                                        isSiteInfoGet = true;
                                    }   //end of if (lineConent.Contains("Device Results:"))
                                }   //end of while (isSiteInfoGet)

                                #endregion Get site & actual device number

                                #endregion *** Single device data ***
                            }

                            //Detemine if data header is complete
                            if (!isDataHeaderGet)
                            {
                                if (strParameterString.Length < strParameterBuffer.Length)
                                {
                                    strParameterString = strParameterBuffer;
                                    strUnitsString = strUnitsBuffer;
                                    strLowLimitString = strLowLimitBuffer;
                                    strHighLimitString = strHighLimitBuffer;

                                    intFixedParameterCounter = intParameterCounter;
                                    //intisDataHeaderGetCount = 0;
                                }
                                else
                                {
                                    //    intisDataHeaderGetCount++;
                                    //    if (intisDataHeaderGetCount > 10)
                                    isDataHeaderGet = true;
                                }
                            }

                            //Check if any error on the data parse
                            if (intParameterCounter != intFixedParameterCounter)
                            {
                                throw new Exception("Previous parameter count is: " + intFixedParameterCounter.ToString() + "\nCurrent parameter count is: " + intParameterCounter.ToString());
                            }
                        }
                        #endregion *** Device section ***
                        //Jump to next device
                        lineConent = _StreamReader.ReadLine();   intLineCount++;
                    }
                    strDataString = sbDataString.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Parse data error \n " + ex.Message);
                }

                //Timer session
                ts = DateTime.Now - dtStart;
                ParseTime = ts.TotalMilliseconds;
                dtStart = DateTime.Now;

                #endregion *** Parse test data from txt file ***

                #region *** Parse string into datatable ***
                try
                {
                    //Timer session
                    dtStart = DateTime.Now;

                    //Generate Column            
                    string[] Parameter = strParameterString.Split(',');
                    int intColumnLength = Parameter.Length;
                    int i;

                    for (i = 0; i < intColumnLength + 1; i++)
                    {
                        if (i == 0)
                            DataParseResult.Columns.Add(new DataColumn("Device#", typeof(string)));
                        else
                            DataParseResult.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
                    }
                    //DataParseResult.Columns.Add(new DataColumn(i.ToString(), typeof(string)));  //add Status column
                    //Insert Parameter 

                    dr = DataParseResult.NewRow();
                    for (i = 0; i < intColumnLength; i++)
                    {
                        dr[i] = Parameter[i];
                    }
                    dr[i] = "Status";
                    DataParseResult.Rows.Add(dr);

                    //Insert Units  
                    string[] Units = strUnitsString.Split(',');
                    dr = DataParseResult.NewRow();
                    for (i = 0; i < intColumnLength; i++)
                    {
                        dr[i] = Units[i];
                    }
                    DataParseResult.Rows.Add(dr);

                    //Insert Low Limit  
                    string[] LowLimit = strLowLimitString.Split(',');
                    dr = DataParseResult.NewRow();

                    for (i = 0; i < intColumnLength; i++)
                    {
                        dr[i] = LowLimit[i];
                    }
                    DataParseResult.Rows.Add(dr);

                    //Insert High Limit
                    string[] HighLimit = strHighLimitString.Split(',');
                    dr = DataParseResult.NewRow();

                    for (i = 0; i < intColumnLength; i++)
                    {
                        dr[i] = HighLimit[i];
                    }
                    DataParseResult.Rows.Add(dr);

                    //Insert Test Data
                    string[] Device = strDataString.Split('&');
                    int intDeviceCount = Device.Length;
                    //m_TestedDevice = intDeviceCount;

                    for (i = 0; i < intDeviceCount; i++)
                    {
                        bool PassStatus = true;
                        string[] DeviceData = Device[i].Split(',');
                        dr = DataParseResult.NewRow();

                        if (DeviceData.Length > Parameter.Length)
                            throw new Exception("DeviceData.Length = " + DeviceData.Length.ToString() +
                                                                        "  /  Parameter.Length = " + Parameter.Length.ToString() + "DeviceID = " + i.ToString());

                        dr[0] = DeviceData[0];
                        for (int j = 1; j < DeviceData.Length; j++)
                        {
                            if (j < 3)
                                dr[j] = DeviceData[j + DeviceData.Length - 3];
                            else
                            {
                                dr[j] = DeviceData[j - 2];
                                try
                                {
                                    if ((Convert.ToDouble(DeviceData[j - 2]) < Convert.ToDouble(LowLimit[j])
                                                                   || Convert.ToDouble(DeviceData[j - 2]) > Convert.ToDouble(HighLimit[j])))
                                    {
                                        PassStatus = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //throw new Exception("DeviceID = " + i.ToString() + "  /  " + ex.Message);
                                    dr[j] = Convert.ToString(-99);
                                    PassStatus = false;
                                }
                            }
                        }
                        if (PassStatus)
                        {
                            dr[DataParseResult.Columns.Count - 1] = "Pass";
                            //m_PassedDevice++;
                        }
                        else
                        {
                            dr[DataParseResult.Columns.Count - 1] = "Fail";
                            //m_FailedDevice++;
                        }
                        DataParseResult.Rows.Add(dr);
                    }

                    //Timer session
                    ts = DateTime.Now - dtStart;
                    InsertTime = ts.TotalMilliseconds;
                }
                catch (Exception ex)
                {
                    throw new Exception("Insert into datatable error \n " + ex.Message);
                }


                #endregion Parse string into datatable
            }
            //Dual site
            else if (intSiteNumber == 2)
            {
                #region *** Parse test data from txt file ***
                try
                {
                    //Timer session
                    dtStart = DateTime.Now;

                    #region *** Read to memory ***
                    //string content = string.Empty;
                    //using (StreamReader ms_StreamReader = new StreamReader(fileName))
                    //{
                    //    content = ms_StreamReader.ReadToEnd();//一次性读入内存
                    //}
                    //MemoryStream _MemoryStream = new MemoryStream(Encoding.GetEncoding("GB2312").GetBytes(content));//放入内存流，以便逐行读取
                    //StreamReader _StreamReader = new StreamReader(_MemoryStream);

                    //ts = DateTime.Now - dtStart;
                    //ParseTime = ts.TotalMilliseconds;
                    #endregion *** Read to memory ***

                    #region *** Direct Read ***
                    FileStream _FileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    StreamReader _StreamReader = new StreamReader(_FileStream);
                    _StreamReader.BaseStream.Seek(0, SeekOrigin.Begin);

                    #endregion *** Diresct Read ***

                    intDeviceCounter = 1;
                    lineConent = _StreamReader.ReadLine();
                    while (lineConent != null)
                    {
                        #region *** Header section ***

                        #region *** Product & ProgramRev ***
                        //Get product
                        if (!isParseHeaderFinish && lineConent.Contains("Program Name"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            string[] tempArray = temp.Split('_');
                            m_Header.Product = tempArray[0].Trim();
                            try
                            {
                                m_Header.ProgramRev = tempArray[1].Trim() + "." + tempArray[2].Trim() + "." + tempArray[3].Trim();
                            }
                            catch
                            {
                                try
                                {
                                    m_Header.ProgramRev = tempArray[1].Trim() + "." + tempArray[2].Trim();
                                }
                                catch
                                {
                                    m_Header.ProgramRev = tempArray[1].Trim();
                                }
                            }

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** Product & ProgramRev ***

                        #region *** StartTime ***
                        //Get StartTime
                        if (!isParseHeaderFinish && lineConent.Contains("Start Time"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            string[] tempArray = temp.Split(' ');
                            string[] tempArrayTrimed = new string[5];
                            StringBuilder _StringBuilder = new StringBuilder();
                            int j = 0;

                            for (int i = 0; i < tempArray.Length; i++)
                            {
                                if (tempArray[i] != "")
                                {
                                    tempArrayTrimed[j] = tempArray[i];
                                    j++;
                                }
                            }
                            _StringBuilder.Append(tempArrayTrimed[4]);  //Year
                            _StringBuilder.Append("/");
                            _StringBuilder.Append(tempArrayTrimed[1]);  //Month
                            _StringBuilder.Append("/");
                            _StringBuilder.Append(tempArrayTrimed[2]);  //Day
                            _StringBuilder.Append(" ");
                            _StringBuilder.Append(tempArrayTrimed[3]);  //Time
                            m_Header.LotStartDateTime = Convert.ToDateTime(_StringBuilder.ToString());

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        } //end of is contains
                        #endregion *** StartTime ***

                        #region *** Device Name ***
                        //Get Device Name
                        if (!isParseHeaderFinish && lineConent.Contains("Device Name"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);

                            //m_Header.LotStartDateTime = temp.ToString();

                            m_Header.DeviceName = temp.Trim();
                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        } //end of is contains
                        #endregion *** Device Name ***

                        #region *** Lot Retest ***
                        //Get Lot Retest
                        if (!isParseHeaderFinish && lineConent.Contains("Lot Retest"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);

                            //m_Header.LotStartDateTime = temp.ToString();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        } //end of is contains
                        #endregion *** Lot Retest ***

                        #region *** LotID ***
                        //Get LotID
                        if (!isParseHeaderFinish && lineConent.Contains("Lot ID"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            //string[] tempArray = temp.Split('_');
                            m_Header.LotID = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** LotID ***

                        #region *** Sub LotID ***
                        //Get Sub LotID
                        if (!isParseHeaderFinish && lineConent.Contains("Sublot ID"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            //string[] tempArray = temp.Split('_');
                            m_Header.SubLotID = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** SubLotID ***

                        #region *** Tester ***
                        //Get Tester
                        if (!isParseHeaderFinish && lineConent.Contains("Tester Node Name"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            m_Header.Tester = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** Tester ***

                        #region *** Test Head ***
                        //Get Test Head
                        if (!isParseHeaderFinish && lineConent.Contains("Test Head"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            //m_Header.Tester = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** Test Head ***

                        #region *** TesterType ***
                        //Get TesterType
                        if (!isParseHeaderFinish && lineConent.Contains("Tester Type"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            m_Header.TesterType = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** TesterType ***

                        #region *** Serial Number ***
                        //Get Serial Number
                        if (!isParseHeaderFinish && lineConent.Contains("Serial Number"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            //m_Header.Tester = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** Serial Number ***

                        #region *** Operator ID ***
                        //Get Operator ID
                        if (!isParseHeaderFinish && lineConent.Contains("Operator ID"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            m_Header.OperatorID = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** Operator ID ***

                        #region *** enVision Version ***
                        //Get enVision Version
                        if (!isParseHeaderFinish && lineConent.Contains("enVision Version"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            m_Header.enVision_Version = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** enVision Version ***

                        #region *** FAB ID ***
                        //Get FAB ID
                        if (!isParseHeaderFinish && lineConent.Contains("FAB ID"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            //m_Header.TesterType = temp.Trim();

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** FAB ID ***

                        #region *** Active Flow ***
                        //Get Active Flow
                        if (!isParseHeaderFinish && lineConent.Contains("Active Flow"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            //m_Header.Tester = temp.Trim();
                            isParseHeaderFinish = true;
                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** Active Flow ***

                        #region *** Software Bin Summary ***
                        //Get Software Bin Summary
                        //if (lineConent.Contains("Software Bin Summary"))
                        //{
                        //    int j = 0;
                        //    int BinCount = 0;
                        //    string[] tempArrayTrim = new string[5];
                        //    lineConent = _StreamReader.ReadLine();   intLineCount++;
                        //    lineConent = _StreamReader.ReadLine();   intLineCount++;
                        //    lineConent = _StreamReader.ReadLine();   intLineCount++;
                        //    while (lineConent != "" && BinCount < 7)
                        //    {
                        //        string[] tempArray = lineConent.Split(' ');
                        //        for (int i = 0; i < tempArray.Length; i++)
                        //        {
                        //            if (tempArray[i] != "")
                        //            {
                        //                tempArrayTrim[j] = tempArray[i];
                        //                j++;
                        //            }
                        //        }
                        //        m_Bin.Number[BinCount] = Convert.ToInt32(tempArrayTrim[0]);
                        //        m_Bin.DeviceCount[BinCount] = Convert.ToInt32(tempArrayTrim[2]);
                        //        m_Bin.Percent[BinCount] = Convert.ToDouble(tempArrayTrim[3]);
                        //        m_Bin.Name[BinCount] = tempArrayTrim[4];

                        //        lineConent = _StreamReader.ReadLine();   intLineCount++;
                        //        BinCount++;
                        //        j = 0;
                        //    } //end of while (strline != "")

                        //    //lineConent = _StreamReader.ReadLine();   intLineCount++;
                        //    lineConent = _StreamReader.ReadLine();   intLineCount++; //jump to Device Count Summary
                        //}//end of if contains
                        #endregion *** Software Bin Summary ***

                        #region *** Device Count Summary ***
                        //Get Device Count Summary
                        if (lineConent.Contains("Device Count Summary"))
                        {
                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                            lineConent = _StreamReader.ReadLine();   intLineCount++;

                            string temp = lineConent.Substring(intSubStringStart);
                            string[] tempArray = temp.Split(' ');
                            string[] tempArrayTrimed = new string[5];
                            int j = 0;

                            for (int i = 0; i < tempArray.Length; i++)
                            {
                                if (tempArray[i] != "")
                                {
                                    tempArrayTrimed[j] = tempArray[i];
                                    j++;
                                }
                            }
                            m_TestedDevice = m_Header.TestQuantity = Convert.ToInt32(tempArrayTrimed[0]);
                            m_PassedDevice = m_Header.PassQuantity = Convert.ToInt32(tempArrayTrimed[1]);
                            m_FailedDevice = m_Header.FailQuantity = m_Header.TestQuantity - m_Header.PassQuantity;
                            //Yield
                            double yield = Convert.ToDouble(m_Header.PassQuantity) / Convert.ToDouble(m_Header.TestQuantity) * 100;
                            m_Header.Yield = Math.Round(yield, 2);

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                            lineConent = _StreamReader.ReadLine();   intLineCount++; // jump to lot finish time

                        }//end of if strline.Contains
                        #endregion *** Device Count Summary ***

                        #region *** Finish Time ***
                        //Get Finish Time
                        if (lineConent.Contains("Finish Time"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            string[] tempArray = temp.Split(' ');
                            string[] tempArrayTrimed = new string[5];
                            StringBuilder _StringBuilder = new StringBuilder();
                            int j = 0;

                            for (int i = 0; i < tempArray.Length; i++)
                            {
                                if (tempArray[i] != "")
                                {
                                    tempArrayTrimed[j] = tempArray[i];
                                    j++;
                                }
                            }
                            _StringBuilder.Append(tempArrayTrimed[4]);  //Year
                            _StringBuilder.Append("/");
                            _StringBuilder.Append(tempArrayTrimed[1]);  //Month
                            _StringBuilder.Append("/");
                            _StringBuilder.Append(tempArrayTrimed[2]);  //Day
                            _StringBuilder.Append(" ");
                            _StringBuilder.Append(tempArrayTrimed[3]);  //Time
                            m_Header.LotFinishDateTime = Convert.ToDateTime(_StringBuilder.ToString());

                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        } //end of if contains
                        #endregion *** StartTime ***

                        #region *** Lot Description ***
                        //Get FAB ID
                        if (lineConent.Contains("Lot Description"))
                        {
                            string temp = lineConent.Substring(intSubStringStart);
                            //m_Header.TesterType = temp.Trim();

                            m_Header.LotDesc = temp.Trim();
                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                        }
                        #endregion *** FAB ID ***

                        #region *** Test Mode ***
                        ////Get Test
                        //if (isParseHeaderFinish && !isTestModeGet)
                        //{
                        //    string lotName = "";
                        //    if (m_Header.LotID != null)
                        //        lotName = m_Header.LotID;
                        //    else
                        //        lotName = m_Header.SubLotID;

                        //    string[] temp = lotName.Split('_');
                        //    m_Header.TestMode = temp[1].Trim();
                        //    isTestModeGet = true;
                        //}
                        #endregion *** Test Mode ***

                        #endregion *** Header section ***

                        #region *** Device section ***

                        if (lineConent.Contains("Test Description"))
                        {
                            lineConent = _StreamReader.ReadLine();   intLineCount++;
                            lineConent = _StreamReader.ReadLine();   intLineCount++; //Jump to GSM idle Current
                            ////accumulative parameter counter
                            //intDeviceCounter++;
                            //Reset parameter counter
                            intParameterCounter = 0;
                            //Reset Header buffer
                            strParameterBuffer = "Test Item,Site No,Device ID";
                            strLowLimitBuffer = "Low Limit,,";
                            strHighLimitBuffer = "High Limit,,";
                            strUnitsBuffer = "Units,,";

                            StringBuilder sbSite1DataTemp = new StringBuilder();
                            StringBuilder sbSite2DataTemp = new StringBuilder();
                            sbSite1DataTemp.Append(intDeviceCounter);
                            sbSite2DataTemp.Append(intDeviceCounter + 1);

                            #region *** Single device data ***

                            //Get test result and get data header at fisrt device                   
                            while (lineConent != "" && lineConent != null)
                            {
                                //strline = strline.Trim();
                                strOriginalData = new string[8];
                                strTrimedData = new string[8];
                                int i;

                                try
                                {
                                    strTrimedData[0] = lineConent.Substring(11, 10).Trim();   //Low limt
                                    strTrimedData[1] = lineConent.Substring(28, 10).Trim();   //High limit
                                    strTrimedData[2] = lineConent.Substring(38, 5).Trim();    //Units
                                    strTrimedData[3] = lineConent.Substring(48, 10).Trim();   //Site#1 result
                                    strTrimedData[4] = lineConent.Substring(68, 10).Trim();   //Site#2 result
                                    strTrimedData[5] = lineConent.Substring(100).Trim();      //Parameter name
                                    strTrimedData[6] = lineConent.Substring(58, 3).Trim();    //Site#1 unit
                                    strTrimedData[7] = lineConent.Substring(78, 3).Trim();    //Site#2 unit
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message);
                                }

                                ////trim data string
                                //for (i = 0; i < strOriginalData.Length; i++)
                                //{
                                //    strTrimedData[i] = strOriginalData[i].Trim();
                                //}

                                //Get data header
                                if (!isDataHeaderGet)
                                {
                                    strParameterBuffer += "," + strTrimedData[5];
                                    strLowLimitBuffer += "," + strTrimedData[0];

                                    if (strTrimedData[2].ToLower() == "ks")
                                    {
                                        strTrimedData[1] = (Convert.ToDouble(strTrimedData[1]) * 1000 * 1000).ToString();
                                        strHighLimitBuffer += "," + strTrimedData[1];
                                        strUnitsBuffer += "," + "ms";
                                    }
                                    else
                                    {
                                        strHighLimitBuffer += "," + strTrimedData[1];
                                        strUnitsBuffer += "," + strTrimedData[2];
                                    }
                                }

                                //Get site#1 device test result
                                sbSite1DataTemp.Append(",");
                                //Site#1 data
                                if (strTrimedData[6].ToLower() == "s")
                                {
                                    sbSite1DataTemp.Append(Convert.ToDouble(strTrimedData[3]) * 1000);
                                }
                                else if (strTrimedData[6].ToLower() == "ks")
                                {
                                    sbSite1DataTemp.Append(Convert.ToDouble(strTrimedData[3]) * 1000 * 1000);
                                }
                                else 
                                {
                                    sbSite1DataTemp.Append(strTrimedData[3]);
                                }
                              
                                //Get site#2 device test result
                                sbSite2DataTemp.Append(",");
                                //Site#2 data
                                if (strTrimedData[7].ToLower() == "s")
                                {
                                    sbSite2DataTemp.Append(Convert.ToDouble(strTrimedData[4]) * 1000);
                                }
                                else if (strTrimedData[7].ToLower() == "ks")
                                {
                                    sbSite2DataTemp.Append(Convert.ToDouble(strTrimedData[4]) * 1000);
                                }
                                else
                                {
                                    sbSite2DataTemp.Append(strTrimedData[4]);
                                }

                                //Read next parameter
                                lineConent = _StreamReader.ReadLine();   intLineCount++;
                                //accumulative parameter counter
                                intParameterCounter++;
                            }

                            #region Get site & actual device number
                            //Get site & actual device number
                            bool isSiteInfoGet = false;
                            while (!isSiteInfoGet && lineConent != null)
                            {
                                lineConent = _StreamReader.ReadLine();   intLineCount++;
                                if (lineConent.Contains("Device Results:"))
                                {
                                    lineConent = _StreamReader.ReadLine();   intLineCount++;
                                    lineConent = _StreamReader.ReadLine();   intLineCount++;
                                    lineConent = _StreamReader.ReadLine();   intLineCount++;//Jump to Site#1 & actual device number
                                    strOriginalData = new string[15];
                                    strOriginalData = lineConent.Split(' ');

                                    if (Convert.ToInt32(strOriginalData[5]) == 1)
                                    {
                                        sbSite1DataTemp.Append(",");
                                        sbSite1DataTemp.Append(strOriginalData[5]); // Site number
                                        sbSite1DataTemp.Append(",");
                                        sbSite1DataTemp.Append(strOriginalData[7]); // Actual device number

                                        if (intDeviceCounter == 1)
                                            sbDataString.Append(sbSite1DataTemp.ToString());
                                        else
                                        {
                                            sbDataString.Append("&");
                                            sbDataString.Append(sbSite1DataTemp.ToString());
                                        }
                                    }
                                    else if (Convert.ToInt32(strOriginalData[5]) == 2) // if only site#2, dispose site#1 data
                                    {
                                        int a = sbSite2DataTemp.ToString().IndexOf(",");
                                        sbSite2DataTemp.Remove(0, a);
                                        sbSite2DataTemp.Insert(0, intDeviceCounter);
                                        sbSite2DataTemp.Append(",");
                                        sbSite2DataTemp.Append(strOriginalData[5]); // Site number
                                        sbSite2DataTemp.Append(",");
                                        sbSite2DataTemp.Append(strOriginalData[7]); // Actual device number

                                        if (intDeviceCounter == 1)
                                            sbDataString.Append(sbSite2DataTemp.ToString());
                                        else
                                        {
                                            sbDataString.Append("&");
                                            sbDataString.Append(sbSite2DataTemp.ToString());
                                        }
                                    }
                                    intDeviceCounter++;
                                    //Jump to next site & actual device number
                                    lineConent = _StreamReader.ReadLine();   intLineCount++;
                                    if (lineConent != "")
                                    {
                                        strOriginalData = new string[15];
                                        strOriginalData = lineConent.Split(' ');
                                        sbSite2DataTemp.Append(",");
                                        sbSite2DataTemp.Append(strOriginalData[5]); // Site number
                                        sbSite2DataTemp.Append(",");
                                        sbSite2DataTemp.Append(strOriginalData[7]); // Actual device number
                                        intDeviceID = Convert.ToInt32(strOriginalData[7]);
                                        intDeviceCounter++;
                                        sbDataString.Append("&");
                                        sbDataString.Append(sbSite2DataTemp.ToString());
                                        //Jump to next site & actual device number
                                        lineConent = _StreamReader.ReadLine();   intLineCount++;
                                    }
                                    isSiteInfoGet = true;
                                }   //end of if (lineConent.Contains("Device Results:"))
                            }   //end of while (isSiteInfoGet)

                            #endregion Get site & actual device number

                            #endregion *** Single device data ***

                            //Detemine if data header is complete
                            if (!isDataHeaderGet)
                            {
                                if (strParameterString.Length < strParameterBuffer.Length)
                                {
                                    strParameterString = strParameterBuffer;
                                    strUnitsString = strUnitsBuffer;
                                    strLowLimitString = strLowLimitBuffer;
                                    strHighLimitString = strHighLimitBuffer;

                                    intFixedParameterCounter = intParameterCounter;
                                    //intisDataHeaderGetCount = 0;
                                }
                                else
                                {
                                    //    intisDataHeaderGetCount++;
                                    //    if (intisDataHeaderGetCount > 1000)
                                    isDataHeaderGet = true;
                                }
                            }

                            //Check if any error on the data parse
                            if (intParameterCounter != intFixedParameterCounter)
                            {
                                throw new Exception("Previous parameter count is: " + intFixedParameterCounter.ToString() + "\nCurrent parameter count is: " + intParameterCounter.ToString());
                            }

                        }
                        #endregion *** Device section ***
                        //Jump to next device
                        lineConent = _StreamReader.ReadLine();   intLineCount++;
                    }
                    strDataString = sbDataString.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Parse data error \n  Line Number: " + intLineCount.ToString() + " \n  Device ID " + intDeviceID.ToString() + "\n\n" + ex.Message);
                }

                //Timer session
                ts = DateTime.Now - dtStart;
                ParseTime = ts.TotalMilliseconds;
                dtStart = DateTime.Now;

                #endregion *** Parse test data from txt file ***

                #region *** Parse string into datatable ***
                try
                {
                    //Timer session
                    dtStart = DateTime.Now;

                    //Generate Column            
                    string[] Parameter = strParameterString.Split(',');
                    int intColumnLength = Parameter.Length;
                    int i;

                    for (i = 0; i < intColumnLength + 1; i++)
                    {
                        if (i == 0)
                            DataParseResult.Columns.Add(new DataColumn("Device#", typeof(string)));
                        else
                            DataParseResult.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
                    }
                    //DataParseResult.Columns.Add(new DataColumn(i.ToString(), typeof(string)));  //add Status column
                    //Insert Parameter 

                    dr = DataParseResult.NewRow();
                    for (i = 0; i < intColumnLength; i++)
                    {
                        dr[i] = Parameter[i];
                    }
                    dr[i] = "Status";
                    DataParseResult.Rows.Add(dr);

                    //Insert Units  
                    string[] Units = strUnitsString.Split(',');
                    dr = DataParseResult.NewRow();
                    for (i = 0; i < intColumnLength; i++)
                    {
                        dr[i] = Units[i];
                    }
                    DataParseResult.Rows.Add(dr);

                    //Insert Low Limit  
                    string[] LowLimit = strLowLimitString.Split(',');
                    dr = DataParseResult.NewRow();

                    for (i = 0; i < intColumnLength; i++)
                    {
                        dr[i] = LowLimit[i];
                    }
                    DataParseResult.Rows.Add(dr);

                    //Insert High Limit
                    string[] HighLimit = strHighLimitString.Split(',');
                    dr = DataParseResult.NewRow();

                    for (i = 0; i < intColumnLength; i++)
                    {
                        dr[i] = HighLimit[i];
                    }
                    DataParseResult.Rows.Add(dr);

                    //Insert Test Data
                    string[] Device = strDataString.Split('&');
                    int intDeviceCount = Device.Length;
                    //m_TestedDevice = intDeviceCount;

                    for (i = 0; i < intDeviceCount; i++)
                    {
                        bool PassStatus = true;
                        string[] DeviceData = Device[i].Split(',');
                        dr = DataParseResult.NewRow();

                        if (DeviceData.Length > Parameter.Length)
                            throw new Exception("DeviceData.Length = " + DeviceData.Length.ToString() +
                                                                        "  /  Parameter.Length = " + Parameter.Length.ToString());

                        dr[0] = DeviceData[0];
                        for (int j = 1; j < DeviceData.Length; j++)
                        {
                            if (j < 3)
                                dr[j] = DeviceData[j + DeviceData.Length - 3];
                            else
                            {
                                dr[j] = DeviceData[j - 2];
                                if (DeviceData[j - 2] != "")
                                {
                                    try
                                    {
                                        if ((Convert.ToDouble(DeviceData[j - 2]) < Convert.ToDouble(LowLimit[j])
                                                                       || Convert.ToDouble(DeviceData[j - 2]) > Convert.ToDouble(HighLimit[j])))
                                        {
                                            PassStatus = false;
                                        }
                                    }
                                    catch(Exception ex)
                                    {
                                        //throw new Exception("Device ID: " + i.ToString() + "\r\n" + ex.Message);
                                        dr[j] = Convert.ToString(-99);
                                        PassStatus = false;
                                    }
                                }
                            }
                        }
                        if (PassStatus)
                        {
                            dr[DataParseResult.Columns.Count - 1] = "Pass";
                            //m_PassedDevice++;
                        }
                        else
                        {
                            dr[DataParseResult.Columns.Count - 1] = "Fail";
                            //m_FailedDevice++;
                        }
                        DataParseResult.Rows.Add(dr);
                    }

                    //Timer session
                    ts = DateTime.Now - dtStart;
                    InsertTime = ts.TotalMilliseconds;
                }
                catch (Exception ex)
                {
                    throw new Exception("Insert into datatable error \n " + ex.Message);
                }


                #endregion Parse string into datatable
            }

            DataParseResult.PrimaryKey = new DataColumn[] { DataParseResult.Columns[0] };
            return DataParseResult;
            
        }//end of GetDataFromTxt

        ///<summary>
        ///<para>Parse test data from LTX txt test result in to datatable (multifile)</para>
        ///<seealso cref="DataParse.GetDataFromTxt"/>
        ///</summary>
        /// <param name="fileName">Full patch with file name of the txt file (multifile)</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataFromTxt(string[] arrayFileName)
        {
            #region *** Variable define ***
            DataTable DataParseResult = new DataTable();
            DataTable DataParseResultTemp = new DataTable();

            double parseTime = 0;
            double insertTime = 0;

            int deviceCount = 0;

            int testedDevice = 0;
            int passedDevice = 0;
            int failedDevice = 0;

            DataHeader header = new Common.DataHeader();
            Bin bin = new Common.Bin();

            bool isFirstFile = true;
            bool isFirstTable = true;

            DateTime dtLotStart = DateTime.Now;
            DateTime dtLotFinish = DateTime.Now;

            Dictionary<string, int> headerDic = new Dictionary<string, int>();

            #endregion *** Variable define ***

            #region *** Innitial Properties ***
            bin.DeviceCount = new int[8];
            bin.Name = new string[8];
            bin.Number = new int[8];
            bin.Percent = new double[8];

            header.SessionCount = 0;

            #endregion *** Innitial Properties ***

            #region *** Get and merge data table ***
            try
            {
                foreach (string fileName in arrayFileName)
                {
                    DataParseResultTemp = this.GetDataFromTxt(fileName);

                    #region *** Caculate Properties ***
                    header.SessionCount++;

                    parseTime += m_ParseTime;
                    insertTime += m_InsertTime;
                    testedDevice += m_TestedDevice;
                    passedDevice += m_PassedDevice;
                    failedDevice += m_FailedDevice;

                    header.FailQuantity += m_Header.FailQuantity;
                    header.LotQuantity += m_Header.LotQuantity;
                    header.PassQuantity += m_Header.PassQuantity;
                    header.TestQuantity += m_Header.TestQuantity;
                    header.enVision_Version = m_Header.enVision_Version;

                    if (isFirstFile)
                    {
                        header.LotID = m_Header.LotID;
                        header.SubLotID = m_Header.SubLotID;
                        header.OperatorID = m_Header.OperatorID;
                        header.TestBoard = m_Header.TestBoard;
                        header.DeviceName = m_Header.DeviceName;
                        header.Tester = m_Header.Tester;
                        header.Handler = m_Header.Handler;
                        header.TesterType = m_Header.TesterType;
                        header.Product = m_Header.Product;
                        header.ProgramRev = m_Header.ProgramRev;

                        dtLotStart = Convert.ToDateTime(m_Header.LotStartDateTime);
                        dtLotFinish = Convert.ToDateTime(m_Header.LotFinishDateTime);

                        isFirstFile = false;
                    }
                    else
                    {
                        if (m_Header.LotID != "" && m_Header.LotID != null)
                            header.LotID = header.LotID + " & " + m_Header.LotID;

                        if (m_Header.SubLotID != "" && m_Header.SubLotID != null)
                            header.SubLotID = header.SubLotID + " & " + m_Header.SubLotID;

                        if (m_Header.OperatorID != "" && m_Header.OperatorID != null)
                            header.OperatorID = header.OperatorID + " & " + m_Header.OperatorID;

                        if (m_Header.TestBoard != "" && m_Header.TestBoard != null)
                            header.TestBoard = header.TestBoard + " & " + m_Header.TestBoard;

                        if (m_Header.Tester != "" && m_Header.Tester != null)
                            header.Tester = header.Tester + " & " + m_Header.Tester;

                        if (m_Header.TesterType != "" && m_Header.TesterType != null)
                            header.TesterType = header.TesterType + " & " + m_Header.TesterType;

                        if (m_Header.Handler != "" && m_Header.Handler != null)
                            header.Handler = header.Handler + " & " + m_Header.Handler;

                        //Lot Start Datetime
                        if (m_Header.LotStartDateTime < dtLotStart)
                        {
                            dtLotStart = m_Header.LotStartDateTime;
                        }
                        //Lot Finish Datetime
                        if (m_Header.LotFinishDateTime > dtLotFinish)
                        {
                            dtLotFinish = m_Header.LotFinishDateTime;
                        }

                        //Check Product Name
                        if (m_Header.Product != header.Product)
                        {
                            throw new Exception("file " + fileName + " has different Product name with before.");
                        }
                        //Check Program Rev
                        if (m_Header.ProgramRev != header.ProgramRev)
                        {
                            throw new Exception("file " + fileName + " has different Program Rev with before.");
                        }
                    }//end of if firstfile

                    header.Yield = Math.Round(Convert.ToDouble(header.PassQuantity) / Convert.ToDouble(header.TestQuantity) * 100, 3);
                    header.LotStartDateTime = dtLotStart;
                    header.LotFinishDateTime = dtLotFinish;

                    for (int i = 0; i < 8; i++)
                    {
                        bin.DeviceCount[i] += m_Bin.DeviceCount[i];
                        bin.Name[i] = m_Bin.Name[i];
                        bin.Number[i] = m_Bin.Number[i];
                        bin.Percent[i] = Math.Round(Convert.ToDouble(bin.DeviceCount[i]) / Convert.ToDouble(m_TestedDevice) * 100, 3);
                    }
                    #endregion *** Caculate Properties ***

                    #region *** Merge Data Table ***
                    if (isFirstTable)
                    {
                        //Build table structure
                        DataParseResult = DataParseResultTemp.Clone();
                        foreach (DataRow dr in DataParseResultTemp.Rows)
                        {
                            DataParseResult.ImportRow(dr);
                        }
                        //get first table device count
                        deviceCount = DataParseResult.Rows.Count - 4;
                        isFirstTable = false;
                    }
                    else
                    {
                        //merge less columns datatable to more columns datatable
                        DataParseResultTemp.PrimaryKey = null;
                        if (DataParseResultTemp.Columns.Count > DataParseResult.Columns.Count)
                        {
                            foreach (DataRow dr in DataParseResult.Rows)
                            {
                                if (DataParseResult.Rows.IndexOf(dr) > 3)
                                {
                                    //reset device count
                                    deviceCount++;
                                    dr[0] = deviceCount;
                                    DataParseResultTemp.ImportRow(dr);
                                    //Move Status Column to last
                                    int lastRowIndex = DataParseResultTemp.Rows.Count - 1;
                                    int lastColumnIndex = DataParseResultTemp.Columns.Count - 1;
                                    DataParseResultTemp.Rows[lastRowIndex][lastColumnIndex] = dr[DataParseResult.Columns.Count - 1];
                                    DataParseResultTemp.Rows[lastRowIndex][DataParseResult.Columns.Count - 1] = null;
                                }
                            }
                            DataParseResult = DataParseResultTemp;
                        }
                        else
                        {
                            DataParseResultTemp.PrimaryKey = null;
                            foreach (DataRow dr in DataParseResultTemp.Rows)
                            {
                                if (DataParseResultTemp.Rows.IndexOf(dr) > 3)
                                {
                                    //reset device count
                                    deviceCount++;
                                    dr[0] = deviceCount;
                                    DataParseResult.ImportRow(dr);
                                    //Move Status Column to last
                                    if (DataParseResultTemp.Columns.Count < DataParseResult.Columns.Count)
                                    {
                                        int lastRowIndex = DataParseResult.Rows.Count - 1;
                                        int lastColumnIndex = DataParseResult.Columns.Count - 1;
                                        DataParseResult.Rows[lastRowIndex][lastColumnIndex] = dr[DataParseResultTemp.Columns.Count - 1];
                                        DataParseResult.Rows[lastRowIndex][DataParseResultTemp.Columns.Count - 1] = null;
                                    }
                                }
                            }//end of foreach (DataRow dr in DataParseResultTemp.Rows)
                        }//end of if (DataParseResultTemp.Columns.Count > DataParseResult.Columns.Count)
                    }// end of if else (isFirstTable)

                    #endregion *** Merge Data Table ***

                }//end of foreach (string fileName in arrayFileName)

                #region *** Final Properties ***
                m_Bin = bin;
                m_Header = header;
                m_FailedDevice = failedDevice;
                m_PassedDevice = passedDevice;
                m_TestedDevice = testedDevice;
                m_ParseTime = parseTime;
                m_InsertTime = insertTime;

                #endregion *** Final Properties ***
            }
            catch (Exception ex)
            {
                throw new Exception("Parse data error \n " + ex.Message);
            }

            #endregion *** Parse test data from txt file ***

            DataParseResult.PrimaryKey = new DataColumn[] { DataParseResult.Columns[0] };
            return DataParseResult;

        }//end of GetDataFromTxt MultiFile

        ///<summary>
        ///<para>Parse test data from LTX std test result in to datatable</para>
        ///<seealso cref="DataParse.GetDataFromTxt"/>
        ///</summary>
        /// <param name="fileName">Full patch with file name of the std file</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataFromStd(string fileName)
        {
            #region *** Variable define ***
            DateTime dtStart;
            TimeSpan ts;

            DataRow dr;
            DataTable DataParseResult = new DataTable();
            StringBuilder sbData1 = new StringBuilder();
            StringBuilder sbData2 = new StringBuilder();

            StringBuilder sbColumnName = new StringBuilder("Device#,1,2,3,4");
            StringBuilder sbParameter = new StringBuilder("Test Item,Site No,Soft Bin,Hard Bin,Device ID");
            StringBuilder sbLowLimit = new StringBuilder("Low Limit,,,,");
            StringBuilder sbHighLimit = new StringBuilder("High Limit,,,,");
            StringBuilder sbUnits = new StringBuilder("Units,,,,");

            bool isLimitSet = false;
            int ParameterLength = 0;
            int intColumnLength = 0;
            int intDeviceCounter = 1;
            int i;

            string PassFail1, PassFail2;


            #endregion *** Variable define ***

            #region *** Initialize properties ***
            m_FreezeColumn = 5;
            m_ParseTime = 0;
            m_InsertTime = 0;
            m_TestedDevice = 0;
            m_PassedDevice = 0;
            m_FailedDevice = 0;
            m_Header = new DataHeader();
            m_Bin = new Bin();
            m_Bin.DeviceCount = new int[8];
            m_Bin.Name = new string[8];
            m_Bin.Number = new int[8];
            m_Bin.Percent = new double[8];

            m_Header.SessionCount = 0;

            #endregion *** Initialize properties ***

            //Timer session
            dtStart = DateTime.Now;

            STDFManager stdf = new STDFManager(STDFFileMode.Read, fileName);
            STDFRecord lineRecord = null;
            //sbData.Append(intDeviceCounter);
            while ((lineRecord = stdf.ReadRecord()) != null)
            {
                #region --- STDF File Version Check ---
                //STDF file version
                if (lineRecord.RECORD_TYPE == STDFRecordType.FAR)
                {
                    int stdf_ver = int.Parse(((FARRecord)lineRecord).STDF_VER.ToString());
                    if (stdf_ver != 4) throw new Exception("Datalog is not in STDF_V4 fromat");
                }
                #endregion --- STDF File Version Check ---

                #region --- Datalog Header Information ---
                // Lot information
                if (lineRecord.RECORD_TYPE == STDFRecordType.MIR)
                {
                    string temp = ((MIRRecord)lineRecord).JOB_NAM;
                    this.GetProgramNameRev(temp);

                    m_Header.SessionCount++;

                    m_Header.enVision_Version = ((MIRRecord)lineRecord).EXEC_VER;
                    m_Header.LotID = ((MIRRecord)lineRecord).LOT_ID;
                    m_Header.Tester = ((MIRRecord)lineRecord).NODE_NAM;
                    m_Header.DeviceName = ((MIRRecord)lineRecord).PART_TYP;
                    m_Header.SubLotID = ((MIRRecord)lineRecord).SBLOT_ID;
                    m_Header.LotStartDateTime = ((MIRRecord)lineRecord).START_T;
                    m_Header.TesterType = ((MIRRecord)lineRecord).TSTR_TYP;
                }

                if (lineRecord.RECORD_TYPE == STDFRecordType.MRR)
                {
                    m_Header.LotFinishDateTime = ((MRRRecord)lineRecord).FINISH_T;
                    m_Header.LotDesc = ((MRRRecord)lineRecord).USR_DESC;
                }
                // Device Count
                if (lineRecord.RECORD_TYPE == STDFRecordType.PCR)
                {
                    m_TestedDevice = m_Header.TestQuantity = (int)((PCRRecord)lineRecord).PART_CNT;
                    m_PassedDevice = m_Header.PassQuantity = (int)((PCRRecord)lineRecord).GOOD_CNT;
                    m_FailedDevice = m_Header.FailQuantity = m_Header.TestQuantity - m_Header.PassQuantity;
                    //Yield
                    double yield = Convert.ToDouble(m_Header.PassQuantity) / Convert.ToDouble(m_Header.TestQuantity) * 100;
                    m_Header.Yield = Math.Round(yield, 2);
                }
                #endregion Datalog header information

                try
                {
                    #region --- Device Test Result ---
                    while (lineRecord.RECORD_TYPE == STDFRecordType.PTR)
                    {
                        #region -- Parse PTR Data From STD File --
                        if (!isLimitSet)
                        {
                            if (((PTRRecord)lineRecord).UNITS.ToLower() == "s" && ((PTRRecord)lineRecord).SITE_NUM == 1)
                            {
                                sbColumnName.Append("," + ((PTRRecord)lineRecord).TEST_NUM);
                                sbParameter.Append("," + ParameterParse(((PTRRecord)lineRecord).TEST_TXT));
                                sbLowLimit.Append("," + Math.Round(((PTRRecord)lineRecord).LO_LIMIT * 1000, 3));
                                sbHighLimit.Append("," + Math.Round(((PTRRecord)lineRecord).HI_LIMIT * 1000, 3));
                                sbUnits.Append(",m" + ((PTRRecord)lineRecord).UNITS);
                            }
                            else if (((PTRRecord)lineRecord).SITE_NUM == 1)
                            {
                                sbColumnName.Append("," + ((PTRRecord)lineRecord).TEST_NUM);
                                sbParameter.Append("," + ParameterParse(((PTRRecord)lineRecord).TEST_TXT));
                                sbLowLimit.Append("," + ResultScale(((PTRRecord)lineRecord).LO_LIMIT, ((PTRRecord)lineRecord).RES_SCAL));
                                sbHighLimit.Append("," + ResultScale(((PTRRecord)lineRecord).HI_LIMIT, ((PTRRecord)lineRecord).RES_SCAL));
                                sbUnits.Append("," + UnitScale(((PTRRecord)lineRecord).UNITS, ((PTRRecord)lineRecord).RES_SCAL));
                            }
                        }

                        if (((PTRRecord)lineRecord).SITE_NUM == 1)
                        {
                            if (((PTRRecord)lineRecord).UNITS.ToLower() == "s")
                                sbData1.Append("," + Math.Round(((PTRRecord)lineRecord).RESULT * 1000, 3));
                            else
                                sbData1.Append("," + ResultScale(((PTRRecord)lineRecord).RESULT, ((PTRRecord)lineRecord).RES_SCAL));
                        }
                        else if (((PTRRecord)lineRecord).SITE_NUM == 2)
                        {
                            if (((PTRRecord)lineRecord).UNITS.ToLower() == "s")
                                sbData2.Append("," + Math.Round(((PTRRecord)lineRecord).RESULT * 1000, 3));
                            else
                                sbData2.Append("," + ResultScale(((PTRRecord)lineRecord).RESULT, ((PTRRecord)lineRecord).RES_SCAL));

                        }

                        lineRecord = stdf.ReadRecord();     //jump to PRR

                        #endregion -- Parse ptr data from std file --

                        while (lineRecord.RECORD_TYPE == STDFRecordType.PRR)
                        {
                            #region -- Parse PRR Data From STD File --
                            // Parse Site#1 PRR Data
                            if (((PRRRecord)lineRecord).SITE_NUM == 1)
                            {
                                if (((PRRRecord)lineRecord).PartPassed)
                                    PassFail1 = "Pass";
                                else
                                    PassFail1 = "Fail";
                                sbData1.Append("," + PassFail1);

                                sbData1.Insert(0, intDeviceCounter + "," + ((PRRRecord)lineRecord).SITE_NUM.ToString()
                                                                   + "," + ((PRRRecord)lineRecord).SOFT_BIN.ToString()
                                                                   + "," + ((PRRRecord)lineRecord).HARD_BIN.ToString()
                                                                   + "," + ((PRRRecord)lineRecord).PART_ID.ToString());

                            }
                            else // Parse Site#2 PRR Data
                            {
                                if (((PRRRecord)lineRecord).PartPassed)
                                    PassFail2 = "Pass";
                                else
                                    PassFail2 = "Fail";
                                sbData2.Append("," + PassFail2);

                                sbData2.Insert(0, intDeviceCounter + "," + ((PRRRecord)lineRecord).SITE_NUM.ToString()
                                                                   + "," + ((PRRRecord)lineRecord).SOFT_BIN.ToString()
                                                                   + "," + ((PRRRecord)lineRecord).HARD_BIN.ToString()
                                                                   + "," + ((PRRRecord)lineRecord).PART_ID.ToString());

                            }
                            #endregion -- Parse prrdata from std file --

                            #region -- Build Datatable and Insert limit info --
                            if (!isLimitSet)
                            {
                                ParameterLength = ((PRRRecord)lineRecord).NUM_TEST;
                                intColumnLength = ParameterLength + 6;
                                // Build Data Table
                                sbColumnName.Append(",*");
                                string[] ColumnName = sbColumnName.ToString().Split(',');
                                for (i = 0; i < ColumnName.Length; i++)
                                {
                                    DataParseResult.Columns.Add(new DataColumn(ColumnName[i], typeof(string)));
                                }
                                //Insert Parameter 
                                string[] Parameter = sbParameter.ToString().Split(',');
                                dr = DataParseResult.NewRow();
                                for (i = 0; i < intColumnLength - 1; i++)
                                {
                                    dr[i] = Parameter[i];
                                }
                                dr[i] = "Status";
                                DataParseResult.Rows.Add(dr);

                                //Insert Units  
                                string[] Units = sbUnits.ToString().Split(',');
                                dr = DataParseResult.NewRow();
                                for (i = 0; i < intColumnLength - 1; i++)
                                {
                                    dr[i] = Units[i];
                                }
                                DataParseResult.Rows.Add(dr);

                                //Insert Low Limit  
                                string[] LowLimit = sbLowLimit.ToString().Split(',');
                                dr = DataParseResult.NewRow();

                                for (i = 0; i < intColumnLength - 1; i++)
                                {
                                    dr[i] = LowLimit[i];
                                }
                                DataParseResult.Rows.Add(dr);

                                //Insert High Limit
                                string[] HighLimit = sbHighLimit.ToString().Split(',');
                                dr = DataParseResult.NewRow();

                                for (i = 0; i < intColumnLength - 1; i++)
                                {
                                    dr[i] = HighLimit[i];
                                }
                                DataParseResult.Rows.Add(dr);

                                isLimitSet = true;
                            }
                            #endregion Build Datatable

                            #region -- Insert Test Data Result --
                            string[] Data;
                            //Insert site#1 Test Data Result
                            if (((PRRRecord)lineRecord).SITE_NUM == 1)
                            {
                                Data = sbData1.ToString().Split(',');
                            }
                            else //Insert site#2 Test Data Result
                            {
                                Data = sbData2.ToString().Split(',');
                            }

                            dr = DataParseResult.NewRow();

                            int j = intColumnLength;
                            if (Data.Length < j) j = Data.Length;

                            for (i = 0; i < j; i++)
                            {
                                dr[i] = Data[i];
                            }
                            DataParseResult.Rows.Add(dr);
                            #endregion Insert Test Data Result

                            #region -- Next Device --
                            if (((PRRRecord)lineRecord).SITE_NUM == 1)
                                sbData1 = new StringBuilder();
                            else
                                sbData2 = new StringBuilder();

                            intDeviceCounter++;

                            if ((lineRecord = stdf.ReadRecord()).RECORD_TYPE == STDFRecordType.PIR)     //jump to PIR
                            {
                                lineRecord = stdf.ReadRecord();     //jump to PTR
                            }

                            #endregion -- Next Device --
                        }
                    }

                    #endregion --- Device Test Result ---
                }
                catch (Exception ex)
                {
                    throw new Exception("Parse Data Error \r\n" + ex.Message);
                }
            }

            //Timer session
            ts = DateTime.Now - dtStart;
            ParseTime = ts.TotalMilliseconds;
            dtStart = DateTime.Now;

            DataParseResult.PrimaryKey = new DataColumn[] { DataParseResult.Columns[0] };
            return DataParseResult;
            //throw new Exception("ooops");
        }

        ///<summary>
        ///<para>Parse test data from LTX stdf test result in to datatable (multifile)</para>
        ///<seealso cref="DataParse.GetDataFromStd"/>
        ///</summary>
        /// <param name="fileName">Full patch with file name of the stdf file (multifile)</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataFromStd(string[] arrayFileName)
        {

            #region *** Variable define ***
            DataTable DataParseResult = new DataTable();
            DataTable DataParseResultTemp = new DataTable();

            double parseTime = 0;
            double insertTime = 0;

            int deviceCount = 0;

            int testedDevice = 0;
            int passedDevice = 0;
            int failedDevice = 0;

            DataHeader header = new Common.DataHeader();
            Bin bin = new Common.Bin();

            bool isFirstFile = true;
            bool isFirstTable = true;

            DateTime dtLotStart = DateTime.Now;
            DateTime dtLotFinish = DateTime.Now;

            Dictionary<string, int> headerDic = new Dictionary<string, int>();

            #endregion *** Variable define ***

            #region *** Innitial Properties ***
            bin.DeviceCount = new int[8];
            bin.Name = new string[8];
            bin.Number = new int[8];
            bin.Percent = new double[8];

            header.SessionCount = 0;

            #endregion *** Innitial Properties ***

            #region *** Get and merge data table ***
            try
            {
                foreach (string fileName in arrayFileName)
                //Parallel.ForEach(arrayFileName, fileName =>
                {
                    DataParseResultTemp = this.GetDataFromStd(fileName);

                    #region *** Caculate Properties ***
                    header.SessionCount++;

                    parseTime += m_ParseTime;
                    insertTime += m_InsertTime;
                    testedDevice += m_TestedDevice;
                    passedDevice += m_PassedDevice;
                    failedDevice += m_FailedDevice;

                    header.FailQuantity += m_Header.FailQuantity;
                    header.LotQuantity += m_Header.LotQuantity;
                    header.PassQuantity += m_Header.PassQuantity;
                    header.TestQuantity += m_Header.TestQuantity;
                    header.enVision_Version = m_Header.enVision_Version;

                    if (isFirstFile)
                    {
                        header.LotID = m_Header.LotID;
                        header.SubLotID = m_Header.SubLotID;
                        header.OperatorID = m_Header.OperatorID;
                        header.TestBoard = m_Header.TestBoard;
                        header.DeviceName = m_Header.DeviceName;
                        header.Tester = m_Header.Tester;
                        header.Handler = m_Header.Handler;
                        header.TesterType = m_Header.TesterType;
                        header.Product = m_Header.Product;
                        header.ProgramRev = m_Header.ProgramRev;

                        dtLotStart = Convert.ToDateTime(m_Header.LotStartDateTime);
                        dtLotFinish = Convert.ToDateTime(m_Header.LotFinishDateTime);

                        isFirstFile = false;
                    }
                    else
                    {
                        if (m_Header.LotID != "" && m_Header.LotID != null)
                            header.LotID = header.LotID + " & " + m_Header.LotID;

                        if (m_Header.SubLotID != "" && m_Header.SubLotID != null)
                            header.SubLotID = header.SubLotID + " & " + m_Header.SubLotID;

                        if (m_Header.OperatorID != "" && m_Header.OperatorID != null)
                            header.OperatorID = header.OperatorID + " & " + m_Header.OperatorID;

                        if (m_Header.TestBoard != "" && m_Header.TestBoard != null)
                            header.TestBoard = header.TestBoard + " & " + m_Header.TestBoard;

                        if (m_Header.Tester != "" && m_Header.Tester != null)
                            header.Tester = header.Tester + " & " + m_Header.Tester;

                        if (m_Header.TesterType != "" && m_Header.TesterType != null)
                            header.TesterType = header.TesterType + " & " + m_Header.TesterType;

                        if (m_Header.Handler != "" && m_Header.Handler != null)
                            header.Handler = header.Handler + " & " + m_Header.Handler;

                        //Lot Start Datetime
                        if (m_Header.LotStartDateTime < dtLotStart)
                        {
                            dtLotStart = m_Header.LotStartDateTime;
                        }
                        //Lot Finish Datetime
                        if (m_Header.LotFinishDateTime > dtLotFinish)
                        {
                            dtLotFinish = m_Header.LotFinishDateTime;
                        }

                        //Check Product Name
                        if (m_Header.Product != header.Product)
                        {
                            throw new Exception("file " + fileName + " has different Product name with before.");
                        }
                        //Check Program Rev
                        if (m_Header.ProgramRev != header.ProgramRev)
                        {
                            throw new Exception("file " + fileName + " has different Program Rev with before.");
                        }
                    }//end of if firstfile

                    header.Yield = Math.Round(Convert.ToDouble(header.PassQuantity) / Convert.ToDouble(header.TestQuantity) * 100, 3);
                    header.LotStartDateTime = dtLotStart;
                    header.LotFinishDateTime = dtLotFinish;

                    for (int i = 0; i < 8; i++)
                    {
                        bin.DeviceCount[i] += m_Bin.DeviceCount[i];
                        bin.Name[i] = m_Bin.Name[i];
                        bin.Number[i] = m_Bin.Number[i];
                        bin.Percent[i] = Math.Round(Convert.ToDouble(bin.DeviceCount[i]) / Convert.ToDouble(m_TestedDevice) * 100, 3);
                    }
                    #endregion *** Caculate Properties ***

                    #region *** Merge Data Table ***
                    if (isFirstTable)
                    {
                        isFirstTable = false;
                        //Build table structure
                        DataParseResult = DataParseResultTemp.Clone();
                        foreach (DataRow dr in DataParseResultTemp.Rows)
                        {
                            DataParseResult.ImportRow(dr);
                        }
                        //get first table device count
                        deviceCount = DataParseResult.Rows.Count - 4;
                    }
                    else
                    {
                        //merge less columns datatable to more columns datatable
                        if (DataParseResultTemp.Columns.Count > DataParseResult.Columns.Count)
                        {
                            foreach (DataRow dr in DataParseResult.Rows)
                            {
                                if (DataParseResult.Rows.IndexOf(dr) > 3)
                                {
                                    //reset device count
                                    deviceCount++;
                                    dr[0] = deviceCount;
                                    DataParseResultTemp.ImportRow(dr);
                                    //Move Status Column to last
                                    int lastRowIndex = DataParseResultTemp.Rows.Count - 1;
                                    int lastColumnIndex = DataParseResultTemp.Columns.Count - 1;
                                    DataParseResultTemp.Rows[lastRowIndex][lastColumnIndex] = dr[DataParseResult.Columns.Count - 1];
                                    DataParseResultTemp.Rows[lastRowIndex][DataParseResult.Columns.Count - 1] = null;
                                }
                            }
                            DataParseResult = DataParseResultTemp;
                        }
                        else
                        {
                            foreach (DataRow dr in DataParseResultTemp.Rows)
                            {
                                if (DataParseResultTemp.Rows.IndexOf(dr) > 3)
                                {
                                    //reset device count
                                    deviceCount++;
                                    dr[0] = deviceCount;
                                    DataParseResult.ImportRow(dr);
                                    //Move Status Column to last
                                    if (DataParseResultTemp.Columns.Count < DataParseResult.Columns.Count)
                                    {
                                        int lastRowIndex = DataParseResult.Rows.Count - 1;
                                        int lastColumnIndex = DataParseResult.Columns.Count - 1;
                                        DataParseResult.Rows[lastRowIndex][lastColumnIndex] = dr[DataParseResultTemp.Columns.Count - 1];
                                        DataParseResult.Rows[lastRowIndex][DataParseResultTemp.Columns.Count - 1] = null;
                                    }
                                }
                            }//end of foreach (DataRow dr in DataParseResultTemp.Rows)
                        }//end of if (DataParseResultTemp.Columns.Count > DataParseResult.Columns.Count)
                    }// end of if else (isFirstTable)

                    #endregion *** Merge Data Table ***

                }//);//end of foreach (string fileName in arrayFileName)

                #region *** Final Properties ***
                m_Bin = bin;
                m_Header = header;
                m_FailedDevice = failedDevice;
                m_PassedDevice = passedDevice;
                m_TestedDevice = testedDevice;
                m_ParseTime = parseTime;
                m_InsertTime = insertTime;

                #endregion *** Final Properties ***
            }
            catch (Exception ex)
            {
                throw new Exception("Parse data error \n " + ex.Message);
            }

            #endregion *** Parse test data from txt file ***

            DataParseResult.PrimaryKey = new DataColumn[] { DataParseResult.Columns[0] };
            return DataParseResult;
        }

        ///<summary>
        ///<para>Parse test data from LTX std test result in to datatable(use stdf viewer tool)</para>
        ///<seealso cref="DataParse.GetDataFromTxt"/>
        ///</summary>
        /// <param name="fileName">Full patch with file name of the std/stdf file</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataFromStdfviewer(string fileName)
        {
            #region *** Variable define ***
            DateTime dtStart;
            TimeSpan ts;

            DataRow dr;
            DataTable temp_header = new DataTable();
            DataTable temp_data = new DataTable();
            DataTable DataParseResult = new DataTable();
            StringBuilder sbData1 = new StringBuilder();
            StringBuilder sbData2 = new StringBuilder();

            StringBuilder sbColumnName = new StringBuilder("Device#,1,2,3,4");
            StringBuilder sbParameter = new StringBuilder("Test Item,Site No,Soft Bin,Hard Bin,Device ID");
            StringBuilder sbLowLimit = new StringBuilder("Low Limit,,,,");
            StringBuilder sbHighLimit = new StringBuilder("High Limit,,,,");
            StringBuilder sbUnits = new StringBuilder("Units,,,,");

            bool isLimitSet = false;
            int ParameterLength = 0;
            int intColumnLength = 0;
            int intDeviceCounter = 1;
            int i;

            string PassFail1, PassFail2;


            #endregion *** Variable define ***

            #region *** Initialize properties ***
            m_FreezeColumn = 5;
            m_ParseTime = 0;
            m_InsertTime = 0;
            m_TestedDevice = 0;
            m_PassedDevice = 0;
            m_FailedDevice = 0;
            m_Header = new DataHeader();
            m_Bin = new Bin();
            m_Bin.DeviceCount = new int[8];
            m_Bin.Name = new string[8];
            m_Bin.Number = new int[8];
            m_Bin.Percent = new double[8];

            m_Header.SessionCount = 0;

            #endregion *** Initialize properties ***

            //Timer session
            dtStart = DateTime.Now;

            #region // Analysis stdf file
            STDF_Viewer.Record stdf_viewer = new STDF_Viewer.Record();
            stdf_viewer.AnalyzeFile(fileName);
            temp_header = stdf_viewer.GetHeaderStr();
            temp_data = stdf_viewer.GetData();
            #endregion

            #region // Build datatable
            int col_index = 0;
            foreach (DataColumn temp_dc in temp_data.Columns)
            {
                if (col_index == 0)
                    DataParseResult.Columns.Add("Device#", typeof(string));
                else
                {
                    DataParseResult.Columns.Add(col_index.ToString(), typeof(string));

                    if (col_index >= 5)
                        sbParameter.Append("," + temp_dc.ColumnName.ToString());
                }

                col_index++;
            }
            #endregion

            #region // insert test decription
            col_index =0;
            DataRow dr_Parameter = DataParseResult.NewRow();
            string[] temp_array_Parameter = sbParameter.ToString().Split(',');
            foreach (string temp_Parameter in temp_array_Parameter)
            {
                dr_Parameter[col_index] = temp_Parameter;
                col_index++;
            }
            DataParseResult.Rows.Add(dr_Parameter);
            #endregion

            #region // Insert unit and limit
            int row_index =0;
            col_index = 0;
            Dictionary<int, double> dic_multiplier = new Dictionary<int, double>();

            foreach (DataRow temp_dr in temp_data.Rows)
            {
                //initialize the unit nultiplier
                if (row_index == 0)
                {
                    temp_dr[0] = "Unit";
                    for (col_index = 5; col_index < temp_dr.ItemArray.Length; col_index++)
                    {
                        if (temp_dr[col_index].ToString().ToUpper() == "A")
                        {
                            if (temp_data.Columns[col_index].ToString().ToLower().Contains("leakage"))
                            {
                                temp_dr[col_index] = "uA";
                                dic_multiplier.Add(col_index, 1000 * 1000);
                            }
                            else
                            {
                                temp_dr[col_index] = "mA";
                                dic_multiplier.Add(col_index, 1000);
                            }
                        }
                        else if (temp_dr[col_index].ToString().ToUpper() == "S")
                        {
                            temp_dr[col_index] = "ms";
                            dic_multiplier.Add(col_index, 1000);
                        }
                        else if (temp_data.Columns[col_index].ToString().ToLower().Contains("status"))
                        {
                            dic_multiplier.Add(col_index, 5272); // 5272 is keyValue for pass/fail columns
                        }
                    }
                }
                else
                {
                    if (row_index == 1)
                        temp_dr[0] = "Low Limit";
                    else if (row_index == 2)
                        temp_dr[0] = "High Limit";

                    // apply unit multiplier
                    foreach (var item in dic_multiplier)
                    {
                        if (item.Value != 5272) temp_dr[item.Key] = Convert.ToDouble(temp_dr[item.Key]) * item.Value;
                    }
                }

                DataParseResult.Rows.Add (temp_dr.ItemArray);

                row_index++;
                if (row_index > 2) break;
            }
            #endregion

            #region // insert data
            // remove limit and unit which none float rows
            temp_data.Rows.RemoveAt(0);
            temp_data.Rows.RemoveAt(0);
            temp_data.Rows.RemoveAt(0);

            // unit convertion (A -> mA / uA, S -> ms )
            // generate extra columns by using multiplier then copy to new datatable
            DataTable temp_data_mid = new DataTable();
            temp_data_mid = temp_data.Clone();
            for (col_index = 0; col_index < temp_data.Columns.Count; col_index++)
            {
                if (col_index < 5)
                    temp_data_mid.Columns[col_index].DataType = typeof(int);
                else if (col_index == temp_data.Columns.Count - 1)
                    temp_data_mid.Columns[col_index].DataType = typeof(int);
                else
                    temp_data_mid.Columns[col_index].DataType = typeof(float);
            }
            
            foreach (DataRow dr_mid in temp_data.Rows)
            {
                temp_data_mid.Rows.Add(dr_mid.ItemArray);
            }
            for (col_index = 0; col_index < temp_data.Columns.Count; col_index++)
            {
                DataColumn dc_temp_mid = new DataColumn(col_index.ToString(), typeof(string));
                temp_data_mid.Columns.Add(dc_temp_mid);
                // set exrpession
                if (dic_multiplier.ContainsKey(col_index))
                {
                    if (dic_multiplier[col_index] == 5272)
                        dc_temp_mid.Expression = "IIF(" + temp_data_mid.Columns[col_index].ColumnName + "=0, 'Pass', 'Fail')";
                    else
                    {
                        dc_temp_mid.Expression = "[" + temp_data_mid.Columns[col_index].ColumnName + "] * " + dic_multiplier[col_index];
                        //dc_temp_mid.Expression = "Convert([" + temp_data_mid.Columns[col_index].ColumnName + "] * " + dic_multiplier[col_index] + " * 1000, 'System.Int32')/1000";
                    }
                }
                else
                {
                    dc_temp_mid.Expression = "[" + temp_data_mid.Columns[col_index].ColumnName + "] * 1";
                    //dc_temp_mid.Expression = "Convert([" + temp_data_mid.Columns[col_index].ColumnName + "] * 1000, 'System.Int32')/1000";
                }
                
            }

            int start = DataParseResult.Columns.Count;
            int length = start;
            foreach (DataRow dr_temp in temp_data_mid.Rows)
            {
                object[] arr_temp = new object[length];
                Array.ConstrainedCopy(dr_temp.ItemArray, start, arr_temp, 0, length);
                DataParseResult.Rows.Add(arr_temp);
            }

            // set primary key
            DataParseResult.PrimaryKey = new DataColumn[] { DataParseResult.Columns[0] };

            #endregion

            #region // header info
            Dictionary<string, string> dic_header = new Dictionary<string, string>();
            foreach (DataRow dc_header in temp_header.Rows)
            {
                dic_header.Add(dc_header[0].ToString().Trim(), dc_header[1].ToString().Trim());
            }

            string temp = dic_header["MIR.JOB_NAM"];
            this.GetProgramNameRev(temp);

            m_Header.SessionCount++;

            m_Header.enVision_Version = dic_header["MIR.EXEC_VER"];
            m_Header.LotID = dic_header["MIR.LOT_ID"];
            m_Header.Tester = dic_header["MIR.NODE_NAM"];
            m_Header.DeviceName = dic_header["MIR.PART_TYP"];
            m_Header.SubLotID = dic_header["MIR.SBLOT_ID"];
            m_Header.TesterType = dic_header["MIR.TSTR_TYP"];
            m_Header.TestSession = dic_header["MIR.TEST_COD"];
            try
            {
                m_Header.LotStartDateTime = DateTime.Parse(dic_header["MIR.START_T"]);
            }
            catch
            {
                m_Header.LotStartDateTime = DateTime.Parse("1900-1-1 0:00");
            }

            try
            {
                m_Header.LotFinishDateTime = DateTime.Parse(dic_header["MRR.FINISH_T"]);
            }
            catch
            {
                m_Header.LotFinishDateTime = DateTime.Parse("1900-1-1 0:00");
            }

            m_Header.LotDesc = dic_header["MRR.USR_DESC"];

            m_TestedDevice = m_Header.TestQuantity = Convert.ToInt32(dic_header["PCR.PART_CNT"]);
            if (m_TestedDevice == 0) m_TestedDevice = m_Header.TestQuantity = DataParseResult.Rows.Count - 4;


            m_PassedDevice = m_Header.PassQuantity = Convert.ToInt32(dic_header["PCR.GOOD_CNT"]);
            if (m_PassedDevice == 0)
            {
                DataRow[] drs = DataParseResult.Select("[" + (DataParseResult.Columns.Count - 1) + "] Like 'Pass'");
                m_PassedDevice = m_Header.PassQuantity = drs.Count();
            }

            m_FailedDevice = m_Header.FailQuantity = m_Header.TestQuantity - m_Header.PassQuantity;
            //Yield
            double yield = Convert.ToDouble(m_Header.PassQuantity) / Convert.ToDouble(m_Header.TestQuantity) * 100;
            m_Header.Yield = Math.Round(yield, 2);

            #endregion

            //Timer session
            ts = DateTime.Now - dtStart;
            ParseTime = ts.TotalMilliseconds;
            dtStart = DateTime.Now;

            DataParseResult.PrimaryKey = new DataColumn[] { DataParseResult.Columns[0] };
            return DataParseResult;
            //throw new Exception("ooops");
        }

        ///<summary>
        ///<para>Parse test data from LTX stdf test result in to datatable (multifile)</para>
        ///<seealso cref="DataParse.GetDataFromStd"/>
        ///</summary>
        /// <param name="fileName">Full patch with file name of the stdf file (multifile)</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataFromStdfviewer(string[] arrayFileName)
        {

            #region *** Variable define ***
            DataTable DataParseResult = new DataTable();
            DataTable DataParseResultTemp = new DataTable();

            double parseTime = 0;
            double insertTime = 0;

            int deviceCount = 0;

            int testedDevice = 0;
            int passedDevice = 0;
            int failedDevice = 0;

            DataHeader header = new Common.DataHeader();
            Bin bin = new Common.Bin();

            bool isFirstFile = true;
            bool isFirstTable = true;

            DateTime dtLotStart = DateTime.Now;
            DateTime dtLotFinish = DateTime.Now;

            Dictionary<string, int> headerDic = new Dictionary<string, int>();

            #endregion *** Variable define ***

            #region *** Innitial Properties ***
            bin.DeviceCount = new int[8];
            bin.Name = new string[8];
            bin.Number = new int[8];
            bin.Percent = new double[8];

            header.SessionCount = 0;

            #endregion *** Innitial Properties ***

            #region *** Get and merge data table ***
            try
            {
                foreach (string fileName in arrayFileName)
                //Parallel.ForEach(arrayFileName, fileName =>
                {
                    DataParseResultTemp = this.GetDataFromStdfviewer(fileName);

                    #region *** Caculate Properties ***
                    header.SessionCount++;

                    parseTime += m_ParseTime;
                    insertTime += m_InsertTime;
                    testedDevice += m_TestedDevice;
                    passedDevice += m_PassedDevice;
                    failedDevice += m_FailedDevice;

                    header.FailQuantity += m_Header.FailQuantity;
                    header.LotQuantity += m_Header.LotQuantity;
                    header.PassQuantity += m_Header.PassQuantity;
                    header.TestQuantity += m_Header.TestQuantity;
                    header.enVision_Version = m_Header.enVision_Version;

                    if (isFirstFile)
                    {
                        header.LotID = m_Header.LotID;
                        header.SubLotID = m_Header.SubLotID;
                        header.OperatorID = m_Header.OperatorID;
                        header.TestBoard = m_Header.TestBoard;
                        header.DeviceName = m_Header.DeviceName;
                        header.Tester = m_Header.Tester;
                        header.Handler = m_Header.Handler;
                        header.TesterType = m_Header.TesterType;
                        header.Product = m_Header.Product;
                        header.ProgramRev = m_Header.ProgramRev;
                        header.TestSession = m_Header.TestSession;

                        dtLotStart = Convert.ToDateTime(m_Header.LotStartDateTime);
                        dtLotFinish = Convert.ToDateTime(m_Header.LotFinishDateTime);

                        isFirstFile = false;
                    }
                    else
                    {
                        if (m_Header.LotID != "" && m_Header.LotID != null)
                            header.LotID = header.LotID + " & " + m_Header.LotID;

                        if (m_Header.SubLotID != "" && m_Header.SubLotID != null)
                            header.SubLotID = header.SubLotID + " & " + m_Header.SubLotID;

                        if (m_Header.OperatorID != "" && m_Header.OperatorID != null)
                            header.OperatorID = header.OperatorID + " & " + m_Header.OperatorID;

                        if (m_Header.TestBoard != "" && m_Header.TestBoard != null)
                            header.TestBoard = header.TestBoard + " & " + m_Header.TestBoard;

                        if (m_Header.Tester != "" && m_Header.Tester != null)
                            header.Tester = header.Tester + " & " + m_Header.Tester;

                        if (m_Header.TesterType != "" && m_Header.TesterType != null)
                            header.TesterType = header.TesterType + " & " + m_Header.TesterType;

                        if (m_Header.Handler != "" && m_Header.Handler != null)
                            header.Handler = header.Handler + " & " + m_Header.Handler;

                        if (m_Header.TestSession != "" && m_Header.TestSession != null)
                            header.TestSession = header.TestSession + " & " + m_Header.TestSession;

                        //Lot Start Datetime
                        if (m_Header.LotStartDateTime < dtLotStart)
                        {
                            dtLotStart = m_Header.LotStartDateTime;
                        }
                        //Lot Finish Datetime
                        if (m_Header.LotFinishDateTime > dtLotFinish)
                        {
                            dtLotFinish = m_Header.LotFinishDateTime;
                        }

                        //Check Product Name
                        if (m_Header.Product != header.Product)
                        {
                            throw new Exception("file " + fileName + " has different Product name with before.");
                        }
                        //Check Program Rev
                        if (m_Header.ProgramRev != header.ProgramRev)
                        {
                            throw new Exception("file " + fileName + " has different Program Rev with before.");
                        }
                    }//end of if firstfile

                    header.Yield = Math.Round(Convert.ToDouble(header.PassQuantity) / Convert.ToDouble(header.TestQuantity) * 100, 3);
                    header.LotStartDateTime = dtLotStart;
                    header.LotFinishDateTime = dtLotFinish;

                    for (int i = 0; i < 8; i++)
                    {
                        bin.DeviceCount[i] += m_Bin.DeviceCount[i];
                        bin.Name[i] = m_Bin.Name[i];
                        bin.Number[i] = m_Bin.Number[i];
                        bin.Percent[i] = Math.Round(Convert.ToDouble(bin.DeviceCount[i]) / Convert.ToDouble(m_TestedDevice) * 100, 3);
                    }
                    #endregion *** Caculate Properties ***

                    #region *** Merge Data Table ***
                    if (isFirstTable)
                    {
                        isFirstTable = false;
                        //Build table structure
                        DataParseResult = DataParseResultTemp.Clone();
                        foreach (DataRow dr in DataParseResultTemp.Rows)
                        {
                            DataParseResult.ImportRow(dr);
                        }
                        //get first table device count
                        deviceCount = DataParseResult.Rows.Count - 4;
                    }
                    else
                    {
                        //merge less columns datatable to more columns datatable
                        if (DataParseResultTemp.Columns.Count > DataParseResult.Columns.Count)
                        {
                            foreach (DataRow dr in DataParseResult.Rows)
                            {
                                if (DataParseResult.Rows.IndexOf(dr) > 3)
                                {
                                    //reset device count
                                    deviceCount++;
                                    dr[0] = deviceCount;
                                    DataParseResultTemp.ImportRow(dr);
                                    //Move Status Column to last
                                    int lastRowIndex = DataParseResultTemp.Rows.Count - 1;
                                    int lastColumnIndex = DataParseResultTemp.Columns.Count - 1;
                                    DataParseResultTemp.Rows[lastRowIndex][lastColumnIndex] = dr[DataParseResult.Columns.Count - 1];
                                    DataParseResultTemp.Rows[lastRowIndex][DataParseResult.Columns.Count - 1] = null;
                                }
                            }
                            DataParseResult = DataParseResultTemp;
                        }
                        else
                        {
                            foreach (DataRow dr in DataParseResultTemp.Rows)
                            {
                                if (DataParseResultTemp.Rows.IndexOf(dr) > 3)
                                {
                                    //reset device count
                                    deviceCount++;
                                    dr[0] = deviceCount;
                                    DataParseResult.ImportRow(dr);
                                    //Move Status Column to last
                                    if (DataParseResultTemp.Columns.Count < DataParseResult.Columns.Count)
                                    {
                                        int lastRowIndex = DataParseResult.Rows.Count - 1;
                                        int lastColumnIndex = DataParseResult.Columns.Count - 1;
                                        DataParseResult.Rows[lastRowIndex][lastColumnIndex] = dr[DataParseResultTemp.Columns.Count - 1];
                                        DataParseResult.Rows[lastRowIndex][DataParseResultTemp.Columns.Count - 1] = null;
                                    }
                                }
                            }//end of foreach (DataRow dr in DataParseResultTemp.Rows)
                        }//end of if (DataParseResultTemp.Columns.Count > DataParseResult.Columns.Count)
                    }// end of if else (isFirstTable)

                    #endregion *** Merge Data Table ***

                }//);//end of foreach (string fileName in arrayFileName)

                #region *** Final Properties ***
                m_Bin = bin;
                m_Header = header;
                m_FailedDevice = failedDevice;
                m_PassedDevice = passedDevice;
                m_TestedDevice = testedDevice;
                m_ParseTime = parseTime;
                m_InsertTime = insertTime;

                #endregion *** Final Properties ***
            }
            catch (Exception ex)
            {
                throw new Exception("Parse data error \n " + ex.Message);
            }

            #endregion *** Parse test data from txt file ***

            DataParseResult.PrimaryKey = new DataColumn[] { DataParseResult.Columns[0] };
            return DataParseResult;
        }

        ///<summary>
        ///<para>Parse test data from csv file(Acetech format) in to datatable</para>
        ///<seealso cref="DataParse.GetDataFromCsv"/>
        ///</summary>
        /// <param name="fileName">Full patch with file name of the csv file</param>
        /// <returns>DataTable</returns>
        public DataTable[] GetDataFromAceTechCsv(string fileName)
        {
            #region *** Variable define ***
            DateTime dtStart;
            TimeSpan ts;

            DataRow dr;
            DataTable[] tblParseResult = new DataTable[6];

            for (int i = 0; i < 6; i++)
            {
                tblParseResult[i] = new DataTable();
            }

            //  0   -- KGU 
            //  1   -- FT 
            //  2   -- RT1 
            //  3   -- RT2 
            //  4   -- EQC 
            //  5   -- EQCV 

            #endregion *** Variable define ***

            #region *** Initialize properties ***
            m_FreezeColumn = 5;
            m_ParseTime = 0;
            m_InsertTime = 0;
            m_TestedDevice = 0;
            m_PassedDevice = 0;
            m_FailedDevice = 0;

            m_Bin = new Bin();
            m_Bin.DeviceCount = new int[8];
            m_Bin.Name = new string[8];
            m_Bin.Number = new int[8];
            m_Bin.Percent = new double[8];

            m_Header = new DataHeader();
            a_Header = new AceTechDataHeader();
            a_Header.KGU = false;
            a_Header.FT = false;
            a_Header.RT1 = false;
            a_Header.RT2 = false;
            a_Header.EQC = false;
            a_Header.EQCV = false;

            #endregion *** Initialize properties ***

            #region *** Parsing csv file data into datatable ***
            // *** Timer session ***
            dtStart = DateTime.Now;

            try
            {
                using (CsvReader csv = new CsvReader(new StreamReader(fileName), false))
                {
                    #region *** Variable declare ***

                    int rowsIndex = 0;
                    int fieldcount = csv.FieldCount - 4;
                    //string[] columnName = csv.GetFieldHeaders();
                    bool isFreezeColumnSet = false;
                    bool isStatusExist = false;
                    bool isStatusAdded = false;
                    bool isHeaderFinish = false;
                    bool isColumnsGenerate = false;

                    #endregion *** Variable declare ***

                    #region *** Parse header information ***
                    //Parse header
                    csv.ReadNextRecord();
                    if (csv[0].Contains("Header Section"))
                    {
                        //csv.ReadNextRecord();
                        while (!isHeaderFinish)
                        {
                            csv.ReadNextRecord();
                            string temp = csv[0];
                            if (csv[0].Contains("Data Section"))
                            {
                                isHeaderFinish = true;
                                temp = csv[0];
                            }
                            _FieldInfo FI = a_Header.GetType().GetField(csv[0]);
                            if (FI != null)
                            {
                                try
                                {
                                    Type type = FI.FieldType;
                                    FI.SetValueDirect(__makeref(a_Header), TypeDescriptor.GetConverter(type).ConvertFrom(csv[1]));
                                    //csv.ReadNextRecord();
                                }
                                catch (Exception ex)
                                {
                                    Type type = FI.FieldType;
                                    if (type.Name == "Double")
                                    {
                                        FI.SetValueDirect(__makeref(a_Header), 0.0);
                                    }
                                    else if (type.Name == "DateTime")
                                    {
                                        string strTemp = csv[1];
                                        string[] chrTemp = strTemp.Split('-');
                                        strTemp = chrTemp[0] + "/" + chrTemp[1] + "/" + chrTemp[2] + " " + chrTemp[3] + ":" + chrTemp[4] + ":" + chrTemp[5];

                                        FI.SetValueDirect(__makeref(a_Header), TypeDescriptor.GetConverter(type).ConvertFrom(strTemp));
                                    }
                                    //csv.ReadNextRecord(); 
                                }
                            }
                        }
                    }
                    #endregion *** Parse header information ***

                    #region *** Parsing data ***

                    while (csv.ReadNextRecord())
                    {
                        #region *** Build datatable column ***
                        //generate columns
                        if (!isColumnsGenerate)
                        {
                            for (int i = 0; i < fieldcount; i++)
                            {
                                for (int j = 0; j < 6; j++)
                                {
                                    if (i == 0)
                                        tblParseResult[j].Columns.Add("Device#", typeof(string));
                                    else
                                        tblParseResult[j].Columns.Add(i.ToString(), typeof(string));
                                }
                            }

                            csv.ReadNextRecord();
                            isColumnsGenerate = true;
                        }
                        #endregion *** Build datatable column ***

                        #region *** Check freeze column ***

                        if (!isFreezeColumnSet)
                        {
                            //if (csv[2] == "Device ID")
                                m_FreezeColumn = 3;
                            //else
                            //    m_FreezeColumn = 5;

                            isFreezeColumnSet = true;
                        }
                        #endregion *** Check freeze column ***

                        //*** Build Limit ***
                        try
                        {
                            #region --- Build Limit ---
                            if (csv[5].ToString() == "Unit")
                            {
                                // Units
                                try
                                {
                                    dr = tblParseResult[0].NewRow();
                                    for (int i = 0; i < fieldcount; i++)
                                    {
                                        if (i == 0)
                                        {
                                            dr[0] = csv[5];
                                            dr[1] = "";
                                            dr[2] = "";
                                        }
                                        else if (i >= 3)
                                        {
                                            dr[i] = csv[i + 3];
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message);
                                }
                                tblParseResult[0].Rows.Add(dr);
                                csv.ReadNextRecord();

                                // UL
                                dr = tblParseResult[0].NewRow();
                                for (int i = 0; i < fieldcount; i++)
                                {
                                    if (i == 0)
                                    {
                                        dr[0] = csv[5];
                                        dr[1] = "";
                                        dr[2] = "";
                                    }
                                    else if (i >= 3)
                                    {
                                        dr[i] = csv[i + 3];
                                    }
                                }
                                tblParseResult[0].Rows.Add(dr);
                                csv.ReadNextRecord();

                                // LL
                                dr = tblParseResult[0].NewRow();
                                for (int i = 0; i < fieldcount; i++)
                                {
                                    if (i == 0)
                                    {
                                        dr[0] = csv[5];
                                        dr[1] = "";
                                        dr[2] = "";
                                    }
                                    else if (i >= 3)
                                    {
                                        dr[i] = csv[i + 3];
                                    }
                                }
                                tblParseResult[0].Rows.InsertAt(dr, 1);
                                csv.ReadNextRecord();

                                // Parameter
                                dr = tblParseResult[0].NewRow();
                                for (int i = 0; i < fieldcount; i++)
                                {
                                    if (i == 0)
                                    {
                                        dr[0] = "TestItem";
                                        dr[1] = "SiteNo";
                                        dr[2] = "Device ID";
                                    }
                                    else if (i >= 3)
                                    {
                                        dr[i] = csv[i + 3];
                                    }
                                }
                                dr[fieldcount - 1] = "Status";
                                tblParseResult[0].Rows.InsertAt(dr, 0);
                                csv.ReadNextRecord();

                                for (int i = 1; i < 6; i++)
                                {
                                    for (int j = 0; j <= 3; j++)
                                    {
                                        tblParseResult[i].ImportRow(tblParseResult[0].Rows[j]);
                                    }
                                }
                            }
                            #endregion --- Build Limit ---
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        //*** Import data ***
                        #region --- KGU Data ---
                        if (csv[1].ToString() == "GUV")
                        {
                            dr = tblParseResult[0].NewRow();
                            for (int i = 0; i < fieldcount; i++)
                            {
                                if (i == 0)
                                {
                                    dr[0] = csv[0];
                                    dr[1] = csv[4];
                                    dr[2] = csv[2];
                                }
                                else if (i >= 3)
                                {
                                    if (csv[i] == "")
                                        dr[i] = DBNull.Value;
                                    else
                                    {
                                        dr[i] = csv[i + 3];
                                    }
                                }
                            }
                            //KGU Number
                            a_Header.KGU_Number = csv[2].ToString();                           
                            //a_Header.KGU_Number = a_Header.KGU_Number + "," + csv[2].ToString();

                            tblParseResult[0].Rows.Add(dr);
                            a_Header.KGU = true;
                        }
                        #endregion --- KGU Data ---

                        #region --- FT Data ---
                        else if (csv[1].ToString() == "FT" && csv[3].ToString() == "NEQC")
                        {
                            dr = tblParseResult[1].NewRow();
                            for (int i = 0; i < fieldcount ; i++)
                            {
                                if (i == 0)
                                {
                                    dr[0] = csv[0];
                                    dr[1] = csv[4];
                                    dr[2] = csv[2];
                                }
                                else if (i >= 3)
                                {
                                    if (csv[i] == "")
                                        dr[i] = DBNull.Value;
                                    else
                                    {
                                        dr[i] = csv[i + 3];
                                    }
                                }
                            }
                            tblParseResult[1].Rows.Add(dr);
                            a_Header.FT = true;
                        }
                        #endregion --- FT Data ---

                        #region --- RT1 Data ---
                        else if (csv[1].ToString() == "RT1")
                        {
                            dr = tblParseResult[2].NewRow();
                            for (int i = 0; i < fieldcount; i++)
                            {
                                if (i == 0)
                                {
                                    dr[0] = csv[0];
                                    dr[1] = csv[4];
                                    dr[2] = csv[2];
                                }
                                else if (i >= 3)
                                {
                                    if (csv[i] == "")
                                        dr[i] = DBNull.Value;
                                    else
                                    {
                                        dr[i] = csv[i + 3];
                                    }
                                }
                            }
                            tblParseResult[2].Rows.Add(dr);
                            a_Header.RT1 = true;
                        }
                        #endregion --- RT1 Data ---

                        #region --- RT2 Data ---
                        else if (csv[1].ToString() == "RT2")
                        {
                            dr = tblParseResult[3].NewRow();
                            for (int i = 0; i < fieldcount; i++)
                            {
                                if (i == 0)
                                {
                                    dr[0] = csv[0];
                                    dr[1] = csv[4];
                                    dr[2] = csv[2];
                                }
                                else if (i >= 3)
                                {
                                    if (csv[i] == "")
                                        dr[i] = DBNull.Value;
                                    else
                                    {
                                        dr[i] = csv[i + 3];
                                    }
                                }
                            }
                            tblParseResult[3].Rows.Add(dr);
                            a_Header.RT2 = true;
                        }
                        #endregion --- RT2 Data ---

                        #region --- FT EQC Data ---
                        else if (csv[1].ToString() == "FT" && csv[3].ToString() == "EQC")
                        {
                            dr = tblParseResult[4].NewRow();
                            for (int i = 0; i < fieldcount; i++)
                            {
                                if (i == 0)
                                {
                                    dr[0] = csv[0];
                                    dr[1] = csv[4];
                                    dr[2] = csv[2];
                                }
                                else if (i >= 3)
                                {
                                    if (csv[i] == "")
                                        dr[i] = DBNull.Value;
                                    else
                                    {
                                        dr[i] = csv[i + 3];
                                    }
                                }
                            }
                            tblParseResult[4].Rows.Add(dr);
                            a_Header.EQC = true;
                        }
                        #endregion --- FT EQC Data ---

                        #region --- EQCV Data ---
                        else if (csv[1].ToString() == "EQCV")
                        {
                            dr = tblParseResult[5].NewRow();
                            for (int i = 0; i < fieldcount; i++)
                            {
                                if (i == 0)
                                {
                                    dr[0] = csv[0];
                                    dr[1] = csv[4];
                                    dr[2] = csv[2];
                                }
                                else if (i >= 3)
                                {
                                    if (csv[i] == "")
                                        dr[i] = DBNull.Value;
                                    else
                                    {
                                        dr[i] = csv[i + 3];
                                    }
                                }
                            }
                            tblParseResult[5].Rows.Add(dr);
                            a_Header.EQCV = true;
                        }
                        #endregion --- EQCV Data ---

                    }
                    #endregion *** parsing data ***

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            //*** Timer session ***
            ts = DateTime.Now - dtStart;
            m_ParseTime = ts.TotalMilliseconds;

            #endregion *** Parsing csv file data into datatable ***

            for (int i = 1; i < 6; i++)
            {
                tblParseResult[1].PrimaryKey = new DataColumn[] { tblParseResult[1].Columns[0] };
            }
            return tblParseResult;
        }//end of GetDataFromCsv

        ///<summary>
        ///<para>Parse test data from csv file(Acetech format) in to datatable (multifile)</para>
        ///<seealso cref="DataParse.GetDataFromCsv"/>
        ///</summary>
        /// <param name="fileName">Full patch with file name array of the csv file</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataFromAceTechCsv(string[] arrayFileName)
        {
            throw new Exception("Not implement");
        }


        ///<summary>
        ///<para>Parse test data from formatted csv file in to datatable</para>
        ///<seealso cref="DataParse.GetDataFromCsv"/>
        ///</summary>
        /// <param name="fileName">Full patch with file name of the csv file</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataFromCsv(string fileName)
        {
            #region *** Variable define ***
            DateTime dtStart;
            TimeSpan ts;

            DataTable tblParseResult = new DataTable();
            DataRow dr;
            StringBuilder sbDataString = new StringBuilder();

            #endregion *** Variable define ***

            #region *** Initialize properties ***
            m_FreezeColumn = 5;
            m_ParseTime = 0;
            m_InsertTime = 0;
            m_TestedDevice = 0;
            m_PassedDevice = 0;
            m_FailedDevice = 0;
            m_Header = new DataHeader();
            m_Bin = new Bin();
            m_Bin.DeviceCount = new int[8];
            m_Bin.Name = new string[8];
            m_Bin.Number = new int[8];
            m_Bin.Percent = new double[8];

            #endregion *** Initialize properties ***

            #region *** Parsing csv file data into datatable ***
            // *** Timer session ***
            dtStart = DateTime.Now;

            try
            {
                using (CsvReader csv = new CsvReader(new StreamReader(fileName), false))
                {
                    #region *** Variable declare ***

                    int rowsIndex = 0;
                    int fieldcount = csv.FieldCount;
                    //string[] columnName = csv.GetFieldHeaders();
                    bool isFreezeColumnSet = false;
                    bool isStatusExist = false;
                    bool isStatusAdded = false;
                    bool isHeaderFinish = false;
                    bool isColumnsGenerate = false;

                    #endregion *** Variable declare ***

                    #region *** Parse header information ***
                    //Parse header
                    csv.ReadNextRecord();
                    if (csv[0].Contains("Header Section"))
                    {
                        //csv.ReadNextRecord();
                        while (!isHeaderFinish )
                        {
                            csv.ReadNextRecord();
                            string temp = csv[0];
                            if (csv[0].Contains("Data Section"))
                            {
                                isHeaderFinish = true;
                               temp = csv[0];
                            }
                            _FieldInfo FI = m_Header.GetType().GetField(csv[0]);
                            if (FI != null)
                            {
                                Type type = FI.FieldType;
                                FI.SetValueDirect(__makeref(m_Header), TypeDescriptor.GetConverter(type).ConvertFrom(csv[1]));
                                //csv.ReadNextRecord();
                            }
                        }
                    }
                    #endregion *** Parse header information ***
                    
                    #region *** Parsing data ***

                    while (csv.ReadNextRecord())
                    {
                        bool isDevicePass = true;
                        dr = tblParseResult.NewRow();

                        #region *** Build datatable column ***
                        //generate columns
                        if (!isColumnsGenerate)
                        {
                            for (int i = 0; i < fieldcount; i++)
                            {
                                tblParseResult.Columns.Add(csv[i], typeof(string));
                            }

                            csv.ReadNextRecord();
                            isColumnsGenerate = true;
                        }
                        #endregion *** Build datatable column ***

                        #region *** Check freeze column ***

                        if (!isFreezeColumnSet)
                        {
                            if (csv[2] == "Device ID")
                                m_FreezeColumn = 3;
                            else
                                m_FreezeColumn = 5;

                            isFreezeColumnSet = true;
                        }
                        #endregion *** Check freeze column ***

                        #region *** Check if Status column exist ***

                        if (!isStatusExist && !isStatusAdded)
                        {
                            if (csv[fieldcount - 1] != "Status")
                            {
                                tblParseResult.Columns.Add(fieldcount.ToString(), typeof(string));
                                dr[fieldcount] = "Status";
                                isStatusAdded = true;
                            }
                            else
                                isStatusExist = true;
                        }
                        #endregion *** Check if Status column exist ***

                        //*** Import data ***
                        for (int i = 0; i < fieldcount; i++)
                        {
                            if (csv[i] == "")
                                dr[i] = DBNull.Value;
                            else
                            {
                                dr[i] = csv[i];

                                #region *** Update status ***

                                if (!isStatusExist && rowsIndex > 3 && i > 2)
                                {
                                    if (isDevicePass)
                                    {
                                        try
                                        {

                                            double dblResult = Convert.ToDouble(dr[i]);
                                            double LSL = Convert.ToDouble(tblParseResult.Rows[2][i]);
                                            double USL = Convert.ToDouble(tblParseResult.Rows[3][i]);
                                            if (dblResult < LSL                  //if data < LowLimit
                                                     || dblResult > USL)        //if data > HighLimit
                                            {
                                                dr[fieldcount] = "Fail";
                                                isDevicePass = false;
                                                m_FailedDevice++;
                                            }
                                            else if (i == fieldcount - 1)
                                            {
                                                dr[fieldcount] = "Pass";
                                                m_PassedDevice++;
                                            }
                                        }
                                        catch 
                                        {
                                            dr[fieldcount] = "Fail";
                                        }
                                    }
                                }
                                #endregion *** Update status ***
                            }
                        } 
                        tblParseResult.Rows.Add(dr);
                        rowsIndex++;
                    }
                    #endregion *** parsing data ***
                   
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            //*** Timer session ***
            ts = DateTime.Now - dtStart;
            m_ParseTime = ts.TotalMilliseconds;

            m_TestedDevice = m_PassedDevice + m_FailedDevice;
            if (m_TestedDevice == 0)
            {
                m_TestedDevice = m_Header.TestQuantity;
                m_PassedDevice = m_Header.PassQuantity;
                m_FailedDevice = m_Header.FailQuantity;
            }

            #endregion *** Parsing csv file data into datatable ***

            tblParseResult.PrimaryKey = new DataColumn[] { tblParseResult.Columns[0] };
            return tblParseResult;
        }//end of GetDataFromCsv



        public int GetSiteNumber(string fileName)
        {
            int SiteNumber = 0;
            string lineConent = null;
            string[] strOriginalData;
            string[] strTrimedData;

            try
            {
                #region *** Read to memory ***
                //string content = string.Empty;
                //using (StreamReader ms_StreamReader = new StreamReader(fileName))
                //{
                //    content = ms_StreamReader.ReadToEnd();//一次性读入内存
                //}
                //MemoryStream _MemoryStream = new MemoryStream(Encoding.GetEncoding("GB2312").GetBytes(content));//放入内存流，以便逐行读取
                //StreamReader _StreamReader = new StreamReader(_MemoryStream);

                //ts = DateTime.Now - dtStart;
                //ParseTime = ts.TotalMilliseconds;
                #endregion *** Read to memory ***

                #region *** Diresct Read ***
                FileStream _FileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                StreamReader _StreamReader = new StreamReader(_FileStream);
                _StreamReader.BaseStream.Seek(0, SeekOrigin.Begin);

                #endregion *** Diresct Read ***

                #region *** Get Site Number ***
                lineConent = _StreamReader.ReadLine();
                while (lineConent != null)
                {
                    if (lineConent.Contains("Test Description"))
                    {
                        //lineConent = _StreamReader.ReadLine();
                        //lineConent = _StreamReader.ReadLine();
                        //lineConent = _StreamReader.ReadLine();
                        //lineConent = _StreamReader.ReadLine();
                        if (lineConent.Contains("Site 1") &&  lineConent.Contains("Site 2"))
                        {
                            SiteNumber = 2;
                            break;
                        }
                        else
                        {
                            SiteNumber = 1;
                            break;
                        }
                    }
                    lineConent = _StreamReader.ReadLine();
                }

                #endregion *** Get Site Number ***
            }
            catch (Exception ex)
            {
                throw new Exception("Get Site Number error \n " + ex.Message);
            }
            return SiteNumber;
        }


        #endregion *** Method ***

        #region *** Sub Function ***
        private void GetProgramNameRev(string s)
        {
            string[] tempArray = s.Split('_');
            m_Header.Product = tempArray[0].Trim();
            try
            {
                m_Header.ProgramRev = tempArray[1].Trim() + "." + tempArray[2].Trim() + "." + tempArray[3].Trim();
            }
            catch
            {
                try
                {
                    m_Header.ProgramRev = tempArray[1].Trim() + "." + tempArray[2].Trim();
                }
                catch
                {
                    m_Header.ProgramRev = tempArray[1].Trim();
                }
            }
        }

        private string ParameterParse(string s)
        {
            return s.Substring(0, s.Length - 1);
        }

        private double ResultScale(float f, int scale)
        {
            if (double.IsNaN(f))
                return -99;
            else
                return Math.Round(f * Math.Pow(10, scale), 3);
        }

        private string UnitScale(string s, int scale)
        {
            string unit = s;
            switch (scale)
            {
                case 15:
                    {
                        unit = "f" + s;
                        break;
                    }
                case 12:
                    {
                        unit = "p" + s;
                        break;
                    }
                case 9:
                    {
                        unit = "n" + s;
                        break;
                    }
                case 6:
                    {
                        unit = "u" + s;
                        break;
                    }
                case 3:
                    {
                        unit = "m" + s;
                        break;
                    }
                case 2:
                    {
                        unit = "%" + s;
                        break;
                    }
                case -3:
                    {
                        unit = "K" + s;
                        break;
                    }
                case -6:
                    {
                        unit = "M" + s;
                        break;
                    }
                case -9:
                    {
                        unit = "G" + s;
                        break;
                    }
                case -12:
                    {
                        unit = "T" + s;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return unit;

        }
        #endregion *** Sub Function ***

    }//end of class Vanchip.Data.DataParse



}//end of namespace Vanchip.Data
