using BPOEntry.Tables;
using Common;
using ODPCtrl;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BPOEntry.Menu
{
    public class DaoMenu : DaoBase
    {
        /// <summary>
        /// 自動アサインする未入力状態のエントリ単位を検索します。
        /// ただし指定したユーザーに入力中のデータが残っていればそれを優先的にアサインします。
        /// </summary>
        public DataTable SELECT_D_ENTRY_UNIT_STATUS(string USER_ID)
        {
            var sql = new StringBuilder();
            var param = new List<object>();

            sql.AppendLine("SELECT DES.ENTRY_UNIT_ID");
            sql.AppendLine("      ,DES.ENTRY_USER_ID");
            sql.AppendLine("      ,DES.ENTRY_STATUS STATUS");
            sql.AppendLine("      ,DES.RECORD_KBN");
            sql.AppendLine("      ,DES.ENTRY_STATUS");
            sql.AppendLine("      ,MD.DOC_NAME");
            sql.AppendLine("      ,MD.SINGLE_ENTRY_FLAG");
            sql.AppendLine("  FROM D_ENTRY_STATUS DES");

            sql.AppendLine(" INNER JOIN D_ENTRY_UNIT DEU");
            sql.AppendLine("    ON DEU.ENTRY_UNIT_ID=DES.ENTRY_UNIT_ID");

            sql.AppendLine(" INNER JOIN M_DOC MD");
            sql.AppendLine("    ON DEU.TKSK_CD=MD.TKSK_CD");
            sql.AppendLine("   AND DEU.HNM_CD=MD.HNM_CD");
            sql.AppendLine("   AND DEU.DOC_ID=MD.DOC_ID");

            sql.AppendLine(" WHERE DEU.TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sql.AppendLine("   AND DEU.HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("   AND ((DES.ENTRY_STATUS=?"); param.Add(Consts.EntryStatus.ENTRY_NOT);
            sql.AppendLine("   AND DES.ENTRY_USER_ID IS NULL");
            sql.AppendLine("   AND NOT EXISTS(SELECT 1");
            sql.AppendLine("                    FROM D_ENTRY_STATUS DES2");
            sql.AppendLine("                   WHERE DES2.ENTRY_UNIT_ID=DES.ENTRY_UNIT_ID");
            sql.AppendLine("                     AND DES2.ENTRY_USER_ID=?"); param.Add(USER_ID);
            sql.AppendLine("                     AND DES2.ENTRY_STATUS!=?"); param.Add(Consts.EntryStatus.ENTRY_NOT);
            sql.AppendLine("                   )");
            sql.AppendLine("      ) OR (DES.ENTRY_STATUS=? AND DES.ENTRY_USER_ID=?))"); param.Add(Consts.EntryStatus.ENTRY_ING); param.Add(USER_ID);
            sql.AppendLine("   AND ((MD.SINGLE_ENTRY_FLAG=? AND DES.RECORD_KBN=?)"); param.Add(Consts.Flag.ON); param.Add(Consts.RecordKbn.Entry_1st);
            sql.AppendLine("     OR (MD.SINGLE_ENTRY_FLAG=?))"); param.Add(Consts.Flag.OFF);

            //if (Utils.IsRLI())
            //{
            //sql.AppendLine("   AND ((MD.OCR_COOPERATION_FLAG=? AND DEU.OCR_IMPORT_USER_ID IS NOT NULL) OR (MD.OCR_COOPERATION_FLAG=?))");
            //param.Add(Consts.Flag.ON); param.Add(Consts.Flag.OFF);
            //}

            sql.AppendLine("   AND ((DES.RECORD_KBN='1' AND MD.OCR_IMPORT_1ST_FLAG='1' AND DES.OCR_IMPORT_USER_ID IS NOT NULL)");
            sql.AppendLine("     OR (DES.RECORD_KBN='1' AND MD.OCR_IMPORT_1ST_FLAG='0')");
            sql.AppendLine("     OR (DES.RECORD_KBN='2' AND MD.OCR_IMPORT_2ND_FLAG='1' AND DES.OCR_IMPORT_USER_ID IS NOT NULL)");
            sql.AppendLine("     OR (DES.RECORD_KBN='2' AND MD.OCR_IMPORT_2ND_FLAG='0'))");

            sql.AppendLine("ORDER BY DES.ENTRY_STATUS DESC");
            sql.AppendLine("        ,DES.ENTRY_UNIT_ID");
            sql.AppendLine("        ,DES.RECORD_KBN");
            //sql.AppendLine("FOR UPDATE OF DES.ENTRY_UNIT_ID NOWAIT");

            //sql.AppendLine("SELECT DES.ENTRY_UNIT_ID");
            //sql.AppendLine("      ,DES.RECORD_KBN");
            //sql.AppendLine("      ,DES.ENTRY_USER_ID");
            //sql.AppendLine("      ,DES.ENTRY_STATUS");
            //sql.AppendLine("  FROM D_ENTRY_STATUS DES");

            //sql.AppendLine(" INNER JOIN D_ENTRY_UNIT DEU");
            //sql.AppendLine("    ON DEU.ENTRY_UNIT_ID=DES.ENTRY_UNIT_ID");
            //sql.AppendLine(" WHERE DEU.TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            //sql.AppendLine("   AND DEU.HNM_CD=?"); param.Add(Config.HinmeiCode);
            //sql.AppendLine("   AND DES.ENTRY_STATUS!=?"); param.Add(Consts.EntryStatus.ENTRY_END);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }
/*
        /// <summary>
        /// エントリステータスのロック
        /// </summary>
        /// <param name="des"></param>
        public void LockEntryStatus(D_ENTRY_STATUS des)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT ENTRY_UNIT_ID");
            sql.AppendLine("  FROM D_ENTRY_STATUS");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(des.ENTRY_UNIT_ID);
            sql.AppendLine("   AND RECORD_KBN=?"); param.Add(des.RECORD_KBN);
            sql.AppendLine("FOR UPDATE NOWAIT");
            ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// エントリステータスを「1:入力中」へ変更
        /// </summary>
        /// <param name="es"></param>
        /// <returns></returns>
        public int UPDATE_D_ENTRY_UNIT_STATUS(D_ENTRY_STATUS es)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_ENTRY_STATUS");
            sql.AppendLine("   SET ENTRY_USER_ID=?"); param.Add(es.ENTRY_USER_ID);
            sql.AppendLine("      ,ENTRY_START_TIME=DECODE(ENTRY_START_TIME,NULL,SYSDATE,ENTRY_START_TIME)");
            sql.AppendLine("      ,ENTRY_STATUS=?"); param.Add(Consts.EntryUnitStatus.ENTRY_EDT);
            sql.AppendLine("      ,UPD_USER_ID=?"); param.Add(es.UPD_USER_ID);
            sql.AppendLine("      ,UPD_DATE=SYSDATE");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(es.ENTRY_UNIT_ID);
            sql.AppendLine("   AND RECORD_KBN=?"); param.Add(es.RECORD_KBN);
            sql.AppendLine("   AND ENTRY_STATUS IN (?,?)"); param.Add(Consts.EntryUnitStatus.ENTRY_NOT); param.Add(Consts.EntryUnitStatus.ENTRY_EDT);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        public int UPDATE_D_ENTRY_UNIT(D_ENTRY_UNIT eu)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_ENTRY_UNIT");
            sql.AppendLine("   SET STATUS=?"); param.Add(Consts.EntryStatus.ENTRY_ING);
            sql.AppendLine("      ,UPD_USER_ID=?"); param.Add(eu.UPD_USER_ID);
            sql.AppendLine("      ,UPD_DATE=SYSDATE");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(eu.ENTRY_UNIT_ID);
            sql.AppendLine("   AND STATUS!=?"); param.Add(Consts.EntryStatus.ENTRY_ING);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }
*/
        /// <summary>
        /// プレ論理チェック対象帳票有無チェック
        /// </summary>
        /// <returns></returns>
        public bool IsExistsPrelogicalCheckTarget()
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT 1 FROM M_DOC");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("   AND PRE_LOGICAL_CHECK_FLAG=?"); param.Add(Consts.Flag.ON);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param).Rows.Count == 0 ? false : true;
        }

        /// <summary>
        /// エントリ単位分割チェック
        /// </summary>
        /// <returns></returns>
        public bool IsExistsTergetEntryMethod(string sEntryMethod)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT 1 FROM M_DOC");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("   AND ENTRY_METHOD=?"); param.Add(sEntryMethod);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param).Rows.Count == 0 ? false : true;
        }

        /// <summary>
        /// エントリ結果出力チェック
        /// </summary>
        /// <returns></returns>
        public bool IsExistsUIExport()
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT 1 FROM M_DOC");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("   AND BATCH_EXPORT_FLAG!=?"); param.Add(Consts.Flag.ON);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param).Rows.Count == 0 ? false : true;
        }

        /// <summary>
        /// エントリ結果出力チェック
        /// </summary>
        /// <returns></returns>
        public bool IsExistsUICreateUnit()
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT 1 FROM M_DOC");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("   AND BATCH_CREATE_UNIT_FLAG!=?"); param.Add(Consts.Flag.ON);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param).Rows.Count == 0 ? false : true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="USER_ID"></param>
        /// <returns></returns>
        public DataTable SELECT_D_ENTRY_UNIT(string USER_ID)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT DEU.ENTRY_UNIT_ID");
            sql.AppendLine("      ,DEU.STATUS");
            sql.AppendLine("      ,DEU.UPD_ENTRY_USER_ID");
            sql.AppendLine("      ,'0' RECORD_KBN");
            sql.AppendLine("  FROM D_ENTRY_UNIT DEU");
            sql.AppendLine(" WHERE DEU.TKSK_CD=?"); param.Add(Common.Config.TokuisakiCode);
            sql.AppendLine("   AND DEU.HNM_CD=?"); param.Add(Common.Config.HinmeiCode);
            sql.AppendLine("   AND DEU.STATUS=?"); param.Add(Consts.EntryUnitStatus.COMPARE_EDT);
            sql.AppendLine("   AND (DEU.UPD_ENTRY_USER_ID=?"); param.Add(USER_ID);
            sql.AppendLine("    OR  DEU.UPD_ENTRY_USER_ID IS NULL)");
            sql.AppendLine("   AND NOT EXISTS (SELECT 1 FROM D_ENTRY_STATUS DES");
            sql.AppendLine("                   WHERE DES.ENTRY_UNIT_ID=DEU.ENTRY_UNIT_ID");
            sql.AppendLine("                     AND DES.ENTRY_USER_ID=?)"); param.Add(USER_ID);
            sql.AppendLine(" ORDER BY DEU.UPD_ENTRY_USER_ID");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }
    }
}