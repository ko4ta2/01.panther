using Common;
using Dao;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;

namespace BatchCompareEntryUnit
{
    /// <summary>
    /// コンペア
    /// </summary>
    class BlBatchCompareEntryUnit
    {
        #region 変数
        /// <summary>
        /// NLog
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// データアクセス
        /// </summary>
        private static DaoBatchCompareEntryUnit dao = new DaoBatchCompareEntryUnit();

        private static string Message = null;
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
                log.Info("BatchCompareEntryUnit:start");

                var UpdateCount = 0;
                // ＤＢオープン
                dao.Open(Config.DSN);

                // ＤＢトランザクション開始
                dao.BeginTrans();

                var ListKey = new List<string>();
                if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
                {
                    foreach (var dr in dao.SELECT_M_BUSINESS().AsEnumerable())
                    {
                        ListKey.Add(String.Join("_", dr["BUSINESS_ID"].ToString(), dr["tksk_cd"].ToString(), dr["hnm_cd"].ToString()));
                    }
                }
                else
                {
                    ListKey.Add(String.Join("_", Config.UserId, Config.TokuisakiCode, Config.HinmeiCode));
                }

                ListKey.ForEach(key =>
                {
                    var tksk_cd = key.Split('_')[1];
                    var hnm_cd = key.Split('_')[2];

                    // コンペア対象取得
                    var dtD_ENTRY_UNIT = dao.SELECT_D_ENTRY_UNIT(tksk_cd, hnm_cd, Consts.EntryUnitStatus.COMPARE_ING);

                    Message = String.Format("コンペア対象件数：{0}件", dtD_ENTRY_UNIT.Rows.Count.ToString("#,0").PadLeft(7));
                    log.Info(Message); Console.WriteLine(Message);

                    foreach (var drD_ENTRY_UNIT in dtD_ENTRY_UNIT.AsEnumerable())
                    {
                        log.Info("コンペア対象 ENTRY_UNIT_ID:{0}", drD_ENTRY_UNIT["ENTRY_UNIT_ID"].ToString());

                        // コンペア呼び出し
                        DllCompareEntryUnit.DllCompareEntryUnit.BL_Main(drD_ENTRY_UNIT["ENTRY_UNIT_ID"].ToString(), GVal.UserId);

                        // カウントアップ
                        UpdateCount++;
                        if (UpdateCount % 1000 == 0)
                        {
                            Message = String.Format("コンペア件数：{0}件", UpdateCount.ToString("#,0").PadLeft(11));
                            log.Info(Message); Console.WriteLine(Message);
                        }
                    }
                });

                if (UpdateCount % 1000 != 0)
                {
                    Message = String.Format("コンペア件数：{0}件", UpdateCount.ToString("#,0").PadLeft(11));
                    log.Info(Message); Console.WriteLine(Message);
                }

                // ＤＢトランザクションコミット
                dao.CommitTrans();

                // ok
                return (int)Consts.RetCode.OK;
            }
            catch
            {
                // ＤＢトランザクションロールバック
                dao.RollbackTrans();
                throw;
            }
            finally
            {
                // ＤＢクローズ
                dao.Close();
                log.Info("BatchCompareEntryUnit:end");
            }
        }
        #endregion
    }
}