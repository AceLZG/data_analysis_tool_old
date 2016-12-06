using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDF_Viewer
{
    public class STDFBase
    {
        public int ReadCharN(out string resultStr, byte[] result, int startIndex)
        {
            int endIndex;
            resultStr = null;
            int len = (int)result[startIndex];

            if (len != 0)
            {
                for (int i = 1; i <= len; i++)
                {
                    resultStr = resultStr + Convert.ToString((char)result[startIndex + i]);
                }
                endIndex = startIndex + len;
            }
            else
            {
                endIndex = startIndex;
            }

            return endIndex;
        }

        public int ReadR4(out float R4, byte[] result, int startIndex)
        {
            if (startIndex + 3 < result.Length)
            {
                byte[] buffer = new byte[] { result[startIndex], result[startIndex + 1], result[startIndex + 2], result[startIndex + 3] };
                R4 = BitConverter.ToSingle(buffer, 0);
                return startIndex + 3;
            }
            else
            {
                R4 = 0;
                return result.Length;
            }
        }

        public int ReadU4(out int U4, byte[] result, int startIndex)
        {
            if (startIndex + 3 < result.Length)
            {
                U4 = ((int)result[startIndex + 3] << 24) + ((int)result[startIndex + 2] << 16) + ((int)result[startIndex + 1] << 8) + result[startIndex];
                return startIndex + 3;
            }
            else
            {
                U4 = 0;
                return result.Length;
            }
        }

        public string CalLocalTimeStr(int timeSample)
        {
            System.DateTime localTime = new DateTime(1970, 1, 1, 0, 0, 0);
            localTime = localTime.AddSeconds(timeSample);
            localTime = localTime.AddHours(8);
            return localTime.ToString();
        }

    }
}
