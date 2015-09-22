namespace STDF_V4
{
    using System;
    using System.IO;
    using System.Text;

    public class PRRRecord : STDFRecord
    {
        public ushort HARD_BIN;
        public byte HEAD_NUM;
        public ushort NUM_TEST;
        public byte[] PART_FIX;
        public byte PART_FLG;
        public string PART_ID;
        public string PART_TXT;
        public byte SITE_NUM;
        public ushort SOFT_BIN;
        public uint TEST_T;
        public short X_COORD;
        public short Y_COORD;

        public PRRRecord() : base(STDFRecordType.PRR)
        {
            this.PART_FLG = 0;
            this.PART_FLG = STDFManager.SetBit(this.PART_FLG, 4, true);
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.HEAD_NUM);
            num += base.Read(br, out this.SITE_NUM);
            num += base.Read(br, out this.PART_FLG);
            num += base.Read(br, out this.NUM_TEST);
            num += base.Read(br, out this.HARD_BIN);
            num += base.Read(br, out this.SOFT_BIN);
            num += base.Read(br, out this.X_COORD);
            num += base.Read(br, out this.Y_COORD);
            num += base.Read(br, out this.TEST_T);
            num += base.Read(br, out this.PART_ID);
            num += base.Read(br, out this.PART_TXT);
            return (num + base.Read(br, out this.PART_FIX));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("PRR (Part Result Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-28}{1}\n", "Test Head Number:", this.HEAD_NUM);
            builder.AppendFormat("\t{0,-28}{1}\n", "Test Site Number:", this.SITE_NUM);
            builder.AppendFormat("\t{0,-28}0x{1:X}\n", "Part Information Flag:", this.PART_FLG);
            builder.AppendFormat("\t\t{0,-40}{1}\n", "Bits 2,3,4 ,Part Passed: ", this.PartPassed);
            builder.AppendFormat("\t\tBit 0: {0}\n", this.b0_RetestPART_ID);
            builder.AppendFormat("\t\tBit 1: {0}\n", this.b1_Retest_XY);
            builder.AppendFormat("\t\tBit 2: {0}\n", this.b2_AbnormalEndOfTesting);
            builder.AppendFormat("\t\tBit 3: {0}\n", this.b3_PartFailed);
            builder.AppendFormat("\t\tBit 4: {0}\n", this.b4_NoPassFailIndication);
            builder.AppendFormat("\t\t{0,-40}\n", "Bit 5: Reserved");
            builder.AppendFormat("\t\t{0,-40}\n", "Bit 6: Reserved");
            builder.AppendFormat("\t\t{0,-40}\n", "Bit 7: Reserved");
            builder.AppendFormat("\t{0,-28}{1}\n", "Number of test executed:", this.NUM_TEST);
            builder.AppendFormat("\t{0,-28}{1}\n", "Hard Bin Number:", this.HARD_BIN);
            builder.AppendFormat("\t{0,-28}{1}\n", "Soft Bin Number:", this.SOFT_BIN);
            builder.AppendFormat("\t{0,-28}{1}\n", "Wafer X Coordinate:", this.X_COORD);
            builder.AppendFormat("\t{0,-28}{1}\n", "Wafer Y Coordinate:", this.Y_COORD);
            builder.AppendFormat("\t{0,-28}{1}\n", "Test Time in Milliseconds:", this.TEST_T);
            builder.AppendFormat("\t{0,-28}{1}\n", "Part Identification:", this.PART_ID);
            builder.AppendFormat("\t{0,-28}{1}\n", "Part Text:", this.PART_TXT);
            builder.AppendFormat("\t{0,-28}{1}\n", "Part Repair Information:", this.PART_FIX);
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.PRR));
            bw.Write(this.HEAD_NUM);
            bw.Write(this.SITE_NUM);
            bw.Write(this.PART_FLG);
            bw.Write(this.NUM_TEST);
            bw.Write(this.HARD_BIN);
            bw.Write(this.SOFT_BIN);
            bw.Write(this.X_COORD);
            bw.Write(this.Y_COORD);
            bw.Write(this.TEST_T);
            base.WriteString(bw, this.PART_ID);
            base.WriteString(bw, this.PART_TXT);
            base.WriteArray(bw, this.PART_FIX);
            return 0;
        }

        public bool b0_RetestPART_ID
        {
            get
            {
                return STDFManager.GetBit(this.PART_FLG, 0);
            }
            set
            {
                this.PART_FLG = STDFManager.SetBit(this.PART_FLG, 0, value);
            }
        }

        public bool b1_Retest_XY
        {
            get
            {
                return STDFManager.GetBit(this.PART_FLG, 1);
            }
            set
            {
                this.PART_FLG = STDFManager.SetBit(this.PART_FLG, 1, value);
            }
        }

        public bool b2_AbnormalEndOfTesting
        {
            get
            {
                return STDFManager.GetBit(this.PART_FLG, 2);
            }
            set
            {
                this.PART_FLG = STDFManager.SetBit(this.PART_FLG, 2, value);
            }
        }

        public bool b3_PartFailed
        {
            get
            {
                return STDFManager.GetBit(this.PART_FLG, 3);
            }
            set
            {
                this.PART_FLG = STDFManager.SetBit(this.PART_FLG, 3, value);
            }
        }

        public bool b4_NoPassFailIndication
        {
            get
            {
                return STDFManager.GetBit(this.PART_FLG, 4);
            }
            set
            {
                this.PART_FLG = STDFManager.SetBit(this.PART_FLG, 4, value);
            }
        }

        public bool NewPart
        {
            get
            {
                if (!STDFManager.GetBit(this.PART_FLG, 0))
                {
                    return STDFManager.GetBit(this.PART_FLG, 1);
                }
                return true;
            }
        }

        public bool PartPassed
        {
            get
            {
                return ((!this.b2_AbnormalEndOfTesting && !this.b3_PartFailed) && !this.b4_NoPassFailIndication);
            }
            set
            {
                this.b2_AbnormalEndOfTesting = false;
                this.b4_NoPassFailIndication = false;
                this.b3_PartFailed = value;
            }
        }
    }
}

