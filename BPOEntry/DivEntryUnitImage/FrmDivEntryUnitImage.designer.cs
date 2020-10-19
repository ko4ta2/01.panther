namespace BPOEntry.DivideEntryUnitImage
{
    partial class frmDivideEntryUnitImage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDivideEntryUnitImage));
            this.lblTitle = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpImageCaptureDate = new System.Windows.Forms.DateTimePicker();
            this.nudImageCaptureNum = new System.Windows.Forms.NumericUpDown();
            this.btnExec = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageCaptureNum)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.BackColor = System.Drawing.SystemColors.HotTrack;
            this.lblTitle.Font = new System.Drawing.Font("Meiryo UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(425, 40);
            this.lblTitle.TabIndex = 5;
            this.lblTitle.Text = "エントリ単位分割（イメージ）";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(20, 60);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "連携年月日：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(20, 100);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "回数：";
            // 
            // dtpImageCaptureDate
            // 
            this.dtpImageCaptureDate.CustomFormat = "";
            this.dtpImageCaptureDate.Font = new System.Drawing.Font("Meiryo UI", 15F);
            this.dtpImageCaptureDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpImageCaptureDate.Location = new System.Drawing.Point(140, 56);
            this.dtpImageCaptureDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpImageCaptureDate.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dtpImageCaptureDate.Name = "dtpImageCaptureDate";
            this.dtpImageCaptureDate.Size = new System.Drawing.Size(160, 33);
            this.dtpImageCaptureDate.TabIndex = 1;
            // 
            // nudImageCaptureNum
            // 
            this.nudImageCaptureNum.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.nudImageCaptureNum.Location = new System.Drawing.Point(140, 96);
            this.nudImageCaptureNum.Margin = new System.Windows.Forms.Padding(4);
            this.nudImageCaptureNum.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nudImageCaptureNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudImageCaptureNum.Name = "nudImageCaptureNum";
            this.nudImageCaptureNum.Size = new System.Drawing.Size(49, 32);
            this.nudImageCaptureNum.TabIndex = 2;
            this.nudImageCaptureNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudImageCaptureNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudImageCaptureNum.Enter += new System.EventHandler(this.nudImageCaptureNum_Enter);
            // 
            // btnExec
            // 
            this.btnExec.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnExec.Location = new System.Drawing.Point(20, 140);
            this.btnExec.Margin = new System.Windows.Forms.Padding(4);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(180, 40);
            this.btnExec.TabIndex = 3;
            this.btnExec.Text = "実行";
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnClose.Location = new System.Drawing.Point(222, 140);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(180, 40);
            this.btnClose.TabIndex = 4;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "閉じる";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmDivideEntryUnitImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(420, 190);
            this.ControlBox = false;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnExec);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.nudImageCaptureNum);
            this.Controls.Add(this.dtpImageCaptureDate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDivideEntryUnitImage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "○○○　●●●エントリ業務";
            ((System.ComponentModel.ISupportInitialize)(this.nudImageCaptureNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpImageCaptureDate;
        private System.Windows.Forms.NumericUpDown nudImageCaptureNum;
        private System.Windows.Forms.Button btnExec;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblTitle;
    }
}