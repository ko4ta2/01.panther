using Common;
using Dao;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace BatchImportOcrCsv
{
    /// <summary>
    /// OCR連携結果CSV取込み
    /// </summary>
    class BlBatchImportOcrCsv
    {
        #region 変数
        /// <summary>
        /// log
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// データアクセス
        /// </summary>
        private static DaoBatchImportOcrCsv dao = new DaoBatchImportOcrCsv();

        /// <summary>
        /// OcrCsvファイルリスト
        /// </summary>
        private static string[] sFilePathList = null;

        /// <summary>
        /// イメージ座標ファイルリスト
        /// </summary>
        //private static string[] sFilePathListImageCoordinate = null;

        /// <summary>
        /// OCR項目ID変換マスタ
        /// </summary>
        //private static DataTable dt_M_ITEM_ID_CONV_OCR = null;// dao.SelectM_ITEM_ID_CONV_OCR();

        /// <summary>
        /// 帳票マスタ（OCR連携のみ対象）
        /// </summary>
        private static DataTable dt_M_DOC = null;// dao.SelectM_DOC();

        //private static Dictionary<string, string> dicDocId = new Dictionary<string, string>();

        /// <summary>
        /// 論理チェック対象エントリユニット
        /// </summary>
        private static List<string> sListLogicalCheckTargetUnit = new List<string>();

        /// <summary>
        /// メッセージ文言
        /// </summary>
        private static string sMessage = null;
        #endregion

        #region メイン処理
        /// <summary>
        /// メイン処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int BL_Main(string[] args)
        {
            try
            {
                log.Info("BatchOcrImportCsv:start");

                // ＤＢオープン
                dao.Open(Config.DSN);

                // OCR取込み設定取得
                var dt_M_ITEM_ID_CONV_OCR = dao.SELECT_M_ITEM_ID_CONV_OCR();

                // 帳票マスタ取得
                dt_M_DOC = dao.SELECT_M_DOC();

                var dic_DOC_ID = new Dictionary<string, string>();


                foreach (var dr_M_DOC in dt_M_DOC.AsEnumerable())
                {
                    dic_DOC_ID.Add(String.Join("_", dr_M_DOC["TKSK_CD"].ToString()
                                                , dr_M_DOC["HNM_CD"].ToString()
                                                , dr_M_DOC["DOC_ID"].ToString())
                                                , dr_M_DOC["DOC_ID_ENTRY"].ToString().Length == 0 ? dr_M_DOC["DOC_ID"].ToString() : dr_M_DOC["DOC_ID_ENTRY"].ToString());
                }

                // OCR結果CSV読込み
                var dt_OCT_CSV = GetOcrCsv();

                #region 取込状況チェック
                log.Info("↓↓↓↓↓↓↓↓↓↓　取込状況チェック　↓↓↓↓↓↓↓↓↓↓");
                foreach (var dr_OCR_CSV in dt_OCT_CSV.AsEnumerable())
                {
                    // ファイル名にパス情報が含まれる場合、ファイル名パス情報を除外する
                    if (dr_OCR_CSV["OCR_IMAGE_FILE_NAME"].ToString().Contains(@"\"))
                        dr_OCR_CSV["OCR_IMAGE_FILE_NAME"] = Path.GetFileName(dr_OCR_CSV["OCR_IMAGE_FILE_NAME"].ToString());

                    log.Debug("chk 得意先コード:{0}、品名コード:{1}、帳票ID:{2}、ファイル名:{3}"
                             , dr_OCR_CSV["TKSK_CD"].ToString()
                             , dr_OCR_CSV["HNM_CD"].ToString()
                             , dr_OCR_CSV["DOC_ID"].ToString()
                             , dr_OCR_CSV["OCR_IMAGE_FILE_NAME"].ToString());

                    // D_IMAGE_INFO 参照
                    var dt_D_IMAGE_INFO = dao.SELECT_D_IMAGE_INFO_OCR_IMAGE_FILE_NAME(dr_OCR_CSV["OCR_IMAGE_FILE_NAME"].ToString());
                    if (dt_D_IMAGE_INFO.Rows.Count == 0)
                    {
                        // エントリ単位分割 未処理
                        sMessage = String.Format("画像情報データが存在しません。得意先コード：{0}、品名コード：{1}、帳票ID：{2}、OCR連携イメージファイル名：{3}"
                                                , dr_OCR_CSV["TKSK_CD"].ToString()
                                                , dr_OCR_CSV["HNM_CD"].ToString()
                                                , dr_OCR_CSV["DOC_ID"].ToString()
                                                , dr_OCR_CSV["OCR_IMAGE_FILE_NAME"].ToString());
                        log.Error(sMessage); throw new ApplicationException(sMessage);
                    }
                    else
                    if (dt_D_IMAGE_INFO.Rows[0]["OCR_IMPORT_DATE"].ToString().Length != 0)
                    {
                        // 取込済
                        sMessage = String.Format("取込済のエントリ結果が連携されました。得意先コード：{0}、品名コード：{1}、帳票ID：{2}、OCR連携イメージファイル名：{3}"
                                                , dr_OCR_CSV["TKSK_CD"].ToString()
                                                , dr_OCR_CSV["HNM_CD"].ToString()
                                                , dr_OCR_CSV["DOC_ID"].ToString()
                                                , dr_OCR_CSV["OCR_IMAGE_FILE_NAME"].ToString());
                        log.Error(sMessage); throw new ApplicationException(sMessage);
                    }
                }
                log.Info("↑↑↑↑↑↑↑↑↑↑　取込状況チェック　↑↑↑↑↑↑↑↑↑↑");
                #endregion

                #region 取込
                // ＤＢトランザクション開始
                dao.BeginTrans();

                var iImportCount = 0;

                log.Info("↓↓↓↓↓↓↓↓↓↓　取　　　　　　込　↓↓↓↓↓↓↓↓↓↓");
                foreach (var dr_OCR_CSV in dt_OCT_CSV.AsEnumerable())
                {
                    log.Debug("imp 得意先コード:{0}、品名コード:{1}、帳票ID:{2}、ファイル名:{3}"
                             , dr_OCR_CSV["TKSK_CD"].ToString()
                             , dr_OCR_CSV["HNM_CD"].ToString()
                             , dr_OCR_CSV["DOC_ID"].ToString()
                             , dr_OCR_CSV["OCR_IMAGE_FILE_NAME"].ToString());

                    // D_IMAGE_INFO 参照
                    var dt_D_IMAGE_INFO = dao.SELECT_D_IMAGE_INFO_OCR_IMAGE_FILE_NAME(dr_OCR_CSV["OCR_IMAGE_FILE_NAME"].ToString());
                    if (dt_D_IMAGE_INFO.Rows.Count == 0)
                    {
                        // エントリ単位分割 未処理
                        sMessage = String.Format("画像情報データが存在しません。得意先コード：{0}、品名コード：{1}、帳票ID：{2}、OCR連携イメージファイル名：{3}"
                                                , dr_OCR_CSV["TKSK_CD"].ToString()
                                                , dr_OCR_CSV["HNM_CD"].ToString()
                                                , dr_OCR_CSV["DOC_ID"].ToString()
                                                , dr_OCR_CSV["OCR_IMAGE_FILE_NAME"].ToString());
                        log.Error(sMessage);                        throw new ApplicationException(sMessage);
                    }
                    /*
                                        // 未エントリのみ対象
                                        if (!Consts.EntryUnitStatus.ENTRY_NOT.Equals(dtD_IMAGE_INFO.Rows[0]["STATUS"].ToString()))
                                        {
                                            sMessage = String.Format("ステータスが取込み対象外です。得意先コード：{0}、品名コード：{1}、帳票ID：{2}、OCR連携イメージファイル名：{3}、ステータス：{4}"
                                                                    , drOcrCsv["TKSK_CD"].ToString()
                                                                    , drOcrCsv["HNM_CD"].ToString()
                                                                    , drOcrCsv["DOC_ID"].ToString()
                                                                    , drOcrCsv["OCR_IMAGE_FILE_NAME"].ToString()
                                                                    , dtD_IMAGE_INFO.Rows[0]["STATUS"].ToString());
                                            log.Warn(sMessage);
                                            continue;
                                        }
                    */
                    var iColunmNum = int.Parse(dr_OCR_CSV["COLUMN_NUM"].ToString());

                    int iIdx = 0;
                    foreach (DataColumn dcOcr in dr_OCR_CSV.Table.Columns)
                    {
                        iIdx++;
                        if (iIdx == (iColunmNum + 5))
                            break;

                        if (iIdx <= 5)
                            continue;

                        // OCR結果が空文字の場合更新しない。
                        if (dr_OCR_CSV[dcOcr.ColumnName].ToString().Length == 0)
                            continue;

                        var sDocIdEntry = dr_OCR_CSV["DOC_ID"].ToString();
                        if (dic_DOC_ID.ContainsKey(String.Join("_", dr_OCR_CSV["TKSK_CD"].ToString(), dr_OCR_CSV["HNM_CD"].ToString(), dr_OCR_CSV["DOC_ID"].ToString())))
                            sDocIdEntry = dic_DOC_ID[String.Join("_", dr_OCR_CSV["TKSK_CD"].ToString(), dr_OCR_CSV["HNM_CD"].ToString(), dr_OCR_CSV["DOC_ID"].ToString())];

                        // 項目ID変換マスタ参照
                        var dr_M_ITEM_ID_CONV_OCR = dt_M_ITEM_ID_CONV_OCR.Select(String.Format("TKSK_CD='{0}' AND HNM_CD='{1}' AND DOC_ID='{2}' AND ITEM_ID_OCR='{3}'"
                                                                                             , dr_OCR_CSV["TKSK_CD"].ToString()
                                                                                             , dr_OCR_CSV["HNM_CD"].ToString()
                                                                                             , dr_OCR_CSV["DOC_ID"].ToString()
                                                                                             , dcOcr.ColumnName));
                        if (dr_M_ITEM_ID_CONV_OCR.Length == 0)
                        {
                            // 存在しない場合、ENTRY_DOC_IDを取得
                            dr_M_ITEM_ID_CONV_OCR = dt_M_ITEM_ID_CONV_OCR.Select(String.Format("TKSK_CD='{0}' AND HNM_CD='{1}' AND DOC_ID='{2}' AND ITEM_ID_OCR='{3}'"
                                                                                             , dr_OCR_CSV["TKSK_CD"].ToString()
                                                                                             , dr_OCR_CSV["HNM_CD"].ToString()
                                                                                             , sDocIdEntry
                                                                                             , dcOcr.ColumnName));
                            if (dr_M_ITEM_ID_CONV_OCR.Length == 0)
                                continue;
                        }

                        #region 取込フラグが帳票IDで設定されている場合の対応
                        var sDocIds = dr_M_ITEM_ID_CONV_OCR[0]["IMPORT_1ST_FLAG"].ToString().Split(',');
                        if (new List<string>(sDocIds).Contains(dr_OCR_CSV["DOC_ID"].ToString()))
                            dr_M_ITEM_ID_CONV_OCR[0]["IMPORT_1ST_FLAG"] = Consts.Flag.ON;

                        sDocIds = dr_M_ITEM_ID_CONV_OCR[0]["IMPORT_2ND_FLAG"].ToString().Split(',');
                        if (new List<string>(sDocIds).Contains(dr_OCR_CSV["DOC_ID"].ToString()))
                            dr_M_ITEM_ID_CONV_OCR[0]["IMPORT_2ND_FLAG"] = Consts.Flag.ON;
                        #endregion

                        if (Consts.Flag.ON.Equals(dr_M_ITEM_ID_CONV_OCR[0]["IMPORT_1ST_FLAG"].ToString())
                            || Consts.Flag.ON.Equals(dr_M_ITEM_ID_CONV_OCR[0]["IMPORT_2ND_FLAG"].ToString()))
                        {
                            switch (dr_M_ITEM_ID_CONV_OCR[0]["TYPE"].ToString())
                            {
                                case "N":
                                    if (!"3".Equals(Config.Encode))
                                    {
                                        // 全角項目に半角が入っている可能性があるので全角に変換
                                        dr_OCR_CSV[dcOcr.ColumnName] = dr_OCR_CSV[dcOcr.ColumnName].ToString().Replace(@"\", @"￥");
                                        dr_OCR_CSV[dcOcr.ColumnName] = Strings.StrConv(dr_OCR_CSV[dcOcr.ColumnName].ToString(), VbStrConv.Wide, 0x411);
                                    }
                                    break;
                                case "9":
                                    // 全角値を半角へ変換
                                    dr_OCR_CSV[dcOcr.ColumnName] = Strings.StrConv(dr_OCR_CSV[dcOcr.ColumnName].ToString(), VbStrConv.Narrow);
                                    // 数値変換が出来ない場合、空文字
                                    decimal dOut;
                                    if (!decimal.TryParse(dr_OCR_CSV[dcOcr.ColumnName].ToString(), out dOut))
                                        dr_OCR_CSV[dcOcr.ColumnName] = string.Empty;
                                    break;
                                case "X":
                                    // 全角値を半角へ変換
                                    dr_OCR_CSV[dcOcr.ColumnName] = Strings.StrConv(dr_OCR_CSV[dcOcr.ColumnName].ToString(), VbStrConv.Narrow);
                                    break;
                            }

                            if (dr_M_ITEM_ID_CONV_OCR[0]["MAX_LENGTH"].ToString().Length != 0
                                && int.Parse(dr_M_ITEM_ID_CONV_OCR[0]["MAX_LENGTH"].ToString()) != 0)
                            {
                                // 長さが指定されていたら・・・
                                if (dr_OCR_CSV[dcOcr.ColumnName].ToString().Length > int.Parse(dr_M_ITEM_ID_CONV_OCR[0]["MAX_LENGTH"].ToString()))
                                {
                                    dr_OCR_CSV[dcOcr.ColumnName] = dr_OCR_CSV[dcOcr.ColumnName].ToString().Substring(0
                                                                                                                    , int.Parse(dr_M_ITEM_ID_CONV_OCR[0]["MAX_LENGTH"].ToString()));
                                }
                            }

                            if (Consts.Flag.ON.Equals(dt_D_IMAGE_INFO.Rows[0]["OCR_IMPORT_1ST_FLAG"].ToString())
                                && Consts.Flag.ON.Equals(dr_M_ITEM_ID_CONV_OCR[0]["IMPORT_1ST_FLAG"].ToString()))
                            {
                                // D_ENTRY 1人目更新
                                dao.UPDATE_D_ENTRY(dt_D_IMAGE_INFO.Rows[0]["ENTRY_UNIT_ID"].ToString()
                                                      , int.Parse(dt_D_IMAGE_INFO.Rows[0]["IMAGE_SEQ"].ToString())
                                                      , Consts.RecordKbn.Entry_1st
                                                      , dr_M_ITEM_ID_CONV_OCR[0]["ITEM_ID_ENTRY"].ToString()
                                                      , dr_OCR_CSV[dcOcr.ColumnName].ToString());
                            }

                            if (Consts.Flag.ON.Equals(dt_D_IMAGE_INFO.Rows[0]["OCR_IMPORT_2ND_FLAG"].ToString())
                                && Consts.Flag.ON.Equals(dr_M_ITEM_ID_CONV_OCR[0]["IMPORT_2ND_FLAG"].ToString()))
                            {
                                // D_ENTRY 2人目更新
                                dao.UPDATE_D_ENTRY(dt_D_IMAGE_INFO.Rows[0]["ENTRY_UNIT_ID"].ToString()
                                                      , int.Parse(dt_D_IMAGE_INFO.Rows[0]["IMAGE_SEQ"].ToString())
                                                      , Consts.RecordKbn.Entry_2nd
                                                      , dr_M_ITEM_ID_CONV_OCR[0]["ITEM_ID_ENTRY"].ToString()
                                                      , dr_OCR_CSV[dcOcr.ColumnName].ToString());
                            }
                        }
                        else
                        {
                            // 取込み対象外
                            sMessage = String.Format("取込み対象外項目　得意先コード：{0}、品名コード：{1}、帳票ID：{2}、項目ID：{3}"
                                                    , dr_OCR_CSV["TKSK_CD"].ToString()
                                                    , dr_OCR_CSV["HNM_CD"].ToString()
                                                    , dr_OCR_CSV["DOC_ID"].ToString()
                                                    , dcOcr.ColumnName);
                            log.Info(sMessage);
                        }
                    }

                    // D_IMAGE_INFO 更新
                    iImportCount += dao.UPDATE_D_IMAGE_INFO(dt_D_IMAGE_INFO.Rows[0]["ENTRY_UNIT_ID"].ToString()
                                                           , int.Parse(dt_D_IMAGE_INFO.Rows[0]["IMAGE_SEQ"].ToString()));

                    // D_OCR_COOPERATION_HISTORY 更新
                    if (dao.UPDATE_D_OCR_COOPERATION_HISTORY(dr_OCR_CSV["OCR_IMAGE_FILE_NAME"].ToString()) != 1)
                        throw new ApplicationException(String.Format("D_OCR_COOPERATION_HISTORY 更新処理で不整合:{0}", dr_OCR_CSV["OCR_IMAGE_FILE_NAME"].ToString()));

                    // エントリ単位レコード数取得
                    var dtD_IMAGE_INFO2 = dao.SELECT_D_IMAGE_INFO_ENTRY_UNIT_ID(dt_D_IMAGE_INFO.Rows[0]["ENTRY_UNIT_ID"].ToString());

                    // 内Ocr取込み済件数取得
                    var iOcrImportedCount = dtD_IMAGE_INFO2.Select(String.Format("OCR_IMPORT_DATE IS NOT NULL")).Length;

                    // エントリ単レコード数と内Ocr取込み件数を比較
                    if (iOcrImportedCount == dtD_IMAGE_INFO2.Rows.Count)
                    {
                        if (Consts.Flag.ON.Equals(dt_D_IMAGE_INFO.Rows[0]["OCR_IMPORT_1ST_FLAG"].ToString()))
                        {
                            if (dao.UPDATE_D_ENTRY_STATUS(dt_D_IMAGE_INFO.Rows[0]["ENTRY_UNIT_ID"].ToString()
                                                         , Consts.RecordKbn.Entry_1st
                                                         , Consts.Flag.ON.Equals(dt_D_IMAGE_INFO.Rows[0]["NO_ENTRY_FLAG"].ToString()) ? Consts.EntryStatus.ENTRY_END : null) != 1)
                                throw new ApplicationException("D_ENTRY_STATUS OCR結果取込ユーザ更新（1st）で不整合発生");
                        }

                        if (Consts.Flag.ON.Equals(dt_D_IMAGE_INFO.Rows[0]["OCR_IMPORT_2ND_FLAG"].ToString()))
                        {
                            if (dao.UPDATE_D_ENTRY_STATUS(dt_D_IMAGE_INFO.Rows[0]["ENTRY_UNIT_ID"].ToString()
                                                         , Consts.RecordKbn.Entry_2nd
                                                         , Consts.Flag.ON.Equals(dt_D_IMAGE_INFO.Rows[0]["NO_ENTRY_FLAG"].ToString()) ? Consts.EntryStatus.ENTRY_END : null) != 1)
                                throw new ApplicationException("D_ENTRY_STATUS OCR結果取込ユーザ更新（2nd）で不整合発生");
                        }

                        // 一致したら D_ENTRY_UNIT 更新
                        // フラグの状態からステータスを設定
                        var UnitStatus = Consts.EntryUnitStatus.ENTRY_NOT;  // 未エントリ
                        if (Consts.Flag.ON.Equals(dt_D_IMAGE_INFO.Rows[0]["NO_ENTRY_FLAG"].ToString()))
                        {
                            if (Consts.Flag.ON.Equals(dt_D_IMAGE_INFO.Rows[0]["SINGLE_ENTRY_FLAG"].ToString()))
                            {
                                // シングルエントリ
                                UnitStatus = Consts.EntryUnitStatus.ENTRY_END;
                            }
                            else
                            {
                                // ダブルエントリ
                                if (Consts.Flag.ON.Equals(dt_D_IMAGE_INFO.Rows[0]["OCR_IMPORT_1ST_FLAG"].ToString())
                                    && Consts.Flag.ON.Equals(dt_D_IMAGE_INFO.Rows[0]["OCR_IMPORT_2ND_FLAG"].ToString()))
                                {
                                    // 両方「ON」→エントリ終了
                                    UnitStatus = Consts.EntryUnitStatus.ENTRY_END;
                                }
                                else
                                if (Consts.Flag.ON.Equals(dt_D_IMAGE_INFO.Rows[0]["OCR_IMPORT_1ST_FLAG"].ToString())
                                    || Consts.Flag.ON.Equals(dt_D_IMAGE_INFO.Rows[0]["OCR_IMPORT_2ND_FLAG"].ToString()))
                                {
                                    UnitStatus = Consts.EntryUnitStatus.ENTRY_EDT;
                                }
                            }
                        }

                        if (!Consts.EntryUnitStatus.ENTRY_NOT.Equals(dt_D_IMAGE_INFO.Rows[0]["OCR_IMPORT_UNIT_STATUS"].ToString()))
                            UnitStatus = dt_D_IMAGE_INFO.Rows[0]["OCR_IMPORT_UNIT_STATUS"].ToString();

                        if (dao.UPDATE_D_ENTRY_UNIT(dt_D_IMAGE_INFO.Rows[0]["ENTRY_UNIT_ID"].ToString(), UnitStatus) != 1)
                            throw new ApplicationException("D_ENTRY_UNIT STATUS 更新で不整合発生");

                        // プレ論理チェック対象ユニット
                        if (Consts.Flag.ON.Equals(dt_D_IMAGE_INFO.Rows[0]["PRE_LOGICAL_CHECK_FLAG"].ToString()))
                            sListLogicalCheckTargetUnit.Add(dt_D_IMAGE_INFO.Rows[0]["ENTRY_UNIT_ID"].ToString());
                    }

                    if (iImportCount % 1000 == 0)
                    {
                        // 1000件単位に表示
                        sMessage = String.Format("OCR連携ファイル処理件数：{0}", iImportCount.ToString("#,0").PadLeft(7));
                        Console.WriteLine(sMessage); log.Info(sMessage);
                    }
                }
                log.Info("↑↑↑↑↑↑↑↑↑↑　取　　　　　　込　↑↑↑↑↑↑↑↑↑↑");
                #endregion

                //sListLogicalCheckTargetUnit.Add(String.Join("_", "20181228", "01", "02010001", "001"));
                //sListLogicalCheckTargetUnit.Add(String.Join("_", "20181228", "01", "02010001", "002"));
                //sListLogicalCheckTargetUnit.Add(String.Join("_", "20181228", "01", "02010001", "003"));
                //sListLogicalCheckTargetUnit.Add(String.Join("_", "20181228", "01", "02010001", "004"));

                if (iImportCount % 1000 != 0)
                {
                    // 最終件数を表示
                    sMessage = String.Format("OCR連携ファイル処理件数：{0}", iImportCount.ToString("#,0").PadLeft(7));
                    Console.WriteLine(sMessage); log.Info(sMessage);
                }
                /*
                                #region 座標取込
                                ImportImagePos();
                                #endregion
                */
                // プレ論理チェック
                if (sListLogicalCheckTargetUnit.Count != 0)
                    DllLogicalCheck.DllLogicalCheck.BL_Main(sListLogicalCheckTargetUnit);

                #region 処理済ファイル退避
                // OCR読取結果
                BackupOcrCsv();
                /*
                                // イメージ座標
                                BackupImageCoordinateTsv();
                */
                #endregion

                // ＤＢトランザクションコミット
                dao.CommitTrans();
                return (int)Consts.RetCode.OK;
            }
            catch /*(Exception ex)*/
            {
                // ＤＢトランザクションロールバック
                dao.RollbackTrans();
                throw;
            }
            finally
            {
                // ＤＢクローズ
                dao.Close();
                log.Info("BatchOcrImportCsv:end");
            }
        }
        #endregion

        #region OCR結果CSV読込
        /// <summary>
        /// OCR結果CSV読込み
        /// </summary>
        /// <returns></returns>
        private static DataTable GetOcrCsv()
        {
            #region 空DataTable作成
            var dt_OCR_CSV = new DataTable();
            dt_OCR_CSV.Columns.Add("TKSK_CD");
            dt_OCR_CSV.Columns.Add("HNM_CD");
            dt_OCR_CSV.Columns.Add("DOC_ID");
            dt_OCR_CSV.Columns.Add("COLUMN_NUM");
            dt_OCR_CSV.Columns.Add("OCR_IMAGE_FILE_NAME");
            for (int iIdx = 1; iIdx <= Consts.iOcrMaxItemCount; iIdx++)
            {
                dt_OCR_CSV.Columns.Add(String.Format("ITEM_{0}", iIdx.ToString("d3")));
            }
            #endregion

            var BussinessIdList = new List<string>();
            if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
            {
                var dt_M_BUSINESS = dao.SELECT_M_BUSINESS();
                foreach (var dr in dt_M_BUSINESS.AsEnumerable())
                {
                    BussinessIdList.Add(String.Join("_", dr["BUSINESS_ID"].ToString(), dr["TKSK_CD"].ToString(), dr["HNM_CD"].ToString()));
                }
            }
            else
            {
                BussinessIdList.Add(String.Join("_", Config.UserId, Config.TokuisakiCode, Config.HinmeiCode));
            }

            foreach (var BussinessId in BussinessIdList)
            {
                var TkskCd = BussinessId.Split('_')[1];
                var HnmCd = BussinessId.Split('_')[2];
                string DocId = null;
                var ColumnNum = -1;

                log.Info($"得意先コード:{TkskCd} 品名コード:{HnmCd}");

                // OcrCsvファイル取得
                sFilePathList = Directory.GetFiles(Config.OcrCsvRoot, String.Format("OCR_{0}_{1}_*.csv", TkskCd, HnmCd), System.IO.SearchOption.TopDirectoryOnly);
                if (sFilePathList.Length == 0)
                {
                    // TSVで再度取得
                    sFilePathList = Directory.GetFiles(Config.OcrCsvRoot, String.Format("OCR_{0}_{1}_*.tsv", TkskCd, HnmCd), System.IO.SearchOption.TopDirectoryOnly);
                }
                log.Info("取得OCR結果ファイル数：{0}件", sFilePathList.Length.ToString("#,0"));

                foreach (var sFilePath in sFilePathList)
                {
                    log.Info("取得パス:{0}", sFilePath/*.FullName*/);
                    var stArrayData = Path.GetFileName(sFilePath/*.FullName*/).Split('_');
                    DocId = String.Concat(stArrayData[3],stArrayData[4]);   // 書類種別 + 枝番

                    //log.Debug("帳票ID:{0}", DocId);

                    var drM_DOC = dt_M_DOC.Select(String.Format("TKSK_CD='{0}' AND HNM_CD='{1}' AND DOC_ID='{2}'", TkskCd, HnmCd, DocId));
                    if (drM_DOC.Length != 1)
                    {
                        sMessage = String.Format("不正なファイル{0}を読込みました。", Path.GetFileName(sFilePath/*.FullName*/));
                        log.Error(sMessage); throw new Exception(sMessage);
                    }
                    ColumnNum = int.Parse(drM_DOC[0]["OCR_CSV_COLUMN_NUM"].ToString());

                    using (var parser = new TextFieldParser(sFilePath/*.FullName*/, Config.Encode.Equals("3") ? Consts.EncUTF8N : Consts.EncShift_JIS))
                    {
                        parser.TextFieldType = FieldType.Delimited;
                        parser.SetDelimiters(",", "\t");

                        var iLine = 0;
                        while (!parser.EndOfData)
                        {
                            iLine++;
                            var DataColumns = parser.ReadFields();
                            // ヘッダレコード除外
                            if (iLine == 1)
                            {
                                if (DataColumns.Length != ColumnNum)
                                {
                                    sMessage = String.Format("CSVファイルのカラム数が想定と異なります。想定カラム数：{0}　実カラム数：{1}"
                                                            , ColumnNum
                                                            , DataColumns.Length);
                                    log.Error(sMessage); throw new Exception(sMessage);
                                }
                                continue;
                            }

                            var drOcrCsv = dt_OCR_CSV.NewRow();
                            drOcrCsv[0] = TkskCd;
                            drOcrCsv[1] = HnmCd;
                            drOcrCsv[2] = DocId;
                            drOcrCsv[3] = ColumnNum;
                            var idx = 4;
                            foreach (var val in DataColumns)
                            {
                                drOcrCsv[idx] = val;
                                idx++;
                            }
                            dt_OCR_CSV.Rows.Add(drOcrCsv);
                        }
                    }
                }
            }
            // デバッグで中身が見れない！！項目多過ぎ？
            return dt_OCR_CSV;
        }
        #endregion
        /*
                #region OCR結果座標CSV読込み
                /// <summary>
                /// OCR結果CSV読込み
                /// </summary>
                /// <returns></returns>
                private static DataTable GetImageCoordinate()
                {
                    // 空DataTable作成
                    DataTable dtImageCoordinate = new DataTable();
                    dtImageCoordinate.Columns.Add("TKSK_CD");
                    dtImageCoordinate.Columns.Add("HNM_CD");
                    dtImageCoordinate.Columns.Add("DOC_ID");
                    dtImageCoordinate.Columns.Add("COLUMN_NUM");
                    dtImageCoordinate.Columns.Add("OCR_IMAGE_FILE_NAME");
                    for (int iIdx = 1; iIdx <= Consts.iOcrMaxItemCount; iIdx++)
                        dtImageCoordinate.Columns.Add(String.Format("ITEM_{0}", iIdx.ToString("d3")));

                    if (Config.ImagePositionRoot.Length == 0)
                        return dtImageCoordinate;

                    // OcrCsvファイル取得
                    sFilePathListImageCoordinate = System.IO.Directory.GetFiles(Config.ImagePositionRoot, String.Format("OCR_{0}_{1}_ImageCoordinate_*.csv", Config.TokuisakiCode, Config.HinmeiCode), System.IO.SearchOption.TopDirectoryOnly);
                    if (sFilePathListImageCoordinate.Length == 0)
                        sFilePathListImageCoordinate = System.IO.Directory.GetFiles(Config.ImagePositionRoot, String.Format("OCR_{0}_{1}_*_*_ImageCoordinate_*.tsv", Config.TokuisakiCode, Config.HinmeiCode), System.IO.SearchOption.TopDirectoryOnly);
                    log.Info("取得OCR結果ファイル数：{0}件", sFilePathList.Length.ToString("#,0"));
                    //fiOcrCsv = di.EnumerateFiles(String.Format("OCR_{0}_{1}_*.csv",Config.TokuisakiCode,Config.HinmeiCode), System.IO.SearchOption.TopDirectoryOnly);
                    //foreach (var sFilePath in fiOcrCsv)
                    //    sFilePathList.Add(sFilePath.FullName);

                    string sTkskCd = null;
                    string sHnmCd = null;
                    string sDocId = null;
                    string sColumnNum = null;
                    foreach (var sFilePath in sFilePathListImageCoordinate)
                    {
                        log.Info("取得パス:{0}", sFilePath);
                        log.Debug("取得ファイル名:{0}", Path.GetFileName(sFilePath));
                        string[] stArrayData = Path.GetFileName(sFilePath).Split('_');
                        sTkskCd = stArrayData[1];                   // 得意先コード
                        sHnmCd = stArrayData[2];                    // 品名コード
                        sDocId = stArrayData[3] + stArrayData[4];   // 書類種別 + 枝番

                        log.Debug("得意先コード:{0}", sTkskCd);
                        log.Debug("品名コード:{0}", sHnmCd);
                        log.Debug("帳票ID:{0}", sDocId);

                        DataRow[] drM_DOC = dtM_DOC.Select(String.Format("TKSK_CD='{0}' AND HNM_CD='{1}' AND DOC_ID='{2}'", sTkskCd, sHnmCd, sDocId));
                        if (drM_DOC.Length != 1)
                        {
                            sMessage = String.Format("不正なファイル{0}を読込みました。", Path.GetFileName(sFilePath));
                            log.Error(sMessage);
                            throw new System.Exception(sMessage);
                        }
                        sColumnNum = drM_DOC[0]["OCR_CSV_COLUMN_NUM"].ToString();
                        log.Debug("項目数:{0}", sColumnNum);

                        using (var parser = new TextFieldParser(sFilePath, Consts.EncShift_JIS))
                        {
                            parser.TextFieldType = FieldType.Delimited;
                            parser.SetDelimiters("\t");

                            int iLine = 0;
                            while (!parser.EndOfData)
                            {
                                iLine++;
                                string[] sColumns = parser.ReadFields();
                                // ヘッダレコード除外
                                if (iLine == 1)
                                {
                                    if (sColumns.Length != int.Parse(drM_DOC[0]["OCR_CSV_COLUMN_NUM"].ToString()))
                                    {
                                        sMessage = String.Format("tsvファイルのカラム数が想定と異なります。想定カラム数：{0}　実カラム数：{1}"
                                                                , int.Parse(drM_DOC[0]["OCR_CSV_COLUMN_NUM"].ToString())
                                                                , sColumns.Length);
                                        log.Error(sMessage);
                                        throw new System.Exception(sMessage);
                                    }
                                    continue;
                                }

                                DataRow drOcrCsv = dtImageCoordinate.NewRow();
                                drOcrCsv[0] = sTkskCd;
                                drOcrCsv[1] = sHnmCd;
                                drOcrCsv[2] = sDocId;
                                drOcrCsv[3] = sColumnNum;
                                int idx = 4;
                                foreach (string val in sColumns)
                                {
                                    drOcrCsv[idx] = val;
                                    idx++;
                                }
                                dtImageCoordinate.Rows.Add(drOcrCsv);
                            }
                        }
                    }
                    // デバッグで中身が見れない！！項目多過ぎ？
                    return dtImageCoordinate;
                }
                #endregion
        */
        /// <summary>
        /// 処理済OCR結果退避
        /// </summary>
        private static void BackupOcrCsv()
        {
            if (sFilePathList.Length == 0)
                return;

            // バックアップ先フォルダ
            DirectoryInfo diDst = new DirectoryInfo(Path.Combine(Config.OcrCsvRoot, String.Format(@"_bk\{0}_{1}\{2}", Config.TokuisakiCode, Config.HinmeiCode, DateTime.Now.ToString("yyyyMMdd_HHmmss"))));
            // 無ければ作成
            if (!diDst.Exists)
                diDst.Create();

            // 移動
            foreach (var sSrcFilePath in sFilePathList)
            {
                var sDstFilePath = Path.Combine(diDst.FullName, Path.GetFileName(sSrcFilePath));
                sMessage = String.Format("バックアップ対象ファイル:{0}→{1}", sSrcFilePath, sDstFilePath);
                log.Info(sMessage);//Console.WriteLine(sMessage);
                System.IO.File.Move(sSrcFilePath, sDstFilePath);
            }
        }
        /*
                /// <summary>
                /// イメージ座標取込
                /// </summary>
                private static void ImportImagePos()
                {
                    sMessage = String.Format("イメージ座標取込:start");
                    Console.WriteLine(sMessage); log.Info(sMessage);

                    var dtImageCoordinate = GetImageCoordinate();
                    var iCount = 0;
                    foreach (DataRow drImageCoordinate in dtImageCoordinate.Rows)
                    {
                        // 項目数取得
                        int iColunmNum = int.Parse(drImageCoordinate["COLUMN_NUM"].ToString()) - 1;

                        // 座標リスト作成
                        List<string> sListCoordinate = new List<string>();
                        for (int iIdx = 1; iIdx <= iColunmNum; iIdx++)
                            sListCoordinate.Add(drImageCoordinate[String.Format("ITEM_{0}", iIdx.ToString("d3"))].ToString());

                        // 帳票ID取得
                        string sDocId = drImageCoordinate["DOC_ID"].ToString();
                        if (dicDocId.ContainsKey(drImageCoordinate["DOC_ID"].ToString()))
                            sDocId = dicDocId[drImageCoordinate["DOC_ID"].ToString()];

                        // DB登録
                        dao.INSERT_READING_PARTS(drImageCoordinate["TKSK_CD"].ToString()
                                                , drImageCoordinate["HNM_CD"].ToString()
                                                , sDocId
                                                , String.Format("OCR_{0}_{1}_{2}_{3}_{4}", Config.TokuisakiCode
                                                                                         , Config.HinmeiCode
                                                                                         , sDocId.Substring(0,4)
                                                                                         , sDocId.Substring(4, 4)
                                                                                         , drImageCoordinate["OCR_IMAGE_FILE_NAME"].ToString())
                                                , sListCoordinate);
                        iCount++;
                        if (iCount % 1000 == 0)
                        {
                            sMessage = String.Format("イメージ座標取込件数：{0}", iCount.ToString("#,0").PadLeft(7));
                            Console.WriteLine(sMessage); log.Info(sMessage);
                        }
                    }
                    if (iCount % 1000 != 0)
                    {
                        sMessage = String.Format("イメージ座標取込件数：{0}", iCount.ToString("#,0").PadLeft(7));
                        Console.WriteLine(sMessage); log.Info(sMessage);
                    }
                    sMessage = String.Format("イメージ座標取込:end");
                    Console.WriteLine(sMessage); log.Info(sMessage);
                }

                /// <summary>
                /// 処理済イメージ座標退避
                /// </summary>
                private static void BackupImageCoordinateTsv()
                {
                    if (sFilePathListImageCoordinate == null || sFilePathListImageCoordinate.Length == 0)
                        return;

                    // バックアップ先フォルダ
                    System.IO.DirectoryInfo diDst = new System.IO.DirectoryInfo(Path.Combine(Config.ImagePositionRoot, String.Format(@"bk\{0}_{1}\{2}", Config.TokuisakiCode, Config.HinmeiCode, DateTime.Now.ToString("yyyyMMdd_HHmmss"))));
                    // 無ければ作成
                    if (!diDst.Exists)
                        diDst.Create();

                    // 移動
                    string sDstFilePath = null;
                    foreach (var sSrcFilePath in sFilePathListImageCoordinate)
                    {
                        sDstFilePath = Path.Combine(diDst.FullName, Path.GetFileName(sSrcFilePath));
                        sMessage = String.Format("バックアップ対象ファイル:{0}→{1}", sSrcFilePath, sDstFilePath);
                        log.Info(sMessage);//Console.WriteLine(sMessage);
                        System.IO.File.Move(sSrcFilePath, sDstFilePath);
                    }
                }
                */
    }
}