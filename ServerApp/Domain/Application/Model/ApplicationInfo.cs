using Domain.DB.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Model
{
    /// <summary>
    /// アプリケーション情報
    /// </summary>
    [Serializable]
    public class ApplicationInfo
    {
        public List<M_General> GeneralList { get; set; }
    }
}
