using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApp.Mvc
{
    /// <summary>
    /// ファイルアップロード・ダウンロードなどのリクエストに対するアクションを表す。
    /// </summary>
    public class StreamRequestAttribute : ActionFilterAttribute
    {

        public StreamRequestAttribute()
        {
        }


        public override async Task OnResultExecutionAsync(ResultExecutingContext filterContext, ResultExecutionDelegate next)
        {
            //ファイルストリーム系のリクエストに対してJson形式のレスポンス返す場合は、ContentTypeでjsonを指定しないようにする。
            //そうしないとIEでダウンロードダイアログが表示されてしまう。

            var response = filterContext.HttpContext.Response;

            if (response.ContentType == null || response.ContentType.Contains("json"))
            {
                response.ContentType = "text/html"; //ContentTypeがjsonだった場合上書き。
            }
            await next();
        }

        //public override void OnResultExecuted(ResultExecutedContext filterContext)
        //{
        //    //ファイルストリーム系のリクエストに対してJson形式のレスポンス返す場合は、ContentTypeでjsonを指定しないようにする。
        //    //そうしないとIEでダウンロードダイアログが表示されてしまう。

        //    var response = filterContext.HttpContext.Response;

        //    if (response.ContentType.Contains("json"))
        //    {
        //        response.ContentType = "text/html"; //ContentTypeがjsonだった場合上書き。
        //    }

        //}


        /// <summary>
        /// ファイルアップロード・ダウンロードなどへのリクエストか。（隠しIframeへのリクエストを想定）
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        public static bool IsStreamFileRequest(ControllerContext controllerContext)
        {
            var actionMethod = controllerContext.ActionDescriptor.MethodInfo;
            return IsStreamFileRequest(actionMethod);
        }

        /// <summary>
        /// ファイルアップロード・ダウンロードなどへのリクエストか。（隠しIframeへのリクエストを想定）
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        public static bool IsStreamFileRequest(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                return false;

            var attribute = methodInfo.GetCustomAttributes(typeof(StreamRequestAttribute), false);

            bool isStreamFileRequest = attribute != null && attribute.Length > 0;
            return isStreamFileRequest;
        }

    }
}