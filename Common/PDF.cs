using System;
using System.Collections.Generic;
using System.IO;
//using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using NLog;

namespace Common
{
    /// <summary>
    /// PDF Controller
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    public class pdfController
    {
        #region メンバ変数

        /// <summary>
        /// ログ出力
        /// </summary>
        private static Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// FileStream
        /// </summary>
        private System.IO.FileStream _fs;
        /// <summary>
        /// Document
        /// </summary>
        private Document _doc;
        /// <summary>
        /// PdfWriter
        /// </summary>
        private PdfWriter _pw;
        /// <summary>
        /// PdfContentByte
        /// </summary>
        private PdfContentByte _pcb;


        PdfImportedPage _pip;
        /// <summary>
        /// Column
        /// </summary>
        private ColumnText _ct;
        /// <summary>
        /// 埋め込み書体リスト
        /// </summary>
        private List<BaseFont> fonts = new List<BaseFont>();

        /// <summary>
        /// ページ高さ
        /// </summary>
        public float _pageHeight = 0;
        /// <summary>
        /// ページ幅
        /// </summary>
        public float PageWidth { get; set; }
        /// <summary>
        /// 編集者
        /// </summary>
        public string PDFAuthor { get; set; }
        /// <summary>
        /// タイトル
        /// </summary>
        public string PDFTitle { get; set; }
        /// <summary>
        /// サブタイトル
        /// </summary>
        public string PDFSubject { get; set; }
        /// <summary>
        /// 印刷用コマンドパラメータ
        /// </summary>
        private const string sFomat_Param = "/printto \"{0}\" \"{1}\"";

        #endregion

        #region PDF作成
        
        /// <summary>
        /// PDF作成(A4縦)
        /// </summary>
        /// <param name="pdffile">PDFファイル</param>
        /// <returns></returns>
        public void PDFCreateA4T(string pdffile)
        {
            try
            {
                PDFCreate(pdffile, PageSize.A4);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// PDF作成(A4横)
        /// </summary>
        /// <param name="pdffile">PDFファイル</param>
        /// <returns></returns>
        public void PDFCreateA4Y(string pdffile)
        {
            try
            {
                PDFCreate(pdffile, PageSize.A4.Rotate());
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// PDF作成(A3縦)
        /// </summary>
        /// <param name="pdffile">PDFファイル</param>
        /// <returns></returns>
        public void PDFCreateA3T(string pdffile)
        {
            try
            {
                PDFCreate(pdffile, PageSize.A3);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// PDF作成(A3横)
        /// </summary>
        /// <param name="pdffile">PDFファイル</param>
        /// <returns></returns>
        public void PDFCreateA3Y(string pdffile)
        {
            try
            {
                PDFCreate(pdffile, PageSize.A3.Rotate());
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// PDF作成
        /// </summary>
        /// <param name="pdffile">PDFファイル名</param>
        /// <param name="pagesize">ページサイズ</param>
        public void PDFCreate(string pdffile, Rectangle pagesize)
        {
            try
            {
                _fs = new System.IO.FileStream(pdffile, FileMode.Create);
                _doc = new Document(pagesize);
                _pw = PdfWriter.GetInstance(_doc, _fs);
                
                _doc.Open();
                _pcb = _pw.DirectContent;

                _doc.AddAuthor(PDFAuthor);
                _doc.AddTitle(PDFTitle);
                _doc.AddSubject(PDFSubject);

                // ページの高さ
                _pageHeight = _doc.PageSize.Height;
                // ページの幅
                PageWidth = _doc.PageSize.Width;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex, "PDF作成で例外が発生しました。");
                throw;
            }
        }

        /// <summary>
        /// PDF作成
        /// </summary>
        /// <param name="pdffile">PDFファイル名</param>
        /// <param name="PageSize">ページサイズ</param>
        public void CreatePage(string sTemplateFilePath,string sNewFilePath, Rectangle PageSize, string sBackGroundFilePath = null,int iPageSize = 100 )
        {
            try
            {
                _fs = new System.IO.FileStream(sNewFilePath, FileMode.Create);
                _doc = new Document(PageSize);
                _pw = PdfWriter.GetInstance(_doc, _fs);

                _doc.Open();
                _pcb = _pw.DirectContent;

                // 背景イメージ
                if (sBackGroundFilePath != null)
                {
                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(new Uri(sBackGroundFilePath));
                    image.ScalePercent((float)(_doc.PageSize.Width / image.Width * iPageSize));
                    image.SetAbsolutePosition(0f, 0f);
                    _doc.Add(image);
                }

                // テンプレート挿入
                PdfReader pr = new PdfReader(sTemplateFilePath);
                 _pip = _pw.GetImportedPage(pr, 1);
                _pcb.AddTemplate(_pip, 0.0f, 0.0f);

                _doc.AddAuthor(PDFAuthor);
                _doc.AddTitle(PDFTitle);
                _doc.AddSubject(PDFSubject);

                // ページの高さ
                _pageHeight = _doc.PageSize.Height;
                // ページの幅
                PageWidth = _doc.PageSize.Width;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex, "PDF作成で例外エラーが発生しました。");
                throw;
            }
        }

        // 改ページ
        public void NewPage()
        {
            _doc.NewPage();
            _pcb.AddTemplate(_pip, 0.0f, 0.0f);
        }

        /// <summary>
        /// PDF作成 クローズ
        /// </summary>
        public void PDFClose()
        {
            if (_doc != null)
            {
                _doc.Close();
                _doc.Dispose();
            }
            if (_fs != null)
            {
                _fs.Close();
                _fs.Dispose();
            }
            if (_pw != null)
            {
                _pw.Close();
                _pw.Dispose();
            }
        }
        #endregion

        #region 埋め込み書体セット
        /// <summary>
        /// 埋め込み書体セット
        /// </summary>
        /// <param name="sFontFilePath">書体ファイルパス</param>
        public void SetFont(string sFontFilePath)
        {
            if (fonts == null)
            {
                fonts = new List<BaseFont>();
            }
            fonts.Add(BaseFont.CreateFont(sFontFilePath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED));
        }

        /// <summary>
        /// 埋め込み書体リストクリア
        /// </summary>
        public void ClearFont()
        {
            fonts.Clear();
        }
        #endregion

        #region 改ページ

        /// <summary>
        /// 改ページ
        /// </summary>
        public void NextPage()
        {
            _doc.NewPage();
        }
        #endregion

        #region テキスト出力

        /// <summary>
        /// テキスト出力
        /// </summary>
        /// <param name="fontno">書体番号</param>
        /// <param name="fontsize">書体サイズ</param>
        /// <param name="x">出力位置:X</param>
        /// <param name="y">出力位置:Y</param>
        /// <param name="text">出力文字列</param>
        public void TextOut(int fontno, float fontsize, float x, float y, string text)
        {
            y = _pageHeight - y;
            _pcb.SetFontAndSize(fonts[fontno], fontsize);
            _pcb.BeginText();
            _pcb.ShowTextAligned(Element.ALIGN_LEFT, text, x, y, 0);
            _pcb.EndText();
        }

        /// <summary>
        /// テキスト出力(右寄せ)
        /// </summary>
        /// <param name="fontno">書体番号</param>
        /// <param name="fontsize">書体サイズ</param>
        /// <param name="x">出力位置:X</param>
        /// <param name="y">出力位置:Y</param>
        /// <param name="text">出力文字列</param>
        public void TextOutR(int fontno, float fontsize, float x, float y, string text)
        {
            y = _pageHeight - y;
            _pcb.SetFontAndSize(fonts[fontno], fontsize);
            _pcb.BeginText();
            _pcb.ShowTextAligned(Element.ALIGN_RIGHT, text, x, y, 0);
            _pcb.EndText();
        }
        #endregion

        #region カラムテキスト出力

        /// <summary>
        /// カラムテキスト開始
        /// </summary>
        public void SetCText()
        {
            _ct = new ColumnText(_pcb);
        }

        /// <summary>
        /// カラムテキスト出力
        /// </summary>
        /// <param name="fontno">書体番号</param>
        /// <param name="fontsize">書体サイズ</param>
        /// <param name="text">出力文字列</param>
        /// <param name="x1">出力位置 左下X</param>
        /// <param name="y1">出力位置 左下Y</param>
        /// <param name="x2">出力位置 右上X</param>
        /// <param name="y2">出力位置 右上Y</param>
        /// <param name="leading">行間</param>
        public void OutCText(int fontno, float fontsize, string text, float x1, float y1, float x2, float y2, float leading = 0)
        {
            y1 = _pageHeight - y1;
            y2 = _pageHeight - y2;
            Font font = new Font(fonts[fontno]);
            font.Size = fontsize;
            Phrase tx = new Phrase(text, font);
            _ct.SetSimpleColumn(tx, x1, y1, x2, y2, leading, Element.ALIGN_LEFT);
            _ct.Go();
        }

        /// <summary>
        /// カラムテキスト出力
        /// </summary>
        /// <param name="fontno">書体番号</param>
        /// <param name="fontsize">書体サイズ</param>
        /// <param name="text">出力文字列</param>
        /// <param name="x1">出力位置 左下X</param>
        /// <param name="y1">出力位置 左下Y</param>
        /// <param name="x2">出力位置 右上X</param>
        /// <param name="y2">出力位置 右上Y</param>
        /// <param name="leading">行間</param>
        public void OutCTextR(int fontno, float fontsize, string text, float x1, float y1, float x2, float y2, float leading = 0)
        {
            y1 = _pageHeight - y1;
            y2 = _pageHeight - y2;
            Font font = new Font(fonts[fontno]);
            font.Size = fontsize;
            Phrase tx = new Phrase(text, font);
            _ct.SetSimpleColumn(tx, x1, y1, x2, y2, leading, Element.ALIGN_RIGHT);
            _ct.Go();
        }
        /// <summary>
        /// カラムテキスト出力
        /// </summary>
        /// <param name="fontno">書体番号</param>
        /// <param name="fontsize">書体サイズ</param>
        /// <param name="text">出力文字列</param>
        /// <param name="x1">出力位置 左下X</param>
        /// <param name="y1">出力位置 左下Y</param>
        /// <param name="x2">出力位置 右上X</param>
        /// <param name="y2">出力位置 右上Y</param>
        /// <param name="leading">行間</param>
        public void OutCTextC(int fontno, float fontsize, string text, float x1, float y1, float x2, float y2, float leading = 0)
        {
            y1 = _pageHeight - y1;
            y2 = _pageHeight - y2;
            Font font = new Font(fonts[fontno]);
            font.Size = fontsize;
            Phrase tx = new Phrase(text, font);
            _ct.SetSimpleColumn(tx, x1, y1, x2, y2, leading, Element.ALIGN_CENTER);
            _ct.Go();
        }
        #endregion

        #region ライン出力

        /// <summary>
        /// ライン出力
        /// </summary>
        /// <param name="fromX">出力位置 左下X</param>
        /// <param name="fromY">出力位置 左下Y</param>
        /// <param name="toX">出力位置 右上X</param>
        /// <param name="toY">出力位置 右上Y</param>
        /// <param name="linewidth">線幅</param>
        public void DrawLine(float fromX, float fromY, float toX, float toY, float linewidth = 0.5f)
        {
            fromY = _pageHeight - fromY;
            toY = _pageHeight -toY;
            _pcb.SetLineWidth(linewidth);
            _pcb.MoveTo(fromX, fromY);
            _pcb.LineTo(toX, toY);
            _pcb.ClosePathStroke();
        }
        #endregion

        #region イメージ出力
        
        /// <summary>
        /// イメージ出力
        /// </summary>
        /// <param name="imagefile">イメージファイル</param>
        /// <param name="x">出力位置 X</param>
        /// <param name="y">出力位置 Y</param>
        public void AddImageFile(string imagefile, float x, float y)
        {
            y = _pageHeight - y;
            Image img = Image.GetInstance(imagefile);
            img.SetAbsolutePosition(x, y);
            _pcb.AddImage(img);
        }
        #endregion

        #region バーコード
        
        /// <summary>
        /// バーコード:Code39
        /// </summary>
        /// <param name="code"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddBC_Code39(string code, float x, float y)
        {
            y = _pageHeight - y;
            Barcode39 bc = new Barcode39();
            bc.Code = code;
            Image img = bc.CreateImageWithBarcode(_pcb, BaseColor.BLACK, BaseColor.BLACK);
            img.SetAbsolutePosition(x, y);
            _doc.Add(img);
        }

        /// <summary>
        /// バーコード:Codebar(NW-7)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddBC_Codebar(string code, float x, float y)
        {
            y = _pageHeight - y;
            BarcodeCodabar bc = new BarcodeCodabar();
            bc.Code = code;
            bc.StartStopText = true;
            bc.BarHeight = 40F;
            Image img = bc.CreateImageWithBarcode(_pcb, BaseColor.BLACK, BaseColor.BLACK);

            img.SetAbsolutePosition(x, y);
            _doc.Add(img);
        }
        #endregion

        //#region PDFファイル印刷

        ///// <summary>
        ///// PDFファイル印刷
        ///// </summary>
        ///// <param name="pdffile"></param>
        ///// <param name="frm"></param>
        ///// <param name="dlg"></param>
        //public void PDFFilePrint(string pdffile, Form frm, bool dlg = false)
        //{
        //    // 印刷
        //    AxAcroPDFLib.AxAcroPDF apdf = new AxAcroPDFLib.AxAcroPDF();
        //    frm.Controls.Add(apdf);
        //    apdf.Visible = false;
        //    apdf.LoadFile(pdffile);
        //    if (dlg)
        //    {
        //        // 印刷ダイアログを表示する
        //        apdf.printWithDialog();
        //    }
        //    else
        //    {
        //        // 直接印刷する
        //        apdf.printAll();
        //    }
        //    frm.Controls.Remove(apdf);
        //}

        //#endregion
        /// <summary>
        /// プリンタ状態チェック
        /// </summary>
        /// <param name="sPrinterName">プリンタ名</param>
        /// <returns></returns>
        public bool CheckPinterStatus(string sPrinterName = null)
        {
            try
            {
                _log.Info("プリンタ状態チェック:開始");
                System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
                if (sPrinterName != null)
                {
                    //プリンタ指定あり
                    pd.PrinterSettings.PrinterName = sPrinterName;
                }

                // OK
                if (pd.PrinterSettings.IsValid)
                {
                    _log.Info("プリンタ状態チェック:OK プリンタ名:{0}", pd.PrinterSettings.PrinterName);
                    return true;
                }

                // NG
                _log.Error("プリンタ状態チェック:NG プリンタ名:{0}", pd.PrinterSettings.PrinterName);
                return false;
            }
            finally
            {
                _log.Info("プリンタ状態チェック:終了");
            }
        }

        /// <summary>
        /// PDF印刷
        /// </summary>
        /// <param name="sPdfFileFullPath">PDFファイルフルパス</param>
        /// <param name="sPrinterName"></param>
        public void PrintPDF(string sPdfFileFullPath, string sPrinterName = null)
        {
            try
            {
                _log.Info("PDF印刷:開始");
                System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
                if (sPrinterName != null)
                {
                    //プリンタ指定あり
                    pd.PrinterSettings.PrinterName = sPrinterName;
                }

                if (!pd.PrinterSettings.IsValid)
                    return;

                string param = string.Format(sFomat_Param, pd.PrinterSettings.PrinterName, sPdfFileFullPath);
                string printapppath = System.IO.Path.Combine(Config.AppPath, "PDFXCview.exe");
                // 印刷プロセスの起動
                Utils.ProcessStartWaitForExit(printapppath, param);
            }
            finally
            {
                _log.Info("PDF印刷:終了");
            }
        }

    }
}
