using BPOEntry.Tables;
using Common;
using ODPCtrl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dao
{
    /// <summary>
    /// エントリユニット作成
    /// </summary>
    public class DaoCreateEntryUnit : DaoBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageCapturePath"></param>
        /// <returns></returns>
        public int INSERT_T_DIV_ENTRY_UNIT_PATH(string tkskCd, string hnmCd, string imageCapturePath)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("INSERT INTO T_DIV_ENTRY_UNIT_PATH");
            sql.AppendLine("      (TKSK_CD"); param.Add(tkskCd);
            sql.AppendLine("      ,HNM_CD"); param.Add(hnmCd);
            sql.AppendLine("      ,IMAGE_CAPTURE_PATH"); param.Add(imageCapturePath);
            sql.AppendLine(")VALUES(?,?,?)");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageCaptureDate"></param>
        /// <param name="imageCaptureNum"></param>
        /// <returns></returns>
        public DataTable SELECT_D_IMAGE_INFO(string tkskCd, string hnmCd, string imageCaptureDate, string imageCaptureNum)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT DII.IMAGE_PATH");
            sql.AppendLine("  FROM D_IMAGE_INFO DII");
            sql.AppendLine(" INNER JOIN D_ENTRY_UNIT DEU");
            sql.AppendLine("    ON DEU.ENTRY_UNIT_ID=DII.ENTRY_UNIT_ID");
            sql.AppendLine(" WHERE DEU.TKSK_CD=?"); param.Add(tkskCd);
            sql.AppendLine("   AND DEU.HNM_CD=?"); param.Add(hnmCd);
            sql.AppendLine("   AND DEU.IMAGE_CAPTURE_DATE=?"); param.Add(imageCaptureDate);
            sql.AppendLine("   AND DEU.IMAGE_CAPTURE_NUM=?"); param.Add(imageCaptureNum);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tkskCd"></param>
        /// <param name="hnmCd"></param>
        /// <param name="BATCH_CREATE_UNIT_FLAG"></param>
        /// <returns></returns>
        public DataTable SELECT_M_DOC(string tkskCd, string hnmCd, string BATCH_CREATE_UNIT_FLAG)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT * FROM M_DOC ");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(tkskCd);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(hnmCd);
            sql.AppendLine("   AND BATCH_CREATE_UNIT_FLAG=?"); param.Add(BATCH_CREATE_UNIT_FLAG);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deu"></param>
        /// <returns></returns>
        public int INSERT_D_ENTRY_UNIT(D_ENTRY_UNIT deu)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("INSERT INTO D_ENTRY_UNIT");
            sql.AppendLine("      (TKSK_CD"); param.Add(deu.TKSK_CD);
            sql.AppendLine("      ,HNM_CD"); param.Add(deu.HNM_CD);
            sql.AppendLine("      ,IMAGE_CAPTURE_DATE"); param.Add(deu.IMAGE_CAPTURE_DATE);
            sql.AppendLine("      ,IMAGE_CAPTURE_NUM"); param.Add(deu.IMAGE_CAPTURE_NUM);
            sql.AppendLine("      ,DOC_ID"); param.Add(deu.DOC_ID);
            sql.AppendLine("      ,ENTRY_UNIT"); param.Add(deu.ENTRY_UNIT);
            sql.AppendLine("      ,STATUS"); param.Add(deu.ENTRY_UNIT_STATUS);
            sql.AppendLine("      ,ENTRY_UNIT_ID"); param.Add(deu.ENTRY_UNIT_ID);
            sql.AppendLine("      ,INS_USER_ID"); param.Add(deu.INS_USER_ID);
            sql.AppendLine(")VALUES(?,?,?,?,?,?,?,?,?)");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dii"></param>
        /// <returns></returns>
        public int INSERT_D_IMAGE_INFO(D_IMAGE_INFO dii)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("INSERT INTO D_IMAGE_INFO");
            sql.AppendLine("      (ENTRY_UNIT_ID"); param.Add(dii.ENTRY_UNIT_ID);
            sql.AppendLine("      ,IMAGE_SEQ "); param.Add(dii.IMAGE_SEQ);
            sql.AppendLine("      ,IMAGE_PATH "); param.Add(dii.IMAGE_PATH);
            sql.AppendLine("      ,OCR_IMAGE_FILE_NAME"); param.Add(dii.OCR_IMAGE_FILE_NAME);
            sql.AppendLine("      ,DUMMY_IMAGE_FLAG"); param.Add(dii.DUMMY_IMAGE_FLAG);
            sql.AppendLine("      ,INS_USER_ID "); param.Add(dii.INS_USER_ID);
            //if (Consts.BusinessID.CDC.Equals(String.Join("_", dii.TKSK_CD, dii.HNM_CD)))
            //{
            //    sql.AppendLine("      ,TKSK_CD"); param.Add(dii.TKSK_CD);
            //    sql.AppendLine("      ,HNM_CD"); param.Add(dii.HNM_CD);
            //    sql.AppendLine("      ,IMAGE_CAPTURE_DATE"); param.Add(dii.IMAGE_CAPTURE_DATE);
            //    sql.AppendLine("      ,IMAGE_CAPTURE_NUM"); param.Add(dii.IMAGE_CAPTURE_NUM);
            //    sql.AppendLine("      ,DOC_ID"); param.Add(dii.DOC_ID);
            //    sql.AppendLine("      ,ENTRY_UNIT"); param.Add(dii.ENTRY_UNIT);
            //    sql.AppendLine(")VALUES(?,?,?,?,?,?,?,?,?,?,?,?)");
            //}
            //else
            //{
                sql.AppendLine(")VALUES(?,?,?,?,?,?)");
            //}
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="des"></param>
        /// <returns></returns>
        public int INSERT_D_ENTRY_STATUS(D_ENTRY_STATUS des)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("INSERT INTO D_ENTRY_STATUS");
            sql.AppendLine("      (ENTRY_UNIT_ID"); param.Add(des.ENTRY_UNIT_ID);
            sql.AppendLine("      ,RECORD_KBN"); param.Add(des.RECORD_KBN);
            sql.AppendLine("      ,INS_USER_ID"); param.Add(des.INS_USER_ID);
            //if (Consts.BusinessID.CDC.Equals(String.Join("_", des.TKSK_CD, des.HNM_CD)))
            //{
            //    sql.AppendLine("      ,TKSK_CD"); param.Add(des.TKSK_CD);
            //    sql.AppendLine("      ,HNM_CD"); param.Add(des.HNM_CD);
            //    sql.AppendLine("      ,IMAGE_CAPTURE_DATE"); param.Add(des.IMAGE_CAPTURE_DATE);
            //    sql.AppendLine("      ,IMAGE_CAPTURE_NUM"); param.Add(des.IMAGE_CAPTURE_NUM);
            //    sql.AppendLine("      ,DOC_ID"); param.Add(des.DOC_ID);
            //    sql.AppendLine("      ,ENTRY_UNIT"); param.Add(des.ENTRY_UNIT);
            //    sql.AppendLine(")VALUES(?,?,?,?,?,?,?,?,?)");
            //}
            //else
            //{
                sql.AppendLine(")VALUES(?,?,?)");
            //}
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public int INSERT_D_ENTRY(D_ENTRY de)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("INSERT INTO D_ENTRY");
            sql.AppendLine("      (ENTRY_UNIT_ID"); param.Add(de.ENTRY_UNIT_ID);
            sql.AppendLine("      ,IMAGE_SEQ"); param.Add(de.IMAGE_SEQ);
            sql.AppendLine("      ,RECORD_KBN"); param.Add(de.RECORD_KBN);
            sql.AppendLine("      ,ITEM_ID"); param.Add(de.ITEM_ID);
            sql.AppendLine("      ,DUMMY_ITEM_FLAG"); param.Add(de.DUMMY_ITEM_FLAG);
            sql.AppendLine("      ,INS_USER_ID"); param.Add(de.INS_USER_ID);
            //if (Consts.BusinessID.CDC.Equals(String.Join("_", de.TKSK_CD, de.HNM_CD)))
            //{
            //    sql.AppendLine("      ,TKSK_CD"); param.Add(de.TKSK_CD);
            //    sql.AppendLine("      ,HNM_CD"); param.Add(de.HNM_CD);
            //    sql.AppendLine("      ,IMAGE_CAPTURE_DATE"); param.Add(de.IMAGE_CAPTURE_DATE);
            //    sql.AppendLine("      ,IMAGE_CAPTURE_NUM"); param.Add(de.IMAGE_CAPTURE_NUM);
            //    sql.AppendLine("      ,DOC_ID"); param.Add(de.DOC_ID);
            //    sql.AppendLine("      ,ENTRY_UNIT"); param.Add(de.ENTRY_UNIT);
            //    sql.AppendLine(")VALUES(?,?,?,?,?,?,?,?,?,?,?,?)");
            //}
            //else
            //{
                sql.AppendLine(")VALUES(?,?,?,?,?,?)");
            //}
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="srcFolderName"></param>
        /// <param name="srcImageFileName"></param>
        /// <param name="ocrImageFileName"></param>
        /// <returns></returns>
        public int INSERT_D_OCR_COOPERATION_HISTORY(string tkskCd, string hnmCd, string docId, string srcFolderName, string srcImageFileName, string ocrImageFileName)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("INSERT INTO D_OCR_COOPERATION_HISTORY");
            sql.AppendLine("      (TKSK_CD"); param.Add(tkskCd);
            sql.AppendLine("      ,HNM_CD"); param.Add(hnmCd);
            sql.AppendLine("      ,DOC_ID"); param.Add(docId);
            sql.AppendLine("      ,SRC_FOLDER_NAME"); param.Add(srcFolderName);
            sql.AppendLine("      ,SRC_IMAGE_FILE_NAME"); param.Add(srcImageFileName);
            sql.AppendLine("      ,OCR_IMAGE_FILE_NAME"); param.Add(ocrImageFileName);
            sql.AppendLine("      ,INS_USER_ID"); param.Add(GVal.UserId);
            sql.AppendLine(")VALUES(?,?,?,?,?,?,?)");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }
    }
}
