using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using Common;
using Dao;
using NLog;

namespace AfterImport
{
    public class AfterImport
    {
        #region 変数
        /// <summary>
        /// log
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// データアクセス
        /// </summary>
        private static DaoEntry dao = new DaoEntry();
        #endregion

        #region メイン処理
        /// <summary>
        /// メイン処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int BL_Main(string date, string times)
        {
            //Config.GetConfig(ConfigurationManager.AppSettings["config"]);
            //log.Info("連携年月日：{0}、連携回数：{1}", date, times);
            //var dtD_IMAGE_INFO = dao.SELECT_D_IMAGE_INFO(date, times);
            //foreach (DataRow drD_IMAGE_INFO in dtD_IMAGE_INFO.Rows)
            //{
            //    var dtT_NARA_PRINT_DATA = dao.SELECT_T_NARA_PRINT_DATA(Path.GetFileNameWithoutExtension(drD_IMAGE_INFO["IMAGE_PATH"].ToString()).Split('_')[0]);
            //    if (dtT_NARA_PRINT_DATA.Rows.Count == 1)
            //    {
            //        // エントリの項目№と印字データの値をセット
            //        var EntryItems = new Dictionary<int, string>();
            //        EntryItems.Add(02, dtT_NARA_PRINT_DATA.Rows[0]["APPLICATION_NO"].ToString());   // 申込書№

            //        #region 依頼主
            //        EntryItems.Add(03, dtT_NARA_PRINT_DATA.Rows[0]["CLIENT_POSTAL_CODE"].ToString()); // 郵便番号
            //        EntryItems.Add(04, dtT_NARA_PRINT_DATA.Rows[0]["CLIENT_ADDRESS_1"].ToString());   // 住所１
            //        EntryItems.Add(05, dtT_NARA_PRINT_DATA.Rows[0]["CLIENT_ADDRESS_2"].ToString());   // 住所２
            //        EntryItems.Add(06, dtT_NARA_PRINT_DATA.Rows[0]["CLIENT_NAME_KANA"].ToString());   // シメイ
            //        EntryItems.Add(07, dtT_NARA_PRINT_DATA.Rows[0]["CLIENT_NAME_KANJI"].ToString());  // 氏名
            //        EntryItems.Add(08, dtT_NARA_PRINT_DATA.Rows[0]["CLIENT_TEL_NO"].ToString());      // 電話番号
            //        #endregion

            //        #region 届け先１
            //        EntryItems.Add(17, dtT_NARA_PRINT_DATA.Rows[0]["DIST2_POSTAL_CODE"].ToString());  // 郵便番号
            //        EntryItems.Add(18, dtT_NARA_PRINT_DATA.Rows[0]["DIST2_ADDRESS_1"].ToString());    // 住所１
            //        EntryItems.Add(19, dtT_NARA_PRINT_DATA.Rows[0]["DIST2_ADDRESS_2"].ToString());    // 住所２
            //        EntryItems.Add(20, dtT_NARA_PRINT_DATA.Rows[0]["DIST2_NAME_KANA"].ToString());    // シメイ
            //        EntryItems.Add(21, dtT_NARA_PRINT_DATA.Rows[0]["DIST2_NAME_KANJI"].ToString());   // 氏名
            //        EntryItems.Add(22, dtT_NARA_PRINT_DATA.Rows[0]["DIST2_TEL_NO"].ToString());       // 電話番号
            //        EntryItems.Add(25, dtT_NARA_PRINT_DATA.Rows[0]["TRACKING_NO_2"].ToString());      // 受注番号
            //        #endregion

            //        #region 届け先２
            //        EntryItems.Add(42, dtT_NARA_PRINT_DATA.Rows[0]["DIST3_POSTAL_CODE"].ToString());  // 郵便番号
            //        EntryItems.Add(43, dtT_NARA_PRINT_DATA.Rows[0]["DIST3_ADDRESS_1"].ToString());    // 住所１
            //        EntryItems.Add(44, dtT_NARA_PRINT_DATA.Rows[0]["DIST3_ADDRESS_2"].ToString());    // 住所２
            //        EntryItems.Add(45, dtT_NARA_PRINT_DATA.Rows[0]["DIST3_NAME_KANA"].ToString());    // シメイ
            //        EntryItems.Add(46, dtT_NARA_PRINT_DATA.Rows[0]["DIST3_NAME_KANJI"].ToString());   // 氏名
            //        EntryItems.Add(47, dtT_NARA_PRINT_DATA.Rows[0]["DIST3_TEL_NO"].ToString());       // 電話番号
            //        EntryItems.Add(50, dtT_NARA_PRINT_DATA.Rows[0]["TRACKING_NO_3"].ToString());      // 受注番号
            //        #endregion

            //        #region 届け先３
            //        EntryItems.Add(68, dtT_NARA_PRINT_DATA.Rows[0]["DIST4_POSTAL_CODE"].ToString());  // 郵便番号
            //        EntryItems.Add(69, dtT_NARA_PRINT_DATA.Rows[0]["DIST4_ADDRESS_1"].ToString());    // 住所１
            //        EntryItems.Add(70, dtT_NARA_PRINT_DATA.Rows[0]["DIST4_ADDRESS_2"].ToString());    // 住所２
            //        EntryItems.Add(71, dtT_NARA_PRINT_DATA.Rows[0]["DIST4_NAME_KANA"].ToString());    // シメイ
            //        EntryItems.Add(72, dtT_NARA_PRINT_DATA.Rows[0]["DIST4_NAME_KANJI"].ToString());   // 氏名
            //        EntryItems.Add(73, dtT_NARA_PRINT_DATA.Rows[0]["DIST4_TEL_NO"].ToString());       // 電話番号
            //        EntryItems.Add(76, dtT_NARA_PRINT_DATA.Rows[0]["TRACKING_NO_4"].ToString());      // 受注番号
            //        #endregion

            //        // 更新
            //        dao.UPDATE_D_ENTRY(drD_IMAGE_INFO["ENTRY_UNIT_ID"].ToString()
            //                          , int.Parse(drD_IMAGE_INFO["IMAGE_SEQ"].ToString())
            //                          , EntryItems);
            //    }
            //}
            //log.Info("更新件数：{0}", dtD_IMAGE_INFO.Rows.Count.ToString("#,0"));
            return (int)Consts.RetCode.OK;
        }
        #endregion
    }
}
