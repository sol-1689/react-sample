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
		public virtual int? GeneralNo { get; set; }
		public virtual string GeneralName { get; set; }
		public virtual int? OrderNo { get; set; }
		public virtual string Remarks { get; set; }
		public virtual string Description { get; set; }
		public virtual string CategoryId { get; set; }
		public virtual string CategoryName { get; set; }

		public virtual DateTime? CreateDateTime { get; set; }
		public virtual DateTime? UpdateDateTime { get; set; }
		public virtual int? DeleteFlag { get; set; }
	}
}
