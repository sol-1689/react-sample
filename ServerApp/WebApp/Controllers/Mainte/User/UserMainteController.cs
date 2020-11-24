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
    [Route("api/mainte/user")]
    [AllowRole(AuthFlag.ADMIN)]
    public class UserMainteController: CommonController
    {
        [Dependency]
        public UserMainteService serivice { get; set; }


        [Route("search")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<RequestResult>> Search([FromBody] UserMainteSearchRequestParameter model)
        {
            if (!ModelState.IsValid)
                return RequestResult.CreateErrorResult(ModelState);

            var result = await serivice.GetList(model);
            //タプルのままだとjsonに変換できないので(名前付きタプルでもダメ)、匿名オブジェクトに変換。
            return RequestResult.CreateSuccessResult(new {result.TotalCount, result.List});
        }


        [Route("detail")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<RequestResult>> SearchDetail([FromBody] UserMainteSearchRequestParameter model)
        {
            if (!ModelState.IsValid)
                return RequestResult.CreateErrorResult(ModelState);

            var result = await serivice.GetDetail(model);
            return RequestResult.CreateSuccessResult(result);
        }


        [Route("register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<RequestResult>> Register([FromBody] UserRegisterModel model)
        {
            if (!ModelState.IsValid)
                return RequestResult.CreateErrorResult(ModelState);

            model.CreateBy = model.UpdateBy = loginUserContext.UserNo;
            var result = await serivice.Create(model);
            if(result.Result)
                return RequestResult.CreateSuccessResult();
            else
                return RequestResult.CreateErrorResult(result.Message);
        }

        [Route("update")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<RequestResult>> Update([FromBody] UserRegisterModel model)
        {
            if (!ModelState.IsValid)
                return RequestResult.CreateErrorResult(ModelState);

            model.UpdateBy = loginUserContext.UserNo;
            var result = await serivice.Update(model);
            if(result.Result)
                return RequestResult.CreateSuccessResult();
            else
                return RequestResult.CreateErrorResult(result.Message);
        }

        [Route("delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<RequestResult>> Delete([FromBody] UserMainteDeleteRequestParameter model)
        {
            if (!ModelState.IsValid)
                return RequestResult.CreateErrorResult(ModelState);

            bool result = await serivice.Delete(model.UserNo.Value, model.IsDeleted.AsFlag(), loginUserContext.UserNo);
            return RequestResult.CreateSuccessResult(result);
        }

    }

}