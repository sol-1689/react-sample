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
    /// データ登録時の重複エラーを表す例外。
    /// </summary>
    public class DuplicationException: Exception
    {
        public DuplicationException(Exception ex) 
            : base("重複エラー。", ex) 
        { }

    }

}
