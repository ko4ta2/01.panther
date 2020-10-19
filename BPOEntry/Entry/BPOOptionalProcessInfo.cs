using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace BPOEntry.EntryForms
{

    #region 特殊処理情報
    //======================================================================================
    /// <summary>
    /// 特殊処理情報
    /// </summary>
    //======================================================================================
    public class OptionalProcessInfo
    {
        /// <summary>
        /// この名前のメソッドを呼び出す
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// 処理名日本語
        /// </summary>
        public string MethodName_JPN { get; private set; }

        /// <summary>
        /// コード定義セクション
        /// </summary>
        public string CodeDefineSection { get; private set; }

        /// <summary>
        /// 強制置換
        /// </summary>
        public string ForcedSubstitution { get; private set; }

        /// <summary>
        /// メソッドに渡すパラメータ順　なくても可
        /// </summary>
        private List<int> ParameterOrder { get; set; }

        /// <summary>
        /// 使用する項目情報
        /// </summary>
        public List<OpInfoItem> Items { get; private set; }

        /// <summary>
        /// メソッドのパラメータ数＞ファイルで指定したアイテム数となる場合、メソッドのパラメータ数が指定されている
        /// </summary>
        public int? specparamCnt { get; private set; }

        #region old
        ///// <summary>
        ///// オプション処理内容設定
        ///// </summary>
        ///// <param name="tbs">画面上のテキストボックスのリスト</param>
        ///// <param name="methodname">呼び出すメソッド名</param>
        ///// <param name="methodname_jpn">処理名称</param>
        ///// <param name="paramorder">メソッドに渡すパラメータ順</param>
        ///// <param name="items">項目のリスト</param>
        ///// <param name="outInfo">出力フラグのリスト</param>
        ///// <param name="IsColName">OUT項目に列名が指定されているか</param>
        ///// <param name="specifiedparamcnt">指定パラメータ数　なければ空のアイテムを設定</param>
        ///// <param name="dicExtProps">テキストボックスの拡張プロパティ</param>
        //public OptionalProcessInfo(List<CTextBox.CTextBox> tbs,
        //                            string methodname,
        //                            string methodname_jpn,
        //                            string paramorder,
        //                            List<string> items,
        //                            List<string> outInfo,
        //                            string IsColName,
        //                            string specifiedparamcnt,
        //                            Dictionary<CTextBox.CTextBox, BPOEntryItemExtensionProperties> dicExtProps
        //                           )
        //{

        //    MethodName = methodname;
        //    MethodName_JPN = methodname_jpn;
        //    specparamCnt = ConvToNullable<int>(specifiedparamcnt);

        //    parameterOrder = new List<int>();
        //    foreach (char c in paramorder)
        //    {
        //        //数字以外が一つでもあれば採用しない
        //        if (!Char.IsDigit(c))
        //            break;
        //        parameterOrder.Add(int.Parse(c.ToString()));
        //    }
        //    int validcnt = items.Where(i => tbs.Any(s => s.ItemName == i)).Count();


        //    #region del
        //    //パラメータ順は、有効なアイテム数以下の数字が一回ずつ出現していなければ無効とする(チェック済）
        //    //bool auto_order = false;
        //    //if (parameterOrder.Count == validcnt)
        //    //{
        //    //    foreach (int i in Enumerable.Range(1, validcnt))
        //    //    {
        //    //        if (parameterOrder.Count(o => o == i) != 1)
        //    //        {
        //    //            auto_order = true;
        //    //            break;
        //    //        }
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    auto_order = true;
        //    //}
        //    #endregion

        //    bool auto_order = parameterOrder.Count != validcnt;

        //    //パラメータ順の自動設定
        //    if (auto_order)
        //    {
        //        parameterOrder.Clear();
        //        foreach (int i in Enumerable.Range(1, validcnt))
        //        {
        //            parameterOrder.Add(i);
        //        }

        //    }


        //    Items = new List<OpInfoItem>();
        //    int realorder = 0;  //有効な項目順
        //    foreach (int i in Enumerable.Range(0, items.Count))
        //    {
        //        if (String.IsNullOrWhiteSpace(items[i]))
        //            continue;
        //        if (!tbs.Any(t => t.ItemName == items[i]))
        //            continue;

        //        CTextBox.CTextBox tb = tbs.Find(t => t.ItemName == items[i]);

        //        bool outflg;
        //        List<string> outcolname = new List<string>();
        //        if (String.IsNullOrEmpty(IsColName))
        //            outflg = outInfo.Count > i && outInfo[i] == "1" ? true : false;
        //        else
        //        {
        //            outflg = outInfo.Count > i && !String.IsNullOrWhiteSpace(outInfo[i]) ? true : false;
        //            outcolname = outInfo[i].Split(',').ToList();

        //        }



        //        realorder++;
        //        int order = parameterOrder.FindIndex(o => o == realorder) + 1;
        //        Items.Add(new OpInfoItem(items[i], tb, outflg,
        //            order,
        //            outcolname,
        //            dicExtProps.Any(d => d.Key == tb) ? dicExtProps[tb] : null));
        //    }

        //    //指定パラメータ数に満たない場合、空の要素を加える
        //    if (specparamCnt == null || (int)specparamCnt == validcnt)
        //        return;
        //    foreach (int i in Enumerable.Range(0, (int)specparamCnt - validcnt))
        //    {

        //        CTextBox.CTextBox tb = null;

        //        bool outflg = false;
        //        List<string> outcolname = new List<string>();



        //        Items.Add(new OpInfoItem(string.Empty, tb, outflg,
        //            realorder++,
        //            outcolname,
        //            null));
        //    }





        //}





        ///// <summary>
        /////ファイルの設定内容が妥当か検証
        ///// </summary>
        ///// <param name="tbs">画面上のテキストボックスのリスト</param>
        ///// <param name="methodname">呼び出すメソッド名</param>
        ///// <param name="methodname_jpn">処理名称</param>
        ///// <param name="paramorder">メソッドに渡すパラメータ順</param>
        ///// <param name="triggername">処理を呼ぶトリガー項目名</param>
        ///// <param name="specparamcnt">指定パラメータ数　足りなければ空項目追加</param>
        ///// <param name="iscolname">カラム名指定しているかどうか</param>
        ///// <param name="items">項目のリスト</param>
        ///// <returns>ok/ng</returns>
        //public static bool Check(List<CTextBox.CTextBox> tbs,
        //                            string methodname,
        //                            string methodname_jpn,
        //                            string paramorder,
        //                            string triggername,
        //                            string specparamcnt,
        //                            string iscolname,
        //                            List<string> items
        //                            )
        //{
        //    try
        //    {
        //        //メソッド名
        //        Type OpProcessType = typeof(OptionalProcess);
        //        MethodInfo mi = OpProcessType.GetMethod(methodname);
        //        if (mi == null)
        //            return false;

        //        //コントロール名
        //        foreach (string item in items)
        //        {

        //            if (!string.IsNullOrWhiteSpace(item) && !tbs.Any(t => t.ItemName == item))
        //                return false;

        //        }


        //        //トリガー名
        //        if (string.IsNullOrWhiteSpace(triggername) || !tbs.Any(t => t.ItemName == triggername))
        //            return false;

        //        //パラメータ数
        //        int paramcnt = items.Where(i => tbs.Any(s => s.ItemName == i)).Count();
        //        ParameterInfo[] paraminfo = mi.GetParameters();

        //        if (paraminfo.Count() != paramcnt )
        //        {
        //            //パラメータ数が明示的に指定されている場合
        //            if (!string.IsNullOrEmpty(specparamcnt))
        //            {
        //                if ((int)ConvToNullable<int>(specparamcnt) != paraminfo.Count())
        //                    return false;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }

        //        //カラム名指定属性と実際の値が異なる
        //        bool atrSpecifiedColName = GetOptionMethodAttribute(mi).SpecifiedColmnName;
        //        bool specifiedColName = string.IsNullOrWhiteSpace(iscolname) ? false:true;
        //        if (atrSpecifiedColName !=specifiedColName)
        //            return false;



        //        //順番
        //        //なくても良い
        //        if (string.IsNullOrWhiteSpace(paramorder))
        //            return true;


        //        List<int> parameterOrder = new List<int>();
        //        foreach (char c in paramorder)
        //        {
        //            //数字以外が一つでもあれば無効
        //            if (!Char.IsDigit(c))
        //                return false;
        //            parameterOrder.Add(int.Parse(c.ToString()));
        //        }

        //        //パラメータ順は、有効なアイテム数以下の数字が一回ずつ出現していなければ無効とする
        //        if (parameterOrder.Count == paramcnt)
        //        {
        //            foreach (int i in Enumerable.Range(1, paramcnt))
        //            {
        //                if (parameterOrder.Count(o => o == i) != 1)
        //                {
        //                    return false;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            return false;
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //}
        #endregion

        /// <summary>
        /// 特殊処理情報の生成
        /// </summary>
        /// <param name="tbs">画面のテキストボックスのリスト</param>
        /// <param name="line">ファイルの内容</param>
        /// <param name="dicExtProps">テキストボックスの追加情報のdictionary</param>
        public OptionalProcessInfo(List<CTextBox.CTextBox> tbs
                                  , OptionalTsvFile line
                                  , Dictionary<CTextBox.CTextBox, BPOEntryItemExtensionProperties> dicExtProps)
        {
            MethodName = line.Method_Name;
            MethodName_JPN = line.Type;
            CodeDefineSection = line.CodeDefineSection;
            ForcedSubstitution = line.ForcedSubstitution;

            specparamCnt = ConvToNullable<int>(line.ParamCnt);

            ParameterOrder = new List<int>();
            foreach (char c in line.Parameter_Order)
            {
                //数字以外が一つでもあれば採用しない
                if (!Char.IsDigit(c))
                    break;
                ParameterOrder.Add(int.Parse(c.ToString()));
            }

            //コントロール名
            var items = new List<string> { line.ITEM_1, line.ITEM_2, line.ITEM_3, line.ITEM_4, line.ITEM_5, line.ITEM_6, line.ITEM_7 };
            int validcnt = items.Where(i => tbs.Any(s => s.ItemName == i)).Count();

            var outInfo = new List<string> { line.OUT_1, line.OUT_2, line.OUT_3, line.OUT_4, line.OUT_5, line.OUT_6, line.OUT_7 };
            bool auto_order = ParameterOrder.Count != validcnt;

            //パラメータ順の自動設定
            if (auto_order)
            {
                ParameterOrder.Clear();
                foreach (int i in Enumerable.Range(1, validcnt))
                {
                    ParameterOrder.Add(i);
                }
            }

            Items = new List<OpInfoItem>();
            int realorder = 0;  //有効な項目順
            foreach (int i in Enumerable.Range(0, items.Count))
            {
                if (String.IsNullOrWhiteSpace(items[i]))
                    continue;
                if (!tbs.Any(t => t.ItemName == items[i]))
                    continue;

                CTextBox.CTextBox tb = tbs.Find(t => t.ItemName == items[i]);

                bool outflg;
                List<string> outcolname = new List<string>();
                if (String.IsNullOrEmpty(line.IsColName))
                    outflg = outInfo.Count > i && outInfo[i] == Consts.Flag.ON ? true : false;
                else
                {
                    outflg = outInfo.Count > i && !String.IsNullOrWhiteSpace(outInfo[i]) ? true : false;
                    outcolname = outInfo[i].Split(',').ToList();
                }

                realorder++;
                int order = ParameterOrder.FindIndex(o => o == realorder) + 1;
                Items.Add(new OpInfoItem(items[i], tb, outflg,
                    order,
                    outcolname,
                    dicExtProps.Any(d => d.Key == tb) ? dicExtProps[tb] : null));
            }

            //指定パラメータ数に満たない場合、空の要素を加える
            if (specparamCnt == null || (int)specparamCnt == validcnt)
                return;

            foreach (int i in Enumerable.Range(0, (int)specparamCnt - validcnt))
            {
                CTextBox.CTextBox tb = null;

                bool outflg = false;
                List<string> outcolname = new List<string>();
                
                Items.Add(new OpInfoItem(string.Empty, tb, outflg,
                    realorder++,
                    outcolname,
                    null));
            }
        }

        /// <summary>
        /// ファイルの内容のチェック
        /// </summary>
        /// <param name="tbs">画面のテキストボックスのリスト</param>
        /// <param name="line">ファイルの内容</param>
        /// <returns></returns>
        public static bool Check(List<CTextBox.CTextBox> tbs,                            OptionalTsvFile line                            )
        {
            try
            {
                //メソッド名
                Type OpProcessType = typeof(OptionalProcess);
                MethodInfo mi = OpProcessType.GetMethod(line.Method_Name);
                if (mi == null)
                    return false;

                //コントロール名
                var items = new List<string> { line.ITEM_1, line.ITEM_2, line.ITEM_3, line.ITEM_4, line.ITEM_5, line.ITEM_6, line.ITEM_7};
                if (items.Any(item => !string.IsNullOrWhiteSpace(item) && !tbs.Any(t => t.ItemName == item)))
                    return false;

                //トリガー名
                if (string.IsNullOrWhiteSpace(line.Trigger_Item) || !tbs.Any(t => t.ItemName == line.Trigger_Item))
                    return false;

                //パラメータ数
                int paramcnt = items.Where(i => tbs.Any(s => s.ItemName == i)).Count();
                ParameterInfo[] paraminfo = mi.GetParameters();

                if (paraminfo.Count() != paramcnt)
                {
                    //パラメータ数が明示的に指定されている場合
                    if (!string.IsNullOrEmpty(line.ParamCnt))
                    {
                        //if ((int)ConvToNullable<int>(line.ParamCnt) != paraminfo.Count() - 2)
                        if ((int)ConvToNullable<int>(line.ParamCnt) != paraminfo.Count() )
                                return false;
                    }
                    else
                    {
                        return false;
                    }
                }

                //カラム名指定属性と実際の値が異なる
                bool atrSpecifiedColName = GetOptionMethodAttribute(mi).SpecifiedColmnName;
                bool specifiedColName = string.IsNullOrWhiteSpace(line.IsColName) ? false : true;
                if (atrSpecifiedColName != specifiedColName)
                    return false;

                //順番
                //なくても良い
                if (String.IsNullOrWhiteSpace(line.Parameter_Order))
                    return true;

                var ParameterOrder = new List<int>();
                foreach (char c in line.Parameter_Order)
                {
                    //数字以外が一つでもあれば無効
                    if (!Char.IsDigit(c))
                        return false;
                    ParameterOrder.Add(int.Parse(c.ToString()));
                }

                //パラメータ順は、有効なアイテム数以下の数字が一回ずつ出現していなければ無効とする
                if (ParameterOrder.Count == paramcnt)
                {
                    foreach (int i in Enumerable.Range(1, paramcnt))
                    {
                        if (ParameterOrder.Count(o => o == i) != 1)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Nullable<T> ConvToNullable<T>(string s) where T : struct
        {
            Nullable<T> result = new Nullable<T>();
            try
            {
                if (!String.IsNullOrEmpty(s) && s.Trim().Length > 0)
                {
                    TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
                    result = (T)conv.ConvertFrom(s);
                }
            }
            catch
            {
            }
            return result;
        }

        private static OptionMethodAttribute GetOptionMethodAttribute(MemberInfo member)
        {
            return (OptionMethodAttribute)member.GetCustomAttributes(typeof(OptionMethodAttribute), true).FirstOrDefault();
        }
    }
    #endregion

    //======================================================================================
    /// <summary>
    /// オプションファイルのメソッドを実行するための個別の項目の情報
    /// </summary>
    //======================================================================================
    public class OpInfoItem
    {
        private string ItemName { get; set; }
        private bool Outflg { get; set; }
        private CTextBox.CTextBox tb { get; set; }
        public int ParameterOrder { get; set; }
        private List<string> Outcolname { get; set; }

        //private BPOEntryItemExtensionProperties extprops { get; set; }

        public OpInfoItem(string name, CTextBox.CTextBox textbox, bool flg, int order, List<string> colnames, BPOEntryItemExtensionProperties ep)
        {
            ItemName = name;
            tb = textbox;
            Outflg = flg;
            ParameterOrder = order;
            //extprops = ep;
            Outcolname = colnames;
        }

        public string GetText()
        {
            return tb.Text;
        }

        public OpArg GetArg()
        {
            return new OpArg(tb == null ? string.Empty : tb.Text,
                new List<string>(Outcolname), this.ItemName);
        }

        public void SetText(string value)
        {
            if (Outflg && tb.Enabled)
            {
                string temp = value;
                // 最大長を超えた場合、カットする
                if (tb != null && value.Length > tb.MaxLength)
                {
                    temp = temp.Substring(0, tb.MaxLength);
                }
                tb.Text = temp;
            }
        }
        public void SetText(OpArg arg)
        {
            SetText(arg.ItemValue);
        }
    }

    //======================================================================================
    /// <summary>
    /// 実際に呼び出すメソッドに渡すクラス
    /// </summary>
    //======================================================================================
    public class OpArg
    {
        /// <summary>
        /// 取得した値
        /// </summary>
        public string ItemValue { get; set; }
        /// <summary>
        /// コントロール名
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// DBカラム名
        /// </summary>
        public List<string> Colname { get; set; }
        public OpArg(string item, List<string> col, string name = null, string propname_1 = null, string propvalue_1 = null, string methodname_1 = null, object[] methodargs_1 = null)
        {
            ItemName = name;
            ItemValue = item;
            Colname = new List<string>(col);
        }
    }

    #region ファイル関連

    //======================================================================================
    /// <summary>
    /// タブ区切りファイル1行の内容がこのクラスのインスタンスとなる
    /// TsvColAttributeには列インデックスを設定
    /// ファイル構造が変わったら（列追加、移動）、ここに追加修正
    /// </summary>
    //======================================================================================
    public class OptionalTsvFile
    {
        //TYPE	METHOD_NAME	ITEM_1	ITEM_2	ITEM_3	ITEM_4	ITEM_5	ITEM_6	ITEM_7	OUT_1	OUT_2	OUT_3	OUT_4	OUT_5	OUT_6	OUT_7	TRIGGER_ITEM	PARAMETER_ORDER	IsCOLNAME	PARAMCNT

        [TsvColAttribute(0)]
        public string Type { get; set; }
        [TsvColAttribute(1)]
        public string Method_Name { get; set; }

        [TsvColAttribute(2)]
        public string ITEM_1 { get; set; }
        [TsvColAttribute(3)]
        public string ITEM_2 { get; set; }
        [TsvColAttribute(4)]
        public string ITEM_3 { get; set; }
        [TsvColAttribute(5)]
        public string ITEM_4 { get; set; }
        [TsvColAttribute(6)]
        public string ITEM_5 { get; set; }
        [TsvColAttribute(7)]
        public string ITEM_6 { get; set; }
        [TsvColAttribute(8)]
        public string ITEM_7 { get; set; }

        [TsvColAttribute(9)]
        public string OUT_1 { get; set; }
        [TsvColAttribute(10)]
        public string OUT_2 { get; set; }
        [TsvColAttribute(11)]
        public string OUT_3 { get; set; }
        [TsvColAttribute(12)]
        public string OUT_4 { get; set; }
        [TsvColAttribute(13)]
        public string OUT_5 { get; set; }
        [TsvColAttribute(14)]
        public string OUT_6 { get; set; }
        [TsvColAttribute(15)]
        public string OUT_7 { get; set; }
        
        [TsvColAttribute(16)]
        public string Trigger_Item { get; set; }
        [TsvColAttribute(17)]
        public string Parameter_Order { get; set; }
        [TsvColAttribute(18)]
        public string IsColName { get; set; }
        [TsvColAttribute(19)]
        public string ParamCnt { get; set; }

        [TsvColAttribute(20)]
        public string TargetDocId { get; set; }

        [TsvColAttribute(21)]
        public string CodeDefineSection { get; set; }

        [TsvColAttribute(22)]
        public string ForcedSubstitution { get; set; }

    }

    //======================================================================================
    /// <summary>
    /// OptionalTsvFileクラスのメンバ属性（ファイル内の列インデックス）
    /// </summary>
    //======================================================================================
    public class TsvColAttribute : Attribute
    {
        public int ColIdx;
        public TsvColAttribute(int idx)
        {
            this.ColIdx = idx;
        }

    }


    //======================================================================================
    /// <summary>
    /// タブ区切りファイルを読んで任意のクラスに値をセットするクラス
    /// </summary>
    /// <typeparam name="T">値をセットしたいクラス</typeparam>
    //======================================================================================
    public class TSVReader<T> : IEnumerable<T>, IDisposable where T : class,new()
    {
        private Dictionary<Type, TypeConverter> converters = new Dictionary<Type, TypeConverter>();

        private Dictionary<int, Action<object, string>> setters = new Dictionary<int, Action<object, string>>();


        private void LoadType()
        {
            Type type = typeof(T);

            var memberTypes = new MemberTypes[] { MemberTypes.Field, MemberTypes.Property };
            BindingFlags flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (MemberInfo member in type.GetMembers(flag).Where((member) => memberTypes.Contains(member.MemberType)))
            {
                TsvColAttribute tsvColumn = GetTsvColAttribute(member);
                if (tsvColumn == null)
                    continue;

                int columnIndex = tsvColumn.ColIdx;

                if (member.MemberType == MemberTypes.Field)
                {
                    FieldInfo fieldInfo = type.GetField(member.Name, flag);
                    setters[columnIndex] = (target, value) =>
                        fieldInfo.SetValue(target, GetConvertedValue(fieldInfo, value));

                }
                else
                {
                    PropertyInfo propertyInfo = type.GetProperty(member.Name, flag);
                    setters[columnIndex] = (target, value) =>
                        propertyInfo.SetValue(target, GetConvertedValue(propertyInfo, value), null);
                }

            }


        }


        private object GetConvertedValue(MemberInfo info, object value)
        {

            Type type = null;

            if (info is FieldInfo)
                type = (info as FieldInfo).FieldType;
            else if (info is PropertyInfo)
                type = (info as PropertyInfo).PropertyType;

            if (!converters.ContainsKey(type))
            {
                converters[type] = TypeDescriptor.GetConverter(type);
            }

            TypeConverter converter = converters[type];

            try
            {
                return converter.ConvertFrom(value);
            }
            catch /*(Exception ex)*/
            {
                return string.Empty;
            }

        }



        private StreamReader reader;
        private string filepath;
        private bool skipFirstLine;
        private Encoding encoding;

        private Microsoft.VisualBasic.FileIO.TextFieldParser parser;

        public TSVReader(string filepath)
            : this(filepath, true)
        {
        }

        public TSVReader(string filepath, bool skipFirstLine)
            : this(filepath, skipFirstLine, null)
        {
        }
        public TSVReader(string filepath, bool skipFirstLine, Encoding encoding)
        {

            this.filepath = filepath;
            this.skipFirstLine = skipFirstLine;
            this.encoding = encoding;

            //規定のエンコード
            if (this.encoding == null)
                this.encoding = System.Text.Encoding.GetEncoding("UTF-8");


            //Tの解析
            LoadType();


            parser =
                new Microsoft.VisualBasic.FileIO.TextFieldParser(filepath);
            parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
            parser.SetDelimiters("\t"); // タブ区切り(TSVファイルの場合)
            if (skipFirstLine)
                parser.ReadLine();      //最初の行をスキップ
        }


        public void Dispose()
        {
            using (reader)
            {
            }
            reader = null;
        }

        public IEnumerator<T> GetEnumerator()
        {


            // tsv読み取り
            while (!parser.EndOfData)
            {
                var data = new T();

                var line = parser.ReadFields();
                foreach (int idx in Enumerable.Range(0, line.Length))
                {
                    //列indexに対応するsetメソッドがない場合処理しない
                    if (!setters.ContainsKey(idx))
                        continue;

                    //setメソッドでdataに値を入れる
                    setters[idx](data, line[idx]);
                }
                yield return data;

            }



        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private static TsvColAttribute GetTsvColAttribute(MemberInfo member)
        {

            return (TsvColAttribute)member.GetCustomAttributes(typeof(TsvColAttribute), true).FirstOrDefault();

        }

    }
    #endregion

    //======================================================================================
    /// <summary>
    /// optionファイルに設定されたメソッドの実際の処理
    /// </summary>
    //======================================================================================
    public class OptionalProcess
    {
        //DAOを使わないなら削除=================
        private static DaoEntry _Dao { get; set; }

        public static void Dao(DaoEntry dao)
        {
            _Dao = dao;
        }
        //==============================================

        //親画面
        private static Form Parentform { get; set; }
        public static void SetParent(Form frm)
        {
            Parentform = frm;

            //キリンラガー コードと値の組み合わせ
            CodeValueList = CodeValueInfo.list;
        }

        //2018/04/09 add コードと値の組み合わせ
        public static List<CodeValue> CodeValueList { get; set; }
        public static string DocId { get; private set; }//未使用

        #region ①郵便番号→住所１をセットする処理
        /// <summary>
        /// ①郵便番号→住所１をセットする処理
        /// </summary>
        /// <param name="zipcode">郵便番号</param>
        /// <param name="adr1">住所1+2+3が設定される</param>
        /// <param name="adr2"></param>
        /// <returns></returns>
        [OptionMethodAttribute]
        public static bool GetAddressByZipCode(ref string zipcode, ref string adr1, ref string adr2)
        {
            try
            {
                //SetZipInfoと同じ処理をする
                // 郵便番号の入力値が7桁でないと無視する
                if (zipcode.Replace("-", string.Empty).Trim().Length != 7)
                    return false;

                // 住所１、住所２が一つでも空欄ではないと無視する
                if (!string.IsNullOrEmpty(adr1) || !string.IsNullOrEmpty(adr2))
                    return false;

                // 上記以外の場合、郵便番号で検索して住所を自動設定する
                DataTable dtZip = _Dao.SelectZipInfo(zipcode);
                if (dtZip == null || dtZip.Rows.Count == 0)
                    return false;


                DataRow rowZip = dtZip.Rows[0];
                //住所1部分
                adr1 = rowZip["ADDRESS_1"].ToString() + rowZip["ADDRESS_2"].ToString() + rowZip["ADDRESS_3"].ToString() + rowZip["ADDRESS_4"].ToString();

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region ②郵便番号、住所１→住所コード、住所１をセットする処理
        /// <summary>
        /// ②郵便番号、住所１→住所コード、住所１をセットする処理
        /// </summary>
        /// <param name="zipno">郵便番号</param>
        /// <param name="address1">子画面で選択した住所</param>
        /// <param name="addresscd">住所コード</param>
        /// <returns></returns>
        [OptionMethodAttribute]
        public static bool GetAddressCode(ref string zipno, ref string address1, ref string addresscd)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(zipno) && string.IsNullOrWhiteSpace(address1))
                {
                    address1 = string.Empty;
                    addresscd = string.Empty;
                    return true;
                }

                //選択画面を開く
                using (AddressEntry.Address.FrmAddressList frmAddress = new AddressEntry.Address.FrmAddressList(zipno, address1, addresscd))
                {
                    frmAddress.ShowDialog(Parentform);

                    zipno = frmAddress._sPostalCd;
                    address1 = frmAddress._sAddress;
                    addresscd = string.IsNullOrEmpty(frmAddress._sAddressCd) ? string.Empty : frmAddress._sAddressCd;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region ③郵便番号→都道府県、住所をセットする処理
        /// <summary>
        /// ③郵便番号→都道府県、住所をセットする処理
        /// </summary>
        /// <param name="zipcode">郵便番号</param>
        /// <param name="pref">住所1</param>
        /// <param name="adr">住所2+3</param>
        /// <returns></returns>
        [OptionMethodAttribute]
        public static bool GetAddressByZipCodeSplitPref(ref string zipcode, ref string pref, ref string adr)
        {
            try
            {

                //SetZipInfoと同じ処理をする
                // 郵便番号の入力値が7桁でないと無視する
                if (zipcode.Replace("-", string.Empty).Trim().Length != 7)
                    return false;

                // 都道府県、住所が一つでも空欄ではないと無視する
                if (!string.IsNullOrEmpty(pref) || !string.IsNullOrEmpty(adr))
                    return false;

                // 上記以外の場合、郵便番号で検索して住所を自動設定する
                DataTable dtZip = _Dao.SelectZipInfo(zipcode);
                if (dtZip == null || dtZip.Rows.Count == 0)
                    return false;


                DataRow rowZip = dtZip.Rows[0];
                //住所1部分
                pref = rowZip["ADDRESS_1"].ToString();
                adr = rowZip["ADDRESS_2"].ToString() + rowZip["ADDRESS_3"].ToString() + rowZip["ADDRESS_4"].ToString();

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region ④郵便番号マスタより値を戻す処理
        /// <summary>
        /// ④郵便番号マスタより値を戻す処理
        /// オプションファイルの「カラム名指定」に値必須
        /// </summary>
        /// <param name="ele1"></param>
        /// <param name="ele2"></param>
        /// <param name="ele3"></param>
        /// <param name="ele4"></param>
        /// <param name="ele5"></param>
        /// <param name="ele6"></param>
        /// <param name="ele7"></param>
        /// <returns></returns>
        [OptionMethodAttribute(true)]
        public static bool GetAddressInfo(ref OpArg ele1, ref OpArg ele2, ref OpArg ele3, ref OpArg ele4, ref OpArg ele5, ref OpArg ele6, ref OpArg ele7)
        {
            try
            {
                List<OpArg> args = new List<OpArg>();
                args.Add(ele1);
                args.Add(ele2);
                args.Add(ele3);
                args.Add(ele4);
                args.Add(ele5);
                args.Add(ele6);
                args.Add(ele7);
                //SetZipInfoと同じ処理をする
                // 郵便番号の入力値が7桁でないと無視する
                //string zipcode = args.Where(arg => arg.itemname == "郵便番号").First().itemvalue;
                var zipCode = args.Where(arg => arg.ItemName.Contains("郵便番号")).First().ItemValue;
                if (zipCode.Replace("-", string.Empty).Trim().Length != 7)
                    return false;

                // 住所情報に値が入っていたら無視する
                //if (args.Where(arg => arg.itemname != "郵便番号").Any(arg => !String.IsNullOrEmpty(arg.itemvalue)))
                if (args.Where(arg => !arg.ItemName.Contains("郵便番号")).Any(arg => !String.IsNullOrEmpty(arg.ItemValue)))
                        return false;

                // 上記以外の場合、郵便番号で検索して住所を自動設定する
                DataTable dtZip = _Dao.SelectZipInfo(zipCode);
                if (dtZip == null || dtZip.Rows.Count == 0)
                    return false;

                if (dtZip.Rows.Count == 1)
                {
                    DataRow dr = dtZip.Rows[0];
                    ////住所1部分

                    foreach (int i in Enumerable.Range(0, args.Count))
                    {
                        if (String.IsNullOrEmpty(args[i].ItemName))
                            continue;

                        string tmp = string.Empty;
                        args[i].Colname.ForEach(col =>
                        {
                            try
                            {
                                tmp += dr[col].ToString();
                            }
                            catch
                            {
                            //変な列名は無視
                        }

                        });
                        args[i].ItemValue = tmp;
                    }
                }
                else
                {
                    //選択画面を開く
                    using (AddressList.Address.FrmAddressList2 frmAddress = new AddressList.Address.FrmAddressList2(zipCode))
                    {
                        frmAddress.ShowDialog(Parentform);

                        if (frmAddress.DialogResult.Equals(DialogResult.Cancel))
                            return false;

                        var seq = frmAddress._Seq;
                        var dtZip2 = _Dao.SelectZipInfo(zipCode,seq);
                        var dr2 = dtZip2.Rows[0];

                        foreach (int i in Enumerable.Range(0, args.Count))
                        {
                            if (String.IsNullOrEmpty(args[i].ItemName))
                                continue;

                            string tmp = string.Empty;
                            args[i].Colname.ForEach(col =>
                            {
                                try
                                {
                                    tmp += dr2[col].ToString();
                                }
                                catch
                                {
                                    //変な列名は無視
                                }

                            });
                            args[i].ItemValue = tmp;
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 
        /// <summary>
        /// </summary>
        /// <param name="ele1"></param>
        /// <param name="ele2"></param>
        /// <param name="ele3"></param>
        /// <param name="ele4"></param>
        /// <param name="ele5"></param>
        /// <param name="ele6"></param>
        /// <param name="ele7"></param>
        /// <returns></returns>
        [OptionMethodAttribute(true)]
        public static bool GetCodeDefine(ref OpArg ele1, ref OpArg ele2, ref OpArg ele3, ref OpArg ele4, ref OpArg ele5, ref OpArg ele6, ref OpArg ele7,string CodeDefineSection,string ForcedSubstitution)
        {
            try
            {
                List<OpArg> args = new List<OpArg>();
                args.Add(ele1);
                args.Add(ele2);
                args.Add(ele3);
                args.Add(ele4);
                args.Add(ele5);
                args.Add(ele6);
                args.Add(ele7);
                //SetZipInfoと同じ処理をする
                // 郵便番号の入力値が7桁でないと無視する
                //string zipcode = args.Where(arg => arg.itemname == "郵便番号").First().itemvalue;
                var Key = ele1.ItemValue;
                if (Key.Trim().Length == 0)
                    return false;

                var TargetItem = ele1.ItemName;
                // 住所情報に値が入っていたら無視する
                //if (args.Where(arg => arg.itemname != "郵便番号").Any(arg => !String.IsNullOrEmpty(arg.itemvalue)))
                if (!"T".Equals(ForcedSubstitution))
                    if (args.Where(arg => !arg.ItemName.Contains(TargetItem)).Any(arg => !String.IsNullOrEmpty(arg.ItemValue)))
                        return false;

                // 上記以外の場合、郵便番号で検索して住所を自動設定する
                var dtM_CODE_DEFINE = _Dao.SELECT_M_CODE_DEFINE(CodeDefineSection, Key);
                if (dtM_CODE_DEFINE.Rows.Count == 0)
                    return false;

                if (dtM_CODE_DEFINE.Rows.Count == 1)
                {
                    DataRow dr = dtM_CODE_DEFINE.Rows[0];
                    ////住所1部分

                    foreach (int i in Enumerable.Range(0, args.Count))
                    {
                        if (String.IsNullOrEmpty(args[i].ItemName))
                            continue;

                        string tmp = string.Empty;
                        args[i].Colname.ForEach(col =>
                        {
                            try
                            {
                                tmp += dr[col].ToString();
                            }
                            catch
                            {
                                //変な列名は無視
                            }

                        });
                        args[i].ItemValue = tmp;
                    }
                }
                //else
                //{
                //    //選択画面を開く
                //    using (AddressList.Address.FrmAddressList2 frmAddress = new AddressList.Address.FrmAddressList2(zipCode))
                //    {
                //        frmAddress.ShowDialog(parentform);

                //        if (frmAddress.DialogResult.Equals(DialogResult.Cancel))
                //            return false;

                //        var seq = frmAddress._Seq;
                //        var dtZip2 = _Dao.SelectZipInfo(zipCode, seq);
                //        var dr2 = dtZip2.Rows[0];

                //        foreach (int i in Enumerable.Range(0, args.Count))
                //        {
                //            if (String.IsNullOrEmpty(args[i].itemname))
                //                continue;

                //            string tmp = string.Empty;
                //            args[i].colname.ForEach(col =>
                //            {
                //                try
                //                {
                //                    tmp += dr2[col].ToString();
                //                }
                //                catch
                //                {
                //                    //変な列名は無視
                //                }

                //            });
                //            args[i].itemvalue = tmp;
                //        }
                //    }
                //}
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region ⑤コード→値をセットする処理
        [OptionMethodAttribute]
        public static bool SetValueByCode(ref OpArg code, ref OpArg codevalue)
        {
            try
            {
                //値がすでに入っていたら何もしない
                if (!String.IsNullOrWhiteSpace(codevalue.ItemValue))
                    return true;


                string inputcd = code.ItemValue;
                codevalue.ItemValue = String.IsNullOrEmpty(inputcd) || CodeValueList == null ? string.Empty : !CodeValueList.Any(s => s.Code == inputcd) ? string.Empty
                                                                                    : CodeValueList.First(s => s.Code == inputcd).Value;

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
/*
        #region ⑥郵便番号マスタより値を戻す処理（値が入っていても強制置換）
        /// <summary>
        /// ⑥郵便番号マスタより値を戻す処理
        /// オプションファイルの「カラム名指定」に値必須
        /// </summary>
        /// <param name="ele1"></param>
        /// <param name="ele2"></param>
        /// <param name="ele3"></param>
        /// <param name="ele4"></param>
        /// <param name="ele5"></param>
        /// <param name="ele6"></param>
        /// <param name="ele7"></param>
        /// <returns></returns>
        [OptionMethodAttribute(true)]
        public static bool GetAddressInfoForced(ref OpArg ele1, ref OpArg ele2, ref OpArg ele3, ref OpArg ele4, ref OpArg ele5, ref OpArg ele6, ref OpArg ele7)
        {
            try
            {
                List<OpArg> args = new List<OpArg>();
                args.Add(ele1);
                args.Add(ele2);
                args.Add(ele3);
                args.Add(ele4);
                args.Add(ele5);
                args.Add(ele6);
                args.Add(ele7);
                //SetZipInfoと同じ処理をする
                // 郵便番号の入力値が7桁でないと無視する
                //string zipcode = args.Where(arg => arg.itemname == "郵便番号").First().itemvalue;
                var zipCode = args.Where(arg => arg.itemname.Contains("郵便番号")).First().itemvalue;
                if (zipCode.Replace("-", string.Empty).Trim().Length != 7)
                    return false;

                // 住所情報に値が入っていたら無視する
                //if (args.Where(arg => arg.itemname != "郵便番号").Any(arg => !String.IsNullOrEmpty(arg.itemvalue)))
                //if (args.Where(arg => !arg.itemname.Contains("郵便番号")).Any(arg => !String.IsNullOrEmpty(arg.itemvalue)))
                //    return false;

                // 上記以外の場合、郵便番号で検索して住所を自動設定する
                DataTable dtZip = _Dao.SelectZipInfo(zipCode);
                if (dtZip == null || dtZip.Rows.Count == 0)
                    return false;

                if (dtZip.Rows.Count == 1)
                {
                    DataRow dr = dtZip.Rows[0];
                    ////住所1部分

                    foreach (int i in Enumerable.Range(0, args.Count))
                    {
                        if (String.IsNullOrEmpty(args[i].itemname))
                            continue;

                        string tmp = string.Empty;
                        args[i].colname.ForEach(col =>
                        {
                            try
                            {
                                tmp += dr[col].ToString();
                            }
                            catch
                            {
                                //変な列名は無視
                            }

                        });
                        args[i].itemvalue = tmp;
                    }
                }
                else
                {
                    //選択画面を開く
                    using (AddressList.Address.FrmAddressList2 frmAddress = new AddressList.Address.FrmAddressList2(zipCode))
                    {
                        frmAddress.ShowDialog(parentform);

                        if (frmAddress.DialogResult.Equals(DialogResult.Cancel))
                            return false;

                        var seq = frmAddress._Seq;
                        var dtZip2 = _Dao.SelectZipInfo(zipCode, seq);
                        var dr2 = dtZip2.Rows[0];

                        foreach (int i in Enumerable.Range(0, args.Count))
                        {
                            if (String.IsNullOrEmpty(args[i].itemname))
                                continue;

                            string tmp = string.Empty;
                            args[i].colname.ForEach(col =>
                            {
                                try
                                {
                                    tmp += dr2[col].ToString();
                                }
                                catch
                                {
                                    //変な列名は無視
                                }

                            });
                            args[i].itemvalue = tmp;
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
*/
    }
    //======================================================================================
    //OptionalProcess内のメソッドの属性（IsColNameに値が必要かどうか＝DBカラム指定かどうか）
    //======================================================================================
    public class OptionMethodAttribute : Attribute
    {
        public bool SpecifiedColmnName;
        public OptionMethodAttribute(bool specified)
        {
            this.SpecifiedColmnName = specified;
        }
        public OptionMethodAttribute()
            : this(false)
        {
        }

    }

    /// <summary>
    /// コードと値の組み合わせ
    /// </summary>
    public class CodeValue
    {
        [TsvColAttribute(0)]    //tsvから読む時用の属性
        public string Code { get; set; }

        [TsvColAttribute(1)]
        public string Value { get; set; }

        public CodeValue(string c, string v)
        {
            Code = c;
            Value = v;
        }
    }
    public static class CodeValueInfo
    {
        public static List<CodeValue> list = new List<CodeValue>
        {
            new CodeValue("1","ezweb.ne.jp"),
            new CodeValue("2","au.com"),
            new CodeValue("3","docomo.ne.jp"),
            new CodeValue("4","softbank.ne.jp"),
            new CodeValue("5","i.softbank.ne.jp"),
            new CodeValue("6","ymobile.ne.jp"),
            new CodeValue("7","ymobile1.ne.jp"),
            new CodeValue("8","wcm.ne.jp"),
            new CodeValue("9","emnet.ne.jp"),
            new CodeValue("11","yahoo.co.jp"),
            new CodeValue("12","gmail.com"),
            new CodeValue("13","disney.ne.jp"),
            new CodeValue("14","uqmobile.jp"),
            new CodeValue("15","outlook.jp"),
            new CodeValue("16","outlook.com"),
            new CodeValue("17","hotmail.co.jp"),
            new CodeValue("18","excite.co.jp"),
            new CodeValue("19","aol.jp"),
            new CodeValue("20","mail.goo.ne.jp"),
            new CodeValue("21","infoseek.jp"),
        };
    }
}
