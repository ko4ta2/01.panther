﻿using Common;
using ODPCtrl;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dao
{
    public class DaoEntry : DaoBase
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
    }
}
