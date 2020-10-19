using BPOEntry.UserManage;
using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace BPOEntry.SelectGyomu
{
    public partial class FrmSelectGyomu : Form
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

        private static DaoSelectGyomu dao = new DaoSelectGyomu();

        public FrmSelectGyomu()
        {
            InitializeComponent();

            this.Text = Utils.GetFormText();
            this.lblTitle.BackColor = Config.LblTitleBackColor;

            var src = new List<ItemSet>();
            foreach (var dr in dao.SELECT_M_BUSINESS().AsEnumerable())
                src.Add(new ItemSet(String.Join("_", dr["BUSINESS_ID"].ToString(), dr["TKSK_CD"].ToString(), dr["HNM_CD"].ToString(), dr["BUSINESS_NAME"].ToString())
                                   , String.Join(":", dr["BUSINESS_ID"].ToString(), dr["BUSINESS_NAME"].ToString())));

            DropDownList.DataSource = src;
            DropDownList.DisplayMember = "ItemDisp";
            DropDownList.ValueMember = "ItemValue";

            foreach (var s in src)
            {
                if(Config.UserId.Equals(s.ItemValue.Split('_')[0]))
                {
                    DropDownList.SelectedValue = s.ItemValue;
                    break;
                }
            }
        }

        /// <summary>
        /// 選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonExec_Click(object sender, EventArgs e)
        {
            var SelectedKey = this.DropDownList.SelectedValue.ToString().Split('_');

            Config.UserId = SelectedKey[0];              // 業務ID
            Config.TokuisakiCode = SelectedKey[1];       // 得意先コード
            Config.HinmeiCode = SelectedKey[2];          // 品名コード
            Config.CompanyName = SelectedKey[3];         // 得意先名
            if (SelectedKey[4].Length != 0)
                Config.BusinessName = SelectedKey[4];    // 業務名

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
            //this.Dispose();
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
