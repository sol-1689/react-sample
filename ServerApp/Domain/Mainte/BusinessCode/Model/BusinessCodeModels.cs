using Domain.DB.Model;
using Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Model
{
	/// <summary>
	/// 事業コード情報検索結果用モデル。
	/// </summary>
	public class BusinessCodeSearchResult : M_BusinessCode
	{
		public bool IsDeleted { get { return DeleteFlag.AsFlag(); } }
		public bool IsPaid { get { return PaymentFlag.AsFlag(); } }

		public string AccountingCode { get; set; }
		public string AccountingCodeName { get; set; }
	}

	/// <summary>
	/// 事業コード情報登録用のモデル。
	/// </summary>
	public class BusinessCodeRegisterModel : M_BusinessCode
	{
		public bool IsPaid { get; set; }
		public bool IsDeleted { get;  set; }

	}
}
