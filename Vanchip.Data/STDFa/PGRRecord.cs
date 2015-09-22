namespace STDFInterface
{
    using System;
    using System.IO;
    using System.Text;

    public class PGRRecord : STDFRecord
    {
        public ushort GRP_INDX;
        public string GRP_NAM;
        public ushort INDX_CNT;
        public ushort[] PMR_INDX;

        public PGRRecord() : base(STDFRecordType.PGR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.GRP_INDX);
            num += base.Read(br, out this.GRP_NAM);
            num += base.Read(br, out this.INDX_CNT);
            return (num + base.Read(br, this.INDX_CNT, out this.PMR_INDX));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x800);
            builder.AppendFormat("PGR (Pin Group Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-23}{1}\n", "Index for Pin Group:", this.GRP_INDX.ToString());
            builder.AppendFormat("\t{0,-23}{1}\n", "Name of Pin Group:", this.GRP_NAM.ToString());
            builder.AppendFormat("\t{0,-23}{1}\n", "Count of PMR Indexes:", this.INDX_CNT.ToString());
            for (int i = 0; i < this.INDX_CNT; i++)
            {
                builder.AppendFormat("\t\t {0}: {1}\n", i, this.PMR_INDX[i].ToString());
            }
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            bw.Write(Convert.ToUInt16(STDFRecordType.PGR));
            bw.Write(this.GRP_INDX);
            base.WriteString(bw, this.GRP_NAM);
            bw.Write(Convert.ToUInt16(this.PMR_INDX.Length));
            for (int i = 0; i < this.PMR_INDX.Length; i++)
            {
                bw.Write(this.PMR_INDX[i]);
            }
            return 0;
        }
    }
}

