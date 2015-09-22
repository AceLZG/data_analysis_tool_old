namespace STDF_V4
{
    using System;
    using System.IO;
    using System.Text;

    public class BPSRecord : STDFRecord
    {
        public string SEQ_NAME;

        public BPSRecord() : base(STDFRecordType.BPS)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            return (num + base.Read(br, out this.SEQ_NAME));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("BPS (Begin Program Section Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-25}{1}\n", "Sequence name:", this.SEQ_NAME.ToString());
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.BPS));
            base.WriteString(bw, this.SEQ_NAME);
            return 0;
        }
    }
}

