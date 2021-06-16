using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unity;

namespace WebApp.Controllers
{
    public class CommonController : Controller
    {
        /// <summary>ロガー。 </summary>
        [Dependency]
        public ILogger<CommonController> _logger { get; set; }

        [Dependency]
        public LoginUserContext loginUserContext { get; set; }

        [Dependency]
        public ApplicationContext applicationContext { get; set; }


        #region レポート用

        /// <summary>
        /// レポートview呼び出し用の秘密キーを付与したリクエストパラメータを作成する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        protected Dictionary<string, object> AddReportCredential(object model = null)
        {
            if (model == null)
                return new Dictionary<string, object>() { { ConstWeb.ReportCredentialParamName, ConstWeb.ReportCredentialValue } };

            //モデルをDictionaryに変換。
            var dict = model.ConvertToDictionary(false);

            //ReportのViewはChromiumから呼び出される。
            //その為Cookieの認証情報などは送られてこないので、通常の認証ではなく、
            //リクエストに秘密キーが含まれているかで認証することとする。
            dict.Add(ConstWeb.ReportCredentialParamName, ConstWeb.ReportCredentialValue); //秘密キーを付与。

            return dict;
        }
        #endregion

    }
}