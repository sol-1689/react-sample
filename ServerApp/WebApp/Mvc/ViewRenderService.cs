using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp
{
    /// <summary>
    /// cshtmlをStringに変換する機能を保持する。
    /// </summary>
    public interface IViewRenderService
    {
        string RenderToString(string viewPath, ViewDataDictionary viewData);
        string RenderToString<TModel>(string viewPath, TModel model, ViewDataDictionary viewData);
        Task<string> RenderToStringAsync(string viewName, ViewDataDictionary viewData);
        Task<string> RenderToStringAsync<TModel>(string viewName, TModel model, ViewDataDictionary viewData);
    }

    /// <summary>
    /// cshtmlをStringに変換する機能を保持したクラス。
    /// </summary>
    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;

        public ViewRenderService(IRazorViewEngine viewEngine, IHttpContextAccessor httpContextAccessor,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IActionContextAccessor actionContextAccessor)
        {
            _viewEngine = viewEngine;
            _httpContextAccessor = httpContextAccessor;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _actionContextAccessor = actionContextAccessor;
        }


        public string RenderToString(string viewPath, ViewDataDictionary viewData)
        {
            return RenderToString(viewPath, string.Empty, viewData);
        }

        public string RenderToString<TModel>(string viewPath, TModel model, ViewDataDictionary viewData)
        {
            try
            {
                var viewEngineResult = _viewEngine.GetView("~/", viewPath, false);

                if (!viewEngineResult.Success)
                {
                    throw new InvalidOperationException($"Couldn't find view {viewPath}");
                }

                var view = viewEngineResult.View;

                using (var sw = new StringWriter())
                {
                    viewData.Model = model;
                    var viewContext = new ViewContext()
                    {
                        HttpContext = _httpContextAccessor.HttpContext ?? new DefaultHttpContext { RequestServices = _serviceProvider },
                        ViewData = viewData,
                        Writer = sw
                    };
                    view.RenderAsync(viewContext).GetAwaiter().GetResult();
                    return sw.ToString();
                }
            }
            catch 
            {
                throw;
            }
        }


        public Task<string> RenderToStringAsync(string viewName, ViewDataDictionary viewData)
        {
            return RenderToStringAsync<string>(viewName, string.Empty, viewData);
        }

        public async Task<string> RenderToStringAsync<TModel>(string viewName, TModel model, ViewDataDictionary viewData)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext ?? new DefaultHttpContext { RequestServices = _serviceProvider };
                //var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
                var actionContext = _actionContextAccessor.ActionContext ?? new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

                using (var sw = new StringWriter())
                {
                    var viewResult = _viewEngine.FindView(actionContext, viewName, false);


                    // Fallback - the above seems to consistently return null when using the EmbeddedFileProvider
                    if (viewResult.View == null)
                    {
                        viewResult = _viewEngine.GetView("~/", viewName, false);
                    }

                    if (viewResult.View == null)
                    {
                        throw new ArgumentNullException($"{viewName} does not match any available view");
                    }

                    viewData.Model = model;
                    var viewDictionary = viewData;

                    //var viewDictionary = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(),
                    //        _actionContextAccessor.ActionContext?.ModelState ?? new ModelStateDictionary())
                    //{
                    //    Model = model
                    //};

                    var viewContext = new ViewContext(
                        actionContext,
                        viewResult.View,
                        viewDictionary,
                        new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                        sw,
                        new HtmlHelperOptions()
                    );

                    await viewResult.View.RenderAsync(viewContext);
                    return sw.ToString();
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
