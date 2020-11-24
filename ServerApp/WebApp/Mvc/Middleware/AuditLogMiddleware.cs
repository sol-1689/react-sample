using Domain.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebApp.Mvc
{
    /// <summary>
    /// Httpリクエストの内容をログに出力する。
    /// </summary>
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;
        ILogger logger;

        public AuditLogMiddleware(RequestDelegate next,
                                    ILoggerFactory loggerFactory)
        {
            _next = next;
            logger = loggerFactory.CreateLogger<AuditLogMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
        	//ヘルスチェックリクエストの場合はログに出力しない。
            if (context.Request.Path.Value.Contains("/health"))
            {
                await _next(context);
                return;
            }
            
            var serverIp = Dns.GetHostAddresses(Dns.GetHostName()).Where(r => r.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).FirstOrDefault();
            var loginUser = context.Session?.Get<LoginUserContext>(LoginUserContext.InSessionKey);

            context.Items["serverip"] = serverIp;
            context.Items["userno"] = loginUser?.UserNo;

            logger.Info(context.GetAbsoluteUrlWithMethod(), LogType.RequestStart, result: context.Response.StatusCode);

            await _next(context);

            logger.Info("", LogType.RequestEnd, result: context.Response.StatusCode);
        }
    }

    public static class AuditLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuditLog(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuditLogMiddleware>();
        }
    }
}
