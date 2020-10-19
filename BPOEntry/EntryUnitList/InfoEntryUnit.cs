using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPOEntry.ReEntrySelect
{
    public class InfoEntryUnit
    {
        public string DOC_ID { get; set; }

        public string DOC_NAME { get; set; }

        public string IMAGE_CAPTURE_DATE { get; set; }

        public string IMAGE_CAPTURE_NUM { get; set; }

        public string ENTRY_UNIT { get; set; }

        public override string ToString()
        {
            return string.Format("{0, -30} {1} {2} {3}"
                , this.DOC_NAME
                , this.IMAGE_CAPTURE_DATE
                , this.IMAGE_CAPTURE_NUM
                , this.ENTRY_UNIT
            );
        }
    }
}
