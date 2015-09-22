namespace STDF_V4
{
    using System;
    using System.IO;
    using System.Text;

    public class WRRRecord : STDFRecord
    {
        public uint ABRT_CNT;
        public string EXC_DESC;
        public string FABWF_ID;
        public DateTime FINISH_T;
        public string FRAME_ID;
        public uint FUNC_CNT;
        public uint GOOD_CNT;
        public byte HEAD_NUM;
        public string MASK_ID;
        public uint PART_CNT;
        public uint RTST_CNT;
        public byte SITE_GRP;
        public string USR_DESC;
        public string WAFER_ID;

        public WRRRecord() : base(STDFRecordType.WRR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.HEAD_NUM);
            num += base.Read(br, out this.SITE_GRP);
            num += base.Read(br, out this.FINISH_T);
            num += base.Read(br, out this.PART_CNT);
            num += base.Read(br, out this.RTST_CNT);
            num += base.Read(br, out this.ABRT_CNT);
            num += base.Read(br, out this.GOOD_CNT);
            num += base.Read(br, out this.FUNC_CNT);
            num += base.Read(br, out this.WAFER_ID);
            num += base.Read(br, out this.FABWF_ID);
            num += base.Read(br, out this.FRAME_ID);
            num += base.Read(br, out this.MASK_ID);
            num += base.Read(br, out this.USR_DESC);
            return (num + base.Read(br, out this.EXC_DESC));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x400);
            builder.AppendFormat("WRR (Wafer Result Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-35}{1}\n", "Test Head Number:", this.HEAD_NUM);
            builder.AppendFormat("\t{0,-35}{1}\n", "Site Group Number:", this.SITE_GRP);
            builder.AppendFormat("\t{0,-35}{1}\n", "Date/Time Last Part Tested:", this.FINISH_T);
            builder.AppendFormat("\t{0,-35}{1}\n", "Number of parts tested:", this.PART_CNT);
            builder.AppendFormat("\t{0,-35}{1}\n", "Number of parts retested:", this.RTST_CNT);
            builder.AppendFormat("\t{0,-35}{1}\n", "Number of aborts during testing:", this.ABRT_CNT);
            builder.AppendFormat("\t{0,-35}{1}\n", "Number of passed parts tested:", this.GOOD_CNT);
            builder.AppendFormat("\t{0,-35}{1}\n", "Number of functional parts tested:", this.FUNC_CNT);
            builder.AppendFormat("\t{0,-35}{1}\n", "Wafer ID:", this.WAFER_ID);
            builder.AppendFormat("\t{0,-35}{1}\n", "Fab Wafer ID:", this.FABWF_ID);
            builder.AppendFormat("\t{0,-35}{1}\n", "Wafer Frame ID:", this.FRAME_ID);
            builder.AppendFormat("\t{0,-35}{1}\n", "Wafer Mask ID:", this.MASK_ID);
            builder.AppendFormat("\t{0,-35}{1}\n", "Wafer Description (User):", this.USR_DESC);
            builder.AppendFormat("\t{0,-35}{1}\n", "Wafer Description (Exec):", this.EXC_DESC);
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.WRR));
            bw.Write(this.HEAD_NUM);
            bw.Write(this.SITE_GRP);
            base.WriteTime(bw, this.FINISH_T);
            bw.Write(this.PART_CNT);
            bw.Write(this.RTST_CNT);
            bw.Write(this.ABRT_CNT);
            bw.Write(this.GOOD_CNT);
            bw.Write(this.FUNC_CNT);
            base.WriteString(bw, this.WAFER_ID);
            base.WriteString(bw, this.FABWF_ID);
            base.WriteString(bw, this.FRAME_ID);
            base.WriteString(bw, this.MASK_ID);
            base.WriteString(bw, this.USR_DESC);
            base.WriteString(bw, this.EXC_DESC);
            return 0;
        }
    }
}

