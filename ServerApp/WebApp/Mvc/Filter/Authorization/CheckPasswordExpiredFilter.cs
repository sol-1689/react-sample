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
using WebApp.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace WebApp
{
    /// <summary>
    /// ログインパスワードの有効期限が切れているかをチェックするクラス。
    /// </summary>
    public class CheckPasswordExpiredFilter : IAsyncAuthorizationFilter
    {
        ILogger<CheckPasswordExpiredFilter> logger;
        public CheckPasswordExpiredFilter(ILogger<CheckPasswordExpiredFilter> logger)
        {
            this.logger = logger;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            bool isAnonymous = context.ActionDescriptor.HasAttribute<AllowAnonymousAttribute>();

            if (isAnonymous)
                return;

            var userInfo = context.HttpContext.Session.Get<LoginUserContext>(CommonStrings.COMMON_INFO_IN_SESSION);
            bool isNoCheck = context.ActionDescriptor.HasAttribute<NoCheckPasswordExpiredAttribute>();

            if (userInfo?.PasswordExpired == true && !isNoCheck)
            {
                logger.Info("パスワードの有効期限切れ。パスワード変更画面に遷移します。");
                string passwordChangeUrl = new PathString("/K0030_ChangePassword/Index");

                //パスワードの有効期限が切れている場合は、パスワード変更画面にしかいけない。
                if (context.HttpContext.Request.IsAjaxRequest())
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Result = WebUtil.JsonContent(AjaxResult.CreateRedirectResult(passwordChangeUrl));
                }
                else
                {
                    context.Result = new RedirectResult(passwordChangeUrl);
                }
            }

        }
    }

    public class NoCheckPasswordExpiredAttribute : Attribute
    {

    }

}