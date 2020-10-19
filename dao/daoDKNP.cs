using System.Collections;
using System.Text;
using Common;
using ODPCtrl;

namespace DAO
{
    /// <summary>
    /// ドリームかんぽキャンペーン2018　はがきエントリ
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    public class daoDKNP : ComBasicDAO
    {
        public void InsertT_SCAN_INFO(string sQrValue)
        {
            var sql = new StringBuilder();
            var param = new ArrayList();
            sql.AppendLine("INSERT INTO T_SCAN_INFO");
            sql.AppendLine("       (QR_VALUE");
            sql.AppendLine("       ,SCAN_DATE");
            sql.AppendLine("       ,INS_USER_ID");
            sql.AppendLine("       ,INS_DATE");
            sql.AppendLine("       )VALUES");
            sql.AppendLine("       (?"); param.Add(sQrValue);
            sql.AppendLine("       ,?"); param.Add(sQrValue.Substring(0,8));
            sql.AppendLine("       ,?"); param.Add(GVal.UserId);
            sql.AppendLine("       ,SYSDATE ) ");
            ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }
    }
}
