using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SAIN.Util
{
    /// <summary>
    /// 監査ログ
    /// </summary>
    public static class AuditLogHelper
    {
        

        private static string CreateRequestUrl(HttpContext context)
        {
            return $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
        }

        /// <summary>
        /// リクエスト開始の監査ログを出力する
        /// </summary>
        /// <param name="context"></param>
        public static void RequestStartLog(HttpContext context)
        {
            LogImpl(LogType.RequestStart, CreateRequestUrl(context));
        }

        /// <summary>
        /// SQL発行の監査ログを出力する
        /// </summary>
        /// <param name="sql">発行したSQL</param>
        public static void ExecutedSqlLog(string sql)
        {
            LogImpl(LogType.ExecutedSql, sql);
        }

        /// <summary>
        /// SQL発行の監査ログを出力する
        /// </summary>
        /// <param name="sql">発行したSQL</param>
        /// <param name="result">影響件数</param>
        public static void ExecutedSqlSql(string sql, int result)
        {
            LogImpl(LogType.ExecutedSql, sql, result);
        }
        
        /// <summary>
        /// リクエスト終了の監査ログを出力する
        /// </summary>
        /// <param name="context"></param>
        public static void RequestEndLog(HttpContext context)
        {
            LogImpl(LogType.RequestEnd, CreateRequestUrl(context));
        }

        /// <summary>
        /// リクエストエラーの監査ログを出力する
        /// </summary>
        /// <param name="context"></param>
        public static void RequestErrorLog(HttpContext context, string errorMessage)
        {
            LogImpl(LogType.RequestError, CreateRequestUrl(context), errorMessage: errorMessage);
        }

        /// <summary>
        /// リクエストエラーの監査ログを出力する
        /// </summary>
        /// <param name="context"></param>
        public static void RequestErrorLog(string sql, string errorMessage)
        {
            LogImpl(LogType.RequestError, sql, errorMessage: errorMessage);
        }

        private static void LogImpl(LogType message, string content, int? result = null, string errorMessage = null)
        {
            var logger = LogManager.GetLogger("customLogger");

            var defaultEvent = new LogEventInfo(LogLevel.Info, "", ConvertMessage(message));

            defaultEvent.Properties["logType"] = content;
            defaultEvent.Properties["result"] = result;
            defaultEvent.Properties["errormessage"] = errorMessage;

            logger.Log(defaultEvent);
        }

    }
}