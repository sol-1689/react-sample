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
        Task<int> Insert(BusinessCodeRegisterModel model);
        Task<int> Update(BusinessCodeRegisterModel model);
    }
}