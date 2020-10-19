﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BPOEntry.EntryForms
{
      /// <summary>
    /// 郵便番号に関する住所情報
    /// </summary>
    public class HandlerInfo
    {
        /// <summary>
        /// 住所情報の入力項目
        /// </summary>
        public TextBox Item { get; set; }

        /// <summary>
        /// カナ住所
        /// </summary>
        public string sItem { get; set; }

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
        public HandlerInfo(TextBox tbItem, string sItem, params string[] sValues)
        {
            this.Item = tbItem;
            this.sItem = sItem;
            this.AddrValues = sValues.ToList();
        }
    }
}
