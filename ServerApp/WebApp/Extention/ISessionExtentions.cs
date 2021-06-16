using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

/// <summary>
/// ISessionの拡張メソッド。
/// </summary>
public static class ISessionExtentions
{
    /// <summary>
    /// Sessionに値を設定する。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="session"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Sessionから値を取得する。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="session"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default(T) :
                              JsonConvert.DeserializeObject<T>(value);
    }

}
