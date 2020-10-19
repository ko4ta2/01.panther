using BPOEntry.Tables;
using Common;
using ODPCtrl;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dao
{
    /// <summary>
    /// エントリ結果テキスト出力
    /// </summary>
    public class DaoExportEntryUnit : DaoBase
    {
        ///// <summary>
        ///// D_ENTRY_UNIT 取得
        ///// </summary>
        ///// <param name="deu"></param>
        ///// <returns></returns>
        //public DataTable SELECT_D_ENTRY_UNIT(D_ENTRY_UNIT deu)
        //{
        //    var sql = new StringBuilder();
        //    var param = new List<object>();
        //    sql.AppendLine("SELECT MIN(DEU.STATUS) MAX_STATUS");
        //    sql.AppendLine("  FROM D_ENTRY_UNIT DEU");
        //    sql.AppendLine(" INNER JOIN M_DOC MD");
        //    sql.AppendLine("    ON MD.TKSK_CD=DEU.TKSK_CD");
        //    sql.AppendLine("   AND MD.HNM_CD=DEU.HNM_CD");
        //    sql.AppendLine("   AND MD.DOC_ID=DEU.DOC_ID");
        //    sql.AppendLine("   AND MD.BATCH_EXPORT_FLAG=?"); param.Add(deu.BATCH_EXPORT_FLAG);
        //    sql.AppendLine(" WHERE DEU.TKSK_CD=?"); param.Add(deu.TKSK_CD);
        //    sql.AppendLine("   AND DEU.HNM_CD=?"); param.Add(deu.HNM_CD);
        //    switch (Config.ExportUnit)
        //    {
        //        case "1":
        //            // 連携年月日、連携回数（一致）コンペア済を出力（修正済以外がある場合出力不可）
        //            sql.AppendLine("   AND DEU.IMAGE_CAPTURE_DATE=?"); param.Add(deu.IMAGE_CAPTURE_DATE);
        //            sql.AppendLine("   AND DEU.IMAGE_CAPTURE_NUM=?"); param.Add(deu.IMAGE_CAPTURE_NUM);
        //            break;
        //        case "2":
        //            // 連携年月日（一致）コンペア済を出力（修正済以外がある場合出力不可）
        //            sql.AppendLine("   AND DEU.IMAGE_CAPTURE_DATE=?"); param.Add(deu.IMAGE_CAPTURE_DATE);
        //            break;
        //        case "3":
        //            // 連携年月日（まで）コンペア済を出力（修正済以外がある場合出力不可）
        //            sql.AppendLine("   AND DEU.IMAGE_CAPTURE_DATE<=?"); param.Add(deu.IMAGE_CAPTURE_DATE);
        //            break;
        //        case "9":
        //            // エントリバッチ単位でコンペア済を出力
        //            sql.AppendLine("   AND DEU.IMAGE_CAPTURE_DATE<=?"); param.Add(deu.IMAGE_CAPTURE_DATE);
        //            sql.AppendLine("   AND DEU.STATUS=?"); param.Add(Consts.EntryUnitStatus.COMPARE_END);
        //            break;
        //    }
        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        //}

        /// <summary>
        /// SELECT_M_DOC 取得
        /// </summary>
        /// <returns></returns>
        public DataTable SELECT_M_DOC(D_ENTRY_UNIT deu)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT *");
            sql.AppendLine("  FROM M_DOC");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(deu.TKSK_CD);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(deu.HNM_CD);
            sql.AppendLine("   AND BATCH_EXPORT_FLAG=?"); param.Add(deu.BATCH_EXPORT_FLAG);
            sql.AppendLine("ORDER BY DOC_ID ");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// SELECT_D_ENTRY 取得
        /// </summary>
        /// <param name="deu"></param>
        /// <returns></returns>
        public DataTable SELECT_D_ENTRY(D_ENTRY_UNIT deu)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT ENT.ENTRY_UNIT_ID");
            sql.AppendLine("      ,ENT.ITEM_ID");
            sql.AppendLine("      ,ENT.VALUE");
            sql.AppendLine("      ,IMG.IMAGE_SEQ");
            sql.AppendLine("      ,IMG.IMAGE_PATH");
            sql.AppendLine("  FROM D_ENTRY_UNIT UNI");
            sql.AppendLine(" INNER JOIN D_IMAGE_INFO IMG");
            sql.AppendLine("    ON IMG.ENTRY_UNIT_ID=UNI.ENTRY_UNIT_ID");
            sql.AppendLine("   AND IMG.DUMMY_IMAGE_FLAG!=?"); param.Add(Consts.Flag.ON);
            sql.AppendLine(" INNER JOIN D_ENTRY ENT");
            sql.AppendLine("    ON ENT.ENTRY_UNIT_ID=UNI.ENTRY_UNIT_ID");
            sql.AppendLine("   AND ENT.ENTRY_UNIT_ID=IMG.ENTRY_UNIT_ID");
            sql.AppendLine("   AND ENT.DUMMY_ITEM_FLAG!=?"); param.Add(Consts.Flag.ON);
            sql.AppendLine(" WHERE ENT.IMAGE_SEQ=IMG.IMAGE_SEQ");
            sql.AppendLine("   AND UNI.TKSK_CD=?"); param.Add(deu.TKSK_CD);
            sql.AppendLine("   AND UNI.HNM_CD=?"); param.Add(deu.HNM_CD);
            switch (deu.EXPORT_METHOD)
            {
                case "1":
                    // 連携年月日、回数指定
                    sql.AppendLine("   AND UNI.IMAGE_CAPTURE_DATE=?"); param.Add(deu.IMAGE_CAPTURE_DATE);
                    sql.AppendLine("   AND UNI.IMAGE_CAPTURE_NUM=?"); param.Add(deu.IMAGE_CAPTURE_NUM);
                    break;
                case "2":
                    // 連携年月日指定
                    sql.AppendLine("   AND UNI.IMAGE_CAPTURE_DATE=?"); param.Add(deu.IMAGE_CAPTURE_DATE);
                    break;
                case "3":
                case "9":
                    // 連携年月日まで指定
                    sql.AppendLine("   AND UNI.IMAGE_CAPTURE_DATE<=?"); param.Add(deu.IMAGE_CAPTURE_DATE);
                    break;
            }
            sql.AppendLine("   AND UNI.DOC_ID=?"); param.Add(deu.DOC_ID);
            sql.AppendLine("   AND ENT.RECORD_KBN=?"); param.Add(Consts.RecordKbn.ADMIN);
            sql.AppendLine("   AND UNI.STATUS=?"); param.Add(Consts.EntryUnitStatus.COMPARE_END);
            //sql.AppendLine("   AND ENT.DUMMY_ITEM_FLAG!=?"); param.Add(Consts.Flag.ON);
            sql.AppendLine(" ORDER BY ENT.ENTRY_UNIT_ID");
            sql.AppendLine("         ,ENT.IMAGE_SEQ");
            sql.AppendLine("         ,ENT.ITEM_ID");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// UPDATE_D_ENTRY_UNIT 更新
        /// </summary>
        /// <param name="deu"></param>
        /// <returns></returns>
        public int UPDATE_D_ENTRY_UNIT(D_ENTRY_UNIT deu)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_ENTRY_UNIT");
            sql.AppendLine("   SET STATUS=?"); param.Add(Consts.EntryUnitStatus.EXPORT_END);
            sql.AppendLine("      ,TEXT_EXPORT_DATE=SYSDATE");
            sql.AppendLine("      ,UPD_USER_ID=?"); param.Add(deu.UPD_USER_ID);
            sql.AppendLine("      ,UPD_DATE=SYSDATE");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(deu.ENTRY_UNIT_ID);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }
    }
}
