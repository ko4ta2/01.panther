using BPOEntry.DeleteEntryUnit;
using BPOEntry.DivideEntryUnit;
using BPOEntry.DivideEntryUnitImage;
using BPOEntry.EntryForms;
using BPOEntry.ExportEntry;
using BPOEntry.Progress;
using BPOEntry.ReEntrySelect;
using BPOEntry.SelectGyomu;
using BPOEntry.UserManage;
using Common;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace BPOEntry.Menu
{
    /// <summary>
    /// メインメニュー画面
    /// </summary>
    public partial class FrmMenu : Form
    {
        #region CreateParams
        /// <summary>
        /// ショートカットキーなどでフォームが閉じられないようにします。
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_NOCLOSE = 0x200;
                CreateParams createParams = base.CreateParams;
                createParams.ClassStyle |= CS_NOCLOSE;
                return createParams;
            }
        }
        #endregion

        /// <summary>
        /// log
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        private static DaoMenu dao = new DaoMenu();

        private static Dao.DaoEntry _dao = new Dao.DaoEntry();

        //private const int iMaxRetryCount = 512;

        private static List<Button> _btns = null;

        public FrmMenu()
        {
            InitializeComponent();

            Initialize();

            // ログイン情報
            log.Info("ログインユーザID:{0} ログインユーザ権限:{1}", Program.LoginUser.USER_ID, Program.LoginUser.USER_KBN);
        }

        /// <summary>
        /// 初期処理
        /// </summary>
        private void Initialize()
        {
            EditTitle();

            CreateMenu();
        }

        /// <summary>
        /// タイトルバー編集
        /// </summary>
        private void EditTitle()
        {
            this.Text = Utils.GetFormText();
            this.label1.Text = String.Format("Ver.{0}", Utils.GetVersion());
            this.lblTitle.BackColor = Config.LblTitleBackColor;

            this.lblTitle.Text = Utils.GetFormText();
            if (this.lblTitle.Text.Length >= 24)
                this.lblTitle.Font = new System.Drawing.Font("Meiryo UI", 14f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            else if (this.lblTitle.Text.Length >= 20)
                this.lblTitle.Font = new System.Drawing.Font("Meiryo UI", 16f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            else if (this.lblTitle.Text.Length >= 16)
                this.lblTitle.Font = new System.Drawing.Font("Meiryo UI", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            else
                this.lblTitle.Font = new System.Drawing.Font("Meiryo UI", 20f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
        }

        #region 閉じる
        /// <summary>
        /// 閉じる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClose_Click(object sender, EventArgs e)
        {
            DisposeMenu();
        }
        #endregion

        /// <summary>
        /// ユニット一覧表示
        /// </summary>
        /// <param name="UnitListType"></param>
        private void ShowEntryUnitList(string UnitListType)
        {
            var dt = new DataTable();
            var varb = string.Empty;
            DataRow[] row = null;
            if (Consts.UnitListType.Entry.Equals(UnitListType))
            {
                dt = dao.SELECT_D_ENTRY_UNIT_STATUS(Program.LoginUser.USER_ID);
                varb = "エントリ";
            }
            if (Consts.UnitListType.Modify.Equals(UnitListType))
            {
                dt = dao.SELECT_D_ENTRY_UNIT(Program.LoginUser.USER_ID);
                varb = "修正";
            }

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show($"{varb}対象ユニットがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {

                if (Consts.UnitListType.Entry.Equals(UnitListType))
                {
                    // ログインユーザが入力中
                    row = dt.AsEnumerable().Where(rows => rows["ENTRY_USER_ID"].ToString().Equals(Program.LoginUser.USER_ID)
                                           && rows["ENTRY_STATUS"].ToString().Equals(Consts.EntryStatus.ENTRY_ING)).ToArray();
                }
                else
                {
                    // ログインユーザが修正中
                    row = dt.AsEnumerable().Where(rows => rows["UPD_ENTRY_USER_ID"].ToString().Equals(Program.LoginUser.USER_ID)
                                           && rows["STATUS"].ToString().Equals(Consts.EntryUnitStatus.COMPARE_EDT)).ToArray();
                }

                if (row.Count() > 0)
                {
                    // 入力中、修正中のユニットがあれば優先的に表示
                    using (var fm = new FrmEntry(row[0]["ENTRY_UNIT_ID"].ToString(), row[0]["RECORD_KBN"].ToString(), Consts.Flag.OFF))
                    {
                        this.Hide();
                        fm.ShowDialog(this);
                        this.Show();
                        this.Activate();
                    }
                }
                else
                {
                    // リストから選択させる                
                    using (var fm = new FrmEntryUnitList(UnitListType))
                    {
                        this.Hide();
                        fm.ShowDialog(this);
                        this.Show();
                        this.Activate();
                    }
                }
            }
        }

        /// <summary>
        /// メニュー作成
        /// </summary>
        private void CreateMenu()
        {
            _btns = new List<Button>();
            var btn = new Button();
            var BusinessCount = 0;
            if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
                BusinessCount = _dao.SELECT_M_BUSINESS().Rows.Count;
            if (Consts.RecordKbn.ADMIN.Equals(Program.LoginUser.USER_KBN))
            {
                gbAdmin.Text = "管理者メニュー";

                if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG)
                    && BusinessCount > 1)
                {
                    btn = new Button();
                    btn.Text = "業務選択";
                    btn.Tag = "SelectGyomu";
                    _btns.Add(btn);
                }

                if (dao.IsExistsUICreateUnit())
                {
                    if (dao.IsExistsTergetEntryMethod(Consts.EntryMethod.IMAGE))
                    {
                        btn = new Button();
                        btn.Text = "エントリ単位分割（イメージ）";
                        btn.Tag = "DivImage";
                        //btn.BackColor = System.Drawing.SystemColors.Control;
                        _btns.Add(btn);
                    }

                    if (dao.IsExistsTergetEntryMethod(Consts.EntryMethod.PAPER))
                    {
                        btn = new Button();
                        btn.Text = "エントリ単位分割（現物）";
                        btn.Tag = "DivActual";
                        //btn.BackColor = System.Drawing.SystemColors.Control;
                        _btns.Add(btn);
                    }
                }
                btn = new Button();
                btn.Text = "エントリ";
                btn.Tag = "Entry";
                _btns.Add(btn);

                if (dao.IsExistsPrelogicalCheckTarget())
                {
                    btn = new Button();
                    btn.Text = "OCR取り込み結果不備検証";
                    btn.Tag = "VerifyNg";
                    _btns.Add(btn);
                }

                btn = new Button();
                btn.Text = "エントリ修正";
                btn.Tag = "Modify";
                _btns.Add(btn);

                btn = new Button();
                btn.Text = "エントリ検証";
                btn.Tag = "Verify";
                _btns.Add(btn);

                if (dao.IsExistsUIExport())
                {
                    btn = new Button();
                    btn.Text = "エントリ結果出力";
                    btn.Tag = "Export";
                    _btns.Add(btn);
                }

                btn = new Button();
                btn.Text = "エントリ状況確認";
                btn.Tag = "ShowProgressList";
                _btns.Add(btn);

                btn = new Button();
                btn.Text = "ユーザ管理";
                btn.Tag = "ShowUserList";
                _btns.Add(btn);

                if ("sysadmin".Equals(Program.LoginUser.USER_ID))
                {
                    btn = new Button();
                    btn.Text = "エントリデータ削除";
                    btn.Tag = "DeleteEntryData";
                    _btns.Add(btn);
                }
            }
            else
            {
                gbAdmin.Text = "担当者メニュー";

                if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG)
                    && BusinessCount > 1)
                {
                    btn = new Button();
                    btn.Text = "業務選択";
                    btn.Tag = "SelectGyomu";
                    _btns.Add(btn);
                }

                btn = new Button();
                btn.Text = "エントリ";
                btn.Tag = "Entry";
                _btns.Add(btn);

                if (dao.IsExistsPrelogicalCheckTarget())
                {
                    btn = new Button();
                    btn.Text = "OCR取り込み結果不備検証";
                    btn.Tag = "VerifyNg";
                    _btns.Add(btn);
                }

                btn = new Button();
                btn.Text = "エントリ修正";
                btn.Tag = "Modify";
                _btns.Add(btn);

                btn = new Button();
                btn.Text = "エントリ検証";
                btn.Tag = "Verify";
                _btns.Add(btn);

                btn = new Button();
                btn.Text = "エントリ状況確認";
                btn.Tag = "ShowProgressList";
                _btns.Add(btn);
            }

            var iY = 35;
            foreach (var btn2 in _btns)
            {
                btn2.Location = new System.Drawing.Point(15, iY);
                btn2.Font = new System.Drawing.Font("Meiryo UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
                btn2.Size = new System.Drawing.Size(this.ButtonClose.Width, 50);
                //btn2.BackColor = System.Drawing.SystemColors.Control;

                iY = iY + 60;
                btn2.Click += new EventHandler(this.ButtonActionClick);
                this.gbAdmin.Controls.Add(btn2);
            }

            gbAdmin.Height = _btns.Count * 60 + 40;
            ButtonClose.Top = gbAdmin.Top + gbAdmin.Height + 10;
            this.Height = ButtonClose.Top + ButtonClose.Height + 45;
        }

        /// <summary>
        /// メニュー廃棄
        /// </summary>
        private void DisposeMenu()
        {
            _btns.ForEach(btn => btn.Click -= new EventHandler(this.ButtonActionClick));
            this.Close();
            this.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonActionClick(object sender, EventArgs e)
        {
            var Verb = ((Button)sender).Tag.ToString();
            log.Info("verb:{0}", Verb);

            switch (Verb)
            {
                case "SelectGyomu":
                    // 業務選択
                    using (var fm = new FrmSelectGyomu())
                    {
                        this.Hide();
                        fm.ShowDialog();
                        EditTitle();
                        this.Show();
                        this.Activate();
                    }
                    break;
                case "DivImage":
                    // エントリ単位分割（イメージ）
                    using (var fm = new frmDivideEntryUnitImage())
                    {
                        this.Hide();
                        fm.ShowDialog();
                        this.Show();
                        this.Activate();
                    }
                    break;
                case "DivActual":
                    // エントリ単位分割（現物）
                    using (var fm = new frmDivideEntryUnit())
                    {
                        this.Hide();
                        fm.ShowDialog();
                        this.Show();
                        this.Activate();
                    }
                    break;
                case "Entry":
                    // エントリ
                    ShowEntryUnitList(Consts.UnitListType.Entry);
                    break;
                case "VerifyNg":
                    // OCR取り込み結果不備検証
                    using (var fm = new FrmEntryUnitList(Consts.UnitListType.Verify))
                    {
                        this.Hide();
                        fm.ShowDialog();
                        this.Show();
                        this.Activate();
                    }
                    break;
                case "Modify":
                    // エントリ修正
                    //using (var fm = new FrmEntryUnitList(Consts.UnitListType.Modify))
                    //{
                    //    this.Hide();
                    //    fm.ShowDialog();
                    //    this.Show();
                    //    this.Activate();
                    //}
                    ShowEntryUnitList(Consts.UnitListType.Modify);
                    break;
                case "Verify":
                    // エントリ検証
                    using (var fm = new FrmEntryUnitList(Consts.UnitListType.Verify))
                    {
                        this.Hide();
                        fm.ShowDialog();
                        this.Show();
                        this.Activate();
                    }
                    break;
                case "Export":
                    using (var fm = new FrmExportEntryUnit())
                    {
                        this.Hide();
                        fm.ShowDialog();
                        this.Show();
                        this.Activate();
                    }
                    break;
                case "ShowProgressList":
                    // エントリ条件確認
                    using (var fm = new FrmProgressList())
                    {
                        this.Hide();
                        fm.ShowDialog();
                        this.Show();
                        this.Activate();
                    }
                    break;
                case "ShowUserList":
                    // ユーザ管理
                    using (var fm = new FrmUserList())
                    {
                        this.Hide();
                        fm.ShowDialog();
                        this.Show();
                        this.Activate();
                    }
                    break;
                case "DeleteEntryData":
                    // エントリデータ削除
                    using (var fm = new FrmDeleteEntryUnit())
                    {
                        this.Hide();
                        fm.ShowDialog();
                        this.Show();
                        this.Activate();
                    }
                    break;
            }
        }
    }
}
