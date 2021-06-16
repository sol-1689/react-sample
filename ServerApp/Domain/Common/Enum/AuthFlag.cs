using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    /// <summary>
    /// 権限フラグ
    /// </summary>
    public enum AuthFlag
    {
        /// <summary>一般</summary>
        [Description("一般")]
        [Display(Name = "一般")]
        NORMAL = 10,
        /// <summary>リーダー</summary>
        [Description("代理")]
        [Display(Name = "代理")]
        DAIRI = 11,

        /// <summary>リーダー</summary>
        [Description("課長")]
        [Display(Name = "課長")]
        KACHO = 12,

        /// <summary>システム</summary>
        [Description("管理者")]
        [Display(Name = "管理者")]
        ADMIN = 13
    }

}
