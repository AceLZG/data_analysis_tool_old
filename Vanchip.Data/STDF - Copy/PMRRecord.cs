namespace STDF_V4
{
    using System;
    using System.IO;
    using System.Text;

    public class PMRRecord : STDFRecord
    {
        public string CHAN_NAM;
        public ushort CHAN_TYP;
        public byte HEAD_NUM;
        public string LOG_NAM;
        public string PHY_NAM;
        public ushort PMR_INDX;
        public byte SITE_NUM;

        public PMRRecord() : base(STDFRecordType.PMR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.PMR_INDX);
            num += base.Read(br, out this.CHAN_TYP);
            num += base.Read(br, out this.CHAN_NAM);
            num += base.Read(br, out this.PHY_NAM);
            num += base.Read(br, out this.LOG_NAM);
            num += base.Read(br, out this.HEAD_NUM);
            return (num + base.Read(br, out this.SITE_NUM));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("PMR (Pin Map Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-35}{1}\n", "Unique Index for pin:", this.PMR_INDX.ToString());
            builder.AppendFormat("\t{0,-35}{1}\n", "Channel Type:", this.CHAN_TYP.ToString());
            builder.AppendFormat("\t{0,-35}{1}\n", "Channel Name:", this.CHAN_NAM.ToString());
            builder.AppendFormat("\t{0,-35}{1}\n", "Physical Name of Pin:", this.PHY_NAM.ToString());
            builder.AppendFormat("\t{0,-35}{1}\n", "Logical Name of Pin:", this.LOG_NAM.ToString());
            builder.AppendFormat("\t{0,-35}{1}\n", "Head Number associated with Chann:", this.HEAD_NUM.ToString());
            builder.AppendFormat("\t{0,-35}{1}\n", "Site Number associated with Chann:", this.SITE_NUM.ToString());
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.PMR));
            bw.Write(this.PMR_INDX);
            bw.Write(this.CHAN_TYP);
            base.WriteString(bw, this.CHAN_NAM);
            base.WriteString(bw, this.PHY_NAM);
            base.WriteString(bw, this.LOG_NAM);
            bw.Write(this.HEAD_NUM);
            bw.Write(this.SITE_NUM);
            return 0;
        }
    }
}

