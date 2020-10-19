using System;
using System.Data;
using System.Windows.Forms;
using NLog;
using System.Drawing;

namespace AddressEntry.Address
{
    public partial class FrmAddressList : Form
    {
        ///// <summary>
        ///// log
        ///// </summary>
        //private static Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 住所コード
        /// </summary>
        public string _sAddressCd = null;

        /// <summary>
        /// 郵便番号
        /// </summary>
        public string _sPostalCd = null;

        /// <summary>
        /// 住所
        /// </summary>
        public string _sAddress = null;

        /// <summary>
        /// DataAccessObject
        /// </summary>
        private static DaoAddress _dao = new DaoAddress();

        /// <summary>
        /// 最大表示件数
        /// </summary>
        private const int iMAX_RECORD = 100000;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sAddress"></param>
        public FrmAddressList(string sPostalCd, string sAddress, string sAddressCd)
        {
            InitializeComponent();
            _sAddress = sAddress;
            _sPostalCd = sPostalCd;
        }

        /// <summary>
        /// Form_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmAddressList_Load(object sender, EventArgs e)
        {
            // 親画面で入力された住所の文字列長を取得
            int iIdx = _sAddress.Length;
            while (true)
            {
                if (iIdx == 0)
                {
                    MessageBox.Show("住所コードを取得出来ません", "住所検索", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.Close();
                    return;
                }

                // 住所マスタを検索
                DataTable dtM_ADDRESS = _dao.SelectM_ADDRESS(_sPostalCd,_sAddress.Substring(0, iIdx));
                if (dtM_ADDRESS.Rows.Count != 0)
                {
                    #region 検索結果大杉
                    if (dtM_ADDRESS.Rows.Count > iMAX_RECORD)
                    {
                        MessageBox.Show(String.Format("検索結果が{0}件を超えています。", iMAX_RECORD.ToString("#,0")), "住所検索", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.Close();
                        return;
                    }
                    #endregion

                    #region 完全一致で１件の場合は自動採用
                    if (dtM_ADDRESS.Rows.Count == 1
                        //&& dtM_ADDRESS.Rows[0]["ADDRESS_ALL"].ToString().Replace("大字", string.Empty).Equals(_sAddress))
                        && dtM_ADDRESS.Rows[0]["ADDRESS_ALL"].ToString().Equals(_sAddress))
                    {
                        _sAddressCd = dtM_ADDRESS.Rows[0]["ADDRESS_CD"].ToString();
                        _sAddress = dtM_ADDRESS.Rows[0]["ADDRESS_ALL"].ToString();
                        this.Close();
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        return;
                    }
                    #endregion

                    #region 表示対象がある場合のみリストビューを構築
                    lvAddressList.BeginUpdate();
                    lvAddressList.Height = this.Height - 24;
                    lvAddressList.Width = this.Width;

                    //詳細表示にする
                    lvAddressList.Clear();
                    lvAddressList.View = View.Details;

                    //ヘッダーを追加する（ヘッダー名、幅、アライメント）
                    lvAddressList.Columns.Add("dummy", 0, HorizontalAlignment.Center);
                    lvAddressList.Columns.Add("№", 60, HorizontalAlignment.Center);
                    lvAddressList.Columns.Add("郵便番号", 100, HorizontalAlignment.Center);
                    lvAddressList.Columns.Add("住所コード", 0/*146*/, HorizontalAlignment.Left);
                    lvAddressList.Columns.Add("住所", 700, HorizontalAlignment.Left);
                    //System.Windows.Forms.Application.DoEvents();

                    // リストビュー
                    int iCount = 1;
                    foreach (DataRow drM_ADDRESS in dtM_ADDRESS.Rows)
                    {
                        ListViewItem lvItems = new ListViewItem();
                        //アイテムの作成
                        lvItems.Text = "*";

                        // №
                        lvItems.SubItems.Add((lvAddressList.Items.Count + 1).ToString().PadLeft(4));

                        // 郵便番号
                        lvItems.SubItems.Add(drM_ADDRESS["POSTAL_CD"].ToString());

                        // 住所コード
                        lvItems.SubItems.Add(drM_ADDRESS["ADDRESS_CD"].ToString());

                        //
                        //drM_ADDRESS["ADDRESS_ALL"] = drM_ADDRESS["ADDRESS_ALL"].ToString() + "_(" + drM_ADDRESS["OYA_FLAG"].ToString() + ")";

                        // 住所
                        if (drM_ADDRESS["HAISHI_YYYYMM"].ToString().Length == 0)
                        {
                            lvItems.SubItems.Add(drM_ADDRESS["ADDRESS_ALL"].ToString());
                        }
                        else
                        {
                            lvItems.SubItems.Add(drM_ADDRESS["ADDRESS_ALL"].ToString() + "（" + int.Parse(drM_ADDRESS["HAISHI_YYYYMM"].ToString()).ToString("0000/00") + "廃止）");
                        }
                        
                        //アイテムをリスビューに追加する
                        this.lvAddressList.Items.Add(lvItems);
                        if ((iCount % 2) == 0)
                        {
                            this.lvAddressList.Items[iCount-1].BackColor = Color.Azure;
                        }
                        iCount++;
                    }
                    lvAddressList.EndUpdate();

                    // 件数
                    this.lblCount.Text = String.Format("検索結果:{0}件", dtM_ADDRESS.Rows.Count.ToString("#,0"));
                    #endregion
                    break;
                }
                iIdx--;
            }
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
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    break;
                case Keys.Enter:
                    // 「Enter」で選択
                    if (lvAddressList.SelectedItems.Count != 0)
                    {
                        ListViewItem itemx = lvAddressList.SelectedItems[0];
                        _sPostalCd = itemx.SubItems[2].Text;
                        _sAddressCd = itemx.SubItems[3].Text;
                        _sAddress = itemx.SubItems[4].Text;
                        this.Close();
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
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
            // ダブルクリックで選択
            ListViewItem itemx = lvAddressList.SelectedItems[0];
            _sPostalCd = itemx.SubItems[2].Text;
            _sAddressCd = itemx.SubItems[3].Text;
            _sAddress = itemx.SubItems[4].Text;
            this.Close();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
