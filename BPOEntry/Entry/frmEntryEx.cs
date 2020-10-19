using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using CTextBox;

namespace DIWEntry.EntryForms
{
    public partial class frmEntryEx : frmEntry
    {

        private System.Windows.Forms.GroupBox[] gbsx;
        private CTextBox.CTextBox[] _tbsx;
        private System.Windows.Forms.Label[] lbsx;

        private static Encoding encsjis = Encoding.GetEncoding("Shift_JIS");
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <param name="num"></param>
        /// <param name="grp"></param>
        public frmEntryEx(string id, string date, string num, string grp)
            : base(id, date, num, grp)
        {
            InitializeComponent();

            this.Top = 0;
            this.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2 + 5;
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - 2;
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2 - 5;
        }

        protected override void InitTextBoxExtentionProperties(string sDocId)
        {
            _Log.Info(String.Format("帳票ID：{0}", sDocId));

            TextFieldParser parser = new TextFieldParser(System.Windows.Forms.Application.StartupPath + String.Format(@"\{0}.txt", sDocId), encsjis);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters("\t");                    // タブ区切り(TSVファイルの場合)

            DataTable dt = new DataTable();
            string[] data = null;
            //データがあるか確認します。
            if (!parser.EndOfData)
            {
                //CSVファイルから1行読み取ります。
                data = parser.ReadFields();

                //カラムの数を取得します。
                int cols = data.Length;

                for (int i = 0; i < cols; i++)
                {
                    //カラム名をセットします
                    dt.Columns.Add(data[i]);
                }
            }

            // CSVをデータテーブルに格納
            while (!parser.EndOfData)
            {
                data = parser.ReadFields();
                DataRow row = dt.NewRow();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    row[i] = data[i];
                }
                dt.Rows.Add(row);
            }
            parser.Dispose();

            // GroupBox
            DataRow[] drs = dt.Select("Kind='G'");
            if (drs.Length != 0)
                this.gbsx = new GroupBox[drs.Length];
            int iItemNumber = 0;
            foreach (DataRow dr in drs)
            {
                this.gbsx[iItemNumber] = new GroupBox();
                //this.gbs[iItemNumber].BackColor = Color.Red;
                this.gbsx[iItemNumber].Name = "GroupBox_" + dr["ControlName"].ToString();
                this.gbsx[iItemNumber].Text = dr["Text"].ToString();
                this.gbsx[iItemNumber].Location = new Point(int.Parse(dr["Location_Y"].ToString()), int.Parse(dr["Location_X"].ToString()));
                this.gbsx[iItemNumber].Width = int.Parse(dr["Width"].ToString());
                this.gbsx[iItemNumber].Height = int.Parse(dr["Height"].ToString());
                this.gbsx[iItemNumber].Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));

                iItemNumber++;
            }

            // TextBox
            drs = dt.Select("Kind='T'");
            if (drs.Length != 0)
                this._tbsx = new CTextBox.CTextBox[drs.Length];
            iItemNumber = 0;
            System.Windows.Forms.Label lblDummy = new Label();
            lblDummy.Visible = false;
            lblDummy.AutoSize = true;
            
            //string sDummy = string.Empty;

            foreach (DataRow dr in drs)
            {

                this._tbsx[iItemNumber] = new CTextBox.CTextBox();

                // Name
                this._tbsx[iItemNumber].Name = "text" + (iItemNumber + 1).ToString("000");

                // Text
                this._tbsx[iItemNumber].Text = dr["Text"].ToString();

                // Location
                this._tbsx[iItemNumber].Location = new Point(int.Parse(dr["Location_Y"].ToString()), int.Parse(dr["Location_X"].ToString()));

                // Font
                this._tbsx[iItemNumber].Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));

                if ("T".Equals(dr["TabStop"].ToString()))
                {
                    // TabStop
                    this._tbsx[iItemNumber].TabStop = true;

                    // TabIndex
                    this._tbsx[iItemNumber].TabIndex = int.Parse(dr["TabIndex"].ToString());
                }
                else
                {
                    // TabStop
                    this._tbsx[iItemNumber].TabStop = false;

                    // TabIndex
                    this._tbsx[iItemNumber].TabIndex = 99999;
                }


                // MaxLength
                this._tbsx[iItemNumber].MaxLength = int.Parse(dr["Maxlength"].ToString());


                // Width
                if (!string.Empty.Equals(dr["Height"].ToString())
                    || !string.Empty.Equals(dr["Width"].ToString()))
                {
                    if (!string.Empty.Equals(dr["Height"].ToString()))
                    {
                        this._tbsx[iItemNumber].Height = int.Parse(dr["Height"].ToString());
                    }
                    if (!string.Empty.Equals(dr["Width"].ToString()))
                    {
                        this._tbsx[iItemNumber].Width = int.Parse(dr["Width"].ToString());
                    }
                }
                else
                {
                    if ("カナ".Equals(dr["ImeMode"].ToString())
                        || "ひらがな".Equals(dr["ImeMode"].ToString()))
                    {
                        lblDummy.Text = "".PadRight(int.Parse(dr["MaxLength"].ToString()), '亜');
                    }
                    else
                    {
                        lblDummy.Text = "".PadRight(int.Parse(dr["MaxLength"].ToString()), 'W');
                    }
                    this._tbsx[iItemNumber].Width = lblDummy.Width;
                }

                // MultiLine
                if ("T".Equals(dr["MultiLine"].ToString()))
                {
                    this._tbsx[iItemNumber].Multiline = true;
                }

                // Visible
                if ("F".Equals(dr["Visible"].ToString()))
                {
                    this._tbsx[iItemNumber].Visible = false;
                }


                //this.tbs[iItemNumber].Validating += new CancelEventHandler(this.textBox_Validating);

                // Required
                this._tbsx[iItemNumber].Required = "T".Equals(dr["Required"].ToString()) ? true : false;

                // Enabled
                this._tbsx[iItemNumber].Enabled = "T".Equals(dr["Enabled"].ToString()) ? true : false;

                // TabStop
                this._tbsx[iItemNumber].TabStop = "T".Equals(dr["TabStop"].ToString()) ? true : false;

                // Add To GroupBox
                for (int iIdx = 0; iIdx < this.gbsx.Length; iIdx++)
                {
                    if (("GroupBox_" + dr["GroupBox"].ToString()).Equals(gbsx[iIdx].Name))
                    {
                        this.gbsx[iIdx].Controls.Add(this._tbsx[iItemNumber]);
                    }
                }

                // ImeMode
                switch (dr["ImeMode"].ToString())
                {
                    case "9":
                    case "X":
                        this._tbsx[iItemNumber].ImeMode = System.Windows.Forms.ImeMode.Disable;
                        break;
                    case "XK":
                        this._tbsx[iItemNumber].ImeMode = System.Windows.Forms.ImeMode.KatakanaHalf;
                        break;
                    case "NK":
                        this._tbsx[iItemNumber].ImeMode = System.Windows.Forms.ImeMode.Katakana;
                        break;
                    case "NH":
                        this._tbsx[iItemNumber].ImeMode = System.Windows.Forms.ImeMode.Hiragana;
                        break;
                }
                this._tbsx[iItemNumber].ImeModeFix = this._tbsx[iItemNumber].ImeMode;

                // ValidPattern
                this._tbsx[iItemNumber].ValidPattern = dr["VALIDPattern"].ToString();

                // Tips
                this._tbsx[iItemNumber].Tips = dr["Tips"].ToString();

                // ItemName
                this._tbsx[iItemNumber].ItemName = dr["ItemName"].ToString();

                // Regex
                this._tbsx[iItemNumber].Regex = dr["Regex"].ToString();

                iItemNumber++;
            }

            // Label
            drs = dt.Select("Kind='L'");
            if (drs.Length != 0)
                this.lbsx = new Label[drs.Length];
            iItemNumber = 0;
            foreach (DataRow dr in drs)
            {
                this.lbsx[iItemNumber] = new Label();

                // Name
                this.lbsx[iItemNumber].Name = "label_" + (iItemNumber + 1).ToString("000");

                // Text
                this.lbsx[iItemNumber].Text = dr["Text"].ToString();

                // Location
                this.lbsx[iItemNumber].Location = new Point(int.Parse(dr["Location_Y"].ToString()), int.Parse(dr["Location_X"].ToString()));

                // Font
                this.lbsx[iItemNumber].Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));

                // AutoSize
                this.lbsx[iItemNumber].AutoSize = true;

                // ForeColor
                switch (dr["ForeColor"].ToString())
                {
                    case "R":
                        this.lbsx[iItemNumber].ForeColor = Color.Red;
                        break;
                    case "B":
                        this.lbsx[iItemNumber].ForeColor = Color.Blue;
                        break;
                }

                // Add To GroupBox
                for (int iIdx = 0; iIdx < this.gbsx.Length; iIdx++)
                {
                    if (("GroupBox_" + dr["GroupBox"].ToString()).Equals(gbsx[iIdx].Name))
                    {
                        this.gbsx[iIdx].Controls.Add(this.lbsx[iItemNumber]);
                    }
                }
                iItemNumber++;
            }

            // Add To Panel
            this.pnlEntry.Controls.AddRange(this.gbsx);

            //for (int iIdx = 0; iIdx < this.tbs.Length; iIdx++)
            //{
            //    this._extProps.Add(this.tbs[iIdx], new EntryItemExtensionProperties("X", this.tbs[iIdx].MaxLength, required: this.tbs[iIdx].Required)
            //    {
            //        ValidStringPattern = this.tbs[iIdx].ValidPattern,
            //        AcceptKeyCharsPattern = this.tbs[iIdx].Regex,
            //        ItemName = this.tbs[iIdx].ItemName
            //        ,
            //        Tips = this.tbs[iIdx].Tips
            //    });
            //}



            // Focus Set
            if (this._tbsx != null)
            {
                for (int iIdx = 0; iIdx < this._tbsx.Length; iIdx++)
                {
                    if (this._tbsx[iIdx].Visible
                        && this._tbsx[iIdx].TabStop)
                    {
                        this._tbsx[iIdx].Focus();
                        this._tbsx[0].SelectAll();
                        break;
                    }
                }
            }












            /*


            this._extProps.Add(this.tbs[0], new EntryItemExtensionProperties("X", 21)
            {
            });
            this._extProps.Add(this.tbs[1], new EntryItemExtensionProperties("9", 9)
            {
            });
                        this._extProps.Add(text003, new EntryItemExtensionProperties("9", 3)
                        {
                        });
                        this._extProps.Add(text004, new EntryItemExtensionProperties("9", 8)
                        {
                        });

                        this._extProps.Add(text005, new EntryItemExtensionProperties("9", 2)
                        {
                            ValidStringPattern = "^(00|01|02|03|04|05|06|07|08|09|10)*$",
                            AcceptKeyCharsPattern = "^[0-9]$",
                            ItemName = "Ｑ１"
                            //,                Tips = "00～10で入力"
                        });
                        this._extProps.Add(text006, new EntryItemExtensionProperties("9", 2)
                        {
                            ValidStringPattern = "^(00|01|02|03|04|05|06|07|08|09|10)*$",
                            AcceptKeyCharsPattern = "^[0-9]$",
                            ItemName = "Ｑ２"
                            //,                Tips = "00～10で入力"
                        });
                        this._extProps.Add(text007, new EntryItemExtensionProperties("9", 2)
                        {
                            ValidStringPattern = "^(00|01|02|03|04|05|06|07|08|09|10)*$",
                            AcceptKeyCharsPattern = "^[0-9]$",
                            ItemName = "Ｑ３"
                            //,                Tips = "00～10で入力"
                        });
                        this._extProps.Add(text008, new EntryItemExtensionProperties("NH", 501, input2: false)
                        {
                            ItemName = "Ｑ４"
                            //,Tips = "※不明文字は＊（全角アスタリスク）を入力　個人情報は●を入力"
                        });
                        this._extProps.Add(text009, new EntryItemExtensionProperties("NH", 501, input2: false)
                        {
                            ItemName = "Ｑ５"
                            //,Tips = "※不明文字は＊（全角アスタリスク）を入力　個人情報は●を入力"
                        });
                        */
              }
    }
}
