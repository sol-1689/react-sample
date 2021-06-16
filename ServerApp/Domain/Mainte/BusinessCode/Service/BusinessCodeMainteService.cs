using Domain.DB.Model;
using Domain.Model;
using Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Domain.Service
{
    /// <summary>
    /// 事業コードメンテ用のサービス。
    /// </summary>
    public class BusinessCodeMainteService : CommonService
    {
         [Dependency]
        public IBusinessCodeMainteRepository repository { get; set; }


        /// <summary>
        /// 検索処理を行う。
        /// </summary>
        /// <param name="TotalCount"></param>
        /// <param name="List"></param>
        /// <returns></returns>
        public async Task<(int TotalCount, List<BusinessCodeSearchResult> List)> GetList(int? SelectedAccountingCode, string SearchDebitBusinessCode, string SearchDebitBusinessName, bool showDeleted, 
            IEnumerable<SortParam> sortParams, IPagingParam pagingParam)
        {
            pagingParam.SetDefaultPagingParamIfNull();
            var result = await repository.GetList(SelectedAccountingCode, SearchDebitBusinessCode, SearchDebitBusinessName,showDeleted, sortParams, pagingParam);
            return result;
        }

        /// <summary>
        /// 事業コード番号を作成する。
        /// </summary>
        /// <returns></returns>
        public async Task<int> CreateBusinessCodeNo()
        {
            var result = await repository.CreateBusinessCodeNo();
            return result;
        }

        /// <summary>
        /// 新規登録処理を行う。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<(bool Result, string Message)> Create(BusinessCodeRegisterModel model)
        {
            int dupliCount = await repository.GetCodeCount(model.DebitBusinessCode, null);
            if(dupliCount > 0)
                return (Result: false, Message: "既に登録されている借方事業コードです。");

            model.BusinessCodeNo = await CreateBusinessCodeNo();
            model.PaymentFlag = model.IsPaid.AsFlag();
            model.DeleteFlag = model.IsDeleted.AsFlag();
            model.CreateBy = model.UpdateBy = loginUserContext.UserNo;
            
            var result = await repository.Insert(model);
            if(result == 1)
                return (Result: true, Message: "");
            else
                return (Result: false, Message: "登録に失敗しました。");
        }


        /// <summary>
        /// 更新処理を行う。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<(bool Result, string Message)> Update(BusinessCodeRegisterModel model)
        {
            int dupliCount = await repository.GetCodeCount(model.DebitBusinessCode, model.BusinessCodeNo);
            if(dupliCount > 0)
                return (Result: false, Message: "既に登録されている借方事業コードです。");

            model.PaymentFlag = model.IsPaid.AsFlag();
            model.DeleteFlag = model.IsDeleted.AsFlag();
            model.UpdateBy = loginUserContext.UserNo;
            
            var result = await repository.Update(model);
            if(result == 1)
                return (Result: true, Message: "");
            else
                return (Result: false, Message: "登録に失敗しました。");
        }

    }
}
