using ODPCtrl;

namespace BPOEntry.DivideEntryUnitImage
{
    public class DaoDivideEntryUnitImage : DaoBase
    {
        ///// <summary>
        ///// 入力単位分割済みの画像リストを取得します。
        ///// </summary>
        ///// <param name="date">画像取込日</param>
        ///// <param name="capNum">画像取込回数</param>
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

        ///// <summary>
        ///// 各帳票の入力項目数を取得します。
        ///// </summary>
        //public DataTable SelectEntryItemNum()
        //{
        //    var sql = new StringBuilder();
        //    var param = new List<object>();
        //    sql.AppendLine("SELECT * FROM M_DOC ");
        //    sql.AppendLine(" WHERE TKSK_CD=?"); param.Add(Common.Config.TokuisakiCode);
        //    sql.AppendLine("   AND HNM_CD=?"); param.Add(Common.Config.HinmeiCode);
        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteQuery(sql.ToString(), param);
        //}

        ///// <summary>
        ///// 入力単位データを登録します。
        ///// </summary>
        ///// <param name="record">登録情報</param>
        ///// <returns>件数</returns>
        //public int InsertD_ENTRY_UNIT(D_ENTRY_UNIT record)
        //{
        //    var sql = new StringBuilder();
        //    var param = new List<object>();
        //    sql.AppendLine("INSERT INTO D_ENTRY_UNIT ( ");
        //    sql.AppendLine("       DOC_ID "); param.Add(record.DOC_ID);
        //    sql.AppendLine("     , IMAGE_CAPTURE_DATE "); param.Add(record.IMAGE_CAPTURE_DATE);
        //    sql.AppendLine("     , IMAGE_CAPTURE_NUM "); param.Add(record.IMAGE_CAPTURE_NUM);
        //    sql.AppendLine("     , ENTRY_UNIT "); param.Add(record.ENTRY_UNIT);
        //    sql.AppendLine("     , STATUS "); param.Add(record.ENTRY_UNIT_STATUS);
        //    sql.AppendLine("     , INS_USER_ID "); param.Add(record.INS_USER_ID);
        //    //sql.AppendLine("     , UPD_USER_ID "); param.Add(record.UPD_USER_ID);
        //    sql.AppendLine("     , TKSK_CD "); param.Add(Common.Config.TokuisakiCode);
        //    sql.AppendLine("     , HNM_CD "); param.Add(Common.Config.HinmeiCode);
        //    sql.AppendLine(") ");
        //    sql.AppendLine("VALUES ( ?,?,?,?,?,?,?,?/*,?*/)");
        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        //}

        ///// <summary>
        ///// 画像情報データを登録します。
        ///// </summary>
        ///// <param name="record">登録情報</param>
        ///// <returns>件数</returns>
        //public int InsertD_IMAGE_INFO(D_IMAGE_INFO record)
        //{
        //    var sql = new StringBuilder();
        //    var param = new List<object>();
        //    sql.AppendLine("INSERT INTO D_IMAGE_INFO ( ");
        //    sql.AppendLine("       TKSK_CD "); param.Add(Common.Config.TokuisakiCode);
        //    sql.AppendLine("     , HNM_CD "); param.Add(Common.Config.HinmeiCode);
        //    sql.AppendLine("     , IMAGE_CAPTURE_DATE "); param.Add(record.IMAGE_CAPTURE_DATE);
        //    sql.AppendLine("     , IMAGE_CAPTURE_NUM "); param.Add(record.IMAGE_CAPTURE_NUM);
        //    sql.AppendLine("     , DOC_ID "); param.Add(record.DOC_ID);
        //    sql.AppendLine("     , ENTRY_UNIT "); param.Add(record.ENTRY_UNIT);
        //    sql.AppendLine("     , IMAGE_SEQ "); param.Add(record.IMAGE_SEQ);
        //    sql.AppendLine("     , IMAGE_PATH "); param.Add(record.IMAGE_PATH);
        //    sql.AppendLine("     , OCR_IMAGE_FILE_NAME"); param.Add(record.OCR_IMAGE_FILE_NAME);
        //    sql.AppendLine("     , DUMMY_IMAGE_FLAG"); param.Add(record.DUMMY_IMAGE_FLAG);
        //    sql.AppendLine("     , INS_USER_ID "); param.Add(record.INS_USER_ID);
        //    sql.AppendLine(") ");
        //    sql.AppendLine("VALUES ( ?,?,?,?,?,?,?,?,?,?,?)");
        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        //}

        ///// <summary>
        ///// 入力状態データを登録します。
        ///// </summary>
        ///// <param name="record">登録情報</param>
        ///// <returns>件数</returns>
        //public int InsertD_ENTRY_STATUS(D_ENTRY_STATUS record)
        //{
        //    var sql = new StringBuilder();
        //    var param = new List<object>();
        //    sql.AppendLine("INSERT INTO D_ENTRY_STATUS ( ");
        //    sql.AppendLine("       TKSK_CD "); param.Add(Common.Config.TokuisakiCode);
        //    sql.AppendLine("     , HNM_CD "); param.Add(Common.Config.HinmeiCode);
        //    sql.AppendLine("     , IMAGE_CAPTURE_DATE "); param.Add(record.IMAGE_CAPTURE_DATE);
        //    sql.AppendLine("     , IMAGE_CAPTURE_NUM "); param.Add(record.IMAGE_CAPTURE_NUM);
        //    sql.AppendLine("     , DOC_ID "); param.Add(record.DOC_ID);
        //    sql.AppendLine("     , ENTRY_UNIT "); param.Add(record.ENTRY_UNIT);
        //    sql.AppendLine("     , RECORD_KBN "); param.Add(record.RECORD_KBN);
        //    sql.AppendLine("     , INS_USER_ID "); param.Add(record.INS_USER_ID);
        //    sql.AppendLine(") ");
        //    sql.AppendLine("VALUES ( ?,?,?,?,?,?,?,?/*,?*/)");
        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        //}

        //public int InsertD_ENTRY(D_ENTRY record)
        //{
        //    var sql = new StringBuilder();
        //    var param = new List<object>();
        //    sql.AppendLine("INSERT INTO D_ENTRY ( ");
        //    sql.AppendLine("       DOC_ID "); param.Add(record.DOC_ID);
        //    sql.AppendLine("     , IMAGE_CAPTURE_DATE "); param.Add(record.IMAGE_CAPTURE_DATE);
        //    sql.AppendLine("     , IMAGE_CAPTURE_NUM "); param.Add(record.IMAGE_CAPTURE_NUM);
        //    sql.AppendLine("     , ENTRY_UNIT "); param.Add(record.ENTRY_UNIT);
        //    sql.AppendLine("     , IMAGE_SEQ "); param.Add(record.IMAGE_SEQ);
        //    sql.AppendLine("     , RECORD_KBN "); param.Add(record.RECORD_KBN);
        //    sql.AppendLine("     , ITEM_ID "); param.Add(record.ITEM_ID);
        //    sql.AppendLine("     , INS_USER_ID "); param.Add(record.INS_USER_ID);
        //    //sql.AppendLine("     , UPD_USER_ID "); param.Add(record.UPD_USER_ID);
        //    sql.AppendLine("     , TKSK_CD "); param.Add(Common.Config.TokuisakiCode);
        //    sql.AppendLine("     , HNM_CD "); param.Add(Common.Config.HinmeiCode);
        //    sql.AppendLine(") ");
        //    sql.AppendLine("VALUES ( ?,?,?,?,?,?,?,?,?,?/*,?*/)");
        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        //}

        ///// <summary>
        ///// OCR連携履歴登録
        ///// </summary>
        ///// <param name="sDocId"></param>
        ///// <param name="sSrcFolder"></param>
        ///// <param name="sSrcImageFileName"></param>
        ///// <param name="sOcrImageFileName"></param>
        ///// <returns></returns>
        //public int InsertD_OCR_COOPERATION_HISTORY(string sDocId, string sSrcFolder, string sSrcImageFileName, string sOcrImageFileName)
        //{
        //    var sql = new StringBuilder();
        //    var param = new List<object>();
        //    sql.AppendLine("INSERT INTO D_OCR_COOPERATION_HISTORY");
        //    sql.AppendLine("      (TKSK_CD"); param.Add(Common.Config.TokuisakiCode);
        //    sql.AppendLine("      ,HNM_CD"); param.Add(Common.Config.HinmeiCode);
        //    sql.AppendLine("      ,DOC_ID"); param.Add(sDocId);
        //    sql.AppendLine("      ,SRC_FOLDER_NAME"); param.Add(sSrcFolder);
        //    sql.AppendLine("      ,SRC_IMAGE_FILE_NAME"); param.Add(sSrcImageFileName);
        //    sql.AppendLine("      ,OCR_IMAGE_FILE_NAME"); param.Add(sOcrImageFileName);
        //    sql.AppendLine("      ,INS_USER_ID"); param.Add(Program.LoginUser.USER_ID);
        //    sql.AppendLine("      )VALUES(?,?,?,?,?,?,?)");
        //    return ComContext.GetSQLFacade(Config.DSN).ExecuteNonQuery(sql.ToString(), param);
        //}
    
    }
}
