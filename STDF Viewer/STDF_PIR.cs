using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDF_Viewer
{
    public class PIR : STDFBase
    {
        public void Initial(byte[] result)
        {
            this.HEAD_NUM = result[0];
            this.SITE_NUM = result[1];
        }

        public int HEAD_NUM { get; private set; }
        public int SITE_NUM { get; private set; }
    }
}
