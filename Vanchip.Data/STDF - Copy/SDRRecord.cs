namespace STDF_V4
{
    using System;
    using System.IO;
    using System.Text;

    public class SDRRecord : STDFRecord
    {
        public string CABL_ID;
        public string CABL_TYP;
        public string CARD_ID;
        public string CARD_TYP;
        public string CONT_ID;
        public string CONT_TYP;
        public string DIB_ID;
        public string DIB_TYP;
        public string EXTR_ID;
        public string EXTR_TYP;
        public string HAND_ID;
        public string HAND_TYP;
        public byte HEAD_NUM;
        public string LASR_ID;
        public string LASR_TYP;
        public string LOAD_ID;
        public string LOAD_TYP;
        public byte SITE_CNT;
        public byte SITE_GRP;
        public byte[] SITE_NUM;

        public SDRRecord() : base(STDFRecordType.SDR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.HEAD_NUM);
            num += base.Read(br, out this.SITE_GRP);
            num += base.Read(br, out this.SITE_CNT);
            num += base.Read(br, this.SITE_CNT, out this.SITE_NUM);
            num += base.Read(br, out this.HAND_TYP);
            num += base.Read(br, out this.HAND_ID);
            num += base.Read(br, out this.CARD_TYP);
            num += base.Read(br, out this.CARD_ID);
            num += base.Read(br, out this.LOAD_TYP);
            num += base.Read(br, out this.LOAD_ID);
            num += base.Read(br, out this.DIB_TYP);
            num += base.Read(br, out this.DIB_ID);
            num += base.Read(br, out this.CABL_TYP);
            num += base.Read(br, out this.CABL_ID);
            num += base.Read(br, out this.CONT_TYP);
            num += base.Read(br, out this.CONT_ID);
            num += base.Read(br, out this.LASR_TYP);
            num += base.Read(br, out this.LASR_ID);
            num += base.Read(br, out this.EXTR_TYP);
            return (num + base.Read(br, out this.EXTR_ID));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("SDR (Site Description Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-23}{1}\n", "Head Number:", this.HEAD_NUM);
            builder.AppendFormat("\t{0,-23}{1}\n", "Site Group:", this.SITE_GRP);
            builder.AppendFormat("\t{0,-23}{1}\n", "Site Count:", this.SITE_CNT);
            builder.AppendFormat("\tSite Numbers: ", new object[0]);
            for (int i = 0; i < this.SITE_CNT; i++)
            {
                builder.AppendFormat("{0} ", this.SITE_NUM[i]);
            }
            builder.AppendFormat("\n", new object[0]);
            builder.AppendFormat("\t{0,-23}{1}\n", "Handler/Prober Type:", this.HAND_TYP);
            builder.AppendFormat("\t{0,-23}{1}\n", "Handler/Prober ID:", this.HAND_ID);
            builder.AppendFormat("\t{0,-23}{1}\n", "Prober Card Type:", this.CARD_TYP);
            builder.AppendFormat("\t{0,-23}{1}\n", "Prober Card ID:", this.CARD_ID);
            builder.AppendFormat("\t{0,-23}{1}\n", "Load Board Type:", this.LOAD_TYP);
            builder.AppendFormat("\t{0,-23}{1}\n", "Load Board ID:", this.LOAD_ID);
            builder.AppendFormat("\t{0,-23}{1}\n", "DIB Board TYPE:", this.DIB_TYP);
            builder.AppendFormat("\t{0,-23}{1}\n", "DIB Board ID:", this.DIB_ID);
            builder.AppendFormat("\t{0,-23}{1}\n", "Interface Cable Type:", this.CABL_TYP);
            builder.AppendFormat("\t{0,-23}{1}\n", "Interface cable ID:", this.CABL_ID);
            builder.AppendFormat("\t{0,-23}{1}\n", "Handler Contactor type:", this.CONT_TYP);
            builder.AppendFormat("\t{0,-23}{1}\n", "Handler Contactor ID:", this.CONT_ID);
            builder.AppendFormat("\t{0,-23}{1}\n", "Laser Type:", this.LASR_TYP);
            builder.AppendFormat("\t{0,-23}{1}\n", "Laser ID:", this.LASR_ID);
            builder.AppendFormat("\t{0,-23}{1}\n", "Extra Equipment Type:", this.EXTR_TYP);
            builder.AppendFormat("\t{0,-23}{1}\n", "Extra Equipment ID:", this.EXTR_ID);
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            if (this.SITE_CNT == 0)
            {
                this.SITE_CNT = Convert.ToByte(this.SITE_NUM.Length);
            }
            else if (this.SITE_CNT != this.SITE_NUM.Length)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("SDR({0}) Record data inconsistency:\n", base.RecordNumber);
                builder.AppendFormat("\tSITE_CNT must match the size of array SITE_NUM\n", new object[0]);
                builder.AppendFormat("\tSITE_CNT = {0}, size of SITE_NUM is {1}\n", this.SITE_CNT.ToString(), this.SITE_NUM.Length.ToString());
                throw new Exception(builder.ToString());
            }
            bw.Write(Convert.ToUInt16(STDFRecordType.SDR));
            bw.Write(this.HEAD_NUM);
            bw.Write(this.SITE_GRP);
            bw.Write(this.SITE_CNT);
            for (int i = 0; i < this.SITE_CNT; i++)
            {
                bw.Write(this.SITE_NUM[i]);
            }
            base.WriteString(bw, this.HAND_TYP);
            base.WriteString(bw, this.HAND_ID);
            base.WriteString(bw, this.CARD_TYP);
            base.WriteString(bw, this.CARD_ID);
            base.WriteString(bw, this.LOAD_TYP);
            base.WriteString(bw, this.LOAD_ID);
            base.WriteString(bw, this.DIB_TYP);
            base.WriteString(bw, this.DIB_ID);
            base.WriteString(bw, this.CABL_TYP);
            base.WriteString(bw, this.CABL_ID);
            base.WriteString(bw, this.CONT_TYP);
            base.WriteString(bw, this.CONT_ID);
            base.WriteString(bw, this.LASR_TYP);
            base.WriteString(bw, this.LASR_ID);
            base.WriteString(bw, this.EXTR_TYP);
            base.WriteString(bw, this.EXTR_ID);
            return 0;
        }
    }
}

