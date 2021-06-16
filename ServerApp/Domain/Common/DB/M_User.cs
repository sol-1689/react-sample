using Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DB.Model
{
	/// <summary>
	/// M_Userテーブルのメンバーを保持するモデル。
	/// </summary>
	public class M_User
    {
		public virtual int? UserNo { get; set; }
		public virtual string UserName { get; set; }
		public virtual string LoginId { get; set; }
		public virtual byte[] Password { get; set; }
		public virtual int? Role { get; set; }
		public virtual int? CreateBy { get; set; }
		public virtual DateTime? CreateDateTime { get; set; }
		public virtual int? UpdateBy { get; set; }
		public virtual DateTime? UpdateDateTime { get; set; }
		public virtual int? DeleteFlag { get; set; }
	}
}
