
using Domain.Model;
using Domain.Util;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp
{
    public static class IConfigurationExtension
    {

        /// <summary>
        /// configファイルからSendGrid情報を取得する。
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static SendGridInfo GetSendGridInfo(this IConfiguration config)
        {
            var sendGridSection = config.GetSection("SendGrid");
            var sendGridInfo = new SendGridInfo(sendGridSection["ApiKey"], sendGridSection["DefaultFromAddress"], sendGridSection["DefaultToAddress"]);
            return sendGridInfo;
        }

        /// <summary>
        /// configファイルからContextPath情報を取得する。
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ContextPathInfo GetContextPathInfo(this IConfiguration config)
        {
            //コンテキストパス情報
            string contextPath = config.GetValue<string>("ContextPath");
            var result = new ContextPathInfo() { ContextPath = contextPath };
            return result;

        }

    }
}
