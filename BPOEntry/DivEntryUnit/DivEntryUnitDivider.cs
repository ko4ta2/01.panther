using System;
using System.Collections.Generic;
using System.Data;
using BPOEntry.Tables;
using Common;
using NLog;

namespace BPOEntry.DivideEntryUnit
{
    public class DivideEntryUnitDivider
    {
        /// <summary>
        /// log
        /// </summary>
        private static Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// データアクセス
        /// </summary>
        private static DaoDivideEntryUnit _dao = new DaoDivideEntryUnit();

        /// <summary>
        /// 帳票マスタ
        /// </summary>
        private static DataTable dtM_DOC = null;

        /// <summary>
        /// 帳票別分割数
        /// </summary>
        public Dictionary<string, int> DivNumbers { get; protected set; }

        /// <summary>
        /// エントリ単位分割処理
        /// </summary>
        /// <param name="sImageCaptureDate"></param>
        /// <param name="sImageCaptureNum"></param>
        public int DivideEntryunit(string imageCaptureDate, string imageCaptureNum, string docId, int captureCount)
        {
            _log.Info("DivideEntryUnit:start");

            // 指定の日付、回数の中で既に分割済みのファイル一覧を取得します。
            try
            {
                int iCount = 0;

                //_dao.Open(Config.DSN);
                _dao.BeginTrans();

                // 各帳票のアイテム数を取得します。
                var M_DOC = new Dictionary<string, short>();
                dtM_DOC = _dao.SELECT_M_DOC(docId);
                foreach (DataRow row in dtM_DOC.Rows)
                {
                    string id = (string)row["DOC_ID"];
                    if (M_DOC.ContainsKey(id))
                        M_DOC[id] = (short)row["ENTRY_ITEMS_NUM"];
                    else
                        M_DOC.Add(id, (short)row["ENTRY_ITEMS_NUM"]);
                }

                this.DivNumbers = new Dictionary<string, int>();
                foreach (DataRow row in dtM_DOC.Rows)
                {
                    this.DivNumbers.Add(row["DOC_ID"].ToString(), int.Parse(row["ENTRY_UNIT_NUM"].ToString()));
                }
                int cntImg = 0;
                int iEntryUnit = 0;
                int iImageSeq = 0;
                DataRow[] drM_DOC = dtM_DOC.Select(String.Format("DOC_ID='{0}'", docId));
                int unitNum = this.DivNumbers[docId];

                var EntryUnitId = string.Empty;

                for (int iIdx = 1; iIdx <= captureCount; iIdx++)
                {
                    // 初回と分割単位数毎にグループを変更します。
                    if (cntImg % unitNum == 0)
                    {
                        iEntryUnit++;
                        iImageSeq = 0;

                        // ENTRY_UNIT_ID 編集
                        EntryUnitId = String.Join("_", Config.TokuisakiCode, Config.HinmeiCode, imageCaptureDate, imageCaptureNum, docId, iEntryUnit.ToString("d3"));

                        // 入力単位データを登録します。
                        var recEntryUnit = new D_ENTRY_UNIT(EntryUnitId, string.Empty)
                        {
                            //ENTRY_UNIT_ID=EntryUnitId,
                            INS_USER_ID = Program.LoginUser.USER_ID
                        };
                        _dao.INSERT_D_ENTRY_UNIT(recEntryUnit);

                        // 入力状態データをユーザー区分１と２でそれぞれ登録します。
                        var recEntryStatus = new D_ENTRY_STATUS(EntryUnitId, Consts.RecordKbn.Entry_1st)
                        {
                            INS_USER_ID = Program.LoginUser.USER_ID
                        };
                        _dao.INSERT_D_ENTRY_STATUS(recEntryStatus);

                        recEntryStatus = new D_ENTRY_STATUS(EntryUnitId, Consts.RecordKbn.Entry_2nd)
                        {
                            INS_USER_ID = Program.LoginUser.USER_ID
                        };
                        _dao.INSERT_D_ENTRY_STATUS(recEntryStatus);
                    }

                    cntImg++;
                    iImageSeq++;
                    var recImgInfo = new D_IMAGE_INFO(EntryUnitId, iImageSeq)
                    {
                        IMAGE_PATH = String.Join("_", Config.TokuisakiCode, Config.HinmeiCode, imageCaptureDate, imageCaptureNum, iEntryUnit.ToString("d3"), iImageSeq.ToString())
                        ,
                        INS_USER_ID = Program.LoginUser.USER_ID
                    };
                    _dao.INSERT_D_IMAGE_INFO(recImgInfo);
                    iCount++;

                    // 入力項目数分の入力データを登録します。
                    for (int i = 1; i <= M_DOC[docId]; i++)
                    {
                        var entry1 = new D_ENTRY(EntryUnitId, iImageSeq, Consts.RecordKbn.Entry_1st, string.Format("ITEM_{0:000}", i))
                        {
                            INS_USER_ID = Program.LoginUser.USER_ID
                        };
                        _dao.INSERT_D_ENTRY(entry1);

                        var entry2 = new D_ENTRY(EntryUnitId, iImageSeq, Consts.RecordKbn.Entry_2nd, string.Format("ITEM_{0:000}", i))
                        {
                            INS_USER_ID = Program.LoginUser.USER_ID
                        };
                        _dao.INSERT_D_ENTRY(entry2);
                    }
                }

                // エントリ単位分割後処理
                _log.Info($"エントリ単位分割後処理：{Config.ExecAfterImportFlag}" );
                if (Consts.Flag.ON.Equals(Config.ExecAfterImportFlag))
                    AfterImport.AfterImport.BL_Main(imageCaptureDate, imageCaptureNum);

                // DBコミット
                _dao.CommitTrans();
                return iCount;
                //return;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                _dao.RollbackTrans();
                throw;
            }
            finally
            {
                //_dao.Close();
                _log.Info("DivideEntryUnit:end");
            }
        }
    }
}
