using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// HttpRequestの拡張クラス
/// </summary>
public static class HttpRequestExtentions
{
    /// <summary>
    /// Ajaxリクエストかを判別する。
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static bool IsApiOrAjaxRequest(this HttpRequest request)
    {
        //ajaxリクエストか。
        bool isAjax = string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
            string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
            request.ContentType == "application/json";

        //apiへのリクエストか。
        bool isApi = request.Path.HasValue && request.Path.Value.Contains("/api/");

        return isAjax || isApi;
    }


    /// <summary>
    /// リクエストURLのコンテキストパスまでを元にしたURLの作成。
    /// </summary>
    /// <param name="request"></param>
    /// <param name="path"></param>
    /// <param name="needPathBase">コンテキストパスを付与するか。pathに既に含まれるならfalseにする。(デフォルト)</param>
    /// <returns></returns>
    public static string CreateUrl(this HttpRequest request, string path, bool needPathBase = false )
    {
        return $"{request.Scheme}://{request.Host.Value}{(needPathBase ? request.PathBase.ToString() : "")}{path}";
    }


    /// <summary>
    /// リクエストURLのホストをlocalhostに変更したURLを元に、当たらなURLを作成する。（帳票で使用）
    /// </summary>
    /// <param name="request"></param>
    /// <param name="path"></param>
    /// <param name="needPathBase">コンテキストパスを付与するか。pathに既に含まれるならfalseにする。(デフォルト)</param>
    /// <returns></returns>
    public static string CreateLocalUrl(this HttpRequest request, string path, bool needPathBase = false)
    {
        return $"{request.Scheme}://localhost:{request.Host.Port}{(needPathBase ? request.PathBase.ToString() : "")}{path}";
    }


    /// <summary>
    /// フルURLの取得。
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static string FullUrl(this HttpRequest request)
    {
        string result = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";
        return result;
    }

    static readonly IReadOnlyCollection<string> localhostHosts = new string[] { "localhost", "127.0.0.1"};
    /// <summary>
    /// localhost宛てのURLかを取得する。
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static bool IsLocalhostUrl(this HttpRequest request)
    {
        bool result = localhostHosts.Contains(request.Host.Host);
        return result;
    }
}