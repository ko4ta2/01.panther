using Common;
using System;

namespace BPOEntry.Tables
{
    public class D_ENTRY
    {
        /// <summary>
        /// 得意先コード
        /// </summary>
        public string TKSK_CD { get; private set; }

        /// <summary>
        /// 品名コード
        /// </summary>
        public string HNM_CD { get; private set; }

        /// <summary>
        /// イメージ連携年月日
        /// </summary>
        public string IMAGE_CAPTURE_DATE { get; private set; }

        /// <summary>
        /// イメージ連携回数
        /// </summary>
        public string IMAGE_CAPTURE_NUM { get; private set; }

        /// <summary>
        ///  帳票ID
        /// </summary>
        public string DOC_ID { get; private set; }

        /// <summary>
        /// エントリ単位
        /// </summary>
        public string ENTRY_UNIT { get; private set; }

        /// <summary>
        /// イメージ連番
        /// </summary>
        public int IMAGE_SEQ;

        /// <summary>
        /// レコード区分
        /// </summary>
        public string RECORD_KBN;

        /// <summary>
        /// 項目ID
        /// </summary>
        public string ITEM_ID;

        /// <summary>
        /// 値
        /// </summary>
        public string VALUE;

        /// <summary>
        /// OCR値
        /// </summary>
        public string OCR_VALUE;

        /// <summary>
        /// 差異有無フラグ
        /// </summary>
        public string DIFF_FLAG;

        /// <summary>
        /// ダミー項目フラグ
        /// </summary>
        public string DUMMY_ITEM_FLAG;

        /// <summary>
        /// 登録ユーザID
        /// </summary>
        public string INS_USER_ID;

        // 登録日時
        public DateTime? INS_DATE;

        // 更新ユーザID
        public string UPD_USER_ID;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime? UPD_DATE;

        /// <summary>
        /// エントリ単位ID
        /// </summary>
        public string ENTRY_UNIT_ID;

        public string NO_ENTRY_FLAG;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        //public D_ENTRY()
        //{
        //    TKSK_CD = Config.TokuisakiCode;
        //    HNM_CD = Config.HinmeiCode;
        //}

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sImageCaptureDate"></param>
        /// <param name="sImageCaptureNum"></param>
        /// <param name="sDocId"></param>
        /// <param name="sEntryUnit"></param>
        /// <param name="iImageSeq"></param>
        /// <param name="sRecordKbn"></param>
        /// <param name="sItemId"></param>
        public D_ENTRY(string ENTRY_UNIT_ID, int IMAGE_SEQ, string RECORD_KBN, string ITEM_ID)
        {
            this.ENTRY_UNIT_ID = ENTRY_UNIT_ID;
            this.IMAGE_SEQ = IMAGE_SEQ;
            this.RECORD_KBN = RECORD_KBN;
            this.ITEM_ID = ITEM_ID;

            var x = ENTRY_UNIT_ID.Split('_');
            this.TKSK_CD = x[0];
            this.HNM_CD = x[1];
            this.IMAGE_CAPTURE_DATE = x[2];
            this.IMAGE_CAPTURE_NUM = x[3];
            this.DOC_ID = x[4];
            this.ENTRY_UNIT = x[5];
        }
    }
}
