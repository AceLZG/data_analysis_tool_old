///     Reversion history 
///     Rev1.0          Initial build by Weiyu Zong                                                             Ace Li      2016-05-21
///     Rev1.1          Motified output datatable format                                                        Ace Li      2016-11-18
///     Rev1.2.0.1      Support gz format                                                                       Ace Li      2016-11-18




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.IO.Compression;
using System.Data;

namespace STDF_Viewer
{
    public class Record
    {
        System.Data.DataTable dataTable;
        System.Data.DataTable headerTable;
        List<string> lMsg = new List<string>();

        public void AnalyzeFile(string filename)
        {
            dataTable = new System.Data.DataTable();
            dataTable.Columns.Add("Row Index");
            dataTable.Columns.Add("Site Number");
            dataTable.Columns.Add("Soft Bin");
            dataTable.Columns.Add("Hard Bin");
            dataTable.Columns.Add("Device ID");

            headerTable = new System.Data.DataTable();
            headerTable.Columns.Add("Field Name");
            headerTable.Columns.Add("Values");

            FAR far = new FAR();
            MIR mir = new MIR();
            SDR sdr = new SDR();
            PMR pmr = new PMR();
            PIR pir = new PIR();
            PTR ptr = new PTR();
            MRR mrr = new MRR();
            PRR prr = new PRR();
            PCR pcr = new PCR();

            using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[4];
                byte[] result;

                int rowIndex = 1;
                List<object[]> dataList = new List<object[]>();
                List<PTR> ptrList = new List<PTR>();

                while (fileStream.Read(buffer, 0, buffer.Length) > 0)
                {
                    HeadAnalyze(buffer);
                    result = new byte[REC_LEN];
                    fileStream.Read(result, 0, REC_LEN);

                    //string s = null;

                    if (REC_TYP == 0 && REC_SUB == 10)
                    {
                        far.Initial(result);
                    }
                    else if (REC_TYP == 1 && REC_SUB == 10) mir.Initial(result);
                    else if (REC_TYP == 1 && REC_SUB == 80) sdr.Initial(result);
                    //else if (REC_TYP == 1 && REC_SUB == 60) pmr.Initial(result);
                    //PIR
                    else if (REC_TYP == 5 && REC_SUB == 10)
                    {
                        pir.Initial(result);
                        dataList.Clear();
                    }
                    //PTR
                    else if (REC_TYP == 15 && REC_SUB == 10)
                    {
                        ptr.Initial1(result);
                        dataList.Add(new object[] { ptr.SITE_NUM, ptr.RESULT });

                        if (rowIndex == 1)
                        {
                            PTR ptr1 = new PTR();
                            ptr1.Initial(result);
                            ptrList.Add(ptr1);    
                        }
                        
                    }
                    //PRR
                    else if (REC_TYP == 5 && REC_SUB == 20)
                    {
                        prr.Initial(result);

                        if (rowIndex == 1)
                        {
                            List<PTR> _site = new List<PTR>();

                            for (int i = 0; i < ptrList.Count; i++)
                            {
                                if (ptrList[i].SITE_NUM == prr.SITE_NUM)
                                {
                                    _site.Add(ptrList[i]);
                                }
                            }

                            int offset = 5;
                            object[] Units = new object[offset + _site.Count];
                            object[] lowLimit = new object[offset + _site.Count];
                            object[] highLimit = new object[offset + _site.Count];

                            for (int i = 0; i < _site.Count; i++)
                            {
                                if (dataTable.Columns.Count < _site.Count + 5)
                                {
                                    dataTable.Columns.Add(_site[i].TEST_TXT.TrimEnd('/'));
                                }
                                Units[offset + i] = _site[i].UNITS;
                                lowLimit[offset + i] = _site[i].LO_LIMIT;
                                highLimit[offset + i] = _site[i].HI_LIMIT;
                            }
                            dataTable.Columns.Add("Status");
                            dataTable.Rows.Add(Units);
                            dataTable.Rows.Add(lowLimit);
                            dataTable.Rows.Add(highLimit);
                            ptrList.Clear();
                        }

                        List<object> objArray = new List<object>();
                        objArray.Add(rowIndex);
                        objArray.Add(prr.SITE_NUM);
                        objArray.Add(prr.SOFT_BIN);
                        objArray.Add(prr.HARD_BIN);
                        objArray.Add(prr.PART_ID);

                        for (int i = 0; i < dataList.Count; i++)
                        {
                            if ((int)dataList[i][0] == prr.SITE_NUM)
                            {
                                objArray.Add(dataList[i][1]);
                            }
                        }
                        objArray.Add(prr.PART_FLG);

                        dataTable.Rows.Add(objArray.ToArray());

                        //int ii = 0;
                        //DataRow nerow = dataTable.NewRow();
                        //foreach (var t in objArray)
                        //{
                        //    nerow[ii++] = t;
                        //}
                        //dataTable.Rows.Add(nerow);

                        rowIndex += 1;
                    }
                    else if (REC_TYP == 10 && REC_SUB == 30)
                    {
                        // TSR
                    }
                    else if (REC_TYP == 1 && REC_SUB == 40)
                    {
                        // HBR
                    }
                    else if (REC_TYP == 1 && REC_SUB == 50)
                    {
                        // SBR
                    }
                    else if (REC_TYP == 1 && REC_SUB == 30)
                    {
                        // PCR
                        pcr.Initial(result);
                    }
                    else if (REC_TYP == 1 && REC_SUB == 20)
                    {
                        // MRR
                        mrr.Initial(result);
                    }
                }
            }
            
            headerTable.Rows.Add(new object[] { "FAR.CPU_TYPE", far.CPU_TYPE });
            headerTable.Rows.Add(new object[] { "FAR.STDF_VER", far.STDF_VER });

            headerTable.Rows.Add(new object[] { "MIR.SETUP_T", mir.SETUP_T });
            headerTable.Rows.Add(new object[] { "MIR.START_T", mir.START_T });
            headerTable.Rows.Add(new object[] { "MIR.STAT_NUM", mir.STAT_NUM });
            headerTable.Rows.Add(new object[] { "MIR.MODE_COD", mir.MODE_COD });
            headerTable.Rows.Add(new object[] { "MIR.RTST_COD ", mir.RTST_COD });
            headerTable.Rows.Add(new object[] { "MIR.PROT_COD ", mir.PROT_COD });
            headerTable.Rows.Add(new object[] { "MIR.BURN_TIM ", mir.BURN_TIM });
            headerTable.Rows.Add(new object[] { "MIR.CMOD_COD ", mir.CMOD_COD });
            headerTable.Rows.Add(new object[] { "MIR.LOT_ID ", mir.LOT_ID });
            headerTable.Rows.Add(new object[] { "MIR.PART_TYP ", mir.PART_TYP });
            headerTable.Rows.Add(new object[] { "MIR.NODE_NAM ", mir.NODE_NAM });
            headerTable.Rows.Add(new object[] { "MIR.TSTR_TYP ", mir.TSTR_TYP });
            headerTable.Rows.Add(new object[] { "MIR.JOB_NAM ", mir.JOB_NAM });
            headerTable.Rows.Add(new object[] { "MIR.JOB_REV ", mir.JOB_REV });
            headerTable.Rows.Add(new object[] { "MIR.SBLOT_ID ", mir.SBLOT_ID });
            headerTable.Rows.Add(new object[] { "MIR.OPER_NAM ", mir.OPER_NAM });
            headerTable.Rows.Add(new object[] { "MIR.EXEC_TYP ", mir.EXEC_TYP });
            headerTable.Rows.Add(new object[] { "MIR.EXEC_VER ", mir.EXEC_VER });
            headerTable.Rows.Add(new object[] { "MIR.TEST_COD ", mir.TEST_COD });
            headerTable.Rows.Add(new object[] { "MIR.TST_TEMP ", mir.TST_TEMP });
            headerTable.Rows.Add(new object[] { "MIR.USER_TXT ", mir.USER_TXT });
            headerTable.Rows.Add(new object[] { "MIR.AUX_FILE ", mir.AUX_FILE });
            headerTable.Rows.Add(new object[] { "MIR.PKG_TYP ", mir.PKG_TYP });
            headerTable.Rows.Add(new object[] { "MIR.FAMLY_ID ", mir.FAMLY_ID });
            headerTable.Rows.Add(new object[] { "MIR.DATE_COD ", mir.DATE_COD });
            headerTable.Rows.Add(new object[] { "MIR.FACIL_ID ", mir.FACIL_ID });
            headerTable.Rows.Add(new object[] { "MIR.FLOOR_ID ", mir.FLOOR_ID });
            headerTable.Rows.Add(new object[] { "MIR.PROC_ID ", mir.PROC_ID });
            headerTable.Rows.Add(new object[] { "MIR.OPER_FRQ ", mir.OPER_FRQ });
            headerTable.Rows.Add(new object[] { "MIR.SPEC_NAM ", mir.SPEC_NAM });
            headerTable.Rows.Add(new object[] { "MIR.SPEC_VER ", mir.SPEC_VER });
            headerTable.Rows.Add(new object[] { "MIR.FLOW_ID ", mir.FLOW_ID });
            headerTable.Rows.Add(new object[] { "MIR.SETUP_ID ", mir.SETUP_ID });
            headerTable.Rows.Add(new object[] { "MIR.DSGN_REV ", mir.DSGN_REV });
            headerTable.Rows.Add(new object[] { "MIR.ENG_ID ", mir.ENG_ID });
            headerTable.Rows.Add(new object[] { "MIR.ROM_COD ", mir.ROM_COD });
            headerTable.Rows.Add(new object[] { "MIR.SERL_NUM ", mir.SERL_NUM });
            headerTable.Rows.Add(new object[] { "MIR.SUPR_NAM ", mir.SUPR_NAM });

            headerTable.Rows.Add(new object[] { "SDR.HEAD_NUM", sdr.HEAD_NUM });
            headerTable.Rows.Add(new object[] { "SDR.SITE_GRP", sdr.SITE_GRP });
            headerTable.Rows.Add(new object[] { "SDR.SITE_CNT", sdr.SITE_CNT });
            headerTable.Rows.Add(new object[] { "SDR.SITE_NUM", sdr.SITE_NUM });
            headerTable.Rows.Add(new object[] { "SDR.HAND_TYP", sdr.HAND_TYP });
            headerTable.Rows.Add(new object[] { "SDR.HAND_ID", sdr.HAND_ID });
            headerTable.Rows.Add(new object[] { "SDR.CARD_TYP", sdr.CARD_TYP });
            headerTable.Rows.Add(new object[] { "SDR.CARD_ID", sdr.CARD_ID });
            headerTable.Rows.Add(new object[] { "SDR.LOAD_TYP", sdr.LOAD_TYP });
            headerTable.Rows.Add(new object[] { "SDR.LOAD_ID", sdr.LOAD_ID });
            headerTable.Rows.Add(new object[] { "SDR.DIB_TYP", sdr.DIB_TYP });
            headerTable.Rows.Add(new object[] { "SDR.DIB_ID", sdr.DIB_ID });
            headerTable.Rows.Add(new object[] { "SDR.CABL_TYP", sdr.CABL_TYP });
            headerTable.Rows.Add(new object[] { "SDR.CABL_ID", sdr.CABL_ID });
            headerTable.Rows.Add(new object[] { "SDR.CONT_TYP", sdr.CONT_TYP });
            headerTable.Rows.Add(new object[] { "SDR.CONT_ID", sdr.CONT_ID });
            headerTable.Rows.Add(new object[] { "SDR.LASR_TYP", sdr.LASR_TYP });
            headerTable.Rows.Add(new object[] { "SDR.LASR_ID", sdr.LASR_ID });
            headerTable.Rows.Add(new object[] { "SDR.EXTR_TYP", sdr.EXTR_TYP });
            headerTable.Rows.Add(new object[] { "SDR.EXTR_ID", sdr.EXTR_ID });

            headerTable.Rows.Add(new object[] { "PCR.HEAD_NUM", pcr.HEAD_NUM });
            headerTable.Rows.Add(new object[] { "PCR.SITE_NUM", pcr.SITE_NUM });
            headerTable.Rows.Add(new object[] { "PCR.PART_CNT", pcr.PART_CNT });
            headerTable.Rows.Add(new object[] { "PCR.RTST_CNT", pcr.RTST_CNT });
            headerTable.Rows.Add(new object[] { "PCR.ABRT_CNT", pcr.ABRT_CNT });
            headerTable.Rows.Add(new object[] { "PCR.GOOD_CNT", pcr.GOOD_CNT });
            headerTable.Rows.Add(new object[] { "PCR.FUNC_CNT", pcr.FUNC_CNT });
            //headerTable.Rows.Add(new object[] { "FailQuantity", pcr.PART_CNT - pcr.GOOD_CNT });
            //headerTable.Rows.Add(new object[] { "Yield", 100 * pcr.GOOD_CNT / pcr.PART_CNT });

            headerTable.Rows.Add(new object[] { "MRR.DISP_COD", mrr.DISP_COD });
            headerTable.Rows.Add(new object[] { "MRR.USR_DESC", mrr.USR_DESC });
            headerTable.Rows.Add(new object[] { "MRR.EXC_DESC", mrr.EXC_DESC });
            headerTable.Rows.Add(new object[] { "MRR.FINISH_T", mrr.FINISH_T });

            
        }

        public void AnalyzeFile_raw(Stream stream)
        {
            #region --- Variable ---
            dataTable = new System.Data.DataTable();
            dataTable.Columns.Add("Row Index");
            dataTable.Columns.Add("Site Number");
            dataTable.Columns.Add("Soft Bin");
            dataTable.Columns.Add("Hard Bin");
            dataTable.Columns.Add("Device ID");

            headerTable = new System.Data.DataTable();
            headerTable.Columns.Add("Field Name");
            headerTable.Columns.Add("Values");

            FAR far = new FAR();
            MIR mir = new MIR();
            SDR sdr = new SDR();
            PMR pmr = new PMR();
            PIR pir = new PIR();
            PTR ptr = new PTR();
            MRR mrr = new MRR();
            PRR prr = new PRR();
            PCR pcr = new PCR();

            byte[] buffer = new byte[4];
            byte[] result;

            int rowIndex = 1;
            List<object[]> dataList = new List<object[]>();
            List<PTR> ptrList = new List<PTR>();
            #endregion --- Variable ---

            //stream.Position = 0;
            while ((stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                HeadAnalyze(buffer);
                result = new byte[REC_LEN];
                stream.Read(result, 0, REC_LEN);
                switch (REC_TYP)
                {
                    case 0:
                        if (REC_SUB == 10) far.Initial(result);                 //FAR
                        break;
                    case 1:
                        if (REC_SUB == 10) mir.Initial(result);                 //MIR
                        else if (REC_SUB == 80) sdr.Initial(result);            //SDR
                        //else if (REC_SUB == 60) pmr.Initial(result);            //PMR
                        else if (REC_SUB == 30) pcr.Initial(result);            //PCR
                        else if (REC_SUB == 20) mrr.Initial(result);            //MRR
                        break;
                    case 15:
                        #region --- PTR ---
                        if (REC_SUB == 10)                                      //PTR
                        {
                            ptr.Initial1(result);
                            dataList.Add(new object[] { ptr.SITE_NUM, ptr.RESULT });

                            if (rowIndex == 1)
                            {
                                PTR ptr1 = new PTR();
                                ptr1.Initial(result);
                                ptrList.Add(ptr1);
                            }
                        }
                        #endregion --- PTR ---
                        break;
                    case 5:
                        if (REC_SUB == 10)                                      //PIR
                        {
                            pir.Initial(result);
                            dataList.Clear();
                        }
                        #region --- PRR ---
                        else if (REC_SUB == 20)                                 //PRR
                        {
                            prr.Initial(result);

                            if (rowIndex == 1)
                            {
                                List<PTR> _site = new List<PTR>();

                                for (int i = 0; i < ptrList.Count; i++)
                                {
                                    if (ptrList[i].SITE_NUM == prr.SITE_NUM)
                                    {
                                        _site.Add(ptrList[i]);
                                    }
                                }

                                int offset = 5;
                                object[] Units = new object[offset + _site.Count];
                                object[] lowLimit = new object[offset + _site.Count];
                                object[] highLimit = new object[offset + _site.Count];

                                for (int i = 0; i < _site.Count; i++)
                                {
                                    if (dataTable.Columns.Count < _site.Count + 5)
                                    {
                                        dataTable.Columns.Add(_site[i].TEST_TXT.TrimEnd('/'));
                                    }
                                    Units[offset + i] = _site[i].UNITS;
                                    lowLimit[offset + i] = _site[i].LO_LIMIT;
                                    highLimit[offset + i] = _site[i].HI_LIMIT;
                                }
                                dataTable.Columns.Add("Status");
                                dataTable.Rows.Add(Units);
                                dataTable.Rows.Add(lowLimit);
                                dataTable.Rows.Add(highLimit);
                                ptrList.Clear();
                            }

                            List<object> objArray = new List<object>();
                            objArray.Add(rowIndex);
                            objArray.Add(prr.SITE_NUM);
                            objArray.Add(prr.SOFT_BIN);
                            objArray.Add(prr.HARD_BIN);
                            objArray.Add(prr.PART_ID);

                            for (int i = 0; i < dataList.Count; i++)
                            {
                                if ((int)dataList[i][0] == prr.SITE_NUM)
                                {
                                    objArray.Add(dataList[i][1]);
                                }
                            }
                            objArray.Add(prr.PART_FLG);

                            dataTable.Rows.Add(objArray.ToArray());

                            rowIndex += 1;
                        }
                        #endregion --- PRR ---
                        break;
                    default:
                        break;
                }
            }

            #region --- Header ---
            headerTable.Rows.Add(new object[] { "FAR.CPU_TYPE", far.CPU_TYPE });
            headerTable.Rows.Add(new object[] { "FAR.STDF_VER", far.STDF_VER });

            headerTable.Rows.Add(new object[] { "MIR.SETUP_T", mir.SETUP_T });
            headerTable.Rows.Add(new object[] { "MIR.START_T", mir.START_T });
            headerTable.Rows.Add(new object[] { "MIR.STAT_NUM", mir.STAT_NUM });
            headerTable.Rows.Add(new object[] { "MIR.MODE_COD", mir.MODE_COD });
            headerTable.Rows.Add(new object[] { "MIR.RTST_COD ", mir.RTST_COD });
            headerTable.Rows.Add(new object[] { "MIR.PROT_COD ", mir.PROT_COD });
            headerTable.Rows.Add(new object[] { "MIR.BURN_TIM ", mir.BURN_TIM });
            headerTable.Rows.Add(new object[] { "MIR.CMOD_COD ", mir.CMOD_COD });
            headerTable.Rows.Add(new object[] { "MIR.LOT_ID ", mir.LOT_ID });
            headerTable.Rows.Add(new object[] { "MIR.PART_TYP ", mir.PART_TYP });
            headerTable.Rows.Add(new object[] { "MIR.NODE_NAM ", mir.NODE_NAM });
            headerTable.Rows.Add(new object[] { "MIR.TSTR_TYP ", mir.TSTR_TYP });
            headerTable.Rows.Add(new object[] { "MIR.JOB_NAM ", mir.JOB_NAM });
            headerTable.Rows.Add(new object[] { "MIR.JOB_REV ", mir.JOB_REV });
            headerTable.Rows.Add(new object[] { "MIR.SBLOT_ID ", mir.SBLOT_ID });
            headerTable.Rows.Add(new object[] { "MIR.OPER_NAM ", mir.OPER_NAM });
            headerTable.Rows.Add(new object[] { "MIR.EXEC_TYP ", mir.EXEC_TYP });
            headerTable.Rows.Add(new object[] { "MIR.EXEC_VER ", mir.EXEC_VER });
            headerTable.Rows.Add(new object[] { "MIR.TEST_COD ", mir.TEST_COD });
            headerTable.Rows.Add(new object[] { "MIR.TST_TEMP ", mir.TST_TEMP });
            headerTable.Rows.Add(new object[] { "MIR.USER_TXT ", mir.USER_TXT });
            headerTable.Rows.Add(new object[] { "MIR.AUX_FILE ", mir.AUX_FILE });
            headerTable.Rows.Add(new object[] { "MIR.PKG_TYP ", mir.PKG_TYP });
            headerTable.Rows.Add(new object[] { "MIR.FAMLY_ID ", mir.FAMLY_ID });
            headerTable.Rows.Add(new object[] { "MIR.DATE_COD ", mir.DATE_COD });
            headerTable.Rows.Add(new object[] { "MIR.FACIL_ID ", mir.FACIL_ID });
            headerTable.Rows.Add(new object[] { "MIR.FLOOR_ID ", mir.FLOOR_ID });
            headerTable.Rows.Add(new object[] { "MIR.PROC_ID ", mir.PROC_ID });
            headerTable.Rows.Add(new object[] { "MIR.OPER_FRQ ", mir.OPER_FRQ });
            headerTable.Rows.Add(new object[] { "MIR.SPEC_NAM ", mir.SPEC_NAM });
            headerTable.Rows.Add(new object[] { "MIR.SPEC_VER ", mir.SPEC_VER });
            headerTable.Rows.Add(new object[] { "MIR.FLOW_ID ", mir.FLOW_ID });
            headerTable.Rows.Add(new object[] { "MIR.SETUP_ID ", mir.SETUP_ID });
            headerTable.Rows.Add(new object[] { "MIR.DSGN_REV ", mir.DSGN_REV });
            headerTable.Rows.Add(new object[] { "MIR.ENG_ID ", mir.ENG_ID });
            headerTable.Rows.Add(new object[] { "MIR.ROM_COD ", mir.ROM_COD });
            headerTable.Rows.Add(new object[] { "MIR.SERL_NUM ", mir.SERL_NUM });
            headerTable.Rows.Add(new object[] { "MIR.SUPR_NAM ", mir.SUPR_NAM });

            headerTable.Rows.Add(new object[] { "SDR.HEAD_NUM", sdr.HEAD_NUM });
            headerTable.Rows.Add(new object[] { "SDR.SITE_GRP", sdr.SITE_GRP });
            headerTable.Rows.Add(new object[] { "SDR.SITE_CNT", sdr.SITE_CNT });
            headerTable.Rows.Add(new object[] { "SDR.SITE_NUM", sdr.SITE_NUM });
            headerTable.Rows.Add(new object[] { "SDR.HAND_TYP", sdr.HAND_TYP });
            headerTable.Rows.Add(new object[] { "SDR.HAND_ID", sdr.HAND_ID });
            headerTable.Rows.Add(new object[] { "SDR.CARD_TYP", sdr.CARD_TYP });
            headerTable.Rows.Add(new object[] { "SDR.CARD_ID", sdr.CARD_ID });
            headerTable.Rows.Add(new object[] { "SDR.LOAD_TYP", sdr.LOAD_TYP });
            headerTable.Rows.Add(new object[] { "SDR.LOAD_ID", sdr.LOAD_ID });
            headerTable.Rows.Add(new object[] { "SDR.DIB_TYP", sdr.DIB_TYP });
            headerTable.Rows.Add(new object[] { "SDR.DIB_ID", sdr.DIB_ID });
            headerTable.Rows.Add(new object[] { "SDR.CABL_TYP", sdr.CABL_TYP });
            headerTable.Rows.Add(new object[] { "SDR.CABL_ID", sdr.CABL_ID });
            headerTable.Rows.Add(new object[] { "SDR.CONT_TYP", sdr.CONT_TYP });
            headerTable.Rows.Add(new object[] { "SDR.CONT_ID", sdr.CONT_ID });
            headerTable.Rows.Add(new object[] { "SDR.LASR_TYP", sdr.LASR_TYP });
            headerTable.Rows.Add(new object[] { "SDR.LASR_ID", sdr.LASR_ID });
            headerTable.Rows.Add(new object[] { "SDR.EXTR_TYP", sdr.EXTR_TYP });
            headerTable.Rows.Add(new object[] { "SDR.EXTR_ID", sdr.EXTR_ID });

            headerTable.Rows.Add(new object[] { "PCR.HEAD_NUM", pcr.HEAD_NUM });
            headerTable.Rows.Add(new object[] { "PCR.SITE_NUM", pcr.SITE_NUM });
            headerTable.Rows.Add(new object[] { "PCR.PART_CNT", pcr.PART_CNT });
            headerTable.Rows.Add(new object[] { "PCR.RTST_CNT", pcr.RTST_CNT });
            headerTable.Rows.Add(new object[] { "PCR.ABRT_CNT", pcr.ABRT_CNT });
            headerTable.Rows.Add(new object[] { "PCR.GOOD_CNT", pcr.GOOD_CNT });
            headerTable.Rows.Add(new object[] { "PCR.FUNC_CNT", pcr.FUNC_CNT });
            //headerTable.Rows.Add(new object[] { "FailQuantity", pcr.PART_CNT - pcr.GOOD_CNT });
            //headerTable.Rows.Add(new object[] { "Yield", 100 * pcr.GOOD_CNT / pcr.PART_CNT });

            headerTable.Rows.Add(new object[] { "MRR.DISP_COD", mrr.DISP_COD });
            headerTable.Rows.Add(new object[] { "MRR.USR_DESC", mrr.USR_DESC });
            headerTable.Rows.Add(new object[] { "MRR.EXC_DESC", mrr.EXC_DESC });
            headerTable.Rows.Add(new object[] { "MRR.FINISH_T", mrr.FINISH_T });
            #endregion --- Header ---

        }

        public void AnalyzeFile(Stream stream)
        {
            #region --- Variable ---
            dataTable = new System.Data.DataTable();
            dataTable.Columns.Add("Device#"); dataTable.Columns.Add("1"); dataTable.Columns.Add("2"); dataTable.Columns.Add("3"); dataTable.Columns.Add("4");

            headerTable = new System.Data.DataTable(); 
            headerTable.Columns.Add("Field Name"); headerTable.Columns.Add("Values");

            // test text(parameter) , scale number
            Dictionary<string, int> dic_scale = new Dictionary<string, int>();

            FAR far = new FAR();
            MIR mir = new MIR();
            SDR sdr = new SDR();
            PMR pmr = new PMR();
            PIR pir = new PIR();
            PTR ptr = new PTR();
            MRR mrr = new MRR();
            PRR prr = new PRR();
            PCR pcr = new PCR();

            byte[] buffer = new byte[4];
            byte[] result;
            
            bool dual_site = false;

            int rowIndex = 1;
            List<object[]> dataList = new List<object[]>();
            List<PTR> ptrList = new List<PTR>();

            int intTestNum = 0;
            #endregion --- Variable ---

            while ((stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                HeadAnalyze(buffer);
                result = new byte[REC_LEN];
                stream.Read(result, 0, REC_LEN);
                switch (REC_TYP)
                {
                    case 0:
                        if (REC_SUB == 10) far.Initial(result);                 //FAR
                        break;
                    case 1:
                        if (REC_SUB == 10) mir.Initial(result);                 //MIR
                        else if (REC_SUB == 80) sdr.Initial(result);            //SDR
                        //else if (REC_SUB == 60) pmr.Initial(result);            //PMR
                        else if (REC_SUB == 30) pcr.Initial(result);            //PCR
                        else if (REC_SUB == 20) mrr.Initial(result);            //MRR
                        break;
                    case 15:
                        #region --- PTR ---
                        if (REC_SUB == 10)                                      //PTR
                        {
                            ptr.Initial1(result);
                            dataList.Add(new object[] { ptr.SITE_NUM, ptr.RESULT });

                            if (rowIndex == 1)
                            {
                                PTR ptr1 = new PTR();
                                ptr1.Initial(result);
                                ptrList.Add(ptr1);
                                try
                                {
                                    if (!dic_scale.ContainsKey(ptr1.TEST_TXT.TrimEnd('/')) && ptr1.RES_SCAL != 0) dic_scale.Add(ptr1.TEST_TXT.TrimEnd('/'), ptr1.RES_SCAL);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message);
                                }
                            }
                        }
                        #endregion --- PTR ---
                        break;
                    case 5:
                        if (REC_SUB == 10)                                      //PIR
                        {
                            pir.Initial(result);
                            dataList.Clear();
                        }
                        #region --- PRR ---
                        else if (REC_SUB == 20)                                 //PRR
                        {
                            prr.Initial(result);
                            try
                            {
                                #region // Build Data Table
                                if (rowIndex == 1)
                                {
                                    intTestNum = prr.NUM_TEST;
                                    List<PTR> _site = new List<PTR>();

                                    for (int i = 0; i < ptrList.Count; i++)
                                    {
                                        if (ptrList[i].SITE_NUM == prr.SITE_NUM)
                                        {
                                            _site.Add(ptrList[i]);
                                        }
                                    }

                                    int offset = 5;
                                    object[] Parameters = new object[offset + _site.Count + 1];
                                    object[] Units = new object[offset + _site.Count + 1];
                                    object[] lowLimit = new object[offset + _site.Count + 1];
                                    object[] highLimit = new object[offset + _site.Count + 1];
                                    Parameters[0] = "Test Item"; Parameters[1] = "Site No"; Parameters[2] = "Soft Bin"; Parameters[3] = "Hard Bin"; Parameters[4] = "Device ID";
                                    Units[0] = "Unit";
                                    lowLimit[0] = "Low Limit";
                                    highLimit[0] = "High Limit";

                                    for (int i = 0; i < _site.Count; i++)
                                    {
                                        if (dataTable.Columns.Count < _site.Count + offset)
                                        {
                                            dataTable.Columns.Add((i + offset).ToString());
                                        }

                                        string key = _site[i].TEST_TXT.TrimEnd('/');
                                        Parameters[offset + i] = key;

                                        if (dic_scale.ContainsKey(key))
                                        {
                                            Units[offset + i] = new_unit(_site[i].UNITS, dic_scale[key]);
                                            lowLimit[offset + i] = float.Parse(new_double(_site[i].LO_LIMIT, dic_scale[key]).ToString("f3"));
                                            highLimit[offset + i] = float.Parse(new_double(_site[i].HI_LIMIT, dic_scale[key]).ToString("f3"));
                                        }
                                        else
                                        {
                                            Units[offset + i] = _site[i].UNITS;
                                            lowLimit[offset + i] = float.Parse(_site[i].LO_LIMIT.ToString("f3"));
                                            highLimit[offset + i] = float.Parse(_site[i].HI_LIMIT.ToString("f3"));
                                        }
                                    }
                                    dataTable.Columns.Add((_site.Count + offset).ToString());
                                    Parameters[offset + _site.Count] = ("Status");

                                    dataTable.Rows.Add(Parameters);
                                    dataTable.Rows.Add(Units);
                                    dataTable.Rows.Add(lowLimit);
                                    dataTable.Rows.Add(highLimit);
                                    ptrList.Clear();

                                    //if (!dual_site && dataTable.Columns.Count < dataList.Count) dual_site = true;

                                }
                                #endregion // Build Data Table

                                if (dataTable.Columns.Count < dataList.Count)
                                    dual_site = true;
                                else
                                    dual_site = false;

                                if (prr.NUM_TEST != intTestNum)
                                    lMsg.Add("Data length is not match with previous, DeviceID: " + prr.PART_ID + " & TestNum: " + prr.PART_FLG);
                                    //throw new Exception("Data length is not match with previous, DeviceID: " + prr.PART_ID + " & TestNum: " + prr.PART_FLG);
                               
                                List<object> objArray = new List<object>();
                                objArray.Add(rowIndex);
                                objArray.Add(prr.SITE_NUM);
                                objArray.Add(prr.SOFT_BIN);
                                objArray.Add(prr.HARD_BIN);
                                objArray.Add(prr.PART_ID);

                                for (int i = 0; i < dataList.Count; i++)
                                {
                                    if ((int)dataList[i][0] == prr.SITE_NUM)
                                    {
                                        int column_index = 0;
                                        if (!dual_site)
                                        {
                                            column_index = i + 5;
                                        }
                                        else
                                        {
                                            if (prr.SITE_NUM == 1)
                                            {
                                                column_index = i / (prr.SITE_NUM + 1) + 5;
                                            }
                                            else if (prr.SITE_NUM == 2)
                                            {
                                                column_index = (i - 1) / prr.SITE_NUM + 5;
                                            }
                                        }

                                        string key = dataTable.Rows[0][column_index].ToString();
                                        //int pid = int.Parse(prr.PART_ID);
                                        //if (pid == 3915)
                                        //{
                                        //    string asdf = "11123";
                                        //}

                                        double value = Convert.ToDouble(dataList[i][1]);
                                        if (dic_scale.ContainsKey(key))
                                        {
                                            objArray.Add(new_double(value, dic_scale[key]).ToString("f3"));
                                            //objArray.Add(((float)dataList[i][1]).ToString("f3"));
                                        }
                                        else
                                        {
                                            objArray.Add(((float)dataList[i][1]).ToString("f3"));
                                        }
                                    }
                                }
                                // Status Pass/Fail
                                if (prr.PART_FLG == 0)
                                    objArray.Add("Pass");
                                else
                                    objArray.Add("Fail");

                                dataTable.Rows.Add(objArray.ToArray());

                                rowIndex += 1;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                        #endregion --- PRR ---
                        break;
                    default:
                        break;
                }
            }

            #region --- Header ---
            headerTable.Rows.Add(new object[] { "FAR.CPU_TYPE", far.CPU_TYPE });
            headerTable.Rows.Add(new object[] { "FAR.STDF_VER", far.STDF_VER });

            headerTable.Rows.Add(new object[] { "MIR.SETUP_T", mir.SETUP_T });
            headerTable.Rows.Add(new object[] { "MIR.START_T", mir.START_T });
            headerTable.Rows.Add(new object[] { "MIR.STAT_NUM", mir.STAT_NUM });
            headerTable.Rows.Add(new object[] { "MIR.MODE_COD", mir.MODE_COD });
            headerTable.Rows.Add(new object[] { "MIR.RTST_COD ", mir.RTST_COD });
            headerTable.Rows.Add(new object[] { "MIR.PROT_COD ", mir.PROT_COD });
            headerTable.Rows.Add(new object[] { "MIR.BURN_TIM ", mir.BURN_TIM });
            headerTable.Rows.Add(new object[] { "MIR.CMOD_COD ", mir.CMOD_COD });
            headerTable.Rows.Add(new object[] { "MIR.LOT_ID ", mir.LOT_ID });
            headerTable.Rows.Add(new object[] { "MIR.PART_TYP ", mir.PART_TYP });
            headerTable.Rows.Add(new object[] { "MIR.NODE_NAM ", mir.NODE_NAM });
            headerTable.Rows.Add(new object[] { "MIR.TSTR_TYP ", mir.TSTR_TYP });
            headerTable.Rows.Add(new object[] { "MIR.JOB_NAM ", mir.JOB_NAM });
            headerTable.Rows.Add(new object[] { "MIR.JOB_REV ", mir.JOB_REV });
            headerTable.Rows.Add(new object[] { "MIR.SBLOT_ID ", mir.SBLOT_ID });
            headerTable.Rows.Add(new object[] { "MIR.OPER_NAM ", mir.OPER_NAM });
            headerTable.Rows.Add(new object[] { "MIR.EXEC_TYP ", mir.EXEC_TYP });
            headerTable.Rows.Add(new object[] { "MIR.EXEC_VER ", mir.EXEC_VER });
            headerTable.Rows.Add(new object[] { "MIR.TEST_COD ", mir.TEST_COD });
            headerTable.Rows.Add(new object[] { "MIR.TST_TEMP ", mir.TST_TEMP });
            headerTable.Rows.Add(new object[] { "MIR.USER_TXT ", mir.USER_TXT });
            headerTable.Rows.Add(new object[] { "MIR.AUX_FILE ", mir.AUX_FILE });
            headerTable.Rows.Add(new object[] { "MIR.PKG_TYP ", mir.PKG_TYP });
            headerTable.Rows.Add(new object[] { "MIR.FAMLY_ID ", mir.FAMLY_ID });
            headerTable.Rows.Add(new object[] { "MIR.DATE_COD ", mir.DATE_COD });
            headerTable.Rows.Add(new object[] { "MIR.FACIL_ID ", mir.FACIL_ID });
            headerTable.Rows.Add(new object[] { "MIR.FLOOR_ID ", mir.FLOOR_ID });
            headerTable.Rows.Add(new object[] { "MIR.PROC_ID ", mir.PROC_ID });
            headerTable.Rows.Add(new object[] { "MIR.OPER_FRQ ", mir.OPER_FRQ });
            headerTable.Rows.Add(new object[] { "MIR.SPEC_NAM ", mir.SPEC_NAM });
            headerTable.Rows.Add(new object[] { "MIR.SPEC_VER ", mir.SPEC_VER });
            headerTable.Rows.Add(new object[] { "MIR.FLOW_ID ", mir.FLOW_ID });
            headerTable.Rows.Add(new object[] { "MIR.SETUP_ID ", mir.SETUP_ID });
            headerTable.Rows.Add(new object[] { "MIR.DSGN_REV ", mir.DSGN_REV });
            headerTable.Rows.Add(new object[] { "MIR.ENG_ID ", mir.ENG_ID });
            headerTable.Rows.Add(new object[] { "MIR.ROM_COD ", mir.ROM_COD });
            headerTable.Rows.Add(new object[] { "MIR.SERL_NUM ", mir.SERL_NUM });
            headerTable.Rows.Add(new object[] { "MIR.SUPR_NAM ", mir.SUPR_NAM });

            headerTable.Rows.Add(new object[] { "SDR.HEAD_NUM", sdr.HEAD_NUM });
            headerTable.Rows.Add(new object[] { "SDR.SITE_GRP", sdr.SITE_GRP });
            headerTable.Rows.Add(new object[] { "SDR.SITE_CNT", sdr.SITE_CNT });
            headerTable.Rows.Add(new object[] { "SDR.SITE_NUM", sdr.SITE_NUM });
            headerTable.Rows.Add(new object[] { "SDR.HAND_TYP", sdr.HAND_TYP });
            headerTable.Rows.Add(new object[] { "SDR.HAND_ID", sdr.HAND_ID });
            headerTable.Rows.Add(new object[] { "SDR.CARD_TYP", sdr.CARD_TYP });
            headerTable.Rows.Add(new object[] { "SDR.CARD_ID", sdr.CARD_ID });
            headerTable.Rows.Add(new object[] { "SDR.LOAD_TYP", sdr.LOAD_TYP });
            headerTable.Rows.Add(new object[] { "SDR.LOAD_ID", sdr.LOAD_ID });
            headerTable.Rows.Add(new object[] { "SDR.DIB_TYP", sdr.DIB_TYP });
            headerTable.Rows.Add(new object[] { "SDR.DIB_ID", sdr.DIB_ID });
            headerTable.Rows.Add(new object[] { "SDR.CABL_TYP", sdr.CABL_TYP });
            headerTable.Rows.Add(new object[] { "SDR.CABL_ID", sdr.CABL_ID });
            headerTable.Rows.Add(new object[] { "SDR.CONT_TYP", sdr.CONT_TYP });
            headerTable.Rows.Add(new object[] { "SDR.CONT_ID", sdr.CONT_ID });
            headerTable.Rows.Add(new object[] { "SDR.LASR_TYP", sdr.LASR_TYP });
            headerTable.Rows.Add(new object[] { "SDR.LASR_ID", sdr.LASR_ID });
            headerTable.Rows.Add(new object[] { "SDR.EXTR_TYP", sdr.EXTR_TYP });
            headerTable.Rows.Add(new object[] { "SDR.EXTR_ID", sdr.EXTR_ID });

            headerTable.Rows.Add(new object[] { "PCR.HEAD_NUM", pcr.HEAD_NUM });
            headerTable.Rows.Add(new object[] { "PCR.SITE_NUM", pcr.SITE_NUM });
            headerTable.Rows.Add(new object[] { "PCR.PART_CNT", pcr.PART_CNT });
            headerTable.Rows.Add(new object[] { "PCR.RTST_CNT", pcr.RTST_CNT });
            headerTable.Rows.Add(new object[] { "PCR.ABRT_CNT", pcr.ABRT_CNT });
            headerTable.Rows.Add(new object[] { "PCR.GOOD_CNT", pcr.GOOD_CNT });
            headerTable.Rows.Add(new object[] { "PCR.FUNC_CNT", pcr.FUNC_CNT });
            //headerTable.Rows.Add(new object[] { "FailQuantity", pcr.PART_CNT - pcr.GOOD_CNT });
            //headerTable.Rows.Add(new object[] { "Yield", 100 * pcr.GOOD_CNT / pcr.PART_CNT });

            headerTable.Rows.Add(new object[] { "MRR.DISP_COD", mrr.DISP_COD });
            headerTable.Rows.Add(new object[] { "MRR.USR_DESC", mrr.USR_DESC });
            headerTable.Rows.Add(new object[] { "MRR.EXC_DESC", mrr.EXC_DESC });
            headerTable.Rows.Add(new object[] { "MRR.FINISH_T", mrr.FINISH_T });
            #endregion --- Header ---

        }

        private string new_unit(string unit, int scale)
        {
            string new_unit = unit;

            try
            {
                if (unit.ToUpper() == "A")
                {
                    if (scale == 3) new_unit = "mA";
                    else if (scale == 6) new_unit = "uA";
                    else if (scale == 9) new_unit = "nA";
                }
                else if (unit.ToUpper() == "V")
                {
                    if (scale == 3) new_unit = "mV";
                    else if (scale == 6) new_unit = "uV";
                    else if (scale == 9) new_unit = "nV";
                }
                else if (unit.ToUpper() == "S")
                {
                    if (scale == 3) new_unit = "mS";
                    else if (scale == 6) new_unit = "uS";
                    else if (scale == 9) new_unit = "nS";
                }
                else if (unit.ToUpper() == "OHM" || unit.ToUpper() == "OHMS")
                {
                    if (scale == -3) new_unit = "kohm";
                }
                else
                {
                    new_unit = unit;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return new_unit;
        }
        private double new_double(double result, int scale)
        {
            double newresult = result;

            double multi = Math.Pow(10, scale);
            newresult = newresult * multi;

            return newresult;
            //return float.Parse(newresult.ToString("f3"));
        }

        public DataTable GetHeaderStr()
        {
            return headerTable;
        }

        public DataTable GetData()
        {
            return dataTable;
        }

        public List<string> GetMessage()
        {
            return lMsg;
        }
        private void HeadAnalyze(byte[] buffer)
        {
            REC_LEN = ((int)buffer[1] << 8) + (int)buffer[0];
            REC_TYP = (int)buffer[2];
            REC_SUB = (int)buffer[3];
        }

        private int REC_LEN { get; set; }
        private int REC_TYP { get; set; }
        private int REC_SUB { get; set; }
    }
}
