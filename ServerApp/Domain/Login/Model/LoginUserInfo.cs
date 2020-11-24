using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Model
{
    /// <summary>
    /// ログインユーザー情報
    /// </summary>
    [Serializable]
    public class LoginUserInfo
    {
        public int UserNo { get; set; }
        public string UserName { get; set; }
        public List<int> DivisionNoList { get; set; }
        public string LoginId { get; set; }
        public int Role { get; set; }

    }
}
