using Common;
using ODPCtrl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CodeDefineList
{
    public class daoCodeDefineList : DaoBase
    {
        /// <summary>
        /// コード定義情報を取得
        /// </summary>
        /// <returns></returns>
        public DataTable SELECT_M_CODE_DEFINE(string Section, string SearchCondition = null)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT KEY");
            sql.AppendLine("      ,VALUE_1");
            sql.AppendLine("      ,VALUE_2");
            sql.AppendLine("  FROM M_CODE_DEFINE");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("   AND SECTION=?"); param.Add(Section);
            sql.AppendLine("   AND INVALID_FLAG!=?"); param.Add(Consts.Flag.ON);
            if (SearchCondition != null)
            {
                sql.AppendLine(String.Format("AND UPPER(SEARCH_KEY) LIKE UPPER('%{0}%')", SearchCondition));
            }
            sql.AppendLine(" ORDER BY DISPLAY_ORDER,KEY");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }
    }
}
