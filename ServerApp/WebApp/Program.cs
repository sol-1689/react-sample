using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using Unity.Microsoft.DependencyInjection;

namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = LoadNLogConfig();
            var logger = factory.GetCurrentClassLogger();
            try
            {
                logger.Info("アプリ開始");

                //Dapperは標準の設定だとstringがnvarchar(4000)にマッピングされる。
                //SQLServerではvarcharとnvarcharを比較すると型変換が発生し、インデックスが効かなくなる。
                //Where句はvachar列と比較する事が多いので、デフォルトをvarcharに変更する。
                //DBの照合順序は「Japanese_CI_AS」の想定。
                Dapper.SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString);
                Dapper.SqlMapper.AddTypeMap(typeof(DateTime), System.Data.DbType.DateTime2);


                //SSLの自己証明書を許可。
                //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                //puppeteerからChromeを取得。
                //HtmlをPdfにする際にChromeを利用している為、起動時に初期化しておく。
                //ダウンロードが重い場合は最初からフォルダに配置しておく。(.local-chromium)
                //ファイルは以下からダウンロード
                //http://chromium.woolyss.com/download/
                var task = new PuppeteerSharp.BrowserFetcher().DownloadAsync(PuppeteerSharp.BrowserFetcher.DefaultRevision).Result;

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "アプリ開始でエラー。");
                throw;
            }
            finally
            {
                logger.Info("アプリ終了");
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        /// <summary>
        /// NLogの設定ファイルを読み込む。
        /// </summary>
        /// <returns></returns>
        private static LogFactory LoadNLogConfig()
        {
            try
            {
                var aspTargetEnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var factory = NLogBuilder.ConfigureNLog(string.IsNullOrEmpty(aspTargetEnvironmentName) ? "nlog.config" : $"nlog.{aspTargetEnvironmentName}.config");
                return factory;
            }
            catch (FileNotFoundException ex)
            {
                Trace.WriteLine(ex);
                return NLogBuilder.ConfigureNLog("nlog.config");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(s =>
                {
                    //Startupで参照する為に追加。
                    s.AddHttpContextAccessor();
                    s.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
                })
                .UseUnityServiceProvider() //DIコンテナをUnityに変更。
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    //NLog用設定
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog(); //NLog用設定
    }
}
