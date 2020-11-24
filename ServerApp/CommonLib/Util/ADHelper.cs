using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Security.Principal;
using System.Text;

namespace SAIN
{
    public class ADHelper
    {
        ADInfo info;
        ILoggerFactory loggerFactory;

        public ADHelper(ADInfo info, ILoggerFactory loggerFactory)
        {
            this.info = info;
            this.loggerFactory = loggerFactory;
        }

        public bool getIsAuthLdap()
        {
            return info.IsLdapAuth;
        }

        /// <summary>
        /// LDAPによるADアクセス結果を取得
        /// </summary>
        /// <param name="isAuth">
        ///                     認証の要否
        ///                     ※オーバーライドにしないのは明確に認証の要否を意図した上で使用する為。
        /// </param>
        /// <param name="userId">ユーザID</param>
        /// <param name="password">パスワード</param>
        /// <returns>LDAPによるAD認証（検索）結果</returns>
        private SearchResult GetLdapSearchResult(bool isAuth, string userId = null, string password = null)
        {
            try
            {
                DirectoryEntry entry;
                if (isAuth)
                    entry = new DirectoryEntry(info.LdapQuery, userId, password);
                else
                    entry = new DirectoryEntry(info.LdapQuery);

                DirectorySearcher searcher = new DirectorySearcher(entry);
                searcher.Filter = "(SAMAccountName=" + userId + ")";
                return searcher.FindOne();
            }
            catch (DirectoryServicesCOMException ex)
            {
                // 認証失敗時にはExceptionになる
                if (ex.ErrorCode == -2147023570)
                {
                    return null;
                }
                throw ex;
            }
        }

        /// <summary>
        /// SID取得
        /// </summary>
        /// <param name="searchResult"></param>
        /// <returns></returns>
        private string GetSid(SearchResult searchResult)
        {
            if (searchResult == null)
                return string.Empty;

            var objectSid = searchResult.GetDirectoryEntry().Properties["objectSid"].Value as byte[];
            return new SecurityIdentifier(objectSid, 0).ToString(); //byte配列からStringに変換しているだけ。
        }

        /// <summary>
        /// LDAP認証
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns>認証成功：SID、認証失敗：空文字</returns>
        public string AuthADByLdap(string userId, string password)
        {
            return GetSid(GetLdapSearchResult(true, userId, password));
        }

        /// <summary>
        /// UserIdによるSID検索
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetSidByUserId(string userId)
        {
            return GetSid(GetLdapSearchResult(false, userId));
        }
    }


    public class ADInfo
    {
        public bool IsLdapAuth { get; set; }
        public string LdapQuery { get; set; }

        public ADInfo(bool isLdapAuth,string ldapQuery)
        {
            IsLdapAuth = isLdapAuth;
            LdapQuery = ldapQuery;
        }

    }
}
