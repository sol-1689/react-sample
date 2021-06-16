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
		public virtual int? BusinessCodeNo { get; set; }
		public virtual int? AccountingCodeNo { get; set; }
		public virtual string DebitBusinessCode { get; set; }
		public virtual string DebitBusinessName { get; set; }
		public virtual string DebitAccountingItemCode { get; set; }
		public virtual string DebitAccountingAssistItemCode { get; set; }
		public virtual string DebitTaxCode { get; set; }
		public virtual string CreditBusinessCode { get; set; }
		public virtual string CreditAccountingItemCode { get; set; }
		public virtual string CreditAccountingAssistItemCode { get; set; }
		public virtual string CreditTaxCode { get; set; }
		public virtual int? PaymentFlag { get; set; }
		public virtual int? CreateBy { get; set; }
		public virtual DateTime? CreateDateTime { get; set; }
		public virtual int? UpdateBy { get; set; }
		public virtual DateTime? UpdateDateTime { get; set; }
		public virtual int? DeleteFlag { get; set; }
	}
}
