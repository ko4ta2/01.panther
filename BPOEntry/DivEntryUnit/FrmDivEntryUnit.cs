using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using BPOEntry.UserManage;
using Common;
using NLog;

namespace BPOEntry.DivideEntryUnit
{
    public partial class frmDivideEntryUnit : Form
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

        /// <summary>
        /// log
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        private static DaoDivideEntryUnit dao = new DaoDivideEntryUnit();

        /// <summary>
        /// StopWatch
        /// </summary>
        private static System.Diagnostics.Stopwatch _sw = new System.Diagnostics.Stopwatch();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public frmDivideEntryUnit()
        {
            InitializeComponent();

            Initialize();
        }

        #region 閉じる
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
        #endregion

        private void Initialize()
        {
            this.Text = Utils.GetFormText();
            this.lblTitle.BackColor = Config.LblTitleBackColor;

            var ds = new List<ItemSet>();
            ds.Add(new ItemSet("********", "********:選択してください"));

            foreach (var dr in dao.SELECT_M_DOC().AsEnumerable())
                ds.Add(new ItemSet(dr["DOC_ID"].ToString(), dr["DOC_NAME"].ToString()));
            DropDownList.DataSource = ds;
            DropDownList.DisplayMember = "ItemDisp";
            DropDownList.ValueMember = "ItemValue";

        }
        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExec_Click(object sender, EventArgs e)
        {
            var sImageCaptureDate = this.dtpCaptureDate.Value.ToString("yyyyMMdd");
            var sImageCaptureNum = this.nudCaptureNum.Value.ToString("00");
            var sDocId = this.DropDownList.SelectedValue.ToString();
            int iCaptureCount = int.Parse(this.nudCaptureCount.Value.ToString());

            // 連携年月日、回数で登録済チェック
            if (dao.SELECT_D_ENTRY_UNIT(sImageCaptureDate, sImageCaptureNum) != 0)
            {
                MessageBox.Show("エントリ単位分割済です。\n回数を変更してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.nudCaptureCount.Focus();
                return;
            }

            // 帳票選択チェック
            if ("********".Equals(sDocId))
            {
                MessageBox.Show("帳票を選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DropDownList.Focus();
                return;
            }

            // 件数チェック
            if (iCaptureCount == 0)
            {
                MessageBox.Show("件数を入力してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.nudCaptureCount.Focus();
                return;
            }

            // 実行確認
            if (MessageBox.Show(String.Format("実行しますか？"), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                this.dtpCaptureDate.Focus();
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                Application.DoEvents();
                _sw.Start();

                int iTotalCount = new DivideEntryUnitDivider().DivideEntryunit(sImageCaptureDate, sImageCaptureNum, sDocId, iCaptureCount);

                _sw.Stop();

                this.Cursor = Cursors.Default;

                log.Info("経過時間：{0}", _sw.Elapsed);
                MessageBox.Show(String.Format("終了しました。") + Environment.NewLine + String.Format("処理件数:{0}", iTotalCount.ToString("#,0件")), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(String.Format("例外が発生しました。") + Environment.NewLine + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.dtpCaptureDate.Focus();
                this.Cursor = Cursors.Default;
            }
        }

        private void nudImageCaptureNum_Enter(object sender, EventArgs e)
        {
            this.nudCaptureNum.Select(0, this.nudCaptureNum.Value.ToString().Length);
        }

        private void nudCaptureCount_Enter(object sender, EventArgs e)
        {
            this.nudCaptureCount.Select(0, this.nudCaptureCount.Value.ToString().Length);
        }
    }
}
