using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDF_Viewer
{
    public class PTR : STDFBase
    {
        public void Initial(byte[] result)
        {
            int index = 0; string charNStr = null;
            float R4 = 0;
            int U4 = 0;
            index = ReadU4(out U4, result, index); this.TEST_NUM = U4;
            this.HEAD_NUM = result[index + 1];
            this.SITE_NUM = result[index + 2];
            this.TEST_FLG = result[index + 3];
            this.PARM_FLG = result[index + 4];
            index = ReadR4(out R4, result, index + 5); this.RESULT = R4;
            index = ReadCharN(out charNStr, result, index + 1); this.TEST_TXT = charNStr;
            index = ReadCharN(out charNStr, result, index + 1); this.ALARM_ID = charNStr;

            if (index < result.Length - 2)
            {
                this.OPT_FLAG = result[index + 1];
                this.RES_SCAL = (sbyte)result[index + 2];
                this.LLM_SCAL = (sbyte)result[index + 3];
                this.HLM_SCAL = (sbyte)result[index + 4];
                index = ReadR4(out R4, result, index + 5); this.LO_LIMIT = R4;
                index = ReadR4(out R4, result, index + 1); this.HI_LIMIT = R4;
                index = ReadCharN(out charNStr, result, index + 1); this.UNITS = charNStr;
                index = ReadCharN(out charNStr, result, index + 1); this.C_RESFMT = charNStr;
                index = ReadCharN(out charNStr, result, index + 1); this.C_LLMFMT = charNStr;
                index = ReadCharN(out charNStr, result, index + 1); this.C_HLMFMT = charNStr;
                index = ReadR4(out R4, result, index + 1); this.LO_SPEC = R4;
                index = ReadR4(out R4, result, index + 1); this.HI_SPEC = R4;
            }
        }

        public void Initial1(byte[] result)
        {
            int index = 0; string charNStr = null;
            float R4 = 0;
            int U4 = 0;
            index = ReadU4(out U4, result, index); this.TEST_NUM = U4;
            this.HEAD_NUM = result[index + 1];
            this.SITE_NUM = result[index + 2];
            this.TEST_FLG = result[index + 3];
            this.PARM_FLG = result[index + 4];
            index = ReadR4(out R4, result, index + 5); this.RESULT = R4;
            //index = ReadCharN(out charNStr, result, index + 1); this.TEST_TXT = charNStr;
            //index = ReadCharN(out charNStr, result, index + 1); this.ALARM_ID = charNStr;
            //this.OPT_FLAG = result[index + 1];
            //this.RES_SCAL = result[index + 2];
            //this.LLM_SCAL = result[index + 3];
            //this.HLM_SCAL = result[index + 4];
            //index = ReadR4(out R4, result, index + 5); this.LO_LIMIT = R4;
            //index = ReadR4(out R4, result, index + 1); this.HI_LIMIT = R4;
            //index = ReadCharN(out charNStr, result, index + 1); this.UNITS = charNStr;
            //index = ReadCharN(out charNStr, result, index + 1); this.C_RESFMT = charNStr;
            //index = ReadCharN(out charNStr, result, index + 1); this.C_LLMFMT = charNStr;
            //index = ReadCharN(out charNStr, result, index + 1); this.C_HLMFMT = charNStr;
            //index = ReadR4(out R4, result, index + 1); this.LO_SPEC = R4;
            //index = ReadR4(out R4, result, index + 1); this.HI_SPEC = R4;
        }

        public int TEST_NUM { get; private set; }
        public int HEAD_NUM { get; private set; }
        public int SITE_NUM { get; private set; }
        public int TEST_FLG { get; private set; }
        public int PARM_FLG { get; private set; }
        public float RESULT { get; private set; }
        public string TEST_TXT { get; private set; }
        public string ALARM_ID { get; private set; }
        public int OPT_FLAG { get; private set; }
        public int RES_SCAL { get; private set; }
        public int LLM_SCAL { get; private set; }
        public int HLM_SCAL { get; private set; }
        public float LO_LIMIT { get; private set; }
        public float HI_LIMIT { get; private set; }
        public string UNITS { get; private set; }
        public string C_RESFMT { get; private set; }
        public string C_LLMFMT { get; private set; }
        public string C_HLMFMT { get; private set; }
        public float LO_SPEC { get; private set; }
        public float HI_SPEC { get; private set; }
    }
}
