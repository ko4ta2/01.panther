using System;
using BPOEntry.Tables;
using Common;

namespace BPOEntry.Login
{
    /// <summary>
    /// ログイン
    /// </summary>
    public class LoginControl
    {
        private static DaoLogin _dao = new DaoLogin();

        /// <summary>
        /// ログイン
        /// </summary>
        /// <param name="userId">ID</param>
        /// <param name="password">パスワード</param>
        /// <returns></returns>
        public static Tuple<bool, string> Login(string userId, string password)
        {
            // ID未入力
            if (String.IsNullOrEmpty(userId))
                return Tuple.Create(false, "ユーザIDが未入力です。");

            // パスワード未入力
            if (String.IsNullOrEmpty(password))
                return Tuple.Create(false, "パスワードが未入力です。");

            // ユーザーマスタを検索します。
            //var loginDao = new DaoLogin();
            var dt_M_USER = _dao.SELECT_M_USER(userId);
            if (dt_M_USER.Rows.Count == 0)
                return Tuple.Create(false, "入力されたユーザIDは登録されていません。\n管理者に確認して下さい。");

            if (!dt_M_USER.Rows[0]["PASSWORD"].ToString().Equals(password))
                return Tuple.Create(false, "入力されたパスワードに誤りがあります。\n管理者に確認して下さい。");

            if ((!"admn".Equals(userId)
                && !"admn".Equals(password))
                && (!"sysadmin".Equals(userId)
                && !"sysadmin".Equals(password)))
            {
                if (!Consts.Flag.ON.Equals(dt_M_USER.Rows[0]["VALID_FLAG"].ToString()))
                    return Tuple.Create(false, "入力されたユーザIDは無効です。\n管理者に確認して下さい。");

                if (dt_M_USER.Rows[0]["LOGIN_START_DATE"].ToString().Length != 0
                    && !dt_M_USER.Rows[0]["LOGIN_IP_ADDRESS"].ToString().Equals(Utils.GetIPAddress()))
                    return Tuple.Create(false, String.Format("既に同一ユーザID「{0}」でログイン中です。\n管理者に確認して下さい。", userId));
            }

            // 最終ログイン更新
            _dao.BeginTrans();
            _dao.UPDATE_M_USER(userId, Utils.GetIPAddress(), true);
            _dao.CommitTrans();

            // ログインユーザー情報を登録する
            var user = dt_M_USER.Rows[0];

            if (Program.LoginUser != null
                && !user["user_id"].ToString().Equals(Program.LoginUser.USER_ID)
                && Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
            {
                Config.UserId = Consts.DEFAULT_USER_ID;
            }

            Program.LoginUser = new M_USER();
            Program.LoginUser.USER_ID = (string)user["USER_ID"];
            Program.LoginUser.USER_NAME = (string)user["USER_NAME"];
            Program.LoginUser.USER_KBN = (string)user["USER_KBN"];

            // ログインID
            return Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Load()
        {
            _dao.Open(Config.DSN);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Close()
        {
            _dao.Close();
        }
    }
}
