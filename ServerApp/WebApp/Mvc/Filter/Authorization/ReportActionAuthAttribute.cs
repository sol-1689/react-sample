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
using Domain.Service;
using Microsoft.AspNetCore.Http;

namespace WebApp
{
    /// <summary>
    /// Chromiumから帳票作成用に呼ばれる画面の権限チェックを行う。
    /// </summary>
    public class ReportActionAuthAttribute : TypeFilterAttribute
    {
        public ReportActionAuthAttribute()
            : base(typeof(InnerReportActionAuthAttributeAttribute))
        {
            Order = 0;
        }

        /// <summary>
        /// 内部クラス。メインの処理はここに記述。
        /// </summary>
        public class InnerReportActionAuthAttributeAttribute : IAuthorizationFilter
        {
            ILogger<ReportActionAuthAttribute> logger;

            public InnerReportActionAuthAttributeAttribute(ILogger<ReportActionAuthAttribute> logger)
            {
                this.logger = logger;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                //リクエストがlocalhost宛てかをチェック。
                bool isLocalhostUrl = context.HttpContext.Request.IsLocalhostUrl();
                //リクエストパラメータに認証用の秘密キーが設定されているかをチェック。
                var secret = context.HttpContext.Request.Query.Where(kv => kv.Key == ConstWeb.ReportCredentialParamName).FirstOrDefault();

                if(!isLocalhostUrl || !secret.Value.Contains(ConstWeb.ReportCredentialValue))
                {
                    logger.Error(errorMessage: "不正なリクエストです。");
                    context.Result = new ViewResult() { ViewName = "Error" };
                    return;
                }
                return;
            }
        }
    }
}
