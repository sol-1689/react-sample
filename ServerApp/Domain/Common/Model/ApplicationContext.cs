using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;
using Domain.Util;

namespace Domain.Model
{
    /// <summary>
    /// ログインユーザに関する情報を保持する。
    /// </summary>
    [Serializable]
    public class ApplicationContext
    {
        public const string InSessionKey = "APPLICATION_INFO_IN_SESSION";

        public ApplicationContext(ApplicationInfo applicationInfo)
        {
            this.applicationInfo = applicationInfo;
        }

        [JsonIgnore]
        public ApplicationInfo applicationInfo { get; set; }



    }

}