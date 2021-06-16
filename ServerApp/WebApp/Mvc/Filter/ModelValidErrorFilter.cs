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

/// <summary>
/// モデルのvalidate処理失敗のログ出力処理を一括で行うフィルター。
/// </summary>
public class ModelValidErrorFilter : IActionFilter
{

    ILogger<ModelValidErrorFilter> logger;


    public ModelValidErrorFilter(ILogger<ModelValidErrorFilter> logger)
    {
        this.logger = logger;
        
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            string errorMessage = context.ModelState.GetAllErrorMessge();
            logger.Error("不正なパラメータです。", LogType.ParameterError, errorMessage:errorMessage);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}