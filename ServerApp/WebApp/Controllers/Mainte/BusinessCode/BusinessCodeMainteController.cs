using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Model;
using Unity;

namespace WebApp.Controllers
{
    /// <summary>
    /// マスタメンテ系機能のを受け付けるController。
    /// 各マスタ画面はクライアント側で切り替える。
    /// URLのルート定義はStartup.csで行っている。
    /// </summary>
    //[ApiController] ModelStateやLoginUserContextを利用するため、Apiも通常のコントローラを用いる。
    [Route("api/mainte/businessCode")]
    [AllowRole(AuthFlag.ADMIN)]
    public class BusinessCodeMainteController: CommonController
    {
        [Dependency]
        public BusinessCodeMainteService service { get; set; }


        [Route("search")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<RequestResult>> Search([FromBody] BusinessCodeMainteSearchRequestParameter model)
        {
            if (!ModelState.IsValid)
                return RequestResult.CreateErrorResult(ModelState);

            var result = await service.GetList(model.SelectedAccountingCode, model.SearchDebitBusinessCode, model.SearchDebitBusinessName, model.ShowDelete, model.SortParams, model.PagingParam);
            //タプルのままだとjsonに変換できないので(名前付きタプルでもダメ)、匿名オブジェクトに変換。
            return RequestResult.CreateSuccessResult(new {result.TotalCount, result.List});
        }

        [Route("register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<RequestResult>> Register([FromBody] BusinessCodeRegisterModel model)
        {
            if (!ModelState.IsValid)
                return RequestResult.CreateErrorResult(ModelState);

            var result = await service.Create(model);
            if(result.Result)
                return RequestResult.CreateSuccessResult();
            else
                return RequestResult.CreateErrorResult(result.Message);
        }

        [Route("update")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<RequestResult>> Update([FromBody] BusinessCodeRegisterModel model)
        {
            if (!ModelState.IsValid)
                return RequestResult.CreateErrorResult(ModelState);

            var result = await service.Update(model);
            if(result.Result)
                return RequestResult.CreateSuccessResult();
            else
                return RequestResult.CreateErrorResult(result.Message);

        }
    }

}