using BPOEntry.Tables;
using Common;
using Dao;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace DllExportEntryUnit
{
    public class DllExportEntryUnit
    {
        /// <summary>
        /// log
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// データアクセス
        /// </summary>
        private static DaoExportEntryUnit dao = new DaoExportEntryUnit();

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="deu"></param>
        /// <returns></returns>
        public static string BL_Main(string ENTRY_UNIT_ID,string BATCH_EXPORT_FLAG, string USER_ID/*,D_ENTRY_UNIT deux*/)
        {
            Config.GetConfig(ConfigurationManager.AppSettings["config"]);

            var ee = ENTRY_UNIT_ID.Split('_');

            var deu = new D_ENTRY_UNIT($"{ee[0]}_{ee[1]}_{ee[2]}_{ee[3]}_XXXXXXXX_XXX")
            {
                BATCH_EXPORT_FLAG = BATCH_EXPORT_FLAG,
                EXPORT_METHOD = Config.ExportUnit,
                USER_ID = USER_ID
            };

            var sMessage = $"USER_ID:{deu.USER_ID} 得意先コード:{deu.TKSK_CD} 品名コード:{deu.HNM_CD} 連携年月日:{deu.IMAGE_CAPTURE_DATE} 連携回数:{deu.IMAGE_CAPTURE_NUM}";
            Console.WriteLine(sMessage); log.Info(sMessage);

            var sw1 = new System.Diagnostics.Stopwatch();

            // トータル件数
            var AllCount = 0;

            // 帳票毎件数
            var DocIdCount = 0;

            try
            {
                var sCountList = new List<string>();
                var CreateDateTime = DateTime.Now;
                var RootExportFolder = string.Empty;
                var FilePath = string.Empty;
                var PCIDSS_FLAG = string.Empty;

                sw1.Start();

                var dtM_DOC = dao.SELECT_M_DOC(deu);
                foreach (var drM_Doc in dtM_DOC.AsEnumerable())
                {
                    PCIDSS_FLAG = drM_Doc["PCIDSS_FLAG"].ToString().Length != 0 ? drM_Doc["PCIDSS_FLAG"].ToString() : Consts.Flag.OFF;

                    deu.DOC_ID = drM_Doc["DOC_ID"].ToString();

                    // エントリ結果取得
                    var dtD_ENTRY = dao.SELECT_D_ENTRY(deu);
                    if (Consts.Flag.OFF.Equals(Config.CreateZeroDataFlag) && dtD_ENTRY.Rows.Count == 0)
                        continue;

                    if (Consts.Flag.ON.Equals(PCIDSS_FLAG))
                    {
                        RootExportFolder = Config.PCIDSSExportFolder;
                    }
                    else
                    {
                        RootExportFolder = Config.ExportFolder;
                    }

                    FilePath = Path.Combine(RootExportFolder, $"{deu.USER_ID}_EntryData_{deu.DOC_ID.Substring(0, 4)}_{deu.DOC_ID.Substring(4, 4)}_{CreateDateTime:yyyyMMddHHmmss}.tsv");

                    // 文字コード設定
                    var enc = Consts.EncShift_JIS;
                    if (Consts.Encode.UTF8.Equals(Config.Encode))
                    {
                        enc = Consts.EncUTF8N;
                    }

                    if (Consts.Flag.ON.Equals(PCIDSS_FLAG))
                    {
                        enc = Consts.EncUTF8N;
                    }

                    using (var sw = new StreamWriter(FilePath, false, enc))
                    {
                        // ヘッダー行
                        if (!Consts.Flag.ON.Equals(PCIDSS_FLAG))
                        {
                            var head = new List<string>();
                            head.Add(Consts.EntryResultRecordType.Head);    // レコード種別
                            head.Add(CreateDateTime.ToString("yyyyMMdd"));  // データ作成日付
                            head.Add(CreateDateTime.ToString("HHmmss"));    // データ作成時間
                            head.Add(deu.TKSK_CD);                          // 得意先コード
                            head.Add(deu.HNM_CD);                           // 品名コード
                            sw.WriteLine(string.Join("\t", head));
                        }

                        // 明細行
                        var detail = new List<string>();

                        // 同じページ毎にグループ化
                        var pages = from DataRow drD_ENTRY in dtD_ENTRY.Rows
                                    group drD_ENTRY by new
                                    {
                                        ENTRY_UNIT_ID = drD_ENTRY["ENTRY_UNIT_ID"].ToString(),
                                        IMAGE_SEQ = drD_ENTRY["IMAGE_SEQ"].ToString(),
                                        IMAGE_PATH = drD_ENTRY["IMAGE_PATH"].ToString()
                                    };

                        DocIdCount = 0;
                        foreach (var page in pages)
                        {
                            // レコード区分
                            if (!Consts.Flag.ON.Equals(PCIDSS_FLAG))
                                detail.Add(Consts.EntryResultRecordType.Item);
                            else
                                detail.Add(Path.GetFileName((string)page.Key.IMAGE_PATH));

                            // 各項目をリスト化する
                            foreach (var item in page)
                            {
                                //                                string sItemId = item["ITEM_ID"].ToString();
                                //string sValue = ;
                                detail.Add(item["VALUE"].ToString().Replace(Config.ReadNotCharNarrowInput, Config.ReadNotCharNarrowOutput).Replace(Config.ReadNotCharWideInput, Config.ReadNotCharWideOutput));
                            }

                            // 末尾に画像パスを追加
                            if (!Consts.Flag.ON.Equals(PCIDSS_FLAG))
                            {
                                detail.Add((string)page.Key.IMAGE_PATH);
                            }

                            sw.WriteLine(String.Join("\t", detail));
                            detail.Clear();
                            DocIdCount++;
                            AllCount++;
                        }

                        if (sCountList.Count == 0)
                        {
                            sCountList.Add(Environment.NewLine + "（内訳）");
                        }
                        sCountList.Add("  " + Utils.LeftB(drM_Doc["DOC_NAME"].ToString().PadRight(60, ' '), 60) + DocIdCount.ToString("#,0 件").PadLeft(9));

                        // トレーラ行
                        if (!Consts.Flag.ON.Equals(PCIDSS_FLAG))
                        {
                            var trail = new List<string>();
                            trail.Add(Consts.EntryResultRecordType.End); // レコード種別
                            trail.Add(DocIdCount.ToString("d10"));       // レコード件数
                            trail.Add("DNP");                            // 会社区分
                            sw.WriteLine(string.Join("\t", trail));
                        }
                    }

                    // エントリ単位毎にグループ化
                    var EntryUnits = from DataRow drD_ENTRY in dtD_ENTRY.Rows
                                     group drD_ENTRY by new
                                     {
                                         ENTRY_UNIT_ID = drD_ENTRY["ENTRY_UNIT_ID"].ToString()
                                     };
                    // ステータスをテキスト出力済に更新
                    foreach (var EntryUnit in EntryUnits)
                    {
                        deu.ENTRY_UNIT_ID = EntryUnit.Key.ENTRY_UNIT_ID;
                        if (dao.UPDATE_D_ENTRY_UNIT(deu) != 1)
                        {
                            throw new ApplicationException($"D_ENTRY_UNIT の更新で不整合が発生しました。\nENTRY_UNIT_ID:{EntryUnit.Key.ENTRY_UNIT_ID}");
                        }
                    }
                }

                // エントリデータ出力後処理
                log.Info($"エントリデータ出力後処理：{Config.ExecAfterExportFlag}");
                if (Consts.Flag.ON.Equals(Config.ExecAfterExportFlag))
                {
                    AfterExport.AfterExport.BL_Main();
                }

                dao.CommitTrans();
                sw1.Stop();
                log.Info("経過時間：{0}", sw1.Elapsed);

                var sb = new StringBuilder();
                log.Info("↓↓↓↓↓↓↓↓↓↓　件数内訳　↓↓↓↓↓↓↓↓↓↓");
                foreach (var sCount in sCountList)
                    sb.AppendLine(sCount);
                log.Info("↑↑↑↑↑↑↑↑↑↑　件数内訳　↑↑↑↑↑↑↑↑↑↑");

                sMessage = "終了しました。" + Environment.NewLine + String.Format("出力件数:{0}", AllCount.ToString("#,0 件") + Environment.NewLine + sb.ToString());
                Console.WriteLine(sMessage); log.Info(sMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
            finally
            {
            }

            // 件数返却
            return sMessage;
        }
    }
}
