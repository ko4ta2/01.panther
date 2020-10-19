namespace BPOEntry.DivideEntryUnit
{
    partial class frmDivideEntryUnit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDivideEntryUnit));
            this.lblTitle = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpCaptureDate = new System.Windows.Forms.DateTimePicker();
            this.nudCaptureNum = new System.Windows.Forms.NumericUpDown();
            this.btnExec = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.nudCaptureCount = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.DropDownList = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureCount)).BeginInit();
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
            this.lblTitle.Size = new System.Drawing.Size(649, 40);
            this.lblTitle.TabIndex = 5;
            this.lblTitle.Text = "エントリ単位分割（現物）";
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
            // dtpCaptureDate
            // 
            this.dtpCaptureDate.CustomFormat = "";
            this.dtpCaptureDate.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpCaptureDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpCaptureDate.Location = new System.Drawing.Point(140, 56);
            this.dtpCaptureDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpCaptureDate.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dtpCaptureDate.Name = "dtpCaptureDate";
            this.dtpCaptureDate.Size = new System.Drawing.Size(160, 32);
            this.dtpCaptureDate.TabIndex = 1;
            // 
            // nudCaptureNum
            // 
            this.nudCaptureNum.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.nudCaptureNum.Location = new System.Drawing.Point(140, 96);
            this.nudCaptureNum.Margin = new System.Windows.Forms.Padding(4);
            this.nudCaptureNum.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nudCaptureNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCaptureNum.Name = "nudCaptureNum";
            this.nudCaptureNum.Size = new System.Drawing.Size(49, 32);
            this.nudCaptureNum.TabIndex = 2;
            this.nudCaptureNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudCaptureNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCaptureNum.Enter += new System.EventHandler(this.nudImageCaptureNum_Enter);
            // 
            // btnExec
            // 
            this.btnExec.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnExec.Location = new System.Drawing.Point(20, 220);
            this.btnExec.Margin = new System.Windows.Forms.Padding(4);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(180, 40);
            this.btnExec.TabIndex = 100;
            this.btnExec.Text = "実行";
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnClose.Location = new System.Drawing.Point(445, 220);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(180, 40);
            this.btnClose.TabIndex = 101;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "閉じる";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // nudCaptureCount
            // 
            this.nudCaptureCount.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.nudCaptureCount.Location = new System.Drawing.Point(140, 176);
            this.nudCaptureCount.Margin = new System.Windows.Forms.Padding(4);
            this.nudCaptureCount.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.nudCaptureCount.Name = "nudCaptureCount";
            this.nudCaptureCount.Size = new System.Drawing.Size(73, 32);
            this.nudCaptureCount.TabIndex = 4;
            this.nudCaptureCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudCaptureCount.Enter += new System.EventHandler(this.nudCaptureCount_Enter);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(20, 180);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 24);
            this.label3.TabIndex = 6;
            this.label3.Text = "件数：";
            // 
            // DropDownList
            // 
            this.DropDownList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DropDownList.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.DropDownList.FormattingEnabled = true;
            this.DropDownList.Location = new System.Drawing.Point(140, 138);
            this.DropDownList.Margin = new System.Windows.Forms.Padding(4);
            this.DropDownList.Name = "DropDownList";
            this.DropDownList.Size = new System.Drawing.Size(485, 28);
            this.DropDownList.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(20, 140);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 24);
            this.label4.TabIndex = 103;
            this.label4.Text = "帳票：";
            // 
            // frmDivideEntryUnit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(644, 270);
            this.ControlBox = false;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.DropDownList);
            this.Controls.Add(this.nudCaptureCount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnExec);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.nudCaptureNum);
            this.Controls.Add(this.dtpCaptureDate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDivideEntryUnit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "○○○　●●●エントリ業務";
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCaptureCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpCaptureDate;
        private System.Windows.Forms.NumericUpDown nudCaptureNum;
        private System.Windows.Forms.Button btnExec;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.NumericUpDown nudCaptureCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox DropDownList;
        private System.Windows.Forms.Label label4;
    }
}