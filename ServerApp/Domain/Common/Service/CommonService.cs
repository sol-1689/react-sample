using Domain.DB.Model;
using Domain.Model;
using Domain.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Domain.Service
{
    /// <summary>
    /// 共通のメンバーを保持したサービス。
    /// </summary>
    public class CommonService
    {
        /// <summary>ロガー。 </summary>
        [Dependency]
        public ILogger<CommonService> _logger { get; set; }

        [Dependency]
        public LoginUserContext loginUserContext { get; set; }

        [Dependency]
        public ApplicationContext applicationContext { get; set; }


    }

}
