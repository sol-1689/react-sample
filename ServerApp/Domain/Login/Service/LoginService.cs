using Domain.Model;
using Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Domain.Service
{
    public class LoginService
    {
         [Dependency]
        public ILoginRepository repository { get; set; }

        public async Task<LoginUserInfo> GetUserByLoginInfo(string loginId, string passwordStr)
        {
            LoginUserInfo user = await repository.GetUserByLoginInfo(loginId, passwordStr);
            return user;
        }
    }

}
