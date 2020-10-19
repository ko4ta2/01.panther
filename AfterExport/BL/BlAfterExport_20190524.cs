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
    /// エントリデータ出力後処理
    /// 損保ジャパン日本興亜　ＰｏＣ案件
    /// </summary>
    public static class AfterExport
    {
        #region 変数
        /// <summary>
        /// log
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        private static string DOC_ID;

        private static int ITEM_COUNT;

        private const string CONST_CHOFUKU = "?";

        private const string separate_string = "\t";
        #endregion

        #region メイン処理
        /// <summary>
        /// メイン処理
        /// </summary>
        /// <returns></returns>
        public static int BL_Main()
        {
            Config.GetConfig(ConfigurationManager.AppSettings["config"]);

            //int iWriteCount = 0;
            var ListEntryItems = new List<string[]>();
            var ListEntryFile = new List<string>();

            // 出力フォルダ内のファイル取得
            var sListSrcFiles = System.IO.Directory.GetFiles(Config.ExportFolder, String.Format("{0}_EntryData_*.tsv", Config.UserId), System.IO.SearchOption.TopDirectoryOnly);
            foreach (string sSrcFile in sListSrcFiles)
            {
                log.Info("処理対象ファイル：{0}", sSrcFile);
                ListEntryFile.Add(sSrcFile);

                // 帳票ID取得
                var sDocId = Path.GetFileName(sSrcFile).Split('_')[2];

                // エントリテキスト読込み
                string[] items = null;
                //List<string> items2;
                using (TextFieldParser parser = new TextFieldParser(sSrcFile, Consts.EncShift_JIS))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters("\t");
                    while (!parser.EndOfData)
                    {
                        items = parser.ReadFields();
                        // 明細行のみ対象
                        if (!"02".Equals(items[0]))
                            continue;

                        // 明細レコードの１項目目に帳票IDを設定
                        items[0] = sDocId;

                        // Listに追加
                        ListEntryItems.Add(items);
                    }
                }
            }

            var ListDocId = new List<string>();
            ListDocId.Add("0101\t65366_{0}.tsv");
            ListDocId.Add("0102\t64025_{0}.tsv");
            ListDocId.Add("0103\t00019_{0}.tsv");
            ListDocId.Add("0104\t00027_{0}.tsv");

            ListDocId.Add("0201\t54399_{0}.tsv");
            ListDocId.Add("0202\t61964_{0}.tsv");
            ListDocId.Add("0203\t54267_{0}.tsv");
            ListDocId.Add("0204\t64009_{0}.tsv");
            ListDocId.Add("0205\t61921_{0}.tsv");
            ListDocId.Add("0206\t65374_{0}.tsv");

            var iWriteCount = 0;
            foreach (var DocId in ListDocId)
            {
                var ListEntry = new List<string>();
                //var ItemRecord = string.Empty;
                var sOutFilePath = System.IO.Path.Combine(Config.DeliveryFolder, String.Format(DocId.Split('\t')[1], Consts.sSysDate));

                foreach (var sEntryItems in ListEntryItems)
                {
                    if (!sEntryItems[0].Equals(DocId.Split('\t')[0]))
                        continue;

                    switch (DocId.Split('\t')[0])
                    {
                        case "0101":
                            //ItemRecord = ;
                            ListEntry.Add(Edit0101(sEntryItems));
                            break;
                        case "0102":
                            //ItemRecord = ;
                            ListEntry.Add(Edit0102(sEntryItems));
                            break;
                        case "0103":
                            //ItemRecord = ;
                            ListEntry.Add(Edit0103(sEntryItems));
                            break;
                        case "0104":
                            //ItemRecord = ;
                            ListEntry.Add(Edit0104(sEntryItems));
                            break;

                        case "0201":
                            //ItemRecord = ;
                            ListEntry.Add(Edit0201(sEntryItems));
                            break;
                        case "0202":
                            //ItemRecord = ;
                            ListEntry.Add(Edit0202(sEntryItems));
                            break;
                        case "0203":
                            //ItemRecord = ;
                            ListEntry.Add(Edit0203(sEntryItems));
                            break;
                        case "0204":
                            //ItemRecord = ;
                            ListEntry.Add(Edit0204(sEntryItems));
                            break;
                        case "0205":
                            //ItemRecord = ;
                            ListEntry.Add(Edit0205(sEntryItems));
                            break;
                        case "0206":
                            //ItemRecord = ;
                            ListEntry.Add(Edit0206(sEntryItems));
                            break;
                    }
                }

                if (ListEntry.Count != 0)
                {
                    using (StreamWriter sw = new StreamWriter(sOutFilePath, false, Consts.EncShift_JIS))
                    {
                        //                        sw.WriteLine(letterHeader);
                        foreach (var line in ListEntry)
                        {
                            sw.WriteLine(line);
                            iWriteCount++;
                        }
                    }
                }
            }

            // エントリ結果ファイルバックアップ
            Utils.BackUp(ListEntryFile, Config.ExportFolder, true);
            log.Info("納品データ出力件数：{0}", iWriteCount.ToString("#,0"));

            // 正常終了
            return (int)Consts.RetCode.OK;
        }
        #endregion

        private static string[] CreateStringArray(int Count)
        {
            var items = new string[Count + 1];
            for (int iIdx = 1; iIdx < items.Length; iIdx++)
                items[iIdx] = string.Empty;
            return items;
        }

        #region 0101
        /// <summary>
        /// 団体契約申込書＜６５３６６－２＞編集
        /// </summary>
        /// <param name="sItems"></param>
        /// <returns></returns>
        private static string Edit0101(string[] sItems)
        {
            DOC_ID = "65366-2";

            var items = new string[201];
            for (int iIdx = 1; iIdx <= items.Length - 1; iIdx++)
                items[iIdx] = string.Empty;

            // 編集用変数
            //var edit = string.Empty;
            var list = new List<string>();
            var Count = 0;

            // 帳票ID取得
            var entry_doc_id = sItems[102];

            // 証券番号・主番
            items[1] = sItems[104];

            // 証券番号・枝番
            items[2] = sItems[105];

            // 帳票番号
            items[3] = sItems[102];

            // 締切区分
            items[4] = sItems[257];

            // 更改方法
            Count = 0;
            if ("1".Equals(sItems[249]))
                Count++;
            if ("2".Equals(sItems[250]))
                Count++;
            if ("3".Equals(sItems[251]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[249]))
                    items[5] = "1";
                if ("2".Equals(sItems[250]))
                    items[5] = "2";
                if ("3".Equals(sItems[251]))
                    items[5] = "3";
            }
            else if (Count == 0)
            {
                // 未選択
                items[5] = string.Empty;
            }
            else
            {
                // 複数選択
                items[5] = CONST_CHOFUKU;//"更改方法_複数選択";
            }

            // 前年証券番号
            items[006] = sItems[67];

            // 領収証番号
            items[007] = sItems[70];

            // 領収日
            items[008] = GetDate(entry_doc_id, sItems, 68);

            // 新規・更改継続区分
            items[009] = sItems[66];

            // 整理番号１
            items[010] = sItems[100];

            // 整理番号２
            items[011] = sItems[101];

            // 特殊処理①
            items[012] = sItems[252];

            // 特殊処理②
            items[013] = sItems[253];

            // 成果分析①
            items[014] = sItems[254];

            // 成果分析②
            items[015] = sItems[255];

            // 引照
            items[016] = sItems[261];

            // 保険証券（要･不要･済）
            Count = 0;
            if ("1".Equals(sItems[81]))
                Count++;
            if ("2".Equals(sItems[82]))
                Count++;
            if ("3".Equals(sItems[83]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[81]))
                    items[017] = "1";
                if ("2".Equals(sItems[82]))
                    items[017] = "2";
                if ("3".Equals(sItems[83]))
                    items[017] = "3";
            }
            else if (Count == 0)
            {
                // 未選択
                items[017] = string.Empty;
            }
            else
            {
                // 複数選択
                items[017] = CONST_CHOFUKU;//"保険証券（要･不要･済）_複数選択";
            }

            // 保険証券 送付区分
            Count = 0;
            if ("1".Equals(sItems[84]))
                Count++;
            if ("2".Equals(sItems[85]))
                Count++;
            if ("3".Equals(sItems[86]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[84]))
                    items[018] = "1";
                if ("2".Equals(sItems[85]))
                    items[018] = "2";
                if ("3".Equals(sItems[86]))
                    items[018] = "3";
            }
            else if (Count == 0)
            {
                // 未選択
                items[018] = string.Empty;
            }
            else
            {
                // 複数選択
                items[018] = CONST_CHOFUKU;// "保険証券 送付区分_複数選択";
            }

            // 保険証券 写枚数
            items[019] = sItems[89];

            // 申込日
            items[020] = GetDate(entry_doc_id, sItems, 1);

            // 確認欄１
            items[021] = sItems[258];

            // 確認欄２
            items[022] = sItems[259];

            // 確認欄３
            items[023] = sItems[260];

            // 申込人住所１
            items[024] = sItems[5];

            // 申込人住所２
            items[025] = sItems[6];

            // 申込人住所３
            items[026] = sItems[7];

            // 電話番号
            items[027] = sItems[4];

            // 申込人氏名・法人名
            items[028] = sItems[14];

            // 申込人氏名・肩書
            items[029] = sItems[15];

            // 申込人氏名・代表者名
            items[030] = sItems[16];

            // 代理店／仲立人
            items[031] = sItems[121];

            // 保険期間  始期日
            items[032] = GetDate(entry_doc_id, sItems, 106);

            // 保険期間始期
            Count = 0;
            if ("1".Equals(sItems[108]))
                Count++;
            if ("2".Equals(sItems[109]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[108]))
                    items[033] = "1";
                if ("2".Equals(sItems[109]))
                    items[033] = "2";
            }
            else if (Count == 0)
            {
                // 未選択
                items[033] = string.Empty;
            }
            else
            {
                // 複数選択
                items[033] = CONST_CHOFUKU;// "保険期間始期_複数選択";
            }

            // 保険期間始期 時刻
            items[034] = sItems[110].PadLeft(2, '0');

            // 保険期間  終期日
            items[035] = GetDate(entry_doc_id, sItems, 111);

            // 保険期間始期
            Count = 0;
            if ("1".Equals(sItems[113]))
                Count++;
            if ("2".Equals(sItems[114]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[113]))
                    items[033] = "1";
                if ("2".Equals(sItems[114]))
                    items[033] = "2";
            }
            else if (Count == 0)
            {
                // 未選択
                items[036] = string.Empty;
            }
            else
            {
                // 複数選択
                items[036] = CONST_CHOFUKU; //"保険期間終期_複数選択";
            }

            // 保険期間始期 時刻
            items[037] = sItems[115].PadLeft(2, '0');

            // 保険期間
            list = new List<string>();
            if (sItems[116].Length != 0
                || sItems[117].Length != 0
                || sItems[118].Length != 0
                || sItems[119].Length != 0)
            {
                list = new List<string>();
                Count = 0;
                list.Add(sItems[116].PadLeft(2, '0'));
                list.Add(sItems[117].PadLeft(3, '0'));
                if ("1".Equals(sItems[118]))
                    Count++;
                if ("2".Equals(sItems[119]))
                    Count++;
                if (Count == 1)
                {
                    if ("1".Equals(sItems[118]))
                        list.Add("M");
                    if ("2".Equals(sItems[119]))
                        list.Add("D");
                }
                else if (Count == 0)
                {
                    list.Add(string.Empty);
                }
                else
                {
                    list.Add(CONST_CHOFUKU);
                }
            }
            items[038] = String.Join(string.Empty, list.ToArray());

            // 郵便番号
            items[039] = sItems[3];

            // 団体構成員の範囲
            list = new List<string>();
            if ("01".Equals(sItems[20]))
                list.Add("01");
            if ("02".Equals(sItems[21]))
                list.Add("02");
            if ("03".Equals(sItems[22]))
                list.Add("03");
            if ("04".Equals(sItems[23]))
                list.Add("04");
            if ("05".Equals(sItems[24]))
                list.Add("05");
            if ("99".Equals(sItems[25]))
                list.Add("99");
            items[040] = String.Join(string.Empty, list.ToArray());

            // 募集人ＩＤ
            items[041] = sItems[71];

            // 補助
            Count = 0;
            if ("1".Equals(sItems[94]))
                Count++;
            if ("2".Equals(sItems[95]))
                Count++;
            if ("9".Equals(sItems[96]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[94]))
                    items[042] = "1";
                if ("2".Equals(sItems[95]))
                    items[042] = "2";
                if ("9".Equals(sItems[96]))
                    items[042] = "9";
            }
            else if (Count == 0)
            {
                // 未選択
                items[042] = string.Empty;
            }
            else
            {
                // 複数選択
                items[042] = CONST_CHOFUKU; //"補助_複数選択";
            }

            // 営業所コード
            items[043] = sItems[91];

            // 契約方式
            Count = 0;
            if ("27".Equals(sItems[26]))
                Count++;
            if ("36".Equals(sItems[27]))
                Count++;
            if (Count == 1)
            {
                if ("27".Equals(sItems[26]))
                    items[044] = "27";
                if ("36".Equals(sItems[27]))
                    items[044] = "36";
            }
            else if (Count == 0)
            {
                // 未選択
                items[044] = string.Empty;
            }
            else
            {
                // 複数選択
                items[044] = CONST_CHOFUKU; //"契約方式_複数選択";
            }

            // 団体類別
            Count = 0;
            if ("1".Equals(sItems[28]))
                Count++;
            if ("2".Equals(sItems[29]))
                Count++;
            if ("3".Equals(sItems[30]))
                Count++;
            if ("4".Equals(sItems[31]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[28]))
                    items[045] = "1";
                if ("2".Equals(sItems[29]))
                    items[045] = "2";
                if ("3".Equals(sItems[30]))
                    items[045] = "3";
                if ("4".Equals(sItems[31]))
                    items[045] = "4";
            }
            else if (Count == 0)
            {
                // 未選択
                items[045] = string.Empty;
            }
            else
            {
                // 複数選択
                items[045] = CONST_CHOFUKU; //"団体種別_複数選択";
            }

            // 一類団体の種類
            Count = 0;
            if ("1".Equals(sItems[32]))
                Count++;
            if ("2".Equals(sItems[33]))
                Count++;
            if ("9".Equals(sItems[34]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[32]))
                    items[046] = "1";
                if ("2".Equals(sItems[33]))
                    items[046] = "2";
                if ("9".Equals(sItems[34]))
                    items[046] = "9";
            }
            else if (Count == 0)
            {
                // 未選択
                items[046] = string.Empty;
            }
            else
            {
                // 複数選択
                items[046] = CONST_CHOFUKU; //"一類団体の種類_複数選択";
            }

            // 保険種類①～⑮
            items[047] = sItems[125];
            items[048] = sItems[128];
            items[049] = sItems[131];
            items[050] = sItems[134];
            items[051] = sItems[137];
            items[052] = sItems[140];
            items[053] = sItems[143];
            items[054] = sItems[146];
            items[055] = sItems[149];
            items[056] = sItems[152];
            items[057] = sItems[155];
            items[058] = sItems[158];
            items[059] = sItems[161];
            items[060] = sItems[164];
            items[061] = sItems[167];

            // 口座振替継続表示
            Count = 0;
            if ("2".Equals(sItems[238]))
                Count++;
            if ("3".Equals(sItems[239]))
                Count++;
            if (Count == 1)
            {
                if ("2".Equals(sItems[238]))
                    items[062] = "2";
                if ("3".Equals(sItems[239]))
                    items[062] = "3";
            }
            else if (Count == 0)
            {
                // 未選択
                items[062] = string.Empty;
            }
            else
            {
                // 複数選択
                items[062] = CONST_CHOFUKU; //"口座振替継続表示_複数選択";
            }

            // 漢字
            items[063] = sItems[103];

            // 計上申請書作成
            items[064] = sItems[63];

            // 加入者カ―ド 要・不要
            Count = 0;
            if ("1".Equals(sItems[35]))
                Count++;
            if ("2".Equals(sItems[36]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[35]))
                    items[065] = "1";
                if ("2".Equals(sItems[36]))
                    items[065] = "2";
            }
            else if (Count == 0)
            {
                // 未選択
                items[065] = string.Empty;
            }
            else
            {
                // 複数選択
                items[065] = CONST_CHOFUKU; //"加入者カ―ド 要・不要_複数選択";
            }

            // 加入者カ―ド 打出し順
            Count = 0;
            if ("1".Equals(sItems[37]))
                Count++;
            if ("2".Equals(sItems[38]))
                Count++;
            if ("3".Equals(sItems[39]))
                Count++;
            if ("4".Equals(sItems[40]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[37]))
                    items[066] = "1";
                if ("2".Equals(sItems[38]))
                    items[066] = "2";
                if ("3".Equals(sItems[39]))
                    items[066] = "3";
                if ("4".Equals(sItems[40]))
                    items[066] = "4";
            }
            else if (Count == 0)
            {
                // 未選択
                items[066] = string.Empty;
            }
            else
            {
                // 複数選択
                items[066] = CONST_CHOFUKU; //"加入者カ―ド 打出し順_複数選択";
            }

            // あて名ラベル
            items[067] = sItems[43];

            // 控除証明書
            Count = 0;
            if ("1".Equals(sItems[41]))
                Count++;
            if ("2".Equals(sItems[42]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[41]))
                    items[068] = "1";
                if ("2".Equals(sItems[42]))
                    items[068] = "2";
            }
            else if (Count == 0)
            {
                // 未選択
                items[068] = string.Empty;
            }
            else
            {
                // 複数選択
                items[068] = CONST_CHOFUKU; //"控除証明書_複数選択";
            }

            // 加入者明細書 要・不要
            Count = 0;
            if ("1".Equals(sItems[44]))
                Count++;
            if ("2".Equals(sItems[45]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[44]))
                    items[069] = "1";
                if ("2".Equals(sItems[45]))
                    items[069] = "2";
            }
            else if (Count == 0)
            {
                // 未選択
                items[069] = string.Empty;
            }
            else
            {
                // 複数選択
                items[069] = CONST_CHOFUKU; //"加入者明細書 要・不要_複数選択";
            }

            // 加入者明細書 打出し順
            Count = 0;
            if ("1".Equals(sItems[46]))
                Count++;
            if ("2".Equals(sItems[47]))
                Count++;
            if ("3".Equals(sItems[48]))
                Count++;
            if ("4".Equals(sItems[49]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[46]))
                    items[070] = "1";
                if ("2".Equals(sItems[47]))
                    items[070] = "2";
                if ("3".Equals(sItems[48]))
                    items[070] = "3";
                if ("4".Equals(sItems[49]))
                    items[070] = "4";
            }
            else if (Count == 0)
            {
                // 未選択
                items[070] = string.Empty;
            }
            else
            {
                // 複数選択
                items[070] = CONST_CHOFUKU; //"加入者明細書 打出し順_複数選択";
            }

            // 加入者明細書所属ｼ - ﾄ替
            items[071] = sItems[50];

            // 集金内訳データ配信 要・不要
            items[072] = sItems[53];

            // ５０音別索引簿 要･不要
            items[073] = sItems[55];

            // "満期ｾｯﾄ作成指示満期処理確認資料送付月"
            items[074] = GetDate(entry_doc_id, sItems, 61, 4);

            // 種目合計保険料（合計保険料1～30）
            items[075] = sItems[215];

            // 営業所名
            items[076] = sItems[90];

            // 社員コード
            items[077] = sItems[93];

            // 社員名
            items[078] = sItems[92];

            // 旧ＳＪ・ＮＫ契約判別フラグ
            Count = 0;
            if ("1".Equals(sItems[72]))
                Count++;
            if ("2".Equals(sItems[73]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[72]))
                    items[079] = "1";
                if ("2".Equals(sItems[73]))
                    items[079] = "2";
            }
            else if (Count == 0)
            {
                // 未選択
                items[079] = string.Empty;
            }
            else
            {
                // 複数選択
                items[079] = CONST_CHOFUKU; //"旧ＳＪ・ＮＫ契約判別フラグ_複数選択";
            }

            // 加入勧奨
            Count = 0;
            if ("1".Equals(sItems[98]))
                Count++;
            if ("2".Equals(sItems[98]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[98]))
                    items[080] = "1";
                if ("2".Equals(sItems[99]))
                    items[080] = "2";
            }
            else if (Count == 0)
            {
                // 未選択
                items[080] = string.Empty;
            }
            else
            {
                // 複数選択
                items[080] = CONST_CHOFUKU; //"加入勧奨_複数選択";
            }

            // 代理店コード
            items[081] = sItems[122];

            // 代理店サブコード
            items[082] = sItems[123];

            // 団体区分
            Count = 0;
            if ("1".Equals(sItems[262]))
                Count++;
            if ("2".Equals(sItems[263]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[262]))
                    items[083] = "F";
                if ("2".Equals(sItems[263]))
                    items[083] = "G";
            }
            else if (Count == 0)
            {
                // 未選択
                items[083] = string.Empty;
            }
            else
            {
                // 複数選択
                items[083] = CONST_CHOFUKU; //"団体区分_複数選択";
            }

            // 不備コ―ド
            items[084] = sItems[264];

            // 用途欄
            items[085] = sItems[97];

            // 一時払払込期日 
            items[086] = GetDate(entry_doc_id, sItems, 229);

            // 第１回払込期日 
            items[087] = GetDate(entry_doc_id, sItems, 225);

            // 第２回払込期日 
            items[088] = GetDate(entry_doc_id, sItems, 227);

            // 第２回目以降払込日
            items[089] = sItems[231];

            // 収納システム
            items[090] = sItems[256];

            // 払込方法
            items[091] = GetHaraikomiHoho_0102(sItems, 216);

            // 共同保険特約
            items[092] = GetKyodoHokenTokuyaku_0102(sItems, 74);

            // 代理人／仲立人　分担
            items[093] = GetNakadachinin_0102(sItems, 76);

            // 成績補正割合
            items[094] = sItems[248];

            // 自己物件
            items[095] = GetJikoTokutei_0102(sItems, 78, 0);

            // 代理店手数料区分　
            items[096] = GetJimuhiUmu_0102(sItems, 240);

            // 代理店手数料割合
            items[097] = sItems[244];

            // 集金事務費割合
            items[098] = sItems[245];

            // 控除月　第１回目
            items[099] = GetDate(entry_doc_id, sItems, 232, 4);

            // 控除月　第２回目
            items[100] = GetDate(entry_doc_id, sItems, 234, 4);

            // 控除月　一払払込猶予
            items[101] = GetDate(entry_doc_id, sItems, 236, 4);

            // 控　要･不要
            Count = 0;
            if ("1".Equals(sItems[87]))
                Count++;
            if ("2".Equals(sItems[88]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[87]))
                    items[102] = "1";
                if ("2".Equals(sItems[88]))
                    items[102] = "2";
            }
            else if (Count == 0)
            {
                items[102] = string.Empty;
            }
            else
            {
                items[102] = CONST_CHOFUKU; //"保険証券（控）複数選択";
            }

            // 異動計上明細データ　要･不要
            items[103] = sItems[51];

            // 異動計上明細データ配信先送付書計上口座ごと
            items[104] = sItems[52];

            // 集金内訳データ配信先送付書計上口座ごと
            items[105] = sItems[54];

            // ５０音別索引簿　送付先送付書計上口座ごと
            items[106] = sItems[56];

            // 無事故返戻保険料データ　要･不要
            items[107] = sItems[57];

            // 無事故返戻保険料データ配信先送付書計上口座ごと
            items[108] = sItems[58];

            // 無事故返戻データ　送付月の指定
            items[109] = GetDate(entry_doc_id, sItems, 59, 4);

            // 保険種類⑯～
            items[110] = sItems[170];
            items[111] = sItems[173];
            items[112] = sItems[176];
            items[113] = sItems[179];
            items[114] = sItems[182];

            items[115] = sItems[185];
            items[116] = sItems[188];
            items[117] = sItems[191];
            items[118] = sItems[194];
            items[119] = sItems[197];
            items[120] = sItems[200];
            items[121] = sItems[203];
            items[122] = sItems[206];
            items[123] = sItems[209];
            items[124] = sItems[212];

            // 払込方法①～㉚
            items[125] = sItems[126];
            items[126] = sItems[129];
            items[127] = sItems[132];
            items[128] = sItems[135];
            items[129] = sItems[138];
            items[130] = sItems[141];
            items[131] = sItems[144];
            items[132] = sItems[147];
            items[133] = sItems[150];
            items[134] = sItems[153];

            items[135] = sItems[156];
            items[136] = sItems[159];
            items[137] = sItems[162];
            items[138] = sItems[165];
            items[139] = sItems[168];
            items[140] = sItems[171];
            items[141] = sItems[174];
            items[142] = sItems[177];
            items[143] = sItems[180];
            items[144] = sItems[183];

            items[145] = sItems[186];
            items[146] = sItems[189];
            items[147] = sItems[192];
            items[148] = sItems[195];
            items[149] = sItems[198];
            items[150] = sItems[201];
            items[151] = sItems[204];
            items[152] = sItems[207];
            items[153] = sItems[210];
            items[154] = sItems[213];

            // 保険料①～㉚
            items[155] = sItems[127];
            items[156] = sItems[130];
            items[157] = sItems[133];
            items[158] = sItems[136];
            items[159] = sItems[139];
            items[160] = sItems[142];
            items[161] = sItems[145];
            items[162] = sItems[148];
            items[163] = sItems[151];
            items[164] = sItems[154];

            items[165] = sItems[157];
            items[166] = sItems[160];
            items[167] = sItems[163];
            items[168] = sItems[166];
            items[169] = sItems[169];
            items[170] = sItems[172];
            items[171] = sItems[175];
            items[172] = sItems[178];
            items[173] = sItems[181];
            items[174] = sItems[184];

            items[175] = sItems[187];
            items[176] = sItems[190];
            items[177] = sItems[193];
            items[178] = sItems[196];
            items[179] = sItems[199];
            items[180] = sItems[202];
            items[181] = sItems[205];
            items[182] = sItems[208];
            items[183] = sItems[211];
            items[184] = sItems[214];

            // 部店担当店
            if (sItems[268].Length != 0)
                items[185] = sItems[268];
            else
                items[185] = sItems[120];

            // 包括変更承認請求有
            items[186] = sItems[65];

            // 加入依頼書の種類
            Count = 0;
            if ("1".Equals(sItems[265]))
                Count++;
            if ("2".Equals(sItems[266]))
                Count++;
            if ("3".Equals(sItems[267]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(sItems[265]))
                    items[187] = "1";
                if ("2".Equals(sItems[266]))
                    items[187] = "2";
                if ("3".Equals(sItems[267]))
                    items[187] = "3";
            }
            else if (Count == 0)
            {
                // 未選択
                items[187] = "加入依頼書の種類_未選択";
            }
            else
            {
                // 複数選択
                items[187] = "加入依頼書の種類_複数選択";
            }

            // その他証券記載事項１
            items[188] = sItems[17];

            // その他証券記載事項２
            items[189] = sItems[18];

            // その他証券記載事項３
            items[190] = sItems[19];

            // 送付書あり表示
            items[191] = sItems[64];

            // 申込人住所１（漢字）
            items[192] = sItems[8];

            // 申込人住所２（漢字）
            items[193] = sItems[9];

            // 申込人住所３（漢字）
            items[194] = sItems[10];

            // 申込人氏名・法人名（漢字）
            items[195] = sItems[11];

            // 申込人氏名・肩書（漢字）
            items[196] = sItems[12];

            // 申込人氏名・代表者名（漢字）
            items[197] = sItems[13];

            // 団体コード
            items[198] = sItems[124];

            // 補正受　代理店コード
            items[199] = sItems[246];

            // 補正受 代理店サブコード
            items[200] = sItems[247];

            // 配列の0番目を削除
            var ListItems = new List<string>();
            ListItems.AddRange(items);
            ListItems.RemoveAt(0);
            return String.Join("\t", ListItems.ToArray());
        }
        #endregion

        #region 0102
        /// <summary>
        /// 団体契約更改送付書＜６４０２５－１＞編集
        /// </summary>
        /// <param name="sItems"></param>
        /// <returns></returns>
        private static string Edit0102(string[] sItems)
        {
            DOC_ID = "64025-1";
            ITEM_COUNT = 263;
            var items = new string[ITEM_COUNT + 1];
            for (int iIdx = 1; iIdx < items.Length; iIdx++)
                items[iIdx] = string.Empty;

            // 編集用変数
            //var edit = string.Empty;
            var list = new List<string>();
            //var Count = 0;

            // 帳票ID取得
            var entry_doc_id = sItems[3];

            // 証券番号・主番
            items[1] = sItems[4];

            // 証券番号・枝番
            items[2] = sItems[5];

            // 送付書番号
            items[3] = sItems[6];

            // 帳票番号
            items[4] = sItems[3];

            // 申請書番号 計上用
            items[5] = sItems[478];

            // 申請書番号 満期用
            items[6] = sItems[479];

            // 前年証券番号
            items[7] = sItems[475];

            // 前年送付書番号
            items[8] = sItems[476];

            // 管理番号
            items[9] = sItems[482];

            //領収日
            items[10] = GetDate(entry_doc_id, sItems, 480);

            //特殊処理①
            items[11] = sItems[484];

            //特殊処理②
            items[12] = sItems[485];

            //連絡票指示
            items[13] = sItems[486];

            //不備コ―ド
            items[14] = sItems[487];

            //団体名
            items[15] = sItems[1];

            //送付書用途欄
            items[16] = sItems[483];

            //一時払支払猶予払込期日
            items[17] = GetDate(entry_doc_id, sItems, 469);

            //2回払 第1回
            items[18] = GetDate(entry_doc_id, sItems, 453);

            //2回払 第2回
            items[19] = GetDate(entry_doc_id, sItems, 457);

            //6回払 第1回
            items[20] = GetDate(entry_doc_id, sItems, 461);

            //6回払 第2回
            items[21] = GetDate(entry_doc_id, sItems, 465);

            //12回払 第1回
            items[22] = GetDate(entry_doc_id, sItems, 445);

            //12回払 第2回
            items[23] = GetDate(entry_doc_id, sItems, 449);

            //その他団体分割払込日
            items[24] = sItems[473];

            //収納システム利用表示
            items[25] = sItems[474];

            //１募集する保険種目種類
            items[26] = sItems[10];

            //１払込方法
            items[27] = GetHaraikomiHoho_0102(sItems, 14, 1);

            //１共同保険特約
            items[28] = GetKyodoHokenTokuyaku_0102(sItems, 23, 1);

            //１代理店／仲立人 分担
            items[29] = GetNakadachinin_0102(sItems, 25, 1);

            //2回払控除月 第1回目
            items[30] = GetDate(entry_doc_id, sItems, 455, 4);

            //１補正 割合
            items[31] = sItems[29];

            //１自己物件 非or自己
            items[32] = GetJikoTokutei_0102(sItems, 30, 1);

            //１Ag Com 事務費の有無
            items[33] = GetJimuhiUmu_0102(sItems, 32, 1);

            //１Ag Com 特殊手数料率
            items[34] = sItems[36];

            //１Ag Com 特殊集金事務費
            items[35] = sItems[37];

            //１再保険種類コード
            items[36] = sItems[38];

            //2回払控除月 第2回目
            items[37] = GetDate(entry_doc_id, sItems, 459, 4);

            //２募集する保険種目種類
            items[38] = sItems[39];

            //２払込方法
            items[39] = GetHaraikomiHoho_0102(sItems, 43, 2);

            //２共同保険特約
            items[40] = GetKyodoHokenTokuyaku_0102(sItems, 52, 2);

            //２代理店／仲立人 分担
            items[41] = GetNakadachinin_0102(sItems, 54, 2);

            //6回払控除月 第1回目
            items[42] = GetDate(entry_doc_id, sItems, 463, 4);

            //２補正 割合
            items[43] = sItems[58];

            //２自己物件 非or自己
            items[44] = GetJikoTokutei_0102(sItems, 59, 2);

            //２Ag Com 事務費の有無
            items[45] = GetJimuhiUmu_0102(sItems, 61, 2);

            //２Ag Com 特殊手数料率
            items[46] = sItems[65];

            //２Ag Com 特殊集金事務費
            items[47] = sItems[66];

            //２再保険種類コード
            items[48] = sItems[67];

            //6回払控除月 第2回目
            items[49] = GetDate(entry_doc_id, sItems, 467, 4);

            //12回払控除月 第1回目
            items[50] = GetDate(entry_doc_id, sItems, 447, 4);

            //12回払控除月 第2回目
            items[51] = GetDate(entry_doc_id, sItems, 451, 4);

            //３募集する保険種目種類
            items[52] = sItems[68];

            //３払込方法
            items[53] = GetHaraikomiHoho_0102(sItems, 72, 3);

            //３共同保険特約
            items[54] = GetKyodoHokenTokuyaku_0102(sItems, 81, 3);

            //３代理店／仲立人 分担
            items[55] = GetNakadachinin_0102(sItems, 83, 3);

            //一時払払込猶予控除月
            items[56] = GetDate(entry_doc_id, sItems, 471, 4);

            //３補正 割合
            items[57] = sItems[87];

            //３自己物件 非or自己
            items[58] = GetJikoTokutei_0102(sItems, 88, 3);

            //３Ag Com 事務費の有無
            items[59] = GetJimuhiUmu_0102(sItems, 90, 3);

            //３Ag Com 特殊手数料率
            items[60] = sItems[94];

            //３Ag Com 特殊集金事務費
            items[61] = sItems[95];

            //３再保険種類コード
            items[62] = sItems[96];

            //４募集する保険種目種類
            items[63] = sItems[97];

            //４払込方法
            items[64] = GetHaraikomiHoho_0102(sItems, 101, 4);

            //４共同保険特約
            items[65] = GetKyodoHokenTokuyaku_0102(sItems, 110, 4);

            //４代理店／仲立人 分担
            items[66] = GetNakadachinin_0102(sItems, 112, 4);

            //４補正 割合
            items[67] = sItems[116];

            //４自己物件 非or自己
            items[68] = GetJikoTokutei_0102(sItems, 117, 4);

            //４Ag Com 事務費の有無
            items[69] = GetJimuhiUmu_0102(sItems, 119, 4);

            //４Ag Com 特殊手数料率
            items[70] = sItems[123];

            //４Ag Com 特殊集金事務費
            items[71] = sItems[124];

            //４再保険種類コード
            items[72] = sItems[125];

            //５募集する保険種目種類
            items[73] = sItems[126];

            //５払込方法
            items[74] = GetHaraikomiHoho_0102(sItems, 130, 5);

            //５共同保険特約
            items[75] = GetKyodoHokenTokuyaku_0102(sItems, 139, 5);

            //５代理店／仲立人 分担
            items[76] = GetNakadachinin_0102(sItems, 141, 5);

            //５補正 割合
            items[77] = sItems[145];

            //５自己物件 非or自己
            items[78] = GetJikoTokutei_0102(sItems, 146, 5);

            //５Ag Com 事務費の有無
            items[79] = GetJimuhiUmu_0102(sItems, 148, 5);

            //５Ag Com 特殊手数料率
            items[80] = sItems[152];

            //５Ag Com 特殊集金事務費
            items[81] = sItems[153];

            //５再保険種類コード
            items[82] = sItems[154];

            //６募集する保険種目種類
            items[83] = sItems[155];

            //６払込方法
            items[84] = GetHaraikomiHoho_0102(sItems, 159, 6);

            //６共同保険特約
            items[85] = GetKyodoHokenTokuyaku_0102(sItems, 168, 6);

            //６代理店／仲立人 分担
            items[86] = GetNakadachinin_0102(sItems, 170, 6);

            //６補正 割合
            items[87] = sItems[174];

            //６自己物件 非or自己
            items[88] = GetJikoTokutei_0102(sItems, 175, 6);

            //６Ag Com 事務費の有無
            items[89] = GetJimuhiUmu_0102(sItems, 177, 6);

            //６Ag Com 特殊手数料率
            items[90] = sItems[181];

            //６Ag Com 特殊集金事務費
            items[91] = sItems[182];

            //６再保険種類コード
            items[92] = sItems[183];

            //７募集する保険種目種類
            items[93] = sItems[184];

            //７払込方法
            items[94] = GetHaraikomiHoho_0102(sItems, 188, 7);

            //７共同保険特約
            items[95] = GetKyodoHokenTokuyaku_0102(sItems, 197, 7);

            //７代理店／仲立人 分担
            items[96] = GetNakadachinin_0102(sItems, 199, 7);

            //７補正 割合
            items[97] = sItems[203];

            //７自己物件 非or自己
            items[98] = GetJikoTokutei_0102(sItems, 204, 7);

            //７Ag Com 事務費の有無
            items[99] = GetJimuhiUmu_0102(sItems, 206, 7);

            //７Ag Com 特殊手数料率
            items[100] = sItems[210];

            //７Ag Com 特殊集金事務費
            items[101] = sItems[211];

            //７再保険種類コード
            items[102] = sItems[212];

            //８募集する保険種目種類
            items[103] = sItems[213];

            //８払込方法
            items[104] = GetHaraikomiHoho_0102(sItems, 217, 8);

            //８共同保険特約
            items[105] = GetKyodoHokenTokuyaku_0102(sItems, 226, 8);

            //８代理店／仲立人 分担
            items[106] = GetNakadachinin_0102(sItems, 228, 8);

            //８補正 割合
            items[107] = sItems[232];

            //８自己物件 非or自己
            items[108] = GetJikoTokutei_0102(sItems, 233, 8);

            //８Ag Com 事務費の有無
            items[109] = GetJimuhiUmu_0102(sItems, 235, 8);

            //８Ag Com 特殊手数料率
            items[110] = sItems[239];

            //８Ag Com 特殊集金事務費
            items[111] = sItems[240];

            //８再保険種類コード
            items[112] = sItems[241];

            //９募集する保険種目種類
            items[113] = sItems[242];

            //９払込方法
            items[114] = GetHaraikomiHoho_0102(sItems, 246, 9);

            //９共同保険特約
            items[115] = GetKyodoHokenTokuyaku_0102(sItems, 255, 9);

            //９代理店／仲立人 分担
            items[116] = GetNakadachinin_0102(sItems, 257, 9);

            //９補正 割合
            items[117] = sItems[261];

            //９自己物件 非or自己
            items[118] = GetJikoTokutei_0102(sItems, 262, 9);

            //９Ag Com 事務費の有無
            items[119] = GetJimuhiUmu_0102(sItems, 264, 9);

            //９Ag Com 特殊手数料率
            items[120] = sItems[268];

            //９Ag Com 特殊集金事務費
            items[121] = sItems[269];

            //９再保険種類コード
            items[122] = sItems[270];

            //10募集する保険種目種類
            items[123] = sItems[271];

            //10払込方法
            items[124] = GetHaraikomiHoho_0102(sItems, 275, 10);

            //10共同保険特約
            items[125] = GetKyodoHokenTokuyaku_0102(sItems, 284, 10);

            //10代理店／仲立人 分担
            items[126] = GetNakadachinin_0102(sItems, 286, 10);

            //10補正 割合
            items[127] = sItems[290];

            //10自己物件 非or自己
            items[128] = GetJikoTokutei_0102(sItems, 291, 10);

            //10Ag Com 事務費の有無
            items[129] = GetJimuhiUmu_0102(sItems, 293, 10);

            //10Ag Com 特殊手数料率
            items[130] = sItems[297];

            //10Ag Com 特殊集金事務費
            items[131] = sItems[298];

            //10再保険種類コード
            items[132] = sItems[299];

            //団体名（漢字）
            items[133] = sItems[2];

            // 代理店コード
            items[134] = sItems[8];

            //代理店サブコード
            items[135] = sItems[9];

            //送付書営業所コード
            items[136] = sItems[477];

            //部店担当店
            items[137] = sItems[7];

            //１種目別代理店コード
            items[138] = sItems[11];

            //２種目別代理店コード
            items[139] = sItems[40];

            //３種目別代理店コード
            items[140] = sItems[69];

            //４種目別代理店コード
            items[141] = sItems[98];

            //５種目別代理店コード
            items[142] = sItems[127];

            //６種目別代理店コード
            items[143] = sItems[156];

            //７種目別代理店コード
            items[144] = sItems[185];

            //８種目別代理店コード
            items[145] = sItems[214];

            //９種目別代理店コード
            items[146] = sItems[243];

            //10種目別代理店コード
            items[147] = sItems[272];

            //１種目代理店サブコード
            items[148] = sItems[12];

            //２種目代理店サブコード
            items[149] = sItems[41];

            //３種目代理店サブコード
            items[150] = sItems[70];

            //４種目代理店サブコード
            items[151] = sItems[99];

            //５種目代理店サブコード
            items[152] = sItems[128];

            //６種目代理店サブコード
            items[153] = sItems[157];

            //７種目代理店サブコード
            items[154] = sItems[186];

            //８種目代理店サブコード
            items[155] = sItems[215];

            //９種目代理店サブコード
            items[156] = sItems[244];

            //10種目代理店サブコード
            items[157] = sItems[273];

            //11種目別代理店コード
            items[158] = sItems[301];

            //12種目別代理店コード
            items[159] = sItems[330];

            //13種目別代理店コード
            items[160] = sItems[359];

            //14種目別代理店コード
            items[161] = sItems[388];

            //15種目別代理店コード
            items[162] = sItems[417];

            //11種目代理店サブコード
            items[163] = sItems[302];

            //12種目代理店サブコード
            items[164] = sItems[331];

            //13種目代理店サブコード
            items[165] = sItems[360];

            //14種目代理店サブコード
            items[166] = sItems[389];

            //15種目代理店サブコード
            items[167] = sItems[418];

            //１団体コード
            items[168] = sItems[13];

            //２団体コード
            items[169] = sItems[42];

            //３団体コード
            items[170] = sItems[71];

            //４団体コード
            items[171] = sItems[100];

            //５団体コード
            items[172] = sItems[129];

            //６団体コード
            items[173] = sItems[158];

            //７団体コード
            items[174] = sItems[187];

            //８団体コード
            items[175] = sItems[216];

            //９団体コード
            items[176] = sItems[245];

            //10団体コード
            items[177] = sItems[274];

            //11団体コード
            items[178] = sItems[303];

            //12団体コード
            items[179] = sItems[332];

            //13団体コード
            items[180] = sItems[361];

            //14団体コード
            items[181] = sItems[390];

            //15団体コード
            items[182] = sItems[419];

            //１補正 代理店コード
            items[183] = sItems[27];

            //２補正 代理店コード
            items[184] = sItems[56];

            //３補正 代理店コード
            items[185] = sItems[85];

            //４補正 代理店コード
            items[186] = sItems[114];

            //５補正 代理店コード
            items[187] = sItems[143];

            //６補正 代理店コード
            items[188] = sItems[172];

            //７補正 代理店コード
            items[189] = sItems[201];

            //８補正 代理店コード
            items[190] = sItems[230];

            //９補正 代理店コード
            items[191] = sItems[259];

            //10補正 代理店コード
            items[192] = sItems[288];

            //１補正 代理店サブコード
            items[193] = sItems[28];

            //２補正 代理店サブコード
            items[194] = sItems[57];

            //３補正 代理店サブコード
            items[195] = sItems[86];

            //４補正 代理店サブコード
            items[196] = sItems[115];

            //５補正 代理店サブコード
            items[197] = sItems[144];

            //６補正 代理店サブコード
            items[198] = sItems[173];

            //７補正 代理店サブコード
            items[199] = sItems[202];

            //８補正 代理店サブコード
            items[200] = sItems[231];

            //９補正 代理店サブコード
            items[201] = sItems[260];

            //10補正 代理店サブコード
            items[202] = sItems[289];

            //11補正 代理店コード
            items[203] = sItems[317];

            //12補正 代理店コード
            items[204] = sItems[346];

            //13補正 代理店コード
            items[205] = sItems[375];

            //14補正 代理店コード
            items[206] = sItems[404];

            //15補正 代理店コード
            items[207] = sItems[433];

            //11補正 代理店サブコード
            items[208] = sItems[318];

            //12補正 代理店サブコード
            items[209] = sItems[347];

            //13補正 代理店サブコード
            items[210] = sItems[376];

            //14補正 代理店サブコード
            items[211] = sItems[405];

            //15補正 代理店サブコード
            items[212] = sItems[434];

            // 11募集する保険種目種類
            items[213] = sItems[300];

            //11払込方法
            items[214] = GetHaraikomiHoho_0102(sItems, 304, 11);

            //11共同保険特約
            items[215] = GetKyodoHokenTokuyaku_0102(sItems, 313, 11);

            //11代理店／仲立人 分担
            items[216] = GetNakadachinin_0102(sItems, 315, 11);

            //11補正 割合
            items[217] = sItems[319];

            //11自己物件 非or自己
            items[218] = GetJikoTokutei_0102(sItems, 317, 11);

            //11Ag Com 事務費の有無
            items[219] = GetJimuhiUmu_0102(sItems, 322, 11);

            //11Ag Com 特殊手数料率
            items[220] = sItems[326];

            //11Ag Com 特殊集金事務費
            items[221] = sItems[327];

            //11再保険種類コード
            items[222] = sItems[328];

            //12募集する保険種目種類
            items[223] = sItems[329];

            //12払込方法
            items[224] = GetHaraikomiHoho_0102(sItems, 333, 12);

            //12共同保険特約
            items[225] = GetKyodoHokenTokuyaku_0102(sItems, 342, 12);

            //12代理店／仲立人 分担
            items[226] = GetNakadachinin_0102(sItems, 344, 12);

            //12補正 割合
            items[227] = sItems[348];

            //12自己物件 非or自己
            items[228] = GetJikoTokutei_0102(sItems, 349, 12);

            //12Ag Com 事務費の有無
            items[229] = GetJimuhiUmu_0102(sItems, 351, 12);

            //12Ag Com 特殊手数料率
            items[230] = sItems[355];

            //12Ag Com 特殊集金事務費
            items[231] = sItems[356];

            //12再保険種類コード
            items[232] = sItems[357];

            //13募集する保険種目種類
            items[233] = sItems[358];

            //13払込方法
            items[234] = GetHaraikomiHoho_0102(sItems, 362, 13);

            //13共同保険特約
            items[235] = GetKyodoHokenTokuyaku_0102(sItems, 371, 13);

            //13代理店／仲立人 分担
            items[236] = GetNakadachinin_0102(sItems, 373, 13);

            //13補正 割合
            items[237] = sItems[377];

            //13自己物件 非or自己
            items[238] = GetJikoTokutei_0102(sItems, 378, 13);

            //13Ag Com 事務費の有無
            items[239] = GetJimuhiUmu_0102(sItems, 380, 13);

            //13Ag Com 特殊手数料率
            items[240] = sItems[384];

            //13Ag Com 特殊集金事務費
            items[241] = sItems[385];

            //13再保険種類コード
            items[242] = sItems[386];

            //14募集する保険種目種類
            items[243] = sItems[387];

            //14払込方法
            items[244] = GetHaraikomiHoho_0102(sItems, 391, 14);

            //14共同保険特約
            items[245] = GetKyodoHokenTokuyaku_0102(sItems, 400, 14);

            //14代理店／仲立人 分担
            items[246] = GetNakadachinin_0102(sItems, 402, 14);

            //14補正 割合
            items[247] = sItems[406];

            //14自己物件 非or自己
            items[248] = GetJikoTokutei_0102(sItems, 407, 14);

            //14Ag Com 事務費の有無
            items[249] = GetJimuhiUmu_0102(sItems, 409, 14);

            //14Ag Com 特殊手数料率
            items[250] = sItems[413];

            //14Ag Com 特殊集金事務費
            items[251] = sItems[414];

            //14再保険種類コード
            items[252] = sItems[415];

            //15募集する保険種目種類
            items[253] = sItems[416];

            //15払込方法
            items[254] = GetHaraikomiHoho_0102(sItems, 420, 15);

            //15共同保険特約
            items[255] = GetKyodoHokenTokuyaku_0102(sItems, 429, 15);

            //15代理店／仲立人 分担
            items[256] = GetNakadachinin_0102(sItems, 431, 15);

            //15補正 割合
            items[257] = sItems[435];

            //15自己物件 非or自己
            items[258] = GetJikoTokutei_0102(sItems, 436, 15);

            //15Ag Com 事務費の有無
            items[259] = GetJimuhiUmu_0102(sItems, 438, 15);

            //15Ag Com 特殊手数料率
            items[260] = sItems[442];

            //15Ag Com 特殊集金事務費
            items[261] = sItems[443];

            //15再保険種類コード
            items[262] = sItems[444];

            var ListItems = new List<string>();
            ListItems.AddRange(items);
            ListItems.RemoveAt(0);
            return String.Join("\t", ListItems.ToArray());
        }
        #endregion 0102

        #region 0103
        /// <summary>
        /// Ｊ－ＦＬＥＸ・計上管理票＜０００１９＞編集
        /// </summary>
        /// <param name="sItems"></param>
        /// <returns></returns>
        private static string Edit0103(string[] sItems)
        {
            DOC_ID = "00019";
            ITEM_COUNT = 11;
            var items = new string[ITEM_COUNT + 1];
            for (int iIdx = 1; iIdx < items.Length; iIdx++)
                items[iIdx] = string.Empty;

            // 編集用変数
            //var edit = string.Empty;
            var list = new List<string>();
            //var Count = 0;

            // 帳票番号
            items[1] = sItems[1];

            // 処理区分
            items[2] = sItems[2];//.PadRight(2);

            // 連番
            items[3] = sItems[3];//.PadRight(4);

            // チェックデジット
            items[4] = sItems[4];//.PadRight(1);

            // 計上回数表示
            items[5] = string.Empty;

            // 奇偶表示
            items[6] = sItems[5];

            // 締め表示
            if (sItems[11].Length == 0)
            {
                items[7] = sItems[6];
            }
            else
            {
                items[7] = sItems[11];
            }

            // 扱年月日
            items[8] = sItems[7];

            // 担当者名
            items[9] = sItems[8].PadRight(10);

            // 件数
            items[10] = sItems[10].PadLeft(6, '0');

            // 枚数
            items[11] = sItems[9].PadLeft(6, '0');

            var ListItems = new List<string>();
            ListItems.AddRange(items);
            ListItems.RemoveAt(0);
            return String.Join("\t", ListItems.ToArray());
        }
        private static string Edit0103new(string[] sItems)
        {
            DOC_ID = "00019";
            ITEM_COUNT = 13;
            var items = new string[ITEM_COUNT + 1];
            for (int iIdx = 1; iIdx < items.Length; iIdx++)
                items[iIdx] = string.Empty;

            // 編集用変数
            //var edit = string.Empty;
            var list = new List<string>();
            //var Count = 0;

            items[1] = "01";
            items[2] = sItems[1].PadRight(2);
            items[3] = sItems[2].PadRight(5);
            // 管理番号
            items[4] = sItems[3].PadRight(8);
            // 奇偶表示
            items[5] = sItems[4].PadRight(1);
            // 扱年月日
            items[6] = sItems[5].PadRight(6);
            items[7] = sItems[6].PadRight(6); 
            items[8] = sItems[7].PadRight(10);
            items[9] = sItems[8].PadLeft(6, '0');
            items[10] = sItems[10].PadLeft(6, '0');
            items[11] = string.Empty.PadRight(1);
            items[12] = string.Empty.PadRight(8);
            items[13] = string.Empty.PadRight(4);

            var ListItems = new List<string>();
            ListItems.AddRange(items);
            ListItems.RemoveAt(0);
            return String.Join("\t", ListItems.ToArray());
        }
        #endregion

        #region 0104
        /// <summary>
        /// Ｊ－ＦＬＥＸ・明細管理票＜０００２７＞編集
        /// </summary>
        /// <param name="sItems"></param>
        /// <returns></returns>
        //private static string Edit0104old(string[] sItems)
        //{
        //    DOC_ID = "00027";
        //    ITEM_COUNT = 6;
        //    var items = new string[ITEM_COUNT + 1];
        //    for (int iIdx = 1; iIdx < items.Length; iIdx++)
        //        items[iIdx] = string.Empty;

        //    // 編集用変数
        //    var list = new List<string>();

        //    // 帳票番号
        //    items[1] = sItems[1];//.PadRight(5);

        //    // 処理区分
        //    items[2] = sItems[2];//.PadRight(2);

        //    // 連番
        //    items[3] = sItems[3];//.PadRight(4);

        //    // チェックデジット
        //    items[4] = sItems[4];//.PadRight(1);

        //    // 明細管理番号
        //    items[5] = sItems[5].PadLeft(4, '0');

        //    // 件数
        //    items[6] = sItems[6].PadLeft(6, '0');

        //    var ListItems = new List<string>();
        //    ListItems.AddRange(items);
        //    ListItems.RemoveAt(0);
        //    return String.Join("\t", ListItems.ToArray());
        //}
        private static string Edit0104(string[] sItems)
        {
            DOC_ID = "00027";
            //ITEM_COUNT = 7;
            var items = CreateStringArray(7);
            //var items = new string[ITEM_COUNT + 1];
            //for (int iIdx = 1; iIdx < items.Length; iIdx++)
            //    items[iIdx] = string.Empty;

            items[1] = "02";
            items[2] = sItems[1].PadRight(2);
            items[3] = sItems[2].PadRight(5);
            items[4] = sItems[3].PadRight(8);
            items[5] = sItems[4].PadLeft(4, '0');
            items[6] = sItems[5].PadLeft(6, '0');
            items[7] = string.Empty.PadRight(8);

            var ListItems = new List<string>();
            ListItems.AddRange(items);
            ListItems.RemoveAt(0);
            return String.Join(separate_string, ListItems.ToArray());
        }
        #endregion

        #region 0201
        private static string Edit0201(string[] sItems)
        {
            DOC_ID = "54399-3";

            //ITEM_COUNT = 183;
            var items = CreateStringArray(183);

            // 帳票ID取得
            var entry_doc_id = sItems[1];

            #region Group01
            items[1] = "81";
            items[2] = sItems[1].PadRight(7);
            items[3] = "E17";
            items[4] = string.Empty.PadRight(1);
            items[5] = string.Empty.PadRight(1);
            items[6] = string.Empty.PadRight(1);
            items[7] = string.Empty.PadRight(1);
            items[8] = string.Empty.PadRight(1);
            items[9] = string.Empty.PadRight(1);
            items[10] = string.Empty.PadRight(1);

            items[11] = string.Empty.PadRight(1);
            items[12] = string.Empty.PadRight(1);
            items[13] = string.Empty.PadRight(1);
            items[14] = string.Empty.PadRight(1);
            items[15] = string.Empty.PadRight(1);
            #endregion

            #region Group02
            // 郵便番号
            items[16] = sItems[2].PadRight(7);

            // 電話番号
            items[17] = sItems[3].PadRight(20);

            // 携帯電話
            items[18] = sItems[4].PadRight(20);

            // 申込人住所１（漢字）
            items[19] = sItems[8].PadRight(15, '　');

            // 申込人住所２（漢字）
            items[20] = sItems[9].PadRight(15, '　');

            // 申込人住所３（漢字）
            items[21] = sItems[10].PadRight(15, '　');

            // 申込人住所１（カナ）
            items[22] = sItems[5].PadRight(22);

            // 申込人住所２（カナ）
            items[23] = sItems[6].PadRight(22);

            // 申込人住所３（カナ）
            items[24] = sItems[7].PadRight(22); ;

            // 申込人　氏名１（漢字）
            items[25] = sItems[11].PadRight(15, '　');

            // 申込人　氏名２（漢字）
            items[26] = sItems[12].PadRight(15, '　');

            // 申込人　氏名３（漢字）
            items[27] = sItems[13].PadRight(15, '　');

            // 申込人　法人名
            items[28] = sItems[14].PadRight(50);

            // 申込人　肩書
            items[29] = sItems[15].PadRight(30);

            // 申込人　代表者名
            items[30] = sItems[16].PadRight(20);

            // 性別
            items[31] = Get1or2(sItems, 17).PadRight(1);

            // 生年月日
            items[32] = GetBirthDay( sItems, 19);
            #endregion

            #region Group03
            items[33] = sItems[24].PadRight(20);
            items[34] = sItems[25].PadRight(20);
            items[35] = sItems[26].PadRight(20);
            items[36] = sItems[27].PadRight(20);
            items[37] = sItems[28].PadRight(20);

            items[38] = sItems[29].PadRight(30);
            items[39] = sItems[30].PadRight(30); 
            items[40] = sItems[31].PadRight(30); 
            items[41] = sItems[32].PadRight(30);

            items[42] = sItems[33].PadRight(20); 
            items[43] = sItems[34].PadLeft(13,'0');
            #endregion

            #region Group04-01
            items[44] = sItems[36].PadLeft(15, '　');
            items[45] = sItems[35].PadLeft(20);
            items[46] = GetDate(entry_doc_id, sItems, 37).PadRight(7);
            items[47] = Get1or2(sItems, 42);
            items[48] = sItems[44].PadRight(10);
            items[49] = sItems[45].PadRight(1);
            items[50] = string.Empty.PadRight(1);
            items[51] = sItems[46].PadRight(1);
            items[52] = GetRelation(sItems, 47);
            items[53] = sItems[54].PadRight(16);
            items[54] = string.Empty.PadRight(1);
            items[55] = Get1or2(sItems, 55);
            items[56] = string.Empty.PadRight(1);
            items[57] = sItems[57].PadRight(4);
            items[58] = sItems[58].PadLeft(2, '0');
            items[59] = sItems[59].PadLeft(9, '0');
            items[60] = string.Empty.PadRight(1);
            items[61] = string.Empty.PadRight(1);
            items[62] = string.Empty.PadRight(4);
            items[63] = string.Empty.PadRight(2);
            items[64] = string.Empty.PadRight(9);
            items[65] = string.Empty.PadRight(1);
            items[66] = sItems[60].PadRight(20);
            items[67] = sItems[61].PadRight(20);
            items[68] = sItems[62].PadRight(10);
            items[69] = sItems[63].PadRight(20);
            items[70] = Get1or2(sItems, 64);
            items[71] = sItems[66].PadRight(4);
            items[72] = sItems[67].PadRight(20);
            items[73] = string.Empty.PadRight(60);
            items[74] = sItems[68].PadRight(4);
            items[75] = sItems[69].PadRight(20);
            items[76] = string.Empty.PadRight(60);
            items[77] = sItems[70].PadRight(22);
            items[78] = sItems[71].PadRight(22);
            items[79] = sItems[72].PadRight(22);
            items[80] = sItems[73].PadRight(22);
            items[81] = sItems[74].PadRight(22);
            items[82] = sItems[75].PadRight(22);
            items[83] = string.Empty.PadRight(1);
            #endregion

            #region Group04-02
            items[84] = sItems[77].PadLeft(15, '　');
            items[85] = sItems[76].PadLeft(20);
            items[86] = GetDate(entry_doc_id, sItems, 78).PadRight(7);
            items[87] = Get1or2(sItems, 83);
            items[88] = sItems[85].PadRight(10);
            items[89] = sItems[86].PadRight(1);
            items[90] = string.Empty.PadRight(1);
            items[91] = sItems[87].PadRight(1);
            items[92] = GetRelation(sItems, 88);
            items[93] = sItems[95].PadRight(16);
            items[94] = string.Empty.PadRight(1);
            items[95] = Get1or2(sItems, 96);
            items[96] = string.Empty.PadRight(1);
            items[97] = sItems[98].PadRight(4);
            items[98] = sItems[99].PadLeft(2, '0');
            items[99] = sItems[100].PadLeft(9, '0');
            items[100] = string.Empty.PadRight(1);
            items[101] = string.Empty.PadRight(1);
            items[102] = string.Empty.PadRight(4);
            items[103] = string.Empty.PadRight(2);
            items[104] = string.Empty.PadRight(9);
            items[105] = string.Empty.PadRight(1);
            items[106] = sItems[101].PadRight(20);
            items[107] = sItems[102].PadRight(20);
            items[108] = sItems[103].PadRight(10);
            items[109] = sItems[104].PadRight(20);
            items[110] = Get1or2(sItems, 105);
            items[111] = sItems[107].PadRight(4);
            items[112] = sItems[108].PadRight(20);
            items[113] = string.Empty.PadRight(60);
            items[114] = sItems[109].PadRight(4);
            items[115] = sItems[110].PadRight(20);
            items[116] = string.Empty.PadRight(60);
            items[117] = sItems[111].PadRight(22);
            items[118] = sItems[112].PadRight(22);
            items[119] = sItems[113].PadRight(22);
            items[120] = sItems[114].PadRight(22);
            items[121] = sItems[115].PadRight(22);
            items[122] = sItems[116].PadRight(22);
            items[123] = string.Empty.PadRight(1);
            #endregion

            #region Group04-03
            items[124] = sItems[118].PadLeft(15, '　');
            items[125] = sItems[117].PadLeft(20);
            items[126] = GetDate(entry_doc_id, sItems, 119).PadRight(7);
            items[127] = Get1or2(sItems, 124);
            items[128] = sItems[126].PadRight(10);
            items[129] = sItems[127].PadRight(1);
            items[130] = string.Empty.PadRight(1);
            items[131] = sItems[128].PadRight(1);
            items[132] = GetRelation(sItems, 129);
            items[133] = sItems[136].PadRight(16);
            items[134] = string.Empty.PadRight(1);
            items[135] = Get1or2(sItems, 137);
            items[136] = string.Empty.PadRight(1);
            items[137] = sItems[139].PadRight(4);
            items[138] = sItems[140].PadLeft(2, '0');
            items[139] = sItems[141].PadLeft(9, '0');
            items[140] = string.Empty.PadRight(1);
            items[141] = string.Empty.PadRight(1);
            items[142] = string.Empty.PadRight(4);
            items[143] = string.Empty.PadRight(2);
            items[144] = string.Empty.PadRight(9);
            items[145] = string.Empty.PadRight(1);
            items[146] = sItems[142].PadLeft(20);
            items[147] = sItems[143].PadLeft(20);
            items[148] = sItems[144].PadLeft(10);
            items[149] = sItems[145].PadLeft(20);
            items[150] = Get1or2(sItems, 146);
            items[151] = sItems[148].PadRight(4);
            items[152] = sItems[149].PadRight(20);
            items[153] = string.Empty.PadRight(60);
            items[154] = sItems[150].PadRight(4);
            items[155] = sItems[151].PadRight(20);
            items[156] = string.Empty.PadRight(60);
            items[157] = sItems[152].PadRight(22);
            items[158] = sItems[153].PadRight(22);
            items[159] = sItems[154].PadRight(22);
            items[160] = sItems[155].PadRight(22);
            items[161] = sItems[156].PadRight(22);
            items[162] = sItems[157].PadRight(22);
            items[163] = string.Empty.PadRight(1);
            #endregion

            #region Group05
            items[164] = sItems[158].PadRight(1);
            items[165] = sItems[159].PadRight(1);
            items[166] = sItems[160].PadRight(1);
            items[167] = sItems[161].PadRight(1);
            items[168] = sItems[162].PadRight(1);
            items[169] = sItems[163].PadRight(1);
            items[170] = sItems[164].PadRight(1);
            items[171] = sItems[165].PadRight(15);
            items[172] = sItems[166].PadRight(15);
            items[173] = sItems[167].PadRight(15);
            items[174] = sItems[168].PadRight(4);
            items[175] = sItems[169].PadRight(4);
            items[176] = sItems[170].PadRight(1);
            items[177] = sItems[171].PadRight(30);
            items[178] = string.Empty.PadRight(1);
            items[179] = string.Empty.PadRight(1);
            items[180] = sItems[172].PadRight(13);
            items[181] = sItems[173].PadRight(12);
            items[182] = sItems[174].PadRight(2);
            items[183] = sItems[175].PadLeft(5, '0');
            #endregion

            var ListItems = new List<string>();
            ListItems.AddRange(items);
            ListItems.RemoveAt(0);
            return String.Join(separate_string, ListItems.ToArray());
        }
        #endregion

        #region 0202
        private static string Edit0202(string[] sItems)
        {
            DOC_ID = "61964";
            ITEM_COUNT = 144;
            var items = new string[ITEM_COUNT + 1];
            for (int iIdx = 1; iIdx < items.Length; iIdx++)
                items[iIdx] = string.Empty;

            // 帳票ID取得
            var entry_doc_id = sItems[1];

            #region Group01
            items[1] = "30";
            items[2] = sItems[1].PadRight(5);
            items[3] = "E00";
            items[4] = sItems[2].PadRight(3);
            items[5] = sItems[3].PadRight(10);
            items[6] = sItems[4].PadRight(3);
            items[7] = sItems[5].PadRight(1);
            items[8] = sItems[6].PadRight(1);
            items[9] = string.Empty.PadRight(1);
            items[10] = sItems[7].PadRight(1);

            items[11] = sItems[8].PadRight(4);
            items[12] = GetHaraikomiHoho_0202(sItems,9);
            items[13] = Get1or2(sItems, 19);
            items[14] = Get1or2(sItems, 21);
            #endregion

            #region Group02-01
            items[15] = sItems[23].PadRight(3);
            items[16] = sItems[24].PadRight(9).PadLeft(9,'0');
            items[17] = string.Empty.PadRight(1);
            items[18] = sItems[25].PadRight(5);
            items[19] = sItems[26].PadRight(3);
            items[20] = sItems[27].PadRight(9).PadLeft(9, '0');
            items[21] = GetJikoTokutei_0102(sItems,28).PadRight(1);
            items[22] = sItems[30].PadLeft(9,'0');
            #endregion

            #region Group02-02
            items[23] = sItems[31].PadRight(3);
            items[24] = sItems[32].PadRight(9).PadLeft(9, '0');
            items[25] = string.Empty.PadRight(1);
            items[26] = sItems[33].PadRight(5);
            items[27] = sItems[34].PadRight(3);
            items[28] = sItems[35].PadRight(9).PadLeft(9, '0');
            items[29] = GetJikoTokutei_0102(sItems,36).PadRight(1);
            items[30] = sItems[38].PadLeft(9,'0');
            #endregion

            #region Group02-03
            items[31] = sItems[39].PadRight(3);
            items[32] = sItems[40].PadRight(9).PadLeft(9, '0');
            items[33] = string.Empty.PadRight(1);
            items[34] = sItems[41].PadRight(5);
            items[35] = sItems[42].PadRight(3);
            items[36] = sItems[43].PadRight(9).PadLeft(9, '0');
            items[37] = GetJikoTokutei_0102(sItems,44).PadRight(1);
            items[38] = sItems[46].PadLeft(9, '0');
            #endregion

            #region Group02-04
            items[39] = sItems[47].PadRight(3);
            items[40] = sItems[48].PadRight(9).PadLeft(9, '0') ;
            items[41] = string.Empty.PadRight(1);
            items[42] = sItems[49].PadRight(5);
            items[43] = sItems[50].PadRight(3);
            items[44] = sItems[51].PadRight(9).PadLeft(9, '0');
            items[45] = GetJikoTokutei_0102(sItems,52).PadRight(1);
            items[46] = sItems[54].PadLeft(9, '0');
            #endregion

            #region Group02-05
            items[47] = sItems[55].PadRight(3);
            items[48] = sItems[56].PadRight(9).PadLeft(9, '0');
            items[49] = string.Empty.PadRight(1);
            items[50] = sItems[57].PadRight(5);
            items[51] = sItems[58].PadRight(3);
            items[52] = sItems[59].PadRight(9).PadLeft(9, '0');
            items[53] = GetJikoTokutei_0102(sItems,60).PadRight(1);
            items[54] = sItems[62].PadLeft(9, '0');
            #endregion

            #region Group02-06
            items[55] = sItems[63].PadRight(3);
            items[56] = sItems[64].PadRight(9).PadLeft(9, '0');
            items[57] = string.Empty.PadRight(1);
            items[58] = sItems[65].PadRight(5);
            items[59] = sItems[66].PadRight(3);
            items[60] = sItems[67].PadRight(9).PadLeft(9, '0');
            items[61] = GetJikoTokutei_0102(sItems,68).PadRight(1);
            items[62] = sItems[70].PadLeft(9, '0');
            #endregion


            #region Group02-07
            items[63] = sItems[71].PadRight(3);
            items[64] = sItems[72].PadRight(9).PadLeft(9, '0');
            items[65] = string.Empty.PadRight(1);
            items[66] = sItems[73].PadRight(5);
            items[67] = sItems[74].PadRight(3);
            items[68] = sItems[75].PadRight(9).PadLeft(9, '0');
            items[69] = GetJikoTokutei_0102(sItems,76).PadRight(1);
            items[70] = sItems[78].PadLeft(9, '0');
            #endregion

            #region Group02-08
            items[71] = sItems[79].PadRight(3);
            items[72] = sItems[80].PadRight(9).PadLeft(9, '0');
            items[73] = string.Empty.PadRight(1);
            items[74] = sItems[81].PadRight(5);
            items[75] = sItems[82].PadRight(3);
            items[76] = sItems[83].PadRight(9).PadLeft(9, '0');
            items[77] = GetJikoTokutei_0102(sItems,84).PadRight(1);
            items[78] = sItems[86].PadLeft(9, '0');
            #endregion

            #region Group02-09
            items[79] = sItems[87].PadRight(3);
            items[80] = sItems[88].PadRight(9).PadLeft(9, '0') ;
            items[81] = string.Empty.PadRight(1);
            items[82] = sItems[89].PadRight(5);
            items[83] = sItems[90].PadRight(3);
            items[84] = sItems[91].PadRight(9).PadLeft(9, '0');
            items[85] = GetJikoTokutei_0102(sItems,92).PadRight(1);
            items[86] = sItems[94].PadLeft(9, '0');
            #endregion

            #region Group02-10
            items[87] = sItems[95].PadRight(3);
            items[88] = sItems[96].PadRight(9).PadLeft(9, '0') ;
            items[89] = string.Empty.PadRight(1);
            items[90] = sItems[97].PadRight(5);
            items[91] = sItems[98].PadRight(3);
            items[92] = sItems[99].PadRight(9).PadLeft(9, '0');
            items[93] = GetJikoTokutei_0102(sItems,100).PadRight(1);
            items[94] = sItems[102].PadLeft(9, '0');
            #endregion

            #region Group03
            items[95] = string.Empty.PadRight(1);
            items[96] = EditXX(sItems[103], 9, 2);
            items[97] = string.Empty.PadRight(1); 
            items[98] = sItems[104].PadRight(5); 
            items[99] = sItems[105].PadRight(3); 
            items[100] = EditXX(sItems[106], 9, 2);
            items[101] = string.Empty.PadRight(1);
            items[102] = sItems[107].PadRight(5);
            items[103] = sItems[108].PadRight(3);
            items[104] = EditXX(sItems[109], 9, 2);
            items[105] = string.Empty.PadRight(1);
            items[106] = sItems[110].PadRight(5);
            items[107] = sItems[111].PadRight(3);
            items[108] = EditXX(sItems[112], 9, 2);
            items[109] = string.Empty.PadRight(1);
            items[110] = sItems[113].PadRight(5);
            items[111] = sItems[114].PadRight(3);
            items[112] = EditXX(sItems[115], 9, 2);
            items[113] = string.Empty.PadRight(1);
            items[114] = sItems[116].PadRight(5);
            items[115] = sItems[117].PadRight(3);
            items[116] = EditXX(sItems[118], 9, 2);
            items[117] = string.Empty.PadRight(1);
            items[118] = sItems[119].PadRight(5);
            items[119] = sItems[120].PadRight(3);
            items[120] = EditXX(sItems[121], 9, 2);
            items[121] = string.Empty.PadRight(1);
            items[122] = sItems[122].PadRight(5);
            items[123] = sItems[123].PadRight(3);
            items[124] = EditXX(sItems[124], 9, 2);
            items[125] = string.Empty.PadRight(1);
            items[126] = sItems[125].PadRight(5);
            items[127] = sItems[126].PadRight(3);
            items[128] = EditXX(sItems[127], 9, 2);
            items[129] = string.Empty.PadRight(1);
            items[130] = sItems[128].PadRight(5);
            items[131] = sItems[129].PadRight(3);
            items[132] = EditXX(sItems[130], 9, 2);
            items[133] = string.Empty.PadRight(1);
            items[134] = sItems[131].PadRight(5);
            items[135] = sItems[132].PadRight(3);
            items[136] = EditXX(sItems[133], 9, 2);
            items[137] = string.Empty.PadRight(1);
            items[138] = sItems[134].PadRight(5);
            items[139] = sItems[135].PadRight(3);
            items[140] = EditXX(sItems[136], 9, 2);
            items[141] = string.Empty.PadRight(1);
            items[142] = sItems[137].PadRight(5);
            items[143] = sItems[138].PadRight(3);
            items[144] = EditXX(sItems[139], 9, 2);//.PadLeft(9,'0');
            #endregion

            var ListItems = new List<string>();
            ListItems.AddRange(items);
            ListItems.RemoveAt(0);
            return String.Join(separate_string, ListItems.ToArray());
        }
        #endregion

        #region 0203
        private static string Edit0203(string[] sItems)
        {
            DOC_ID = "54267-1";
            ITEM_COUNT = 221;
            var items = new string[ITEM_COUNT + 1];
            for (int iIdx = 1; iIdx < items.Length; iIdx++)
                items[iIdx] = string.Empty;

            // 帳票ID取得
            var entry_doc_id = sItems[1];

            items[1] = "63";
            items[2] = sItems[1].PadRight(7);
            items[3] = "E09";
            items[4] = string.Empty.PadRight(1);
            items[5] = string.Empty.PadRight(1);
            items[6] = string.Empty.PadRight(1);
            items[7] = string.Empty.PadRight(1);
            items[8] = string.Empty.PadRight(1);
            items[9] = string.Empty.PadRight(1);
            items[10] = string.Empty.PadRight(1);

            items[11] = string.Empty.PadRight(1);
            items[12] = string.Empty.PadRight(1);
            items[13] = string.Empty.PadRight(1);
            items[14] = string.Empty.PadRight(1);
            items[15] = string.Empty.PadRight(1);

            // 郵便番号
            items[16] = sItems[2].PadRight(7);

            // 電話番号
            items[17] = sItems[3].PadRight(20);

            // 携帯電話番号
            items[18] = sItems[4].PadRight(20);

            // 住所（漢字）１
            items[19] = sItems[5].PadRight(15, '　');

            // 住所（漢字）２
            items[20] = sItems[6].PadRight(15, '　');

            // 住所（漢字）３
            items[21] = sItems[7].PadRight(15, '　');

            // 住所１
            items[22] = sItems[8].PadRight(22);

            // 住所２
            items[23] = sItems[9].PadRight(22);

            // 住所３
            items[24] = sItems[10].PadRight(22);

            // 氏名（漢字）１
            items[25] = sItems[11].PadRight(15, '　');

            // 氏名（漢字）２
            items[26] = sItems[12].PadRight(15, '　');

            // 氏名（漢字）３
            items[27] = sItems[13].PadRight(15, '　');

            // 法人名
            items[28] = sItems[14].PadRight(50);

            // 肩書
            items[29] = sItems[15].PadRight(30);

            // 役職
            items[30] = sItems[16].PadRight(20);

            // 性別
            items[31] = Get1or2(sItems, 17);

            // 生年月日
            items[32] = GetBirthDay(sItems, 19);

            // 扶養者
            items[33] = sItems[24].PadRight(20);

            #region Group02
            // 明細整理番号１
            items[34] = sItems[25].PadRight(20);

            // 明細整理番号２
            items[35] = sItems[26].PadRight(20);

            // 明細整理番号３
            items[36] = sItems[27].PadRight(20);

            // 明細整理番号４
            items[37] = sItems[28].PadRight(20);

            // 明細整理番号５
            items[38] = sItems[29].PadRight(20);

            // 明細整理番号６（漢字）１
            items[39] = sItems[30].PadRight(15, '　');

            // 明細整理番号６（漢字）２
            items[40] = sItems[31].PadRight(15, '　');

            // 明細整理番号７（漢字）１
            items[41] = sItems[32].PadRight(15, '　');

            // 明細整理番号７（漢字）２
            items[42] = sItems[33].PadRight(15, '　');
            #endregion

            items[43] = string.Empty.PadLeft(1);
            items[44] = string.Empty.PadLeft(1);
            items[45] = string.Empty.PadLeft(1);

            #region Group03
            // 合計保険料
            items[46] = EditXX(sItems[34], 13);

            // 即時保険料
            items[47] = EditXX(sItems[35], 13);
            #endregion

            #region Group04-01
            items[48] = sItems[36].PadRight(1);
            items[49] = sItems[37].PadRight(15,'　');
            items[50] = sItems[38].PadRight(20);
            items[51] = Get1or2(sItems, 39);//39,40
            items[52] = sItems[41].PadRight(1);
            items[53] = GetBirthDay(sItems, 42);//42,3,4,5,6
            items[54] = GetTsuzukigara0203(sItems, 47);//47,48,49,50,51,52,53
            items[55] = sItems[54].PadRight(10);
            items[56] = sItems[55].PadRight(16);
            items[57] = string.Empty.PadLeft(1);
            items[58] = sItems[56].PadRight(4);
            items[59] = sItems[57].PadRight(2);
            items[60] = EditXX(sItems[58],9);
            items[61] = EditXX(sItems[59],9);
            items[62] = sItems[60].PadRight(4);
            items[63] = sItems[61].PadRight(2);
            items[64] = EditXX(sItems[62],9);
            items[65] = EditXX(sItems[63],9);
            items[66] = sItems[64].PadRight(4);
            items[67] = sItems[65].PadRight(2);
            items[68] = EditXX(sItems[66],9);
            items[69] = EditXX(sItems[67],9);
            items[70] = sItems[68].PadRight(4);
            items[71] = sItems[69].PadRight(2);
            items[72] = EditXX(sItems[70],9);
            items[73] = EditXX(sItems[71],9);
            items[74] = sItems[72].PadRight(4);
            items[75] = sItems[73].PadRight(2);
            items[76] = EditXX(sItems[74],9);
            items[77] = EditXX(sItems[75],9);
            items[78] = sItems[76].PadRight(4);
            items[79] = sItems[77].PadRight(20);
            items[80] = sItems[78].PadRight(20);
            items[81] = sItems[79].PadRight(20);
            items[82] = sItems[80].PadRight(20);
            items[83] = sItems[81].PadRight(4);
            items[84] = sItems[82].PadRight(20);
            items[85] = sItems[83].PadRight(20);
            items[86] = sItems[84].PadRight(20);
            items[87] = sItems[85].PadRight(20);
            #endregion

            #region Group04-02
            items[88] = sItems[86].PadRight(15, '　');
            items[89] = sItems[87].PadRight(20);
            items[90] = Get1or2(sItems,88);//88,89
            items[91] = sItems[90].PadRight(1);
            items[92] = GetBirthDay(sItems, 91);//91,92,93,94,95
            items[93] = GetTsuzukigara0203(sItems, 96);//96,7,8,9,0,1,2
            items[94] = sItems[103].PadRight(10);
            items[95] = sItems[104].PadRight(16);
            items[96] = string.Empty.PadLeft(1);
            items[97] = sItems[105].PadRight(4);
            items[98] = sItems[106].PadRight(2);
            items[99] = EditXX(sItems[107],9);
            items[100] = EditXX(sItems[108],9);
            items[101] = sItems[109].PadRight(4);
            items[102] = sItems[110].PadRight(2);
            items[103] = EditXX(sItems[111],9);
            items[104] = EditXX(sItems[112],9);
            items[105] = sItems[113].PadRight(4);
            items[106] = sItems[114].PadRight(2);
            items[107] = EditXX(sItems[115],9);
            items[108] = EditXX(sItems[116],9);
            items[109] = sItems[117].PadRight(4);
            items[110] = sItems[118].PadRight(2);
            items[111] = EditXX(sItems[119],9);
            items[112] = EditXX(sItems[120],9);
            items[113] = sItems[121].PadRight(4);
            items[114] = sItems[122].PadRight(2);
            items[115] = EditXX(sItems[123],9);
            items[116] = EditXX(sItems[124],9);
            items[117] = sItems[125].PadRight(4);
            items[118] = sItems[126].PadRight(20);
            items[119] = sItems[127].PadRight(20);
            items[120] = sItems[128].PadRight(20);
            items[121] = sItems[129].PadRight(20);
            items[122] = sItems[130].PadRight(4);
            items[123] = sItems[131].PadRight(20);
            items[124] = sItems[132].PadRight(20);
            items[125] = sItems[133].PadRight(20);
            items[126] = sItems[134].PadRight(20);
            #endregion

            #region Group04-03
            items[127] = sItems[135].PadRight(15, '　');
            items[128] = sItems[136].PadRight(20);
            items[129] = Get1or2(sItems, 137);//136,37
            items[130] = sItems[139].PadRight(1);
            items[131] = GetBirthDay(sItems, 140);// 139,0,1,2,3
            items[132] = GetTsuzukigara0203(sItems, 145);//4,5,6,7,8,9,0
            items[133] = sItems[152].PadRight(10);
            items[134] = sItems[153].PadRight(16);
            items[135] = string.Empty.PadLeft(1);
            items[136] = sItems[154].PadRight(4);
            items[137] = sItems[155].PadRight(2);
            items[138] = EditXX(sItems[156], 9);
            items[139] = EditXX(sItems[157], 9);
            items[140] = sItems[158].PadRight(4);
            items[141] = sItems[159].PadRight(2);
            items[142] = EditXX(sItems[160], 9);
            items[143] = EditXX(sItems[161], 9);
            items[144] = sItems[162].PadRight(4);
            items[145] = sItems[163].PadRight(2);
            items[146] = EditXX(sItems[164], 9);
            items[147] = EditXX(sItems[165], 9);
            items[148] = sItems[166].PadRight(4);
            items[149] = sItems[167].PadRight(2);
            items[150] = EditXX(sItems[168], 9);
            items[151] = EditXX(sItems[169], 9);
            items[152] = sItems[170].PadRight(4);
            items[153] = sItems[171].PadRight(2);
            items[154] = EditXX(sItems[172], 9);
            items[155] = EditXX(sItems[173], 9);
            items[156] = sItems[174].PadRight(4);
            items[157] = sItems[175].PadRight(20);
            items[158] = sItems[176].PadRight(20);
            items[159] = sItems[177].PadRight(20);
            items[160] = sItems[178].PadRight(20);
            items[161] = sItems[179].PadRight(4);
            items[162] = sItems[180].PadRight(20);
            items[163] = sItems[181].PadRight(20);
            items[164] = sItems[182].PadRight(20);
            items[165] = sItems[183].PadRight(20);
            #endregion

            #region Group04-04
            items[166] = sItems[184].PadRight(15,'　');
            items[167] = sItems[185].PadRight(20);
            items[168] = Get1or2(sItems, 186);//86,87
            items[169] = sItems[188].PadRight(1);
            items[170] = GetBirthDay(sItems, 189);//189,90,91,92,93
            items[171] = GetTsuzukigara0203(sItems, 194);//94,5,6,7,8,9,100
            items[172] = sItems[201].PadRight(10);
            items[173] = sItems[202].PadRight(16);
            items[174] = string.Empty.PadLeft(1);
            items[175] = sItems[203].PadRight(4);
            items[176] = sItems[204].PadRight(2);
            items[177] = EditXX(sItems[205],9);
            items[178] = EditXX(sItems[206],9);
            items[179] = sItems[207].PadRight(4);
            items[180] = sItems[208].PadRight(2);
            items[181] = EditXX(sItems[209],9);
            items[182] = EditXX(sItems[210],9);
            items[183] = sItems[211].PadRight(4);
            items[184] = sItems[212].PadRight(2);
            items[185] = EditXX(sItems[213],9);
            items[186] = EditXX(sItems[214],9);
            items[187] = sItems[215].PadRight(4);
            items[188] = sItems[216].PadRight(2);
            items[189] = EditXX(sItems[217],9);
            items[190] = EditXX(sItems[218],9);
            items[191] = sItems[219].PadRight(4);
            items[192] = sItems[220].PadRight(2);
            items[193] = EditXX(sItems[221],9);
            items[194] = EditXX(sItems[222],9);
            items[195] = sItems[223].PadRight(4);
            items[196] = sItems[224].PadRight(20);
            items[197] = sItems[225].PadRight(20);
            items[198] = sItems[226].PadRight(20);
            items[199] = sItems[227].PadRight(20);
            items[200] = sItems[228].PadRight(4);
            items[201] = sItems[229].PadRight(20);
            items[202] = sItems[230].PadRight(20);
            items[203] = sItems[231].PadRight(20);
            items[204] = sItems[232].PadRight(20);
            #endregion

            #region Group05
            items[205] = sItems[233].PadRight(1);
            items[206] = sItems[234].PadRight(1);
            items[207] = sItems[235].PadRight(1);
            items[208] = sItems[236].PadRight(1);
            items[209] = sItems[237].PadRight(1);
            items[210] = sItems[238].PadRight(1);
            items[211] = sItems[239].PadRight(15);
            items[212] = sItems[240].PadRight(15);
            items[213] = sItems[241].PadRight(15);
            items[214] = sItems[242].PadRight(4);
            items[215] = sItems[243].PadRight(4);
            items[216] = sItems[244].PadRight(1);
            items[217] = sItems[245].PadRight(30);
            items[218] = sItems[246].PadRight(13);
            items[219] = sItems[247].PadRight(12);
            items[220] = sItems[248].PadRight(2);
            items[221] = sItems[249].PadLeft(5, '0');
            #endregion

            var ListItems = new List<string>();
            ListItems.AddRange(items);
            ListItems.RemoveAt(0);
            return String.Join(separate_string, ListItems.ToArray());
        }
        #endregion

        #region 0204
        private static string Edit0204(string[] sItems)
        {
            DOC_ID = "64009-1";
            ITEM_COUNT = 285;
            var items = new string[ITEM_COUNT + 1];
            for (int iIdx = 1; iIdx < items.Length; iIdx++)
                items[iIdx] = string.Empty;

            // 帳票ID取得
            var entry_doc_id = sItems[1];

            if (entry_doc_id.Equals(DOC_ID))
            {
                items[1] = "31";
                items[2] = sItems[1].PadRight(7);
                items[3] = "E19";
            }
            else
            {
                items[1] = "07";
                items[2] = sItems[1].PadRight(5);
                items[3] = "E00";
            }
            items[4] = sItems[2].PadRight(10);
            items[5] = sItems[3].PadRight(3);
//            if (sItems[4].Length != 0)
                items[6] = sItems[4].PadLeft(5, '0');
//            else
//                items[6] = sItems[4].PadRight(5);
            items[7] = sItems[5].PadRight(4); 
            items[8] = sItems[6].PadRight(16); 
            items[9] = sItems[7].PadRight(5); 
            items[10] = sItems[8].PadRight(3); 
            items[11] = sItems[9].PadRight(16,'　');
            items[12] = sItems[10].PadRight(16);

            #region Group02-01
            items[13] = sItems[11].PadRight(4);
            items[14] = sItems[12].PadRight(4);
            items[15] = sItems[13].PadRight(5);
            items[16] = sItems[14].PadRight(3);
            items[17] = sItems[15].PadRight(5);
            items[18] = GetHaraikomiHoho_0204(sItems,16);//16,7,8,9,20,1,2,3,4,5
            items[19] = Get1or2(sItems,26);//26,27
            items[20] = Get1or2(sItems,28);//28,29
            items[21] = sItems[30].PadRight(5);
            items[22] = sItems[31].PadRight(3);
            items[23] = sItems[32].PadRight(9);
            items[24] = GetJikoTokutei_0102(sItems,33);//33,34
            items[25] = GetJimuhiUmu_0102(sItems,35);//35,6,7,8
            items[26] = sItems[39].PadRight(9);
            items[27] = sItems[40].PadRight(9);
            items[28] = sItems[41].PadRight(3);
            #endregion

            #region Group02-02
            items[29] = sItems[42].PadRight(4);
            items[30] = sItems[43].PadRight(4);
            items[31] = sItems[44].PadRight(5);
            items[32] = sItems[45].PadRight(3);
            items[33] = sItems[46].PadRight(5);
            items[34] = GetHaraikomiHoho_0204(sItems,47);//47,8,9,0,1,2,3,4,5,6
            items[35] = Get1or2(sItems,57);
            items[36] = Get1or2(sItems,59);
            items[37] = sItems[61].PadRight(5);
            items[38] = sItems[62].PadRight(3);
            items[39] = sItems[63].PadRight(9);
            items[40] = GetJikoTokutei_0102(sItems,64);//64,5
            items[41] = GetJimuhiUmu_0102(sItems,66);//66,7,8,9
            items[42] = sItems[70].PadRight(9);
            items[43] = sItems[71].PadRight(9);
            items[44] = sItems[72].PadRight(3);
            #endregion

            #region Group02-03
            items[45] = sItems[73].PadRight(4);
            items[46] = sItems[74].PadRight(4);
            items[47] = sItems[75].PadRight(5);
            items[48] = sItems[76].PadRight(3);
            items[49] = sItems[77].PadRight(5);
            items[50] = GetHaraikomiHoho_0204(sItems,78);//78,9,0,1,2,3,4,5,6,7
            items[51] = Get1or2(sItems,88);
            items[52] = Get1or2(sItems,90);
            items[53] = sItems[92].PadRight(5);
            items[54] = sItems[93].PadRight(3);
            items[55] = sItems[94].PadRight(9);
            items[56] = GetJikoTokutei_0102(sItems,95);//95,6
            items[57] = GetJimuhiUmu_0102(sItems,97);//97,8,9,80
            items[58] = sItems[101].PadRight(9);
            items[59] = sItems[102].PadRight(9);
            items[60] = sItems[103].PadRight(3);
            #endregion

            #region Group02-04
            items[61] = sItems[104].PadRight(4);
            items[62] = sItems[105].PadRight(4);
            items[63] = sItems[106].PadRight(5);
            items[64] = sItems[107].PadRight(3);
            items[65] = sItems[108].PadRight(5);
            items[66] = GetHaraikomiHoho_0204(sItems,109);//109,0,1,2,3,4,5,6,7,8
            items[67] = Get1or2(sItems,119);//119,20
            items[68] = Get1or2(sItems,121);//121,22
            items[69] = sItems[123].PadRight(5);
            items[70] = sItems[124].PadRight(3);
            items[71] = sItems[125].PadRight(9);
            items[72] = GetJikoTokutei_0102(sItems,126);//126,7
            items[73] = GetJimuhiUmu_0102(sItems,128);//128,9,0,1
            items[74] = sItems[132].PadRight(9);
            items[75] = sItems[133].PadRight(9);
            items[76] = sItems[134].PadRight(3);
            #endregion

            #region Group02-05
            items[77] = sItems[135].PadRight(4);
            items[78] = sItems[136].PadRight(4);
            items[79] = sItems[137].PadRight(5);
            items[80] = sItems[138].PadRight(3);
            items[81] = sItems[139].PadRight(5);
            items[82] = GetHaraikomiHoho_0204(sItems,140);//140,1,2,3,4,5,6,7,8,9
            items[83] = Get1or2(sItems,150);//150,1
            items[84] = Get1or2(sItems,152);//152,3
            items[85] = sItems[154].PadRight(5);
            items[86] = sItems[155].PadRight(3);
            items[87] = sItems[156].PadRight(9);
            items[88] = GetJikoTokutei_0102(sItems,157);//157,8
            items[89] = GetJimuhiUmu_0102(sItems,159);//159,0,1,2
            items[90] = sItems[163].PadRight(9);
            items[91] = sItems[164].PadRight(9);
            items[92] = sItems[165].PadRight(3);
            #endregion

            #region Group02-06
            items[93] = sItems[166].PadRight(4);
            items[94] = sItems[167].PadRight(4);
            items[95] = sItems[168].PadRight(5);
            items[96] = sItems[169].PadRight(3);
            items[97] = sItems[170].PadRight(5);
            items[98] = GetHaraikomiHoho_0204(sItems,171);//171,2,3,4,5,6,7,8,9,0
            items[99] = Get1or2(sItems,181);//181,2
            items[100] = Get1or2(sItems,183);//183,4
            items[101] = sItems[185].PadRight(5);
            items[102] = sItems[186].PadRight(3);
            items[103] = sItems[187].PadRight(9);
            items[104] = GetJikoTokutei_0102(sItems,188);//188,189
            items[105] = GetJimuhiUmu_0102(sItems,190);//190,1,2,3
            items[106] = sItems[194].PadRight(9);
            items[107] = sItems[195].PadRight(9);
            items[108] = sItems[196].PadRight(3);
            #endregion

            #region Group02-07
            items[109] = sItems[197].PadRight(4);
            items[110] = sItems[198].PadRight(4);
            items[111] = sItems[199].PadRight(5);
            items[112] = sItems[200].PadRight(3);
            items[113] = sItems[201].PadRight(5);
            items[114] = GetHaraikomiHoho_0204(sItems,202);//202,3,4,5,6,7,8,9,0,1
            items[115] = Get1or2(sItems,212);//212,3
            items[116] = Get1or2(sItems,214);//214,5
            items[117] = sItems[216].PadRight(5);
            items[118] = sItems[217].PadRight(3);
            items[119] = sItems[218].PadRight(9);
            items[120] = GetJikoTokutei_0102(sItems,219);//219,20
            items[121] = GetJimuhiUmu_0102(sItems,221);//221,2,3,4
            items[122] = sItems[225].PadRight(9);
            items[123] = sItems[226].PadRight(9);
            items[124] = sItems[227].PadRight(3);
            #endregion

            #region Group02-08
            items[125] = sItems[228].PadRight(4);
            items[126] = sItems[229].PadRight(4);
            items[127] = sItems[230].PadRight(5);
            items[128] = sItems[231].PadRight(3);
            items[129] = sItems[232].PadRight(5);
            items[130] = GetHaraikomiHoho_0204(sItems,233);//233,4,5,6,7,8,9,0,1,2
            items[131] = Get1or2(sItems,243);//243,44
            items[132] = Get1or2(sItems,245);//245,6
            items[133] = sItems[247].PadRight(5);
            items[134] = sItems[248].PadRight(3);
            items[135] = sItems[249].PadRight(9);
            items[136] = GetJikoTokutei_0102(sItems,250);//250,1
            items[137] = GetJimuhiUmu_0102(sItems,252);//252,3,4,5
            items[138] = sItems[256].PadRight(9);
            items[139] = sItems[257].PadRight(9);
            items[140] = sItems[258].PadRight(3);
            #endregion

            #region Group02-09
            items[141] = sItems[259].PadRight(4);
            items[142] = sItems[260].PadRight(4);
            items[143] = sItems[261].PadRight(5);
            items[144] = sItems[262].PadRight(3);
            items[145] = sItems[263].PadRight(5);
            items[146] = GetHaraikomiHoho_0204(sItems,264);//264,5,6,7,8,9,0,1,2,3
            items[147] = Get1or2(sItems,274);//274,5
            items[148] = Get1or2(sItems,276);//276,7
            items[149] = sItems[278].PadRight(5);
            items[150] = sItems[279].PadRight(3);
            items[151] = sItems[280].PadRight(9);
            items[152] = GetJikoTokutei_0102(sItems,281);//281,2
            items[153] = GetJimuhiUmu_0102(sItems,283);//283,4,5,6
            items[154] = sItems[287].PadRight(9);
            items[155] = sItems[288].PadRight(9);
            items[156] = sItems[289].PadRight(3);
            #endregion

            #region Group02-10
            items[157] = sItems[290].PadRight(4);
            items[158] = sItems[291].PadRight(4);
            items[159] = sItems[292].PadRight(5);
            items[160] = sItems[293].PadRight(3);
            items[161] = sItems[294].PadRight(5);
            items[162] = GetHaraikomiHoho_0204(sItems,295);//295,6,7,8,9,0,1,2,3,4
            items[163] = Get1or2(sItems,305);//305,6
            items[164] = Get1or2(sItems,307);//307,8
            items[165] = sItems[309].PadRight(5);
            items[166] = sItems[310].PadRight(3);
            items[167] = sItems[311].PadRight(9);
            items[168] = GetJikoTokutei_0102(sItems,312);//312,3
            items[169] = GetJimuhiUmu_0102(sItems,314);//314,5,6,7
            items[170] = sItems[318].PadRight(9);
            items[171] = sItems[319].PadRight(9);
            items[172] = sItems[320].PadRight(3);
            #endregion

            #region Group02-11
            items[173] = sItems[321].PadRight(4);
            items[174] = sItems[322].PadRight(4);
            items[175] = sItems[323].PadRight(5);
            items[176] = sItems[324].PadRight(3);
            items[177] = sItems[325].PadRight(5);
            items[178] = GetHaraikomiHoho_0204(sItems,326);//326,7,8,9,0,1,2,3,4,5
            items[179] = Get1or2(sItems,336);//336,7
            items[180] = Get1or2(sItems,338);//338,9
            items[181] = sItems[340].PadRight(5);
            items[182] = sItems[341].PadRight(3);
            items[183] = sItems[342].PadRight(9);
            items[184] = GetJikoTokutei_0102(sItems,343);//343,4
            items[185] = GetJimuhiUmu_0102(sItems,345);//345,6,7,8
            items[186] = sItems[349].PadRight(9);
            items[187] = sItems[350].PadRight(9);
            items[188] = sItems[351].PadRight(3);
            #endregion

            #region Group02-12
            items[189] = sItems[352].PadRight(4);
            items[190] = sItems[353].PadRight(4);
            items[191] = sItems[354].PadRight(5);
            items[192] = sItems[355].PadRight(3);
            items[193] = sItems[356].PadRight(5);
            items[194] = GetHaraikomiHoho_0204(sItems,357);//357,8,9,0,1,2,3,4,5,6
            items[195] = Get1or2(sItems,367);//367,8
            items[196] = Get1or2(sItems,369);//369,0
            items[197] = sItems[371].PadRight(5);
            items[198] = sItems[372].PadRight(3);
            items[199] = sItems[373].PadRight(9);
            items[200] = GetJikoTokutei_0102(sItems,374);//374,5
            items[201] = GetJimuhiUmu_0102(sItems,376);//376,7,8,9
            items[202] = sItems[380].PadRight(9);
            items[203] = sItems[381].PadRight(9);
            items[204] = sItems[382].PadRight(3);
            #endregion

            #region Group02-13
            items[205] = sItems[383].PadRight(4);
            items[206] = sItems[384].PadRight(4);
            items[207] = sItems[385].PadRight(5);
            items[208] = sItems[386].PadRight(3);
            items[209] = sItems[387].PadRight(5);
            items[210] = GetHaraikomiHoho_0204(sItems,388);//388,9,0,1,2,3,4,5,6,7
            items[211] = Get1or2(sItems,398);//398,9
            items[212] = Get1or2(sItems,400);//400,1
            items[213] = sItems[402].PadRight(5);
            items[214] = sItems[403].PadRight(3);
            items[215] = sItems[404].PadRight(9);
            items[216] = GetJikoTokutei_0102(sItems,405);//405,6
            items[217] = GetJimuhiUmu_0102(sItems,407);//407,8,9,0
            items[218] = sItems[411].PadRight(9);
            items[219] = sItems[412].PadRight(9);
            items[220] = sItems[413].PadRight(3);
            #endregion

            #region Group02-14
            items[221] = sItems[414].PadRight(4);
            items[222] = sItems[415].PadRight(4);
            items[223] = sItems[416].PadRight(5);
            items[224] = sItems[417].PadRight(3);
            items[225] = sItems[418].PadRight(5);
            items[226] = GetHaraikomiHoho_0204(sItems,419);//419,0,1,2,3,4,5,6,7,8
            items[227] = Get1or2(sItems,429);//429,30
            items[228] = Get1or2(sItems,431);//431,2
            items[229] = sItems[433].PadRight(5);
            items[230] = sItems[434].PadRight(3);
            items[231] = sItems[435].PadRight(9);
            items[232] = GetJikoTokutei_0102(sItems,436);//436,7
            items[233] = GetJimuhiUmu_0102(sItems,438);//438,9,0,1
            items[234] = sItems[442].PadRight(9);
            items[235] = sItems[443].PadRight(9);
            items[236] = sItems[444].PadRight(3);
            #endregion

            #region Group02-15
            items[237] = sItems[445].PadRight(4);
            items[238] = sItems[446].PadRight(4);
            items[239] = sItems[447].PadRight(5);
            items[240] = sItems[448].PadRight(3);
            items[241] = sItems[449].PadRight(5);
            items[242] = GetHaraikomiHoho_0204(sItems,450);//450,1,2,3,4,5,6,7,8,9
            items[243] = Get1or2(sItems,460);//460,1
            items[244] = Get1or2(sItems,462);//462,3
            items[245] = sItems[464].PadRight(5);
            items[246] = sItems[465].PadRight(3);
            items[247] = sItems[466].PadRight(9);
            items[248] = GetJikoTokutei_0102(sItems,467);//467,8
            items[249] = GetJimuhiUmu_0102(sItems,469);//469,0,1,2
            items[250] = sItems[473].PadRight(9);
            items[251] = sItems[474].PadRight(9);
            items[252] = sItems[475].PadRight(3);
            #endregion

            #region Group03
            items[253] = GetDate(entry_doc_id, sItems, 476);//476,7,8
            items[254] = GetDate(entry_doc_id, sItems, 479, 4);
            items[255] = GetDate(entry_doc_id, sItems, 482);
            items[256] = GetDate(entry_doc_id, sItems, 485, 4);
            items[257] = GetDate(entry_doc_id, sItems, 488);
            items[258] = GetDate(entry_doc_id, sItems, 491, 4);
            items[259] = GetDate(entry_doc_id, sItems, 494);
            items[260] = GetDate(entry_doc_id, sItems, 497, 4);
            items[261] = GetDate(entry_doc_id, sItems, 500);
            items[262] = GetDate(entry_doc_id, sItems, 503, 4);
            items[263] = GetDate(entry_doc_id, sItems, 506);
            items[264] = GetDate(entry_doc_id, sItems, 509, 4);
            items[265] = GetDate(entry_doc_id, sItems, 512);
            items[266] = GetDate(entry_doc_id, sItems, 515, 4);
//            if (sItems[518].Length!=0)
                items[267] = sItems[518].PadLeft(2,'0');
//            else
//            items[267] = sItems[518].PadLeft(2);
            #endregion

            #region Group04
            items[268] = sItems[519].PadRight(1);

            var lst = new List<string>();
            if (sItems[520].Length != 0)
                lst.Add(sItems[520]);
            if (sItems[521].Length != 0)
                lst.Add(sItems[521]);
            if (sItems[522].Length != 0)
                lst.Add(sItems[522]);
            if (sItems[523].Length != 0)
                if (lst.Count < 3)
                    lst.Add(sItems[523]);
            items[269] = String.Join(" ", lst.ToArray()).PadRight(10);
            #endregion

            items[270] = string.Empty.PadRight(1);
            items[271] = string.Empty.PadRight(1);

            #region Group05
            items[272] = sItems[524].PadRight(13);
//            if (sItems[522].Length != 0)
                items[273] = sItems[525].PadLeft(5, '0');
//            else
//                items[273] = sItems[522].PadRight(5);
            items[274] = sItems[526].PadRight(5);
            items[275] = GetDate(entry_doc_id, sItems,527);
            items[276] = sItems[530].PadRight(3);
            items[277] = sItems[531].PadRight(3);
            items[278] = sItems[532].PadRight(15);
            items[279] = sItems[533].PadRight(30);
            items[280] = string.Empty.PadRight(14);
            items[281] = sItems[534].PadRight(4);
            items[282] = sItems[535].PadRight(4);
            items[283] = sItems[536].PadRight(1);
            items[284] = sItems[537].PadRight(30);
            items[285] = sItems[538].PadRight(4);
            #endregion

            var ListItems = new List<string>();
            ListItems.AddRange(items);
            ListItems.RemoveAt(0);
            return String.Join(separate_string, ListItems.ToArray());
        }
        #endregion

        #region 0205
        private static string Edit0205(string[] sItems)
        {

            sItems[0] = string.Empty;
            DOC_ID = "61921";
            ITEM_COUNT = 175;
            var items = new string[ITEM_COUNT + 1];
            for (int iIdx = 1; iIdx < items.Length; iIdx++)
                items[iIdx] = string.Empty;

            // 帳票ID取得
            var entry_doc_id = sItems[1];
            items[1] = "09";
            items[2] = sItems[1].PadRight(5);
            items[3] = "E00";
            items[4] = sItems[2].PadRight(3);
            items[5] = sItems[3].PadRight(10);
            items[6] = sItems[4].PadRight(3);
            items[7] = sItems[5].PadRight(1);
            items[8] = sItems[6].PadRight(1);
            items[9] = sItems[7].PadRight(1);
            items[10] = sItems[8].PadRight(1);

            items[11] = sItems[9].PadRight(4);
            items[12] = GetHaraikomiHoho0205(sItems, 10);
            items[13] = Get1or2(sItems, 19);
            items[14] = Get1or2(sItems, 21);

            #region Group02-01
            items[15] = sItems[23].PadRight(3);
            items[16] = EditXX(sItems[24], 9);
            items[17] = string.Empty.PadRight(1);
            items[18] = sItems[25].PadRight(5);
            items[19] = sItems[26].PadRight(3);
            items[20] = EditXX(sItems[27], 9);
            items[21] = GetJikoTokutei_0102(sItems, 28);
            items[22] = EditXX(sItems[30], 9);
            #endregion

            items[23] = sItems[31].PadRight(3);
            items[24] = EditXX(sItems[32], 9);
            items[25] = string.Empty.PadRight(1);
            items[26] = sItems[33].PadRight(5);
            items[27] = sItems[34].PadRight(3);
            items[28] = EditXX(sItems[35], 9);
            items[29] = GetJikoTokutei_0102(sItems,36 );
            items[30] = EditXX(sItems[38], 9);

            items[31] = sItems[39].PadRight(3);
            items[32] = EditXX(sItems[40], 9);
            items[33] = string.Empty.PadRight(1);
            items[34] = sItems[41].PadRight(5);
            items[35] = sItems[42].PadRight(3);
            items[36] = EditXX(sItems[43], 9);
            items[37] = GetJikoTokutei_0102(sItems,44 );
            items[38] = EditXX(sItems[46], 9);

            items[39] = sItems[47].PadRight(3);
            items[40] = EditXX(sItems[48], 9);
            items[41] = string.Empty.PadRight(1);
            items[42] = sItems[49].PadRight(5);
            items[43] = sItems[50].PadRight(3);
            items[44] = EditXX(sItems[51], 9);
            items[45] = GetJikoTokutei_0102(sItems,52 );
            items[46] = EditXX(sItems[54], 9);

            items[47] = sItems[55].PadRight(3);
            items[48] = EditXX(sItems[56], 9);
            items[49] = string.Empty.PadRight(1);
            items[50] = sItems[57].PadRight(5);
            items[51] = sItems[58].PadRight(3);
            items[52] = EditXX(sItems[59], 9);
            items[53] = GetJikoTokutei_0102(sItems, 60);
            items[54] = EditXX(sItems[62], 9);

            items[55] = sItems[63].PadRight(3);
            items[56] = EditXX(sItems[64], 9);
            items[57] = string.Empty.PadRight(1);
            items[58] = sItems[65].PadRight(5);
            items[59] = sItems[66].PadRight(3);
            items[60] = EditXX(sItems[67], 9);
            items[61] = GetJikoTokutei_0102(sItems,68 );
            items[62] = EditXX(sItems[70], 9);

            items[63] = sItems[71].PadRight(3);
            items[64] = EditXX(sItems[72], 9);
            items[65] = string.Empty.PadRight(1);
            items[66] = sItems[73].PadRight(5);
            items[67] = sItems[74].PadRight(3);
            items[68] = EditXX(sItems[75], 9);
            items[69] = GetJikoTokutei_0102(sItems, 76);
            items[70] = EditXX(sItems[78], 9);

            items[71] = sItems[79].PadRight(3);
            items[72] = EditXX(sItems[80], 9);
            items[73] = string.Empty.PadRight(1);
            items[74] = sItems[81].PadRight(5);
            items[75] = sItems[82].PadRight(3);
            items[76] = EditXX(sItems[83], 9);
            items[77] = GetJikoTokutei_0102(sItems, 84);
            items[78] = EditXX(sItems[86], 9);

            items[79] = sItems[87].PadRight(3);
            items[80] = EditXX(sItems[88], 9);
            items[81] = string.Empty.PadRight(1);
            items[82] = sItems[89].PadRight(5);
            items[83] = sItems[90].PadRight(3);
            items[84] = EditXX(sItems[91], 9);
            items[85] = GetJikoTokutei_0102(sItems,92 );
            items[86] = EditXX(sItems[94], 9);

            items[87] = sItems[95].PadRight(3);
            items[88] = EditXX(sItems[96], 9);
            items[89] = string.Empty.PadRight(1);
            items[90] = sItems[97].PadRight(5);
            items[91] = sItems[98].PadRight(3);
            items[92] = EditXX(sItems[99], 9);
            items[93] = GetJikoTokutei_0102(sItems,100 );
            items[94] = EditXX(sItems[102], 9);

//            帳票に存在しない

                        items[95] = sItems[0].PadRight(1);
                        items[96] = sItems[0].PadRight(4);
                        items[97] = sItems[0].PadRight(3);
                        items[98] = sItems[0].PadRight(9);
                        items[99] = sItems[0].PadRight(13);
                        items[100] = sItems[0].PadRight(3);
                        items[101] = sItems[0].PadRight(9);
                        items[102] = sItems[0].PadRight(13);
                        items[103] = sItems[0].PadRight(3);
                        items[104] = sItems[0].PadRight(9);
                        items[105] = sItems[0].PadRight(13);
                        items[106] = sItems[0].PadRight(3);
                        items[107] = sItems[0].PadRight(9);
                        items[108] = sItems[0].PadRight(13);
                        items[109] = sItems[0].PadRight(3);
                        items[110] = sItems[0].PadRight(9);
                        items[111] = sItems[0].PadRight(13);
                        items[112] = sItems[0].PadRight(3);
                        items[113] = sItems[0].PadRight(9);
                        items[114] = sItems[0].PadRight(13);
                        items[115] = sItems[0].PadRight(3);
                        items[116] = sItems[0].PadRight(9);
                        items[117] = sItems[0].PadRight(13);
                        items[118] = sItems[0].PadRight(3);
                        items[119] = sItems[0].PadRight(9);
                        items[120] = sItems[0].PadRight(13);
                        items[121] = sItems[0].PadRight(3);
                        items[122] = sItems[0].PadRight(9);
                        items[123] = sItems[0].PadRight(13);
                        items[124] = sItems[0].PadRight(3);
                        items[125] = sItems[0].PadRight(9);
                        items[126] = sItems[0].PadRight(13);
            
            items[127] = EditXX(sItems[135], 9);
            items[128] = string.Empty.PadRight(1);

            items[129] = sItems[136].PadRight(5);
            items[130] = sItems[137].PadRight(3);
            items[131] = EditXX(sItems[138],9);
            items[132] = string.Empty.PadRight(1);

            items[133] = sItems[139].PadRight(5);
            items[134] = sItems[140].PadRight(3);
            items[135] = EditXX(sItems[141], 9);
            items[136] = string.Empty.PadRight(1);

            items[137] = sItems[142].PadRight(5);
            items[138] = sItems[143].PadRight(3);
            items[139] = EditXX(sItems[144], 9);
            items[140] = string.Empty.PadRight(1);

            items[141] = sItems[145].PadRight(5);
            items[142] = sItems[146].PadRight(3);
            items[143] = EditXX(sItems[147], 9);
            items[144] = string.Empty.PadRight(1);

            items[145] = sItems[148].PadRight(5);
            items[146] = sItems[149].PadRight(3);
            items[147] = EditXX(sItems[150], 9);
            items[148] = string.Empty.PadRight(1);

            items[149] = sItems[151].PadRight(5);
            items[150] = sItems[152].PadRight(3);
            items[151] = EditXX(sItems[153], 9);
            items[152] = string.Empty.PadRight(1);

            items[153] = sItems[154].PadRight(5);
            items[154] = sItems[155].PadRight(3);
            items[155] = EditXX(sItems[156], 9);
            items[156] = string.Empty.PadRight(1);

            items[157] = sItems[157].PadRight(5);
            items[158] = sItems[158].PadRight(3);
            items[159] = EditXX(sItems[159], 9);
            items[160] = string.Empty.PadRight(1);

            items[161] = sItems[160].PadRight(5);
            items[162] = sItems[161].PadRight(3);
            items[163] = EditXX(sItems[162], 9);
            items[164] = string.Empty.PadRight(1);

            items[165] = sItems[163].PadRight(5);
            items[166] = sItems[164].PadRight(3);
            items[167] = EditXX(sItems[165], 9);
            items[168] = string.Empty.PadRight(1);

            items[169] = sItems[166].PadRight(5);
            items[170] = sItems[167].PadRight(3);
            items[171] = EditXX(sItems[168], 9);
            items[172] = string.Empty.PadRight(1);

            items[173] = sItems[169].PadRight(5);
            items[174] = sItems[170].PadRight(3);
            items[175] = EditXX(sItems[161], 9);

            var ListItems = new List<string>();
            ListItems.AddRange(items);
            ListItems.RemoveAt(0);
            return String.Join(separate_string, ListItems.ToArray());
        }
        #endregion

        #region 0206
        private static string Edit0206(string[] sItems)
        {
            DOC_ID = "65374-3";
            ITEM_COUNT = 222;
            var items = new string[ITEM_COUNT + 1];
            for (int iIdx = 1; iIdx < items.Length; iIdx++)
                items[iIdx] = string.Empty;

            // 帳票ID取得
            var entry_doc_id = sItems[1];
            if (entry_doc_id.Equals(DOC_ID))
            {
                items[1] = "37";
                items[2] = sItems[1].PadRight(7);
                items[3] = "E19";
            }
            else
            {
                items[1] = "18";
                items[2] = sItems[1].PadRight(5);
                items[3] = "E16";
            }
            items[4] = sItems[2].PadRight(1);
            items[5] = sItems[3].PadRight(10);
            items[6] = sItems[4].PadRight(3);
            items[7] = sItems[5].PadRight(4);
            items[8] = sItems[6].PadRight(16);
            items[9] = sItems[7].PadRight(5);
            items[10] = sItems[8].PadRight(3);
            items[11] = sItems[9].PadRight(5);
            items[12] = string.Empty.PadRight(1);
            items[13] = string.Empty.PadRight(1);
            items[14] = GetDate2(entry_doc_id, sItems, 10);
            items[15] = sItems[13].PadRight(7);
            items[16] = sItems[14].PadRight(20);
            items[17] = sItems[15].PadRight(1);
            items[18] = sItems[16].PadRight(15, '　');
            items[19] = sItems[17].PadRight(15, '　');
            items[20] = sItems[18].PadRight(15, '　');
            items[21] = sItems[19].PadRight(22);
            items[22] = sItems[20].PadRight(22);
            items[23] = sItems[21].PadRight(22);
            items[24] = sItems[22].PadRight(15, '　');
            items[25] = sItems[23].PadRight(15, '　');
            items[26] = sItems[24].PadRight(15, '　');
            items[27] = sItems[25].PadRight(50);
            items[28] = sItems[26].PadRight(30);
            items[29] = sItems[27].PadRight(20);
            items[30] = sItems[28].PadRight(20);
            items[31] = sItems[29].PadRight(20);
            items[32] = sItems[30].PadRight(20);

            var lst = new List<string>();
            if (sItems[30].Length != 0)
                lst.Add(sItems[30]);
            if (sItems[31].Length != 0)
                lst.Add(sItems[31]);
            if (sItems[32].Length != 0)
                lst.Add(sItems[32]);

            if (sItems[33].Length != 0)
                if (lst.Count<3)
                    lst.Add(sItems[33]);
            if (sItems[34].Length != 0)
                if (lst.Count < 3)
                    lst.Add(sItems[34]);
            if (sItems[35].Length != 0)
                if (lst.Count < 3)
                    lst.Add(sItems[35]);
            items[33] = String.Join(" ", lst.ToArray()).PadRight(10);

            items[34] = GetKeiyakuHoshiki(sItems, 36);
            items[35] = Get1or2or3or4(sItems, 38);
            items[36] = Get1or2or9(sItems, 42);

            items[37] = Get1or2(sItems, 45);
            items[38] = Get1or2or3or4(sItems, 47);
            items[39] = Get1or2(sItems, 51);
            items[40] = sItems[53].PadRight(1);

            items[41] = Get1or2(sItems, 54); 
            items[42] = Get1or2or3or4(sItems, 56);
            items[43] = sItems[60].PadRight(1);
            items[44] = sItems[61].PadRight(1);
            items[45] = sItems[62].PadRight(1);
            items[46] = sItems[63].PadRight(1);
            items[47] = sItems[64].PadRight(1);
            items[48] = sItems[65].PadRight(1);
            items[49] = sItems[66].PadRight(1);
            items[50] = sItems[67].PadRight(1);
            items[51] = sItems[68].PadRight(1);

            items[52] = GetDate2(entry_doc_id, sItems, 69, 4);
            items[53] = GetDate2(entry_doc_id, sItems, 72, 4);
            items[54] = sItems[75].PadRight(1);
            items[55] = Get2or3(sItems, 76);
            items[56] = sItems[78].PadRight(1);
            items[57] = sItems[79].PadRight(1);
            items[58] = string.Empty.PadRight(1);

            #region Group09
            items[59] = GetDate2(entry_doc_id, sItems, 80);
            items[60] = Get1or2(sItems, 83);
            items[61] = sItems[85].Length != 0 ? sItems[85].PadLeft(2, '0') : string.Empty.PadRight(2);
            items[62] = GetDate2(entry_doc_id, sItems, 86);
            items[63] = Get1or2(sItems, 89);
            items[64] = sItems[91].Length != 0 ? sItems[91].PadLeft(2, '0') : string.Empty.PadRight(2);
            items[65] = GetHokenKikan(sItems, 92);
            #endregion

            #region Group10
            #region Group10-01
            items[66] = sItems[96].PadRight(4);
            items[67] = GetHaraikomiHoho0206(sItems, 97);
            items[68] = EditXX(sItems[105], 13);
            #endregion

            items[69] = sItems[106].PadRight(4);
            items[70] = GetHaraikomiHoho0206(sItems, 107);
            items[71] = EditXX(sItems[115], 13);

            items[72] = sItems[116].PadRight(4);
            items[73] = GetHaraikomiHoho0206(sItems, 117);
            items[74] = EditXX(sItems[125], 13);

            items[75] = sItems[126].PadRight(4);
            items[76] = GetHaraikomiHoho0206(sItems, 127);
            items[77] = EditXX(sItems[135], 13);

            items[78] = sItems[136].PadRight(4);
            items[79] = GetHaraikomiHoho0206(sItems, 137);
            items[80] = EditXX(sItems[145], 13);

            items[81] = sItems[146].PadRight(4);
            items[82] = GetHaraikomiHoho0206(sItems, 147);
            items[83] = EditXX(sItems[155], 13);

            items[84] = sItems[156].PadRight(4);
            items[85] = GetHaraikomiHoho0206(sItems, 157);
            items[86] = EditXX(sItems[165], 13);

            items[87] = sItems[166].PadRight(4);
            items[88] = GetHaraikomiHoho0206(sItems, 167);
            items[89] = EditXX(sItems[175], 13);

            items[90] = sItems[176].PadRight(4);
            items[91] = GetHaraikomiHoho0206(sItems, 177);
            items[92] = EditXX(sItems[185], 13);

            items[93] = sItems[186].PadRight(4);
            items[94] = GetHaraikomiHoho0206(sItems, 187);
            items[95] = EditXX(sItems[195], 13);

            items[96] = sItems[196].PadRight(4);
            items[97] = GetHaraikomiHoho0206(sItems, 197);
            items[98] = EditXX(sItems[205], 13);

            items[99] = sItems[206].PadRight(4);
            items[100] = GetHaraikomiHoho0206(sItems, 207);
            items[101] = EditXX(sItems[215], 13);

            items[102] = sItems[216].PadRight(4);
            items[103] = GetHaraikomiHoho0206(sItems, 217);
            items[104] = EditXX(sItems[225], 13);

            items[105] = sItems[226].PadRight(4);
            items[106] = GetHaraikomiHoho0206(sItems, 227);
            items[107] = EditXX(sItems[235], 13);

            items[108] = sItems[236].PadRight(4);
            items[109] = GetHaraikomiHoho0206(sItems, 237);
            items[110] = EditXX(sItems[245], 13);

            items[111] = sItems[246].PadRight(4);
            items[112] = GetHaraikomiHoho0206(sItems, 247);
            items[113] = EditXX(sItems[255], 13);

            items[114] = sItems[256].PadRight(4);
            items[115] = GetHaraikomiHoho0206(sItems, 257);
            items[116] = EditXX(sItems[265], 13);

            items[117] = sItems[266].PadRight(4);
            items[118] = GetHaraikomiHoho0206(sItems, 267);
            items[119] = EditXX(sItems[275], 13);

            items[120] = sItems[276].PadRight(4);
            items[121] = GetHaraikomiHoho0206(sItems, 277);
            items[122] = EditXX(sItems[285], 13);

            items[123] = sItems[286].PadRight(4);
            items[124] = GetHaraikomiHoho0206(sItems, 287);
            items[125] = EditXX(sItems[295], 13);

            items[126] = sItems[296].PadRight(4);
            items[127] = GetHaraikomiHoho0206(sItems, 297);
            items[128] = EditXX(sItems[305], 13);

            items[129] = sItems[306].PadRight(4);
            items[130] = GetHaraikomiHoho0206(sItems, 307);
            items[131] = EditXX(sItems[315], 13);

            items[132] = sItems[316].PadRight(4);
            items[133] = GetHaraikomiHoho0206(sItems, 317);
            items[134] = EditXX(sItems[325], 13);

            items[135] = sItems[326].PadRight(4);
            items[136] = GetHaraikomiHoho0206(sItems, 327);
            items[137] = EditXX(sItems[335], 13);

            items[138] = sItems[336].PadRight(4);
            items[139] = GetHaraikomiHoho0206(sItems, 337);
            items[140] = EditXX(sItems[345], 13);

            items[141] = sItems[346].PadRight(4);
            items[142] = GetHaraikomiHoho0206(sItems, 347);
            items[143] = EditXX(sItems[355], 13);

            items[144] = sItems[356].PadRight(4);
            items[145] = GetHaraikomiHoho0206(sItems, 357);
            items[146] = EditXX(sItems[365], 13);

            items[147] = sItems[366].PadRight(4);
            items[148] = GetHaraikomiHoho0206(sItems, 367);
            items[149] = EditXX(sItems[375], 13);

            items[150] = sItems[376].PadRight(4);
            items[151] = GetHaraikomiHoho0206(sItems, 377);
            items[152] = EditXX(sItems[385], 13);

            items[153] = sItems[386].PadRight(4);
            items[154] = GetHaraikomiHoho0206(sItems, 387);
            items[155] = EditXX(sItems[395], 13);

            // 合計保険料
            items[156] = EditXX(sItems[396], 13);
            #endregion

            #region Group11
            // 払込方法（団体）
            items[157] = GetHaraikomiHohoDantai0206(sItems, 397);
            items[158] = GetDate2(entry_doc_id, sItems, 402);

            items[159] = GetDate2(entry_doc_id, sItems, 405);
            items[160] = GetDate2(entry_doc_id, sItems,408);


            items[161] = GetDate2(entry_doc_id, sItems, 411, 4);
            items[162] = string.Empty.PadRight(1);
            items[163] = GetDate2(entry_doc_id, sItems, 414, 4);
            items[164] = string.Empty.PadRight(1);
            items[165] = GetDate2(entry_doc_id, sItems, 417, 4);
            items[166] = string.Empty.PadRight(1);
            items[167] = string.Empty.PadRight(1);
            items[168] = string.Empty.PadRight(1);
            // 払込方法（一般）
            items[169] = GetHaraikomiHohoIppan0206(sItems, 420);
            items[170] = sItems[424].Length != 0? sItems[424].PadLeft(2, '0'):string.Empty.PadRight(2);
            #endregion

            items[171] = Get2or3or4(sItems, 425);
            items[172] = sItems[428].PadRight(13);
            items[173] = GetDate2(entry_doc_id, sItems, 429);
            items[174] = sItems[432].PadRight(15);
            items[175] = Get1or2(sItems, 433);
            items[176] = Get1or2(sItems, 435);
            items[177] = string.Empty.PadRight(2);
            items[178] = Get3or4or5(sItems, 437);
            items[179] = Get1or2or3(sItems, 440);
            items[180] = Get1or2or3(sItems, 443);

            items[181] = Get1or2(sItems, 446);
            items[182] = sItems[448].PadRight(1);
            items[183] = sItems[449].PadRight(15);
            items[184] = sItems[450].PadRight(15);
            items[185] = sItems[451].PadRight(30);
            items[186] = Get1or2(sItems, 452);
            items[187] = sItems[454].PadRight(10);
            items[188] = Get1or2or9(sItems, 455);
            items[189] = Get1or2(sItems, 458);
            items[190] = sItems[460].PadRight(20);
            items[191] = sItems[461].PadRight(10);
            items[192] = sItems[462].PadRight(20);
            items[193] = sItems[463].PadRight(10);

            items[194] = Get1or2or3or4(sItems, 464);
            items[195] = EditXX(sItems[468],9);
            items[196] = EditXX(sItems[469],9);
            items[197] = sItems[470].PadRight(5);
            items[198] = sItems[471].PadRight(3);
            items[199] = string.Empty.PadRight(1);
            items[200] = EditXX( sItems[472],9);

            items[201] = Get1or2or3(sItems, 473);
            items[202] = string.Empty.PadRight(1);
            items[203] = string.Empty.PadRight(1);
            items[204] = sItems[476].PadRight(4);
            items[205] = sItems[477].PadRight(4);
            items[206] = sItems[478].PadRight(5);
            items[207] = sItems[479].PadRight(5);
            items[208] = string.Empty.PadRight(1);
            items[209] = string.Empty.PadRight(1);
            items[210] = string.Empty.PadRight(1);

            items[211] = sItems[480].PadRight(1);
            items[212] = sItems[481].PadRight(1);

            items[213] = sItems[482].PadRight(1);
            items[214] = sItems[483].PadRight(1);
            items[215] = sItems[484].PadRight(1);
            items[216] = string.Empty.PadRight(1);
            items[217] = string.Empty.PadRight(1);

            items[218] = sItems[485].PadRight(3);
            items[219] = GetGroupType(sItems, 486);
            items[220] = sItems[488].PadRight(30);
            items[221] = Get1or2or3(sItems, 489);
            items[222] = sItems[492].PadRight(4);

            var ListItems = new List<string>();
            ListItems.AddRange(items);
            ListItems.RemoveAt(0);
            return String.Join(separate_string, ListItems.ToArray());
        }
        #endregion

        #region 項目編集
        private static string GetRelation(string[] values, int itemNo)
        {
            var Count = 0;
            var value = string.Empty;
            if ("1".Equals(values[itemNo]))
                Count++;
            if ("2".Equals(values[itemNo + 1]))
                Count++;
            if ("3".Equals(values[itemNo + 2]))
                Count++;
            if ("4".Equals(values[itemNo + 3]))
                Count++;
            if ("5".Equals(values[itemNo + 4]))
                Count++;
            if ("6".Equals(values[itemNo + 5]))
                Count++;
            if ("7".Equals(values[itemNo + 6]))
                if (Count == 1)
                {
                    if ("1".Equals(values[itemNo]))
                        value = "1";
                    if ("2".Equals(values[itemNo + 1]))
                        value = "2";
                    if ("3".Equals(values[itemNo + 2]))
                        value = "3";
                    if ("4".Equals(values[itemNo + 3]))
                        value = "4";
                    if ("5".Equals(values[itemNo + 4]))
                        value = "5";
                    if ("6".Equals(values[itemNo + 5]))
                        value = "6";
                    if ("7".Equals(values[itemNo + 6]))
                        value = "7";
                }
                else if (Count == 0)
                {
                    // 未選択
                    value = string.Empty;
                }
                else
                {
                    // 複数選択
                    value = CONST_CHOFUKU;// String.Format("{0}_払込方法_複数設定", n);
                }
            return value.PadRight(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="itemNo"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static string GetHaraikomiHoho_0102(string[] values, int itemNo, int n = 0)
        {
            var Count = 0;
            var value = string.Empty;
            if ("10".Equals(values[itemNo]))
                Count++;
            if ("63".Equals(values[itemNo + 1]))
                Count++;
            if ("62".Equals(values[itemNo + 2]))
                Count++;
            if ("61".Equals(values[itemNo + 3]))
                Count++;
            if ("60".Equals(values[itemNo + 4]))
                Count++;
            if ("46".Equals(values[itemNo + 5]))
                Count++;
            if ("30".Equals(values[itemNo + 6]))
                Count++;
            if ("42".Equals(values[itemNo + 7]))
                Count++;
            if ("75".Equals(values[itemNo + 8]))
                Count++;
            if (Count == 1)
            {
                if ("10".Equals(values[itemNo]))
                    value = "10";
                if ("63".Equals(values[itemNo + 1]))
                    value = "63";
                if ("62".Equals(values[itemNo + 2]))
                    value = "62";
                if ("61".Equals(values[itemNo + 3]))
                    value = "61";
                if ("60".Equals(values[itemNo + 4]))
                    value = "60";
                if ("46".Equals(values[itemNo + 5]))
                    value = "46";
                if ("30".Equals(values[itemNo + 6]))
                    value = "30";
                if ("42".Equals(values[itemNo + 7]))
                    value = "42";
                if ("75".Equals(values[itemNo + 8]))
                    value = "75";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;// String.Format("{0}_払込方法_複数設定", n);
            }
            return value;
        }

        private static string GetHaraikomiHoho0205(string[] values, int itemNo, int n = 0)
        {
            var Count = 0;
            var value = string.Empty;
            if ("10".Equals(values[itemNo]))
                Count++;
            if ("63".Equals(values[itemNo + 1]))
                Count++;
            if ("62".Equals(values[itemNo + 2]))
                Count++;
            if ("61".Equals(values[itemNo + 3]))
                Count++;
            if ("60".Equals(values[itemNo + 4]))
                Count++;
            if ("46".Equals(values[itemNo + 5]))
                Count++;
            if ("75".Equals(values[itemNo + 6]))
                Count++;
            if ("42".Equals(values[itemNo + 7]))
                Count++;
            if ("30".Equals(values[itemNo + 8]))
                Count++;
            if (Count == 1)
            {
                if ("10".Equals(values[itemNo]))
                    value = "10";
                if ("63".Equals(values[itemNo + 1]))
                    value = "63";
                if ("62".Equals(values[itemNo + 2]))
                    value = "62";
                if ("61".Equals(values[itemNo + 3]))
                    value = "61";
                if ("60".Equals(values[itemNo + 4]))
                    value = "60";
                if ("46".Equals(values[itemNo + 5]))
                    value = "46";
                if ("75".Equals(values[itemNo + 6]))
                    value = "75";
                if ("42".Equals(values[itemNo + 7]))
                    value = "42";
                if ("30".Equals(values[itemNo + 8]))
                    value = "30";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;// String.Format("{0}_払込方法_複数設定", n);
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="itemNo"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static string GetHaraikomiHoho_0204(string[] values, int itemNo)
        {
            var Count = 0;
            var value = string.Empty;
            if ("63".Equals(values[itemNo]))
                Count++;
            if ("62".Equals(values[itemNo + 1]))
                Count++;
            if ("61".Equals(values[itemNo + 2]))
                Count++;
            if ("10".Equals(values[itemNo + 3]))
                Count++;
            if ("60".Equals(values[itemNo + 4]))
                Count++;
            if ("46".Equals(values[itemNo + 5]))
                Count++;
            if ("30".Equals(values[itemNo + 6]))
                Count++;
            if ("42".Equals(values[itemNo + 7]))
                Count++;
            if ("75".Equals(values[itemNo + 8]))
                Count++;
            if ("10".Equals(values[itemNo + 9]))
                Count++;
            if (Count == 1)
            {
                if ("63".Equals(values[itemNo]))
                    value = "63";
                if ("62".Equals(values[itemNo + 1]))
                    value = "62";
                if ("61".Equals(values[itemNo + 2]))
                    value = "61";
                if ("10".Equals(values[itemNo + 3]))
                    value = "10";
                if ("60".Equals(values[itemNo + 4]))
                    value = "60";
                if ("46".Equals(values[itemNo + 5]))
                    value = "46";
                if ("30".Equals(values[itemNo + 6]))
                    value = "30";
                if ("42".Equals(values[itemNo + 7]))
                    value = "42";
                if ("75".Equals(values[itemNo + 8]))
                    value = "75";
                if ("10".Equals(values[itemNo + 9]))
                    value = "10";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;
            }
            return value.PadRight(2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="itemNo"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static string GetHaraikomiHoho0206(string[] values, int itemNo)
        {
            var Count = 0;
            var value = string.Empty;
            if ("63".Equals(values[itemNo]))
                Count++;
            if ("10".Equals(values[itemNo + 1]))
                Count++;
            if ("60".Equals(values[itemNo + 2]))
                Count++;
            if (values[itemNo + 3].Length!=0)
                Count++;
            if ("46".Equals(values[itemNo + 4]))
                Count++;
            if ("10".Equals(values[itemNo + 5]))
                Count++;
            if ("75".Equals(values[itemNo + 6]))
                Count++;
            if (values[itemNo + 7].Length!=0)
                Count++;
            if (Count == 1)
            {
                if ("63".Equals(values[itemNo]))
                    value = "63";
                if ("10".Equals(values[itemNo + 1]))
                    value = "10";
                if ("60".Equals(values[itemNo + 2]))
                    value = "60";
                if (values[itemNo + 3].Length!=0)
                    value = values[itemNo + 3];
                if ("46".Equals(values[itemNo + 4]))
                    value = "46";
                if ("10".Equals(values[itemNo + 5]))
                    value = "10";
                if ("75".Equals(values[itemNo + 6]))
                    value = "75";
                if (values[itemNo + 7].Length != 0)
                    value = values[itemNo + 7];
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;
            }
            return value.PadRight(2);
        }

        private static string GetHaraikomiHohoDantai0206(string[] values, int itemNo)
        {
            var Count = 0;
            var value = string.Empty;
            if ("63".Equals(values[itemNo]))
                Count++;
            if ("10".Equals(values[itemNo + 1]))
                Count++;
            if ("60".Equals(values[itemNo + 2]))
                Count++;
            if (values[itemNo + 3].Length != 0)
                Count++;
            if (values[itemNo + 4].Length != 0)
                Count++;
            if (Count == 1)
            {
                if ("63".Equals(values[itemNo]))
                    value = "63";
                if ("10".Equals(values[itemNo + 1]))
                    value = "10";
                if ("60".Equals(values[itemNo + 2]))
                    value = "60";
                if (values[itemNo + 3].Length != 0)
                    value = values[itemNo + 3];
                if (values[itemNo + 4].Length != 0)
                    value = values[itemNo + 4];
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;
            }
            return value.PadRight(2);
        }
        private static string GetHaraikomiHohoIppan0206(string[] values, int itemNo)
        {
            var Count = 0;
            var value = string.Empty;
            if ("46".Equals(values[itemNo]))
                Count++;
            if ("75".Equals(values[itemNo + 1]))
                Count++;
            if (values[itemNo + 2].Length != 0)
                Count++;
            if (values[itemNo + 3].Length != 0)
                Count++;
            if (Count == 1)
            {
                if ("46".Equals(values[itemNo]))
                    value = "46";
                if ("75".Equals(values[itemNo + 1]))
                    value = "75";
               if (values[itemNo + 2].Length != 0)
                    value = values[itemNo + 2];
                if (values[itemNo + 3].Length != 0)
                    value = values[itemNo + 3];
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;
            }
            return value.PadRight(2);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="itemNo"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static string GetKyodoHokenTokuyaku_0102(string[] values, int itemNo, int n = 0)
        {
            var Count = 0;
            var value = string.Empty;
            if ("1".Equals(values[itemNo]))
                Count++;
            if ("2".Equals(values[itemNo + 1]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(values[itemNo]))
                    value = "1";
                if ("2".Equals(values[itemNo + 1]))
                    value = "2";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;// String.Format("{0}_共同保険特約_未設定", n);
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;//String.Format("{0}_共同保険特約_複数選択", n);
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="itemNo"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static string GetNakadachinin_0102(string[] values, int itemNo, int n = 0)
        {
            var Count = 0;
            var value = string.Empty;
            if ("1".Equals(values[itemNo]))
                Count++;
            if ("2".Equals(values[itemNo + 1]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(values[25]))
                    value = "1";
                if ("2".Equals(values[26]))
                    value = "2";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;//String.Format("{0}_代理店／仲立人 分担_複数選択", n);
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="itemNo"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static string GetJikoTokutei_0102(string[] values, int itemNo, int n=0)
        {
            var Count = 0;
            var value = string.Empty;
            if ("4".Equals(values[itemNo]))
                Count++;
            if ("5".Equals(values[itemNo + 1]))
                Count++;
            if (Count == 1)
            {
                if ("4".Equals(values[itemNo]))
                    value = "4";
                if ("5".Equals(values[itemNo + 1]))
                    value = "5";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;// String.Format("{0}_自己物件 非or自己_複数選択", n);
            }
            return value.PadRight(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="itemNo"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static string GetJimuhiUmu_0102(string[] values, int itemNo, int n = 0)
        {
            var Count = 0;
            var value = string.Empty;
            if ("1".Equals(values[itemNo]))
                Count++;
            if ("2".Equals(values[itemNo + 1]))
                Count++;
            if ("3".Equals(values[itemNo + 2]))
                Count++;
            if ("4".Equals(values[itemNo + 3]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(values[itemNo]))
                    value = "1";
                if ("2".Equals(values[itemNo + 1]))
                    value = "2";
                if ("3".Equals(values[itemNo + 2]))
                    value = "3";
                if ("4".Equals(values[itemNo + 3]))
                    value = "4";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;// String.Format("{0}_Ag Com 事務費の有無_複数選択", n);
            }
            return value.PadRight(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="values"></param>
        /// <param name="itemNo"></param>
        /// <param name="dateLength"></param>
        /// <returns></returns>
        private static string GetDate(string docId, string[] values, int itemNo, int dateLength = 6)
        {
            var value = string.Empty;
            if (values[itemNo + 1].Length == 0)
            {
                if (docId.Equals(DOC_ID))
                {
                    if (dateLength == 6)
                        value = value.PadRight(7);
                    else
                        value = value.PadRight(5);
                }
                else
                {
                    if (dateLength == 6)
                        value = value.PadRight(6);
                    else
                        value = value.PadRight(4);
                }
                return value;
            }

            if (values[itemNo].Length != 0
                || values[itemNo + 1].Length != 0)
            {
                if (docId.Equals(DOC_ID))
                {
                    values[itemNo] = values[itemNo].PadRight(1);
                }
                else
                {
                    values[itemNo] = string.Empty;
                }
                value = String.Join(string.Empty, values[itemNo], values[itemNo + 1].PadLeft(dateLength, '0'));
            }
            if (docId.Equals(DOC_ID))
                return value.PadRight(7);
            return value.PadRight(6);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="values"></param>
        /// <param name="itemNo"></param>
        /// <param name="dateLength"></param>
        /// <returns></returns>
        private static string GetDate2(string docId, string[] values, int itemNo, int dateLength = 6)
        {
            var Count = 0;
            var value = string.Empty;
            if (docId.Equals(DOC_ID))
            {
                if ("5".Equals(values[itemNo]))
                    Count++;
                if ("4".Equals(values[itemNo + 1]))
                    Count++;
                if (Count == 1)
                {
                    if ("5".Equals(values[itemNo]))
                        value = "5";
                    if ("4".Equals(values[itemNo + 1]))
                        value = "4";
                }
                else if (Count == 0)
                {
                    value = " ";
                }
                else if (Count > 1)
                {
                    value = Config.ReadNotCharNarrowOutput;
                }
                if (values[itemNo + 2].Length != 0)
                    value += values[itemNo + 2].PadLeft(dateLength, '0');
                else
                    value += string.Empty.PadRight(dateLength);
            }
            else
            {
                if (values[itemNo + 2].Length != 0)
                    value = values[itemNo + 2].PadLeft(dateLength, '0');
                else
                    value = string.Empty.PadRight(dateLength);
            }
            if (docId.Equals(DOC_ID))
            {
                return value.PadRight(dateLength+1);
            }
            return value.PadRight(dateLength);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="itemNo"></param>
        /// <returns></returns>
        private static string GetBirthDay(string[] values, int itemNo)
        {
            var value = string.Empty;
            if (values[itemNo].Length == 0      // 大正
                && values[itemNo + 1].Length == 0   // 昭和
                && values[itemNo + 2].Length == 0   // 平成
                && values[itemNo + 3].Length == 0   // 令和
                )
                return string.Empty.PadLeft(7);

            if (values[itemNo + 4].Length == 0)
                return string.Empty.PadLeft(7);

            var Count = 0;
            if ("2".Equals(values[itemNo]))
                Count++;
            if ("3".Equals(values[itemNo]))
                Count++;
            if ("4".Equals(values[itemNo]))
                Count++;
            if ("5".Equals(values[itemNo]))
                Count++;
            if (Count == 1)
            {
                if ("2".Equals(values[itemNo]))
                    value = "2";
                if ("3".Equals(values[itemNo]))
                    value = "3";
                if ("4".Equals(values[itemNo]))
                    value = "4";
                if ("5".Equals(values[itemNo]))
                    value = "5";
                value += values[itemNo + 4].PadLeft(6, '0');
            }
            else if (Count == 0)
            {
                value = " " + values[itemNo + 4].PadLeft(6, '0');
            }
            else
            {
                value = CONST_CHOFUKU + values[itemNo + 4].PadLeft(6, '0');
            }
            return value;
        }
/*
        private static string GetGender(string[] values, int itemNo)
        {
            var Count = 0;
            var value = string.Empty;
            if ("1".Equals(values[itemNo]))
                Count++;
            if ("2".Equals(values[itemNo + 1]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(values[itemNo]))
                    value = "1";
                if ("2".Equals(values[itemNo + 1]))
                    value = "2";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;// String.Format("{0}_Ag Com 事務費の有無_複数選択", n);
            }
            return value.PadRight(1);
        }
*/
        private static string Get1or2(string[] values, int itemNo)
        {
            var Count = 0;
            var value = string.Empty;
            if ("1".Equals(values[itemNo]))
                Count++;
            if ("2".Equals(values[itemNo + 1]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(values[itemNo]))
                    value = "1";
                if ("2".Equals(values[itemNo + 1]))
                    value = "2";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;// String.Format("{0}_Ag Com 事務費の有無_複数選択", n);
            }
            return value.PadRight(1);
        }
        private static string Get2or3(string[] values, int itemNo)
        {
            var Count = 0;
            var value = string.Empty;
            if ("2".Equals(values[itemNo]))
                Count++;
            if ("3".Equals(values[itemNo + 1]))
                Count++;
            if (Count == 1)
            {
                if ("2".Equals(values[itemNo]))
                    value = "2";
                if ("3".Equals(values[itemNo + 1]))
                    value = "3";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;// String.Format("{0}_Ag Com 事務費の有無_複数選択", n);
            }
            return value.PadRight(1);
        }
        private static string Get1or2or3(string[] values, int itemNo)
        {
            var Count = 0;
            var value = string.Empty;
            if ("1".Equals(values[itemNo]))
                Count++;
            if ("2".Equals(values[itemNo + 1]))
                Count++;
            if ("3".Equals(values[itemNo + 2]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(values[itemNo]))
                    value = "1";
                if ("2".Equals(values[itemNo + 1]))
                    value = "2";
                if ("3".Equals(values[itemNo + 2]))
                    value = "3";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;// String.Format("{0}_Ag Com 事務費の有無_複数選択", n);
            }
            return value.PadRight(1);
        }
        private static string Get3or4or5(string[] values, int itemNo)
        {
            var Count = 0;
            var value = string.Empty;
            if ("3".Equals(values[itemNo]))
                Count++;
            if ("4".Equals(values[itemNo + 1]))
                Count++;
            if ("5".Equals(values[itemNo + 2]))
                Count++;
            if (Count == 1)
            {
                if ("3".Equals(values[itemNo]))
                    value = "3";
                if ("4".Equals(values[itemNo + 1]))
                    value = "4";
                if ("5".Equals(values[itemNo + 2]))
                    value = "5";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;// String.Format("{0}_Ag Com 事務費の有無_複数選択", n);
            }
            return value.PadRight(1);
        }
        private static string Get2or3or4(string[] values, int itemNo)
        {
            var Count = 0;
            var value = string.Empty;
            if ("2".Equals(values[itemNo]))
                Count++;
            if ("3".Equals(values[itemNo + 1]))
                Count++;
            if ("4".Equals(values[itemNo + 2]))
                Count++;
            if (Count == 1)
            {
                if ("2".Equals(values[itemNo]))
                    value = "2";
                if ("3".Equals(values[itemNo + 1]))
                    value = "3";
                if ("4".Equals(values[itemNo + 2]))
                    value = "4";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;// String.Format("{0}_Ag Com 事務費の有無_複数選択", n);
            }
            return value.PadRight(1);
        }
        private static string Get1or2or9(string[] values, int itemNo)
        {
            var Count = 0;
            var value = string.Empty;
            if ("1".Equals(values[itemNo]))
                Count++;
            if ("2".Equals(values[itemNo + 1]))
                Count++;
            if ("9".Equals(values[itemNo + 2]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(values[itemNo]))
                    value = "1";
                if ("2".Equals(values[itemNo + 1]))
                    value = "2";
                if ("9".Equals(values[itemNo + 2]))
                    value = "9";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;// String.Format("{0}_Ag Com 事務費の有無_複数選択", n);
            }
            return value.PadRight(1);
        }
        private static string Get1or2or3or4(string[] values, int itemNo)
        {
            var Count = 0;
            var value = string.Empty;
            if ("1".Equals(values[itemNo]))
                Count++;
            if ("2".Equals(values[itemNo + 1]))
                Count++;
            if ("3".Equals(values[itemNo + 2]))
                Count++;
            if ("4".Equals(values[itemNo + 3]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(values[itemNo]))
                    value = "1";
                if ("2".Equals(values[itemNo + 1]))
                    value = "2";
                if ("3".Equals(values[itemNo + 2]))
                    value = "3";
                if ("4".Equals(values[itemNo + 4]))
                    value = "4";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;
            }
            return value.PadRight(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="itemNo"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static string GetHaraikomiHoho_0202(string[] values, int itemNo, int n = 0)
        {
            var Count = 0;
            var value = string.Empty;
            if ("10".Equals(values[itemNo]))
                Count++;
            if ("63".Equals(values[itemNo + 1]))
                Count++;
            if ("62".Equals(values[itemNo + 2]))
                Count++;
            if ("61".Equals(values[itemNo + 3]))
                Count++;
            if ("60".Equals(values[itemNo + 4]))
                Count++;
            if ("46".Equals(values[itemNo + 5]))
                Count++;
            if ("75".Equals(values[itemNo + 6]))
                Count++;
            if ("42".Equals(values[itemNo + 7]))
                Count++;
            if ("30".Equals(values[itemNo + 8]))
                Count++;
            if (!string.Empty.Equals(values[itemNo + 9]))
                Count++;
            if (Count == 1)
            {
                if ("10".Equals(values[itemNo]))
                    value = "10";
                if ("63".Equals(values[itemNo + 1]))
                    value = "63";
                if ("62".Equals(values[itemNo + 2]))
                    value = "62";
                if ("61".Equals(values[itemNo + 3]))
                    value = "61";
                if ("60".Equals(values[itemNo + 4]))
                    value = "60";
                if ("46".Equals(values[itemNo + 5]))
                    value = "46";
                if ("75".Equals(values[itemNo + 6]))
                    value = "75";
                if ("42".Equals(values[itemNo + 7]))
                    value = "42";
                if ("30".Equals(values[itemNo + 8]))
                    value = "30";
                if (!string.Empty.Equals(values[itemNo + 9]))
                    value = values[itemNo + 9];
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;// String.Format("{0}_払込方法_複数設定", n);
            }
            return value;
        }
        #endregion

        private static string EditXX(string val ,int iAll,int iFew=0)
        {
            if (val.Length == 0)
                return string.Empty.PadLeft(iAll);
            var ret = string.Empty;
            if (iFew == 0)
                ret = val.ToDecimal().ToString(String.Format("{0}", string.Empty.PadLeft(iAll, '0')));
            else
            {
                ret = val.ToDecimal().ToString(String.Format(String.Format("{0}.{1}", string.Empty.PadLeft(iAll - 1 - iFew, '0'), string.Empty.PadLeft(iFew, '0'))));
                ret = ret.Replace(String.Format(".{0}", string.Empty.PadLeft(iFew, '0')), string.Empty.PadLeft(iFew+1));
            }
            return ret;
        }

        private static string GetTsuzukigara0203(string[] values, int itemNo)
        {
            var Count = 0;
            var value = string.Empty;
            if ("1".Equals(values[itemNo]))
                Count++;
            if ("2".Equals(values[itemNo + 1]))
                Count++;
            if ("3".Equals(values[itemNo + 2]))
                Count++;
            if ("4".Equals(values[itemNo + 3]))
                Count++;
            if ("5".Equals(values[itemNo + 4]))
                Count++;
            if ("6".Equals(values[itemNo + 5]))
                Count++;
            if ("7".Equals(values[itemNo + 6]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(values[itemNo]))
                    value = "1";
                if ("2".Equals(values[itemNo + 1]))
                    value = "2";
                if ("3".Equals(values[itemNo + 2]))
                    value = "3";
                if ("4".Equals(values[itemNo + 3]))
                    value = "4";
                if ("5".Equals(values[itemNo + 4]))
                    value = "5";
                if ("6".Equals(values[itemNo + 5]))
                    value = "6";
                if ("7".Equals(values[itemNo + 6]))
                    value = "7";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU;// String.Format("{0}_払込方法_複数設定", n);
            }
            return value.PadRight(1);
        }

        private static string GetGroupType(string[] values, int itemNo)
        {
            // 団体区分
            var Count = 0;
            var value = string.Empty;
            if ("1".Equals(values[itemNo]))
                Count++;
            if ("2".Equals(values[itemNo+1]))
                Count++;
            if (Count == 1)
            {
                if ("1".Equals(values[itemNo]))
                    value = "F";
                if ("2".Equals(values[itemNo+1]))
                    value = "G";
            }
            else if (Count == 0)
            {
                // 未選択
                value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU; //"団体区分_複数選択";
            }
            return value.PadRight(1);
        }

        private static string GetKeiyakuHoshiki(string[] values,int itemNo)
        {
            // 契約方式
var            Count = 0;
            var value = string.Empty;
            if ("27".Equals(values[itemNo]))
                Count++;
            if ("36".Equals(values[itemNo+1]))
                Count++;
            if (Count == 1)
            {
                if ("27".Equals(values[itemNo]))
                    value = "27";
                if ("36".Equals(values[itemNo+1]))
                    value = "36";
            }
            else if (Count == 0)
            {
                // 未選択
value = string.Empty;
            }
            else
            {
                // 複数選択
                value = CONST_CHOFUKU; //"契約方式_複数選択";
            }
            return value.PadRight(2);
        }

        private static string GetHokenKikan(string[] values,int itemNo)
        {
            // 保険期間
var            list = new List<string>();
            var Count = 0;
            if (values[itemNo].Length != 0
                || values[itemNo+1].Length != 0
                || values[itemNo + 2].Length != 0
                || values[itemNo + 3].Length != 0)
            {
                list = new List<string>();
                Count = 0;
                list.Add(values[itemNo+1].PadLeft(2, '0'));
                list.Add(values[itemNo + 2].PadLeft(3, '0'));
                if ("1".Equals(values[itemNo+2]))
                    Count++;
                if ("2".Equals(values[itemNo + 3]))
                    Count++;
                if (Count == 1)
                {
                    if ("1".Equals(values[itemNo+2]))
                        list.Add("M");
                    if ("2".Equals(values[itemNo+3]))
                        list.Add("D");
                }
                else if (Count == 0)
                {
                    list.Add(string.Empty);
                }
                else
                {
                    list.Add(CONST_CHOFUKU);
                }
            }
            return String.Join(string.Empty, list.ToArray()).PadRight(6);
        }
    }
}