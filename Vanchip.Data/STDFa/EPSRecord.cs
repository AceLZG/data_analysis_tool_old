namespace STDFInterface
{
    using System;
    using System.IO;
    using System.Text;

    public class EPSRecord : STDFRecord
    {
        public byte placeHolder;

        public EPSRecord() : base(STDFRecordType.EPS)
        {
        }

        public override int Read(BinaryReader br)
        {
            return 0;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("EPS (End (last) program section): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.EPS));
            return 0;
        }
    }
}

