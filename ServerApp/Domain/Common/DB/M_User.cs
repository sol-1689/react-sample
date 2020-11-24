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
		public int? UserNo { get; set; }
		public string UserName { get; set; }
		//public int? DivisionNo { get; set; }
		//public string OwnerId { get; set; }
		public string LoginId { get; set; }
		public byte[] Password { get; set; }
		public int? Role { get; set; }
		public int? CreateBy { get; set; }
		public DateTime? CreateDateTime { get; set; }
		public int? UpdateBy { get; set; }
		public DateTime? UpdateDateTime { get; set; }
		public int? DeleteFlag { get; set; }
	}
}
