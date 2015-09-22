namespace STDFInterface
{
    using System;
    using System.IO;
    using System.Text;

    public class MIRRecord : STDFRecord
    {
        public string AUX_FILE;
        public ushort BURN_TIM;
        public byte CMOD_COD;
        public string DATE_COD;
        public string DSGN_REV;
        public string ENG_ID;
        public string EXEC_TYP;
        public string EXEC_VER;
        public string FACIL_ID;
        public string FAMILY_ID;
        public string FLOOR_ID;
        public string FLOW_ID;
        public string JOB_NAM;
        public string JOB_REV;
        public string LOT_ID;
        public byte MODE_COD;
        public string NODE_NAM;
        public string OPER_FRQ;
        public string OPER_NAM;
        public string PART_TYP;
        public string PKG_TYP;
        public string PROC_ID;
        public byte PROT_COD;
        public string ROM_COD;
        public byte RTST_COD;
        public string SBLOT_ID;
        public string SERL_NUM;
        public string SETUP_ID;
        public DateTime SETUP_T;
        public string SPEC_NAM;
        public string SPEC_VER;
        public DateTime START_T;
        public byte STAT_NUM;
        public string SUPR_NAM;
        public string TEST_COD;
        public string TST_TEMP;
        public string TSTR_TYP;
        public string USER_TXT;

        public MIRRecord() : base(STDFRecordType.MIR)
        {
        }

        public override int Read(BinaryReader br)
        {
            int num = 0;
            num += base.Read(br, out this.SETUP_T);
            num += base.Read(br, out this.START_T);
            num += base.Read(br, out this.STAT_NUM);
            num += base.Read(br, out this.MODE_COD);
            num += base.Read(br, out this.RTST_COD);
            num += base.Read(br, out this.PROT_COD);
            num += base.Read(br, out this.BURN_TIM);
            num += base.Read(br, out this.CMOD_COD);
            num += base.Read(br, out this.LOT_ID);
            num += base.Read(br, out this.PART_TYP);
            num += base.Read(br, out this.NODE_NAM);
            num += base.Read(br, out this.TSTR_TYP);
            num += base.Read(br, out this.JOB_NAM);
            num += base.Read(br, out this.JOB_REV);
            num += base.Read(br, out this.SBLOT_ID);
            num += base.Read(br, out this.OPER_NAM);
            num += base.Read(br, out this.EXEC_TYP);
            num += base.Read(br, out this.EXEC_VER);
            num += base.Read(br, out this.TEST_COD);
            num += base.Read(br, out this.TST_TEMP);
            num += base.Read(br, out this.USER_TXT);
            num += base.Read(br, out this.AUX_FILE);
            num += base.Read(br, out this.PKG_TYP);
            num += base.Read(br, out this.FAMILY_ID);
            num += base.Read(br, out this.DATE_COD);
            num += base.Read(br, out this.FACIL_ID);
            num += base.Read(br, out this.FLOOR_ID);
            num += base.Read(br, out this.PROC_ID);
            num += base.Read(br, out this.OPER_FRQ);
            num += base.Read(br, out this.SPEC_NAM);
            num += base.Read(br, out this.SPEC_VER);
            num += base.Read(br, out this.FLOW_ID);
            num += base.Read(br, out this.SETUP_ID);
            num += base.Read(br, out this.DSGN_REV);
            num += base.Read(br, out this.ENG_ID);
            num += base.Read(br, out this.ROM_COD);
            num += base.Read(br, out this.SERL_NUM);
            return (num + base.Read(br, out this.SUPR_NAM));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x800);
            builder.AppendFormat("MIR (Master Information Record): Record# {0}  FileOffset:{1} Record Length:{2}\n", base.RecordNumber, base.FilePos, base.REC_LEN);
            builder.AppendFormat("\t{0,-23}{1}\n", "SetupTime:", this.SETUP_T);
            builder.AppendFormat("\t{0,-23}{1}\n", "StartTime:", this.START_T);
            builder.AppendFormat("\t{0,-23}{1}\n", "TestStation:", this.STAT_NUM);
            builder.AppendFormat("\t{0,-23}{1}\n", "ModeCode:", Convert.ToChar(this.MODE_COD));
            builder.AppendFormat("\t{0,-23}{1}\n", "RetestCode:", Convert.ToChar(this.RTST_COD));
            builder.AppendFormat("\t{0,-23}{1}\n", "ProtectCode:", Convert.ToChar(this.PROT_COD));
            builder.AppendFormat("\t{0,-23}{1}\n", "BurnTime:", this.BURN_TIM);
            builder.AppendFormat("\t{0,-23}{1}\n", "CmdModeCode:", Convert.ToChar(this.CMOD_COD));
            builder.AppendFormat("\t{0,-23}{1}\n", "LotID:", this.LOT_ID);
            builder.AppendFormat("\t{0,-23}{1}\n", "PartType:", this.PART_TYP);
            builder.AppendFormat("\t{0,-23}{1}\n", "NodeName:", this.NODE_NAM);
            builder.AppendFormat("\t{0,-23}{1}\n", "TesterType:", this.TSTR_TYP);
            builder.AppendFormat("\t{0,-23}{1}\n", "TestProgName/JobName:", this.JOB_NAM);
            builder.AppendFormat("\t{0,-23}{1}\n", "TestProgRevision:", this.JOB_REV);
            builder.AppendFormat("\t{0,-23}{1}\n", "SubLotID:", this.SBLOT_ID);
            builder.AppendFormat("\t{0,-23}{1}\n", "OperatorName:", this.OPER_NAM);
            builder.AppendFormat("\t{0,-23}{1}\n", "TestExecSWType:", this.EXEC_TYP);
            builder.AppendFormat("\t{0,-23}{1}\n", "TestExecSWVer:", this.EXEC_VER);
            builder.AppendFormat("\t{0,-23}{1}\n", "TestCode:", this.TEST_COD);
            builder.AppendFormat("\t{0,-23}{1}\n", "TestTemperature:", this.TST_TEMP);
            builder.AppendFormat("\t{0,-23}{1}\n", "GenericUsrTxt:", this.USER_TXT);
            builder.AppendFormat("\t{0,-23}{1}\n", "AuxFile:", this.AUX_FILE);
            builder.AppendFormat("\t{0,-23}{1}\n", "PkgType:", this.PKG_TYP);
            builder.AppendFormat("\t{0,-23}{1}\n", "ProductFamily:", this.FAMILY_ID);
            builder.AppendFormat("\t{0,-23}{1}\n", "DateCode:", this.DATE_COD);
            builder.AppendFormat("\t{0,-23}{1}\n", "FacilityID:", this.FACIL_ID);
            builder.AppendFormat("\t{0,-23}{1}\n", "FloorID:", this.FLOOR_ID);
            builder.AppendFormat("\t{0,-23}{1}\n", "ProcessID:", this.PROC_ID);
            builder.AppendFormat("\t{0,-23}{1}\n", "OperationStep:", this.OPER_FRQ);
            builder.AppendFormat("\t{0,-23}{1}\n", "SpecificationName:", this.SPEC_NAM);
            builder.AppendFormat("\t{0,-23}{1}\n", "SpecificationVersion:", this.SPEC_VER);
            builder.AppendFormat("\t{0,-23}{1}\n", "TestFlowID:", this.FLOW_ID);
            builder.AppendFormat("\t{0,-23}{1}\n", "SetupID:", this.SETUP_ID);
            builder.AppendFormat("\t{0,-23}{1}\n", "DesignRevision:", this.DSGN_REV);
            builder.AppendFormat("\t{0,-23}{1}\n", "Engineering LotID:", this.ENG_ID);
            builder.AppendFormat("\t{0,-23}{1}\n", "RomCodeID:", this.ROM_COD);
            builder.AppendFormat("\t{0,-23}{1}\n", "TesterSerial#:", this.SERL_NUM);
            builder.AppendFormat("\t{0,-23}{1}\n", "Supervisior Name/ID:", this.SUPR_NAM);
            return builder.ToString();
        }

        public override int Write(BinaryWriter bw)
        {
            ushort num = Convert.ToUInt16(STDFRecordType.MIR);
            bw.Write(num);
            base.WriteTime(bw, this.SETUP_T);
            base.WriteTime(bw, this.START_T);
            bw.Write(this.STAT_NUM);
            bw.Write(this.MODE_COD);
            bw.Write(this.RTST_COD);
            bw.Write(this.PROT_COD);
            bw.Write(this.BURN_TIM);
            bw.Write(this.CMOD_COD);
            base.WriteString(bw, this.LOT_ID);
            base.WriteString(bw, this.PART_TYP);
            base.WriteString(bw, this.NODE_NAM);
            base.WriteString(bw, this.TSTR_TYP);
            base.WriteString(bw, this.JOB_NAM);
            base.WriteString(bw, this.JOB_REV);
            base.WriteString(bw, this.SBLOT_ID);
            base.WriteString(bw, this.OPER_NAM);
            base.WriteString(bw, this.EXEC_TYP);
            base.WriteString(bw, this.EXEC_VER);
            base.WriteString(bw, this.TEST_COD);
            base.WriteString(bw, this.TST_TEMP);
            base.WriteString(bw, this.USER_TXT);
            base.WriteString(bw, this.AUX_FILE);
            base.WriteString(bw, this.PKG_TYP);
            base.WriteString(bw, this.FAMILY_ID);
            base.WriteString(bw, this.DATE_COD);
            base.WriteString(bw, this.FACIL_ID);
            base.WriteString(bw, this.FLOOR_ID);
            base.WriteString(bw, this.PROC_ID);
            base.WriteString(bw, this.OPER_FRQ);
            base.WriteString(bw, this.SPEC_NAM);
            base.WriteString(bw, this.SPEC_VER);
            base.WriteString(bw, this.FLOW_ID);
            base.WriteString(bw, this.SETUP_ID);
            base.WriteString(bw, this.DSGN_REV);
            base.WriteString(bw, this.ENG_ID);
            base.WriteString(bw, this.ROM_COD);
            base.WriteString(bw, this.SERL_NUM);
            base.WriteString(bw, this.SUPR_NAM);
            return 0;
        }
    }
}

