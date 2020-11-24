using Domain.DB.Model;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Domain.Util;
using System.Linq;
using System.Data;

namespace Domain.Repository
{
    public class KubunRepository : CommonRepository
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="connection">IDbConnectionを外部から渡したい場合に使用。</param>
        public KubunRepository(CustomProfiledDbConnection connection)
            : base(connection)
        {
        }

        /// <summary>
        /// 区分一覧を取得する。
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public async Task<List<KubunSearchResult>> GetKubunList(string CategoryId)
        {
            //SQL内にコメント書くとDockerではSQLが変になる(多分改行のせい)
            string sql =
$@"SELECT 
     GeneralNo AS Value
    ,GeneralName AS Text
    ,Remarks
  FROM M_General base
  WHERE CategoryId = @CategoryId
    AND DeleteFlag = {ConstFlag.FalseValue}
  ORDER BY OrderNo
";
            var list = (await Connection.QueryAsync<KubunSearchResult>(sql, new{CategoryId}))
                .ToList();

            return list;
        }


        /// <summary>
        /// 課の一覧を取得する。
        /// </summary>
        /// <returns></returns>
        public async Task<List<KubunSearchResult>> GetDivisionList()
        {
            //SQL内にコメント書くとDockerではSQLが変になる(多分改行のせい)
            string sql =
$@"SELECT 
     DivisionNo AS Value
    ,DivisionName AS Text
    ,Remarks
  FROM M_Division base
  WHERE DeleteFlag = {ConstFlag.FalseValue}
  ORDER BY OrderNo
";
            var list = (await Connection.QueryAsync<KubunSearchResult>(sql))
                .ToList();

            return list;
        }


        /// <summary>
        /// 事業コードの一覧を取得する。
        /// </summary>
        /// <returns></returns>
        public async Task<List<KubunSearchResult>> GetBusinessCodeList()
        {
            string sql =
$@"SELECT 
     BusinessCodeNo AS Value
    ,DebitBusinessName AS Text
  FROM M_BusinessCode base
  WHERE DeleteFlag = {ConstFlag.FalseValue}
  ORDER BY DebitBusinessCode
";
            var list = (await Connection.QueryAsync<KubunSearchResult>(sql))
                .ToList();

            return list;
        }



        /// <summary>
        /// 直近の理事会Noを取得する。
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public async Task<int?> GetCurrentMeetingNo(DateTime? today)
        {
            // 理事会日期間の最終日（次回理事会日期間の開始日）は、次回理事会日ではなく今の理事会を初期選択させる。
            string sql =
$@"SELECT 
     TOP (1) OfficerMeetingNo
  FROM M_OfficerMeeting
  WHERE OfficerMeetingDate >= @today
  ORDER BY OfficerMeetingDate
";
            
            var result = (await Connection.QueryAsync<int?>(sql, new { today }))
                .FirstOrDefault();

            return result;
        }

    }
}
