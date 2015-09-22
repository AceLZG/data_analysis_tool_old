namespace STDF_V4
{
    using System;
    using System.IO;
    using System.Text;

    public class FARRecord : STDFRecord
    {
        public byte CPU_TYPE;
        public byte STDF_VER;

        public FARRecord() : base(STDFRecordType.FAR)
        {
        }

        public override int Read(BinaryReader br)
        {
            this.CPU_TYPE = br.ReadByte();
            this.STDF_VER = br.ReadByte();
            return 2;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("FAR (File Attribute Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-15}{1}\n", "CPU Type:", this.CPU_TYPE);
            builder.AppendFormat("\t{0,-15}{1}\n", "STDF Version:", this.STDF_VER);
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            this.CPU_TYPE = 2;
            this.STDF_VER = 4;
            bw.Write(Convert.ToUInt16(STDFRecordType.FAR));
            bw.Write(this.CPU_TYPE);
            bw.Write(this.STDF_VER);
            return 0;
        }
    }
}

