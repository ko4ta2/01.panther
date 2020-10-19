using Common;
using ODPCtrl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPOEntry.EntryForms
{
    /// <summary>
    /// 入力画面共通DAO
    /// </summary>
    public class DaoEntry : DaoBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ENTRY_UNIT_ID"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public DataTable SELECT_D_IMAGE_INFO(string ENTRY_UNIT_ID, bool isAdmin = false)
        {
            var sql = new StringBuilder();
            var param = new List<object>();

            if (isAdmin)
            {
                if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
                {
                    sql.AppendLine("SELECT DII.IMAGE_SEQ, DII.IMAGE_PATH, DII.OCR_IMAGE_FILE_NAME,DII.RECORD_KBN_0_ENTRY_FLAG,WEWT.BASE_IMAGE_FILE_PATH");
                    sql.AppendLine("  FROM D_IMAGE_INFO DII");
                    sql.AppendLine("  LEFT JOIN WORK_ENTRY_WAIT_TBL WEWT");
                    sql.AppendLine("    ON WEWT.IMAGE_FILE_PATH=DII.IMAGE_PATH");
                }
                else
                {
                    sql.AppendLine("SELECT DII.IMAGE_SEQ, DII.IMAGE_PATH, DII.OCR_IMAGE_FILE_NAME,DII.RECORD_KBN_0_ENTRY_FLAG,NULL BASE_IMAGE_FILE_PATH");
                    sql.AppendLine("  FROM D_IMAGE_INFO DII");
                }
                sql.AppendLine(" WHERE DII.ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
                sql.AppendLine("   AND EXISTS (SELECT 'X'");
                sql.AppendLine("	             FROM D_ENTRY DE");
                sql.AppendLine("	            WHERE DE.ENTRY_UNIT_ID=DII.ENTRY_UNIT_ID");
                sql.AppendLine("	              AND DE.IMAGE_SEQ=DII.IMAGE_SEQ");
                sql.AppendLine("	              AND DE.RECORD_KBN=?"); param.Add(Consts.RecordKbn.ADMIN);
                sql.AppendLine("	              AND DE.DIFF_FLAG=?"); param.Add(Consts.Flag.ON);
                sql.AppendLine("              ) ");
                sql.AppendLine(" ORDER BY DII.IMAGE_SEQ");
            }
            else
            {
                if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
                {
                    sql.AppendLine("SELECT IMAGE_SEQ,IMAGE_PATH,OCR_IMAGE_FILE_NAME,RECORD_KBN_0_ENTRY_FLAG,RECORD_KBN_1_ENTRY_FLAG,RECORD_KBN_2_ENTRY_FLAG,WEWT.BASE_IMAGE_FILE_PATH");
                    sql.AppendLine("  FROM D_IMAGE_INFO");
                    sql.AppendLine("  LEFT JOIN WORK_ENTRY_WAIT_TBL WEWT");
                    sql.AppendLine("    ON WEWT.IMAGE_FILE_PATH=IMAGE_PATH");
                }
                else
                {
                    sql.AppendLine("SELECT IMAGE_SEQ,IMAGE_PATH,OCR_IMAGE_FILE_NAME,RECORD_KBN_0_ENTRY_FLAG,RECORD_KBN_1_ENTRY_FLAG,RECORD_KBN_2_ENTRY_FLAG,NULL BASE_IMAGE_FILE_PATH");
                    sql.AppendLine("  FROM D_IMAGE_INFO");
                }
                sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
                sql.AppendLine(" ORDER BY IMAGE_SEQ");
            }
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ENTRY_UNIT_ID"></param>
        /// <param name="RECORD_KBN"></param>
        /// <param name="IMAGE_SEQ"></param>
        /// <returns></returns>
        public DataTable SELECT_D_ENTRY(string ENTRY_UNIT_ID, string RECORD_KBN, int IMAGE_SEQ)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            //sql.AppendLine("SELECT nvl(VALUE,'_null_') as value");
            //sql.AppendLine("      ,nvl(DIFF_FLAG,'0') as diff_flag");

            sql.AppendLine("SELECT VALUE");
            sql.AppendLine("      ,DIFF_FLAG");
            sql.AppendLine("      ,DUMMY_ITEM_FLAG");

            sql.AppendLine("  FROM D_ENTRY");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
            sql.AppendLine("   AND RECORD_KBN=?"); param.Add(RECORD_KBN);
            sql.AppendLine("   AND IMAGE_SEQ=?"); param.Add(IMAGE_SEQ);
            sql.AppendLine(" ORDER BY ITEM_ID");

            //var dt = ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
            //foreach (var dr in dt.AsEnumerable())
            //    if ("_null_".Equals(dr["value"].ToString()))
            //            dr["value"] = string.Empty;

            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param); 
        }

        /// <summary>
        /// D_ENTRY_STATUS 取得
        /// </summary>
        /// <param name="ENTRY_UNIT_ID"></param>
        /// <param name="RECORD_KBN"></param>
        /// <returns></returns>
        public DataTable SELECT_D_ENTRY_STATUS(string ENTRY_UNIT_ID, string RECORD_KBN)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT RECORD_KBN,ENTRY_STATUS");
            sql.AppendLine("  FROM D_ENTRY_STATUS");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
            sql.AppendLine("   AND RECORD_KBN=?"); param.Add(RECORD_KBN);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// ユーザ名取得
        /// </summary>
        /// <param name="sDocId"></param>
        /// <param name="sImageCaptureDate"></param>
        /// <param name="sImageCaptureNum"></param>
        /// <param name="sEntryUnit"></param>
        /// <returns></returns>
        public DataTable SelectUserName(string ENTRY_UNIT_ID, int IMAGE_SEQ, string ITEM_ID)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT DE.RECORD_KBN");
            sql.AppendLine("      ,DE.VALUE");
            sql.AppendLine("      ,DES.ENTRY_USER_ID");
            sql.AppendLine("      ,MU.USER_NAME");
            sql.AppendLine("  FROM D_ENTRY DE");
            sql.AppendLine(" INNER JOIN D_ENTRY_STATUS DES");
            sql.AppendLine("    ON DES.ENTRY_UNIT_ID=DE.ENTRY_UNIT_ID");
            sql.AppendLine("   AND DES.RECORD_KBN=DE.RECORD_KBN");
            sql.AppendLine("  LEFT JOIN M_USER MU");
            sql.AppendLine("    ON MU.USER_ID=DES.ENTRY_USER_ID");
            sql.AppendLine(" WHERE DE.ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
            sql.AppendLine("   AND DE.IMAGE_SEQ=?"); param.Add(IMAGE_SEQ);
            sql.AppendLine("   AND DE.ITEM_ID=?"); param.Add(ITEM_ID);
            sql.AppendLine("   AND DE.RECORD_KBN!=?"); param.Add(Consts.RecordKbn.ADMIN);
            sql.AppendLine(" ORDER BY DE.RECORD_KBN");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// D_ENTRY更新
        /// </summary>
        /// <param name="ENTRY_UNIT_ID"></param>
        /// <param name="IMAGE_SEQ"></param>
        /// <param name="RECORD_KBN"></param>
        /// <param name="ENTRY_VALUES"></param>
        /// <returns></returns>
        public int UPDATE_D_ENTRY(string ENTRY_UNIT_ID, int IMAGE_SEQ, string RECORD_KBN, List<string[]> ENTRY_VALUES)
        {
            var sql = new StringBuilder();
            var param = new List<object>();

            sql.AppendLine("UPDATE D_ENTRY");
            sql.AppendLine("   SET VALUE=?");
            sql.AppendLine("      ,DUMMY_ITEM_FLAG=?");
            sql.AppendLine("      ,UPD_USER_ID=?");
            sql.AppendLine("      ,UPD_DATE=SYSDATE");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?");
            sql.AppendLine("   AND IMAGE_SEQ=?");
            sql.AppendLine("   AND RECORD_KBN=?");
            sql.AppendLine("   AND ITEM_ID=?");

            //IEnumerable<string[]> UpdateTarget = null;
            //if (IsUsingDummyItem && !Consts.RecordKbn.ADMIN.Equals(RECORD_KBN))
            //{
            //    UpdateTarget = ENTRY_VALUES.Where(_ => !_[1].Equals(_[2]) || !_[4].Equals(_[3]));
            //}
            //else
            //{
            //    //UpdateTarget = ENTRY_VALUES.Where(_ => !_[1].Equals(_[2]) && Consts.Flag.OFF.Equals(_[3]));
            //    UpdateTarget = ENTRY_VALUES.Where(_ => !_[1].Equals(_[2]));
            //}

            var UpdateCount = 0;
            foreach (var VALUE_AFTER in ENTRY_VALUES.AsEnumerable().Where(_ => !_[1].Equals(_[2]) || !_[4].Equals(_[3])))
            {
                param.Clear();
                param.Add(VALUE_AFTER[1]);
                param.Add(VALUE_AFTER[3]);
                param.Add(Program.LoginUser.USER_ID);
                param.Add(ENTRY_UNIT_ID);
                param.Add(IMAGE_SEQ);
                param.Add(RECORD_KBN);
                param.Add(VALUE_AFTER[0]);
                UpdateCount += ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
            }

            //ParallelOptions po = new ParallelOptions();
            //po.MaxDegreeOfParallelism = ProcessorCount;
            //Parallel.ForEach(UpdateTarget, po, VALUE_AFTER =>
            // {
            //     param.Clear();
            //     param.Add(VALUE_AFTER[1]);
            //     param.Add(VALUE_AFTER[3]);
            //     param.Add(Program.LoginUser.USER_ID);
            //     param.Add(ENTRY_UNIT_ID);
            //     param.Add(IMAGE_SEQ);
            //     param.Add(RECORD_KBN);
            //     param.Add(VALUE_AFTER[0]);
            //     if (ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param) != 1)
            //         throw new ApplicationException("D_ENTRY 更新処理で不整合発生");
            // });

            return UpdateCount;
        }

        /// <summary>
        /// D_IMAGE_INFO 更新
        /// </summary>
        /// <param name="ENTRY_UNIT_ID"></param>
        /// <param name="IMAGE_SEQ"></param>
        /// <param name="USER_KBN"></param>
        /// <returns></returns>
        public int UPDATE_D_IMAGE_INFO(string ENTRY_UNIT_ID, int IMAGE_SEQ, string USER_KBN)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_IMAGE_INFO");
            sql.AppendLine(String.Format("   SET RECORD_KBN_{0}_ENTRY_FLAG=?", USER_KBN)); param.Add(Consts.Flag.ON);
            sql.AppendLine("      ,UPD_USER_ID=?"); param.Add(Program.LoginUser.USER_ID);
            sql.AppendLine("      ,UPD_DATE=SYSDATE");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
            sql.AppendLine("   AND IMAGE_SEQ=?"); param.Add(IMAGE_SEQ);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 入力ステータス更新
        /// </summary>
        /// <param name="ENTRY_UNIT_ID"></param>
        /// <param name="RECORD_KBN"></param>
        /// <param name="ENTRY_STATUS"></param>
        /// <returns></returns>
        public int UPDATE_D_ENTRY_STATUS(string ENTRY_UNIT_ID, string RECORD_KBN, string ENTRY_STATUS)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_ENTRY_STATUS");
            sql.AppendLine("   SET ENTRY_STATUS=?"); param.Add(ENTRY_STATUS);
            sql.AppendLine("      ,ENTRY_END_TIME=DECODE(ENTRY_END_TIME,NULL,SYSDATE,ENTRY_END_TIME)");
            sql.AppendLine("      ,UPD_USER_ID=?"); param.Add(Program.LoginUser.USER_ID);
            sql.AppendLine("      ,UPD_DATE=SYSDATE");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
            sql.AppendLine("   AND RECORD_KBN=?"); param.Add(RECORD_KBN);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ENTRY_UNIT_ID"></param>
        /// <param name="STATUS"></param>
        /// <returns></returns>
        public int UPDATE_D_ENTRY_UNIT(string ENTRY_UNIT_ID, string STATUS)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_ENTRY_UNIT");
            sql.AppendLine("   SET STATUS=?"); param.Add(STATUS);
            if (Consts.EntryUnitStatus.COMPARE_END.Equals(STATUS))
            {
                // コンペア修正終了時時刻
                sql.AppendLine("      ,UPD_ENTRY_END_TIME=DECODE(UPD_ENTRY_END_TIME,NULL,SYSDATE,UPD_ENTRY_END_TIME)");
            }
            sql.AppendLine("      ,UPD_USER_ID=?"); param.Add(Program.LoginUser.USER_ID);
            sql.AppendLine("      ,UPD_DATE=SYSDATE");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 郵便番号から住所情報を取得する
        /// </summary>
        /// <param name="zip_code"></param>
        /// <returns></returns>
        public DataTable SelectZipInfo(string zipCode,int seq =0)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT *");
            sql.AppendLine("  FROM M_ZIP_CODE");
            sql.AppendLine(" WHERE ZIP_CODE=?"); param.Add(zipCode);
            if (seq != 0)
            {
                sql.AppendLine(" AND SEQ=?"); param.Add(seq);
            }
            sql.AppendLine(" ORDER BY SEQ");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// M_DOC 取得
        /// </summary>
        /// <param name="DOC_ID"></param>
        /// <returns></returns>
        public DataTable SELECT_M_DOC(string DOC_ID)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT *");
            sql.AppendLine("  FROM M_DOC");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("   AND DOC_ID=?"); param.Add(DOC_ID);
            //sql.AppendLine("ORDER BY DOC_ID");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        public DataTable SELECT_D_ENTRY_UNIT(string ENTRY_UNIT_ID, bool LOCK_FLAG = false)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT STATUS");
            sql.AppendLine("  FROM D_ENTRY_UNIT");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
            if (LOCK_FLAG)
                sql.AppendLine(" FOR UPDATE");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ENTRY_UNIT_ID"></param>
        /// <returns></returns>
        public int UPDATE_D_ENTRY_UNIT_VERIFY_ING_FLAG(string ENTRY_UNIT_ID)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_ENTRY_UNIT");
            sql.AppendLine("   SET VERIFY_ING_FLAG=?"); param.Add(Consts.Flag.OFF);
            sql.AppendLine("      ,UPD_USER_ID=?"); param.Add(Program.LoginUser.USER_ID);
            sql.AppendLine("      ,UPD_DATE=SYSDATE");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        public DataTable SELECT_READING_PARTS_FILE_NAME(string sOcrImageFileName)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT RP2.COLUNM_NO");
            sql.AppendLine("      ,RP2.X");
            sql.AppendLine("      ,RP2.Y");
            sql.AppendLine("      ,RP2.WIDTH");
            sql.AppendLine("      ,RP2.HEIGHT");
            sql.AppendLine("  FROM READING_PAGES RP1");
            sql.AppendLine(" INNER JOIN READING_PARTS RP2");
            sql.AppendLine("    ON RP1.ID=RP2.READING_PAGES_ID");
            sql.AppendLine(" WHERE RP1.FILE_NAME=?"); param.Add(sOcrImageFileName);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        public DataTable SELECT_READING_PARTS_TEMPLATE(string sDocId)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT RP2.COLUNM_NO");
            sql.AppendLine("      ,RP2.X");
            sql.AppendLine("      ,RP2.Y");
            sql.AppendLine("      ,RP2.WIDTH");
            sql.AppendLine("      ,RP2.HEIGHT ");
            sql.AppendLine("  FROM READING_PAGES RP1 ");
            sql.AppendLine(" INNER JOIN READING_PARTS RP2 ");
            sql.AppendLine("    ON RP1.ID=RP2.READING_PAGES_ID ");
            sql.AppendLine(" WHERE RP1.TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sql.AppendLine("   AND RP1.HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("   AND RP1.DOC_ID=?"); param.Add(sDocId);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        public bool SELECT_M_ITEM_CHECK(string sDocId, string sItemName, string sValue)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT 1");
            sql.AppendLine("  FROM M_ITEM_CHECK");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("   AND DOC_ID=?"); param.Add(sDocId);
            sql.AppendLine("   AND ITEM_NAME=?"); param.Add(sItemName);
            sql.AppendLine("   AND VALUE=?"); param.Add(sValue);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param).Rows.Count != 0;//) ? true : false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ENTRY_UNIT_ID"></param>
        /// <param name="IMAGE_SEQ"></param>
        /// <returns></returns>
        public int UPDATE_D_IMAGE_INFO_OCR_NG_STATUS(string ENTRY_UNIT_ID, int IMAGE_SEQ)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_IMAGE_INFO");
            sql.AppendLine("   SET OCR_NG_STATUS=?"); param.Add("2");
            sql.AppendLine("      ,UPD_USER_ID=?"); param.Add(Program.LoginUser.USER_ID);
            sql.AppendLine("      ,UPD_DATE=SYSDATE");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
            sql.AppendLine("   AND IMAGE_SEQ=?"); param.Add(IMAGE_SEQ);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 入力情報テーブル取得
        /// </summary>
        /// <param name="tn"></param>
        /// <returns></returns>
        public DataTable SELECT_T_NYURYOKUJYOHO(string OKYAKUSAMA_BANGO)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT tn.*");
            sql.AppendLine("  FROM T_NYURYOKUJYOHO tn");
            sql.AppendLine(" INNER JOIN T_CUSTOMER tc");
            sql.AppendLine("    ON tc.MOUSHIKOMISYO_WORK_NO=tn.MOUSHIKOMISYO_WORK_NO");
            sql.AppendLine("   AND tc.JUNBAN=tn.JUNBAN");
            sql.AppendLine(" INNER JOIN PROGRESS_TBL pt");
            sql.AppendLine("    ON pt.WORK_NO=tc.CUSTOMER_WORK_NO");
            sql.AppendLine(" WHERE tn.OKYAKUSAMA_BANGO=?"); param.Add(OKYAKUSAMA_BANGO);
            sql.AppendLine("   AND pt.PROG_KBN=?"); param.Add("EM3");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 受領データ表示
        /// </summary>
        /// <param name="DOC_ID"></param>
        /// <param name="ITEM_NAME"></param>
        /// <param name="KEY_VALUE"></param>
        /// <returns></returns>
        public DataTable SELECT_T_ENTRY_RECEIPT(string DOC_ID, string ITEM_NAME, string KEY_VALUE)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT *");
            sql.AppendLine("  FROM T_ENTRY_RECEIPT");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("   AND DOC_ID=?"); param.Add(DOC_ID);
            sql.AppendLine("   AND ITEM_NAME=?"); param.Add(ITEM_NAME);
            sql.AppendLine("   AND KEY_VALUE=?"); param.Add(KEY_VALUE);
            sql.AppendLine("   AND INVALID_FLAG!=?"); param.Add(Consts.Flag.ON);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        public DataTable SELECT_M_FINANCIAL_INSTITUTION(string FINANCIAL_INSTITUTION_NAME, string BRANCH_NAME = null)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT *");
            sql.AppendLine("  FROM M_FINANCIAL_INSTITUTION");
            sql.AppendLine(" WHERE FINANCIAL_INSTITUTION_NAME=?"); param.Add(FINANCIAL_INSTITUTION_NAME);
            if (BRANCH_NAME != null)
            {
                sql.AppendLine("   AND BRANCH_NAME=?"); param.Add(BRANCH_NAME);
            }
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        public DataTable SELECT_M_FINANCIAL_INSTITUTION_2(string FINANCIAL_INSTITUTION_NAME, string BRANCH_CD)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT *");
            sql.AppendLine("  FROM M_FINANCIAL_INSTITUTION");
            sql.AppendLine(" WHERE FINANCIAL_INSTITUTION_NAME=?"); param.Add(FINANCIAL_INSTITUTION_NAME);
            sql.AppendLine("   AND BRANCH_CD=?"); param.Add(BRANCH_CD);
            sql.AppendLine(" ORDER BY ORDER_CD");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        public DataTable SELECT_M_CODE_DEFINE(string Section ,string Key)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT *");
            sql.AppendLine("  FROM M_CODE_DEFINE");
            sql.AppendLine(" WHERE SECTION=?"); param.Add(Section);
            sql.AppendLine("   AND KEY=?"); param.Add(Key);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        //public DataTable SELECT_WORK_ENTRY_WAIT_TBL(string ImageFilePath)
        //{
        //    var sql = new StringBuilder();
        //    var param = new List<object>();
        //    sql.AppendLine("SELECT BASE_IMAGE_FILE_PATH");
        //    sql.AppendLine("  FROM WORK_ENTRY_WAIT_TBL");
        //    sql.AppendLine(" WHERE IMAGE_FILE_PATH=?"); param.Add(ImageFilePath);
        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);

        //}
        ///// <summary>
        ///// 画像情報をＮページ分だけまとめて取得します。
        ///// </summary>
        ///// <param name="sDocId">帳票ID</param>
        ///// <param name="sImageCaptureDate">画像取込日</param>
        ///// <param name="sImageCaptureNum">画像取込回数</param>
        ///// <param name="sEntryUnit">エントリ単位</param>
        //public DataTable SelectEntryImageInfo(string sDocId, string sImageCaptureDate, string sImageCaptureNum, string sEntryUnit)
        //{
        //    var sql = new StringBuilder();
        //    var param = new List<object>();
        //    sql.AppendLine("SELECT IMAGE_SEQ,IMAGE_PATH,OCR_IMAGE_FILE_NAME,RECORD_KBN_0_ENTRY_FLAG,RECORD_KBN_1_ENTRY_FLAG,RECORD_KBN_2_ENTRY_FLAG");
        //    sql.AppendLine("  FROM D_IMAGE_INFO");
        //    sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
        //    sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
        //    sql.AppendLine("   AND IMAGE_CAPTURE_DATE=?"); param.Add(sImageCaptureDate);
        //    sql.AppendLine("   AND IMAGE_CAPTURE_NUM=?"); param.Add(sImageCaptureNum);
        //    sql.AppendLine("   AND DOC_ID=?"); param.Add(sDocId);
        //    sql.AppendLine("   AND ENTRY_UNIT=?"); param.Add(sEntryUnit);
        //    sql.AppendLine(" ORDER BY IMAGE_SEQ");
        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        //}
    }
}
