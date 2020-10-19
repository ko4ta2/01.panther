using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using Common;
using BPOEntry.Tables;
using ODPCtrl;

namespace AddressList.Address
{
    public class daoAddressList : DaoBase
    {
        /// <summary>
        /// 住所情報を取得
        /// </summary>
        /// <returns></returns>
        public DataTable SELECT_M_ZIP_CODE(string ZipCode)
        {
            var sql = new StringBuilder();
            var param = new List<object>();

            sql.AppendLine("SELECT *");
            sql.AppendLine("  FROM M_ZIP_CODE");
            sql.AppendLine(" WHERE ZIP_CODE=?"); param.Add(ZipCode);
            sql.AppendLine(" ORDER BY SEQ");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }
    }
}
