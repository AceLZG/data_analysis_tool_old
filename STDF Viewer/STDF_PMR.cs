using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDF_Viewer
{
    public class PMR : STDFBase
    {
        public void Initial(byte[] result)
        {
            this.PMR_INDX = ((int)result[1] << 8) + (int)result[0];
            this.CHAN_TYP = ((int)result[3] << 8) + (int)result[2];

            int index = 3; string charNStr = null;
            index = ReadCharN(out charNStr, result, index + 1); this.CHAN_NAM = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.PHY_NAM = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.LOG_NAM = charNStr;
            this.HEAD_NUM = result[index + 1];
            this.SITE_NUM = result[index + 2];

        }

        public int PMR_INDX { get; private set; }
        public int CHAN_TYP { get; private set; }
        public string CHAN_NAM { get; private set; }
        public string PHY_NAM { get; private set; }
        public string LOG_NAM { get; private set; }
        public int HEAD_NUM { get; private set; }
        public int SITE_NUM { get; private set; }
    }
}
