using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication;
using WebApp.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Reflection;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Hosting;
using Unity;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using System.IO;
using Unity.Lifetime;
using Domain.Service;

using Domain.Repository;
using Domain.Model;
using Microsoft.Extensions.Hosting;
using Domain.Util;
using StackExchange.Redis;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApp
{
    /// <summary>
    /// IServiceCollectionの拡張メソッド。
    /// </summary>
    public static class IServiceCollectionExtentions
    {
        /// <summary>認証の有効期限。 </summary>
        static readonly TimeSpan AuthTimeoutSpan = TimeSpan.FromMinutes(300);

        /// <summary>
        /// Sessionを設定する。
        /// </summary>
        /// <param name="services"></param>
        public static void Session(this IServiceCollection services, IConfiguration Configuration)
        {
            //Sessionの設定
            var sessionConnectionString = Configuration.GetConnectionString("AppCache");
            //Console.WriteLine($"調査用_Redis接続文字列【{sessionConnectionString}】");

            if (string.IsNullOrEmpty(sessionConnectionString))
            {
                Console.WriteLine("Sessionの格納にメモリキャッシュを利用");
                //Radisなどへの接続文字列が指定されていない場合(APIサーバなどセッションを使わない場合)は、仮でメモリを使用する。
                services.AddDistributedMemoryCache();
            }
            else
            {
                Console.WriteLine("Sessionの格納にRedisキャッシュを利用");
                //セッション情報はRedisに格納するよう修正
                services.AddStackExchangeRedisCache(redisCacheConfig =>
                {
                    redisCacheConfig.ConfigurationOptions = ConfigurationOptions.Parse(sessionConnectionString);
                });
            }

            services.AddSession(o =>
            {
                //コンテキストパスがある場合はCookie名に付与する。(同じブラウザでみた場合にクッキー値を上書きしないように。)
                var contextPathInfo = Configuration.GetContextPathInfo();

                o.Cookie.Name = $".MM.Session.{contextPathInfo.NoSlashContextPath}";
                o.IdleTimeout = AuthTimeoutSpan; //セッションの有効期間
                //o.Cookie.HttpOnly = true;
                //o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                o.Cookie.SameSite = SameSiteMode.None;
                o.Cookie.IsEssential = true;
            });
        }


        /// <summary>
        /// Jsonを設定する。
        /// </summary>
        /// <param name="services"></param>
        public static void Json(this IServiceCollection services)
        {
            services.AddControllersWithViews()
             //Json
             .AddNewtonsoftJson(options => {
                    //デフォルトだとJson戻り値の先頭文字が小文字に変換されてしまう。
                    //大文字のままにするように以下を設定。
                    options.SerializerSettings.ContractResolver
                     = new Newtonsoft.Json.Serialization.DefaultContractResolver();
             })
             .AddJsonOptions(options =>
             {
                    //デフォルトだとJson戻り値の先頭文字が小文字に変換されてしまう。
                    //大文字のままにするように以下を設定。
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                 options.JsonSerializerOptions.PropertyNamingPolicy = null;
             });
        }

        /// <summary>
        /// 静的リソースを設定する。
        /// </summary>
        /// <param name="services"></param>
        public static void StaticFile(this IServiceCollection services)
        {
            // 本番環境では以下のディレクトリにコンパイル後のjsなどが保存される。
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        /// <summary>
        /// 言語ファイルを設定する。
        /// </summary>
        /// <param name="services"></param>
        public static void Localize(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>
                        {
                            new CultureInfo("ja")
                        };

                    options.DefaultRequestCulture = new RequestCulture(culture: "ja", uiCulture: "ja");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;

                    options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());

                }
            );
        }
        
        /// <summary>
        /// ヘルスチェックを設定する。
        /// </summary>
        /// <param name="services"></param>
        public static void HealthCheck(this IServiceCollection services)
        {
            //設定方法
            //TODO DBに適当なクエリを投げてみて、成功した際のみOKを返す。
            //https://docs.microsoft.com/ja-jp/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-3.1
            services.AddHealthChecks()
                .AddCheck("ok", () => HealthCheckResult.Healthy("OK")); //無条件でOKを返す。

            //ヘルスチェックリクエストを受け付けるパスは、Startup.Configureにて定義している。
        }

        /// <summary>
        /// MVCの設定をする。
        /// </summary>
        /// <param name="services"></param>
        public static void Mvc(this IServiceCollection services)
        {
            //MVCの設定
            services.AddMvc(op =>
            {
                //グローバルフィルタの設定
                op.Filters.Add(typeof(ExceptionFilter));

                //この記述が無いと、CustomPolicyProviderと併用した際に、policyがnull的なエラーになった。
                //op.AllowCombiningAuthorizeFilters = false;

                //デフォルトで全コントローラーに[Authorize]属性がついた状態にする。
                //ログイン無しで利用できるControllerもしくはActionには、[AllowAnonymous]属性を付与する。
                var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

                op.Filters.Add(new AuthorizeFilter(policy));
                op.Filters.Add(typeof(CheckUserInSessionFilter));
                //op.Filters.Add(typeof(CheckPasswordExpiredFilter));
                op.Filters.Add(typeof(ModelValidErrorFilter));

                //ControllerNameAttributeの有効化。
                op.Conventions.Add(new ControllerNameAttributeConvention());

                //2.2からのルーティングを無効に。(これが有効だと、viewでUrl.Action使用した際に先頭にapi/が付いてしまったので。)
                //op.EnableEndpointRouting = false;

            })
                //
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                //コントローラにUnityでプロパティインジェクションする為に必要。
                //(デフォルトだと、Unityコンテナに差し替えてもコントローラはコンストラクタインジェクションしか効かない。)
                .AddControllersAsServices()
                .AddJsonOptions(options =>
                {
                    //デフォルトだとJson戻り値の先頭文字が小文字に変換されてしまう。
                    //大文字のままにするように以下を設定。
                    //options.SerializerSettings.ContractResolver
                    //    = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                })
                .AddSessionStateTempDataProvider() //TempData用
                .AddViewLocalization() //言語ファイル用
                //.AddDataAnnotationsLocalization(options =>
                //{
                //    options.DataAnnotationLocalizerProvider = (type, factory) =>
                //    {
                //        var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
                //        return factory.Create("SharedResource", assemblyName.Name);
                //    };
                //})
                //RazorのSharedフォルダを追加。
                //.AddRazorOptions(opt =>
                //{
                //    opt.ViewLocationFormats.Add("/Views/Shared/Dialog/{0}.cshtml");
                //})
                // 日付入力欄の書式チェックを追加
                //.AddViewOptions(opt => opt.ClientModelValidatorProviders.Add(new CustomDateFormatModelValidatorProvider()))
                ;

        }

        /// <summary>
        /// Razorの設定をする。
        /// </summary>
        /// <param name="services"></param>
        public static void Razor(this IServiceCollection services, IWebHostEnvironment Env)
        {
            IMvcBuilder builder = services.AddRazorPages();

#if DEBUG
            if (Env.IsDevelopment())
            {
                //この設定がないと、.cshtml変更時にビルドし直さないと修正が反映されない。
                //https://docs.microsoft.com/en-us/aspnet/core/mvc/views/view-compilation?view=aspnetcore-3.1
                builder.AddRazorRuntimeCompilation();
            }
#endif
        }

        public const string ReturnUrlParam = "ReturnUrl";
        public const string LoginPageUrl = "/view/Login/Index";

        /// <summary>
        /// 認証機能を設定する。
        /// </summary>
        /// <param name="services"></param>
        public static void Authentication(this IServiceCollection services, IConfiguration Configuration)
        {
            //コンテキストパスがある場合はCookie名に付与する。(同じブラウザでみた場合に、クッキー値を上書きしあわないように。)
            var contextPathInfo = Configuration.GetContextPathInfo();

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = Startup.AuthScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = new PathString(LoginPageUrl); //ログインページ
                options.LogoutPath = new PathString("/view/Login/Logout"); //ログアウトページ
                options.AccessDeniedPath = new PathString("/view/Login/Deny"); //権限が無いページ表示時の処理

                options.Cookie.Name = $".sample.Auth.{contextPathInfo.NoSlashContextPath}";
                options.SlidingExpiration = true;
                //クッキーの有効期間
                options.ExpireTimeSpan = AuthTimeoutSpan;
                //HttpOnlyなどは、Startup.Configure内で一律で設定している。
                //options.Cookie.HttpOnly = false;
                //options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                //未認証時のリダイレクト処理を上書き
                options.Events.OnRedirectToLogin = (context) =>
                {
                    //Ajaxの場合は401コードを返す(Javascript側でログインページにリダイレクト。)
                    if (context.Request.IsApiOrAjaxRequest())
                    {
                        context.Response.Clear();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                        //ajaxのURLはPOSTなのでそのままリダイレクトすると404になる。
                        //Refererでajaxリクエストの送り元ページのURLを取得する。
                        string returnUrl = context.HttpContext.Request.Headers["Referer"].ToString(); //ログイン後に遷移するURL
                        returnUrl = WebUtil.ToLocalUri(returnUrl); //ドメイン外のサイトに飛ばないよう、ローカルURL(http://ドメイン部分を除いたURL)に変換

                        string redirectUrl = $"{new PathString(LoginPageUrl)}?{ReturnUrlParam}={returnUrl}"; //?をPathStringのコンストラクタに渡すと%3Fに変換されてしまうので、外側で連結すること。
                        context.Response.Headers.Add("redirecturl", redirectUrl); //axiosでヘッダー名を取得すると全て小文字に変換される。ややこしいのでサーバー側も全て小文字にしておく。

                        return Task.FromResult<object>(null);
                    }

                    //string originalUrl = context.Request.FullUrl();
                    //Ajax以外の場合は通常のログイン画面へのリダイレクト処理。
                    //context.Response.Redirect(new PathString($"{LoginPageUrl}?{ReturnUrlParam}={originalUrl}"));
                    context.Response.Redirect(context.RedirectUri);
                    return Task.FromResult<object>(null);
                };

                //権限が無い場合の処理
                options.Events.OnRedirectToAccessDenied = (context) =>
                {
                    //Ajaxの場合は403コードを返す(Javascript側で権限ない旨のメッセージを表示。)
                    if (context.Request.IsApiOrAjaxRequest())
                    {
                        context.Response.Clear();
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.FromResult<object>(null);
                    }

                    //Ajax以外の場合は通常のログイン画面へのリダイレクト処理。
                    context.Response.Redirect(context.RedirectUri);
                    return Task.FromResult<object>(null);
                };


            });

            //偽造防止用設定
            services.AddAntiforgery(opt =>
            {
                opt.Cookie.Name = $".sample.Token.{contextPathInfo.NoSlashContextPath}";
                //HttpOnlyなどは、Startup.Configure内で一律で設定している。
                //opt.Cookie.HttpOnly = false;
                //opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
        }

        /// <summary>
        /// 承認機能を設定する。
        /// </summary>
        /// <param name="services"></param>
        public static void Authorization(this IServiceCollection services)
        {
            //カスタムのPolicyProvider。
            //IAuthorizationPolicyProviderに対するマッピングとして定義すること。
            services.AddScoped<IAuthorizationPolicyProvider, CustomPolicyProvider>();

            //承認用のハンドラー。
            //IAuthorizationHandlerに対するマッピングとして定義すること。
            services.AddScoped<IAuthorizationHandler, AuthRoleHandler>(); //権限
            services.AddScoped<IAuthorizationHandler, AuthDivisionHandler>(); //課

            services.AddAuthorization(o =>
            {
                //ここの処理はカスタムプロバイダー内で実装。

                // o.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
                //o.AddPolicy(AuthRole.Manager.ToString(), policy =>
                //{
                //    //policy.AddAuthenticationSchemes(Startup.AuthScheme);
                //    policy.AddRequirements(new AuthRoleRequirement(AuthRole.Manager));
                //});

                //o.AddPolicy(AuthRole.Assistant.ToString(), policy =>
                //{
                //    //policy.AddAuthenticationSchemes(Startup.AuthScheme);
                //    policy.AddRequirements(new AuthRoleRequirement(AuthRole.Assistant));
                //});
            });
        }


        /// <summary>
        /// レスポンスをgZip圧縮するように設定する。
        /// </summary>
        /// <param name="services"></param>
        public static void Compression(this IServiceCollection services)
        {
            //レスポンスをgZip圧縮するように設定。
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Fastest;
            });
            services.AddResponseCompression();
        }


        /// <summary>
        ///  DI情報を設定する
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Provider"></param>
        /// <param name="HttpContextAccessor"></param>
        /// <param name="Configuration"></param>
        public static void DI(this IServiceCollection services, IConfiguration Configuration)
        {
            //cshtmlをstringに変更するクラスの設定。
            services.AddSingleton<IViewRenderService, ViewRenderService>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //ログインユーザ情報
            services.AddScoped<LoginUserContext>(p =>
            {
                //セッションから値を取り出す。
                //HttpContextAccessorのDIコンテナへの登録はProgram.csで行っている。
                //セッションへの値の登録はLoginControllerで行っている。
                var httpCotnextAccessor = p.GetService<IHttpContextAccessor>();
                var context = httpCotnextAccessor.HttpContext.Session.Get<LoginUserContext>(LoginUserContext.InSessionKey);
                return context ?? new LoginUserContext(null); //nullを返すとTypeFilterAttribute内でエラーになる。
            });

            // システム設定値
            services.AddScoped<ApplicationContext>(p =>
            {
                //セッションから値を取り出す。
                //HttpContextAccessorのDIコンテナへの登録はProgram.csで行っている。
                //セッションへの値の登録はLoginControllerで行っている。
                var httpCotnextAccessor = p.GetService<IHttpContextAccessor>();
                var context = httpCotnextAccessor.HttpContext.Session.Get<ApplicationContext>(ApplicationContext.InSessionKey);
                return context ?? new ApplicationContext(null); //nullを返すとTypeFilterAttribute内でエラーになる。
            }
            );

            //DB用。Connectionは下のメソッドで定義している。
            services.AddScoped<TraceDbProfiler>();

            //コンテキストパス情報
            var contextPathInfo = Configuration.GetContextPathInfo();
            services.AddSingleton(contextPathInfo);

            //SendGrid用の設定
            var sendGridInfo = Configuration.GetSendGridInfo();
            services.AddSingleton(sendGridInfo);
            services.AddSingleton<IMailSendService, SendGridService>();

            //帳票作成用
            //Chromiumとのプロセスは１つでいいはずなので、Singletonにしてみる。問題あればAddScopedに戻す。
            services.AddSingleton<ReportService>();

            //リポジトリ
            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<IUserMainteRepository, UserMainteRepository>();
            services.AddScoped<IBusinessCodeMainteRepository, BusinessCodeMainteRepository>();

        }


        /// <summary>
        /// UnityContainerに直接設定したい場合。
        /// 名前付きの登録など。ConfigureServicesメソッドの後に呼ばれる。
        /// </summary>
        /// <param name="container"></param>
        public static IUnityContainer ConfigureContainer(IUnityContainer container, IConfiguration configuration)
        {
            //UnityContainer container = new UnityContainer();
            //新DBへの接続設定。デフォルト。名前指定しなければこればバインドされる。
            string connectionString = configuration.GetConnectionString("DB");
            container.RegisterFactory<CustomProfiledDbConnection>(scope =>
            {
                var baseConnection = new SqlConnection(connectionString);
                var profiler = scope.Resolve<TraceDbProfiler>();     //発行SQLをログに書き出すため、作成したコネクションをProfiledDbConnectionでラップする。
                var wrapperConnection = new CustomProfiledDbConnection(baseConnection, profiler);

                return wrapperConnection;
            }, new HierarchicalLifetimeManager());

            //既存DBへの接続設定
            string evConnectionString = configuration.GetConnectionString("DB_EV");
            container.RegisterFactory<CustomProfiledDbConnection>(ConstDb.ConnectionStringName_KizonDB, scope =>
            {
                var baseConnection = new SqlConnection(evConnectionString);
                var profiler = scope.Resolve<TraceDbProfiler>();     //発行SQLをログに書き出すため、作成したコネクションをProfiledDbConnectionでラップする。
                var wrapperConnection = new CustomProfiledDbConnection(baseConnection, profiler);

                return wrapperConnection;
            }, new HierarchicalLifetimeManager());

            return container;
        }

    }
}
