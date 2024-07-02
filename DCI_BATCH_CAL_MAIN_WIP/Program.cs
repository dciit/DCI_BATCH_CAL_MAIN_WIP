using DCI_BATCH_CAL_MAIN_WIP.Contexts;
using DCI_BATCH_CAL_MAIN_WIP.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI_BATCH_CAL_MAIN_WIP
{
    internal class Program
    {
        public static SqlConnectDB dbSCM = new SqlConnectDB("dbSCM");
        public static SqlConnectDB dbIOT = new SqlConnectDB("dbIoT");
        public static List<EkbWipPartStock> rWip = new List<EkbWipPartStock>();
        static void Main(string[] args)
        {
            DateTime dtNow = DateTime.Now.AddHours(-8);
            //using (var efSCM = new DBSCM())
            //{
            //    rWip = efSCM.EkbWipPartStock.Where(x => x.Ym == dtNow.ToString("yyyyMM") ).ToList();
            //}

            string _WCNO = "904";
            string _LineType = "MAIN";
            string strResult = @"SELECT Serial, SUBSTRING(Serial,1,4) ModelCode, [Model_Code], DATEADD(HOUR, -10,GETDATE()) PERIOD_ST, GETDATE() PERIOD_EN
                        FROM  [dbIoT].[dbo].[SCR_GasTight] G
                        WHERE Serial NOT LIKE 'ERROR%' AND DATEADD(HOUR,-8,G.Insert_Date) BETWEEN  DATEADD(HOUR, -10,GETDATE()) AND GETDATE()
                           AND Serial NOT IN (SELECT Serial FROM [192.168.226.86].[dbSCM].[dbo].EKB_WIP_PART_RESULT_TRANSACTION_LOG 
                                              WHERE WCNO = @WCNO AND LineType = @LineType 
				                                    AND DATEADD(HOUR,-8, PeriodStart) BETWEEN  DATEADD(MONTH, -1,GETDATE()) AND GETDATE()) ";
            SqlCommand cmdResult = new SqlCommand();
            cmdResult.CommandText = strResult;
            cmdResult.Parameters.Add(new SqlParameter("@WCNO", _WCNO));
            cmdResult.Parameters.Add(new SqlParameter("@LineType", _LineType));
            DataTable dtResult = dbIOT.Query(cmdResult);

            int idx = 1;
            if(dtResult.Rows.Count > 0)
            {
                // =====  BOM ========
                SqlCommand sqlGetUsagePart = new SqlCommand();
                sqlGetUsagePart.CommandText = @"SELECT [Code] ModelCode
                                        ,M.[DESCRIPTION] ModelName
                                        ,M.[REF1] WCNO 
                                        ,M.[REF2] PartNo
                                        ,M.[REF3] CM
                                        ,CAST(M.[REF4] as int) as UsageQty
                                        ,M.[NOTE] LineType 
                  FROM [dbSCM].[dbo].[DictMstr] M
                  WHERE DICT_SYSTEM = 'WIP_STOCK' and DICT_TYPE = 'PART_SET_OUT' AND [DICT_STATUS] = 'ACTIVE'  ";
                DataTable dtPartUsage = dbSCM.Query(sqlGetUsagePart);
                // ===== END BOM ========

                //DataView dvResult = new DataView(dtResult);
                //DataTable disResult = dvResult.ToTable(true, "Column1", "Column2");
                var grpResult = dtResult.AsEnumerable()
                .GroupBy(row => new { Model_Code = row.Field<string>("Model_Code"), ModelCode = row.Field<string>("ModelCode") })
                .Select(g => new
                {
                    Model_Code = g.Key.Model_Code,
                    ModelCode = g.Key.ModelCode,
                    Cnt = g.Count()
                })
                .ToList();
                foreach (var oResult in grpResult)
                {
                    var oPartBOMs = dtPartUsage.AsEnumerable().Where((c)=> oResult.ModelCode == c.Field<string>("ModelCode")).Select(
                        g => new
                        {
                            WCNO = g.Field<string>("WCNO"),
                            PartNo = g.Field<string>("ModelCode"),
                            PartName = g.Field<string>("PartNo"),
                            CM = g.Field<string>("CM"),
                            UsageQty = g.Field<int>("UsageQty")
                        }).ToList();
                    foreach (var oPartBOM in oPartBOMs)
                    {
                        // update Stock new
                        int issQty = oResult.Cnt * oPartBOM.UsageQty;

                        //====== Update Stock =======
                        string strUpd = "UPDATE EKB_WIP_PART_STOCK SET ISSQTY = (ISSQTY + @ISSQTY), BAL = (BAL- @BAL), UpdateBy=@UpdateBy, UpdateDate=CURRENT_TIMESTAMP WHERE YM=@YM AND WCNO=@WCNO AND PARTNO=@PARTNO AND CM=@CM ";
                        SqlCommand cmdUpd = new SqlCommand();
                        cmdUpd.CommandText = strUpd;
                        cmdUpd.Parameters.Add(new SqlParameter("@ISSQTY", issQty));
                        cmdUpd.Parameters.Add(new SqlParameter("@BAL", issQty));
                        cmdUpd.Parameters.Add(new SqlParameter("@UpdateBy", "BATCH"));
                        cmdUpd.Parameters.Add(new SqlParameter("@YM", dtNow.ToString("yyyyMM")));
                        cmdUpd.Parameters.Add(new SqlParameter("@WCNO", oPartBOM.WCNO));
                        cmdUpd.Parameters.Add(new SqlParameter("@PARTNO", oPartBOM.PartName));
                        cmdUpd.Parameters.Add(new SqlParameter("@CM", oPartBOM.CM));
                        int update = dbSCM.ExecuteNonCommand(cmdUpd);
                        if (update == 0)
                        {
                            Console.WriteLine($"WCNO : {oPartBOM.WCNO}, PARTNO : {oPartBOM.PartName}, CM : {oPartBOM.CM}");
                            Console.ReadKey();
                        }


                        //====== Insert Transaction =======
                        string strInstr = @"INSERT INTO [EKB_WIP_PART_STOCK_TRANSACTION] ([nbr],[YM],[WCNO],[PARTNO],[CM],[TransType],[TransQty],[QRCodeData],[CreateBy],[CreateDate],[RefNo]) 
		        VALUES (@nbr,@YM,@WCNO,@PARTNO,@CM,'OUT',@TransQty,'',@UpdateBy,CURRENT_TIMESTAMP,'')";
                        SqlCommand cmdInstr = new SqlCommand();
                        cmdInstr.CommandText = strInstr;
                        cmdInstr.Parameters.Add(new SqlParameter("@nbr", $"EKB{DateTime.Now.ToString("yyyyMMddHHmmss")}{idx.ToString("00000")}")); 
                        cmdInstr.Parameters.Add(new SqlParameter("@UpdateBy", "BATCH"));
                        cmdInstr.Parameters.Add(new SqlParameter("@YM", dtNow.ToString("yyyyMM")));
                        cmdInstr.Parameters.Add(new SqlParameter("@WCNO", oPartBOM.WCNO));
                        cmdInstr.Parameters.Add(new SqlParameter("@PARTNO", oPartBOM.PartNo));
                        cmdInstr.Parameters.Add(new SqlParameter("@CM", oPartBOM.CM));
                        cmdInstr.Parameters.Add(new SqlParameter("@TransQty", issQty));
                        dbSCM.ExecuteNonCommand(cmdInstr);


                        idx++;
                    } // end foreach part bom

                } // end foreach model count group by


                foreach (DataRow drResult in dtResult.Rows)
                {

                    DateTime PeriodST =  Convert.ToDateTime(drResult["PERIOD_ST"].ToString());
                    DateTime PeriodEN = Convert.ToDateTime(drResult["PERIOD_EN"].ToString());

                    string strInstr = @"INSERT INTO [EKB_WIP_PART_RESULT_TRANSACTION_LOG] ([WCNO],[LineType],[SerialNo],[ModelCode],[ModelName],[PeriodStart],[PeriodEnd]) 
		        VALUES (@WCNO,@LineType,@SerialNo,@ModelCode,@ModelName,@PeriodStart,@PeriodEnd)";
                    SqlCommand cmdInstr = new SqlCommand();
                    cmdInstr.CommandText = strInstr;
                    cmdInstr.Parameters.Add(new SqlParameter("@WCNO", _WCNO ));
                    cmdInstr.Parameters.Add(new SqlParameter("@LineType", _LineType ));
                    cmdInstr.Parameters.Add(new SqlParameter("@SerialNo", drResult["Serial"].ToString() ));
                    cmdInstr.Parameters.Add(new SqlParameter("@ModelCode", drResult["ModelCode"].ToString()));
                    cmdInstr.Parameters.Add(new SqlParameter("@ModelName", drResult["ModelCode"].ToString()));
                    cmdInstr.Parameters.Add(new SqlParameter("@PeriodStart", PeriodST));
                    cmdInstr.Parameters.Add(new SqlParameter("@PeriodEnd", PeriodEN));
                    dbSCM.ExecuteNonCommand(cmdInstr);
                }
            }



            Console.ReadKey();
        }
    }

    internal class Mdw27Props
    {
        public string modelCode { get; set; }
        public string modelName { get; set; }
    }
}
