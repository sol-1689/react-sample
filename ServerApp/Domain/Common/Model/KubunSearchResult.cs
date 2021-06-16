
using System;
using Domain.DB.Model;

namespace Domain.Model
{
    public class KubunSearchResult
    {
        public string Value {get; set;}
        public string Text {get; set;}
        public string Remarks {get; set;}
        
        public bool DisplayDisabled {get;set;}

        //public bool IsSelected {get; set;}
    }

    // public class DivisionSearchResult: M_Division
    // {

    // }
}
