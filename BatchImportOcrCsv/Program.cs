﻿using Common;
using NLog;
using System;
using System.Configuration;

namespace BatchImportOcrCsv
{
    /// <summary>
    /// OCR連携結果CSV取込み
    /// </summary>
    class Program
    {
        #region 変数

        /// <summary>
        /// log
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// stopwatch
        /// </summary>
        private static System.Diagnostics.Stopwatch sw = null;

        #endregion

        /// <summary>
        /// エントリ ポイント
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static int Main(string[] args)
        {
            string sMessage = string.Empty;
            try
            {
                // タイトルセット
                Console.Title = String.Format("{0} [Ver.{1}]", Utils.GetAssemblyName(), Utils.GetVersion());

                // stopwatch start
                sw = System.Diagnostics.Stopwatch.StartNew();

                sMessage = String.Format("開始:[Ver.{0}]", Utils.GetVersion());
                log.Info(sMessage); Console.WriteLine(sMessage);

                // Config Load
                Config.GetConfig(ConfigurationManager.AppSettings["config"]);
                log.Info("接続先DB:{0}",Config.DSN);

                // グローバル変数クリア
                GVal.Clear();

                // ユーザID設定
                GVal.UserId = Utils.GetAssemblyName();

                // BusinessLogic呼出し
                Environment.ExitCode = BlBatchImportOcrCsv.BL_Main(args);
                return Environment.ExitCode;
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0}\n{1}", ex.Message, ex.StackTrace));
                Console.WriteLine(String.Format("{0}\n{1}", ex.Message, ex.StackTrace));
#if DEBUG
                Console.ReadKey();
#endif
                Environment.ExitCode = (int)Consts.RetCode.ABEND;
                return Environment.ExitCode;
            }
            finally
            {
                // stopwatch stop
                sw.Stop();
                sMessage = String.Format("終了:[復帰値:{0}][経過時間:{1}]", Environment.ExitCode, sw.Elapsed);
                log.Info(sMessage); Console.WriteLine(sMessage);
#if DEBUG
                if (Environment.ExitCode == (int)Consts.RetCode.OK)
                {
                    Console.ReadKey();
                }
#endif
            }
        }
    }
}
