namespace BPOEntry.EntryForms
{
    partial class ReEntryForm
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
            this.lblInp1 = new System.Windows.Forms.Label();
            this.btnUse1 = new System.Windows.Forms.Button();
            this.textInp1 = new System.Windows.Forms.TextBox();
            this.lblInp2 = new System.Windows.Forms.Label();
            this.btnUse2 = new System.Windows.Forms.Button();
            this.textInp2 = new System.Windows.Forms.TextBox();
            this.btnReEntry = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.lblEntryItemName = new System.Windows.Forms.Label();
            this.lblTtlCount = new System.Windows.Forms.Label();
            this.lblCrtCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblInp1
            // 
            this.lblInp1.AutoSize = true;
            this.lblInp1.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblInp1.Location = new System.Drawing.Point(25, 83);
            this.lblInp1.Name = "lblInp1";
            this.lblInp1.Size = new System.Drawing.Size(72, 24);
            this.lblInp1.TabIndex = 0;
            this.lblInp1.Text = "label1";
            // 
            // btnUse1
            // 
            this.btnUse1.CausesValidation = false;
            this.btnUse1.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnUse1.Location = new System.Drawing.Point(603, 78);
            this.btnUse1.Margin = new System.Windows.Forms.Padding(4);
            this.btnUse1.Name = "btnUse1";
            this.btnUse1.Size = new System.Drawing.Size(124, 35);
            this.btnUse1.TabIndex = 2;
            this.btnUse1.Text = "採用";
            this.btnUse1.UseVisualStyleBackColor = true;
            this.btnUse1.Click += new System.EventHandler(this.btnUse1_Click);
            // 
            // textInp1
            // 
            this.textInp1.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F);
            this.textInp1.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.textInp1.Location = new System.Drawing.Point(29, 121);
            this.textInp1.Margin = new System.Windows.Forms.Padding(4);
            this.textInp1.MaxLength = 90;
            this.textInp1.Multiline = true;
            this.textInp1.Name = "textInp1";
            this.textInp1.Size = new System.Drawing.Size(698, 82);
            this.textInp1.TabIndex = 1;
            this.textInp1.Text = "１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１" +
    "２３４５６７８９０";
            this.textInp1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            this.textInp1.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // lblInp2
            // 
            this.lblInp2.AutoSize = true;
            this.lblInp2.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblInp2.Location = new System.Drawing.Point(25, 243);
            this.lblInp2.Name = "lblInp2";
            this.lblInp2.Size = new System.Drawing.Size(72, 24);
            this.lblInp2.TabIndex = 0;
            this.lblInp2.Text = "label1";
            // 
            // btnUse2
            // 
            this.btnUse2.CausesValidation = false;
            this.btnUse2.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnUse2.Location = new System.Drawing.Point(603, 238);
            this.btnUse2.Margin = new System.Windows.Forms.Padding(4);
            this.btnUse2.Name = "btnUse2";
            this.btnUse2.Size = new System.Drawing.Size(124, 35);
            this.btnUse2.TabIndex = 4;
            this.btnUse2.Text = "採用";
            this.btnUse2.UseVisualStyleBackColor = true;
            this.btnUse2.Click += new System.EventHandler(this.btnUse2_Click);
            // 
            // textInp2
            // 
            this.textInp2.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F);
            this.textInp2.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.textInp2.Location = new System.Drawing.Point(29, 281);
            this.textInp2.Margin = new System.Windows.Forms.Padding(4);
            this.textInp2.MaxLength = 90;
            this.textInp2.Multiline = true;
            this.textInp2.Name = "textInp2";
            this.textInp2.Size = new System.Drawing.Size(698, 82);
            this.textInp2.TabIndex = 3;
            this.textInp2.Text = "１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１" +
    "２３４５６７８９０";
            this.textInp2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            this.textInp2.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // btnReEntry
            // 
            this.btnReEntry.CausesValidation = false;
            this.btnReEntry.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnReEntry.Location = new System.Drawing.Point(29, 389);
            this.btnReEntry.Margin = new System.Windows.Forms.Padding(4);
            this.btnReEntry.Name = "btnReEntry";
            this.btnReEntry.Size = new System.Drawing.Size(124, 35);
            this.btnReEntry.TabIndex = 5;
            this.btnReEntry.Text = "再入力";
            this.btnReEntry.UseVisualStyleBackColor = true;
            this.btnReEntry.Click += new System.EventHandler(this.btnReEntry_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(172, 371);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(63, 18);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "メッセージ";
            // 
            // btnClose
            // 
            this.btnClose.CausesValidation = false;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnClose.Location = new System.Drawing.Point(603, 389);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(124, 35);
            this.btnClose.TabIndex = 254;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "閉じる";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblEntryItemName
            // 
            this.lblEntryItemName.Font = new System.Drawing.Font("Meiryo UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblEntryItemName.Location = new System.Drawing.Point(25, 20);
            this.lblEntryItemName.Name = "lblEntryItemName";
            this.lblEntryItemName.Size = new System.Drawing.Size(702, 54);
            this.lblEntryItemName.TabIndex = 0;
            // 
            // lblTtlCount
            // 
            this.lblTtlCount.AutoSize = true;
            this.lblTtlCount.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTtlCount.Location = new System.Drawing.Point(80, 3);
            this.lblTtlCount.Name = "lblTtlCount";
            this.lblTtlCount.Size = new System.Drawing.Size(35, 17);
            this.lblTtlCount.TabIndex = 0;
            this.lblTtlCount.Text = "999";
            this.lblTtlCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCrtCount
            // 
            this.lblCrtCount.AutoSize = true;
            this.lblCrtCount.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblCrtCount.Location = new System.Drawing.Point(26, 3);
            this.lblCrtCount.Name = "lblCrtCount";
            this.lblCrtCount.Size = new System.Drawing.Size(35, 17);
            this.lblCrtCount.TabIndex = 0;
            this.lblCrtCount.Text = "999";
            this.lblCrtCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(63, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "/";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ReEntryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(755, 445);
            this.ControlBox = false;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.textInp2);
            this.Controls.Add(this.btnReEntry);
            this.Controls.Add(this.btnUse2);
            this.Controls.Add(this.textInp1);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblInp2);
            this.Controls.Add(this.btnUse1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblCrtCount);
            this.Controls.Add(this.lblTtlCount);
            this.Controls.Add(this.lblEntryItemName);
            this.Controls.Add(this.lblInp1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Location = new System.Drawing.Point(1000, 320);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReEntryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "楽天生命 エントリー業務";
            this.Shown += new System.EventHandler(this.ReEntryForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInp1;
        private System.Windows.Forms.Button btnUse1;
        private System.Windows.Forms.TextBox textInp1;
        private System.Windows.Forms.Label lblInp2;
        private System.Windows.Forms.Button btnUse2;
        private System.Windows.Forms.TextBox textInp2;
        private System.Windows.Forms.Button btnReEntry;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ToolTip toolTips;
        private System.Windows.Forms.Label lblEntryItemName;
        private System.Windows.Forms.Label lblTtlCount;
        private System.Windows.Forms.Label lblCrtCount;
        private System.Windows.Forms.Label label3;
    }
}