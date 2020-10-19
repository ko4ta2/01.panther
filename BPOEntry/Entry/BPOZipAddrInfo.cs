using System.Collections.Generic;
using System.Linq;

namespace BPOEntry.EntryForms
{
    /// <summary>
    /// 郵便番号に関する住所情報
    /// </summary>
    public class BPOZipAddrInfo
    {
        /// <summary>
        /// 住所情報の入力項目
        /// </summary>
        public CTextBox.CTextBox AddrItem { get; set; }

        /// <summary>
        /// カナ住所
        /// </summary>
        public bool IsKanaAddr { get; set; }

        /// <summary>
        /// 住所の設定値
        /// </summary>
        public List<string> AddrValues { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isKana"></param>
        /// <param name="sValues"></param>
        public BPOZipAddrInfo(CTextBox.CTextBox item, bool isKana, params string[] sValues)
        {
            this.AddrItem = item;
            this.IsKanaAddr = isKana;
            this.AddrValues = sValues.ToList();
        }
    }
}
