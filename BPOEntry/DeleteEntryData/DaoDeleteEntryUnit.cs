using Common;
using ODPCtrl;
using System.Collections.Generic;
using System.Text;

namespace BPOEntry.DeleteEntryUnit
{
    public class DaoDeleteEntryUnit : DaoBase
    {
        public int DELETE_D_ENTRY_UNIT(string IMAGE_CAPTURE_DATE)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            var DeleteCount = 0;

            sql.AppendLine("DELETE FROM D_ENTRY_UNIT");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("   AND IMAGE_CAPTURE_DATE<=?"); param.Add(IMAGE_CAPTURE_DATE);
            sql.AppendLine("   AND STATUS=?"); param.Add(Consts.EntryUnitStatus.EXPORT_END);
            ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);

            sql.Clear();
            sql.AppendLine("DELETE FROM D_ENTRY_STATUS DES");
            sql.AppendLine(" WHERE NOT EXISTS(SELECT 1 FROM D_ENTRY_UNIT DEU");
            sql.AppendLine("                   WHERE DEU.ENTRY_UNIT_ID=DES.ENTRY_UNIT_ID)");
            ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString());

            sql.Clear();
            sql.AppendLine("DELETE FROM D_ENTRY DE");
            sql.AppendLine(" WHERE NOT EXISTS(SELECT 1 FROM D_ENTRY_UNIT DEU");
            sql.AppendLine("                   WHERE DEU.ENTRY_UNIT_ID=DE.ENTRY_UNIT_ID)");
            ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString());

            sql.Clear();
            sql.AppendLine("DELETE FROM D_IMAGE_INFO DII");
            sql.AppendLine(" WHERE NOT EXISTS(SELECT 1 FROM D_ENTRY_UNIT DEU");
            sql.AppendLine("                   WHERE DEU.ENTRY_UNIT_ID=DII.ENTRY_UNIT_ID)");
            DeleteCount = ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString());

            sql.Clear();
            param.Clear();
            sql.AppendLine("DELETE FROM D_OCR_COOPERATION_HISTORY");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("   AND TO_CHAR(OCR_CSV_IMPORT_DATE,'YYYYMMDD')<=?"); param.Add(IMAGE_CAPTURE_DATE);
            try
            {
                ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
            }
            catch { }

            // 画像削除件数
            return DeleteCount;
        }
    }
}
