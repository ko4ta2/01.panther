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
                catch
                {

                }

                // 実行ファイルのパスを取得する
                Assembly asm = Assembly.GetEntryAssembly();
                AppPath = Path.GetDirectoryName(asm.Location);

                // DB接続
                DSN = xml.SelectSingleNode("Config/DSN/@value").InnerText;

                // 会社名
                try { CompanyName = xml.SelectSingleNode("Config/CompanyName/@value").InnerText; }
                catch { CompanyName = string.Empty; }

                // 業務名
                BusinessName = xml.SelectSingleNode("Config/BusinessName/@value").InnerText;

                // ユーザ識別
                UserId = xml.SelectSingleNode("Config/UserId/@value").InnerText.ToUpper();

                // エントリデータ出力　連携フォルダ
                ExportFolder = xml.SelectSingleNode("Config/ExportFolder/@value").InnerText;

                try { PCIDSSExportFolder = xml.SelectSingleNode("Config/PCIDSSExportFolder/@value").InnerText; }
                catch { PCIDSSExportFolder = string.Empty; }

                try { DeliveryFolder = xml.SelectSingleNode("Config/DeliveryFolder/@value").InnerText; }
                catch { DeliveryFolder = string.Empty; }

                // エントリデータ出力　連携年月日差異
                iExportDateDiff = int.Parse(xml.SelectSingleNode("Config/ExportDateDiff /@value").InnerText);

                // エントリデータ出力　出力単位
                ExportUnit = xml.SelectSingleNode("Config/ExportUnit/@value").InnerText;

                // 得意先コード
                TokuisakiCode = xml.SelectSingleNode("Config/TokuisakiCode/@value").InnerText;

                // 楽天生命得意先コード
                RLITokuisakiCode = xml.SelectSingleNode("Config/RLITokuisakiCode/@value").InnerText;

                // 品名コード
                HinmeiCode = xml.SelectSingleNode("Config/HinmeiCode/@value").InnerText;

                // 楽天生命品名コード
                RLIHinmeiCode = xml.SelectSingleNode("Config/RLIHinmeiCode/@value").InnerText;

                //OmitChars = xml.SelectSingleNode("Config/OmitChars/@value").InnerText;
                // 禁則文字
                ProhibitionChars = xml.SelectSingleNode("Config/ProhibitionChars/@value").InnerText;

                ExecAfterImport = xml.SelectSingleNode("Config/ExecAfterImport/@value").InnerText;
                ExecAfterExport = xml.SelectSingleNode("Config/ExecAfterExport/@value").InnerText;

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
                NoInputMessage = xml.SelectSingleNode("Config/NoInputMessage/@value").InnerText;

                // エントリ単位分割　連携フォルダ
                DivImageRoot = xml.SelectSingleNode("Config/DivRoot/@value").InnerText;

                // OCR　イメージ出力　連携フォルダ
                OcrImageRoot = xml.SelectSingleNode("Config/OcrImageRoot/@value").InnerText;

                // OCR　CSV取込み　連携フォルダ
                OcrCsvRoot = xml.SelectSingleNode("Config/OcrCsvRoot/@value").InnerText;

                // エントリ対象選択方法
                EntryUnitSelectMode = xml.SelectSingleNode("Config/EntryUnitSelectMode/@value").InnerText;

                //// 抽選処理　当選数
                //iLotteryWinningNumber = int.Parse(xml.SelectSingleNode("Config/LotteryWinningNumber/@value").InnerText);

                //// 抽選結果PDF　出力先フォルダ
                //sLotteryPdfFolder = xml.SelectSingleNode("Config/LotteryPdfFolder/@value").InnerText;

                // DWFC使用
                sUseDWFC = xml.SelectSingleNode("Config/useDWFC/@value").InnerText;

                //TextPassword = xml.SelectSingleNode("Config/TextPassword/@value").InnerText;

                SevenZipPath = xml.SelectSingleNode("Config/SevenZipPath/@value").InnerText;

                //AfterExport納品ファイル名
                //OutFileName = xml.SelectSingleNode("Config/OutFileName/@value").InnerText;

                // 半角フォント
                try { sEntryTextBoxFont = xml.SelectSingleNode("Config/EntryTextBoxFont/@value").InnerText; }
                catch { sEntryTextBoxFont = "ＭＳ 明朝"; }

                // 全角フォント
                try { sEntryTextBoxFontN = xml.SelectSingleNode("Config/EntryTextBoxFontN/@value").InnerText; }
                catch
                {
                    try { sEntryTextBoxFontN = xml.SelectSingleNode("Config/EntryTextBoxFont/@value").InnerText; }
                    catch { sEntryTextBoxFontN = "ＭＳ 明朝"; }
                }

                // チェック方法
                try { sCheckMethod = xml.SelectSingleNode("Config/CheckMethod/@value").InnerText; }
                catch { sCheckMethod = "1"; }

                // Encode
                try { sEnc = xml.SelectSingleNode("Config/Enc/@value").InnerText; }
                catch { sEnc = "1"; }

                try
                {
                    sHulftFolder = xml.SelectSingleNode("Config/HulftFolder/@value").InnerText;
                    //sUid = xml.SelectSingleNode("Config/uid/@value").InnerText;
                    //sPwd = xml.SelectSingleNode("Config/pwd/@value").InnerText;
                }
                catch
                {
                    sHulftFolder = string.Empty;
                    //sUid = string.Empty;
                    //sPwd = string.Empty; 
                }
                try
                {
                    sReceiveHulftFolder = xml.SelectSingleNode("Config/ReceiveHulftFolder/@value").InnerText;
                    sReceiveUid = xml.SelectSingleNode("Config/receiveuid/@value").InnerText;
                    sReceivePwd = xml.SelectSingleNode("Config/receivepwd/@value").InnerText;
                }
                catch
                {
                    sReceiveHulftFolder = string.Empty;
                    sReceiveUid = string.Empty;
                    sReceivePwd = string.Empty;
                }

                // ゼロ件データ作成フラグ
                try { CreateZeroDataFlag = xml.SelectSingleNode("Config/CreateZeroDataFlag/@value").InnerText; }
                catch { CreateZeroDataFlag = Consts.Flag.OFF; }

                try { sTargetImageFile = xml.SelectSingleNode("Config/TargetImageFileExtension/@value").InnerText; }
                catch { sTargetImageFile = "*.jpg*"; }

                try { DocFolder = xml.SelectSingleNode("Config/DocFolder/@value").InnerText; }
                catch { DocFolder = string.Empty; }
            }
            catch (Exception ex)
            {
                log.Error("設定ファイル読取で例外が発生しました" + Environment.NewLine + ex);
                throw;
            }
        }
        #endregion

        public static Color LblTitleBackColor { get; set; }
        public static bool IsTestMode { get; set; }
        public static int iExportDateDiff { get; set; }
        //public static int iLotteryWinningNumber { get; set; }
        //public static string AllowMultiByteChar { get; protected set; }
        public static string AppPath { get; set; }
        public static string BusinessName { get; set; }
        public static string ConfigPath { get; protected set; }
        public static string DSN { get; protected set; }
        public static string CompanyName { get; private set; }
        public static string DivImageRoot { get; set; }
        public static string EntryUnitSelectMode { get; set; }
        public static string ExecAfterExport { get; protected set; }
        public static string ExecAfterImport { get; protected set; }
        public static string ExportFolder { get; protected set; }
        public static string DeliveryFolder { get; private set; }
        public static string ExportUnit { get; set; }
        public static string HinmeiCode { get; protected set; }
        public static string NoInputMessage { get; set; }
        public static string OcrCsvRoot { get; set; }
        public static string OcrImageRoot { get; set; }
        //public static string OmitChars { get; protected set; }
        public static string OutFileName { get; set; }
        public static string RLIHinmeiCode { get; set; }
        public static string ProhibitionChars { get; private set; }
        public static string RLITokuisakiCode { get; set; }
        public static string ReadNotCharNarrowInput { get; set; }
        public static string ReadNotCharNarrowOutput { get; set; }
        public static string ReadNotCharWideInput { get; set; }
        public static string ReadNotCharWideOutput { get; set; }
        public static string SevenZipPath { get; set; }
        public static string TextPassword { get; set; }
        public static string TokuisakiCode { get; protected set; }
        public static string UserId { get; set; }
        public static string sEntryTextBoxFont { get; private set; }
        //public static string sLotteryPdfFolder { get; set; }
        public static string sUseDWFC { get; set; }
        //public static int iMaxLengthDiff { get; private set; }
        public static string sCheckMethod { get; private set; }
        public static string sEnc { get; private set; }
        public static string sHulftFolder { get; private set; }
        //public static string sUid { get; private set; }
        //public static string sPwd { get; private set; }
        public static string sReceiveHulftFolder { get; private set; }
        public static string sReceiveUid { get; private set; }
        public static string sReceivePwd { get; private set; }
        public static string PCIDSSExportFolder { get; private set; }
        public static string sTargetImageFile { get; private set; }
        public static string CreateZeroDataFlag { get; private set; }
        public static string sEntryTextBoxFontN { get; private set; }
        public static string DocFolder { get; private set; }
    }
}
