using Common;
using ODPCtrl;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dao
{
    /// <summary>
    /// OCR取込み
    /// </summary>
    public class DaoBatchImportOcrCsv : DaoBase
    {
        /// <summary>
        /// M_ITEM_ID_CONV_OCR 取得
        /// </summary>
        /// <returns></returns>
        public DataTable SELECT_M_ITEM_ID_CONV_OCR()
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT * FROM M_ITEM_ID_CONV_OCR");
            //sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            //sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine(" ORDER BY TKSK_CD,HNM_CD,DOC_ID,ITEM_ID_OCR");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        ///  M_DOC 取得
        /// </summary>
        /// <returns></returns>
        public DataTable SELECT_M_DOC()
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT * FROM M_DOC");
            //sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            //sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine(" WHERE OCR_COOPERATION_FLAG=?"); param.Add(Consts.Flag.ON);
            sql.AppendLine(" ORDER BY TKSK_CD,HNM_CD,DOC_ID");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// D_IMAGE_INFO 更新
        /// </summary>
        /// <param name="entryUnitId"></param>
        /// <param name="imageSeq"></param>
        /// <returns></returns>
        public int UPDATE_D_IMAGE_INFO(string entryUnitId, int imageSeq)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_IMAGE_INFO");
            sql.AppendLine("   SET OCR_IMPORT_DATE=SYSDATE");
            sql.AppendLine("     , UPD_USER_ID=?"); param.Add(GVal.UserId);
            sql.AppendLine("     , UPD_DATE=SYSDATE ");
            //sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(sTkskCd);
            //sql.AppendLine("   AND HNM_CD=?"); param.Add(sHnmCd);
            //sql.AppendLine("   AND IMAGE_CAPTURE_DATE=?"); param.Add(sImageCaptureDate);
            //sql.AppendLine("   AND IMAGE_CAPTURE_NUM=?"); param.Add(sImageCaptureNum);
            //sql.AppendLine("   AND DOC_ID=?"); param.Add(sDocId);
            //sql.AppendLine("   AND ENTRY_UNIT=?"); param.Add(sEntryUnit);
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(entryUnitId);
            sql.AppendLine("   AND IMAGE_SEQ=?"); param.Add(imageSeq);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// D_IMAGE_INFO 取得
        /// </summary>
        /// <param name="ocrImageFileName"></param>
        /// <returns></returns>
        public DataTable SELECT_D_IMAGE_INFO_OCR_IMAGE_FILE_NAME(string ocrImageFileName)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT DII.*");
            sql.AppendLine("      ,DEU.STATUS");
            sql.AppendLine("      ,MD.NO_ENTRY_FLAG");
            sql.AppendLine("      ,MD.PRE_LOGICAL_CHECK_FLAG");
            sql.AppendLine("      ,MD.OCR_IMPORT_1ST_FLAG");
            sql.AppendLine("      ,MD.OCR_IMPORT_2ND_FLAG");
            sql.AppendLine("      ,MD.SINGLE_ENTRY_FLAG");
            sql.AppendLine("      ,MD.OCR_IMPORT_UNIT_STATUS");
            sql.AppendLine("  FROM D_IMAGE_INFO DII");

            sql.AppendLine(" INNER JOIN D_ENTRY_UNIT DEU");
            sql.AppendLine("    ON DEU.ENTRY_UNIT_ID=DII.ENTRY_UNIT_ID");
            //sql.AppendLine("        AND DEU.TKSK_CD=?"); param.Add(tkskCd);
            //sql.AppendLine("        AND DEU.HNM_CD=?"); param.Add(hnmCd);
            //sql.AppendLine("        AND DEU.HNM_CD=DII.HNM_CD");
            //sql.AppendLine("        AND DEU.IMAGE_CAPTURE_DATE=DII.IMAGE_CAPTURE_DATE");
            //sql.AppendLine("        AND DEU.IMAGE_CAPTURE_NUM=DII.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("        AND DEU.DOC_ID=DII.DOC_ID");
            //sql.AppendLine("        AND DEU.ENTRY_UNIT=DII.ENTRY_UNIT");

            sql.AppendLine(" INNER JOIN M_DOC MD");
            sql.AppendLine("    ON MD.TKSK_CD=DEU.TKSK_CD");
            sql.AppendLine("   AND MD.HNM_CD=DEU.HNM_CD");
            sql.AppendLine("   AND MD.DOC_ID=DEU.DOC_ID");

            //sql.AppendLine("        AND MD.TKSK_CD=DII.TKSK_CD");
            //sql.AppendLine("        AND MD.HNM_CD=DII.HNM_CD");
            //sql.AppendLine("        AND MD.DOC_ID=DII.DOC_ID");

            //sql.AppendLine(" WHERE DII.TKSK_CD=?"); param.Add(sTkskCd);
            //sql.AppendLine("   AND DII.HNM_CD=?"); param.Add(sHnmCd);
            sql.AppendLine(" WHERE DII.OCR_IMAGE_FILE_NAME=?"); param.Add(ocrImageFileName);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// D_IMAGE_INFO 取得
        /// </summary>
        /// <param name="entryUnitId"></param>
        /// <returns></returns>
        public DataTable SELECT_D_IMAGE_INFO_ENTRY_UNIT_ID(string entryUnitId)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT OCR_IMPORT_DATE");
            sql.AppendLine("  FROM D_IMAGE_INFO");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(entryUnitId);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// D_ENTRY 更新
        /// </summary>
        /// <param name="entryUnitId"></param>
        /// <param name="imageSeq"></param>
        /// <param name="recordKbn"></param>
        /// <param name="itemId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int UPDATE_D_ENTRY(string entryUnitId, int imageSeq, string recordKbn, string itemId, string value)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_ENTRY");
            sql.AppendLine("   SET VALUE=?"); param.Add(value.Trim());
            if (Consts.RecordKbn.Entry_1st.Equals(recordKbn))
            {
                sql.AppendLine("      ,OCR_VALUE=?"); param.Add(value.Trim());
            }
            sql.AppendLine("      ,UPD_USER_ID=?"); param.Add(GVal.UserId);
            sql.AppendLine("      ,UPD_DATE=SYSDATE ");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(entryUnitId);
            sql.AppendLine("   AND IMAGE_SEQ=?"); param.Add(imageSeq);
            sql.AppendLine("   AND RECORD_KBN=?"); param.Add(recordKbn);
            sql.AppendLine("   AND ITEM_ID=?"); param.Add(itemId);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// D_ENTRY_UNIT 更新
        /// </summary>
        /// <param name="entryUnitId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UPDATE_D_ENTRY_UNIT(string entryUnitId, string status)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_ENTRY_UNIT");
            sql.AppendLine("   SET OCR_IMPORT_USER_ID=?"); param.Add(GVal.UserId);
            sql.AppendLine("      ,STATUS=DECODE(STATUS,'0',?,'2')"); param.Add(status);
            sql.AppendLine("      ,UPD_USER_ID=?"); param.Add(GVal.UserId);
            sql.AppendLine("      ,UPD_DATE=SYSDATE");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(entryUnitId);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// D_ENTRY_STATUS 更新
        /// </summary>
        /// <param name="entryUnitId"></param>
        /// <param name="recordKbn"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UPDATE_D_ENTRY_STATUS(string entryUnitId, string recordKbn, string status = null)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_ENTRY_STATUS DES");
            sql.AppendLine("   SET DES.OCR_IMPORT_USER_ID=?"); param.Add(GVal.UserId);
            if (status != null)
            {
                sql.AppendLine("  ,DES.ENTRY_STATUS=?"); param.Add(Consts.EntryStatus.ENTRY_END);
                sql.AppendLine("  ,DES.ENTRY_USER_ID=?"); param.Add(GVal.UserId);
            }
            sql.AppendLine("      ,DES.UPD_USER_ID=?"); param.Add(GVal.UserId);
            sql.AppendLine("      ,DES.UPD_DATE=SYSDATE");
            sql.AppendLine(" WHERE DES.ENTRY_UNIT_ID=?"); param.Add(entryUnitId);
            sql.AppendLine("   AND DES.RECORD_KBN=?"); param.Add(recordKbn);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// D_OCR_COOPERATION_HISTORY 更新
        /// </summary>
        /// <param name="ocrImageFileName"></param>
        /// <returns></returns>
        public int UPDATE_D_OCR_COOPERATION_HISTORY(string ocrImageFileName)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_OCR_COOPERATION_HISTORY");
            sql.AppendLine("   SET OCR_CSV_IMPORT_DATE=SYSDATE");
            sql.AppendLine("      ,UPD_USER_ID=?"); param.Add(GVal.UserId);
            sql.AppendLine("      ,UPD_DATE=SYSDATE");
            sql.AppendLine(" WHERE OCR_IMAGE_FILE_NAME=?"); param.Add(ocrImageFileName);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        public DataTable SELECT_M_BUSINESS()
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT *");
            sql.AppendLine("  FROM M_BUSINESS");
            sql.AppendLine(" WHERE INVALID_FLAG!=?"); param.Add(Consts.Flag.ON);
            sql.AppendLine(" ORDER BY DISPLAY_ORDER,BUSINESS_ID");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }
    }
}

