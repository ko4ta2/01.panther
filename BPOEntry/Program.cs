using BPOEntry.Login;
using BPOEntry.Tables;
using Common;
using NLog;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace BPOEntry
{
    /// <summary>
    /// 
    /// </summary>
    static class Program
    {
        /// <summary>
        /// ログインユーザー情報
        /// </summary>
        public static M_USER LoginUser { get; set; }

        /// <summary>
        /// stopwatch
        /// </summary>
        private static System.Diagnostics.Stopwatch sw = null;

        /// <summary>
        /// 
        /// </summary>
        private static System.Threading.Mutex mutex;

        /// <summary>
        /// log
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            string sMessage = string.Empty;
            try
            {
                // stopwatch start
                sw = System.Diagnostics.Stopwatch.StartNew();

                sMessage = String.Format("開始:[Ver.{0}]", Utils.GetVersion());
                log.Info(sMessage); Console.WriteLine(sMessage);

                // Config Load
                Config.GetConfig(ConfigurationManager.AppSettings["config"]);

                //Mutexクラスの作成
                mutex = new System.Threading.Mutex(false, Utils.GetAssemblyName());
                //ミューテックスの所有権を要求する
                if (!mutex.WaitOne(0, false))
                {
                    //すでに起動していると判断して終了
                    sMessage = "多重起動は出来ません。";
                    MessageBox.Show(sMessage, Config.BusinessName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    log.Error(sMessage); Console.WriteLine(sMessage);
                    return;
                }

                // 既定の初期化処理
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmLogin());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Config.BusinessName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // stopwatch stop
                sw.Stop();
                sMessage = String.Format("終了:[復帰値:{0}][経過時間:{1}]", Environment.ExitCode, sw.Elapsed);
                log.Info(sMessage); Console.WriteLine(sMessage);
            }
        }
    }
}
