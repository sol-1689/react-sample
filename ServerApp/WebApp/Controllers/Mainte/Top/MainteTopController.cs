using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Model;

namespace WebApp.Controllers
{
    /// <summary>
    /// マスタメンテ系機能のを受け付けるController。
    /// 各マスタ画面はクライアント側で切り替える。
    /// URLのルート定義はStartup.csで行っている。
    /// </summary>
    [AllowRole(AuthFlag.ADMIN)]
    public class MainteTopController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }
    }

}