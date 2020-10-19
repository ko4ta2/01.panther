namespace BPOEntry.EntryForms
{
    partial class FrmImage
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmImage));
            this.pnlImage = new System.Windows.Forms.Panel();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.pbBaseImage = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.cbAutoScroll = new System.Windows.Forms.CheckBox();
            this.stsImagePath = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblImageZoomRate = new System.Windows.Forms.Label();
            this.lblImageposition = new System.Windows.Forms.Label();
            this.lblImageHighlight = new System.Windows.Forms.Label();
            this.lblImageMask = new System.Windows.Forms.Label();
            this.LabelBaseImagePath = new System.Windows.Forms.Label();
            this.pnlBaseImage = new System.Windows.Forms.Panel();
            this.pnlImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBaseImage)).BeginInit();
            this.stsImagePath.SuspendLayout();
            this.pnlBaseImage.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlImage
            // 
            this.pnlImage.AutoScroll = true;
            this.pnlImage.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnlImage.Controls.Add(this.pbImage);
            this.pnlImage.Location = new System.Drawing.Point(0, 26);
            this.pnlImage.Name = "pnlImage";
            this.pnlImage.Size = new System.Drawing.Size(826, 880);
            this.pnlImage.TabIndex = 251;
            // 
            // pbImage
            // 
            this.pbImage.Location = new System.Drawing.Point(0, 0);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(0, 0);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbImage.TabIndex = 28;
            this.pbImage.TabStop = false;
            this.pbImage.Paint += new System.Windows.Forms.PaintEventHandler(this.PbImage_Paint);
            // 
            // pbBaseImage
            // 
            this.pbBaseImage.Location = new System.Drawing.Point(0, 0);
            this.pbBaseImage.Name = "pbBaseImage";
            this.pbBaseImage.Size = new System.Drawing.Size(0, 0);
            this.pbBaseImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbBaseImage.TabIndex = 29;
            this.pbBaseImage.TabStop = false;
            this.pbBaseImage.Paint += new System.Windows.Forms.PaintEventHandler(this.PbBaseImage_Paint);
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.SystemColors.HotTrack;
            this.lblTitle.Font = new System.Drawing.Font("Meiryo UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTitle.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(0, 40);
            this.lblTitle.TabIndex = 255;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.Visible = false;
            // 
            // cbAutoScroll
            // 
            this.cbAutoScroll.Checked = true;
            this.cbAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoScroll.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cbAutoScroll.Location = new System.Drawing.Point(2, 0);
            this.cbAutoScroll.Name = "cbAutoScroll";
            this.cbAutoScroll.Size = new System.Drawing.Size(188, 26);
            this.cbAutoScroll.TabIndex = 283;
            this.cbAutoScroll.TabStop = false;
            this.cbAutoScroll.Text = "F12:自動スクロール";
            this.cbAutoScroll.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.cbAutoScroll.UseVisualStyleBackColor = true;
            this.cbAutoScroll.CheckedChanged += new System.EventHandler(this.CheckBoxAutoScroll_CheckedChanged);
            // 
            // stsImagePath
            // 
            this.stsImagePath.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.stsImagePath.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.stsImagePath.Location = new System.Drawing.Point(0, 878);
            this.stsImagePath.Name = "stsImagePath";
            this.stsImagePath.Size = new System.Drawing.Size(984, 22);
            this.stsImagePath.TabIndex = 285;
            this.stsImagePath.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(133, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.TextChanged += new System.EventHandler(this.ToolStripStatusLabel_TextChanged);
            // 
            // lblImageZoomRate
            // 
            this.lblImageZoomRate.AllowDrop = true;
            this.lblImageZoomRate.BackColor = System.Drawing.SystemColors.Control;
            this.lblImageZoomRate.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblImageZoomRate.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblImageZoomRate.Location = new System.Drawing.Point(0, 0);
            this.lblImageZoomRate.Name = "lblImageZoomRate";
            this.lblImageZoomRate.Size = new System.Drawing.Size(984, 26);
            this.lblImageZoomRate.TabIndex = 286;
            this.lblImageZoomRate.Text = "F1:↓、F2:↑、F3:→、1F4:←、F5:拡大、F6:縮小、F7:右回転";
            this.lblImageZoomRate.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblImageposition
            // 
            this.lblImageposition.AutoSize = true;
            this.lblImageposition.ForeColor = System.Drawing.Color.Red;
            this.lblImageposition.Location = new System.Drawing.Point(355, 4);
            this.lblImageposition.Name = "lblImageposition";
            this.lblImageposition.Size = new System.Drawing.Size(16, 16);
            this.lblImageposition.TabIndex = 287;
            this.lblImageposition.Text = "*";
            this.lblImageposition.Visible = false;
            this.lblImageposition.TextChanged += new System.EventHandler(this.LabelImagePosition_TextChanged);
            // 
            // lblImageHighlight
            // 
            this.lblImageHighlight.Location = new System.Drawing.Point(488, 6);
            this.lblImageHighlight.Name = "lblImageHighlight";
            this.lblImageHighlight.Size = new System.Drawing.Size(35, 12);
            this.lblImageHighlight.TabIndex = 29;
            this.lblImageHighlight.Visible = false;
            // 
            // lblImageMask
            // 
            this.lblImageMask.BackColor = System.Drawing.SystemColors.Control;
            this.lblImageMask.Font = new System.Drawing.Font("ＭＳ ゴシック", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblImageMask.ForeColor = System.Drawing.Color.Red;
            this.lblImageMask.Location = new System.Drawing.Point(466, 6);
            this.lblImageMask.Name = "lblImageMask";
            this.lblImageMask.Size = new System.Drawing.Size(16, 16);
            this.lblImageMask.TabIndex = 291;
            this.lblImageMask.Text = "イメージ全体マスク";
            this.lblImageMask.Visible = false;
            // 
            // LabelBaseImagePath
            // 
            this.LabelBaseImagePath.AutoSize = true;
            this.LabelBaseImagePath.Location = new System.Drawing.Point(832, 340);
            this.LabelBaseImagePath.Name = "LabelBaseImagePath";
            this.LabelBaseImagePath.Size = new System.Drawing.Size(152, 16);
            this.LabelBaseImagePath.TabIndex = 292;
            this.LabelBaseImagePath.Text = "LabelBaseImagePath";
            this.LabelBaseImagePath.Visible = false;
            // 
            // pnlBaseImage
            // 
            this.pnlBaseImage.AutoScroll = true;
            this.pnlBaseImage.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnlBaseImage.Controls.Add(this.pbBaseImage);
            this.pnlBaseImage.Location = new System.Drawing.Point(0, 27);
            this.pnlBaseImage.Name = "pnlBaseImage";
            this.pnlBaseImage.Size = new System.Drawing.Size(200, 184);
            this.pnlBaseImage.TabIndex = 294;
            // 
            // FrmImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 900);
            this.ControlBox = false;
            this.Controls.Add(this.cbAutoScroll);
            this.Controls.Add(this.lblImageZoomRate);
            this.Controls.Add(this.pnlBaseImage);
            this.Controls.Add(this.LabelBaseImagePath);
            this.Controls.Add(this.lblImageMask);
            this.Controls.Add(this.lblImageHighlight);
            this.Controls.Add(this.stsImagePath);
            this.Controls.Add(this.pnlImage);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblImageposition);
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmImage";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FrmImage";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmImage_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmImage_KeyDown);
            this.pnlImage.ResumeLayout(false);
            this.pnlImage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBaseImage)).EndInit();
            this.stsImagePath.ResumeLayout(false);
            this.stsImagePath.PerformLayout();
            this.pnlBaseImage.ResumeLayout(false);
            this.pnlBaseImage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        //private System.Windows.Forms.Label lblImagePath;
        public System.Windows.Forms.Panel pnlImage;
        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.PictureBox pbBaseImage;
        //private System.Windows.Forms.RichTextBox richTextBoxDiff;
        public System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.CheckBox cbAutoScroll;
        private System.Windows.Forms.StatusStrip stsImagePath;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label lblImageZoomRate;
        private System.Windows.Forms.Label lblImageposition;
        private System.Windows.Forms.Label lblImageHighlight;
        private System.Windows.Forms.Label lblImageMask;
        private System.Windows.Forms.Label LabelBaseImagePath;
        private System.Windows.Forms.Panel pnlBaseImage;
    }
}