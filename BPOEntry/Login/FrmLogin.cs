using BPOEntry.Menu;
using BPOEntry.SelectGyomu;
using Common;
using System;
using System.Windows.Forms;
using Dao;

namespace BPOEntry.Login
{
    public partial class FrmLogin : Form
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

        private static DaoEntry _dao = new DaoEntry();

        /// <summary>
        /// ログイン
        /// </summary>
        public FrmLogin()
        {
            InitializeComponent();

            this.Text = Utils.GetFormLoginText(true);
            this.label1.Text = String.Format("Ver.{0}", Utils.GetVersion());
            this.lblTitle.BackColor = Config.LblTitleBackColor;

            this.lblTitle.Text = Utils.GetFormLoginText();
            if (this.lblTitle.Text.Length >= 24)
                this.lblTitle.Font = new System.Drawing.Font("Meiryo UI", 14f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            else if (this.lblTitle.Text.Length >= 20)
                this.lblTitle.Font = new System.Drawing.Font("Meiryo UI", 16f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            else if (this.lblTitle.Text.Length >= 16)
                this.lblTitle.Font = new System.Drawing.Font("Meiryo UI", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            else
                this.lblTitle.Font = new System.Drawing.Font("Meiryo UI", 20f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));

            Application.DoEvents();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        public void Clear(int mode = 0)
        {
            if (mode == 0)
            {
                this.TextUserId.Text = string.Empty;
                this.TextPassword.Text = string.Empty;
            }
            else
            {
                this.TextPassword.Text = string.Empty;
            }
        }
        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            var result = LoginControl.Login(this.TextUserId.Text, this.TextPassword.Text);
            if (!result.Item1)
            {
                // ログイン失敗
                MessageBox.Show(result.Item2, Utils.GetFormText(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.TextUserId.Focus();
                this.TextUserId.SelectAll();
                this.TextPassword.Text = string.Empty;
                return;
            }

            if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG)
                && Consts.DEFAULT_USER_ID.Equals(Config.UserId))
            {
                var dt = _dao.SELECT_M_BUSINESS();
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("業務が設定されていません。", Utils.GetFormText(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.TextUserId.Focus();
                    this.TextUserId.SelectAll();
                    return;
                }
                else if (dt.Rows.Count == 1)
                {
                    Config.UserId = dt.Rows[0]["BUSINESS_ID"].ToString();           // 業務ID
                    Config.TokuisakiCode= dt.Rows[0]["TKSK_CD"].ToString();         // 得意先コード
                    Config.HinmeiCode = dt.Rows[0]["HNM_CD"].ToString();            // 品名コード
                    Config.BusinessName = dt.Rows[0]["BUSINESS_NAME"].ToString();   // 業務名
                }
                else
                {
                    // 業務選択
                    using (var frm = new FrmSelectGyomu())
                    {
                        // ログイン画面を非表示にして業務選択画面を表示します。
                        this.Hide();
                        frm.ShowDialog(this);

                        if (frm.DialogResult == DialogResult.Cancel)
                        {
                            // メニューを閉じたらログイン画面を再表示します。
                            this.Clear(1);
                            this.Show();

                            // ログイン情報クリア
                            var dao = new DaoLogin();
                            dao.UPDATE_M_USER(this.TextUserId.Text, null, false);

                            this.Activate();
                            this.TextUserId.Focus();
                            return;
                        }
                    }
                }
            }

            // メニュー画面表示
            using (var menu = new FrmMenu())
            {
                // ログイン画面を非表示にしてメニュー画面を表示します。
                this.Hide();
                menu.ShowDialog();

                // メニューを閉じたらログイン画面を再表示します。
                this.Clear(1);
                this.Show();

                // ログイン情報クリア
                var dao = new DaoLogin();
                dao.UPDATE_M_USER(this.TextUserId.Text, null, false);

                this.Activate();
                this.TextUserId.Focus();
            }

            // ログイン情報クリア
            //Config.UserId = string.Empty;
            //Config.TokuisakiCode = string.Empty;
            //Config.HinmeiCode = string.Empty;
            //Config.BusinessName = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonExit_Click(object sender, EventArgs e)
        {
            EndProc();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUserID_Enter(object sender, EventArgs e)
        {
            this.TextUserId.SelectAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUserPass_Enter(object sender, EventArgs e)
        {
            this.TextPassword.SelectAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                EndProc();
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndProc()
        {
            if (MessageBox.Show("終了しますか？", Utils.GetFormText(), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;
            LoginControl.Close();
            this.Close();
            this.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmLogin_Load(object sender, EventArgs e)
        {
            LoginControl.Load();
        }
    }
}
