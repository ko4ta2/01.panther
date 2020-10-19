using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using NLog;
using System.Security.Permissions;
using Common;

namespace BPOEntry.UserManage
{
    /// <summary>
    /// ユーザ編集
    /// </summary>
    public partial class FrmUserEdit : Form
    {
        /// <summary>
        /// Log
        /// </summary>
        private static Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Dao
        /// </summary>
        private DaoUserManage _dao = new DaoUserManage();

        /// <summary>
        /// 処理モード
        /// </summary>
        private string _Mode;

        /// <summary>
        /// ユーザID
        /// </summary>
        private string _UserId;

        /// <summary>
        /// ショートカットキーなどでフォームが閉じられないようにします。
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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="Mode"></param>
        /// <param name="UserId"></param>
        public FrmUserEdit(string Mode, string UserId = null)
        {
            InitializeComponent();

            //this.Text = Config.BusinessName;
            this.Text = Utils.GetFormText();
            this.lblTitle.BackColor = Config.LblTitleBackColor;

            _Mode = Mode;
            _UserId = UserId;

            if ("UPD".Equals(_Mode))
            {
                this.lblTitle.Text = "ユーザ編集";
                TextBoxUserId.Enabled = false;
                ButtonExec.Text = "更新";
            }
            else
            {
                this.lblTitle.Text = "ユーザ登録";
                TextBoxUserId.Enabled = true;
                ButtonExec.Text = "登録";
            }

            var src = new List<ItemSet>();
            src.Add(new ItemSet("1", "入力担当"));
            src.Add(new ItemSet("0", "管理者"));
            //src.Add(new ItemSet("S", "システム管理者"));

            ComboBoxAuthority.DataSource = src;
            ComboBoxAuthority.DisplayMember = "ItemDisp";
            ComboBoxAuthority.ValueMember = "ItemValue";
            ComboBoxAuthority.SelectedValue = "1";

            ShowUserItems();
        }

        #region 閉じるボタン
        private void btnClose_Click(object sender, EventArgs e)
        {
            EndProc();
        }
        #endregion

        private void EndProc()
        {
            this.Close();
            this.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ShowUserItems()
        {
            if ("UPD".Equals(_Mode))
            {
                var dtUser = _dao.SELECT_M_USER(_UserId);
                //if (dtUser.Rows.Count != 0)
                //{
                // ユーザID
                TextBoxUserId.Text = dtUser.Rows[0]["user_id"].ToString();
                // ユーザ名
                TextBoxUserName.Text = dtUser.Rows[0]["user_name"].ToString();
                // パスワード
                TextBoxPassword.Text = dtUser.Rows[0]["password"].ToString();
                // パスワード（確認）
                TextBoxPassword2.Text = dtUser.Rows[0]["password"].ToString();
                // valid_flag
                //if (Consts.Flag.ON.Equals(dtUser.Rows[0]["valid_flag"].ToString()))
                //{
                // 有効フラグ
                CheckBoxValid.Checked = Consts.Flag.ON.Equals(dtUser.Rows[0]["valid_flag"].ToString()) ? true : false;
                //}
                //else
                //{
                //    CheckBoxValid.Checked = false;
                //}
                // 権限
                if ("0".Equals(dtUser.Rows[0]["user_kbn"].ToString())
                    || "1".Equals(dtUser.Rows[0]["user_kbn"].ToString())
                    || "S".Equals(dtUser.Rows[0]["user_kbn"].ToString())
                    /*|| "2".Equals(dtUser.Rows[0]["user_kbn"].ToString())*/)
                {
                    ComboBoxAuthority.SelectedValue = dtUser.Rows[0]["user_kbn"].ToString();
                }

                // ログイン開始時刻
                label8.Text = dtUser.Rows[0]["login_start_date"].ToString();

                // 自身を更新する場合、ログイン開始時刻は解除不可    
                if (TextBoxUserId.Text.Equals(Program.LoginUser.USER_ID))
                    checkBox1.Enabled = false;
                //}
            }
            else
            {
                // user_id
                TextBoxUserId.Text = string.Empty;
                // user_name
                TextBoxUserName.Text = string.Empty;
                // password
                TextBoxPassword.Text = string.Empty;
                TextBoxPassword2.Text = string.Empty;
                // valid_flag
                CheckBoxValid.Checked = false;
                // user_kbn
                ComboBoxAuthority.SelectedValue = "1";
                checkBox1.Enabled = false;
            }
        }

        private bool IsValidCondition()
        {
            if (TextBoxUserId.Text.Length == 0)
            {
                MessageBox.Show("ユーザIDが入力されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                TextBoxUserId.Focus();
                TextBoxUserId.SelectAll();
                return false;
            }
            else if (TextBoxPassword.Text.Length == 0)
            {
                MessageBox.Show("パスワードが入力されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                TextBoxPassword.Focus();
                TextBoxPassword.SelectAll();
                return false;
            }
            else if (TextBoxPassword.Text != TextBoxPassword2.Text)
            {
                MessageBox.Show("パスワードとパスワード（確認）が異なります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                TextBoxPassword.Focus();
                TextBoxPassword.SelectAll();
                return false;
            }

            // check     
            if ("NEW".Equals(_Mode))
            {
                if (_dao.SELECT_M_USER(TextBoxUserId.Text).Rows.Count != 0)
                {
                    MessageBox.Show("既に登録済のユーザIDです。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    TextBoxUserId.Focus();
                    TextBoxUserId.SelectAll();
                    return false;
                }
            }
            return true;
        }

        private void btnExec_Click(object sender, EventArgs e)
        {
            //if (textUserId.Text.Length == 0)
            //{
            //    MessageBox.Show("ユーザIDが入力されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    textUserId.Focus();
            //    textUserId.SelectAll();
            //    return;
            //}
            //else if (textPassword.Text.Length == 0)
            //{
            //    MessageBox.Show("パスワードが入力されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    textPassword.Focus();
            //    textPassword.SelectAll();
            //    return;
            //}
            //else if (textPassword.Text != textPassword2.Text)
            //{
            //    MessageBox.Show("パスワードとパスワード（確認）が異なります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    textPassword.Focus();
            //    textPassword.SelectAll();
            //    return;
            //}

            if (!IsValidCondition())
                return;

            //string sMessage = string.Empty;
            //if ("NEW".Equals(_sMode))
            //{
            //    // check
            //    if (_dao.SELECT_M_USER(textUserId.Text).Rows.Count != 0)
            //    {
            //        MessageBox.Show("既に登録済のユーザIDです。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        textUserId.Focus();
            //        textUserId.SelectAll();
            //        return;
            //    }
            //    sMessage = "登録";
            //}
            //else if ("UPD".Equals(_sMode))
            //{
            //    sMessage = "更新";
            //}

            if (MessageBox.Show($"{this.ButtonExec.Text}します。\nよろしいですか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _dao.BeginTrans();
                if ("NEW".Equals(_Mode))
                {
                    // insert
                    _dao.INSERT_M_USER(TextBoxUserId.Text
                                      , TextBoxUserName.Text
                                      , TextBoxPassword.Text
                                      , ComboBoxAuthority.SelectedValue.ToString()
                                      , CheckBoxValid.Checked ? Consts.Flag.ON : Consts.Flag.OFF);
                    _log.Info("登録：ユーザID：{0} ユーザ名：{1} パスワード：{2} 権限：{3} 状態：{4}"
                              , TextBoxUserId.Text
                              , TextBoxUserName.Text
                              , string.Empty.PadLeft(TextBoxPassword.Text.Length, '*')
                              , ComboBoxAuthority.SelectedValue.ToString()
                              , CheckBoxValid.Checked ? Consts.Flag.ON : Consts.Flag.OFF);
                }
                else if ("UPD".Equals(_Mode))
                {
                    // update
                    _dao.UPDATE_M_USER(TextBoxUserId.Text
                                      , TextBoxUserName.Text
                                      , TextBoxPassword.Text
                                      , ComboBoxAuthority.SelectedValue.ToString()
                                      , CheckBoxValid.Checked ? Consts.Flag.ON : Consts.Flag.OFF
                                      , checkBox1.Checked ? true : false);
                    _log.Info("更新：ユーザID：{0} ユーザ名：{1} パスワード：{2} 権限：{3} 状態：{4}"
                              , TextBoxUserId.Text
                              , TextBoxUserName.Text
                              , string.Empty.PadLeft(TextBoxPassword.Text.Length, '*')
                              , ComboBoxAuthority.SelectedValue.ToString()
                              , CheckBoxValid.Checked ? Consts.Flag.ON : Consts.Flag.OFF                              );
                }
                _dao.CommitTrans();

                // 閉じる
                EndProc();
                //this.Close();
                //this.Dispose();
            }
            catch
            {
                _dao.RollbackTrans();
                throw;
            }
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            tb.SelectAll();
            if ("textUserName".ToUpper().Equals(tb.Name.ToUpper()))
            {
                tb.ImeMode = ImeMode.Hiragana;
            }
        }
    }

    public class ItemSet
    {
        // DisplayMemberとValueMemberにはプロパティで指定する仕組み
        public string ItemDisp { get; set; }
        public string ItemValue { get; set; }

        // プロパティをコンストラクタでセット
        public ItemSet(string v, string s)
        {
            ItemDisp = s;
            ItemValue = v;
        }
    }
}
