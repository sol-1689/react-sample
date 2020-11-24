using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    /// <summary>
    /// ページング情報を保持することを表す。
    /// </summary>
    public interface IPagingParam: IPagingTotalCount
    {
        /// <summary>現在のページ </summary>
        int? CurrentPage { get; set; }
        /// <summary>1ページ内の行数 </summary>
        int? RowCount { get; set; }
        /// <summary>SELECT開始行 </summary>
        int? StartRow { get; set; }
    }

    /// <summary>
    /// ページング情報を保持するクラス。
    /// </summary>
    public class PagingParam : IPagingParam
    {
        public int? CurrentPage { get; set; }
        public int? RowCount { get; set; }
        public int? StartRow { get; set; }
        public int TotalCount { get; set; }
    }


    /// <summary>
    /// 検索結果にページングを無視した総件数を保持することを表す。
    /// </summary>
    public interface IPagingTotalCount
    {
        /// <summary>総件数。 </summary>
        int TotalCount { get; set; }
    }


    public static class IPagingParamExtentions
    {
        const int DefaultPageCount = 20;
        /// <summary>
        /// ページング用の値を設定する。
        /// 既に値が設定されている場合は何もしない。
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="defaultCurrentPage"></param>
        /// <param name="defaultRowCount"></param>
        public static void SetDefaultPagingParamIfNull(this IPagingParam cond,
            int defaultCurrentPage = 0, int defaultRowCount = DefaultPageCount)
        {
            cond.CurrentPage = cond.CurrentPage == null || cond.CurrentPage == 0
                ? defaultCurrentPage : cond.CurrentPage;

            cond.RowCount = cond.RowCount ?? defaultRowCount;

            cond.StartRow = GetStartRow(cond);
        }

        /// <summary>
        /// SELECT開始行を取得する。
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public static int GetStartRow(this IPagingParam cond)
        {
            //int result = (cond.CurrentPage.Value - 1) * cond.RowCount.Value; //検索開始位置;
            //1ページ目は0で来る想定。
            int result = (cond.CurrentPage.Value) * cond.RowCount.Value; //検索開始位置;
            return result;
        }

        /// <summary>
        /// 合計ページ数を算出する。
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public static int CulcTotalPageCount(this IPagingParam cond)
        {
            int totalPageCount = cond.TotalCount / cond.RowCount.Value; //総商品数を１ページの表示数で割る。
            if ((cond.TotalCount % cond.RowCount.Value) > 0)
                totalPageCount++; //余りあったらページ数を1増やす。

            return totalPageCount;
        }

        /// <summary>
        /// ページ内の表示開始位置を取得する。
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public static int CulcStartCount(this IPagingParam cond)
        {
            int? startCount = (cond.RowCount * (cond.CurrentPage)) + 1;

            return startCount ?? 1;
        }

        /// <summary>
        /// ページ内の表示終了位置を取得する。
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="startCount"></param>
        /// <returns></returns>
        public static int CulcEndCount(this IPagingParam cond, int startCount)
        {
            int? endCount = startCount + cond.RowCount - 1;
            endCount = endCount > cond.TotalCount ? cond.TotalCount : endCount;

            return endCount ?? 1;
        }

        /// <summary>
        /// SELECT開始～終了行の表示用の文言を取得する。
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetStartEndCountText(this IPagingParam cond, string format = "{0}～{1}件まで表示")
        {
            if (cond == null)
                return "";

            int start = CulcStartCount(cond);
            int end = CulcEndCount(cond, start);

            string text = string.Format(format, start, end);
            return text;
        }


    }

    //public interface ISearchCountResult
    //{

    //    int? TotalCount { get; set; }
    //    int? TotalPageCount { get; set; }

    //    int? StartCount { get; set; }
    //    int? EndCount { get; set; }
    //}
}
