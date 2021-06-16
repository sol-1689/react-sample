using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;
using Domain.Util;

namespace Domain.Model
{
    /// <summary>
    /// ログインユーザに関する情報を保持する。
    /// </summary>
    [Serializable]
    public class LoginUserContext
    {
        public const string InSessionKey = "COMMON_INFO_IN_SESSION";

        //public LoginUserContext(){}

        public LoginUserContext(LoginUserInfo loginUserInfo)
        {
            this.loginUserInfo = loginUserInfo;
        }

        [JsonIgnore]
        public LoginUserInfo loginUserInfo {get; set;}

        public int? UserNo { get { return loginUserInfo?.UserNo; } }
        public string UserName { get { return loginUserInfo?.UserName; } }
        public int? Role { get { return loginUserInfo?.Role; } }

        /// <summary>権限フラグ </summary>
        public AuthFlag Auth
        {
            get
            {
                if(Role == null || !Enum.IsDefined(typeof(AuthFlag), Role.Value))
                {
                    return AuthFlag.NORMAL; //DBに値が無いか、想定しない値が入っていた場合は一般にしておく。
                }
                AuthFlag flag = (AuthFlag)Role.Value;
                return flag;
            }
        }

        /// <summary>一般権限かを取得する。</summary>
        public bool IsNormal { get { return Auth == AuthFlag.NORMAL; } }

        /// <summary>Admin権限かを取得する。</summary>
        public bool IsAdmin { get { return Auth == AuthFlag.ADMIN; } }

        /// <summary>所属する課の一覧を取得する。</summary>
        public List<int> DivisionNoList { get { return loginUserInfo?.DivisionNoList; } }

        /// <summary>所属する課の一覧を取得する。</summary>
        [JsonIgnore]
        public List<DivisionCode> DivisionNoListAsEnum
        {
            get
            {
                return loginUserInfo?.DivisionNoList.Select(d =>
                        {
                            if (!Enum.IsDefined(typeof(DivisionCode), d))
                            {
                                return DivisionCode.OTHER; //DBに値が無いか、想定しない値が入っていた場合はその他にしておく。
                            }
                            DivisionCode flag = (DivisionCode)d;
                            return flag;
                        }).ToList();
            }
        }

        /// <summary>総務課かを取得する。</summary>
        public bool IsSoumu {get { return DivisionNoList?.Any(d => d == (int)DivisionCode.Soumu) == true; }}

        /// <summary>経理課かを取得する。</summary>
        public bool IsAccounting {get { return DivisionNoList?.Any(d => d == (int)DivisionCode.Accounting) == true; }}
    }

    public static class LoginContextExtensions
    {
        ///// <summary>
        ///// 対象の権限を保持しているかを確認する。
        ///// </summary>
        ///// <param name="loginContext"></param>
        ///// <param name="auth"></param>
        ///// <returns></returns>
        //public static bool ContainsAuth(this LoginUserContext loginContext, params AuthFlag[] auth)
        //{
        //    if (auth.IsNullOrEmpty() || loginContext.Auth == null)
        //        return false;

        //    bool result = auth.Any(m => m == loginContext.Auth);
        //    return result;
        //}



    }

}