using Common;
using ODPCtrl;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BPOEntry.ExportEntry
{
    public class DaoExportEntryUnit : DaoBase
    {
        public DataTable SELECT_D_ENTRY_UNIT(string sImageCaptureDate, string sImageCaptureNum)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT MIN(EU.STATUS) MAX_STATUS");
            sql.AppendLine("  FROM D_ENTRY_UNIT EU");
            sql.AppendLine(" INNER JOIN M_DOC MD");
            sql.AppendLine("    ON MD.TKSK_CD=EU.TKSK_CD");
            sql.AppendLine("   AND MD.HNM_CD=EU.HNM_CD");
            sql.AppendLine("   AND MD.DOC_ID=EU.DOC_ID");
            sql.AppendLine("   AND MD.BATCH_EXPORT_FLAG!=?"); param.Add(Common.Consts.Flag.ON);
            sql.AppendLine(" WHERE EU.TKSK_CD=?"); param.Add(Common.Config.TokuisakiCode);
            sql.AppendLine("   AND EU.HNM_CD=?"); param.Add(Common.Config.HinmeiCode);
            switch(Config.ExportUnit)
            { 
                case "1":
                    // 連携年月日、連携回数（一致）コンペア済を出力（修正済以外がある場合出力不可）
                    sql.AppendLine("   AND EU.IMAGE_CAPTURE_DATE=?"); param.Add(sImageCaptureDate);
                    sql.AppendLine("   AND EU.IMAGE_CAPTURE_NUM=?"); param.Add(sImageCaptureNum);
                    break;
                case "2":
                    // 連携年月日（一致）コンペア済を出力（修正済以外がある場合出力不可）
                    sql.AppendLine("   AND EU.IMAGE_CAPTURE_DATE=?"); param.Add(sImageCaptureDate);
                    break;
                case "3":
                    // 連携年月日（まで）コンペア済を出力（修正済以外がある場合出力不可）
                    sql.AppendLine("   AND EU.IMAGE_CAPTURE_DATE<=?"); param.Add(sImageCaptureDate);
                    break;
                case "9":
                    // エントリバッチ単位でコンペア済を出力
                    sql.AppendLine("   AND EU.IMAGE_CAPTURE_DATE<=?"); param.Add(sImageCaptureDate);
                    sql.AppendLine("   AND EU.STATUS=?"); param.Add(Consts.EntryUnitStatus.COMPARE_END);
                    break;
            }
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        public DataTable SELECT_M_DOC()
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT *");
            sql.AppendLine("  FROM M_DOC");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Common.Config.TokuisakiCode);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(Common.Config.HinmeiCode);
            sql.AppendLine("   AND BATCH_EXPORT_FLAG!=?"); param.Add(Common.Consts.Flag.ON);
            sql.AppendLine("ORDER BY DOC_ID ");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        public DataTable SELECT_D_ENTRY(string sImageCaptureDate, string sImageCaptureNum,string sDocId)
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
            sql.AppendLine(" INNER JOIN D_ENTRY ENT");
            sql.AppendLine("    ON ENT.ENTRY_UNIT_ID=UNI.ENTRY_UNIT_ID");
            sql.AppendLine("   AND ENT.ENTRY_UNIT_ID=IMG.ENTRY_UNIT_ID");

            //sql.AppendLine(" WHERE ENT.TKSK_CD=IMG.TKSK_CD");
            //sql.AppendLine("   AND ENT.HNM_CD=IMG.HNM_CD");
            //sql.AppendLine("   AND ENT.IMAGE_CAPTURE_DATE=IMG.IMAGE_CAPTURE_DATE");
            //sql.AppendLine("   AND ENT.IMAGE_CAPTURE_NUM=IMG.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("   AND ENT.DOC_ID=IMG.DOC_ID");
            //sql.AppendLine("   AND ENT.ENTRY_UNIT=IMG.ENTRY_UNIT");

            //sql.AppendLine(" WHERE ENT.ENTRY_UNIT_ID=IMG.ENTRY_UNIT_ID");
            sql.AppendLine(" WHERE ENT.IMAGE_SEQ=IMG.IMAGE_SEQ");

            //sql.AppendLine("   AND IMG.TKSK_CD=UNI.TKSK_CD");
            //sql.AppendLine("   AND IMG.HNM_CD=UNI.HNM_CD");
            //sql.AppendLine("   AND IMG.IMAGE_CAPTURE_DATE=UNI.IMAGE_CAPTURE_DATE");
            //sql.AppendLine("   AND IMG.IMAGE_CAPTURE_NUM=UNI.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("   AND IMG.DOC_ID=UNI.DOC_ID");
            //sql.AppendLine("   AND IMG.ENTRY_UNIT=UNI.ENTRY_UNIT");
            //sql.AppendLine("   AND IMG.ENTRY_UNIT_ID=UNI.ENTRY_UNIT_ID");
            //sql.AppendLine("   AND ENT.ENTRY_UNIT_ID=UNI.ENTRY_UNIT_ID");

            sql.AppendLine("   AND IMG.DUMMY_IMAGE_FLAG!=?"); param.Add(Consts.Flag.ON);
            //sql.AppendLine("   AND IMG.TEXT_EXPORT_OMIT_FLAG!=?"); param.Add(Consts.Flag.ON);

            sql.AppendLine("   AND UNI.TKSK_CD=?"); param.Add(Common.Config.TokuisakiCode);
            sql.AppendLine("   AND UNI.HNM_CD=?"); param.Add(Common.Config.HinmeiCode);
            switch (Config.ExportUnit)
            {
                case "1":
                    // 連携年月日、回数指定
                    sql.AppendLine("   AND UNI.IMAGE_CAPTURE_DATE=?"); param.Add(sImageCaptureDate);
                    sql.AppendLine("   AND UNI.IMAGE_CAPTURE_NUM=?"); param.Add(sImageCaptureNum);
                    break;
                case "2":
                    // 連携年月日指定
                    sql.AppendLine("   AND UNI.IMAGE_CAPTURE_DATE=?"); param.Add(sImageCaptureDate);
                    break;
                case "3":
                case "9":
                    // 連携年月日まで指定
                    sql.AppendLine("   AND UNI.IMAGE_CAPTURE_DATE<=?"); param.Add(sImageCaptureDate);
                    //sql.AppendLine("   AND UNI.STATUS!=?"); param.Add(Consts.EntryUnitStatus.EXPORT_END);
                    break;
            }
            sql.AppendLine("   AND UNI.DOC_ID=?"); param.Add(sDocId);
            sql.AppendLine("   AND ENT.RECORD_KBN=?"); param.Add(Consts.RecordKbn.ADMIN);
            sql.AppendLine("   AND UNI.STATUS=?"); param.Add(Consts.EntryUnitStatus.COMPARE_END);
            sql.AppendLine("   AND ENT.DUMMY_ITEM_FLAG!=?"); param.Add(Consts.Flag.ON);
            //sql.AppendLine(" ORDER BY ENT.TKSK_CD");
            //sql.AppendLine("         ,ENT.HNM_CD");
            //sql.AppendLine("         ,ENT.IMAGE_CAPTURE_DATE");
            //sql.AppendLine("         ,ENT.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("         ,ENT.ENTRY_UNIT");
            //sql.AppendLine("         ,ENT.DOC_ID");
            sql.AppendLine(" ORDER BY ENT.ENTRY_UNIT_ID");
            sql.AppendLine("         ,ENT.IMAGE_SEQ");
            //sql.AppendLine("         ,ENT.RECORD_KBN");
            sql.AppendLine("         ,ENT.ITEM_ID");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        //public int UpdateD_ENTRY_UNIT(D_ENTRY_UNIT entryUnit)
        public int UpdateD_ENTRY_UNIT(string entryUnitId)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_ENTRY_UNIT");
            sql.AppendLine("   SET STATUS=?"); param.Add(Consts.EntryUnitStatus.EXPORT_END);
            sql.AppendLine("      ,TEXT_EXPORT_DATE=SYSDATE");
            sql.AppendLine("      ,UPD_USER_ID=?"); param.Add(Program.LoginUser.USER_ID);
            sql.AppendLine("      ,UPD_DATE=SYSDATE");
            //sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Common.Config.TokuisakiCode);
            //sql.AppendLine("   AND HNM_CD=?"); param.Add(Common.Config.HinmeiCode);
            //sql.AppendLine("   AND IMAGE_CAPTURE_DATE=?"); param.Add(entryUnit.IMAGE_CAPTURE_DATE);
            //sql.AppendLine("   AND IMAGE_CAPTURE_NUM=?"); param.Add(entryUnit.IMAGE_CAPTURE_NUM);
            //sql.AppendLine("   AND DOC_ID=?"); param.Add(entryUnit.DOC_ID);
            //sql.AppendLine("   AND ENTRY_UNIT=?"); param.Add(entryUnit.ENTRY_UNIT);
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(entryUnitId);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }
    }
}
