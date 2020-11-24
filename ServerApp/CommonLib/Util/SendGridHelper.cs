using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Util
{
    public class SendGridHelper
    {
        SendGridInfo info;
        ILoggerFactory loggerFactory;

        public SendGridHelper(SendGridInfo info, ILoggerFactory loggerFactory)
        {
            this.info = info;
            this.loggerFactory = loggerFactory;
        }

        public SendGridHandler CreateMailHandler()
        {
            return new SendGridHandler(info.ApiKey, loggerFactory.CreateLogger<SendGridHandler>());
        }


        public async Task SendMessage(string title, string text, string from = null, string to = null)
        {
            if(info == null || !info.IsValid)
                return;

            var handler = CreateMailHandler();

            from = from ?? info.DefaultFromAddress;
            to = to ?? info.DefaultToAddress;

            if(from == null || to == null)
                return;

            var toList = to.Split(',');
            await handler.SendMail(title, text, from, toList);
        }
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
