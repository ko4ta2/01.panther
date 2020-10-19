using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BPOEntry.EntryForms
{
    /// <summary>
    /// 入力項目の領域分けを行う
    /// </summary>
    [Serializable]
    public class EntryItemRegions
    {
        #region インデクサ
        /// <summary>
        /// インデクサ
        /// </summary>
        public RegionInfo this[int index]
        {
            get
            {
                return this.Regions[index];
            }
            set
            {
                this.Regions[index] = value;
            }
        }
        #endregion

        /// <summary>
        /// 領域
        /// </summary>
        [XmlElementAttribute("Region")]
        public List<RegionInfo> Regions { get; set; }

        /// <summary>
        /// 対象の項目を含む領域情報を取得します。
        /// 領域情報が見つからない場合は null を返します。
        /// </summary>
        /// <param name="name">項目名</param>
        /// <returns>領域情報</returns>
        public RegionInfo Find(string name)
        {
            foreach (var info in this.Regions)
            {
                if (info.IncludeNames.Contains(name))
                    return info;
            }
            return null;
        }
    }
}
