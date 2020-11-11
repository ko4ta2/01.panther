using Microsoft.VisualBasic.FileIO;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Utils
    {
        #region 変数

        /// <summary>
        /// log
        /// </summary>
        private static Logger log = LogManager.GetCurrentClassLogger();

        #endregion

        ///// <summary>
        ///// ディスプレイの作業エリア(タスクバーを除いた領域)を取得します
        ///// </summary>
        ///// <param name="ctrl"></param>
        ///// <returns></returns>
        //public static Rectangle GetScreenWorkingArea(Control ctrl)
        //{
        //    // デフォルトはプライマリディスプレイの作業エリアを取得します
        //    var screen = Screen.PrimaryScreen.WorkingArea;

        //    // コントロールが指定されていれば、コントロールのあるディスプレイの作業エリアを取得します
        //    if (ctrl != null)
        //        screen = Screen.FromControl(ctrl).WorkingArea;

        //    return screen;
        //}

//        /// <summary>
//        /// 入力可能なマルチバイト文字であるかの入力チェックを行う
//        /// </summary>
//        /// <param name="sInputText">対象のテキスト</param>
//        /// <returns>入力可能な文字のみの場合はtrue、入力不可文字を含む場合はfalseを返します。</returns>
//        public static bool IsAllowMultiByteChar(string sInputText)
//        {
///*
//            // 入力は何でもOK
//            if (Consts.Flag.ON.Equals(Config.AllowMultiByteChar))
//            {
//                return true;
//            }

//            // 入力可能全角文字チェック
//            // shift-jis（第一水準＋第二水準）以外の文字を変換すると"？"に変換される特性を利用してチェックをします。
//            // ただしshift-jisの中で第一水準＋第二水準に含まれないものもあるためそれが含まれている場合もＮＧとします。
//            string convertedText = Microsoft.VisualBasic.Strings.StrConv(sInputText, Microsoft.VisualBasic.VbStrConv.Wide);
//            if (convertedText.Contains("？")
//                || Regex.IsMatch(convertedText, "[" + Config.OmitChars + "]"))
//            {
//                return false;
//            }

//            // 禁則文字チェック
//            if (Regex.IsMatch(convertedText, "[" + Config.DenyChars + "]"))
//                return false;
//*/
//            return true;
//        }

        #region バージョン
        /// <summary>
        /// バージョンを取得する
        /// </summary>
        /// <returns>バージョン</returns>
        public static string GetVersion()
        {
            var s = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            return String.Join(".", s[0], s[1], s[2], int.Parse(s[3]).ToString("d3"));
        }
        #endregion

        #region プロセス起動
        /// <summary>
        /// プロセス起動
        /// </summary>
        /// <param name="sExecuteFilePath">実行ファイルパス</param>
        /// <param name="sParam">パラメータ</param>
        public static void ProcessStartWaitForExit(string sExecuteFilePath, string sParam = null, bool check = false)
        {
            //try
            //{
            Console.WriteLine("実行ファイル:{0} 起動パラメータ:{1}", sExecuteFilePath, sParam);

            var psi = new ProcessStartInfo();
            psi.FileName = sExecuteFilePath;
            psi.RedirectStandardInput = false;
            psi.RedirectStandardOutput = check;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = false;
            psi.Arguments = sParam;
            using (var p = Process.Start(psi))
            {
                p.WaitForExit();

                log.Debug("実行ファイル:{0} 起動パラメータ:{1} ExitCode:{2}", sExecuteFilePath, sParam, p.ExitCode);
                Console.WriteLine("実行ファイル:{0} 起動パラメータ:{1} ExitCode:{2}", sExecuteFilePath, sParam, p.ExitCode);

                if (check)
                {
                    var output = p.StandardOutput.ReadToEnd();  // 標準出力の読み取り
                    output = output.Replace("\r\r\n", "\n");    // 改行コードの修正
                    log.Info(output); Console.WriteLine(output);

                    if (p.ExitCode != 0
                        || output.Contains("0 files, 0 bytes"))
                    {
                        log.Error("↓↓↓↓↓↓↓↓↓↓  Prosess Console Output  ↓↓↓↓↓↓↓↓↓↓"); // ［出力］ウィンドウに出力
                        log.Error(output); // ［出力］ウィンドウに出力
                        log.Error("↑↑↑↑↑↑↑↑↑↑  Prosess Console Output  ↑↑↑↑↑↑↑↑↑↑"); // ［出力］ウィンドウに出力
                        throw new ApplicationException("実行したプロセスで異常が検知されました。");
                    }
                }
            }
            //}
            //catch
            //{
            //    throw;
            //}
        }
        #endregion

        ///// <summary>
        ///// 入力NG文字判定
        ///// </summary>
        ///// <param name="sChar"></param>
        ///// <returns></returns>
        //public static bool IsNgChar(string sChar)
        //{
        //    var byteData = Consts.Enciso2022jp.GetBytes(sChar);

        //    if (byteData.Length == 8)
        //    {
        //        // 2バイトコード
        //        int 区 = byteData[3] - 32;
        //        int 点 = byteData[4] - 32;
        //        Console.WriteLine(sChar + " 区点:" + 区 + "-" + 点);
        //        //if ((区 >= 9 && 区 <= 15) || (区 >= 85 && 区 <= 92))
        //        if ((区 == 13) || (区 >= 85))
        //        {
        //            //Console.Beep();
        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        //Console.Beep();
        //        return true;
        //    }
        //    // OK
        //    return false;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sInput"></param>
        /// <returns></returns>
        public static int strLenB(char sInput)
        {
            string sConv = Microsoft.VisualBasic.Strings.StrConv(sInput.ToString(), Microsoft.VisualBasic.VbStrConv.Wide);
            if ("？".Equals(sConv))
            {
                if ("?".Equals(sInput.ToString()))
                {
                    // 半角「?」は「？」に変換させる
                    return 1;
                }
                else
                {
                    // 第１・第２水準以外
                    return -1;
                }
            }
            else
            {
                return Consts.EncShift_JIS.GetByteCount(sInput.ToString());
            }
        }

        /// <summary>
        /// エントリバッチID編集
        /// </summary>
        /// <param name="sImageCaptureDate"></param>
        /// <param name="sImageCaptureNum"></param>
        /// <param name="sDocId"></param>
        /// <param name="sEntryUnit"></param>
        /// <returns></returns>
        public static string EditEntryBatchId(string ENTRY_UNT_ID)
        {
            var x = ENTRY_UNT_ID.Split('_');
            return $"{Config.UserId.ToUpper()}-{x[2]}-{x[3].Trim()}-{x[4]}-{x[5]}";
        }

        ///// <summary>
        ///// RLI判定
        ///// </summary>
        ///// <returns></returns>
        //public static bool IsRLI()
        //{
        //    if (Config.TokuisakiCode.Equals(Config.RLITokuisakiCode)
        //        && Config.HinmeiCode.Equals(Config.RLIHinmeiCode))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// アセンブリ名取得
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyName()
        {
            return Assembly.GetExecutingAssembly().GetName().Name.ToString(); // 実行中のアセンブリを取得する。
        }

        public static string GetAssemblyTitle()
        {
            return ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute))).Title;
        }

        public static char ToMultiByte(char cKeyChar)
        {
             return Microsoft.VisualBasic.Strings.StrConv(cKeyChar.ToString().Replace(@"\", @"￥"), Microsoft.VisualBasic.VbStrConv.Wide)[0];
        }

        /// <summary>
        /// 第一水準・第二水準チェック用エンコード
        /// </summary>
        //private static System.Text.Encoding iso2022jp = System.Text.Encoding.GetEncoding("iso-2022-jp");

        //private static System.Text.Encoding sjis = System.Text.Encoding.GetEncoding("Shift_JIS");

        //private static System.Text.Encoding utf8 = System.Text.Encoding.UTF8;

        /// <summary>
        /// 入力文字チェック
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValidChar(string inputMode, string value)
        {
            //byte[] bytes = null;
            switch (Config.Encode)
            {
                case Consts.Encode.SJIS:
                    return IsShiftJis(value);
                case Consts.Encode.JIS:
                case Consts.Encode.JIS_EX:
                    // バイト配列へ変換
                    byte[] bytes = Consts.Enciso2022jp.GetBytes(value);
                    if (bytes.Length == 1) { return false; }

                    // 区点コードへ変換
                    var ku = bytes[3] - 0x20;
                    var tn = bytes[4] - 0x20;
                    var kutn = ku * 100 + tn;

                    if (Consts.Encode.JIS.Equals(Config.Encode))
                    {
                        if ((kutn > 0832 && kutn < 1601) || kutn > 8406) { return false; }
                    }
                    else
                    {
                        if (kutn > 9494) { return false; }
                    }
                    break;
                case Consts.Encode.UTF8:
                    return IsUtf8(value);
                    // utf8
                    //bytes = Consts.EncUTF8.GetBytes(sValue);
                    //break;
            }

            // 禁則文字（コンフィグに設定）
            if (Config.ProhibitionChars.IndexOf(value) >= 0) { return false; }

            // OK
            return true;
        }

        /// <summary>
        /// DataTableソート
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sSortKey"></param>
        /// <returns></returns>
        public static DataTable SortDataTable(DataTable dt ,string sSortKey)
        {
            var dv = new DataView(dt);
            dv.Sort = sSortKey;
            return dv.ToTable();
        }

        #region 汎用バックアップ
        /// <summary>
        /// ファイルバックアップ
        /// </summary>
        /// <param name="sBackupFileList"></param>
        /// <param name="sTargetFolder"></param>
        /// <param name="bDelete"></param>
        public static void BackUp(string[] sBackupFileList, string sTargetFolder, bool bDelete = false)
        {
            // 出力ファイル退避
            if (sBackupFileList.Length == 0)
                return;

            //int iDestFileCount = 0;
            var sMessage = "↓↓↓↓↓↓↓↓↓↓　ファイルバックアップ　↓↓↓↓↓↓↓↓↓↓";
            Console.WriteLine(sMessage); log.Info(sMessage);
            var sDestFolder = sTargetFolder + @"\_bk";
            if (!Directory.Exists(sDestFolder))
                Directory.CreateDirectory(sDestFolder);

            while (true)
            {
                sDestFolder = Path.Combine(sTargetFolder + @"\_bk", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                if (Directory.Exists(sDestFolder))
                {
                    System.Threading.Thread.Sleep(1000);
                    continue;
                }
                Directory.CreateDirectory(sDestFolder);
                break;
            }

            Parallel.ForEach(sBackupFileList, sFilePath =>
            {
                if (bDelete)
                    File.Move(sFilePath, Path.Combine(sDestFolder, Path.GetFileName(sFilePath)));
                else
                    File.Copy(sFilePath, Path.Combine(sDestFolder, Path.GetFileName(sFilePath)));
                sMessage = $"バックアップ:「{sFilePath}」→「{Path.Combine(sDestFolder, Path.GetFileName(sFilePath))}」";
                Console.WriteLine(sMessage); log.Info(sMessage);
            });
            sMessage = $"バックアップファイル数:{sBackupFileList.Length:#,0}" ;
            Console.WriteLine(sMessage); log.Info(sMessage);
            sMessage = "↑↑↑↑↑↑↑↑↑↑　ファイルバックアップ　↑↑↑↑↑↑↑↑↑↑";
            Console.WriteLine(sMessage); log.Info(sMessage);
        }
        #endregion

        /// <summary>
        /// objectをDecimal型に変換します
        /// </summary>
        /// <param name="value">オブジェクト</param>
        /// <returns>数値</returns>
        public static decimal ToDecimal(this object value)
        {
            if (value == null) { return decimal.Zero; }
            decimal num;
            if (decimal.TryParse(value.ToString(), out num)) { return num; }
            return decimal.Zero;
        }

        #region ネットワーク認証
        /// <summary>
        /// ネットワーク認証
        /// </summary>
        /// <param name="sTargetPath"></param>
        /// <param name="sUserId"></param>
        /// <param name="sPassword"></param>
        public static void CertificationNetwork(string sTargetPath, string sUserId, string sPassword, bool bConnect = true)
        {
            if (sUserId.Length == 0 || sPassword.Length == 0)
                return;

            var nr = new NETRESOURCE();
            nr.dwScope = 0;
            nr.dwType = 0;
            nr.dwDisplayType = 0;
            nr.dwUsage = 0;
            nr.lpLocalName = string.Empty;
            nr.lpRemoteName = sTargetPath;
            nr.lpProvider = string.Empty;

            if (bConnect)
            {
                // 認証
                int iRetCancel = WNetCancelConnection2(sTargetPath, 0, true);
                log.Info("キャンセル結果:{0}", iRetCancel);

                int iRet = WNetAddConnection2(ref nr, sPassword, sUserId, 0);
                log.Info("認証結果:{0}", iRet);
            }
            else
            {
                // キャンセル
                int iRetCancel = WNetCancelConnection2(sTargetPath, 0, true);
                log.Info("キャンセル結果:{0}", iRetCancel);
            }
        }

        [DllImport("mpr.dll", EntryPoint = "WNetAddConnection2",
            CharSet = CharSet.Unicode)]
        private static extern int WNetAddConnection2(ref NETRESOURCE lpNetResource, string lpPassword, string lpUsername, Int32 dwFlags);

        [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2",
            CharSet = CharSet.Unicode)]
        private static extern int WNetCancelConnection2(string lpName, Int32 dwFlags, bool fForce);

        [StructLayout(LayoutKind.Sequential)]
        internal struct NETRESOURCE
        {
            public int dwScope;
            public int dwType;
            public int dwDisplayType;
            public int dwUsage;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpLocalName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpRemoteName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpComment;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpProvider;
        }
        #endregion

        /// <summary>
        /// 数値検証
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(string value)
        {
            if (!decimal.TryParse(value, out _)) { return false; }
            return true;
        }

        #region マイナンバー検証
        /// <summary>
        /// マイナンバー検証
        /// </summary>
        /// <param name="sMyNumber"></param>
        /// <returns></returns>
        public static bool IsValidMyNumber(string value)
        {
            //12文字でなければ偽
            if (value.Length != 12)
                return false;

            //整数の列挙にして逆転
            var digits = value.Select(e => e - '0').Reverse();

            //（↑で逆転しているので）最初の数字がチェックデジット
            var checkDigit = digits.First();

            var reminder = digits
                .Skip(1)
                .Select((e, i) => { var p = e; var q = i <= 5 ? i + 2 : i - 4; return p * q; })
                .Sum() % 11;

            return checkDigit == (reminder == 0 || reminder == 1 ? 0 : 11 - reminder);
        }
        #endregion

        /// <summary>
        /// nDR検証
        /// </summary>
        /// <param name="value"></param>
        /// <param name="div"></param>
        /// <param name="Subtract"></param>
        /// <returns></returns>
        public static bool IsValidDr(string value, string div, bool subtract = false)
        {
            if (value.Contains(Config.ReadNotCharNarrowInput) || value.Length ==0)
                return true;

            if (!decimal.TryParse(value, out _)) { return false; }

            if (value.Length < 2) { return false; }

            int iDiviser = int.Parse(div);
            if (subtract)
            {
                if (iDiviser - (decimal.Parse(value.Substring(0, value.Length - 1)) % iDiviser) != decimal.Parse(value.Substring(value.Length - 1, 1)))
                    return false;
            }
            else
            {
                if (decimal.Parse(value.Substring(0, value.Length - 1)) % iDiviser != decimal.Parse(value.Substring(value.Length - 1, 1)))
                    return false;
            }

            return true;
        }

        #region　LeftB メソッド
        /// -----------------------------------------------------------------------------------------
        /// <summary>
        ///     文字列の左端から指定したバイト数分の文字列を返します。</summary>
        /// <param name="stTarget">
        ///     取り出す元になる文字列。<param>
        /// <param name="iByteSize">
        ///     取り出すバイト数。</param>
        /// <returns>
        ///     左端から指定されたバイト数分の文字列。</returns>
        /// -----------------------------------------------------------------------------------------
        public static string LeftB(string stTarget, int iByteSize)
        {
            return MidB(stTarget, 1, iByteSize);
        }

        #endregion

        #region　MidB メソッド (+1)
        /// -----------------------------------------------------------------------------------------
        /// <summary>
        ///     文字列の指定されたバイト位置以降のすべての文字列を返します。</summary>
        /// <param name="stTarget">
        ///     取り出す元になる文字列。</param>
        /// <param name="iStart">
        ///     取り出しを開始する位置。</param>
        /// <returns>
        ///     指定されたバイト位置以降のすべての文字列。</returns>
        /// -----------------------------------------------------------------------------------------
        public static string MidB(string stTarget, int iStart)
        {
            var hEncoding = Consts.EncShift_JIS;
            var btBytes = hEncoding.GetBytes(stTarget);
            return hEncoding.GetString(btBytes, iStart - 1, btBytes.Length - iStart + 1);
        }

        /// -----------------------------------------------------------------------------------------
        /// <summary>
        ///     文字列の指定されたバイト位置から、指定されたバイト数分の文字列を返します。</summary>
        /// <param name="stTarget">
        ///     取り出す元になる文字列。</param>
        /// <param name="iStart">
        ///     取り出しを開始する位置。</param>
        /// <param name="iByteSize">
        ///     取り出すバイト数。</param>
        /// <returns>
        ///     指定されたバイト位置から指定されたバイト数分の文字列。</returns>
        /// -----------------------------------------------------------------------------------------
        public static string MidB(string stTarget, int iStart, int iByteSize)
        {
            var hEncoding = Consts.EncShift_JIS;
            var btBytes = hEncoding.GetBytes(stTarget);
            return hEncoding.GetString(btBytes, iStart - 1, iByteSize);
        }
        #endregion

        #region　RightB メソッド
        /// -----------------------------------------------------------------------------------------
        /// <summary>
        ///     文字列の右端から指定されたバイト数分の文字列を返します。</summary>
        /// <param name="stTarget">
        ///     取り出す元になる文字列。</param>
        /// <param name="iByteSize">
        ///     取り出すバイト数。</param>
        /// <returns>
        ///     右端から指定されたバイト数分の文字列。</returns>
        /// -----------------------------------------------------------------------------------------
        public static string RightB(string stTarget, int iByteSize)
        {
            var hEncoding = Consts.EncShift_JIS;
            var btBytes = hEncoding.GetBytes(stTarget);
            return hEncoding.GetString(btBytes, btBytes.Length - iByteSize, iByteSize);
        }

        #endregion

        //public static bool IsZenkaku(string s)
        //{
        //    if (Consts.EncShift_JIS.GetByteCount(s) == s.Length * 2)
        //        return true;
        //    return false;
        //}

/// <summary>
/// 
/// </summary>
/// <param name="value"></param>
/// <returns></returns>
        public static bool IsSingleByteChar(string value)
        {
            if (Config.Encode.Equals(Consts.Encode.UTF8))
            {
                // 全角カナと半角カナの区別がつかない！！
                const string x = "ｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜｦﾝｧｨｩｪｫｯｬｭｮｰﾟﾞ｡｢｣､･";
                for (int i = 0; i <= x.Length - 1; i++)
                    value = value.Replace(x[i].ToString(), string.Empty);
                return Consts.EncUTF8.GetByteCount(value) == value.Length;
            }
            else
                return Consts.EncShift_JIS.GetByteCount(value) == value.Length;
        }

        public static int ByteLength(string value)
        {
            return Consts.EncShift_JIS.GetByteCount(value);
        }

        public static Image LoadImage(string FilePath, bool Lock = true)
        {
            Image Img = null;
            if (Lock)
                return Image.FromStream(File.OpenRead(FilePath), false, false);
            else
                using (var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                    Img = Image.FromStream(fs);
            return Img;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetIPAddress()
        {
            var IPAddress = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());
            var sIPAddress = string.Empty;
            foreach (var s in IPAddress[1].ToString().Split('.'))
                sIPAddress += s.PadLeft(3, '0');
            return sIPAddress;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setVersion"></param>
        /// <returns></returns>
        public static string GetFormLoginText(bool setVersion = false)
        {
            return String.Join(" ", Config.CompanyName, Config.BusinessName, setVersion ? $" Ver.{GetVersion()}" : string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setVersion"></param>
        /// <returns></returns>
        public static string GetFormText(bool setVersion = false)
        {
            var s = String.Join(" ", Config.CompanyName, Config.BusinessName, setVersion ? $" Ver.{GetVersion()}" : string.Empty);
            if (Consts.Flag.ON.Equals(Config.COSMOS_FLAG))
                s = String.Concat(Config.BusinessName, setVersion ? $" Ver.{GetVersion()}" : string.Empty);
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetBussinessId()
        {
            return String.Join("_", Config.TokuisakiCode, Config.HinmeiCode);
        }

        public static bool IsValidMailAddress(string mailaddress)
        {
            if (String.IsNullOrEmpty(mailaddress)) { return false; }
            try
            {
                var a = new System.Net.Mail.MailAddress(mailaddress);
            }
            catch { return false; }
            return true;
        }

        public static bool IsValidDate(string value, string format)
        {
            if (String.IsNullOrEmpty(value) || String.IsNullOrEmpty(format)) { return false; }
            if (value.IndexOf(Config.ReadNotCharNarrowInput[0]) != -1)
                return true;
            try
            {
                DateTime.ParseExact(value, format, null);
            }
            catch { return false; }
            return true;
        }

        public static void /* (List<string>, List<string[]>)*/ GetEntryTsv(string BusinessId, Encoding enc)
        {
            ////int iWriteCount = 0;
            //var ListEntryFiles = new List<string>();
            var ListEntryItems = new List<string[]>();

            // 出力フォルダ内のファイル取得
            var ListEntryFiles = Directory.GetFiles(Config.ExportFolder, String.Format("{0}_EntryData_*.tsv", BusinessId), System.IO.SearchOption.TopDirectoryOnly);

//            sListSrcFiles.AsParallel
            foreach(var SrcFile in ListEntryFiles)
            {
                log.Info("処理対象ファイル：{0}", SrcFile);
                //ListEntryFiles.Add(SrcFile);

                // 帳票ID取得
                var sDocId = Path.GetFileName(SrcFile).Split('_')[2];

                // エントリテキスト読込み
                string[] items = null;
                //List<string> items2;
                using (var ps = new TextFieldParser(SrcFile, enc))
                {
                    ps.TextFieldType = FieldType.Delimited;
                    ps.SetDelimiters("\t");
                    while (!ps.EndOfData)
                    {
                        items = ps.ReadFields();
                        // 明細行のみ対象
                        if (!Consts.EntryResultRecordType.Item.Equals(items[0]))
                            continue;

                        // 明細レコードの１項目目に帳票IDを設定
                        items[0] = sDocId;

                        // Listに追加
                        ListEntryItems.Add(items);
                    }
                }
            }
            //return (ListEntryFiles.ToList(), ListEntryItems);
        }

        private static bool IsShiftJis(string val)
        {
            return val.Equals(Encoding.GetEncoding(932).GetString(Encoding.GetEncoding(932).GetBytes(val)));
        }

        private static bool IsUtf8(string val)
        {
            return val.Equals(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(val)));
        }

        public static string ToUpperKana(string value)
        {
            return value;
        }

        public static bool IsValidCheckDigit(string value)
        {
            try
            {
                //#region 未チェック条件
                //if (value.Contains(Config.ReadNotCharNarrowInput))
                //    return true;
                //#endregion

                if (value.Length != 12)
                    throw new ApplicationException();

                var conv = new string[] { null, "B", "C", "H", "K", "M", "R", "U", "X", "Y", "Z", "A" };
                if (!conv.Contains(value.Substring(10, 1)))
                    throw new ApplicationException();

                var S = 0;
                for (int idx = 0; idx <= 9; idx++)
                    S += int.Parse(value.Substring(idx, 1)) * (10 - idx);

                var D = 11 - S % 11;
                if (!conv[D].Equals(value.Substring(10, 1)))
                    throw new ApplicationException();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
