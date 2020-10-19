namespace BPOEntry.ReEntrySelect
{
    partial class FrmEntryUnitList
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
            if(disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmEntryUnitList));
            this.lblTitle = new System.Windows.Forms.Label();
            this.ButtonSearch = new System.Windows.Forms.Button();
            this.ButtonClose = new System.Windows.Forms.Button();
            this.ButtonExec = new System.Windows.Forms.Button();
            this.lvReEntryList = new System.Windows.Forms.ListView();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.DropDownListGyoumKbn = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
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
            this.lblTitle.Size = new System.Drawing.Size(1221, 40);
            this.lblTitle.TabIndex = 6;
            this.lblTitle.Text = "修正対象エントリ一覧";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSearch
            // 
            this.ButtonSearch.Location = new System.Drawing.Point(1088, 46);
            this.ButtonSearch.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonSearch.Name = "btnSearch";
            this.ButtonSearch.Size = new System.Drawing.Size(120, 34);
            this.ButtonSearch.TabIndex = 1;
            this.ButtonSearch.Text = "再検索";
            this.ButtonSearch.UseVisualStyleBackColor = true;
            this.ButtonSearch.Click += new System.EventHandler(this.ButtonReSearch_Click);
            // 
            // btnClose
            // 
            this.ButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonClose.Location = new System.Drawing.Point(1028, 738);
            this.ButtonClose.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonClose.Name = "btnClose";
            this.ButtonClose.Size = new System.Drawing.Size(180, 40);
            this.ButtonClose.TabIndex = 0;
            this.ButtonClose.TabStop = false;
            this.ButtonClose.Text = "閉じる";
            this.ButtonClose.UseVisualStyleBackColor = true;
            this.ButtonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // btnExec
            // 
            this.ButtonExec.Location = new System.Drawing.Point(20, 738);
            this.ButtonExec.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonExec.Name = "btnExec";
            this.ButtonExec.Size = new System.Drawing.Size(180, 40);
            this.ButtonExec.TabIndex = 2;
            this.ButtonExec.Text = "修正";
            this.ButtonExec.UseVisualStyleBackColor = true;
            this.ButtonExec.Click += new System.EventHandler(this.ButtonExec_Click);
            // 
            // lvReEntryList
            // 
            this.lvReEntryList.BackColor = System.Drawing.SystemColors.Window;
            this.lvReEntryList.Font = new System.Drawing.Font("Meiryo UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lvReEntryList.FullRowSelect = true;
            this.lvReEntryList.GridLines = true;
            this.lvReEntryList.Location = new System.Drawing.Point(20, 86);
            this.lvReEntryList.Margin = new System.Windows.Forms.Padding(4);
            this.lvReEntryList.MultiSelect = false;
            this.lvReEntryList.Name = "lvReEntryList";
            this.lvReEntryList.Size = new System.Drawing.Size(1188, 642);
            this.lvReEntryList.TabIndex = 0;
            this.lvReEntryList.UseCompatibleStateImageBehavior = false;
            this.lvReEntryList.DoubleClick += new System.EventHandler(this.lvReEntryList_DoubleClick);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("Meiryo UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.checkBox1.Location = new System.Drawing.Point(20, 54);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(266, 29);
            this.checkBox1.TabIndex = 7;
            this.checkBox1.Text = "エントリデータ出力済みを表示";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Visible = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Enabled = false;
            this.dateTimePicker1.Font = new System.Drawing.Font("Meiryo UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(437, 52);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(160, 33);
            this.dateTimePicker1.TabIndex = 8;
            this.dateTimePicker1.Visible = false;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.DateTimePicker1_ValueChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Font = new System.Drawing.Font("Meiryo UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.checkBox2.Location = new System.Drawing.Point(296, 54);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(151, 29);
            this.checkBox2.TabIndex = 17;
            this.checkBox2.Text = "連携年月日：";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.Visible = false;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.CheckBox2_CheckedChanged);
            // 
            // DropDownListGyoumKbn
            // 
            this.DropDownListGyoumKbn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DropDownListGyoumKbn.Font = new System.Drawing.Font("Meiryo UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.DropDownListGyoumKbn.FormattingEnabled = true;
            this.DropDownListGyoumKbn.Location = new System.Drawing.Point(769, 52);
            this.DropDownListGyoumKbn.Margin = new System.Windows.Forms.Padding(4);
            this.DropDownListGyoumKbn.Name = "DropDownListGyoumKbn";
            this.DropDownListGyoumKbn.Size = new System.Drawing.Size(310, 33);
            this.DropDownListGyoumKbn.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Meiryo UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(600, 56);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 25);
            this.label1.TabIndex = 23;
            this.label1.Text = "帳票種別グループ：";
            // 
            // FrmEntryUnitList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonClose;
            this.ClientSize = new System.Drawing.Size(1221, 790);
            this.ControlBox = false;
            this.Controls.Add(this.DropDownListGyoumKbn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.lvReEntryList);
            this.Controls.Add(this.ButtonExec);
            this.Controls.Add(this.ButtonClose);
            this.Controls.Add(this.ButtonSearch);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmEntryUnitList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "○○○　●●●エントリ業務";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonSearch;
        private System.Windows.Forms.Button ButtonClose;
        private System.Windows.Forms.Button ButtonExec;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ListView lvReEntryList;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.ComboBox DropDownListGyoumKbn;
        private System.Windows.Forms.Label label1;
    }
}