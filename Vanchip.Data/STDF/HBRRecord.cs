namespace STDF_V4
{
    using System;
    using System.IO;
    using System.Text;

    public class HBRRecord : STDFRecord
    {
        public uint HBIN_CNT;
        public string HBIN_NAM;
        public ushort HBIN_NUM;
        public byte HBIN_PF;
        public byte HEAD_NUM;
        public byte SITE_NUM;

        public HBRRecord() : base(STDFRecordType.HBR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.HEAD_NUM);
            num += base.Read(br, out this.SITE_NUM);
            num += base.Read(br, out this.HBIN_NUM);
            num += base.Read(br, out this.HBIN_CNT);
            num += base.Read(br, out this.HBIN_PF);
            return (num + base.Read(br, out this.HBIN_NAM));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("HBR (Hardware Bin Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-25}{1}\n", "Head Number:", this.HEAD_NUM.ToString());
            builder.AppendFormat("\t{0,-25}{1}\n", "Site Number:", this.SITE_NUM.ToString());
            builder.AppendFormat("\t{0,-25}{1}\n", "Hardware Bin Number:", this.HBIN_NUM.ToString());
            builder.AppendFormat("\t{0,-25}{1}\n", "Number of parts in bin:", this.HBIN_CNT.ToString());
            builder.AppendFormat("\t{0,-25}{1}\n", "Pass Fail Indication:", Convert.ToChar(this.HBIN_PF));
            builder.AppendFormat("\t{0,-25}{1}\n", "Name of Hardware Bin:", this.HBIN_NAM.ToString());
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.HBR));
            bw.Write(this.HEAD_NUM);
            bw.Write(this.SITE_NUM);
            bw.Write(this.HBIN_NUM);
            bw.Write(this.HBIN_CNT);
            bw.Write(this.HBIN_PF);
            base.WriteString(bw, this.HBIN_NAM);
            return 0;
        }
    }
}

