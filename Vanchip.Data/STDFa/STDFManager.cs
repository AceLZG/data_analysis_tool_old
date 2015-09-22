namespace STDFInterface
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class STDFManager : IDisposable
    {
        public long BeginningOfBadRecord;
        private static byte[] BitMask = new byte[] { 1, 2, 4, 8, 0x10, 0x20, 0x40, 0x80 };
        private BinaryReader br;
        private BinaryWriter bw;
        public long FileLength;
        public string FileProcessing;
        private FileStream fs;
        public long LastGoodFilePosition;
        public STDFRecordType LastGoodSTDFRecord;
        private STDFFileMode Mode;
        public double Percent;
        public BYTE_ORIENTATION Processor;
        private int RecIndex;
        public List<STDFRecordIndex> RecIndexList;
        public long RecordCount;
        public StatusMessage StatusMessagePublisher;

        public STDFManager()
        {
            this.Processor = BYTE_ORIENTATION.UNKNOWN;
            throw new Exception("Must Create STDFManager with parameters STDFFileMode.xxx (read or write) and a STDF file name to read or write");
        }

        public STDFManager(STDFFileMode mode, string fileName)
        {
            this.Processor = BYTE_ORIENTATION.UNKNOWN;
            this.Mode = mode;
            this.FileProcessing = fileName;
            if (this.Mode == STDFFileMode.Write)
            {
                try
                {
                    if (fileName == @"c:\StdfTest.std")
                    {
                        File.Delete(fileName);
                    }
                    this.fs = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.None, 0x4000);
                    this.bw = new BinaryWriter(this.fs);
                    this.Logger("File:{0} Created for output.\n", new object[] { fileName });
                    return;
                }
                catch
                {
                    this.Logger("Unable to create file:{0} (File may already exist)\n", new object[] { fileName });
                    throw;
                }
            }
            if (this.Mode == STDFFileMode.Read)
            {
                try
                {
                    this.fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 0x4000);
                    this.Logger("File:{0} opened for Reading\n", new object[] { fileName });
                    this.FileLength = this.fs.Length;
                    if (this.FileLength < 0x19L)
                    {
                        throw new Exception("File length less than 25 bytes " + fileName.ToString() + " is not a valid stdf file");
                    }
                    this.br = new BinaryReader(this.fs);
                    this.Processor = BYTE_ORIENTATION.UNKNOWN;
                    return;
                }
                catch
                {
                    this.Logger("Unable to open file:{0} for reading\n", new object[] { fileName });
                    throw;
                }
            }
            throw new Exception("STDFManager:Unknown File Mode Requested");
        }

        public void breaker(BinaryReader br)
        {
            new StringBuilder(0x100).AppendFormat("Position is: {0:X}", br.BaseStream.Position);
        }

        public void Close()
        {
            this.Logger("Closing File:{0}{1}{0}", new object[] { Environment.NewLine, this.FileProcessing });
            if (this.bw != null)
            {
                this.bw.Flush();
                this.bw.BaseStream.Close();
                this.bw = null;
            }
            if (this.br != null)
            {
                this.br.BaseStream.Close();
                this.br = null;
            }
        }

        private BYTE_ORIENTATION DetermineProcessorType(BinaryReader br)
        {
            long position = br.BaseStream.Position;
            byte num2 = 0;
            byte num3 = 0;
            byte num4 = 0;
            ushort num5 = 0;
            br.BaseStream.Position = 0L;
            BYTE_ORIENTATION uNKNOWN = BYTE_ORIENTATION.UNKNOWN;
            num5 = br.ReadUInt16();
            num2 = br.ReadByte();
            num3 = br.ReadByte();
            num4 = br.ReadByte();
            if ((num2 != 0) || (num3 != 10))
            {
                throw new Exception("First Record is not a far record, unable to process file\n");
            }
            if (num5 == 2)
            {
                uNKNOWN = BYTE_ORIENTATION.IBMPC;
            }
            else
            {
                switch (num4)
                {
                    case 0:
                        uNKNOWN = BYTE_ORIENTATION.DEC;
                        break;

                    case 1:
                        uNKNOWN = BYTE_ORIENTATION.SUN;
                        break;

                    case 2:
                        throw new Exception("First Record Length does not match processor type, unable to process file");
                }
            }
            br.BaseStream.Position = position;
            return uNKNOWN;
        }

        public void Dispose()
        {
            if (this.bw != null)
            {
                this.bw = null;
            }
            if (this.fs != null)
            {
                if (this.Mode == STDFFileMode.Write)
                {
                    this.fs.Flush();
                }
                this.fs.Close();
                this.fs = null;
            }
            if (this.br != null)
            {
                this.br = null;
            }
        }

        public void Flush()
        {
            if (this.Mode == STDFFileMode.Write)
            {
                this.bw.Flush();
            }
            else
            {
                this.Logger("Unable to flush, file not open for writing.\n", new object[0]);
                throw new Exception("File Not open for writing, Unable to flush.");
            }
        }

        public static bool GetBit(byte b, int bit)
        {
            return (0 != (b & BitMask[bit]));
        }

        public STDFRecord GoToRecord(int Record)
        {
            if (this.RecIndexList == null)
            {
                this.MakeIndex();
            }
            Record--;
            if (Record >= this.RecIndexList.Count)
            {
                Record = this.RecIndexList.Count - 1;
            }
            if (Record < 0)
            {
                Record = 0;
            }
            this.RecIndex = Record;
            return this.ReadRecord(this.RecIndexList[this.RecIndex]);
        }

        public STDFRecord GoToTestNumber(int TestNumber)
        {
            STDFRecordKeys keys = STDFRecordKeys.FTR | STDFRecordKeys.MPR | STDFRecordKeys.PTR;
            PTRRecord record = null;
            MPRRecord record2 = null;
            FTRRecord record3 = null;
            STDFRecord record4 = null;
            if (this.RecIndexList == null)
            {
                this.MakeIndex();
            }
            this.RecIndex++;
            if (this.RecIndex > this.RecIndexList.Count)
            {
                this.RecIndex = 0;
            }
            int num = 0;
            for (int i = this.RecIndex; num < this.RecIndexList.Count; i++)
            {
                if (i >= this.RecIndexList.Count)
                {
                    i = 0;
                }
                if ((this.RecIndexList[i].Key & keys) != STDFRecordKeys.NAN)
                {
                    record4 = this.ReadRecord(this.RecIndexList[i]);
                    if (this.RecIndexList[i].Key == STDFRecordKeys.PTR)
                    {
                        record = (PTRRecord) record4;
                        if (record.TEST_NUM == TestNumber)
                        {
                            this.RecIndex = i;
                            return record4;
                        }
                    }
                    else if (this.RecIndexList[i].Key == STDFRecordKeys.MPR)
                    {
                        record2 = (MPRRecord) record4;
                        if (record2.TEST_NUM == TestNumber)
                        {
                            this.RecIndex = i;
                            return record4;
                        }
                    }
                    else if (this.RecIndexList[i].Key == STDFRecordKeys.FTR)
                    {
                        record3 = (FTRRecord) record4;
                        if (record3.TEST_NUM == TestNumber)
                        {
                            this.RecIndex = i;
                            return record4;
                        }
                    }
                }
                num++;
            }
            return record4;
        }

        public STDFRecord IDXReadFirstRecord()
        {
            if (this.RecIndexList == null)
            {
                this.MakeIndex();
            }
            this.RecIndex = 0;
            return this.ReadRecord(this.RecIndexList[this.RecIndex]);
        }

        public STDFRecord IDXReadLastRecord()
        {
            if (this.RecIndexList == null)
            {
                this.MakeIndex();
            }
            this.RecIndex = this.RecIndexList.Count - 1;
            if (this.RecIndex < 0)
            {
                this.RecIndex = 0;
            }
            return this.ReadRecord(this.RecIndexList[this.RecIndex]);
        }

        public STDFRecord IDXReadNextRecord()
        {
            if (this.RecIndexList == null)
            {
                this.MakeIndex();
            }
            else
            {
                this.RecIndex++;
            }
            if (this.RecIndex >= this.RecIndexList.Count)
            {
                this.RecIndex = 0;
            }
            return this.ReadRecord(this.RecIndexList[this.RecIndex]);
        }

        public STDFRecord IDXReadNextRecord(STDFRecordKeys sf)
        {
            if (this.RecIndexList == null)
            {
                this.MakeIndex();
            }
            else
            {
                this.RecIndex++;
            }
            if (this.RecIndex >= this.RecIndexList.Count)
            {
                this.RecIndex = 0;
            }
            int num = 0;
            for (int i = this.RecIndex; num < this.RecIndexList.Count; i++)
            {
                if (i >= this.RecIndexList.Count)
                {
                    i = 0;
                }
                if ((this.RecIndexList[i].Key & sf) != STDFRecordKeys.NAN)
                {
                    this.RecIndex = i;
                    break;
                }
                num++;
            }
            return this.ReadRecord(this.RecIndexList[this.RecIndex]);
        }

        public STDFRecord IDXReadPreviousRecord()
        {
            if (this.RecIndexList == null)
            {
                this.MakeIndex();
            }
            else
            {
                this.RecIndex--;
            }
            if (this.RecIndex < 0)
            {
                this.RecIndex = this.RecIndexList.Count - 1;
            }
            return this.ReadRecord(this.RecIndexList[this.RecIndex]);
        }

        public STDFRecord IDXReadPreviousRecord(STDFRecordKeys sf)
        {
            if (this.RecIndexList == null)
            {
                this.MakeIndex();
            }
            else
            {
                this.RecIndex--;
            }
            if (this.RecIndex < 0)
            {
                this.RecIndex = this.RecIndexList.Count - 1;
            }
            int num = 0;
            for (int i = this.RecIndex; num < this.RecIndexList.Count; i--)
            {
                if (i < 0)
                {
                    i = this.RecIndexList.Count - 1;
                }
                if ((this.RecIndexList[i].Key & sf) != STDFRecordKeys.NAN)
                {
                    this.RecIndex = i;
                    break;
                }
                num++;
            }
            return this.ReadRecord(this.RecIndexList[this.RecIndex]);
        }

        public STDFRecord IDXReadSkipNextRecord(STDFRecordKeys sf)
        {
            if (this.RecIndexList == null)
            {
                this.MakeIndex();
            }
            else
            {
                this.RecIndex++;
            }
            if (this.RecIndex >= this.RecIndexList.Count)
            {
                this.RecIndex = 0;
            }
            int num = 0;
            for (int i = this.RecIndex; num < this.RecIndexList.Count; i++)
            {
                if (i >= this.RecIndexList.Count)
                {
                    i = 0;
                }
                if (this.RecIndexList[i].Key != sf)
                {
                    this.RecIndex = i;
                    break;
                }
                num++;
            }
            return this.ReadRecord(this.RecIndexList[this.RecIndex]);
        }

        public STDFRecord IDXReadSkipPreviousRecord(STDFRecordKeys sf)
        {
            if (this.RecIndexList == null)
            {
                this.MakeIndex();
            }
            else
            {
                this.RecIndex--;
            }
            if (this.RecIndex < 0)
            {
                this.RecIndex = this.RecIndexList.Count - 1;
            }
            int num = 0;
            for (int i = this.RecIndex; num < this.RecIndexList.Count; i--)
            {
                if (i < 0)
                {
                    i = this.RecIndexList.Count - 1;
                }
                if (this.RecIndexList[i].Key != sf)
                {
                    this.RecIndex = i;
                    break;
                }
                num++;
            }
            return this.ReadRecord(this.RecIndexList[this.RecIndex]);
        }

        public long IndexSTDF()
        {
            if (this.Mode != STDFFileMode.Read)
            {
                throw new Exception("File must be opened for Read before indexing\n");
            }
            return 0L;
        }

        public void Logger(string format, params object[] vars)
        {
            try
            {
                StringBuilder builder = new StringBuilder(0x80);
                builder.AppendFormat(format, vars);
                if (this.StatusMessagePublisher != null)
                {
                    this.StatusMessagePublisher(builder.ToString());
                }
            }
            catch
            {
            }
        }

        public int MakeIndex()
        {
            int capacity = 0x3d0900;
            ushort num2 = 0;
            byte num3 = 0;
            STDFRecordType nAN = STDFRecordType.NAN;
            long fPosition = 0L;
            long length = 0L;
            uint num7 = 1;
            this.Logger("Indexing File:{0}{1}{0}", new object[] { Environment.NewLine, this.FileProcessing.ToString() });
            if (this.Mode != STDFFileMode.Read)
            {
                throw new Exception("Cannot index a file opened for write.");
            }
            long position = this.br.BaseStream.Position;
            this.br.BaseStream.Position = 0L;
            length = this.br.BaseStream.Length;
            if (this.Processor == BYTE_ORIENTATION.UNKNOWN)
            {
                this.Processor = this.DetermineProcessorType(this.br);
            }
            this.RecIndexList = new List<STDFRecordIndex>(capacity);
            do
            {
                fPosition = this.br.BaseStream.Position;
                num2 = this.br.ReadUInt16();
                if (this.Processor != BYTE_ORIENTATION.IBMPC)
                {
                    uint num9 = Convert.ToUInt32(num2);
                    num9 = ((num9 >> 8) | (num9 << 8)) & 0xffff;
                    num2 = Convert.ToUInt16(num9);
                }
                num3 = this.br.ReadByte();
                nAN = (STDFRecordType) ((this.br.ReadByte() << 8) + num3);
                this.RecIndexList.Add(new STDFRecordIndex(nAN, num2, fPosition, num7++, RecordLookup(nAN)));
                this.br.ReadBytes(num2);
            }
            while (this.br.BaseStream.Position < length);
            this.br.BaseStream.Position = position;
            this.RecIndex = 0;
            return 0;
        }

        public STDFRecord ReadRecord()
        {
            ushort num = 0;
            byte num2 = 0;
            byte num3 = 0;
            long position = 0L;
            STDFRecord record = null;
            if (this.Mode == STDFFileMode.Write)
            {
                throw new Exception("Cannot read from a file opened for write");
            }
            position = this.br.BaseStream.Position;
            this.BeginningOfBadRecord = position;
            if (this.Processor == BYTE_ORIENTATION.UNKNOWN)
            {
                this.Processor = this.DetermineProcessorType(this.br);
            }
            if ((position + 4L) > this.FileLength)
            {
                return null;
            }
            num = this.br.ReadUInt16();
            num2 = this.br.ReadByte();
            num3 = this.br.ReadByte();
            if (this.Processor != BYTE_ORIENTATION.IBMPC)
            {
                uint num5 = Convert.ToUInt32(num);
                num5 = ((num5 >> 8) | (num5 << 8)) & 0xffff;
                num = Convert.ToUInt16(num5);
            }
            if ((position + num) >= this.FileLength)
            {
                throw new Exception("File not long enough, Probable File Corruption at offset:" + this.br.BaseStream.Position.ToString());
            }
            int num6 = (num3 << 8) + num2;
            switch (num6)
            {
                case 0xa14:
                    record = new BPSRecord();
                    break;

                case 0xa32:
                    record = new GDRRecord();
                    break;

                case 0xa00:
                    record = new FARRecord();
                    break;

                case 0xa01:
                    record = new MIRRecord();
                    break;

                case 0xa02:
                    record = new WIRRecord();
                    break;

                case 0xa05:
                    record = new PIRRecord();
                    break;

                case 0xa0f:
                    record = new PTRRecord();
                    break;

                case 0x1400:
                    record = new ATRRecord();
                    break;

                case 0x1401:
                    record = new MRRRecord();
                    break;

                case 0x1402:
                    record = new WRRRecord();
                    break;

                case 0x1405:
                    record = new PRRRecord();
                    break;

                case 0xf0f:
                    record = new MPRRecord();
                    break;

                case 0x1e01:
                    record = new PCRRecord();
                    break;

                case 0x1e02:
                    record = new WCRRecord();
                    break;

                case 0x1414:
                    record = new EPSRecord();
                    break;

                case 0x140f:
                    record = new FTRRecord();
                    break;

                case 0x2801:
                    record = new HBRRecord();
                    break;

                case 0x3201:
                    record = new SBRRecord();
                    break;

                case 0x1e0a:
                    record = new TSRRecord();
                    break;

                case 0x1e32:
                    record = new DTRRecord();
                    break;

                case 0x3c01:
                    record = new PMRRecord();
                    break;

                case 0x3e01:
                    record = new PGRRecord();
                    break;

                case 0x3f01:
                    record = new PLRRecord();
                    break;

                case 0x4601:
                    record = new RDRRecord();
                    break;

                case 0x5001:
                    record = new SDRRecord();
                    break;

                default:
                {
                    StringBuilder builder = new StringBuilder(0x800);
                    builder.AppendFormat("Corrupt stdf file encountered {0}\n", this.FileProcessing);
                    builder.AppendFormat("\tLast Known Good Position 0x{0:X}\n", this.LastGoodFilePosition);
                    builder.AppendFormat("\tLast Known Good Record {0)\n", this.LastGoodSTDFRecord.ToString());
                    builder.AppendFormat("\tBeginning of Bad Record 0x{0:X}\n", this.BeginningOfBadRecord);
                    break;
                }
            }
            record.REC_LEN = num;
            record.RemainingBytes = num;
            record.RecordNumber = this.RecordCount += 1L;
            record.RECORD_TYPE = (STDFRecordType) num6;
            record.Processor = this.Processor;
            record.FilePos = position;
            record.Read(this.br);
            this.br.BaseStream.Position = (record.FilePos + record.REC_LEN) + 4L;
            this.LastGoodFilePosition = position;
            this.LastGoodSTDFRecord = record.RECORD_TYPE;
            if (this.FileLength > 0L)
            {
                this.Percent = (Convert.ToDouble(position) / Convert.ToDouble(this.FileLength)) * 100.0;
                return record;
            }
            this.Percent = 0.0;
            return record;
        }

        public STDFRecord ReadRecord(STDFRecordIndex RI)
        {
            STDFRecord record = null;
            record = this.ReadRecord(RI.FilePosition);
            record.RecordNumber = RI.RecordNumber;
            return record;
        }

        public STDFRecord ReadRecord(long position)
        {
            if (this.Mode != STDFFileMode.Read)
            {
                throw new Exception("Cannot Read a file opened for write.");
            }
            this.br.BaseStream.Position = position;
            return this.ReadRecord();
        }

        public static STDFRecordType RecordLookup(STDFRecordKeys rk)
        {
            if (rk == STDFRecordKeys.PTR)
            {
                return STDFRecordType.PTR;
            }
            if (rk == STDFRecordKeys.PIR)
            {
                return STDFRecordType.PIR;
            }
            if (rk == STDFRecordKeys.PRR)
            {
                return STDFRecordType.PRR;
            }
            if (rk == STDFRecordKeys.PMR)
            {
                return STDFRecordType.PMR;
            }
            if (rk == STDFRecordKeys.PGR)
            {
                return STDFRecordType.PGR;
            }
            if (rk == STDFRecordKeys.RDR)
            {
                return STDFRecordType.RDR;
            }
            if (rk == STDFRecordKeys.TSR)
            {
                return STDFRecordType.TSR;
            }
            if (rk == STDFRecordKeys.BPS)
            {
                return STDFRecordType.BPS;
            }
            if (rk == STDFRecordKeys.SDR)
            {
                return STDFRecordType.SDR;
            }
            if (rk == STDFRecordKeys.SBR)
            {
                return STDFRecordType.SBR;
            }
            if (rk == STDFRecordKeys.HBR)
            {
                return STDFRecordType.HBR;
            }
            if (rk == STDFRecordKeys.PLR)
            {
                return STDFRecordType.PLR;
            }
            if (rk == STDFRecordKeys.PCR)
            {
                return STDFRecordType.PCR;
            }
            if (rk == STDFRecordKeys.FTR)
            {
                return STDFRecordType.FTR;
            }
            if (rk == STDFRecordKeys.MPR)
            {
                return STDFRecordType.MPR;
            }
            if (rk == STDFRecordKeys.ATR)
            {
                return STDFRecordType.ATR;
            }
            if (rk == STDFRecordKeys.WRR)
            {
                return STDFRecordType.WRR;
            }
            if (rk == STDFRecordKeys.WCR)
            {
                return STDFRecordType.WCR;
            }
            if (rk == STDFRecordKeys.WIR)
            {
                return STDFRecordType.WIR;
            }
            if (rk == STDFRecordKeys.MIR)
            {
                return STDFRecordType.MIR;
            }
            if (rk == STDFRecordKeys.MRR)
            {
                return STDFRecordType.MRR;
            }
            if (rk == STDFRecordKeys.FAR)
            {
                return STDFRecordType.FAR;
            }
            if (rk == STDFRecordKeys.GDR)
            {
                return STDFRecordType.GDR;
            }
            if (rk == STDFRecordKeys.DTR)
            {
                return STDFRecordType.DTR;
            }
            if (rk == STDFRecordKeys.GDR)
            {
                return STDFRecordType.GDR;
            }
            if (rk == STDFRecordKeys.EPS)
            {
                return STDFRecordType.EPS;
            }
            return STDFRecordType.NAN;
        }

        public static STDFRecordKeys RecordLookup(STDFRecordType rt)
        {
            if (rt == STDFRecordType.PTR)
            {
                return STDFRecordKeys.PTR;
            }
            if (rt == STDFRecordType.PIR)
            {
                return STDFRecordKeys.PIR;
            }
            if (rt == STDFRecordType.PRR)
            {
                return STDFRecordKeys.PRR;
            }
            if (rt == STDFRecordType.PMR)
            {
                return STDFRecordKeys.PMR;
            }
            if (rt == STDFRecordType.PGR)
            {
                return STDFRecordKeys.PGR;
            }
            if (rt == STDFRecordType.RDR)
            {
                return STDFRecordKeys.RDR;
            }
            if (rt == STDFRecordType.TSR)
            {
                return STDFRecordKeys.TSR;
            }
            if (rt == STDFRecordType.BPS)
            {
                return STDFRecordKeys.BPS;
            }
            if (rt == STDFRecordType.SDR)
            {
                return STDFRecordKeys.SDR;
            }
            if (rt == STDFRecordType.SBR)
            {
                return STDFRecordKeys.SBR;
            }
            if (rt == STDFRecordType.HBR)
            {
                return STDFRecordKeys.HBR;
            }
            if (rt == STDFRecordType.PLR)
            {
                return STDFRecordKeys.PLR;
            }
            if (rt == STDFRecordType.PCR)
            {
                return STDFRecordKeys.PCR;
            }
            if (rt == STDFRecordType.ATR)
            {
                return STDFRecordKeys.ATR;
            }
            if (rt == STDFRecordType.FTR)
            {
                return STDFRecordKeys.FTR;
            }
            if (rt == STDFRecordType.MPR)
            {
                return STDFRecordKeys.MPR;
            }
            if (rt == STDFRecordType.WRR)
            {
                return STDFRecordKeys.WRR;
            }
            if (rt == STDFRecordType.WCR)
            {
                return STDFRecordKeys.WCR;
            }
            if (rt == STDFRecordType.WIR)
            {
                return STDFRecordKeys.WIR;
            }
            if (rt == STDFRecordType.MIR)
            {
                return STDFRecordKeys.MIR;
            }
            if (rt == STDFRecordType.MRR)
            {
                return STDFRecordKeys.MRR;
            }
            if (rt == STDFRecordType.FAR)
            {
                return STDFRecordKeys.FAR;
            }
            if (rt == STDFRecordType.GDR)
            {
                return STDFRecordKeys.GDR;
            }
            if (rt == STDFRecordType.DTR)
            {
                return STDFRecordKeys.DTR;
            }
            if (rt == STDFRecordType.GDR)
            {
                return STDFRecordKeys.GDR;
            }
            if (rt == STDFRecordType.EPS)
            {
                return STDFRecordKeys.EPS;
            }
            return STDFRecordKeys.NAN;
        }

        public static byte SetBit(byte b, int bit, bool value)
        {
            if (!value)
            {
                return (byte) (b & ~BitMask[bit]);
            }
            return (byte) (b | BitMask[bit]);
        }

        public void Write(STDFRecord stdfRecord)
        {
            if (this.Mode == STDFFileMode.Read)
            {
                throw new Exception("Cannot write to a file opened for Read");
            }
            this.RecordCount += 1L;
            stdfRecord.RecordNumber = this.RecordCount;
            long position = this.bw.BaseStream.Position;
            stdfRecord.FilePos = position;
            stdfRecord.REC_LEN = 0;
            this.bw.Write(stdfRecord.REC_LEN);
            stdfRecord.Write(this.bw);
            long num2 = this.bw.BaseStream.Position;
            stdfRecord.REC_LEN = Convert.ToUInt16((long) (num2 - position));
            this.bw.BaseStream.Position = position;
            this.bw.Write(Convert.ToUInt16((int) (stdfRecord.REC_LEN - 4)));
            this.bw.BaseStream.Position = num2;
        }
    }
}

