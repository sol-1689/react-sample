using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Controllers;
using WebApp.Service;
using Microsoft.AspNetCore.Http;

namespace WebApp.Mvc
{
    /// <summary>
    /// Apiの例外処理を表す属性。
    /// </summary>
    public class ApiExceptionFilterAttribute : TypeFilterAttribute
    {
        public const int ApiErrorCode = 599;

        public ApiExceptionFilterAttribute()
            : base(typeof(InnerApiExceptionFilterAttribute))
        {
            Order = 0;
        }

        /// <summary>
        /// filter。
        /// </summary>
        public class InnerApiExceptionFilterAttribute : ExceptionFilterAttribute
        {
            ILogger<ApiExceptionFilterAttribute> logger;
            SendGridHelper sendGridHelper;

            public InnerApiExceptionFilterAttribute(SendGridHelper sendGridHelper, ILogger<ApiExceptionFilterAttribute> logger)
            {
                this.sendGridHelper = sendGridHelper;
                this.logger = logger;
            }

            public override void OnException(ExceptionContext context)
            {
                if (context.ExceptionHandled)
                    return; //他の例外で処理済なら何もしない。

                context.ExceptionHandled = true;

                Exception ex = context.Exception;
                logger.Error("Apiエラーが発生しました。", LogType.Exception, ex:ex);
        
                //今回の案件では599を返す。
                context.Result = new StatusCodeResult(ApiErrorCode);
            }
        }
    }
}
