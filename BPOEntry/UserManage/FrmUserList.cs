using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Common;
using NLog;

namespace BPOEntry.UserManage
{
    /// <summary>
    /// ユーザ一覧
    /// </summary>
    public partial class FrmUserList : Form
    {
        /// <summary>
        /// Log
        /// </summary>
        //private static Logger _Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Dao
        /// </summary>
        private DaoUserManage _dao = new DaoUserManage();

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
        public FrmUserList()
        {
            InitializeComponent();

            //this.Text = Config.BusinessName;
            this.Text = Utils.GetFormText();
            this.lblTitle.BackColor = Config.LblTitleBackColor;

            this.textUserId.Text = string.Empty;
            this.textUserName.Text = string.Empty;
            
            // 一覧表示
            ShowUserList();
            this.textUserId.Focus();
            this.textUserId.SelectAll();
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
        /// 一覧表示
        /// </summary>
        private void ShowUserList()
        {
            //詳細表示にする
            listViewUser.BeginUpdate();
            listViewUser.Clear();
            listViewUser.View = View.Details;

            //ヘッダーを追加する（ヘッダー名、幅、アライメント）
            listViewUser.Columns.Add("dummy", 0, HorizontalAlignment.Center);
            listViewUser.Columns.Add("№", 45, HorizontalAlignment.Center);
            listViewUser.Columns.Add("ユーザID", 180, HorizontalAlignment.Left);
            listViewUser.Columns.Add("ユーザ名", 320, HorizontalAlignment.Left);
            listViewUser.Columns.Add("権限", 100, HorizontalAlignment.Left);
            listViewUser.Columns.Add("状態", 50, HorizontalAlignment.Left);

            int LoginCount = 0;
            var dtUserList = _dao.SELECT_M_USER(textUserId.Text,textUserName.Text,checkBox1.Checked);
            foreach (var drUserList in dtUserList.AsEnumerable())
            {
                var itemx = new ListViewItem();
                //アイテムの作成
                itemx.Text = "*";
                // No
                itemx.SubItems.Add((listViewUser.Items.Count + 1).ToString().PadLeft(3));
                // user_id
                itemx.SubItems.Add(drUserList["user_id"].ToString());
                if (drUserList["login_ip_address"].ToString().Length == 0)
                {
                    itemx.SubItems.Add(drUserList["user_name"].ToString());
                }
                else
                {
                    itemx.SubItems.Add(drUserList["user_name"].ToString() + "（ログイン中）");
                    LoginCount++;
                }
                itemx.SubItems.Add(drUserList["user_kbn_"].ToString());
                itemx.SubItems.Add(drUserList["valid_flag_"].ToString());

                //アイテムをリスビューに追加する
                listViewUser.Items.Add(itemx);

                if (listViewUser.Items.Count % 2 != 0)
                    this.listViewUser.Items[this.listViewUser.Items.Count - 1].BackColor = Color.LightCyan;
            }
            listViewUser.EndUpdate();

            this.label3.Text = $"ログイン中ユーザ：{LoginCount}名";
        }

        /// <summary>
        /// 新規
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNew_Click(object sender, EventArgs e)
        {
            
            //using (var frm = new FrmUserEdit("NEW"))
            //{
            //    frm.ShowDialog();

            //}

            ShowUserEdit("NEW");

            // 再表示
            ShowUserList();
        }

        private void ShowUserEdit(string Mode, string UserId = null)
        {
            using (var frm = new FrmUserEdit(Mode, UserId))
            {
                frm.ShowDialog();
            }
        }

        /// <summary>
        /// 修正
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewUser_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // リスト未設定なら何もしない
            if (listViewUser.Items == null
                || listViewUser.Items.Count == 0)
            {
                return;
            }
            // 未選択なら何もしない
            if (listViewUser.SelectedItems == null
                || listViewUser.SelectedItems.Count <= 0)
            {
                return;
            }

            // user_id
            var UserId = listViewUser.SelectedItems[0].SubItems[2].Text;

            //using (var frm = new FrmUserEdit("UPD", sUserId))
            //{
            //    frm.ShowDialog();
            //}

            ShowUserEdit("UPD", UserId);

            // 再表示
            ShowUserList();
        }

        /// <summary>
        /// 検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            ShowUserList();
        }

        private void textUserName_TextChanged(object sender, EventArgs e)
        {
            ShowUserList();
        }

        private void textUserId_TextChanged(object sender, EventArgs e)
        {
            ShowUserList();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ShowUserList();
        }
    }
}