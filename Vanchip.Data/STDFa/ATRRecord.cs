namespace STDFInterface
{
    using System;
    using System.IO;
    using System.Text;

    public class ATRRecord : STDFRecord
    {
        public string CMD_LINE;
        public DateTime MOD_TIM;

        public ATRRecord() : base(STDFRecordType.ATR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.MOD_TIM);
            if (num < base.REC_LEN)
            {
                num += base.Read(br, out this.CMD_LINE);
            }
            return num;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("ATR (Audit Trail Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-15}{1}\n", "Date Time:", this.MOD_TIM.ToString());
            builder.AppendFormat("\t{0,-15}{1}\n", "Command Line:", this.CMD_LINE.ToString());
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            ushort num = Convert.ToUInt16(STDFRecordType.ATR);
            bw.Write(num);
            base.WriteTime(bw, this.MOD_TIM);
            base.WriteString(bw, this.CMD_LINE);
            return 0;
        }
    }
}

