using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDF_Viewer
{
    public class MRR : STDFBase
    {
        public void Initial(byte[] result)
        {
            int index = 0; string charNStr = null;
            float R4 = 0;
            int U4 = 0;
            index = ReadU4(out U4, result, index); this.FINISH_T = CalLocalTimeStr(U4);
            string s = Encoding.ASCII.GetString(new byte[] { result[index + 1] });
            this.DISP_COD = s;
            index = ReadCharN(out charNStr, result, index + 2); this.USR_DESC = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.EXC_DESC = charNStr;
        }

        public string FINISH_T { get; private set; }
        public string DISP_COD { get; private set; }
        public string USR_DESC { get; private set; }
        public string EXC_DESC { get; private set; }
    }
}
