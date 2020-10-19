using System;
using System.Collections.Generic;
using System.IO;
using Common;
using NLog;

namespace BPOEntry.DivideEntryUnitImage
{
    public class DivideEntryUnitImageDivider
    {
        /// <summary>
        /// log
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// データアクセス
        /// </summary>
        private static DaoDivideEntryUnitImage dao = new DaoDivideEntryUnitImage();

        /// <summary>
        /// 帳票マスタ
        /// </summary>
        //private static DataTable dtM_DOC = _Dao.SelectEntryItemNum();

        /// <summary>
        /// 帳票別分割数
        /// </summary>
        //public Dictionary<string, int> DivNumbers { get; protected set; }

        /// <summary>
        /// エントリ単位分割処理
        /// </summary>
        /// <param name="sImageCaptureDate"></param>
        /// <param name="sImageCaptureNum"></param>
        public int DivideEntryunitImage(string sImageCaptureDate, string sImageCaptureNum)
        {
            log.Info("DivideEntryUnitImage:start");

            try
            {
                dao.Open(Config.DSN);
                dao.BeginTrans();

                // エントリ単位分割呼出し
                //DivEntryUnit.DivEntryUnit deu = new DivEntryUnit.DivEntryUnit();
                int iCount = DllCreateEntryUnit.DllCreateEntryUnit.BL_Main(Config.UserId,Config.TokuisakiCode,Config.HinmeiCode,sImageCaptureDate, sImageCaptureNum, Program.LoginUser.USER_ID, "1");

                log.Info("エントリ単位分割件数：{0}", iCount.ToString("#,0件"));
                //// 各帳票のアイテム数を取得します。
                //var M_DOC = new Dictionary<string, short>();
                //foreach (DataRow row in dtM_DOC.Rows)
                //{
                //    string id = (string)row["DOC_ID"];
                //    if (M_DOC.ContainsKey(id))
                //        M_DOC[id] = (short)row["ENTRY_ITEMS_NUM"];
                //    else
                //        M_DOC.Add(id, (short)row["ENTRY_ITEMS_NUM"]);
                //}

                //this.DivNumbers = new Dictionary<string, int>();
                //foreach (DataRow row in dtM_DOC.Rows)
                //{
                //    this.DivNumbers.Add(row["DOC_ID"].ToString(), int.Parse(row["ENTRY_UNIT_NUM"].ToString()));
                //}

                //// 分割済みの画像ファイルを取得します。
                //var table = _Dao.SelectDividedImages(sImageCaptureDate, sImageCaptureNum);
                //var imageFileNames = from DataRow row in table.Rows
                //                     select row["IMAGE_PATH"];

                //// ファイルパスの一覧をFileInfoにコンバートします。
                //var dividedFiles = new List<FileInfo>();
                //imageFileNames.ToList().ForEach(o => dividedFiles.Add(new FileInfo((string)o)));

                //// 指定のフォルダ内の全てのファイルを取得します。
                //var targetPath = Path.Combine(Config.DivImageRoot, sImageCaptureDate + sImageCaptureNum);
                //var targetDirectory = new DirectoryInfo(targetPath);
                //var targetFileInfos = targetDirectory.GetFiles("*.jp*", SearchOption.AllDirectories).ToList();

                //#region ダミーイメージ対応
                //bool bCopuFlag = false;
                //foreach (var info in targetFileInfos)
                //{
                //    string sDocId = info.DirectoryName.ToString().Substring(info.DirectoryName.ToString().Length - 9, 9);
                //    DataRow[] drM_DOC = dtM_DOC.Select(String.Format("DOC_ID='{0}'", sDocId.Replace(@"\", string.Empty)));
                //    if (drM_DOC[0]["DUMMY_IMAGE_FOLDER_NAME"].ToString().Length != 0
                //        && drM_DOC[0]["MIN_ENTRY_UNIT_NUM"].ToString().Length != 0)
                //    {
                //        var targetFileInfosSub = new DirectoryInfo(Path.Combine(Config.DivImageRoot, sImageCaptureDate + sImageCaptureNum, sDocId)).GetFiles("*.jp*", SearchOption.AllDirectories).ToList();

                //        int iMinEntryUnitNum = int.Parse(drM_DOC[0]["MIN_ENTRY_UNIT_NUM"].ToString());
                //        if (iMinEntryUnitNum > targetFileInfosSub.Count)
                //        {
                //            int iDiff = iMinEntryUnitNum - targetFileInfosSub.Count;
                //            var targetFileInfosSub2 = new DirectoryInfo(drM_DOC[0]["DUMMY_IMAGE_FOLDER_NAME"].ToString()).GetFiles("*.jp*", SearchOption.TopDirectoryOnly).ToList();

                //            // ダミーファイル取込み
                //            for (int iIdx = 0; iIdx < iDiff; iIdx++)
                //            {
                //                bCopuFlag = true;
                //                File.Copy(targetFileInfosSub2[iIdx].FullName, targetPath + @"\" + sDocId + @"\" + targetFileInfosSub2[iIdx].Name);
                //            }
                //        }
                //    }
                //}

                //// 再取得
                //if (bCopuFlag)
                //    targetFileInfos = targetDirectory.GetFiles("*.jp*", SearchOption.AllDirectories).ToList();
                //#endregion

                //// 既に分割済みのファイルを除きます。
                //foreach (var div in dividedFiles)
                //{
                //    var fi = targetFileInfos.Where(o => o.FullName == div.FullName).FirstOrDefault();
                //    if (fi != null)
                //        targetFileInfos.Remove(fi);
                //}

                //// 帳票ID毎にグルーピング(/書類ID/0001/*.jpg)
                //var docGrp = new Dictionary<string, List<FileInfo>>();
                //foreach (var info in targetFileInfos)
                //{
                //    string id = info.Directory.Parent.Name + info.Directory.Name;

                //    // 帳票IDが帳票マスタに含まれていなければ対象外
                //    if (!M_DOC.ContainsKey(id))
                //        continue;

                //    if (docGrp.ContainsKey(id))
                //        docGrp[id].Add(info);
                //    else
                //        docGrp.Add(id, new List<FileInfo>() { info });
                //}

                //// 分割対象となるファイルが無ければ中断します。
                //if (docGrp.Count == 0)
                //{
                //    throw new FileNotFoundException();
                //}
                //// 帳票ID毎に分割します
                //foreach (var doc in docGrp)
                //{
                //    int cntImg = 0;
                //    int iEntryUnit = 0;
                //    int iImageSeq = 0;
                //    string sDocId = doc.Key;
                //    DataRow[] drM_DOC = dtM_DOC.Select(String.Format("DOC_ID='{0}'", sDocId));
                //    // OCR連携フラグ
                //    string sOcrCooperationFlag = drM_DOC[0]["OCR_COOPERATION_FLAG"].ToString();

                //    int unitNum = this.DivNumbers[sDocId];

                //    // データを作成します。
                //    foreach (var imgInfo in doc.Value)
                //    {
                //        // 初回と分割単位数毎にグループを変更します。
                //        if (cntImg % unitNum == 0)
                //        {
                //            iEntryUnit++;
                //            iImageSeq = 0;

                //            // 入力単位データを登録します。
                //            var recEntryUnit = new D_ENTRY_UNIT(sImageCaptureDate, sImageCaptureNum, sDocId, iEntryUnit.ToString("d3"), string.Empty)
                //            {
                //                INS_USER_ID = Program.LoginUser.USER_ID
                //            };
                //            _Dao.InsertD_ENTRY_UNIT(recEntryUnit);

                //            // 入力状態データをユーザー区分１と２でそれぞれ登録します。
                //            var recEntryStatus = new D_ENTRY_STATUS(sImageCaptureDate, sImageCaptureNum, sDocId, iEntryUnit.ToString("d3"), Consts.RecordKbn.Entry_1st)
                //            {
                //                INS_USER_ID = Program.LoginUser.USER_ID
                //            };
                //            _Dao.InsertD_ENTRY_STATUS(recEntryStatus);

                //            recEntryStatus = new D_ENTRY_STATUS(sImageCaptureDate, sImageCaptureNum, sDocId, iEntryUnit.ToString("d3"), Consts.RecordKbn.Entry_2nd)
                //            {
                //                INS_USER_ID = Program.LoginUser.USER_ID
                //            };
                //            _Dao.InsertD_ENTRY_STATUS(recEntryStatus);
                //        }

                //        cntImg++;
                //        iImageSeq++;

                //        #region  画像情報データ
                //        // OCR連携画像ファイル名
                //        string sOcrImageFile = string.Empty;
                //        if (Consts.Flag.ON.Equals(sOcrCooperationFlag))
                //        {
                //            sOcrImageFile = String.Format("OCR_{0}_{1}_{2}_{3}_{4}", Config.TokuisakiCode, Config.HinmeiCode, sDocId.Substring(0, 4), sDocId.Substring(4, 4), Path.GetFileName(imgInfo.FullName));
                //        }

                //        // ダミーイメージフラグ
                //        string sDummyImageFlag = Consts.Flag.OFF;
                //        drM_DOC = dtM_DOC.Select(String.Format("DOC_ID='{0}'", sDocId));
                //        if (drM_DOC[0]["DUMMY_IMAGE_FOLDER_NAME"].ToString().Length != 0
                //            && drM_DOC[0]["MIN_ENTRY_UNIT_NUM"].ToString().Length != 0)
                //        {
                //            if (File.Exists(Path.Combine(drM_DOC[0]["DUMMY_IMAGE_FOLDER_NAME"].ToString(), imgInfo.Name)))
                //            {
                //                sDummyImageFlag = Consts.Flag.ON;
                //            }
                //        }

                //        var recImgInfo = new D_IMAGE_INFO(sImageCaptureDate, sImageCaptureNum, sDocId, iEntryUnit.ToString("d3"), iImageSeq)
                //        {
                //            IMAGE_PATH = imgInfo.FullName
                //            ,
                //            OCR_IMAGE_FILE_NAME = sOcrImageFile
                //            ,
                //            DUMMY_IMAGE_FLAG = sDummyImageFlag
                //            ,
                //            INS_USER_ID = Program.LoginUser.USER_ID
                //        };
                //        _Dao.InsertD_IMAGE_INFO(recImgInfo);
                //        iCount++;
                //        #endregion

                //        #region エントリ
                //        // 入力項目数分の入力データを登録します。
                //        for (int i = 1; i <= M_DOC[sDocId]; i++)
                //        {
                //            var entry1 = new D_ENTRY(Config.TokuisakiCode, Config.HinmeiCode, sImageCaptureDate, sImageCaptureNum, sDocId, iEntryUnit.ToString("d3"), iImageSeq, Consts.RecordKbn.Entry_1st, string.Format("ITEM_{0:000}", i))
                //            {
                //                INS_USER_ID = Program.LoginUser.USER_ID
                //            };
                //            _Dao.InsertD_ENTRY(entry1);

                //            var entry2 = new D_ENTRY(Config.TokuisakiCode, Config.HinmeiCode, sImageCaptureDate, sImageCaptureNum, sDocId, iEntryUnit.ToString("d3"), iImageSeq, Consts.RecordKbn.Entry_2nd, string.Format("ITEM_{0:000}", i))
                //            {
                //                INS_USER_ID = Program.LoginUser.USER_ID
                //            };
                //            _Dao.InsertD_ENTRY(entry2);
                //        }
                //        #endregion
                //    }

                //    // OCR連携
                //    _Log.Info("帳票ID:{0}",sDocId);
                //    if (Consts.Flag.ON.Equals(sOcrCooperationFlag))
                //    {
                //       // OCR連携対象帳票
                //        var sTargetFolder = System.IO.Path.Combine(targetDirectory.FullName, sDocId.Substring(0, 4), sDocId.Substring(4, 4));
                //        _Log.Info("OCR連携帳票:{0} {1}件", sTargetFolder, CopyOcrImage(sTargetFolder).ToString("#,0"));
                //    }
                //}

                //// エントリ単位分割後処理
                //log.Info("エントリ単位分割後処理：{0}", Config.ExecAfterImport);
                //if (Consts.Flag.ON.Equals(Config.ExecAfterImport))
                //    AfterImport.AfterImport.BL_Main(sImageCaptureDate, sImageCaptureNum);

                // DBコミット
                dao.CommitTrans();
                return iCount;
                //return;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                dao.RollbackTrans();
                throw;
            }
            finally
            {
                dao.Close();
                log.Info("DivideEntryUnitImage:end");
            }
        }

        //private static int CopyOcrImage(string sTargetFolder)
        //{
        //    int iCount = 0;
        //    IEnumerable<string> lstFilePath =
        //        System.IO.Directory.EnumerateFiles(sTargetFolder, "*.jp*", System.IO.SearchOption.TopDirectoryOnly);

        //    string sDstFolder = Config.OcrImageRoot;
        //    string[] sTemp = sTargetFolder.Split('\\');
        //    string sDocId = sTemp[sTemp.Length - 2] + sTemp[sTemp.Length - 1];

        //    // OCRイメージ連携履歴登録
        //    _Dao.InsertD_OCR_COOPERATION_HISTORY(sDocId, sTargetFolder, string.Empty, string.Empty);

        //    //ファイルを列挙する
        //    System.IO.FileInfo fi = null;
        //    string sDstFileParh = null;
        //    foreach (string sSrcFilePath in lstFilePath)
        //    {
        //        fi = new System.IO.FileInfo(sSrcFilePath);

        //        // 元のファイル名のままコピー
        //        sDstFileParh = Path.Combine(sDstFolder, Path.GetFileName(sSrcFilePath));
        //        fi.CopyTo(sDstFileParh);

        //        // リネーム
        //        fi = new System.IO.FileInfo(sDstFileParh);
        //        sDstFileParh = Path.Combine(sDstFolder, String.Format("OCR_{0}_{1}_{2}_{3}_{4}"
        //                                                             , Config.TokuisakiCode
        //                                                             , Config.HinmeiCode
        //                                                             , sDocId.Substring(0, 4)
        //                                                             , sDocId.Substring(4, 4)
        //                                                             , Path.GetFileName(sSrcFilePath)));
        //        fi.MoveTo(sDstFileParh);
        //        // OCRイメージ連携履歴登録
        //        _Dao.InsertD_OCR_COOPERATION_HISTORY(sDocId, string.Empty, Path.GetFileName(sSrcFilePath), Path.GetFileName(sDstFileParh));
        //        iCount++;
        //    }
        //    return iCount;
        //}
    }
}
