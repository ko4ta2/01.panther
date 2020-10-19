using System.Collections;
using System.Collections.Generic;
using System.Text;
using Common;
using NLog;
using ODPCtrl;

namespace Dao
{
    /// <summary>
    /// エントリ単位分割後処理　データアクセス
    /// 大和証券　トップダウンアンケート入力業務
    /// </summary>
    public class DaoAfterImport : ComBasicDAO
    {
        /// <summary>
        /// ログ出力
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        #region エントリ単位分割後処理
        /// <summary>
        /// エントリ単位分割後処理
        /// </summary>
        /// <param name="sPpNo"></param>
        /// <param name="dEntry"></param>
        /// <param name="sEntryDate"></param>
        /// <param name="sEntryType"></param>
        /// <returns></returns>
        public int AfterImportBatch(string sImageCaptureDate, string sImageCaptureNum)
        {
            var sSQL = new List<string>();
            var sql = new StringBuilder();
            var param = new ArrayList();

            // キャンペーンコード
            sql.AppendLine(@"UPDATE D_ENTRY DE SET DE.VALUE =(SELECT SUBSTRB(REPLACE(IMAGE_PATH, RTRIM(DII.IMAGE_PATH,REPLACE(DII.IMAGE_PATH, '\', '')), ''),1,5) FROM D_IMAGE_INFO DII WHERE DII.DOC_ID=DE.DOC_ID AND DII.IMAGE_CAPTURE_DATE=DE.IMAGE_CAPTURE_DATE AND DII.IMAGE_CAPTURE_NUM=DE.IMAGE_CAPTURE_NUM AND DII.ENTRY_UNIT=DE.ENTRY_UNIT AND DII.IMAGE_SEQ=DE.IMAGE_SEQ)");
            sql.AppendLine(@" WHERE DE.TKSK_CD=? AND DE.HNM_CD=? AND DE.ITEM_ID='ITEM_001' AND DE.VALUE IS NULL AND DE.IMAGE_CAPTURE_DATE=? AND DE.IMAGE_CAPTURE_NUM=?");
            param.Add(Config.TokuisakiCode);
            param.Add(Config.HinmeiCode);
            param.Add(sImageCaptureDate);
            param.Add(sImageCaptureNum);
            sSQL.Add(sql.ToString());

            // SEQ
            sql.Clear();
            sql.AppendLine(@"UPDATE D_ENTRY DE SET DE.VALUE =(SELECT SUBSTRB(REPLACE(IMAGE_PATH, RTRIM(DII.IMAGE_PATH,REPLACE(DII.IMAGE_PATH, '\', '')), ''),7,11) FROM D_IMAGE_INFO DII WHERE DII.DOC_ID=DE.DOC_ID AND DII.IMAGE_CAPTURE_DATE=DE.IMAGE_CAPTURE_DATE AND DII.IMAGE_CAPTURE_NUM=DE.IMAGE_CAPTURE_NUM AND DII.ENTRY_UNIT=DE.ENTRY_UNIT AND DII.IMAGE_SEQ=DE.IMAGE_SEQ)");
            sql.AppendLine(@" WHERE DE.TKSK_CD=? AND DE.HNM_CD=? AND DE.ITEM_ID='ITEM_002' AND DE.VALUE IS NULL AND DE.IMAGE_CAPTURE_DATE=? AND DE.IMAGE_CAPTURE_NUM=?");
            sSQL.Add(sql.ToString());

            // スキャン日
            sql.Clear();
            sql.AppendLine(@"UPDATE D_ENTRY DE SET DE.VALUE =(SELECT SUBSTRB(REPLACE(IMAGE_PATH, RTRIM(DII.IMAGE_PATH,REPLACE(DII.IMAGE_PATH, '\', '')), ''),19,8) FROM D_IMAGE_INFO DII WHERE DII.DOC_ID=DE.DOC_ID AND DII.IMAGE_CAPTURE_DATE=DE.IMAGE_CAPTURE_DATE AND DII.IMAGE_CAPTURE_NUM=DE.IMAGE_CAPTURE_NUM AND DII.ENTRY_UNIT=DE.ENTRY_UNIT AND DII.IMAGE_SEQ=DE.IMAGE_SEQ)");
            sql.AppendLine(@" WHERE DE.TKSK_CD=? AND DE.HNM_CD=? AND DE.ITEM_ID='ITEM_003' AND DE.VALUE IS NULL AND DE.IMAGE_CAPTURE_DATE=? AND DE.IMAGE_CAPTURE_NUM=?");
            sSQL.Add(sql.ToString());

            var iUpdateCount = 0;
            foreach (string s in sSQL)
            {
                iUpdateCount += ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(s, param);
            }
            return iUpdateCount;
        }
        #endregion
    }
}