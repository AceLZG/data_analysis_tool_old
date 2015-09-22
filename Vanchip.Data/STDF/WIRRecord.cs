namespace STDF_V4
{
    using System;
    using System.IO;
    using System.Text;

    public class WIRRecord : STDFRecord
    {
        public byte HEAD_NUM;
        public byte SITE_GRP;
        public DateTime START_T;
        public string WAFER_ID;

        public WIRRecord() : base(STDFRecordType.WIR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.HEAD_NUM);
            num += base.Read(br, out this.SITE_GRP);
            num += base.Read(br, out this.START_T);
            return (num + base.Read(br, out this.WAFER_ID));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("WIR (Wafer Information Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-30}{1}\n", "Test Head #:", this.HEAD_NUM);
            builder.AppendFormat("\t{0,-30}{1}\n", "Site Group #:", this.SITE_GRP);
            builder.AppendFormat("\t{0,-30}{1}\n", "Date/Time First Part Tested:", this.START_T);
            builder.AppendFormat("\t{0,-30}{1}\n", "Wafer ID:", this.WAFER_ID);
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.WIR));
            bw.Write(this.HEAD_NUM);
            bw.Write(this.SITE_GRP);
            base.WriteTime(bw, this.START_T);
            base.WriteString(bw, this.WAFER_ID);
            return 0;
        }
    }
}

