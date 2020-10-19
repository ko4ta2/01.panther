using BPOEntry.Tables;
using Common;
using Dao;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;

namespace DllCreateEntryUnit
{
    public class DllCreateEntryUnit
    {
        /// <summary>
        /// log
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// データアクセス
        /// </summary>
        private static DaoCreateEntryUnit dao = new DaoCreateEntryUnit();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="tkskCd"></param>
        /// <param name="hnmCd"></param>
        /// <param name="sImageCaptureDate"></param>
        /// <param name="sImageCaptureNum"></param>
        /// <param name="sUserId"></param>
        /// <param name="sFunc"></param>
        /// <param name="BATCH_CREATE_UNIT_FLAG"></param>
        /// <returns></returns>
        public static int BL_Main(string businessId, string tkskCd, string hnmCd, string sImageCaptureDate, string sImageCaptureNum, string sUserId, string sFunc, string BATCH_CREATE_UNIT_FLAG = Consts.Flag.OFF)
        {
            Config.GetConfig(ConfigurationManager.AppSettings["config"]);

            var sMessage = String.Format("連携年月日:{0} 連携回数:{1}", sImageCaptureDate, sImageCaptureNum);
            Console.WriteLine(sMessage); log.Info(sMessage);
            GVal.UserId = sUserId;

            var iCount = 0;
            var DivNumbers = new Dictionary<string, int>();
            //Dictionary<string, int>  new Dictionary(string,int);
            var OcrDocTypeList = new List<string>();

            // 各帳票のアイテム数を取得します。
            var M_DOC = new Dictionary<string, int>();
            var dtM_DOC = dao.SELECT_M_DOC(tkskCd, hnmCd, BATCH_CREATE_UNIT_FLAG);
            foreach (var row in dtM_DOC.AsEnumerable())
            {
                M_DOC.Add(row["DOC_ID"].ToString(), int.Parse(row["ENTRY_ITEMS_NUM"].ToString()));
            }

            DivNumbers = new Dictionary<string, int>();
            foreach (var row in dtM_DOC.AsEnumerable())
            {
                DivNumbers.Add(row["DOC_ID"].ToString(), int.Parse(row["ENTRY_UNIT_NUM"].ToString()));
            }

            // 分割済みの画像ファイルを取得します。
            var dtD_IMAGE_INFO = dao.SELECT_D_IMAGE_INFO(tkskCd, hnmCd, sImageCaptureDate, sImageCaptureNum);
            var imageFileNames = from DataRow row in dtD_IMAGE_INFO.Rows
                                 select row["IMAGE_PATH"];

            // ファイルパスの一覧をFileInfoにコンバートします。
            var dividedFiles = new List<FileInfo>();
            imageFileNames.ToList().ForEach(o => dividedFiles.Add(new FileInfo((string)o)));

            // 指定のフォルダ内の全てのファイルを取得します。
            var targetPath = string.Empty;
            if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
            {
                targetPath = Path.Combine(Config.DivImageRoot, businessId, sImageCaptureDate + sImageCaptureNum);
            }
            else
            {
                targetPath = Path.Combine(Config.DivImageRoot, sImageCaptureDate + sImageCaptureNum);
            }
            var targetDirectory = new DirectoryInfo(targetPath);
            var targetFileInfos = targetDirectory.GetFiles(Config.ImageFileExtension, SearchOption.AllDirectories).OrderBy(file => file.FullName).ToList();

            #region ダミーイメージ対応
            bool bCopuFlag = false;
            targetFileInfos.ForEach(info =>
            {
                var sDocId = info.DirectoryName.ToString().Substring(info.DirectoryName.ToString().Length - 9, 9);
                var drM_DOC = dtM_DOC.Select($"TKSK_CD='{tkskCd}' AND HNM_CD='{hnmCd}' AND DOC_ID='{sDocId.Replace(@"\", string.Empty)}'");
                if (drM_DOC[0]["DUMMY_IMAGE_FOLDER_NAME"].ToString().Length != 0 && drM_DOC[0]["MIN_ENTRY_UNIT_NUM"].ToString().Length != 0)
                {
                    var targetFileInfosSub = new List<FileInfo>();
                    if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
                    {
                        targetFileInfosSub = new DirectoryInfo(Path.Combine(Config.DivImageRoot, businessId, sImageCaptureDate + sImageCaptureNum, sDocId)).GetFiles(Config.ImageFileExtension, SearchOption.AllDirectories).ToList();
                    }
                    else
                    {
                        targetFileInfosSub = new DirectoryInfo(Path.Combine(Config.DivImageRoot, sImageCaptureDate + sImageCaptureNum, sDocId)).GetFiles(Config.ImageFileExtension, SearchOption.AllDirectories).ToList();
                    }

                    var MinEntryUnitNum = int.Parse(drM_DOC[0]["MIN_ENTRY_UNIT_NUM"].ToString());
                    if (MinEntryUnitNum > targetFileInfosSub.Count)
                    {
                        var iDiff = MinEntryUnitNum - targetFileInfosSub.Count;
                        var targetFileInfosSub2 = new DirectoryInfo(drM_DOC[0]["DUMMY_IMAGE_FOLDER_NAME"].ToString()).GetFiles(Config.ImageFileExtension, SearchOption.TopDirectoryOnly).ToList();

                        // ダミーファイル取込み
                        for (int iIdx = 0; iIdx < iDiff; iIdx++)
                        {
                            bCopuFlag = true;
                            File.Copy(targetFileInfosSub2[iIdx].FullName, targetPath + @"\" + sDocId + @"\" + targetFileInfosSub2[iIdx].Name);
                        }
                    }
                }
            });

            // 再取得
            if (bCopuFlag)
            {
                targetFileInfos = targetDirectory.GetFiles(Config.ImageFileExtension, SearchOption.AllDirectories).OrderBy(f => f.FullName).ToList();
            }
            #endregion

            // 既に分割済みのファイルを除きます。
            /*foreach (var div in */
            dividedFiles.ForEach(div =>
            {
                var fi = targetFileInfos.Where(fileInfo => fileInfo.FullName == div.FullName).FirstOrDefault();
                if (fi != null)
                {
                    targetFileInfos.Remove(fi);
                }
            });

            // 帳票ID毎にグルーピング(/書類ID/0001/*.jpg)
            var docGrp = new Dictionary<string, List<FileInfo>>();
            foreach (var info in targetFileInfos)
            {
                var DocId = info.Directory.Parent.Name + info.Directory.Name;

                // 帳票IDが帳票マスタに含まれていなければ対象外
                if (!M_DOC.ContainsKey(DocId))
                {
                    continue;
                }

                if (docGrp.ContainsKey(DocId))
                {
                    docGrp[DocId].Add(info);
                }
                else
                {
                    docGrp.Add(DocId, new List<FileInfo>() { info });
                }
            }

            // 分割対象となるファイルが無ければ中断します。
            if (docGrp.Count == 0)
            {
                throw new FileNotFoundException();
            }

            // 帳票ID毎に分割します
            foreach (var doc in docGrp)
            {
                var ImageCount = 0;
                var EntryUnit = 0;
                var ImageSeq = 0;
                var iDocCount = 0;
                var sDocId = doc.Key;
                var drM_DOC = dtM_DOC.Select(String.Format("DOC_ID='{0}'", sDocId));
                // OCR連携フラグ
                //var sOcrCooperationFlag = drM_DOC[0]["OCR_COOPERATION_FLAG"].ToString();

                var unitNum = DivNumbers[sDocId];

                var EntryUnitId = string.Empty;

                D_ENTRY_UNIT recEntryUnit = null;

                D_ENTRY_STATUS recEntryStatus = null;

                // データを作成します。
                foreach (var imgInfo in doc.Value)
                {
                    // 初回と分割単位数毎にグループを変更します。
                    if (ImageCount % unitNum == 0)
                    {
                        EntryUnit++;
                        ImageSeq = 0;

                        // ENTRY_UNIT_ID 編集
                        EntryUnitId = String.Join("_", tkskCd, hnmCd, sImageCaptureDate, sImageCaptureNum, sDocId, EntryUnit.ToString("d3"));

                        #region D_ENTRY_UNIT登録
                        recEntryUnit = new D_ENTRY_UNIT(EntryUnitId)
                        {
                            INS_USER_ID = GVal.UserId
                        };
                        dao.INSERT_D_ENTRY_UNIT(recEntryUnit);
                        #endregion

                        #region D_ENTRY_STATUS登録
                        // 入力状態データをユーザー区分１と２でそれぞれ登録します。
                        recEntryStatus = new D_ENTRY_STATUS(EntryUnitId, Consts.RecordKbn.Entry_1st)
                        {
                            INS_USER_ID = GVal.UserId
                        };

                        // 区分1登録
                        dao.INSERT_D_ENTRY_STATUS(recEntryStatus);

                        recEntryStatus.RECORD_KBN = Consts.RecordKbn.Entry_2nd;
                        // 区分2登録
                        dao.INSERT_D_ENTRY_STATUS(recEntryStatus);
                        #endregion
                    }

                    ImageCount++;
                    ImageSeq++;
                    iDocCount++;

                    #region D_IMAGE_INFO登録
                    // OCR連携画像ファイル名
                    var sOcrImageFile = string.Empty;
                    if (Consts.Flag.ON.Equals(drM_DOC[0]["OCR_COOPERATION_FLAG"].ToString()))
                    {
                        sOcrImageFile = String.Format("OCR_{0}_{1}_{2}_{3}_{4}_{5}_{6}"
                                                     , tkskCd
                                                     , hnmCd
                                                     , sDocId.Substring(0, 4)
                                                     , sDocId.Substring(4, 4)
                                                     , sImageCaptureDate
                                                     , sImageCaptureNum
                                                     , Path.GetFileName(imgInfo.FullName));
                    }

                    // ダミーイメージフラグ
                    var sDummyImageFlag = Consts.Flag.OFF;
                    drM_DOC = dtM_DOC.Select(String.Format("DOC_ID='{0}'", sDocId));
                    if (drM_DOC[0]["DUMMY_IMAGE_FOLDER_NAME"].ToString().Length != 0
                        && drM_DOC[0]["MIN_ENTRY_UNIT_NUM"].ToString().Length != 0)
                    {
                        if (File.Exists(Path.Combine(drM_DOC[0]["DUMMY_IMAGE_FOLDER_NAME"].ToString(), imgInfo.Name)))
                        {
                            sDummyImageFlag = Consts.Flag.ON;
                        }
                    }

                    var recImgInfo = new D_IMAGE_INFO(EntryUnitId, ImageSeq)
                    {
                        IMAGE_PATH = imgInfo.FullName
                        ,
                        OCR_IMAGE_FILE_NAME = sOcrImageFile
                        ,
                        DUMMY_IMAGE_FLAG = sDummyImageFlag
                        ,
                        INS_USER_ID = GVal.UserId
                    };
                    iCount += dao.INSERT_D_IMAGE_INFO(recImgInfo);
                    #endregion

                    #region D_ENTRY登録
                    // 入力項目数分の入力データを登録します。
                    for (int i = 1; i <= M_DOC[sDocId]; i++)
                    {
                        var entry = new D_ENTRY(EntryUnitId, ImageSeq, Consts.RecordKbn.Entry_1st, String.Format("ITEM_{0:000}", i))
                        {
                            INS_USER_ID = GVal.UserId,
                            DUMMY_ITEM_FLAG = Consts.Flag.ON.Equals(drM_DOC[0]["DUMMY_ITEM_FLAG"].ToString()) ? Consts.Flag.ON : Consts.Flag.OFF
                        };
                        dao.INSERT_D_ENTRY(entry);

                        entry = new D_ENTRY(EntryUnitId, ImageSeq, Consts.RecordKbn.Entry_2nd, String.Format("ITEM_{0:000}", i))
                        {
                            INS_USER_ID = GVal.UserId,
                            DUMMY_ITEM_FLAG = Consts.Flag.ON.Equals(drM_DOC[0]["DUMMY_ITEM_FLAG"].ToString()) ? Consts.Flag.ON : Consts.Flag.OFF
                        };
                        dao.INSERT_D_ENTRY(entry);
                    }
                    #endregion
                }

                log.Info("帳票ID:{0} 登録件数:{1}件", sDocId, iDocCount.ToString("#,0"));

                // OCR連携
                if (Consts.Flag.ON.Equals(drM_DOC[0]["OCR_COOPERATION_FLAG"].ToString()))
                {
                    // OCR連携対象帳票
                    //                    var sTargetFolder = System.IO.Path.Combine(targetDirectory.FullName, sDocId.Substring(0, 4), sDocId.Substring(4, 4));
                    OcrDocTypeList.Add(Path.Combine(targetDirectory.FullName, sDocId.Substring(0, 4), sDocId.Substring(4, 4)));
                    //                    var CopyImageCount = CopyOcrImage(tkskCd, hnmCd, sTargetFolder, drM_DOC[0]["OCR_IMAGE_COOPERATION_PATH"].ToString());
                    //                    log.Info("OCR連携帳票:{0} {1}件", sTargetFolder, CopyImageCount.ToString("#,0"));
                }
            }

            // エントリ単位分割済テーブルへ登録
            if ("2".Equals(sFunc))
            {
                if (dao.INSERT_T_DIV_ENTRY_UNIT_PATH(tkskCd, hnmCd, sImageCaptureDate + sImageCaptureNum) != 1)
                {
                    throw new ApplicationException("T_DIV_ENTRY_UNIT_PATH の登録で不整合が発生");
                }
            }

            // エントリ単位分割後処理
            log.Info($"エントリ単位分割後処理フラグ：{Config.ExecAfterImportFlag}");
            if (Consts.Flag.ON.Equals(Config.ExecAfterImportFlag))
            {
                AfterImport.AfterImport.BL_Main(sImageCaptureDate, sImageCaptureNum);
            }

            // OCR用イメージコピー
            CopyOcrImage(tkskCd, hnmCd, sImageCaptureDate, sImageCaptureNum, OcrDocTypeList);

            // 件数返却
            return iCount;
        }

        /*
                private static int CopyOcrImage(string tkskCd, string hnmCd, string sTargetFolder, string sDstPath)
                {
                    var CopyImageCount = 0;
                    IEnumerable<string> lstFilePath =
                        System.IO.Directory.EnumerateFiles(sTargetFolder, Config.ImageFileExtension, System.IO.SearchOption.TopDirectoryOnly);

                    if (sDstPath.Length == 0)
                    {
                        sDstPath = Config.OcrImageRoot;
                    }
                    var sTemp = sTargetFolder.Split('\\');
                    var sDocId = sTemp[sTemp.Length - 2] + sTemp[sTemp.Length - 1];

                    // OCRイメージ連携履歴登録
                    dao.INSERT_D_OCR_COOPERATION_HISTORY(tkskCd, hnmCd, sDocId, sTargetFolder, string.Empty, string.Empty);

                    //ファイルを列挙する
        //            System.IO.FileInfo fi = null;
                    var sDstFileParh = string.Empty;
        //            foreach (string sSrcFilePath in  
                    lstFilePath.ToList().ForEach(sSrcFilePath =>
                    {
                        // "temporary_" 付きでコピー
                        sDstFileParh = Path.Combine(sDstPath, $"temporary_OCR_{tkskCd}_{hnmCd}_{sDocId.Substring(0, 4)}_{sDocId.Substring(4, 4)}_{Path.GetFileName(sSrcFilePath)}");
                        File.Copy(sSrcFilePath, sDstFileParh);
        #if DEBUG
                        //Thread.Sleep(1000);
        #endif
                        // "temporary_" を削除
                        File.Copy(sDstFileParh, sDstFileParh.Replace("temporary_", string.Empty));

                        CopyImageCount++;
                    });
                    return CopyImageCount;
                }
        */
        private static int CopyOcrImage(string tkskCd, string hnmCd, string date, string num, List<string> OcrImageDirectoryList)
        {
            log.Info("CopyOcrImage:start");

            var FileNameList = new List<string>();

            // stopwatch start
            var sw = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                if (OcrImageDirectoryList.Count == 0)
                {
                    return 0;
                }

                var dstDirectory = Config.OcrImageRoot;
                if (Consts.BusinessID.CDC.Equals(Utils.GetBussinessId()))
                {
                    dstDirectory = Path.Combine(dstDirectory, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                    Directory.CreateDirectory(dstDirectory);
                }

                //var tkskCd = Config.TokuisakiCode;
                //var hnmCd = Config.HinmeiCode;
                OcrImageDirectoryList.ForEach(OcrImageDirectory =>
                //    foreach (var OcrImageDirectory in OcrImageDirectoryList)
                {
                    var OcrImageFileList = Directory.GetFiles(OcrImageDirectory, Config.ImageFileExtension, SearchOption.TopDirectoryOnly);

                    var sTemp = OcrImageDirectory.Split('\\');
                    var DocType = sTemp[sTemp.Length - 2];
                    var SplitNum = sTemp[sTemp.Length - 1];

                    // OCRイメージ連携履歴登録
                    dao.INSERT_D_OCR_COOPERATION_HISTORY(tkskCd
                                                        , hnmCd
                                                        , String.Concat(DocType, SplitNum)
                                                        , OcrImageDirectory
                                                        , string.Empty
                                                        , string.Empty);

                    // ファイルコピーは並列処理で行う
                    OcrImageFileList.ToList().ForEach(OcrImageFile =>
                    //foreach (var OcrImageFile in OcrImageFileList)
                    {
                        var destTempFile = Path.Combine(dstDirectory
                                                       , $"_temporary_OCR_{tkskCd}_{hnmCd}_{DocType}_{SplitNum}_{date}_{num}_{Path.GetFileName(OcrImageFile)}");

                        var destFile = destTempFile.Replace("_temporary_", string.Empty);

                        File.Copy(OcrImageFile, destTempFile);
                        File.Move(destTempFile, destFile);
                        // OCRイメージ連携履歴登録
                        dao.INSERT_D_OCR_COOPERATION_HISTORY(tkskCd
                                                            , hnmCd
                                                            , String.Concat(DocType, SplitNum)
                                                            , string.Empty
                                                            , Path.GetFileName(OcrImageFile)
                                                            , Path.GetFileName(destFile));
                        //CopyCorImageCount++;
                        //lock (FileNameList)
                        //{
                        FileNameList.Add(Path.GetFileName(destFile));
                        //}
                    });
                });

                if (Consts.BusinessID.CDC.Equals(Utils.GetBussinessId()))
                {
                    File.WriteAllLines(Path.Combine(dstDirectory, "CDC_IMAGE.end"), FileNameList);
                }
                return FileNameList.Count;
            }
            finally
            {
                sw.Stop();
                log.Info($"CopyOcrImage:end [コピー件数:{FileNameList.Count:#,0}] [経過時間:{sw.Elapsed}]");
            }
        }
    }
}
