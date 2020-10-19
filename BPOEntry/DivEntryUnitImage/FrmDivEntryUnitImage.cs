using System;
using System.IO;
using System.Windows.Forms;
using Common;
using NLog;

namespace BPOEntry.DivideEntryUnitImage
{
    public partial class frmDivideEntryUnitImage : Form
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
        /// StopWatch
        /// </summary>
        private static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public frmDivideEntryUnitImage()
        {
            InitializeComponent();

            //this.Text = Config.BusinessName;
            this.Text = Utils.GetFormText();
            this.lblTitle.BackColor = Config.LblTitleBackColor;

            //if (Utils.IsRLI())
            //    this.lblTitle.Text = "エントリ単位分割";
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

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExec_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(String.Format("実行しますか？"), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                dtpImageCaptureDate.Focus();
                return;
            }

            // イメージ連携年月日
            string sImageCaptureDate = this.dtpImageCaptureDate.Value.ToString("yyyyMMdd");
            // イメージ連携回数
            string sImageCaptureNum = this.nudImageCaptureNum.Value.ToString("00");

            #region DWFC待ち
            if (Consts.Flag.ON.Equals(Config.UseDWFC))
            {
                this.Cursor = Cursors.WaitCursor;
                var sTargetPath = Path.Combine(Config.DivImageRoot, sImageCaptureDate + sImageCaptureNum);

                if (!System.IO.Directory.Exists(sTargetPath))
                {
                    MessageBox.Show(String.Format("対象のディレクトリが見つかりません。"), this.lblTitle.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Cursor = Cursors.Default;
                    dtpImageCaptureDate.Focus();
                    return;
                }

                int iTimes = 0;
                while (iTimes <= 10)
                {
                    iTimes++;
                    int iFileCountOld = System.IO.Directory.GetFiles(sTargetPath, "*", System.IO.SearchOption.AllDirectories).Length;
                    System.Threading.Thread.Sleep(10000);
                    int iFileCountNew = System.IO.Directory.GetFiles(sTargetPath, "*", System.IO.SearchOption.AllDirectories).Length;
                    if (iFileCountNew == iFileCountOld)
                    {
                        break;
                    }
                }
                if (iTimes > 10)
                {
                    MessageBox.Show(String.Format("イメージ連携処理中です。"), this.lblTitle.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Cursor = Cursors.Default;
                    dtpImageCaptureDate.Focus();
                    return;
                }
                this.Cursor = Cursors.Default;
            }
            #endregion

            try
            {
                this.Cursor = Cursors.WaitCursor;
                Application.DoEvents();
                sw.Start();

                int iTotalCount = new DivideEntryUnitImageDivider().DivideEntryunitImage(sImageCaptureDate, sImageCaptureNum);

                sw.Stop();

                this.Cursor = Cursors.Default;

                log.Info("経過時間：{0}", sw.Elapsed);
                MessageBox.Show(String.Format("終了しました。") + Environment.NewLine + String.Format("処理件数:{0}", iTotalCount.ToString("#,0件")), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (DirectoryNotFoundException)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(String.Format("ディレクトリが見つかりません。"), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FileNotFoundException)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(String.Format("イメージファイルが見つかりません。"), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(String.Format("例外が発生しました。") + Environment.NewLine + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.dtpImageCaptureDate.Focus();
                this.Cursor = Cursors.Default;
            }
        }

        private void nudImageCaptureNum_Enter(object sender, EventArgs e)
        {
            this.nudImageCaptureNum.Select(0, this.nudImageCaptureNum.Value.ToString().Length);
        }
    }
}
