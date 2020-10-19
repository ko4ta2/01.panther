using BPOEntry.Tables;
using Common;
using ODPCtrl;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DllCompareEntryUnit
{
    /// <summary>
    /// 入力画面共通DAO
    /// </summary>
    public class DaoDllCompareEntryUnit : DaoBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="STATUS"></param>
        /// <returns></returns>
        //public DataTable SELECT_D_ENTRY_UNIT(string STATUS)
        //{
        //    var sql = new StringBuilder();
        //    var param = new List<object>();
        //    sql.AppendLine("SELECT ENTRY_UNIT");
        //    sql.AppendLine("  FROM D_ENTRY_UNIT");
        //    sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Config.TokuisakiCode);
        //    sql.AppendLine("   AND HNM_CD=?"); param.Add(Config.HinmeiCode);
        //    sql.AppendLine("   AND STATUS=?"); param.Add(STATUS);
        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ENTRY_UNIT_ID"></param>
        /// <returns></returns>
        public int SELECT_D_IMAGE_INFO(string ENTRY_UNIT_ID)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT COUNT(ENTRY_UNIT_ID) CNT");
            sql.AppendLine("  FROM D_IMAGE_INFO");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
            return int.Parse(ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param).Rows[0]["CNT"].ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ENTRY_UNIT_ID"></param>
        /// <param name="IMAGE_SEQ"></param>
        /// <param name="RECORD_KBN"></param>
        /// <returns></returns>
        public DataTable SELECT_D_ENTRY_COMPARE(string ENTRY_UNIT_ID, string RECORD_KBN, int IMAGE_SEQ)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            sql.AppendLine("SELECT ITEM_ID,VALUE,OCR_VALUE,DUMMY_ITEM_FLAG");
            sql.AppendLine("  FROM D_ENTRY");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
            sql.AppendLine("   AND RECORD_KBN=?"); param.Add(RECORD_KBN);
            sql.AppendLine("   AND IMAGE_SEQ=?"); param.Add(IMAGE_SEQ);
            sql.AppendLine(" ORDER BY ITEM_ID");
            return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
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
            sql.AppendLine("      ,UPD_USER_ID=?"); param.Add(GVal.UserId);
            sql.AppendLine("      ,UPD_DATE=SYSDATE");
            sql.AppendLine(" WHERE ENTRY_UNIT_ID=?"); param.Add(ENTRY_UNIT_ID);
            return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EntryList"></param>
        /// <returns></returns>
        public int INSERT_D_ENTRY_ADMIN(List<D_ENTRY> EntryList)
        {
            var sql = new StringBuilder();
            var param = new List<object>();
            var cntResult = 0;

            EntryList.ForEach(entry =>
            {
                param.Clear();
                sql.Clear();
                sql.AppendLine("INSERT INTO D_ENTRY");
                sql.AppendLine("      (ENTRY_UNIT_ID"); param.Add(entry.ENTRY_UNIT_ID);
                sql.AppendLine("      ,RECORD_KBN"); param.Add(entry.RECORD_KBN);
                sql.AppendLine("      ,IMAGE_SEQ"); param.Add(entry.IMAGE_SEQ);
                sql.AppendLine("      ,ITEM_ID"); param.Add(entry.ITEM_ID);
                sql.AppendLine("      ,VALUE"); param.Add(entry.VALUE);
                sql.AppendLine("      ,OCR_VALUE"); param.Add(entry.OCR_VALUE);
                sql.AppendLine("      ,DIFF_FLAG"); param.Add(entry.DIFF_FLAG);
                sql.AppendLine("      ,DUMMY_ITEM_FLAG"); param.Add(entry.DUMMY_ITEM_FLAG);
                sql.AppendLine("      ,INS_USER_ID"); param.Add(entry.INS_USER_ID);
                sql.AppendLine(")VALUES(?,?,?,?,?,?,?,?,?)");
                cntResult += ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
            });
            return cntResult;
        }
    }
}
