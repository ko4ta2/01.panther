using BPOEntry.Tables;
using Common;
using ODPCtrl;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BPOEntry.DivideEntryUnit
{
    public class DaoDivideEntryUnit : DaoBase
    {
        /// <summary>
        /// 入力単位分割済みの画像リストを取得します。
        /// </summary>
        /// <param name="date">画像取込日</param>
        /// <param name="capNum">画像取込回数</param>
        //public DataTable SelectDividedImages(string date, string capNum)
        //{
        //    var sql = new StringBuilder();
        //    var param = new List<object>();
        //    sql.AppendLine("SELECT IMAGE_PATH");
        //    sql.AppendLine("  FROM D_IMAGE_INFO");
        //    sql.AppendLine(" WHERE IMAGE_CAPTURE_DATE=?"); param.Add(date);
        //    sql.AppendLine("   AND IMAGE_CAPTURE_NUM=?"); param.Add(capNum);
        //    sql.AppendLine("   AND TKSK_CD=?"); param.Add(Common.Config.TokuisakiCode);
        //    sql.AppendLine("   AND HNM_CD=?"); param.Add(Common.Config.HinmeiCode);
        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        //}

        /// <summary>
        /// 各帳票の入力項目数を取得します。
        /// </summary>
        //public DataTable SelectEntryItemNum(string sDocId)
        //{
        //    var sql = new StringBuilder();
        //    var param = new List<object>();
        //    sql.AppendLine("SELECT * FROM M_DOC ");
        //    sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Common.Config.TokuisakiCode);
        //    sql.AppendLine("   AND HNM_CD=?"); param.Add(Common.Config.HinmeiCode);
        //    sql.AppendLine("   AND DOC_ID=?"); param.Add(sDocId);
        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        //}

        /// <summary>
        /// 入力単位データを登録します。
        /// </summary>
        /// <param name="record">登録情報</param>
        /// <returns>件数</returns>
        public int INSERT_D_ENTRY_UNIT(D_ENTRY_UNIT deu)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("INSERT INTO D_ENTRY_UNIT(");
            sql.AppendLine("       TKSK_CD"); param.Add(deu.TKSK_CD);
            sql.AppendLine("      ,HNM_CD"); param.Add(deu.HNM_CD);
            sql.AppendLine("      ,DOC_ID"); param.Add(deu.DOC_ID);
            sql.AppendLine("      ,IMAGE_CAPTURE_DATE"); param.Add(deu.IMAGE_CAPTURE_DATE);
            sql.AppendLine("      ,IMAGE_CAPTURE_NUM"); param.Add(deu.IMAGE_CAPTURE_NUM);
            sql.AppendLine("      ,ENTRY_UNIT"); param.Add(deu.ENTRY_UNIT);
            sql.AppendLine("      ,STATUS"); param.Add(deu.ENTRY_UNIT_STATUS);
            sql.AppendLine("      ,ENTRY_UNIT_ID"); param.Add(deu.ENTRY_UNIT_ID);
            sql.AppendLine("     ,INS_USER_ID"); param.Add(deu.INS_USER_ID);
            sql.AppendLine(")");
            sql.AppendLine("VALUES(?,?,?,?,?,?,?,?,?)");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 画像情報データを登録します。
        /// </summary>
        /// <param name="dii">登録情報</param>
        /// <returns>件数</returns>
        public int INSERT_D_IMAGE_INFO(D_IMAGE_INFO dii)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("INSERT INTO D_IMAGE_INFO(");
            //sql.AppendLine("       TKSK_CD"); param.Add(Common.Config.TokuisakiCode);
            //sql.AppendLine("      ,HNM_CD"); param.Add(Common.Config.HinmeiCode);
            //sql.AppendLine("      ,DOC_ID"); param.Add(dii.DOC_ID);
            //sql.AppendLine("      ,IMAGE_CAPTURE_DATE"); param.Add(dii.IMAGE_CAPTURE_DATE);
            //sql.AppendLine("      ,IMAGE_CAPTURE_NUM"); param.Add(dii.IMAGE_CAPTURE_NUM);
            //sql.AppendLine("      ,ENTRY_UNIT"); param.Add(dii.ENTRY_UNIT);
            sql.AppendLine("       ENTRY_UNIT_ID"); param.Add(dii.ENTRY_UNIT_ID);
            sql.AppendLine("      ,IMAGE_SEQ"); param.Add(dii.IMAGE_SEQ);
            sql.AppendLine("      ,IMAGE_PATH"); param.Add(dii.IMAGE_PATH);
            sql.AppendLine("      ,OCR_IMAGE_FILE_NAME"); param.Add(dii.OCR_IMAGE_FILE_NAME);
            sql.AppendLine("      ,INS_USER_ID"); param.Add(dii.INS_USER_ID);
            sql.AppendLine(")");
            sql.AppendLine("VALUES(?,?,?,?,?)");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 入力状態データを登録します。
        /// </summary>
        /// <param name="des">登録情報</param>
        /// <returns>件数</returns>
        public int INSERT_D_ENTRY_STATUS(D_ENTRY_STATUS des)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("INSERT INTO D_ENTRY_STATUS(");
            //sql.AppendLine("       TKSK_CD"); param.Add(Common.Config.TokuisakiCode);
            //sql.AppendLine("      ,HNM_CD"); param.Add(Common.Config.HinmeiCode);
            //sql.AppendLine("      ,DOC_ID"); param.Add(des.DOC_ID);
            //sql.AppendLine("      ,IMAGE_CAPTURE_DATE"); param.Add(des.IMAGE_CAPTURE_DATE);
            //sql.AppendLine("      ,IMAGE_CAPTURE_NUM"); param.Add(des.IMAGE_CAPTURE_NUM);
            //sql.AppendLine("      ,ENTRY_UNIT"); param.Add(des.ENTRY_UNIT);
            sql.AppendLine("       ENTRY_UNIT_ID"); param.Add(des.ENTRY_UNIT_ID);
            sql.AppendLine("      ,RECORD_KBN"); param.Add(des.RECORD_KBN);
            sql.AppendLine("      ,INS_USER_ID"); param.Add(des.INS_USER_ID);
            sql.AppendLine(") ");
            sql.AppendLine("VALUES (?,?,?)");
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
            sql.AppendLine("INSERT INTO D_ENTRY(");
            //sql.AppendLine("       TKSK_CD"); param.Add(de.TKSK_CD);
            //sql.AppendLine("      ,HNM_CD"); param.Add(de.HNM_CD);
            //sql.AppendLine("      ,IMAGE_CAPTURE_DATE"); param.Add(de.IMAGE_CAPTURE_DATE);
            //sql.AppendLine("      ,IMAGE_CAPTURE_NUM"); param.Add(de.IMAGE_CAPTURE_NUM);
            //sql.AppendLine("      ,DOC_ID"); param.Add(de.DOC_ID);
            //sql.AppendLine("      ,ENTRY_UNIT"); param.Add(de.ENTRY_UNIT);
            sql.AppendLine("       ENTRY_UNIT_ID"); param.Add(de.ENTRY_UNIT_ID);
            sql.AppendLine("      ,IMAGE_SEQ"); param.Add(de.IMAGE_SEQ);
            sql.AppendLine("      ,RECORD_KBN"); param.Add(de.RECORD_KBN);
            sql.AppendLine("      ,ITEM_ID"); param.Add(de.ITEM_ID);
            sql.AppendLine("      ,INS_USER_ID"); param.Add(de.INS_USER_ID);
            sql.AppendLine(")");
            sql.AppendLine("VALUES(?,?,?,?,?)");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public DataTable SELECT_M_DOC(string docId = "*")
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT DOC_ID");
            sql.AppendLine("      ,DOC_ID || ':' || DOC_NAME AS DOC_NAME");
            sql.AppendLine("      ,ENTRY_UNIT_NUM");
            sql.AppendLine("      ,ENTRY_ITEMS_NUM");
            sql.AppendLine("  FROM M_DOC");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Common.Config.TokuisakiCode);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(Common.Config.HinmeiCode);
            sql.AppendLine("   AND ENTRY_METHOD=?"); param.Add(Consts.EntryMethod.PAPER);
            if (!"*".Equals(docId))
            {
                sql.AppendLine("   AND DOC_ID=?"); param.Add(docId);
            }
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sImageCaptureDate"></param>
        /// <param name="sImageCaptureNum"></param>
        /// <returns></returns>
        public int SELECT_D_ENTRY_UNIT(string sImageCaptureDate,string sImageCaptureNum)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT COUNT(*)");
            sql.AppendLine("  FROM D_ENTRY_UNIT");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Common.Config.TokuisakiCode);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(Common.Config.HinmeiCode);
            sql.AppendLine("   AND IMAGE_CAPTURE_DATE=?"); param.Add(sImageCaptureDate);
            sql.AppendLine("   AND IMAGE_CAPTURE_NUM=?"); param.Add(sImageCaptureNum);
            sql.AppendLine("   AND ENTRY_UNIT=?"); param.Add("001");
            return int.Parse(ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param).Rows[0][0].ToString());
        }
    }
}
