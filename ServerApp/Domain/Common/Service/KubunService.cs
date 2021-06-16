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
    /// 区分値取得用のサービス。
    /// </summary>
    public class KubunService
    {
         [Dependency]
        public KubunRepository repository { get; set; }

        /// <summary>
        /// 区分値取得処理を行う。
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<List<KubunSearchResult>> GetKubunList(string categoryId)
        {
            var result = await repository.GetKubunList(categoryId);
            return result;
        }

        /// <summary>
        /// 課の一覧取得処理を行う。
        /// </summary>
        /// <returns></returns>
        public async Task<List<KubunSearchResult>> GetDivisionList()
        {
            var result = await repository.GetDivisionList();
            return result;
        }


        /// <summary>
        /// 事業コードの一覧取得処理を行う。
        /// </summary>
        /// <returns></returns>
        public async Task<List<KubunSearchResult>> GetBusinessCodeList()
        {
            var result = await repository.GetBusinessCodeList();
            return result;
        }



    }

}
