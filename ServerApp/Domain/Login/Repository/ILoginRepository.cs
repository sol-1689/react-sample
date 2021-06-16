using Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public interface ILoginRepository: ICommonRepository
    {
        Task<LoginUserInfo> GetUserByLoginInfo(string LoginId, string password);
    }
}
