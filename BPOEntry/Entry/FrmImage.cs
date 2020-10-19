using Common;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BPOEntry.EntryForms
{
    public partial class FrmImage : Form
    {
        #region CreateParams
        /// <summary>
        /// CreateParams
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

        //表示する画像
        public Image _CurrentImage = null;
        public Image _CurrentBaseImage = null;

        //private Bitmap CurrentImage;

        // 初期ズーム率
        private double _DefaultImageZoomRate = 0.0d;

        // ズーム率
        private double _ImageZoomRate = 0.0d;

        // ズーム率
        private double _BaseImageZoomRate = 0.0d;

        // ズームカウント
        private int _ImageZoomCount = 0;

        // ズームカウント
        private int _BaseImageZoomCount = 0;

        // イメージマスク
        private string _ImageMaskStyle = "0";

        // ズーム比率
        private readonly double _ImageZoomRatio = 1.10d;

        // ズーム比率
        private readonly double _BaseImageZoomRatio = 1.10d;

        //倍率変更後の画像のサイズと位置
        private Rectangle _DrawRectangle;

        private Rectangle _BaseImageDrawRectangle;

        private bool IsShowBaseImage = false;

        /// <summary>
        /// log
        /// </summary>
        protected static Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 回転カウンタ
        /// </summary>
        private int _RotateCounter = 0;
        private int _BaseRotateCounter = 0;

        private int _ImaegPostion_X = 0;

        private int _ImaegPostion_Y = 0;

        private int _BaseImaegPostion_X = 0;

        private int _BaseImaegPostion_Y = 0;


        private int _MaxVerticalScrollValue = 0;

        private int _MaxHorizontalScrollValue = 0;

        private int _BaseMaxVerticalScrollValue = 0;

        private int _BaseMaxHorizontalScrollValue = 0;

        //public bool _turbo = true;

        /// <summary>
        /// frmBPOImage
        /// </summary>
        public FrmImage()
        {
            InitializeComponent();

            this.Text = Utils.GetFormText();
            this.lblTitle.BackColor = Config.LblTitleBackColor;

            //プライマリディスプレイの作業領域の高さと幅を取得
            this.Top = 0;
            this.Left = 0;// 5;
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;// - 2;
            this.Width = Screen.PrimaryScreen.WorkingArea.Width / 2;// - 5;

            this.pnlImage.Height = this.Height - this.stsImagePath.Height - 54; // 80;
            this.pnlImage.Width = this.Width - 5;

            this.pnlBaseImage.Top = this.pnlImage.Top;
            this.pnlBaseImage.Left = this.pnlImage.Left;

            this.pnlBaseImage.Height = this.pnlImage.Height; // 80;
            this.pnlBaseImage.Width = this.pnlImage.Width;

            this.pnlBaseImage.Visible = false;

            this.lblImageHighlight.Parent = this.pbImage;

            this.lblImageMask.Text = string.Empty;
            this.lblImageMask.Parent = this.pbImage;
            this.lblImageMask.BackColor = Color.FromArgb(170, Color.Black);
            this.lblImageMask.Location = new Point(0, 0);

            var FuncList = new List<string>();
            FuncList.Add("F1:↓");
            FuncList.Add("F2:↑");
            FuncList.Add("F3:→");
            FuncList.Add("F4:←");
            FuncList.Add("F5:拡大");
            FuncList.Add("F6:縮小");
            FuncList.Add("F7:右回転");

            if (Consts.BusinessID.MYD.Equals(Utils.GetBussinessId())
                || Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
            {
                FuncList.Add("F8:画像切替");
            }

            if (Consts.BusinessID.MYD.Equals(Utils.GetBussinessId()))
            {
                FuncList.Add("F9:項目jump");
            }
            this.lblImageZoomRate.Text = String.Join("、", FuncList.ToArray());
        }

        public void ChangePage(bool forward)
        {
            if (!Consts.BusinessID.MYD.Equals(Utils.GetBussinessId()))
                return;
            if (forward)
                this.PanelImage_AutoScrollPosition = new Point(0, this.pbImage.Height / 2);
            else
                this.PanelImage_AutoScrollPosition = new Point(0, 0);
        }

        #region エントリ画面とやりとり
        public void ImageRotate()
        {
            if (!IsShowBaseImage)
            {
                this._RotateCounter++;
                if (this._RotateCounter == 4)
                    this._RotateCounter = 0;

                if (this._RotateCounter == 0)
                {
                    if (!"0".Equals(this._ImageMaskStyle))
                        this.lblImageHighlight.Visible = true;
                    if ("2".Equals(this._ImageMaskStyle))
                        this.lblImageMask.Visible = true;
                }
                else
                {
                    this.lblImageHighlight.Visible = false;
                    this.lblImageMask.Visible = false;
                }

                var iWidth = _DrawRectangle.Width;
                var iHeight = _DrawRectangle.Height;

                //初期化
                this._DrawRectangle = new Rectangle(0, 0, iHeight, iWidth);
                this._DrawRectangle.Width = (int)(iHeight);
                this._DrawRectangle.Height = (int)(iWidth);
                this._DrawRectangle.X = 0;
                this._DrawRectangle.Y = 0;
                this.pbImage.Width = _DrawRectangle.Width;
                this.pbImage.Height = _DrawRectangle.Height;
                this._CurrentImage.RotateFlip(RotateFlipType.Rotate90FlipNone);

                this.pnlImage.AutoScrollPosition = new Point(1000000, 1000000);
                this.IMaxVerticalScrollValue = this.pnlImage.VerticalScroll.Value;
                this.IMaxHorizontalScrollValue = this.pnlImage.HorizontalScroll.Value;

                this.pbImage.Refresh();
            }
            else
            {
                this._BaseRotateCounter++;
                if (this._BaseRotateCounter == 4)
                    this._BaseRotateCounter = 0;

                var iWidth = _BaseImageDrawRectangle.Width;
                var iHeight = _BaseImageDrawRectangle.Height;

                //初期化
                this._BaseImageDrawRectangle = new Rectangle(0, 0, iHeight, iWidth);
                this._BaseImageDrawRectangle.Width = (int)(iHeight);
                this._BaseImageDrawRectangle.Height = (int)(iWidth);
                this._BaseImageDrawRectangle.X = 0;
                this._BaseImageDrawRectangle.Y = 0;

                this.pbBaseImage.Top = _BaseImageDrawRectangle.Top;
                this.pbBaseImage.Left = _BaseImageDrawRectangle.Left;

                this.pbBaseImage.Width = _BaseImageDrawRectangle.Width;
                this.pbBaseImage.Height = _BaseImageDrawRectangle.Height;
                this._CurrentBaseImage.RotateFlip(RotateFlipType.Rotate90FlipNone);

                this.pnlBaseImage.AutoScrollPosition = new Point(1000000, 1000000);
                this.IBaseMaxVerticalScrollValue = this.pnlBaseImage.VerticalScroll.Value;
                this.IBaseMaxHorizontalScrollValue = this.pnlBaseImage.HorizontalScroll.Value;

                this.pbBaseImage.Refresh();
            }
        }

        public bool FormActivate
        {
            set { this.Focus(); }
        }

        public string LabelImagePosition_Text
        {
            set { this.lblImageposition.Text = value; }
        }

        public string ImageMaskStyle
        {
            set
            {
                this._ImageMaskStyle = value;
                switch (this._ImageMaskStyle.Split('|')[0].PadLeft(1, '0'))
                {
                    case "R":
                        // 赤
                        this.lblImageHighlight.BackColor = Color.FromArgb(int.Parse(_ImageMaskStyle.Split('|')[1]), Color.Red);
                        break;
                    case "1":
                    case "G":
                    default:
                        // 緑
                        this.lblImageHighlight.BackColor = Color.FromArgb(int.Parse(_ImageMaskStyle.Split('|')[1]), Color.Green);
                        break;
                    case "B":
                        // 青
                        this.lblImageHighlight.BackColor = Color.FromArgb(int.Parse(_ImageMaskStyle.Split('|')[1]), Color.Blue);
                        break;
                    case "W":
                        // 白抜き
                        this.lblImageMask.Visible = true;
                        this.lblImageHighlight.BackColor = Color.FromArgb(75, Color.Transparent);
                        break;
                }
            }
        }

        public string StatusImagePath_Text
        {
            set { this.toolStripStatusLabel1.Text = value; }
        }

        public string LabelBaseImagePath_Text
        {
            set { this.LabelBaseImagePath.Text = value; }
        }

        public bool IsShowBaseImageValue
        {
            set
            {
                this.IsShowBaseImage = value;
                if (IsShowBaseImage)
                {
                    if (_CurrentBaseImage != null)
                    {
                        this.pnlImage.Visible = false;
                        this.pnlBaseImage.Visible = true;
                    }
                    else
                    {
                        if (ShowBaseImage())
                        {
                            this.pnlBaseImage.Visible = true;
                        }
                        else
                        {
                            // 全面画像が存在しない場合戻す
                            this.IsShowBaseImage = false;
                        }
                    }
                }
                else
                {
                    this.pnlImage.Visible = true;
                    this.pnlBaseImage.Visible = false;
                }
            }
            get { return this.IsShowBaseImage; }
        }

        public bool CheckBokAutoScroll_Checked
        {
            set { this.cbAutoScroll.Checked = value; }
            get { return this.cbAutoScroll.Checked; }
        }

        public int ImaegPostion_X_Value
        {
            set
            {
                if (!IsShowBaseImage)
                {
                    this._ImaegPostion_X = value;
                }
                else
                {
                    this._BaseImaegPostion_X = value;
                }
            }
            get
            {
                if (!IsShowBaseImage)
                {
                    return this._ImaegPostion_X;
                }
                else
                {
                    return this._BaseImaegPostion_X;
                }
            }
        }

        public int ImaegPostion_Y_Value
        {
            set
            {
                if (!IsShowBaseImage)
                {
                    this._ImaegPostion_Y = value;
                }
                else
                {
                    this._BaseImaegPostion_Y = value;
                }
            }
            get
            {
                if (!IsShowBaseImage)
                {
                    return this._ImaegPostion_Y;
                }
                else
                {
                    return this._BaseImaegPostion_Y;
                }
            }
        }

        public Point PanelImage_AutoScrollPosition
        {
            set
            {
                if (IsShowBaseImage)
                {
                    this.pnlBaseImage.AutoScrollPosition = value;
                }
                else
                {
                    this.pnlImage.AutoScrollPosition = value;
                }
            }
        }

        public int PnlImage_VerticalScroll_Maximum
        {
            get
            {
                if (IsShowBaseImage)
                    return this.IBaseMaxVerticalScrollValue;
                return this.IMaxVerticalScrollValue;
            }
        }

        public int PnlImage_HorizontalScroll_Maximum
        {
            get
            {
                if (IsShowBaseImage)
                    return this.IBaseMaxHorizontalScrollValue;
                return this.IMaxHorizontalScrollValue;
            }
        }

        public double DefaultZoomRate
        {
            set
            {
                this._DefaultImageZoomRate = value;
                this._ImageZoomRate = value;
                this._BaseImageZoomRate = value;
            }
        }

        public int ImageZoomCount
        {
            set
            {
                if (!this.IsShowBaseImage)
                {
                    if (value == 1)
                    {
                        _ImageZoomRate *= _ImageZoomRatio;
                    }
                    else if (value == -1)
                    {
                        _ImageZoomRate /= _ImageZoomRatio;
                    }
                    else if (value == 0)
                    {
                        _ImageZoomRate = _DefaultImageZoomRate;
                    }
                    _ImageZoomCount += value;

                    //倍率変更後の画像のサイズと位置を計算する
                    _DrawRectangle.Width = (int)Math.Round(_CurrentImage.Width * _ImageZoomRate);
                    _DrawRectangle.Height = (int)Math.Round(_CurrentImage.Height * _ImageZoomRate);
                    _DrawRectangle.X = 0;
                    _DrawRectangle.Y = 0;

                    this.pbImage.Width = _DrawRectangle.Width;
                    this.pbImage.Height = _DrawRectangle.Height;
                    this.ResumeLayout();
                    this.pbImage.Visible = false;
                    // 画像を表示する
                    this.pbImage.Invalidate();
                    this.pbImage.Visible = true;

                    this.pnlImage.AutoScrollPosition = new Point(1000000, 1000000);
                    this.IMaxVerticalScrollValue = this.pnlImage.VerticalScroll.Value;
                    this.IMaxHorizontalScrollValue = this.pnlImage.HorizontalScroll.Value;
                    this.pnlImage.AutoScrollPosition = new Point(0, 0);

                    // イメージハイライト移動
                    MoveImageHighlight();

                    this.SuspendLayout();
                }
                else
                {
                    if (value == 1)
                    {
                        _BaseImageZoomRate *= _BaseImageZoomRatio;
                    }
                    else if (value == -1)
                    {
                        _BaseImageZoomRate /= _BaseImageZoomRatio;
                    }
                    else if (value == 0)
                    {
                        _BaseImageZoomRate = _DefaultImageZoomRate;
                    }
                    _BaseImageZoomCount += value;

                    //倍率変更後の画像のサイズと位置を計算する
                    _BaseImageDrawRectangle.Width = (int)Math.Round(_CurrentBaseImage.Width * _BaseImageZoomRate);
                    _BaseImageDrawRectangle.Height = (int)Math.Round(_CurrentBaseImage.Height * _BaseImageZoomRate);
                    _BaseImageDrawRectangle.X = 0;
                    _BaseImageDrawRectangle.Y = 0;

                    this.pbBaseImage.Width = _BaseImageDrawRectangle.Width;
                    this.pbBaseImage.Height = _BaseImageDrawRectangle.Height;
                    this.ResumeLayout();
                    this.pbBaseImage.Visible = false;
                    // 画像を表示する
                    this.pbBaseImage.Invalidate();
                    this.pbBaseImage.Visible = true;

                    this.pnlBaseImage.AutoScrollPosition = new Point(1000000, 1000000);
                    this.IBaseMaxVerticalScrollValue = this.pnlBaseImage.VerticalScroll.Value;
                    this.IBaseMaxHorizontalScrollValue = this.pnlBaseImage.HorizontalScroll.Value;
                    this.pnlBaseImage.AutoScrollPosition = new Point(0, 0);

                    this.SuspendLayout();

                }
            }
            get
            {
                if (!this.IsShowBaseImage)
                {
                    return _ImageZoomCount;
                }
                else
                {
                    return _BaseImageZoomCount;
                }
            }
        }

        public int IMaxVerticalScrollValue { get => _MaxVerticalScrollValue; set => _MaxVerticalScrollValue = value; }

        public int IMaxHorizontalScrollValue { get => _MaxHorizontalScrollValue; set => _MaxHorizontalScrollValue = value; }

        public int IBaseMaxVerticalScrollValue { get => _BaseMaxVerticalScrollValue; set => _BaseMaxVerticalScrollValue = value; }

        public int IBaseMaxHorizontalScrollValue { get => _BaseMaxHorizontalScrollValue; set => _BaseMaxHorizontalScrollValue = value; }

        #endregion

        /// <summary>
        /// イメージパス変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripStatusLabel_TextChanged(object sender, EventArgs e)
        {
            ShowImage();
        }

        private void ShowImage()
        {
            var swImage = System.Diagnostics.Stopwatch.StartNew();
            _log.Info($"ShowImage:start");

            if (_CurrentImage != null)
            {
                _CurrentImage.Dispose();
                if (_CurrentBaseImage != null)
                {
                    _CurrentBaseImage.Dispose();
                    _CurrentBaseImage = null;
                    IsShowBaseImageValue = false;
                }
                GC.Collect();
            }

            _ImaegPostion_X = 0;
            _ImaegPostion_Y = 0;

            //表示する画像を読み込む
            _CurrentImage = Utils.LoadImage(this.toolStripStatusLabel1.Text);

            //初期化
            _DrawRectangle = new Rectangle(0, 0, _CurrentImage.Width, _CurrentImage.Height);
            _DrawRectangle.Width = (int)(_CurrentImage.Width * _ImageZoomRate);
            _DrawRectangle.Height = (int)(_CurrentImage.Height * _ImageZoomRate);

            this.pbImage.Width = (int)(_CurrentImage.Width * _ImageZoomRate);
            this.pbImage.Height = (int)(_CurrentImage.Height * _ImageZoomRate);

            this.ResumeLayout();

            //画像を表示する
            this.pbImage.Invalidate();

            this.Activate();

            // イメージが変わったら倍率を初期に戻す
            this.ImageZoomCount = 0;

            this.pnlImage.AutoScrollPosition = new Point(1000000, 1000000);
            IMaxVerticalScrollValue = this.pnlImage.VerticalScroll.Value;
            IMaxHorizontalScrollValue = this.pnlImage.HorizontalScroll.Value;
            this.pnlImage.AutoScrollPosition = new Point(0, 0);

            this.SuspendLayout();

            swImage.Stop();
            _log.Info($"ShowImage.end:[経過時間:{swImage.Elapsed}]");
        }

        /// <summary>
        /// pbImage_Paint
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PbImage_Paint(object sender, PaintEventArgs e)
        {
            //画像を指定された位置、サイズで描画する
            if (_CurrentImage != null)
            {
                e.Graphics.DrawImage(_CurrentImage, _DrawRectangle);
            }
        }

        /// <summary>
        /// pbImage_Paint
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PbBaseImage_Paint(object sender, PaintEventArgs e)
        {
            //画像を指定された位置、サイズで描画する
            if (_CurrentBaseImage != null)
                e.Graphics.DrawImage(_CurrentBaseImage, _BaseImageDrawRectangle);
        }

        /// <summary>
        /// frmImage_KeyDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmImage_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    // F1　↓スクロール
                    this.ImaegPostion_Y_Value += this.PnlImage_VerticalScroll_Maximum / 4;
                    if (this.ImaegPostion_Y_Value > this.PnlImage_VerticalScroll_Maximum)
                    {
                        this.ImaegPostion_Y_Value = this.PnlImage_VerticalScroll_Maximum;
                    }
                    this.pnlImage.AutoScrollPosition = new Point(this.ImaegPostion_X_Value, this.ImaegPostion_Y_Value);
                    e.Handled = true;
                    break;
                case Keys.F2:
                    // F2　↑スクロール
                    this.ImaegPostion_Y_Value -= this.PnlImage_VerticalScroll_Maximum / 4;
                    if (this.ImaegPostion_Y_Value < 0)
                    {
                        this.ImaegPostion_Y_Value = 0;
                    }
                    this.PanelImage_AutoScrollPosition = new Point(this.ImaegPostion_X_Value, this.ImaegPostion_Y_Value);
                    e.Handled = true;
                    break;
                case Keys.F3:
                    // F3　→スクロール
                    this.ImaegPostion_X_Value += this.PnlImage_HorizontalScroll_Maximum / 4;
                    if (this.ImaegPostion_X_Value > this.PnlImage_HorizontalScroll_Maximum)
                    {
                        this.ImaegPostion_X_Value = this.PnlImage_HorizontalScroll_Maximum;
                    }
                    this.PanelImage_AutoScrollPosition = new Point(this.ImaegPostion_X_Value, this.ImaegPostion_Y_Value);
                    e.Handled = true;
                    break;
                case Keys.F4:
                    // F4　←スクロール
                    this.ImaegPostion_X_Value -= this.PnlImage_HorizontalScroll_Maximum / 4;
                    if (this.ImaegPostion_X_Value < 0)
                    {
                        this.ImaegPostion_X_Value = 0;
                    }
                    this.PanelImage_AutoScrollPosition = new Point(this.ImaegPostion_X_Value, this.ImaegPostion_Y_Value);
                    e.Handled = true;
                    break;
                case Keys.F5:
                    // F5　拡大
                    this.ImageZoomCount = 1;
                    this.CheckBokAutoScroll_Checked = false;
                    e.Handled = true;
                    break;
                case Keys.F6:
                    // F6　縮小
                    this.ImageZoomCount = -1;
                    this.CheckBokAutoScroll_Checked = false;
                    e.Handled = true;
                    break;
                case Keys.F7:
                    // F7　回転
                    ImageRotate();
                    e.Handled = true;
                    break;
                case Keys.F8:
                    // F8　画像切替
                    if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
                    {
                        this.IsShowBaseImageValue = !IsShowBaseImageValue;
                    }
                    else
                    {
                        this.ChangePage(!((Control.ModifierKeys & Keys.Shift) == Keys.Shift));
                    }
                    e.Handled = true;
                    break;
                case Keys.F12:
                    // F12　自動スクロールのチェック状態を反転
                    this.CheckBokAutoScroll_Checked = !this.CheckBokAutoScroll_Checked;
                    ImageZoomCount = 0;
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// cbAutoScroll_CheckedChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAutoScroll_CheckedChanged(object sender, EventArgs e)
        {
            // イメージを初期状態に戻す
            if (cbAutoScroll.Checked)
                ImageZoomCount = 0;
        }

        private void LabelImagePosition_TextChanged(object sender, EventArgs e)
        {
            MoveImageHighlight();
        }

        /// <summary>
        /// イメージハイライト移動
        /// </summary>
        private void MoveImageHighlight()
        {
            if ("*".Equals(this.lblImageposition.Text))
                return;

            //if (this.LabelBaseImagePath.Text.Length != 0)
            //    return;

            int iMarginLocation = 0;
            int iMarginSize = iMarginLocation * -2;

            var s = this.lblImageposition.Text.Split(',');
            if (s.Length < 2)
                return;

            double iX = double.Parse(((double.Parse(s[0]) + iMarginLocation) * _ImageZoomRate).ToString());
            double iY = double.Parse(((double.Parse(s[1]) + iMarginLocation) * _ImageZoomRate).ToString());

            double iWidth = double.Parse(((double.Parse(s[2]) + iMarginSize) * _ImageZoomRate).ToString());
            double iHeight = double.Parse(((double.Parse(s[3]) + iMarginSize) * _ImageZoomRate).ToString());

            this.lblImageHighlight.Visible = false;

            this.lblImageHighlight.Location = new Point((int)iX, (int)iY);
            this.lblImageHighlight.Size = new Size((int)iWidth, (int)iHeight);

            if (!"0".Equals(_ImageMaskStyle))
                this.lblImageHighlight.Visible = true;

            this.lblImageMask.Size = new Size(_DrawRectangle.Width, _DrawRectangle.Height);
        }

        private void FrmImage_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_CurrentImage != null)
                _CurrentImage.Dispose();
            if (_CurrentBaseImage != null)
                _CurrentBaseImage.Dispose();
            GC.Collect();
        }

        private bool ShowBaseImage()
        {
            if (this.LabelBaseImagePath.Text.Length == 0
                || !File.Exists(this.LabelBaseImagePath.Text))
            {
                MessageBox.Show("全面画像が設定されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (_CurrentBaseImage != null)
            {
                _CurrentBaseImage.Dispose();
                GC.Collect();
            }

            var swImage = System.Diagnostics.Stopwatch.StartNew();

            //表示する画像を読み込む
            _CurrentBaseImage = Utils.LoadImage(this.LabelBaseImagePath.Text);

            //初期化
            _BaseImageDrawRectangle = new Rectangle(0, 0, _CurrentBaseImage.Width, _CurrentBaseImage.Height);
            _BaseImageDrawRectangle.Width = (int)(_CurrentBaseImage.Width * _BaseImageZoomRate);
            _BaseImageDrawRectangle.Height = (int)(_CurrentBaseImage.Height * _BaseImageZoomRate);

            this.pbBaseImage.Width = (int)(_CurrentBaseImage.Width * _BaseImageZoomRate);
            this.pbBaseImage.Height = (int)(_CurrentBaseImage.Height * _BaseImageZoomRate);

            this.ResumeLayout();

            //画像を表示する
            this.pbBaseImage.Invalidate();

            this.Activate();

            // イメージが変わったら倍率を初期に戻す
            this.ImageZoomCount = 0;

            this.pnlBaseImage.AutoScrollPosition = new Point(1000000, 1000000);
            IBaseMaxVerticalScrollValue = this.pnlBaseImage.VerticalScroll.Value;
            IBaseMaxHorizontalScrollValue = this.pnlBaseImage.HorizontalScroll.Value;
            this.pnlBaseImage.AutoScrollPosition = new Point(0, 0);

            this.SuspendLayout();

            swImage.Stop();
            _log.Debug(String.Format("BaseImage表示.end:[経過時間:{0}]", swImage.Elapsed));

            return true;
        }
    }
}
