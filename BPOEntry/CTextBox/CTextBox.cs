using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CTextBox
{
    public class CTextBox : TextBox
    {
        private const int WM_PAINT = 0x000F;

        private const int WM_KEYUP = 0x0101;

        private readonly string[] kana_Half = new string[] { "ｧ", "ｨ", "ｩ", "ｪ", "ｫ", "ｯ", "ｬ", "ｭ", "ｮ" };

        private readonly string[] KANA_Half = new string[] { "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ﾂ", "ﾔ", "ﾕ", "ﾖ" };

        private readonly string[] kana_Full = new string[] { "ァ", "ィ", "ゥ", "ェ", "ォ", "ッ", "ャ", "ュ", "ョ" };

        private readonly string[] KANA_Full = new string[] { "ア", "イ", "ウ", "エ", "オ", "ツ", "ヤ", "ユ", "ヨ" };

        /// <summary>
        /// 区分２入力
        /// </summary>
        //private bool _Input2;
        public bool IsInput2 { get; set; } = true;

        /// <summary>
        /// 必須項目
        /// </summary>
        //private bool _Required;
        public bool IsRequired { get; set; } = false;

        /// <summary>
        /// ダミーアイテム
        /// </summary>
        public bool IsDummyItem { get; set; } = false;

        /// <summary>
        /// 受領データ有無
        /// </summary>
        public bool IsExistingReceipt { get; set; } = false;

        /// <summary>
        /// 条件付き必須項目
        /// </summary>
        public string Conditional_Required_Item { get; set; }

        /// <summary>
        /// 条件付き必須値
        /// </summary>
        public string[] Conditional_Required_Value { get; set; }

        public string[] Conditional_Required_Omit_Value { get; set; }

        /// <summary>
        /// 正規表現
        /// </summary>
        //private string _Regex;
        public string Regex { get; set; } = string.Empty;

        /// <summary>
        /// ValidPattern
        /// </summary>
        //private string _ValidPattern;
        public string ValidPattern { get; set; } = string.Empty;

        /// <summary>
        /// ImagePosition
        /// </summary>
        //private string _ImagePosition;
        public string ImagePosition { get; set; } = string.Empty;

        /// <summary>
        /// Tip
        /// </summary>
        //private string _Tips;
        public string Tips { get; set; } = string.Empty;

        /// <summary>
        /// ItemName
        /// </summary>
        //private string _ItemName;
        public string ItemName { get; set; } = string.Empty;

        /// <summary>
        /// InputMode
        /// </summary>
        //private string _InputMode;
        public string InputMode { get; set; } = Consts.InputMode.AlphabetNumeric;

        public bool SelectAllText { get; set; } = true;

        /// <summary>
        /// フル桁入力
        /// </summary>
        public bool FullLength { get; set; } = false;

        //public int MinLength { get; set; } = -1;

        public bool IsMailAddress { get; set; } = false;

        public string DateFormat { get; set; } = string.Empty;

        public string MyNumber1 { get; set; } = string.Empty;

        public string MyNumber2 { get; set; } = string.Empty;

        public bool DisplayCorrect { get; set; }

        public bool AutoRelease { get; set; } = false;

        public bool Square { get; set; } = false;

        public bool MasterCheck { get; set; } = false;

        public bool JumpTab { get; set; } = false;

        public string Range { get; set; } = string.Empty;

        public string ControlAlign { get; set; } = "L";

        public string DR { get; set; } = string.Empty;

        public ImeMode DefaultImeMode { get; set; }

        public string DummyItemFlag { get; set; } = string.Empty;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.BackColor = SystemColors.WindowText;
            this.ForeColor = SystemColors.Window;
            this.DoubleBuffered = true;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            // 共通的に許容するキー入力は無視します。
            var acceptKeys = new List<char>() { '\b' };
            if (this.InputMode == Consts.InputMode.Hiragana
                || this.InputMode == Consts.InputMode.KanaFull
                || this.InputMode == Consts.InputMode.Full)
            {
                acceptKeys.Add(Config.ReadNotCharWideInput[0]);
            }
            else
            {
                acceptKeys.Add(Config.ReadNotCharNarrowInput[0]);
            }

            if (acceptKeys.Contains(e.KeyChar)) { return; }

            // 全角入力時に半角文字を全角変換
            if (this.InputMode == Consts.InputMode.Hiragana
                || this.InputMode == Consts.InputMode.KanaFull
                || this.InputMode == Consts.InputMode.Full)
            {
                if (Utils.strLenB(e.KeyChar) == 1)
                {
                    e.KeyChar = Utils.ToMultiByte(e.KeyChar);
                    if (e.KeyChar.ToString().Equals(Config.ReadNotCharWideInput)) { return; }
                }
            }

            // カナ大文字変換
            if (this.CharacterCasing.Equals(CharacterCasing.Upper))
            {
                if (Consts.InputMode.KanaHalf.Equals(this.InputMode)
                    || Consts.InputMode.AllHalf.Equals(this.InputMode)
                    || Consts.InputMode.MixFull.Equals(this.InputMode))
                {
                    // 半角ｶﾅ大文字に変換
                    for (int i = 0; i < kana_Half.Length; i++)
                    {
                        if (e.KeyChar.ToString().Equals(kana_Half[i]))
                        {
                            e.KeyChar = KANA_Half[i][0];
                        }
                    }
                }

                if (Consts.InputMode.KanaFull.Equals(this.InputMode)
                    || Consts.InputMode.Full.Equals(this.InputMode)
                    || Consts.InputMode.Hiragana.Equals(this.InputMode)
                    || Consts.InputMode.MixFull.Equals(this.InputMode))
                {
                    // 全角ｶﾅ大文字に変換
                    for (int i = 0; i < kana_Full.Length; i++)
                    {
                        if (e.KeyChar.ToString().Equals(kana_Full[i]))
                        {
                            e.KeyChar = KANA_Full[i][0];
                        }
                    }
                }
            }

            // 半角（ｶﾅ含む）で全角入力不可
            if ((Consts.InputMode.AllHalf.Equals(this.InputMode) || Consts.InputMode.KanaHalf.Equals(this.InputMode))
                && !Utils.IsSingleByteChar(e.KeyChar.ToString())
                && !string.Empty.PadRight(1).Equals(e.KeyChar.ToString()))
            {
                //MessageBox.Show("全角文字は入力出来ません。", "222", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
            }

            // 入力可能な文字列パターンの設定が無ければ無視します。
            var pattern = this.Regex;
            if (pattern.Length == 0) { return; }

            // 入力値が指定範囲以外ならハンドル済みに設定します。
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @pattern))
            {
                e.Handled = true;
            }
        }

        //protected override void OnPaint(EventArgs e)
        //{
        //}
        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            // 強調色を標準の色に戻します。
            this.BackColor = SystemColors.Window;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            this.SelectAll();

            // IMEをデフォルトに戻す
            switch (InputMode)
            {
                case Consts.InputMode.AlphabetNumeric:
                    this.ImeMode = ImeMode.Disable;
                    this.BackColor = Consts.TEXT_BOX_BACK_COLOR.TergetItemBackColorHalf;
                    break;
                case Consts.InputMode.KanaHalf:
                    this.ImeMode = ImeMode.KatakanaHalf;
                    this.BackColor = Consts.TEXT_BOX_BACK_COLOR.TergetItemBackColorHalf;
                    break;
                case Consts.InputMode.AllHalf:
                    this.ImeMode = this.DefaultImeMode;
                    this.BackColor = Consts.TEXT_BOX_BACK_COLOR.TergetItemBackColorHalf;
                    break;
                case Consts.InputMode.KanaFull:
                    this.ImeMode = ImeMode.Katakana;
                    this.BackColor = Consts.TEXT_BOX_BACK_COLOR.TergetItemBackColorFull;
                    break;
                case Consts.InputMode.Hiragana:
                case Consts.InputMode.Full:
                    this.ImeMode = ImeMode.Hiragana;
                    this.BackColor = Consts.TEXT_BOX_BACK_COLOR.TergetItemBackColorFull;
                    break;
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // 描画メッセージ or IMEウインドウの変化メッセージ
            // + 罫線が必要な場合処理します
            if (m.Msg == WM_PAINT || m.Msg == WM_KEYUP)
            {
                if (!this.Square) { return; }

                // 中央寄せの場合処理しない！！
                if (this.TextAlign.Equals(HorizontalAlignment.Center)) { return; }

                var g = this.CreateGraphics();
                var sf = new StringFormat();
                // 文字列の高さから行の高さを計算します
                CharacterRange[] characterRanges = { new CharacterRange(0, 1) };
                sf.SetMeasurableCharacterRanges(characterRanges);
                var charRect = g.MeasureCharacterRanges("*", this.Font, new RectangleF(0, 0, 100, 100), sf)[0].GetBounds(g);
                int charWidth = (int)charRect.Width;
                if (this.InputMode == Consts.InputMode.KanaFull
                    || this.InputMode == Consts.InputMode.Hiragana
                    || this.InputMode == Consts.InputMode.Full)
                {
                    charWidth = charWidth * 2;          // 全角
                }
                using (var pen = new Pen(Brushes.Silver))
                {
                    this.SuspendLayout();
                    var x = 0;
                    if (this.TextAlign.Equals(HorizontalAlignment.Right))
                        x = 10;

                    for (int iIdx = 1; iIdx < this.MaxLength; iIdx++)
                    {
                        g.DrawLine(pen, new Point(iIdx * charWidth + x, 0), new Point(iIdx * charWidth + x, this.ClientSize.Height));
                    }
                    this.ResumeLayout();
                }
            }
        }
    }
}
