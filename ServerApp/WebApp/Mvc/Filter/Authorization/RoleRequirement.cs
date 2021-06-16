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
    /// 実行権限があるロールを定義する。
    /// enumで初期化できるようにAuthorizeAttributeを継承。
    /// </summary>
    public class AllowRoleAttribute : AuthorizeAttribute
    {
        public const string POLICY_PREFIX = "AllowRole";

        AuthFlag[] roleList;

        public AllowRoleAttribute(params AuthFlag[] roles)
        {
            roleList = roles;
            Policy = CustomPolicyProvider.CreatePolicyString(POLICY_PREFIX, roles);
        }

    }


    /// <summary>
    /// 実行権限がないロールを定義する。
    /// enumで初期化できるようにAuthorizeAttributeを継承。
    /// </summary>
    public class DenyRoleAttribute : AuthorizeAttribute
    {
        public const string POLICY_PREFIX = "DenyRole";
        AuthFlag[] roleList;

        public DenyRoleAttribute(params AuthFlag[] roles)
        {
            roleList = roles;
            Policy = CustomPolicyProvider.CreatePolicyString(POLICY_PREFIX, roles);
        }
    }


    public class AuthRoleRequirement : IAuthorizationRequirement
    {
        public AuthFlag[] RoleList { get; set; }
        public bool IsAllowRole { get; set; }

        public AuthRoleRequirement(bool isAllowRole, params AuthFlag[] roles)
        {
            RoleList = roles;
            IsAllowRole = isAllowRole;
        }
    }

    public class AuthRoleHandler : AuthorizationHandler<AuthRoleRequirement>
    {
        LoginUserContext LoginContext { get; set; }

        public AuthRoleHandler(LoginUserContext loginContext)
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
            AuthRoleRequirement requirement)
        {
            var loginRole = LoginContext?.Auth;
            if (loginRole != null)
            {
                //属性で指定した権限と、ログインユーザーの権限を比較。
                bool isContain = requirement.RoleList.Any(r => r == loginRole);
                if ((requirement.IsAllowRole && isContain) || (!requirement.IsAllowRole && !isContain))
                {
                    //権限があれば成功。
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}

