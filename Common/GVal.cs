
namespace Common
{
    /// <summary>
    /// グローバル変数
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    public static class GVal
    {
        /// <summary>
        /// ログインユーザID
        /// </summary>
        public static string UserId { get; set; }

        /// <summary>
        /// バージョン情報
        /// </summary>
        public static string Version { get; set; }

        /// <summary>
        /// 変数値クリア
        /// </summary>
        public static void Clear()
        {
            UserId = string.Empty;
            Version = string.Empty;
        }
   
    }
}
