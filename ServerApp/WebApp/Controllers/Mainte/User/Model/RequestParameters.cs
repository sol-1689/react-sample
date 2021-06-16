using Domain.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Domain.Model;

namespace WebApp.Model
{
    /// <summary>
    /// 検索リクエストを受け付けるパラメータ。
    /// </summary>
    public class UserMainteSearchRequestParameter : UserSearchParam, ISortParams
    {

    }

    public class UserMainteDeleteRequestParameter
    {
        [Required]
        public int? UserNo { get;  set; }
        [Required]
        public bool IsDeleted { get;  set; }
    }
}
