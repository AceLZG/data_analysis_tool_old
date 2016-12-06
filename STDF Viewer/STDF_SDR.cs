using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDF_Viewer
{
    public class SDR : STDFBase
    {
        public void Initial(byte[] result)
        {
            this.HEAD_NUM = (int)result[0];
            this.SITE_GRP = (int)result[1];
            this.SITE_CNT = (int)result[2];
            this.SITE_NUM = new int[SITE_CNT];
            for (int i = 0; i < this.SITE_CNT; i++)
            {
                this.SITE_NUM[i] = result[3 + i];
            }

            string charNStr = null; int index = 2 + this.SITE_CNT;
            index = ReadCharN(out charNStr, result, index + 1); this.HAND_TYP = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.HAND_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.CARD_TYP = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.CARD_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.LOAD_TYP = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.LOAD_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.DIB_TYP = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.DIB_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.CABL_TYP = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.CABL_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.CONT_TYP = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.CONT_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.LASR_TYP = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.LASR_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.EXTR_TYP = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.EXTR_ID = charNStr;
        }

        public int HEAD_NUM { get; private set; }
        public int SITE_GRP { get; private set; }
        public int SITE_CNT { get; private set; }
        public int[] SITE_NUM { get; private set; }
        public string HAND_TYP { get; private set; }
        public string HAND_ID { get; private set; }
        public string CARD_TYP { get; private set; }
        public string CARD_ID { get; private set; }
        public string LOAD_TYP { get; private set; }
        public string LOAD_ID { get; private set; }
        public string DIB_TYP { get; private set; }
        public string DIB_ID { get; private set; }
        public string CABL_TYP { get; private set; }
        public string CABL_ID { get; private set; }
        public string CONT_TYP { get; private set; }
        public string CONT_ID { get; private set; }
        public string LASR_TYP { get; private set; }
        public string LASR_ID { get; private set; }
        public string EXTR_TYP { get; private set; }
        public string EXTR_ID { get; private set; }

    }
}
