namespace STDF_V4
{
    using System;
    using System.IO;
    using System.Text;

    public class PTRRecord : STDFRecord
    {
        public string ALARM_ID;
        public string C_HLMFMT;
        public string C_LLMFMT;
        public string C_RESFMT;
        public byte HEAD_NUM;
        public float HI_LIMIT;
        public float HI_SPEC;
        public sbyte HLM_SCAL;
        public sbyte LLM_SCAL;
        public float LO_LIMIT;
        public float LO_SPEC;
        public byte OPT_FLAG;
        public byte PARM_FLG;
        public sbyte RES_SCAL;
        public float RESULT;
        public byte SITE_NUM;
        public byte TEST_FLG;
        public uint TEST_NUM;
        public string TEST_TXT;
        public string UNITS;

        public PTRRecord() : base(STDFRecordType.PTR)
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
            num += base.Read(br, out this.RESULT);
            num += base.Read(br, out this.TEST_TXT);
            num += base.Read(br, out this.ALARM_ID);
            num += base.Read(br, out this.OPT_FLAG);
            num += base.Read(br, out this.RES_SCAL);
            num += base.Read(br, out this.LLM_SCAL);
            num += base.Read(br, out this.HLM_SCAL);
            num += base.Read(br, out this.LO_LIMIT);
            num += base.Read(br, out this.HI_LIMIT);
            num += base.Read(br, out this.UNITS);
            num += base.Read(br, out this.C_RESFMT);
            num += base.Read(br, out this.C_LLMFMT);
            num += base.Read(br, out this.C_HLMFMT);
            num += base.Read(br, out this.LO_SPEC);
            return (num + base.Read(br, out this.HI_SPEC));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0xffc);
            builder.AppendFormat("PTR (Parametric Test Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-51}{1}\n", "Test number:", this.TEST_NUM);
            builder.AppendFormat("\t{0,-51}{1}\n", "Test head number:", this.HEAD_NUM);
            builder.AppendFormat("\t{0,-51}{1}\n", "Test site number:", this.SITE_NUM);
            builder.AppendFormat("\t{0,-51}{1:X}\n", "Test Flags (fail,alarm, etc):", this.TEST_FLG);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B0-Alarm Detected:", this.b0_TEST_FLG_alarmed);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B1-Result Field not valid:", this.b1_TEST_FLG_result_invalid);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B2-Test Result is unreliable:", this.b2_TEST_FLG_result_unreliable);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B3-Timeout Occured:", this.b3_TEST_FLG_timeout);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B4-Test not executed:", this.b4_TEST_FLG_test_not_executed);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B5-Test aborted:", this.b5_TEST_FLG_test_aborted);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B6-Test completed,NO Pass Fail indication:", this.b6_TEST_FLG_no_pass_fail);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B7-Test Failed:", this.b7_TEST_FLG_test_passed);
            builder.AppendFormat("\t{0,-51}{1:X}\n", "Parametric test flags (drift, etc):", this.PARM_FLG);
            builder.AppendFormat("\t{0,-50}{1}\n", "Result Valid", this.RESULT_VALID);
            builder.AppendFormat("\t{0,-64}\n", "(combination of bits TEST_FLG:b0-b6+PARM_FLG:b0-b2)");
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B0-Scale Error:", this.b0_PARM_FLG_scale_error);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B1-Drift Error:", this.b1_PARM_FLG_drift_error);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B2-Oscillation detected:", this.b2_PARM_FLG_oscillation_detected);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B3-Results higher than limit:", this.b3_PARM_FLG_value_higher);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B4-Results Lower than limit:", this.b4_PARM_FLG_value_lower);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B5-Test Passed Alternate limits:", this.b5_PARM_FLG_passed_alternate);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B6-Result = Low Limit Pass:", this.b6_PARM_FLG_low_limit_pass);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B7-Result = High Limit Pass", this.b7_PARM_FLG_high_limit_pass);
            builder.AppendFormat("\t{0,-51}{1}\n", "Test result:", this.RESULT);
            builder.AppendFormat("\t{0,-51}{1}\n", "Test description text or label:", this.TEST_TXT);
            builder.AppendFormat("\t{0,-51}{1}\n", "Name of alarm:", this.ALARM_ID);
            builder.AppendFormat("\t{0,-51}0x{1:X}\n", "Optional data flag:", this.OPT_FLAG);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B0-RES_SCAL Invalid:", this.b0_OPT_FLAG_res_scale_Invalid);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B1-Reserved must be 1:", this.b1_OPT_FLAG_Reserved);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B2-No Low Spec Limit:", this.b2_OPT_FLAG_no_low_spec_limit);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B3-No High Spec Limit:", this.b3_OPT_FLAG_no_high_spec_limit);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B4-LO_LIMIT and LLM_SCAL are invalid:", this.b4_OPT_FLAG_lo_limit_invalid);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B5-HI_LIMIT and HLM_SCAL are invalid:", this.b5_OPT_FLAG_high_limit_invalid);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B6-No Low Limit for this test:", this.b6_OPT_FLAG_no_low_limit);
            builder.AppendFormat("\t\t{0,-43}{1}\n", "B7-No High Limit for this test:", this.b7_OPT_FLAG_no_high_limit);
            builder.AppendFormat("\t{0,-51}{1}\n", "Test results scaling exponent:", this.RES_SCAL);
            builder.AppendFormat("\t{0,-51}{1}\n", "Low limit scaling exponent:", this.LLM_SCAL);
            builder.AppendFormat("\t{0,-51}{1}\n", "High limit scaling exponent:", this.HLM_SCAL);
            builder.AppendFormat("\t{0,-51}{1}\n", "Low test limit value:", this.LO_LIMIT);
            builder.AppendFormat("\t{0,-51}{1}\n", "High test limit value:", this.HI_LIMIT);
            builder.AppendFormat("\t{0,-51}{1}\n", "Test units:", this.UNITS);
            builder.AppendFormat("\t{0,-51}{1}\n", "C result format string:", this.C_RESFMT);
            builder.AppendFormat("\t{0,-51}{1}\n", "C low limit format string:", this.C_LLMFMT);
            builder.AppendFormat("\t{0,-51}{1}\n", "C high limit format string:", this.C_HLMFMT);
            builder.AppendFormat("\t{0,-51}{1}\n", "Low Specificaiton limit Value:", this.LO_SPEC);
            builder.AppendFormat("\t{0,-51}{1}\n", "High Specification limit value:", this.HI_SPEC);
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.PTR));
            bw.Write(this.TEST_NUM);
            bw.Write(this.HEAD_NUM);
            bw.Write(this.SITE_NUM);
            bw.Write(this.TEST_FLG);
            bw.Write(this.PARM_FLG);
            bw.Write(this.RESULT);
            base.WriteString(bw, this.TEST_TXT);
            base.WriteString(bw, this.ALARM_ID);
            byte b = (byte)~this.OPT_FLAG;
            if (STDFManager.GetBit(b, 4))
            {
                b = STDFManager.SetBit(b, 6, true);
            }
            if (STDFManager.GetBit(b, 6))
            {
                b = STDFManager.SetBit(b, 4, true);
            }
            if (STDFManager.GetBit(b, 5))
            {
                b = STDFManager.SetBit(b, 7, true);
            }
            if (STDFManager.GetBit(b, 7))
            {
                b = STDFManager.SetBit(b, 5, true);
            }
            byte num2 = 0;
            if (this.UNITS.Length != 0)
            {
                num2 = STDFManager.SetBit(num2, 0, true);
            }
            if (this.C_RESFMT.Length != 0)
            {
                num2 = STDFManager.SetBit(num2, 1, true);
            }
            if (this.C_LLMFMT.Length != 0)
            {
                num2 = STDFManager.SetBit(num2, 2, true);
            }
            if (this.C_HLMFMT.Length != 0)
            {
                num2 = STDFManager.SetBit(num2, 3, true);
            }
            bw.Write(this.OPT_FLAG);
            if ((b != 0) || (num2 != 0))
            {
                bw.Write(this.RES_SCAL);
                b = STDFManager.SetBit(b, 0, false);
                if ((b == 0) && (num2 == 0))
                {
                    return 0;
                }
                bw.Write(this.LLM_SCAL);
                b = STDFManager.SetBit(b, 4, false);
                if ((b == 0) && (num2 == 0))
                {
                    return 0;
                }
                bw.Write(this.HLM_SCAL);
                b = STDFManager.SetBit(b, 5, false);
                if ((b == 0) && (num2 == 0))
                {
                    return 0;
                }
                bw.Write(this.LO_LIMIT);
                b = STDFManager.SetBit(b, 6, false);
                if ((b == 0) && (num2 == 0))
                {
                    return 0;
                }
                bw.Write(this.HI_LIMIT);
                b = STDFManager.SetBit(b, 7, false);
                if ((b == 0) && (num2 == 0))
                {
                    return 0;
                }
                base.WriteString(bw, this.UNITS);
                num2 = STDFManager.SetBit(num2, 0, false);
                if ((b == 0) && (num2 == 0))
                {
                    return 0;
                }
                base.WriteString(bw, this.C_RESFMT);
                num2 = STDFManager.SetBit(num2, 1, false);
                if ((b == 0) && (num2 == 0))
                {
                    return 0;
                }
                base.WriteString(bw, this.C_LLMFMT);
                num2 = STDFManager.SetBit(num2, 2, false);
                if ((b == 0) && (num2 == 0))
                {
                    return 0;
                }
                base.WriteString(bw, this.C_HLMFMT);
                num2 = STDFManager.SetBit(num2, 3, false);
                if ((b == 0) && (num2 == 0))
                {
                    return 0;
                }
                bw.Write(this.LO_SPEC);
                if (STDFManager.SetBit(b, 2, false) == 0)
                {
                    return 0;
                }
                bw.Write(this.HI_SPEC);
            }
            return 0;
        }

        public bool b0_OPT_FLAG_res_scale_Invalid
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

        public bool b0_PARM_FLG_scale_error
        {
            get
            {
                return STDFManager.GetBit(this.PARM_FLG, 0);
            }
            set
            {
                this.PARM_FLG = STDFManager.SetBit(this.PARM_FLG, 0, value);
            }
        }

        public bool b0_TEST_FLG_alarmed
        {
            get
            {
                return STDFManager.GetBit(this.TEST_FLG, 0);
            }
            set
            {
                this.TEST_FLG = STDFManager.SetBit(this.TEST_FLG, 0, value);
            }
        }

        public bool b1_OPT_FLAG_Reserved
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

        public bool b1_PARM_FLG_drift_error
        {
            get
            {
                return STDFManager.GetBit(this.PARM_FLG, 1);
            }
            set
            {
                this.PARM_FLG = STDFManager.SetBit(this.PARM_FLG, 1, value);
            }
        }

        public bool b1_TEST_FLG_result_invalid
        {
            get
            {
                return STDFManager.GetBit(this.TEST_FLG, 1);
            }
            set
            {
                this.TEST_FLG = STDFManager.SetBit(this.TEST_FLG, 1, value);
            }
        }

        public bool b2_OPT_FLAG_no_low_spec_limit
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

        public bool b2_PARM_FLG_oscillation_detected
        {
            get
            {
                return STDFManager.GetBit(this.PARM_FLG, 2);
            }
            set
            {
                this.PARM_FLG = STDFManager.SetBit(this.PARM_FLG, 2, value);
            }
        }

        public bool b2_TEST_FLG_result_unreliable
        {
            get
            {
                return STDFManager.GetBit(this.TEST_FLG, 2);
            }
            set
            {
                this.TEST_FLG = STDFManager.SetBit(this.TEST_FLG, 2, value);
            }
        }

        public bool b3_OPT_FLAG_no_high_spec_limit
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

        public bool b3_PARM_FLG_value_higher
        {
            get
            {
                return STDFManager.GetBit(this.PARM_FLG, 3);
            }
            set
            {
                this.PARM_FLG = STDFManager.SetBit(this.PARM_FLG, 3, value);
            }
        }

        public bool b3_TEST_FLG_timeout
        {
            get
            {
                return STDFManager.GetBit(this.TEST_FLG, 3);
            }
            set
            {
                this.TEST_FLG = STDFManager.SetBit(this.TEST_FLG, 3, value);
            }
        }

        public bool b4_OPT_FLAG_lo_limit_invalid
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

        public bool b4_PARM_FLG_value_lower
        {
            get
            {
                return STDFManager.GetBit(this.PARM_FLG, 4);
            }
            set
            {
                this.PARM_FLG = STDFManager.SetBit(this.PARM_FLG, 4, value);
            }
        }

        public bool b4_TEST_FLG_test_not_executed
        {
            get
            {
                return STDFManager.GetBit(this.TEST_FLG, 4);
            }
            set
            {
                this.TEST_FLG = STDFManager.SetBit(this.TEST_FLG, 4, value);
            }
        }

        public bool b5_OPT_FLAG_high_limit_invalid
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

        public bool b5_PARM_FLG_passed_alternate
        {
            get
            {
                return STDFManager.GetBit(this.PARM_FLG, 5);
            }
            set
            {
                this.PARM_FLG = STDFManager.SetBit(this.PARM_FLG, 5, value);
            }
        }

        public bool b5_TEST_FLG_test_aborted
        {
            get
            {
                return STDFManager.GetBit(this.TEST_FLG, 5);
            }
            set
            {
                this.TEST_FLG = STDFManager.SetBit(this.TEST_FLG, 5, value);
            }
        }

        public bool b6_OPT_FLAG_no_low_limit
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

        public bool b6_PARM_FLG_low_limit_pass
        {
            get
            {
                return STDFManager.GetBit(this.PARM_FLG, 6);
            }
            set
            {
                this.PARM_FLG = STDFManager.SetBit(this.PARM_FLG, 6, value);
            }
        }

        public bool b6_TEST_FLG_no_pass_fail
        {
            get
            {
                return STDFManager.GetBit(this.TEST_FLG, 6);
            }
            set
            {
                this.TEST_FLG = STDFManager.SetBit(this.TEST_FLG, 6, value);
            }
        }

        public bool b7_OPT_FLAG_no_high_limit
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

        public bool b7_PARM_FLG_high_limit_pass
        {
            get
            {
                return STDFManager.GetBit(this.PARM_FLG, 7);
            }
            set
            {
                this.PARM_FLG = STDFManager.SetBit(this.PARM_FLG, 7, value);
            }
        }

        public bool b7_TEST_FLG_test_passed
        {
            get
            {
                return STDFManager.GetBit(this.TEST_FLG, 7);
            }
            set
            {
                this.TEST_FLG = STDFManager.SetBit(this.TEST_FLG, 7, value);
            }
        }

        public bool RESULT_VALID
        {
            get
            {
                return (((this.TEST_FLG & 0x3f) == 0) && ((this.PARM_FLG & 7) == 0));
            }
        }
    }
}

