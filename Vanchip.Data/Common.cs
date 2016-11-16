using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;

namespace Vanchip.Common
{

    #region *** Struct Define ***
    //Last Saved
    public struct LastSaved
    {
        public int filetype;
        public string filepathopen;
        public string filepathsave;
    }

    //RF test cable loss
    public struct LossComp
    {
        public double[] RFIN;
        public double[] RFOUT;
        public double[] RFCoupleOut;
        public double[] RXOUT;
        public double[] LBHarmonic;
        public double[] HBHarmonic;
    }

    //PA test data failure mode
    public struct FailureMode
    {
        public string Parameter;
        public string[] Device;
        public int Count;
    }

    //PA test data header information
    public struct DataHeader
    {
        public string Product;
        public string ProgramRev;
        public string DeviceName;
        public string LotID;
        public string SubLotID;
        public string TestSession;
        public string Tester;
        public string TesterType;
        public string enVision_Version;
        
        public DateTime LotStartDateTime;
        public DateTime LotFinishDateTime;

        public int SessionCount;
        public int LotQuantity;
        public int TestQuantity;
        public int PassQuantity;
        public int FailQuantity;

        public double Yield;

        public string TestBoard;
        public string Handler;
        public string OperatorID;
        public string LotDesc;

    }

    //AceTech PA test data header information
    public struct AceTechDataHeader
    {
        public string Product;
        public string ProgramRev;
        public string DeviceName;
        public string LotNumber;
        public string SubLotNumber;
        public string TestSession;
        public string Tester;
        //public string TesterType;
        //public string enVision_Version;

        public DateTime LotStartDateTime;
        public DateTime LotEndDateTime;

        public int SessionCount;
        public int LotQty;

        public int TestedQty_FT;
        public int PassedQty_FT;
        //public int FailedQty_FT;
        public double Yield_FT;

        public int TestedQty_RT1;
        public int PassedQty_RT1;
        //public int FailedQty_RT1;
        public double Yield_RT1;

        public int TestedQty_RT2;
        public int PassedQty_RT2;
        //public int FailedQty_RT2;
        public double Yield_RT2;

        public int TestedQty_EQC;
        public int PassedQty_EQC;
        //public int FailedQty_EQC;
        public double Yield_EQC;

        public int TestedQty_EQCV;
        public int PassedQty_EQCV;
        //public int FailedQty_EQCV;
        public double Yield_EQCV;

        public string TestBoard;
        public string Handler;
        public string OperatorID;
        public string LotDesc;

        public bool KGU;
        public bool FT;
        public bool RT1;
        public bool RT2;
        public bool EQC;
        public bool EQCV;

        public string KGU_Number;

    }

    //PA test data bin information
    public struct Bin
    {
        public int[] Number;
        public int[] DeviceCount;
        public double[] Percent;
        public string[] Name;

    }

    //Cpk
    public struct ParameterCpk
    {
        public double Average;
        public double MIN;
        public double MAX;
        public double LSL;
        public double USL;
        public double Stdev;
        public double Stdevp;
        public double U;
        public double T;
        public double Cp;
        public double Ca;
        public double CpkU;
        public double CpkL;
        public double Value;
        public int Count;
    }

    //Distribution Parameter
    public struct DistPara
    {
        public double[] LowLimit;
        public double[] HighLimit;
        public double[] AxisMax;
        public double[] AxisMin;
        public double[] AxisIncr;
        public double[] AxisTicks;
    }

    // Product config information
    public struct Product
    {
        public int[]    Item;
        public string[] TestItem;
        public string[] Description;
        public string[] Units;
        public string[] LowLimit;
        public string[] HighLimit;
        public string[] VCC;
        public string[] Vramp;
        public string[] Txen;
        public string[] Gpctrl0;
        public string[] Gpctrl1;
        public string[] Gpctrl2;
        public string[] Pin;
        public string[] Pout;
        public string[] Freq;
    }

    public struct ProductTest
    {
        public int Item;
        public string TestItem;
        public string Description;
        public string Units;
        public double LowLimit;
        public double HighLimit;
        public double VCC;
        public double Vramp;
        public double Txen;
        public double Gpctrl0;
        public double Gpctrl1;
        public double Gpctrl2;
        public double Pin;
        public double Pout;
        public double FreqIn;
        public double FreqOut;
        public double LossIn;
        public double LossOut;
        public double SocketOffset;
    }

    #endregion *** Struct Define ***
    

    #region *** Enum Define ***

    public enum AnalysisType
    {
        FailureMode = 0,
        FailureRate = 1
    }
    #endregion *** Enum Define ***


    /// <summary>
    /// Util class contains all structs and misc functions used in vanchip
    /// </summary>
    public class Util
    {
        public const string MasterDB = @"\\192.168.21.251\TestData\_DataBase\data.mdb";
        public const string SlaveDB = @".\GoldenSample\data.mdb";
        public const string Pin = ";User ID='admin';Password='';Jet OleDb:DataBase Password='vc7810Eu_A6_01'";


        public const string PathDataParsingService = @"\\192.168.21.251\TestData\DataParsingService";
        public const string MysqlVanchip = @"server=192.168.21.52;userid=webuser;password=Vanchip301;database=testdata";
        public const string MysqlAce = @"server=45.76.104.155;userid=webuser;password=Vanchip301;database=testdata";

        #region *** Misc. Functions ***
        /// <summary>
        /// Use to hold program for a while
        /// </summary>
        /// <param name="intValue_in_MiniSecond">Value in miniSecond for delay</param>
        public void Wait(int intValue_in_MiniSecond)
        {

            Thread.Sleep(intValue_in_MiniSecond);
        }
 
        /// <summary>
        ///<para>描述：快速排序算法</para>   
        ///<para>类名：QuickSort</para> 
        ///<para>作者：洪晓军</para> 
        ///<para>时间：2004-11-2 </para>
        /// </summary>
        /// <param name="a">数组</param>
        /// <param name="left">数组a第一个元素位置</param>
        /// <param name="right">数组a最后一个元素位置</param>
        public void quickSort(int[] a, int left, int right)
        {
            int i, j, pivot, temp;     //pivot为支点 
            if (left >= right)
                return;
            i = left;
            j = right + 1;
            pivot = a[left];
            while (true)
            {
                do
                {
                    i++;
                } while (a[i] < pivot && i < right);     //从左向右寻找大于支点元素  
                do
                {
                    j--;
                } while (a[j] > pivot && j > 0);     //从右向左寻找小于支点元素 
                if (i >= j)
                    break;
                else     //满足i<j则交换i和j位置元素值 
                {
                    temp = a[i];
                    a[i] = a[j];
                    a[j] = temp;
                }
            }
            a[left] = a[j];
            a[j] = pivot;     //以a[j]为新支点，j位置左边元素值均小于a[j],j位置右边元素值均大于a[j] 
            quickSort(a, left, j - 1);      //递归排序 
            quickSort(a, j + 1, right);
        }

        /// <summary>
        /// Get a FileInfo array based on the criteria
        /// </summary>
        /// <param name="DirectoryPath">The full directory path</param>
        /// /// <param name="Extension">File extension </param>
        public FileInfo[] GetFileInfoArray(string DirectoryPath, string Extension)
        {
            string searchPattern = "*." + Extension;
            DirectoryInfo DI = new DirectoryInfo(DirectoryPath);
            FileInfo[] FI = DI.GetFiles(searchPattern);
    

            return FI;
        }

        /// <summary>
        /// Use to file based on the criteria
        /// </summary>
        /// <param name="FI">FileInfo array</param>
        public int DeleteFiles(FileInfo[] FI)
        {
            int FileCount = 0;
            try
            {
                foreach (FileInfo tmpFI in FI)
                {
                    File.SetAttributes(tmpFI.FullName, FileAttributes.Normal);
                    File.Delete(tmpFI.FullName);
                    FileCount++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            return FileCount;
        }

        /// <summary>
        /// Use to file based on the criteria
        /// </summary>
        /// <param name="FI">FileInfo array</param>
        public bool IsNumeral( object objValue )
        {
            double doubleValue;
            bool isnumeral;

            try
            {
                doubleValue = Convert.ToDouble(objValue);
                isnumeral = true;
            }
            catch 
            {
                doubleValue = -99.99;
                isnumeral = false;
            }
            return isnumeral;
        }



        #endregion *** Misc. Functions ***
    }

    #region *** Loss Compensation Stuff Class ***

    public class RFIN_Loss
    {
        public const int Freq_915MHz = 0;
        public const int Freq_1785MHz = 1;
        public const int Freq_925MHz = 2;
        public const int Freq_1805MHz = 3;
        public const int Freq_915MHz_2Fo = 4;
        public const int Freq_915MHz_3Fo = 5;
        public const int Freq_915MHz_4Fo = 6;
        public const int Freq_915MHz_5Fo = 7;
        public const int Freq_915MHz_6Fo = 8;
        public const int Freq_1785MHz_2Fo = 9;
        public const int Freq_1785MHz_3Fo = 10;
        public const int Freq_1785MHz_4Fo = 11;
        public const int Freq_1785MHz_5Fo = 12;
        public const int Freq_1785MHz_6Fo = 13;
    }

    public class RFOUT_Loss
    {
        public const int Freq_915MHz = 0;
        public const int Freq_1785MHz = 1;
        public const int Freq_925MHz = 2;
        public const int Freq_1805MHz = 3;
    }

    public class RxOUT_Loss
    {
        public const int Freq_915MHz = 0;
        public const int Freq_1785MHz = 1;
        public const int Freq_925MHz = 2;
        public const int Freq_1805MHz = 3;
    }

    public class RFCouplerOUT_Loss
    {
        public const int Freq_915MHz = 0;
        public const int Freq_1785MHz = 1;
        public const int Freq_925MHz = 2;
        public const int Freq_1805MHz = 3;
    }

    public class RFLBHar_Loss
    {
        public const int Freq_915MHz_2Fo = 0;
        public const int Freq_915MHz_3Fo = 1;
        public const int Freq_915MHz_4Fo = 2;
        public const int Freq_915MHz_5Fo = 3;
        public const int Freq_915MHz_6Fo = 4;
    }

    public class RFHBHar_Loss
    {
        public const int Freq_1785MHz_2Fo = 0;
        public const int Freq_1785MHz_3Fo = 1;
        //public const int Freq_1785MHz_4Fo = 2;
        //public const int Freq_1785MHz_5Fo = 3;
        //public const int Freq_1785MHz_6Fo = 4;
    }

    public static class Freqs
    {

        public static double F915 = 915;
        public static double F1785 = 1785;

        public static double F915_2Fo = 2 * F915;
        public static double F915_3Fo = 3 * F915;
        public static double F915_4Fo = 4 * F915;
        public static double F915_5Fo = 5 * F915;
        public static double F915_6Fo = 6 * F915;

        public static double F1785_2Fo = 2 * F1785;
        public static double F1785_3Fo = 3 * F1785;
        public static double F1785_4Fo = 4 * F1785;
        public static double F1785_5Fo = 5 * F1785;
        public static double F1785_6Fo = 6 * F1785;

        //RX Band Freqs
        public static double F925 = 925;
        public static double F1805 = 1805;

    }

    public struct Limit
    {
        public double[] RFIN_LL;
        public double[] RFIN_UL;
        public double[] RFOUT_LL;
        public double[] RFOUT_UL;
        public double[] RFCouplerOUT_LL;
        public double[] RFCouplerOUT_UL;
        public double[] RxOUT_LL;
        public double[] RxOUT_UL;
        public double[] RFLBHar_LL;
        public double[] RFLBHar_UL;
        public double[] RFHBHar_LL;
        public double[] RFHBHar_UL;
    }

    #endregion *** Loss Compensation Stuff Class ***
}

