namespace STDF_V4
{
    using System;
    using System.IO;
    using System.Text;

    public class PLRRecord : STDFRecord
    {
        public ushort GRP_CNT;
        public ushort[] GRP_INDX;
        public ushort[] GRP_MODE;
        public byte[] GRP_RADX;
        public string[] PGM_CHAL;
        public string[] PGM_CHAR;
        public string[] RTN_CHAL;
        public string[] RTN_CHAR;

        public PLRRecord() : base(STDFRecordType.PLR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.GRP_CNT);
            num += base.Read(br, this.GRP_CNT, out this.GRP_INDX);
            num += base.Read(br, this.GRP_CNT, out this.GRP_MODE);
            num += base.Read(br, this.GRP_CNT, out this.GRP_RADX);
            num += base.Read(br, this.GRP_CNT, out this.PGM_CHAR);
            num += base.Read(br, this.GRP_CNT, out this.RTN_CHAR);
            num += base.Read(br, this.GRP_CNT, out this.PGM_CHAL);
            return (num + base.Read(br, this.GRP_CNT, out this.RTN_CHAL));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("PLR (Pin List Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\tCount of Pins/Pin Groups\t\t{0}\n", this.GRP_CNT);
            ushort[] numArray = new ushort[] { 0, 10, 20, 0x15, 0x16, 0x17, 30, 0x1f, 0x20, 0x21 };
            string[] strArray = new string[] { "Unknown", "Normal", "SCIO (Same Cycle I/O", "SCIO Midband", "SCIO Valid", "SCIO Window Sustain", "Dual drive (two drive bits per cycle)", "Dual drive Midband", "Dual drive Valid", "Dual drive Window Sustain" };
            builder.AppendFormat("\nGroup   Pin    ProgramStateChar   ReturnStateChar    Group\n", new object[0]);
            builder.AppendFormat("\n        Index  Left Right         Left Right         Mode\n", new object[0]);
            for (int i = 0; i < this.GRP_CNT; i++)
            {
                builder.AppendFormat(" {0,-7}{1,-7} {2,-4} {3,-13} {4,-13} ", new object[] { i + 1, this.PGM_CHAL[i], this.PGM_CHAR[i], this.RTN_CHAL[i], this.RTN_CHAR[i] });
                int index = 0;
                for (int j = 0; j < numArray.Length; j++)
                {
                    if (this.GRP_MODE[i] == numArray[j])
                    {
                        index = j;
                        break;
                    }
                }
                builder.AppendFormat("{0}\n", strArray[index]);
            }
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            if (this.GRP_CNT != this.GRP_INDX.Length)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("PLR({0}) Record data inconsistency:\n", base.RecordNumber);
                builder.AppendFormat("\tGRP_CNT must match the size of array GRP_INDX\n", new object[0]);
                builder.AppendFormat("\tGRP_CNT = {0}, size of GRP_INDX is {1}\n", this.GRP_CNT, this.GRP_INDX.Length);
                throw new Exception(builder.ToString());
            }
            if (this.GRP_CNT != this.GRP_MODE.Length)
            {
                StringBuilder builder2 = new StringBuilder();
                builder2.AppendFormat("PLR({0}) Record data inconsistency:\n", base.RecordNumber);
                builder2.AppendFormat("\tGRP_CNT must match the size of array GRP_MODE\n", new object[0]);
                builder2.AppendFormat("\tGRP_CNT = {0}, size of GRP_MODE is {1}\n", this.GRP_CNT, this.GRP_MODE.Length);
                throw new Exception(builder2.ToString());
            }
            if (this.GRP_CNT != this.GRP_RADX.Length)
            {
                StringBuilder builder3 = new StringBuilder();
                builder3.AppendFormat("PLR({0}) Record data inconsistency:\n", base.RecordNumber);
                builder3.AppendFormat("\tGRP_CNT must match the size of array GRP_RADX\n", new object[0]);
                builder3.AppendFormat("\tGRP_CNT = {0}, size of GRP_RADX is {1}\n", this.GRP_CNT, this.GRP_RADX.Length);
                throw new Exception(builder3.ToString());
            }
            if (this.GRP_CNT != this.PGM_CHAR.Length)
            {
                StringBuilder builder4 = new StringBuilder();
                builder4.AppendFormat("PLR({0}) Record data inconsistency:\n", base.RecordNumber);
                builder4.AppendFormat("\tGRP_CNT must match the size of array PGM_CHAR\n", new object[0]);
                builder4.AppendFormat("\tGRP_CNT = {0}, size of GRP_CHAR is {1}\n", this.GRP_CNT, this.PGM_CHAR.Length);
                throw new Exception(builder4.ToString());
            }
            if (this.GRP_CNT != this.RTN_CHAR.Length)
            {
                StringBuilder builder5 = new StringBuilder();
                builder5.AppendFormat("PLR({0}) Record data inconsistency:\n", base.RecordNumber);
                builder5.AppendFormat("\tGRP_CNT must match the size of array RTN_CHAR\n", new object[0]);
                builder5.AppendFormat("\tGRP_CNT = {0}, size of RTN_CHAR is {1}\n", this.GRP_CNT, this.RTN_CHAR.Length);
                throw new Exception(builder5.ToString());
            }
            if (this.GRP_CNT != this.PGM_CHAL.Length)
            {
                StringBuilder builder6 = new StringBuilder();
                builder6.AppendFormat("PLR({0}) Record data inconsistency:\n", base.RecordNumber);
                builder6.AppendFormat("\tGRP_CNT must match the size of array PGM_CHAL\n", new object[0]);
                builder6.AppendFormat("\tGRP_CNT = {0}, size of GRP_CHAL is {1}\n", this.GRP_CNT, this.PGM_CHAL.Length);
                throw new Exception(builder6.ToString());
            }
            if (this.GRP_CNT != this.RTN_CHAL.Length)
            {
                StringBuilder builder7 = new StringBuilder();
                builder7.AppendFormat("PLR({0}) Record data inconsistency:\n", base.RecordNumber);
                builder7.AppendFormat("\tGRP_CNT must match the size of array RTN_CHAL\n", new object[0]);
                builder7.AppendFormat("\tGRP_CNT = {0}, size of RTN_CHAR is {1}\n", this.GRP_CNT, this.RTN_CHAL.Length);
                throw new Exception(builder7.ToString());
            }
            bw.Write(Convert.ToUInt16(STDFRecordType.PLR));
            bw.Write(this.GRP_CNT);
            for (int i = 0; i < this.GRP_CNT; i++)
            {
                bw.Write(this.GRP_INDX[i]);
            }
            for (int j = 0; j < this.GRP_CNT; j++)
            {
                bw.Write(this.GRP_MODE[j]);
            }
            for (int k = 0; k < this.GRP_CNT; k++)
            {
                bw.Write(this.GRP_RADX[k]);
            }
            for (int m = 0; m < this.GRP_CNT; m++)
            {
                bw.Write(this.GRP_INDX[m]);
            }
            return 0;
        }
    }
}

