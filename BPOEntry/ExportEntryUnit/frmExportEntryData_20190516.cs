using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using BPOEntry.Tables;
using NLog;
using BPOEntry.ExportEntryData;


namespace BPOEntry.ExportEntry
{
    public partial class frmExportEntry : Form
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
        private static Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// dao
        /// </summary>
        private static DaoExportEntry _dao = new DaoExportEntry();

        public string s = "";
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public frmExportEntry()
        {
            InitializeComponent();
            this.Text = Config.BusinessName;
            this.lblTitle.BackColor = Config.LblTitleBackColor;

            if (Config.iExportDateDiff < 0)
            {
                DateTime dtNow = System.DateTime.Now;
                DateTime dtTarget = System.DateTime.Now;
                int iIdx = Config.iExportDateDiff;
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
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExec_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(String.Format("{0}を実行しますか？",this.lblTitle.Text), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                dtpImageCaptureDate.Focus();
                return;
            }

            string sCaptureDate = this.dtpImageCaptureDate.Value.ToString("yyyyMMdd");
            string sCaptureNum = this.nudImageCaptureNum.Value.ToString("00");

            this.Cursor = Cursors.WaitCursor;

            System.Diagnostics.Stopwatch stw = new System.Diagnostics.Stopwatch();
            try
            {
                int iRecordCount = 0;
                int iTotalRecordCount = 0;
                stw.Start();

                _dao.Open(Config.DSN);

                // 指定の日付、回数のエントリ単位が全てテキスト出力可能かどうか判定します。
                if (!this.IsExistsExportData(sCaptureDate, sCaptureNum))
                {
                    MessageBox.Show("出力対象データが存在しません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 書類種別毎の件数
                List<string> sCountList = new List<string>();

                _dao.BeginTrans();

                // 全帳票IDを取得します。
                DataTable dtM_DOC = _dao.SelectM_DOC();
                foreach (DataRow drM_Doc in dtM_DOC.Rows)
                {
                    // 対象帳票のテキスト出力可能データを抽出します。
                    string sDocId = drM_Doc["DOC_ID"].ToString();
                    DataTable dtD_ENTRY = _dao.SelectD_ENTRY(sCaptureDate, sCaptureNum, sDocId);

                    // 明細が無ければ処理しません。
                    if (dtD_ENTRY.Rows.Count == 0)
                        continue;

                    // 帳票ID別のファイルを作成します。
                    DateTime createDateTime = DateTime.Now;
                    string name = string.Format("{0}_EntryData_{1}_{2}_{3:yyyyMMddHHmmss}.tsv", Config.UserId, sDocId.Substring(0, 4), sDocId.Substring(4, 4), createDateTime);
                    string fullName = Path.Combine(Config.ExportFolder, name);

                    System.Text.Encoding enc = Consts.EncShift_JIS;
                    if ("3".Equals(Config.sEnc))
                        enc = Consts.EncUTF8;

                    if (Consts.Flag.ON.Equals(Config.PCIDSS))
                        enc = new System.Text.UTF8Encoding(false);

                    using (var sw = new StreamWriter(fullName, false, enc))
                    {
                        // ヘッダー行
                        if (!Consts.Flag.ON.Equals(Config.PCIDSS))
                        {
                            var head = new List<string>();
                            head.Add("01");                                  // レコード区分
                            head.Add(createDateTime.ToString("yyyyMMdd"));   // データ作成日付
                            head.Add(createDateTime.ToString("HHmmss"));     // データ作成時間
                            head.Add(Config.TokuisakiCode);                  // 得意先コード
                            head.Add(Config.HinmeiCode);                     // 品名コード
                            sw.WriteLine(string.Join("\t", head));
                        }

                        // 明細行
                        var detail = new List<string>();

                        // 同じページ毎にグループ化
                        var pages = from DataRow drD_ENTRY in dtD_ENTRY.Rows
                                    group drD_ENTRY by new
                                    {
                                        TKSK_CD = drD_ENTRY["TKSK_CD"].ToString(),
                                        HNM_CD = drD_ENTRY["HNM_CD"].ToString(),
                                        IMAGE_CAPTURE_DATE = drD_ENTRY["IMAGE_CAPTURE_DATE"].ToString(),
                                        IMAGE_CAPTURE_NUM = drD_ENTRY["IMAGE_CAPTURE_NUM"].ToString(),
                                        ENTRY_UNIT = drD_ENTRY["ENTRY_UNIT"].ToString(),
                                        DOC_ID = drD_ENTRY["DOC_ID"].ToString(),
                                        IMAGE_SEQ = drD_ENTRY["IMAGE_SEQ"].ToString(),
                                        IMAGE_PATH = drD_ENTRY["IMAGE_PATH"].ToString()
                                    };

                        iRecordCount = 0;
                        foreach (var page in pages)
                        {
                            // レコード区分
                            if (!Consts.Flag.ON.Equals(Config.PCIDSS))
                                detail.Add("02");
                            else
                                detail.Add(System.IO.Path.GetFileName((string)page.Key.IMAGE_PATH));

                            // 各項目をリスト化する
                            foreach (var item in page)
                            {
                                string sItemId = item["ITEM_ID"].ToString();
                                string sValue = item["VALUE"].ToString();

                                #region 楽天生命個別変換出力
                                if (Utils.IsRLI())
                                {
                                    // 特定の項目の値を変換して出力します。
                                    if (sDocId == "17010001")
                                        sValue = ConvertValue17010001(sItemId, sValue);
                                    else if (sDocId == "17020001")
                                        sValue = ConvertValue17020001(sItemId, sValue);
                                    else if (sDocId == "18010001")
                                        sValue = ConvertValue18010001(sItemId, sValue);
                                    else if (sDocId == "18020001")
                                        sValue = ConvertValue18020001(sItemId, sValue);
                                    else if (sDocId == "19010001")
                                        sValue = ConvertValue19010001(sItemId, sValue);
                                    else if (sDocId == "23010001")
                                        sValue = ConvertValue23010001(sItemId, sValue);
                                    else if (sDocId == "26020001")
                                        sValue = ConvertValue26020001(sItemId, sValue);

                                    else if (sDocId == "30020001")
                                        // 申込書（乗合限定告知型）
                                        sValue = ConvertValue30020001(sItemId, sValue);
                                    else if (sDocId == "22010001")
                                        // 取扱者報告書
                                        sValue = ConvertValue22010001(sItemId, sValue);
                                    else if (sDocId == "14010001")
                                        // 告知書（総合）
                                        sValue = ConvertValue14010001(sItemId, sValue);
                                    else if (sDocId == "28010001")
                                        // 復活請求書
                                        sValue = ConvertValue28010001(sItemId, sValue);
                                    else if (sDocId == "50010001")
                                        // 申込書（1年定期）
                                        sValue = ConvertValue50010001(sItemId, sValue);
                                }
                                #endregion

                                detail.Add(sValue.Replace(Config.ReadNotCharNarrowInput, Config.ReadNotCharNarrowOutput).Replace(Config.ReadNotCharWideInput, Config.ReadNotCharWideOutput));
                            }

                            // 末尾に画像パスを追加しTab区切りで出力します。
                            if (!Consts.Flag.ON.Equals(Config.PCIDSS))
                                detail.Add((string)page.Key.IMAGE_PATH);

                            sw.WriteLine(String.Join("\t", detail));
                            detail.Clear();
                            iRecordCount++;
                            iTotalRecordCount++;
                        }

                        if (sCountList.Count == 0)
                            sCountList.Add(Environment.NewLine + "（内訳）");

                        sCountList.Add("  " + Utils.LeftB(drM_Doc["DOC_NAME"].ToString().PadRight(50, ' '), 50) + iRecordCount.ToString("#,0 件").PadLeft(9));

                        // トレーラー行
                        if (!"1".Equals(Config.PCIDSS))
                        {
                            var trail = new List<string>();
                            trail.Add("09");                            // レコード区分
                            trail.Add(iRecordCount.ToString("d10"));    // レコード件数
                            trail.Add("DNP");                           // 会社区分
                            sw.WriteLine(string.Join("\t", trail));
                        }
                    }

                    // エントリ単位毎にグループ化
                    var entryUnits = from DataRow drD_ENTRY in dtD_ENTRY.Rows
                                     group drD_ENTRY by new
                                     {
                                         TKSK_CD = drD_ENTRY["TKSK_CD"].ToString(),
                                         HNM_CD = drD_ENTRY["HNM_CD"].ToString(),
                                         IMAGE_CAPTURE_DATE = drD_ENTRY["IMAGE_CAPTURE_DATE"].ToString(),
                                         IMAGE_CAPTURE_NUM = drD_ENTRY["IMAGE_CAPTURE_NUM"].ToString(),
                                         DOC_ID = drD_ENTRY["DOC_ID"].ToString(),
                                         ENTRY_UNIT = drD_ENTRY["ENTRY_UNIT"].ToString()
                                     };

                    try
                    {
                        // ステータスをテキスト出力済に更新
                        foreach (var unit in entryUnits)
                        {
                            var entryUnit = new D_ENTRY_UNIT(unit.Key.IMAGE_CAPTURE_DATE, unit.Key.IMAGE_CAPTURE_NUM, unit.Key.DOC_ID, unit.Key.ENTRY_UNIT, string.Empty)
                            {
                                UPD_USER_ID = Program.LoginUser.USER_ID,
                            };
                            _dao.UpdateD_ENTRY_UNIT(entryUnit);
                        }
                    }
                    catch (Exception ex)
                    {
                        _dao.RollbackTrans();
                        //_log.Error(ex);
                        throw; 
                        //continue;
                    }
                }

                // エントリデータ出力後処理
                _log.Info("エントリデータ出力後処理：{0}", Config.ExecAfterExport);
                if (Consts.Flag.ON.Equals(Config.ExecAfterExport))
                    AfterExport.AfterExport.BL_Main();

                _dao.CommitTrans();
                stw.Stop();
                _log.Info("経過時間：{0}", stw.Elapsed);
                this.Cursor = Cursors.Default;

                StringBuilder sb = new StringBuilder();
                _log.Info("↓↓↓↓↓↓↓↓↓↓　件数内訳　↓↓↓↓↓↓↓↓↓↓");
                foreach (var sCount in sCountList)
                    sb.AppendLine(sCount);
                _log.Info("↑↑↑↑↑↑↑↑↑↑　件数内訳　↑↑↑↑↑↑↑↑↑↑");

                string sMessage = String.Format("{0}が終了しました。", this.lblTitle.Text) + Environment.NewLine + String.Format("処理件数:{0}", iTotalRecordCount.ToString("#,0 件") + Environment.NewLine + sb.ToString());
                //MessageBox.Show(String.Format("{0}が終了しました。", this.lblTitle.Text) + Environment.NewLine + String.Format("処理件数:{0}", iTotalRecordCount.ToString("#,0 件") + Environment.NewLine + sb.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                using (var dlg = new frmExportResult(sMessage))
                    dlg.ShowDialog();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                _dao.RollbackTrans();
                _log.Error(ex);
                MessageBox.Show(String.Format("{0}で例外が発生しました。", this.lblTitle.Text) + Environment.NewLine + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _dao.Close();
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
            var dt = _dao.SelectD_ENTRY_UNIT(sCaptureDate, sCaptureNum);
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

        private void nudImageCaptureNum_Enter(object sender, EventArgs e)
        {
            this.nudImageCaptureNum.Select(0, this.nudImageCaptureNum.Value.ToString().Length);
        }

        #region 楽天生命変換出力
        /// <summary>
        /// 申込書（総合型）
        /// </summary>
        /// <param name="sItemId"></param>
        /// <param name="sValue"></param>
        /// <returns></returns>
        private string ConvertValue17010001(string sItemId, string sValue)
        {
            switch (sItemId)
            {
                //case "ITEM_009":
                case "ITEM_010":
                    if (sValue.Length == 0) return "0";
                    break;

                //case "ITEM_062":
                case "ITEM_063":
                    if (sValue.Length == 0) return "0";
                    if (sValue == "1") return "004";
                    break;

                //case "ITEM_063":
                case "ITEM_064":
                    if (sValue.Length == 0) return "0";
                    if (sValue == "1") return "005";
                    break;

                //case "ITEM_072":
                case "ITEM_073":
                    if (sValue.Length == 0) return "0";
                    break;

                //case "ITEM_075":
                case "ITEM_076":
                    if (sValue.Length == 0) return "0";
                    break;

                //case "ITEM_078":
                case "ITEM_079":
                    if (sValue.Length == 0) return "0";
                    break;

                //case "ITEM_081":
                case "ITEM_082":
                    if (sValue.Length == 0) return "0";
                    break;

                //case "ITEM_082":
                case "ITEM_083":
                    if (sValue == "1") return "5000";
                    if (sValue == "2") return "10000";
                    if (sValue == "3") return "15000";
                    break;

                //case "ITEM_083":
                case "ITEM_084":
                    if (sValue == "1") return "1000000";
                    if (sValue == "2") return "2000000";
                    if (sValue == "3") return "3000000";
                    break;

                //case "ITEM_085":
                case "ITEM_086":
                    if (sValue.Length == 0) return "0";
                    break;

                //case "ITEM_086":
                case "ITEM_087":
                    if (sValue == "1") return "100000";
                    break;

                //case "ITEM_088":
                case "ITEM_089":
                    if (sValue.Length == 0) return "0";
                    break;

                //case "ITEM_089":
                case "ITEM_090":
                    if (sValue == "1") return "50000";
                    break;

                //case "ITEM_090":
                case "ITEM_091":
                    if (sValue == "1") return "1000000";
                    if (sValue == "2") return "2000000";
                    if (sValue == "3") return "3000000";
                    break;

                //case "ITEM_092":
                case "ITEM_093":
                    if (sValue.Length == 0) return "0";
                    break;

                //case "ITEM_093":
                case "ITEM_094":
                    if (sValue == "1") return "10000";
                    break;

                //case "ITEM_094":
                case "ITEM_095":
                    if (sValue == "1") return "500000";
                    if (sValue == "2") return "1000000";
                    break;

                //case "ITEM_105":
                case "ITEM_106":
                    if (sValue == "1") return "100000";
                    if (sValue == "2") return "200000";
                    break;

                //case "ITEM_107":
                case "ITEM_108":
                    {
                        decimal price;
                        if (decimal.TryParse(sValue, out price))
                            return (price * 10000).ToString();
                        break;
                    }

                //case "ITEM_109":
                case "ITEM_110":
                    {
                        decimal price;
                        if (decimal.TryParse(sValue, out price))
                            return (price * 10000).ToString();
                        break;
                    }

                //case "ITEM_111":
                case "ITEM_112":
                    if (sValue == "A") return "01";
                    if (sValue == "B") return "02";
                    if (sValue == "C") return "03";
                    if (sValue == "D") return "04";
                    if (sValue == "E") return "05";
                    if (sValue == "W") return "06";
                    if (sValue == "X") return "07";
                    if (sValue == "Y") return "08";
                    if (sValue == "Z") return "09";
                    if (sValue == "S1") return "10";
                    if (sValue == "S2") return "11";
                    break;
            }
            return sValue;
        }

        /// <summary>
        /// 17010001 と同じ
        /// </summary>
        /// <param name="sItemId"></param>
        /// <param name="sValue"></param>
        /// <returns></returns>
        private string ConvertValue18010001(string sItemId, string sValue)
        {
            return ConvertValue17010001(sItemId, sValue);
        }

        /// <summary>
        /// 申込書（限定告知型）
        /// </summary>
        /// <param name="sItemId"></param>
        /// <param name="sValue"></param>
        /// <returns></returns>
        private string ConvertValue17020001(string sItemId, string sValue)
        {
            switch (sItemId)
            {
                //case "ITEM_009":
                case "ITEM_010":
                    if (sValue.Length == 0) return "0";
                    break;

                //case "ITEM_047":
                case "ITEM_048":
                    if (sValue.Length == 0) return "0";
                    break;

                //case "ITEM_050":
                case "ITEM_051":
                    if (sValue.Length == 0) return "0";
                    break;

                //case "ITEM_052":
                case "ITEM_053":
                    if (sValue == "1") return "500000";
                    if (sValue == "2") return "1000000";
                    break;

                //case "ITEM_063":
                case "ITEM_064":
                    if (sValue.Length == 0) return "0";
                    if (sValue == "1") return "005";
                    break;
            }
            return sValue;
        }

        private string ConvertValue18020001(string sItemId, string sValue)
        {
            return ConvertValue17020001(sItemId, sValue);
        }

        /// <summary>
        /// 申込書（大型定期）
        /// </summary>
        /// <param name="sItemId"></param>
        /// <param name="sValue"></param>
        /// <returns></returns>
        private string ConvertValue19010001(string sItemId, string sValue)
        {
            switch (sItemId)
            {
                //case "ITEM_019":
                case "ITEM_020":
                    if (sValue.Length == 0) return "?";
                    break;

                //case "ITEM_048":
                case "ITEM_049":
                    decimal price;
                    if (decimal.TryParse(sValue, out price))
                        return (price * 10000).ToString();
                    break;

                //case "ITEM_050":
                case "ITEM_051":
                    if (sValue.Length == 0) return "0";
                    if (sValue == "1") return "004";
                    break;

                //case "ITEM_057":
                case "ITEM_058":
                    if (sValue.Length == 0) return "0";
                    if (sValue == "1") return "005";
                    break;
            }
            return sValue;
        }

        /// <summary>
        /// 取扱者報告書（大型定期）
        /// </summary>
        /// <param name="sItemId"></param>
        /// <param name="sValue"></param>
        /// <returns></returns>
        private string ConvertValue23010001(string sItemId, string sValue)
        {
            switch (sItemId)
            {
                //case "ITEM_007":
                case "ITEM_008":
                    if (sValue.Length != 0)
                        return sValue.PadLeft(2, '0');
                    break;

                //case "ITEM_029":
                //case "ITEM_030":
                //case "ITEM_031":
                //case "ITEM_032":
                //case "ITEM_052":
                case "ITEM_030":
                case "ITEM_031":
                case "ITEM_032":
                case "ITEM_033":
                case "ITEM_053":
                    if (sValue.Length == 0) return "0";
                    break;
            }
            return sValue;
        }

        /// <summary>
        /// 復活請求書
        /// </summary>
        /// <param name="sItemId"></param>
        /// <param name="sValue"></param>
        /// <returns></returns>
        private string ConvertValue28010001(string sItemId, string sValue)
        {
            // 変換無し
            return sValue;
        }

        /// <summary>
        /// 限定告知１２０
        /// </summary>
        /// <param name="sItemId"></param>
        /// <param name="sValue"></param>
        /// <returns></returns>
        private string ConvertValue26020001(string sItemId, string sValue)
        {
            switch (sItemId)
            {
                //case "ITEM_009":
                case "ITEM_010":
                    // 契約者 性別
                    if (sValue.Length == 0) return "0";
                    break;
                //case "ITEM_047":
                case "ITEM_048":
                    // 限定告知型医療保険 先進医療特約
                    if (sValue.Length == 0) return Consts.Flag.OFF;
                    break;
                //case "ITEM_050":
                case "ITEM_051":
                    // 限定告知型医療保険ガン特則付 先進医療特約
                    if (sValue.Length == 0) return Consts.Flag.OFF;
                    break;
                //case "ITEM_052":
                case "ITEM_053":
                    // 限定告知型医療保険ガン特則付 ガン診断給付金
                    if (sValue == "1") return "500000";
                    if (sValue == "2") return "1000000";
                    break;
                //case "ITEM_063":
                case "ITEM_064":
                    // 指定代理請求特約
                    if (sValue.Length == 0) return Consts.Flag.OFF;
                    if (sValue == "1") return "005";
                    break;
                //case "ITEM_090":
                case "ITEM_091":
                    // リビング・ニーズ特約
                    if (sValue.Length == 0) return Consts.Flag.OFF;
                    break;
                //case "ITEM_091":
                case "ITEM_092":
                    // 死亡保険金
                    if (sValue.Length != 0)
                    {
                        return (decimal.Parse(sValue) * 10000).ToString();
                    }
                    break;
            }
            return sValue;
        }

        /// <summary>
        /// 申込書（乗合限定告知型）
        /// </summary>
        /// <param name="sItemId"></param>
        /// <param name="sValue"></param>
        /// <returns></returns>
        private string ConvertValue30020001(string sItemId, string sValue)
        {
            // 申込書（限定告知型）と同一
            return ConvertValue26020001(sItemId, sValue);
        }

        /// <summary>
        /// 申込書（１年定期）
        /// </summary>
        /// <param name="sItemId"></param>
        /// <param name="sValue"></param>
        /// <returns></returns>
        private string ConvertValue50010001(string sItemId, string sValue)
        {
            switch (sItemId)
            {
                case "ITEM_011":
                    // 契約者 性別
                    if (sValue.Length == 0)
                    {
                        sValue = "0";
                    }
                    break;
                case "ITEM_055":
                    // 保険金額
                    if (sValue.Length != 0)
                    {
                        sValue = (decimal.Parse(sValue) * 10000).ToString();
                    }
                    break;
                case "ITEM_073":
                    // リビング・ニーズ特約
                    if (sValue.Length == 0)
                    {
                        sValue = Consts.Flag.OFF;
                    }
                    break;
            }
            return sValue;
        }

        /// <summary>
        /// 取扱者報告書（共通）
        /// </summary>
        /// <param name="sItemId"></param>
        /// <param name="sValue"></param>
        /// <returns></returns>
        private string ConvertValue22010001(string sItemId, string sValue)
        {
            switch (sItemId)
            {
                case "ITEM_018":
                    // 申込経路
                    if (sValue.Length != 0
                        && !Config.ReadNotCharNarrowInput.Equals(sValue))
                    {
                        sValue = sValue.PadLeft(2, '0');
                    }
                    break;
            }
            return sValue;
        }

        /// <summary>
        /// 告知書（総合型）
        /// </summary>
        /// <param name="sItemId"></param>
        /// <param name="oValue"></param>
        /// <returns></returns>
        private string ConvertValue14010001(string sItemId, string sValue)
        {
            // 変換無し
            return sValue;
        }
        #endregion
    }
}
