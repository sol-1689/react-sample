using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Domain.Util
{
    public class SendGridHandler
    {
        SendGridClient client;
        ILogger<SendGridHandler> logger;

        public SendGridHandler(string apiKey, ILogger<SendGridHandler> logger)
        {
            client = new SendGridClient(apiKey);
            this.logger = logger;
        }


        async Task SendMessage(SendGridMessage message)
        {
            logger.LogInformation("メール送信開始。" + message);
            try
            {
                Response response = await client.SendEmailAsync(message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "メールエラー。");
            }
            finally
            {
                logger.LogInformation("メール送信終了。");
            }
        }


        public async Task SendMail(string title, string text, string from, string to, string cc = null, string bcc = null, string mimeType = null)
        {
            var message = CreateMessage(title, text, from, CreateAddressStrList(to), CreateAddressStrList(cc), CreateAddressStrList(bcc), mimeType);
            await SendMessage(message);
        }

        public async Task SendMail(string title, string text, string from, IEnumerable<string> to, IEnumerable<string> cc = null, IEnumerable<string> bcc = null, string mimeType = null)
        {
            var message = CreateMessage(title, text, from, to, cc, bcc, mimeType);
            await SendMessage(message);
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
            message.SetFrom(new EmailAddress(from));

            if (to != null && to.Any())
                message.AddTos(to.Select(m => new EmailAddress(m)).ToList());

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

   
}
