using Common;
using System;

namespace BPOEntry.Tables
{
    /// <summary>
    /// イメージ情報
    /// </summary>
    public class D_IMAGE_INFO
    {
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
        /// イメージ連番
        /// </summary>
        public int? IMAGE_SEQ;

        /// <summary>
        /// イメージパス
        /// </summary>
        public string IMAGE_PATH;

        /// <summary>
        /// ＯＣＲ連携イメージファイル名
        /// </summary>
        public string OCR_IMAGE_FILE_NAME;

        /// <summary>
        /// ダミーイメージフラグ
        /// </summary>
        public string DUMMY_IMAGE_FLAG;

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

        public string TKSK_CD;
        public string HNM_CD;

        public string ENTRY_UNIT_ID;

        ///// <summary>
        ///// コンストラクタ
        ///// </summary>
        //public D_IMAGE_INFO()
        //{
        //    TKSK_CD = Config.TokuisakiCode;
        //    HNM_CD = Config.HinmeiCode;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ENTRY_UNIT_ID"></param>
        /// <param name="IMAGE_SEQ"></param>
        public D_IMAGE_INFO(string ENTRY_UNIT_ID, int IMAGE_SEQ)
        {
            this.ENTRY_UNIT_ID = ENTRY_UNIT_ID;
            this.IMAGE_SEQ = IMAGE_SEQ;

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
