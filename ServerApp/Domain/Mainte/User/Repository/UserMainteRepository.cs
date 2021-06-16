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
    public class UserMainteRepository : CommonRepository, IUserMainteRepository
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="connection">IDbConnectionを外部から渡したい場合に使用。</param>
        public UserMainteRepository(CustomProfiledDbConnection connection)
            : base(connection)
        {
        }

        /// <summary>
        /// ソート対象の列名。
        /// </summary>
        static readonly IReadOnlyCollection<string> SortColumns = new string[] { nameof(M_User.UserName),
            nameof(M_User.LoginId), "DivisionNoStr", nameof(M_User.Role) };

        /// <summary>
        /// ユーザー一覧を取得する。
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns>合計件数と一覧の名前付きタプル型を返す。</returns>
        public async Task<(int TotalCount, List<UserSearchResult> List)> GetList(UserSearchParam model)
        {
            DynamicParameters parameter = new DynamicParameters(model.PagingParam);
            string whereSql = CreateSearchText(model, parameter);
            var sortParams = ISortParamsExtensions.CheckSortParams(model.SortParams, SortColumns); //SQLインジェクションを防ぐ為、画面から送られてきたソート用パラメータが想定した文字列かチェックする。
            
            string dispOrder = CreateSortOrderStr(sortParams);
            string orderText = string.IsNullOrEmpty(dispOrder) ? " ORDER BY UserNo" : dispOrder;

            //SQL内にコメント書くとDockerではSQLが変になる(多分改行のせい)
            string sql =
$@"SELECT * FROM
(
     SELECT 
        main.UserNo
	   ,main.UserName
	   ,main.LoginId
	   ,main.Role
       ,({CreateGeneralNameSubQuery(ConstKubun.Role, "main.Role")}) AS RoleName
	   ,main.DeleteFlag
       ,COUNT(*) OVER() AS TotalCount
       ,(
         SELECT 
          CONVERT(varchar, udiv.DivisionNo) + '{EachFieldDelimiter}' + ISNULL(div.DivisionName, '') + '{EachRowDelimiter}'
         FROM M_UserDivision udiv
         INNER JOIN M_Division div
           ON udiv.DivisionNo = div.DivisionNo
           AND div.DeleteFlag = {ConstFlag.FalseValue}
         WHERE udiv.UserNo = main.UserNo
         FOR XML PATH('')
        ) AS DivisionNoStr
  FROM M_User main
  WHERE 
  {whereSql}
)as base
{orderText}
OFFSET @StartRow ROWS FETCH NEXT @RowCount ROWS ONLY
";
            int totalCount = 0;
            //SELECT結果とTotalCountを取得。
            var list = (await Connection.QueryAsync<UserSearchResult, int, string, UserSearchResult>(sql,
                (user, _totalCount, divisionNoStr) =>
                {
                    totalCount = _totalCount;

                    user.DivisionNoList = new List<int>();

                    var _ = GetResultFromSqlXmlText(divisionNoStr, (fields) =>
                    {
                        if (int.TryParse(fields.ElementAtOrDefault(0, null), out var d))
                        {
                            user.DivisionNoList.Add(d);
                            user.DivisionNameList.Add(fields.ElementAtOrDefault(1, ""));
                        }

                        return (int?)null; //適当に返す。
                    });

                    return user;
                }, parameter, splitOn: "TotalCount,DivisionNoStr")) //TotalCountはSelect句の末尾に書くこと。
                .ToList();

            var result = (TotalCount: totalCount, List: list);
            return result;
        }

        /// <summary>
        /// ユーザー検索のWhere句を作成する。
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="paramater"></param>
        /// <returns></returns>
        private string CreateSearchText(UserSearchParam model, DynamicParameters paramater)
        {
            List<string> whereSql = new List<string>();

            //ユーザ名等
            if (!String.IsNullOrEmpty(model.SearchUserName))
            {
                whereSql.Add(base.CreateSearchText(CommonUtil.SplitSearchText(model.SearchUserName), new String[]{"UserName"},  paramater, "main"));
            }

            //ユーザーID
            if (!String.IsNullOrEmpty(model.SearchLoginId))
            {
                whereSql.Add(base.CreateSearchText(CommonUtil.SplitSearchText(model.SearchLoginId), new String[]{"LoginId"},  paramater, "main"));
            }

            //担当
            if (model.SearchDivisionNo != null)
            {
                paramater.Add("@DivisionNo", model.SearchDivisionNo, dbType: DbType.Int32);
                whereSql.Add(@" EXISTS (SELECT * FROM M_UserDivision WHERE UserNo = main.UserNo AND DivisionNo = @DivisionNo) ");
            }

            //役割
            if (model.SearchRole != null)
            {
                paramater.Add("@Role", model.SearchRole, dbType: DbType.Int32);
                whereSql.Add(@" main.Role = @Role ");
            }

            //主キー指定する場合
            if (model.SearchUserNo != null)
            {
                paramater.Add("@UserNo", model.SearchUserNo);
                whereSql.Add(@" main.UserNo = @UserNo ");
            }


            //直接主キーが指定されている場合はDeleteFlagはチェックしない。
            if (model.SearchUserNo == null)
            {
                //削除フラグ
                paramater.Add("@DeleteFlag", model.ShowDelete.AsFlag(), dbType: DbType.Int32);
                whereSql.Add(@" main.DeleteFlag = @DeleteFlag ");
            }

            return whereSql.IsNullOrEmpty() ? "" : string.Join(" AND ", whereSql);
        }


        /// <summary>
        /// アカウントの件数を取得する。(重複チェックに利用を想定。)
        /// </summary>
        /// <param name="LoginId"></param>
        /// <returns></returns>
        public async Task<int> GetAccountCount(string LoginId, int? UserNo = null)
        {
            string sql =
@"SELECT COUNT(*)
  FROM M_User
  WHERE LoginId = @LoginId
";
            if(UserNo != null)
                sql += " AND UserNo != @UserNo ";
                
            int result = (await Connection.QueryAsync<int?>(sql, new { LoginId, UserNo })).FirstOrDefault() ?? 0;
            return result;
        }

        /// <summary>
        /// UserNoの作成。
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<int> CreateUserNo()
        {
            string sql =
@"SELECT
 NEXT VALUE FOR Seq_M_User
";
            int result = (await Connection.QueryAsync<int>(sql)).FirstOrDefault();
            return result;
        }


        /// <summary>
        /// 変更情報を更新する。
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<int> Update(UserRegisterModel model)
        {
            string sql =
$@"UPDATE M_User
 SET
  UserName = @UserName,
  LoginId = @LoginId,
  Role = @Role,
  UpdateBy = @UpdateBy,
  UpdateDateTime = GETDATE(),
  DeleteFlag = @DeleteFlag
";

            DynamicParameters parameter = new DynamicParameters(model);

            if (!string.IsNullOrEmpty(model.PasswordStr))
            {
                sql += " ,Password = @Password ";
                parameter.Add("Password", GetPasswordHash(model.PasswordStr));
            }
            sql += " WHERE UserNo = @UserNo";

            int result = await Connection.ExecuteAsync(sql, parameter);
            return result;
        }

        /// <summary>
        /// データ情報を登録する。
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<int> Insert(UserRegisterModel model)
        {
            string sql =
@"INSERT INTO M_User(
   UserNo
  ,UserName
  ,LoginId
  ,Password
  ,Role
  ,CreateBy
  ,CreateDateTime
  ,UpdateBy
  ,UpdateDateTime
  ,DeleteFlag
)VALUES (
   @UserNo
  ,@UserName
  ,@LoginId
  ,@Password
  ,@Role
  ,@CreateBy
  ,GETDATE()
  ,@UpdateBy
  ,GETDATE()
  ,@DeleteFlag
)";
            model.Password = GetPasswordHash(model.PasswordStr);
            DynamicParameters parameter = new DynamicParameters(model);
            //.parameter.Add("Password", GetPasswordHash(model.PasswordStr));
            int result = await Connection.ExecuteAsync(sql, parameter);
            return result;
        }

        /// <summary>
        /// ユーザー・課の関連情報を登録する。
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<int> InsertDivision(int UserNo, int DivisionNo)
        {
            string sql =
@"INSERT INTO M_UserDivision(
   UserNo
  ,DivisionNo
)VALUES (
   @UserNo
  ,@DivisionNo
)";
            int result = await Connection.ExecuteAsync(sql, new { UserNo, DivisionNo });
            return result;
        }

        /// <summary>
        /// ユーザー・課の関連情報を削除する。
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<int> DeleteDivision(int UserNo)
        {
            string sql =
@"DELETE FROM M_UserDivision
WHERE UserNo = @UserNo";

            int result = await Connection.ExecuteAsync(sql, new { UserNo });
            return result;
        }

        /// <summary>
        /// データを削除する。
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<int> Delete(int UserNo, int DeleteFlag, int? UpdateBy)
        {
            string sql =
 $@"Update M_User 
SET
    UpdateBy = @UpdateBy,
    UpdateDateTime = GETDATE(),
    DeleteFlag = @DeleteFlag
 WHERE UserNo = @UserNo
";
            var result = await Connection.ExecuteAsync(sql, new { UserNo, UpdateBy, DeleteFlag });
            return result;
        }

        /// <summary>
        /// データ変更情報をMERGEする。
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        //        public int InsertUpdateR_User_Team(M_SAIN_User_View model)
        //        {
        //            string sql =
        //@"MERGE INTO R_SAIN_User_Team AS target
        //        USING
        //        (
        //          SELECT 
        //            @user_no AS user_no,
        //            @team_no AS team_no   
        //        ) AS source
        //        ON 
        //        target.team_no = source.team_no
        //         AND target.user_no = source.user_no
        //        WHEN MATCHED THEN
        //        UPDATE SET
        //            target.user_no = @user_no,
        //            target.team_no = @team_no,
        //            target.update_user = @update_user,
        //            target.update_date = @update_date
        //        WHEN NOT MATCHED THEN
        //        INSERT VALUES 
        //        (
        //            @user_no,
        //            @team_no,
        //            '0',
        //            @update_user,
        //            @update_date
        //        );";
        //            var result = Connection.Execute(sql, model);
        //            return result;
        //        }


                // /// <summary>
        // /// ユーザー検索のWhere句を作成する。
        // /// </summary>
        // /// <param name="SearchParams"></param>
        // /// <param name="param"></param>
        // /// <param name="prefix"></param>
        // /// <param name="needBeforeAndWord"></param>
        // /// <returns></returns>
        // private string CreateSearchText(string[] SearchParams, DynamicParameters param,
        //     string prefix = "", bool needBeforeAndWord = false)
        // {
        //     StringBuilder sb = new StringBuilder();
        //     prefix = string.IsNullOrWhiteSpace(prefix) ? "" : prefix + ".";

        //     string result =
        //         CreateMultiWordWhereSql(SearchParams, param, (parameters, index) =>
        //         {
        //             sb.Clear();

        //             sb.Append(" (");
        //             sb.Append(prefix).Append(@"UserName LIKE ('%' + @SearchText").Append(index).Append(" +'%')");
        //             sb.Append(" OR ");
        //             sb.Append(prefix).Append(@"LoginId LIKE ('%' + @SearchText").Append(index).Append(" +'%')");
        //             sb.Append(" ) ");

        //             parameters.Add("@SearchText" + index, SearchParams[index], dbType: DbType.String);

        //             return sb.ToString();
        //         }, needBeforeAndWord);

        //     return result;
        // }

    }
}
