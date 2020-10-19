using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Common;
using BPOEntry.Tables;
//using MySqlCtrl;
using ODPCtrl;

namespace BPOEntry.EntryForms
{
    /// <summary>
    /// 入力画面共通DAO
    /// </summary>
    public class DaoOcr : DaoBase
    //public class DaoOcr : DaoBase
    {
        public DataTable SelectOcrImageInfo(string sOcrImageFileName)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            
            sql.AppendLine("            SELECT RP2.COLUNM_NO");
            sql.AppendLine(", RP2.X");
            sql.AppendLine(", RP2.Y");
            sql.AppendLine(", RP2.WIDTH");
  sql.AppendLine(", RP2.HEIGHT ");
sql.AppendLine("FROM");
  sql.AppendLine("READING_PAGES RP1 ");
  sql.AppendLine("INNER JOIN READING_PARTS RP2 ");
    sql.AppendLine("ON RP1.ID = RP2.READING_PAGES_ID ");
sql.AppendLine("WHERE");
sql.AppendLine("  RP1.FILE_NAME = ?");
param.Add(sOcrImageFileName);
            
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

    }
}
