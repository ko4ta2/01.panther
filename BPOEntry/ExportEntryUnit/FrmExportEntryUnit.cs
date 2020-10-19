using BPOEntry.ExportEntryUnit;
using BPOEntry.Tables;
using Common;
using NLog;
using System;
using System.Windows.Forms;

namespace BPOEntry.ExportEntry
{
    public partial class FrmExportEntryUnit : Form
    {
        #region CreateParams
        /// <summary>
        /// ショートカットキーなどでフォームが閉じられないようにします。
        /// </summary>
        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                const int CS_NOCLOSE = 0x200;
                System.Windows.Forms.CreateParams createParams = base.CreateParams;
                createParams.ClassStyle |= CS_NOCLOSE;
                return createParams;
            }
        }
        #endregion

        /// <summary>
        /// log
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// dao
        /// </summary>
        private static DaoExportEntryUnit dao = new DaoExportEntryUnit();

        //public string s = "";
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrmExportEntryUnit()
        {
            InitializeComponent();

            InitProc();
        }

        /// <summary>
        /// 初期処理
        /// </summary>
        private void InitProc()
        {
            //this.Text = Config.BusinessName;
            this.Text = Utils.GetFormText();
            this.lblTitle.BackColor = Config.LblTitleBackColor;

            if (Config.ExportDateDiff < 0)
            {
                var dtNow = DateTime.Now;
                var dtTarget = DateTime.Now;
                var iIdx = Config.ExportDateDiff;
                while (true)
                {
                    dtTarget = dtNow.AddDays(iIdx);
                    if (dtTarget.DayOfWeek.Equals(DayOfWeek.Saturday)
                        || dtTarget.DayOfWeek.Equals(DayOfWeek.Sunday))
                    {
                        iIdx--;
                        continue;
                    }
                    this.dtpImageCaptureDate.Value = dtTarget;
                    break;
                }
            }

            // 出力単位
            if (!"1".Equals(Config.ExportUnit))
            {
                this.label2.Visible = false;
                this.nudImageCaptureNum.Visible = false;
            }
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClose_Click(object sender, EventArgs e)
        {
            EndProc(); 
        }

        private void EndProc()
        {
            this.Close();
            this.Dispose();
        }

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExec_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("実行しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                dtpImageCaptureDate.Focus();
                return;
            }

            var sCaptureDate = this.dtpImageCaptureDate.Value.ToString("yyyyMMdd");
            var sCaptureNum = this.nudImageCaptureNum.Value.ToString("00");
            var stw = new System.Diagnostics.Stopwatch();

            try
            {
                dao.Open(Config.DSN);

                // 対象データ存在チェック
                if (!this.IsExistsExportData(sCaptureDate, sCaptureNum))
                {
                    MessageBox.Show("対象データが存在しません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (Consts.Flag.ON.Equals(Config.CreateZeroDataFlag)
                        && Consts.Flag.ON.Equals(Config.ExecAfterExportFlag))
                    {
                        if (MessageBox.Show("ゼロ件データを作成しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            AfterExport.AfterExport.BL_Main();
                        }
                    }
                    return;
                }

                var tksk_cd = Config.TokuisakiCode;
                var hnm_cd = Config.HinmeiCode;
                var deu = new D_ENTRY_UNIT($"{tksk_cd}_{hnm_cd}_{sCaptureDate}_{sCaptureNum}_XXXXXXXX_XXX")
                {
                    BATCH_EXPORT_FLAG = Consts.Flag.OFF,
                    EXPORT_METHOD = Config.ExportUnit,
                    USER_ID = Config.UserId
                };
                var sMessage = DllExportEntryUnit.DllExportEntryUnit.BL_Main(deu.ENTRY_UNIT_ID,deu.BATCH_EXPORT_FLAG,deu.USER_ID);

                using (var dlg = new FrmExportResult(sMessage))
                    dlg.ShowDialog();

                //var iRecordCount = 0;
                //var iTotalRecordCount = 0;

                //var sCountList = new List<string>();
                //var CreateDateTime = DateTime.Now;
                //var RootExportFolder = string.Empty;
                //var FilePath = string.Empty;
                //var DOC_ID = string.Empty;
                //var PCIDSS_FLAG = string.Empty;

                //this.Cursor = Cursors.WaitCursor;

                //stw.Start();

                //dao.BeginTrans();

                //var dtM_DOC = dao.SELECT_M_DOC();
                //foreach (DataRow drM_Doc in dtM_DOC.Rows)
                //{
                //    DOC_ID = drM_Doc["DOC_ID"].ToString();
                //    PCIDSS_FLAG = drM_Doc["PCIDSS_FLAG"].ToString().Length != 0 ? drM_Doc["PCIDSS_FLAG"].ToString() : Consts.Flag.OFF;
                //    var dtD_ENTRY = dao.SELECT_D_ENTRY(sCaptureDate, sCaptureNum, DOC_ID);

                //    if (dtD_ENTRY.Rows.Count == 0)
                //        continue;

                //    if (Consts.Flag.ON.Equals(PCIDSS_FLAG))
                //        RootExportFolder = Config.PCIDSSExportFolder;
                //    else
                //        RootExportFolder = Config.ExportFolder;

                //    FilePath = Path.Combine(RootExportFolder, String.Format("{0}_EntryData_{1}_{2}_{3:yyyyMMddHHmmss}.tsv", Config.UserId, DOC_ID.Substring(0, 4), DOC_ID.Substring(4, 4), CreateDateTime));

                //    // 文字コード設定
                //    var enc = Consts.EncShift_JIS;
                //    if (Consts.Encode.UTF8.Equals(Config.Encode))
                //        enc = Consts.EncUTF8N;

                //    // 
                //    if (Consts.Flag.ON.Equals(PCIDSS_FLAG))
                //        enc = Consts.EncUTF8N;

                //    using (var sw = new StreamWriter(FilePath, false, enc))
                //    {
                //        // ヘッダー行
                //        if (!Consts.Flag.ON.Equals(PCIDSS_FLAG))
                //        {
                //            var head = new List<string>();
                //            head.Add("01");                                  // レコード区分
                //            head.Add(CreateDateTime.ToString("yyyyMMdd"));   // データ作成日付
                //            head.Add(CreateDateTime.ToString("HHmmss"));     // データ作成時間
                //            head.Add(Config.TokuisakiCode);                  // 得意先コード
                //            head.Add(Config.HinmeiCode);                     // 品名コード
                //            sw.WriteLine(string.Join("\t", head));
                //        }

                //        // 明細行
                //        var detail = new List<string>();

                //        // 同じページ毎にグループ化
                //        var pages = from DataRow drD_ENTRY in dtD_ENTRY.Rows
                //                    group drD_ENTRY by new
                //                    {
                //                        //TKSK_CD = drD_ENTRY["TKSK_CD"].ToString(),
                //                        //HNM_CD = drD_ENTRY["HNM_CD"].ToString(),
                //                        //IMAGE_CAPTURE_DATE = drD_ENTRY["IMAGE_CAPTURE_DATE"].ToString(),
                //                        //IMAGE_CAPTURE_NUM = drD_ENTRY["IMAGE_CAPTURE_NUM"].ToString(),
                //                        //ENTRY_UNIT = drD_ENTRY["ENTRY_UNIT"].ToString(),
                //                        //DOC_ID = drD_ENTRY["DOC_ID"].ToString(),
                //                        ENTRY_UNIT_ID = drD_ENTRY["ENTRY_UNIT_ID"].ToString(),
                //                        IMAGE_SEQ = drD_ENTRY["IMAGE_SEQ"].ToString(),
                //                        IMAGE_PATH = drD_ENTRY["IMAGE_PATH"].ToString()
                //                    };

                //        iRecordCount = 0;
                //        foreach (var page in pages)
                //        {
                //            // レコード区分
                //            if (!Consts.Flag.ON.Equals(PCIDSS_FLAG))
                //                detail.Add("02");
                //            else
                //                detail.Add(System.IO.Path.GetFileName((string)page.Key.IMAGE_PATH));

                //            // 各項目をリスト化する
                //            foreach (var item in page)
                //            {
                //                string sItemId = item["ITEM_ID"].ToString();
                //                string sValue = item["VALUE"].ToString();
                //                /*
                //                                                #region 楽天生命個別変換出力
                //                                                if (Utils.IsRLI())
                //                                                {
                //                                                    // 特定の項目の値を変換して出力します。
                //                                                    if (sDocId == "17010001")
                //                                                        sValue = ConvertValue17010001(sItemId, sValue);
                //                                                    else if (sDocId == "17020001")
                //                                                        sValue = ConvertValue17020001(sItemId, sValue);
                //                                                    else if (sDocId == "18010001")
                //                                                        sValue = ConvertValue18010001(sItemId, sValue);
                //                                                    else if (sDocId == "18020001")
                //                                                        sValue = ConvertValue18020001(sItemId, sValue);
                //                                                    else if (sDocId == "19010001")
                //                                                        sValue = ConvertValue19010001(sItemId, sValue);
                //                                                    else if (sDocId == "23010001")
                //                                                        sValue = ConvertValue23010001(sItemId, sValue);
                //                                                    else if (sDocId == "26020001")
                //                                                        sValue = ConvertValue26020001(sItemId, sValue);

                //                                                    else if (sDocId == "30020001")
                //                                                        // 申込書（乗合限定告知型）
                //                                                        sValue = ConvertValue30020001(sItemId, sValue);
                //                                                    else if (sDocId == "22010001")
                //                                                        // 取扱者報告書
                //                                                        sValue = ConvertValue22010001(sItemId, sValue);
                //                                                    else if (sDocId == "14010001")
                //                                                        // 告知書（総合）
                //                                                        sValue = ConvertValue14010001(sItemId, sValue);
                //                                                    else if (sDocId == "28010001")
                //                                                        // 復活請求書
                //                                                        sValue = ConvertValue28010001(sItemId, sValue);
                //                                                    else if (sDocId == "50010001")
                //                                                        // 申込書（1年定期）
                //                                                        sValue = ConvertValue50010001(sItemId, sValue);
                //                                                }
                //                                                #endregion
                //                */
                //                detail.Add(sValue.Replace(Config.ReadNotCharNarrowInput, Config.ReadNotCharNarrowOutput).Replace(Config.ReadNotCharWideInput, Config.ReadNotCharWideOutput));
                //            }

                //            // 末尾に画像パスを追加
                //            if (!Consts.Flag.ON.Equals(PCIDSS_FLAG))
                //                detail.Add((string)page.Key.IMAGE_PATH);

                //            sw.WriteLine(String.Join("\t", detail));
                //            detail.Clear();
                //            iRecordCount++;
                //            iTotalRecordCount++;
                //        }

                //        if (sCountList.Count == 0)
                //            sCountList.Add(Environment.NewLine + "（内訳）");

                //        sCountList.Add("  " + Utils.LeftB(drM_Doc["DOC_NAME"].ToString().PadRight(60, ' '), 60) + iRecordCount.ToString("#,0 件").PadLeft(9));

                //        // トレーラ行
                //        if (!Consts.Flag.ON.Equals(PCIDSS_FLAG))
                //        {
                //            var trail = new List<string>();
                //            trail.Add("09");                            // レコード区分
                //            trail.Add(iRecordCount.ToString("d10"));    // レコード件数
                //            trail.Add("DNP");                           // 会社区分
                //            sw.WriteLine(string.Join("\t", trail));
                //        }
                //    }

                //    // エントリ単位毎にグループ化
                //    var EntryUnits = from DataRow drD_ENTRY in dtD_ENTRY.Rows
                //                     group drD_ENTRY by new
                //                     {
                //                         //TKSK_CD = drD_ENTRY["TKSK_CD"].ToString(),
                //                         //HNM_CD = drD_ENTRY["HNM_CD"].ToString(),
                //                         //IMAGE_CAPTURE_DATE = drD_ENTRY["IMAGE_CAPTURE_DATE"].ToString(),
                //                         //IMAGE_CAPTURE_NUM = drD_ENTRY["IMAGE_CAPTURE_NUM"].ToString(),
                //                         //DOC_ID = drD_ENTRY["DOC_ID"].ToString(),
                //                         //ENTRY_UNIT = drD_ENTRY["ENTRY_UNIT"].ToString()
                //                         ENTRY_UNIT_ID = drD_ENTRY["ENTRY_UNIT_ID"].ToString()
                //                     };
                //    //try
                //    //{
                //    // ステータスをテキスト出力済に更新
                //    foreach (var unit in EntryUnits)
                //    {
                //        //var deu = new D_ENTRY_UNIT(String.Join("_", Config.TokuisakiCode, Config.HinmeiCode, unit.Key.IMAGE_CAPTURE_DATE, unit.Key.IMAGE_CAPTURE_NUM, unit.Key.DOC_ID, unit.Key.ENTRY_UNIT), string.Empty)
                //        //{
                //        //    UPD_USER_ID = Program.LoginUser.USER_ID,
                //        //};
                //        //dao.UpdateD_ENTRY_UNIT(deu);
                //        if (dao.UpdateD_ENTRY_UNIT(unit.Key.ENTRY_UNIT_ID) != 1)
                //        {
                //            throw new ApplicationException(String.Format("D_ENTRY_UNIT の更新で不整合が発生しました。\nENTRY_UNIT_ID:{0}",unit.Key.ENTRY_UNIT_ID));
                //        }
                //    }
                //    //}
                //    //catch (Exception ex)
                //    //{
                //    //    _dao.RollbackTrans();
                //    //    //_log.Error(ex);
                //    //    throw; 
                //    //    //continue;
                //    //}
                //}

                //// エントリデータ出力後処理
                //log.Info($"エントリデータ出力後処理：{Config.ExecAfterExportFlag}");
                //if (Consts.Flag.ON.Equals(Config.ExecAfterExportFlag))
                //    AfterExport.AfterExport.BL_Main();

                //dao.CommitTrans();
                //stw.Stop();
                //log.Info("経過時間：{0}", stw.Elapsed);
                //this.Cursor = Cursors.Default;

                //var sb = new StringBuilder();
                //log.Info("↓↓↓↓↓↓↓↓↓↓　件数内訳　↓↓↓↓↓↓↓↓↓↓");
                //foreach (var sCount in sCountList)
                //    sb.AppendLine(sCount);
                //log.Info("↑↑↑↑↑↑↑↑↑↑　件数内訳　↑↑↑↑↑↑↑↑↑↑");

                //string sMessage = "終了しました。" + Environment.NewLine + String.Format("出力件数:{0}", iTotalRecordCount.ToString("#,0 件") + Environment.NewLine + sb.ToString());
                ////MessageBox.Show(String.Format("{0}が終了しました。", this.lblTitle.Text) + Environment.NewLine + String.Format("処理件数:{0}", iTotalRecordCount.ToString("#,0 件") + Environment.NewLine + sb.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                //using (var dlg = new FrmExportResult(sMessage))
                //    dlg.ShowDialog();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                dao.RollbackTrans();
                log.Error(ex);
                MessageBox.Show("例外が発生しました。" + Environment.NewLine + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dao.Close();
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 出力データ有無確認
        /// </summary>
        /// <param name="sCaptureDate"></param>
        /// <param name="sCaptureNum"></param>
        /// <returns></returns>
        private bool IsExistsExportData(string sCaptureDate, string sCaptureNum)
        {
            var dt = dao.SELECT_D_ENTRY_UNIT(sCaptureDate, sCaptureNum);
            if ("1".Equals(Config.ExportUnit)
                || "2".Equals(Config.ExportUnit)
                || "3".Equals(Config.ExportUnit))
            {
                // 対象データなし
                if (dt.Rows[0]["MAX_STATUS"].ToString().Length == 0)
                    return false;

                // コンペア済(7)のデータを対象に出力します。
                if (Consts.EntryUnitStatus.COMPARE_END.Equals(dt.Rows[0]["MAX_STATUS"].ToString()))
                    return true;
            }
            else
            {
                // 対象データなし
                if (Consts.EntryUnitStatus.COMPARE_END.Equals(dt.Rows[0]["MAX_STATUS"].ToString()))
                    return true;
            }
            return false;
        }

        private void NudImageCaptureNum_Enter(object sender, EventArgs e)
        {
            this.nudImageCaptureNum.Select(0, this.nudImageCaptureNum.Value.ToString().Length);
        }
    }
}
