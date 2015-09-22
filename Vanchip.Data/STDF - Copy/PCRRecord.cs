namespace STDF_V4
{
    using System;
    using System.IO;
    using System.Text;

    public class PCRRecord : STDFRecord
    {
        public uint ABRT_CNT;
        public uint FUNC_CNT;
        public uint GOOD_CNT;
        public byte HEAD_NUM;
        public uint PART_CNT;
        public uint RTST_CNT;
        public byte SITE_NUM;

        public PCRRecord() : base(STDFRecordType.PCR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.HEAD_NUM);
            num += base.Read(br, out this.SITE_NUM);
            num += base.Read(br, out this.PART_CNT);
            num += base.Read(br, out this.RTST_CNT);
            num += base.Read(br, out this.ABRT_CNT);
            num += base.Read(br, out this.GOOD_CNT);
            return (num + base.Read(br, out this.FUNC_CNT));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("PCR (Part Count Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-23}{1}\n", "Head Number:", this.HEAD_NUM.ToString());
            builder.AppendFormat("\t{0,-23}{1}\n", "Site Number:", this.SITE_NUM.ToString());
            builder.AppendFormat("\t{0,-23}{1}\n", "Parts Tested:", this.PART_CNT.ToString());
            builder.AppendFormat("\t{0,-23}{1}\n", "Retest Count:", this.RTST_CNT.ToString());
            builder.AppendFormat("\t{0,-23}{1}\n", "Abort Count:", this.ABRT_CNT.ToString());
            builder.AppendFormat("\t{0,-23}{1}\n", "Good Count:", this.GOOD_CNT.ToString());
            builder.AppendFormat("\t{0,-23}{1}\n", "Functional Count:", this.FUNC_CNT.ToString());
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.PCR));
            bw.Write(this.HEAD_NUM);
            bw.Write(this.SITE_NUM);
            bw.Write(this.PART_CNT);
            bw.Write(this.RTST_CNT);
            bw.Write(this.ABRT_CNT);
            bw.Write(this.GOOD_CNT);
            bw.Write(this.FUNC_CNT);
            return 0;
        }
    }
}

