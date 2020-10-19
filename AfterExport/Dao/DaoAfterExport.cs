//using ODPCtrl;

//namespace Dao
//{
//    /// <summary>
//    /// エントリデータ出力後処理　データアクセス
//    /// 第一生命保険　高齢者現況確認通知
//    /// </summary>
//    public class DaoAfterExport : DaoBase
//    {
//        ///// <summary>
//        ///// 受付回数取得
//        ///// </summary>
//        ///// <param name="sSeq"></param>
//        ///// <returns></returns>
//        //public DataTable SELECT_T_APPLICATION(string sSeq)
//        //{
//        //    var sql = new StringBuilder();
//        //    var param = new List<object>();
//        //    sql.AppendLine("SELECT (TO_NUMBER(NVL(MAX(ITEM_003),'0') + 1)) AS ITEM_003");
//        //    sql.AppendLine("  FROM T_APPLICATION_KRN");
//        //    sql.AppendLine(" WHERE ITEM_002=?"); param.Add(sSeq);
//        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
//        //}

//        ///// <summary>
//        ///// 申込登録
//        ///// </summary>
//        ///// <param name="sCpCd"></param>
//        ///// <param name="sSeq"></param>
//        ///// <param name="sTimes"></param>
//        ///// <returns></returns>
//        //public int INSERT_T_APPLICATION(DataRow drEntry)
//        //{
//        //    var sql = new StringBuilder();
//        //    var param = new List<object>();
//        //    for (int iIdx1 = 1; iIdx1 <= 80; iIdx1++)
//        //        param.Add(drEntry[String.Format("ITEM_{0}", iIdx1.ToString("d3"))].ToString());

//        //    sql.AppendLine("INSERT INTO T_APPLICATION_KRN");
//        //    sql.AppendLine("      (ITEM_001");
//        //    sql.AppendLine("      ,ITEM_002");
//        //    sql.AppendLine("      ,ITEM_003");
//        //    sql.AppendLine("      ,ITEM_004");
//        //    sql.AppendLine("      ,ITEM_005");
//        //    sql.AppendLine("      ,ITEM_006");
//        //    sql.AppendLine("      ,ITEM_007");
//        //    sql.AppendLine("      ,ITEM_008");
//        //    sql.AppendLine("      ,ITEM_009");
//        //    sql.AppendLine("      ,ITEM_010");

//        //    sql.AppendLine("      ,ITEM_011");
//        //    sql.AppendLine("      ,ITEM_012");
//        //    sql.AppendLine("      ,ITEM_013");
//        //    sql.AppendLine("      ,ITEM_014");
//        //    sql.AppendLine("      ,ITEM_015");
//        //    sql.AppendLine("      ,ITEM_016");
//        //    sql.AppendLine("      ,ITEM_017");
//        //    sql.AppendLine("      ,ITEM_018");
//        //    sql.AppendLine("      ,ITEM_019");
//        //    sql.AppendLine("      ,ITEM_020");

//        //    sql.AppendLine("      ,ITEM_021");
//        //    sql.AppendLine("      ,ITEM_022");
//        //    sql.AppendLine("      ,ITEM_023");
//        //    sql.AppendLine("      ,ITEM_024");
//        //    sql.AppendLine("      ,ITEM_025");
//        //    sql.AppendLine("      ,ITEM_026");
//        //    sql.AppendLine("      ,ITEM_027");
//        //    sql.AppendLine("      ,ITEM_028");
//        //    sql.AppendLine("      ,ITEM_029");
//        //    sql.AppendLine("      ,ITEM_030");

//        //    sql.AppendLine("      ,ITEM_031");
//        //    sql.AppendLine("      ,ITEM_032");
//        //    sql.AppendLine("      ,ITEM_033");
//        //    sql.AppendLine("      ,ITEM_034");
//        //    sql.AppendLine("      ,ITEM_035");
//        //    sql.AppendLine("      ,ITEM_036");
//        //    sql.AppendLine("      ,ITEM_037");
//        //    sql.AppendLine("      ,ITEM_038");
//        //    sql.AppendLine("      ,ITEM_039");
//        //    sql.AppendLine("      ,ITEM_040");

//        //    sql.AppendLine("      ,ITEM_041");
//        //    sql.AppendLine("      ,ITEM_042");
//        //    sql.AppendLine("      ,ITEM_043");
//        //    sql.AppendLine("      ,ITEM_044");
//        //    sql.AppendLine("      ,ITEM_045");
//        //    sql.AppendLine("      ,ITEM_046");
//        //    sql.AppendLine("      ,ITEM_047");
//        //    sql.AppendLine("      ,ITEM_048");
//        //    sql.AppendLine("      ,ITEM_049");
//        //    sql.AppendLine("      ,ITEM_050");

//        //    sql.AppendLine("      ,ITEM_051");
//        //    sql.AppendLine("      ,ITEM_052");
//        //    sql.AppendLine("      ,ITEM_053");
//        //    sql.AppendLine("      ,ITEM_054");
//        //    sql.AppendLine("      ,ITEM_055");
//        //    sql.AppendLine("      ,ITEM_056");
//        //    sql.AppendLine("      ,ITEM_057");
//        //    sql.AppendLine("      ,ITEM_058");
//        //    sql.AppendLine("      ,ITEM_059");
//        //    sql.AppendLine("      ,ITEM_060");

//        //    sql.AppendLine("      ,ITEM_061");
//        //    sql.AppendLine("      ,ITEM_062");
//        //    sql.AppendLine("      ,ITEM_063");
//        //    sql.AppendLine("      ,ITEM_064");
//        //    sql.AppendLine("      ,ITEM_065");
//        //    sql.AppendLine("      ,ITEM_066");
//        //    sql.AppendLine("      ,ITEM_067");
//        //    sql.AppendLine("      ,ITEM_068");
//        //    sql.AppendLine("      ,ITEM_069");
//        //    sql.AppendLine("      ,ITEM_070");

//        //    sql.AppendLine("      ,ITEM_071");
//        //    sql.AppendLine("      ,ITEM_072");
//        //    sql.AppendLine("      ,ITEM_073");
//        //    sql.AppendLine("      ,ITEM_074");
//        //    sql.AppendLine("      ,ITEM_075");
//        //    sql.AppendLine("      ,ITEM_076");
//        //    sql.AppendLine("      ,ITEM_077");
//        //    sql.AppendLine("      ,ITEM_078");
//        //    sql.AppendLine("      ,ITEM_079");
//        //    sql.AppendLine("      ,ITEM_080");

//        //    sql.AppendLine("      ,IMAGE_PATH"); param.Add(drEntry["IMAGE_PATH"].ToString());
//        //    sql.AppendLine("      ,DOC_ID"); param.Add(drEntry["DOC_ID"].ToString());
//        //    sql.AppendLine("      ,INS_USER_ID"); param.Add("AfterExport");
//        //    sql.AppendLine("      ,INS_DATE");
//        //    sql.AppendLine("      )VALUES(?,?,?,?,?,?,?,?,?,?");
//        //    sql.AppendLine("             ,?,?,?,?,?,?,?,?,?,?");
//        //    sql.AppendLine("             ,?,?,?,?,?,?,?,?,?,?");
//        //    sql.AppendLine("             ,?,?,?,?,?,?,?,?,?,?");
//        //    sql.AppendLine("             ,?,?,?,?,?,?,?,?,?,?");
//        //    sql.AppendLine("             ,?,?,?,?,?,?,?,?,?,?");
//        //    sql.AppendLine("             ,?,?,?,?,?,?,?,?,?,?");
//        //    sql.AppendLine("             ,?,?,?,?,?,?,?,?,?,?");
//        //    sql.AppendLine("             ,?,?,?,SYSDATE)");
//        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
//        //}

//        ///// <summary>
//        ///// キャンペーンマスタ取得
//        ///// </summary>
//        ///// <returns></returns>
//        //public DataTable SELECT_M_CAMPAIGN()
//        //{
//        //    var sql = new StringBuilder();
//        //    var param = new List<object>();
//        //    sql.AppendLine("SELECT DISTINCT CAMPAIGN_CD,SUBSTRB(DOC_ID,1,4) || ',0' AS DOC_ID");
//        //    sql.AppendLine("  FROM M_CAMPAIGN_KRN");
//        //    sql.AppendLine(" WHERE TO_CHAR(LAST_ENTRY_DATE,'YYYYMMDD') >=?"); param.Add(Consts.sSysDate);
//        //    sql.AppendLine(" UNION ALL");
//        //    sql.AppendLine("SELECT DISTINCT CAMPAIGN_CD,SUBSTRB(TSN_ENTRY_DOC_ID,1,4) || ',1' AS DOC_ID");
//        //    sql.AppendLine("  FROM M_CAMPAIGN_KRN");
//        //    sql.AppendLine(" WHERE TO_CHAR(LAST_ENTRY_DATE,'YYYYMMDD') >=?"); param.Add(Consts.sSysDate);
//        //    sql.AppendLine("   AND TSN_ENTRY_DOC_ID IS NOT NULL");

//        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
//        //}

//        ///// <summary>
//        ///// キャンペーンマスタ取得
//        ///// </summary>
//        ///// <returns></returns>
//        //public DataTable SELECT_M_CAMPAIGN_CLOSED(string sCpCd)
//        //{
//        //    var sql = new StringBuilder();
//        //    var param = new List<object>();
//        //    sql.AppendLine("SELECT *");
//        //    sql.AppendLine("  FROM M_CAMPAIGN_KRN");
//        //    sql.AppendLine(" WHERE CAMPAIGN_CD=?"); param.Add(sCpCd);
//        //    sql.AppendLine("   AND CAMPAIGN_TYPE=?"); param.Add("C");
//        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
//        //}


//        ///*
//        // * 1026
//        // * M_CANPAIGNを読み編集した内容を戻す
//        // */
//        //public DataTable GetEditedM_CAMPAIGN_KRN()
//        //{
//        //    var sql = new StringBuilder();
//        //    sql.AppendLine("SELECT CAMPAIGN_CD,DOC_ID,TSN_ENTRY_DOC_ID,CAMPAIGN_TYPE");
//        //    sql.AppendLine(" FROM M_CAMPAIGN_KRN");
//        //    sql.AppendLine(" WHERE LAST_ENTRY_DATE >= TRUNC(sysdate)");
//        //    DataTable source = ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString());

//        //    DataTable dt = new DataTable();
//        //    dt.Columns.Add("CAMPAIGN_CD", typeof(string));
//        //    dt.Columns.Add("DOC_ID", typeof(string));
//        //    dt.Columns.Add("TYPE", typeof(string));
//        //    foreach (DataRow row in source.Rows)
//        //    {
//        //        switch (row["CAMPAIGN_TYPE"].ToString())
//        //        {
//        //            case "C":
//        //                //応募者
//        //                DataRow[] found_o = dt.Select(String.Format("CAMPAIGN_CD = '{0}' AND TYPE = '_応募者'", row["CAMPAIGN_CD"].ToString()));
//        //                if (found_o.Length > 0)
//        //                {
//        //                    found_o[0]["DOC_ID"] += ((String.IsNullOrEmpty(found_o[0]["DOC_ID"].ToString()) || String.IsNullOrEmpty(row["DOC_ID"].ToString())
//        //                                                ? string.Empty : ",")
//        //                                                + row["DOC_ID"].ToString());
//        //                }
//        //                else
//        //                {
//        //                    DataRow nr = dt.NewRow();
//        //                    nr["CAMPAIGN_CD"] = row["CAMPAIGN_CD"].ToString();
//        //                    nr["DOC_ID"] = row["DOC_ID"].ToString();
//        //                    nr["TYPE"] = "_応募者";
//        //                    dt.Rows.Add(nr);
//        //                }
//        //                //当選者
//        //                DataRow[] found_t = dt.Select(String.Format("CAMPAIGN_CD = '{0}' AND TYPE = '_当選者'", row["CAMPAIGN_CD"].ToString()));
//        //                if (found_t.Length > 0)
//        //                {
//        //                    found_t[0]["DOC_ID"] += ((String.IsNullOrEmpty(found_t[0]["DOC_ID"].ToString()) || String.IsNullOrEmpty(row["TSN_ENTRY_DOC_ID"].ToString())
//        //                                                ? string.Empty : ",")
//        //                                                + row["TSN_ENTRY_DOC_ID"].ToString());
//        //                }
//        //                else
//        //                {
//        //                    DataRow nr = dt.NewRow();
//        //                    nr["CAMPAIGN_CD"] = row["CAMPAIGN_CD"].ToString();
//        //                    nr["DOC_ID"] = row["TSN_ENTRY_DOC_ID"].ToString();
//        //                    nr["TYPE"] = "_当選者";
//        //                    dt.Rows.Add(nr);
//        //                }
//        //                break;


//        //            case "M":
//        //            default:

//        //                DataRow[] found = dt.Select(String.Format("CAMPAIGN_CD = '{0}'", row["CAMPAIGN_CD"].ToString()));
//        //                if (found.Length > 0)
//        //                    found[0]["DOC_ID"] += ((String.IsNullOrEmpty(found[0]["DOC_ID"].ToString()) || String.IsNullOrEmpty(row["DOC_ID"].ToString())
//        //                                                ? string.Empty : ",")
//        //                                                + row["DOC_ID"].ToString());
//        //                else
//        //                {
//        //                    DataRow nr = dt.NewRow();
//        //                    nr["CAMPAIGN_CD"] = row["CAMPAIGN_CD"].ToString();
//        //                    nr["DOC_ID"] = row["DOC_ID"].ToString();
//        //                    nr["TYPE"] = string.Empty;
//        //                    dt.Rows.Add(nr);
//        //                }
//        //                break;
//        //        }


//        //    }

//        //    return dt;
//        //}
//    }
//}
