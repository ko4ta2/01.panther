using BPOEntry.Tables;
using Common;
using Dao;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;

namespace BatchExportEntryUnit
{
    /// <summary>
    /// エントリ結果出力
    /// </summary>
    class BlBatchExportEntryUnit
    {
        #region 変数
        /// <summary>
        /// NLog
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// データアクセス
        /// </summary>
        private static DaoBatchExportEntryUnit dao = new DaoBatchExportEntryUnit();

        //private static string sMessage = null;

        /// <summary>
        /// システム年月日
        /// </summary>
        //private static string SysDate = System.DateTime.Now.ToString("yyyyMMdd");
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
                log.Info("BatchExportEntryUnit:start");

                // Open
                dao.Open(Config.DSN);

                // Begin
                dao.BeginTrans();

                var ListKey = new List<string>();
                if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
                {
                    foreach(var dr in dao.SELECT_M_BUSINESS().AsEnumerable())
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
                    // エントリユニットクラス作成
                    var tksk_cd = key.Split('_')[1];
                    var hnm_cd = key.Split('_')[2];
                    var deu = new D_ENTRY_UNIT($"{tksk_cd}_{hnm_cd}_{Consts.sSysDate}_XX_XXXXXXXX_XXX")
                    {
                        BATCH_EXPORT_FLAG = Consts.Flag.ON,
                        EXPORT_METHOD = Config.ExportUnit,
                        USER_ID = key.Split('_')[0]

                    };
                    var Message = DllExportEntryUnit.DllExportEntryUnit.BL_Main(deu.ENTRY_UNIT_ID,deu.BATCH_EXPORT_FLAG,deu.USER_ID);
                });

                //foreach (var key in ListKey)
                //{
                //    // エントリユニットクラス作成
                //    var tksk_cd = key.Split('_')[1];
                //    var hnm_cd = key.Split('_')[2];
                //    var deu = new D_ENTRY_UNIT($"{tksk_cd}_{hnm_cd}_{Consts.sSysDate}_XX_XXXXXXXX_XXX")
                //    {
                //        BATCH_EXPORT_FLAG = Consts.Flag.ON,
                //        EXPORT_METHOD = Config.ExportUnit,
                //        USER_ID = key.Split('_')[0]
                //    };
                //    var Message = DllExportEntryUnit.DllExportEntryUnit.BL_Main(deu);
                //}

                // Commit
                dao.CommitTrans();

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
                log.Info("BatchExportEntryUnit:end");
            }
        }
        #endregion
    }
}