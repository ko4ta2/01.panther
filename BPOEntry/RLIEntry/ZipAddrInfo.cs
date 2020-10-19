using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BPOEntry.EntryForms
{
    /// <summary>
    /// 郵便番号に関する住所情報
    /// </summary>
    public class ZipAddrInfo
    {
        /// <summary>
        /// 住所情報の入力項目
        /// </summary>
        public TextBox AddrItem { get; set; }

        /// <summary>
        /// カナ住所
        /// </summary>
        public bool IsKanaAddr { get; set; }

        /// <summary>
        /// 住所の設定値
        /// </summary>
        public List<string> AddrValues { get; set; }

        public ZipAddrInfo(TextBox item, bool isKana, params string[] values)
        {
            this.AddrItem = item;
            this.IsKanaAddr = isKana;
            this.AddrValues = values.ToList();
        }
    }
}
