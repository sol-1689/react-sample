using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Util
{
	/// <summary>
	/// DBの定数値を保持する。
	/// </summary>
	public class ConstDb
	{
		/// <summary>新DBへの接続文字列 </summary>
		public const string ConnectionStringName_NewDB = "NewDB";

		/// <summary>既存システムのDBへの接続文字列 </summary>
		public const string ConnectionStringName_KizonDB = "KizonDB";


    }

    public class ConstAccountingcodeNo
    {

    }

    public class ConstHonorificsNo
    {
    }

    /// <summary>
	/// ヒストリーの区分を取得する。
	/// </summary>
	public class ConstHisKbn
    {
        /// <summary>削除</summary>
        public const string Delete = "D";

        /// <summary>新規</summary>
        public const string Insert = "I";

        /// <summary>更新</summary>
        public const string Update = "U";
    }


}
