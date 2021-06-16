using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.Model;
using Unity;

namespace WebApp.Controllers
{
    [AllowAnonymous]
    [Route("view/[controller]")]
    public class LoginController : CommonController
    {
        /// <summary>
        /// ログインページを表示する。
        /// </summary>
        /// <returns></returns>
        [Route("[action]")]
        public IActionResult Index()
        {
            if(HttpContext.User.Identity.IsAuthenticated)
            {
                 return RedirectToAction("Index", "Top");
            }
            return View();
            
        }

        /// <summary>
        /// ログアウト処理を行う。
        /// </summary>
        /// <returns></returns>
        [Route("[action]")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(Startup.AuthScheme);
            return View("Logout");
        }

        /// <summary>
        /// 閲覧権限がない場合に表示されるページ。
        /// </summary>
        /// <returns></returns>
        [Route("[action]")]
        public IActionResult Deny()
        {
            return View("Deny");
        }
    }

    [AllowAnonymous]
    [Route("api/login")]
    public class LoginApiController: CommonController
    {
        [Dependency]
        public LoginService service { get; set; }
        [Dependency]
        public ApplicationService appService { get; set; }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<RequestResult>> Login([FromBody] LoginRequestParameter parameter)
        {
            if (!ModelState.IsValid)
                return RequestResult.CreateErrorResult("");

            var now = DateTime.Now;

            var userInfo = await service.GetUserByLoginInfo(parameter.LoginId, parameter.Password);
            if (userInfo == null)
                return RequestResult.CreateErrorResult("ユーザーIDまたはパスワードが間違っています。");

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userInfo.UserNo.ToString())
                };

            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "login"));

            //一度ログアウト・セッションのクリアを行う。
            await HttpContext.SignOutAsync(Startup.AuthScheme);
            HttpContext.Session.Clear();

            //ログイン処理
            await HttpContext.SignInAsync(Startup.AuthScheme, principal, new AuthenticationProperties()
            {
                //IsPersistent = true
            });

            //セッションにユーザ情報をセットする。
            var context = new LoginUserContext(userInfo);
            HttpContext.Session.Set<LoginUserContext>(LoginUserContext.InSessionKey, context);

            //セッションにアプリケーション情報をセットする。
            var appInfo = await appService.GetApplicationInfo();
            var appContext = new ApplicationContext(appInfo);
            HttpContext.Session.Set<ApplicationContext>(ApplicationContext.InSessionKey, appContext);


            string redirectUrl = "";
            //ReturnUrlはjavascript側でurlから取得して設定している。
            if (!string.IsNullOrEmpty(parameter.ReturnUrl) && Url.IsLocalUrl(parameter.ReturnUrl))
                redirectUrl = new PathString(parameter.ReturnUrl);

            return RequestResult.CreateSuccessResult(new { RedirectUrl = redirectUrl });
  
        }
    }

    



}