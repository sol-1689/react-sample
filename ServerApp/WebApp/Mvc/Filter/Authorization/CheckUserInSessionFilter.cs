using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Authorization;
using Domain.Model;

namespace WebApp
{
    /// <summary>
    ///Sessionのユーザ情報が存在するかをチェックするクラス。
    /// </summary>
    public class CheckUserInSessionFilter : IAsyncAuthorizationFilter
    {
        ILogger<CheckUserInSessionFilter> logger;
        public CheckUserInSessionFilter(ILogger<CheckUserInSessionFilter> logger)
        {
            this.logger = logger;
        }


        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            bool isAnonymous = context.ActionDescriptor.HasAttribute<AllowAnonymousAttribute>();

            if (isAnonymous)
                return;

            //TODO
            var userInfo = context.HttpContext.Session.Get<LoginUserContext>(LoginUserContext.InSessionKey);

            if (userInfo== null)
            {
                logger.Info("セッション内にユーザ情報がありません。ログイン画面に遷移します。");

                //セッションにユーザ情報がなければログアウト状態にする。
                await context.HttpContext.SignOutAsync(Startup.AuthScheme);

                //Ajaxの場合は401コードを返す(Javascript側でログインページにリダイレクト。)
                if (context.HttpContext.Request.IsApiOrAjaxRequest())
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    string returnUrl = context.HttpContext.Request.Headers["Referer"].ToString(); //ログイン後に遷移するURL
                    returnUrl = WebUtil.ToLocalUri(returnUrl); //ドメイン外のサイトに飛ばないよう、ローカルURL(http://ドメイン部分を除いたURL)に変換

                    string redirectUrl = $"{new PathString($"{IServiceCollectionExtentions.LoginPageUrl}")}?{IServiceCollectionExtentions.ReturnUrlParam}={returnUrl}";
                    context.HttpContext.Response.Headers.Add("redirectUrl", redirectUrl);
                    
                    context.Result = new EmptyResult();
                }
                else
                {
                    //context.Result = new RedirectResult(new PathString(IServiceCollectionExtentions.LoginPageUrl));
                    string originalUrl = context.HttpContext.Request.FullUrl();
                    string returnUrl = WebUtil.ToLocalUri(originalUrl); //ドメイン外のサイトに飛ばないよう、ローカルURL(http://ドメイン部分を除いたURL)に変換

                    // //Ajax以外の場合は通常のログイン画面へのリダイレクト処理。
                    var redirectUrl = $"{new PathString($"{IServiceCollectionExtentions.LoginPageUrl}")}?{IServiceCollectionExtentions.ReturnUrlParam}={returnUrl}";
                    context.Result = new RedirectResult(redirectUrl);
                }
            }

        }
    }

}