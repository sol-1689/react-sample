using Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DB.Model
{
	/// <summary>
	/// M_BusinessCodeテーブルのメンバーを保持するモデル。
	/// </summary>
	public class M_BusinessCode
	{
		public int? BusinessCodeNo { get; set; }
		public int? AccountingCodeNo { get; set; }
		public string DebitBusinessCode { get; set; }
		public string DebitBusinessName { get; set; }
		public string DebitAccountingItemCode { get; set; }
		public string DebitAccountingAssistItemCode { get; set; }
		public string DebitTaxCode { get; set; }
		public string CreditBusinessCode { get; set; }
		public string CreditAccountingItemCode { get; set; }
		public string CreditAccountingAssistItemCode { get; set; }
		public string CreditTaxCode { get; set; }
		public int? PaymentFlag { get; set; }
		public int? CreateBy { get; set; }
		public DateTime? CreateDateTime { get; set; }
		public int? UpdateBy { get; set; }
		public DateTime? UpdateDateTime { get; set; }
		public int? DeleteFlag { get; set; }
	}
}
