using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Controllers;
using WebApp.Model;
using WebApp.Service;
using Microsoft.AspNetCore.Http;

namespace WebApp.Mvc
{
    /// <summary>
    /// Apiの認証処理を表す属性。
    /// </summary>
    public class ApiAuthenticationAttribute : TypeFilterAttribute
    {
        //public const string ApiUserInItemsKey = "ApiUserInItemsKey";

        public const string AppIdHeaderName = "app_id";
        public const string AppIdInItemsKey = "AppIdInItemsKey";

        public ApiAuthenticationAttribute()
            : base(typeof(InnerApiAuthenticationAttribute))
        {
            Order = 0;
        }

        /// <summary>
        /// filter。
        /// </summary>
        public class InnerApiAuthenticationAttribute : IAsyncAuthorizationFilter
        {
            ILogger<ApiAuthenticationAttribute> logger;
            //ApiAuthenticationService service;

            public InnerApiAuthenticationAttribute(ILogger<ApiAuthenticationAttribute> logger)
            {
                this.logger = logger;
                //this.service = service;
            }

            public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
            {
                //Apiのシークレットキーは全Apiリクエストに対して検査。
                if (!CheckSecretApiKey(context.HttpContext))
                {
                    logger.Error(errorMessage: "APIの認証キーが不正です。");
                    context.Result = new StatusCodeResult(ApiExceptionFilterAttribute.ApiErrorCode);
                    return;
                }

                //リクエストの機能IDを保持。
                //var appId = context.ActionDescriptor.GetAttribute<AppIdAttribute>()?.AppId;
                string appId = context.HttpContext.Request.Headers.TryGetValue(AppIdHeaderName, out var o) ? o.ToString() : "";
                context.HttpContext.Items.Add(AppIdInItemsKey, appId);
                logger.Info($"リクエストAPI-ID:{appId}");


                bool noApiAuthCheck = context.ActionDescriptor.HasAttribute<NoApiAuthCheckAttribute>();
                if (noApiAuthCheck)
                {
                    return;
                }

                //bool isAuthUser = await CheckUserToken(context);
                //if (!isAuthUser)
                //{
                //    context.Result = new ForbidResult();
                //    return;
                //}

            }


            const string UserSecretApiKeyHeaderName = CommonStrings.ApiParameter.UserSecretApiKeyHeaderName;
            const string SecretApiKeyValue = CommonStrings.ApiParameter.SecretApiKeyValue;

            private bool CheckSecretApiKey(HttpContext context)
            {
                string secretKey = context.Request.Headers[UserSecretApiKeyHeaderName];
                
                bool result = secretKey == SecretApiKeyValue;
                if(!result)
                    logger.Info($"リクエストSecretKey:{secretKey}");

                return result;
            }


        }
    }

    /// <summary>
    /// 権限チェックを行わないApiに付与する属性。
    /// </summary>
    public class NoApiAuthCheckAttribute : Attribute
    {

    }
}
