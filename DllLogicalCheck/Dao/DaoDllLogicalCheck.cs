using Common;
using ODPCtrl;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dao
{
    /// <summary>
    /// プレ論理チェック　データアクセス
    /// </summary>
    public class DaoDllLogicalCheck : DaoBase
    {
        public DataTable SELECT_D_ENTRY(string sTargetUnit)
        {
            var sb = new StringBuilder();
            var param = new List<object>();
            sb.Append("SELECT DE.IMAGE_CAPTURE_DATE ");
            sb.Append("      ,DE.IMAGE_CAPTURE_NUM ");
            sb.Append("      ,DE.DOC_ID ");
            sb.Append("      ,DE.ENTRY_UNIT ");
            sb.Append("      ,DE.IMAGE_SEQ ");
            sb.Append("  FROM D_IMAGE_INFO DII ");
            sb.Append(" INNER JOIN D_ENTRY_UNIT DEU ");
            sb.Append("    ON DEU.TKSK_CD=DII.TKSK_CD ");
            sb.Append("   AND DEU.HNM_CD=DII.HNM_CD ");
            sb.Append("   AND DEU.IMAGE_CAPTURE_DATE=DII.IMAGE_CAPTURE_DATE ");
            sb.Append("   AND DEU.IMAGE_CAPTURE_NUM=DII.IMAGE_CAPTURE_NUM ");
            sb.Append("   AND DEU.DOC_ID=DII.DOC_ID ");
            sb.Append("   AND DEU.ENTRY_UNIT=DII.ENTRY_UNIT ");
            sb.Append("   AND DEU.STATUS='7' ");
            sb.Append(" INNER JOIN D_ENTRY DE ");
            sb.Append("    ON DE.TKSK_CD=DII.TKSK_CD ");
            sb.Append("   AND DE.HNM_CD=DII.HNM_CD ");
            sb.Append("   AND DE.IMAGE_CAPTURE_DATE=DII.IMAGE_CAPTURE_DATE ");
            sb.Append("   AND DE.IMAGE_CAPTURE_NUM=DII.IMAGE_CAPTURE_NUM ");
            sb.Append("   AND DE.DOC_ID=DII.DOC_ID ");
            sb.Append("   AND DE.ENTRY_UNIT=DII.ENTRY_UNIT ");
            sb.Append("   AND DE.IMAGE_SEQ=DII.IMAGE_SEQ ");
            sb.Append("   AND DE.RECORD_KBN='0' ");
            sb.Append("   AND DE.ITEM_ID BETWEEN 'ITEM_018' AND 'ITEM_026' ");
            sb.Append(" WHERE DEU.TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sb.Append("   AND DEU.HNM_CD=?"); param.Add(Config.HinmeiCode);
            sb.Append("   AND DEU.IMAGE_CAPTURE_DATE=?"); param.Add(sTargetUnit.Split('_')[0]);
            sb.Append("   AND DEU.IMAGE_CAPTURE_NUM=?"); param.Add(sTargetUnit.Split('_')[1]);
            sb.Append("   AND DEU.DOC_ID=?"); param.Add(sTargetUnit.Split('_')[2]);
            sb.Append("   AND DEU.ENTRY_UNIT=?"); param.Add(sTargetUnit.Split('_')[3]);
            sb.Append(" GROUP BY DE.TKSK_CD ");
            sb.Append("         ,DE.HNM_CD ");
            sb.Append("         ,DE.IMAGE_CAPTURE_DATE ");
            sb.Append("         ,DE.IMAGE_CAPTURE_NUM ");
            sb.Append("         ,DE.DOC_ID ");
            sb.Append("         ,DE.ENTRY_UNIT ");
            sb.Append("         ,DE.IMAGE_SEQ ");
            sb.Append("HAVING SUM(TO_NUMBER(NVL(DE.VALUE,0))) != 1 ");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sb.ToString(), param);
        }

        public int UPDATE_D_IMAGE_INFO(DataRow drTergetImage)
        {
            var sb = new StringBuilder();
            var param = new List<object>();
            sb.AppendLine("UPDATE D_IMAGE_INFO");
            sb.AppendLine("   SET OCR_NG_STATUS=?");param.Add(Consts.Flag.ON);
            sb.AppendLine("      ,UPD_USER_ID=?"); param.Add("PreLogicalCheck");
            sb.AppendLine("      ,UPD_DATE=SYSDATE");
            sb.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sb.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
            sb.AppendLine("   AND IMAGE_CAPTURE_DATE=?"); param.Add(drTergetImage["IMAGE_CAPTURE_DATE"].ToString());
            sb.AppendLine("   AND IMAGE_CAPTURE_NUM=?"); param.Add(drTergetImage["IMAGE_CAPTURE_NUM"].ToString());
            sb.AppendLine("   AND DOC_ID=?"); param.Add(drTergetImage["DOC_ID"].ToString());
            sb.AppendLine("   AND ENTRY_UNIT=?"); param.Add(drTergetImage["ENTRY_UNIT"].ToString());
            sb.AppendLine("   AND EMAGE_SEQ=?"); param.Add(int.Parse(drTergetImage["IMAGE_SEQ"].ToString()));
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sb.ToString(), param);
        }
    }
}
