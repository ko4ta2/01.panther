using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BPOEntry.EntryForms
{
    [Serializable]
    public class RegionInfo
    {
        /// <summary>
        /// 原点(X)
        /// </summary>
        [XmlAttribute]
        public int OriginX { get; set; }

        /// <summary>
        /// Y座標
        /// </summary>
        [XmlAttribute]
        public int OriginY { get; set; }

        /// <summary>
        /// この領域に含める項目の名称
        /// </summary>
        [XmlElementAttribute("Name")]
        public List<string> IncludeNames { get; set; }
    }
}
