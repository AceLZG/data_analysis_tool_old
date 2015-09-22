namespace STDF_V4
{
    using System;
    using System.IO;
    using System.Text;

    public class DTRRecord : STDFRecord
    {
        public string TEXT_DAT;

        public DTRRecord() : base(STDFRecordType.DTR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            return (num + base.Read(br, out this.TEXT_DAT));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("DTR (Datalog Text Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\tText data:\t\t{0}\n", this.TEXT_DAT.ToString());
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.DTR));
            base.WriteString(bw, this.TEXT_DAT);
            return 0;
        }
    }
}

