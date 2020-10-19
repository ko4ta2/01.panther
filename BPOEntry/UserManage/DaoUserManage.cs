using Common;
using ODPCtrl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BPOEntry.UserManage
{
    /// <summary>
    /// ユーザ管理
    /// </summary>
    public class DaoUserManage : DaoBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataTable SELECT_M_USER(string sUserId,string sUserName,bool bInvalidUser)
        {
            var sql = new StringBuilder();
            var param = new List<object>();

            sql.AppendLine("SELECT USER_ID");
            sql.AppendLine("      ,USER_NAME");
            sql.AppendLine("      ,DECODE(USER_KBN,'1','入力担当','0','管理者','S','システム管理者','その他') AS USER_KBN_");
            sql.AppendLine("      ,DECODE(VALID_FLAG,'0','無効','1','有効','その他') AS VALID_FLAG_");
            sql.AppendLine("      ,LOGIN_IP_ADDRESS");
            sql.AppendLine("  FROM M_USER");
            sql.AppendLine(" WHERE USER_ID IS NOT NULL");
            sql.AppendLine("   AND USER_KBN!=?");param.Add("S");
            if (!bInvalidUser)
            {
                sql.AppendLine(" AND VALID_FLAG=?"); param.Add(Consts.Flag.ON);
            }
            if (sUserId.Length != 0)
            {
                sql.AppendLine(String.Format("AND UPPER(USER_ID) LIKE UPPER('{0}%')",sUserId));
            }
            if (sUserName.Length != 0)
            {
                sql.AppendLine(String.Format(" AND UPPER(USER_NAME) LIKE UPPER('{0}%')", sUserName));
            }
            sql.AppendLine(" ORDER BY USER_ID");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sUserId"></param>
        /// <returns></returns>
        public DataTable SELECT_M_USER(string sUserId)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT * ");
            sql.AppendLine("  FROM M_USER");
            sql.AppendLine(" WHERE USER_ID=?"); param.Add(sUserId);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// M_USER 更新
        /// </summary>
        /// <param name="sUserId"></param>
        /// <param name="sUserName"></param>
        /// <param name="sPassword"></param>
        /// <param name="sUserKbn"></param>
        /// <param name="sValidFlag"></param>
        /// <param name="isUnlock"></param>
        /// <returns></returns>
        public int UPDATE_M_USER(string sUserId, string sUserName, string sPassword, string sUserKbn, string sValidFlag, bool isUnlock)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE M_USER");
            sql.AppendLine("   SET USER_NAME=? "); param.Add(sUserName);
            sql.AppendLine("      ,PASSWORD=?"); param.Add(sPassword);
            sql.AppendLine("      ,USER_KBN=?"); param.Add(sUserKbn);
            sql.AppendLine("      ,VALID_FLAG=?"); param.Add(sValidFlag);
            if (isUnlock)
            {
                sql.AppendLine("     , LOGIN_START_DATE=NULL");
                sql.AppendLine("     , LOGIN_IP_ADDRESS=NULL");
            }
            sql.AppendLine("     , UPD_USER_ID=?"); param.Add(Program.LoginUser.USER_ID);
            sql.AppendLine("     , UPD_DATE = SYSDATE ");
            sql.AppendLine("WHERE USER_ID=?"); param.Add(sUserId);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// M_USER 登録
        /// </summary>
        /// <param name="sUserId"></param>
        /// <param name="sUserName"></param>
        /// <param name="sPassword"></param>
        /// <param name="sUserKbn"></param>
        /// <param name="sValidFlag"></param>
        /// <returns></returns>
        public int INSERT_M_USER(string sUserId, string sUserName, string sPassword, string sUserKbn, string sValidFlag)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("INSERT INTO M_USER");
            sql.AppendLine("      (USER_ID"); param.Add(sUserId);
            sql.AppendLine("      ,USER_NAME"); param.Add(sUserName);
            sql.AppendLine("      ,USER_KBN"); param.Add(sUserKbn);
            sql.AppendLine("      ,PASSWORD"); param.Add(sPassword);
            sql.AppendLine("      ,VALID_FLAG"); param.Add(sValidFlag);
            sql.AppendLine("      ,INS_USER_ID"); param.Add(Program.LoginUser.USER_ID);
            sql.AppendLine("      ,INS_DATE");
            sql.AppendLine("      )VALUES");
            sql.AppendLine("      (?,?,?,?,?,?,SYSDATE)");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }
    }
}