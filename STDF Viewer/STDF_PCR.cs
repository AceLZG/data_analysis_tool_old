using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDF_Viewer
{
    public class PCR : STDFBase
    {
        public void Initial(byte[] result)
        {
            int U4 = 0; int index = 1;
            HEAD_NUM = (int)result[0];
            SITE_NUM = (int)result[1];


            index = ReadU4(out U4, result, index + 1); this.PART_CNT = U4;
            index = ReadU4(out U4, result, index + 1); this.RTST_CNT = U4;
            index = ReadU4(out U4, result, index + 1); this.ABRT_CNT = U4;
            index = ReadU4(out U4, result, index + 1); this.GOOD_CNT = U4;
            index = ReadU4(out U4, result, index + 1); this.FUNC_CNT = U4;
        }

        public int HEAD_NUM { get; private set; }
        public int SITE_NUM { get; private set; }
        public int PART_CNT { get; private set; }
        public int RTST_CNT { get; private set; }
        public int ABRT_CNT { get; private set; }
        public int GOOD_CNT { get; private set; }
        public int FUNC_CNT { get; private set; }
    }
}
