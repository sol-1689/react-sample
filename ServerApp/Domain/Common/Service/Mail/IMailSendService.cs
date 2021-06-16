using Domain.DB.Model;
using Domain.Model;
using Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Domain.Service
{
    /// <summary>
    /// メール送信用のサービス
    /// </summary>
     public interface IMailSendService
    {
        Task<bool> SendMail(string title, string text, string from = null, string to = null, string cc = null, string bcc = null, string mimeType = null);
        Task<bool> SendMails(string title, string text, string from = null, IEnumerable<string> to = null, IEnumerable<string> cc = null, IEnumerable<string> bcc = null, string mimeType = null);
    }

}
