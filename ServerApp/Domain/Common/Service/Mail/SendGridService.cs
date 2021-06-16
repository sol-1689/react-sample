using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IO;
using Microsoft.Extensions.Logging;
using Unity;

namespace Domain.Service
{
   
    public class SendGridService: IMailSendService
    {
        SendGridClient client;

        ILogger<SendGridService> logger;

        SendGridInfo info;

        public SendGridService(SendGridInfo info, ILogger<SendGridService> logger)
        {
            client = new SendGridClient(info.ApiKey);
            this.logger = logger;
            //logger.Info($"SendGridApiKey:【{info.ApiKey}】");
            this.info = info;
        }


        async Task<bool> SendMessage(SendGridMessage message)
        {
            logger.LogInformation("メール送信開始。" + message);
            try
            {
                Response response = await client.SendEmailAsync(message);
                if(!response.IsSuccessStatusCode)
                {
                    logger.Error($"メール送信失敗【{response.StatusCode}】");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "メールエラー。");
                return false;
            }
            finally
            {
                logger.LogInformation("メール送信終了。");
            }
        }


        public async Task<bool> SendMail(string title, string text, string from = null, string to = null, string cc = null, string bcc = null, string mimeType = null)
        {
            var message = CreateMessage(title, text, from, CreateAddressStrList(to), CreateAddressStrList(cc), CreateAddressStrList(bcc), mimeType);
            return await SendMessage(message);
        }

        public async Task<bool> SendMails(string title, string text, string from = null, IEnumerable<string> to = null, IEnumerable<string> cc = null, IEnumerable<string> bcc = null, string mimeType = null)
        {
            var message = CreateMessage(title, text, from, to, cc, bcc, mimeType);
            return await SendMessage(message);
        }

        private IEnumerable<string> CreateAddressStrList(string address)
        {
            var result = address == null ? null : new List<string>() { address };
            return result;
        }

        SendGridMessage CreateMessage(string title, string text, string from, IEnumerable<string> to, IEnumerable<string> cc = null, IEnumerable<string> bcc = null, string mimeType = null
            , IEnumerable<Attachment> files = null)
        {
            var message = new SendGridMessage();

            from = from ?? info.DefaultFromAddress;
            message.SetFrom(new EmailAddress(from));

            string[] toList = to?.Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();

            if (toList != null && toList.Any())
                message.AddTos(toList.Select(m => new EmailAddress(m)).ToList());
            else
                message.AddTos(new List<EmailAddress>(){ new EmailAddress(info.DefaultToAddress) });

            if (cc != null && cc.Any())
                message.AddTos(cc.Select(m => new EmailAddress(m)).ToList());

            if (bcc != null && bcc.Any())
                message.AddTos(bcc.Select(m => new EmailAddress(m)).ToList());

            if (files != null && files.Any())
            {
                message.Attachments = files.ToList();
                //var attachList = files.Select(m => new Attachment()
                //{
                //    Content = Convert.ToBase64String(m.ReadAllBytes())
                //    //Type = "image/png",
                //    //Filename = "banner2.png",
                //    //Disposition = "inline",
                //    //ContentId = "Banner 2"
                //}).ToList();
            }

            message.SetSubject(title);

            if (string.IsNullOrEmpty(mimeType))
                mimeType = MimeType.Text;

            message.AddContent(MimeType.Text, text);
            return message;
        }

        //SendGridMessage CreateMessage(string title, string text, string from, List<EmailAddress> to, List<EmailAddress> cc = null, List<EmailAddress> bcc = null, string mimeType = null
        //    , IEnumerable<Stream> files = null)
        //{
        //    var message = new SendGridMessage();
        //    message.SetFrom(new EmailAddress(from));

        //    foreach (var each in to)
        //        message.AddTo(each);

        //    foreach (var each in cc)
        //        message.AddCc(each);

        //    foreach (var each in bcc)
        //        message.AddBcc(each);

        //    message.SetSubject(title);

        //    if (string.IsNullOrEmpty(mimeType))
        //        mimeType = MimeType.Text;

        //    message.AddContent(MimeType.Text, text);

        //}

        //List<EmailAddress> CreateAddressList(params string[] adsress)
        //{
        //    if (adsress != null && adsress.Any())
        //        return adsress.Select(m => new EmailAddress(m)).ToList();
        //    else
        //        return new List<EmailAddress>();
        //}

    }


    public class SendGridInfo
    {
        public string ApiKey { get; }
        public string DefaultFromAddress { get; }
        public string DefaultToAddress { get; }

        public bool IsValid { get { return !string.IsNullOrEmpty(ApiKey); } }

        public SendGridInfo(string apiKey, string defaultFromAddress, string defaultToAddress)
        {
            ApiKey = apiKey;
            DefaultFromAddress = defaultFromAddress;
            DefaultToAddress = defaultToAddress;
        }

    }


}
