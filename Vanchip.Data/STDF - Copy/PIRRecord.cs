namespace STDF_V4
{
    using System;
    using System.IO;
    using System.Text;

    public class PIRRecord : STDFRecord
    {
        public byte HEAD_NUM;
        public byte SITE_NUM;

        public PIRRecord() : base(STDFRecordType.PIR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.HEAD_NUM);
            return (num + base.Read(br, out this.SITE_NUM));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("PIR (Part Information Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-23}{1}\n", "Head Number:", this.HEAD_NUM);
            builder.AppendFormat("\t{0,-23}{1}\n", "Site Number:", this.SITE_NUM);
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.PIR));
            bw.Write(this.HEAD_NUM);
            bw.Write(this.SITE_NUM);
            return 0;
        }
    }
}

