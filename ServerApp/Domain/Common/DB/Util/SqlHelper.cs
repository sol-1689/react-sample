using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Reflection;

using System.Data.SqlClient;

using System.Collections;
using System.Transactions;

namespace Domain.DB
{
    /// <summary>
    /// DB処理のヘルパーメソッドを保持したクラス
    /// </summary>
	public class SqlHelper
    {
        //public const int SQL_UNIQUE_ERROR_INTEGER = -5;

        /// <summary>
        /// ストアドプロシージャを実行する。Model内に共通のカラム(update_user等)に対応するプロパティがない・もしくはNULLの場合は、自動的に設定する。
        /// </summary>
        /// <param name="procedureName">呼び出すプロシージャ名</param>
        /// <param name="models">Modelのリスト（DataTableに変換しプロシージャに送る）</param>
        /// <param name="typeName">タイプ名。指定しない場合はモデル名の頭にTYPE_をつけたものになる。</param>
        /// <param name="parameterName">パラメータ名。指定しない場合はモデル名の頭に@TYPE_をつけたものになる。</param>
        /// <param name="parameterAdjuster">プロシージャに渡すSqlParameterのリストを調整（SqlParameterの追加等）するデリゲート（任意）</param>
        /// <returns>プロシージャ実行時の影響件数</returns>
        public static int ExecuteStoredNonQueryOneModel<T>(string procedureName, T model,
            string typeName = null, string parameterName = null, Action<List<SqlParameter>> parameterAdjuster = null)
        {
            IEnumerable<T> models = new T[] { model };
            return ExecuteStoredNonQuery(procedureName, models, typeName, parameterName, parameterAdjuster);
        }

        /// <summary>
        /// ストアドプロシージャを実行する。Model内に共通のカラム(update_user等)に対応するプロパティがない・もしくはNULLの場合は、自動的に設定する。
        /// </summary>
        /// <param name="procedureName">呼び出すプロシージャ名</param>
        /// <param name="models">Modelのリスト（DataTableに変換しプロシージャに送る）</param>
        /// <param name="typeName">タイプ名。指定しない場合はモデル名の頭にTYPE_をつけたものになる。</param>
        /// <param name="parameterName">パラメータ名。指定しない場合はモデル名の頭に@TYPE_をつけたものになる。</param>
        /// <param name="parameterAdjuster">プロシージャに渡すSqlParameterのリストを調整（SqlParameterの追加等）するデリゲート（任意）</param>
        /// <returns>プロシージャ実行時の影響件数</returns>
        public static int ExecuteStoredNonQuery<T>(string procedureName, IEnumerable<T> models,
            string typeName = null, string parameterName = null, Action<List<SqlParameter>> parameterAdjuster = null)
        {
            string modelName = typeof(T).Name;

            typeName = typeName ?? "TYPE_" + modelName;
            parameterName = parameterName ?? "@TYPE_" + modelName;

            using (DataTable dataTable = CreateDataTableFromProperty(models, typeName))
            {
                SqlParameter tableParameter = CreateTableParameter(dataTable, typeName, parameterName);

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(tableParameter);

                if (parameterAdjuster != null)
                    parameterAdjuster(parameters);

                int result = ExecuteStoredNonQuery(procedureName, parameters);
                return result;
            }
        }

        /// <summary>
        /// ストアドプロシージャを実行する。Model内に共通のカラム(update_user等)に対応するプロパティがない・もしくはNULLの場合は、自動的に設定する。
        /// </summary>
        /// <param name="procedureName">呼び出すプロシージャ名</param>
        /// <param name="parameters">プロシージャに渡すSqlParameterのリスト</param>
        /// <returns>プロシージャ実行時の影響件数</returns>
        public static int ExecuteStoredNonQuery(string procedureName,
            IEnumerable<SqlParameter> parameters, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand sqlCommand =
                    new SqlCommand(procedureName, connection);

                sqlCommand.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter eachParameter in parameters)
                {
                    sqlCommand.Parameters.Add(eachParameter);
                }

                sqlCommand.CommandTimeout = 0;  //2012/08/22 for making t_sales

                int resultCount = sqlCommand.ExecuteNonQuery();
                return resultCount;
            }
        }

        /// <summary>
        /// Table変数のパラメータを取得する。
        /// </summary>
        /// <param name="table">DataTable変数</param>
        /// <param name="typeName">DataTableのタイプ名</param>
        /// <param name="parameterName">SqlParameter名</param>
        /// <returns></returns>
        public static SqlParameter CreateTableParameter(DataTable table, string typeName, string parameterName)
        {
            SqlParameter param = new SqlParameter(parameterName, table);
            param.SqlDbType = SqlDbType.Structured;
            param.TypeName = typeName;

            return param;
        }


        /// <summary>
        /// 引数で渡されたModelのリストからDataTableを作成する。
        /// </summary>
        /// <typeparam name="T">Modelのタイプ</typeparam>
        /// <param name="models">Modelのリスト</param>
        /// <returns>作成されたDataTable</returns>
        public static DataTable CreateDataTableFromProperty<T>(IEnumerable<T> models, string dataTableName = "")
        {
            Type modelType = typeof(T);
            PropertyInfo[] properties = GetProperties(modelType);
            DataTable dataTable = CreateDataTable(modelType, dataTableName);

            foreach (T model in models)
            {
                List<object> values = new List<object>();
                foreach (PropertyInfo pi in properties)
                {
                    if (IsDataTableMember(pi))
                    {
                        object propertyValue = pi.GetValue(model, null);
                        string propertyName = pi.Name.ToLower();

                        values.Add(propertyValue);
                    }
                }
                dataTable.Rows.Add(values.ToArray());
            }

            return dataTable;
        }



        /// <summary>
        /// 対象のtypeからプロパティの配列を取得する。
        /// </summary>
        /// <param name="entityType">対象のtype</param>
        /// <returns>対象のtypeのプロパティ配列</returns>
        private static PropertyInfo[] GetProperties(Type entityType)
        {
            return entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
        }

        /// <summary>
        /// 対象のタイプからDataTableを作成する。
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        private static DataTable CreateDataTable(Type entityType, string dataTableName = "")
        {
            PropertyInfo[] properties = GetProperties(entityType);

            DataTable dataTable = new DataTable(dataTableName);
            foreach (PropertyInfo pi in properties)
            {
                if (IsDataTableMember(pi))
                {
                    Type propertyType = GetGenericInnerType(pi) ?? pi.PropertyType;
                    dataTable.Columns.Add(pi.Name, propertyType);
                }
            }
            return dataTable;
        }

        /// <summary>
        /// ジェネリック型クラスの＜＞内のタイプを取得する。
        /// </summary>
        /// <param name="pi">プロパティ</param>
        /// <returns>ジェネリック型クラスの＜＞内のタイプ。ジェネリック型でない場合はNullが返る。</returns>
        private static Type GetGenericInnerType(PropertyInfo pi)
        {
            if (!pi.PropertyType.IsGenericType)
                return null;

            Type[] genericTypes = pi.PropertyType.GetGenericArguments();
            return genericTypes[0];
        }


        /// <summary>
        /// DataTableに変換するプロパティか判断する。
        /// </summary>
        /// <param name="pi">Modelのプロパティ</param>
        /// <returns>DataTableに変換するプロパティならtrue、そうでないならfalse</returns>
        private static bool IsDataTableMember(PropertyInfo pi)
        {
            var noTargetAttribute = Attribute.GetCustomAttribute(pi, typeof(NoDataTableMemberAttribute));
            bool isDataTableMember = (noTargetAttribute == null);
            return isDataTableMember;
        }

        /// <summary>
        /// 一意制約違反の例外かを判断する。
        /// </summary>
        /// <param name="ex">発生した例外</param>
        /// <returns>一意制約違反の例外ならtrue、そうでないならfalse</returns>
        public static bool IsUniqueConstException(Exception e)
        {
            if (e is SqlException ex)
            {
                //同期メソッド用
                if (UniqueErrorCodes.Contains(ex.Number))
                    return true;
                return false;
            }
            else
            {
                //非同期メソッド用
                if (e is AggregateException ae)
                {
                    foreach (var each in ae.InnerExceptions)
                    {
                        if (e is SqlException innerEx)
                        {
                            if (UniqueErrorCodes.Contains(innerEx.Number))
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 一意制約時のエラーコード 
        /// https://docs.microsoft.com/ja-jp/sql/relational-databases/errors-events/database-engine-events-and-errors?view=sql-server-ver15
        /// </summary>
        public static readonly IReadOnlyCollection<int> UniqueErrorCodes = new int[] { 2601, 2627 };
        
        /// <summary>デッドロック時のエラーコード </summary>
        public const int DeadLockErrorCode = 1205;
    }

    

    /// <summary>
    /// DataTableに変換する際に、変換対象外とするプロパティを表す属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NoDataTableMemberAttribute : Attribute
    {
    }

}