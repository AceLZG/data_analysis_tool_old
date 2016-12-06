using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDF_Viewer
{
    public class FAR : STDFBase
    {
        public void Initial(byte[] result)
        {
            this.CPU_TYPE = result[0];
            this.STDF_VER = result[1];
        }

        public int CPU_TYPE { get; private set; }
        public int STDF_VER { get; private set; }
    }
}
