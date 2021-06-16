using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Util;

namespace Domain.Model
{
    /// <summary>
    /// ページング情報を保持することを表す。
    /// </summary>
    public class ContextPathInfo
    {
        /// <summary>コンテキストパス </summary>
        public string ContextPath { get; set; }

        /// <summary>/なしのコンテキストパス </summary>
        public string NoSlashContextPath { get { return CommonUtil.RemoveSlash(ContextPath);} }

    }

}
