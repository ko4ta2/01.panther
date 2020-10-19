using System;
using Common;

namespace BPOEntry.Tables
{
    /// <summary>
    /// エントリ単位
    /// </summary>
    public class D_ENTRY_UNIT
    {
        /// <summary>
        /// 得意先コード
        /// </summary>
        public string TKSK_CD;

        /// <summary>
        /// 品名コード
        /// </summary>
        public string HNM_CD;

        /// <summary>
        /// イメージ連携年月日
        /// </summary>
        public string IMAGE_CAPTURE_DATE;

        /// <summary>
        /// イメージ連携回数
        /// </summary>
        public string IMAGE_CAPTURE_NUM;

        /// <summary>
        ///  帳票ID
        /// </summary>
        public string DOC_ID;

        /// <summary>
        /// エントリ単位
        /// </summary>
        public string ENTRY_UNIT;

        /// <summary>
        /// エントリ単位状況
        /// </summary>
        public string ENTRY_UNIT_STATUS { set; get; } = Consts.EntryUnitStatus.ENTRY_NOT;

        /// <summary>
        /// 登録ユーザID
        /// </summary>
        public string INS_USER_ID;

        /// <summary>
        /// 登録日時
        /// </summary>
        public DateTime? INS_DATE;

        /// <summary>
        /// 更新ユーザID
        /// </summary>
        public string UPD_USER_ID;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime? UPD_DATE;

        /// <summary>
        /// レコード区分（テーブルには存在しない）
        /// </summary>
        public string RECORD_KBN;

        /// <summary>
        /// エントリ単位ID
        /// </summary>
        public string ENTRY_UNIT_ID { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string BATCH_EXPORT_FLAG { set; get; } = Consts.Flag.OFF;

        public string EXPORT_METHOD { set; get; }
        public string USER_ID { set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ENTRY_UNIT_ID"></param>
        /// <param name="RECORD_KBN"></param>
        public D_ENTRY_UNIT(string ENTRY_UNIT_ID, string RECORD_KBN = null)
        {
            this.ENTRY_UNIT_ID = ENTRY_UNIT_ID;

            var x = ENTRY_UNIT_ID.Split('_');
            this.TKSK_CD = x[0];
            this.HNM_CD = x[1];
            this.IMAGE_CAPTURE_DATE = x[2];
            this.IMAGE_CAPTURE_NUM = x[3];
            this.DOC_ID = x[4];
            this.ENTRY_UNIT = x[5];
            this.RECORD_KBN = RECORD_KBN;
        }
    }
}
