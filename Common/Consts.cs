using System.Drawing;

namespace Common
{
    class Consts
    {
        /// <summary>
        /// エントリ結果レコード種別
        /// </summary>
        public struct EntryResultRecordType
        {
            /// <summary>
            /// ヘッダーレコード
            /// </summary>
            public const string Head = "01";

            /// <summary>
            /// 明細レコード
            /// </summary>
            public const string Item = "02";
        
            /// <summary>
            /// エンドレコード
            /// </summary>
            public const string End = "09";
        }

        /// <summary>
        /// レコード区分
        /// </summary>
        public struct RecordKbn
        {
            /// <summary>
            /// コンペア
            /// </summary>
            public const string ADMIN = "0";

            /// <summary>
            /// １人目
            /// </summary>
            public const string Entry_1st = "1";

            /// <summary>
            /// ２人目
            /// </summary>
            public const string Entry_2nd = "2";

            /// <summary>
            /// ＯＣＲ
            /// </summary>
            public const string OCR = "*";
        }

        /// <summary>
        /// フラグ
        /// </summary>
        public struct Flag
        {
            /// <summary>
            /// オン
            /// </summary>
            public const string ON = "1";

            /// <summary>
            /// オフ
            /// </summary>
            public const string OFF = "0";
        }

        /// <summary>
        /// 入力状態
        /// </summary>
        public struct EntryStatus
        {
            /// <summary>
            /// 未入力
            /// </summary>
            public const string ENTRY_NOT = "0";

            /// <summary>
            /// 入力中
            /// </summary>
            public const string ENTRY_ING = "1";

            /// <summary>
            /// 入力済
            /// </summary>
            public const string ENTRY_END = "2";
        }

        /// <summary>
        /// エントリ単位選択方法
        /// </summary>
        public struct EntryUnitSelectMode
        {
            /// <summary>
            /// 自動選択
            /// </summary>
            public const string Auto = "1";

            /// <summary>
            /// マニュアル選択
            /// </summary>
            public const string Manual = "2";
        }

        /// <summary>
        /// エントリ単位ステータス
        /// </summary>
        public struct EntryUnitStatus
        {
            /// <summary>
            /// 未エントリ
            /// </summary>
            public const string ENTRY_NOT = "0";

            /// <summary>
            /// エントリ中
            /// </summary>
            public const string ENTRY_EDT = "1";

            /// <summary>
            /// エントリ済
            /// </summary>
            public const string ENTRY_END = "2";

            /// <summary>
            /// コンペア中
            /// </summary>
            public const string COMPARE_ING = "5";
            /// <summary>
            /// コンペア修正中
            /// </summary>
            public const string COMPARE_EDT = "6";

            /// <summary>
            /// コンペア修正済
            /// </summary>
            public const string COMPARE_END = "7";
            
            /// <summary>
            /// テキスト出力中（実際には無い）
            /// </summary>
            public const string EXPORT_ING = "8";
            
            /// <summary>
            /// テキスト出力済
            /// </summary>
            public const string EXPORT_END = "9";
        }

        ///// <summary>
        ///// 差異フラグ(差異有：1；差異無：0)
        ///// </summary>
        //public struct DiffFlag
        //{
        //    public const string ON = "1";
        //    public const string OFF = "0";
        //}

        public struct TEXT
        {
            public const string RE_ENTRY_TEXT = "の入力";
            public const string PASSWORD_CHAR = "●●●●●●●●●●";
        }

        public struct Messages
        {
            //public const string RE_ENTRY_MSG1 = "区分１を再入力してください。\n入力が終わったらもう一度再入力ボタンを押してください。";
            //public const string RE_ENTRY_MSG2 = "区分２を再入力してください。\n入力が終わったらもう一度再入力ボタンを押してください。";
            public const string RE_ENTRY_MSG1 = "１人目を再入力してください。\n入力が終わったら「確定」ボタンを押してください。";
            public const string RE_ENTRY_MSG2 = "２人目を再入力してください。\n入力が終わったら「確定」ボタンを押してください。";
            public const string RE_ENTRY_ERR_MSG = "入力した内容が一致しません。";
        }
        
        /// <summary>
        /// カナ入力文字
        /// </summary>
		public const string KANA_PATTERN = @"^[\u30A1-\u30F4\u30FC）（]*$";

        /// <summary>
        /// タイトル背景色
        /// </summary>
        public struct TITLE_BACK_COLOR
        {
            /// <summary>
            /// 本番環境
            /// </summary>
            public static readonly System.Drawing.Color HNBN = Color.FromArgb(20, 50, 170);

            /// <summary>
            /// テスト環境
            /// </summary>
            public static readonly System.Drawing.Color TEST = Color.FromArgb(220, 20, 60); 
        }

        #region 返却コード
        /// <summary>
        /// 返却コード
        /// </summary>
        public enum RetCode
        {
            /// <summary>返却コード:0 正常終了</summary>
            OK = 0,
            /// <summary>返却コード:10 異常終了</summary>
            ABEND = 10,
            /// <summary>返却コード:2 パラメータエラー</summary>
            PARAMERR = 2,
            /// <summary>返却コード:3 警告</summary>
            WARNNING = 3,
            /// <summary>返却コード:1 除外有り</summary>
            OMIT = 1
        }

        #endregion

        public struct TEXT_BOX_BACK_COLOR
        {
            /// <summary>
            /// 半角項目背景色（水色）
            /// </summary>
            public static readonly System.Drawing.Color TergetItemBackColorHalf = Color.FromArgb(210, 255, 255);

            /// <summary>
            /// 全角項目背景色（ピンク）
            /// </summary>
            public static readonly System.Drawing.Color TergetItemBackColorFull = Color.FromArgb(255, 210, 255);

            /// <summary>
            /// 全半角混在項目背景色（ピンク）
            /// </summary>
            public static readonly System.Drawing.Color TergetItemBackColorMulti = Color.FromArgb(255, 255, 210);
        }

        /// <summary>
        /// 状態区分(入力状態)
        /// </summary>
        public struct InputMode
        {
            /// <summary>
            /// 半角数値・アルファベット
            /// </summary>
            public const string AlphabetNumeric = "X";

            /// <summary>
            /// 半角カナ
            /// </summary>
            public const string KanaHalf = "KH";

            /// <summary>
            /// 全角カナ
            /// </summary>
            public const string KanaFull = "KF";

            /// <summary>
            /// 全角フリー
            /// </summary>
            public const string Hiragana = "H";
            public const string Full = "N";

            /// <summary>
            /// 半角フリー
            /// </summary>
            public const string AllHalf = "AH";

            /// <summary>
            /// 全半角混在
            /// </summary>
            public const string MixFull = "M";
        }

        /// <summary>
        /// 業務ID
        /// </summary>
        public struct BusinessID
        {
            /// <summary>
            /// DNP FATF
            /// </summary>
            public const string DNP_FATF = "162867_001";

            /// <summary>
            /// りそな銀行 FATF
            /// </summary>
            public const string RBF = "323316_001";

            /// <summary>
            /// 静岡銀行 在留カード
            /// </summary>
            public const string SBZ = "293947_021";

            /// <summary>
            /// 中部電力 クレジットカード
            /// </summary>
            public const string CDC = "466137_001";

            /// <summary>
            /// メルペイ　再審査
            /// </summary>
            public const string MRR = "915958_001";

            /// <summary>
            /// 第一フロンティア生命　解約請求
            /// </summary>
            public const string DFK = "408107_013";

            /// <summary>
            /// 明治安田生命
            /// </summary>
            public const string MYD = "915221_026";

            /// <summary>
            /// 早稲田大学
            /// </summary>
            public const string NTO = "996529_003";
        }

        /// <summary>
        /// 932 エンコード
        /// </summary>
        public static System.Text.Encoding EncShift_JIS = System.Text.Encoding.GetEncoding("Shift_JIS");

        public static System.Text.Encoding Enciso2022jp = System.Text.Encoding.GetEncoding("iso-2022-jp");

        /// <summary>
        /// UTF8（BOM無し）
        /// </summary>
        public static System.Text.Encoding EncUTF8N = new System.Text.UTF8Encoding(false);

        /// <summary>
        /// UTF8
        /// </summary>
        public static System.Text.Encoding EncUTF8  = new System.Text.UTF8Encoding(true);

        /// <summary>
        /// エントリ方法
        /// </summary>
        public struct EntryMethod
        {
            /// <summary>
            /// イメージ
            /// </summary>
            public const string IMAGE = "1";

            /// <summary>
            /// 現物
            /// </summary>
            public const string PAPER = "2";
        }

        /// <summary>
        /// OCR最大項目数
        /// </summary>
        public static readonly int iOcrMaxItemCount = 999;

        public static readonly string sSysDateTime = System.DateTime.Now.ToString("yyyyMMddHHmmss");

        public static readonly string sSysDate = System.DateTime.Now.ToString("yyyyMMdd");

        /// <summary>
        /// エンコード
        /// </summary>
        public struct Encode
        {
            /// <summary>
            /// ShiftJis
            /// </summary>
            public const string SJIS = "0";

            /// <summary>
            /// JIS
            /// </summary>
            public const string JIS = "1";

            /// <summary>
            /// JIS拡張文字含む
            /// </summary>
            public const string JIS_EX = "2";

            /// <summary>
            /// UTF8
            /// </summary>
            public const string UTF8 = "3";
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly string DEFAULT_USER_ID = "###";

        /// <summary>
        /// ユニットリストタイプ
        /// </summary>
        public struct UnitListType
        {
            /// <summary>
            /// エントリ
            /// </summary>
            public const string Entry = "1";

            /// <summary>
            /// 修正
            /// </summary>
            public const string Modify = "2";

            /// <summary>
            /// 検証
            /// </summary>
            public const string Verify = "3";
        }

        public readonly string[] kana_Half =  { "ｧ", "ｨ", "ｩ", "ｪ", "ｫ", "ｯ", "ｬ", "ｭ", "ｮ" };

        public readonly string[] KANA_Half =  { "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ﾂ", "ﾔ", "ﾕ", "ﾖ" };

        public readonly string[] kana_Full =  { "ァ", "ィ", "ゥ", "ェ", "ォ", "ッ", "ャ", "ュ", "ョ" };

        public readonly string[] KANA_Full =  { "ア", "イ", "ウ", "エ", "オ", "ツ", "ヤ", "ユ", "ヨ" };

    }
}
