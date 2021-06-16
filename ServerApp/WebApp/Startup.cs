using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApp.Mvc;
using System;
using System.Data.SqlClient;
using Unity;
using Unity.Lifetime;
using Unity.Microsoft.DependencyInjection;

namespace WebApp
{
    public class Startup
    {
        /// <summary>認証スキーマ </summary>
        public const string AuthScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        public IConfiguration configuration { get; }
        IWebHostEnvironment Env { get; }
        //IHttpContextAccessor httpContextAccessor { get; set; }
        //IServiceProvider provider { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment Env)
        {
            this.configuration = configuration;
            this.Env = Env;
            //this.httpContextAccessor = httpContextAccessor;
            //this.provider = provider;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Jsonを設定する。
            services.Json();
            //静的リソースを設定する。
            services.StaticFile();

            //Sessionの設定
            services.Session(configuration);

            //言語ファイルを設定する。
            services.Localize();
            
            //ヘルスチェック設定を定義する。
            services.HealthCheck();

            //MVCの設定
            services.Mvc();

            //Razorの設定
            services.Razor(Env);

            //認証用の設定
            services.Authentication(configuration);

            //承認用の設定
            services.Authorization();

            //DI用の設定
            services.DI(configuration);

            //レスポンスをgZip圧縮するように設定。
            services.Compression();

            //UnityContainer container = IServiceCollectionExtentions.ConfigureContainer(configuration);
            //var unityProvider = services.BuildServiceProvider(container);
            //return unityProvider;
        }


        /// <summary>
        /// UnityContainerに直接設定したい場合。
        /// 名前付きの登録など。ConfigureServicesメソッドの後に呼ばれる。
        /// </summary>
        /// <param name="container"></param>
        public void ConfigureContainer(IUnityContainer container)
        {
            IServiceCollectionExtentions.ConfigureContainer(container, configuration);
        }

        /// <summary>
        /// HTTPリクエストのパイプラインを構築する。
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression(); //gZip処理。UseStaticFilesより前に記述しないとjsが圧縮されない。
            
            // webサーバー(nginxなど)からこちらのアプリにリクエストが転送される際に、httpsがhttpに変換される。
            // その状態でリダイレクトレスポンスを返すと、Httpスキーマのurlが返ってしまう。
            // 元のリクエストのスキーマはヘッダーのX-Forwarded-Protoに記述されている為、その値を使用するよう以下を設定する。
            // 診断およびエラー処理ミドルウェア以外のミドルウェアより前に記述する必要がある。
            // 参考: https://tech.tanaka733.net/entry/http-header-override-behind-a-proxy-server-in-aspnetcore
            // https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-2.1&tabs=visual-studio#http-strict-transport-security-protocol-hsts
            
            // XForwardedHostはAzureFrontDoorなどの背後に配置するAppServiceの場合などに必要（設定しないとリダイレクトのURLがAppServiceのものになってしまう）
            // https://www.phillipsj.net/posts/using-azure-front-door-with-dotnet-core/
            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost,
                RequireHeaderSymmetry = false
            };
            // 下記Known～にマッチする転送元からのリクエストのみ、ForwardedHeaderを受け入れる。
            // だがデフォルトだとループバックアドレス(127.0.0.1とか)のみが指定されている為、大体のケースでマッチしない。
            // なので、クリアしておく。
            // https://docs.microsoft.com/ja-jp/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-3.1
            // https://stackoverflow.com/questions/43860128/asp-net-core-google-authentication/
            forwardedHeadersOptions.KnownNetworks.Clear();
            forwardedHeadersOptions.KnownProxies.Clear();

            //本来はXForwardedに指定可能なドメインを設定すべき。環境変数から取るか？
            //forwardedHeadersOptions.AllowedHosts = new List<string>() { "myfrontdoor.azurefd.net", "app.domain.com", "myapp.azurewebsites.net" };
            app.UseForwardedHeaders(forwardedHeadersOptions);

            // エラーページの設定
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // HTTPSの強制などの設定
                // https://aka.ms/aspnetcore-hsts
                //app.UseHsts();
            }
            
            //チェックを行うサービスはConfigureServices()の方で指定している。

            //ContextPathの指定。(1サイトに複数アプリを展開し(サブアプリケーション)、エイリアスで振り分けたい場合に使用。)
            string pathBase = configuration.GetValue<string>("ContextPath");
            if (!string.IsNullOrEmpty(pathBase))
            {
                //IISはこちらのみでよさそう。リダイレクトにPathBaseの値が付与される。
                app.Use((context, next) =>
                {
                    context.Request.PathBase = pathBase;
                    return next();
                });
                //こっちはIISで不要っぽい。(サブアプリケーションに振り分けられる時点で、エイリアス部分がtrimされている?為。)
                //app.UsePathBase(pathBase);
            }

            //app.UseHttpsRedirection();

            //UseRoutingよりも前に記述する。
            app.UseStaticFiles();
            if (env.IsDevelopment())
            {
                app.UseSpaStaticFiles(); //reactのビルド結果をwwwroot下に配置するようにしたので、不要。
            }

            //クッキーの設定を一律で行う。UseSession等の前に書かないとダメっぽい。
            app.UseCookiePolicy(new CookiePolicyOptions()
            {
                HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.SameAsRequest,
                //MinimumSameSitePolicy = SameSiteMode.Strict
            });

            app.UseSession(); //セッション

            app.UseAuditLog();  // 監査ログ対応

            //クリックジャッキング対策ヘッダを設定。
            app.Use(async (ctx, next) =>
            {
                ctx.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                ctx.Response.Headers.Add("Content-Security-Policy", "frame-ancestors 'self'");
                await next();
            });


            //ルーティング設定
            app.UseRouting();
           
            //認証。認可。UseRoutingとUseEndpointsの間に記述する。
            //呼び出し順参考⇒https://docs.microsoft.com/ja-jp/aspnet/core/migration/22-to-30?view=aspnetcore-3.1&tabs=visual-studio
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization(); 

            app.UseEndpoints(endpoints =>
            {
            	//ロードバランサから送られてくるヘルスチェックリクエストを受け付けるパスを定義。
                endpoints.MapHealthChecks("/health");
                
                //view/mainte/～とあればメンテ系のトップ画面へ遷移する。
                endpoints.MapControllerRoute(
                    name: "mainteSpaRoot",
                    pattern: "view/mainte/{*target}",
                    defaults: new { controller = "MainteTop", action = "Index" });

                //view/schedule/～とあれば予定系のトップ画面へ遷移する。
                endpoints.MapControllerRoute(
                    name: "mainteSpaRoot",
                    pattern: "view/schedule/{*target}",
                    defaults: new { controller = "ScheduleTop", action = "Index" });

                //そのほかは通常のcontroller/action形式で遷移。
                //ただしAPI系はControllerのRoute要素で直接パスを指定している。
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}",
                    defaults: new { controller = "Top", action = "Index" });
            });


            //開発時は、↑のEndpoints設定にマッチしないリクエストをSpaリクエストとして、webpack_devserverに転送する。
            if (env.IsDevelopment())
            {
                app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp";

                    //webpack_devserverはC#側のデバッグで自動的に立ち上がらない。。
                    //コンソールから、ClientAppをカレントディレクトリとし、npm run debug を実行し、自分で立ち上げる。

                    //spa.UseReactDevelopmentServer(npmScript: "start");
                    //プロジェクト右クリック→プロパティ→デバッグの「SSLを有効にする」のチェックを外すこと。
                    //外さないと、HMR用のリクエストがhttpsになり、通信ができなくなる。
                    //(チェックを外さないままwebpack-devserverをssl:trueにして、「dotnet dev-certs https --trust」を実行してlocalhostの証明書を信頼してもダメだった。)
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:8083");
                    //spa.UseProxyToSpaDevelopmentServer("https://localhost:8083"); //webpack_devserverでhttps:trueを設定した場合はhttpsにする？
                });
            }
        }
    }
}
