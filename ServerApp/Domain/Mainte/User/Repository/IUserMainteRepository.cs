using Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public interface IUserMainteRepository: ICommonRepository
    {
        Task<(int TotalCount, List<UserSearchResult> List)> GetList(UserSearchParam model);
        Task<int> GetAccountCount(string LoginId, int? UserNo = null);
        Task<int> CreateUserNo();
        Task<int> InsertM_User(UserRegisterModel model);
        Task<int> UpdateM_User(UserRegisterModel model);
        Task<int> DeleteM_User(int UserNo, int DeleteFlag, int? UpdateBy);
        Task<int> DeleteM_UserDivision(int value);
        Task<int> InsertM_UserDivision(int value, int divisionNo);
    }
}