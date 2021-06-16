using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Util
{
    public class CsvCreator
    {
        /// <summary>データ区切り記号 </summary>
        const string CSV_DELIMITER = ",";

        ///// <summary>データ文字コード </summary>
        //static readonly Encoding Encoding = Encoding.GetEncoding("Shift_JIS");
        /// <summary>データ文字コード </summary>
        //static readonly Encoding Encoding = Encoding.UTF8;


        StringBuilder sb { get; set; }

        public CsvCreator()
        {
            sb = new StringBuilder();
        }

        /// <summary>
        /// CSVの各フィールドの形式を整えて文字列に追加する
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="value"></param>
        /// <param name="needAfterComma"></param>
        public CsvCreator Append(string value, bool needAfterComma = true, bool needBeforeComma = false)
        {
            value = value ?? "";
            string afterComma = needAfterComma ? "," : "";
            string beforeComma = needBeforeComma ? "," : "";
            sb.AppendFormat(beforeComma + "\"{0}\"" + afterComma, value.Replace("\"", "\"\""));
            return this;
        }

        /// <summary>
        /// CSVの各フィールドの形式を整えて文字列に追加する
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="value"></param>
        /// <param name="needAfterComma"></param>
        public CsvCreator Append(int? value, bool needAfterComma = true, bool needBeforeComma = false)
        {
            string val = value == null ? "" : value.ToString();
            Append(val, needAfterComma, needBeforeComma);
            return this;
        }

        /// <summary>
        /// CSVの各フィールドの形式を整えて文字列に追加する
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="value"></param>
        /// <param name="needAfterComma"></param>
        public CsvCreator Append(decimal? value, bool needAfterComma = true, bool needBeforeComma = false)
        {
            string val = value == null ? "" : value.ToString();
            Append(val, needAfterComma, needBeforeComma);
            return this;
        }

        /// <summary>
        /// CSVの各フィールドの形式を整えて文字列に追加する
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="value"></param>
        /// <param name="needAfterComma"></param>
        public CsvCreator Append(object value, bool needAfterComma = true, bool needBeforeComma = false)
        {
            string val = value == null ? "" : value.ToString();
            Append(val, needAfterComma, needBeforeComma);
            return this;
        }

        /// <summary>
        /// 改行文字を追加する。
        /// </summary>
        /// <returns></returns>
        public CsvCreator AppendNewLine()
        {
            sb.AppendLine();
            return this;
        }

        /// <summary>
        /// 結果の文字列を取得する。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return sb.ToString();
        }

        /// <summary>
        /// クリアする。
        /// </summary>
        /// <returns></returns>
        public void Clear()
        {
            sb.Clear();
        }


    }
}
