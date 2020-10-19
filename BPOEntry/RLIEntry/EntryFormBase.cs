using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BPOEntry.Tables;
using Common;
using NLog;

namespace BPOEntry.EntryForms
{
    public partial class EntryFormBase : Form
    {
        #region CreateParams
        /// <summary>
        /// CreateParams
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

        #region 変数

        /// <summary>
        /// 帳票ID
        /// </summary>
        protected string _sDocId { get; set; }

        /// <summary>
        /// 画像取込日
        /// </summary>
        protected string _sImageCaptureDate { get; set; }

        /// <summary>
        /// 画像取込回数
        /// </summary>
        protected string _sImageCaptureNum { get; set; }

        /// <summary>
        /// 入力単位
        /// </summary>
        protected string _sEntryUnit { get; set; }

        /// <summary>
        /// レコード区分
        /// </summary>
        protected string _sRecordKbn { get; set; }

        /// <summary>
        /// 検証フラグ
        /// </summary>
        protected string _sVerifyFlag { get; set; }

        /// <summary>
        /// log
        /// </summary>
        protected static Logger _Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// データアクセス
        /// </summary>
        protected static DaoBPOEntry _Dao = new DaoBPOEntry();

        // トータル件数
        protected int _iTotalCount = 0;

        // 処理中件数
        protected int _iProcessingCount = 0;

        protected bool needSkipValidation = false;

        protected bool hasBeforeRecord = false;

        // 画像情報
        protected DataTable dtImageInfo = new DataTable();

        // 帳票情報
        protected DataTable dtM_DOC = new DataTable();

        // 画像連番
        protected int _iImageSeq = 0;

        // 入力項目
        protected DataTable dtEntry = new DataTable();

        // 各TextBoxのの拡張プロパティ
        protected Dictionary<TextBox, EntryItemExtensionProperties> _extProps;

        // 前回フォーカス
        protected Control backFocusTextBox;

        // 入力項目リスト
        protected List<TextBox> inputItems;

        // 領域情報
        protected EntryItemRegions autoScrollRegions;

        // 郵便番号情報
        protected Dictionary<TextBox, List<ZipAddrInfo>> dicZipInfo = new Dictionary<TextBox, List<ZipAddrInfo>>();

        // 代理店・取扱者情報
        protected Dictionary<TextBox[], List<HandlerInfo>> dicHandlerInfo = new Dictionary<TextBox[], List<HandlerInfo>>();

        // 差異がある項目数
        private Dictionary<TextBox, int> dicDiffCount = new Dictionary<TextBox, int>();

        private List<Control> IgnoreControls = new List<Control>();

        //private bool bBatchCompareFlag = false;

        #endregion

        public EntryFormBase()
            : this(null, null, null, null, null) { }

        public EntryFormBase(string sDocId, string sImageCaptureDate, string sImageCaptureNum, string sEntryUnit, string sRecordKbn)
        {
            if (DesignMode)
            {
                return;
            }

            InitializeComponent();

            if (Config.IsTestMode)
            {
                this.lblTitle.BackColor = Consts.TITLE_BACK_COLOR.TEST;
            }

            this._sDocId = sDocId;
            this._sImageCaptureDate = sImageCaptureDate;
            this._sImageCaptureNum = sImageCaptureNum;
            this._sEntryUnit = sEntryUnit;
            this._sRecordKbn = sRecordKbn;
            this._sVerifyFlag = Consts.Flag.OFF;
        }

        /// <summary>
        /// EntryFormBase_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryFormBase_Load(object sender, EventArgs e)
        {
            if (DesignMode)
            {
                return;
            }

            // 設定ファイルを読み込む
            this.autoScrollRegions = XmlFileLoader.CreateObject<EntryItemRegions>(String.Format(@"xml\{0}\{1}_{2}_{3}.xml", Config.UserId, Config.TokuisakiCode, Config.HinmeiCode, _sDocId));

            // 帳票情報取得
            dtM_DOC = _Dao.SelectM_DOC(_sDocId);
            lblTitle.Text = dtM_DOC.Rows[0]["DOC_NAME"].ToString();

            // 郵便番号・住所情報設定
            InitZipItems();

            // 取扱者情報設定
            InitHandlerItems();

            inputItems = new List<TextBox>();

            int loopCount = int.Parse(dtM_DOC.Rows[0]["ENTRY_ITEMS_NUM"].ToString().Trim());

            for (int i = 1; i <= loopCount; i++)
            {
                object o = this.GetType().GetField("text" + i.ToString("d3"),
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
                TextBox txtbox = (TextBox)o;
                inputItems.Add(txtbox);
            }

            // OCR連携フラグ
            _sOcrCooperationFlag = dtM_DOC.Rows[0]["OCR_COOPERATION_FLAG"].ToString();

            // OCR上書きフラグ
            _sOcrOverwriteFlag = dtM_DOC.Rows[0]["OCR_OVERWRITE_FLAG"].ToString();

            // バッチコンペア
            //this.bBatchCompareFlag = Consts.Flag.ON.Equals(dtM_DOC.Rows[0]["BATCH_COMPARE_FLAG"].ToString()) ? true : false;

            // 全入力項目に共通のイベントハンドラを登録します
            inputItems.ForEach(o =>
            {
                o.Leave += TextBox_Leave;
                o.KeyPress += TextBox_KeyPress;
                o.Validating += TextBox_Validating;
                o.Validated += TextBox_Validated;

                // 管理者モードの場合、フォーカスインの時、再入力画面を表示する
                if (Consts.RecordKbn.ADMIN.Equals(this._sRecordKbn))
                {
                    o.Enter += TextBox_Enter_Admin;
                }
                else
                {
                    // 表示画像の自動スクロール
                    o.Enter += TextBox_Enter;
                }
            });

            IgnoreControls = new Control[] { btnClose, btnBack, chkAutoScroll }.ToList();

            this._extProps = new Dictionary<TextBox, EntryItemExtensionProperties>();

            // 入力ルール設定
            InitTextBoxExtentionProperties();

            // ImeMode設定
            SetTextBoxImeMode();

            // MaxLength設定
            SetTextBoxMaxLength();

            // AI-OCR Enabled設定
            SetTextBoxEnabled();

            // ＤＢオープン
            _Dao.Open(Config.DSN);

            if (Consts.RecordKbn.ADMIN.Equals(this._sRecordKbn))
            {
                dtImageInfo = _Dao.SelectEntryImageInfoAdmin(_sDocId, _sImageCaptureDate, _sImageCaptureNum, _sEntryUnit);
            }
            else
            {
                dtImageInfo = _Dao.SelectEntryImageInfo(_sDocId, _sImageCaptureDate, _sImageCaptureNum, _sEntryUnit);
            }
            // トータル件数取得
            _iTotalCount = dtImageInfo.Rows.Count;

            // ?件目処理
            _iProcessingCount++;
            if (Consts.RecordKbn.Entry_1st.Equals(this._sRecordKbn))
            {
                _iProcessingCount += dtImageInfo.Select("RECORD_KBN_1_ENTRY_FLAG='1'").Count();
            }
            else if (Consts.RecordKbn.Entry_2nd.Equals(this._sRecordKbn))
            {
                _iProcessingCount += dtImageInfo.Select("RECORD_KBN_2_ENTRY_FLAG='1'").Count();
            }

            // 最後まで登録済の場合、１件目を表示
            if (_iProcessingCount == (_iTotalCount + 1))
            {
                _iProcessingCount = 1;
            }

            if (_iTotalCount == 0)
            {
                this.Hide();
                //MessageBox.Show("入力対象データがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("エントリ対象データがありません。", this.lblTitle.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }

            ShowNext();
        }

        /// <summary>
        /// ダイアログ キーを処理します。
        /// </summary>
        /// <param name="keyData">処理するキーを表す Keys 値の 1 つ。</param>
        /// <returns>キーがコントロールによって処理された場合は true。それ以外の場合は false。</returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            //bCheck = false;
            // Enterキー押下によるフォーカス移動
            if (((keyData & Keys.KeyCode) == Keys.Return) &&
                ((keyData & (Keys.Alt | Keys.Control)) == Keys.None))
            {
                if (!(this.ActiveControl is Button))
                {
                    this.MoveNextControl();
                    return true;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// 次のコントロールをアクティブにします。
        /// </summary>
        /// <param name="forward">タブオーダー内を前方に移動する場合はtrue。後方に移動する場合はfalse。</param>
        protected void MoveNextControl(bool forward = true)
        {
            if (forward)
            {
                //bCheck = true;
                SendKeys.Send("{TAB}");
            }
            else
            {
                SendKeys.Send("+{TAB}");
            }
        }

        /// <summary>
        /// 登録
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRegist_Click(object sender, EventArgs e)
        {
            // エントリ更新
            List<string> entryList = new List<string>();
            List<string> entryListBefore = new List<string>();
            List<string> UpdateTargetColumn = new List<string>();

            int loopCount = inputItems.Count;
            for (int i = 1; i <= loopCount; i++)
            {
                object o = this.GetType().GetField("text" + i.ToString("d3"),
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

                TextBox txtbox = (TextBox)o;
                if (!ValidateEmpty(txtbox) && txtbox.Enabled)
                {
                    backFocusTextBox = txtbox;
                    backFocusTextBox.Focus();
                    return;
                }
                entryList.Add(txtbox.Text);
                entryListBefore.Add(txtbox.Tag.ToString());
                if (txtbox.Enabled)
                {
                    UpdateTargetColumn.Add(Consts.Flag.ON);
                }
                else
                {
                    UpdateTargetColumn.Add(Consts.Flag.OFF);
                }
            }

            try
            {
                _Dao.BeginTrans();

                int registResult = -1;
                // 管理者モードの場合の更新
                if (Consts.RecordKbn.ADMIN.Equals(this._sRecordKbn))
                {
                    registResult = _Dao.UpdateEntryDataByAdmin(_sDocId, _sImageCaptureDate, _sImageCaptureNum, _sEntryUnit, _iImageSeq, this._sRecordKbn, entryList);
                    // すべて登録完了後、入力単位データの状態をコンペア済にする
                    if (_iProcessingCount == _iTotalCount)
                    {
                        registResult = _Dao.UpdateD_ENTRY_UNIT(_sDocId, _sImageCaptureDate, _sImageCaptureNum, _sEntryUnit, Consts.EntryUnitStatus.COMPARE_END);
                    }
                }
                else
                {
                    var DummyItemFlagList = new List<string>();
                    // D_ENTRY 更新
                    registResult = _Dao.MergeEntryData(_sDocId, _sImageCaptureDate, _sImageCaptureNum, _sEntryUnit, _iImageSeq, this._sRecordKbn, entryList, entryListBefore, UpdateTargetColumn, _sVerifyFlag,DummyItemFlagList);

                    // D_IMAGE_INFO 更新
                    registResult = _Dao.UpdateEntryFlag(_sDocId, _sImageCaptureDate, _sImageCaptureNum, _sEntryUnit, _iImageSeq, this._sRecordKbn);

                    // ユーザの入力が完了したかどうかを判断する
                    if (IsEntryStatusEnd(this._sRecordKbn)
                        && _iProcessingCount == _iTotalCount)
                    {
                        registResult = _Dao.UpdateD_ENTRY_STATUS(_sDocId, _sImageCaptureDate, _sImageCaptureNum, _sEntryUnit, this._sRecordKbn, Consts.EntryStatus.ENTRY_END);

                        //// 入力単位データの状態区分更新
                        //if (this.bBatchCompareFlag)
                        //{
                        //    // バッチ
                        //    registResult = _Dao.InsertCompareTrigger(_sImageCaptureDate, _sImageCaptureNum, _sDocId, _sEntryUnit, this._sRecordKbn);
                        //}
                        //else
                        //{
                            // リアルタイム
                            registResult = UpdateEntryUnitStatus();
                        //}
                    }
                }

                // コミット
                _Dao.CommitTrans();

                string slogMessage = string.Format("イメージ連携年月日:{0} イメージ連携回数:{1} 帳票ID:{2} エントリ単位:{3} イメージ連番:{4} レコード区分:{5}"
                                          , _sImageCaptureDate, _sImageCaptureNum, _sDocId, _sEntryUnit, _iImageSeq, _sRecordKbn);
                _Log.Info(slogMessage);

                // 処理中件数Up
                _iProcessingCount++;

                if (_iProcessingCount > _iTotalCount)
                {
                    //MessageBox.Show("エントリー単位分の登録が完了しました。", " 楽天生命 エントリー業務[" + this.lblTitle.Text + "]",
                    //    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show(String.Format("{0} の登録が完了しました。", Utils.EditEntryBatchId(_sImageCaptureDate, _sImageCaptureNum, _sDocId, _sEntryUnit)), this.lblTitle.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    EndProc();
                    return;
                }

                ShowNext();
            }
            catch
            {
                _Dao.RollbackTrans();
            }
        }

        /// <summary>
        /// 指定されたユーザの入力がすべて完了されたかを確認
        /// </summary>
        /// <param name="user_kbn"></param>
        /// <returns></returns>
        private bool IsEntryStatusEnd(string user_kbn)
        {
            foreach (DataRow row in dtImageInfo.Rows)
            {
                DataTable dt = _Dao.SelectEntryData(this._sDocId, this._sImageCaptureDate, this._sImageCaptureNum, this._sEntryUnit, int.Parse(row["IMAGE_SEQ"].ToString()), user_kbn);
                if (dt == null || dt.Rows.Count == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private int UpdateEntryUnitStatus()
        {
            int registResult = -1;
            DataTable dtEntryStatus = _Dao.SelectEntryStatusData(this._sDocId, this._sImageCaptureDate, this._sImageCaptureNum, this._sEntryUnit);

            // ユーザ区分が1の入力状態を取得する
            DataRow[] row_inp1 = dtEntryStatus.Select("RECORD_KBN = '" + Consts.RecordKbn.Entry_1st + "'");
            string status_inp1 = row_inp1[0]["ENTRY_STATUS"].ToString().Trim();
            // ユーザ区分が1の入力状態を取得する
            DataRow[] row_inp2 = dtEntryStatus.Select("RECORD_KBN = '" + Consts.RecordKbn.Entry_2nd + "'");
            string status_inp2 = row_inp2[0]["ENTRY_STATUS"].ToString().Trim();

            // １人目、２人目分の入力が両方入力済になっていれば、入力単位データの状態区分をコンペア中("5")へ更新
            if (status_inp1 == Consts.EntryStatus.ENTRY_END && status_inp2 == Consts.EntryStatus.ENTRY_END)
            {
                registResult = _Dao.UpdateD_ENTRY_UNIT(_sDocId, _sImageCaptureDate, _sImageCaptureNum, _sEntryUnit, Consts.EntryUnitStatus.COMPARE_ING);

                // コンペア処理を行う
                bool diff_flg = DoCompare();

                // 差異があれば入力単位データの状態区分を管理者修正中("6")へ更新
                // 差異がなければ入力単位データの状態区分をコンペア済("7")へ更新
                string unitStatus = string.Empty;
                if (diff_flg)
                {
                    unitStatus = Consts.EntryUnitStatus.COMPARE_EDT;
                }
                else
                {
                    unitStatus = Consts.EntryUnitStatus.COMPARE_END;
                }
                registResult = _Dao.UpdateD_ENTRY_UNIT(_sDocId, _sImageCaptureDate, _sImageCaptureNum, _sEntryUnit, unitStatus);
            }
            return registResult;
        }

        private bool DoCompare()
        {
            bool diff_flg = false;
            foreach (DataRow row in dtImageInfo.Rows)
            {
                int imageSq = int.Parse(row["IMAGE_SEQ"].ToString().Trim());
                // ユーザ１、２の入力データをそれぞれ取得して格納する
                DataTable entryData1 = _Dao.SelectEntryData(this._sDocId, this._sImageCaptureDate, this._sImageCaptureNum, this._sEntryUnit, imageSq, Consts.RecordKbn.Entry_1st);
                Dictionary<string, string> entryData_Inp1 = GetEntryDic(entryData1);
                DataTable entryData2 = _Dao.SelectEntryData(this._sDocId, this._sImageCaptureDate, this._sImageCaptureNum, this._sEntryUnit, imageSq, Consts.RecordKbn.Entry_2nd);
                Dictionary<string, string> entryData_Inp2 = GetEntryDic(entryData2);

                // 差異を検出して、ユーザ区分が0のデータを登録する
                List<D_ENTRY> entry_List = new List<D_ENTRY>();
                foreach (string entry_item in entryData_Inp1.Keys)
                {
                    D_ENTRY entry_record = new D_ENTRY(this._sImageCaptureDate, this._sImageCaptureNum, this._sDocId, this._sEntryUnit, imageSq, Consts.RecordKbn.ADMIN, entry_item);

                    // 入力１と２の入力値が同じかを判断する
                    if (entryData_Inp1[entry_item] == entryData_Inp2[entry_item])
                    {
                        entry_record.VALUE = entryData_Inp1[entry_item];
                        entry_record.DIFF_FLAG = Consts.DiffFlag.OFF;
                    }
                    else
                    {
                        entry_record.VALUE = string.Empty;
                        entry_record.DIFF_FLAG = Consts.DiffFlag.ON;
                        diff_flg = true;
                    }
                    entry_record.INS_USER_ID = Program.LoginUser.USER_ID;
                    //entry_record.UPD_USER_ID = Program.LoginUser.USER_ID;
                    entry_List.Add(entry_record);
                }
                // ユーザ区分が0のデータを登録する
                _Dao.INSERT_D_ENTRY_ADMIN(entry_List);
            }
            return diff_flg;
        }

        private Dictionary<string, string> GetEntryDic(DataTable entryData)
        {
            Dictionary<string, string> entryDataDic = new Dictionary<string, string>();

            foreach (DataRow row in entryData.Rows)
            {
                if (!entryDataDic.Keys.Contains(row["ITEM_ID"].ToString().Trim()))
                {
                    entryDataDic.Add(row["ITEM_ID"].ToString().Trim(), row["VALUE"].ToString().Trim());
                }
            }
            return entryDataDic;
        }

        #region 閉じるボタン
        /// <summary>
        /// 閉じる:_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this.lblTitle.Text + "を終了します。\nよろしいですか？", this.lblTitle.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                this.EndProc();
            }
            else
            {
                if (!Consts.RecordKbn.ADMIN.Equals(_sRecordKbn))
                {
                    var txtBox = this.Controls.Find("text001", true)[0] as TextBox;
                    txtBox.Select();
                }
            }
        }
        #endregion

        private void ShowImage()
        {
            // イメージ表示
            const float ZOOM = 0.35f;
            var img = Image.FromFile(dtImageInfo.Rows[_iProcessingCount - 1]["IMAGE_PATH"].ToString());
            this.pictureBox.Width = (int)(img.Width * ZOOM);
            this.pictureBox.Height = (int)(img.Height * ZOOM);
            this.pictureBox.Image = img;
            this._iImageSeq = int.Parse(dtImageInfo.Rows[_iProcessingCount - 1]["IMAGE_SEQ"].ToString());
        }

        private void ShowNext()
        {
            //// 処理状態表示
            //this.lblStatus.Text = String.Format("{0}入力 {1}/{2}"
            //                           , " 初回　"
            //                           , _iProcessingCount.ToString("#,0")
            //                           , _iTotalCount.ToString("#,0")
            //                           );
            // 処理状態表示
            string sRecordKbnName = string.Empty;
            if (Consts.RecordKbn.Entry_1st.Equals(this._sRecordKbn))
            {
                sRecordKbnName = "１人目入力";
            }
            else if (Consts.RecordKbn.Entry_2nd.Equals(this._sRecordKbn))
            {
                sRecordKbnName = "２人目入力";
            }
            else if (Consts.RecordKbn.ADMIN.Equals(this._sRecordKbn))
            {
                if (Consts.Flag.OFF.Equals(_sVerifyFlag))
                {
                    sRecordKbnName = "修正";
                }
                else
                {
                    sRecordKbnName = "検証";
                }
            }
            this.lblParams.Text = String.Format("エントリバッチID:{0}　{1}"
                                                , Utils.EditEntryBatchId(_sImageCaptureDate, _sImageCaptureNum, _sDocId, _sEntryUnit)
                                                , sRecordKbnName);

            this.lblStatus.Text = String.Format("{0}/{1}"
                                                , _iProcessingCount.ToString("d4")
                                                , _iTotalCount.ToString("d4"));

            ShowImage();

            dtEntry = _Dao.SelectEntryData(this._sDocId, this._sImageCaptureDate, this._sImageCaptureNum, this._sEntryUnit, _sRecordKbn);

            _iImageSeq = int.Parse(dtImageInfo.Rows[_iProcessingCount - 1]["IMAGE_SEQ"].ToString());

            // 前レコード
            DataRow[] rows = dtEntry.Select("IMAGE_SEQ = '" + _iImageSeq + "'", "ITEM_ID");
            dicDiffCount = new Dictionary<TextBox, int>();

            // テキストボックスの表示を初期化します。
            foreach (var item in this.inputItems.Select((textbox, index) => new { textbox, index }))
            {
                if (item.index + 1 <= rows.Count())
                {
                    item.textbox.Text = rows[item.index]["VALUE"].ToString();
                    item.textbox.Tag = rows[item.index]["VALUE"].ToString();

                    // 管理者モードの場合、差異がない項目を入力不可にする
                    if (Consts.RecordKbn.ADMIN.Equals(_sRecordKbn))
                    {
                        if (Consts.DiffFlag.OFF.Equals(rows[item.index]["DIFF_FLAG"].ToString()))
                        {
                            item.textbox.Enabled = false;
                            item.textbox.ReadOnly = false;
                            item.textbox.TabStop = false;
                            item.textbox.BackColor = SystemColors.Control;
                        }
                        else
                        {
                            item.textbox.Enabled = true;
                            item.textbox.ReadOnly = true;
                            item.textbox.TabStop = true;
                            item.textbox.BackColor = SystemColors.Window;
                            item.textbox.KeyDown += ReEntryTextBox_KeyDown;
                            dicDiffCount.Add(item.textbox, 1);
                        }
                    }
                }
                else
                {
                    item.textbox.Text = string.Empty;
                }
            }

            // 管理者モードの場合、差異がない項目を入力不可にする
            if (Consts.RecordKbn.ADMIN.Equals(this._sRecordKbn))
            {
                // 登録ボタンを制御する
                CheckDiffCount();
            }

            // 前レコードボタン制御
            btnBack.Enabled = (_iProcessingCount >= 2);

            hasBeforeRecord = false;
            this.chkAutoScroll.Checked = true;

            //先頭へフォーカスセット
            if (!Consts.RecordKbn.ADMIN.Equals(this._sRecordKbn))
            {
                InitScrollBar();
                foreach (TextBox tbox in this._extProps.Keys)
                {
                    if (tbox.Enabled)
                    {
                        tbox.SelectAll();
                        tbox.Focus();
                        break;
                    }
                }
            }
        }

        #region 終了処理
        /// <summary>
        /// 終了処理
        /// </summary>
        private void EndProc()
        {
            // 終了フラグセット
            needSkipValidation = true;

            // ＤＢクローズ
            _Dao.Close();

            // テキストボックス共通のハンドラを破棄する
            inputItems.ForEach(o =>
            {
                o.Leave -= TextBox_Leave;
                o.KeyPress -= TextBox_KeyPress;
                o.Validating -= TextBox_Validating;
                o.Validated -= TextBox_Validated;

                if (this._sRecordKbn == Consts.RecordKbn.ADMIN)
                {
                    o.Enter -= TextBox_Enter_Admin;
                }
                else
                {
                    o.Enter -= TextBox_Enter;
                }
            });

            this.Close();
            this.Dispose();
        }
        #endregion

        private void btnBack_Click(object sender, EventArgs e)
        {
            _iProcessingCount--;
            hasBeforeRecord = true;
            ShowNext();
        }

        /// <summary>
        /// 入力項目のImeModeを設定する
        /// </summary>
        private void SetTextBoxImeMode()
        {
            foreach (TextBox txtBox in this._extProps.Keys)
            {
                EntryItemExtensionProperties rule = this._extProps[txtBox];

                switch (rule.InputElement)
                {
                    case "9":
                        txtBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
                        break;
                    case "X":
                        txtBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
                        break;
                    case "NK":
                        txtBox.ImeMode = System.Windows.Forms.ImeMode.Katakana;
                        break;
                    default:
                        txtBox.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
                        break;
                }
            }
        }

        private void SetTextBoxMaxLength()
        {
            foreach (TextBox txtBox in this._extProps.Keys)
            {
                EntryItemExtensionProperties rule = this._extProps[txtBox];
                txtBox.MaxLength = rule.InputMaxLength;
            }
        }

        private void SetTextBoxEnabled()
        {
            if (Consts.Flag.ON.Equals(_sOcrCooperationFlag)
                && !Consts.Flag.ON.Equals(_sOcrOverwriteFlag))
            {
                if (Consts.RecordKbn.Entry_2nd.Equals(this._sRecordKbn))
                {
                    DataTable dtM_ITEM_ID_CONV_OCR = _Dao.SelectM_ITEM_ID_CONV_OCR(_sDocId);
                    foreach (TextBox txtBox in this._extProps.Keys)
                    {
                        DataRow[] drM_ITEM_ID_CONV_OCR = dtM_ITEM_ID_CONV_OCR.Select(String.Format("ITEM_ID_ENTRY='{0}'"
                                                                                    , String.Format("ITEM_{0}"
                                                                                    , txtBox.Name.Substring(4, 3))));
                        if (drM_ITEM_ID_CONV_OCR.Length != 0)
                        {
                            if (Consts.Flag.ON.Equals(drM_ITEM_ID_CONV_OCR[0]["IMPORT_FLAG"].ToString()))
                            {
                                this._extProps[txtBox].Input2Flag = false;
                            }
                        }
                    }

                    foreach (TextBox txtBox in this._extProps.Keys)
                    {
                        EntryItemExtensionProperties rule = this._extProps[txtBox];
                        txtBox.Enabled = rule.Input2Flag;
                    }
                }
            }
        }

        /// <summary>
        /// 入力制限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 共通的に許容するキー入力は無視します。
            var acceptKeys = new List<char>() { '\b', Config.ReadNotCharNarrowInput[0]};
            if (acceptKeys.Contains(e.KeyChar))
                return;

            var textbox = sender as TextBox;
            if (textbox == null)
                return;

            // ルールが未設定のテキストボックスの入力は無視します。
            if (!this._extProps.ContainsKey(textbox))
                return;

            // 入力可能な文字列パターンの設定が無ければ無視します。
            string pattern = this._extProps[textbox].AcceptKeyCharsPattern;
            if (string.IsNullOrEmpty(pattern))
                return;

            // 入力値が指定範囲以外ならハンドル済みに設定します。
            if (!Regex.IsMatch(e.KeyChar.ToString(), pattern))
                e.Handled = true;
        }

        private void TextBox_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            // 登録ボタン以外のボタンへの遷移時には処理しません。
            if (IgnoreControls.Contains(this.ActiveControl))
                return;

            // 拡張プロパティの登録が無ければ中断します。
            if (!this._extProps.ContainsKey(textBox))
                return;

            // 必須項目は未入力の場合にフォーカスを戻します。
            if (!ValidateEmpty(textBox))
            {
                backFocusTextBox.Focus();
                //MessageBox.Show("必須項目は必ず入力してください。");
                MessageBox.Show(String.Format("「{0}」は入力必須項目です。", this._extProps[textBox].ItemName), this.lblTitle.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
                return;
            }

            // 入力可能パターン違反の場合にフォーカスを戻します。
            string pattern = this._extProps[textBox].ValidStringPattern;
            if (!string.IsNullOrEmpty(pattern))
            {
                // 判別不能文字を除いてチェックする
                string omitedText = textBox.Text.Replace(Config.ReadNotCharNarrowInput, string.Empty).Replace(Config.ReadNotCharWideInput, string.Empty);
                if (!Regex.IsMatch(omitedText, pattern))
                {
                    backFocusTextBox.Focus();
                    //MessageBox.Show("「" + textBox.Text + "」は入力できません。");
                    MessageBox.Show(String.Format("「{0}」に「{1}」は入力出来ません。", this._extProps[textBox].ItemName, textBox.Text), this.lblTitle.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                    return;
                }
            }

            // 全角ひらがな項目のみ
            if (this._extProps[textBox].InputElement == "NH")
            {
                // 入力可能全角文字チェック
                if (!Utils.IsAllowMultiByteChar(textBox.Text))
                {
                    backFocusTextBox.Focus();
                    MessageBox.Show(String.Format("「{0}」に入力禁止文字が含まれています。", this._extProps[textBox].ItemName), this.lblTitle.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                    return;
                }
            }
        }

        /// <summary>
        /// TextBox.Validated
        /// </summary>
        void TextBox_Validated(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            // 全角項目は、半角⇒全角へ変換します。
            if (this._extProps[textBox].InputElement.StartsWith("N"))
                textBox.Text = Microsoft.VisualBasic.Strings.StrConv(textBox.Text, Microsoft.VisualBasic.VbStrConv.Wide);
        }

        /// <summary>
        /// TextBox.Leave
        /// </summary>
        private void TextBox_Leave(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            // 強調色を標準の色に戻します。
            textBox.BackColor = SystemColors.Window;

            // 郵便番号の場合、住所を自動設定する
            // 入力担当者の場合のみ実行する
            if (this._sRecordKbn != Consts.RecordKbn.ADMIN && dicZipInfo.ContainsKey(textBox))
            {
                SetZipInfo(textBox);
            }

            // 募集人の場合、代理店名と募集人名（姓・名）を自動設定する
            if (this._sRecordKbn != Consts.RecordKbn.ADMIN)
            {
                foreach (var key in dicHandlerInfo.Keys)
                {
                    //if (key[0] == textBox)
                    //{
                    //MessageBox.Show("代理店");
                    //}
                    if (key[1] == textBox)
                    {
                        //MessageBox.Show("募集人");
                        SetHandlerInfo(key);
                    }
                }
            }
        }

        private bool ValidateEmpty(TextBox textBox)
        {
            // 前後の空白を取り除きます。
            textBox.Text = textBox.Text.Trim();

            // 必須項目は未入力の場合にフォーカスを戻します。
            if (this._extProps[textBox].IsRequired)
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// TextBox.Enterイベント
        /// </summary>
        private void TextBox_Enter(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            // 強調色に変更します。
            textBox.BackColor = Color.LightSalmon;

            // このテキストボックスをフォーカスの戻り先に設定します。
            backFocusTextBox = textBox;

            // 表示画像の自動スクロールを処理します。
            this.AutoSyncScrollImage(textBox);

            // 拡張プロパティに対する処理を行います。
            if (this._extProps.ContainsKey(textBox))
            {
                // TipsをTextBoxの右側に表示します。
                // 表示するTipsが無ければ非表示にします。
                if (string.IsNullOrEmpty(this._extProps[textBox].Tips))
                {
                    this.toolTips.Hide(this);
                }
                else
                {
                    var pt = new Point(0, 0);
                    WindowsFormUtils.AbsolutePositionFromForm(textBox, ref pt);
                    pt.X += textBox.Width + 15;
                    this.toolTips.Show(this._extProps[textBox].Tips, this, pt);
                }
            }
        }

        /// <summary>
        /// TextBox.Enterイベント(管理者モード)
        /// </summary>
        private void TextBox_Enter_Admin(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            // 強調色に変更します。
            textBox.BackColor = Color.LightSalmon;

            // このテキストボックスをフォーカスの戻り先に設定します。
            backFocusTextBox = textBox;

            // 表示画像の自動スクロールを処理します。
            this.AutoSyncScrollImage(textBox);

            // 入力項目の自動スクロール
            textBox.Focus();

            // 再入力画面を表示する
            ShowReEntryForm(textBox);
        }

        /// <summary>
        /// 郵便番号情報自動設定
        /// </summary>
        /// <param name="txtBox"></param>
        private void SetZipInfo(TextBox txtBox)
        {
            // 郵便番号の入力値が7桁でないと無視する
            if (txtBox.Text.Trim().Length != 7)
            {
                return;
            }

            // 住所１、住所２、住所３、同じくカナの６カ所のどれか一つでも空欄ではないと無視する
            List<ZipAddrInfo> addrInfo = dicZipInfo[txtBox];
            foreach (ZipAddrInfo zipAddrInfo in addrInfo)
            {
                if (!string.IsNullOrEmpty(zipAddrInfo.AddrItem.Text.Trim()))
                {
                    return;
                }
            }

            // 上記以外の場合、郵便番号で検索して住所を自動設定する
            DataTable dtZip = _Dao.SelectZipInfo(txtBox.Text.Trim());

            if (dtZip == null || dtZip.Rows.Count == 0)
            {
                return;
            }

            DataRow rowZip = dtZip.Rows[0];

            string temp = string.Empty;
            foreach (ZipAddrInfo zipAddrInfo in addrInfo)
            {
                temp = string.Empty;
                foreach (string column in zipAddrInfo.AddrValues)
                {
                    temp += rowZip[column];
                }

                // カナ住所は全角に変換
                if (zipAddrInfo.IsKanaAddr)
                {
                    temp = Microsoft.VisualBasic.Strings.StrConv(temp, Microsoft.VisualBasic.VbStrConv.Wide);
                }

                // 最大長を超えた場合、カットする
                if (temp.Length > this._extProps[zipAddrInfo.AddrItem].InputMaxLength)
                {
                    temp = temp.Substring(0, this._extProps[zipAddrInfo.AddrItem].InputMaxLength);
                }
                zipAddrInfo.AddrItem.Text = temp;
                zipAddrInfo.AddrItem.Focus();
            }
        }

        /// <summary>
        /// 代理店・取扱者情報自動設定
        /// </summary>
        /// <param name="txtBox"></param>
        private void SetHandlerInfo(TextBox[] txtBox)
        {
            // 代理店コードの入力値が7桁、取扱者コードの入力値が3桁でないと無視する
            if (txtBox[0].Text.Trim().Length != 6
                || txtBox[1].Text.Trim().Length != 3)
            {
                return;
            }

            List<HandlerInfo> lsHandlerInfo = dicHandlerInfo[txtBox];
            foreach (var zipAddrInfo in lsHandlerInfo)
            {
                if (!string.IsNullOrEmpty(zipAddrInfo.Item.Text.Trim()))
                {
                    return;
                }
            }

            DataTable dtHandler = _Dao.SelectHandlerInfo(txtBox[0].Text, txtBox[1].Text);
            if (dtHandler == null || dtHandler.Rows.Count == 0)
            {
                return;
            }

            DataRow drHandler = dtHandler.Rows[0];

            string temp = string.Empty;
            foreach (var HandlerInfo in lsHandlerInfo)
            {
                temp = string.Empty;
                foreach (string column in HandlerInfo.AddrValues)
                {
                    temp += drHandler[column];
                }

                // 最大長を超えた場合、カットする
                if (temp.Length > this._extProps[HandlerInfo.Item].InputMaxLength)
                {
                    temp = temp.Substring(0, this._extProps[HandlerInfo.Item].InputMaxLength);
                }
                HandlerInfo.Item.Text = temp;
            }
        }

        private void ShowReEntryForm(TextBox textBox)
        {
            string item_id = textBox.Name.Substring(4);
            item_id = (int.Parse(item_id) /*- 1*/).ToString("d3");
            item_id = "ITEM_" + item_id;

            int textIndex = dicDiffCount.Keys.ToList().IndexOf(textBox) + 1;

            ReEntryForm frm = new ReEntryForm(this._sDocId, this._sImageCaptureDate, this._sImageCaptureNum,
                this._sEntryUnit, this._iImageSeq.ToString(), item_id, this._extProps[textBox], textIndex, dicDiffCount.Count);
            frm.ShowDialog();

            if (frm.IsReEntry())
            {
                textBox.Text = frm.GetRetValue();

                // 画面で記憶する差異フラグを0に更新する
                dicDiffCount[textBox] = 0;
                CheckDiffCount();
            }
        }

        /// <summary>
        /// 自動同期スクロール処理
        /// </summary>
        private void AutoSyncScrollImage(TextBox textBox)
        {
            if (this.chkAutoScroll.Checked == false)
                return;

            var info = this.autoScrollRegions.Find(textBox.Name);
            if (info == null)
                return;

            this.pnlImageArea.AutoScrollPosition = new Point(info.OriginX, info.OriginY);
        }

        /// <summary>
        /// 差異がある項目数をチェックして、なければ登録ボタンを有効にする
        /// </summary>
        private void CheckDiffCount()
        {
            btnRegist.Enabled = !dicDiffCount.Values.Any(flg => flg == 1);
        }

        /// <summary>
        /// Panel.Scroll
        /// </summary>
        private void pnlImageArea_Scroll(object sender, ScrollEventArgs e)
        {
            // ユーザーによるスクロールが行われたとき、自動スクロールのチェックを外します。
            this.chkAutoScroll.Checked = false;
        }

        private void EntryFormBase_KeyDown(object sender, KeyEventArgs e)
        {
            // F12キーで自動スクロールのチェック状態を反転します。
            if (e.KeyCode == Keys.F12)
                this.chkAutoScroll.Checked = !this.chkAutoScroll.Checked;
        }

        private void ReEntryTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;

            // F2キーで再入力画面を開く
            if (e.KeyCode == Keys.F2)
            {
                ShowReEntryForm(textBox);
            }
        }

        #region MustOverrideメソッド
        // 基底フォームをabstructにするとデザイナでエラーが発生するためvirtualとしていますが、
        // 必ず継承先でオーバーライドして使用します。

        /// <summary>
        /// 入力項目の拡張プロパティを初期化する
        /// </summary>
        protected virtual void InitTextBoxExtentionProperties() { }

        /// <summary>
        /// 郵便番号自動入力対象を初期化する
        /// </summary>
        protected virtual void InitZipItems() { }
        protected virtual void InitHandlerItems() { }

        /// <summary>
        /// スクロールバー初期設定
        // </summary>
        protected virtual void InitScrollBar() { }

        #endregion

        //private string GetEntryBatchId()
        //{
        //    return String.Format("{0}{1}{2}-{3}-{4}"
        //                                        , Config.UserId
        //                                        , this._sImageCaptureDate
        //                                        , this._sImageCaptureNum
        //                                        , this._sDocId
        //                                        , this._sEntryUnit);
        //}

        // OCR連携フラグ
        protected string _sOcrCooperationFlag { get; set; }

        public string _sOcrOverwriteFlag { get; set; }
    }
}
