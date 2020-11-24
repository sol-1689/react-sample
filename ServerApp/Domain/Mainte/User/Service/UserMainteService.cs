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
    /// ユーザーメンテ用のサービス。
    /// </summary>
    public class UserMainteService
    {
         [Dependency]
        public IUserMainteRepository repository { get; set; }


        /// <summary>
        /// 検索処理を行う。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<(int TotalCount, List<UserSearchResult> List)> GetList(UserSearchParam model)
        {
            model.PagingParam.SetDefaultPagingParamIfNull();
            var result = await repository.GetList(model);
            return result;
        }

        /// <summary>
        /// 詳細情報の取得処理を行う。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<UserSearchResult> GetDetail(UserSearchParam model)
        {
            model.PagingParam = new PagingParam();
            model.PagingParam.SetDefaultPagingParamIfNull();

            var result = (await repository.GetList(model)).List.FirstOrDefault();
            return result;
        }


        /// <summary>
        /// ユーザーNoを作成する。
        /// </summary>
        /// <returns></returns>
        public async Task<int> CreateUserNo()
        {
            var result = await repository.CreateUserNo();
            return result;
        }

        /// <summary>
        /// アカウントの重複数を取得する。
        /// </summary>
        /// <param name="LoginId"></param>
        /// <returns></returns>
        public async Task<int> GetAccountCount(string LoginId)
        {
            var result = await repository.GetAccountCount(LoginId);
            return result;
        }
        
        /// <summary>
        /// 新規登録処理を行う。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<(bool Result, string Message)> Create(UserRegisterModel model)
        {
            int dupliCount = await repository.GetAccountCount(model.LoginId);
            if(dupliCount > 0)
                return (Result: false, Message: "既に登録されているログインIDです。");

            using (var transaction = repository.AsyncTransactionScope())
            {

                model.UserNo = await CreateUserNo();
                model.DeleteFlag = model.IsDeleted.AsFlag();

                var result = await repository.InsertM_User(model);
                if (result != 1)
                    return (Result: false, Message: "登録に失敗しました。");

                _ = await repository.DeleteM_UserDivision(model.UserNo.Value);

                foreach (var divisionNo in model.DivisionNoList)
                {
                    int divCount = await repository.InsertM_UserDivision(model.UserNo.Value, divisionNo);
                    if (divCount != 1)
                        return (Result: false, Message: "登録に失敗しました。");
                }

                transaction.Complete();
                return (Result: true, Message: "");
            }
        }


        /// <summary>
        /// 更新処理を行う。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<(bool Result, string Message)> Update(UserRegisterModel model)
        {
            int dupliCount = await repository.GetAccountCount(model.LoginId, model.UserNo);
            if (dupliCount > 0)
                return (Result: false, Message: "既に登録されているログインIDです。");

            using (var transaction = repository.AsyncTransactionScope())
            {
                model.DeleteFlag = model.IsDeleted.AsFlag();
                var result = await repository.UpdateM_User(model);
                if (result != 1)
                    return (Result: false, Message: "登録に失敗しました。");

                _ = await repository.DeleteM_UserDivision(model.UserNo.Value);

                foreach(var divisionNo in model.DivisionNoList)
                {
                    int divCount = await repository.InsertM_UserDivision(model.UserNo.Value, divisionNo);
                    if (divCount != 1)
                        return (Result: false, Message: "登録に失敗しました。");
                }

                transaction.Complete();
                return (Result: true, Message: "");
            }
        }

        /// <summary>
        /// 削除処理を行う。
        /// </summary>
        /// <param name="UserNo"></param>
        /// <param name="DeleteFlag"></param>
        /// <param name="UpdateBy"></param>
        /// <returns></returns>
        public async Task<bool> Delete(int UserNo, int DeleteFlag, int? UpdateBy)
        {
            var result = await repository.DeleteM_User(UserNo, DeleteFlag, UpdateBy);
            return result == 1;
        }
    }

}
