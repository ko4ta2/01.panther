using System.Windows.Forms;

namespace BPOEntry.EntryForms
{
    /// <summary>
    /// 入力項目用の拡張プロパティ
    /// </summary>
    public class BPOEntryItemExtensionProperties
    {
        /// <summary>
        /// 入力属性
        /// N:全角文字(2byte)
        /// X:半角文字(1byte)
        /// 9:数字    (1byte)
        /// </summary>
        //public string InputMode { get; set; }

        /// <summary>
        /// 最大入力文字数
        /// </summary>
        //public int InputMaxLength { get; set; }

        /// <summary>
        /// 必須項目
        /// </summary>
        //public bool IsRequired { get; set; }

        /// <summary>
        /// 入力可能なキーのパターン(入力前に規制)
        /// </summary>
        //public string AcceptKeyCharsPattern { get; set; }

        /// <summary>
        /// 入力値の妥当性検証(入力後の検証)
        /// </summary>
        //public string ValidPattern { get; set; }
        
        /// <summary>
        /// ヒント
        /// </summary>
        //public string Tips { get; set; }

        /// <summary>
        /// 項目名
        /// </summary>
        //public string ItemName { get; set; }

        /// <summary>
        /// ２人目入力フラグ
        /// </summary>
        //public bool IsInput2 { get; set; }

        //public CharacterCasing CharacterCasing { get; set; }
        /// <summary>
        /// 2018/04 add 長さを厳密にチェックするか（MaxLength未満は×） 
        /// </summary>
        //public bool FullLength { get; set; }

        ///// <summary>
        ///// コンストラクタ
        ///// </summary>
        //public BPOEntryItemExtensionProperties(string sInputMode, int iMaxLength, bool bRequired = false, bool bInput2 = true, bool bFullLength = false)
        //{
        //    this.InputMode = sInputMode;
        //    this.InputMaxLength = iMaxLength;
        //    this.IsRequired = bRequired;
        //    this.IsInput2 = bInput2;
        //    this.FullLength = bFullLength;
        //}
    }
}
