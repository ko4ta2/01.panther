using Common;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace CodeDefineList
{
    public partial class FrmCodeDefineList : Form
    {
        ///// <summary>
        ///// log
        ///// </summary>
        //private static Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// セクション
        /// </summary>
        public string _Section = null;

        /// <summary>
        /// Seq
        /// </summary>
        public string _Key = null;

        public string _Value1 = null;
        public string _Value2 = null;

        /// <summary>
        /// DataAccessObject
        /// </summary>
        private static daoCodeDefineList _dao = new daoCodeDefineList();

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
        /// <param name="Section"></param>
        public FrmCodeDefineList(string Section)
        {
            InitializeComponent();
            _Section = Section;
            lvAddressList.Height = this.Height - 28;
            lvAddressList.Width = this.Width;

            this.Text = Utils.GetFormText();
        }

        /// <summary>
        /// Form_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmAddressList_Load(object sender, EventArgs e)
        {
            lvAddressList.BeginUpdate();

            //詳細表示にする
            lvAddressList.Clear();
            lvAddressList.View = View.Details;

            //ヘッダーを追加する（ヘッダー名、幅、アライメント）
            lvAddressList.Columns.Add("dummy", 0, HorizontalAlignment.Center);
            lvAddressList.Columns.Add("№", 65, HorizontalAlignment.Center);
            lvAddressList.Columns.Add("コード", 100, HorizontalAlignment.Center);
            lvAddressList.Columns.Add("名称", 650, HorizontalAlignment.Left);
            lvAddressList.Columns.Add("名称2", 0, HorizontalAlignment.Left);

            int iCount = 1;
            // コード定義マスタを検索
            //var dtM_CODE_DEFINE = _dao.SELECT_M_CODE_DEFINE(_Section);
            foreach (var drM_CODE_DEFINE in _dao.SELECT_M_CODE_DEFINE(_Section).AsEnumerable())
            {
                var lvItems = new ListViewItem();
                //アイテムの作成
                lvItems.Text = "*";

                // №
                lvItems.SubItems.Add((lvAddressList.Items.Count + 1).ToString().PadLeft(4));

                // キー
                lvItems.SubItems.Add(drM_CODE_DEFINE["KEY"].ToString());

                lvItems.SubItems.Add(String.Join("　", drM_CODE_DEFINE["VALUE_1"].ToString()));
                lvItems.SubItems.Add(String.Join("　", drM_CODE_DEFINE["VALUE_2"].ToString()));

                //アイテムをリスビューに追加する
                this.lvAddressList.Items.Add(lvItems);
                if ((iCount % 2) != 0)
                    this.lvAddressList.Items[iCount - 1].BackColor = Color.Azure;
                iCount++;
            }
            lvAddressList.EndUpdate();

            // １行目を選択
            if (this.lvAddressList.Items.Count != 0)
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
                        var itemx = lvAddressList.SelectedItems[0];
                        _Key = itemx.SubItems[2].Text;
                        _Value1 = itemx.SubItems[3].Text;
                        _Value2 = itemx.SubItems[4].Text;
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
            var itemx = lvAddressList.SelectedItems[0];
            _Key = itemx.SubItems[2].Text;
            _Value1 = itemx.SubItems[3].Text;
            _Value1 = itemx.SubItems[4].Text;
            this.Close();
            this.DialogResult = DialogResult.OK;
        }
    }
}
