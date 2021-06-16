using Domain.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Model
{
    public class BusinessCodeMainteSearchRequestParameter : ISortParams
    {
        public int? SelectedAccountingCode { get; set; }
        public string SearchDebitBusinessCode { get; set; }
        public string SearchDebitBusinessName { get; set; }
        public bool ShowDelete { get; set; }


        public List<SortParam> SortParams { get; set; }
        public PagingParam PagingParam { get; set; }
    }
}
