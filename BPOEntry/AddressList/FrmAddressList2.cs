using System;
using System.Data;
using System.Windows.Forms;
using NLog;
using System.Drawing;

namespace AddressList.Address
{
    public partial class FrmAddressList2 : Form
    {
        ///// <summary>
        ///// log
        ///// </summary>
        //private static Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 郵便番号
        /// </summary>
        public string _ZipCode = null;

        /// <summary>
        /// Seq
        /// </summary>
        public int _Seq = 0;

        /// <summary>
        /// DataAccessObject
        /// </summary>
        private static daoAddressList _dao = new daoAddressList();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sAddress"></param>
        public FrmAddressList2(string sZipCode)
        {
            InitializeComponent();
            _ZipCode = sZipCode;
        }

        /// <summary>
        /// Form_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmAddressList_Load(object sender, EventArgs e)
        {
            lvAddressList.BeginUpdate();
            lvAddressList.Height = this.Height - 24;
            lvAddressList.Width = this.Width;

            //詳細表示にする
            lvAddressList.Clear();
            lvAddressList.View = View.Details;

            //ヘッダーを追加する（ヘッダー名、幅、アライメント）
            lvAddressList.Columns.Add("dummy", 0, HorizontalAlignment.Center);
            lvAddressList.Columns.Add("№", 60, HorizontalAlignment.Center);
            lvAddressList.Columns.Add("SEQ", 0, HorizontalAlignment.Center);    // 選択者に関係無いので非表示
            lvAddressList.Columns.Add("住所", 800, HorizontalAlignment.Left);

            int iCount = 1;
            // 郵便番号マスタを検索
            var dtM_ZIP_CODE = _dao.SELECT_M_ZIP_CODE(_ZipCode);
            foreach (DataRow drM_ADDRESS in dtM_ZIP_CODE.Rows)
            {
                ListViewItem lvItems = new ListViewItem();
                //アイテムの作成
                lvItems.Text = "*";

                // №
                lvItems.SubItems.Add((lvAddressList.Items.Count + 1).ToString().PadLeft(4));

                lvItems.SubItems.Add(drM_ADDRESS["SEQ"].ToString());

                // 住所２～４
                if (String.Join(drM_ADDRESS["ADDRESS_1"].ToString(), drM_ADDRESS["ADDRESS_2"].ToString(), drM_ADDRESS["ADDRESS_3"].ToString(), drM_ADDRESS["ADDRESS_4"].ToString()).Length > 44)
                {
                    lvItems.SubItems.Add(String.Join("　", drM_ADDRESS["ADDRESS_2"].ToString(), drM_ADDRESS["ADDRESS_3"].ToString(), drM_ADDRESS["ADDRESS_4"].ToString()));
                }
                else
                {
                    lvItems.SubItems.Add(String.Join("　", drM_ADDRESS["ADDRESS_1"].ToString(), drM_ADDRESS["ADDRESS_2"].ToString(), drM_ADDRESS["ADDRESS_3"].ToString(), drM_ADDRESS["ADDRESS_4"].ToString()));
                }

                //アイテムをリスビューに追加する
                this.lvAddressList.Items.Add(lvItems);
                if ((iCount % 2) != 0)
                {
                    this.lvAddressList.Items[iCount - 1].BackColor = Color.Azure;
                }
                iCount++;
            }
            lvAddressList.EndUpdate();

            // １行目を選択
            this.lvAddressList.Items[0].Selected = true;
        }

        /// <summary>
        /// Form_KeyDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmAddressList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    //「Esc」で閉じる
                    this.Close();
                    this.DialogResult = DialogResult.Cancel;
                    break;
                case Keys.Enter:
                    // 「Enter」で選択
                    if (lvAddressList.SelectedItems.Count != 0)
                    {
                        ListViewItem itemx = lvAddressList.SelectedItems[0];
                        _Seq = int.Parse(itemx.SubItems[2].Text);
                        this.Close();
                        this.DialogResult = DialogResult.OK;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// ListView_DoubleClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvAddressList_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem itemx = lvAddressList.SelectedItems[0];
            _Seq = int.Parse(itemx.SubItems[2].Text);
            this.Close();
            this.DialogResult = DialogResult.OK;
        }
    }
}
