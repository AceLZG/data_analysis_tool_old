using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDF_Viewer
{
    public class MIR : STDFBase
    {
        public void Initial(byte[] result)
        {
            int timeSampleSetup = ((int)result[3] << 24) + ((int)result[2] << 16) + ((int)result[1] << 8) + ((int)result[0]);
            this.SETUP_T = CalLocalTimeStr(timeSampleSetup);
            int timeSampleStart = ((int)result[7] << 24) + ((int)result[6] << 16) + ((int)result[5] << 8) + ((int)result[4]);
            this.START_T = CalLocalTimeStr(timeSampleStart);
            this.STAT_NUM = (int)result[8];
            this.MODE_COD = (int)result[9];
            this.RTST_COD = (int)result[10];
            this.PROT_COD = (int)result[11];
            this.BURN_TIM = ((int)result[13] << 8) + (int)result[12];
            this.CMOD_COD = (int)result[14];

            string charNStr = null; int index = 14;

            index = ReadCharN(out charNStr, result, index + 1); this.LOT_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.PART_TYP = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.NODE_NAM = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.TSTR_TYP = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.JOB_NAM = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.JOB_REV = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.SBLOT_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.OPER_NAM = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.EXEC_TYP = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.EXEC_VER = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.TEST_COD = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.TST_TEMP = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.USER_TXT = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.AUX_FILE = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.PKG_TYP = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.FAMLY_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.DATE_COD = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.FACIL_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.FLOOR_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.PROC_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.OPER_FRQ = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.SPEC_NAM = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.SPEC_VER = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.FLOW_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.SETUP_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.DSGN_REV = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.ENG_ID = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.ROM_COD = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.SERL_NUM = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.SUPR_NAM = charNStr;
        }

        public string SETUP_T { get; private set; }
        public string START_T { get; private set; }
        public int STAT_NUM { get; private set; }
        public int MODE_COD { get; private set; }
        public int RTST_COD { get; private set; }
        public int PROT_COD { get; private set; }
        public int BURN_TIM { get; private set; }
        public int CMOD_COD { get; private set; }
        public string LOT_ID { get; private set; }
        public string PART_TYP { get; private set; }
        public string NODE_NAM { get; private set; }
        public string TSTR_TYP { get; private set; }
        public string JOB_NAM { get; private set; }
        public string JOB_REV { get; private set; }
        public string SBLOT_ID { get; private set; }
        public string OPER_NAM { get; private set; }
        public string EXEC_TYP { get; private set; }
        public string EXEC_VER { get; private set; }
        public string TEST_COD { get; private set; }
        public string TST_TEMP { get; private set; }
        public string USER_TXT { get; private set; }
        public string AUX_FILE { get; private set; }
        public string PKG_TYP { get; private set; }
        public string FAMLY_ID { get; private set; }
        public string DATE_COD { get; private set; }
        public string FACIL_ID { get; private set; }
        public string FLOOR_ID { get; private set; }
        public string PROC_ID { get; private set; }
        public string OPER_FRQ { get; private set; }
        public string SPEC_NAM { get; private set; }
        public string SPEC_VER { get; private set; }
        public string FLOW_ID { get; private set; }
        public string SETUP_ID { get; private set; }
        public string DSGN_REV { get; private set; }
        public string ENG_ID { get; private set; }
        public string ROM_COD { get; private set; }
        public string SERL_NUM { get; private set; }
        public string SUPR_NAM { get; private set; }
    }
}
