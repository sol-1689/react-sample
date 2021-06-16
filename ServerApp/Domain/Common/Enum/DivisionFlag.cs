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
    /// 課コード
    /// </summary>
    public enum DivisionCode
    {
        /// <summary>総務</summary>
        [Description("総務")]
        [Display(Name = "総務")]
        Soumu = 1,

        /// <summary>経理</summary>
        [Description("経理")]
        [Display(Name = "経理")]
        Accounting = 3,

        /// <summary>その他</summary>
        [Description("その他")]
        [Display(Name = "その他")]
        OTHER = -1,
    }

}
