using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using WebApp.Controllers;
using WebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp
{
    /// <summary>
    /// 監査ログ用のActionメソッドの属性
    /// </summary>
    public class AppIdAttribute : TypeFilterAttribute
    {
        public string AppId { get; private set; }
        /// <summary>
        /// 機能IDを指定して各Actionに定義
        /// </summary>
        /// <param name="appId"></param>
        public AppIdAttribute(string appId)
            : base(typeof(AuditLogFilter))
        {
            Order = 1;
            this.Arguments = new string[] { appId };
            AppId = appId;
        }

        /// <summary>
        /// 監査ログ用のFilter
        /// </summary>
        private class AuditLogFilter : IAsyncActionFilter
        {
            private readonly LoginUserContext loginContext;
            private readonly string appId;
            ILogger<AuditLogFilter> logger;

            public AuditLogFilter(LoginUserContext loginContext, string appId, ILogger<AuditLogFilter> logger)
            {
                this.loginContext = loginContext;
                this.appId = appId;
                this.logger = logger;
            }
            
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                // 機能IDをセット
                context.HttpContext.Items["appid"] = appId;

                //AuditLogHelper.RequestStartLog(context.HttpContext);
                //logger.Info(context.HttpContext.CreateAbsoluteUrlWithMethod(), LogType.RequestStart);
                
                //エラーがなければactionを実行。
                await next();
            }

        }
    }

}
