using Common;
using ODPCtrl;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dao
{
    /// <summary>
    /// エントリ単位分割
    /// </summary>
    public class DaoBatchCreateEntryUnit : DaoBase
    {
        public DataTable SELECT_T_DIV_ENTRY_UNIT_PATH()
        {
            var sql = new StringBuilder();
            List<object> param = new List<object>();
            sql.AppendLine("SELECT *");
            sql.AppendLine("  FROM T_DIV_ENTRY_UNIT_PATH");
            //sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            //sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("   FOR UPDATE");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        public DataTable SELECT_M_BUSINESS(string BUSINESS_ID)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT *");
            sql.AppendLine("  FROM M_BUSINESS");
            sql.AppendLine(" WHERE BUSINESS_ID=?"); param.Add(BUSINESS_ID);
            sql.AppendLine("   AND INVALID_FLAG!=?"); param.Add(Consts.Flag.ON);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }
    }
}
