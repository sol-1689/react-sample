using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Dapper;
using System.Data;
using System.Text;
using System.Configuration;
using System.Data.Common;
using System.Collections.Specialized;
using StackExchange.Profiling.Data;
using System.Threading.Tasks;
using System.Transactions;
using Unity;
using Microsoft.Extensions.Logging;
using Domain.Util;
using Domain.DB;

namespace Domain.Repository
{
    /// <summary>
    /// 共通リポジトリ。
    /// </summary>
    public class CommonRepository : ICommonRepository
    {
        #region 定数

        public const string COMMA = ",";

        #endregion

        #region コネクション作成部分
        //Dapperメモ 
        //SQL文の変数にオブジェクトの変数をマップする際、SQLServerの場合はSQL文内に @変数名　と記述する。
        //(ちなみにOracleの場合は:変数名 とする。)

        //Oracleの場合、SQL文の末尾に;(セミコロン)を付けると構文エラーになるので注意。

        //※SQL文の書き方について
        //.NETのstringを定義する際、文字列の前に@をつけると、改行やスペースもそのまま文字とみなされる（verbatim文字列）
        //SQLを真ん中の方に寄せて定義すると余計なスペースがSQL文に含まれてしまうので、少し見にくいがなるべく左に寄せて定義するようにする。

        //public const int DefaultCommandTimeOut = 300;

        /// <summary>コマンドタイムアウト値。 </summary>
        public int? CommandTimeout
        {
            get { return _connection?.CommandTimeout; }
            set
            {
                if (_connection != null)
                    _connection.CommandTimeout = value;
            }
        }


        CustomProfiledDbConnection _connection;
        /// <summary>
        /// DB接続用のIDbConnectionを取得・設定する。
        /// </summary>
        public CustomProfiledDbConnection Connection
        {
            get
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                return _connection;
            }
        }


        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="connection">IDbConnectionを外部から渡したい場合に使用。</param>
        public CommonRepository(CustomProfiledDbConnection connection)
        {
            _connection = connection;
        }


        /// <summary>ロガー。 </summary>
        [Dependency]
        public ILogger<CommonRepository> logger { get; set; }


        #endregion

        #region 共通処理

        /// <summary> </summary>
        public static class AndOrTerm
        {
            public const string AND = " AND ";
            public const string OR = " OR ";
        }

        /// <summary>
        /// スペースで複数の検索条件が区切られた文字列からWhere句のSQLを作成する。
        /// </summary>
        /// <param name="SearchParams"></param>
        /// <param name="parameters"></param>
        /// <param name="prefix"></param>
        /// <param name="appendWhereFunc"></param>
        /// <param name="needBeforeAndWord"></param>
        /// <returns></returns>
        protected static string CreateMultiWordWhereSql(string[] SearchParams, DynamicParameters parameters,
            Func<DynamicParameters, int, string> appendWhereFunc, bool needBeforeAndWord = true, string AndOr = AndOrTerm.OR)
        {
            StringBuilder sb = new StringBuilder();

            bool isFirst = true;

            //検索キーワードはスペースで区切って複数指定できる仕様となっている。
            //検索キーワードの数だけAND句を追加していく。
            for (int index = 0; index < SearchParams.Length; index++)
            {
                if (string.IsNullOrWhiteSpace(SearchParams[index]))
                    continue;

                if (!isFirst)
                    sb.Append(AndOr);
                else
                    isFirst = false;

                string eachSql = appendWhereFunc(parameters, index);
                sb.Append(eachSql);
            }

            if (sb.Length > 0)
            {
                if(needBeforeAndWord)
                    sb.Insert(0, " AND ( ").Append(" ) ");
                else
                    sb.Insert(0, " ( ").Append(" ) ");
            }

            return sb.ToString();
        }


        /// <summary>
        /// Where句のSQLを作成する。(複数文字列で複数列に対してLIKE検索をする。)
        /// </summary>
        /// <param name="SearchParams">検索文字列。画面からの入力文字列をスペースで区切ったものを想定。</param>
        /// <param name="targetColumns">検索対象カラム名</param>
        /// <param name="param">パラメータを設定するDynamicParametersオブジェクト。</param>
        /// <param name="prefix">指定すると、{prefix}.{列名} = @～ とう形式で検索する。</param>
        /// <param name="searchTextSuffix">@パラメータの名前が被らないように付ける文字列。
        /// (指定しない場合は検索対象カラムの先頭を付与する。同じSQL用の文字列で、同じカラムを対象として複数回使用する場合は、適当に被らない値を指定すること。)</param>
        /// <param name="needBeforeAndWord">完成したSQLの前にANDを足すかどうか。</param>
        /// <param name="AndOr">各検索文字列に対するSQLを、ANDとORのどちらで結合するかどうか</param>
        /// <returns></returns>
        protected string CreateSearchText(string[] SearchParams, string[] targetColumns, DynamicParameters param, string prefix = "",
        string searchTextSuffix = "", bool needBeforeAndWord = false, string AndOr = AndOrTerm.OR)
        {
            StringBuilder sb = new StringBuilder();
            prefix = string.IsNullOrWhiteSpace(prefix) ? "" : prefix + ".";

            searchTextSuffix = string.IsNullOrEmpty(searchTextSuffix) ? targetColumns.FirstOrDefault() : searchTextSuffix;
            var searchText = "@SearchText" + searchTextSuffix;

            string result =
                CreateMultiWordWhereSql(SearchParams, param, (parameters, index) =>
                {
                    sb.Clear();

                    sb.Append(" (");
                    for (int i = 0; i <= targetColumns.Count() - 1; i++)
                    {
                        sb.Append(prefix).Append(targetColumns[i]).Append(@" LIKE ('%' + ").Append(searchText).Append(index).Append(" +'%')");
                        if (i != targetColumns.Count() - 1)
                            sb.Append($" OR ");
                    }
                    sb.Append(" ) ");

                    parameters.Add(searchText + index, SearchParams[index], dbType: DbType.String);

                    return sb.ToString();
                }, needBeforeAndWord, AndOr);

            return result;
        }


        /// <summary>
        /// ソート用の文字列を作成する。
        /// </summary>
        /// <param name="sortParams"></param>
        /// <param name="_prefix"></param>
        /// <returns></returns>
        public static string CreateSortOrderStr(IEnumerable<SortParam> sortParams, string _prefix = "", bool needOrderBy = true)
        {
            if (sortParams.IsNullOrEmpty())
                return null;

            StringBuilder sb = new StringBuilder();

            bool isFirst = true;

            foreach (var each in sortParams)
            {
                string prefix = string.IsNullOrWhiteSpace(_prefix) ? "" : _prefix + ".";

                if (isFirst)
                    isFirst = false;
                else
                    sb.Append(",");

                //複数カラムをまとめてソートする場合はカンマで区切っておく。
                string[] splited = each.ColumnName.Split(COMMA);
                for (int i = 0; i < splited.Length; i++)
                {
                    string splitedCol = splited[i];
                    sb.Append(prefix).Append(splitedCol).Append(" ").Append(each.Order);

                    if (splited.Length != 1 && i != splited.Length - 1)
                        sb.Append(",");
                }
            }

            sb.Insert(0, (needOrderBy ? " ORDER BY " : " ")).Append(" ");
            return sb.ToString();
        }

        public const string EachRowDelimiter = "###";
        public const string EachFieldDelimiter = "%%%";

        /// <summary>
        /// SQLでFOR XML PATH('')を使って取得した文字列からモデルを作成する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseText"></param>
        /// <param name="setInfo"></param>
        /// <returns></returns>
        public static List<T> GetResultFromSqlXmlText<T>(string baseText, Func<string[], T> CreateModel) where T : new()
        {
            var result = new List<T>();
            if (string.IsNullOrEmpty(baseText))
                return result;

            string[] rowList = baseText.Split(new string[] { EachRowDelimiter }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();

            foreach (string eachRow in rowList)
            {
                string[] fields = eachRow.Split(new string[] { EachFieldDelimiter }, StringSplitOptions.None)
                    //.Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();

                var model = CreateModel(fields);
                result.Add(model);
            }
            return result;
        }

        /// <summary>
        /// パスワードのハッシュ値を取得する。
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static byte[] GetPasswordHash(string password)
        {
            return CommonUtil.CreateSHA256Hash("OWM" + password);
        }

        /// <summary>
        /// 区分値取得のサブクエリを取得する。
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <param name="GeneralNoColumn"></param>
        /// <returns></returns>
        public static string CreateGeneralNameSubQuery(string CategoryId, string GeneralNoColumn)
        {
            return $@" SELECT GeneralName FROM M_General WHERE CategoryId = '{CategoryId}' AND GeneralNo = {GeneralNoColumn} ";
        }

        /// <summary>
        /// 課取得のサブクエリを取得する。
        /// </summary>
        /// <param name="DivisionNoColumn"></param>
        /// <returns></returns>
        public static string CreateDivisionNameSubQuery(string DivisionNoColumn)
        {
            return $@" SELECT DivisionName FROM M_Division WHERE DivisionNo = {DivisionNoColumn} ";
        }

        /// <summary>
        /// 備考取得のサブクエリを取得する。
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <param name="GeneralNoColumn"></param>
        /// <returns></returns>
        public static string CreateRemarksSubQuery(string CategoryId, string GeneralNoColumn)
        {
            return $@" SELECT Remarks FROM M_General WHERE CategoryId = '{CategoryId}' AND GeneralNo = {GeneralNoColumn} ";
        }

        /// <summary>
        /// 一意制約違反の例外かをチェックする。
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsUniqueException(Exception ex)
        {
            return SqlHelper.IsUniqueConstException(ex);
        }

        #endregion


        #region 非同期用Transaction

        /// <summary>
        /// 既存のトランザクションに参加する。
        /// </summary>
        public void EnlistTransaction()
        {
            Connection?.EnlistTransaction(Transaction.Current);
        }

        private static readonly TimeSpan TransactionTimeout = new TimeSpan(0, 3, 0);

        /// <summary>
        /// 非同期に対応したTransactionScopeを作成する。
        /// </summary>
        /// <returns></returns>
        public TransactionScope AsyncTransactionScope(TimeSpan? timeSpan = null)
        {
            timeSpan = timeSpan ?? TransactionTimeout;
            var tranOption = new TransactionOptions()
            {
                Timeout = timeSpan.Value,
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            };

            var tran = new TransactionScope(TransactionScopeOption.Required, tranOption, TransactionScopeAsyncFlowOption.Enabled);
            EnlistTransaction();
            return tran;

        }

        /// <summary>
        /// 非同期に対応したTransactionScopeを作成する。
        /// </summary>
        /// <returns></returns>
        public TransactionScope AsyncTransactionScope(Transaction transactionToUse)
        {
            var tran = new TransactionScope(transactionToUse, TransactionScopeAsyncFlowOption.Enabled);
            EnlistTransaction();
            return tran;
        }

        /// <summary>
        /// 非同期に対応したTransactionScopeを作成する。
        /// </summary>
        /// <returns></returns>
        public TransactionScope AsyncTransactionScope(TransactionScopeOption scopeOption)
        {
            var tran = new TransactionScope(scopeOption, TransactionScopeAsyncFlowOption.Enabled);
            EnlistTransaction();
            return tran;
        }

        /// <summary>
        /// 非同期に対応したTransactionScopeを作成する。
        /// </summary>
        /// <returns></returns>
        public TransactionScope AsyncTransactionScope(Transaction transactionToUse, TimeSpan scopeTimeout)
        {
            var tran = new TransactionScope(transactionToUse, scopeTimeout, TransactionScopeAsyncFlowOption.Enabled);
            EnlistTransaction();
            return tran;
        }

        /// <summary>
        /// 非同期に対応したTransactionScopeを作成する。
        /// </summary>
        /// <returns></returns>
        public TransactionScope AsyncTransactionScope(TransactionScopeOption scopeOption, TimeSpan scopeTimeout)
        {
            var tran = new TransactionScope(scopeOption, scopeTimeout, TransactionScopeAsyncFlowOption.Enabled);
            EnlistTransaction();
            return tran;
        }

        /// <summary>
        /// 非同期に対応したTransactionScopeを作成する。
        /// </summary>
        /// <returns></returns>
        public TransactionScope AsyncTransactionScope(TransactionScopeOption scopeOption, TransactionOptions transactionOptions)
        {
            var tran = new TransactionScope(scopeOption, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
            EnlistTransaction();
            return tran;
        }

        #endregion




    }
}