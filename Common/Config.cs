using NLog;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Common
{
    /// <summary>
    /// アプリ設定値
    /// </summary>
    class Config
    {
        /// <summary>
        /// log
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        //public static string PCIDSS { get; private set; }

        #region 環境設定取得

        /// <summary>
        /// Configファイルから値を取得する
        /// </summary>
        public static void GetConfig(string sConfigFileName)
        {
            try
            {
                if (!File.Exists(sConfigFileName))
                    throw new Exception(String.Format("環境ファイル：{0}が見つかりません", sConfigFileName));

                Config.ConfigPath = sConfigFileName;

                XmlDocument xml = new XmlDocument();
                xml.Load(ConfigPath);

                IsTestMode = false;
                LblTitleBackColor = Common.Consts.TITLE_BACK_COLOR.HNBN;
                try
                {
                    using (StreamReader sr = new StreamReader(Path.Combine(System.Environment.CurrentDirectory, "TestMode.txt"), Consts.EncShift_JIS))
                    {
                        var sValue = sr.ReadToEnd();
                        if ("TestMode".Equals(sValue))
                        {
                            IsTestMode = true;
                            LblTitleBackColor = Common.Consts.TITLE_BACK_COLOR.TEST;
                        }
                    }
                }
                catch { }

                // 実行ファイルのパスを取得する
                Assembly asm = Assembly.GetEntryAssembly();
                AppPath = Path.GetDirectoryName(asm.Location);

                // DB接続
                try { DSN = xml.SelectSingleNode("Config/DSN/@value").InnerText; }
                catch { DSN = string.Empty; }

                if (CompanyName == null)
                {
                    // 会社名
                    try { CompanyName = xml.SelectSingleNode("Config/CompanyName/@value").InnerText; }
                    catch { CompanyName = string.Empty; }
                }

                if (BusinessName == null)
                {
                    // 業務名
                    try { BusinessName = xml.SelectSingleNode("Config/BusinessName/@value").InnerText; }
                    catch { BusinessName = string.Empty; }
                }

                if (UserId == null)
                {
                    // ユーザ識別
                    try { UserId = xml.SelectSingleNode("Config/UserId/@value").InnerText.ToUpper(); }
                    catch { UserId = string.Empty; }
                }

                // エントリデータ出力　連携フォルダ
                ExportFolder = xml.SelectSingleNode("Config/ExportFolder/@value").InnerText;

                try { PCIDSSExportFolder = xml.SelectSingleNode("Config/PCIDSSExportFolder/@value").InnerText; }
                catch { PCIDSSExportFolder = string.Empty; }

                try { DeliveryFolder = xml.SelectSingleNode("Config/DeliveryFolder/@value").InnerText; }
                catch { DeliveryFolder = string.Empty; }

                // エントリデータ出力　連携年月日差異
                ExportDateDiff = int.Parse(xml.SelectSingleNode("Config/ExportDateDiff/@value").InnerText);

                // エントリデータ出力　出力単位
                ExportUnit = xml.SelectSingleNode("Config/ExportUnit/@value").InnerText;

                if (TokuisakiCode == null)
                {
                    // 得意先コード
                    try { TokuisakiCode = xml.SelectSingleNode("Config/TokuisakiCode/@value").InnerText; }
                    catch { TokuisakiCode = string.Empty; }
                }

                // 楽天生命得意先コード
                //RLITokuisakiCode = xml.SelectSingleNode("Config/RLITokuisakiCode/@value").InnerText;

                if (HinmeiCode == null)
                {
                    // 品名コード
                    try { HinmeiCode = xml.SelectSingleNode("Config/HinmeiCode/@value").InnerText; }
                    catch { HinmeiCode = string.Empty; }
                }

                // 楽天生命品名コード
                //RLIHinmeiCode = xml.SelectSingleNode("Config/RLIHinmeiCode/@value").InnerText;

                //OmitChars = xml.SelectSingleNode("Config/OmitChars/@value").InnerText;
                // 禁則文字
                ProhibitionChars = xml.SelectSingleNode("Config/ProhibitionChars/@value").InnerText;

                ExecAfterImportFlag = xml.SelectSingleNode("Config/ExecAfterImportFlag/@value").InnerText;
                ExecAfterExportFlag = xml.SelectSingleNode("Config/ExecAfterExportFlag/@value").InnerText;

                //AllowMultiByteChar = xml.SelectSingleNode("Config/AllowMultiByteChar/@value").InnerText;

                // 判読不可入力文字（半角）
                ReadNotCharNarrowInput = xml.SelectSingleNode("Config/ReadNotCharNarrowInput/@value").InnerText;

                // 判読不可入力文字（全角）
                ReadNotCharWideInput = xml.SelectSingleNode("Config/ReadNotCharWideInput/@value").InnerText;

                // 判読不可出力文字（半角）
                ReadNotCharNarrowOutput = xml.SelectSingleNode("Config/ReadNotCharNarrowOutput/@value").InnerText;

                // 判読不可出力文字（全角）
                ReadNotCharWideOutput = xml.SelectSingleNode("Config/ReadNotCharWideOutput/@value").InnerText;

                // 未入力時の警告メッセージ有無
                NoInputMessageFlag = xml.SelectSingleNode("Config/NoInputMessageFlag/@value").InnerText;

                // エントリ単位分割　連携フォルダ
                DivImageRoot = xml.SelectSingleNode("Config/DivRoot/@value").InnerText;

                // OCR　イメージ出力　連携フォルダ
                OcrImageRoot = xml.SelectSingleNode("Config/OcrImageRoot/@value").InnerText;

                // OCR　CSV取込み　連携フォルダ
                OcrCsvRoot = xml.SelectSingleNode("Config/OcrCsvRoot/@value").InnerText;

                // エントリ対象選択方法
                //EntryUnitSelectMode = xml.SelectSingleNode("Config/EntryUnitSelectMode/@value").InnerText;

                // DWFC使用
                try { UseDWFC = xml.SelectSingleNode("Config/useDWFCFlag/@value").InnerText; }
                catch { UseDWFC = string.Empty; }

                // 7zパス
                try { SevenZipPath = xml.SelectSingleNode("Config/SevenZipPath/@value").InnerText; }
                catch { SevenZipPath = string.Empty; }

                // 半角フォント
                try { EntryTextBoxFont = xml.SelectSingleNode("Config/EntryTextBoxFont/@value").InnerText; }
                catch { EntryTextBoxFont = "ＭＳ 明朝"; }

                // 全角フォント
                try { EntryTextBoxFontN = xml.SelectSingleNode("Config/EntryTextBoxFontN/@value").InnerText; }
                catch { EntryTextBoxFontN = "ＭＳ 明朝"; }

                //// チェック方法
                //try { sCheckMethod = xml.SelectSingleNode("Config/CheckMethod/@value").InnerText; }
                //catch { sCheckMethod = "1"; }

                // Encode
                try { Encode = xml.SelectSingleNode("Config/Enc/@value").InnerText; }
                catch { Encode = "1"; }

                // Hulftフォルダ
                try { SendHulftFolder = xml.SelectSingleNode("Config/HulftFolder/@value").InnerText; }
                catch { SendHulftFolder = string.Empty; }

                try
                {
                    ReceiveHulftFolder = xml.SelectSingleNode("Config/ReceiveHulftFolder/@value").InnerText;
                    ReceiveFolderUid = xml.SelectSingleNode("Config/receiveuid/@value").InnerText;
                    ReceiveFilderPwd = xml.SelectSingleNode("Config/receivepwd/@value").InnerText;
                }
                catch
                {
                    ReceiveHulftFolder = string.Empty;
                    ReceiveFolderUid = string.Empty;
                    ReceiveFilderPwd = string.Empty;
                }

                // ゼロ件データ作成フラグ
                try { CreateZeroDataFlag = xml.SelectSingleNode("Config/CreateZeroDataFlag/@value").InnerText; }
                catch { CreateZeroDataFlag = Consts.Flag.OFF; }

                try { ImageFileExtension = xml.SelectSingleNode("Config/ImageFileExtension/@value").InnerText; }
                catch { ImageFileExtension = "*.jpg*"; }

                try { DocFolder = xml.SelectSingleNode("Config/DocFolder/@value").InnerText; }
                catch { DocFolder = string.Empty; }

                // COSMOS連携
                try { COSMOS_FLAG = xml.SelectSingleNode("Config/COSMOS_FLAG/@value").InnerText; }
                catch { COSMOS_FLAG = Consts.Flag.OFF; }

                try { END_FILE_DIRECTORY = xml.SelectSingleNode("Config/END_FILE_DIRECTORY/@value").InnerText; }
                catch { END_FILE_DIRECTORY = string.Empty; }
            }
            catch (Exception ex)
            {
                log.Error("設定ファイル読取で例外が発生しました" + Environment.NewLine + ex);
                throw;
            }
        }
        #endregion

        public static Color LblTitleBackColor { get; private set; }

        public static bool IsTestMode { get; private set; }

        public static int ExportDateDiff { get; private set; }

        public static string AppPath { get; private set; }

        public static string BusinessName { get; set; }

        public static string ConfigPath { get; private set; }

        public static string DSN { get; private set; }

        public static string CompanyName { get; set; }

        public static string DivImageRoot { get; private set; }

        public static string ExecAfterExportFlag { get; private set; }

        public static string ExecAfterImportFlag { get; private set; }

        public static string ExportFolder { get; private set; }

        public static string DeliveryFolder { get; private set; }

        public static string ExportUnit { get; private set; }

        public static string HinmeiCode { get; set; }

        public static string NoInputMessageFlag { get; private set; }

        public static string OcrCsvRoot { get; private set; }

        public static string OcrImageRoot { get; private set; }

        public static string ProhibitionChars { get; private set; }

        public static string ReadNotCharNarrowInput { get; private set; }

        public static string ReadNotCharNarrowOutput { get; private set; }

        public static string ReadNotCharWideInput { get; private set; }

        public static string ReadNotCharWideOutput { get; private set; }

        public static string SevenZipPath { get; private set; }

        public static string TokuisakiCode { get; set; }

        public static string UserId { get; set; }

        public static string EntryTextBoxFont { get; private set; }

        public static string UseDWFC { get; private set; }

        public static string Encode { get; private set; }

        public static string SendHulftFolder { get; private set; }

        public static string ReceiveHulftFolder { get; private set; }

        public static string ReceiveFolderUid { get; private set; }

        public static string ReceiveFilderPwd { get; private set; }

        public static string PCIDSSExportFolder { get; private set; }

        public static string CreateZeroDataFlag { get; private set; }

        public static string EntryTextBoxFontN { get; private set; }

        public static string DocFolder { get; private set; }

        public static string COSMOS_FLAG { get; set; }

        public static string END_FILE_DIRECTORY { get; private set; }

        public static string ImageFileExtension { get; private set; }
    }
}
