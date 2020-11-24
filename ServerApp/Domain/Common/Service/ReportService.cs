using PuppeteerSharp;
using PuppeteerSharp.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Domain.Service
{
    /// <summary>
    /// 帳票作成用のサービス。
    /// </summary>
    public class ReportService : IDisposable
    {
        Browser browser;

        async Task<Browser> GetBrowser()
        {
            if (browser == null)
                browser = await Puppeteer.LaunchAsync(defaultLaunchOption).ConfigureAwait(false);

            return browser;
        }

        readonly LaunchOptions defaultLaunchOption = new LaunchOptions
        {
            Headless = true,
            IgnoreHTTPSErrors = true, //オレオレ証明書や、localhost宛てでドメイン違う際の証明書エラーを無視。
            //コンテナで実行する場合はこの指定が必要。
            Args = new[]
            {
                "--no-sandbox"
            }
        };


        /// <summary>
        /// Html文字列からPdfを作成する。
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public async Task<Stream> CreatePdfStreamAsync(string htmlOrUrl, bool needCommonHeaderFooter = false, 
            string headerTime = null, string headerFooterPattern = null, bool fromUrl = true)
        {
            var defaultOption = CreateDefaultOption(needCommonHeaderFooter, headerTime, headerFooterPattern);
            return await CreatePdfStreamAsync(htmlOrUrl, defaultOption, fromUrl);
        }

        /// <summary>
        /// Html文字列からPdfを作成する。
        /// </summary>
        /// <param name="html"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public async Task<Stream> CreatePdfStreamAsync(string htmlOrUrl, PdfOptions option, bool fromUrl = true)
        {
            var launchOption = new LaunchOptions
            {
                Headless = true
            };

            var browser = await GetBrowser();

            using (var page = await browser.NewPageAsync())
            {
                //await page.GoToAsync("http://localhost:59375/");
                //await page.GoToAsync("https://github.com/kblok/puppeteer-sharp");
                //await page.SetContentAsync("<html><head></head><body>test</body></html>");
                //await page.SetContentAsync(html);

                htmlOrUrl = fromUrl ? htmlOrUrl : "data:text/html," + htmlOrUrl;

                await page.GoToAsync(htmlOrUrl, new NavigationOptions()
                {
                    WaitUntil = new WaitUntilNavigation[] { WaitUntilNavigation.Load, WaitUntilNavigation.Networkidle0 },
                    Timeout = 600000 // Timeoutを10分（初期値は30秒）
                });

                var stream = await page.PdfStreamAsync(option);
                return stream;
            }
        }

        /// <summary>
        /// デフォルトのPdfオプションを作成する。
        /// </summary>
        /// <returns></returns>
        public PdfOptions CreateDefaultOption(bool needCommonHeaderFooter = false, string headerTime = null, string headerFooterPattern = null)
        {
            // ヘッダーにデフォルトの時間をセット
            headerTime = headerTime ?? DateTime.Now.ToDefaultYYYYMMDDHHMMSS();
            var defaultOption = new PdfOptions()
            {
                Format = PaperFormat.A4,
                Landscape = false,
                DisplayHeaderFooter = true,
                //class指定でのstyleが効かない。
                HeaderTemplate = needCommonHeaderFooter ? $@"<div class='print_header_container' style=""font-family: 'ＭＳ Ｐ明朝', 'HGP行書体';font-size:10px;text-align:right;width:100%;border:0;""><span style='vertical-align:top;margin-right:10px;' class=''>{headerTime}</span><span style='vertical-align:top;margin-right:33px;' class=''>&nbsp;</span></div>" : $@"<div class='' style='width:100%;border:0;'></div>",
                FooterTemplate = needCommonHeaderFooter ? $@"<div class='print_footer_container' style='font-size:10px;text-align:center;width:100%;border:0;'><span style='vertical-align:bottom;' class='pageNumber'></span>/<span style='vertical-align:bottom;' class='totalPages'></span></div>" : $@"<div class='' style='width:100%;border:0;'></div>",
                MarginOptions = needCommonHeaderFooter ? new MarginOptions() { Top = "50px", Bottom = "50px", Left = "50px", Right = "50px" } : new MarginOptions() { Top = "50px", Bottom = "30px", Left = "30px", Right = "30px" }  //mmは効かない。
            };

            return defaultOption;
        }

        public void Dispose()
        {
            if (browser != null)
            {
                browser.Dispose();
                browser = null;
            }
        }
    }
}
