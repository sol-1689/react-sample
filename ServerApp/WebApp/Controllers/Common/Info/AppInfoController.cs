using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Model;
using Unity;

namespace WebApp.Controllers
{
    [Route("api/info")]
    public class AppInfoController : CommonController
    {
        [Dependency]
        public KubunService service { get; set; }

        //cshtmlのSpaLayoutにて直接javascriptに設定するため、APIでは取得しないこととする。
        // [Route("loginUser")]
        // [HttpPost]
        // public RequestResult GetLoginUserInfo()
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return RequestResult.CreateErrorResult(ModelState);
        //     }

        //     return RequestResult.CreateSuccessResult(new
        //     {
        //         UserNo = loginUserContext.UserNo,
        //         Role = loginUserContext.Role,
        //         Auth = loginUserContext.Auth,
        //         IsAdmin = loginUserContext.IsAdmin,
        //         DivisionNoList = loginUserContext.DivisionNoList
        //     });
        // }

    }
}