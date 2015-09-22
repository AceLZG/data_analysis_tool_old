namespace STDFInterface
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;

    public class FTRRecord : STDFRecord
    {
        public string ALARM_ID;
        public uint CYCL_CNT;
        public BitArray FAIL_PIN;
        public byte HEAD_NUM;
        public uint NUM_FAIL;
        public string OP_CODE;
        public byte OPT_FLAG;
        public byte PATG_NUM;
        public ushort PGM_ICNT;
        public ushort[] PGM_INDX;
        public byte[] PGM_STAT;
        public string PROG_TXT;
        public uint REL_VADR;
        public uint REPT_CNT;
        public string RSLT_TXT;
        public ushort RTN_ICNT;
        public ushort[] RTN_INDX;
        public byte[] RTN_STAT;
        public bool Sampled;
        public byte SITE_NUM;
        public BitArray SPIN_MAP;
        public byte TEST_FLG;
        public uint TEST_NUM;
        public string TEST_TXT;
        public string TIME_SET;
        public string VECT_NAM;
        public short VECT_OFF;
        public int XFAIL_AD;
        public int YFAIL_AD;

        public FTRRecord() : base(STDFRecordType.FTR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.TEST_NUM);
            num += base.Read(br, out this.HEAD_NUM);
            num += base.Read(br, out this.SITE_NUM);
            num += base.Read(br, out this.TEST_FLG);
            num += base.Read(br, out this.OPT_FLAG);
            num += base.Read(br, out this.CYCL_CNT);
            num += base.Read(br, out this.REL_VADR);
            num += base.Read(br, out this.REPT_CNT);
            num += base.Read(br, out this.NUM_FAIL);
            num += base.Read(br, out this.XFAIL_AD);
            num += base.Read(br, out this.YFAIL_AD);
            num += base.Read(br, out this.VECT_OFF);
            num += base.Read(br, out this.RTN_ICNT);
            num += base.Read(br, out this.PGM_ICNT);
            num += base.Read(br, out this.VECT_NAM);
            num += base.Read(br, out this.TIME_SET);
            num += base.Read(br, out this.OP_CODE);
            num += base.Read(br, out this.TEST_TXT);
            num += base.Read(br, out this.ALARM_ID);
            num += base.Read(br, out this.PROG_TXT);
            num += base.Read(br, out this.RSLT_TXT);
            return (num + base.Read(br, out this.PATG_NUM));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("FTR (Functional Test Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-35}{1}\n", "Test number:", this.TEST_NUM);
            builder.AppendFormat("\t{0,-35}{1}\n", "Test head number:", this.HEAD_NUM);
            builder.AppendFormat("\t{0,-35}{1}\n", "Test site number:", this.SITE_NUM);
            builder.AppendFormat("\t{0,-35}{1}\n", "Test flags (fail,alarm,etc):", this.TEST_FLG);
            builder.AppendFormat("\t{0,-35}{1}\n", "Optional data flag:", this.OPT_FLAG);
            builder.AppendFormat("\t{0,-35}{1}\n", "Cycle count of vector:", this.CYCL_CNT);
            builder.AppendFormat("\t{0,-35}{1}\n", "Relative vector address:", this.REL_VADR);
            builder.AppendFormat("\t{0,-35}{1}\n", "Repeat count of vector:", this.REPT_CNT);
            builder.AppendFormat("\t{0,-35}{1}\n", "# of Pins with 1 or more failures:", this.NUM_FAIL);
            builder.AppendFormat("\t{0,-35}{1}\n", "X logical device failure address:", this.XFAIL_AD);
            builder.AppendFormat("\t{0,-35}{1}\n", "Y logical device failure address:", this.YFAIL_AD);
            builder.AppendFormat("\t{0,-35}{1}\n", "Offset from vector of interest:", this.VECT_OFF);
            builder.AppendFormat("\t{0,-35}{1}\n", "Count of return data PMR indexes (j):", this.RTN_ICNT);
            builder.AppendFormat("\t{0,-35}{1}\n", "Count of programmed state indexes (k):", this.PGM_ICNT);
            builder.AppendFormat("\t{0,-35}{1}\n", "Vector moudle pattern name:", this.VECT_NAM);
            builder.AppendFormat("\t{0,-35}{1}\n", "Time set name:", this.TIME_SET);
            builder.AppendFormat("\t{0,-35}{1}\n", "Vector Op Code:", this.OP_CODE);
            builder.AppendFormat("\t{0,-35}{1}\n", "Descriptive text/label:", this.TEST_TXT);
            builder.AppendFormat("\t{0,-35}{1}\n", "Name of alarm:", this.ALARM_ID);
            builder.AppendFormat("\t{0,-35}{1}\n", "Additional programmed information:", this.PROG_TXT);
            builder.AppendFormat("\t{0,-35}{1}\n", "Additional result information:", this.RSLT_TXT);
            builder.AppendFormat("\t{0,-35}{1}\n", "Pattern generator number:", this.PATG_NUM);
            builder.AppendFormat("\t{0,-35}{1}\n", "Bit map of enabled comparators:", this.SPIN_MAP);
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            if (this.RTN_ICNT != this.RTN_INDX.Length)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("FTR({0}) Record data inconsistency:\n", base.RecordNumber);
                builder.AppendFormat("\tRTN_ICNT must match the size of array RTN_INDX\n", new object[0]);
                builder.AppendFormat("\tRNT_ICNT = {0}, size of RTN_INDX is {1}\n", this.RTN_ICNT.ToString(), this.RTN_INDX.Length.ToString());
                throw new Exception(builder.ToString());
            }
            if (this.RTN_ICNT != this.RTN_STAT.Length)
            {
                StringBuilder builder2 = new StringBuilder();
                builder2.AppendFormat("FTR({0}) Record data inconsistency:\n", base.RecordNumber);
                builder2.AppendFormat("\tRTN_ICNT must match the size of array RTN_STAT\n", new object[0]);
                builder2.AppendFormat("\tRNT_ICNT = {0}, size of RTN_STAT is {1}\n", this.RTN_ICNT.ToString(), this.RTN_STAT.Length.ToString());
                throw new Exception(builder2.ToString());
            }
            if (this.PGM_ICNT != this.PGM_INDX.Length)
            {
                StringBuilder builder3 = new StringBuilder();
                builder3.AppendFormat("FTR({0}) Record data inconsistency:\n", base.RecordNumber);
                builder3.AppendFormat("\tPGM_ICNT must match the size of array PGM_INDX\n", new object[0]);
                builder3.AppendFormat("\tPGM_ICNT = {0}, size of PGM_INDX is {1}\n", this.PGM_ICNT.ToString(), this.PGM_INDX.Length.ToString());
                throw new Exception(builder3.ToString());
            }
            if (this.PGM_ICNT != this.PGM_STAT.Length)
            {
                StringBuilder builder4 = new StringBuilder();
                builder4.AppendFormat("FTR({0}) Record data inconsistency:\n", base.RecordNumber);
                builder4.AppendFormat("\tPGM_ICNT must match the size of array PGM_STAT\n", new object[0]);
                builder4.AppendFormat("\tPGM_ICNT = {0}, size of PGM_STAT is {1}\n", this.PGM_ICNT.ToString(), this.PGM_STAT.Length.ToString());
                throw new Exception(builder4.ToString());
            }
            bw.Write(Convert.ToUInt16(STDFRecordType.FTR));
            bw.Write(this.TEST_NUM);
            bw.Write(this.HEAD_NUM);
            bw.Write(this.SITE_NUM);
            bw.Write(this.TEST_FLG);
            bw.Write(this.OPT_FLAG);
            bw.Write(this.CYCL_CNT);
            bw.Write(this.REL_VADR);
            bw.Write(this.REPT_CNT);
            bw.Write(this.NUM_FAIL);
            bw.Write(this.XFAIL_AD);
            bw.Write(this.YFAIL_AD);
            bw.Write(this.VECT_OFF);
            bw.Write(this.RTN_ICNT);
            bw.Write(this.PGM_ICNT);
            base.WriteArray(bw, this.RTN_INDX);
            base.WriteArrayNibble(bw, this.RTN_STAT);
            base.WriteArray(bw, this.PGM_INDX);
            base.WriteArrayNibble(bw, this.PGM_STAT);
            base.WriteArray(bw, this.FAIL_PIN);
            base.WriteString(bw, this.VECT_NAM);
            base.WriteString(bw, this.TIME_SET);
            base.WriteString(bw, this.OP_CODE);
            base.WriteString(bw, this.TEST_TXT);
            base.WriteString(bw, this.ALARM_ID);
            base.WriteString(bw, this.PROG_TXT);
            base.WriteString(bw, this.RSLT_TXT);
            bw.Write(this.PATG_NUM);
            base.WriteArray(bw, this.SPIN_MAP);
            return 0;
        }
    }
}

