using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    /// <summary>
    /// 検索対象列情報を保持することをあらわす。
    /// 検索時にソートを行う場合、検索用モデルの付与する。
    /// </summary>
    public interface ISortParams
    {
        /// <summary>検索対象列 </summary>
        List<SortParam> SortParams { get; set; }
    }

    /// <summary>
    /// 検索列情報
    /// </summary>
    public class SortParam
    {
        public SortParam() { }

        public SortParam(string ColumnName = "", string Prefix = "", string Order = "")
        {
            this.ColumnName = ColumnName;
            this.Prefix = Prefix;
            this.Order = Order;
        }
        /// <summary>カラム名 </summary>
        public string ColumnName { get; set; }
        /// <summary>列名付与されるprefix。SQLのORDER句で使用する。base.IDみたいな。 </summary>
        public string Prefix { get; set; }
        /// <summary>順序。ASC、DESC </summary>
        public string Order { get; set; }

    }

    public static class ISortParamsExtensions
    {
        /// <summary>
        /// Order句の文言。
        /// </summary>
        public static readonly string[] SortOrderStrings = new string[] { SortAsc, SortDesc };

        public const string SortAsc = "asc";
        public const string SortDesc = "desc";

        /// <summary>
        /// 検索対象列に含まれるかを確認する。
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sortColumns">検索対象列</param>
        /// <returns></returns>
        public static List<SortParam> CheckSortParams(this ISortParams model, IEnumerable<string> sortColumns)
        {
            var result = CheckSortParams(model?.SortParams, sortColumns);
            return result;
        }

        /// <summary>
        /// 検索対象列に含まれるかを確認する。
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sortColumns">検索対象列</param>
        /// <returns></returns>
        public static List<SortParam> CheckSortParams(IEnumerable<SortParam> SortParams, IEnumerable<string> sortColumns)
        {
            var result = SortParams == null ? new List<SortParam>() :
                 SortParams.Where(m => SortOrderStrings.Contains(m.Order) && sortColumns.Contains(m.ColumnName)).ToList();

            return result;
        }

        /// <summary>
        /// model内の対象列の現在の順序を取得する。
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columnName">カラム名</param>
        /// <returns></returns>
        public static string GetCurrentSortOrder(this ISortParams model, string columnName)
        {
            if (model?.SortParams == null)
                return "";

            string result = model.SortParams.Where(m => m.ColumnName == columnName).Select(m => m.Order).FirstOrDefault();
            return result;
        }
    }

}
