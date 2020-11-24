using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Service;
using Unity;
using Domain.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mime;
using System.Text;

namespace WebApp.Controllers
{
    
    /// <summary>
    /// トップページ用コントローラ。
    /// </summary>
    public class TopController:CommonController
    {

        /// <summary>
        /// トップページにリダイレクトする。
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            //とりあえずスケジュールのトップに飛ばす。
            return RedirectToAction("Index", "MainteTop");
        }

    }
}
