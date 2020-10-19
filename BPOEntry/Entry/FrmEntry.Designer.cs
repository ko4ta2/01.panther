namespace BPOEntry.EntryForms
{
    partial class FrmEntry
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmEntry));
            this.ButtonBack = new System.Windows.Forms.Button();
            this.ButtonClose = new System.Windows.Forms.Button();
            this.ButtonExecute = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.PanelEntry = new System.Windows.Forms.Panel();
            this.lblParams = new System.Windows.Forms.Label();
            this.TextDummy = new System.Windows.Forms.TextBox();
            this.LabelDummy = new System.Windows.Forms.Label();
            this.GroupBoxDummy = new System.Windows.Forms.GroupBox();
            this.RadioButtonNo = new System.Windows.Forms.RadioButton();
            this.RadioButtonYes = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.labelKansaId = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.GroupBoxDummy.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonBack
            // 
            this.ButtonBack.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ButtonBack.Location = new System.Drawing.Point(10, 916);
            this.ButtonBack.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonBack.Name = "ButtonBack";
            this.ButtonBack.Size = new System.Drawing.Size(180, 40);
            this.ButtonBack.TabIndex = 253;
            this.ButtonBack.TabStop = false;
            this.ButtonBack.Text = "前レコード";
            this.ButtonBack.UseVisualStyleBackColor = true;
            this.ButtonBack.Click += new System.EventHandler(this.ButtonBack_Click);
            // 
            // ButtonClose
            // 
            this.ButtonClose.CausesValidation = false;
            this.ButtonClose.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ButtonClose.Location = new System.Drawing.Point(649, 916);
            this.ButtonClose.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.Size = new System.Drawing.Size(180, 40);
            this.ButtonClose.TabIndex = 251;
            this.ButtonClose.TabStop = false;
            this.ButtonClose.Text = "閉じる";
            this.ButtonClose.UseVisualStyleBackColor = true;
            this.ButtonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // ButtonExecute
            // 
            this.ButtonExecute.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ButtonExecute.Location = new System.Drawing.Point(200, 916);
            this.ButtonExecute.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonExecute.Name = "ButtonExecute";
            this.ButtonExecute.Size = new System.Drawing.Size(180, 40);
            this.ButtonExecute.TabIndex = 999;
            this.ButtonExecute.Text = "登録";
            this.ButtonExecute.UseVisualStyleBackColor = true;
            this.ButtonExecute.Click += new System.EventHandler(this.ButtonExecute_Click);
            this.ButtonExecute.Enter += new System.EventHandler(this.ButtonExec_Enter);
            this.ButtonExecute.Leave += new System.EventHandler(this.ButtonExec_Leave);
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.SystemColors.HotTrack;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Meiryo UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTitle.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(921, 40);
            this.lblTitle.TabIndex = 254;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.BackColor = System.Drawing.SystemColors.Control;
            this.lblStatus.Font = new System.Drawing.Font("Meiryo UI", 21F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblStatus.Location = new System.Drawing.Point(790, 37);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(133, 36);
            this.lblStatus.TabIndex = 281;
            this.lblStatus.Text = "lblStatus";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // PanelEntry
            // 
            this.PanelEntry.AutoScroll = true;
            this.PanelEntry.BackColor = System.Drawing.Color.DarkGray;
            this.PanelEntry.ForeColor = System.Drawing.SystemColors.ControlText;
            this.PanelEntry.Location = new System.Drawing.Point(0, 80);
            this.PanelEntry.Name = "PanelEntry";
            this.PanelEntry.Size = new System.Drawing.Size(259, 180);
            this.PanelEntry.TabIndex = 283;
            this.PanelEntry.Visible = false;
            // 
            // lblParams
            // 
            this.lblParams.AutoSize = true;
            this.lblParams.Font = new System.Drawing.Font("Meiryo UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblParams.Location = new System.Drawing.Point(-2, 38);
            this.lblParams.Name = "lblParams";
            this.lblParams.Size = new System.Drawing.Size(126, 30);
            this.lblParams.TabIndex = 284;
            this.lblParams.Text = "lblParams";
            // 
            // TextDummy
            // 
            this.TextDummy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.TextDummy.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextDummy.Enabled = false;
            this.TextDummy.Location = new System.Drawing.Point(10000, 10000);
            this.TextDummy.Name = "TextDummy";
            this.TextDummy.Size = new System.Drawing.Size(75, 16);
            this.TextDummy.TabIndex = 10000;
            this.TextDummy.TabStop = false;
            this.TextDummy.Visible = false;
            // 
            // LabelDummy
            // 
            this.LabelDummy.AutoSize = true;
            this.LabelDummy.Location = new System.Drawing.Point(12, 893);
            this.LabelDummy.Name = "LabelDummy";
            this.LabelDummy.Size = new System.Drawing.Size(56, 16);
            this.LabelDummy.TabIndex = 285;
            this.LabelDummy.Text = "label1";
            this.LabelDummy.Visible = false;
            // 
            // GroupBoxDummy
            // 
            this.GroupBoxDummy.Controls.Add(this.RadioButtonNo);
            this.GroupBoxDummy.Controls.Add(this.RadioButtonYes);
            this.GroupBoxDummy.Controls.Add(this.label2);
            this.GroupBoxDummy.Font = new System.Drawing.Font("Meiryo UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.GroupBoxDummy.Location = new System.Drawing.Point(20, 135);
            this.GroupBoxDummy.Name = "GroupBoxDummy";
            this.GroupBoxDummy.Size = new System.Drawing.Size(910, 168);
            this.GroupBoxDummy.TabIndex = 10004;
            this.GroupBoxDummy.TabStop = false;
            this.GroupBoxDummy.Text = "書類仕分";
            this.GroupBoxDummy.Visible = false;
            // 
            // RadioButtonNo
            // 
            this.RadioButtonNo.AutoSize = true;
            this.RadioButtonNo.Location = new System.Drawing.Point(33, 122);
            this.RadioButtonNo.Name = "RadioButtonNo";
            this.RadioButtonNo.Size = new System.Drawing.Size(135, 30);
            this.RadioButtonNo.TabIndex = 2;
            this.RadioButtonNo.TabStop = true;
            this.RadioButtonNo.Text = "規定外書類";
            this.RadioButtonNo.UseVisualStyleBackColor = true;
            this.RadioButtonNo.Click += new System.EventHandler(this.RadioButtonNo_Click);
            // 
            // RadioButtonYes
            // 
            this.RadioButtonYes.AutoSize = true;
            this.RadioButtonYes.Checked = true;
            this.RadioButtonYes.Location = new System.Drawing.Point(33, 79);
            this.RadioButtonYes.Name = "RadioButtonYes";
            this.RadioButtonYes.Size = new System.Drawing.Size(65, 30);
            this.RadioButtonYes.TabIndex = 1;
            this.RadioButtonYes.TabStop = true;
            this.RadioButtonYes.Text = "はい";
            this.RadioButtonYes.UseVisualStyleBackColor = true;
            this.RadioButtonYes.Click += new System.EventHandler(this.RadioButtonYes_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(169, 26);
            this.label2.TabIndex = 0;
            this.label2.Text = "規定内書類ですか";
            // 
            // labelKansaId
            // 
            this.labelKansaId.AutoSize = true;
            this.labelKansaId.Font = new System.Drawing.Font("ＭＳ 明朝", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelKansaId.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelKansaId.Location = new System.Drawing.Point(14, 106);
            this.labelKansaId.Name = "labelKansaId";
            this.labelKansaId.Size = new System.Drawing.Size(802, 21);
            this.labelKansaId.TabIndex = 10006;
            this.labelKansaId.Text = "監査ID：WWWWWWWW10WWWWWWWW20WWWWWWWW20WWWWWWWW20WWWWWWWW20WWWWWWWW20WWWW";
            this.labelKansaId.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Meiryo UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label1.Location = new System.Drawing.Point(59, 500);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 26);
            this.label1.TabIndex = 10007;
            this.label1.Text = "監査ID：";
            this.label1.Visible = false;
            // 
            // FrmEntry
            // 
            this.AcceptButton = this.ButtonExecute;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(921, 1020);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelKansaId);
            this.Controls.Add(this.GroupBoxDummy);
            this.Controls.Add(this.TextDummy);
            this.Controls.Add(this.LabelDummy);
            this.Controls.Add(this.PanelEntry);
            this.Controls.Add(this.ButtonBack);
            this.Controls.Add(this.ButtonClose);
            this.Controls.Add(this.ButtonExecute);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblParams);
            this.Controls.Add(this.lblStatus);
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmEntry";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmEntry";
            this.Load += new System.EventHandler(this.FrmEntry_Load);
            this.Shown += new System.EventHandler(this.FrmEntry_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmEntry_KeyDown);
            this.GroupBoxDummy.ResumeLayout(false);
            this.GroupBoxDummy.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonBack;
        private System.Windows.Forms.Button ButtonClose;
        private System.Windows.Forms.Button ButtonExecute;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ToolTip toolTips;
        public System.Windows.Forms.Label lblTitle;
        protected System.Windows.Forms.Panel PanelEntry;
        private System.Windows.Forms.Label lblParams;
        private System.Windows.Forms.TextBox TextDummy;
        private System.Windows.Forms.Label LabelDummy;
        //private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        //private System.Windows.Forms.GroupBox groupBox1;
        //private System.Windows.Forms.Button button1;
        //private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox GroupBoxDummy;
        private System.Windows.Forms.RadioButton RadioButtonNo;
        private System.Windows.Forms.RadioButton RadioButtonYes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelKansaId;
        private System.Windows.Forms.Label label1;
    }
}