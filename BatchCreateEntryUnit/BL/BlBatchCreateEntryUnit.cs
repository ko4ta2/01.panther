using Common;
using Dao;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace BatchCreateEntryUnit
{
    /// <summary>
    /// エントリ単位分割
    /// </summary>
    class BlBatchCreateEntryUnit
    {
        #region 変数
        /// <summary>
        /// NLog
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// データアクセス
        /// </summary>
        private static DaoBatchCreateEntryUnit dao = new DaoBatchCreateEntryUnit();

        private static string sMessage = null;

        /// <summary>
        /// システム年月日
        /// </summary>
        private static string sSysDate = System.DateTime.Now.ToString("yyyyMMdd");
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
                log.Info("BatchCreateEntryUnit:start");

                // ＤＢオープン
                dao.Open(Config.DSN);

                // ＤＢトランザクション開始
                dao.BeginTrans();

                if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
                {
                    Cosmos();
                }
                else
                {
                    Normal();
                }

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
                log.Info("BatchCreateEntryUnit:end");
            }
        }
        #endregion

        #region COSMOS
        /// <summary>
        /// COSMOS
        /// </summary>
        private static void Cosmos()
        {
            var iCount = 0;

            // エントリ単位分割済フォルダ取得
            var dtT_DIV_ENTRY_UNIT_PATH = dao.SELECT_T_DIV_ENTRY_UNIT_PATH();
            var ListEnd = new List<string>();
            foreach (var drT_DIV_ENTRY_UNIT_PATH in dtT_DIV_ENTRY_UNIT_PATH.AsEnumerable())
            {
                ListEnd.Add(String.Join("_"
                                       , drT_DIV_ENTRY_UNIT_PATH["TKSK_CD"].ToString()
                                       , drT_DIV_ENTRY_UNIT_PATH["HNM_CD"].ToString()
                                       , drT_DIV_ENTRY_UNIT_PATH["IMAGE_CAPTURE_PATH"].ToString()));
            }

            // 
            var ListEndFile = new List<string>();
            foreach (var file in Directory.GetFiles(Config.END_FILE_DIRECTORY, "*.end", SearchOption.TopDirectoryOnly))
            {
                ListEndFile.Add(file);
            }

            var ListEndFileProcessed = new List<string>();
            foreach (var dinfo in ListEndFile)
            {
                var x = Path.GetFileNameWithoutExtension(dinfo).Split('_');
                var businessId = x[0];
                var sImageCaptureDate = x[1];
                var sImageCaptureNum = x[2];

                // 数値？
                if (!Utils.IsNumeric(String.Concat(sImageCaptureDate, sImageCaptureNum)))
                {
                    sMessage = String.Format("フォルダ名が数値以外:        {0}", dinfo);
                    log.Info(sMessage);
                    continue;
                }

                // 連携日+回数　10桁？
                if ((String.Concat(sImageCaptureDate, sImageCaptureNum)).Length != 10)
                {
                    sMessage = String.Format("フォルダ名の長さ10バイト以外:{0}", dinfo);
                    log.Info(sMessage);
                    continue;
                }

                // 連携回数が"00"以外
                if ("00".Equals(sImageCaptureNum))
                {
                    sMessage = String.Format(@"連携回数が""00"":              {0}", dinfo);
                    log.Info(sMessage);
                    continue;
                }

                // 未来日は対象外
                if (sImageCaptureDate.CompareTo(sSysDate) == 1)
                {
                    sMessage = String.Format(@"未来年月日:                    {0}", dinfo);
                    log.Info(sMessage);
                    continue;
                }

                // フォルダ存在チェック
                if (!Directory.Exists(Path.Combine(Config.DivImageRoot, businessId, String.Concat(sImageCaptureDate, sImageCaptureNum))))
                {
                    sMessage = String.Format(@"ディレクトリが存在しません:                    {0}"
                                            , Path.Combine(Config.DivImageRoot, businessId, String.Concat(sImageCaptureDate, sImageCaptureNum)));
                    log.Info(sMessage);
                    continue;
                }
                ListEndFileProcessed.Add(dinfo);

                sMessage = String.Format("処理対象フォルダ:            {0}", dinfo);
                log.Info(sMessage);

                var dt_M_CODE_DEFINE = dao.SELECT_M_BUSINESS(businessId);

                // エントリ単位分割呼出し
                iCount += DllCreateEntryUnit.DllCreateEntryUnit.BL_Main(dt_M_CODE_DEFINE.Rows[0]["BUSINESS_ID"].ToString()
                                                                       , dt_M_CODE_DEFINE.Rows[0]["TKSK_CD"].ToString()
                                                                       , dt_M_CODE_DEFINE.Rows[0]["HNM_CD"].ToString()
                                                                       , sImageCaptureDate
                                                                       , sImageCaptureNum
                                                                       , GVal.UserId
                                                                       , "2"
                                                                       , Consts.Flag.ON);
            }

            // 処理済Endファイルをバックアップ
            Utils.BackUp(ListEndFileProcessed.ToArray(), Config.END_FILE_DIRECTORY, true);

            log.Info("エントリ分割イメージ件数：{0}", iCount.ToString("#,0件"));
        }
#endregion

        #region 通常
        /// <summary>
        /// 通常
        /// </summary>
        private static void Normal()
        {
            var iCount = 0;

            // エントリ単位分割済フォルダ取得
            var dtT_DIV_ENTRY_UNIT_PATH = dao.SELECT_T_DIV_ENTRY_UNIT_PATH();
            var ListEnd = new List<string>();
            foreach (var drT_DIV_ENTRY_UNIT_PATH in dtT_DIV_ENTRY_UNIT_PATH.AsEnumerable())
            {
                ListEnd.Add(drT_DIV_ENTRY_UNIT_PATH["IMAGE_CAPTURE_PATH"].ToString());
            }

            // エントリ連携フォルダ取得
            var di = new DirectoryInfo(Config.DivImageRoot);
            var directries = di.GetDirectories();
            var ListAll = new List<string>();
            foreach (var dinfo in directries)
            {
                ListAll.Add(dinfo.FullName.Split('\\')[dinfo.FullName.Split('\\').Length - 1]);
            }

            // エントリ連携フォルダにあって、エントリ単位分割済フォルダに存在しないフォルダをリスト化
            var ListTarget = new List<string>();
            ListAll.ForEach(a =>
            {
                if (!ListEnd.Contains(a))
                {
                    ListTarget.Add(a);
                }
            });

            ListTarget.Sort();
            foreach (var dinfo in ListTarget)
            {
                // 処理済
                if (ListEnd.Contains(dinfo))
                {
                    sMessage = String.Format("エントリ単位分割処理済:      {0}", dinfo);
                    log.Info(sMessage);
                    continue;
                }

                // 数値？
                if (!Utils.IsNumeric(dinfo))
                {
                    sMessage = String.Format("フォルダ名が数値以外:        {0}", dinfo);
                    log.Info(sMessage);
                    continue;
                }

                // 連携日+回数　10桁？
                if (dinfo.Length != 10)
                {
                    sMessage = String.Format("フォルダ名の長さ10バイト以外:{0}", dinfo);
                    log.Info(sMessage);
                    continue;
                }

                // 連携回数が"00"以外
                if ("00".Equals(dinfo.Substring(8, 2)))
                {
                    sMessage = String.Format(@"連携回数が""00"":              {0}", dinfo);
                    log.Info(sMessage);
                    continue;
                }

                // 未来日は対象外
                if (dinfo.Substring(0, 8).CompareTo(sSysDate) == 1)
                {
                    sMessage = String.Format(@"未来年月日:                    {0}", dinfo);
                    log.Info(sMessage);
                    continue;
                }

                var sImageCaptureDate = dinfo.Substring(0, 8);
                var sImageCaptureNum = dinfo.Substring(8, 2);

                // コピー中チェック
                #region DWFC待ち
                if (Consts.Flag.ON.Equals(Config.UseDWFC))
                {
                    var iTimes = 0;
                    var wTime = 10;
                    while (iTimes <= 10)
                    {
                        iTimes++;
                        int iFileCountOld = Directory.GetFiles(dinfo, Config.ImageFileExtension, SearchOption.AllDirectories).Length;
                        System.Threading.Thread.Sleep(wTime * 1000);
                        int iFileCountNew = Directory.GetFiles(dinfo, Config.ImageFileExtension, SearchOption.AllDirectories).Length;
                        if (iFileCountNew == iFileCountOld)
                            break;

                        sMessage = String.Format("連携処理中です。{0}秒間待機します。", wTime);
                        log.Info(sMessage);
                    }
                    if (iTimes > 10)
                        throw new Exception(String.Format("エントリ対象イメージ連携処理中です。"));
                }
                #endregion

                sMessage = String.Format("処理対象フォルダ:            {0}", dinfo);
                log.Info(sMessage);

                // エントリ単位分割呼出し
                iCount += DllCreateEntryUnit.DllCreateEntryUnit.BL_Main(Config.UserId
                                                                       , Config.TokuisakiCode
                                                                       , Config.HinmeiCode
                                                                       , sImageCaptureDate
                                                                       , sImageCaptureNum
                                                                       , GVal.UserId
                                                                       , "2"
                                                                       , Consts.Flag.ON);
            }
            log.Info("エントリ分割イメージ件数：{0}", iCount.ToString("#,0件"));
        }
        #endregion
    }
}