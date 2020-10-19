using Common;
using System;

namespace BPOEntry.Tables
{
    /// <summary>
    /// エントリステータス
    /// </summary>
    public class D_ENTRY_STATUS
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
        /// 帳票ID
        /// </summary>
        public string DOC_ID { get; private set; }

        /// <summary>
        /// エントリ単位
        /// </summary>
        public string ENTRY_UNIT { get; private set; }

        /// <summary>
        /// レコード区分
        /// </summary>
        public string RECORD_KBN;

        /// <summary>
        /// 登録ユーザ担当者ID
        /// </summary>
        public string ENTRY_USER_ID;

        /// <summary>
        /// 入力状態区分
        /// </summary>
        public string ENTRY_STATUS;

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
        /// エントリ単位ID
        /// </summary>
        public string ENTRY_UNIT_ID;
        //{
        //    get
        //    {
        //        return String.Join("_", TKSK_CD
        //                              , HNM_CD
        //                              , IMAGE_CAPTURE_DATE
        //                              , IMAGE_CAPTURE_NUM
        //                              , DOC_ID
        //                              , ENTRY_UNIT);
        //    }
        //    set { }
        //}

        /// <summary>
        /// コンストラクタ
        /// </summary>
        //public D_ENTRY_STATUS()
        //{
        //    TKSK_CD = Config.TokuisakiCode;
        //    HNM_CD = Config.HinmeiCode;
        //}

        
        public D_ENTRY_STATUS(string ENTRY_UNIT_ID, string RECORD_KBN)
        {
            this.ENTRY_UNIT_ID = ENTRY_UNIT_ID;
            this.RECORD_KBN = RECORD_KBN;

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
