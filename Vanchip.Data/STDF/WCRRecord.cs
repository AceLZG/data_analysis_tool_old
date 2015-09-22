namespace STDF_V4
{
    using System;
    using System.IO;
    using System.Text;

    public class WCRRecord : STDFRecord
    {
        public short CENTER_X;
        public short CENTER_Y;
        public float DIE_HT;
        public float DIE_WID;
        public byte POS_X;
        public byte POS_Y;
        public float WAFR_SIZ;
        public byte WF_FLAT;
        public byte WF_UNITS;

        public WCRRecord() : base(STDFRecordType.WCR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.WAFR_SIZ);
            num += base.Read(br, out this.DIE_HT);
            num += base.Read(br, out this.DIE_WID);
            num += base.Read(br, out this.WF_UNITS);
            num += base.Read(br, out this.WF_FLAT);
            num += base.Read(br, out this.CENTER_X);
            num += base.Read(br, out this.CENTER_Y);
            num += base.Read(br, out this.POS_X);
            return (num + base.Read(br, out this.POS_Y));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("WCR (Wafer Configuration Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-37}{1}\n", "Wafer Size:", this.WAFR_SIZ);
            builder.AppendFormat("\t{0,-37}{1}\n", "Die Height:", this.DIE_HT);
            builder.AppendFormat("\t{0,-37}{1}\n", "Width of Die:", this.DIE_WID);
            builder.AppendFormat("\t{0,-37}{1}\n", "Units for wafer dimensions:", this.WF_UNITS);
            builder.AppendFormat("\t{0,-37}{1}\n", "Orientation:", this.WF_FLAT);
            builder.AppendFormat("\t{0,-37}{1}\n", "X coordinate of center die on wafer:", this.CENTER_X);
            builder.AppendFormat("\t{0,-37}{1}\n", "Y coordinate of center die on wafer:", this.CENTER_Y);
            builder.AppendFormat("\t{0,-37}{1}\n", "Positive X direction of Wafer:", this.POS_X);
            builder.AppendFormat("\t{0,-37}{1}\n", "Positive Y direction of Wafer:", this.POS_Y);
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.WCR));
            bw.Write(this.WAFR_SIZ);
            bw.Write(this.DIE_HT);
            bw.Write(this.DIE_WID);
            bw.Write(this.WF_UNITS);
            bw.Write(this.WF_FLAT);
            bw.Write(this.CENTER_X);
            bw.Write(this.CENTER_Y);
            bw.Write(this.POS_X);
            bw.Write(this.POS_Y);
            return 0;
        }
    }
}

