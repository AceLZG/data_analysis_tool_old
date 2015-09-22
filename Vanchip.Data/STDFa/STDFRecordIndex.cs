namespace STDFInterface
{
    using System;

    public class STDFRecordIndex
    {
        public long FilePosition;
        public STDFRecordKeys Key;
        public ushort RecordLength;
        public uint RecordNumber;
        public STDFRecordType RecordType;

        public STDFRecordIndex(STDFRecordType rType, ushort rLength, long fPosition, uint rNumber, STDFRecordKeys rKey)
        {
            this.RecordType = rType;
            this.RecordLength = rLength;
            this.FilePosition = fPosition;
            this.RecordNumber = rNumber;
            this.Key = rKey;
        }
    }
}

