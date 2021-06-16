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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebApp.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Domain.Util;
using Domain.Service;

namespace WebApp
{
    /// <summary>
    /// Actionメソッドからthrowされた例外の処理を一括で行うクラス。
    /// </summary>
    public class ExceptionFilter : IExceptionFilter
    {

        /// <summary>エラー時のView名。 </summary>
        public string ErrorViewName { get; set; } = "Error"; //デフォルト。


        ILogger<ExceptionFilter> logger;
        IMailSendService mailSendService;
        ITempDataDictionaryFactory tempDataDictionaryFactory;
        IModelMetadataProvider modelMetadataProvider;

        public ExceptionFilter(IMailSendService mailSendService,
            ILogger<ExceptionFilter> logger,
            ITempDataDictionaryFactory tempDataDictionaryFactory,
            IModelMetadataProvider modelMetadataProvider)
        {
            this.mailSendService = mailSendService;
            this.logger = logger;
            this.tempDataDictionaryFactory = tempDataDictionaryFactory;
            this.modelMetadataProvider = modelMetadataProvider;
        }


        /// <summary>
        /// 各Actionメソッドから例外がthrowされた際に呼び出される。
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled)
                return; //他の例外で処理済なら何もしない。

            context.ExceptionHandled = true;

            Exception ex = context.Exception;
            logger.Error("エラーが発生しました。", LogType.Exception, ex: ex);

            string message = (ex as CustomException)?.Message ?? "エラーが発生しました。";

            _ = mailSendService.SendMail("【執務管理システム】 エラーが発生しました。", ex.ToString());

            //Ajaxもしくはformアプリからのリクエストの場合はJsonを返す。
            if (context.HttpContext.Request.IsApiOrAjaxRequest())
            {
                //Ajaxリクエストに対するresult。
                context.Result = WebUtil.JsonString(RequestResult.CreateErrorResult(message));
            }
            else if (StreamRequestAttribute.IsStreamFileRequest((context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo))
            {
                //Streamリクエストに対するresult。
                //とりあえずJsonと一緒。
                context.Result = WebUtil.JsonContent(RequestResult.CreateErrorResult(message));
            }
            else
            {
                ViewResult errorPageResult = new ViewResult()
                {
                    ViewName = ErrorViewName, //デフォルトでViews\SharedのError.cshtmlが呼ばれる。 
                    TempData = tempDataDictionaryFactory.GetTempData(context.HttpContext)
                };
                if (!string.IsNullOrEmpty(message))
                {
                    errorPageResult.ViewData = new ViewDataDictionary(modelMetadataProvider, context.ModelState);
                    errorPageResult.ViewData["ERROR_KEY"] = message;
                }
                context.Result = errorPageResult;
            }
        }

    }
}