using Common;
using ODPCtrl;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dao
{
    /// <summary>
    /// コンペア
    /// </summary>
    public class DaoBatchCompareEntryUnit : DaoBase
    {
        public DataTable SELECT_M_BUSINESS()
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT *");
            sql.AppendLine("  FROM M_BUSINESS");
            sql.AppendLine(" WHERE INVALID_FLAG!=?"); param.Add(Consts.Flag.ON);
            sql.AppendLine(" ORDER BY DISPLAY_ORDER,BUSINESS_ID");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        public DataTable SELECT_D_ENTRY_UNIT(string tksk_cd,string hnm_cd,string STATUS)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT ENTRY_UNIT_ID");
            sql.AppendLine("  FROM D_ENTRY_UNIT");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(tksk_cd);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(hnm_cd);
            sql.AppendLine("   AND STATUS=?"); param.Add(STATUS);
            sql.AppendLine("   FOR UPDATE");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }
    }
}
