namespace STDF_V4
{
    using System;

    [Flags]
    public enum STDFRecordKeys
    {
        ALL = 0x1ffffff,
        ATR = 2,
        BPS = 0x200000,
        DTR = 0x1000000,
        EPS = 0x400000,
        FAR = 1,
        FTR = 0x100000,
        GDR = 0x800000,
        HBR = 0x20,
        MIR = 4,
        MPR = 0x80000,
        MRR = 8,
        NAN = 0,
        PCR = 0x10,
        PGR = 0x100,
        PIR = 0x8000,
        PLR = 0x200,
        PMR = 0x80,
        PRR = 0x10000,
        PTR = 0x40000,
        RDR = 0x400,
        SBR = 0x40,
        SDR = 0x800,
        TSR = 0x20000,
        WCR = 0x4000,
        WIR = 0x1000,
        WRR = 0x2000
    }
}

