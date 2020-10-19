using Common;
using NLog;
using System;
using System.Windows.Forms;

namespace BPOEntry.DeleteEntryUnit
{
    public partial class FrmDeleteEntryUnit : Form
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

        /// <summary>
        /// dao
        /// </summary>
        private static DaoDeleteEntryUnit dao = new DaoDeleteEntryUnit();

        private const int DTM_GETMONTHCAL = 0x1000 + 8;
        private const int MCM_SETCURRENTVIEW = 0x1000 + 32;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        //public string s = "";
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrmDeleteEntryUnit()
        {
            InitializeComponent();

            InitProc();
        }

        /// <summary>
        /// 初期処理
        /// </summary>
        private void InitProc()
        {
            //this.Text = Config.BusinessName;
            this.Text = Utils.GetFormText();
            this.lblTitle.BackColor = Config.LblTitleBackColor;

            // 1年前を初期表示
            this.dtpImageCaptureDate.Value = System.DateTime.Now.AddYears(-1);
            this.dtpImageCaptureDate.MaxDate = System.DateTime.Now.AddMonths(-3);
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonExecute_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("実行しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                dtpImageCaptureDate.Focus();
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;

                Application.DoEvents();

                dao.Open(Config.DSN);

                dao.BeginTrans();

                var DeleteCount = dao.DELETE_D_ENTRY_UNIT(this.dtpImageCaptureDate.Value.ToString("yyyyMM99"));

                dao.CommitTrans();

                MessageBox.Show(String.Format("終了しました。", this.lblTitle.Text) + Environment.NewLine + String.Format("削除件数：{0}", DeleteCount.ToString("#,0 件")), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                dao.RollbackTrans();
                log.Error(ex);
                MessageBox.Show("例外が発生しました。" + Environment.NewLine + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dao.Close();
                this.Cursor = Cursors.Default;
            }
        }

        private void dtpImageCaptureDate_DropDown(object sender, EventArgs e)
        {
            var dtp = (DateTimePicker)sender;

            IntPtr cal = SendMessage(dtpImageCaptureDate.Handle, DTM_GETMONTHCAL, IntPtr.Zero, IntPtr.Zero);
            SendMessage(cal, MCM_SETCURRENTVIEW, IntPtr.Zero, (IntPtr)1);
        }
    }
}
