namespace STDFInterface
{
    using System;
    using System.IO;
    using System.Text;

    public class SBRRecord : STDFRecord
    {
        public byte HEAD_NUM;
        public uint SBIN_CNT;
        public string SBIN_NAM;
        public ushort SBIN_NUM;
        public byte SBIN_PF;
        public byte SITE_NUM;

        public SBRRecord() : base(STDFRecordType.SBR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.HEAD_NUM);
            num += base.Read(br, out this.SITE_NUM);
            num += base.Read(br, out this.SBIN_NUM);
            num += base.Read(br, out this.SBIN_CNT);
            num += base.Read(br, out this.SBIN_PF);
            return (num + base.Read(br, out this.SBIN_NAM));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("SBR (Software Bin Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-25}{1}\n", "Head Number:", this.HEAD_NUM.ToString());
            builder.AppendFormat("\t{0,-25}{1}\n", "Site Number:", this.SITE_NUM.ToString());
            builder.AppendFormat("\t{0,-25}{1}\n", "Software Bin Number:", this.SBIN_NUM.ToString());
            builder.AppendFormat("\t{0,-25}{1}\n", "Number of parts in bin:", this.SBIN_CNT.ToString());
            builder.AppendFormat("\t{0,-25}{1}\n", "Pass Fail Indication:", Convert.ToChar(this.SBIN_PF));
            builder.AppendFormat("\t{0,-25}{1}\n", "Name of Software Bin:", this.SBIN_NAM.ToString());
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.SBR));
            bw.Write(this.HEAD_NUM);
            bw.Write(this.SITE_NUM);
            bw.Write(this.SBIN_NUM);
            bw.Write(this.SBIN_CNT);
            bw.Write(this.SBIN_PF);
            base.WriteString(bw, this.SBIN_NAM);
            return 0;
        }
    }
}

