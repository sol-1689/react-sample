using Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public interface IBusinessCodeMainteRepository
    {
        Task<(int TotalCount, List<BusinessCodeSearchResult> List)> GetList(int? SelectedAccountingCode, string SearchDebitBusinessCode, string SearchDebitBusinessName, bool showDeleted, IEnumerable<SortParam> sortParams, IPagingParam pagingParam);
        Task<int> GetCodeCount(string debitBusinessCode, int? businessCodeNo);
        Task<int> CreateBusinessCodeNo();
        Task<int> InsertM_BusinessCode(BusinessCodeRegisterModel model);
        Task<int> UpdateM_BusinessCode(BusinessCodeRegisterModel model);
    }
}