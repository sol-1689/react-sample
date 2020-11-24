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
    public class BusinessCodeMainteRepository : CommonRepository, IBusinessCodeMainteRepository
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="connection">IDbConnectionを外部から渡したい場合に使用。</param>
        public BusinessCodeMainteRepository(CustomProfiledDbConnection connection)
            : base(connection)
        {
        }

        /// <summary>
        /// ソート対象の列名。
        /// </summary>
        static readonly IReadOnlyCollection<string> SortColumns = new string[] { 
            nameof(BusinessCodeSearchResult.AccountingCodeName), nameof(BusinessCodeSearchResult.DebitBusinessCode), 
            nameof(BusinessCodeSearchResult.DebitBusinessName) ,nameof(BusinessCodeSearchResult.DebitAccountingItemCode) ,
            nameof(BusinessCodeSearchResult.DebitAccountingAssistItemCode) ,nameof(BusinessCodeSearchResult.DebitTaxCode) ,
            nameof(BusinessCodeSearchResult.CreditBusinessCode) ,nameof(BusinessCodeSearchResult.CreditAccountingItemCode) ,
            nameof(BusinessCodeSearchResult.CreditAccountingAssistItemCode) ,nameof(BusinessCodeSearchResult.CreditTaxCode) 
            
            };

        /// <summary>
        /// 事業コード一覧を取得する。
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns>合計件数と一覧の名前付きタプル型を返す。</returns>
        public async Task<(int TotalCount, List<BusinessCodeSearchResult> List)> GetList(int? selectedAccountingCode, string searchDebitBusinessCode, string searchDebitBusinessName,  bool showDeleted,
            IEnumerable<SortParam> sortParams, IPagingParam pagingParam)
        {
            DynamicParameters parameter = new DynamicParameters(pagingParam);
            string whereSql1 = CreateSearchTextForBusinessCode(searchDebitBusinessCode, searchDebitBusinessName, 
            selectedAccountingCode, showDeleted, parameter);
            //string whereSql2 = CreateSearchTextForBusinessCode(selectedAccountingCode);
            sortParams = ISortParamsExtensions.CheckSortParams(sortParams, SortColumns); //SQLインジェクションを防ぐ為、画面から送られてきたソート用パラメータが想定した文字列かチェックする。
            
            string dispOrder = CreateSortOrderStr(sortParams);
            string orderText = string.IsNullOrEmpty(dispOrder) ? " ORDER BY AccountingCodeNo" : dispOrder;

            string sql =
$@"SELECT * FROM
(
     SELECT 
        ({CreateGeneralNameSubQuery(ConstKubun.AccountingCode, "main.AccountingCodeNo")}) AS AccountingCode
       ,({CreateRemarksSubQuery(ConstKubun.AccountingCode, "main.AccountingCodeNo")}) AS AccountingCodeName
       ,main.BusinessCodeNo
       ,main.AccountingCodeNo
       ,main.DebitBusinessCode
	   ,main.DebitBusinessName
	   ,main.DebitAccountingItemCode
	   ,main.DebitAccountingAssistItemCode
	   ,main.DebitTaxCode
	   ,main.CreditBusinessCode
       ,main.CreditAccountingItemCode
       ,main.CreditAccountingAssistItemCode
       ,main.CreditTaxCode
       ,main.PaymentFlag
	   ,main.DeleteFlag
       ,COUNT(*) OVER() AS TotalCount
  FROM M_BusinessCode main
  WHERE 
  {whereSql1}
)as base
{orderText}
OFFSET @StartRow ROWS FETCH NEXT @RowCount ROWS ONLY
";
            int totalCount = 0;
            //SELECT結果とTotalCountを取得。
            var list = (await Connection.QueryAsync<BusinessCodeSearchResult, int, BusinessCodeSearchResult>(sql,
                (r, _totalCount) =>
                {
                    totalCount = _totalCount;
                    return r;
                }, parameter, splitOn: "TotalCount"))
                .ToList();

            var result = (TotalCount: totalCount, List: list);
            return result;
        }

        /// <summary>
        /// 事業コード検索のWhere句を作成する。
        /// </summary>
        private string CreateSearchTextForBusinessCode(string searchDebitBusinessCode, string searchDebitBusinessName, int? selectedAccountingCode,
         bool showDeleted, DynamicParameters paramater)
        {
            List<string> whereSql = new List<string>();

            //借方事業コード
            if (!String.IsNullOrEmpty(searchDebitBusinessCode))
            {
                whereSql.Add(base.CreateSearchText(CommonUtil.SplitSearchText(searchDebitBusinessCode), new String[]{"DebitBusinessCode"},  paramater, "main"));
            }

            //借方事業名
            if (!String.IsNullOrEmpty(searchDebitBusinessName))
            {
                whereSql.Add(base.CreateSearchText(CommonUtil.SplitSearchText(searchDebitBusinessName), new String[]{"DebitBusinessName"},  paramater, "main"));
            }

            //
            if (selectedAccountingCode != null)
            {
                paramater.Add("@AccountingCodeNo", selectedAccountingCode, dbType: DbType.Int32);
                whereSql.Add(@" main.AccountingCodeNo = @AccountingCodeNo ");
            }

            paramater.Add("@DeleteFlag", showDeleted.AsFlag(), dbType: DbType.Int32);
            whereSql.Add(@" main.DeleteFlag = @DeleteFlag ");

            return whereSql.IsNullOrEmpty() ? "" : string.Join(" AND ", whereSql);

        }

        /// <summary>
        /// 事業コード検索のWhere句を作成する。
        /// </summary>
        private string CreateSearchTextForBusinessCode(string selectedAccountingCode)
        {
            if (string.IsNullOrWhiteSpace(selectedAccountingCode)){
                return "";
            }
            return "WHERE AccountingCode = @selectedAccountingCode ";
           
        }

        /// <summary>
        /// 同一借方事業コードの件数を取得する。(重複チェック)
        /// </summary>
        /// <param name="debitBusinessCode"></param>
        /// <returns></returns>
        public async Task<int> GetCodeCount(string debitBusinessCode, int? businessCodeNo = null)
        {
            string sql =
@"SELECT COUNT(*)
  FROM M_BusinessCode
  WHERE DebitBusinessCode = @debitBusinessCode
";
            if(businessCodeNo != null)
                sql += " AND BusinessCodeNo != @businessCodeNo ";
                
            int result = (await Connection.QueryAsync<int?>(sql, new { debitBusinessCode, businessCodeNo })).FirstOrDefault() ?? 0;
            return result;
        }

        /// <summary>
        /// BusinessCodeNoの作成。
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<int> CreateBusinessCodeNo()
        {
            string sql =
@"SELECT
 NEXT VALUE FOR Seq_M_BusinessCode
";
            int result = (await Connection.QueryAsync<int>(sql)).FirstOrDefault();
            return result;
        }


        /// <summary>
        /// 変更情報を更新する。
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<int> UpdateM_BusinessCode(BusinessCodeRegisterModel model)
        {
            string sql =
$@"UPDATE M_BusinessCode
 SET
  AccountingCodeNo = @AccountingCodeNo
 ,DebitBusinessCode = @DebitBusinessCode
 ,DebitBusinessName = @DebitBusinessName
 ,DebitAccountingItemCode = @DebitAccountingItemCode
 ,DebitAccountingAssistItemCode = @DebitAccountingAssistItemCode
 ,DebitTaxCode = @DebitTaxCode
 ,CreditBusinessCode = @CreditBusinessCode
 ,CreditAccountingItemCode = @CreditAccountingItemCode
 ,CreditAccountingAssistItemCode = @CreditAccountingAssistItemCode
 ,CreditTaxCode = @CreditTaxCode
 ,PaymentFlag = @PaymentFlag
 ,UpdateBy = @UpdateBy
 ,UpdateDateTime = GETDATE()
 ,DeleteFlag = @DeleteFlag
WHERE BusinessCodeNo = @BusinessCodeNo
";

            DynamicParameters parameter = new DynamicParameters(model);
            int result = await Connection.ExecuteAsync(sql, parameter);
            return result;
        }

        /// <summary>
        /// データ情報を登録する。
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<int> InsertM_BusinessCode(BusinessCodeRegisterModel model)
        {
            string sql =
@"INSERT INTO M_BusinessCode(
   BusinessCodeNo
  ,AccountingCodeNo
  ,DebitBusinessCode
  ,DebitBusinessName
  ,DebitAccountingItemCode
  ,DebitAccountingAssistItemCode
  ,DebitTaxCode
  ,CreditBusinessCode
  ,CreditAccountingItemCode
  ,CreditAccountingAssistItemCode
  ,CreditTaxCode
  ,PaymentFlag
  ,CreateBy
  ,CreateDateTime
  ,UpdateBy
  ,UpdateDateTime
  ,DeleteFlag
)VALUES (
   @BusinessCodeNo
  ,@AccountingCodeNo
  ,@DebitBusinessCode
  ,@DebitBusinessName
  ,@DebitAccountingItemCode
  ,@DebitAccountingAssistItemCode
  ,@DebitTaxCode
  ,@CreditBusinessCode
  ,@CreditAccountingItemCode
  ,@CreditAccountingAssistItemCode
  ,@CreditTaxCode
  ,@PaymentFlag
  ,@CreateBy
  ,GETDATE()
  ,@UpdateBy
  ,GETDATE()
  ,@DeleteFlag
)";
            DynamicParameters parameter = new DynamicParameters(model);
            int result = await Connection.ExecuteAsync(sql, parameter);
            return result;
        }

    }
}
