using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using Common;
using BPOEntry.Tables;
using ODPCtrl;

namespace AddressEntry.Address
{
    //20171113 add
    public class DaoAddress : DaoBase
    {
        /// <summary>
        /// 住所情報を取得
        /// </summary>
        /// <returns></returns>
        public DataTable SelectM_ADDRESS(string sPostalCd, string sAddress)
        {
            //int iOut = 0;
            var sql = new StringBuilder();
            var param = new List<object>();
            //sql.AppendLine("SELECT ADDRESS_CD");
            //sql.AppendLine("      ,POSTAL_CD");
            //sql.AppendLine("      ,ADDRESS_ALL");
            //sql.AppendLine("      ,RECORD_KBN");
            //sql.AppendLine("      ,HAISHI_YYYYMM");
            //sql.AppendLine("  FROM M_ADDRESS");
            //sql.AppendLine(" WHERE ADDRESS_ALL LIKE ?"); param.Add(sAddress + "%");
            ////sql.AppendLine(" WHERE REPLACE(ADDRESS_ALL,'大字',null) LIKE ?"); param.Add(sAddress + "%");
            //if (sPostalCd.Length != 0)
            //{
            //    sql.AppendLine("   AND POSTAL_CD=?"); param.Add(sPostalCd);
            //}
            ////sql.AppendLine(" ORDER BY RECORD_KBN ASC,ADDRESS_CD ASC");
            //var dt = ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
            //if (dt.Rows.Count == 1)
            //{
            //    return dt;
            //}

            //// 再検索
            //sql = new StringBuilder();
            //param = new ArrayList();
            sql.AppendLine("SELECT ADDRESS_CD");
            sql.AppendLine("      ,POSTAL_CD");
            sql.AppendLine("      ,ADDRESS_ALL");
            sql.AppendLine("      ,RECORD_KBN");
            sql.AppendLine("      ,HAISHI_YYYYMM");
            sql.AppendLine("  FROM M_ADDRESS");
            sql.AppendLine(" WHERE ADDRESS_ALL LIKE ?"); param.Add(sAddress + "%");
            //sql.AppendLine(" WHERE REPLACE(ADDRESS_ALL,'大字',null) LIKE ?"); param.Add(sAddress + "%");
            sql.AppendLine(" ORDER BY RECORD_KBN ASC,ADDRESS_ALL ASC,POSTAL_CD ASC");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }
    }
}
