using Domain.Model;
using Domain.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace Domain.Repository
{
    public class LoginRepository : CommonRepository, ILoginRepository
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="connection">IDbConnectionを外部から渡したい場合に使用。</param>
        public LoginRepository(CustomProfiledDbConnection connection)
            : base(connection)
        {
        }

        /// <summary>
        /// ID・パスワードからユーザを取得する。
        /// </summary>
        /// <param name="LoginId">ログインID</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<LoginUserInfo> GetUserByLoginInfo(string LoginId, string password)
        {
            string sql =
$@"SELECT 
     UserNo
    ,UserName
    ,LoginId
    ,Role
    ,(
        SELECT 
         CONVERT(varchar, udiv.DivisionNo) + '{EachRowDelimiter}'
        FROM M_UserDivision udiv
        INNER JOIN M_Division div
           ON udiv.DivisionNo = div.DivisionNo
           AND div.DeleteFlag = {ConstFlag.FalseValue}
        WHERE udiv.UserNo = u.UserNo
        FOR XML PATH('')
    ) AS DivisionNoStr
 FROM M_User u
 WHERE LoginId = @LoginId
  AND Password = @Password
  AND DeleteFlag = '{ConstFlag.FalseValue}'
";

            var result = (await Connection.QueryAsync<LoginUserInfo, string, LoginUserInfo>(sql,
                (user, divisionNoStr) =>
                {
                    var divisionNoList = GetResultFromSqlXmlText(divisionNoStr, (fields) =>
                    {
                        int? divisionNo = int.TryParse(fields.ElementAtOrDefault(0, null), out var d) ? d : (int?)null;
                        return divisionNo;
                    });
                    user.DivisionNoList = divisionNoList.Where(d => d != null).Select(d => d.Value).ToList();

                    return user;
                }, new { LoginId, Password = GetPasswordHash(password) }, splitOn: "DivisionNoStr")).FirstOrDefault();

            return result;
        }
    }
}
