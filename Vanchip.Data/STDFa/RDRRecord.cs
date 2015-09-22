namespace STDFInterface
{
    using System;
    using System.IO;
    using System.Text;

    public class RDRRecord : STDFRecord
    {
        public ushort NUM_BINS;
        public ushort[] RTST_BIN;

        public RDRRecord() : base(STDFRecordType.RDR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.NUM_BINS);
            return (num + base.Read(br, this.NUM_BINS, out this.RTST_BIN));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x800);
            builder.AppendFormat("RDR (Retest Data Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\tNumber of Bins:\t\t{0}\n", this.NUM_BINS);
            builder.AppendFormat("\t\tRetest Bin Numbers\n", new object[0]);
            builder.AppendFormat("\tIndex\tRetest Bin #", new object[0]);
            for (int i = 0; i < this.NUM_BINS; i++)
            {
                builder.AppendFormat("\t{0}\t{1}\n", i, this.RTST_BIN[i]);
            }
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            if (this.NUM_BINS == 0)
            {
                this.NUM_BINS = Convert.ToByte(this.RTST_BIN.Length);
            }
            if (this.NUM_BINS != this.RTST_BIN.Length)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("RDR({0}) Record data inconsistency:\n", base.RecordNumber);
                builder.AppendFormat("\tNUM_BINS must match the size of array RTST_BIN\n", new object[0]);
                builder.AppendFormat("\tNUM_BINS = {0}, size of RTST_BIN is {1}\n", this.NUM_BINS, this.RTST_BIN.Length);
                throw new Exception(builder.ToString());
            }
            bw.Write(Convert.ToUInt16(STDFRecordType.RDR));
            bw.Write(this.NUM_BINS);
            for (int i = 0; i < this.NUM_BINS; i++)
            {
                bw.Write(this.RTST_BIN[i]);
            }
            return 0;
        }
    }
}

