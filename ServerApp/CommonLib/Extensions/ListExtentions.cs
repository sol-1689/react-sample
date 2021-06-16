using Domain.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ListExtentions
{
    /// <summary>
    /// リスト自体がnull、もしくは要素数が0ならtrueを返す。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
    {
        bool result = list == null || !list.Any();
        return result;
    }

    /// <summary>
    ///  シーケンス内の指定したインデックス位置にある要素を返します。
    ///  インデックスが範囲外の場合は既定値を返します。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="argList"></param>
    /// <param name="index"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static T ElementAtOrDefault<T>(this IEnumerable<T> argList, int index, T defaultValue)
    {
        if (argList.Count() <= index)
            return defaultValue;

        return argList.ElementAt(index);
    }
    
        /// <summary>
    /// Selectのラムダ式がasyncの際に使用。
    /// 例）Select(async ～).WhenAll().Where(～～)
    /// </summary>
    /// <param name="tasks"></param>
    /// <returns></returns>
    public static Task WhenAll(this IEnumerable<Task> tasks)
    {
        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Selectのラムダ式がasyncの際に使用。
    /// 例）Select(async ～).WhenAll().Where(～～)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tasks"></param>
    /// <returns></returns>
    public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks)
    {
        return Task.WhenAll(tasks);
    }


    /// <summary>
    /// 渡されたリストの組み合わせ結果を取得する。
    /// </summary>
    /// <example>
    /// {1,2,3,4}というリストを引数で受け取り、argIterationが2の場合、
    /// 結果は {1,2} {1,3} {1,4} {2,3} {2,4} {3,4} を返す。
    /// 3の場合は [1, 2, 3], [1, 2, 4], [1, 3, 4], [2, 3, 4]。
    /// 重複した値は無視される。
    /// </example>
    /// <typeparam name="T"></typeparam>
    /// <param name="argList"></param>
    /// <param name="argSetSize"></param>
    /// <returns></returns>
    public static IEnumerable<T[]> GetPermutationSubset<T>(this IList<T> argList, int argSetSize)
    {
        if (argList == null) return null;
        if (argSetSize <= 0) return null;

        return CommonUtil.GetPermutationSubset(argList, 0, argSetSize - 1);
    }


    /// <summary>
    /// Distinctの拡張メソッド。ラムダ式でDistinct対象を指定できるようにした。
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="source"></param>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>
     (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        HashSet<TKey> knownKeys = new HashSet<TKey>();
        foreach (TSource element in source)
        {
            if (knownKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }

    /// <summary>
    /// リストを指定サイズのチャンクに分割する拡張メソッド。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<T>> Chunks<T>(this IEnumerable<T> list, int size)
    {
        while (list.Any())
        {
            yield return list.Take(size);
            list = list.Skip(size);
        }
    }

    /// <summary>
    /// Nullable型のリストのnullではない値だけを抽出する。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static IEnumerable<T> ToNotNull<T>(this IEnumerable<T?> list) where T:struct
    {
        return list.Where(m => m != null).Select(m => m.Value);
    }

}

