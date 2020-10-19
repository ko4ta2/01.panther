using Common;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BPOEntry.ExportEntryUnit
{
    public partial class FrmExportResult : Form
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

        Bitmap _canvas = null;
        
        public FrmExportResult(string sMessage)
        {
            InitializeComponent();

            //this.Text = Config.BusinessName;
            this.Text = Utils.GetFormText();

            this.textBox1.Text = sMessage;

            _canvas = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            //ImageオブジェクトのGraphicsオブジェクトを作成する
            Graphics g = Graphics.FromImage(_canvas);
            g.DrawIcon(SystemIcons.Information, 0, 0);
            g.Dispose();

            //PictureBox1に表示する
            pictureBox1.Image = _canvas;

            this.ButtonOK.Focus();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            _canvas.Dispose();
            this.Close();
            this.Dispose();
        }
    }
}
