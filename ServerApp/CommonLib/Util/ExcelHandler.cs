using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SAIN
{
    /// <summary>
    /// エクセル操作用クラス。ExcelPackageをラップしたクラス。
    /// </summary>
    public class ExcelHandler : IDisposable
    {
        /// <summary>ラップするExcelPackage。 </summary>
        public ExcelPackage ExcelPackage;

        /// <summary>ExcelWorkbookにアクセスしやすくする為のプロパティ。 </summary>
        public ExcelWorkbook WorkBook { get { return ExcelPackage.Workbook; } }

        /// <summary>現在操作対象のシート。</summary>
        public ExcelWorksheet CurrentSheet { get; private set; }

        /// <summary>現在選択中のセル。</summary>
        public ExcelRangeBase CurrentCellRange { get; private set; }

        /// <summary>選択中のセルのアドレスを取得する。(A1:C5など) </summary>
        public string CurrentAddress { get { return CurrentCellRange.Address; } }
        /// <summary>選択中のセルの開始セルの行番号を取得する。 </summary>
        public int CurrentStartRow { get { return CurrentCellRange.Start.Row; } }
        /// <summary>選択中のセルの開始セルの列番号を取得する。 </summary>
        public int CurrentStartColumn { get { return CurrentCellRange.Start.Column; } }
        /// <summary>選択中のセルの終了セルの行番号を取得する。 </summary>
        public int CurrentEndRow { get { return CurrentCellRange.End.Row; } }
        /// <summary>選択中のセルの終了セルの列番号を取得する。 </summary>
        public int CurrentEndColumn { get { return CurrentCellRange.End.Column; } }

        /// <summary>フォント </summary>
        const string FontName = "Meiryo UI";


        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="filePath"></param>
        public ExcelHandler() :
            this(new ExcelPackage())
        {

        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="filePath"></param>
        public ExcelHandler(string filePath) :
            this(new FileInfo(filePath))
        {

        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="currentSheetNum">シート番号。1から開始。</param>
        public ExcelHandler(string filePath, int currentSheetNum)
            : this(new FileInfo(filePath), currentSheetNum)
        {

        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="stream"></param>
        public ExcelHandler(string filePath, string currentSheetName)
            : this(new FileInfo(filePath), currentSheetName)
        {

        }


        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="fileInfo"></param>
        public ExcelHandler(FileInfo fileInfo)
            : this(new ExcelPackage(fileInfo))
        {

        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="fileInfo"></param>
        public ExcelHandler(FileInfo fileInfo, int currentSheetNum)
            : this(new ExcelPackage(fileInfo), currentSheetNum)
        {

        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="currentSheetNum">シート番号。1から開始。</param>
        public ExcelHandler(FileInfo fileInfo, string currentSheetName)
            : this(new ExcelPackage(fileInfo), currentSheetName)
        {

        }


        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="stream"></param>
        public ExcelHandler(Stream stream)
            : this(new ExcelPackage(stream))
        {

        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="currentSheetNum">シート番号。1から開始。</param>
        public ExcelHandler(Stream stream, int currentSheetNum)
            : this(new ExcelPackage(stream), currentSheetNum)
        {

        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="stream"></param>
        public ExcelHandler(Stream stream, string currentSheetName)
            : this(new ExcelPackage(stream), currentSheetName)
        {

        }


        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="excelPackage"></param>
        public ExcelHandler(ExcelPackage excelPackage)
        {
            this.ExcelPackage = excelPackage;
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="currentSheetNum">シート番号。1から開始。</param>
        public ExcelHandler(ExcelPackage excelPackage, int currentSheetNum)
        {
            this.ExcelPackage = excelPackage;
            SetCurrentSheet(currentSheetNum);
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="excelPackage"></param>
        public ExcelHandler(ExcelPackage excelPackage, string currentSheetName)
        {
            this.ExcelPackage = excelPackage;
            SetCurrentSheet(currentSheetName);
        }

        #endregion


        /// <summary>
        /// 操作対象のシートを設定する。
        /// </summary>
        /// <param name="sheetName">シート名</param>
        public ExcelHandler SetCurrentSheet(string sheetName)
        {
            CurrentSheet = WorkBook.Worksheets[sheetName];
            return this;
        }

        /// <summary>
        /// 操作対象のシートを設定する。
        /// </summary>
        /// <param name="sheetNum">シート番号。1から開始。</param>
        public ExcelHandler SetCurrentSheet(int sheetNum)
        {
            CurrentSheet = WorkBook.Worksheets[sheetNum];
            return this;
        }

        /// <summary>
        /// セルを選択する。
        /// </summary>
        /// <param name="sheetNum"></param>
        public ExcelHandler SetCurrentCell(int row, int column)
        {
            CurrentCellRange = CurrentSheet.Cells[row, column];
            return this;
        }

        /// <summary>
        /// セルを選択する。
        /// </summary>
        /// <param name="sheetNum"></param>
        public ExcelHandler SetCurrentCellRange(int startRow, int startColumn, int endRow, int endColumn)
        {
            CurrentCellRange = CurrentSheet.Cells[startRow, startColumn, endRow, endColumn];
            return this;
        }

        /// <summary>
        /// セルを選択する。
        /// </summary>
        /// <param name="sheetNum"></param>
        public ExcelHandler SetCurrentCell(string address)
        {
            CurrentCellRange = CurrentSheet.Cells[address];
            return this;
        }
        
        /// <summary>
        /// 指定されたExcel名前定義をカレントセルに設定する
        /// </summary>
        /// <param name="nameDefinition">Excel名前定義</param>
        /// <returns></returns>
        public ExcelHandler SetCurrentCellByNameDefinition(string nameDefinition)
        {
            if (WorkBook.Names.ContainsKey(nameDefinition))
                CurrentCellRange = WorkBook.Names[nameDefinition];
            return this;
        }

        /// <summary>
        /// 空のExcelファイルを新規作成する。(ファイルはMemoryStream上に作成される。)
        /// </summary>
        /// <param name="firstSheetName"></param>
        /// <returns></returns>
        public static ExcelHandler CreateEmptyExcel(string firstSheetName = "sheet1")
        {
            //MemoryStream ms = new MemoryStream();
            ExcelHandler handler = new ExcelHandler();
            handler.AddSheet(firstSheetName)
                .SetCurrentSheet(firstSheetName)
                .SetCurrentCell(1, 1);

            return handler;
        }
        
        /// <summary>
        /// 選択中のセルの背景色を変更する。
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public ExcelHandler SetBackGroundColor(Color color)
        {
            CurrentCellRange.Style.Fill.PatternType = ExcelFillStyle.Solid;//SetColorの前にPatternTypeを設定しないとエラーになる。
            CurrentCellRange.Style.Fill.BackgroundColor.SetColor(color);
            return this;
        }

 

        /// <summary>
        /// シートを追加する。
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public ExcelHandler AddSheet(string sheetName = "")
        {
            WorkBook.Worksheets.Add(sheetName);
            return this;
        }


        /// <summary>
        /// セルに値を設定する。設定後、同じ行の次の列のセルを選択する。
        /// CurrentCellRangeは1つのセルを指定していると想定。
        /// CsvをExcelに変更する際に使いやすいよう、StringBuilderと同じ感じのメソッドにしている。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="numberFormat">数値の表示書式。デフォルトは通貨形式。</param>
        /// <returns></returns>
        public ExcelHandler Append(int? value, string numberFormat = "#,##0")
        {
            CurrentCellRange.Style.Numberformat.Format = numberFormat;
            CurrentCellRange.Style.Font.Name = FontName;
            CurrentCellRange.Value = value;
            return SetCurrentCell(CurrentStartRow, CurrentStartColumn + 1);
        }

        /// <summary>
        /// セルに値を設定する。設定後、同じ行の次の列のセルを選択する。
        /// CurrentCellRangeは1つのセルを指定していると想定。
        /// CsvをExcelに変更する際に使いやすいよう、StringBuilderと同じ感じのメソッドにしている。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="numberFormat">数値の表示書式。デフォルトは通貨形式。</param>
        /// <returns></returns>
        public ExcelHandler Append(decimal? value, string numberFormat = "#,##0.0")
        {
            CurrentCellRange.Style.Numberformat.Format = numberFormat;
            CurrentCellRange.Style.Font.Name = FontName;
            CurrentCellRange.Value = value;
            return SetCurrentCell(CurrentStartRow, CurrentStartColumn + 1);
        }

        /// <summary>
        /// セルに値を設定する。設定後、同じ行の次の列のセルを選択する。
        /// CurrentCellRangeは1つのセルを指定していると想定。
        /// CsvをExcelに変更する際に使いやすいよう、StringBuilderと同じ感じのメソッドにしている。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="numberFormat">数値の表示書式。デフォルトは通貨形式。</param>
        /// <returns></returns>
        public ExcelHandler Append(double? value, string numberFormat = "#,##0.0")
        {
            CurrentCellRange.Style.Numberformat.Format = numberFormat;
            CurrentCellRange.Style.Font.Name = CommonStrings.ExcelFont.Meiryo;
            CurrentCellRange.Value = value;
            return SetCurrentCell(CurrentStartRow, CurrentStartColumn + 1);
        }

        /// <summary>
        /// セルに値を設定する。設定後、同じ行の次の列のセルを選択する。
        /// CurrentCellRangeは1つのセルを指定していると想定。
        /// CsvをExcelに変更する際に使いやすいよう、StringBuilderと同じ感じのメソッドにしている。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ExcelHandler Append(object value)
        {
            CurrentCellRange.Value = value;
            CurrentCellRange.Style.Font.Name = FontName;
            return SetCurrentCell(CurrentStartRow, CurrentStartColumn + 1);
        }

        /// <summary>
        /// セルに値を設定する。設定後、同じ行の次の列のセルを選択する。
        /// CurrentCellRangeは1つのセルを指定していると想定。
        /// CsvをExcelに変更する際に使いやすいよう、StringBuilderと同じ感じのメソッドにしている。
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public ExcelHandler AppendFunction(string formula, string numberFormat = "#,##0")
        {
            CurrentCellRange.Style.Numberformat.Format = numberFormat;
            CurrentCellRange.Formula = formula;
            CurrentCellRange.Style.Font.Name = FontName;
            return SetCurrentCell(CurrentStartRow, CurrentStartColumn + 1);
        }



        /// <summary>
        /// セルに値を設定する。設定後、次の行の最初の列のセルを選択する。
        /// CurrentCellRangeは1つのセルを指定していると想定。
        /// CsvをExcelに変更する際に使いやすいよう、StringBuilderと同じ感じのメソッドにしている。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="numberFormat">数値の表示書式。デフォルトは通貨形式。</param>
        /// <returns></returns>
        public ExcelHandler AppendLine(int? value, string numberFormat = "#,##0")
        {
            CurrentCellRange.Style.Numberformat.Format = numberFormat;
            CurrentCellRange.Style.Font.Name = FontName;
            CurrentCellRange.Value = value;
            return SetCurrentCell(CurrentStartRow + 1, 1);
        }

        /// <summary>
        /// セルに値を設定する。設定後、次の行の最初の列のセルを選択する。
        /// CurrentCellRangeは1つのセルを指定していると想定。
        /// CsvをExcelに変更する際に使いやすいよう、StringBuilderと同じ感じのメソッドにしている。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="numberFormat">数値の表示書式。デフォルトは通貨形式。</param>
        /// <returns></returns>
        public ExcelHandler AppendLine(decimal? value, string numberFormat = "#,##0.0")
        {
            CurrentCellRange.Style.Numberformat.Format = numberFormat;
            CurrentCellRange.Value = value;
            CurrentCellRange.Style.Font.Name = FontName;
            return SetCurrentCell(CurrentStartRow + 1, 1);
        }


        /// <summary>
        /// セルに値を設定する。設定後、次の行の最初の列のセルを選択する。
        /// CurrentCellRangeは1つのセルを指定していると想定。
        /// CsvをExcelに変更する際に使いやすいよう、StringBuilderと同じ感じのメソッドにしている。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ExcelHandler AppendLine(object value)
        {
            CurrentCellRange.Value = value;
            CurrentCellRange.Style.Font.Name = FontName;
            return SetCurrentCell(CurrentStartRow + 1, 1);
        }

        /// <summary>
        /// 次の行の最初の列のセルを選択する。
        /// CurrentCellRangeは1つのセルを指定していると想定。
        /// CsvをExcelに変更する際に使いやすいよう、StringBuilderと同じ感じのメソッドにしている。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ExcelHandler AppendLine()
        {
            return SetCurrentCell(CurrentStartRow + 1, 1);
        }

        /// <summary>
        /// カレントセルに値を設定する。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ExcelHandler SetValueToCurrentCell(object value)
        {
            CurrentCellRange.Value = value;
            return this;
        }

        /// <summary>
        /// カレントセルに値を設定する。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dateFormat">日付書式（初期値：yy/MM/dd）</param>
        /// <returns></returns>
        public ExcelHandler SetValueToCurrentCell(DateTime? value, string dateFormat = "yy/mm/dd")
        {
            CurrentCellRange.Value = value;
            CurrentCellRange.Style.Numberformat.Format = dateFormat;
            return this;
        }
        
        public ExcelHandler SetPictureToCurrentCell(string photoFullFilePath, Size size, int qualityLevel = 30)
        {
            using (var ms = new MemoryStream())
            {
                // 0～100までの数値、数値が低いほど圧縮率が高くなる
                ImageUtil.Compression(ms, photoFullFilePath, qualityLevel, size);

                var picture = CurrentSheet.Drawings.AddPicture($"pict{CurrentSheet.Drawings.Count}", Image.FromStream(ms));
                SetPictureCell(picture, size);

                return this;
            }
        }

        public ExcelHandler SetPictureToCurrentCell(byte[] originalImage, Size size, int qualityLevel = 30)
        {
            using (var ms = new MemoryStream())
            {
                // 0～100までの数値、数値が低いほど圧縮率が高くなる
                ImageUtil.Compression(ms, originalImage, qualityLevel, size);

                var picture = CurrentSheet.Drawings.AddPicture($"pict{CurrentSheet.Drawings.Count}", Image.FromStream(ms));
                SetPictureCell(picture, size);

                return this;
            }
        }

        private void SetPictureCell(ExcelPicture picture, Size size)
        {
            int rowNumber = CurrentCellRange.Start.Row;
            int colNumber = CurrentCellRange.Start.Column;

            picture.From.Row = (rowNumber - 1);
            picture.From.Column = (colNumber - 1);

            picture.SetSize(size.Width, size.Height);
        }

        /// <summary>
        /// Excelファイルを保存する。
        /// </summary>
        /// <returns></returns>
        public ExcelHandler Save()
        {
            ExcelPackage.Save();
            return this;
        }

        /// <summary>
        /// Excelファイルを保存する。
        /// </summary>
        /// <returns></returns>
        public ExcelHandler SaveAs(Stream outputStream)
        {
            ExcelPackage.SaveAs(outputStream);
            return this;
        }

        /// <summary>
        /// Excelファイルを保存する。
        /// </summary>
        /// <returns></returns>
        public ExcelHandler SaveAs(FileInfo fileInfo)
        {
            ExcelPackage.SaveAs(fileInfo);
            return this;
        }

        /// <summary>
        /// Excelファイルをバイト配列として取得する。
        /// (バイト化前にSave()を呼んでいる。)
        /// </summary>
        /// <returns></returns>
        public byte[] AsByte()
        {
            //いったんMemoryStreamに入れる。
            using (MemoryStream ms = new MemoryStream())
            {
                ExcelPackage.SaveAs(ms);
                return ms.ToArray();
            }

            //この方式だとダウンロードしたExcelファイルを開こうとすると「ファイルが壊れています」的なメッセージが出てしまう。
            //ExcelPackage.Save();
            //return ExcelPackage.GetAsByteArray();
        }

        /// <summary>
        /// 現在選択中のセル範囲のデータをDataTableに入れて返す。
        /// </summary>
        /// <param name="firstRowIsHeader"></param>
        /// <returns></returns>
        public DataTable ReadCurrentRangeAsDataTable(bool firstRowIsHeader = false)
        {
            return ReadAsDataTable(CurrentStartRow, CurrentEndRow, CurrentStartRow, CurrentEndColumn, firstRowIsHeader);
        }

        /// <summary>
        /// Excel内の全てデータをDataTableに入れて返す。
        /// </summary>
        /// <param name="firstRowIsHeader"></param>
        /// <returns></returns>
        public DataTable ReadAllAsDataTable(bool firstRowIsHeader = false)
        {
            int startRow = 1;
            int startCol = 1;
            int endRow = CurrentSheet.Dimension.End.Row;
            int endCol = CurrentSheet.Dimension.End.Column;

            return ReadAsDataTable(startRow, endRow, startCol, endCol, firstRowIsHeader);
        }

        /// <summary>
        /// Excel内の指定した範囲のデータをDataTableに入れて返す。
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="startCol"></param>
        /// <param name="firstRowIsHeader"></param>
        /// <returns></returns>
        public DataTable ReadAsDataTable(int startRow, int startCol, bool firstRowIsHeader = false)
        {
            int endRow = CurrentSheet.Dimension.End.Row;
            int endCol = CurrentSheet.Dimension.End.Column;

            return ReadAsDataTable(startRow, endRow, startCol, endCol, firstRowIsHeader);
        }

        /// <summary>
        /// Excel内の指定した範囲のデータをDataTableに入れて返す。
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="endRow"></param>
        /// <param name="startCol"></param>
        /// <param name="endCol"></param>
        /// <param name="firstRowIsHeader"></param>
        /// <returns></returns>
        public DataTable ReadAsDataTable(int startRow, int endRow, int startCol, int endCol, bool firstRowIsHeader = false)
        {
            DataTable result = new DataTable();

            //ヘッダー設定
            if (firstRowIsHeader)
            {
                //foreach (var headerCell in CurrentSheet.Cells[startRow, startCol, startRow, endCol])

                for (int col = startCol; col <= endCol; col++)
                {
                    var headerCell = CurrentSheet.Cells[startRow, col];
                    result.Columns.Add(headerCell.Text);
                }

                startRow++;
            }
            else
            {
                for (int i = 0; i <= (endCol - startCol); i++)
                    result.Columns.Add();
            }

            //Body作成
            for (int rowNum = startRow; rowNum <= endRow; rowNum++)
            {
                DataRow row = result.Rows.Add();

                int colInRow = 0;
                //foreach (var eachCell in CurrentSheet.Cells[rowNum, startCol, rowNum, endCol])
                for (int col = startCol; col <= endCol; col++)
                {
                    var eachCell = CurrentSheet.Cells[rowNum, col];
                    row[colInRow] = eachCell.Value;
                    colInRow++;
                }
            }
            return result;
        }

        /// <summary>
        /// Dispose処理を行う。
        /// </summary>
        public void Dispose()
        {
            if (ExcelPackage != null)
                ExcelPackage.Dispose();
        }
    }
}