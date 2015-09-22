namespace STDF_V4
{
    using System;
    using System.IO;
    using System.Text;

    public class TSRRecord : STDFRecord
    {
        public uint ALRM_CNT;
        public uint EXEC_CNT;
        public uint FAIL_CNT;
        public byte HEAD_NUM;
        public byte OPT_FLAG;
        public string SEQ_NAME;
        public byte SITE_NUM;
        public string TEST_LBL;
        public float TEST_MAX;
        public float TEST_MIN;
        public string TEST_NAM;
        public uint TEST_NUM;
        public float TEST_TIM;
        public byte TEST_TYP;
        public float TST_SQRS;
        public float TST_SUMS;

        public TSRRecord() : base(STDFRecordType.TSR)
        {
            this.OPT_FLAG = 0;
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.HEAD_NUM);
            num += base.Read(br, out this.SITE_NUM);
            num += base.Read(br, out this.TEST_TYP);
            num += base.Read(br, out this.TEST_NUM);
            num += base.Read(br, out this.EXEC_CNT);
            num += base.Read(br, out this.FAIL_CNT);
            num += base.Read(br, out this.ALRM_CNT);
            num += base.Read(br, out this.TEST_NAM);
            num += base.Read(br, out this.SEQ_NAME);
            num += base.Read(br, out this.TEST_LBL);
            num += base.Read(br, out this.OPT_FLAG);
            num += base.Read(br, out this.TEST_TIM);
            num += base.Read(br, out this.TEST_MIN);
            num += base.Read(br, out this.TEST_MAX);
            num += base.Read(br, out this.TST_SUMS);
            return (num + base.Read(br, out this.TST_SQRS));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("TSR (Test Synopsis Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-39}{1}\n", "Test head number:", this.HEAD_NUM);
            builder.AppendFormat("\t{0,-39}{1}\n", "Test site number:", this.SITE_NUM);
            builder.AppendFormat("\t{0,-39}{1}\n", "Test type:", this.TEST_TYP);
            builder.AppendFormat("\t{0,-39}{1}\n", "Test number:", this.TEST_NUM);
            builder.AppendFormat("\t{0,-39}{1}\n", "Number of test executions:", this.EXEC_CNT);
            builder.AppendFormat("\t{0,-39}{1}\n", "Number of test failures:", this.FAIL_CNT);
            builder.AppendFormat("\t{0,-39}{1}\n", "Number of alarmed tests:", this.ALRM_CNT);
            builder.AppendFormat("\t{0,-39}{1}\n", "Test name:", this.TEST_NAM);
            builder.AppendFormat("\t{0,-39}{1}\n", "Sequence (program segment/flow) name:", this.SEQ_NAME);
            builder.AppendFormat("\t{0,-39}{1}\n", "Test label or text:", this.TEST_LBL);
            builder.AppendFormat("\t{0,-39}0x{1:X}\n", "Optional data flag:", this.OPT_FLAG);
            builder.AppendFormat("\t\t{0,-35}{1}\n", "B0: TEST_MIN is invalid:", this.b0_TEST_MIN_invalid);
            builder.AppendFormat("\t\t{0,-35}{1}\n", "B1: TEST_MAX is invalid:", this.b1_TEST_MAX_invalid);
            builder.AppendFormat("\t\t{0,-35}{1}\n", "B2: TEST_TIM is invalid:", this.b2_TEST_TIM_invalid);
            builder.AppendFormat("\t\t{0,-35}{1}\n", "B3: Reserved must be ture:", this.b3_Reserved);
            builder.AppendFormat("\t\t{0,-35}{1}\n", "B4: TST_SUMS is invalid:", this.b4_TEST_SUMS_invalid);
            builder.AppendFormat("\t\t{0,-35}{1}\n", "B5: TST_SQRS is invalid:", this.b5_TEST_SQRS_invalid);
            builder.AppendFormat("\t\t{0,-35}{1}\n", "B6: Reserved must be true:", this.b6_Reserved);
            builder.AppendFormat("\t\t{0,-35}{1}\n", "B7: Reserved must be true:", this.b7_Reserved);
            builder.AppendFormat("\t{0,-39}{1}\n", "Average test execution time (seconds):", this.TEST_TIM);
            builder.AppendFormat("\t{0,-39}{1}\n", "Lowest test result value:", this.TEST_MIN);
            builder.AppendFormat("\t{0,-39}{1}\n", "Highest test result value:", this.TEST_MAX);
            builder.AppendFormat("\t{0,-39}{1}\n", "Sum of test result values:", this.TST_SUMS);
            builder.AppendFormat("\t{0,-39}{1}\n", "Sum of squares of test result values:", this.TST_SQRS);
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.TSR));
            bw.Write(this.HEAD_NUM);
            bw.Write(this.SITE_NUM);
            bw.Write(this.TEST_TYP);
            bw.Write(this.TEST_NUM);
            bw.Write(this.EXEC_CNT);
            bw.Write(this.FAIL_CNT);
            bw.Write(this.ALRM_CNT);
            base.WriteString(bw, this.TEST_NAM);
            base.WriteString(bw, this.SEQ_NAME);
            base.WriteString(bw, this.TEST_LBL);
            uint num = (uint)~this.OPT_FLAG;
            num &= 0xff;
            byte b = Convert.ToByte(num);
            if (b != 0)
            {
                bw.Write(this.OPT_FLAG);
                bw.Write(this.TEST_TIM);
                b = STDFManager.SetBit(b, 2, false);
                if (b == 0)
                {
                    return 0;
                }
                bw.Write(this.TEST_MIN);
                b = STDFManager.SetBit(b, 0, false);
                if (b == 0)
                {
                    return 0;
                }
                bw.Write(this.TEST_MAX);
                b = STDFManager.SetBit(b, 1, false);
                if (b == 0)
                {
                    return 0;
                }
                bw.Write(this.TST_SUMS);
                if (STDFManager.SetBit(b, 4, false) == 0)
                {
                    return 0;
                }
                bw.Write(this.TST_SQRS);
            }
            return 0;
        }

        public bool b0_TEST_MIN_invalid
        {
            get
            {
                return STDFManager.GetBit(this.OPT_FLAG, 0);
            }
            set
            {
                this.OPT_FLAG = STDFManager.SetBit(this.OPT_FLAG, 0, value);
            }
        }

        public bool b1_TEST_MAX_invalid
        {
            get
            {
                return STDFManager.GetBit(this.OPT_FLAG, 1);
            }
            set
            {
                this.OPT_FLAG = STDFManager.SetBit(this.OPT_FLAG, 1, value);
            }
        }

        public bool b2_TEST_TIM_invalid
        {
            get
            {
                return STDFManager.GetBit(this.OPT_FLAG, 2);
            }
            set
            {
                this.OPT_FLAG = STDFManager.SetBit(this.OPT_FLAG, 2, value);
            }
        }

        public bool b3_Reserved
        {
            get
            {
                return STDFManager.GetBit(this.OPT_FLAG, 3);
            }
            set
            {
                this.OPT_FLAG = STDFManager.SetBit(this.OPT_FLAG, 3, value);
            }
        }

        public bool b4_TEST_SUMS_invalid
        {
            get
            {
                return STDFManager.GetBit(this.OPT_FLAG, 4);
            }
            set
            {
                this.OPT_FLAG = STDFManager.SetBit(this.OPT_FLAG, 4, value);
            }
        }

        public bool b5_TEST_SQRS_invalid
        {
            get
            {
                return STDFManager.GetBit(this.OPT_FLAG, 5);
            }
            set
            {
                this.OPT_FLAG = STDFManager.SetBit(this.OPT_FLAG, 5, value);
            }
        }

        public bool b6_Reserved
        {
            get
            {
                return STDFManager.GetBit(this.OPT_FLAG, 6);
            }
            set
            {
                this.OPT_FLAG = STDFManager.SetBit(this.OPT_FLAG, 6, value);
            }
        }

        public bool b7_Reserved
        {
            get
            {
                return STDFManager.GetBit(this.OPT_FLAG, 7);
            }
            set
            {
                this.OPT_FLAG = STDFManager.SetBit(this.OPT_FLAG, 7, value);
            }
        }
    }
}

