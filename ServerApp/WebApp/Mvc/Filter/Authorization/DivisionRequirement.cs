using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Model;
using Microsoft.Extensions.DependencyInjection;
using Domain.Model;

namespace WebApp
{
    /// <summary>
    /// 実行権限がある課を定義する。
    /// enumで初期化できるようにAuthorizeAttributeを継承。
    /// </summary>
    public class AllowDivisionAttribute : AuthorizeAttribute
    {
        public const string POLICY_PREFIX = "AllowDivision";

        DivisionCode[] divisionList;

        public AllowDivisionAttribute(params DivisionCode[] divisions)
        {
            divisionList = divisions;
            Policy = CustomPolicyProvider.CreatePolicyString(POLICY_PREFIX, divisions);
        }
    }

    /// <summary>
    /// 実行権限がない課を定義する。
    /// enumで初期化できるようにAuthorizeAttributeを継承。
    /// </summary>
    public class DenyDivisionAttribute : AuthorizeAttribute
    {
        public const string POLICY_PREFIX = "DenyDivision";
        DivisionCode[] divisionList;

        public DenyDivisionAttribute(params DivisionCode[] divisions)
        {
            divisionList = divisions;
            Policy = CustomPolicyProvider.CreatePolicyString(POLICY_PREFIX, divisions);
        }
    }

    public class AuthDivisionRequirement : IAuthorizationRequirement
    {
        public DivisionCode[] DivisionList { get; set; }
        public bool IsAllowDivision { get; set; }

        public AuthDivisionRequirement(bool isAllowDivision, params DivisionCode[] divisions)
        {
            DivisionList = divisions;
            IsAllowDivision = isAllowDivision;
        }
    }

    public class AuthDivisionHandler : AuthorizationHandler<AuthDivisionRequirement>
    {
        LoginUserContext LoginContext { get; set; }

        public AuthDivisionHandler(LoginUserContext loginContext)
        {
            this.LoginContext = loginContext;
        }

        /// <summary>
        /// 権限のチェックを行う。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AuthDivisionRequirement requirement)
        {
            var userDivisionList = LoginContext?.DivisionNoListAsEnum;
            var auth = LoginContext.Auth;

            if(auth == AuthFlag.ADMIN)
            {
                //管理者権限は常に成功。
                context.Succeed(requirement);
            }
            else if (userDivisionList != null)
            {
                //属性で指定した課と、ログインユーザーの課を比較。
                bool isContain = requirement.DivisionList.Any(d => userDivisionList.Any(ud => ud == d));
                if ((requirement.IsAllowDivision && isContain) || (!requirement.IsAllowDivision && !isContain))
                {
                    //権限があれば成功。
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}

