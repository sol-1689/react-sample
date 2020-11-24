using Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DB.Model
{
	/// <summary>
	/// M_Generalテーブルのメンバーを保持するモデル。
	/// </summary>
	public class M_General
    {
		public int? GeneralNo { get; set; }
		public string GeneralName { get; set; }
		public int? OrderNo { get; set; }
		public string Remarks { get; set; }
		public string Description { get; set; }
		public string CategoryId { get; set; }
		public string CategoryName { get; set; }

		public DateTime? CreateDateTime { get; set; }
		public DateTime? UpdateDateTime { get; set; }
		public int? DeleteFlag { get; set; }
	}
}
