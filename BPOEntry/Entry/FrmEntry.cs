using Common;
using Microsoft.VisualBasic.FileIO;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BPOEntry.EntryForms
{
    public partial class FrmEntry : Form
    {
        #region CreateParams
        /// <summary>
        /// CreateParams
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_NOCLOSE = 0x200;
                CreateParams createParams = base.CreateParams;
                createParams.ClassStyle |= CS_NOCLOSE;
                return createParams;
            }
        }
        #endregion

        #region 変数
        /// <summary>
        /// GroupBox
        /// </summary>
        private GroupBox[] _gbs = null;
        //private Button[] _btns = null;
        /// <summary>
        /// PictureBox
        /// </summary>
        private PictureBox[] _pbs = null;// new List<PictureBox>();
        //private List<System.Windows.Forms.GroupBox> _gbs = new List<System.Windows.Forms.GroupBox>();

        /// <summary>
        /// 
        /// </summary>
        private GroupBox _gb = new GroupBox();

        /// <summary>
        /// 
        /// </summary>
        private TableLayoutPanel TableLayoutPanel = null;

        /// <summary>
        /// CTextBox
        /// </summary>
        //public CTextBox.CTextBox[] _tbs = null;

        /// <summary>
        /// Lable
        /// </summary>
        private List<Label> _lbs = new List<Label>();


        /// <summary>
        /// 帳票ID
        /// </summary>
        protected string _DOC_ID { get; set; }

        /// <summary>
        /// 帳票ID（エントリ用）
        /// </summary>
        protected string _DOC_ID_ENTRY { get; set; }

        /// <summary>
        /// 画像連携日
        /// </summary>
        protected string _IMAGE_CAPTURE_DATE { get; set; }

        /// <summary>
        /// 画像連携回数
        /// </summary>
        protected string _IMAGE_CAPTURE_NUM { get; set; }

        /// <summary>
        /// 入力単位
        /// </summary>
        protected string _ENTRY_UNIT { get; set; }

        /// <summary>
        /// 画像連番
        /// </summary>
        protected int _IMAGE_SEQ { get; set; } = 0;

        /// <summary>
        /// レコード区分
        /// </summary>
        protected string _RECORD_KBN { get; set; }

        /// <summary>
        /// ダミーアイテムフラグ
        /// </summary>
        protected bool _DUMMY_ITEM_FLAG { get; set; } = false;

        /// <summary>
        /// 検証モード
        /// </summary>
        protected bool _IsVerifyMode { get; set; }

        /// <summary>
        /// ステータス
        /// </summary>
        protected string _STATUS { get; set; }

        /// <summary>
        /// OCR連携フラグ 
        /// </summary>
        //private string _OCR_COOPERATION_FLAG { get; set; }

        /// <summary>
        /// 自動レイアウトモード
        /// </summary>
        private bool _IsAutoLayoutMode { get; set; }

        /// <summary>
        /// バッチコンペアフラグ
        /// </summary>
        //private string _BATCH_COMPARE_FLAG { get; set; }

        /// <summary>
        /// エントリ方法 
        /// </summary>
        public string _ENTRY_METHOD { get; set; }

        /// <summary>
        /// エントリユニットID
        /// </summary>
        private string _ENTRY_UNIT_ID { get; set; }

        /// <summary>
        /// log
        /// </summary>
        protected static Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// データアクセス
        /// </summary>
        protected static DaoEntry _dao = new DaoEntry();

        /// <summary>
        /// ユニット内トータル件数
        /// </summary>
        protected int _TotalCount = 0;

        /// <summary>
        /// 処理中件数
        /// </summary>
        protected int _ProcessingCount = 0;

        protected bool needSkipValidation = false;

        protected bool hasBeforeRecord = false;

        /// <summary>
        /// 画像情報
        /// </summary>
        protected DataTable dtD_IMAGE_INFO = null;

        /// <summary>
        /// 帳票情報
        /// </summary>
        protected DataTable dtM_DOC = null;

        // 入力項目
        //protected DataTable dtEntry = new DataTable();

        // 各TextBoxのの拡張プロパティ
        public Dictionary<CTextBox.CTextBox, BPOEntryItemExtensionProperties> _extProps;

        // 前回フォーカス
        protected Control backFocusTextBox;

        // 入力項目リスト
        //protected List<CTextBox.CTextBox> EntryItems;

        /// <summary>
        /// CTextBox
        /// </summary>
        public CTextBox.CTextBox[] _tbs = null;

        // 領域情報
        protected EntryItemRegions autoScrollRegions;

        #region マスタ参照
        // 郵便番号情報
        protected Dictionary<CTextBox.CTextBox, List<BPOZipAddrInfo>> dicZipInfo = new Dictionary<CTextBox.CTextBox, List<BPOZipAddrInfo>>();

        // 募集人情報
        protected Dictionary<CTextBox.CTextBox[], List<BPOHandlerInfo>> dicHandlerInfo = new Dictionary<CTextBox.CTextBox[], List<BPOHandlerInfo>>();
        #endregion

        // 差異がある項目数
        private Dictionary<CTextBox.CTextBox, string> dicDiffCount = new Dictionary<CTextBox.CTextBox, string>();

        private List<Control> IgnoreControls = new List<Control>();

        /// <summary>
        /// イメージズーム率 
        /// </summary>
        private double _DEFAULT_IMAGE_ZOOM_RATE { get; set; } = 0.0d;

        /// <summary>
        /// シングルエントリ？
        /// </summary>
        private bool _IsSingleEntryMode { get; set; }

        /// <summary>
        /// イメージ表示画面 
        /// </summary>
        private FrmImage _ImgForm = new FrmImage();

        //private bool bBatchCompareFlag = false;

        /// <summary>
        /// フォントサイズ
        /// </summary>
        private float _FONT_SIZE { get; set; } = 12F;

        //20171109 add begin
        Dictionary<CTextBox.CTextBox, List<OptionalProcessInfo>> dicOptionalProcessInfos = new Dictionary<CTextBox.CTextBox, List<OptionalProcessInfo>>();

        private int idxtbox = 0;

        //private int _iTextBoxTotalPosition = 0;
        //20171109 add end

        /// <summary>
        /// 
        /// </summary>
        private int _MARGIN_Y { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        private int _MARGIN_X { get; set; } = 0;

        private Dictionary<string, string> dict = null;

        //画像ファイルのImageオブジェクトを作成する
        //private Image _CurrentImage = null;

        //private int iTotalPosition = 0;

        private string _sOcrNgCheck;

        //private int _ImaegPostion_X = 0;

        //private int _ImaegPostion_Y = 0;

        //private int _BaseImaegPostion_X = 0;

        //private int _BaseImaegPostion_Y = 0;

        // 画面描画制御
        //private bool _Turbo = true;

        /// <summary>
        /// プロセッサ数
        /// </summary>
        //private int ProcessorCount = Environment.ProcessorCount;

        private bool IsMerpay = false;


        private bool IsFormShown = false;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ENTRY_UNIT_ID"></param>
        /// <param name="sRecordKbn"></param>
        /// <param name="sVerifyFlag"></param>
        /// <param name="sOcrNgCheck"></param>
        public FrmEntry(string ENTRY_UNIT_ID, string sRecordKbn, string sVerifyFlag, string sOcrNgCheck = Consts.Flag.OFF)
        {
            if (DesignMode)
                return;

            InitializeComponent();

            //Config.COSMOS_FLAG = "1";

            // DB Open
            //dao.Open(Config.DSN);

            // 帳票情報取得
            var EntryUnitId = ENTRY_UNIT_ID.Split('_');

            // 連携年月日
            this._IMAGE_CAPTURE_DATE = EntryUnitId[2];

            // 連携回数
            this._IMAGE_CAPTURE_NUM = EntryUnitId[3];

            // 帳票ID
            this._DOC_ID = EntryUnitId[4];

            // エントリユニット
            this._ENTRY_UNIT = EntryUnitId[5];

            // レコード区分
            this._RECORD_KBN = sRecordKbn;

            // エントリユニットID
            this._ENTRY_UNIT_ID = ENTRY_UNIT_ID;// String.Join("_", Config.TokuisakiCode, Config.HinmeiCode, sImageCaptureDate, sImageCaptureNum, sDocId, sEntryUnit);

            // 帳票マスタ取得
            dtM_DOC = _dao.SELECT_M_DOC(this._DOC_ID);
            this._ENTRY_METHOD = dtM_DOC.Rows[0]["ENTRY_METHOD"].ToString();
            this._DOC_ID_ENTRY = dtM_DOC.Rows[0]["DOC_ID_ENTRY"].ToString();

            if (Consts.EntryMethod.IMAGE.Equals(_ENTRY_METHOD))
            {
                this.Top = 0;
                this.Left = Screen.PrimaryScreen.WorkingArea.Width / 2;// + 5;
            }
            else
            {
                this.StartPosition = FormStartPosition.CenterScreen;
            }

            // 高さ
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;// - 2;
            // 幅
            this.Width = Screen.PrimaryScreen.WorkingArea.Width / 2;// - 5;

            this.ButtonBack.Top = this.Height - 80;
            this.ButtonExecute.Top = this.ButtonBack.Top;

            this.ButtonClose.Top = this.ButtonBack.Top;
            this.ButtonClose.Left = this.Width - 195;

            this.Text = Utils.GetFormText();
            this.lblTitle.BackColor = Config.LblTitleBackColor;

            // 検証モード？
            this._IsVerifyMode = Consts.Flag.ON.Equals(sVerifyFlag);

            this._sOcrNgCheck = sOcrNgCheck;

            //this._ImgForm._turbo = this._Turbo;

            if (Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN) && !_IsVerifyMode)
            {
                this.TextDummy.TabStop = true;
                this.TextDummy.Enabled = true;
                this.TextDummy.Visible = true;
                this.TextDummy.TabIndex = 0;
            }

            #region Merpay再審査
            if (Consts.BusinessID.MRR.Equals(Utils.GetBussinessId())
                && "10010001".Equals(this._DOC_ID))
            {
                this.IsMerpay = true;
                this.labelKansaId.Visible = true;
                this.label1.Visible = true;
                this.labelKansaId.Location = new Point(100, 90);
                this.label1.Location = new Point(16, 85);

                this.GroupBoxDummy.Visible = true;
                this.GroupBoxDummy.Location = new Point(20, 125);

                this.GroupBoxDummy.Width = 910;
            }
            #endregion
        }

        private void FrmEntry_Shown(object sender, EventArgs e)
        {

            //先頭へフォーカスセット
            if (Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN) && !_IsVerifyMode)
            {
                TextDummy.Focus();
                TextDummy.Enabled = false;
                TextDummy.Visible = false;
                // 修正入力
                for (int iIdx = 0; iIdx <= this._tbs.Length - 1; iIdx++)
                {
                    if (this._tbs[iIdx].Enabled)
                    {
                        this._tbs[iIdx].Focus();
                        break;
                    }
                }
                IsFormShown = true;
            }
            //else
            //{
            //    // 通常エントリ、検証
            //    GotoTopPosition();
            //}
        }


        /// <summary>
        /// FrmBPOEntry_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmEntry_Load(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            var swInit = System.Diagnostics.Stopwatch.StartNew();

            // 自動スクロール設定ファイルを読み込む
            if (Consts.EntryMethod.IMAGE.Equals(_ENTRY_METHOD))
            {
                this.autoScrollRegions = XmlFileLoader.CreateObject<EntryItemRegions>(String.Format(@"xml\{0}\{1}_{2}_{3}.xml"
                                                                                                   , Config.UserId
                                                                                                   , Config.TokuisakiCode
                                                                                                   , Config.HinmeiCode
                                                                                                   , _DOC_ID_ENTRY.Length == 0 ? _DOC_ID : _DOC_ID_ENTRY));
            }

            #region 帳票情報
            // 帳票情報取得
            //dtM_DOC = dao.SelectM_DOC(_DOC_ID);
            lblTitle.Text = dtM_DOC.Rows[0]["DOC_NAME"].ToString();

            // イメージズーム率
            _DEFAULT_IMAGE_ZOOM_RATE = Double.Parse(dtM_DOC.Rows[0]["DEFAULT_IMAGE_ZOOM_RATE"].ToString());

            // OCR連携フラグ
            //_OCR_COOPERATION_FLAG = dtM_DOC.Rows[0]["OCR_COOPERATION_FLAG"].ToString();

            // 自動レイアウトフラグ
            _IsAutoLayoutMode = Consts.Flag.ON.Equals(dtM_DOC.Rows[0]["AUTO_LAYOUT_FLAG"].ToString());

            // シングルエントリフラグ
            _IsSingleEntryMode = Consts.Flag.ON.Equals(dtM_DOC.Rows[0]["SINGLE_ENTRY_FLAG"].ToString());

            // バッチコンペアフラグ
            //_BATCH_COMPARE_FLAG = dtM_DOC.Rows[0]["BATCH_COMPARE_FLAG"].ToString();

            // フォントサイズ
            if (dtM_DOC.Rows[0]["FONT_SIZE"].ToString().Length != 0
                && int.Parse(dtM_DOC.Rows[0]["FONT_SIZE"].ToString()) != 0)
            {
                this._FONT_SIZE = float.Parse(dtM_DOC.Rows[0]["FONT_SIZE"].ToString());
            }

            // エントリ方法
            this._ENTRY_METHOD = dtM_DOC.Rows[0]["ENTRY_METHOD"].ToString();

            // 縦マージン
            this._MARGIN_Y = dtM_DOC.Rows[0]["MARGIN_Y"].ToString().Length == 0 ? 0 : int.Parse(dtM_DOC.Rows[0]["MARGIN_Y"].ToString());

            // 横マージン
            this._MARGIN_X = dtM_DOC.Rows[0]["MARGIN_X"].ToString().Length == 0 ? 0 : int.Parse(dtM_DOC.Rows[0]["MARGIN_X"].ToString());

            // イメージマスクスタイル
            this._ImgForm.ImageMaskStyle = dtM_DOC.Rows[0]["IMAGE_MASK_STYLE"].ToString() + "|" + (dtM_DOC.Rows[0]["IMAGE_MASK_TRANSMITTANCE"].ToString().Length == 0 ? "25" : dtM_DOC.Rows[0]["IMAGE_MASK_TRANSMITTANCE"].ToString());

            // ダミーアイテムフラグ
            try { this._DUMMY_ITEM_FLAG = Consts.Flag.ON.Equals(dtM_DOC.Rows[0]["DUMMY_ITEM_FLAG"].ToString()); } catch { }
            #endregion

            // 入力ルール設定
            this._extProps = new Dictionary<CTextBox.CTextBox, BPOEntryItemExtensionProperties>();

            // エントリ画面構築
            if (_IsAutoLayoutMode)
            {
                CreateFormAutoLayout();
            }
            else
            {
                CreateForm();
            }

            //            var xx = this._tbs.Any(tb => tb.JumpStop == true).ToArray();
            if (this._tbs.Any(tb => tb.JumpTab == true))
            {
                this._tbs[0].JumpTab = true;
            }

            // ステータス取得
            this._STATUS = _dao.SELECT_D_ENTRY_UNIT(_ENTRY_UNIT_ID).Rows[0]["STATUS"].ToString();

            if (Consts.EntryUnitStatus.EXPORT_END.Equals(this._STATUS))
            {
                this.ButtonExecute.Text = "次へ";
                for (int iIdx = 0; iIdx <= _tbs.Length - 1; iIdx++)
                    _tbs[iIdx].Enabled = false;
            }

            // テキストボックス初期化
            var iEntryItemsNum = int.Parse(dtM_DOC.Rows[0]["ENTRY_ITEMS_NUM"].ToString().Trim());

            foreach (var tb in this._tbs.AsEnumerable())
            {
                tb.Leave += new EventHandler(TextBox_Leave);
                tb.Validating += new CancelEventHandler(TextBox_Validating);
                tb.Validated += new EventHandler(TextBox_Validated);
                tb.TextChanged += new EventHandler(TextBox_TextChanged);

                if (Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN) && !_IsVerifyMode)
                {
                    // 管理者モードの場合、フォーカスインの時、再入力画面を表示する
                    tb.Enter += new EventHandler(TextBox_Enter_Admin);
                }
                else
                {
                    // 表示画像の自動スクロール
                    tb.Enter += new EventHandler(TextBox_Enter);
                }
            }

            //foreach (var tb in this._btns.AsEnumerable())
            //{
            //    if (tb != null)
            //        tb.Click += new EventHandler(Button_Click);
            //}

            IgnoreControls = new Control[] { ButtonClose, ButtonBack/*, chkAutoScroll*/ }.ToList();

            if (Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN) && !_IsVerifyMode)
            {
                dtD_IMAGE_INFO = _dao.SELECT_D_IMAGE_INFO(_ENTRY_UNIT_ID, true);
            }
            else
            {
                dtD_IMAGE_INFO = _dao.SELECT_D_IMAGE_INFO(_ENTRY_UNIT_ID);
            }

            // 入力対象IMAGE_SEQ
            foreach (var drD_IMAGE_INFO in dtD_IMAGE_INFO.AsEnumerable())
            {
                _log.Debug(String.Format("入力対象 IMAGE_SEQ:{0}", drD_IMAGE_INFO["IMAGE_SEQ"].ToString().PadLeft(4)));
            }

            // トータル件数取得
            _TotalCount = dtD_IMAGE_INFO.Rows.Count;
            if (_TotalCount == 0)
            {
                this.Hide();
                MessageBox.Show("エントリ対象データがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                this.Dispose();
                return;
            }

            // ?件目処理
            _ProcessingCount++;
            //if (Consts.Flag.OFF.Equals(_IsVerifyMode))
            if (!_IsVerifyMode)
            {
                if (Consts.RecordKbn.Entry_1st.Equals(this._RECORD_KBN))
                {
                    _ProcessingCount += dtD_IMAGE_INFO.Select(String.Format("RECORD_KBN_1_ENTRY_FLAG='{0}'", Consts.Flag.ON)).Count();
                }
                else if (Consts.RecordKbn.Entry_2nd.Equals(this._RECORD_KBN))
                {
                    _ProcessingCount += dtD_IMAGE_INFO.Select(String.Format("RECORD_KBN_2_ENTRY_FLAG='{0}'", Consts.Flag.ON)).Count();
                }
                else if (Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN))
                {
                    _ProcessingCount += dtD_IMAGE_INFO.Select(String.Format("RECORD_KBN_0_ENTRY_FLAG='{0}'", Consts.Flag.ON)).Count();
                }
                else
                {
                    throw new ApplicationException("想定外のレコードレコード区分で呼び出されました。");
                }
            }

            // 最後まで登録済の場合、１件目を表示
            if (_ProcessingCount == (_TotalCount + 1))
            {
                _ProcessingCount = 1;
            }

            // イメージフォーム表示
            if (Consts.EntryMethod.IMAGE.Equals(_ENTRY_METHOD))
            {
                if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
                {
                    this._ImgForm.Show(this);
                }
                else
                {
                    this._ImgForm.Show();
                }
            }

            // 次を表示
            ShowNext();

            swInit.Stop();
            _log.Debug(String.Format($"初期表示終了:[経過時間:{swInit.Elapsed}]"));

        }

        protected void Button_Click(object sender, EventArgs e)
        {
            //_tbs[11].Text = string.Empty;
            //_tbs[12].Text = string.Empty;
            //_tbs[13].Text = string.Empty;
            //_tbs[10].Focus();
        }

        /// <summary>
        /// ダイアログ キーを処理します。
        /// </summary>
        /// <param name="keyData">処理するキーを表す Keys 値の 1 つ。</param>
        /// <returns>キーがコントロールによって処理された場合は true。それ以外の場合は false。</returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            // Enterキー押下によるフォーカス移動
            if (((keyData & Keys.KeyCode) == Keys.Return) && ((keyData & (Keys.Alt | Keys.Control)) == Keys.None))
            {
                if (!(this.ActiveControl is Button))
                {
                    NextControl();
                    return true;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// 次のコントロールをアクティブにします。
        /// </summary>
        /// <param name="forward">タブオーダー内を前方に移動する場合はtrue。後方に移動する場合はfalse。</param>
        protected void NextControl(bool forward = true)
        {
            if (forward)
            {
                SendKeys.Send("{TAB}");
            }
            else
            {
                SendKeys.Send("+{TAB}");
            }
        }

        /// <summary>
        /// 登録・次へ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ButtonExecute_Click(object sender, EventArgs e)
        {
            if (IsMerpay)
            {
                if (this.RadioButtonYes.Checked)
                {
                    this._tbs[0].Text = "1";
                }
                else
                {
                    this._tbs[0].Text = "2";
                }
            }

            System.Diagnostics.Stopwatch swTotal;
            System.Diagnostics.Stopwatch swCheck;
            System.Diagnostics.Stopwatch swUpdate;

            swTotal = System.Diagnostics.Stopwatch.StartNew();
            swCheck = System.Diagnostics.Stopwatch.StartNew();

            // エントリ更新
            var entryList = new List<string[]>();
            //List<string> entryListBefore = new List<string>();
            //List<string> UpdateTargetColumn = new List<string>();
            //List<string> DummyItemFlagList = new List<string>();

            //            int loopCount = EntryItems.Count;
            //int loopCount = this._tbs.Length;
            for (int i = 1; i <= this._tbs.Length; i++)
            {
                if (this._tbs[i - 1].Enabled)
                {
                    #region 必須入力
                    if (this._tbs[i - 1].Text.Length == 0 && this._tbs[i - 1].IsRequired)
                    {
                        MessageBox.Show($"「{this._tbs[i - 1].ItemName}」は入力必須です。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this._tbs[i - 1].Focus();
                        return;
                    }
                    #endregion

                    #region MaxLength
                    if (this._tbs[i - 1].Text.Length > this._tbs[i - 1].MaxLength)
                    {
                        MessageBox.Show($"「{this._tbs[i - 1].ItemName}」の最大入力桁数を超えています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this._tbs[i - 1].Focus();
                        return;
                    }
                    #endregion

                    #region 条件付き必須
                    if (this._tbs[i - 1].Text.Length == 0 && this._tbs[i - 1].Conditional_Required_Item.Length != 0)
                    {
                        for (int iIdx = 0; iIdx <= _tbs.Length - 1; iIdx++)
                        {
                            if (_tbs[iIdx].ItemName.Equals(this._tbs[i - 1].Conditional_Required_Item))
                            {
                                if (this._tbs[i - 1].Conditional_Required_Value.Contains(_tbs[iIdx].Text)
                                    && this._tbs[i - 1].Conditional_Required_Value[0].Length != 0)
                                {
                                    MessageBox.Show($"「{this._tbs[i - 1].Conditional_Required_Item}」に「{String.Join(",", this._tbs[i - 1].Conditional_Required_Value)}」を入力した場合\n「{this._tbs[i - 1].ItemName}」は入力必須です。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    this._tbs[i - 1].Focus();
                                    return;
                                }
                                else if ("ANYTHING".Equals(this._tbs[i - 1].Conditional_Required_Value[0].ToUpper()) && _tbs[iIdx].Text.Length != 0)
                                {
                                    MessageBox.Show($"「{this._tbs[i - 1].Conditional_Required_Item}」に入力がある場合、\n「{this._tbs[i - 1].ItemName}」は入力必須です。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    this._tbs[i - 1].Focus();
                                    return;
                                }
                                else if (!this._tbs[i - 1].Conditional_Required_Omit_Value.Contains(_tbs[iIdx].Text) && this._tbs[i - 1].Conditional_Required_Omit_Value[0].Length != 0)
                                {
                                    MessageBox.Show($"「{this._tbs[i - 1].Conditional_Required_Item}」が「{ String.Join(",", this._tbs[i - 1].Conditional_Required_Omit_Value[0])}」以外の場合\n「{ this._tbs[i - 1].ItemName}」は入力必須です。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    this._tbs[i - 1].Focus();
                                    return;
                                }
                            }
                        }
                    }
                    #endregion

                    #region 最小桁数
                    //if (this._tbs[i - 1].MinLength != -1 && (this._tbs[i - 1].Text.Length < this._tbs[i - 1].MinLength))
                    //{
                    //    MessageBox.Show($"「{this._tbs[i - 1].ItemName}」は{this._tbs[i - 1].MinLength}文字以上で入力して下さい。 ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //    this._tbs[i - 1].Focus();
                    //    return;
                    //}
                    #endregion

                    #region 個人番号
                    if (this._tbs[i - 1].MyNumber1.Length != 0 && this._tbs[i - 1].MyNumber2.Length != 0)
                    {
                        string sMyNumber = string.Empty;
                        foreach (var tb in _tbs)
                            if (tb.ItemName.Equals(this._tbs[i - 1].MyNumber1) || tb.ItemName.Equals(this._tbs[i - 1].MyNumber2))
                                sMyNumber += tb.Text;
                        sMyNumber += this._tbs[i - 1].Text;
                        if (sMyNumber.Contains(Config.ReadNotCharNarrowInput))
                        {
                            MessageBox.Show("個人番号に判読不可文字が入力されています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else if (!Utils.IsValidMyNumber(sMyNumber))
                        {
                            MessageBox.Show("個人番号として不正な値が入力されています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            this._tbs[i - 1].Focus();
                            return;
                        }
                    }
                    #endregion

                    #region マスタ存在チェック
                    if (this._tbs[i - 1].MasterCheck)
                    {
                        if (!this._tbs[i - 1].Text.Contains(Config.ReadNotCharNarrowInput)
                            && this._tbs[i - 1].Text.Length != 0)
                        {
                            if (!_dao.SELECT_M_ITEM_CHECK(_DOC_ID, this._tbs[i - 1].ItemName, _tbs[i - 1].Text))
                            {
                                MessageBox.Show($"「{this._tbs[i - 1].ItemName}」に「{this._tbs[i - 1].Text}」は入力出来ません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                this._tbs[i - 1].Focus();
                                return;
                            }
                        }
                    }
                    #endregion

                    #region 範囲チェック
                    if (this._tbs[i - 1].Range.Length != 0 && this._tbs[i - 1].Text.Length != 0)
                    {
                        string[] sRange = this._tbs[i - 1].Range.Split(',');
                        if (decimal.Parse(this._tbs[i - 1].Text) < decimal.Parse(sRange[0])
                            || decimal.Parse(this._tbs[i - 1].Text) > decimal.Parse(sRange[1]))
                        {
                            MessageBox.Show($"「{this._tbs[i - 1].ItemName}」に「{sRange[0]}～{sRange[1]}」以外の値が入力されています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            this._tbs[i - 1].Focus();
                            return;
                        }
                    }
                    //}
                    #endregion

                    #region 正規表現
                    if (this._tbs[i - 1].ValidPattern.Length != 0 && this._tbs[i - 1].Text.Length != 0)
                    {
                        // 判別不能文字を除いてチェックする
                        string omitedText = this._tbs[i - 1].Text.Replace(Config.ReadNotCharNarrowInput, string.Empty).Replace(Config.ReadNotCharWideInput, string.Empty);
                        if (!Regex.IsMatch(omitedText, this._tbs[i - 1].ValidPattern))
                        {
                            MessageBox.Show($"「{this._tbs[i - 1].ItemName}」に「{this._tbs[i - 1].Text}」は入力出来ません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            this._tbs[i - 1].Focus();
                            return;
                        }
                    }
                    #endregion

                    #region フル桁入力
                    if (this._tbs[i - 1].FullLength)
                    {
                        if (this._tbs[i - 1].Text.Length != 0 && this._tbs[i - 1].Text.Length != this._tbs[i - 1].MaxLength/* && this._tbs[i - 1].Text != Config.ReadNotCharNarrowInput && this._tbs[i - 1].Text != Config.ReadNotCharWideInput*/)
                        {
                            MessageBox.Show($"「{this._tbs[i - 1].ItemName}」は{this._tbs[i - 1].MaxLength}文字で入力してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            this._tbs[i - 1].Focus();
                            return;
                        }
                    }
                    #endregion

                    #region 禁則文字
                    if (Consts.InputMode.Hiragana.Equals(this._tbs[i - 1].InputMode)
                        || Consts.InputMode.Full.Equals(this._tbs[i - 1].InputMode)
                        || Consts.InputMode.MixFull.Equals(this._tbs[i - 1].InputMode))
                    {
                        // 入力可能全角文字チェック
                        //if (!Utils.IsAllowMultiByteChar(textBox.Text))
                        //{
                        //    MessageBox.Show(String.Format("「{0}」に入力禁止文字が含まれています。", textBox.ItemName), this.lblTitle.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //    e.Cancel = true;
                        //    return;
                        //}
                        //bool b = true;
                        //int iIdx = 0;
                        for (int iIdx = 0; iIdx <= this._tbs[i - 1].Text.Length - 1; iIdx++)
                        {
                            //                            b = ;
                            if (!Utils.IsValidChar(this._tbs[i - 1].InputMode, this._tbs[i - 1].Text.Substring(iIdx, 1)))
                            {
                                MessageBox.Show($"「{ this._tbs[i - 1].ItemName}」に入力禁止文字が含まれています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                this._tbs[i - 1].Focus();
                                this._tbs[i - 1].Select(iIdx, 1);
                                return;
                                //break;
                            }
                        }

                        //if (!b)
                        //{
                        //}
                    }
                    #endregion

                    #region 半角項目に全角入力とその逆
                    if (this._tbs[i - 1].Text.Length != 0)
                    {
                        if (this._tbs[i - 1].InputMode == Consts.InputMode.AllHalf
                            || this._tbs[i - 1].InputMode == Consts.InputMode.AlphabetNumeric
                            || this._tbs[i - 1].InputMode == Consts.InputMode.KanaHalf)
                        {
                            for (int iIdx = 0; iIdx <= this._tbs[i - 1].Text.Length - 1; iIdx++)
                            {
                                if (!Utils.IsSingleByteChar(this._tbs[i - 1].Text.Substring(iIdx, 1)))
                                {
                                    MessageBox.Show($"「{this._tbs[i - 1].ItemName}」に全角文字は入力出来ません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    this._tbs[i - 1].Focus();
                                    this._tbs[i - 1].Select(iIdx, 1);
                                    return;
                                }
                            }
                        }
                        if (this._tbs[i - 1].InputMode == Consts.InputMode.Full
                            || this._tbs[i - 1].InputMode == Consts.InputMode.Hiragana
                            || this._tbs[i - 1].InputMode == Consts.InputMode.KanaFull)
                        {
                            for (int iIdx = 0; iIdx <= this._tbs[i - 1].Text.Length - 1; iIdx++)
                            {
                                if (Utils.IsSingleByteChar(this._tbs[i - 1].Text.Substring(iIdx, 1)))
                                {
                                    MessageBox.Show($"「{this._tbs[i - 1].ItemName}」に半角文字は入力出来ません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    this._tbs[i - 1].Focus();
                                    this._tbs[i - 1].Select(iIdx, 1);
                                    return;
                                }
                            }
                        }
                    }
                    #endregion

                    #region AI-OCR対応
                    //#region 正規表現
                    //if (this._tbs[i - 1].ValidPattern.Length != 0
                    //    && this._tbs[i - 1].Text.Length != 0)
                    //{
                    //    // 判別不能文字を除いてチェックする
                    //    var omitedText = this._tbs[i - 1].Text.Replace(Config.ReadNotCharNarrowInput, string.Empty).Replace(Config.ReadNotCharWideInput, string.Empty);
                    //    if (!Regex.IsMatch(omitedText, this._tbs[i - 1].ValidPattern))
                    //    {
                    //        MessageBox.Show(String.Format("「{0}」に「{1}」は入力出来ません。", this._tbs[i - 1].ItemName, this._tbs[i - 1].Text), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //        this._tbs[i - 1].Focus();
                    //        return;
                    //    }
                    //}
                    //#endregion

                    #region 最大文字数
                    if (this._tbs[i - 1].Text.Length > this._tbs[i - 1].MaxLength)
                    {
                        MessageBox.Show($"「{ this._tbs[i - 1].ItemName}」の最大文字数は{this._tbs[i - 1].MaxLength}文字です。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this._tbs[i - 1].Focus();
                        return;
                    }
                    #endregion
                    #endregion

                    #region DR
                    if (this._tbs[i - 1].DR.Length != 0 && !Utils.IsValidDr(this._tbs[i - 1].Text, this._tbs[i - 1].DR))
                    {
                        MessageBox.Show($"{this._tbs[i - 1].DR}DRチェックデジットエラー。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this._tbs[i - 1].Focus();
                        return;
                    }
                    //}
                    #endregion

                    #region MailAddress
                    if (this._tbs[i - 1].IsMailAddress && this._tbs[i - 1].Text.Length != 0)
                    {
                        if (!Utils.IsValidMailAddress(this._tbs[i - 1].Text))
                        {
                            MessageBox.Show($"無効な書式で入力されています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            this._tbs[i - 1].Focus();
                            return;
                        }
                    }
                    #endregion

                    #region Date
                    if (this._tbs[i - 1].Text.Length != 0 && this._tbs[i - 1].DateFormat.Length != 0)
                    {
                        if (!Utils.IsValidDate(this._tbs[i - 1].Text, this._tbs[i - 1].DateFormat))
                        {
                            MessageBox.Show($"無効な書式で入力されています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            this._tbs[i - 1].Focus();
                            return;
                        }
                    }
                    #endregion
                }

                var param = new string[] { $"ITEM_{i:d3}"
                                         , this._tbs[i - 1].Text.Trim()
                                         , this._tbs[i - 1].Tag.ToString()
                                         , this._tbs[i - 1].IsDummyItem ? Consts.Flag.ON : Consts.Flag.OFF
                                         , this._tbs[i - 1].DummyItemFlag

                };
                /*
                                if (this._tbs[i - 1].IsDummyItem)
                                {
                                    entryList.Add(string.Empty);
                                    DummyItemFlagList.Add(Consts.Flag.ON);
                                }
                                else
                                {
                                    entryList.Add(this._tbs[i - 1].Text);
                                    DummyItemFlagList.Add(Consts.Flag.OFF);
                                }

                                entryListBefore.Add(this._tbs[i - 1].Tag.ToString());
                                if (this._tbs[i - 1].Enabled)
                                    UpdateTargetColumn.Add(Consts.Flag.ON);
                                else
                                    UpdateTargetColumn.Add(Consts.Flag.OFF);
                */
                entryList.Add(param);
            }

            #region 第一フロンティア生命　解約請求　固有処理
            if (this._RECORD_KBN != Consts.RecordKbn.ADMIN)
            {
                if ("408107".Equals(Config.TokuisakiCode)
                    && "013".Equals(Config.HinmeiCode))
                {
                    // その他の金融機関
                    // 金融機関名で存在チェック
                    //if ("TEXT005".Equals(tb.Name.ToUpper()))
                    //{
                    if (_tbs[4].Text.Length != 0)
                    {
                        if (!_tbs[4].Text.Contains(Config.ReadNotCharWideInput)
                            && !_tbs[4].Text.Contains("＃"))
                        {
                            var dt408107 = _dao.SELECT_M_FINANCIAL_INSTITUTION(_tbs[4].Text);
                            if (dt408107.Rows.Count == 0)
                            {
                                MessageBox.Show("入力された金融機関名は金融機関マスタに存在しません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                _tbs[4].Focus();
                                return;
                            }
                        }
                    }
                    //}

                    // 金融機関名＋支店名で存在チェック　
                    //if ("TEXT011".Equals(tb.Name.ToUpper()))
                    //{
                    if (_tbs[4].Text.Length != 0
                        && _tbs[5].Text.Length != 0
                        && _tbs[6].Text.Length != 0)
                    {
                        if (!_tbs[4].Text.Contains(Config.ReadNotCharWideInput)
                            && !_tbs[4].Text.Contains("＃")
                            && !_tbs[5].Text.Contains(Config.ReadNotCharNarrowInput)
                            && !_tbs[5].Text.Contains("#")
                            && !_tbs[6].Text.Contains(Config.ReadNotCharWideInput)
                            && !_tbs[6].Text.Contains("＃"))
                        {
                            var i1 = 0;
                            var addName = string.Empty;
                            // 支店
                            if (Consts.Flag.ON.Equals(_tbs[7].Text))
                            {
                                i1++;
                            }
                            // 営業部
                            if (Consts.Flag.ON.Equals(_tbs[8].Text))
                            {
                                addName = "営業部";
                                i1++;
                            }
                            // 出張所
                            if (Consts.Flag.ON.Equals(_tbs[9].Text))
                            {
                                addName = "出張所";
                                i1++;
                            }
                            // 支所
                            if (Consts.Flag.ON.Equals(_tbs[10].Text))
                            {
                                i1++;
                            }
                            // 複数選択
                            if (i1 != 1)
                            {
                                addName = string.Empty;
                            }
                            var dt408107 = _dao.SELECT_M_FINANCIAL_INSTITUTION(_tbs[4].Text, _tbs[6].Text + addName);
                            if (dt408107.Rows.Count == 0)
                            {
                                MessageBox.Show("入力された金融機関名、支店名は金融機関マスタに存在しません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                //_tbs[6].Focus();  // 20190828 削除
                                //return;           // 20190828 削除
                            }
                            else
                            {
                                //if (!_tbs[5].Text.Contains(Config.ReadNotCharNarrowInput)
                                //    && !_tbs[5].Text.Contains("#"))
                                //{
                                if (!dt408107.Rows[0]["BRANCH_CD"].ToString().Equals(_tbs[5].Text))
                                {
                                    MessageBox.Show("金融機関名、店番号、支店名の整合性が取れていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    //_tbs[6].Focus();
                                    //return;
                                }
                                //}
                            }
                        }
                    }
                    //}

                    // 通常金融機関
                    // 金融機関名で存在チェック
                    //if ("TEXT021".Equals(tb.Name.ToUpper()))
                    //{
                    if (_tbs[15].Text.Length != 0)
                    {
                        if (!_tbs[15].Text.Contains(Config.ReadNotCharWideInput)
                            && !_tbs[15].Text.Contains("＃"))
                        {
                            var i1 = 0;
                            var addName = string.Empty;
                            // 銀行
                            if (Consts.Flag.ON.Equals(_tbs[16].Text))
                            {
                                i1++;
                            }
                            // 信用金庫
                            if (Consts.Flag.ON.Equals(_tbs[17].Text))
                            {
                                addName = "信金";
                                i1++;
                            }
                            // 信用組合
                            if (Consts.Flag.ON.Equals(_tbs[18].Text))
                            {
                                addName = "信組";
                                i1++;
                            }
                            // 農協
                            if (Consts.Flag.ON.Equals(_tbs[19].Text))
                            {
                                addName = "農協";
                                i1++;
                            }
                            // 労働金庫
                            if (Consts.Flag.ON.Equals(_tbs[20].Text))
                            {
                                addName = "労金";
                                i1++;
                            }
                            // 複数選択
                            if (i1 != 1)
                            {
                                addName = string.Empty;
                            }
                            var dt408107 = _dao.SELECT_M_FINANCIAL_INSTITUTION(_tbs[15].Text + addName);
                            if (dt408107.Rows.Count == 0)
                            {
                                MessageBox.Show("入力された金融機関名は金融機関マスタに存在しません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                //e.Cancel = true;
                                _tbs[15].Focus();
                                return;
                            }
                        }
                    }
                    //}

                    // 金融機関名＋支店名で存在チェック　
                    if (_tbs[15].Text.Length != 0
                        && _tbs[21].Text.Length != 0
                        && _tbs[22].Text.Length != 0)
                    {
                        if (!_tbs[15].Text.Contains(Config.ReadNotCharWideInput)
                            && !_tbs[15].Text.Contains("＃")
                            && !_tbs[21].Text.Contains(Config.ReadNotCharNarrowInput)
                            && !_tbs[21].Text.Contains("#")
                            && !_tbs[22].Text.Contains(Config.ReadNotCharWideInput)
                            && !_tbs[22].Text.Contains("＃"))
                        {
                            var i1 = 0;
                            var addName = string.Empty;
                            // 銀行
                            if (Consts.Flag.ON.Equals(_tbs[16].Text))
                            {
                                i1++;
                            }
                            // 信用金庫
                            if (Consts.Flag.ON.Equals(_tbs[17].Text))
                            {
                                addName = "信金";
                                i1++;
                            }
                            // 信用組合
                            if (Consts.Flag.ON.Equals(_tbs[18].Text))
                            {
                                addName = "信組";
                                i1++;
                            }
                            // 農協
                            if (Consts.Flag.ON.Equals(_tbs[19].Text))
                            {
                                addName = "農協";
                                i1++;
                            }
                            // 労働金庫
                            if (Consts.Flag.ON.Equals(_tbs[20].Text))
                            {
                                addName = "労金";
                                i1++;
                            }
                            // 複数選択
                            if (i1 != 1)
                            {
                                addName = string.Empty;
                            }
                            var i2 = 0;
                            var addName2 = string.Empty;
                            // 支店
                            if (Consts.Flag.ON.Equals(_tbs[23].Text))
                            {
                                i2++;
                            }
                            // 営業部
                            if (Consts.Flag.ON.Equals(_tbs[24].Text))
                            {
                                addName2 = "営業部";
                                i2++;
                            }
                            // 出張所
                            if (Consts.Flag.ON.Equals(_tbs[25].Text))
                            {
                                addName2 = "出張所";
                                i2++;
                            }
                            // 支所
                            if (Consts.Flag.ON.Equals(_tbs[26].Text))
                            {
                                i2++;
                            }
                            // 複数選択
                            if (i2 != 1)
                            {
                                addName2 = string.Empty;
                            }
                            var dt408107 = _dao.SELECT_M_FINANCIAL_INSTITUTION(_tbs[15].Text + addName, _tbs[22].Text + addName2);
                            if (dt408107.Rows.Count == 0)
                            {
                                MessageBox.Show("入力された金融機関名、支店名は金融機関マスタに存在しません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                //_tbs[22].Focus();     // 20190828 削除
                                //return;               // 20190828 削除
                            }
                            else
                            {
                                //if (!_tbs[21].Text.Contains(Config.ReadNotCharNarrowInput)
                                //    && !_tbs[21].Text.Contains("#")
                                //    )
                                //{
                                if (!dt408107.Rows[0]["BRANCH_CD"].ToString().Equals(_tbs[21].Text))
                                {
                                    MessageBox.Show("金融機関名、店番号、支店名の整合性が取れていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    //_tbs[22].Focus();
                                    //return;
                                }
                                //}
                            }
                        }
                    }
                }
            }
            #endregion

            swCheck.Stop();
            _log.Debug($"①入力項目チェック終了:[経過時間:{swCheck.Elapsed}]");

            swUpdate = System.Diagnostics.Stopwatch.StartNew();

            if (Consts.EntryUnitStatus.EXPORT_END.Equals(this._STATUS))
            {
                // テキスト出力済の場合、更新しない
                if (_ProcessingCount == _TotalCount)
                {
                    MessageBox.Show($"{Utils.EditEntryBatchId(_ENTRY_UNIT_ID)} の検証が完了しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    EndProc();
                    return;
                }
                // 処理中件数Up
                _ProcessingCount++;

                // 次レコード
                ShowNext();
            }
            else
            {
                // 全項目未入力をチェックする
                if (Consts.Flag.ON.Equals(Config.NoInputMessageFlag))
                {
                    int iInputCount = 0;
                    for (int iIdx = 0; iIdx <= _tbs.Length - 1; iIdx++)
                    {
                        if (Consts.RecordKbn.ADMIN.Equals(_RECORD_KBN))
                        {
                            //if ("1".Equals(_IsVerifyMode))
                            if (_IsVerifyMode)
                            {
                                if (_tbs[iIdx].Enabled && _tbs[iIdx].Text.Length != 0)
                                {
                                    iInputCount++;
                                }
                            }
                            else
                            {
                                if (_tbs[iIdx].Text.Length != 0)
                                {
                                    iInputCount++;
                                }
                            }
                        }
                        else
                        {
                            if (_tbs[iIdx].Enabled && _tbs[iIdx].Text.Length != 0)
                            {
                                iInputCount++;
                            }
                        }
                    }
                    // 全項目未入力
                    if (iInputCount == 0)
                    {
                        if (MessageBox.Show("全ての項目が未入力です。\nこのまま登録しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.Yes)
                        {
                            // 先頭項目へ戻る
                            GotoTopPosition();
                            return;
                        }
                    }
                }

                // エントリ単位内最終レコード
                if (_ProcessingCount == _TotalCount)
                {
                    if (MessageBox.Show($"{Utils.EditEntryBatchId(_ENTRY_UNIT_ID)} の最終レコードです。\nこのまま登録しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.Yes)
                    {
                        // 先頭項目へ戻る
                        _tbs[idxtbox].Focus();
                        //GotoTopPosition();
                        return;
                    }
                }

                // DB更新
                // 出力済ステータスの場合、更新しない。
                if (!Consts.EntryUnitStatus.EXPORT_END.Equals(_STATUS))
                {
                    if (!UpdateEntry(entryList/*, entryListBefore, UpdateTargetColumn, DummyItemFlagList*/)) { return; }
                }

                var sLogMessage = $"ENTRY_UNIT_ID:{_ENTRY_UNIT_ID} IMAGE_SEQ:{_IMAGE_SEQ} RECORD_KBN:{_RECORD_KBN} 検証フラグ:{_IsVerifyMode}";
                _log.Info(sLogMessage);

                //log.Debug(String.Format("次レコード表示開始"));
                //sw.Start();

                swUpdate.Stop();
                _log.Debug($"②DB更新終了:[経過時間:{swUpdate.Elapsed}]");

                if (_ProcessingCount == _TotalCount)
                {
                    swTotal.Stop();
                    _log.Debug($"③登録処理終了:[経過時間:{swTotal.Elapsed}]");

                    MessageBox.Show($"{Utils.EditEntryBatchId(_ENTRY_UNIT_ID)} の登録が完了しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    EndProc();
                    this.DialogResult = DialogResult.OK;
                    return;
                }

                // 処理中件数Up
                _ProcessingCount++;

                // 次レコード
                ShowNext();

                swTotal.Stop();
                _log.Debug($"③登録処理終了:[経過時間:{swTotal.Elapsed}]");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entryList"></param>
        /// <returns></returns>
        private bool UpdateEntry(List<string[]> entryList/*, List<string> entryListBefore, List<string> UpdateTargetColumn, List<string> DummyItemFlagList*/)
        {
            try
            {
                // トランザクション開始
                _dao.BeginTrans();

                var UpdateCount = 0;
                // 管理者モードの場合の更新
                if (Consts.RecordKbn.ADMIN.Equals(_RECORD_KBN) && !_IsVerifyMode)
                {
                    UpdateCount = _dao.UPDATE_D_ENTRY(_ENTRY_UNIT_ID, _IMAGE_SEQ, _RECORD_KBN, entryList);
                    _log.Debug($"D_ENTRY 更新件数:{UpdateCount}");

                    // D_IMAGE_INFO 更新
                    if (_dao.UPDATE_D_IMAGE_INFO(_ENTRY_UNIT_ID, _IMAGE_SEQ, _RECORD_KBN) != 1)
                    {
                        throw new ApplicationException(String.Format("D_IMAGE_INFO の更新で不整合が発生"));
                    }

                    // すべて登録完了後、入力単位データの状態をコンペア済にする
                    if (_ProcessingCount == _TotalCount)
                    {
                        if (_dao.UPDATE_D_ENTRY_UNIT(_ENTRY_UNIT_ID, Consts.EntryUnitStatus.COMPARE_END) != 1)
                        {
                            throw new ApplicationException(String.Format("D_ENTRY_UNIT の更新で不整合が発生"));
                        }
                    }
                }
                else
                {
                    // D_ENTRY 更新
                    UpdateCount = _dao.UPDATE_D_ENTRY(_ENTRY_UNIT_ID, _IMAGE_SEQ, _RECORD_KBN, entryList);
                    _log.Debug($"D_ENTRY 更新件数:{UpdateCount}");

                    //if (Consts.Flag.OFF.Equals(_IsVerifyMode))
                    if (!_IsVerifyMode)
                    {
                        // D_IMAGE_INFO 更新
                        if (_dao.UPDATE_D_IMAGE_INFO(_ENTRY_UNIT_ID, _IMAGE_SEQ, _RECORD_KBN) != 1)
                        {
                            throw new ApplicationException(String.Format("D_IMAGE_INFO の更新で不整合が発生"));
                        }

                        // シングル入力対応（区分１の入力内容で区分２のレコードを更新する）
                        if (this._IsSingleEntryMode
                            && Consts.RecordKbn.Entry_1st.Equals(this._RECORD_KBN))
                        {
                            UpdateCount = _dao.UPDATE_D_ENTRY(_ENTRY_UNIT_ID, _IMAGE_SEQ, Consts.RecordKbn.Entry_2nd, entryList);
                            _log.Debug($"D_ENTRY 更新件数:{UpdateCount}");
                        }

                        // ユーザの入力が完了したかどうかを判断する
                        if (_ProcessingCount == _TotalCount)
                        {
                            if (_dao.UPDATE_D_ENTRY_STATUS(_ENTRY_UNIT_ID, _RECORD_KBN, Consts.EntryStatus.ENTRY_END) != 1)
                            {
                                throw new ApplicationException(String.Format("D_ENTRY_STATUS の更新で不整合が発生"));
                            }

                            // シングル入力対応（区分１の入力内容で区分２のレコードを更新する）
                            if (this._IsSingleEntryMode
                                && Consts.RecordKbn.Entry_1st.Equals(this._RECORD_KBN))
                            {
                                if (_dao.UPDATE_D_ENTRY_STATUS(_ENTRY_UNIT_ID, Consts.RecordKbn.Entry_2nd, Consts.EntryStatus.ENTRY_END) != 1)
                                {
                                    throw new ApplicationException(String.Format("D_ENTRY_STATUS の更新で不整合が発生"));
                                }
                            }

                            // コンペア
                            CompareEntryUnit();
                        }
                    }
                    else
                    {
                        if (_ProcessingCount == _TotalCount)
                        {
                            // D_ENTRY_UNIT 検証中フラグ→"0"
                            if (_dao.UPDATE_D_ENTRY_UNIT_VERIFY_ING_FLAG(_ENTRY_UNIT_ID) != 1)
                            {
                                throw new ApplicationException(String.Format("D_ENTRY_UNIT の更新で不整合が発生"));
                            }
                        }
                    }

                    // OCR_NG_FLAG解除
                    if (Consts.Flag.ON.Equals(_sOcrNgCheck))
                    {
                        if (_dao.UPDATE_D_IMAGE_INFO_OCR_NG_STATUS(_ENTRY_UNIT_ID, _IMAGE_SEQ) != 1)
                        {
                            throw new ApplicationException(String.Format("D_IMAGE_INFO の更新で不整合が発生"));
                        }
                    }
                }

                // コミット
                _dao.CommitTrans();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("登録時に例外が発生しました。" + "\n" + ex.Message + "\n" + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                _dao.RollbackTrans();
                return false;
            }
        }

        /// <summary>
        /// コンペア
        /// </summary>
        private void CompareEntryUnit()
        {
            // 対象エントリユニットをロック
            _log.Debug("排他開始:{0}", _ENTRY_UNIT_ID);
            var dtLock = _dao.SELECT_D_ENTRY_UNIT(_ENTRY_UNIT_ID, true);
            _log.Debug("排他完了:{0}", _ENTRY_UNIT_ID);

            //int registResult = -1;
            var dtEntryStatus = _dao.SELECT_D_ENTRY_STATUS(_ENTRY_UNIT_ID, Consts.RecordKbn.Entry_1st.Equals(_RECORD_KBN) ? Consts.RecordKbn.Entry_2nd : Consts.RecordKbn.Entry_1st);

            // ユーザ区分が1の入力状態を取得する
            //var row_inp1 = dtEntryStatus.Select("RECORD_KBN = '" + Consts.RecordKbn.Entry_1st + "'");
            var status_inp1 = dtEntryStatus.Rows[0]["ENTRY_STATUS"].ToString();
            //// ユーザ区分が1の入力状態を取得する
            //var row_inp2 = dtEntryStatus.Select("RECORD_KBN = '" + Consts.RecordKbn.Entry_2nd + "'");
            //var status_inp2 = row_inp2[0]["ENTRY_STATUS"].ToString().Trim();

            // １人目、２人目分の入力が両方入力済になっていれば、入力単位データの状態区分をコンペア中("5")へ更新
            if (Consts.EntryStatus.ENTRY_END.Equals(status_inp1))
            //&& Consts.EntryStatus.ENTRY_END.Equals(status_inp2))
            {
                //registResult = dao.UPDATE_D_ENTRY_UNIT(_ENTRY_UNIT_ID, Consts.EntryUnitStatus.COMPARE_ING);
                if (_dao.UPDATE_D_ENTRY_UNIT(_ENTRY_UNIT_ID, Consts.EntryUnitStatus.COMPARE_ING) != 1)
                {
                    throw new ApplicationException(String.Format("D_ENTRY_UNIT の更新で不整合が発生"));
                }

                if (!Consts.Flag.ON.Equals(this.dtM_DOC.Rows[0]["BATCH_COMPARE_FLAG"].ToString()))
                {
                    // コンペア処理を行う
                    //bool diff_flg = DoCompare();
                    _log.Debug(String.Format("コンペア開始:{0}", _ENTRY_UNIT_ID));
                    DllCompareEntryUnit.DllCompareEntryUnit.BL_Main(_ENTRY_UNIT_ID, Program.LoginUser.USER_ID);
                    _log.Debug(String.Format("コンペア終了:{0}", _ENTRY_UNIT_ID));

                    //// 差異があれば入力単位データの状態区分を管理者修正中("6")へ更新
                    //// 差異がなければ入力単位データの状態区分をコンペア済("7")へ更新
                    //string unitStatus = string.Empty;
                    //if (diff_flg)
                    //    unitStatus = Consts.EntryUnitStatus.COMPARE_EDT;
                    //else
                    //    unitStatus = Consts.EntryUnitStatus.COMPARE_END;
                    //registResult = dao.UpdateD_ENTRY_UNIT(_DOC_ID, _IMAGE_CAPTURE_DATE, _IMAGE_CAPTURE_NUM, _ENTRY_UNIT, unitStatus);
                }
            }
            //return registResult;
        }
        /*
                /// <summary>
                /// コンペア
                /// </summary>
                /// <returns></returns>
                private bool DoCompare()
                {
                    bool diff_flg = false;

                    var swCompare = System.Diagnostics.Stopwatch.StartNew();

                    foreach (DataRow row in dtD_IMAGE_INFO.Rows)
                    {
                        var imageSq = int.Parse(row["IMAGE_SEQ"].ToString().Trim());
                        // ユーザ１、２の入力データをそれぞれ取得して格納する
                        var entryData1 = dao.SelectEntryData(this._DOC_ID, this._IMAGE_CAPTURE_DATE, this._IMAGE_CAPTURE_NUM, this._ENTRY_UNIT, imageSq, Consts.RecordKbn.Entry_1st);
                        var entryData_Inp1 = GetEntryDic(entryData1);
                        var entryData_OCR = GetEntryDicOCR(entryData1);
                        var DummyItemFlag = GetDummyItemFlag(entryData1);

                        var entryData2 = dao.SelectEntryData(this._DOC_ID, this._IMAGE_CAPTURE_DATE, this._IMAGE_CAPTURE_NUM, this._ENTRY_UNIT, imageSq, Consts.RecordKbn.Entry_2nd);
                        var entryData_Inp2 = GetEntryDic(entryData2);

                        // 差異を検出して、ユーザ区分が0のデータを登録する
                        var entry_List = new List<D_ENTRY>();
                        var iItemIdx = 0;
                        foreach (string entry_item in entryData_Inp1.Keys)
                        {
                            var entry_record = new D_ENTRY( this._IMAGE_CAPTURE_DATE
                                                              , this._IMAGE_CAPTURE_NUM
                                                              , this._DOC_ID
                                                              , this._ENTRY_UNIT
                                                              , imageSq
                                                              , Consts.RecordKbn.ADMIN
                                                              , entry_item);

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

                            // ２人目入力無しの場合、強制的に不一致にする
                            if (!_tbs[iItemIdx].Input2)
                            {
                                entry_record.VALUE = string.Empty;
                                entry_record.DIFF_FLAG = Consts.DiffFlag.ON;
                                diff_flg = true;
                            }

                            // OCR値
                            entry_record.OCR_VALUE = entryData_OCR[entry_item];

                            entry_record.DUMMY_ITEM_FLAG = DummyItemFlag[entry_item];

                            // 登録ユーザID
                            entry_record.INS_USER_ID = Program.LoginUser.USER_ID;
                            entry_List.Add(entry_record);

                            iItemIdx++;
                        }
                        // ユーザ区分が0のデータを登録する
                        dao.INSERT_D_ENTRY_ADMIN(entry_List);
                    }

                    swCompare.Stop();
                    log.Debug(String.Format("㉕コンペア処理終了:[経過時間:{0}]", swCompare.Elapsed));

                    return diff_flg;
                }

                /// <summary>
                /// エントリ項目取得
                /// </summary>
                /// <param name="entryData"></param>
                /// <returns></returns>
                private Dictionary<string, string> GetEntryDic(DataTable entryData)
                {
                    Dictionary<string, string> entryDataDic = new Dictionary<string, string>();
                    foreach (DataRow row in entryData.Rows)
                        if (!entryDataDic.Keys.Contains(row["ITEM_ID"].ToString().Trim()))
                            entryDataDic.Add(row["ITEM_ID"].ToString().Trim(), row["VALUE"].ToString().Trim());
                    return entryDataDic;
                }

                /// <summary>
                /// OCR結果取得
                /// </summary>
                /// <param name="entryData"></param>
                /// <returns></returns>
                private Dictionary<string, string> GetEntryDicOCR(DataTable entryData)
                {
                    Dictionary<string, string> entryDataDic = new Dictionary<string, string>();
                    foreach (DataRow row in entryData.Rows)
                        if (!entryDataDic.Keys.Contains(row["ITEM_ID"].ToString().Trim()))
                            entryDataDic.Add(row["ITEM_ID"].ToString().Trim(), row["OCR_VALUE"].ToString().Trim());
                    return entryDataDic;
                }

                /// <summary>
                /// ダミー項目フラグ取得
                /// </summary>
                /// <param name="entryData"></param>
                /// <returns></returns>
                private Dictionary<string, string> GetDummyItemFlag(DataTable entryData)
                {
                    Dictionary<string, string> entryDataDic = new Dictionary<string, string>();
                    foreach (DataRow row in entryData.Rows)
                        if (!entryDataDic.Keys.Contains(row["ITEM_ID"].ToString().Trim()))
                            entryDataDic.Add(row["ITEM_ID"].ToString().Trim(), row["DUMMY_ITEM_FLAG"].ToString().Trim());
                    return entryDataDic;
                }
        */
        #region 閉じるボタン
        /// <summary>
        /// 閉じる:_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"{this.lblTitle.Text}を終了します。\nよろしいですか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                if (backFocusTextBox != null)
                {
                    backFocusTextBox.Focus();
                    backFocusTextBox.Select();
                }
                //else
                //{
                //    ButtonExecute.Focus();
                //}
                return;
            }

            // 終了処理
            this.EndProc();
        }
        #endregion

        /// <summary>
        /// イメージ表示
        /// </summary>
        private void ShowImage()
        {
            this._IMAGE_SEQ = int.Parse(dtD_IMAGE_INFO.Rows[_ProcessingCount - 1]["IMAGE_SEQ"].ToString());
            this._ImgForm.DefaultZoomRate = this._DEFAULT_IMAGE_ZOOM_RATE;
            this._ImgForm.DefaultZoomRate = this._DEFAULT_IMAGE_ZOOM_RATE;
            this._ImgForm.StatusImagePath_Text = dtD_IMAGE_INFO.Rows[_ProcessingCount - 1]["IMAGE_PATH"].ToString();
            if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
                this._ImgForm.LabelBaseImagePath_Text = dtD_IMAGE_INFO.Rows[_ProcessingCount - 1]["BASE_IMAGE_FILE_PATH"].ToString();
            //this._ImgForm.IsShowBaseImage = false;
        }

        /// <summary>
        /// ShowNext
        /// </summary>
        private void ShowNext()
        {
            if (Consts.EntryMethod.IMAGE.Equals(_ENTRY_METHOD))
            {
                ShowImage();
            }

            if (_IsAutoLayoutMode)
            {
                ShowEntryItem(dtD_IMAGE_INFO.Rows[_ProcessingCount - 1]["OCR_IMAGE_FILE_NAME"].ToString());
            }

            // 処理状態表示
            var sRecordKbnName = string.Empty;
            if (Consts.RecordKbn.Entry_1st.Equals(this._RECORD_KBN))
            {
                sRecordKbnName = "１人目入力";
            }
            else if (Consts.RecordKbn.Entry_2nd.Equals(this._RECORD_KBN))
            {
                sRecordKbnName = "２人目入力";
            }
            else if (Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN))
            {
                if (_IsVerifyMode)
                {
                    sRecordKbnName = "検証";
                }
                else
                {
                    sRecordKbnName = "修正";
                }
            }
            this.lblParams.Text = $"エントリバッチID:{Utils.EditEntryBatchId(_ENTRY_UNIT_ID)}　{sRecordKbnName}";

            this.lblStatus.Text = $"{_ProcessingCount:d4}/{_TotalCount:d4}";

            if (_ProcessingCount == _TotalCount)
            {
                // 最終レコードの時に件数を「赤」
                this.lblStatus.ForeColor = Color.Red;
                if (_ProcessingCount != 1)
                {
                    System.Media.SystemSounds.Beep.Play();
                }
            }
            else
            {
                // 最終レコード以外の時に件数を「黒」
                this.lblStatus.ForeColor = this.lblParams.ForeColor;
            }
            var swSelect = System.Diagnostics.Stopwatch.StartNew();

            // エントリ取得
            _IMAGE_SEQ = int.Parse(dtD_IMAGE_INFO.Rows[_ProcessingCount - 1]["IMAGE_SEQ"].ToString());

            var dtD_ENTRY = _dao.SELECT_D_ENTRY(_ENTRY_UNIT_ID, this._RECORD_KBN, this._IMAGE_SEQ);

            swSelect.Stop();
            _log.Info($"D_ENTRY SELECT 終了:[経過時間:{swSelect.Elapsed}]");

            // 前レコード
            var drD_ENTRY = dtD_ENTRY.Select();
            dicDiffCount = new Dictionary<CTextBox.CTextBox, string>();

            /*
                        // テキストボックスの表示を初期化します。
                        foreach (var item in this.EntryItems.Select((textbox, index) => new { textbox, index }))
                        {
                            item.textbox.KeyDown -= ReEntryTextBox_KeyDown;
                            if (item.index + 1 <= drD_ENTRY.Count())
                            {
                                item.textbox.Text = drD_ENTRY[item.index]["VALUE"].ToString();
                                item.textbox.Tag = drD_ENTRY[item.index]["VALUE"].ToString();

                                // 管理者モードの場合、差異がない項目を入力不可にする
                                if (Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN) && !_IsVerifyMode)
                                {
                                    if (Consts.Flag.OFF.Equals(drD_ENTRY[item.index]["DIFF_FLAG"].ToString()))
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
                                        dicDiffCount.Add(item.textbox, drD_ENTRY[item.index]["DIFF_FLAG"].ToString());
                                    }
                                }
                            }
                            else
                            {
                                item.textbox.Text = string.Empty;
                            }
                        }
            */
            for (int i = 0; i <= _tbs.Length - 1; i++)
            {
                _tbs[i].KeyDown -= ReEntryTextBox_KeyDown;
                //if (item.index + 1 <= drD_ENTRY.Count())
                //{

                if (Consts.InputMode.KanaHalf.Equals(_tbs[i].InputMode)
                    || Consts.InputMode.AllHalf.Equals(_tbs[i].InputMode)
                    || Consts.InputMode.MixFull.Equals(_tbs[i].InputMode))
                {
                    _tbs[i].Text = Utils.ToUpperKana(drD_ENTRY[i]["VALUE"].ToString());
                }
                else
                {
                    _tbs[i].Text = drD_ENTRY[i]["VALUE"].ToString();
                }
                _tbs[i].Tag = drD_ENTRY[i]["VALUE"].ToString();
                _tbs[i].DummyItemFlag = drD_ENTRY[i]["DUMMY_ITEM_FLAG"].ToString();

                // 管理者モードの場合、差異がない項目を入力不可にする
                if (Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN) && !_IsVerifyMode)
                {
                    if (Consts.Flag.OFF.Equals(drD_ENTRY[i]["DIFF_FLAG"].ToString()))
                    {
                        _tbs[i].Enabled = false;
                        _tbs[i].ReadOnly = false;
                        _tbs[i].TabStop = false;
                        _tbs[i].BackColor = SystemColors.Control;
                    }
                    else
                    {
                        _tbs[i].Enabled = true;
                        _tbs[i].ReadOnly = true;
                        _tbs[i].TabStop = true;
                        _tbs[i].BackColor = SystemColors.Window;
                        _tbs[i].KeyDown += ReEntryTextBox_KeyDown;
                        dicDiffCount.Add(_tbs[i], drD_ENTRY[i]["DIFF_FLAG"].ToString());
                    }
                }
            }

            // 管理者モードの場合、差異がない項目を入力不可にする
            //if (Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN) && Consts.Flag.OFF.Equals(_IsVerifyMode))
            if (Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN) && !_IsVerifyMode)
            {
                // 登録ボタンを制御する
                CheckDiffCount();
            }

            // 前レコードボタン制御
            ButtonBack.Enabled = (_ProcessingCount >= 2);

            hasBeforeRecord = false;
            this._ImgForm.CheckBokAutoScroll_Checked = true;

            //先頭へフォーカスセット
            //if (Consts.Flag.ON.Equals(this._sUpdateFlag)
            if (Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN) && !_IsVerifyMode)
            {
                if (IsFormShown)
                {
                    TextDummy.Enabled = true;
                    TextDummy.Visible = true;
                    TextDummy.Focus();
                    TextDummy.Enabled = false;
                    TextDummy.Visible = false;
                    for (int iIdx = 0; iIdx <= this._tbs.Length - 1; iIdx++)
                    {
                        if (this._tbs[iIdx].Enabled)
                        {
                            this._tbs[iIdx].Focus();
                            break;
                        }
                    }
                }
            }
            else
            {
                GotoTopPosition();
            }

            #region メルペイ再審査固有
            if (IsMerpay)
            {
                if ("2".Equals(this._tbs[0].Text))
                {
                    this.RadioButtonNo.Checked = true;
                }
                else
                {
                    this.RadioButtonYes.Checked = true;
                }
                this.labelKansaId.Text = Path.GetFileNameWithoutExtension(dtD_IMAGE_INFO.Rows[_ProcessingCount - 1]["IMAGE_PATH"].ToString());
            }
            #endregion

            //this.TableLayoutPanel.ResumeLayout(false);
            //this.ResumeLayout(false);
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
            //dao.Close();

            // テキストボックス共通のハンドラを破棄する
            //            EntryItems.ForEach(o =>
            foreach (var tb in this._tbs.AsEnumerable())
            {
                tb.Leave -= TextBox_Leave;
                //o.KeyPress -= TextBox_KeyPress;
                tb.Validating -= TextBox_Validating;
                tb.Validated -= TextBox_Validated;
                tb.TextChanged -= TextBox_TextChanged;

                //if (Consts.Flag.ON.Equals(this._sUpdateFlag)
                if (Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN) && !_IsVerifyMode)
                //&& "0".Equals(_IsVerifyMode))
                {
                    tb.Enter -= TextBox_Enter_Admin;
                }
                else
                {
                    tb.Enter -= TextBox_Enter;
                }
            }//);

            //foreach (var btn in this._btns.AsEnumerable())
            //{
            //    if (btn != null)
            //        btn.Click -= new System.EventHandler(Button_Click);
            //}

            // テーブルレイアウトパネルのスクロール
            this.TableLayoutPanel.Scroll -= TableLayoutPanel_Scroll;

            this.Close();
            this.Dispose();

            this._ImgForm.Close();
            this._ImgForm.Dispose();
            GC.Collect();
        }
        #endregion

        /// <summary>
        /// 前レコード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBack_Click(object sender, EventArgs e)
        {
            Back();
        }

        private void Back()
        {
            _ProcessingCount--;
            hasBeforeRecord = true;
            ShowNext();
        }

        /// <summary>
        /// 入力値チェック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!(sender is CTextBox.CTextBox tb)) { return; }

            // 登録ボタン以外のボタンへの遷移時には処理しません。
            if (IgnoreControls.Contains(this.ActiveControl))
            {
                return;
            }

            // 入力可能パターン違反の場合にフォーカスを戻します。
            if (tb.ValidPattern.Length != 0 && tb.Text.Replace(Config.ReadNotCharNarrowInput, string.Empty).Replace(Config.ReadNotCharWideInput, string.Empty).Length != 0)
            {
                // 判別不能文字を除いてチェックする
                var omitedText = tb.Text.Replace(Config.ReadNotCharNarrowInput, string.Empty).Replace(Config.ReadNotCharWideInput, string.Empty);
                if (!Regex.IsMatch(omitedText, tb.ValidPattern))
                {
                    MessageBox.Show($"「{tb.ItemName}」に「{tb.Text}」は入力出来ません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                    return;
                }
            }

            #region 最大文字数
            if (tb.Text.Length > tb.MaxLength)
            {
                MessageBox.Show($"「{tb.ItemName}」の最大文字数は{tb.MaxLength}文字です。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
                return;
            }
            #endregion

            #region フル桁チェック
            if (tb.FullLength)
            {
                if (tb.Text.Length != 0 && tb.Text.Length != tb.MaxLength/* && !Config.ReadNotCharNarrowInput.Equals(tb.Text) && !Config.ReadNotCharWideInput.Equals(tb.Text)*/)
                {
                    MessageBox.Show($"「{tb.ItemName}」は{tb.MaxLength}文字で入力してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                    return;
                }
            }
            #endregion

            #region 入力禁止文字
            if (Consts.InputMode.Hiragana.Equals(tb.InputMode)
                || Consts.InputMode.Full.Equals(tb.InputMode)
                || Consts.InputMode.MixFull.Equals(tb.InputMode))
            {
                //bool b = true;
                //int iIdx = 0;
                for (int iIdx = 0; iIdx <= tb.Text.Length - 1; iIdx++)
                {
                    //b = ;
                    if (!Utils.IsValidChar(tb.InputMode, tb.Text.Substring(iIdx, 1)))
                    {
                        MessageBox.Show($"「{tb.ItemName}」の{iIdx + 1}文字目に入力禁止文字が含まれています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        tb.Focus();
                        tb.Select(iIdx, 1);
                        e.Cancel = true;
                        return;
                    }
                }
            }
            #endregion

            //#region 半角項目に全角入力とその逆
            //if (tb.Text.Length != 0)
            //{
            //    if (tb.InputMode == Consts.InputMode.AllHalf
            //        || tb.InputMode == Consts.InputMode.AlphabetNumeric
            //        || tb.InputMode == Consts.InputMode.KanaHalf)
            //    {
            //        if (!Utils.IsSingleByteChar(tb.Text))
            //        {
            //            MessageBox.Show(String.Format("「{0}」に全角文字は入力出来ません。", tb.ItemName), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //            e.Cancel = true;
            //            return;
            //        }
            //    }
            //    if (tb.InputMode == Consts.InputMode.Full
            //        || tb.InputMode == Consts.InputMode.Hiragana
            //        || tb.InputMode == Consts.InputMode.KanaFull)
            //    {
            //        if (Utils.IsSingleByteChar(tb.Text))
            //        {
            //            MessageBox.Show(String.Format("「{0}」に半角文字は入力出来ません。", tb.ItemName), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //            e.Cancel = true;
            //            return;
            //        }
            //    }
            //}
            //#endregion

            #region 半角項目に全角入力とその逆
            if (tb.Text.Length != 0)
            {
                if (tb.InputMode == Consts.InputMode.AllHalf
                    || tb.InputMode == Consts.InputMode.AlphabetNumeric
                    || tb.InputMode == Consts.InputMode.KanaHalf)
                {
                    for (int iIdx = 0; iIdx <= tb.Text.Length - 1; iIdx++)
                    {
                        if (!Utils.IsSingleByteChar(tb.Text.Substring(iIdx, 1)))
                        {
                            MessageBox.Show($"「{tb.ItemName}」に全角文字は入力出来ません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            tb.Focus();
                            tb.Select(iIdx, 1);
                            e.Cancel = true;
                            return;
                        }
                    }
                }

                if (tb.InputMode == Consts.InputMode.Full
                    || tb.InputMode == Consts.InputMode.Hiragana
                    || tb.InputMode == Consts.InputMode.KanaFull)
                {
                    for (int iIdx = 0; iIdx <= tb.Text.Length - 1; iIdx++)
                    {
                        if (Utils.IsSingleByteChar(tb.Text.Substring(iIdx, 1)))
                        {
                            MessageBox.Show($"「{tb.ItemName}」に半角文字は入力出来ません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            tb.Focus();
                            tb.Select(iIdx, 1);
                            e.Cancel = true;
                            return;
                        }
                    }
                }
            }
            #endregion

            #region マスタ存在チェック
            if (tb.MasterCheck)
            {
                if (!tb.Text.Contains(Config.ReadNotCharNarrowInput)
                    && tb.Text.Length != 0)
                {
                    if (!_dao.SELECT_M_ITEM_CHECK(_DOC_ID, tb.ItemName, tb.Text))
                    {
                        MessageBox.Show($"「{tb.ItemName}」に「{tb.Text}」は入力出来ません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        e.Cancel = true;
                        return;
                    }
                }
            }
            #endregion

            #region DR
            if (tb.DR.Length != 0 && !Utils.IsValidDr(tb.Text, tb.DR))
            {
                MessageBox.Show($"{tb.DR}DRチェックデジットエラー。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
                return;
            }
            //}
            #endregion

            #region MailAddress
            if (tb.IsMailAddress && tb.Text.Length != 0)
            {
                if (!Utils.IsValidMailAddress(tb.Text))
                {
                    MessageBox.Show($"無効な書式で入力されています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                    return;
                }
            }
            #endregion

            #region Date
            if (tb.Text.Length != 0 && tb.DateFormat.Length != 0)
            {
                if (!Utils.IsValidDate(tb.Text, tb.DateFormat))
                {
                    MessageBox.Show($"無効な書式で入力されています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                    return;
                }
            }
            #endregion

            if (!Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN) && !_IsVerifyMode)
            {
                switch (Utils.GetBussinessId())
                {
                    case Consts.BusinessID.NTO:
                        // 早稲田大学　成績請求票
                        if ("10010001".Equals(this._DOC_ID) && "TEXT001".Equals(tb.Name.ToUpper()))
                        {
                            if (_tbs[0].Text.Contains(Config.ReadNotCharNarrowInput))
                            {
                                MessageBox.Show("成績請求コードに判読不可文字が入力されています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            else if (!Utils.IsValidCheckDigit(_tbs[0].Text))
                            {
                                MessageBox.Show("成績請求コードが不正です。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                e.Cancel = true;
                                return;
                            }
                        }
                        break;
                }
            }

            #region 第一フロンティア生命　解約請求　固有処理
            if (!Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN))
            {
                switch (Utils.GetBussinessId())
                {
                    case Consts.BusinessID.DFK:
                        // その他の金融機関
                        // 金融機関名で存在チェック
                        // １－１
                        if ("TEXT005".Equals(tb.Name.ToUpper()))
                        {
                            if (_tbs[4].Text.Length != 0)
                            {
                                if (!_tbs[4].Text.Contains(Config.ReadNotCharWideInput)
                                    && !_tbs[4].Text.Contains("＃"))
                                {
                                    var dt408107 = _dao.SELECT_M_FINANCIAL_INSTITUTION(_tbs[4].Text);
                                    if (dt408107.Rows.Count == 0)
                                    {
                                        MessageBox.Show("入力された金融機関名は金融機関マスタに存在しません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        e.Cancel = true;
                                    }
                                }
                            }
                        }

                        // 金融機関名＋支店コードで支店名表示
                        // １－２
                        if ("TEXT006".Equals(tb.Name.ToUpper()))
                        {
                            if (_tbs[4].Text.Length != 0
                                && _tbs[5].Text.Length != 0)
                            {
                                if (!_tbs[4].Text.Contains(Config.ReadNotCharWideInput)
                                    && !_tbs[4].Text.Contains("＃")
                                    && !_tbs[5].Text.Contains(Config.ReadNotCharNarrowInput)
                                    && !_tbs[5].Text.Contains("#"))
                                {
                                    var dt408107 = _dao.SELECT_M_FINANCIAL_INSTITUTION_2(_tbs[4].Text, _tbs[5].Text);
                                    if (dt408107.Rows.Count > 0)
                                    {
                                        if (_tbs[6].Text.Length == 0
                                            && _tbs[7].Text.Length == 0
                                            && _tbs[8].Text.Length == 0
                                            && _tbs[9].Text.Length == 0
                                            && _tbs[10].Text.Length == 0)
                                        {
                                            _tbs[6].Text = dt408107.Rows[0]["BRANCH_NAME"].ToString().Replace("営業部", string.Empty).Replace("出張所", string.Empty);
                                            if (dt408107.Rows[0]["BRANCH_NAME"].ToString().Contains("営業部"))
                                            {
                                                _tbs[8].Text = Consts.Flag.ON;
                                            }
                                            if (dt408107.Rows[0]["BRANCH_NAME"].ToString().Contains("出張所"))
                                            {
                                                _tbs[9].Text = Consts.Flag.ON;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("入力された金融機関名、店番号は金融機関マスタに存在しません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        //e.Cancel = true;  20190828 削除 
                                    }
                                }
                            }
                        }

                        // 金融機関名＋支店名で存在チェック　
                        // １－３
                        if ("TEXT011".Equals(tb.Name.ToUpper()))
                        {
                            if (_tbs[4].Text.Length != 0
                                && _tbs[5].Text.Length != 0
                                && _tbs[6].Text.Length != 0)
                            {
                                if (!_tbs[4].Text.Contains(Config.ReadNotCharWideInput)
                                    && !_tbs[4].Text.Contains("＃")
                                    && !_tbs[5].Text.Contains(Config.ReadNotCharNarrowInput)
                                    && !_tbs[5].Text.Contains("#")
                                    && !_tbs[6].Text.Contains(Config.ReadNotCharWideInput)
                                    && !_tbs[6].Text.Contains("＃")
                                    )
                                {
                                    var i1 = 0;
                                    var addName = string.Empty;
                                    // 支店
                                    if (Consts.Flag.ON.Equals(_tbs[7].Text))
                                    {
                                        i1++;
                                    }
                                    // 営業部
                                    if (Consts.Flag.ON.Equals(_tbs[8].Text))
                                    {
                                        addName = "営業部";
                                        i1++;
                                    }
                                    // 出張所
                                    if (Consts.Flag.ON.Equals(_tbs[9].Text))
                                    {
                                        addName = "出張所";
                                        i1++;
                                    }
                                    // 支所
                                    if (Consts.Flag.ON.Equals(_tbs[10].Text))
                                    {
                                        i1++;
                                    }
                                    // 複数選択
                                    if (i1 != 1)
                                    {
                                        addName = string.Empty;
                                    }
                                    var dt408107 = _dao.SELECT_M_FINANCIAL_INSTITUTION(_tbs[4].Text, _tbs[6].Text + addName);
                                    if (dt408107.Rows.Count == 0)
                                    {
                                        MessageBox.Show("入力された金融機関名、支店名は金融機関マスタに存在しません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        //e.Cancel = true;
                                        _tbs[6].Focus();
                                    }
                                    else
                                    {
                                        //if (!_tbs[5].Text.Contains(Config.ReadNotCharNarrowInput)
                                        //    && !_tbs[5].Text.Contains("#"))
                                        //{
                                        if (!dt408107.Rows[0]["BRANCH_CD"].ToString().Equals(_tbs[5].Text))
                                        {
                                            MessageBox.Show("金融機関名、店番号、支店名の整合性が取れていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                            //e.Cancel = true;
                                            //_tbs[6].Focus();
                                        }
                                        //}
                                    }
                                }
                            }
                        }

                        // 通常金融機関
                        // 金融機関名で存在チェック
                        // ２－１
                        if ("TEXT021".Equals(tb.Name.ToUpper()))
                        {
                            if (_tbs[15].Text.Length != 0)
                            {
                                if (!_tbs[15].Text.Contains(Config.ReadNotCharWideInput)
                                    && !_tbs[15].Text.Contains("＃"))
                                {
                                    var i1 = 0;
                                    var addName = string.Empty;
                                    // 銀行
                                    if (Consts.Flag.ON.Equals(_tbs[16].Text))
                                    {
                                        i1++;
                                    }
                                    // 信用金庫
                                    if (Consts.Flag.ON.Equals(_tbs[17].Text))
                                    {
                                        addName = "信金";
                                        i1++;
                                    }
                                    // 信用組合
                                    if (Consts.Flag.ON.Equals(_tbs[18].Text))
                                    {
                                        addName = "信組";
                                        i1++;
                                    }
                                    // 農協
                                    if (Consts.Flag.ON.Equals(_tbs[19].Text))
                                    {
                                        addName = "農協";
                                        i1++;
                                    }
                                    // 労働金庫
                                    if (Consts.Flag.ON.Equals(_tbs[20].Text))
                                    {
                                        addName = "労金";
                                        i1++;
                                    }
                                    // 複数選択
                                    if (i1 != 1)
                                    {
                                        addName = string.Empty;
                                    }
                                    var dt408107 = _dao.SELECT_M_FINANCIAL_INSTITUTION(_tbs[15].Text + addName);
                                    if (dt408107.Rows.Count == 0)
                                    {
                                        MessageBox.Show("入力された金融機関名は金融機関マスタに存在しません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        _tbs[15].Focus();
                                        //e.Cancel = true;
                                    }
                                }
                            }
                        }

                        // 金融機関名＋支店コードで支店名表示
                        // ２－２
                        if ("TEXT022".Equals(tb.Name.ToUpper()))
                        {
                            if (_tbs[15].Text.Length != 0
                                && _tbs[21].Text.Length != 0)
                            {
                                if (!_tbs[15].Text.Contains(Config.ReadNotCharWideInput)
                                    && !_tbs[15].Text.Contains("＃")
                                    && !_tbs[21].Text.Contains(Config.ReadNotCharNarrowInput)
                                    && !_tbs[21].Text.Contains("#"))
                                {
                                    var i1 = 0;
                                    var addName = string.Empty;
                                    // 銀行
                                    if (Consts.Flag.ON.Equals(_tbs[16].Text))
                                    {
                                        i1++;
                                    }
                                    // 信用金庫
                                    if (Consts.Flag.ON.Equals(_tbs[17].Text))
                                    {
                                        addName = "信金";
                                        i1++;
                                    }
                                    // 信用組合
                                    if (Consts.Flag.ON.Equals(_tbs[18].Text))
                                    {
                                        addName = "信組";
                                        i1++;
                                    }
                                    // 農協
                                    if (Consts.Flag.ON.Equals(_tbs[19].Text))
                                    {
                                        addName = "農協";
                                        i1++;
                                    }
                                    // 労働金庫
                                    if (Consts.Flag.ON.Equals(_tbs[20].Text))
                                    {
                                        addName = "労金";
                                        i1++;
                                    }
                                    // 複数選択
                                    if (i1 != 1)
                                    {
                                        addName = string.Empty;
                                    }

                                    var dt408107 = _dao.SELECT_M_FINANCIAL_INSTITUTION_2(_tbs[15].Text + addName, _tbs[21].Text);
                                    if (dt408107.Rows.Count > 0)
                                    {
                                        if (_tbs[22].Text.Length == 0
                                            && _tbs[23].Text.Length == 0
                                            && _tbs[24].Text.Length == 0
                                            && _tbs[25].Text.Length == 0
                                            && _tbs[26].Text.Length == 0)
                                        {
                                            _tbs[22].Text = dt408107.Rows[0]["BRANCH_NAME"].ToString().Replace("営業部", string.Empty).Replace("出張所", string.Empty);
                                            if (dt408107.Rows[0]["BRANCH_NAME"].ToString().Contains("営業部"))
                                            {
                                                _tbs[24].Text = Consts.Flag.ON;
                                            }
                                            if (dt408107.Rows[0]["BRANCH_NAME"].ToString().Contains("出張所"))
                                            {
                                                _tbs[25].Text = Consts.Flag.ON;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("入力された金融機関名、店番号は金融機関マスタに存在しません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        //e.Cancel = true; // 201908028 削除
                                    }
                                }
                            }
                        }

                        // 金融機関名＋支店名で存在チェック　
                        if ("TEXT027".Equals(tb.Name.ToUpper()))
                        {
                            if (_tbs[15].Text.Length != 0
                                && _tbs[21].Text.Length != 0
                                && _tbs[22].Text.Length != 0)
                            {
                                if (!_tbs[15].Text.Contains(Config.ReadNotCharWideInput)
                                    && !_tbs[15].Text.Contains("＃")
                                    && !_tbs[21].Text.Contains(Config.ReadNotCharNarrowInput)
                                    && !_tbs[21].Text.Contains("#")
                                    && !_tbs[22].Text.Contains(Config.ReadNotCharWideInput)
                                    && !_tbs[22].Text.Contains("＃"))
                                {
                                    var i1 = 0;
                                    var addName = string.Empty;
                                    // 銀行
                                    if (Consts.Flag.ON.Equals(_tbs[16].Text))
                                    {
                                        i1++;
                                    }
                                    // 信用金庫
                                    if (Consts.Flag.ON.Equals(_tbs[17].Text))
                                    {
                                        addName = "信金";
                                        i1++;
                                    }
                                    // 信用組合
                                    if (Consts.Flag.ON.Equals(_tbs[18].Text))
                                    {
                                        addName = "信組";
                                        i1++;
                                    }
                                    // 農協
                                    if (Consts.Flag.ON.Equals(_tbs[19].Text))
                                    {
                                        addName = "農協";
                                        i1++;
                                    }
                                    // 労働金庫
                                    if (Consts.Flag.ON.Equals(_tbs[20].Text))
                                    {
                                        addName = "労金";
                                        i1++;
                                    }
                                    // 複数選択
                                    if (i1 != 1)
                                    {
                                        addName = string.Empty;
                                    }
                                    var i2 = 0;
                                    var addName2 = string.Empty;
                                    // 支店
                                    if (Consts.Flag.ON.Equals(_tbs[23].Text))
                                    {
                                        i2++;
                                    }
                                    // 営業部
                                    if (Consts.Flag.ON.Equals(_tbs[24].Text))
                                    {
                                        addName2 = "営業部";
                                        i2++;
                                    }
                                    // 出張所
                                    if (Consts.Flag.ON.Equals(_tbs[25].Text))
                                    {
                                        addName2 = "出張所";
                                        i2++;
                                    }
                                    // 支所
                                    if (Consts.Flag.ON.Equals(_tbs[26].Text))
                                    {
                                        i2++;
                                    }
                                    // 複数選択
                                    if (i2 != 1)
                                    {
                                        addName2 = string.Empty;
                                    }
                                    var dt408107 = _dao.SELECT_M_FINANCIAL_INSTITUTION(_tbs[15].Text + addName, _tbs[22].Text + addName2);
                                    if (dt408107.Rows.Count == 0)
                                    {
                                        MessageBox.Show("入力された金融機関名、支店名は金融機関マスタに存在しません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        //e.Cancel = true;
                                        _tbs[22].Focus();
                                    }
                                    else
                                    {
                                        //if (!_tbs[21].Text.Contains(Config.ReadNotCharNarrowInput)
                                        //    && !_tbs[21].Text.Contains("#")
                                        //    && _tbs[21].Text.Length != 0
                                        //    )
                                        //{
                                        if (!dt408107.Rows[0]["BRANCH_CD"].ToString().Equals(_tbs[21].Text))
                                        {
                                            MessageBox.Show("金融機関名、店番号、支店名の整合性が取れていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                            //e.Cancel = true;
                                            //_tbs[22].Focus();
                                        }
                                        //}
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            #endregion

        }

        /// <summary>
        /// TextBox.Validated
        /// </summary>
        private void TextBox_Validated(object sender, EventArgs e)
        {
            // 入力項目ハイライト消去
            if (_IsAutoLayoutMode) { this._ImgForm.LabelImagePosition_Text = "0,0,0,0"; }
        }

        /// <summary>
        /// TextBox.Leave
        /// </summary>
        private void TextBox_Leave(object sender, EventArgs e)
        {
            if (!(sender is CTextBox.CTextBox tb)) { return; }

            //20171109 add begin
            //オプションの処理を行う
            if (this.dicOptionalProcessInfos.ContainsKey(tb))
            {
                InvokeOptionalMethod(this.dicOptionalProcessInfos[tb][0]);
            }
            //20171109 add end

            // 受領データ表示
            TextBox_Leave_Custom(tb);
        }

        /// <summary>
        /// TextBox.Enterイベント
        /// </summary>
        private void TextBox_Enter(object sender, EventArgs e)
        {
            if (!(sender is CTextBox.CTextBox tb)) { return; }

            backFocusTextBox = tb;

            idxtbox = int.Parse(tb.Name.Substring(4)) - 1;

            if (int.Parse(tb.Name.Substring(4, 3)) % 100 == 0)
            {
                TableLayoutPanelReflesh();
            }

            // 入力項目ハイライト
            if (_IsAutoLayoutMode && tb.Enabled)
            {
                this._ImgForm.LabelImagePosition_Text = tb.ImagePosition;
            }

            // 表示画像の自動スクロールを処理します。
            this.AutoSyncScrollImage(tb);

            if (tb.Tips.Length == 0)
            {
                this.toolTips.Hide(this);
            }
            else
            {
                //this.Activate();
                TextDummy.Focus();
                tb.Focus();

                var pt = new Point(0, 0);
                WindowsFormUtils.AbsolutePositionFromForm(tb, ref pt);
                pt.X += tb.Width + 15;
                if (this.toolTips.IsBalloon)
                {
                    pt.Y -= 45;
                }
                this.toolTips.Show(tb.Tips.Replace(@"\n", "\n"), this, pt);
            }

            // カスタマイズ処理
            TextBox_Enter_Custom(tb);
        }

        /// <summary>
        /// TextBox.Enterイベント(管理者モード)
        /// </summary>
        private void TextBox_Enter_Admin(object sender, EventArgs e)
        {
            if (!(sender is CTextBox.CTextBox tb)) { return; }

            if (this.TableLayoutPanel.VerticalScroll.Maximum > 32000)
            {
                TableLayoutPanelReflesh();
            }

            // このテキストボックスをフォーカスの戻り先に設定します。
            backFocusTextBox = tb;

            // 入力項目ハイライト
            if (_IsAutoLayoutMode)
            {
                this._ImgForm.LabelImagePosition_Text = tb.ImagePosition;
            }

            // 入力項目の自動スクロール
            //tb.Focus();
            tb.SelectAll();

            // 表示画像の自動スクロールを処理します。
            this.AutoSyncScrollImage(tb);

            // 再入力画面を表示する
            ShowReEntryForm(tb);
        }

        /// <summary>
        /// ShowReEntryForm
        /// </summary>
        /// <param name="textBox"></param>
        private void ShowReEntryForm(CTextBox.CTextBox textBox)
        {
            var item_id = String.Join("_", "ITEM", int.Parse(textBox.Name.Substring(4)).ToString("d3"));
            var textIndex = dicDiffCount.Keys.ToList().IndexOf(textBox) + 1;

            using (var frm = new FrmReEntry(this._ImgForm, _ENTRY_UNIT_ID, _IMAGE_SEQ, item_id, textIndex, dicDiffCount.Count, textBox))
            {
                frm.Owner = this;
                //frm._TargetTextBox = textBox;
                frm.ShowDialog();

                if (frm.IsReEntry())
                {
                    textBox.Text = frm.GetRetValue();

                    // 画面で記憶する差異フラグを0に更新する
                    dicDiffCount[textBox] = Consts.Flag.OFF;

                    // 修正状態を見て登録ボタンを制御
                    CheckDiffCount();

                    NextControl();
                }
            }
        }

        /// <summary>
        /// 自動同期スクロール処理
        /// </summary>
        private void AutoSyncScrollImage(CTextBox.CTextBox textBox)
        {
            if (Consts.EntryMethod.IMAGE.Equals(_ENTRY_METHOD))
            {
                if (this.autoScrollRegions == null) { return; }

                if (!this._ImgForm.CheckBokAutoScroll_Checked) { return; }

                var info = this.autoScrollRegions.Find(textBox.Name);
                if (info == null) { return; }

                this._ImgForm.PanelImage_AutoScrollPosition = new Point(info.OriginX, info.OriginY);
            }
        }

        /// <summary>
        /// 差異がある項目数をチェックして、なければ登録ボタンを有効にする
        /// </summary>
        private void CheckDiffCount()
        {
            ButtonExecute.Enabled = !dicDiffCount.Values.Any(flg => flg == Consts.Flag.ON);
        }

        ///// <summary>
        ///// Panel.Scroll
        ///// </summary>
        //private void PanelImageArea_Scroll(object sender, ScrollEventArgs e)
        //{
        //    // ユーザーによるスクロールが行われたとき、自動スクロールのチェックを外します。
        //    this._ImgForm.CheckBokAutoScroll_Checked = false;
        //}

        /// <summary>
        /// FrmEntry_KeyDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmEntry_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    // F1　↓スクロール
                    //if (this._ImgForm.IsShowBaseImageValue)
                    //{
                    //    this._BaseImaegPostion_Y += this._ImgForm.PnlImage_VerticalScroll_Maximum / 4;
                    //    if (this._BaseImaegPostion_Y > this._ImgForm.PnlImage_VerticalScroll_Maximum)
                    //    {
                    //        this._BaseImaegPostion_Y = this._ImgForm.PnlImage_VerticalScroll_Maximum;
                    //    }
                    //    this._ImgForm.PanelImage_AutoScrollPosition = new Point(this._BaseImaegPostion_X, this._BaseImaegPostion_Y);
                    //}
                    //else
                    //{
                    //    this._ImaegPostion_Y += this._ImgForm.PnlImage_VerticalScroll_Maximum / 4;
                    //    if (this._ImaegPostion_Y > this._ImgForm.PnlImage_VerticalScroll_Maximum)
                    //    {
                    //        this._ImaegPostion_Y = this._ImgForm.PnlImage_VerticalScroll_Maximum;
                    //    }
                    //    this._ImgForm.PanelImage_AutoScrollPosition = new Point(this._ImaegPostion_X, this._ImaegPostion_Y);
                    //}
                    this._ImgForm.ImaegPostion_Y_Value += this._ImgForm.PnlImage_VerticalScroll_Maximum / 4;
                    if (this._ImgForm.ImaegPostion_Y_Value > this._ImgForm.PnlImage_VerticalScroll_Maximum)
                    {
                        this._ImgForm.ImaegPostion_Y_Value = this._ImgForm.PnlImage_VerticalScroll_Maximum;
                    }
                    this._ImgForm.PanelImage_AutoScrollPosition = new Point(this._ImgForm.ImaegPostion_X_Value, this._ImgForm.ImaegPostion_Y_Value);
                    e.Handled = true;
                    break;
                case Keys.F2:
                    // F2　↑スクロール
                    //if (this._ImgForm.IsShowBaseImageValue)
                    //{
                    //    this._BaseImaegPostion_Y -= this._ImgForm.PnlImage_VerticalScroll_Maximum / 4;
                    //    if (this._BaseImaegPostion_Y < 0)
                    //    {
                    //        this._BaseImaegPostion_Y = 0;
                    //    }
                    //    this._ImgForm.PanelImage_AutoScrollPosition = new Point(this._BaseImaegPostion_X, this._BaseImaegPostion_Y);
                    //}
                    //else
                    //{
                    //    this._ImaegPostion_Y -= this._ImgForm.PnlImage_VerticalScroll_Maximum / 4;
                    //    if (this._ImaegPostion_Y < 0)
                    //    {
                    //        this._ImaegPostion_Y = 0;
                    //    }
                    //    this._ImgForm.PanelImage_AutoScrollPosition = new Point(this._ImaegPostion_X, this._ImaegPostion_Y);
                    //}
                    this._ImgForm.ImaegPostion_Y_Value -= this._ImgForm.PnlImage_VerticalScroll_Maximum / 4;
                    if (this._ImgForm.ImaegPostion_Y_Value < 0)
                    {
                        this._ImgForm.ImaegPostion_Y_Value = 0;
                    }
                    this._ImgForm.PanelImage_AutoScrollPosition = new Point(this._ImgForm.ImaegPostion_X_Value, this._ImgForm.ImaegPostion_Y_Value);
                    //e.Handled = true;
                    break;
                case Keys.F3:
                    // F3　→スクロール
                    //if (this._ImgForm.IsShowBaseImageValue)
                    //{
                    //    this._BaseImaegPostion_X += this._ImgForm.PnlImage_HorizontalScroll_Maximum / 4;
                    //    if (this._BaseImaegPostion_X > this._ImgForm.PnlImage_HorizontalScroll_Maximum)
                    //    {
                    //        this._BaseImaegPostion_X = this._ImgForm.PnlImage_HorizontalScroll_Maximum;
                    //    }
                    //    this._ImgForm.PanelImage_AutoScrollPosition = new Point(this._BaseImaegPostion_X, this._BaseImaegPostion_Y);
                    //}
                    //else
                    //{
                    //    this._ImaegPostion_X += this._ImgForm.PnlImage_HorizontalScroll_Maximum / 4;
                    //    if (this._ImaegPostion_X > this._ImgForm.PnlImage_HorizontalScroll_Maximum)
                    //    {
                    //        this._ImaegPostion_X = this._ImgForm.PnlImage_HorizontalScroll_Maximum;
                    //    }
                    //    this._ImgForm.PanelImage_AutoScrollPosition = new Point(this._ImaegPostion_X, this._ImaegPostion_Y);
                    //                        }
                    this._ImgForm.ImaegPostion_X_Value += this._ImgForm.PnlImage_HorizontalScroll_Maximum / 4;
                    if (this._ImgForm.ImaegPostion_X_Value > this._ImgForm.PnlImage_HorizontalScroll_Maximum)
                    {
                        this._ImgForm.ImaegPostion_X_Value = this._ImgForm.PnlImage_HorizontalScroll_Maximum;
                    }
                    this._ImgForm.PanelImage_AutoScrollPosition = new Point(this._ImgForm.ImaegPostion_X_Value, this._ImgForm.ImaegPostion_Y_Value);
                    e.Handled = true;
                    break;
                case Keys.F4:
                    // F4　←スクロール
                    //if (this._ImgForm.IsShowBaseImageValue)
                    //{
                    //    this._BaseImaegPostion_X -= this._ImgForm.PnlImage_HorizontalScroll_Maximum / 4;
                    //    if (this._BaseImaegPostion_X < 0)
                    //    {
                    //        this._BaseImaegPostion_X = 0;
                    //    }
                    //    this._ImgForm.PanelImage_AutoScrollPosition = new Point(this._BaseImaegPostion_X, this._BaseImaegPostion_Y);
                    //}
                    //else
                    //{
                    //    this._ImaegPostion_X -= this._ImgForm.PnlImage_HorizontalScroll_Maximum / 4;
                    //    if (this._ImaegPostion_X < 0)
                    //    {
                    //        this._ImaegPostion_X = 0;
                    //    }
                    //    this._ImgForm.PanelImage_AutoScrollPosition = new Point(this._ImaegPostion_X, this._ImaegPostion_Y);
                    //}
                    this._ImgForm.ImaegPostion_X_Value -= this._ImgForm.PnlImage_HorizontalScroll_Maximum / 4;
                    if (this._ImgForm.ImaegPostion_X_Value < 0)
                    {
                        this._ImgForm.ImaegPostion_X_Value = 0;
                    }
                    this._ImgForm.PanelImage_AutoScrollPosition = new Point(this._ImgForm.ImaegPostion_X_Value, this._ImgForm.ImaegPostion_Y_Value);
                    e.Handled = true;
                    break;
                case Keys.F5:
                    // F5キーでイメージ拡大
                    this._ImgForm.ImageZoomCount = 1;
                    this._ImgForm.CheckBokAutoScroll_Checked = false;
                    e.Handled = true;
                    break;
                case Keys.F6:
                    // F6キーでイメージ縮小
                    this._ImgForm.ImageZoomCount = -1;
                    this._ImgForm.CheckBokAutoScroll_Checked = false;
                    e.Handled = true;
                    break;
                case Keys.F7:
                    // F7キーでイメージ回転
                    this._ImgForm.ImageRotate();
                    e.Handled = true;
                    break;
                case Keys.F8:
                    // イメージ頁切替
                    // シフトキー確認
                    //var forward = true;
                    //if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) { forward = false; }
                    if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
                    {
                        this._ImgForm.IsShowBaseImageValue = !this._ImgForm.IsShowBaseImageValue;
                    }
                    else
                    {
                        this._ImgForm.ChangePage(!((Control.ModifierKeys & Keys.Shift) == Keys.Shift));
                    }
                    this.Activate();
                    e.Handled = true;
                    break;
                case Keys.F9:
                    // F9キーで項目ジャンプ
                    this.SkipItem();
                    e.Handled = true;
                    break;
                case Keys.F10:
                    // イメージ画面最前化
                    if (!Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
                    {
                        this._ImgForm.Activate();
                        this.Activate();
                    }
                    e.Handled = true;
                    break;
                case Keys.F12:
                    // F12キーで自動スクロールのチェック状態を反転
                    this._ImgForm.CheckBokAutoScroll_Checked = !this._ImgForm.CheckBokAutoScroll_Checked;
                    this._ImgForm.ImageZoomCount = 0;
                    e.Handled = true;
                    break;
                case Keys.PageUp:
                    // 「PageUp」で前レコード。但し押せる場合のみ
                    if (this.ButtonBack.Enabled) { Back(); }
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    //「Esc」で閉じる
                    this.ButtonClose.PerformClick();
                    e.Handled = true;
                    break;
                default:
                    break;
            }
            //this.Activate();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SkipItem()
        {
            if (!Consts.BusinessID.MYD.Equals(Utils.GetBussinessId()))
                return;

            Control ctrl = this.ActiveControl;
            if (ctrl == null) { return; }

            // シフトキー確認
            var forward = true;
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) { forward = false; }

            for (int i = 0; i <= this._tbs.Length - 1; i++)
            {
                if (ctrl.Name.Equals(this._tbs[i].Name))
                {
                    var itemNo = int.Parse(ctrl.Name.Substring(4, 3)) - 1;
                    if (forward)
                    {
                        // 前方スキップ
                        for (int i2 = itemNo + 1; i2 <= this._tbs.Length - 1; i2++)
                        {
                            if (this._tbs[i2].JumpTab)
                            {
                                this._tbs[i2].Focus();
                                TableLayoutPanelReflesh();
                                return;
                            }
                        }
                    }
                    else
                    {
                        // スキップ戻り
                        if (itemNo == 0) { return; }
                        for (int i2 = itemNo - 1; i2 > -1; i2--)
                        {
                            if (this._tbs[i2].JumpTab)
                            {
                                this._tbs[i2].Focus();
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReEntryTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is CTextBox.CTextBox tb)) { return; }

            // F2キーで再入力画面を開く
            if (e.KeyCode == Keys.F2) { ShowReEntryForm(tb); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonExec_Enter(object sender, EventArgs e)
        {
            // Tips消し
            this.toolTips.Hide(this);
            this.ButtonExecute.Font = new Font("Meiryo UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(128)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonExec_Leave(object sender, EventArgs e)
        {
            this.ButtonExecute.Font = new Font("Meiryo UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
        }

        /// <summary>
        /// GotoTopPosition
        /// </summary>
        private void GotoTopPosition()
        {
            this.Activate();
            // パネルスクロールバー
            PanelEntry.AutoScrollPosition = new Point(0, 0);
            if (this.TableLayoutPanel != null) { this.TableLayoutPanel.VerticalScroll.Value = 0; }

            if (Consts.EntryUnitStatus.EXPORT_END.Equals(this._STATUS)) { this.ButtonExecute.Focus(); }
            else
            {
                // テキストボックス
                int iTabIndex = 1000;
                int iTextBoxIndex = 1000;
                for (int iIdx = 0; iIdx <= this._tbs.Length - 1; iIdx++)
                {
                    if (this._tbs[iIdx].Enabled)
                    {
                        if (iTabIndex > this._tbs[iIdx].TabIndex)
                        {
                            iTextBoxIndex = iIdx;
                            iTabIndex = this._tbs[iIdx].TabIndex;
                            break;
                        }
                    }
                }

                if (iTabIndex <= this._tbs.Length && !IsMerpay)
                {
                    // イメージ位置を初期値へ
                    AutoSyncScrollImage(_tbs[iTextBoxIndex]);

                    // フォーカス
                    _tbs[iTextBoxIndex].Focus();

                    // 全選択
                    _tbs[iTextBoxIndex].Select();

                    //TableLayoutPanelReflesh();
                }
                else
                {
                    AutoSyncScrollImage(_tbs[0]);

                    this.ButtonExecute.Focus();

                    _tbs[iTextBoxIndex].Select();

                    //TableLayoutPanelReflesh();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DataTable GetEntryItemsSetting()
        {
            _log.Info("GetEntryItemsSetting:start");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var dtEntryItems = new DataTable();
            try
            {
                using (var tfp = new TextFieldParser(Path.Combine(Config.AppPath, String.Format(@"Items\{0}\{1}_{2}_{3}.txt"
                                                                                                  , Config.UserId
                                                                                                  , Config.TokuisakiCode
                                                                                                  , Config.HinmeiCode
                                                                                                  , _DOC_ID_ENTRY.Length == 0 ? _DOC_ID : _DOC_ID_ENTRY))))
                {
                    tfp.TextFieldType = FieldType.Delimited;
                    tfp.SetDelimiters("\t");

                    string[] data = null;
                    //データがあるか確認します。
                    if (!tfp.EndOfData)
                    {
                        //CSVファイルから1行読み取ります。
                        data = tfp.ReadFields();

                        //カラムの数を取得します。
                        var cols = data.Length;
                        if (cols != 0)
                        {
                            for (var i = 0; i < cols; i++)
                            {
                                //カラム名をセットします
                                dtEntryItems.Columns.Add(data[i]);
                            }
                        }
                    }

                    int ColumnsCount = dtEntryItems.Columns.Count;
                    // CSVをデータテーブルに格納
                    while (!tfp.EndOfData)
                    {
                        data = tfp.ReadFields();
                        var row = dtEntryItems.NewRow();
                        for (var iIdx = 0; iIdx < ColumnsCount; iIdx++)
                        {
                            row[iIdx] = data[iIdx];
                        }
                        dtEntryItems.Rows.Add(row);
                    }
                }
                return dtEntryItems;
            }
            finally
            {
                sw.Stop();
                _log.Info($"GetEntryItemsSetting.end:[経過時間{sw.Elapsed}]");
            }
        }

        #region CreateForm
        /// <summary>
        /// 
        /// </summary>
        private void CreateForm(/*string sDocId*/)
        {
            _log.Info("CreateForm:start");
            var sw = System.Diagnostics.Stopwatch.StartNew();

            var dtTxt = GetEntryItemsSetting();

            this.PanelEntry.Left = 20;
            this.PanelEntry.Top = 85;
            this.PanelEntry.AutoScroll = true;
            this.PanelEntry.Name = "pnlEntry";
            this.PanelEntry.TabIndex = 0;
            this.PanelEntry.Width = 910;
            this.PanelEntry.Height = this.Height - 185;
            this.PanelEntry.BackColor = SystemColors.ControlDark;

            #region GroupBox
            var drs = dtTxt.Select("ItemKind='G'");
            if (drs.Length != 0)
                this._gbs = new GroupBox[drs.Length];
            var ItemNumber = 0;

            var iRowMargin = 20;
            var iRowPitch = 22;
            var iRow = 0;

            var iColnumMargin = 12;
            var iColnumPitch = 8;
            var iColnum = 5;

            foreach (var dr in drs.AsEnumerable())
            {
                //this._gbs.Add(new System.Windows.Forms.GroupBox());
                this._gbs[ItemNumber] = new GroupBox();
                //this._gbs[iItemNumber] = new GroupBox();
                this._gbs[ItemNumber].TabStop = true;

                // Visible
                switch (dr["Visible"].ToString().ToUpper())
                {
                    case "T":
                    default:
                        this._gbs[ItemNumber].Visible = true;
                        this._gbs[ItemNumber].Enabled = true;
                        break;
                    case "F":
                        this._gbs[ItemNumber].Visible = false;
                        this._gbs[ItemNumber].Enabled = false;
                        break;
                }

                // TabIndex
                this._gbs[ItemNumber].TabIndex = int.Parse(dr["TabIndex"].ToString());

                // Name
                this._gbs[ItemNumber].Name = "GroupBox_" + dr["ControlName"].ToString();

                //if ("GroupBox_000".Equals(this._gbs[iItemNumber].Name))
                //{
                //    this._gbs[iItemNumber].BorderColor = Color.Transparent;
                //}

                // Text
                this._gbs[ItemNumber].Text = dr["Text"].ToString();

                // Location
                if (ItemNumber == 0)
                {
                    iRow = int.Parse((float.Parse(dr["Location_Row"].ToString())).ToString());
                    iColnum = int.Parse((float.Parse(dr["Location_Column"].ToString()) * iColnumPitch).ToString());
                }
                else
                {
                    iRow = int.Parse((float.Parse(dr["Location_Row"].ToString()) * iRowPitch).ToString()) + iRowMargin;
                    iColnum = int.Parse((float.Parse(dr["Location_Column"].ToString()) * iColnumPitch).ToString()) + iColnumMargin;
                }
                this._gbs[ItemNumber].Location = new Point(iColnum, iRow);

                // Width
                this._gbs[ItemNumber].Width = int.Parse(dr["Width"].ToString());

                // Height
                this._gbs[ItemNumber].Height = int.Parse(dr["Height"].ToString());

                // Font
                this._gbs[ItemNumber].Font = new Font("Meiryo UI", _FONT_SIZE, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));

                // ForeColor
                switch (dr["ForeColor"].ToString())
                {
                    case "R":
                        this._gbs[ItemNumber].ForeColor = Color.Red;
                        break;
                    case "B":
                        this._gbs[ItemNumber].ForeColor = Color.Blue;
                        break;
                    case "Y":
                        this._gbs[ItemNumber].ForeColor = Color.Yellow;
                        break;
                    case "G":
                        this._gbs[ItemNumber].ForeColor = Color.Green;
                        break;
                }

                // 必須（赤）
                //switch (dr["Required"].ToString().ToUpper())
                //{
                //    case "T":
                //        this._gbs[iItemNumber].ForeColor = Color.Red;
                //        break;
                //    default:
                //        if (dr["Required"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                //            this._gbs[iItemNumber].ForeColor = Color.Red;
                //        break;
                //}
                // 必須項目（赤）
                switch (dr["Required"].ToString().ToUpper())
                {
                    case "T":
                        this._gbs[ItemNumber].ForeColor = Color.Red;
                        //this._gbs[ItemNumber].Text += "（入力必須）";
                        break;
                    default:
                        if (dr["Required"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                        {
                            this._gbs[ItemNumber].ForeColor = Color.Red;
                            //this._gbs[ItemNumber].Text += "（入力必須）";
                        }
                        break;
                }

                this._gbs[ItemNumber].Tag = Consts.Flag.ON;

                // Add To GroupBox
                //for (int iIdx = 0; iIdx < this._gbs.Length; iIdx++)
                for (var iIdx = 0; iIdx <= this._gbs.Length - 1; iIdx++)
                {
                    if (this._gbs[iIdx] != null)
                    {
                        if (("GroupBox_" + dr["GroupBox"].ToString()).Equals(_gbs[iIdx].Name))
                        {
                            this._gbs[iIdx].Controls.Add(this._gbs[ItemNumber]);
                            this._gbs[ItemNumber].Tag = Consts.Flag.OFF;
                        }
                    }
                }
                ItemNumber++;
            }
            #endregion

            #region TextBox
            // TextBox
            //List<string> sAddressInfoList = new List<string>();
            //List<string> sAddressInfoListArea = new List<string>();
            //List<string> sHandlerInfoList = new List<string>();
            drs = dtTxt.Select("ItemKind='T'");
            if (drs.Length != 0)
                this._tbs = new CTextBox.CTextBox[drs.Length];
            ItemNumber = 0;
            foreach (var dr in drs.AsEnumerable())
            {
                //this._tbs.Add(new CTextBox.CTextBox());
                // コンストラクタ
                this._tbs[ItemNumber] = new CTextBox.CTextBox();

                // Name
                this._tbs[ItemNumber].Name = "text" + (ItemNumber + 1).ToString("d3");

                // Text
                this._tbs[ItemNumber].Text = dr["Text"].ToString();

                // Enabled
                //this._tbs[iItemNumber].Enabled = false;
                switch (dr["Enabled"].ToString().ToUpper())
                {
                    case "T":
                        this._tbs[ItemNumber].Enabled = true;
                        break;
                    case "F":
                        this._tbs[ItemNumber].Enabled = false;
                        break;
                    default:
                        // 一致する帳票IDが含まれる場合は活性化する
                        if (dr["Enabled"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                            this._tbs[ItemNumber].Enabled = true;
                        break;
                }

                try
                {
                    switch (dr["DummyItem"].ToString().ToUpper())
                    {
                        case "T":
                            this._tbs[ItemNumber].IsDummyItem = true;
                            //this._tbs[iItemNumber].Enabled = false;
                            break;
                        default:
                            if (dr["DummyItem"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                            {
                                this._tbs[ItemNumber].IsDummyItem = true;
                                //this._tbs[iItemNumber].Enabled = false;
                            }
                            break;
                    }
                }
                catch { }

                try
                {
                    switch (dr["JumpTab"].ToString().ToUpper())
                    {
                        case "T":
                            this._tbs[ItemNumber].JumpTab = true;
                            break;
                        default:
                            if (dr["JumpTab"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                            {
                                this._tbs[ItemNumber].Enabled = true;
                            }
                            break;
                    }
                }
                catch { this._tbs[ItemNumber].JumpTab = false; }

                //if (!this._tbs[iItemNumber].Enabled)
                //{
                //    //this._tbs[iItemNumber].BorderStyle = BorderStyle.None;
                //}

                // Location
                iRow = int.Parse((float.Parse(dr["Location_Row"].ToString()) * iRowPitch).ToString()) + iRowMargin;
                iColnum = int.Parse((float.Parse(dr["Location_Column"].ToString()) * iColnumPitch).ToString()) + iColnumMargin;

                //if (!this._tbs[iItemNumber].Enabled)
                //{
                //    // Font
                //    this._tbs[iItemNumber].Font = new System.Drawing.Font("ＭＳ ゴシック", fFontSize + 2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));

                //    // 調整
                //    switch (fFontSize.ToString())
                //    {
                //        case "12":
                //            iRow += 2;
                //            break;
                //        case "13":
                //        case "14":
                //        case "15":
                //            iRow += 3;
                //            break;
                //    }
                //}
                //else
                //{
                // Font
                this._tbs[ItemNumber].Font = new Font(Config.EntryTextBoxFont, _FONT_SIZE, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
                //}

                this._tbs[ItemNumber].Location = new Point(iColnum, iRow);

                // Visible
                switch (dr["Visible"].ToString().ToUpper())
                {
                    case "T":
                    default:
                        this._tbs[ItemNumber].Visible = true;
                        break;
                    case "F":
                        this._tbs[ItemNumber].Visible = false;
                        break;
                }

                if ("T".Equals(dr["TabStop"].ToString().ToUpper()))
                {
                    // TabStop
                    this._tbs[ItemNumber].TabStop = true;

                    // TabIndex
                    this._tbs[ItemNumber].TabIndex = int.Parse(dr["TabIndex"].ToString());
                }
                else
                {
                    // TabStop
                    this._tbs[ItemNumber].TabStop = false;

                    // TabIndex
                    if (dr["TabIndex"].ToString().Length != 0)
                    {
                        this._tbs[ItemNumber].TabIndex = int.Parse(dr["TabIndex"].ToString());
                    }
                    else
                    {
                        this._tbs[ItemNumber].TabIndex = 99999;
                    }
                }

                // MaxLength
                if (!string.IsNullOrEmpty(dr["Maxlength"].ToString()))
                {
                    this._tbs[ItemNumber].MaxLength = int.Parse(dr["Maxlength"].ToString());
                }

                //// MaxLengthReal
                //if (!string.IsNullOrEmpty(dr["Maxlength"].ToString()))
                //{
                //    this._tbs[iItemNumber].MaxLength = int.Parse(dr["MaxLength"].ToString()) + Config.iMaxLengthDiff;
                //    //this._tbs[iItemNumber].MaxLengthReal = int.Parse(dr["MaxLength"].ToString());
                //}
                // MinLength
                //try
                //{
                //    if (dr["MinLength"].ToString().Length != 0)
                //    {
                //        this._tbs[ItemNumber].MinLength = int.Parse(dr["Minlength"].ToString());
                //    }
                //    else
                //    {
                //        this._tbs[ItemNumber].MinLength = -1;
                //    }
                //}
                //catch { this._tbs[ItemNumber].MinLength = -1; }

                #region FullLength
                //this._tbs[iItemNumber].FullLength = false;
                try
                {
                    switch (dr["FullLength"].ToString().ToUpper())
                    {
                        case "T":
                            this._tbs[ItemNumber].FullLength = true;
                            break;
                        default:
                            if (dr["FullLength"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                                this._tbs[ItemNumber].FullLength = true;
                            break;
                    }
                }
                catch { }
                #endregion

                #region MailAddress
                try
                {
                    switch (dr["MailAddress"].ToString().ToUpper())
                    {
                        case "T":
                            this._tbs[ItemNumber].IsMailAddress = true;
                            break;
                        default:
                            if (dr["MailAddress"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                                this._tbs[ItemNumber].IsMailAddress = true;
                            break;
                    }
                }
                catch { }
                #endregion

                try
                {
                    switch (dr["DateFormat"].ToString().ToUpper())
                    {
                        case "T":
                            this._tbs[ItemNumber].DateFormat = "yyyyMMdd";
                            break;
                        default:
                            if (dr["DateFormat"].ToString().Length != 0)
                            {
                                this._tbs[ItemNumber].DateFormat = dr["DateFormat"].ToString();
                            }
                            break;
                    }
                }
                catch { }

                // Width
                if (!string.Empty.Equals(dr["Height"].ToString())
                    || !string.Empty.Equals(dr["Width"].ToString()))
                {
                    if (!string.Empty.Equals(dr["Height"].ToString()))
                    {
                        this._tbs[ItemNumber].Height = int.Parse(dr["Height"].ToString());
                    }
                    if (!string.Empty.Equals(dr["Width"].ToString()))
                    {
                        this._tbs[ItemNumber].Width = int.Parse(dr["Width"].ToString());
                    }
                }
                else
                {
                    // ラベルコントロールに最大長の文字列を設定し、幅を取得する
                    LabelDummy.Font = this._tbs[ItemNumber].Font;
                    if (Consts.InputMode.KanaFull.Equals(dr["InputMode"].ToString())
                        || Consts.InputMode.Hiragana.Equals(dr["InputMode"].ToString())
                        || Consts.InputMode.Full.Equals(dr["InputMode"].ToString()))
                    {
                        LabelDummy.Text = string.Empty.PadRight(this._tbs[ItemNumber].MaxLength, '＿');    // OCRから最大桁数以上のデータが入った場合に備えて、１文字分広げる必要がある
                    }
                    else
                    {
                        LabelDummy.Text = string.Empty.PadRight(this._tbs[ItemNumber].MaxLength, '*');    // OCRから最大桁数以上のデータが入った場合に備えて、１文字分広げる必要がある
                    }
                    this._tbs[ItemNumber].Width = LabelDummy.Width;
                }

                // MultiLine
                switch (dr["Multiline"].ToString().ToUpper())
                {
                    case "T":
                        this._tbs[ItemNumber].Multiline = true;
                        break;
                    case "F":
                    default:
                        this._tbs[ItemNumber].Multiline = false;
                        break;
                }

                // Required
                this._tbs[ItemNumber].IsRequired = false;
                switch (dr["Required"].ToString().ToUpper())
                {
                    case "T":
                        this._tbs[ItemNumber].IsRequired = true;
                        break;
                    default:
                        if (dr["Required"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                            this._tbs[ItemNumber].IsRequired = true;
                        break;
                }

                // ReadOnly
                this._tbs[ItemNumber].ReadOnly = false;
                switch (dr["ReadOnly"].ToString().ToUpper())
                {
                    case "T":
                        this._tbs[ItemNumber].ReadOnly = true;
                        break;
                    default:
                        if (dr["ReadOnly"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                            this._tbs[ItemNumber].ReadOnly = true;
                        break;
                }

                // TabStop
                switch (dr["TabStop"].ToString().ToUpper())
                {
                    case "T":
                    default:
                        this._tbs[ItemNumber].TabStop = true;
                        break;
                    case "F":
                        this._tbs[ItemNumber].TabStop = false;
                        break;
                }

                // Alignment
                switch (dr["Alignment"].ToString().ToUpper())
                {
                    case "L":
                    default:
                        this._tbs[ItemNumber].TextAlign = HorizontalAlignment.Left;
                        break;
                    case "R":
                        this._tbs[ItemNumber].TextAlign = HorizontalAlignment.Right;
                        break;
                    case "C":
                        this._tbs[ItemNumber].TextAlign = HorizontalAlignment.Center;
                        break;
                }

                // CharacterCasing
                this._tbs[ItemNumber].CharacterCasing = CharacterCasing.Normal;
                try
                {
                    switch (dr["CharacterCasing"].ToString().ToUpper())
                    {
                        case "U":
                            this._tbs[ItemNumber].CharacterCasing = CharacterCasing.Upper;
                            break;
                        case "L":
                            this._tbs[ItemNumber].CharacterCasing = CharacterCasing.Lower;
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                    // 古い定義ファイルに存在しない項目
                    this._tbs[ItemNumber].CharacterCasing = CharacterCasing.Normal;
                }

                try
                {
                    this._tbs[ItemNumber].SelectAllText = dr["TextAddMode"].ToString() == "T" ? false : true;
                    //this._tbs[iItemNumber].FullLength = dr["FullLength"].ToString() == "T" ? true : false;
                }
                catch
                {
                    // 古い定義ファイルに存在しない項目
                    this._tbs[ItemNumber].SelectAllText = true;
                    //this._tbs[iItemNumber].FullLength = false;
                }

                #region 条件付き必須
                try
                {
                    this._tbs[ItemNumber].Conditional_Required_Item = dr["Conditional_Required_Item"].ToString();
                    this._tbs[ItemNumber].Conditional_Required_Value = dr["Conditional_Required_Value"].ToString().Split(',');
                }
                catch
                {
                    this._tbs[ItemNumber].Conditional_Required_Item = string.Empty;
                    this._tbs[ItemNumber].Conditional_Required_Value = string.Empty.Split(',');
                }
                #endregion

                #region 個人番号
                try
                {
                    if ("T".Equals(dr["MyNumber3"].ToString().ToUpper()))
                    {
                        this._tbs[ItemNumber].MyNumber1 = dr["MyNumber1"].ToString();
                        this._tbs[ItemNumber].MyNumber2 = dr["MyNumber2"].ToString();
                    }
                    else
                    {
                        this._tbs[ItemNumber].MyNumber1 = string.Empty;
                        this._tbs[ItemNumber].MyNumber2 = string.Empty;
                    }
                }
                catch { }
                #endregion

                #region AutoRelease
                //this._tbs[iItemNumber].AutoRelease = false;
                try
                {
                    switch (dr["AutoRelease"].ToString().ToUpper())
                    {
                        case "T":
                            this._tbs[ItemNumber].AutoRelease = true;
                            break;
                        default:
                            if (dr["AutoRelease"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                                this._tbs[ItemNumber].AutoRelease = true;
                            break;
                    }
                }
                catch { }
                #endregion

                #region Square
                //this._tbs[iItemNumber].Square = false;
                try
                {
                    switch (dr["Square"].ToString().ToUpper())
                    {
                        case "T":
                            this._tbs[ItemNumber].Square = true;
                            break;
                        default:
                            if (dr["Square"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                                this._tbs[ItemNumber].Square = true;
                            break;
                    }
                }
                catch { }
                #endregion

                #region MasterCheck
                //this._tbs[iItemNumber].MasterCheck = false;
                try
                {
                    switch (dr["MasterCheck"].ToString().ToUpper())
                    {
                        case "T":
                            this._tbs[ItemNumber].MasterCheck = true;
                            break;
                        default:
                            if (dr["MasterCheck"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                                this._tbs[ItemNumber].MasterCheck = true;
                            break;
                    }
                }
                catch { }
                #endregion

                #region Range
                //this._tbs[iItemNumber].Range = string.Empty;
                try
                {
                    switch (dr["Range"].ToString().ToUpper())
                    {
                        case "L":
                        case "C":
                        case "R":
                            this._tbs[ItemNumber].Range = dr["Range"].ToString();
                            break;
                    }
                }
                catch { }
                #endregion

                // Add To GroupBox
                //for (int iIdx = 0; iIdx < this._gbs.Length; iIdx++)
                for (int iIdx = 0; iIdx <= this._gbs.Length - 1; iIdx++)
                {
                    if (("GroupBox_" + dr["GroupBox"].ToString()).Equals(_gbs[iIdx].Name))
                    {
                        this._gbs[iIdx].Controls.Add(this._tbs[ItemNumber]);
                    }
                }

                // InputMode
                this._tbs[ItemNumber].InputMode = dr["InputMode"].ToString().ToUpper();

                // ImeMode
                switch (dr["InputMode"].ToString().ToUpper())
                {
                    case Consts.InputMode.AlphabetNumeric:
                        this._tbs[ItemNumber].ImeMode = ImeMode.Disable;
                        break;
                    case Consts.InputMode.KanaHalf:
                        this._tbs[ItemNumber].ImeMode = ImeMode.KatakanaHalf;
                        break;
                    case Consts.InputMode.AllHalf:
                        if ("K".Equals(dr["DefaultIMEMode"].ToString().ToUpper()))
                            this._tbs[ItemNumber].DefaultImeMode = ImeMode.KatakanaHalf;
                        else
                            this._tbs[ItemNumber].DefaultImeMode = ImeMode.Off;
                        break;
                    case Consts.InputMode.KanaFull:
                        this._tbs[ItemNumber].ImeMode = ImeMode.Katakana;
                        break;
                    case Consts.InputMode.Hiragana:
                    case Consts.InputMode.Full:
                        this._tbs[ItemNumber].ImeMode = ImeMode.Hiragana;
                        break;
                }

                // ValidPattern
                this._tbs[ItemNumber].ValidPattern = GetValidPattern(dr["ValidPattern"].ToString(), _DOC_ID);

                // Tips
                this._tbs[ItemNumber].Tips = dr["Tips"].ToString();

                // ItemName
                this._tbs[ItemNumber].ItemName = this.GetLabelText(dr["ItemName"].ToString(), _DOC_ID);

                // Regex
                this._tbs[ItemNumber].Regex = dr["Regex"].ToString();

                #region DR
                this._tbs[ItemNumber].DR = string.Empty;
                try { this._tbs[ItemNumber].DR = dr["DR"].ToString().ToUpper(); }
                catch { }
                #endregion

                // Input2
                switch (dr["Input2"].ToString().ToUpper())
                {
                    case "T":
                    default:
                        this._tbs[ItemNumber].IsInput2 = true;
                        break;
                    case "F":
                        this._tbs[ItemNumber].IsInput2 = false;
                        break;
                }

                //this._extProps.Add(this._tbs[iItemNumber], new BPOEntryItemExtensionProperties(dr["InputMode"].ToString()
                //    , this._tbs[iItemNumber].MaxLength
                //    , bRequired: this._tbs[iItemNumber].IsRequired
                //    , bInput2: this._tbs[iItemNumber].IsInput2
                //    , bFullLength: this._tbs[iItemNumber].FullLength
                //     )
                //{
                //    ValidPattern = this._tbs[iItemNumber].ValidPattern
                //    ,
                //    AcceptKeyCharsPattern = this._tbs[iItemNumber].Regex
                //    ,
                //    ItemName = this._tbs[iItemNumber].ItemName
                //    ,
                //    Tips = this._tbs[iItemNumber].Tips
                //    ,
                //    CharacterCasing = this._tbs[iItemNumber].CharacterCasing
                //});

                // ショートカットキー
                //#if DEBUG
                this._tbs[ItemNumber].ShortcutsEnabled = true;
                //#else
                //                this._tbs[iItemNumber].ShortcutsEnabled = false;
                //#endif
                #region ２人目入力しない項目
                if (Consts.RecordKbn.Entry_2nd.Equals(this._RECORD_KBN))
                {
                    if (!this._tbs[ItemNumber].IsInput2)
                    {
                        if (this._tbs[ItemNumber].Enabled)
                        {
                            this._tbs[ItemNumber].Enabled = false;
                        }
                    }
                }
                #endregion
                //// 住所対応
                //if ("T".Equals(dr["IsPostCd"].ToString().ToUpper()))
                //{
                //    sAddressInfoList.Add(iItemNumber.ToString()
                //        + "," + dr["Address1"].ToString()
                //        + "," + dr["Address2"].ToString()
                //        + "," + dr["Address3"].ToString()
                //        + "," + dr["AddressKana1"].ToString()
                //        + "," + dr["AddressKana2"].ToString()
                //        + "," + dr["AddressKana3"].ToString()
                //        );
                //}

                //// 取扱者対応
                //if ("T".Equals(dr["IsAgencyCd"].ToString().ToUpper()))
                //{
                //    sHandlerInfoList.Add(iItemNumber.ToString() + "," + dr["HandlerCd"].ToString() + "," + dr["AgencyName"].ToString() + "," + dr["Handler_Last_name"].ToString() + "," + dr["Handler_First_name"].ToString());
                //}
                ItemNumber++;
            }

            //InitZipItems(sAddressInfoList);
            //InitHandlerItems(sHandlerInfoList);
            #endregion

            #region Label
            // Label
            drs = dtTxt.Select("ItemKind='L'");
            //if (drs.Length != 0)
            //    this._lbs = new Label[drs.Length];
            ItemNumber = 0;
            foreach (DataRow dr in drs)
            {
                this._lbs.Add(new System.Windows.Forms.Label());
                //this._lbs[iItemNumber] = new Label();

                // Name
                this._lbs[ItemNumber].Name = "label_" + (ItemNumber + 1).ToString("d3");

                // Text
                this._lbs[ItemNumber].Text = this.GetLabelText(dr["Text"].ToString(), _DOC_ID);

                // Location
                iRow = int.Parse((float.Parse(dr["Location_Row"].ToString()) * iRowPitch).ToString()) + iRowMargin;
                iColnum = int.Parse((float.Parse(dr["Location_Column"].ToString()) * iColnumPitch).ToString()) + iColnumMargin;
                this._lbs[ItemNumber].Location = new Point(iColnum, iRow);

                // Visible
                switch (dr["Visible"].ToString().ToUpper())
                {
                    case "T":
                    default:
                        this._lbs[ItemNumber].Visible = true;
                        break;
                    case "F":
                        this._lbs[ItemNumber].Visible = false;
                        break;
                }

                // Font
                this._lbs[ItemNumber].Font = new System.Drawing.Font("Meiryo UI", _FONT_SIZE, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));

                // AutoSize
                this._lbs[ItemNumber].AutoSize = true;

                // ForeColor
                switch (dr["ForeColor"].ToString().ToUpper())
                {
                    case "R":
                        this._lbs[ItemNumber].ForeColor = Color.Red;
                        break;
                    case "B":
                        this._lbs[ItemNumber].ForeColor = Color.Blue;
                        break;
                    case "Y":
                        this._lbs[ItemNumber].ForeColor = Color.Yellow;
                        break;
                    case "G":
                        this._lbs[ItemNumber].ForeColor = Color.Green;
                        break;
                }

                // 必須（赤）
                switch (dr["Required"].ToString().ToUpper())
                {
                    case "T":
                        this._lbs[ItemNumber].ForeColor = Color.Red;
                        break;
                    default:
                        if (dr["Required"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                            this._lbs[ItemNumber].ForeColor = Color.Red;
                        break;
                }

                // Add To GroupBox
                //for (int iIdx = 0; iIdx < this._gbs.Length; iIdx++)
                for (int iIdx = 0; iIdx <= this._gbs.Length - 1; iIdx++)
                {
                    if (("GroupBox_" + dr["GroupBox"].ToString()).Equals(_gbs[iIdx].Name))
                    {
                        this._gbs[iIdx].Controls.Add(this._lbs[ItemNumber]);
                    }
                }
                ItemNumber++;
            }
            #endregion
            // Add To Panel
            this.SuspendLayout();
            //for (int iIdx = 0; iIdx < this._gbs.Length; iIdx++)
            for (int iIdx = 0; iIdx <= this._gbs.Length - 1; iIdx++)
            {
                if (Consts.Flag.ON.Equals(this._gbs[iIdx].Tag.ToString()))
                {
                    this.PanelEntry.Controls.Add(this._gbs[iIdx]);
                }
            }
            this.PanelEntry.Visible = true;
            this.ResumeLayout();

            //20171109 add begin オプションの処理ファイルを読んで設定する
            //if (!(Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN) && "0".Equals(_IsVerifyMode))) //20180206 add 管理者で再入力画面表示する場合を除く
            if (!(Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN) && !_IsVerifyMode)) //20180206 add 管理者で再入力画面表示する場合を除く
            {
                var msg = GetOptionalPrpcessInfo();
                if (!string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show(msg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            //20171109 add end
            sw.Stop();
            _log.Info($"CreateForm.end:[経過時間{sw.Elapsed}]");
        }
        #endregion

        #region CreateFormAutoLayout
        /// <summary>
        /// 
        /// </summary>
        private void CreateFormAutoLayout()
        {
            _log.Info("CreateFormAutoLayout.start");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            //this.pnlEntry.Visible = false;
            this.TableLayoutPanel = new TableLayoutPanel();

            // 固定
            this._FONT_SIZE = 24F;

            this.SuspendLayout();

            // 設定ファイル読込み
            var dtTxt = GetEntryItemsSetting();
            //dtTxt.Columns.Add("ItemSeq");
            //int iItem_Seq = 0;
            //foreach (DataRow drTxt in dtTxt.Rows)
            //{
            //    iItem_Seq++;
            //    if (drTxt["TabIndex"].ToString().Length != 0)
            //    {
            //        drTxt["TabIndex"] = int.Parse(drTxt["TabIndex"].ToString()).ToString("d3");
            //    }
            //    drTxt["ItemSeq"] = iItem_Seq.ToString("d3");
            //}

            //// TabIndex,ItemSeqでソート
            //dtTxt = Utils.SortDataTable(dtTxt, "Tabindex ASC,ItemSeq ASC");

            #region GroupBox
            var drs = dtTxt.Select("ItemKind='G'");
            //if (drs.Length != 0)
            //    this._gbs = new GroupBox[drs.Length];
            //var ItemNumber = 0;

            this._gb = new GroupBox();

            // TabStop
            this._gb.TabStop = true;

            // TabIndex
            this._gb.TabIndex = 0;// int.Parse(dr["TabIndex"].ToString());

            // Name
            this._gb.Name = "GroupBoxAutoLayout";// + dr["ControlName"].ToString();

            // Text
            this._gb.Text = string.Empty;

            // Location
            this._gb.Location = new Point(20, 70);

            // Width
            this._gb.Width = 915;
            this._gb.Height = this.Height - 160;

            // Font
            this._gb.Font = new Font("Meiryo UI", _FONT_SIZE - 6, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));

            // tag
            this._gb.Tag = Consts.Flag.ON;

            if (IsMerpay)
            {
                this._gb.Visible = false;
            }
            #endregion

            #region TextBox
            // TextBox
            //var sAddressInfoList = new List<string>();
            //var sAddressInfoListArea = new List<string>();
            //var sHandlerInfoList = new List<string>();
            drs = dtTxt.Select("ItemKind='T'");
            this._tbs = new CTextBox.CTextBox[drs.Length];
            this._gbs = new GroupBox[drs.Length];
            //this._btns = new Button[drs.Length];
            this._pbs = new PictureBox[drs.Length];

            this.TableLayoutPanel.Left = 20;
            this.TableLayoutPanel.Top = 30;
            this.TableLayoutPanel.AutoScroll = true;
            this.TableLayoutPanel.ColumnCount = 1;
            this.TableLayoutPanel.Name = "tableLayoutPanel1";
            this.TableLayoutPanel.TabIndex = 1;
            this.TableLayoutPanel.Width = 880;
            this.TableLayoutPanel.Height = this._gb.Height - 45;

            this.TableLayoutPanel.RowCount = drs.Count();
            this.TableLayoutPanel.BackColor = SystemColors.ControlDark;
            this.TableLayoutPanel.Scroll += TableLayoutPanel_Scroll;

            #region TableLayoutPanelの横スクロールバー非表示化
            Padding p = TableLayoutPanel.Padding;
            p.Right = SystemInformation.VerticalScrollBarWidth;
            TableLayoutPanel.Padding = p;
            #endregion TableLayoutPanelの横スクロールバー非表示化

            // 全角
            var TextBoxFontWide = new Font(Config.EntryTextBoxFontN, _FONT_SIZE, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));

            // 半角
            var TextBoxFontNarrow = new Font(Config.EntryTextBoxFont, _FONT_SIZE, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));

            // グループボックス
            var GroupBoxFont = new Font("Meiryo UI", _FONT_SIZE - 8, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));

            //var po = new ParallelOptions();
            //po.MaxDegreeOfParallelism = ProcessorCount;
            //Parallel.ForEach(drs.AsEnumerable(), (dr,state, ItemNumber)  =>
            //   {
            //       _log.Debug($"Parallel index={ItemNumber}");

            //   //});

            var ItemNumber = 0;

            foreach (var dr in drs.AsEnumerable())
            {
                this._gbs[ItemNumber] = new GroupBox();
                this._tbs[ItemNumber] = new CTextBox.CTextBox();
                this._pbs[ItemNumber] = new PictureBox();

                this._pbs[ItemNumber].SizeMode = PictureBoxSizeMode.Zoom;
                this._pbs[ItemNumber].Name = "pb" + (ItemNumber + 1).ToString("000");

                // Enabled
                this._tbs[ItemNumber].Enabled = false;
                try
                {
                    switch (dr["Enabled"].ToString().ToUpper())
                    {
                        case "T":
                            this._tbs[ItemNumber].Enabled = true;
                            break;
                        case "F":
                            this._tbs[ItemNumber].Enabled = false;
                            break;
                        default:
                            // 一致する帳票IDが含まれる場合は活性化する
                            if (dr["Enabled"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                            {
                                this._tbs[ItemNumber].Enabled = true;
                            }
                            break;
                    }
                }
                catch { }

                //#region DNP FATF
                //if (Consts.BusinessID.DNP_FATF.Equals(String.Join("_", Config.TokuisakiCode, Config.HinmeiCode)))
                //{
                if (this._DUMMY_ITEM_FLAG)
                {
                    this._tbs[ItemNumber].IsDummyItem = !_tbs[ItemNumber].Enabled;
                }
                //}
                //#endregion

                this._gbs[ItemNumber].Location = new Point(0, 25);

                _gbs[ItemNumber].Tag = Consts.Flag.ON;
                _gbs[ItemNumber].TabStop = true;
                this._gbs[ItemNumber].Width = 850;
                this._gbs[ItemNumber].Height = -1;
                this._gbs[ItemNumber].Font = GroupBoxFont;// new System.Drawing.Font("Meiryo UI", _FONT_SIZE - 8, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));

                if (this._tbs[ItemNumber].Enabled)
                {
                    _log.Debug($"項目設定:{ItemNumber + 1}");

                    //    continue;
                    //}
#if DEBUG
                    this._gbs[ItemNumber].Text = (ItemNumber + 1).ToString("000_") + this.GetLabelText(dr["ItemName"].ToString(), _DOC_ID) + "_入力モード:" + dr["InputMode"].ToString() + "_" + dr["MAxLength"].ToString() + "文字";// dr["ItemName"].ToString();
#else
                    var ime = string.Empty;
                    switch (dr["InputMode"].ToString().ToUpper())
                    {
                        case Consts.InputMode.Hiragana:
                        case Consts.InputMode.Full:
                            ime = "全角";
                            break;
                        case Consts.InputMode.KanaFull:
                            ime = "全角カナ";
                            break;
                        case Consts.InputMode.KanaHalf:
                            ime = "半角ｶﾅ";
                            break;
                        case Consts.InputMode.AllHalf:
                        case Consts.InputMode.AlphabetNumeric:
                            ime = "半角";
                            break;
                        case Consts.InputMode.MixFull:
                            ime = "全半角混在";
                            break;
                    }
                    this._gbs[ItemNumber].Text = String.Format("{0}（{1} {2}文字）", this.GetLabelText(dr["ItemName"].ToString(), _DOC_ID), ime, dr["MaxLength"].ToString());// dr["ItemName"].ToString();
#endif
                    try
                    {
                        if (Consts.InputMode.Hiragana.Equals(dr["InputMode"].ToString().ToUpper())
                            || Consts.InputMode.Full.Equals(dr["InputMode"].ToString().ToUpper()))
                        {
                            dr["InputMode"] = Consts.InputMode.Hiragana;
                        }
                    }
                    catch { }

                    // 必須項目（赤）
                    try
                    {
                        switch (dr["Required"].ToString().ToUpper())
                        {
                            case "T":
                                this._gbs[ItemNumber].ForeColor = Color.Red;
                                //this._gbs[ItemNumber].Text += "（入力必須）";
                                break;
                            default:
                                if (dr["Required"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                                {
                                    this._gbs[ItemNumber].ForeColor = Color.Red;
                                    //this._gbs[ItemNumber].Text += "（入力必須）";
                                }
                                break;
                        }
                    }
                    catch { }

                    // Name
                    this._tbs[ItemNumber].Name = "text" + (ItemNumber + 1).ToString("d3");

                    // Text
                    try
                    {
                        if (dr["Text"].ToString().Length != 0)
                            this._tbs[ItemNumber].Text = dr["Text"].ToString();
                    }
                    catch { }

                    // Dummy
                    //this._tbs[iItemNumber].DummyItem = false;
                    try
                    {
                        switch (dr["DummyItem"].ToString().ToUpper())
                        {
                            case "T":
                                this._tbs[ItemNumber].IsDummyItem = true;
                                break;
                            default:
                                if (dr["DummyItem"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                                    this._tbs[ItemNumber].IsDummyItem = true;
                                break;
                        }
                    }
                    catch { }

                    try
                    {
                        switch (dr["JumpTab"].ToString().ToUpper())
                        {
                            case "T":
                                this._tbs[ItemNumber].JumpTab = true;
                                break;
                            default:
                                //                            this._tbs[ItemNumber].JumpStop = false;
                                if (dr["JumpTab"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                                {
                                    this._tbs[ItemNumber].JumpTab = true;
                                }
                                break;
                        }
                    }
                    catch { }

                    try
                    {
                        switch (dr["MailAddress"].ToString().ToUpper())
                        {
                            case "T":
                                this._tbs[ItemNumber].IsMailAddress = true;
                                break;
                            default:
                                if (dr["MailAddress"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                                {
                                    this._tbs[ItemNumber].IsMailAddress = true;
                                }
                                break;
                        }
                    }
                    catch { }

                    try
                    {
                        switch (dr["DateFormat"].ToString().ToUpper())
                        {
                            case "T":
                                this._tbs[ItemNumber].DateFormat = "yyyyMMdd";
                                break;
                            default:
                                if (dr["DateFormat"].ToString().Length != 0)
                                {
                                    this._tbs[ItemNumber].DateFormat = dr["DateFormat"].ToString();
                                }
                                break;
                        }
                    }
                    catch { }

                    // 修正画面への表示対象
                    this._tbs[ItemNumber].DisplayCorrect = this._tbs[ItemNumber].Enabled;

                    //if (!this._tbs[iItemNumber].Enabled)
                    //{
                    //    this._tbs[iItemNumber].BorderStyle = BorderStyle.None;
                    //}

                    // Font
                    //this._tbs[iItemNumber].Font = new System.Drawing.Font("ＭＳ ゴシック", fFontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
                    // this._tbs[ItemNumber].Font = new System.Drawing.Font(Config.sEntryTextBoxFont, _FONT_SIZE, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));

                    // Visible
                    //this._tbs[ItemNumber].Visible = true;
                    try
                    {
                        switch (dr["Visible"].ToString().ToUpper())
                        {
                            case "T":
                            default:
                                this._tbs[ItemNumber].Visible = true;
                                break;
                            case "F":
                                this._tbs[ItemNumber].Visible = false;
                                break;
                        }
                    }
                    catch { }

                    try
                    {
                        this._tbs[ItemNumber].TabStop = true;
                        this._tbs[ItemNumber].TabIndex = ItemNumber + 1;
                        if ("T".Equals(dr["TabStop"].ToString().ToUpper()))
                        {
                            // TabStop
                            this._tbs[ItemNumber].TabStop = true;

                            // TabIndex
                            this._tbs[ItemNumber].TabIndex = int.Parse(dr["TabIndex"].ToString());
                        }
                        else
                        {
                            // TabStop
                            this._tbs[ItemNumber].TabStop = false;

                            // TabIndex
                            if (dr["TabIndex"].ToString().Length != 0)
                            {
                                this._tbs[ItemNumber].TabIndex = int.Parse(dr["TabIndex"].ToString());
                            }
                            else
                            {
                                this._tbs[ItemNumber].TabIndex = 99999;
                            }
                        }
                    }
                    catch { }

                    // MaxLength
                    try
                    {
                        if (dr["Maxlength"].ToString().Length != 0)
                        {
                            this._tbs[ItemNumber].MaxLength = int.Parse(dr["Maxlength"].ToString());
                        }
                    }
                    catch { }

                    //// MaxLengthReal
                    //if (!string.IsNullOrEmpty(dr["Maxlength"].ToString()))
                    //{
                    //    this._tbs[iItemNumber].MaxLength = int.Parse(dr["MaxLength"].ToString()) + Config.iMaxLengthDiff;
                    //    //this._tbs[iItemNumber].MaxLengthReal = int.Parse(dr["MaxLength"].ToString());
                    //}

                    //// MinLength
                    //try
                    //{
                    //    if (!string.IsNullOrEmpty(dr["MinLength"].ToString()))
                    //    {
                    //        this._tbs[iItemNumber].MinLength = int.Parse(dr["Minlength"].ToString());
                    //    }
                    //    else
                    //    {
                    //        this._tbs[iItemNumber].MinLength = -1;
                    //    }
                    //}
                    //catch
                    //{
                    //    this._tbs[iItemNumber].MinLength = -1;
                    //}

                    // Width
                    // ラベルコントロールに最大長の文字列を設定し、幅を取得する
                    if (Consts.InputMode.KanaFull.Equals(dr["InputMode"].ToString())
                        || Consts.InputMode.Hiragana.Equals(dr["InputMode"].ToString())
                        || Consts.InputMode.Full.Equals(dr["InputMode"].ToString())
                        )
                    {
                        LabelDummy.Font = TextBoxFontWide;// new System.Drawing.Font(Config.sEntryTextBoxFontN, _FONT_SIZE, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
                    }
                    else
                    {
                        LabelDummy.Font = TextBoxFontNarrow;// new System.Drawing.Font(Config.sEntryTextBoxFont, _FONT_SIZE, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
                    }
                    if (Consts.InputMode.KanaFull.Equals(dr["InputMode"].ToString())
                        || Consts.InputMode.Hiragana.Equals(dr["InputMode"].ToString())
                        || Consts.InputMode.Full.Equals(dr["InputMode"].ToString()))
                    {
                        const char c = '＿';
                        if (_tbs[ItemNumber].MaxLength > 250)
                        {
                            this._tbs[ItemNumber].Multiline = true;
                            LabelDummy.Text = string.Empty.PadRight(25, c);
                            this._tbs[ItemNumber].Width = LabelDummy.Width + 19;
                            this._tbs[ItemNumber].Height = 31 * (10);
                            this._tbs[ItemNumber].ScrollBars = ScrollBars.Vertical;
                        }
                        else if (_tbs[ItemNumber].MaxLength > 25)
                        {
                            this._tbs[ItemNumber].Multiline = true;
                            LabelDummy.Text = string.Empty.PadRight(25, c);
                            this._tbs[ItemNumber].Width = LabelDummy.Width + 3;
                            if (_tbs[ItemNumber].MaxLength % 25 == 0)
                                this._tbs[ItemNumber].Height = 33 * (_tbs[ItemNumber].MaxLength / 25) + 10;
                            else
                                this._tbs[ItemNumber].Height = 33 * (_tbs[ItemNumber].MaxLength / 25 + 1) + 10;
                        }
                        else
                        {
                            LabelDummy.Text = string.Empty.PadRight(int.Parse(dr["MaxLength"].ToString()), c);
                            this._tbs[ItemNumber].Width = LabelDummy.Width + 0;
                        }
                        this._tbs[ItemNumber].Font = TextBoxFontWide;//  new System.Drawing.Font(Config.sEntryTextBoxFontN, _FONT_SIZE, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
                    }
                    else
                    {
                        const char c = '*';
                        if (_tbs[ItemNumber].MaxLength > 50)
                        {
                            this._tbs[ItemNumber].Multiline = true;
                            LabelDummy.Text = string.Empty.PadRight(50, c);
                            this._tbs[ItemNumber].Width = LabelDummy.Width + 3;
                            if (_tbs[ItemNumber].MaxLength % 50 == 0)
                                this._tbs[ItemNumber].Height = 33 * (_tbs[ItemNumber].MaxLength / 50) + 10;
                            else
                                this._tbs[ItemNumber].Height = 33 * (_tbs[ItemNumber].MaxLength / 50 + 1) + 10;
                        }
                        else
                        {
                            LabelDummy.Text = string.Empty.PadRight(int.Parse(dr["MaxLength"].ToString()), c);
                            this._tbs[ItemNumber].Width = LabelDummy.Width + 0;
                        }
                        this._tbs[ItemNumber].Font = TextBoxFontNarrow;// new System.Drawing.Font(Config.sEntryTextBoxFont, _FONT_SIZE, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
                    }

                    // 必須入力
                    try
                    {
                        switch (dr["Required"].ToString().ToUpper())
                        {
                            case "T":
                                this._tbs[ItemNumber].IsRequired = true;
                                break;
                            default:
                                // 一致する帳票IDが含まれる場合は活性化する
                                if (dr["Required"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                                    this._tbs[ItemNumber].IsRequired = true;
                                break;
                        }
                    }
                    catch { }

                    // ReadOnly
                    //this._tbs[ItemNumber].ReadOnly = false;
                    try
                    {
                        switch (dr["ReadOnly"].ToString().ToUpper())
                        {
                            case "T":
                                this._tbs[ItemNumber].ReadOnly = true;
                                break;
                            default:
                                if (dr["ReadOnly"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                                    this._tbs[ItemNumber].ReadOnly = true;
                                break;
                        }
                    }
                    catch { }

                    // TabStop
                    //this._tbs[ItemNumber].TabStop = true;
                    //try
                    //{
                    //    switch (dr["TabStop"].ToString().ToUpper())
                    //    {
                    //        case "T":
                    //        default:
                    //            this._tbs[ItemNumber].TabStop = true;
                    //            break;
                    //        case "F":
                    //            this._tbs[ItemNumber].TabStop = false;
                    //            break;
                    //    }
                    //}
                    //catch { }

                    // 文字寄せ
                    //this._tbs[ItemNumber].TextAlign = HorizontalAlignment.Left;
                    try
                    {
                        switch (dr["Alignment"].ToString().ToUpper())
                        {
                            case "L":
                            default:
                                this._tbs[ItemNumber].TextAlign = HorizontalAlignment.Left;
                                break;
                            case "R":
                                this._tbs[ItemNumber].TextAlign = HorizontalAlignment.Right;
                                break;
                            case "C":
                                this._tbs[ItemNumber].TextAlign = HorizontalAlignment.Center;
                                break;
                        }
                    }
                    catch { }

                    // CharacterCasing
                    //this._tbs[ItemNumber].CharacterCasing = CharacterCasing.Normal;
                    try
                    {
                        switch (dr["CharacterCasing"].ToString().ToUpper())
                        {
                            case "U":
                                this._tbs[ItemNumber].CharacterCasing = CharacterCasing.Upper;
                                break;
                            case "L":
                                this._tbs[ItemNumber].CharacterCasing = CharacterCasing.Lower;
                                break;
                            default:
                                break;
                        }
                    }
                    catch { }
                    //{
                    //    //// 古い定義ファイルに存在しない項目
                    //    //this._tbs[iItemNumber].CharacterCasing = CharacterCasing.Normal;
                    //}

                    // 追加入力
                    //this._tbs[iItemNumber].SelectAllText = true;
                    try { this._tbs[ItemNumber].SelectAllText = !"T".Equals(dr["TextAddMode"].ToString().ToUpper()); } catch { }

                    //// フル桁入力
                    //this._tbs[iItemNumber].FullLength = false;
                    //try
                    //{
                    //    //this._tbs[iItemNumber].SelectAllText = "T".Equals(dr["TextAddMode"].ToString().ToUpper()) ? false : true;
                    //    this._tbs[iItemNumber].FullLength = "T".Equals(dr["FullLength"].ToString().ToUpper()) ? true : false;
                    //}
                    //catch { }
                    //{
                    //    // 古い定義ファイルに存在しない項目
                    //    this._tbs[iItemNumber].SelectAllText = true;
                    //    this._tbs[iItemNumber].FullLength = false;
                    //}

                    #region 条件付き必須
                    try
                    {
                        this._tbs[ItemNumber].Conditional_Required_Item = dr["Conditional_Required_Item"].ToString();
                        this._tbs[ItemNumber].Conditional_Required_Value = dr["Conditional_Required_Value"].ToString().Split(',');
                        this._tbs[ItemNumber].Conditional_Required_Omit_Value = dr["Conditional_Required_Omit_Value"].ToString().Split(',');
                    }
                    catch
                    {
                        this._tbs[ItemNumber].Conditional_Required_Item = string.Empty;
                        this._tbs[ItemNumber].Conditional_Required_Value = string.Empty.Split(',');
                        this._tbs[ItemNumber].Conditional_Required_Omit_Value = string.Empty.Split(',');
                    }
                    #endregion

                    #region 個人番号
                    try
                    {
                        if ("T".Equals(dr["MyNumber3"].ToString().ToUpper()))
                        {
                            this._tbs[ItemNumber].MyNumber1 = dr["MyNumber1"].ToString();
                            this._tbs[ItemNumber].MyNumber2 = dr["MyNumber2"].ToString();
                        }
                        //else
                        //{
                        //    this._tbs[iItemNumber].MyNumber1 = string.Empty;
                        //    this._tbs[iItemNumber].MyNumber2 = string.Empty;
                        //}
                    }
                    catch { }
                    //{
                    //    this._tbs[iItemNumber].MyNumber1 = string.Empty;
                    //    this._tbs[iItemNumber].MyNumber2 = string.Empty;
                    //}
                    #endregion

                    #region AutoRelease
                    try
                    {
                        switch (dr["AutoRelease"].ToString().ToUpper())
                        {
                            case "T":
                                this._tbs[ItemNumber].AutoRelease = true;
                                break;
                            default:
                                if (dr["AutoRelease"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                                    this._tbs[ItemNumber].AutoRelease = true;
                                break;
                        }
                    }
                    catch { }
                    #endregion

                    #region Square
                    try
                    {
                        switch (dr["Square"].ToString().ToUpper())
                        {
                            case "T":
                                this._tbs[ItemNumber].Square = true;
                                break;
                            default:
                                if (dr["Square"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                                    this._tbs[ItemNumber].Square = true;
                                break;
                        }
                    }
                    catch { }
                    #endregion

                    #region FullLength
                    try
                    {
                        switch (dr["FullLength"].ToString().ToUpper())
                        {
                            case "T":
                                this._tbs[ItemNumber].FullLength = true;
                                break;
                            default:
                                if (dr["FullLength"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                                    this._tbs[ItemNumber].FullLength = true;
                                break;
                        }
                    }
                    catch { }
                    #endregion

                    #region MasterCheck
                    try
                    {
                        switch (dr["MasterCheck"].ToString().ToUpper())
                        {
                            case "T":
                                this._tbs[ItemNumber].MasterCheck = true;
                                break;
                            default:
                                if (dr["MasterCheck"].ToString().ToUpper().Split(',').Contains(_DOC_ID))
                                    this._tbs[ItemNumber].MasterCheck = true;
                                break;
                        }
                    }
                    catch { }
                    #endregion

                    #region Range
                    //this._tbs[ItemNumber].Range = string.Empty;
                    try
                    {
                        switch (dr["Range"].ToString().ToUpper())
                        {
                            case "L":
                            case "C":
                            case "R":
                                this._tbs[ItemNumber].Range = dr["Range"].ToString();
                                break;
                        }
                    }
                    catch { }
                    #endregion

                    #region ControlAlign
                    //this._tbs[ItemNumber].ControlAlign = "L";
                    try
                    {
                        if ("C".Equals(dr["ControlAlign"].ToString().ToUpper())
                            || "R".Equals(dr["ControlAlign"].ToString().ToUpper()))
                            this._tbs[ItemNumber].ControlAlign = dr["ControlAlign"].ToString().ToUpper();
                    }
                    catch { }
                    #endregion

                    // InputMode
                    this._tbs[ItemNumber].InputMode = dr["InputMode"].ToString();

                    // ImeMode
                    switch (dr["InputMode"].ToString().ToUpper())
                    {
                        case Consts.InputMode.AlphabetNumeric:
                            this._tbs[ItemNumber].ImeMode = ImeMode.Disable;
                            break;
                        case Consts.InputMode.KanaHalf:
                            this._tbs[ItemNumber].ImeMode = ImeMode.KatakanaHalf;
                            break;
                        case Consts.InputMode.KanaFull:
                            this._tbs[ItemNumber].ImeMode = ImeMode.Katakana;
                            break;
                        case Consts.InputMode.Hiragana:
                        case Consts.InputMode.Full:
                            this._tbs[ItemNumber].ImeMode = ImeMode.Hiragana;
                            break;
                        case Consts.InputMode.AllHalf:
                            if ("K".Equals(dr["DefaultIMEMode"].ToString().ToUpper()))
                                this._tbs[ItemNumber].DefaultImeMode = ImeMode.KatakanaHalf;
                            break;
                    }

                    // ValidPattern
                    try { this._tbs[ItemNumber].ValidPattern = GetValidPattern(dr["ValidPattern"].ToString(), _DOC_ID); } catch { }

                    // Tips
                    try { this._tbs[ItemNumber].Tips = GetTips(dr["Tips"].ToString(), _DOC_ID); } catch { }

                    // ItemName
                    this._tbs[ItemNumber].ItemName = this.GetLabelText(dr["ItemName"].ToString(), _DOC_ID); //dr["ItemName"].ToString();

                    // Regex
                    try { this._tbs[ItemNumber].Regex = dr["Regex"].ToString(); } catch { }

                    // Input2
                    try
                    {
                        switch (dr["Input2"].ToString().ToUpper())
                        {
                            case "T":
                            default:
                                this._tbs[ItemNumber].IsInput2 = true;
                                break;
                            case "F":
                                this._tbs[ItemNumber].IsInput2 = false;
                                break;
                        }
                    }
                    catch { }

                    //this._tbs[iItemNumber].OcrEntryTarget = false;
                    ////try
                    ////{
                    //    switch (dr["Enabled"].ToString().ToUpper())
                    //    {
                    //        case "T":
                    //            this._tbs[iItemNumber].OcrEntryTarget = true;
                    //            break;
                    //        default:
                    //            if (dr["Enabled"].ToString().ToUpper().Split(',').Contains(_sDocId))
                    //                this._tbs[iItemNumber].OcrEntryTarget = true;
                    //            break;
                    //    }
                    //}
                    //catch
                    //{
                    //    this._tbs[iItemNumber].OcrEntryTarget = true;
                    //}

                    //this._tbs[iItemNumber].AutoRelease = false;
                    //try
                    //{
                    //    switch (dr["AutoRelease"].ToString().ToUpper())
                    //    {
                    //        case "T":
                    //            this._tbs[iItemNumber].AutoRelease = true;
                    //            break;
                    //        default:
                    //            if (dr["AutoRelease"].ToString().ToUpper().Split(',').Contains(_sDocId))
                    //                this._tbs[iItemNumber].AutoRelease = true;
                    //            break;
                    //    }
                    //}
                    //catch
                    //{
                    //}

                    //this._extProps.Add(this._tbs[ItemNumber], new BPOEntryItemExtensionProperties(dr["InputMode"].ToString()
                    //    , this._tbs[ItemNumber].MaxLength
                    //    , bRequired: this._tbs[ItemNumber].IsRequired
                    //    , bInput2: this._tbs[ItemNumber].IsInput2
                    //    , bFullLength: this._tbs[ItemNumber].FullLength
                    //     )
                    //{
                    //    ValidPattern = this._tbs[ItemNumber].ValidPattern,
                    //    AcceptKeyCharsPattern = this._tbs[ItemNumber].Regex,
                    //    ItemName = this._tbs[ItemNumber].ItemName,
                    //    //Tips = this._tbs[ItemNumber].Tips,
                    //    //CharacterCasing = this._tbs[ItemNumber].CharacterCasing
                    //});

                    // ショートカットキー
                    //#if DEBUG
                    this._tbs[ItemNumber].ShortcutsEnabled = true;
                    //#else
                    //                this._tbs[iItemNumber].ShortcutsEnabled = false;
                    //#endif

                    #region ２人目入力しない項目
                    if (Consts.RecordKbn.Entry_2nd.Equals(this._RECORD_KBN))
                    {
                        if (!this._tbs[ItemNumber].IsInput2)
                        {
                            if (this._tbs[ItemNumber].Enabled)
                            {
                                this._tbs[ItemNumber].Enabled = false;
                            }
                        }
                    }
                    #endregion

                    #region DR
                    this._tbs[ItemNumber].DR = string.Empty;
                    try { this._tbs[ItemNumber].DR = dr["DR"].ToString().ToUpper(); }
                    catch { }
                    #endregion

                }
                //this._gbs[iItemNumber].Controls.Add(this._tbs[iItemNumber]);
                this._gbs[ItemNumber].Controls.Add(this._pbs[ItemNumber]);

                this.TableLayoutPanel.RowStyles.Add(new RowStyle());
                this.TableLayoutPanel.Controls.Add(_gbs[ItemNumber], 0, ItemNumber);
                this.TableLayoutPanel.RowStyles[ItemNumber] = new RowStyle(SizeType.Absolute, 0.0f);
                ItemNumber++;
            }
            //});

            _log.Debug("TextBox 設定終了");
            dtTxt.Columns.Add("ItemSeq");
            int iItem_Seq = 0;
            foreach (var drTxt in dtTxt.Select("ItemKind='T'").AsEnumerable())
            {
                iItem_Seq++;
                if (drTxt["TabIndex"].ToString().Length != 0)
                {
                    drTxt["TabIndex"] = int.Parse(drTxt["TabIndex"].ToString()).ToString("d3");
                }
                drTxt["ItemSeq"] = iItem_Seq.ToString("d3");
            }
            _log.Debug("ItemSeq 設定終了");

            // TabIndex,ItemSeqでソート
            dtTxt = Utils.SortDataTable(dtTxt, "Tabindex ASC,ItemSeq ASC");

            int iItemSeq = 0;
            foreach (var drTxt in dtTxt.Select("ItemKind='T'").AsEnumerable())
            {
                this._gbs[iItemSeq].Controls.Add(this._tbs[int.Parse(drTxt["ItemSeq"].ToString()) - 1]);
                this._gbs[iItemSeq].Tag = (int.Parse(drTxt["ItemSeq"].ToString()) - 1).ToString();

                //if (iItemSeq == 10)
                //{
                //    this._btns[iItemSeq] = new Button();//
                //    this._btns[iItemSeq].Text = "住所クリア";
                //    this._btns[iItemSeq].TabStop = false;
                //    this._btns[iItemSeq].BackColor = System.Drawing.SystemColors.Control;
                //    this._btns[iItemSeq].Width = 180;
                //    this._btns[iItemSeq].Height = 40;
                //    this._gbs[iItemSeq].Controls.Add(this._btns[iItemSeq]);
                //}
                iItemSeq++;
            }
            _log.Debug("ItemSeqSort 終了");
            #endregion

            // Add To Panel
            this._gb.SuspendLayout();
            this.TableLayoutPanel.SuspendLayout();
            this._gb.Controls.Add(this.TableLayoutPanel);
            _log.Debug("_gb.Controls.Add 終了");
            this.Controls.Add(this._gb);
            this._gb.ResumeLayout();
            this.TableLayoutPanel.ResumeLayout();
            _log.Debug("Controls.Add 終了");

            //20171109 add begin オプションの処理ファイルを読んで設定する
            if (!(Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN) && !_IsVerifyMode))
            {
                var msg = GetOptionalPrpcessInfo();
                if (!string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show(msg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            //20171109 add end

            this.ResumeLayout();

            sw.Stop();
            _log.Info($"CreateFormAutoLayout.end:[経過時間{sw.Elapsed}]");
        }
        #endregion

        /// <summary>
        /// カットイメージ座標取得
        /// </summary>
        /// <param name="imageFileName"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetCutImageCoordinate(string imageFileName)
        {
            _log.Info("GetCutImageCoordinate.start");
            var swSelect = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var dic = new Dictionary<string, string>();
                var dt = _dao.SELECT_READING_PARTS_FILE_NAME(imageFileName);
                if (dt.Rows.Count != 0)
                {
                    // イメージ毎のポジション
                    foreach (var dr in dt.AsEnumerable())
                    {
                        dic[int.Parse(dr["COLUNM_NO"].ToString()).ToString("d3")] = String.Join(","
                                                                                               , dr["x"].ToString()
                                                                                               , dr["y"].ToString()
                                                                                               , dr["width"].ToString()
                                                                                               , dr["height"].ToString());
                    }
                }
                else
                {
                    // テンプレートのポジション
                    foreach (var dr in _dao.SELECT_READING_PARTS_TEMPLATE(_DOC_ID).AsEnumerable())
                    {
                        dic[int.Parse(dr["COLUNM_NO"].ToString()).ToString("d3")] = String.Join(","
                                                                                                , (int.Parse(dr["x"].ToString()) - _MARGIN_X).ToString()
                                                                                                , (int.Parse(dr["y"].ToString()) - _MARGIN_Y).ToString()
                                                                                                , (int.Parse(dr["width"].ToString()) + _MARGIN_X * 2).ToString()
                                                                                                , (int.Parse(dr["height"].ToString()) + _MARGIN_Y * 2).ToString());
                    }
                }
                return dic;
            }
            finally
            {
                swSelect.Stop();
                _log.Info($"GetCutImageCoordinate:[経過時間:{ swSelect.Elapsed}]");
            }
        }

        /// <summary>
        /// エントリ項目表示
        /// </summary>
        /// <param name="ocrImageFileName"></param>
        private void ShowEntryItem(string ocrImageFileName)
        {
            _log.Info("ShowEntryItem.start");
            var sw = System.Diagnostics.Stopwatch.StartNew();

            // カットイメージポジション取得
            if (dict == null)
            {
                dict = GetCutImageCoordinate(ocrImageFileName);
            }

            //newTableLayoutPanel.SuspendLayout();
            //CurrentImage = new Bitmap(dtD_IMAGE_INFO.Rows[_iProcessingCount - 1]["IMAGE_PATH"].ToString());
            //if (File.Exists(dtD_IMAGE_INFO.Rows[_iProcessingCount - 1]["IMAGE_PATH"].ToString()))
            //    _CurrentImage = Image.FromStream(System.IO.File.OpenRead(dtD_IMAGE_INFO.Rows[_iProcessingCount - 1]["IMAGE_PATH"].ToString()), false, false);

            const float fCutImageZoom = 0.66666f;

            const int iControlLeft = 10;

            var iGroupBoxIdx = 0;
            var iPositionInner = 0;
            var iTextBoxleft = iControlLeft;
            //            int iTotalPositionInner = 0;

            var iX = 0;       // イメージ座標:x
            var iY = 0;       // イメージ座標:y
            var iWidth = 0;   // イメージ幅
            var iHeight = 0;  // イメージ高

            TableLayoutPanel.SuspendLayout();

            foreach (var tb in this._gbs)
            {
                // tableLayoutPaneの行スタイル設定
                if (this._tbs[iGroupBoxIdx].DisplayCorrect)
                {
                    if (dict.Count != 0
                        && dict.ContainsKey(this._pbs[iGroupBoxIdx].Name.Substring(2, 3)))
                    {
                        var x = dict[this._pbs[iGroupBoxIdx].Name.Substring(2, 3)].Split(',');
                        iX = int.Parse(x[0].Trim());// + _iMarginX;
                        iY = int.Parse(x[1].Trim());// + _iMarginY;
                        iWidth = int.Parse(x[2].Trim());
                        iHeight = int.Parse(x[3].Trim());
                    }
                    else
                    {
                        iX = 1;
                        iY = 1;
                        iWidth = 0;
                        iHeight = 0;
                    }

                    // カットイメージ位置
                    iPositionInner = 35;

                    this._pbs[iGroupBoxIdx].Location = new Point(iControlLeft, iPositionInner);

                    // カットイメージサイズ
                    if (iWidth < 100 && iHeight < 100)
                    {
                        // そのまま表示
                        this._pbs[iGroupBoxIdx].Size = new Size(Convert.ToInt32(Math.Floor((double)iWidth)), Convert.ToInt32(Math.Floor((double)iHeight)));
                    }
                    else
                    {
                        double dWidth = 0; double dHeight = 0;
                        if (iWidth > 1250)
                        {
                            // 表示しきれない画像を縦横比を保持したまま縮小表示
                            double dZoom = double.Parse(iWidth.ToString()) / 1250;
                            dWidth = iWidth / dZoom;
                            dHeight = iHeight / dZoom;
                        }
                        else
                        {
                            // そのままの倍率で表示
                            dWidth = double.Parse(iWidth.ToString());
                            dHeight = double.Parse(iHeight.ToString());
                        }
                        this._pbs[iGroupBoxIdx].Size = new Size(Convert.ToInt32(Math.Floor(dWidth * fCutImageZoom)), Convert.ToInt32(Math.Floor(dHeight * fCutImageZoom)));
                    }

                    //描画先とするImageオブジェクトを作成する
                    if (this._pbs[iGroupBoxIdx].Width < 1)
                    {
                        this._pbs[iGroupBoxIdx].Width = 1;
                    }
                    if (this._pbs[iGroupBoxIdx].Height < 1)
                    {
                        this._pbs[iGroupBoxIdx].Height = 1;
                    }

                    //if (_CurrentImage != null)
                    if (this._ImgForm._CurrentImage != null)
                    {
                        if (this._pbs[iGroupBoxIdx].Width != 1 && this._pbs[iGroupBoxIdx].Height != 1)
                        {
                            var canvas = new Bitmap(this._pbs[iGroupBoxIdx].Width, this._pbs[iGroupBoxIdx].Height);

                            //ImageオブジェクトのGraphicsオブジェクトを作成する
                            var g = Graphics.FromImage(canvas);

                            // カット範囲
                            var srcRect = new Rectangle(iX, iY, iWidth, iHeight);

                            // 描画範囲
                            var desRect = new Rectangle(0, 0, this._pbs[iGroupBoxIdx].Width, this._pbs[iGroupBoxIdx].Height);

                            // 描画
                            g.DrawImage(this._ImgForm._CurrentImage, desRect, srcRect, GraphicsUnit.Pixel);

                            // Graphics解放する
                            g.Dispose();

                            //PictureBoxに表示する
                            this._pbs[iGroupBoxIdx].Image = canvas;

                            iPositionInner += this._pbs[iGroupBoxIdx].Height + 5;
                        }
                    }

                    int iTextBoxIdx = int.Parse(this._gbs[iGroupBoxIdx].Tag.ToString());

                    switch (this._tbs[iTextBoxIdx].ControlAlign)
                    {
                        case "R":
                            // テキストボックス位置（イメージ右辺合わせ）
                            iTextBoxleft = iControlLeft + this._pbs[iGroupBoxIdx].Width - this._tbs[iTextBoxIdx].Width;
                            break;
                        case "C":
                            // テキストボックス位置（イメージ中央合わせ）
                            iTextBoxleft = iControlLeft + this._pbs[iGroupBoxIdx].Width / 2 - this._tbs[iTextBoxIdx].Width / 2;
                            break;
                        default:
                            // テキストボックス位置（イメージ左辺合わせ）
                            iTextBoxleft = iControlLeft;
                            break;
                    }

                    this._tbs[iTextBoxIdx].Location = new Point(iTextBoxleft < iControlLeft ? iControlLeft : iTextBoxleft, iPositionInner);

                    // 入力項目イメージ位置
                    this._tbs[iTextBoxIdx].ImagePosition = String.Join(",", iX.ToString(), iY.ToString(), iWidth.ToString(), iHeight.ToString());

                    // ポジション調整
                    iPositionInner += (this._tbs[iTextBoxIdx].Height + 10);

                    // Height
                    this._gbs[iGroupBoxIdx].Height = iPositionInner;

                    //TableLayoutPanel.RowStyles[iGroupBoxIdx] = new RowStyle(SizeType.Absolute, float.Parse(_gbs[iGroupBoxIdx].Height.ToString()) + 2.0f);
                    TableLayoutPanel.RowStyles[iGroupBoxIdx] = new RowStyle(SizeType.Absolute, _gbs[iGroupBoxIdx].Height + 8.0f);

                    if (this._tbs[iGroupBoxIdx].IsDummyItem)
                    {
                        this._tbs[iGroupBoxIdx].ReadOnly = true;
                    }

                    //if (_btns[iGroupBoxIdx] != null)
                    //{
                    //    _btns[iGroupBoxIdx].Top = _tbs[iGroupBoxIdx].Top;
                    //    _btns[iGroupBoxIdx].Left = _tbs[iGroupBoxIdx].Left + _tbs[iGroupBoxIdx].Width + 24;
                    //}
                }
                else
                {
                    TableLayoutPanel.RowStyles[iGroupBoxIdx] = new RowStyle(SizeType.Absolute, 0.0f);
                }

                iGroupBoxIdx++;
            }

            //TableLayoutPanelReflesh();

            //#region 項目数が多いとレイアウトが崩れる
            //this.TableLayoutPanel.VerticalScroll.Value = this.TableLayoutPanel.VerticalScroll.Maximum;
            //int idx = 0;
            //for (idx = this._tbs.Length - 1; idx >= 0; idx--)
            //{
            //    if (this._tbs[idx].Enabled)
            //        break;
            //}
            //this._tbs[idx].Focus(); this._tbs[idx].SelectAll();
            //TableLayoutPanelReflesh();
            //this.TableLayoutPanel.VerticalScroll.Value = 0;

            TableLayoutPanel.ResumeLayout(true);
            //Application.DoEvents();

            //#endregion

            sw.Stop();
            _log.Debug($"ShowEntryItem.end:[経過時間{sw.Elapsed}]");
        }

        /// <summary>
        /// 
        /// </summary>
        private void TableLayoutPanelReflesh()
        {
            TableLayoutPanel.SuspendLayout();
            for (int iIdx = 0; iIdx <= _tbs.Length - 1; iIdx++)
            {
                if (_tbs[iIdx].DisplayCorrect)
                {
                    TableLayoutPanel.RowStyles[iIdx] = new RowStyle(SizeType.Absolute, _gbs[iIdx].Height + 8.0f);
                }
                else
                {
                    TableLayoutPanel.RowStyles[iIdx] = new RowStyle(SizeType.Absolute, 0.0f);
                }
            }
            TableLayoutPanel.ResumeLayout(true);
            Application.DoEvents();
        }

        //------------------------------------------------------------------20171109 add
        //opdionのメソッドを呼び出す
        private void InvokeOptionalMethod(OptionalProcessInfo info)
        {
            try
            {
                //foreach (OptionalProcessInfo info in infos)
                //{
                //メソッド呼び出し
                Type OpProcessType = typeof(OptionalProcess);
                MethodInfo mi = OpProcessType.GetMethod(info.MethodName);
                if (mi == null) { return; }

                ParameterInfo[] pi = mi.GetParameters();

                //各メソッドに渡すパラメータの準備
                var args = new List<object>();
                var orderinfo = info.Items.OrderBy(a => a.ParameterOrder).ToList();
                foreach (int i in Enumerable.Range(0, orderinfo.Count()))
                {
                    //引数がstring型なら
                    if (pi[i].ParameterType.GetElementType() == typeof(string))
                    {
                        args.Add(orderinfo[i].GetText());
                    }
                    else
                    {
                        args.Add(orderinfo[i].GetArg());
                    }
                }

                if (pi.Count() != args.Count) { return; }

                object[] args2 = args.ToArray();

                //テキストにセットする値はrefで戻す
                bool ret = (bool)mi.Invoke(this, args2);
                if (!ret) { return; }

                foreach (int i in Enumerable.Range(0, args2.Count()))
                {
                    if (args2[i] != null)
                    {
                        //引数がstring型なら
                        if (pi[i].ParameterType.GetElementType() == typeof(string))
                        {
                            orderinfo[i].SetText((string)args2[i]);
                        }
                        else
                        {
                            orderinfo[i].SetText((OpArg)args2[i]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
        }

        private string GetOptionalPrpcessInfo()
        {
            var message = string.Empty;
            try
            {
                var filepath = Path.Combine(Application.StartupPath, String.Format(@"Items\{0}\{1}_{2}_{3}_option.txt"
                                                                                  , Config.UserId
                                                                                  , Config.TokuisakiCode
                                                                                  , Config.HinmeiCode
                                                                                  , _DOC_ID_ENTRY.Length == 0 ? _DOC_ID : _DOC_ID_ENTRY));
                if (!File.Exists(filepath)) { return message; }

                using (var reader = new TSVReader<OptionalTsvFile>(filepath, true))
                {
                    foreach (var line in reader)
                    {
                        #region ターゲット帳票判定
                        if (line.TargetDocId.Length != 0)
                        {
                            var DocIdList = line.TargetDocId.Replace("*", string.Empty).Split(',');
                            var b = false;
                            foreach (var s in DocIdList)
                            {
                                if (s.Length != 0 && s.Equals(_DOC_ID.Substring(0, s.Length)))
                                {
                                    b = true;
                                }
                            }
                            if (!b)
                            {
                                continue;
                            }
                        }
                        #endregion

                        //ファイル内容のチェック
                        if (!OptionalProcessInfo.Check(this._tbs.ToList(), line))
                        {
                            message = "オプション処理ファイルの設定内容に不備があります";
                            return message;
                        }

                        //設定
                        CTextBox.CTextBox triggeritem = this._tbs.ToList().Find(s => s.ItemName == line.Trigger_Item);

                        var oi = new OptionalProcessInfo(this._tbs.ToList(), line, this._extProps);
                        //トリガー項目の下に処理を格納
                        if (dicOptionalProcessInfos.Any(a => a.Key == triggeritem))
                        {
                            dicOptionalProcessInfos[triggeritem].Add(oi);
                        }
                        else
                        {
                            var list = new List<OptionalProcessInfo>();
                            list.Add(oi);
                            dicOptionalProcessInfos.Add(triggeritem, list);
                        }
                    }
                }

                //実際の処理をするクラスにDAOをセット
                OptionalProcess.Dao(_dao);

                //画面オブジェクトを渡す
                OptionalProcess.SetParent(this);
                return message;
            }
            catch /*(Exception e)*/
            {
                return "オプション処理ファイル読み込み中にエラーが発生しました";
            }
        }

        private string GetTips(string Tips, string DocId)
        {
            return SplitString(Tips, DocId);
        }

        private string GetValidPattern(string ValidPattern, string DocId)
        {
            return SplitString(ValidPattern, DocId);
        }

        private string GetLabelText(string LabelText, string DocId)
        {
            return SplitString(LabelText, DocId);
        }

        private string SplitString(string s1, string DocId)
        {
            string[] x = { ";" };
            var sList = s1.Split(x, StringSplitOptions.RemoveEmptyEntries);
            if (sList.Length == 1)
            {
                return sList[0];
            }
            else
            {
                foreach (var s in sList)
                {
                    string[] x2 = { @"\t" };
                    var ssss = s.Split(x2, StringSplitOptions.RemoveEmptyEntries);
                    if (ssss.Length == 1)
                    {
                        return ssss[0];
                    }
                    if (ssss[0].Contains(DocId))
                    {
                        return ssss[1];
                    }
                }
            }
            return s1;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (!(sender is CTextBox.CTextBox tb)) { return; }

            // オートリリースがtrueの項目でフル桁入力されたら次項目に移動
            if (tb.AutoRelease && tb.SelectionStart == tb.MaxLength) { NextControl(); }
        }

        #region MerPay
        private void RadioButtonYes_Click(object sender, EventArgs e)
        {
            this.ButtonExecute.Focus();
        }

        private void RadioButtonNo_Click(object sender, EventArgs e)
        {
            this.ButtonExecute.Focus();
        }
        #endregion

        private bool IsEmpty(List<string> values)
        {
            return values.Any();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tb"></param>
        private void TextBox_Enter_Custom(CTextBox.CTextBox tb)
        {
            switch (Utils.GetBussinessId())
            {
                #region りそな銀行　FATF
                case Consts.BusinessID.RBF:
                    if ("国籍_コード".Equals(tb.ItemName))
                    {
                        if (tb.Text.Length == 0)
                        {
                            using (var fm = new CodeDefineList.FrmCodeDefineList("国籍"))
                            {
                                fm.ShowDialog(this);
                                if (!fm.DialogResult.Equals(DialogResult.Cancel))
                                {
                                    tb.Text = fm._Key;
                                    foreach (var _tb in this._tbs)
                                    {
                                        if ("国籍_カナ".Equals(_tb.ItemName))
                                        {
                                            _tb.Text = fm._Value1;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if ("在留資格_コード".Equals(tb.ItemName))
                    {
                        if (tb.Text.Length == 0)
                        {
                            using (var fm = new CodeDefineList.FrmCodeDefineList("在留資格"))
                            {
                                fm.ShowDialog(this);
                                if (!fm.DialogResult.Equals(DialogResult.Cancel))
                                {
                                    tb.Text = fm._Key;
                                    foreach (var _tb in this._tbs)
                                    {
                                        if ("在留資格_資格名".Equals(_tb.ItemName))
                                        {
                                            _tb.Text = fm._Value1;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region 静岡銀行　在留カード受付業務 
                case Consts.BusinessID.SBZ:
                    switch (_DOC_ID)
                    {
                        case "20010001":
                        case "30010001":
                            if ("本人確認書類_国籍".Equals(tb.ItemName) && tb.Text.Length == 0)
                            {
                                using (var fm = new CodeDefineList.FrmCodeDefineList("国名"))
                                {
                                    fm.ShowDialog(this);
                                    if (!fm.DialogResult.Equals(DialogResult.Cancel))
                                    {
                                        tb.Text = fm._Key;
                                    }
                                }
                            }

                            if ("本人確認書類_在留資格".Equals(tb.ItemName) && tb.Text.Length == 0)
                            {
                                using (var fm = new CodeDefineList.FrmCodeDefineList("在留資格"))
                                {
                                    fm.ShowDialog(this);
                                    if (!fm.DialogResult.Equals(DialogResult.Cancel))
                                    {
                                        tb.Text = fm._Key;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                    #endregion
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tb"></param>
        private void TextBox_Leave_Custom(CTextBox.CTextBox tb)
        {
            if (!Consts.RecordKbn.ADMIN.Equals(this._RECORD_KBN))
            {
                #region 受領テーブル
                if (tb.IsExistingReceipt && tb.Text.Length != 0)
                {
                    var dt = _dao.SELECT_T_ENTRY_RECEIPT(this._DOC_ID, tb.ItemName, tb.Text);
                    if (dt.Rows.Count == 1)
                    {
                        if (IsEmpty(new List<string> {_tbs[01].Text, _tbs[02].Text, _tbs[03].Text, _tbs[04].Text, _tbs[05].Text
                                                       , _tbs[06].Text, _tbs[07].Text, _tbs[08].Text, _tbs[09].Text}))
                        {
                            _tbs[01].Text = dt.Rows[0]["ITEM_001"].ToString().Substring(0, _tbs[1].MaxLength);
                            _tbs[02].Text = dt.Rows[0]["ITEM_002"].ToString().Substring(0, _tbs[2].MaxLength);
                            _tbs[03].Text = dt.Rows[0]["ITEM_003"].ToString().Substring(0, _tbs[3].MaxLength);
                            _tbs[04].Text = dt.Rows[0]["ITEM_004"].ToString().Substring(0, _tbs[4].MaxLength);
                            _tbs[05].Text = dt.Rows[0]["ITEM_005"].ToString().Substring(0, _tbs[5].MaxLength);
                            _tbs[06].Text = dt.Rows[0]["ITEM_006"].ToString().Substring(0, _tbs[6].MaxLength);
                            _tbs[07].Text = dt.Rows[0]["ITEM_007"].ToString().Substring(0, _tbs[7].MaxLength);
                            _tbs[08].Text = dt.Rows[0]["ITEM_008"].ToString().Substring(0, _tbs[8].MaxLength);
                            _tbs[09].Text = dt.Rows[0]["ITEM_009"].ToString().Substring(0, _tbs[9].MaxLength);
                            //    _tbs[10].Text = dt.Rows[0]["ITEM_010"].ToString();
                        }
                    }
                }
                #endregion

                switch (Utils.GetBussinessId())
                {
                    #region 中電
                    case Consts.BusinessID.CDC:
                        if ("30010001".Equals(this._DOC_ID) && "TEXT001".Equals(tb.Name.ToUpper()))
                        {
                            if (tb.Text.Length != 0)
                            {
                                var dt = _dao.SELECT_T_NYURYOKUJYOHO(tb.Text);
                                if (dt.Rows.Count == 1)
                                {
                                    if (!IsEmpty(new List<string> {_tbs[35].Text, _tbs[36].Text, _tbs[37].Text, _tbs[38].Text, _tbs[39].Text, _tbs[40].Text
                                                                  , _tbs[41].Text, _tbs[42].Text, _tbs[43].Text, _tbs[44].Text, _tbs[45].Text, _tbs[46].Text
                                                                  , _tbs[47].Text, _tbs[48].Text, _tbs[49].Text, _tbs[50].Text, _tbs[51].Text, _tbs[52].Text
                                                                  , _tbs[53].Text }))
                                    {
                                        _tbs[35].Text = dt.Rows[0]["MOUSIKOMIBI"].ToString();
                                        _tbs[36].Text = dt.Rows[0]["MEI_KANA"].ToString();
                                        _tbs[37].Text = dt.Rows[0]["MEI_KANJI"].ToString();
                                        _tbs[38].Text = dt.Rows[0]["DENWABANGO_KUBUN"].ToString();
                                        _tbs[39].Text = dt.Rows[0]["DENWABANGO"].ToString();
                                        _tbs[40].Text = dt.Rows[0]["SOUFUSAKI_JUSHO_HENKO_UMU"].ToString();
                                        _tbs[41].Text = dt.Rows[0]["SOUFUSAKI_YUBIN"].ToString();
                                        _tbs[42].Text = dt.Rows[0]["SOUFUSAKI_TODOFUKEN"].ToString();
                                        _tbs[43].Text = dt.Rows[0]["SOUFUSAKI_SHIKUCHOSON"].ToString();
                                        _tbs[44].Text = dt.Rows[0]["SOUFUSAKI_CHOIKI"].ToString();
                                        _tbs[45].Text = dt.Rows[0]["SOUFUSAKI_CHOME"].ToString();
                                        _tbs[46].Text = dt.Rows[0]["SOUFUSAKI_BANCHI"].ToString();
                                        _tbs[47].Text = dt.Rows[0]["SOUFUSAKI_TATEMONO"].ToString();
                                        _tbs[48].Text = dt.Rows[0]["SOUFUSAKI_HEYABANGO"].ToString();
                                        _tbs[49].Text = dt.Rows[0]["SOUFUSAKI_ATENA"].ToString();
                                        _tbs[50].Text = dt.Rows[0]["CARDJOHO_YUKOKIGEN_TSUKI"].ToString();
                                        _tbs[51].Text = dt.Rows[0]["CARDJOHO_YUKOKIGEN_NEN"].ToString();
                                        _tbs[52].Text = dt.Rows[0]["CARDJOHO_MEI_KANA"].ToString();
                                        _tbs[53].Text = dt.Rows[0]["CARDJOHO_MEI"].ToString();
                                    }
                                }
                            }
                        }
                        break;
                        #endregion
                }
            }
        }

        private void TableLayoutPanel_Scroll(object sender, ScrollEventArgs e)
        {
            if (this.TableLayoutPanel.VerticalScroll.Maximum > 32000)
            {
                TableLayoutPanelReflesh();
            }
        }
    }
}
