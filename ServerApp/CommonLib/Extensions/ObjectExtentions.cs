using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Linq;
using Domain.Util;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// オブジェクトクラスの拡張メソッドを定義する。
/// </summary>
public static class ObjectExtensions
{
    ///// <summary>Binder.GetMemberのcontextのType。</summary>
    //static readonly Type BindContextType = typeof(ObjectExtensions);

    /// <summary>
    /// Getプロパティ用CallSiteのキャッシュ。
    /// </summary>
    readonly static ConcurrentDictionary<string, CallSite<Func<CallSite, object, object>>> GetCallSiteCache
        = new ConcurrentDictionary<string, CallSite<Func<CallSite, object, object>>>();

    /// <summary>
    /// Setプロパティ用CallSiteのキャッシュ。
    /// </summary>
    readonly static ConcurrentDictionary<string, CallSite<Func<CallSite, object, object, object>>> SetCallSiteCache
        = new ConcurrentDictionary<string, CallSite<Func<CallSite, object, object, object>>>();

    /// <summary>
    /// バインドに失敗したTypeとプロパティ名のキャッシュ。
    /// </summary>
    readonly static ConcurrentDictionary<string, string> BindFailedPropCache = new ConcurrentDictionary<string, string>();

    /// <summary>
    /// 文字列で指定したGetプロパティを呼び出す。
    /// </summary>
    /// <param name="target">呼び出すGetプロパティを保持したインスタンス</param>
    /// <param name="memberName">呼び出すGetプロパティ名</param>
    /// <param name="throwBindException">Getプロパティ名が存在しなかった場合、例外を投げるか。デフォルトtrue。</param>
    /// <returns>Getプロパティの呼び出し結果。throwBindExceptionがfalseの場合は、default値を返す。</returns>
    public static dynamic CallGetPropertyByName<T>(this T target, string memberName, bool throwBindException = false)
    {
        Type targetType = target.GetType();
        var key = targetType.FullName + memberName;

        try
        {
            if (!throwBindException && BindFailedPropCache.ContainsKey(key))
                return default(T);

            //キャッシュにCallSiteあったら取得。なかったら作成してキャッシュに入れる。
            var callSite = GetCallSiteCache.GetOrAdd(key, (k) =>
            {
                var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, memberName, targetType,
                    new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
                var createdCallSite = CallSite<Func<CallSite, object, object>>.Create(binder);
                return createdCallSite;
            });

            //CallSite呼び出し。
            return callSite.Target(callSite, target);
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
        {
            BindFailedPropCache.GetOrAdd(key, (t) =>
            {
                //LogUtil.Warn("プロパティの動的取得でエラー。エラーキャッシュに追加します。", ex);
                return "";
            });

            if (throwBindException)
                throw;
            else
                return default(T);
        }
    }


    ///// <summary>
    ///// 文字列で指定したGetプロパティを呼び出す。
    ///// </summary>
    ///// <param name="target">呼び出すGetプロパティを保持したインスタンス</param>
    ///// <param name="memberName">呼び出すGetプロパティ名</param>
    ///// <param name="throwBindException">Getプロパティ名が存在しなかった場合、例外を投げるか。デフォルトtrue。</param>
    ///// <returns>Getプロパティの呼び出し結果。throwBindExceptionがfalseの場合は、default値を返す。</returns>
    //public static dynamic CallGetPropertyByName(this object target, string memberName, Type targetType, bool throwBindException = true)
    //{
    //    var key = Tuple.Create<Type, string>(targetType, memberName);

    //    try
    //    {
    //        if (!throwBindException && BindFailedPropCache.ContainsKey(key))
    //            return GetDefault(targetType);

    //        //キャッシュにCallSiteあったら取得。なかったら作成してキャッシュに入れる。
    //        var callSite = GetCallSiteCache.GetOrAdd(key, (k) =>
    //        {
    //            var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, memberName, targetType,
    //                new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
    //            var createdCallSite = CallSite<Func<CallSite, object, object>>.Create(binder);
    //            return createdCallSite;
    //        });

    //        //CallSite呼び出し。
    //        return callSite.Target(callSite, target);
    //    }
    //    catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
    //    {
    //        BindFailedPropCache.GetOrAdd(key, (t) =>
    //        {
    //            LogUtil.Warn("プロパティの動的取得でエラー。エラーキャッシュに追加します。", ex);
    //            return "";
    //        });

    //        if (throwBindException)
    //            throw;
    //        else
    //            return GetDefault(targetType);
    //    }
    //}

    /// <summary>
    /// 文字列で指定したSetプロパティを呼び出す。
    /// </summary>
    /// <param name="target">呼び出すSetプロパティを保持したインスタンス</param>
    /// <param name="memberName">呼び出すSetプロパティ名</param>
    /// <param name="value">Setプロパティに渡す値</param>
    /// <param name="throwBindException">Getプロパティ名が存在しなかった場合、例外を投げるか。デフォルトtrue。</param>
    /// <returns>throwBindExceptionがfalseの場合のみ有効。Setプロパティを呼びだせた場合はtrue、呼びだせなかった場合はfalse。</returns>
    public static bool CallSetPropertyByName(this object target, string memberName, object value, bool throwBindException = false)
    {
        Type targetType = target.GetType();
        return CallSetPropertyByName(target, memberName, value, targetType, throwBindException);
    }

    /// <summary>
    /// 文字列で指定したSetプロパティを呼び出す。
    /// </summary>
    /// <param name="target">呼び出すSetプロパティを保持したインスタンス</param>
    /// <param name="memberName">呼び出すSetプロパティ名</param>
    /// <param name="value">Setプロパティに渡す値</param>
    /// <param name="throwBindException">Getプロパティ名が存在しなかった場合、例外を投げるか。デフォルトtrue。</param>
    /// <returns>throwBindExceptionがfalseの場合のみ有効。Setプロパティを呼びだせた場合はtrue、呼びだせなかった場合はfalse。</returns>
    public static bool CallSetPropertyByName(this object target, string memberName, object value, Type targetType, bool throwBindException = false)
    {
        var key = targetType.FullName + memberName;
        try
        {
            if (!throwBindException && BindFailedPropCache.ContainsKey(key))
                return false;

            //キャッシュにCallSiteあったら取得。なかったら作成してキャッシュに入れる。
            var callSite = SetCallSiteCache.GetOrAdd(key, (k) =>
            {
                var binder = Microsoft.CSharp.RuntimeBinder.Binder.SetMember(CSharpBinderFlags.None, memberName, targetType,
                    new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }
                );

                var createdCallSite = CallSite<Func<CallSite, object, object, object>>.Create(binder);
                return createdCallSite;
            });

            //CallSite呼び出し。
            callSite.Target(callSite, target, value);

            return true;
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
        {
            BindFailedPropCache.GetOrAdd(key, (t) =>
            {
                //LogUtil.Warn("プロパティの動的取得でエラー。エラーキャッシュに追加します。", ex);
                return "";
            });

            if (throwBindException)
                throw;
            else
                return false;
        }
    }


    public static object GetDefault(Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }
        return null;
    }

    /// <summary>
    /// オブジェクトをディクショナリに変換する。
    /// パブリックプロパティ名がキー・値がValueとなる。
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Dictionary<string, object> ConvertToDictionary(this object obj, bool ignoreNullField = true)
    {
        if (obj == null)
            return null;

        var values = obj.GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Select(prop => Tuple.Create<string,object>(prop.Name, prop.GetValue(obj, null)));

        if (ignoreNullField)
            values = values.Where(tuple => tuple.Item2 != null);


        var result = values
                        .ToDictionary(key => key.Item1, val => val.Item2);

        return result;
    }

}

