using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BPOEntry.EntryForms
{
    /// <summary>
    /// 入力項目用の拡張プロパティ
    /// </summary>
    public class EntryItemExtensionProperties
    {
        /// <summary>
        /// 入力属性
        /// N:全角文字(2byte)
        /// X:半角文字(1byte)
        /// 9:数字    (1byte)
        /// </summary>
        public string InputElement { get; set; }

        /// <summary>
        /// 最大入力文字数
        /// </summary>
        public int InputMaxLength { get; set; }

        /// <summary>
        /// 必須項目
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 2回目入力
        /// </summary>
        public bool Input2Flag { get; set; }

        /// <summary>
        /// 入力可能なキーのパターン(入力前に規制)
        /// </summary>
        public string AcceptKeyCharsPattern { get; set; }

        /// <summary>
        /// 入力値の妥当性検証(入力後の検証)
        /// </summary>
        public string ValidStringPattern { get; set; }
        
        /// <summary>
        /// ヒント
        /// </summary>
        public string Tips { get; set; }

        /// <summary>
        /// 項目名
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EntryItemExtensionProperties(string element, int maxLength, bool required = false,bool bInp2Flag=true)
        {
            this.InputElement = element;
            this.InputMaxLength = maxLength;
            this.IsRequired = required;
            this.Input2Flag = bInp2Flag;
        }
    }
}
