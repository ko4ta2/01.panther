using BPOEntry.Tables;
using Common;
using ODPCtrl;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BPOEntry.ReEntrySelect
{
    public class DaoEntryUnitList : DaoBase
    {
        /// <summary>
        /// コンペア修正、検証対象を取得
        /// </summary>
        /// <param name="sVerifyFlag"></param>
        /// <returns></returns>
        public DataTable SELECT_D_ENTRY_UNIT(string sVerifyFlag,string sOutputEnd,string sImageCaptureDate,string sGyoumKbn)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT DEU.ENTRY_UNIT_ID");
            //sql.AppendLine("      ,DEU.IMAGE_CAPTURE_DATE");
            //sql.AppendLine("      ,DEU.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("      ,DEU.ENTRY_UNIT");
            //sql.AppendLine("      ,DEU.DOC_ID");
            sql.AppendLine("      ,DEU.VERIFY_ING_FLAG");
            sql.AppendLine("      ,MD.DOC_NAME");
            sql.AppendLine("      ,MU.USER_NAME VERIFY_ENTRY_USER_NAME");
            sql.AppendLine("      ,? RECORD_KBN"); param.Add(Consts.RecordKbn.ADMIN);
            sql.AppendLine("      ,DEU.UPD_ENTRY_USER_ID");
            sql.AppendLine("  FROM D_ENTRY_UNIT DEU");
            sql.AppendLine(" INNER JOIN M_DOC MD");
            sql.AppendLine("    ON MD.TKSK_CD=DEU.TKSK_CD");
            sql.AppendLine("   AND MD.HNM_CD=DEU.HNM_CD");
            sql.AppendLine("   AND MD.DOC_ID=DEU.DOC_ID");
            sql.AppendLine("  LEFT OUTER JOIN M_USER MU");
            sql.AppendLine("          ON MU.USER_ID=DEU.VERIFY_ENTRY_USER_ID");
            sql.AppendLine(" WHERE DEU.TKSK_CD=?"); param.Add(Common.Config.TokuisakiCode);
            sql.AppendLine("   AND DEU.HNM_CD=?"); param.Add(Common.Config.HinmeiCode);
            if (Consts.Flag.OFF.Equals(sVerifyFlag))
            {
                sql.AppendLine("  AND DEU.STATUS=?"); param.Add(Consts.EntryUnitStatus.COMPARE_EDT);
                sql.AppendLine("  AND (DEU.UPD_ENTRY_USER_ID=? OR DEU.UPD_ENTRY_USER_ID IS NULL)"); param.Add(Program.LoginUser.USER_ID);
                sql.AppendLine("  AND NOT EXISTS (SELECT 1 FROM D_ENTRY_STATUS DES");
                sql.AppendLine("                   WHERE DES.ENTRY_UNIT_ID=DEU.ENTRY_UNIT_ID");
                //sql.AppendLine("                     AND DES.HNM_CD=DEU.HNM_CD");
                //sql.AppendLine("                     AND DES.IMAGE_CAPTURE_DATE=DEU.IMAGE_CAPTURE_DATE");
                //sql.AppendLine("                     AND DES.IMAGE_CAPTURE_NUM=DEU.IMAGE_CAPTURE_NUM");
                //sql.AppendLine("                     AND DES.DOC_ID=DEU.DOC_ID");
                //sql.AppendLine("                     AND DES.ENTRY_UNIT=DEU.ENTRY_UNIT");
                sql.AppendLine("                     AND DES.ENTRY_USER_ID=?)"); param.Add(Program.LoginUser.USER_ID);
            }
            else
            {
                if (Consts.Flag.ON.Equals(sOutputEnd))
                {
                    sql.AppendLine("  AND DEU.STATUS=?"); param.Add(Consts.EntryUnitStatus.EXPORT_END);
                    if (sImageCaptureDate.Length != 0)
                    {
                        sql.AppendLine("  AND DEU.IMAGE_CAPTURE_DATE=?"); param.Add(sImageCaptureDate);
                    }
                }
                else
                {
                    sql.AppendLine("  AND DEU.STATUS=?"); param.Add(Consts.EntryUnitStatus.COMPARE_END);
                    if (sImageCaptureDate.Length != 0)
                    {
                        sql.AppendLine("  AND DEU.IMAGE_CAPTURE_DATE=?"); param.Add(sImageCaptureDate);
                    }
                }
                //sql.AppendLine("  AND (DEU.UPD_ENTRY_USER_ID!=? OR DEU.UPD_ENTRY_USER_ID IS NULL)"); param.Add(Program.LoginUser.USER_ID);
                //sql.AppendLine("  AND (DEU.VERIFY_ENTRY_USER_ID=? OR DEU.VERIFY_ENTRY_USER_ID IS NULL)"); param.Add(Program.LoginUser.USER_ID);
                //sql.AppendLine("  AND NOT EXISTS (SELECT 1 FROM D_ENTRY_STATUS DES");
                //sql.AppendLine("                   WHERE DES.TKSK_CD=DEU.TKSK_CD");
                //sql.AppendLine("                     AND DES.HNM_CD=DEU.HNM_CD");
                //sql.AppendLine("                     AND DES.IMAGE_CAPTURE_DATE=DEU.IMAGE_CAPTURE_DATE");
                //sql.AppendLine("                     AND DES.IMAGE_CAPTURE_NUM=DEU.IMAGE_CAPTURE_NUM");
                //sql.AppendLine("                     AND DES.DOC_ID=DEU.DOC_ID");
                //sql.AppendLine("                     AND DES.ENTRY_UNIT=DEU.ENTRY_UNIT");
                //sql.AppendLine("                     AND DES.ENTRY_USER_ID=?)"); param.Add(Program.LoginUser.USER_ID);
            }

            if (!"*".Equals(sGyoumKbn))
            {
                sql.AppendLine("   AND MD.GYOUM_KBN=?"); param.Add(sGyoumKbn);
            }

            if (Consts.Flag.OFF.Equals(sVerifyFlag))
            {
                sql.AppendLine("ORDER BY DEU.UPD_ENTRY_USER_ID");
                sql.AppendLine("        ,IMAGE_CAPTURE_DATE");
                sql.AppendLine("        ,DEU.IMAGE_CAPTURE_NUM");
                sql.AppendLine("        ,MD.SORT_ORDER");
                sql.AppendLine("        ,DEU.DOC_ID");
                sql.AppendLine("        ,DEU.ENTRY_UNIT");
            }
            else
            {
                sql.AppendLine("ORDER BY DEU.IMAGE_CAPTURE_DATE");
                sql.AppendLine("        ,DEU.IMAGE_CAPTURE_NUM");
                sql.AppendLine("        ,MD.SORT_ORDER");
                sql.AppendLine("        ,DEU.DOC_ID");
                sql.AppendLine("        ,DEU.ENTRY_UNIT");
            }
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        //public DataTable SelectOcrNgUnit(string sGyoumKbn)
        //{
        //    var sql = new StringBuilder();
        //    var param = new List<object>();
        //    sql.AppendLine("SELECT DEU.IMAGE_CAPTURE_DATE");
        //    sql.AppendLine("      ,DEU.IMAGE_CAPTURE_NUM");
        //    sql.AppendLine("      ,DEU.ENTRY_UNIT");
        //    sql.AppendLine("      ,DEU.DOC_ID");
        //    sql.AppendLine("      ,DEU.VERIFY_ING_FLAG");
        //    sql.AppendLine("      ,MD.DOC_NAME");
        //    sql.AppendLine("      ,MU.USER_NAME VERIFY_ENTRY_USER_NAME");
        //    sql.AppendLine("      ,? RECORD_KBN"); param.Add(Consts.RecordKbn.ADMIN);
        //    sql.AppendLine("      ,DEU.UPD_ENTRY_USER_ID");
        //    sql.AppendLine("  FROM D_ENTRY_UNIT DEU");
        //    sql.AppendLine(" INNER JOIN M_DOC MD");
        //    sql.AppendLine("    ON MD.TKSK_CD=DEU.TKSK_CD");
        //    sql.AppendLine("   AND MD.HNM_CD=DEU.HNM_CD");
        //    sql.AppendLine("   AND MD.DOC_ID=DEU.DOC_ID");
        //    sql.AppendLine("  LEFT OUTER JOIN M_USER MU");
        //    sql.AppendLine("          ON MU.USER_ID=DEU.VERIFY_ENTRY_USER_ID");
        //    sql.AppendLine(" WHERE DEU.TKSK_CD=?"); param.Add(Common.Config.TokuisakiCode);
        //    sql.AppendLine("   AND DEU.HNM_CD=?"); param.Add(Common.Config.HinmeiCode);
        //    //if (Consts.Flag.OFF.Equals(sVerifyFlag))
        //    //{
        //        sql.AppendLine("  AND DEU.STATUS=?"); param.Add(Consts.EntryUnitStatus.COMPARE_END);
        //        sql.AppendLine("  AND (DEU.UPD_ENTRY_USER_ID=? OR DEU.UPD_ENTRY_USER_ID IS NULL)"); param.Add(Program.LoginUser.USER_ID);
        //        //sql.AppendLine("  AND NOT EXISTS (SELECT 1 FROM D_ENTRY_STATUS DES");
        //        //sql.AppendLine("                   WHERE DES.TKSK_CD=DEU.TKSK_CD");
        //        //sql.AppendLine("                     AND DES.HNM_CD=DEU.HNM_CD");
        //        //sql.AppendLine("                     AND DES.IMAGE_CAPTURE_DATE=DEU.IMAGE_CAPTURE_DATE");
        //        //sql.AppendLine("                     AND DES.IMAGE_CAPTURE_NUM=DEU.IMAGE_CAPTURE_NUM");
        //        //sql.AppendLine("                     AND DES.DOC_ID=DEU.DOC_ID");
        //        //sql.AppendLine("                     AND DES.ENTRY_UNIT=DEU.ENTRY_UNIT");
        //        //sql.AppendLine("                     AND DES.ENTRY_USER_ID=?)"); param.Add(Program.LoginUser.USER_ID);

        //    sql.AppendLine("  AND EXISTS (SELECT 1 FROM D_IMAGE_INFO DII");
        //    //sql.AppendLine("               WHERE DII.TKSK_CD=DEU.TKSK_CD");
        //    //sql.AppendLine("                 AND DII.HNM_CD=DEU.HNM_CD");
        //    //sql.AppendLine("                 AND DII.IMAGE_CAPTURE_DATE=DEU.IMAGE_CAPTURE_DATE");
        //    //sql.AppendLine("                 AND DII.IMAGE_CAPTURE_NUM=DEU.IMAGE_CAPTURE_NUM");
        //    //sql.AppendLine("                 AND DII.DOC_ID=DEU.DOC_ID");
        //    //sql.AppendLine("                 AND DII.ENTRY_UNIT=DEU.ENTRY_UNIT");
        //    sql.AppendLine("               WHERE DII.ENTRY_UNIT_ID=DEU.ENTRY_UNIT_ID");
        //    sql.AppendLine("                 AND DII.OCR_NG_STATUS=?)");param.Add(Consts.Flag.ON);


        //    //}
        //    //else
        //    //{
        //    //    if (Consts.Flag.ON.Equals(sOutputEnd))
        //    //    {
        //    //        sql.AppendLine("  AND DEU.STATUS=?"); param.Add(Consts.EntryUnitStatus.EXPORT_END);
        //    //        if (sImageCaptureDate.Length != 0)
        //    //        {
        //    //            sql.AppendLine("  AND DEU.IMAGE_CAPTURE_DATE=?"); param.Add(sImageCaptureDate);
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        sql.AppendLine("  AND DEU.STATUS=?"); param.Add(Consts.EntryUnitStatus.COMPARE_END);
        //    //        if (sImageCaptureDate.Length != 0)
        //    //        {
        //    //            sql.AppendLine("  AND DEU.IMAGE_CAPTURE_DATE=?"); param.Add(sImageCaptureDate);
        //    //        }
        //    //    }
        //    //sql.AppendLine("  AND (DEU.UPD_ENTRY_USER_ID!=? OR DEU.UPD_ENTRY_USER_ID IS NULL)"); param.Add(Program.LoginUser.USER_ID);
        //    //sql.AppendLine("  AND (DEU.VERIFY_ENTRY_USER_ID=? OR DEU.VERIFY_ENTRY_USER_ID IS NULL)"); param.Add(Program.LoginUser.USER_ID);
        //    //sql.AppendLine("  AND NOT EXISTS (SELECT 1 FROM D_ENTRY_STATUS DES");
        //    //sql.AppendLine("                   WHERE DES.TKSK_CD=DEU.TKSK_CD");
        //    //sql.AppendLine("                     AND DES.HNM_CD=DEU.HNM_CD");
        //    //sql.AppendLine("                     AND DES.IMAGE_CAPTURE_DATE=DEU.IMAGE_CAPTURE_DATE");
        //    //sql.AppendLine("                     AND DES.IMAGE_CAPTURE_NUM=DEU.IMAGE_CAPTURE_NUM");
        //    //sql.AppendLine("                     AND DES.DOC_ID=DEU.DOC_ID");
        //    //sql.AppendLine("                     AND DES.ENTRY_UNIT=DEU.ENTRY_UNIT");
        //    //sql.AppendLine("                     AND DES.ENTRY_USER_ID=?)"); param.Add(Program.LoginUser.USER_ID);
        //    //}

        //    if (!"*".Equals(sGyoumKbn))
        //    {
        //        sql.AppendLine("   AND MD.GYOUM_KBN=?"); param.Add(sGyoumKbn);
        //    }

        //        sql.AppendLine("ORDER BY DEU.IMAGE_CAPTURE_DATE");
        //        sql.AppendLine("        ,DEU.IMAGE_CAPTURE_NUM");
        //        sql.AppendLine("        ,MD.SORT_ORDER");
        //        sql.AppendLine("        ,DEU.DOC_ID");
        //        sql.AppendLine("        ,DEU.ENTRY_UNIT");
        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        //}

        /// <summary>
        /// エントリ対象一覧
        /// </summary>
        /// <returns></returns>
        public DataTable SELECT_D_ENTRY_STATUS(string sGyoumKbn)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT /*DES.DOC_ID");
            sql.AppendLine("      ,DES.IMAGE_CAPTURE_DATE");
            sql.AppendLine("      ,DES.IMAGE_CAPTURE_NUM");
            sql.AppendLine("      ,*/DES.ENTRY_UNIT_ID");
            sql.AppendLine("      ,DES.ENTRY_USER_ID");
            sql.AppendLine("      ,MU.USER_NAME ENTRY_USER_NAME");
            sql.AppendLine("      ,DES.ENTRY_STATUS STATUS");
            sql.AppendLine("      ,DES.RECORD_KBN");
            sql.AppendLine("      ,MD.DOC_NAME || '（' || DES.RECORD_KBN || '人目）' DOC_NAME");
            sql.AppendLine("      ,MD.SINGLE_ENTRY_FLAG");
            sql.AppendLine("      ,MD.OCR_IMPORT_1ST_FLAG");
            sql.AppendLine("      ,MD.OCR_IMPORT_2ND_FLAG");
            sql.AppendLine("      ,DES.OCR_IMPORT_USER_ID");
            sql.AppendLine("  FROM D_ENTRY_STATUS DES");


            sql.AppendLine(" INNER JOIN D_ENTRY_UNIT DEU");
            //sql.AppendLine("    ON DEU.TKSK_CD=DES.TKSK_CD");
            //sql.AppendLine("   AND DEU.HNM_CD=DES.HNM_CD");
            //sql.AppendLine("   AND DEU.IMAGE_CAPTURE_DATE=DES.IMAGE_CAPTURE_DATE");
            //sql.AppendLine("   AND DEU.IMAGE_CAPTURE_NUM=DES.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("   AND DEU.DOC_ID=DES.DOC_ID");
            sql.AppendLine("    ON DEU.ENTRY_UNIT_ID=DES.ENTRY_UNIT_ID");
            //sql.AppendLine("   AND DEU.TKSK_CD=MD.TKSK_CD");
            //sql.AppendLine("   AND DEU.HNM_CD=MD.HNM_CD");
            //sql.AppendLine("   AND DEU.DOC_ID=MD.DOC_ID");


            sql.AppendLine(" INNER JOIN M_DOC MD");
            sql.AppendLine("    ON DEU.TKSK_CD=MD.TKSK_CD");
            sql.AppendLine("   AND DEU.HNM_CD=MD.HNM_CD");
            sql.AppendLine("   AND DEU.DOC_ID=MD.DOC_ID");

            //if (Utils.IsRLI())
            //{
                sql.AppendLine(" INNER JOIN D_ENTRY_UNIT DEU");
                //sql.AppendLine("    ON DEU.TKSK_CD=DES.TKSK_CD");
                //sql.AppendLine("   AND DEU.HNM_CD=DES.HNM_CD");
                //sql.AppendLine("   AND DEU.IMAGE_CAPTURE_DATE=DES.IMAGE_CAPTURE_DATE");
                //sql.AppendLine("   AND DEU.IMAGE_CAPTURE_NUM=DES.IMAGE_CAPTURE_NUM");
                //sql.AppendLine("   AND DEU.DOC_ID=DES.DOC_ID");
                sql.AppendLine("    ON DEU.ENTRY_UNIT_ID=DES.ENTRY_UNIT_ID");
                sql.AppendLine("   AND DEU.TKSK_CD=MD.TKSK_CD");
                sql.AppendLine("   AND DEU.HNM_CD=MD.HNM_CD");
                sql.AppendLine("   AND DEU.DOC_ID=MD.DOC_ID");
            //}

            sql.AppendLine(" LEFT OUTER JOIN M_USER MU");
            sql.AppendLine("   ON MU.USER_ID=DES.ENTRY_USER_ID");
            sql.AppendLine(" WHERE DEU.TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sql.AppendLine("   AND DEU.HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("   AND ((DES.ENTRY_STATUS=?"); param.Add(Consts.EntryStatus.ENTRY_NOT);
            sql.AppendLine("   AND DES.ENTRY_USER_ID IS NULL");
            sql.AppendLine("   AND NOT EXISTS(SELECT 1 FROM D_ENTRY_STATUS DES2");
            //sql.AppendLine("                   WHERE DES2.TKSK_CD=DES.TKSK_CD");
            //sql.AppendLine("                     AND DES2.HNM_CD=DES.HNM_CD");
            //sql.AppendLine("                     AND DES2.IMAGE_CAPTURE_DATE=DES.IMAGE_CAPTURE_DATE");
            //sql.AppendLine("                     AND DES2.IMAGE_CAPTURE_NUM=DES.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("                     AND DES2.DOC_ID=DES.DOC_ID");
            sql.AppendLine("                   WHERE DES2.ENTRY_UNIT_ID=DES.ENTRY_UNIT_ID");
            sql.AppendLine("                     AND DES2.ENTRY_USER_ID=?"); param.Add(Program.LoginUser.USER_ID);
            sql.AppendLine("                     AND DES2.ENTRY_STATUS!=?"); param.Add(Consts.EntryStatus.ENTRY_NOT);
            sql.AppendLine("                 )");
            sql.AppendLine("      ) OR (DES.ENTRY_STATUS=? AND DES.ENTRY_USER_ID=?))"); param.Add(Consts.EntryStatus.ENTRY_ING); param.Add(Program.LoginUser.USER_ID);
            sql.AppendLine("   AND ((MD.SINGLE_ENTRY_FLAG=? AND DES.RECORD_KBN=?)"); param.Add(Consts.Flag.ON); param.Add(Consts.RecordKbn.Entry_1st);
            sql.AppendLine("     OR (MD.SINGLE_ENTRY_FLAG=?))"); param.Add(Consts.Flag.OFF);

            ////if (Utils.IsRLI())
            ////{
            //    sql.AppendLine("   AND ((MD.OCR_COOPERATION_FLAG=? AND DEU.OCR_IMPORT_USER_ID IS NOT NULL) OR (MD.OCR_COOPERATION_FLAG=?))");
            //    param.Add(Consts.Flag.ON); param.Add(Consts.Flag.OFF);
            ////}
            sql.AppendLine("   AND ((DES.RECORD_KBN='1' AND MD.OCR_IMPORT_1ST_FLAG='1' AND DES.OCR_IMPORT_USER_ID IS NOT NULL)");
            sql.AppendLine("     OR (DES.RECORD_KBN='1' AND MD.OCR_IMPORT_1ST_FLAG='0')");
            sql.AppendLine("     OR (DES.RECORD_KBN='2' AND MD.OCR_IMPORT_2ND_FLAG='1' AND DES.OCR_IMPORT_USER_ID IS NOT NULL)");
            sql.AppendLine("     OR (DES.RECORD_KBN='2' AND MD.OCR_IMPORT_2ND_FLAG='0'))");

            if (!"*".Equals(sGyoumKbn))
            {
                sql.AppendLine("   AND MD.GYOUM_KBN=?");param.Add(sGyoumKbn);
            }

            sql.AppendLine("ORDER BY DES.ENTRY_STATUS DESC");
            sql.AppendLine("        ,DES.ENTRY_UNIT_ID");
            //sql.AppendLine("        ,DES.IMAGE_CAPTURE_NUM");
            //sql.AppendLine("        ,MD.SORT_ORDER");
            //sql.AppendLine("        ,DES.DOC_ID");
            //sql.AppendLine("        ,DES.ENTRY_UNIT");
            sql.AppendLine("        ,DES.RECORD_KBN");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        /// <summary>
        /// エントリ対象チェック
        /// </summary>
        /// <param name="sImageCaptureDate"></param>
        /// <param name="sImageCaptureNum"></param>
        /// <param name="sDocId"></param>
        /// <param name="sEntryUnit"></param>
        /// <param name="sRecordKbn"></param>
        /// <returns></returns>
        public DataTable SELECT_D_ENTRY_STATUS_FOR_UPDATE(string ENTRY_UNIT_ID, string sRecordKbn)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT DES.ENTRY_USER_ID");
            sql.AppendLine("      ,MU.USER_NAME ENTRY_USER_NAME");
            sql.AppendLine("      ,DES.ENTRY_STATUS STATUS");
            sql.AppendLine("  FROM D_ENTRY_STATUS DES");
            sql.AppendLine("  LEFT OUTER JOIN M_USER MU");
            sql.AppendLine("          ON MU.USER_ID=DES.ENTRY_USER_ID");
            sql.AppendLine(" WHERE DES.ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
            //sql.AppendLine("   AND DES.HNM_CD=?"); param.Add(Config.HinmeiCode);
            //sql.AppendLine("   AND DES.IMAGE_CAPTURE_DATE=?"); param.Add(sImageCaptureDate);
            //sql.AppendLine("   AND DES.IMAGE_CAPTURE_NUM=?"); param.Add(sImageCaptureNum);
            //sql.AppendLine("   AND DES.DOC_ID=?"); param.Add(sDocId);
            //sql.AppendLine("   AND DES.ENTRY_UNIT=?"); param.Add(sEntryUnit);
            sql.AppendLine("   AND DES.RECORD_KBN=?"); param.Add(sRecordKbn);
            sql.AppendLine("   AND DES.ENTRY_STATUS IN(?,?)"); param.Add(Consts.EntryStatus.ENTRY_NOT); param.Add(Consts.EntryStatus.ENTRY_ING);
            sql.AppendLine("   AND (DES.ENTRY_USER_ID IS NULL");
            sql.AppendLine("    OR  DES.ENTRY_USER_ID=?)"); param.Add(Program.LoginUser.USER_ID);
            sql.AppendLine("FOR UPDATE OF DES.ENTRY_UNIT_ID NOWAIT");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        public DataTable SELECT_D_ENTRY_UNIT_FOR_UPDATE(string ENTRY_UNIT_ID)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT DEU.ENTRY_UNIT_ID");
            sql.AppendLine("      ,DEU.STATUS");
            sql.AppendLine("      ,DEU.UPD_ENTRY_USER_ID");
            sql.AppendLine("      ,MUU.USER_NAME UPD_ENTRY_USER_NAME");
            sql.AppendLine("      ,DEU.VERIFY_ENTRY_USER_ID");
            sql.AppendLine("      ,MUV.USER_NAME VERIFY_ENTRY_USER_NAME");
            sql.AppendLine("      ,DEU.VERIFY_ING_FLAG");
            sql.AppendLine("  FROM D_ENTRY_UNIT DEU");
            sql.AppendLine(" LEFT OUTER JOIN M_USER MUU");
            sql.AppendLine("   ON MUU.USER_ID=DEU.UPD_ENTRY_USER_ID");
            sql.AppendLine(" LEFT OUTER JOIN M_USER MUV");
            sql.AppendLine("   ON MUV.USER_ID=DEU.VERIFY_ENTRY_USER_ID");
            sql.AppendLine(" WHERE DEU.ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
            //sql.AppendLine("   AND DEU.HNM_CD=?"); param.Add(Common.Config.HinmeiCode);
            //sql.AppendLine("   AND DEU.IMAGE_CAPTURE_DATE=?"); param.Add(sImageCaptureDate);
            //sql.AppendLine("   AND DEU.IMAGE_CAPTURE_NUM=?"); param.Add(sImageCaptureNum);
            //sql.AppendLine("   AND DEU.DOC_ID=?"); param.Add(sDocId);
            //sql.AppendLine("   AND DEU.ENTRY_UNIT=?"); param.Add(sEntryUnit);
            sql.AppendLine("   FOR UPDATE NOWAIT"); 
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }

        public int UPDATE_D_ENTRY_UNIT_VERIFY(string ENTRY_UNIT_ID, string sVerifyFlag)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_ENTRY_UNIT");
            if (Consts.Flag.OFF.Equals(sVerifyFlag))
            {
                sql.AppendLine("   SET UPD_ENTRY_USER_ID=?"); param.Add(Program.LoginUser.USER_ID);
            }
            else
            {
                sql.AppendLine("   SET VERIFY_ENTRY_USER_ID=?"); param.Add(Program.LoginUser.USER_ID);
                sql.AppendLine("      ,VERIFY_ING_FLAG=?"); param.Add(Consts.Flag.ON);
            }
            sql.AppendLine("      ,UPD_ENTRY_START_TIME=DECODE(UPD_ENTRY_START_TIME,NULL,SYSDATE,UPD_ENTRY_START_TIME)");
            sql.AppendLine("      ,UPD_USER_ID=?"); param.Add(Program.LoginUser.USER_ID);
            sql.AppendLine("      ,UPD_DATE=SYSDATE");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
            sql.AppendLine("   AND STATUS!=?"); param.Add(Consts.EntryUnitStatus.EXPORT_END);   // テキスト出力後のエントリ単位の場合、更新しない
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 入力状態データをアサイン済みに更新します。
        /// </summary>
        /// <returns></returns>
        public int UPDATE_D_ENTRY_STATUS(D_ENTRY_STATUS entryStatus)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_ENTRY_STATUS ES");
            sql.AppendLine("   SET ES.ENTRY_USER_ID=?"); param.Add(entryStatus.ENTRY_USER_ID);
            sql.AppendLine("      ,ES.ENTRY_START_TIME=DECODE(ES.ENTRY_START_TIME,NULL,SYSDATE,ES.ENTRY_START_TIME)");
            sql.AppendLine("      ,ES.ENTRY_STATUS=?"); param.Add(Consts.EntryStatus.ENTRY_ING);
            sql.AppendLine("      ,ES.UPD_USER_ID=?"); param.Add(entryStatus.UPD_USER_ID);
            sql.AppendLine("      ,ES.UPD_DATE=SYSDATE");
            sql.AppendLine(" WHERE ES.ENTRY_UNIT_ID=?"); param.Add(entryStatus.ENTRY_UNIT_ID);
            sql.AppendLine("   AND ES.RECORD_KBN=?"); param.Add(entryStatus.RECORD_KBN);
            sql.AppendLine("   AND ES.ENTRY_USER_ID IS NULL");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 入力単位の状態を未入力(0)⇒入力中(1)に更新します。
        /// </summary>
        /// <returns></returns>
        public int UPDATE_D_ENTRY_UNIT(D_ENTRY_UNIT entryUnit)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("UPDATE D_ENTRY_UNIT DEU");
            sql.AppendLine("   SET DEU.STATUS=?"); param.Add(Consts.EntryUnitStatus.ENTRY_EDT);
            //sql.AppendLine("      ,DEU.UPD_ENTRY_START_TIME=DECODE(DEU.UPD_ENTRY_START_TIME,NULL,SYSDATE,DEU.UPD_ENTRY_START_TIME)");
            sql.AppendLine("      ,DEU.UPD_USER_ID=?"); param.Add(entryUnit.UPD_USER_ID);
            sql.AppendLine("      ,DEU.UPD_DATE=SYSDATE");
            sql.AppendLine("WHERE DEU.DOC_ID=?"); param.Add(entryUnit.DOC_ID);
            sql.AppendLine("  AND DEU.IMAGE_CAPTURE_DATE=?"); param.Add(entryUnit.IMAGE_CAPTURE_DATE);
            sql.AppendLine("  AND DEU.IMAGE_CAPTURE_NUM=?"); param.Add(entryUnit.IMAGE_CAPTURE_NUM);
            sql.AppendLine("  AND DEU.ENTRY_UNIT=?"); param.Add(entryUnit.ENTRY_UNIT);
            sql.AppendLine("  AND DEU.STATUS=?"); param.Add(Consts.EntryStatus.ENTRY_NOT);
            sql.AppendLine("  AND DEU.TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sql.AppendLine("  AND DEU.HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("  AND DEU.STATUS=?"); param.Add(Consts.EntryUnitStatus.ENTRY_NOT);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        public DataTable SELECT_M_CODE_DEFINE(string sSection)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT * ");
            sql.AppendLine("  FROM M_CODE_DEFINE");
            sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
            sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
            sql.AppendLine("   AND SECTION=?"); param.Add(sSection);
            sql.AppendLine(" ORDER BY DISPLAY_ORDER,KEY");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        }
    }
}
