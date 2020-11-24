using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// HttpContextの拡張クラス。
/// </summary>
public static class HttpContextExtentions
{
    /// <summary>
    /// 完全なURL文字列を取得する。
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetAbsoluteUrlBase(this HttpContext context)
    {
        string basePath = $"{context.Request.Scheme}://{context.Request.Host}";
        return basePath;
    }


    public static string GetAbsoluteUrl(this HttpContext context)
    {
        return $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
    }

    public static string GetAbsoluteUrlWithMethod(this HttpContext context)
    {
        return $"{context.Request.Method} {GetAbsoluteUrl(context)}";
    }
}