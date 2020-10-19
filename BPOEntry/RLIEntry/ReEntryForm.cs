using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Common;
using NLog;

namespace BPOEntry.EntryForms
{
    public partial class ReEntryForm : Form
    {
		#region 変数

		/// <summary>
		/// log
		/// </summary>
		private static Logger _Log = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// データアクセス
		/// </summary>
        //private static EntryFormDao _Dao = new EntryFormDao();
        protected static DaoBPOEntry _Dao = new DaoBPOEntry();

		/// <summary>
		/// 再入力回数
		/// </summary>
		private int re_entry_count = 0;

		// 再入力したか
		private bool isReEntry { get; set; }

		// 再入力した内容
		private string ret_value { get; set; }

		private EntryItemExtensionProperties _rule = null;

		// 入力担当者1
		private string entry_Inp1_lbl = string.Empty;
		private string entry_Inp1_value = string.Empty;

		// 入力担当者2
		private string entry_Inp2_lbl = string.Empty;
		private string entry_Inp2_value = string.Empty;

		// 管理者採用で再入力フラグ
		private bool isAdmin_entry = false;

		// 何個目差異を表す
		private int iCrt_Count = 0;

		// 全体の差異の数
		private int iTotal_Count = 0;

		#endregion

		public ReEntryForm(string id, string date, string num, string grp, string imageSq, string entryItemId,
                EntryItemExtensionProperties rule, int crt_cnt, int ttl_cnt)
        {
			InitializeComponent();

            //if (Config.IsTestMode)
            //{
            //    this.lblTitle.BackColor = Consts.TestModeColor;
            //}

			this._rule = rule;
			this.iCrt_Count = crt_cnt;
			this.iTotal_Count = ttl_cnt;

			// ＤＢオープン
			_Dao.Open(Config.DSN);

			// 入力値を取得する
			DataTable dtEntryItem = _Dao.SelectEntryDataByItem(id, date, num, grp, int.Parse(imageSq), entryItemId);

			// 入力担当者名を取得する
            DataTable dtUser = _Dao.SelectUserName(id, date, num, grp, int.Parse(imageSq), entryItemId);

			// 初期値を設定する
			SetInit(dtEntryItem, dtUser);

		}

        public string GetRetValue()
        {
			return this.entry_Inp1_value;
		}

        public bool IsReEntry()
        {
			return this.isReEntry;
		}

        private void SetInit(DataTable dtEntryItem, DataTable dtUser)
        {
			isAdmin_entry = false;
			this.re_entry_count = 0;
			this.isReEntry = false;
			this.ret_value = string.Empty;
			entry_Inp1_lbl = dtUser.Rows[0]["USER_NAME"].ToString().Trim();
			lblInp1.Text = entry_Inp1_lbl + Consts.TEXT.RE_ENTRY_TEXT;
			entry_Inp1_value = dtEntryItem.Rows[0]["VALUE"].ToString().Trim();
			textInp1.Text = entry_Inp1_value;
			entry_Inp2_lbl = dtUser.Rows[1]["USER_NAME"].ToString().Trim();
			lblInp2.Text = entry_Inp2_lbl + Consts.TEXT.RE_ENTRY_TEXT;
			entry_Inp2_value = dtEntryItem.Rows[1]["VALUE"].ToString().Trim();
			textInp2.Text = entry_Inp2_value;
			lblMessage.Text = string.Empty;
			lblEntryItemName.Text = this._rule.ItemName;
			lblTtlCount.Text = this.iTotal_Count.ToString("000");
			lblCrtCount.Text = this.iCrt_Count.ToString("000");

			textInp1.Enabled = false;
			textInp2.Enabled = false;

			SetTextBoxImeMode();
			SetMaxLength();
		}

        private void btnClose_Click(object sender, EventArgs e)
        {
			EndProc();
		}

        private void EndProc()
        {
			_Dao.Close();
			this.Close();
			this.Dispose();
		}

        private void btnReEntry_Click(object sender, EventArgs e)
        {
            switch (this.re_entry_count)
            {
				case 0:
					// 初回再入力の場合
					btnReEntry_1st();
					break;
				case 1:
					// ２回目再入力の場合
					btnReEntry_2nd();
					break;
				case 2:
					btnReEntry_3rd();
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// 初回再入力の場合
		/// </summary>
        private void btnReEntry_1st()
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
			textInp1.BackColor = Color.LightSalmon;
			textInp2.BackColor = SystemColors.Window;
			textInp1.Focus();

			this.re_entry_count = 1;
		}

		/// <summary>
		/// 2回目再入力の場合
		/// </summary>
        private void btnReEntry_2nd()
        {
            if (!ValidateEmpty(textInp1))
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
			textInp2.BackColor = Color.LightSalmon;
			textInp1.BackColor = SystemColors.Window;

			textInp2.Focus();

			this.re_entry_count = 2;
		}

		/// <summary>
		/// 3回目再入力の場合
		/// </summary>
        private void btnReEntry_3rd()
        {
            if (!ValidateEmpty(textInp2))
            {
				textInp2.Focus();
				return;
			}

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
					textInp2.BackColor = Color.LightSalmon;
				}
			}
            else
            {
				this.ret_value = this.entry_Inp1_value;
				this.isReEntry = true;
				this.EndProc();
			}
		}

        private void btnUse1_Click(object sender, EventArgs e)
        {
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
			textInp2.BackColor = Color.LightSalmon;
			textInp1.BackColor = SystemColors.Window;

			lblMessage.Text = Consts.Messages.RE_ENTRY_MSG2;
			this.re_entry_count = 2;

			textInp2.Focus();
		}

        private void btnUse2_Click(object sender, EventArgs e)
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
			textInp2.BackColor = Color.LightSalmon;
			textInp1.BackColor = SystemColors.Window;

			lblMessage.Text = Consts.Messages.RE_ENTRY_MSG2;
			this.re_entry_count = 2;

			textInp2.Focus();
		}

		/// <summary>
		/// 入力項目のImeModeを設定する
		/// </summary>
        private void SetTextBoxImeMode()
        {
            switch (_rule.InputElement)
            {
				case "9":
					textInp1.ImeMode = System.Windows.Forms.ImeMode.Disable;
					textInp2.ImeMode = System.Windows.Forms.ImeMode.Disable;
					break;
				case "X":
					textInp1.ImeMode = System.Windows.Forms.ImeMode.Disable;
					textInp2.ImeMode = System.Windows.Forms.ImeMode.Disable;
					break;
				case "NK":
					textInp1.ImeMode = System.Windows.Forms.ImeMode.Katakana;
					textInp2.ImeMode = System.Windows.Forms.ImeMode.Katakana;
					break;
				default:
					textInp1.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
					textInp2.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
					break;
			}
		}

        private void SetMaxLength()
        {
			EntryItemExtensionProperties rule = this._rule;
			textInp1.MaxLength = rule.InputMaxLength;
			textInp2.MaxLength = rule.InputMaxLength;
		}

		/// <summary>
		/// 入力制限
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void TextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
			// 共通的に許容するキー入力は無視します。
			var acceptKeys = new List<char>() { '\b', Config.ReadNotCharNarrowInput[0] };
			if (acceptKeys.Contains(e.KeyChar))
				return;

			var textbox = sender as TextBox;
			if (textbox == null)
				return;

			// 入力可能な文字列パターンの設定が無ければ無視します。
			string pattern = this._rule.AcceptKeyCharsPattern;
			if (string.IsNullOrEmpty(pattern))
				return;

			// 入力値が指定範囲以外ならハンドル済みに設定します。
			if (!Regex.IsMatch(e.KeyChar.ToString(), pattern))
				e.Handled = true;
		}

		private void TextBox_Leave(object sender, EventArgs e) {
			var textBox = sender as TextBox;
			if (textBox == null)
				return;

			// 拡張プロパティの登録が無ければ中断します。
			if (this._rule == null)
				return;

			// 必須項目は未入力の場合にフォーカスを戻します。
			if (!ValidateEmpty(textBox)) {
				textBox.Focus();
				MessageBox.Show("必須項目は必ず入力してください。");
				return;
			}

			// 入力可能パターン違反の場合にフォーカスを戻します。
			string pattern = this._rule.ValidStringPattern;
			if (!string.IsNullOrEmpty(pattern)) {
				// 判別不能文字を除いてチェックする
				string omitedText = textBox.Text.Replace(Config.ReadNotCharNarrowInput,string.Empty).Replace(Config.ReadNotCharWideInput,string.Empty);
				if (!Regex.IsMatch(omitedText, pattern)) {
					textBox.Focus();
					MessageBox.Show("「" + textBox.Text + "」は入力できません。");
					return;
				}
			}

			//// 全角ひらがな項目のみ
			//if (this._rule.InputElement == "NH") {
			//	// 入力可能全角文字チェック
			//	if (!Utils.IsAllowMultiByteChar(textBox.Text)) {
			//		textBox.Focus();
			//		MessageBox.Show("入力禁止文字を含んでいます。");
			//		return;
			//	}
			//}

			// 全角項目は、半角⇒全角へ変換します。
			if (this._rule.InputElement.StartsWith("N"))
				textBox.Text = Microsoft.VisualBasic.Strings.StrConv(textBox.Text, Microsoft.VisualBasic.VbStrConv.Wide);
		}

        private bool ValidateEmpty(TextBox textBox)
        {
			// 前後の空白を取り除きます。
			textBox.Text = textBox.Text.Trim();

			// 必須項目は未入力の場合にフォーカスを戻します。
            if (this._rule.IsRequired)
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
					return false;
				}
			}
			return true;
		}

        private void ShowToolTips()
        {
			// 表示するTipsが無ければ非表示にします。
            if (string.IsNullOrEmpty(this._rule.Tips))
            {
				this.toolTips.Hide(this);
			}
            else
            {
				var pt = new Point(0, 0);
				WindowsFormUtils.AbsolutePositionFromForm(lblEntryItemName, ref pt);
				pt.X += lblEntryItemName.Width + 5;
				this.toolTips.Show(this._rule.Tips, this, pt);
			}
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

        private void ReEntryForm_Shown(object sender, EventArgs e)
        {
			// Tipsをラベルの右側に常に表示します。
			ShowToolTips();
		}
	}
}
