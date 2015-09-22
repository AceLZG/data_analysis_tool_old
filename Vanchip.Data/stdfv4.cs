using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.tragicphantom.stdf;
using com.tragicphantom.stdf.util;
using com.tragicphantom.stdf.v4;



namespace Vanchip.Data
{
    public class stdfv4
    {
        string strName = @"D:\TestDataTemp\VC7594_P1_0_4_TPA01_KGU_101.106_TPA01_0740_12022102_01__20131114074949.std";
        public void stdf_parse()
        {
            STDFReader stdr = new STDFReader(strName);

            STDFContainer stdc = new STDFContainer(stdr);
            STDFWriter stdw = new STDFWriter(strName);


            
        }
    }
}
