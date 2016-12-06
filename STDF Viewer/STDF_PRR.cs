using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDF_Viewer
{
    public class PRR : STDFBase
    {
        public void Initial(byte[] result)
        {
            int index = 0; string charNStr = null;
            float R4 = 0;
            int U4 = 0;
            this.HEAD_NUM = result[0];
            this.SITE_NUM = result[1];
            this.PART_FLG = result[2];
            this.NUM_TEST = (result[4] << 8) + result[3];
            this.HARD_BIN = (result[6] << 8) + result[5];
            this.SOFT_BIN = (result[8] << 8) + result[7];
            this.X_COORD = (result[10] << 8) + result[9];
            this.Y_COORD = (result[12] << 8) + result[11];
            index = 12;
            index = ReadU4(out U4, result, index + 1); this.TEST_T = U4;
            index = ReadCharN(out charNStr, result, index + 1); this.PART_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.PART_TXT = charNStr;
            this.PART_FIX = 0;
        }

        public int HEAD_NUM { get; private set; }
        public int SITE_NUM { get; private set; }
        public int PART_FLG { get; private set; }
        public int NUM_TEST { get; private set; }
        public int HARD_BIN { get; private set; }
        public int SOFT_BIN { get; private set; }
        public int X_COORD { get; private set; }
        public int Y_COORD { get; private set; }
        public int TEST_T { get; private set; }
        public string PART_ID { get; private set; }
        public string PART_TXT { get; private set; }
        public int PART_FIX { get; private set; }
    }
}
