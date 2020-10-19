using Common;
using ODPCtrl;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BPOEntry.Login
{
    public class DaoLogin : DaoBase
    {
        /// <summary>
        /// ユーザー情報を取得
        /// </summary>
        /// <returns></returns>
        public DataTable SELECT_M_USER(string userId)
        {
            var sql = new StringBuilder();
            var param = new List<object>();

            sql.AppendLine("SELECT *");
            sql.AppendLine("  FROM M_USER");
            sql.AppendLine(" WHERE USER_ID=?"); param.Add(userId);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// ログイン情報を更新
        /// </summary>
        /// <returns></returns>
        public int UPDATE_M_USER(string userId, string ipAddress, bool isLogin)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE M_USER");
            if (isLogin)
            {
                sql.AppendLine("   SET LOGIN_START_DATE=SYSDATE ");
                sql.AppendLine("      ,LOGIN_IP_ADDRESS=?"); param.Add(ipAddress);
            }
            else
            {
                sql.AppendLine("   SET LOGIN_START_DATE=NULL ");
                sql.AppendLine("      ,LOGIN_IP_ADDRESS=NULL");
            }
            sql.AppendLine("  WHERE USER_ID=?"); param.Add(userId);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }
    }
}
