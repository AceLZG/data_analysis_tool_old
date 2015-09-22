namespace STDFInterface
{
    using System;
    using System.IO;
    using System.Text;

    public class MRRRecord : STDFRecord
    {
        public byte DISP_COD;
        public string EXC_DESC;
        public DateTime FINISH_T;
        public string USR_DESC;

        public MRRRecord() : base(STDFRecordType.MRR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.FINISH_T);
            num += base.Read(br, out this.DISP_COD);
            num += base.Read(br, out this.USR_DESC);
            return (num + base.Read(br, out this.EXC_DESC));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("MRR (Master Result Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-23}{1}\n", "Date Time:", this.FINISH_T.ToString());
            builder.AppendFormat("\t{0,-23}{1}\n", "Lot Disposition Code:", Convert.ToChar(this.DISP_COD));
            builder.AppendFormat("\t{0,-23}{1}\n", "User Description:", this.USR_DESC);
            builder.AppendFormat("\t{0,-23}{1}\n", "Exec Description:", this.EXC_DESC);
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.MRR));
            base.WriteTime(bw, this.FINISH_T);
            bw.Write(this.DISP_COD);
            base.WriteString(bw, this.USR_DESC);
            base.WriteString(bw, this.EXC_DESC);
            return 0;
        }
    }
}

