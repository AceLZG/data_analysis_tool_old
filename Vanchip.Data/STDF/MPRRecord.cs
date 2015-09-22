namespace STDF_V4
{
    using System;
    using System.IO;
    using System.Text;

    public class MPRRecord : STDFRecord
    {
        public string ALARM_ID;
        public string C_HLMFMT;
        public string C_LLMFMT;
        public string C_RESFMT;
        public byte HEAD_NUM;
        public float HI_LIMIT;
        public float HI_SPEC;
        public sbyte HLM_SCAL;
        public float INCR_IN;
        public sbyte LLM_SCAL;
        public float LO_LIMIT;
        public float LO_SPEC;
        public byte OPT_FLAG;
        public byte PARM_FLG;
        public sbyte RES_SCAL;
        public ushort RSLT_CNT;
        public ushort RTN_ICNT;
        public ushort[] RTN_INDX;
        public float[] RTN_RSLT;
        public byte[] RTN_STAT;
        public bool Sampled;
        public byte SITE_NUM;
        public float START_IN;
        public byte TEST_FLG;
        public uint TEST_NUM;
        public string TEST_TXT;
        public string UNITS;
        public string UNITS_IN;

        public MPRRecord() : base(STDFRecordType.MPR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.TEST_NUM);
            num += base.Read(br, out this.HEAD_NUM);
            num += base.Read(br, out this.SITE_NUM);
            num += base.Read(br, out this.TEST_FLG);
            num += base.Read(br, out this.PARM_FLG);
            num += base.Read(br, out this.RTN_ICNT);
            num += base.Read(br, out this.RSLT_CNT);
            num += base.ReadArrayNibble(br, this.RTN_ICNT, out this.RTN_STAT);
            num += base.ReadArray(br, this.RSLT_CNT, out this.RTN_RSLT);
            num += base.Read(br, out this.TEST_TXT);
            num += base.Read(br, out this.ALARM_ID);
            num += base.Read(br, out this.OPT_FLAG);
            num += base.Read(br, out this.RES_SCAL);
            num += base.Read(br, out this.LLM_SCAL);
            num += base.Read(br, out this.HLM_SCAL);
            num += base.Read(br, out this.LO_LIMIT);
            num += base.Read(br, out this.HI_LIMIT);
            num += base.Read(br, out this.START_IN);
            num += base.Read(br, out this.INCR_IN);
            num += base.Read(br, this.RTN_ICNT, out this.RTN_INDX);
            num += base.Read(br, out this.UNITS);
            num += base.Read(br, out this.UNITS_IN);
            num += base.Read(br, out this.C_RESFMT);
            num += base.Read(br, out this.C_LLMFMT);
            num += base.Read(br, out this.C_HLMFMT);
            num += base.Read(br, out this.LO_SPEC);
            return (num + base.Read(br, out this.HI_SPEC));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("MPR (Multiple Result Parametric Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-36}{1}\n", "Test number:", this.TEST_NUM);
            builder.AppendFormat("\t{0,-36}{1}\n", "Test head number:", this.HEAD_NUM);
            builder.AppendFormat("\t{0,-36}{1}\n", "Test site number:", this.SITE_NUM);
            builder.AppendFormat("\t{0,-36}{1}\n", "Test flags (fail,alarm,etc):", this.TEST_FLG);
            builder.AppendFormat("\t{0,-36}{1}\n", "Parametric test flags:", this.PARM_FLG);
            builder.AppendFormat("\t{0,-36}{1}\n", "Count of PMR indexes:", this.RTN_ICNT);
            builder.AppendFormat("\t{0,-36}{1}\n", "Count of returned results:", this.RSLT_CNT);
            if (this.RTN_STAT.Length > 0)
            {
                builder.AppendFormat("\n\t{0,-36}\n", "Array of PMR Indexes");
                builder.AppendFormat("\t\t{0}\t{1,-20}{2,-20}\n", "Idx", "Returned", "PMR");
                builder.AppendFormat("\t\t{0}\t{1,-20}{2,-20}\n", "", "States", "Indexes");
                for (int i = 0; i < this.RTN_STAT.Length; i++)
                {
                    builder.AppendFormat("\t\t[{0}]\t{1,-20}{2,-20}\n", i, this.RTN_STAT[i].ToString(), this.RTN_INDX[i].ToString());
                }
            }
            if (this.RTN_RSLT.Length > 0)
            {
                builder.AppendFormat("\t{0,-36}\n", "Array of returned results");
                builder.AppendFormat("\t\t{0}\t{1,-20}\n", "Idx", "Result");
                for (int j = 0; j < this.RTN_RSLT.Length; j++)
                {
                    builder.AppendFormat("\t\t[{0}]\t{1,-20}\n", j, this.RTN_RSLT[j].ToString());
                }
                builder.AppendFormat("\n", new object[0]);
            }
            builder.AppendFormat("\t{0,-36}{1}\n", "Descriptive text/label:", this.TEST_TXT);
            builder.AppendFormat("\t{0,-36}{1}\n", "Name of Alarm:", this.ALARM_ID);
            builder.AppendFormat("\t{0,-36}{1}\n", "Optional data flag:", this.OPT_FLAG);
            builder.AppendFormat("\t{0,-36}{1}\n", "Test result scaling exponent:", this.RES_SCAL);
            builder.AppendFormat("\t{0,-36}{1}\n", "Test low limit scacling exponent:n", this.LLM_SCAL);
            builder.AppendFormat("\t{0,-36}{1}\n", "Test high limit scacling exponent:", this.HLM_SCAL);
            builder.AppendFormat("\t{0,-36}{1}\n", "Test Low limit value:", this.LO_LIMIT);
            builder.AppendFormat("\t{0,-36}{1}\n", "Test High limit value:", this.HI_LIMIT);
            builder.AppendFormat("\t{0,-36}{1}\n", "Starting input value (condition):", this.START_IN);
            builder.AppendFormat("\t{0,-36}{1}\n", "Increment of input condition:", this.INCR_IN);
            builder.AppendFormat("\t{0,-36}{1}\n", "Units of returned results:", this.UNITS);
            builder.AppendFormat("\t{0,-36}{1}\n", "Input condition units:", this.UNITS_IN);
            builder.AppendFormat("\t{0,-36}{1}\n", "C Result format string:", this.C_RESFMT);
            builder.AppendFormat("\t{0,-36}{1}\n", "C Low limit format string:", this.C_LLMFMT);
            builder.AppendFormat("\t{0,-36}{1}\n", "C High limit format string:", this.C_HLMFMT);
            builder.AppendFormat("\t{0,-36}{1}\n", "Low specification limit value:", this.LO_SPEC);
            builder.AppendFormat("\t{0,-36}{1}\n", "High specification limit value:", this.HI_SPEC);
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            if (this.RTN_ICNT != this.RTN_STAT.Length)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("MPR({0}) Record data inconsistency:\n", base.RecordNumber);
                builder.AppendFormat("\tRTN_ICNT must match the size of array RTN_STAT\n", new object[0]);
                builder.AppendFormat("\tRNT_ICNT = {0}, size of RTN_STAT is {1}\n", this.RTN_ICNT.ToString(), this.RTN_STAT.Length.ToString());
                throw new Exception(builder.ToString());
            }
            if (this.RSLT_CNT != this.RTN_RSLT.Length)
            {
                StringBuilder builder2 = new StringBuilder();
                builder2.AppendFormat("MPR({0}) Record data inconsistency:\n", base.RecordNumber);
                builder2.AppendFormat("\tRSLT_CNT must match the size of array RTN_RSLT\n", new object[0]);
                builder2.AppendFormat("\tRSLT_CNT = {0}, size of RTN_RSLT is {1}\n", this.RSLT_CNT.ToString(), this.RTN_RSLT.Length.ToString());
                throw new Exception(builder2.ToString());
            }
            if (this.RTN_ICNT != this.RTN_INDX.Length)
            {
                StringBuilder builder3 = new StringBuilder();
                builder3.AppendFormat("MPR({0}) Record data inconsistency:\n", base.RecordNumber);
                builder3.AppendFormat("\tRTN_ICNT must match the size of array RTN_INDX\n", new object[0]);
                builder3.AppendFormat("\tRTN_ICNT = {0}, size of RTN_RSLT is {1}\n", this.RTN_ICNT.ToString(), this.RTN_INDX.Length.ToString());
                throw new Exception(builder3.ToString());
            }
            bw.Write(Convert.ToUInt16(STDFRecordType.MPR));
            bw.Write(this.TEST_NUM);
            bw.Write(this.HEAD_NUM);
            bw.Write(this.SITE_NUM);
            bw.Write(this.TEST_FLG);
            bw.Write(this.PARM_FLG);
            bw.Write(this.RTN_ICNT);
            bw.Write(this.RSLT_CNT);
            base.WriteArrayNibble(bw, this.RTN_STAT);
            base.WriteArray(bw, this.RTN_RSLT);
            base.WriteString(bw, this.TEST_TXT);
            base.WriteString(bw, this.ALARM_ID);
            bw.Write(this.OPT_FLAG);
            bw.Write(this.RES_SCAL);
            bw.Write(this.LLM_SCAL);
            bw.Write(this.HLM_SCAL);
            bw.Write(this.LO_LIMIT);
            bw.Write(this.HI_LIMIT);
            bw.Write(this.START_IN);
            bw.Write(this.INCR_IN);
            base.WriteArray(bw, this.RTN_INDX);
            base.WriteString(bw, this.UNITS);
            base.WriteString(bw, this.UNITS_IN);
            base.WriteString(bw, this.C_RESFMT);
            base.WriteString(bw, this.C_LLMFMT);
            base.WriteString(bw, this.C_HLMFMT);
            bw.Write(this.LO_SPEC);
            bw.Write(this.HI_SPEC);
            return 0;
        }
    }
}

