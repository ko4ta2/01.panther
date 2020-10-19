﻿using BPOEntry.EntryForms;
using BPOEntry.Tables;
using BPOEntry.UserManage;
using Common;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace BPOEntry.ReEntrySelect
{
    public partial class FrmEntryUnitList : Form
    {
        #region CreateParams
        /// <summary>
        /// ショートカットキーなどでフォームが閉じられないようにします。
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                //var CS_NOCLOSE = ;
                CreateParams createParams = base.CreateParams;
                createParams.ClassStyle |= 0x200;
                return createParams;
            }
        }
        #endregion

        /// <summary>
        /// log
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        private D_ENTRY_UNIT SelectedReEntryList
        {
            get
            {
                var lvItems = this.lvReEntryList.SelectedItems[0];
                return new D_ENTRY_UNIT(lvItems.SubItems[2].Text, lvItems.SubItems[3].Text);
            }
        }

        //private string _sVerifyFlag = null;

        private string _UnitListType = null;

        private static DaoEntryUnitList dao = new DaoEntryUnitList();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrmEntryUnitList(string UnitListType)
        {
            InitializeComponent();

            //_sVerifyFlag = sVerifyFlag;
            _UnitListType = UnitListType;

            //this.Text = Config.BusinessName;
            this.Text = Utils.GetFormText();
            this.lblTitle.BackColor = Config.LblTitleBackColor;

            //if ("3".Equals(_UnitListType))
            //{
            //    //    if (sEntryUnitSelectMode == null)
            //    //    {
            //    //        this.lblTitle.Text = "検証対象エントリ一覧";
            //    //        this.ButtonExec.Text = "検証";

            //    this.checkBox1.Visible = true;
            //    this.checkBox2.Visible = true;
            //    this.dateTimePicker1.Visible = true;
            //    //    }
            //    //    else if ("L".Equals(sEntryUnitSelectMode))
            //    //    {
            //    //        this.lblTitle.Text = "OCR取り込み結果不備検証";
            //    //        this.ButtonExec.Text = "検証";
            //    //    }
            //}
            //else
            //{
            //    if ("2".Equals(_sEntryUnitSelectMode))
            //    {
            //        this.lblTitle.Text = "エントリ対象一覧";
            //        this.ButtonExec.Text = "エントリ";
            //    }
            //}

            switch (_UnitListType)
            {
                case Consts.UnitListType.Entry:
                    this.lblTitle.Text = "エントリ対象一覧";
                    this.ButtonExec.Text = "エントリ";
                    break;
                case Consts.UnitListType.Modify:
                    this.lblTitle.Text = "修正対象一覧";
                    this.ButtonExec.Text = "修正";
                    break;
                case Consts.UnitListType.Verify:
                    this.lblTitle.Text = "検証対象一覧";
                    this.ButtonExec.Text = "検証";
                    this.checkBox1.Visible = true;
                    this.checkBox2.Visible = true;
                    this.dateTimePicker1.Visible = true;
                    break;
            }

            // 業務区分
            var src = new List<ItemSet>();
            src.Add(new ItemSet("*", "全て"));
            var dtM_CODE_DEFINE = dao.SELECT_M_CODE_DEFINE("業務区分");
            if (dtM_CODE_DEFINE.Rows.Count == 0)
            {
                dtM_CODE_DEFINE = dao.SELECT_M_CODE_DEFINE("帳票種別GRP");
            }

            foreach (var drM_CODE_DEFINE in dtM_CODE_DEFINE.AsEnumerable())
            {
                src.Add(new ItemSet(drM_CODE_DEFINE["KEY"].ToString(), String.Format("{0}:{1}", drM_CODE_DEFINE["KEY"].ToString(), drM_CODE_DEFINE["VALUE_1"].ToString())));
            }
            if (src.Count == 1)
            {
                DropDownListGyoumKbn.Enabled = false;
            }

            DropDownListGyoumKbn.DataSource = src;
            DropDownListGyoumKbn.DisplayMember = "ItemDisp";
            DropDownListGyoumKbn.ValueMember = "ItemValue";

            // 初期表示
            this.Search();
            this.DropDownListGyoumKbn.SelectedValueChanged += new System.EventHandler(this.DropDownListGyoumKbn_SelectedValueChanged);
        }

        /// <summary>
        /// 再検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonReSearch_Click(object sender, EventArgs e)
        {
            this.Search();
            if (this.lvReEntryList.Items.Count == 0)
            {
                MessageBox.Show(String.Format("{0}対象が見つかりません。", this.ButtonExec.Text), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 検索
        /// </summary>
        private void Search()
        {
            //詳細表示にする
            lvReEntryList.BeginUpdate();
            lvReEntryList.Clear();
            lvReEntryList.View = View.Details;

            //ヘッダーを追加する（ヘッダー名、幅、アライメント）
            lvReEntryList.Columns.Add("dummy", 0, HorizontalAlignment.Center);
            lvReEntryList.Columns.Add("№", 50, HorizontalAlignment.Center);

            // キー項目
            lvReEntryList.Columns.Add("ENTRY_UNIT_ID", 0, HorizontalAlignment.Left);
            lvReEntryList.Columns.Add("区分", 0, HorizontalAlignment.Center);

            // 表示項目
            lvReEntryList.Columns.Add("エントリバッチID", 348, HorizontalAlignment.Left);
            if (Consts.UnitListType.Verify.Equals(_UnitListType))
            {
                lvReEntryList.Columns.Add("帳票名", 610, HorizontalAlignment.Left);
                lvReEntryList.Columns.Add("検証者", 150, HorizontalAlignment.Left);
            }
            else
            {
                lvReEntryList.Columns.Add("帳票名", 760, HorizontalAlignment.Left);
                //    lvReEntryList.Columns.Add("検証者", 0, HorizontalAlignment.Left);
            }

            try
            {
                var bEntryCancelFlag = false;

                dao.Open(Config.DSN);

                // 対象データを検索する
                DataTable dtEntryUnit = null;
                /*
                                if ("2".Equals(_sEntryUnitSelectMode))
                                {
                                    dtEntryUnit = dao.SELECT_D_ENTRY_STATUS(DropDownListGyoumKbn.SelectedValue.ToString());
                                    if (!Consts.Flag.ON.Equals(_sVerifyFlag))
                                    {
                                        if (dtEntryUnit.Rows.Count != 0 && "1".Equals(dtEntryUnit.Rows[0]["STATUS"].ToString()))
                                        {
                                            bEntryCancelFlag = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if ("L".Equals(_sEntryUnitSelectMode))
                                    {
                                        dtEntryUnit = dao.SelectOcrNgUnit( DropDownListGyoumKbn.SelectedValue.ToString());
                                    }
                                    else
                                    {
                                        dtEntryUnit = dao.SelectUpdEntryUnit(_sVerifyFlag, this.checkBox1.Checked ? Consts.Flag.ON : Consts.Flag.OFF, this.checkBox2.Checked ? this.dateTimePicker1.Value.ToString("yyyyMMdd") : string.Empty, DropDownListGyoumKbn.SelectedValue.ToString());
                                    }
                                    if (!Consts.Flag.ON.Equals(_sVerifyFlag))
                                    {
                                        if (dtEntryUnit.Rows.Count != 0 && !string.Empty.Equals(dtEntryUnit.Rows[0]["UPD_ENTRY_USER_ID"].ToString()))
                                        {
                                            bEntryCancelFlag = true;
                                        }
                                    }
                                }
                */

                switch (_UnitListType)
                {
                    case Consts.UnitListType.Entry:
                        dtEntryUnit = dao.SELECT_D_ENTRY_STATUS(DropDownListGyoumKbn.SelectedValue.ToString());
                        log.Info("エントリModeで{0}UNIT取得", dtEntryUnit.Rows.Count.ToString("#,0"));
                        if (dtEntryUnit.Rows.Count != 0 && Consts.EntryStatus.ENTRY_ING.Equals(dtEntryUnit.Rows[0]["STATUS"].ToString()))
                        {
                            bEntryCancelFlag = true;
                        }
                        break;
                    case Consts.UnitListType.Modify:
                    //case Consts.UnitListType.Verify:
                        dtEntryUnit = dao.SELECT_D_ENTRY_UNIT(Consts.UnitListType.Modify.Equals(_UnitListType) ? Consts.Flag.OFF : Consts.Flag.ON, this.checkBox1.Checked ? Consts.Flag.ON : Consts.Flag.OFF, this.checkBox2.Checked ? this.dateTimePicker1.Value.ToString("yyyyMMdd") : string.Empty, DropDownListGyoumKbn.SelectedValue.ToString());
                        log.Info("修正、検証Modeで{0}UNIT取得", dtEntryUnit.Rows.Count.ToString("#,0"));
                        if (dtEntryUnit.Rows.Count != 0 && !string.Empty.Equals(dtEntryUnit.Rows[0]["UPD_ENTRY_USER_ID"].ToString()) && Consts.UnitListType.Modify.Equals(_UnitListType))
                        {
                            bEntryCancelFlag = true;
                        }
                        break;
                    case Consts.UnitListType.Verify:
                        dtEntryUnit = dao.SELECT_D_ENTRY_UNIT(Consts.UnitListType.Modify.Equals(_UnitListType) ? Consts.Flag.OFF : Consts.Flag.ON, this.checkBox1.Checked ? Consts.Flag.ON : Consts.Flag.OFF, this.checkBox2.Checked ? this.dateTimePicker1.Value.ToString("yyyyMMdd") : string.Empty, DropDownListGyoumKbn.SelectedValue.ToString());
                            //if (dtEntryUnit.Rows.Count != 0 && !string.Empty.Equals(dtEntryUnit.Rows[0]["UPD_ENTRY_USER_ID"].ToString()))
                            //{
                            //    bEntryCancelFlag = true;
                            //}
                            break;
                }

                int iListCount = 0;
                foreach (var drEntryUnit in dtEntryUnit.AsEnumerable())
                {
                    iListCount++;
                    ListViewItem lvItems = new ListViewItem();
                    //アイテムの作成
                    lvItems.Text = "*";
                    // No
                    lvItems.SubItems.Add((lvReEntryList.Items.Count + 1).ToString().PadLeft(3));

                    #region key項目
                    lvItems.SubItems.Add(drEntryUnit["ENTRY_UNIT_ID"].ToString());  // エントリユニットID
                    lvItems.SubItems.Add(drEntryUnit["RECORD_KBN"].ToString());     // レコード区分
                    #endregion

                    #region 表示項目
                    lvItems.SubItems.Add(Utils.EditEntryBatchId(drEntryUnit["ENTRY_UNIT_ID"].ToString()));  // エントリユニットID
                    lvItems.SubItems.Add(drEntryUnit["DOC_NAME"].ToString());                               // 帳票名
                    if (Consts.UnitListType.Verify.Equals(_UnitListType))
                    {
                        if (drEntryUnit["VERIFY_ENTRY_USER_NAME"].ToString().Length != 0)
                        {
                            if (Consts.Flag.ON.Equals(drEntryUnit["VERIFY_ING_FLAG"].ToString()))
                            {
                                lvItems.SubItems.Add(drEntryUnit["VERIFY_ENTRY_USER_NAME"].ToString() + "（検証中）");
                            }
                            else
                            {
                                lvItems.SubItems.Add(drEntryUnit["VERIFY_ENTRY_USER_NAME"].ToString());
                            }
                        }
                        else
                        {
                            lvItems.SubItems.Add("未実施");
                        }
                    }
                    else
                    {
                        lvItems.SubItems.Add(string.Empty);
                    }
                    #endregion

                    //アイテムをリスビューに追加する
                    this.lvReEntryList.Items.Add(lvItems);

                    if (iListCount % 2 != 0)
                    {
                        this.lvReEntryList.Items[this.lvReEntryList.Items.Count - 1].BackColor = Color.LightCyan;
                    }

                    if (bEntryCancelFlag || iListCount == 999)
                    {
                        break;
                    }
                }

                // 0件
                if (this.lvReEntryList.Items.Count == 0)
                {
                    this.ButtonSearch.Focus();
                }
                else
                {
                    // 1行目にフォーカス
                    this.lvReEntryList.Items[0].Selected = true;
                    this.lvReEntryList.Focus();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                lvReEntryList.EndUpdate();
                if (dao != null)
                {
                    dao.Close();
                }
            }
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClose_Click(object sender, EventArgs e)
        {
            //            this.DialogResult = DialogResult.No;
            this.Close();
            this.Dispose();
        }

        /// <summary>
        /// 修正
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonExec_Click(object sender, EventArgs e)
        {
            //var sMessage = "修正";
            //if (Consts.Flag.ON.Equals(_sVerifyFlag))
            //{
            //    sMessage = "検証";
            //}
            //else if ("2".Equals(_sEntryUnitSelectMode))
            //{
            //    sMessage = "エントリ";
            //}

            if (this.lvReEntryList.SelectedItems.Count < 1)
            {
                MessageBox.Show($"{this.ButtonExec.Text}対象が選択されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                dao.Open(Config.DSN);
                dao.BeginTrans();

                #region 最新ステータス取得
                DataTable dtReEntryUnitStatus;
                if (Consts.UnitListType.Entry.Equals(_UnitListType))
                {
                    // エントリ
                    dtReEntryUnitStatus = dao.SELECT_D_ENTRY_STATUS_FOR_UPDATE(this.SelectedReEntryList.ENTRY_UNIT_ID, this.SelectedReEntryList.RECORD_KBN);
                }
                else
                {
                    // 修正・検証
                    dtReEntryUnitStatus = dao.SELECT_D_ENTRY_UNIT_FOR_UPDATE(this.SelectedReEntryList.ENTRY_UNIT_ID);
                }
                if (dtReEntryUnitStatus.Rows.Count == 0)
                {
                    dao.RollbackTrans();
                    MessageBox.Show(String.Format("{0}対象が見つかりません。", this.ButtonExec.Text), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                #endregion

                var sUnitStatus = dtReEntryUnitStatus.Rows[0]["STATUS"].ToString();
                string sUserId = null;
                string sUserName = null;

                //if (Consts.UnitListType.Entry.Equals(_UnitListType))
                //{
                //    sUserId = dtReEntryUnitStatus.Rows[0]["ENTRY_USER_ID"].ToString();
                //    sUserName = dtReEntryUnitStatus.Rows[0]["ENTRY_USER_NAME"].ToString();
                //}
                //else if (Consts.Flag.ON.Equals(_sVerifyFlag))
                //{
                //    sUserId = dtReEntryUnitStatus.Rows[0]["VERIFY_ENTRY_USER_ID"].ToString();
                //    sUserName = dtReEntryUnitStatus.Rows[0]["VERIFY_ENTRY_USER_NAME"].ToString();
                //}
                //else
                //{
                //    sUserId = dtReEntryUnitStatus.Rows[0]["UPD_ENTRY_USER_ID"].ToString();
                //    sUserName = dtReEntryUnitStatus.Rows[0]["UPD_ENTRY_USER_NAME"].ToString();
                //}

                switch (_UnitListType)
                {
                    case Consts.UnitListType.Entry:
                        sUserId = dtReEntryUnitStatus.Rows[0]["ENTRY_USER_ID"].ToString();
                        sUserName = dtReEntryUnitStatus.Rows[0]["ENTRY_USER_NAME"].ToString();
                        break;
                    case Consts.UnitListType.Modify:
                        sUserId = dtReEntryUnitStatus.Rows[0]["UPD_ENTRY_USER_ID"].ToString();
                        sUserName = dtReEntryUnitStatus.Rows[0]["UPD_ENTRY_USER_NAME"].ToString();
                        break;
                    case Consts.UnitListType.Verify:
                        sUserId = dtReEntryUnitStatus.Rows[0]["VERIFY_ENTRY_USER_ID"].ToString();
                        sUserName = dtReEntryUnitStatus.Rows[0]["VERIFY_ENTRY_USER_NAME"].ToString();
                        break;
                }

                var sList = new List<string>();
                if (Consts.UnitListType.Entry.Equals(_UnitListType))
                {
                    // エントリ
                    sList.Add(Consts.EntryUnitStatus.ENTRY_NOT);
                    sList.Add(Consts.EntryUnitStatus.ENTRY_EDT);
                }
                else
                {
                    // 修正・検証
                    sList.Add(Consts.EntryUnitStatus.COMPARE_EDT);  // コンペア修正中
                    sList.Add(Consts.EntryUnitStatus.COMPARE_END);  // コンペア修正済
                    sList.Add(Consts.EntryUnitStatus.EXPORT_END);   // テキスト出力済
                }

                if (!Consts.UnitListType.Verify.Equals(_UnitListType))
                {
                    if (!sList.Contains(sUnitStatus)
                        || (!sUserId.Equals(Program.LoginUser.USER_ID) && !sUserId.Equals(string.Empty)))
                    {
                        dao.RollbackTrans();
                        // 管理者修正中 or コンペア済みのデータ以外は表示不可
                        MessageBox.Show($"既に「{sUserName}」が{this.ButtonExec.Text}しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        // 再検索
                        this.Search();
                        return;
                    }
                }
                else
                {
                    if (Consts.Flag.ON.Equals(dtReEntryUnitStatus.Rows[0]["VERIFY_ING_FLAG"].ToString())
                        && !sUserId.Equals(Program.LoginUser.USER_ID)
                        && !Consts.EntryUnitStatus.EXPORT_END.Equals(sUnitStatus))
                    {
                        dao.RollbackTrans();
                        // 管理者修正中 or コンペア済みのデータ以外は表示不可
                        MessageBox.Show($"「{sUserName}」が{this.ButtonExec.Text}中です。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        // 再検索
                        this.Search();
                        return;
                    }
                }

                // 修正確認（修正再開（エントリ修正ユーザIDがnull以外）の場合は確認しない）
                if (sUserId.Equals(string.Empty))
                {
                    if (MessageBox.Show($"エントリバッチID:{Utils.EditEntryBatchId(this.SelectedReEntryList.ENTRY_UNIT_ID)}\nを{this.ButtonExec.Text}しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != System.Windows.Forms.DialogResult.Yes)
                    {
                        dao.RollbackTrans();
                        return;
                    }
                }

                // エントリデータ修正ユーザIDを更新
                int UpdateCount = 0;
                if (Consts.UnitListType.Entry.Equals(_UnitListType))
                {
                    // エントリ
                    // 入力単位データをインスタンス化します。
                    var assingEntryUnit = new D_ENTRY_UNIT(this.SelectedReEntryList.ENTRY_UNIT_ID)
                    {
                        UPD_USER_ID = Program.LoginUser.USER_ID,
                    };

                    // アサインされた入力単位をインスタンス化します。
                    var assingEntryStatus = new D_ENTRY_STATUS(this.SelectedReEntryList.ENTRY_UNIT_ID, this.SelectedReEntryList.RECORD_KBN)
                    {
                        ENTRY_USER_ID = Program.LoginUser.USER_ID,
                        UPD_USER_ID = Program.LoginUser.USER_ID,
                    };

                    if (dao.UPDATE_D_ENTRY_STATUS(assingEntryStatus) != 1 && !sUserId.Equals(Program.LoginUser.USER_ID))
                    {
                        // ステータス更新対象が存在しなかったら誰かが更新済！！
                        dao.RollbackTrans();
                        // 管理者修正中 or コンペア済みのデータ以外は表示不可
                        MessageBox.Show($"「{sUserName}」が{this.ButtonExec.Text}中です。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        // 再検索
                        this.Search();
                        return;
                    }

                    UpdateCount = dao.UPDATE_D_ENTRY_UNIT(assingEntryUnit);
                }
                else
                {
                    // 修正・検証
                    UpdateCount = dao.UPDATE_D_ENTRY_UNIT_VERIFY(this.SelectedReEntryList.ENTRY_UNIT_ID, Consts.UnitListType.Modify.Equals(_UnitListType) ? Consts.Flag.OFF : Consts.Flag.ON);
                }
                Console.WriteLine("{0}件更新しました。", UpdateCount.ToString("#,0"));
                dao.CommitTrans();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                dao.RollbackTrans();
                if (ex.Message.Contains("ORA-00054"))
                {
                    MessageBox.Show($"他ユーザが{ this.ButtonExec.Text}対象として選択しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    // 再検索
                    this.Search();
                }
                else
                {
                    throw;
                    //                    MessageBox.Show("例外が発生しました。" + "\n" +ex.Message.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                return;
            }
            finally
            {
                if (dao != null)
                {
                    dao.Close();
                }
            }

            #region エントリ画面表示
            try
            {
                using (var frm = new FrmEntry(this.SelectedReEntryList.ENTRY_UNIT_ID    // ユニットID
                                             , this.SelectedReEntryList.RECORD_KBN      // レコード区分
                                             , Consts.UnitListType.Verify.Equals(_UnitListType) ? Consts.Flag.ON : Consts.Flag.OFF   // 検証フラグ
                                             , Consts.Flag.OFF))
                {
                    this.Hide();
                    frm.ShowDialog(this);
                    this.Show();
                    this.Activate();
                }

                // 再検索
                this.Search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
            }
            #endregion
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            // 再検索
            this.Search();
        }

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // 再検索
            this.Search();
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            this.dateTimePicker1.Enabled = this.checkBox2.Checked;
            // 再検索
            this.Search();
        }

        private void DropDownListGyoumKbn_SelectedValueChanged(object sender, EventArgs e)
        {
            // 再検索
            this.Search();
        }

        private void lvReEntryList_DoubleClick(object sender, EventArgs e)
        {
            this.ButtonExec.PerformClick();
        }
    }
}
