namespace STDFInterface
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public class STDFRecord
    {
        public long FilePos;
        public BYTE_ORIENTATION Processor = BYTE_ORIENTATION.UNKNOWN;
        public ushort REC_LEN;
        public STDFRecordType RECORD_TYPE;
        public long RecordNumber;
        public long RemainingBytes;

        public STDFRecord(STDFRecordType type)
        {
            this.RECORD_TYPE = type;
        }

        public virtual int Read(BinaryReader br)
        {
            return 0;
        }

        public int Read(BinaryReader br, out STDFRecordType recType)
        {
            byte num = br.ReadByte();
            int num3 = (br.ReadByte() << 8) + num;
            recType = (STDFRecordType) num3;
            return 2;
        }

        public int Read(BinaryReader br, out byte U1)
        {
            this.RemainingBytes -= 1L;
            if (this.RemainingBytes < 0L)
            {
                U1 = 0;
                return 0;
            }
            U1 = br.ReadByte();
            return 1;
        }

        public int Read(BinaryReader br, out DateTime dt)
        {
            uint num = 0;
            if ((this.RemainingBytes - 4L) < 0L)
            {
                dt = new DateTime(0x7b2, 1, 1);
                return 0;
            }
            int num2 = this.Read(br, out num);
            dt = new DateTime(0x7b2, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds((double) num);
            return num2;
        }

        public int Read(BinaryReader br, out double R8)
        {
            this.RemainingBytes -= 1L;
            if (this.RemainingBytes < 8L)
            {
                R8 = 0.0;
                return 0;
            }
            if (this.Processor == BYTE_ORIENTATION.IBMPC)
            {
                R8 = br.ReadDouble();
            }
            else
            {
                byte[] buffer = new byte[8];
                buffer[7] = br.ReadByte();
                buffer[6] = br.ReadByte();
                buffer[5] = br.ReadByte();
                buffer[4] = br.ReadByte();
                buffer[3] = br.ReadByte();
                buffer[2] = br.ReadByte();
                buffer[1] = br.ReadByte();
                buffer[0] = br.ReadByte();
                R8 = BitConverter.ToDouble(buffer, 0);
            }
            return 8;
        }

        public int Read(BinaryReader br, out short I2)
        {
            this.RemainingBytes -= 2L;
            if (this.RemainingBytes < 0L)
            {
                I2 = 0;
                return 0;
            }
            if (this.Processor == BYTE_ORIENTATION.IBMPC)
            {
                I2 = br.ReadInt16();
            }
            else
            {
                byte[] buffer = new byte[2];
                buffer[1] = br.ReadByte();
                buffer[0] = br.ReadByte();
                I2 = BitConverter.ToInt16(buffer, 0);
            }
            return 2;
        }

        public int Read(BinaryReader br, out int I4)
        {
            this.RemainingBytes -= 4L;
            if (this.RemainingBytes < 0L)
            {
                I4 = 0;
                return 0;
            }
            if (this.Processor == BYTE_ORIENTATION.IBMPC)
            {
                I4 = br.ReadInt32();
            }
            else
            {
                byte[] buffer = new byte[4];
                buffer[3] = br.ReadByte();
                buffer[2] = br.ReadByte();
                buffer[1] = br.ReadByte();
                buffer[0] = br.ReadByte();
                I4 = BitConverter.ToInt32(buffer, 0);
            }
            return 4;
        }

        public int Read(BinaryReader br, out sbyte I1)
        {
            this.RemainingBytes -= 1L;
            if (this.RemainingBytes < 0L)
            {
                I1 = 0;
                return 0;
            }
            I1 = br.ReadSByte();
            return 1;
        }

        public int Read(BinaryReader br, out float R4)
        {
            this.RemainingBytes -= 4L;
            if (this.RemainingBytes < 0L)
            {
                R4 = 0f;
                return 0;
            }
            if (this.Processor == BYTE_ORIENTATION.IBMPC)
            {
                R4 = br.ReadSingle();
            }
            else
            {
                byte[] buffer = new byte[4];
                buffer[3] = br.ReadByte();
                buffer[2] = br.ReadByte();
                buffer[1] = br.ReadByte();
                buffer[0] = br.ReadByte();
                R4 = BitConverter.ToSingle(buffer, 0);
            }
            return 4;
        }

        public int Read(BinaryReader br, out string cn)
        {
            this.RemainingBytes -= 1L;
            if (this.RemainingBytes < 0L)
            {
                cn = "";
                return 0;
            }
            byte count = br.ReadByte();
            this.RemainingBytes -= count;
            if (this.RemainingBytes < 0L)
            {
                cn = "";
                return 0;
            }
            byte[] bytes = br.ReadBytes(count);
            cn = Encoding.GetEncoding(0x4e3).GetString(bytes);
            return (count + 1);
        }

        public int Read(BinaryReader br, out ushort U2)
        {
            this.RemainingBytes -= 2L;
            if (this.RemainingBytes < 0L)
            {
                U2 = 0;
                return 0;
            }
            if (this.Processor == BYTE_ORIENTATION.IBMPC)
            {
                U2 = br.ReadUInt16();
            }
            else
            {
                byte[] buffer = new byte[2];
                buffer[1] = br.ReadByte();
                buffer[0] = br.ReadByte();
                U2 = BitConverter.ToUInt16(buffer, 0);
            }
            return 2;
        }

        public int Read(BinaryReader br, out uint U4)
        {
            this.RemainingBytes -= 4L;
            if (this.RemainingBytes < 0L)
            {
                U4 = 0;
                return 0;
            }
            if (this.Processor == BYTE_ORIENTATION.IBMPC)
            {
                U4 = br.ReadUInt32();
            }
            else
            {
                byte[] buffer = new byte[4];
                buffer[3] = br.ReadByte();
                buffer[2] = br.ReadByte();
                buffer[1] = br.ReadByte();
                buffer[0] = br.ReadByte();
                U4 = BitConverter.ToUInt32(buffer, 0);
            }
            return 4;
        }

        public int Read(BinaryReader br, out byte[] byteArray)
        {
            this.RemainingBytes -= 1L;
            if (this.RemainingBytes < 0L)
            {
                byteArray = null;
                return 0;
            }
            byte count = br.ReadByte();
            this.RemainingBytes -= count;
            if (this.RemainingBytes < 0L)
            {
                byteArray = null;
                return 0;
            }
            byteArray = br.ReadBytes(count);
            return (count + 1);
        }

        public int Read(BinaryReader br, ushort cnt, out byte[] byteArray)
        {
            int num = 0;
            this.RemainingBytes -= cnt;
            if (this.RemainingBytes < 0L)
            {
                byteArray = null;
                return 0;
            }
            byteArray = new byte[cnt];
            for (int i = 0; i < cnt; i++)
            {
                byteArray[i] = br.ReadByte();
                num++;
            }
            return num;
        }

        public int Read(BinaryReader br, ushort cnt, out string[] stringArray)
        {
            int num = 0;
            stringArray = new string[cnt];
            for (int i = 0; i < cnt; i++)
            {
                stringArray[i] = null;
                num += this.Read(br, out stringArray[i]);
            }
            return num;
        }

        public int Read(BinaryReader br, ushort cnt, out ushort[] ushortArray)
        {
            int num = 0;
            if ((this.RemainingBytes - (cnt * 2)) < 0L)
            {
                ushortArray = null;
                return 0;
            }
            ushortArray = new ushort[cnt];
            for (int i = 0; i < cnt; i++)
            {
                num += this.Read(br, out ushortArray[i]);
            }
            return num;
        }

        public int ReadArray(BinaryReader br, ushort cnt, out float[] floatArray)
        {
            int num = 0;
            if ((this.RemainingBytes - (cnt * 4)) < 0L)
            {
                floatArray = null;
                return 0;
            }
            floatArray = new float[cnt];
            for (int i = 0; i < cnt; i++)
            {
                num += this.Read(br, out floatArray[i]);
            }
            return num;
        }

        public int ReadArrayNibble(BinaryReader br, ushort cnt, out byte[] byteArray)
        {
            int num = 0;
            int num2 = 0;
            byteArray = new byte[cnt];
            num = (cnt / 2) + (cnt % 2);
            this.RemainingBytes -= num;
            if (this.RemainingBytes < 0L)
            {
                byteArray = null;
                return 0;
            }
            int num3 = 0;
            int num4 = 0;
            while (num3 < num)
            {
                num2 = br.ReadByte();
                byteArray[num4++] = Convert.ToByte((int) (num2 & 15));
                if (num4 < cnt)
                {
                    byteArray[num4++] = Convert.ToByte((int) ((num2 & 240) >> 4));
                }
                num3++;
            }
            return num;
        }

        public virtual int Write(BinaryWriter bw)
        {
            return 0;
        }

        public int WriteArray(BinaryWriter bw, byte[] byte_array)
        {
            byte num;
            if (byte_array.Length > 0xff)
            {
                num = 0xff;
            }
            else
            {
                num = Convert.ToByte(byte_array.Length);
            }
            bw.Write(num);
            for (int i = 0; i < num; i++)
            {
                bw.Write(byte_array[i]);
            }
            return (num + 1);
        }

        public int WriteArray(BinaryWriter bw, float[] float_array)
        {
            int length = float_array.Length;
            for (int i = 0; i < length; i++)
            {
                bw.Write(float_array[i]);
            }
            return (length * 4);
        }

        public int WriteArray(BinaryWriter bw, ushort[] ushort_array)
        {
            int length = ushort_array.Length;
            for (int i = 0; i < length; i++)
            {
                bw.Write(ushort_array[i]);
            }
            return (length * 2);
        }

        public int WriteArray(BinaryWriter bw, BitArray bit_array)
        {
            int num = 0;
            int num2 = 0;
            int length = bit_array.Length;
            int num4 = (length / 8) + (((length % 8) == 0) ? 0 : 1);
            bw.Write(Convert.ToByte(num4));
            for (int i = 0; i < num4; i++)
            {
                num = 0;
                for (int j = 0; j < 8; j++)
                {
                    num = num << (1 + ((bit_array[num2] != null) ? 1 : 0));
                    num2++;
                }
                bw.Write(Convert.ToByte(num));
            }
            return (num4 + 1);
        }

        public int WriteArrayNibble(BinaryWriter bw, byte[] byteArray)
        {
            byte num = 0;
            int num2 = 0;
            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = Convert.ToByte((int) (byteArray[i] & 15));
                if ((i % 2) == 0)
                {
                    num = byteArray[i];
                }
                else
                {
                    num = (byte) (num | Convert.ToByte((int) (byteArray[i] << 4)));
                    bw.Write(num);
                    num2++;
                }
            }
            if (num2 != ((byteArray.Length / 2) + (byteArray.Length % 2)))
            {
                num = (byte) (num & 15);
                bw.Write(num);
                num2++;
            }
            return num2;
        }

        public int WriteString(BinaryWriter bw, string str)
        {
            byte num = 0;
            int num2 = 0;
            if (str == null)
            {
                bw.Write(num);
                return 1;
            }
            if (str.Length == 0)
            {
                bw.Write(num);
                return 1;
            }
            if (str.Length > 0xff)
            {
                str.Remove(0xff);
            }
            num = Convert.ToByte(str.Length);
            if (num > 0)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(str);
                bw.Write(num);
                num2++;
                bw.Write(bytes);
                num2 += str.Length;
            }
            return num2;
        }

        public int WriteTime(BinaryWriter bw, DateTime dt)
        {
            uint num = 0;
            DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0);
            if (dt.Ticks >= time.Ticks)
            {
                TimeSpan span = new TimeSpan(dt.Ticks - time.Ticks);
                num = Convert.ToUInt32(span.TotalSeconds);
            }
            else
            {
                num = 0;
            }
            bw.Write(num);
            return 4;
        }
    }
}

