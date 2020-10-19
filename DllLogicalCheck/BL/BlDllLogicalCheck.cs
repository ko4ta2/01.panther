using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Common;
using Dao;
using NLog;

namespace DllLogicalCheck
{
    /// <summary>
    /// エントリ単位分割後処理
    /// </summary>
    public static class DllLogicalCheck
    {
        #region 変数
        /// <summary>
        /// log
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// データアクセス
        /// </summary>
        private static DaoDllLogicalCheck dao = new DaoDllLogicalCheck();
        #endregion

        #region メイン処理
        /// <summary>
        /// メイン処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int BL_Main(List<string> sListTargetUnit)
        {
            Config.GetConfig(ConfigurationManager.AppSettings["config"]);

            foreach (var sTargetUnit in sListTargetUnit)
            {
                var dtTargetImage = dao.SELECT_D_ENTRY(sTargetUnit);
                foreach (DataRow drTargetImage in dtTargetImage.Rows)
                    if (dao.UPDATE_D_IMAGE_INFO(drTargetImage) != 1)
                        throw new System.Exception("イメージ情報の更新件数が想定外");
            }

            return (int)Consts.RetCode.OK;
        }
        #endregion
    }
}
