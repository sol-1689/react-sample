using Domain.Model;
using Domain.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using Domain.DB.Model;

namespace Domain.Repository
{
    public class ApplicationRepository : CommonRepository, IApplicationRepository
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="connection">IDbConnectionを外部から渡したい場合に使用。</param>
        public ApplicationRepository(CustomProfiledDbConnection connection)
            : base(connection)
        {
        }


        /// <summary>
        /// アプリケーション情報を取得する。
        /// </summary>
        /// <returns></returns>
        public async Task<List<M_General>> GetApplicationInfo()
        {
            string sql =
$@"
SELECT
  *
FROM
  M_General
";

            var result = (await Connection.QueryAsync<M_General>(sql)).ToList();
            return result;
        }
    }
}
