using BPOEntry.Tables;
using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace DllCompareEntryUnit
{
    public class DllCompareEntryUnit
    {
        /// <summary>
        /// データアクセス
        /// </summary>
        private static DaoDllCompareEntryUnit dao = new DaoDllCompareEntryUnit();

        #region メイン処理
        /// <summary>
        /// メイン処理
        /// </summary>
        /// <returns></returns>
        public static int BL_Main(string ENTRY_UNIT_ID, string USER_ID)
        {
            Config.GetConfig(ConfigurationManager.AppSettings["config"]);
            GVal.UserId = USER_ID;

            bool diff_flg = false;

            int ImageCount = dao.SELECT_D_IMAGE_INFO(ENTRY_UNIT_ID);

            //// 項目分解
            //// 連携年月日
            //var IMAGE_CAPTURE_DATE = ENTRY_UNIT_ID.Split('_')[2];
            //// 連携回数
            //var IMAGE_CAPTURE_NUM = ENTRY_UNIT_ID.Split('_')[3];
            //// 帳票ID
            //var DOC_ID = ENTRY_UNIT_ID.Split('_')[4];
            //// エントリ単位
            //var ENTRY_UNIT = ENTRY_UNIT_ID.Split('_')[5];

            for (int ImageSeq = 1; ImageSeq <= ImageCount; ImageSeq++)
            {
                // ユーザ１、２の入力データをそれぞれ取得して格納する
                var entryData1 = dao.SELECT_D_ENTRY_COMPARE(ENTRY_UNIT_ID, Consts.RecordKbn.Entry_1st, ImageSeq);
                var entryData_Inp1 = GetEntryDic(entryData1);
                var entryData_OCR = GetEntryDicOCR(entryData1);
                var DummyItemFlag = GetDummyItemFlag(entryData1);

                var entryData2 = dao.SELECT_D_ENTRY_COMPARE(ENTRY_UNIT_ID, Consts.RecordKbn.Entry_2nd, ImageSeq);
                var entryData_Inp2 = GetEntryDic(entryData2);

                // 差異を検出して、ユーザ区分が0のデータを登録する
                var iItemIdx = 0;
                var entry_List = new List<D_ENTRY>();
                foreach (var entry_item in entryData_Inp1.Keys)
                {
                    var entry_record = new D_ENTRY(ENTRY_UNIT_ID
                                                  , ImageSeq
                                                  , Consts.RecordKbn.ADMIN
                                                  , entry_item);

                    // 入力１と２の入力値が同じかを判断する
                    if (entryData_Inp1[entry_item] == entryData_Inp2[entry_item])
                    {
                        entry_record.VALUE = entryData_Inp1[entry_item];
                        entry_record.DIFF_FLAG = Consts.Flag.OFF;
                    }
                    else
                    {
                        entry_record.VALUE = string.Empty;
                        entry_record.DIFF_FLAG = Consts.Flag.ON;
                        diff_flg = true;
                    }

                    //// ２人目入力無しの場合、強制的に不一致にする
                    //if (!_tbs[iItemIdx].Input2)
                    //{
                    //    entry_record.VALUE = string.Empty;
                    //    entry_record.DIFF_FLAG = Consts.DiffFlag.ON;
                    //    diff_flg = true;
                    //}

                    // OCR値
                    entry_record.OCR_VALUE = entryData_OCR[entry_item];

                    entry_record.DUMMY_ITEM_FLAG = DummyItemFlag[entry_item];

                    // 登録ユーザID
                    entry_record.INS_USER_ID = GVal.UserId;
                    entry_List.Add(entry_record);

                    iItemIdx++;
                }

                // ユーザ区分が0のデータを登録する
                dao.INSERT_D_ENTRY_ADMIN(entry_List);
            }

            // 差異があれば入力単位データの状態区分を管理者修正中("6")へ更新
            // 差異がなければ入力単位データの状態区分をコンペア済("7")へ更新
            //string unitStatus = string.Empty;
            //if (diff_flg)
            //{
            //    unitStatus = Consts.EntryUnitStatus.COMPARE_EDT;
            //}
            //else
            //{
            //    unitStatus = Consts.EntryUnitStatus.COMPARE_END;
            //}
            if (dao.UPDATE_D_ENTRY_UNIT(ENTRY_UNIT_ID, diff_flg ? Consts.EntryUnitStatus.COMPARE_EDT : Consts.EntryUnitStatus.COMPARE_END) != 1)
            {
                throw new ApplicationException(String.Format("D_ENTRY_UNIT UPDATEで不整合発生:{0}", ENTRY_UNIT_ID));
            }

            // 正常終了
            return (int)Consts.RetCode.OK;
        }
        #endregion

        /// <summary>
        /// エントリ項目取得
        /// </summary>
        /// <param name="entryData"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetEntryDic(DataTable entryData)
        {
            var entryDataDic = new Dictionary<string, string>();
            foreach (var row in entryData.AsEnumerable())
            {
                if (!entryDataDic.Keys.Contains(row["ITEM_ID"].ToString().Trim()))
                {
                    entryDataDic.Add(row["ITEM_ID"].ToString().Trim(), row["VALUE"].ToString().Trim());
                }
            }
            return entryDataDic;
        }

        /// <summary>
        /// OCR結果取得
        /// </summary>
        /// <param name="entryData"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetEntryDicOCR(DataTable entryData)
        {
            var entryDataDic = new Dictionary<string, string>();
            foreach (var row in entryData.AsEnumerable())
            {
                if (!entryDataDic.Keys.Contains(row["ITEM_ID"].ToString().Trim()))
                {
                    entryDataDic.Add(row["ITEM_ID"].ToString().Trim(), row["OCR_VALUE"].ToString().Trim());
                }
            }
            return entryDataDic;
        }

        /// <summary>
        /// ダミー項目フラグ取得
        /// </summary>
        /// <param name="entryData"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetDummyItemFlag(DataTable entryData)
        {
            var entryDataDic = new Dictionary<string, string>();
            foreach (var row in entryData.AsEnumerable())
            {
                if (!entryDataDic.Keys.Contains(row["ITEM_ID"].ToString().Trim()))
                {
                    entryDataDic.Add(row["ITEM_ID"].ToString().Trim(), row["DUMMY_ITEM_FLAG"].ToString().Trim());
                }
            }
            return entryDataDic;
        }
    }
}
