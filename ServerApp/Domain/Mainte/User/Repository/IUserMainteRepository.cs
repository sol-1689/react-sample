﻿using Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public interface IUserMainteRepository: ICommonRepository
    {
        Task<(int TotalCount, List<UserSearchResult> List)> GetList(UserSearchParam model);
        Task<int> GetAccountCount(string LoginId, int? UserNo = null);
        Task<int> CreateUserNo();
        Task<int> Insert(UserRegisterModel model);
        Task<int> Update(UserRegisterModel model);
        Task<int> Delete(int UserNo, int DeleteFlag, int? UpdateBy);
        Task<int> DeleteDivision(int value);
        Task<int> InsertDivision(int value, int divisionNo);
    }
}