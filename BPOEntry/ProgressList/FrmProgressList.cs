using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BPOEntry.UserManage;
using Common;
using NLog;

namespace BPOEntry.Progress
{
    /// <summary>
    /// エントリ状況一覧
    /// </summary>
    public partial class FrmProgressList : Form
    {
        /// <summary>
        /// Log
        /// </summary>
        private static Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Dao
        /// </summary>
        private ProgressDao _dao = new ProgressDao();

        /// <summary>
        /// 最新表示間隔（秒）
        /// </summary>
        private int iSecond = 60;

        private bool bD = false;
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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrmProgressList()
        {
            InitializeComponent();

            #region 画面位置、サイズ調整
            //プライマリディスプレイの作業領域の高さと幅を取得
            this.Top = 0;
            this.Left = 0;
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;// - 2;
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;// - 2;

            this.ButtonShow.Top = this.Height - 80;
            this.label3.Top = this.ButtonShow.Top;

            this.ButtonClose.Top = this.ButtonShow.Top;
            this.ButtonClose.Left = this.Width - 205;

            this.labelDateTime.Left = this.Width - this.labelDateTime.Width - 2;
            this.checkBox2.Left = this.Width - 185;

            this.lvProgressList.Height = this.Height - 200;
            this.lvProgressList.Width = this.Width - 45;

            bool b = _dao.IsExistsPrelogicalCheckTargetDoc();
            this.label7.Visible = b;
            this.label7.Top = this.ButtonShow.Top;
            //this.label7.Left = this.btnClose.Left - 630;

            

            #endregion

            //this.Text = Config.BusinessName;
            this.Text = Utils.GetFormText();
            this.lblTitle.BackColor = Config.LblTitleBackColor;

            #region 連携年月日
            if (Config.ExportDateDiff < 0)
            {
                DateTime dtNow = System.DateTime.Now;
                DateTime dtTarget = System.DateTime.Now;
                int iIdx = Config.ExportDateDiff;
                while (true)
                {
                    dtTarget = dtNow.AddDays(iIdx);
                    if (dtTarget.DayOfWeek.Equals(DayOfWeek.Saturday)
                        || dtTarget.DayOfWeek.Equals(DayOfWeek.Sunday))
                    {
                        iIdx--;
                        continue;
                    }
                    this.dtpImageCaptureDate.Value = dtTarget;
                    break;
                }
            }
            #endregion

            #region ステータス
            List<BPOEntry.UserManage.ItemSet> src = new List<ItemSet>();
            src.Add(new ItemSet("*", "全て"));
            src.Add(new ItemSet("0", "0:未入力"));
            src.Add(new ItemSet("1", "1:入力中"));
            src.Add(new ItemSet("2", "2:入力済み"));
            src.Add(new ItemSet("5", "5:コンペア待ち"));
            src.Add(new ItemSet("6", "6:コンペア修正中"));
            src.Add(new ItemSet("7", "7:コンペア済み"));
            src.Add(new ItemSet("8", "8:テキスト出力中"));
            src.Add(new ItemSet("9", "9:テキスト出力済み"));

            DropDownListStatus.DataSource = src;
            DropDownListStatus.DisplayMember = "ItemDisp";
            DropDownListStatus.ValueMember = "ItemValue";
            #endregion

            #region 業務区分
            src = new List<ItemSet>();
            src.Add(new ItemSet("*", "全て"));
            var dtM_CODE_DEFINE = _dao.SELECT_M_CODE_DEFINE("業務区分");
            if (dtM_CODE_DEFINE.Rows.Count == 0)
                dtM_CODE_DEFINE = _dao.SELECT_M_CODE_DEFINE("帳票種別GRP");
            foreach (DataRow drM_CODE_DEFINE in dtM_CODE_DEFINE.Rows)
                src.Add(new ItemSet(drM_CODE_DEFINE["KEY"].ToString(), String.Format("{0}:{1}", drM_CODE_DEFINE["KEY"].ToString(), drM_CODE_DEFINE["VALUE_1"].ToString())));
            if (src.Count == 1)
                DropDownListGyoumKbn.Enabled = false;

            DropDownListGyoumKbn.DataSource = src;
            DropDownListGyoumKbn.DisplayMember = "ItemDisp";
            DropDownListGyoumKbn.ValueMember = "ItemValue";
            #endregion

            #region 表示順
            src = new List<ItemSet>();
            src.Add(new ItemSet("1", "1:ステータス"));
            src.Add(new ItemSet("2", "2:エントリバッチID"));

            this.comboBox1.DataSource = src;
            this.comboBox1.DisplayMember = "ItemDisp";
            this.comboBox1.ValueMember = "ItemValue";
            #endregion

            this.DropDownListGyoumKbn.SelectedValueChanged += new System.EventHandler(this.DropDownList_SelectedValueChanged);
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            this.DropDownListStatus.SelectedValueChanged += new System.EventHandler(this.DropDownList_SelectedValueChanged);
            this.dtpImageCaptureDate.ValueChanged += new System.EventHandler(this.dtpImageCaptureDate_ValueChanged);

            bD = true;
            // リスト表示
            DisplayProgressList();
        }

        #region 閉じるボタン
        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        private void DisplayProgressList()
        {
            if (!bD)
            {
                return;
            }
            _log.Info("表示開始　連携年月日:{0}　回数:{1}　帳票種別GRP:{2}　ステータス:{3}　テキスト出力:{4}"
                , checkBox1.Checked ? dtpImageCaptureDate.Value.ToString("yyyy/MM/dd") : "指定無し"
                , checkBox1.Checked ? this.numericUpDown1.Value.ToString("00") :"指定無し"
                , DropDownListGyoumKbn.SelectedValue.ToString()
                , DropDownListStatus.SelectedValue.ToString()
                , checkBox3.Checked ? dateTimePicker1.Value.ToString("yyyy/MM/dd") : "指定無し"
                );

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            var iCount = 0;
            var iVerifiedCount = 0;
            var iNgCount = 0;

            //詳細表示にする
            lvProgressList.BeginUpdate();
            lvProgressList.Clear();
            lvProgressList.View = View.Details;

            //ヘッダーを追加する（ヘッダー名、幅、アライメント）
            lvProgressList.Columns.Add("dummy", 0, HorizontalAlignment.Center);
            lvProgressList.Columns.Add("№", 50, HorizontalAlignment.Center);
            lvProgressList.Columns.Add("エントリバッチID", 285, HorizontalAlignment.Left);
            lvProgressList.Columns.Add("帳票", 400, HorizontalAlignment.Left);
            lvProgressList.Columns.Add("件数", 60, HorizontalAlignment.Right);
            lvProgressList.Columns.Add("ステータス", 175, HorizontalAlignment.Left);
            lvProgressList.Columns.Add("１人目", 160, HorizontalAlignment.Left);
            lvProgressList.Columns.Add("エントリ時間", 95, HorizontalAlignment.Center);
            lvProgressList.Columns.Add("２人目", 160, HorizontalAlignment.Left);
            lvProgressList.Columns.Add("エントリ時間", 95, HorizontalAlignment.Center);
            lvProgressList.Columns.Add("ＯＣＲ", 160, HorizontalAlignment.Left);
            lvProgressList.Columns.Add("コンペア修正", 160, HorizontalAlignment.Left);
            lvProgressList.Columns.Add("修正時間", 95, HorizontalAlignment.Center);
            lvProgressList.Columns.Add("検証", 140, HorizontalAlignment.Left);
            lvProgressList.Columns.Add("出力日時", 185, HorizontalAlignment.Center);
            lvProgressList.Columns.Add("帳票種別グループ", 160, HorizontalAlignment.Left);

            var sDate = string.Empty;
            var sExportDate = string.Empty;
            if (checkBox1.Checked)
                sDate = dtpImageCaptureDate.Value.ToString("yyyyMMdd");

            if (checkBox3.Checked)
                sExportDate = dateTimePicker1.Value.ToString("yyyyMMdd");

            // 
            var dtProgressList = _dao.SELECT_D_ENTRY_UNIT(sDate, this.numericUpDown1.Value.ToString("00"), DropDownListStatus.SelectedValue.ToString(), DropDownListGyoumKbn.SelectedValue.ToString(), this.comboBox1.SelectedValue.ToString(),sExportDate);
            int iNumber = 1;
            foreach (DataRow drProgressList in dtProgressList.Rows)
            {
                var itemx = new ListViewItem();
                //アイテムの作成
                itemx.Text = "*";
                // No
                itemx.SubItems.Add((iNumber++).ToString().PadLeft(4));
                itemx.SubItems.Add(Utils.EditEntryBatchId(drProgressList["ENTRY_UNIT_ID"].ToString()));
                itemx.SubItems.Add(drProgressList["DOC_NAME"].ToString());                                  // 帳票
                itemx.SubItems.Add((int.Parse(drProgressList["IMAGE_COUNT"].ToString()).ToString("#,0")));  // 件数
                itemx.SubItems.Add(drProgressList["STATUS_NAME"].ToString());                               // 状態
                itemx.SubItems.Add(drProgressList["ENTRY_USER_NAME_1ST"].ToString());                       // １人目

                // １人目エントリ時間
                if (drProgressList["ENTRY_START_TIME_1ST"].ToString().Length != 0
                    && drProgressList["ENTRY_END_TIME_1ST"].ToString().Length != 0)
                {
                    DateTime st = DateTime.Parse(drProgressList["ENTRY_START_TIME_1ST"].ToString());
                    DateTime ed = DateTime.Parse(drProgressList["ENTRY_END_TIME_1ST"].ToString());
                    TimeSpan ts = ed.Subtract(st);
                    itemx.SubItems.Add(ts.ToString());
                }
                else
                {
                    itemx.SubItems.Add(string.Empty);
                }

                // ２人目（シングルエントリの場合対象外）
                if (Consts.Flag.ON.Equals(drProgressList["SINGLE_ENTRY_FLAG"].ToString()))
                {
                    itemx.SubItems.Add("-");
                }
                else
                {
                    itemx.SubItems.Add(drProgressList["ENTRY_USER_NAME_2ND"].ToString());
                }

                // ２人目エントリ時間
                if (drProgressList["ENTRY_START_TIME_2ND"].ToString().Length != 0
                    && drProgressList["ENTRY_END_TIME_2ND"].ToString().Length != 0)
                {
                    DateTime st = DateTime.Parse(drProgressList["ENTRY_START_TIME_2ND"].ToString());
                    DateTime ed = DateTime.Parse(drProgressList["ENTRY_END_TIME_2ND"].ToString());
                    TimeSpan ts = ed.Subtract(st);
                    itemx.SubItems.Add(ts.ToString());
                }
                else
                {
                    itemx.SubItems.Add(string.Empty);
                }

                // ＯＣＲ
                if (Consts.Flag.ON.Equals(drProgressList["OCR_COOPERATION_FLAG"].ToString()))
                {
                    if (drProgressList["OCR_IMPORT_USER_NAME"].ToString().Length != 0)
                    {
                        itemx.SubItems.Add("取込済");
                    }
                    else
                    {
                        itemx.SubItems.Add("未取込");
                    }
                }
                else
                {
                    itemx.SubItems.Add("-");
                }

                // コンペア修正（シングルエントリの場合対象外）
                if (Consts.Flag.ON.Equals(drProgressList["SINGLE_ENTRY_FLAG"].ToString()))
                {
                    itemx.SubItems.Add("-");
                }
                else
                {
                    if (drProgressList["UPD_ENTRY_END_TIME"].ToString().Length != 0)
                    {
                        itemx.SubItems.Add(drProgressList["UPD_USER_NAME"].ToString());
                    }
                    else
                    {
                        if (drProgressList["UPD_ENTRY_START_TIME"].ToString().Length != 0
                            && drProgressList["UPD_ENTRY_END_TIME"].ToString().Length == 0)
                        {
                            itemx.SubItems.Add(drProgressList["UPD_USER_NAME"].ToString() + "（修正中）");
                        }
                        else
                        {
                            itemx.SubItems.Add(string.Empty);
                        }
                    }
                }

                // コンペア修正時間
                if (drProgressList["UPD_ENTRY_START_TIME"].ToString().Length != 0
                    && drProgressList["UPD_ENTRY_END_TIME"].ToString().Length != 0)
                {
                    DateTime st = DateTime.Parse(drProgressList["UPD_ENTRY_START_TIME"].ToString());
                    DateTime ed = DateTime.Parse(drProgressList["UPD_ENTRY_END_TIME"].ToString());
                    TimeSpan ts = ed.Subtract(st);
                    itemx.SubItems.Add(ts.ToString());
                }
                else
                {
                    itemx.SubItems.Add(string.Empty);
                }

                if (drProgressList["VERIFY_ENTRY_USER_NAME"].ToString().Length != 0)
                {
                    itemx.SubItems.Add(drProgressList["VERIFY_ENTRY_USER_NAME"].ToString());
                }
                else
                {
                    if (drProgressList["UPD_ENTRY_END_TIME"].ToString().Length != 0)
                    {
                        itemx.SubItems.Add("未実施");
                    }
                    else
                    {
                        itemx.SubItems.Add(string.Empty);
                    }
                }

                // テキスト出力
                if (drProgressList["TEXT_EXPORT_DATE"].ToString().Length != 0)
                {
                    itemx.SubItems.Add(drProgressList["TEXT_EXPORT_DATE"].ToString());
                }
                else
                {
                    itemx.SubItems.Add(string.Empty);
                }

                // 業務区分
                itemx.SubItems.Add(drProgressList["GYOUM_KBN"].ToString());

                //アイテムをリスビューに追加する
                lvProgressList.Items.Add(itemx);

                if (Consts.EntryUnitStatus.ENTRY_NOT.Equals(drProgressList["STATUS"].ToString()))
                {
                    lvProgressList.Items[lvProgressList.Items.Count - 1].BackColor = Color.Pink;
                }
                else if (Consts.EntryUnitStatus.ENTRY_EDT.Equals(drProgressList["STATUS"].ToString())
                        || Consts.EntryUnitStatus.COMPARE_ING.Equals(drProgressList["STATUS"].ToString()))
                {
                    lvProgressList.Items[lvProgressList.Items.Count - 1].BackColor = Color.LemonChiffon;
                }
                else if (Consts.EntryUnitStatus.COMPARE_EDT.Equals(drProgressList["STATUS"].ToString())
                        || Consts.EntryUnitStatus.COMPARE_END.Equals(drProgressList["STATUS"].ToString()))
                {
                    lvProgressList.Items[lvProgressList.Items.Count - 1].BackColor = Color.SkyBlue;
                }

                iCount += int.Parse(drProgressList["IMAGE_COUNT"].ToString());
                iVerifiedCount += int.Parse(drProgressList["Verified_COUNT"].ToString());
                iNgCount += int.Parse(drProgressList["NG_COUNT"].ToString());
            }
            lvProgressList.EndUpdate();

            if (this.lvProgressList.Items.Count == 0)
            {
                this.ButtonShow.Focus();
            }
            else
            {
                // 1行目にフォーカス
                this.lvProgressList.Items[0].Selected = true;
                this.lvProgressList.Focus();
            }

            // 最終表示日時設定
            labelDateTime.Text = System.DateTime.Now.ToString("最終表示時刻：yyyy/MM/dd HH:mm:ss");

            if (checkBox2.Checked)
            {
                timer1.Enabled = false;
                timer1.Interval = iSecond * 1000;
                timer1.Enabled = true;
            }
            else
            {
                timer1.Enabled = false;
            }
            sw.Stop();
            _log.Info("表示終了　件数:{0}　経過時間：{1}", lvProgressList.Items.Count.ToString("#,0"), sw.Elapsed);

            this.label3.Text = String.Format("帳票枚数：{0}", iCount.ToString("#,0 件"));
            this.label7.Text = String.Format("不備検証済帳票枚数：{0} 不備件数：{1} ", iVerifiedCount.ToString("#,0 件"), iNgCount.ToString("#,0 件"));
        }

        private void dtpImageCaptureDate_ValueChanged(object sender, EventArgs e)
        {
            DisplayProgressList();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.dtpImageCaptureDate.Enabled = checkBox1.Checked;
            this.numericUpDown1.Enabled = checkBox1.Checked;
            DisplayProgressList();
        }

        private void ButtonShow_Click(object sender, EventArgs e)
        {
            DisplayProgressList();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DisplayProgressList();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                timer1.Interval = iSecond * 1000;
                timer1.Enabled = true;
            }
            else
            {
                timer1.Enabled = false;
            }
        }

        private void DropDownList_SelectedValueChanged(object sender, EventArgs e)
        {
            DisplayProgressList();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            DisplayProgressList();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            DisplayProgressList();
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            DisplayProgressList();
        }

        private void checkBox3_CheckedChanged_1(object sender, EventArgs e)
        {
            this.dateTimePicker1.Enabled = checkBox3.Checked;
            DisplayProgressList();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DisplayProgressList();
        }
    }
}
