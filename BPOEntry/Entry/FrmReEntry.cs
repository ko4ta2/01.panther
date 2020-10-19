using Common;
using NLog;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
//using Entry.Common;
//using Common;

namespace BPOEntry.EntryForms
{
    public partial class FrmReEntry : Form
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

        /// <summary>
        /// 再入力回数
        /// </summary>
        private int re_entry_count = 0;

        // 再入力したか
        private bool isReEntry { get; set; }

        // 再入力した内容
        private string ret_value { get; set; }

        //private BPOEntryItemExtensionProperties _rule = null;

        // 入力担当者1
        private string entry_Inp1_lbl = string.Empty;
        private string entry_Inp1_value = string.Empty;

        // 入力担当者2
        private string entry_Inp2_lbl = string.Empty;
        private string entry_Inp2_value = string.Empty;

        // 管理者採用で再入力フラグ
        private bool isAdmin_entry = false;

        /// <summary>
        /// ターゲットテキストボックス
        /// </summary>
        public CTextBox.CTextBox _TargetTextBox = new CTextBox.CTextBox();

        //private List<CTextBox.CTextBox> _tbs = new List<CTextBox.CTextBox>();

        // 何個目差異を表す
        private int iCrt_Count = 0;

        // 全体の差異の数
        private int iTotal_Count = 0;

        //private int iImaegPostion_X = 0;
        //private int iImaegPostion_Y = 0;

        private string DocId = null;

        // イメージ表示画面
        private FrmImage _ImgForm = null;
        #endregion

        //public frmBPOReEntry(frmBPOImage imgForm, string DOC_ID, string IMAGE_CAPTURE_DATE, string IMAGE_CAPTURE_NUM, string ENTRY_UNIT, string IMAGE_SEQ, string ITEM_ID,
        //        BPOEntryItemExtensionProperties rule, int crt_cnt, int ttl_cnt, CTextBox.CTextBox tb)
        public FrmReEntry(FrmImage IMAGE_FORM, string ENTRY_UNIT_ID, int IMAGE_SEQ, string ITEM_ID
            /*,BPOEntryItemExtensionProperties rule*/, int crt_cnt, int ttl_cnt, CTextBox.CTextBox tb)
        {
            InitializeComponent();

            this.Text = Utils.GetFormText();

            this._ImgForm = IMAGE_FORM;
            this.iCrt_Count = crt_cnt;
            this.iTotal_Count = ttl_cnt;

            this._TargetTextBox = tb;

            this.textInp1.Font = new Font(tb.Font.Name, tb.Font.Size, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
            this.textInp2.Font = new Font(tb.Font.Name, tb.Font.Size, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128))); 
            
            this.textInp1.InputMode = tb.InputMode;
            this.textInp2.InputMode = tb.InputMode;

            this.textInp1.DefaultImeMode = tb.DefaultImeMode;
            this.textInp2.DefaultImeMode = tb.DefaultImeMode;

            this.textInp1.Conditional_Required_Item = tb.Conditional_Required_Item;
            this.textInp2.Conditional_Required_Item = tb.Conditional_Required_Item;

            this.textInp1.Conditional_Required_Value = tb.Conditional_Required_Value;
            this.textInp2.Conditional_Required_Value = tb.Conditional_Required_Value;

            this.textInp1.Conditional_Required_Omit_Value = tb.Conditional_Required_Omit_Value;
            this.textInp2.Conditional_Required_Omit_Value = tb.Conditional_Required_Omit_Value;

            this.textInp1.ItemName = tb.ItemName;
            this.textInp2.ItemName = tb.ItemName;

            this.textInp1.Regex = tb.Regex;
            this.textInp2.Regex = tb.Regex;

            this.textInp1.ValidPattern= tb.ValidPattern;
            this.textInp2.ValidPattern = tb.ValidPattern;

            this.textInp1.DR = tb.DR;
            this.textInp2.DR = tb.DR;

            this.textInp1.MasterCheck = tb.MasterCheck;
            this.textInp2.MasterCheck = tb.MasterCheck;

            textInp1.Enabled = false;
            textInp2.Enabled = false;

            textInp1.CharacterCasing = this._TargetTextBox.CharacterCasing;
            textInp2.CharacterCasing = this._TargetTextBox.CharacterCasing;

            switch (this._TargetTextBox.InputMode)
            {
                case Consts.InputMode.AlphabetNumeric:
                    textInp1.ImeMode = ImeMode.Disable;
                    textInp2.ImeMode = ImeMode.Disable;
                    break;
                case Consts.InputMode.KanaFull:
                    textInp1.ImeMode = ImeMode.Katakana;
                    textInp2.ImeMode = ImeMode.Katakana;
                    break;
                case Consts.InputMode.KanaHalf:
                    textInp1.ImeMode = ImeMode.KatakanaHalf;
                    textInp2.ImeMode = ImeMode.KatakanaHalf;
                    break;
                case Consts.InputMode.Hiragana:
                case Consts.InputMode.Full:
                    textInp1.ImeMode = ImeMode.Hiragana;
                    textInp2.ImeMode = ImeMode.Hiragana;
                    break;
            }

            textInp1.MaxLength = this._TargetTextBox.MaxLength;
            textInp2.MaxLength = this._TargetTextBox.MaxLength;

            textInp1.IsMailAddress = this._TargetTextBox.IsMailAddress;
            textInp2.IsMailAddress = this._TargetTextBox.IsMailAddress;

            textInp1.DateFormat = this._TargetTextBox.DateFormat;
            textInp2.DateFormat = this._TargetTextBox.DateFormat;

            // 入力値、入力担当者名を取得する
            var dtEntryItem = dao.SelectUserName(ENTRY_UNIT_ID, IMAGE_SEQ, ITEM_ID);

            // 初期値を設定する
            SetInit(dtEntryItem/*, dtUser*/);

            // 選択ボタンにフォーカスをあてる
            this.btnUse1.Focus();

            // 帳票ID
            this.DocId = ENTRY_UNIT_ID.Split('_')[4];
        }

        public string GetRetValue()
        {
            return this.entry_Inp1_value;
        }

        public bool IsReEntry()
        {
            return this.isReEntry;
        }

        private void SetInit(DataTable dtEntryItem)
        {
            isAdmin_entry = false;
            this.re_entry_count = 0;
            this.isReEntry = false;
            this.ret_value = string.Empty;

            entry_Inp1_lbl = dtEntryItem.Rows[0]["USER_NAME"].ToString().Trim();
            lblInp1.Text = entry_Inp1_lbl + Consts.TEXT.RE_ENTRY_TEXT;
            entry_Inp1_value = dtEntryItem.Rows[0]["VALUE"].ToString().Trim();
            textInp1.Text = entry_Inp1_value;

            entry_Inp2_lbl = dtEntryItem.Rows[1]["USER_NAME"].ToString().Trim();
            lblInp2.Text = entry_Inp2_lbl + Consts.TEXT.RE_ENTRY_TEXT;
            entry_Inp2_value = dtEntryItem.Rows[1]["VALUE"].ToString().Trim();
            textInp2.Text = entry_Inp2_value;

            lblMessage.Text = string.Empty;
            lblEntryItemName.Text = "修正対象項目：" + this._TargetTextBox.ItemName;
            lblStatus.Text = String.Concat("修正 " , this.iCrt_Count.ToString("000") , "/" , this.iTotal_Count.ToString("000"));

            //SetTextBoxProperty();

            // 大和証券特殊処理
            if (!this._TargetTextBox.IsInput2)
            {
                this.btnReEntry.Text = "登録";
                System.EventArgs e = new System.EventArgs();
                BtnUse1_Click(string.Empty, e);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            EndProc();
        }

        private void EndProc()
        {
            //dao.Close();
            this.Close();
            this.Dispose();
        }

        private void BtnReEntry_Click(object sender, EventArgs e)
        {
            switch (this.re_entry_count)
            {
                case 0:
                    // 初回再入力の場合
                    BtnReEntry_1st();
                    break;
                case 1:
                    // ２回目再入力の場合
                    BtnReEntry_2nd();
                    break;
                case 2:
                    BtnReEntry_3rd();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 初回再入力の場合
        /// </summary>
        private void BtnReEntry_1st()
        {
            // メッセージを表示する
            lblMessage.Text = Consts.Messages.RE_ENTRY_MSG1;

            // 採用ボタンを使用不可にする
            btnUse1.Enabled = false;
            btnUse2.Enabled = false;

            // テキスト１と２の内容をクリアする
            textInp1.Text = string.Empty;
            textInp2.Text = string.Empty;

            // テキスト１を使用可、テキスト２を使用不可にする
            textInp1.Enabled = true;
            textInp2.Enabled = false;
            textInp1.BackColor = Consts.TEXT_BOX_BACK_COLOR.TergetItemBackColorHalf;
            textInp2.BackColor = SystemColors.Window;

            this.btnReEntry.Text = "確定";

            textInp1.Focus();

            this.re_entry_count = 1;
        }

        /// <summary>
        /// 2回目再入力の場合
        /// </summary>
        private void BtnReEntry_2nd()
        {
            if (!IsEntered(textInp1.Text))
            {
                textInp1.Focus();
                return;
            }

            // メッセージを表示する
            lblMessage.Text = Consts.Messages.RE_ENTRY_MSG2;

            // テキスト１を使用不可、テキスト２を使用可にする
            this.entry_Inp1_value = textInp1.Text;
            textInp1.Text = Consts.TEXT.PASSWORD_CHAR;
            textInp1.Enabled = false;
            textInp2.Enabled = true;
            textInp2.BackColor = Consts.TEXT_BOX_BACK_COLOR.TergetItemBackColorHalf;
            textInp1.BackColor = SystemColors.Window;

            textInp2.Focus();

            this.re_entry_count = 2;
        }

        /// <summary>
        /// 3回目再入力の場合
        /// </summary>
        private void BtnReEntry_3rd()
        {
            if (!IsEntered(textInp2.Text))
            {
                textInp2.Focus();
                return;
            }

            #region 大和証券特殊処理
            if (!this._TargetTextBox.IsInput2)
            {
                this.entry_Inp1_value = textInp2.Text;
                this.isReEntry = true;
                this.EndProc();
                return;
            }
            #endregion

            textInp1.BackColor = SystemColors.Window;
            textInp2.BackColor = SystemColors.Window;

            // 2回再入力した内容が不一致の場合
            if (this.entry_Inp1_value != textInp2.Text)
            {
                lblMessage.Text = Consts.Messages.RE_ENTRY_ERR_MSG;
                if (!isAdmin_entry)
                {
                    textInp2.Enabled = false;
                    this.re_entry_count = 0;
                }
                else
                {
                    textInp2.Focus();
                }
            }
            else
            {
                this.ret_value = this.entry_Inp1_value;
                this.isReEntry = true;
                this.EndProc();
            }
        }

        private void BtnUse1_Click(object sender, EventArgs e)
        {
            string sText1 = textInp1.Text;
            isAdmin_entry = true;
            // 採用ボタンを使用不可にする
            btnUse1.Enabled = false;
            btnUse2.Enabled = false;

            lblInp2.Text = Program.LoginUser.USER_NAME + Consts.TEXT.RE_ENTRY_TEXT;

            // テキスト１を使用不可、テキスト２を使用可にする
            textInp1.Text = Consts.TEXT.PASSWORD_CHAR;
            textInp1.Enabled = false;
            textInp2.Enabled = true;
            textInp2.Text = string.Empty;
            ////textInp2.BackColor = Consts.TergetItemBackColorHalf;
            //if (Consts.InputMode.AlphabetNumeric == this._TargetTextBox.InputMode
            //    /*|| Consts.InputMode.KatakanaHalf == _rule.InputMode*/)
            //{
            //    textInp2.BackColor = Consts.TEXT_BOX_BACK_COLOR.TergetItemBackColorHalf;
            //}
            //else
            //{
            //    textInp2.BackColor = Consts.TEXT_BOX_BACK_COLOR.TergetItemBackColorFull;
            //}
            textInp1.BackColor = SystemColors.Window;

            lblMessage.Text = Consts.Messages.RE_ENTRY_MSG2;
            this.re_entry_count = 2;

            #region 大和証券特殊処理
            if (!this._TargetTextBox.IsInput2)
            {
                lblMessage.Text = string.Empty;
                textInp2.Text = sText1;
            }
            #endregion

            this.btnReEntry.Text = "確定";

            textInp2.Focus();
        }

        private void BtnUse2_Click(object sender, EventArgs e)
        {
            isAdmin_entry = true;
            // 採用ボタンを使用不可にする
            btnUse1.Enabled = false;
            btnUse2.Enabled = false;

            lblInp1.Text = this.entry_Inp2_lbl + Consts.TEXT.RE_ENTRY_TEXT;
            lblInp2.Text = Program.LoginUser.USER_NAME + Consts.TEXT.RE_ENTRY_TEXT;

            // テキスト１を使用不可、テキスト２を使用可にする
            textInp1.Text = Consts.TEXT.PASSWORD_CHAR;
            this.entry_Inp1_value = this.entry_Inp2_value;
            textInp1.Enabled = false;
            textInp2.Enabled = true;
            textInp2.Text = string.Empty;
            //if (Consts.InputMode.AlphabetNumeric == this._TargetTextBox.InputMode
            //    /*|| Consts.InputMode.KatakanaHalf == _rule.InputMode*/)
            //{
            //    textInp2.BackColor = Consts.TEXT_BOX_BACK_COLOR.TergetItemBackColorHalf;
            //}
            //else
            //{
            //    textInp2.BackColor = Consts.TEXT_BOX_BACK_COLOR.TergetItemBackColorFull;
            //}
            textInp1.BackColor = SystemColors.Window;

            lblMessage.Text = Consts.Messages.RE_ENTRY_MSG2;
            this.re_entry_count = 2;

            this.btnReEntry.Text = "確定";

            textInp2.Focus();
        }


        //private void SetMaxLength()
        //{

        //    //textInp1.MaxLengthReal = this._TargetTextBox.MaxLengthReal;
        //    //textInp2.MaxLengthReal = this._TargetTextBox.MaxLengthReal;

        //    //textInp1.MinLength = this._TargetTextBox.MinLength;
        //    //textInp2.MinLength = this._TargetTextBox.MinLength;
        //}

        //private void SetCharacterCasing()
        //{
        //    textInp1.CharacterCasing = this._TargetTextBox.CharacterCasing;
        //    textInp2.CharacterCasing = this._TargetTextBox.CharacterCasing;
        //}

        /// <summary>
        /// 必須チェック
        /// </summary>
        /// <param name="sText"></param>
        /// <returns></returns>
        private bool IsEntered(string sText)
        {
            sText = sText.Trim();
            if (this._TargetTextBox.IsRequired)
                if (string.IsNullOrEmpty(sText))
                    return false;
            return true;
        }

        /// <summary>
        /// ツールチップ表示
        /// </summary>
        private void ShowToolTips()
        {
            // 表示するTipsが無ければ非表示にします。
            if (string.IsNullOrEmpty(this._TargetTextBox.Tips))
            {
                this.toolTips.Hide(this);
            }
            else
            {
                var pt = new Point(0, 0);
                WindowsFormUtils.AbsolutePositionFromForm(lblEntryItemName, ref pt);
                pt.X += lblEntryItemName.Width + 5;
                //this.toolTips.Show(this._rule.Tips, this, pt);
                this.toolTips.Show(this._TargetTextBox.Tips.Replace(@"\n", "\n"), this, pt);
            }
        }

        /// <summary>
        /// ダイアログ キーを処理します。
        /// </summary>
        /// <param name="keyData">処理するキーを表す Keys 値の 1 つ。</param>
        /// <returns>キーがコントロールによって処理された場合は true。それ以外の場合は false。</returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            // Enterキー押下によるフォーカス移動
            if (((keyData & Keys.KeyCode) == Keys.Return)
                && ((keyData & (Keys.Alt | Keys.Control)) == Keys.None))
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
                SendKeys.Send("{TAB}");
            else
                SendKeys.Send("+{TAB}");
        }

        private void ReEntryForm_Shown(object sender, EventArgs e)
        {
            // Tipsをラベルの右側に常に表示します。
            ShowToolTips();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReEntry_Enter(object sender, EventArgs e)
        {
            this.btnReEntry.Font = new Font("Meiryo UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(128)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReEntry_Leave(object sender, EventArgs e)
        {
            this.btnReEntry.Font = new Font("Meiryo UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
        }

        private void ReEntryForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    // F1　↓スクロール
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
                    this._ImgForm.ImaegPostion_Y_Value -= this._ImgForm.PnlImage_VerticalScroll_Maximum / 4;
                    if (this._ImgForm.ImaegPostion_Y_Value < 0)
                    {
                        this._ImgForm.ImaegPostion_Y_Value = 0;
                    }
                    this._ImgForm.PanelImage_AutoScrollPosition = new Point(this._ImgForm.ImaegPostion_X_Value, this._ImgForm.ImaegPostion_Y_Value);
                    e.Handled = true;
                    break;
                case Keys.F3:
                    // F3　→スクロール
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
                    this._ImgForm.ImaegPostion_X_Value -= this._ImgForm.PnlImage_HorizontalScroll_Maximum / 4;
                    if (this._ImgForm.ImaegPostion_X_Value < 0)
                    {
                        this._ImgForm.ImaegPostion_X_Value = 0;
                    }
                    this._ImgForm.PanelImage_AutoScrollPosition = new Point(this._ImgForm.ImaegPostion_X_Value, this._ImgForm.ImaegPostion_Y_Value);
                    e.Handled = true;
                    break;
                case Keys.F5:
                    // F5　拡大
                    this._ImgForm.ImageZoomCount = 1;
                    this._ImgForm.CheckBokAutoScroll_Checked = false;
                    e.Handled = true;
                    break;
                case Keys.F6:
                    // F6　縮小
                    this._ImgForm.ImageZoomCount = -1;
                    this._ImgForm.CheckBokAutoScroll_Checked = false;
                    e.Handled = true;
                    break;
                case Keys.F7:
                    // F7　回転
                    this._ImgForm.ImageRotate();
                    e.Handled = true;
                    break;
                case Keys.F8:
                    // イメージ頁切替
                    // シフトキー確認
                    //var forward = true;
                    //if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) { forward = false; }
                    //this._imgForm.ChangePage(!((Control.ModifierKeys & Keys.Shift) == Keys.Shift));
                    if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
                    {
                        this._ImgForm.IsShowBaseImageValue = !this._ImgForm.IsShowBaseImageValue;
                    }
                    else
                    {
                        this._ImgForm.ChangePage(!((Control.ModifierKeys & Keys.Shift) == Keys.Shift));
                    }
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
                    // F12　自動スクロールのチェック状態を反転
                    this._ImgForm.CheckBokAutoScroll_Checked = !this._ImgForm.CheckBokAutoScroll_Checked;
                    this._ImgForm.ImageZoomCount = 0;
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        private void textInp_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(sender is CTextBox.CTextBox tb))
                return;

            //// 拡張プロパティの登録が無ければ中断します。
            //if (this._TargetTextBox == null)
            //    return;

            // 必須項目は未入力の場合にフォーカスを戻します。
            if (!IsEntered(tb.Text))
            {
                tb.Focus();
                //MessageBox.Show("必須項目は必ず入力してください。", "エントリ修正", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                MessageBox.Show($"「{tb.ItemName}」は入力必須項目です。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
                return;
            }
            /*
                        // 条件付き必須
                        if (tb.Text.Length == 0
                            && _TargetTextBox.Conditional_Required_Item.Length != 0
                            && _TargetTextBox.Conditional_Required_Value != null)
                        {
                            for (int iIdx = 0; iIdx <= ((frmBPOEntry)this.Owner)._tbs.Count - 1; iIdx++)
                            {
                                if (((frmBPOEntry)this.Owner)._tbs[iIdx].ItemName.Equals(_TargetTextBox.Conditional_Required_Item)
                                    && _TargetTextBox.Conditional_Required_Value.Contains(((frmBPOEntry)this.Owner)._tbs[iIdx].Text) && ((frmBPOEntry)this.Owner)._tbs[iIdx].Text.Length != 0
                                       || _TargetTextBox.Conditional_Required_Value.Length == 1 && _TargetTextBox.Conditional_Required_Value[0].Length == 0 && ((frmBPOEntry)this.Owner)._tbs[iIdx].Text.Length != 0)
                                {
                                    if (_TargetTextBox.Conditional_Required_Value.Contains(((frmBPOEntry)this.Owner)._tbs[iIdx].Text) && ((frmBPOEntry)this.Owner)._tbs[iIdx].Text.Length != 0)
                                        MessageBox.Show(String.Format("「{0}」は条件付き入力必須項目です。\n{1} = {2} ", _TargetTextBox.ItemName, _TargetTextBox.Conditional_Required_Item, String.Join(",", _TargetTextBox.Conditional_Required_Value)), "エントリ修正", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    else
                                        MessageBox.Show(String.Format("「{0}」に入力がある場合、\n「{1}」は入力必須です。", _TargetTextBox.Conditional_Required_Item, _TargetTextBox.ItemName), "エントリ修正", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    e.Cancel = true;
                                    return;
                                }
                            }
                        }
                        // 条件付き必須（除外）
                        if (tb.Text.Length == 0
                            && _TargetTextBox.Conditional_Required_Item.Length != 0
                            && _TargetTextBox.Conditional_Required_Omit_Value[0].Length != 0)
                        {
                            for (int iIdx = 0; iIdx <= ((frmBPOEntry)this.Owner)._tbs.Count - 1; iIdx++)
                            {
                                if (((frmBPOEntry)this.Owner)._tbs[iIdx].ItemName.Equals(_TargetTextBox.Conditional_Required_Item)
                                    && _TargetTextBox.Conditional_Required_Omit_Value.Contains(((frmBPOEntry)this.Owner)._tbs[iIdx].Text) && ((frmBPOEntry)this.Owner)._tbs[iIdx].Text.Length != 0
                                       || _TargetTextBox.Conditional_Required_Omit_Value.Length == 1 && _TargetTextBox.Conditional_Required_Omit_Value[0].Length != 0 && ((frmBPOEntry)this.Owner)._tbs[iIdx].Text.Length != 0)
                                {
                                    if (_TargetTextBox.Conditional_Required_Omit_Value.Contains(((frmBPOEntry)this.Owner)._tbs[iIdx].Text) && ((frmBPOEntry)this.Owner)._tbs[iIdx].Text.Length != 0)
                                        MessageBox.Show(String.Format("「{0}」は条件付き入力必須項目です。\n{1} = {2} ", _TargetTextBox.ItemName, _TargetTextBox.Conditional_Required_Item, String.Join(",", _TargetTextBox.Conditional_Required_Value)), "エントリ修正", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    else
                                        MessageBox.Show(String.Format("「{0}」に入力がある場合、\n「{1}」は入力必須です。", _TargetTextBox.Conditional_Required_Item, _TargetTextBox.ItemName), "エントリ修正", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    e.Cancel = true;
                                    return;
                                }
                            }
                        }
            */

            #region 条件付き必須
            if (tb.Text.Length == 0
                && tb.Conditional_Required_Item.Length != 0)
            {
                for (int iIdx = 0; iIdx <= ((FrmEntry)this.Owner)._tbs.Length - 1; iIdx++)
                {
                    if (((FrmEntry)this.Owner)._tbs[iIdx].ItemName.Equals(tb.Conditional_Required_Item))
                    {
                        if (tb.Conditional_Required_Value.Contains(((FrmEntry)this.Owner)._tbs[iIdx].Text)
                            && tb.Conditional_Required_Value[0].Length != 0)
                        {
                            MessageBox.Show(String.Format("「{0}」に「{1}」を入力した場合\n「{2}」は入力必須です。", tb.Conditional_Required_Item, String.Join(",", tb.Conditional_Required_Value), tb.ItemName), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            e.Cancel = true;
                        }
                        else if (!tb.Conditional_Required_Omit_Value.Contains(((FrmEntry)this.Owner)._tbs[iIdx].Text)
                            && tb.Conditional_Required_Omit_Value[0].Length != 0)
                        {
                            MessageBox.Show(String.Format("「{0}」が「{1}」以外の場合\n「{2}」は入力必須です。", tb.Conditional_Required_Item, String.Join(",", tb.Conditional_Required_Omit_Value[0]), tb.ItemName), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            e.Cancel = true;
                        }
                        else if ("ANYTHING".Equals(tb.Conditional_Required_Value[0].ToUpper()) && ((FrmEntry)this.Owner)._tbs[iIdx].Text.Length != 0)
                        {
                            MessageBox.Show(String.Format("「{0}」に入力がある場合、\n「{1}」は入力必須です。", tb.Conditional_Required_Item, tb.ItemName), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            e.Cancel = true;
                        }
                    }
                }
            }
            #endregion

            //// 最小文字数
            //if (tb.MinLength != -1
            //    && (tb.Text.Length < tb.MinLength))
            //{
            //    MessageBox.Show($"「{tb.ItemName}」は{tb.MinLength}文字以上で入力して下さい。 ", "エントリ修正", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    e.Cancel = true;
            //    return;
            //}

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

            // 最大文字数
            if (tb.Text.Length > tb.MaxLength)
            {
                MessageBox.Show($"「{tb.ItemName}」の最大入力文字数は{tb.MaxLength}文字です。 ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
                return;
            }

            // 入力可能パターン違反の場合にフォーカスを戻します。
            var pattern = this._TargetTextBox.ValidPattern;
            if (pattern.Length != 0 && tb.Text.Replace(Config.ReadNotCharNarrowInput, string.Empty).Replace(Config.ReadNotCharWideInput, string.Empty).Length != 0)
            {
                // 判別不能文字を除いてチェックする
                var omitedText = tb.Text.Replace(Config.ReadNotCharNarrowInput, string.Empty).Replace(Config.ReadNotCharWideInput, string.Empty);
                if (!Regex.IsMatch(omitedText, pattern))
                {
                    tb.Focus();
                    //MessageBox.Show("「" + tb.Text + "」は入力出来ません。", "エントリ修正", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    MessageBox.Show($"「{ tb.ItemName}」に「{tb.Text}」は入力出来ません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                    return;
                }
            }

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

            #region マイナンバー
            if (this._TargetTextBox.MyNumber1.Length != 0 && this._TargetTextBox.MyNumber2.Length != 0)
            {
                var sMyNumber = string.Empty;
                foreach (var ctbs in ((FrmEntry)this.Owner)._tbs)
                {
                    if (ctbs.ItemName.Equals(this._TargetTextBox.MyNumber1) || ctbs.ItemName.Equals(this._TargetTextBox.MyNumber2))
                    {
                        sMyNumber += ctbs.Text;
                    }
                }
                sMyNumber += tb.Text;
                if (sMyNumber.Contains(Config.ReadNotCharNarrowInput))
                {
                    MessageBox.Show("個人番号に判読不可文字が入力されています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (!Utils.IsValidMyNumber(sMyNumber))
                {
                    MessageBox.Show("個人番号として不正な値が入力されています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
//                    tb.Focus();
                    e.Cancel = true;
                    return;
                }
            }
            #endregion

            // 全角ひらがな項目のみ
            if (Consts.InputMode.Hiragana.Equals(tb.InputMode)
                || Consts.InputMode.Full.Equals(tb.InputMode)
                || Consts.InputMode.MixFull.Equals(tb.InputMode))
            {
                for (int iIdx = 0; iIdx <= tb.Text.Length - 1; iIdx++)
                {
                    //b = ;
                    if (!Utils.IsValidChar(tb.InputMode, tb.Text.Substring(iIdx, 1)))
                    {
                        MessageBox.Show($"「{tb.ItemName}」に入力禁止文字が含まれています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        tb.Focus();
                        tb.Select(iIdx, 1);
                        e.Cancel = true;
                        return;
                    }
                }
            }

            #region マスタ存在チェック
            if (tb.MasterCheck)
            {
                if (!tb.Text.Contains(Config.ReadNotCharNarrowInput)
                    && tb.Text.Length != 0)
                {
                    if (!dao.SELECT_M_ITEM_CHECK(this.DocId, tb.ItemName, tb.Text))
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
                    e.Cancel = true; return;
                }
            }
            #endregion

            tb.BackColor = SystemColors.Window;
        }

        private void FrmReEntry_Load(object sender, EventArgs e)
        {
            this.btnUse1.Focus();
        }
    }
}
