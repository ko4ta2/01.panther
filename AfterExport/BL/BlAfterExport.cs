using Common;
using Microsoft.VisualBasic.FileIO;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace AfterExport
{
    /// <summary>
    /// 
    /// </summary>
    public static class AfterExport
    {
        #region 変数
        /// <summary>
        /// log
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        #endregion

        #region メイン処理
        /// <summary>
        /// メイン処理
        /// </summary>
        /// <returns></returns>
        public static int BL_Main()
        {
            Config.GetConfig(ConfigurationManager.AppSettings["config"]);

            ////int iWriteCount = 0;
            var ListEntryFile = new List<string>();
            var ListEntryItems = new List<string[]>();

            // 出力フォルダ内のファイル取得
            var sListSrcFiles = Directory.GetFiles(Config.ExportFolder, String.Format("{0}_EntryData_*.tsv", Config.UserId), System.IO.SearchOption.TopDirectoryOnly);
            foreach (string sSrcFile in sListSrcFiles)
            {
                log.Info("処理対象ファイル：{0}", sSrcFile);
                ListEntryFile.Add(sSrcFile);

                // 帳票ID取得
                var sDocId = Path.GetFileName(sSrcFile).Split('_')[2];

                // エントリテキスト読込み
                string[] items = null;
                //List<string> items2;
                using (var ps = new TextFieldParser(sSrcFile, Consts.EncShift_JIS))
                {
                    ps.TextFieldType = FieldType.Delimited;
                    ps.SetDelimiters("\t");
                    while (!ps.EndOfData)
                    {
                        items = ps.ReadFields();
                        // 明細行のみ対象
                        if (!Consts.EntryResultRecordType.Item.Equals(items[0]))
                            continue;

                        // 明細レコードの１項目目に帳票IDを設定
                        items[0] = sDocId;

                        // Listに追加
                        ListEntryItems.Add(items);
                    }
                }
            }

            var sss = new List<string>();

            ListEntryItems.ForEach(_ =>
            {
                var st = new string[10];
                st[0] = _[2];                                                   // 氏名
                st[1] = _[1];                                                   // カナ氏名
                st[2] = _[7].PadRight(4, Config.ReadNotCharNarrowOutput[0]);    // 金融機関番号
                st[3] = _[6].PadRight(2, Config.ReadNotCharNarrowOutput[0]);    // 県コード
                st[4] = _[3].PadRight(8, Config.ReadNotCharNarrowOutput[0]);    // 生年月日
                st[5] = _[8];                                                   // 顧客番号
                st[6] = _[9].PadRight(4, Config.ReadNotCharNarrowOutput[0]);    // 投信部店コード
                st[7] = _[10];                                                  // 投信口座番号
                // カード種別
                if ("1".Equals(_[4]))
                    st[8] = "0000";
                else if ("2".Equals(_[4]))
                    st[8] = "9999";
                st[9] = _[5].PadRight(16,Config.ReadNotCharNarrowOutput[0]);    // カード契約番号
                sss.Add(String.Join(",", st));
            });

            var filePath = Path.Combine(Config.DeliveryFolder, $"tohgori_entry_lst_{DateTime.Now.ToString("yyyyMMdd")}_1.csv");
            using (StreamWriter sw = new StreamWriter(filePath, false, Consts.EncShift_JIS))
            {
                sss.ForEach(s => sw.WriteLine(s));
            }

            // バックアップ
            Utils.BackUp(ListEntryFile.ToArray(), Config.ExportFolder, true);

            // 正常終了
            return (int)Consts.RetCode.OK;
        }
        #endregion
    }
}