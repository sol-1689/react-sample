using Domain.DB.Model;
using Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Model
{

	/// <summary>
	/// ユーザー検索条件を保持したモデル。
	/// </summary>
	public class UserSearchParam
    {
        public string SearchUserName { get; set; }
        public string SearchLoginId { get; set; }
        public int? SearchDivisionNo { get; set; }
        public int? SearchRole { get; set; }
        public bool ShowDelete { get; set; }
        public List<SortParam> SortParams { get; set; }
        public PagingParam PagingParam { get; set; }

		/// <summary>主キー指定で抽出したい場合</summary>
		public int? SearchUserNo { get; set; }
	}


	/// <summary>
	/// ユーザー情報検索結果用モデル。
	/// </summary>
	public class UserSearchResult : M_User
	{
		public List<int> DivisionNoList { get; set; } = new List<int>();
		public List<string> DivisionNameList { get; set; } = new List<string>();

		public bool IsDeleted { get { return DeleteFlag.AsFlag(); } }
		public string RoleName{ get; set; }
	}

	/// <summary>
	/// ユーザー情報登録用のモデル。
	/// </summary>
	public class UserRegisterModel : M_User
	{
		public List<int> DivisionNoList { get; set; }

		public string PasswordStr { get; set; }
		public bool IsDeleted { get;  set; }
	}
}
