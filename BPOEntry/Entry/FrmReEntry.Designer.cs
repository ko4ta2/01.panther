namespace BPOEntry.EntryForms
{
    partial class FrmReEntry
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmReEntry));
            this.lblInp1 = new System.Windows.Forms.Label();
            this.btnUse1 = new System.Windows.Forms.Button();
            this.lblInp2 = new System.Windows.Forms.Label();
            this.btnUse2 = new System.Windows.Forms.Button();
            this.btnReEntry = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.lblEntryItemName = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.textInp1 = new CTextBox.CTextBox();
            this.textInp2 = new CTextBox.CTextBox();
            this.SuspendLayout();
            // 
            // lblInp1
            // 
            this.lblInp1.AutoSize = true;
            this.lblInp1.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblInp1.Location = new System.Drawing.Point(14, 43);
            this.lblInp1.Name = "lblInp1";
            this.lblInp1.Size = new System.Drawing.Size(76, 24);
            this.lblInp1.TabIndex = 0;
            this.lblInp1.Text = "lblInp1";
            // 
            // btnUse1
            // 
            this.btnUse1.CausesValidation = false;
            this.btnUse1.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnUse1.Location = new System.Drawing.Point(714, 32);
            this.btnUse1.Margin = new System.Windows.Forms.Padding(4);
            this.btnUse1.Name = "btnUse1";
            this.btnUse1.Size = new System.Drawing.Size(120, 35);
            this.btnUse1.TabIndex = 0;
            this.btnUse1.Text = "採用";
            this.btnUse1.UseVisualStyleBackColor = true;
            this.btnUse1.Click += new System.EventHandler(this.BtnUse1_Click);
            // 
            // lblInp2
            // 
            this.lblInp2.AutoSize = true;
            this.lblInp2.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblInp2.Location = new System.Drawing.Point(14, 273);
            this.lblInp2.Name = "lblInp2";
            this.lblInp2.Size = new System.Drawing.Size(76, 24);
            this.lblInp2.TabIndex = 0;
            this.lblInp2.Text = "lblInp2";
            // 
            // btnUse2
            // 
            this.btnUse2.CausesValidation = false;
            this.btnUse2.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnUse2.Location = new System.Drawing.Point(714, 262);
            this.btnUse2.Margin = new System.Windows.Forms.Padding(4);
            this.btnUse2.Name = "btnUse2";
            this.btnUse2.Size = new System.Drawing.Size(120, 35);
            this.btnUse2.TabIndex = 2;
            this.btnUse2.Text = "採用";
            this.btnUse2.UseVisualStyleBackColor = true;
            this.btnUse2.Click += new System.EventHandler(this.BtnUse2_Click);
            // 
            // btnReEntry
            // 
            this.btnReEntry.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnReEntry.Location = new System.Drawing.Point(13, 520);
            this.btnReEntry.Margin = new System.Windows.Forms.Padding(4);
            this.btnReEntry.Name = "btnReEntry";
            this.btnReEntry.Size = new System.Drawing.Size(180, 40);
            this.btnReEntry.TabIndex = 4;
            this.btnReEntry.Text = "再入力";
            this.btnReEntry.UseVisualStyleBackColor = true;
            this.btnReEntry.Click += new System.EventHandler(this.BtnReEntry_Click);
            this.btnReEntry.Enter += new System.EventHandler(this.BtnReEntry_Enter);
            this.btnReEntry.Leave += new System.EventHandler(this.BtnReEntry_Leave);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(14, 485);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(90, 18);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "lblMessage";
            // 
            // btnClose
            // 
            this.btnClose.CausesValidation = false;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnClose.Location = new System.Drawing.Point(654, 520);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(180, 40);
            this.btnClose.TabIndex = 254;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "閉じる";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // lblEntryItemName
            // 
            this.lblEntryItemName.AutoSize = true;
            this.lblEntryItemName.Font = new System.Drawing.Font("Meiryo UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblEntryItemName.Location = new System.Drawing.Point(2, 2);
            this.lblEntryItemName.Name = "lblEntryItemName";
            this.lblEntryItemName.Size = new System.Drawing.Size(228, 30);
            this.lblEntryItemName.TabIndex = 0;
            this.lblEntryItemName.Text = "lblEntryItemName";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblStatus.Font = new System.Drawing.Font("Meiryo UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblStatus.Location = new System.Drawing.Point(734, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(114, 30);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "lblStatus";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textInp1
            // 
            this.textInp1.AutoRelease = false;
            this.textInp1.Conditional_Required_Item = null;
            this.textInp1.Conditional_Required_Omit_Value = null;
            this.textInp1.Conditional_Required_Value = null;
            this.textInp1.ControlAlign = null;
            this.textInp1.DateFormat = "";
            this.textInp1.DefaultImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.textInp1.DisplayCorrect = false;
            this.textInp1.DR = null;
            this.textInp1.DummyItemFlag = "";
            this.textInp1.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F);
            this.textInp1.FullLength = false;
            this.textInp1.ImagePosition = null;
            this.textInp1.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.textInp1.InputMode = null;
            this.textInp1.IsDummyItem = false;
            this.textInp1.IsExistingReceipt = false;
            this.textInp1.IsInput2 = true;
            this.textInp1.IsMailAddress = false;
            this.textInp1.IsRequired = false;
            this.textInp1.ItemName = null;
            this.textInp1.JumpTab = false;
            this.textInp1.Location = new System.Drawing.Point(14, 70);
            this.textInp1.Margin = new System.Windows.Forms.Padding(4);
            this.textInp1.MasterCheck = false;
            this.textInp1.MaxLength = 90;
            this.textInp1.Multiline = true;
            this.textInp1.MyNumber1 = null;
            this.textInp1.MyNumber2 = null;
            this.textInp1.Name = "textInp1";
            this.textInp1.Range = null;
            this.textInp1.Regex = null;
            this.textInp1.SelectAllText = false;
            this.textInp1.ShortcutsEnabled = false;
            this.textInp1.Size = new System.Drawing.Size(820, 185);
            this.textInp1.Square = false;
            this.textInp1.TabIndex = 1;
            this.textInp1.Tips = null;
            this.textInp1.ValidPattern = null;
            this.textInp1.Validating += new System.ComponentModel.CancelEventHandler(this.textInp_Validating);
            // 
            // textInp2
            // 
            this.textInp2.AutoRelease = false;
            this.textInp2.Conditional_Required_Item = null;
            this.textInp2.Conditional_Required_Omit_Value = null;
            this.textInp2.Conditional_Required_Value = null;
            this.textInp2.ControlAlign = null;
            this.textInp2.DateFormat = "";
            this.textInp2.DefaultImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.textInp2.DisplayCorrect = false;
            this.textInp2.DR = null;
            this.textInp2.DummyItemFlag = "";
            this.textInp2.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F);
            this.textInp2.FullLength = false;
            this.textInp2.ImagePosition = null;
            this.textInp2.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.textInp2.InputMode = null;
            this.textInp2.IsDummyItem = false;
            this.textInp2.IsExistingReceipt = false;
            this.textInp2.IsInput2 = true;
            this.textInp2.IsMailAddress = false;
            this.textInp2.IsRequired = false;
            this.textInp2.ItemName = null;
            this.textInp2.JumpTab = false;
            this.textInp2.Location = new System.Drawing.Point(14, 300);
            this.textInp2.Margin = new System.Windows.Forms.Padding(4);
            this.textInp2.MasterCheck = false;
            this.textInp2.MaxLength = 90;
            this.textInp2.Multiline = true;
            this.textInp2.MyNumber1 = null;
            this.textInp2.MyNumber2 = null;
            this.textInp2.Name = "textInp2";
            this.textInp2.Range = null;
            this.textInp2.Regex = null;
            this.textInp2.SelectAllText = false;
            this.textInp2.ShortcutsEnabled = false;
            this.textInp2.Size = new System.Drawing.Size(820, 185);
            this.textInp2.Square = false;
            this.textInp2.TabIndex = 3;
            this.textInp2.Tips = null;
            this.textInp2.ValidPattern = null;
            this.textInp2.Validating += new System.ComponentModel.CancelEventHandler(this.textInp_Validating);
            // 
            // FrmReEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(848, 570);
            this.ControlBox = false;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnReEntry);
            this.Controls.Add(this.btnUse2);
            this.Controls.Add(this.textInp1);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblInp2);
            this.Controls.Add(this.btnUse1);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblInp1);
            this.Controls.Add(this.lblEntryItemName);
            this.Controls.Add(this.textInp2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Location = new System.Drawing.Point(1000, 320);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmReEntry";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "○○○　●●●エントリ業務";
            this.Load += new System.EventHandler(this.FrmReEntry_Load);
            this.Shown += new System.EventHandler(this.ReEntryForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ReEntryForm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInp1;
        private System.Windows.Forms.Button btnUse1;
        private CTextBox.CTextBox textInp1;
        private System.Windows.Forms.Label lblInp2;
        private System.Windows.Forms.Button btnUse2;
        private CTextBox.CTextBox textInp2;
        private System.Windows.Forms.Button btnReEntry;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ToolTip toolTips;
        private System.Windows.Forms.Label lblEntryItemName;
        private System.Windows.Forms.Label lblStatus;
    }
}