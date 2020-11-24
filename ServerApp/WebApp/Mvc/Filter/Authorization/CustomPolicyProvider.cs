using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp
{
    /// <summary>
    /// 承認用のカスタムプロバイダ。
    /// デフェルトだとPolicyのOR条件を定義できなかったので実装。
    /// </summary>
    public class CustomPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public const string POLICY_SPLITTER = "&";
        public const string AUTH_ROLE_SPLITTER = ",";

        public CustomPolicyProvider(IOptions<AuthorizationOptions> options)
            : base(options)
        {
        }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var policy = new AuthorizationPolicyBuilder();

            //AuthRole用。
            bool isAllowRolePlefix = policyName.StartsWith(AllowRoleAttribute.POLICY_PREFIX, StringComparison.OrdinalIgnoreCase);
            bool isDenyRolePlefix = policyName.StartsWith(DenyRoleAttribute.POLICY_PREFIX, StringComparison.OrdinalIgnoreCase);

            if (isAllowRolePlefix || isDenyRolePlefix)
            {
                var policyList = GetPolicyList<AuthFlag>(policyName);
                policy.AddRequirements(new AuthRoleRequirement(isAllowRolePlefix, policyList));
            }

            //Division用。
            bool isAllowDivisionPlefix = policyName.StartsWith(AllowDivisionAttribute.POLICY_PREFIX, StringComparison.OrdinalIgnoreCase);
            bool isDenyDivisionPlefix = policyName.StartsWith(DenyDivisionAttribute.POLICY_PREFIX, StringComparison.OrdinalIgnoreCase);

            if (isAllowDivisionPlefix || isDenyDivisionPlefix)
            {
                var policyList = GetPolicyList<DivisionCode>(policyName);
                policy.AddRequirements(new AuthDivisionRequirement(isAllowDivisionPlefix, policyList));
            }

            //カスタムの承認があればそれを使う。
            if (policy.Requirements.Any())
                return Task.FromResult(policy.Build());
            else
                return base.GetPolicyAsync(policyName); //それ以外はデフォルト。
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EnumType"></typeparam>
        /// <param name="policyName"></param>
        /// <returns></returns>
        EnumType[] GetPolicyList<EnumType>(string policyName)
        {
            var policy = new AuthorizationPolicyBuilder();
            string policyStr = policyName.Split(POLICY_SPLITTER).ElementAtOrDefault(1);

            var policyList = policyStr.Split(AUTH_ROLE_SPLITTER)
                .Select(s => (EnumType)Enum.Parse(typeof(EnumType), s))
                .ToArray();

            return policyList;
        }


        public static string CreatePolicyString<T>(string policy_pf, params T[] roles)
        {
            return $"{policy_pf}{POLICY_SPLITTER}{string.Join(AUTH_ROLE_SPLITTER, roles.OrderBy(m => m))}"; ;
        }


    }
}
