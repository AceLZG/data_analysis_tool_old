namespace STDF_V4
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public class GDRRecord : STDFRecord
    {
        private List<byte[]> m_gdrFields;
        private int NextFieldNumber;

        public GDRRecord() : base(STDFRecordType.GDR)
        {
            this.m_gdrFields = new List<byte[]>();
        }

        public void AddBits(bool[] value)
        {
            int num = (int) Math.Ceiling((decimal) (value.Length / 8M));
            byte[] destinationArray = new byte[num + 2];
            Array.Copy(BitConverter.GetBytes((ushort) value.Length), destinationArray, 2);
            for (int i = 0; i < value.Length; i++)
            {
                int num3;
                byte num4;
                this.GetBitLocation(i, out num3, out num4);
                if (value[i])
                {
                    destinationArray[2 + num3] = (byte) (destinationArray[2 + num3] | num4);
                }
                else
                {
                    destinationArray[2 + num3] = (byte) (destinationArray[2 + num3] & ~num4);
                }
            }
            this.AddField(GDR_DataTypeCode.DN, destinationArray);
        }

        public void AddBytes(byte[] value)
        {
            if (value.Length > 0xff)
            {
                throw new ArgumentException("You cannot add a buffer longer than 255 bytes.");
            }
            this.AddLengthPrependedField(GDR_DataTypeCode.BN, value);
        }

        private void AddField(GDR_DataTypeCode type, byte[] buf)
        {
            byte[] destinationArray = new byte[buf.Length + 1];
            destinationArray[0] = (byte) type;
            Array.Copy(buf, 0, destinationArray, 1, buf.Length);
            this.m_gdrFields.Add(destinationArray);
        }

        public void AddI1(byte value)
        {
            this.AddField(GDR_DataTypeCode.I1, new byte[] { value });
        }

        public void AddI2(short value)
        {
            this.EnsureValueIsOnTwoByteBoundary();
            this.AddField(GDR_DataTypeCode.I2, BitConverter.GetBytes(value));
        }

        public void AddI4(int value)
        {
            this.EnsureValueIsOnTwoByteBoundary();
            this.AddField(GDR_DataTypeCode.I4, BitConverter.GetBytes(value));
        }

        private void AddLengthPrependedField(GDR_DataTypeCode type, byte[] buf)
        {
            byte[] destinationArray = new byte[buf.Length + 2];
            destinationArray[0] = (byte) type;
            destinationArray[1] = (byte) buf.Length;
            Array.Copy(buf, 0, destinationArray, 2, buf.Length);
            this.m_gdrFields.Add(destinationArray);
        }

        public void AddN1(byte value)
        {
            this.AddField(GDR_DataTypeCode.N1, new byte[] { value });
        }

        public void AddPad()
        {
            this.AddField(GDR_DataTypeCode.B0, new byte[0]);
        }

        public void AddR4(float value)
        {
            this.EnsureValueIsOnTwoByteBoundary();
            this.AddField(GDR_DataTypeCode.R4, BitConverter.GetBytes(value));
        }

        public void AddR8(double value)
        {
            this.EnsureValueIsOnTwoByteBoundary();
            this.AddField(GDR_DataTypeCode.R8, BitConverter.GetBytes(value));
        }

        public void AddString(string value)
        {
            if (value.Length > 0xff)
            {
                throw new ArgumentException("You cannot add a string longer than 255 bytes.");
            }
            this.AddLengthPrependedField(GDR_DataTypeCode.CN, Encoding.ASCII.GetBytes(value));
        }

        public void AddU1(byte value)
        {
            this.AddField(GDR_DataTypeCode.U1, new byte[] { value });
        }

        public void AddU2(ushort value)
        {
            this.EnsureValueIsOnTwoByteBoundary();
            this.AddField(GDR_DataTypeCode.U2, BitConverter.GetBytes(value));
        }

        public void AddU4(uint value)
        {
            this.EnsureValueIsOnTwoByteBoundary();
            this.AddField(GDR_DataTypeCode.U4, BitConverter.GetBytes(value));
        }

        private void EnsureValueIsOnTwoByteBoundary()
        {
            if ((this.RecordLength % 2) == 0)
            {
                this.AddPad();
            }
        }

        private void GetBitLocation(int bitIndex, out int byteIndex, out byte bitMask)
        {
            byteIndex = bitIndex / 8;
            bitMask = 1;
            int num = bitIndex % 8;
            for (int i = 0; i < num; i++)
            {
                bitMask = (byte) (bitMask << 1);
            }
        }

        public GDR_DataTypeCode GetFieldType(int index)
        {
            return (GDR_DataTypeCode)this.m_gdrFields[index][0];
        }

        public object GetFieldValue(int index)
        {
            byte[] buffer = this.m_gdrFields[index];
            switch (this.GetFieldType(index))
            {
                case GDR_DataTypeCode.B0:
                    return null;

                case GDR_DataTypeCode.U1:
                    return buffer[1];

                case GDR_DataTypeCode.U2:
                    return BitConverter.ToUInt16(buffer, 1);

                case GDR_DataTypeCode.U4:
                    return BitConverter.ToUInt32(buffer, 1);

                case GDR_DataTypeCode.I1:
                    return Convert.ToSByte(buffer[1]);

                case GDR_DataTypeCode.I2:
                    return BitConverter.ToInt16(buffer, 1);

                case GDR_DataTypeCode.I4:
                    return BitConverter.ToInt32(buffer, 1);

                case GDR_DataTypeCode.R4:
                    return BitConverter.ToSingle(buffer, 1);

                case GDR_DataTypeCode.R8:
                    return BitConverter.ToDouble(buffer, 1);

                case GDR_DataTypeCode.CN:
                    return Encoding.ASCII.GetString(buffer, 2, buffer.Length - 2);

                case GDR_DataTypeCode.BN:
                {
                    byte[] destinationArray = new byte[buffer.Length - 2];
                    Array.Copy(buffer, 2, destinationArray, 0, destinationArray.Length);
                    return destinationArray;
                }
                case GDR_DataTypeCode.DN:
                {
                    bool[] flagArray = new bool[BitConverter.ToUInt16(buffer, 1)];
                    for (int i = 0; i < flagArray.Length; i++)
                    {
                        byte num3;
                        int num4;
                        this.GetBitLocation(i, out num4, out num3);
                        flagArray[i] = 0 < (buffer[3 + num4] & num3);
                    }
                    return flagArray;
                }
                case GDR_DataTypeCode.N1:
                    return buffer[1];
            }
            throw new ApplicationException("Unknown field type.");
        }

        public object GetNextField()
        {
            object nextField = null;
            while (this.NextFieldNumber < this.m_gdrFields.Count)
            {
                nextField = this.GetNextField(this.NextFieldNumber);
                if (nextField != null)
                {
                    return nextField;
                }
            }
            return null;
        }

        private object GetNextField(int index)
        {
            this.NextFieldNumber = index;
            return this.GetFieldValue(this.NextFieldNumber++);
        }

        public override int Read(BinaryReader br)
        {
            ushort num;
            this.m_gdrFields.Clear();
            base.Read(br, out num);
            for (ushort i = 0; i < num; i = (ushort) (i + 1))
            {
                byte num3;
                base.Read(br, out num3);
                GDR_DataTypeCode type = (GDR_DataTypeCode) num3;
                switch (type)
                {
                    case GDR_DataTypeCode.B0:
                        this.AddField(type, new byte[0]);
                        break;

                    case GDR_DataTypeCode.U1:
                    case GDR_DataTypeCode.I1:
                    case GDR_DataTypeCode.N1:
                        this.AddField(type, br.ReadBytes(1));
                        break;

                    case GDR_DataTypeCode.U2:
                    case GDR_DataTypeCode.I2:
                        this.AddField(type, br.ReadBytes(2));
                        break;

                    case GDR_DataTypeCode.U4:
                    case GDR_DataTypeCode.I4:
                    case GDR_DataTypeCode.R4:
                        this.AddField(type, br.ReadBytes(4));
                        break;

                    case GDR_DataTypeCode.R8:
                        this.AddField(type, br.ReadBytes(8));
                        break;

                    case GDR_DataTypeCode.CN:
                    case GDR_DataTypeCode.BN:
                        this.AddLengthPrependedField(type, br.ReadBytes(br.ReadByte()));
                        break;

                    case GDR_DataTypeCode.DN:
                    {
                        byte[] buffer = br.ReadBytes(2);
                        int length = (int) Math.Ceiling((decimal) (BitConverter.ToUInt16(buffer, 0) / 8M));
                        byte[] destinationArray = new byte[length + buffer.Length];
                        Array.Copy(buffer, destinationArray, 2);
                        Array.Copy(br.ReadBytes(length), 0, destinationArray, 2, length);
                        this.AddField(type, destinationArray);
                        break;
                    }
                }
            }
            return (this.RecordLength - 4);
        }

        public bool SetNextField(int index)
        {
            if ((index < this.FLD_CNT) && (this.FLD_CNT >= 0))
            {
                this.NextFieldNumber = index;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.Append(string.Format("GDR (Generic data record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN));
            builder.Append(string.Format("\tField Count: {0}\n", this.FLD_CNT));
            for (int i = 0; i < this.FLD_CNT; i++)
            {
                string str;
                GDR_DataTypeCode fieldType = this.GetFieldType(i);
                object fieldValue = this.GetFieldValue(i);
                switch (fieldType)
                {
                    case GDR_DataTypeCode.B0:
                        str = "[Pad Byte]";
                        break;

                    case GDR_DataTypeCode.BN:
                        str = "[" + ((byte[]) fieldValue).Length + "-byte array";
                        break;

                    case GDR_DataTypeCode.DN:
                    {
                        bool[] flagArray = (bool[]) this.GetFieldValue(i);
                        StringBuilder builder2 = new StringBuilder();
                        builder2.AppendFormat("[ {0}", flagArray[0].ToString());
                        for (int j = 1; j < ((bool[]) fieldValue).Length; j++)
                        {
                            builder2.AppendFormat(", {0}", flagArray[j].ToString());
                        }
                        builder2.Append(" ]");
                        str = builder2.ToString();
                        break;
                    }
                    default:
                        str = fieldValue.ToString();
                        break;
                }
                builder.Append(string.Format("\t{0}:\t\t{1}\n", fieldType.ToString(), str));
            }
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.GDR));
            int num = 0;
            foreach (byte[] buffer in this.m_gdrFields)
            {
                num += buffer.Length;
            }
            if ((num % 2) != 0)
            {
                bw.Write((ushort) (this.m_gdrFields.Count + 1));
            }
            else
            {
                bw.Write((ushort) this.m_gdrFields.Count);
            }
            foreach (byte[] buffer2 in this.m_gdrFields)
            {
                bw.Write(buffer2);
            }
            if ((num % 2) != 0)
            {
                bw.Write(Convert.ToByte(0));
            }
            return 0;
        }

        public ushort FLD_CNT
        {
            get
            {
                return Convert.ToUInt16(this.m_gdrFields.Count);
            }
        }

        public int RecordLength
        {
            get
            {
                int num = 0;
                num += 2;
                num += 2;
                num += 2;
                foreach (byte[] buffer in this.m_gdrFields)
                {
                    num += buffer.Length;
                }
                return num;
            }
        }
    }
}

