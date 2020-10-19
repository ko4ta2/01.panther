using Common;
using ODPCtrl;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BPOEntry.Progress
{
    /// <summary>
    /// エントリ状況一覧
    /// </summary>
    public class ProgressDao : DaoBase
    {
        public DataTable SELECT_D_ENTRY_UNIT(string sDate, string sImageCaptureNum,string sProgress, string sGyoumKbn,string sSort,string sExportDate)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT DEU.ENTRY_UNIT_ID");
            sql.AppendLine("      ,MD.DOC_NAME");
            sql.AppendLine("      ,MD.SINGLE_ENTRY_FLAG");
            sql.AppendLine("      ,MD.OCR_COOPERATION_FLAG");
            sql.AppendLine("      ,MD.GYOUM_KBN || ':' || MCD.VALUE_1 AS GYOUM_KBN");
            sql.AppendLine("      ,DEU.ENTRY_UNIT");
            sql.AppendLine("      ,TO_CHAR(DEU.TEXT_EXPORT_DATE,'YYYY/MM/DD HH24:MI:SS') AS TEXT_EXPORT_DATE");
            sql.AppendLine("      ,DEU.UPD_ENTRY_START_TIME");
            sql.AppendLine("      ,DEU.UPD_ENTRY_END_TIME");
            sql.AppendLine("      ,(SELECT NVL(MAX(IMAGE_SEQ),0)");
            sql.AppendLine("  		  FROM D_IMAGE_INFO DII");
            //sql.AppendLine(" 		 WHERE DII.TKSK_CD=DEU.TKSK_CD");
            //sql.AppendLine(" 		   AND DII.HNM_CD=DEU.HNM_CD");
            //sql.AppendLine(" 		   AND DII.IMAGE_CAPTURE_DATE=DEU.IMAGE_CAPTURE_DATE");
            //sql.AppendLine("   		   AND DII.IMAGE_CAPTURE_NUM=DEU.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("   	       AND DII.DOC_ID=DEU.DOC_ID");
            sql.AppendLine("   		 WHERE DII.ENTRY_UNIT_ID=DEU.ENTRY_UNIT_ID) AS IMAGE_COUNT");
            sql.AppendLine("      ,DEU.STATUS");
            //sql.AppendLine("      ,DEU.VERIFY_ING_FLAG");
            sql.AppendLine("      ,DECODE(DEU.STATUS,'0','0:未入力','1','1:入力中','2','2:入力済み','5','5:コンペア待ち','6','6:コンペア修正中','7','7:コンペア済み','8','8:テキスト出力中','9','9:テキスト出力済み') AS STATUS_NAME");
            sql.AppendLine("      ,(SELECT DECODE(ENTRY_STATUS,'1',MU1.USER_NAME || '（入力中）',MU1.USER_NAME)");
            sql.AppendLine("  		  FROM D_ENTRY_STATUS DES1");
            sql.AppendLine("         INNER JOIN M_USER MU1");
            sql.AppendLine("   	        ON MU1.USER_ID=DES1.ENTRY_USER_ID");
            //sql.AppendLine(" 		 WHERE DES1.TKSK_CD=DEU.TKSK_CD");
            //sql.AppendLine(" 		   AND DES1.HNM_CD=DEU.HNM_CD");
            //sql.AppendLine(" 		   AND DES1.IMAGE_CAPTURE_DATE=DEU.IMAGE_CAPTURE_DATE");
            //sql.AppendLine("   		   AND DES1.IMAGE_CAPTURE_NUM=DEU.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("   	       AND DES1.DOC_ID=DEU.DOC_ID");
            sql.AppendLine("   		 WHERE DES1.ENTRY_UNIT_ID=DEU.ENTRY_UNIT_ID");
            sql.AppendLine("   		   AND DES1.RECORD_KBN='1') AS ENTRY_USER_NAME_1ST");

            // エントリ開始時刻（１人目）
            sql.AppendLine("      ,(SELECT DES1.ENTRY_START_TIME");
            sql.AppendLine("  		  FROM D_ENTRY_STATUS DES1");
            sql.AppendLine("         INNER JOIN M_USER MU1");
            sql.AppendLine("   	        ON MU1.USER_ID=DES1.ENTRY_USER_ID");
            //sql.AppendLine(" 		 WHERE DES1.TKSK_CD=DEU.TKSK_CD");
            //sql.AppendLine(" 		   AND DES1.HNM_CD=DEU.HNM_CD");
            //sql.AppendLine(" 		   AND DES1.IMAGE_CAPTURE_DATE=DEU.IMAGE_CAPTURE_DATE");
            //sql.AppendLine("   		   AND DES1.IMAGE_CAPTURE_NUM=DEU.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("   	       AND DES1.DOC_ID=DEU.DOC_ID");
            sql.AppendLine("   	     WHERE DES1.ENTRY_UNIT_ID=DEU.ENTRY_UNIT_ID");
            sql.AppendLine("   		   AND DES1.RECORD_KBN='1') AS ENTRY_START_TIME_1ST");

            // エントリ終了時刻（１人目）
            sql.AppendLine("      ,(SELECT DES1.ENTRY_END_TIME");
            sql.AppendLine("  		  FROM D_ENTRY_STATUS DES1");
            sql.AppendLine("         INNER JOIN M_USER MU1");
            sql.AppendLine("   	        ON MU1.USER_ID=DES1.ENTRY_USER_ID");
            //sql.AppendLine(" 		 WHERE DES1.TKSK_CD=DEU.TKSK_CD");
            //sql.AppendLine(" 		   AND DES1.HNM_CD=DEU.HNM_CD");
            //sql.AppendLine(" 		   AND DES1.IMAGE_CAPTURE_DATE=DEU.IMAGE_CAPTURE_DATE");
            //sql.AppendLine("   		   AND DES1.IMAGE_CAPTURE_NUM=DEU.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("   	       AND DES1.DOC_ID=DEU.DOC_ID");
            sql.AppendLine("   		 WHERE DES1.ENTRY_UNIT_ID=DEU.ENTRY_UNIT_ID");
            sql.AppendLine("   		   AND DES1.RECORD_KBN='1') AS ENTRY_END_TIME_1ST");

            sql.AppendLine("      ,(SELECT DECODE(ENTRY_STATUS,'1',MU1.USER_NAME || '（入力中）',MU1.USER_NAME)");
            sql.AppendLine("  		  FROM D_ENTRY_STATUS DES1");
            sql.AppendLine("         INNER JOIN M_USER MU1");
            sql.AppendLine("   	        ON MU1.USER_ID=DES1.ENTRY_USER_ID");
            //sql.AppendLine(" 		 WHERE DES1.TKSK_CD=DEU.TKSK_CD");
            //sql.AppendLine(" 		   AND DES1.HNM_CD=DEU.HNM_CD");
            //sql.AppendLine(" 		   AND DES1.IMAGE_CAPTURE_DATE=DEU.IMAGE_CAPTURE_DATE");
            //sql.AppendLine("   		   AND DES1.IMAGE_CAPTURE_NUM=DEU.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("   	       AND DES1.DOC_ID=DEU.DOC_ID");
            sql.AppendLine("   		 WHERE DES1.ENTRY_UNIT_ID=DEU.ENTRY_UNIT_ID");
            sql.AppendLine("   		   AND DES1.RECORD_KBN='2') AS ENTRY_USER_NAME_2ND");

            // エントリ開始時刻（２人目）
            sql.AppendLine("      ,(SELECT DES1.ENTRY_START_TIME");
            sql.AppendLine("  		  FROM D_ENTRY_STATUS DES1");
            sql.AppendLine("         INNER JOIN M_USER MU1");
            sql.AppendLine("   	        ON MU1.USER_ID=DES1.ENTRY_USER_ID");
            //sql.AppendLine(" 		 WHERE DES1.TKSK_CD=DEU.TKSK_CD");
            //sql.AppendLine(" 		   AND DES1.HNM_CD=DEU.HNM_CD");
            //sql.AppendLine(" 		   AND DES1.IMAGE_CAPTURE_DATE=DEU.IMAGE_CAPTURE_DATE");
            //sql.AppendLine("   		   AND DES1.IMAGE_CAPTURE_NUM=DEU.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("   	       AND DES1.DOC_ID=DEU.DOC_ID");
            sql.AppendLine("   		 WHERE DES1.ENTRY_UNIT_ID=DEU.ENTRY_UNIT_ID");
            sql.AppendLine("   		   AND DES1.RECORD_KBN='2') AS ENTRY_START_TIME_2ND");

            // エントリ終了時刻（２人目）
            sql.AppendLine("      ,(SELECT DES1.ENTRY_END_TIME");
            sql.AppendLine("  		  FROM D_ENTRY_STATUS DES1");
            sql.AppendLine("         INNER JOIN M_USER MU1");
            sql.AppendLine("   	        ON MU1.USER_ID=DES1.ENTRY_USER_ID");
            //sql.AppendLine(" 		 WHERE DES1.TKSK_CD=DEU.TKSK_CD");
            //sql.AppendLine(" 		   AND DES1.HNM_CD=DEU.HNM_CD");
            //sql.AppendLine(" 		   AND DES1.IMAGE_CAPTURE_DATE=DEU.IMAGE_CAPTURE_DATE");
            //sql.AppendLine("   		   AND DES1.IMAGE_CAPTURE_NUM=DEU.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("   	       AND DES1.DOC_ID=DEU.DOC_ID");
            sql.AppendLine("   		 WHERE DES1.ENTRY_UNIT_ID=DEU.ENTRY_UNIT_ID");
            sql.AppendLine("   		   AND DES1.RECORD_KBN='2') AS ENTRY_END_TIME_2ND");

            sql.AppendLine("      ,MUO.USER_NAME OCR_IMPORT_USER_NAME");
            sql.AppendLine("      ,MUU.USER_NAME UPD_USER_NAME");
            sql.AppendLine("      ,DECODE(DEU.VERIFY_ING_FLAG,'1',MUV.USER_NAME || '（検証中）',MUV.USER_NAME) VERIFY_ENTRY_USER_NAME");


            sql.AppendLine("      ,(SELECT COUNT(IMAGE_SEQ)");
            sql.AppendLine("  		  FROM D_IMAGE_INFO DII");
            //sql.AppendLine(" 		 WHERE DII.TKSK_CD=DEU.TKSK_CD");
            //sql.AppendLine(" 		   AND DII.HNM_CD=DEU.HNM_CD");
            //sql.AppendLine(" 		   AND DII.IMAGE_CAPTURE_DATE=DEU.IMAGE_CAPTURE_DATE");
            //sql.AppendLine("   		   AND DII.IMAGE_CAPTURE_NUM=DEU.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("   	       AND DII.DOC_ID=DEU.DOC_ID");
            sql.AppendLine("   		 WHERE DII.ENTRY_UNIT_ID=DEU.ENTRY_UNIT_ID");
            sql.AppendLine("   		   AND DII.OCR_NG_STATUS='2') AS Verified_COUNT");

            sql.AppendLine("      ,(SELECT COUNT(IMAGE_SEQ)");
            sql.AppendLine("  		  FROM D_IMAGE_INFO DII");
            //sql.AppendLine(" 		 WHERE DII.TKSK_CD=DEU.TKSK_CD");
            //sql.AppendLine(" 		   AND DII.HNM_CD=DEU.HNM_CD");
            //sql.AppendLine(" 		   AND DII.IMAGE_CAPTURE_DATE=DEU.IMAGE_CAPTURE_DATE");
            //sql.AppendLine("   		   AND DII.IMAGE_CAPTURE_NUM=DEU.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("   	       AND DII.DOC_ID=DEU.DOC_ID");
            sql.AppendLine("   		 WHERE DII.ENTRY_UNIT_ID=DEU.ENTRY_UNIT_ID");
            sql.AppendLine("   		   AND DII.OCR_NG_STATUS IN('1','2')) AS NG_COUNT");

            sql.AppendLine("  FROM D_ENTRY_UNIT DEU");

            // ユーザマスタ１
            sql.AppendLine(" LEFT OUTER JOIN M_USER MUO");
            sql.AppendLine("   ON DEU.OCR_IMPORT_USER_ID=MUO.USER_ID");
            
            // ユーザマスタ２
            sql.AppendLine(" LEFT OUTER JOIN M_USER MUU");
            sql.AppendLine("   ON DEU.UPD_ENTRY_USER_ID=MUU.USER_ID");

            // ユーザマスタ３
            sql.AppendLine(" LEFT OUTER JOIN M_USER MUV");
            sql.AppendLine("   ON DEU.VERIFY_ENTRY_USER_ID=MUV.USER_ID");

            // 帳票マスタ
            sql.AppendLine(" INNER JOIN M_DOC MD");
            sql.AppendLine("    ON DEU.TKSK_CD=MD.TKSK_CD");
            sql.AppendLine("   AND DEU.HNM_CD=MD.HNM_CD");
            sql.AppendLine("   AND DEU.DOC_ID=MD.DOC_ID");

            // コード定義マスタ
            sql.AppendLine(" INNER JOIN M_CODE_DEFINE MCD");
            sql.AppendLine("    ON MCD.TKSK_CD=MD.TKSK_CD");
            sql.AppendLine("   AND MCD.HNM_CD=MD.HNM_CD");
            sql.AppendLine("   AND MCD.SECTION IN ('業務区分','帳票種別GRP')");
            sql.AppendLine("   AND MCD.KEY=MD.GYOUM_KBN");
            
            if (sDate.Length != 0)
            {
                sql.AppendLine("    WHERE DEU.IMAGE_CAPTURE_DATE=?"); param.Add(sDate);

                if (!"00".Equals(sImageCaptureNum))
                {
                    sql.AppendLine("      AND DEU.IMAGE_CAPTURE_NUM=?"); param.Add(sImageCaptureNum);
                }

                if (!"*".Equals(sProgress))
                {
                    sql.AppendLine("      AND DEU.STATUS=?"); param.Add(sProgress);
                }
            }
            else
            {
                if (!"*".Equals(sProgress))
                {
                    sql.AppendLine("    WHERE DEU.STATUS=?"); param.Add(sProgress);
                }
            }
            sql.AppendLine("      AND DEU.TKSK_CD=?"); param.Add(Common.Config.TokuisakiCode);
            sql.AppendLine("      AND DEU.HNM_CD=?"); param.Add(Common.Config.HinmeiCode);

            if (!"*".Equals(sGyoumKbn))
            {
                sql.AppendLine("      AND MD.GYOUM_KBN=?"); param.Add(sGyoumKbn);
            }

            if (sExportDate.Length != 0)
            {
                sql.AppendLine("      AND TO_CHAR(DEU.TEXT_EXPORT_DATE,'YYYYMMDD')=?"); param.Add(sExportDate);
            }

            switch (sSort)
            {
                case "1":
                    sql.AppendLine(" ORDER BY DEU.STATUS,DEU.ENTRY_UNIT_ID");
                    break;
                case "2":
                    sql.AppendLine(" ORDER BY DEU.ENTRY_UNIT_ID");
                    break;
            }
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        public DataTable SELECT_M_CODE_DEFINE(string sSection)
        {
            try
            {
                var sql = new StringBuilder();
                var param = new List<object>();
                sql.AppendLine("SELECT * ");
                sql.AppendLine("  FROM M_CODE_DEFINE");
                sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
                sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
                sql.AppendLine("   AND SECTION=?"); param.Add(sSection);
                sql.AppendLine(" ORDER BY DISPLAY_ORDER,KEY");
                return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
            }
            catch
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// プレ論理チェック対象帳票有無チェック
        /// </summary>
        /// <returns></returns>
        public bool IsExistsPrelogicalCheckTargetDoc()
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT 1 FROM M_DOC");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("   AND PRE_LOGICAL_CHECK_FLAG=?"); param.Add(Consts.Flag.ON);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param).Rows.Count == 0 ? false : true;
        }
    }
}