using Microsoft.Extensions.Logging;
using StackExchange.Profiling.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Domain.Util
{

    public class CustomProfiledDbConnection : ProfiledDbConnection
    {
        ICustomDbProfiler _profiler;

        public int? CommandTimeout { get; set; }

        /// <summary>ログへのSQL出力を一時的に無効にするか。 </summary>
        public bool TempDisableLogging { get; set; }

        public CustomProfiledDbConnection(DbConnection connection, ICustomDbProfiler profiler)
            : base(connection, profiler)
        {
            _profiler = profiler;
        }

        protected override DbCommand CreateDbCommand()
        {
            var profiler = TempDisableLogging ? null : _profiler;
            var command = new CustomProfiledDbCommand(WrappedConnection.CreateCommand(), this, profiler);
            TempDisableLogging = false; //一時的に無効にするだけなのでfalseに戻す。

            if (CommandTimeout != null)
                command.CommandTimeout = CommandTimeout.Value;

            return command;
        }
    }

    /// <summary>
    /// ProfiledDbCommand実行時にExecuteNonQueryFinishを呼ぶように変更。
    /// </summary>
    public class CustomProfiledDbCommand : ProfiledDbCommand
    {
        ICustomDbProfiler _profiler;
        DbCommand _command;

        public CustomProfiledDbCommand(DbCommand command, DbConnection connection, ICustomDbProfiler profiler)
            : base(command, connection, profiler)
        {
            _profiler = profiler;
            _command = command;
        }



        /// <summary>
        /// Executes a database data reader.
        /// </summary>
        /// <param name="behavior">The command behavior to use.</param>
        /// <returns>The resulting <see cref="DbDataReader"/>.</returns>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            if (_profiler == null || !_profiler.IsActive)
            {
                return _command.ExecuteReader(behavior);
            }

            DbDataReader result = null;
            _profiler.ExecuteStart(this, SqlExecuteType.Reader);
            try
            {
                result = _command.ExecuteReader(behavior);
                result = base.CreateDbDataReader(result, behavior, _profiler);
            }
            catch (Exception e)
            {
                _profiler.OnError(this, SqlExecuteType.Reader, e);
                throw;
            }
            finally
            {
                _profiler.ExecuteFinish(this, SqlExecuteType.Reader, result);
            }

            return result;
        }

        /// <summary>
        /// Executes a database data reader asynchronously.
        /// </summary>
        /// <param name="behavior">The command behavior to use.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for this async operation.</param>
        /// <returns>The resulting <see cref="DbDataReader"/>.</returns>
        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            if (_profiler == null || !_profiler.IsActive)
            {
                return await _command.ExecuteReaderAsync(behavior, cancellationToken).ConfigureAwait(false);
            }

            DbDataReader result = null;
            _profiler.ExecuteStart(this, SqlExecuteType.Reader);
            try
            {
                result = await _command.ExecuteReaderAsync(behavior, cancellationToken).ConfigureAwait(false);
                result = CreateDbDataReader(result, behavior, _profiler);
            }
            catch (Exception e)
            {
                _profiler.OnError(this, SqlExecuteType.Reader, e);
                throw;
            }
            finally
            {
                _profiler.ExecuteFinish(this, SqlExecuteType.Reader, result);
            }

            return result;
        }




        /// <summary>
        /// Executes a SQL statement against a connection object.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        public override int ExecuteNonQuery()
        {
            if (_profiler == null || !_profiler.IsActive)
            {
                return _command.ExecuteNonQuery();
            }

            int result = 0;
            _profiler.ExecuteStart(this, SqlExecuteType.NonQuery);
            try
            {
                result = _command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                _profiler.OnError(this, SqlExecuteType.NonQuery, e);
                throw;
            }
            finally
            {
                _profiler.ExecuteNonQueryFinish(this, SqlExecuteType.NonQuery, result);
            }

            return result;
        }

        /// <summary>
        /// Asynchronously executes a SQL statement against a connection object asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for this async operation.</param>
        /// <returns>The number of rows affected.</returns>
        public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            if (_profiler == null || !_profiler.IsActive)
            {
                return await _command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }

            int result = 0;
            _profiler.ExecuteStart(this, SqlExecuteType.NonQuery);
            try
            {
                result = await _command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _profiler.OnError(this, SqlExecuteType.NonQuery, e);
                throw;
            }
            finally
            {
                _profiler.ExecuteNonQueryFinish(this, SqlExecuteType.NonQuery, result);
            }

            return result;
        }


        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. 
        /// Additional columns or rows are ignored.
        /// </summary>
        /// <returns>The first column of the first row in the result set.</returns>
        public override object ExecuteScalar()
        {
            if (_profiler == null || !_profiler.IsActive)
            {
                return _command.ExecuteScalar();
            }

            object result;
            _profiler.ExecuteStart(this, SqlExecuteType.Scalar);
            try
            {
                result = _command.ExecuteScalar();
            }
            catch (Exception e)
            {
                _profiler.OnError(this, SqlExecuteType.Scalar, e);
                throw;
            }
            finally
            {
                _profiler.ExecuteFinish(this, SqlExecuteType.Scalar, null);
            }

            return result;
        }

        /// <summary>
        /// Asynchronously executes the query, and returns the first column of the first row in the result set returned by the query. 
        /// Additional columns or rows are ignored.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for this async operation.</param>
        /// <returns>The first column of the first row in the result set.</returns>
        public override async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            if (_profiler == null || !_profiler.IsActive)
            {
                return await _command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
            }

            object result;
            _profiler.ExecuteStart(this, SqlExecuteType.Scalar);
            try
            {
                result = await _command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _profiler.OnError(this, SqlExecuteType.Scalar, e);
                throw;
            }
            finally
            {
                _profiler.ExecuteFinish(this, SqlExecuteType.Scalar, null);
            }

            return result;
        }


    }




    public interface ICustomDbProfiler : IDbProfiler
    {
        void ExecuteNonQueryFinish(System.Data.IDbCommand profiledDbCommand, SqlExecuteType executeType, int result);
    }

    /// <summary>
    /// 発行されたSQLをログに出力する為の処理を保持したクラス。
    /// </summary>
    public class TraceDbProfiler : ICustomDbProfiler
    {
        StringBuilder SbForDBLog;

        private readonly ILogger<TraceDbProfiler> logger;

        public TraceDbProfiler(ILogger<TraceDbProfiler> _logger)
        {
            SbForDBLog = new StringBuilder();
            logger = _logger;

        }

        /// <summary>SQL文の改行文字。Repositoryのソースファイルの改行文字と合わせる必要がある。 </summary>
        const string NewLine = "\r\n";


        Stopwatch stopwatch;
        string commandText;
        string parametersText;

        /// <summary>
        /// コマンドが開始された時に呼ばれる(ExecuteReaderとかExecuteNonQueryとか)
        /// </summary>
        /// <param name="profiledDbCommand"></param>
        /// <param name="executeType"></param>
        public void ExecuteStart(System.Data.IDbCommand profiledDbCommand, SqlExecuteType executeType)
        {
            stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Select系のログはここ。
        /// </summary>
        /// <param name="profiledDbCommand"></param>
        /// <param name="executeType"></param>
        /// <param name="reader"></param>
        public void ExecuteFinish(IDbCommand profiledDbCommand, SqlExecuteType executeType, DbDataReader reader)
        {
            commandText = profiledDbCommand.CommandText; //SQL文
            SbForDBLog.Clear();

            parametersText = CreateParametersText(profiledDbCommand.Parameters, SbForDBLog);

            //いまのところSelectはログに出さない。
            if (executeType != SqlExecuteType.Reader)
            {
                stopwatch.Stop();
                SbForDBLog.Clear();

                SetCommonLogString(SbForDBLog, executeType).Replace(NewLine, " ");
                string log = SbForDBLog.ToString();

                //logger.LogInformation(log);
                logger.Info(log, LogType.Sql);
            }
        }

        /// <summary>
        /// コマンドが完了された時に呼ばれる。
        /// </summary>
        /// <param name="profiledDbCommand"></param>
        /// <param name="executeType"></param>
        /// <param name="result"></param>
        public void ExecuteNonQueryFinish(System.Data.IDbCommand profiledDbCommand, SqlExecuteType executeType, int result)
        {
            // IDbProfilerにはないためここには来ない

            if (executeType != SqlExecuteType.NonQuery)
                return;

            commandText = profiledDbCommand.CommandText; //SQL文
            SbForDBLog.Clear();

            parametersText = CreateParametersText(profiledDbCommand.Parameters, SbForDBLog);

            stopwatch.Stop();
            SbForDBLog.Clear();

            SetCommonLogString(SbForDBLog, executeType).Append("【影響行数】").Append(result.ToString())
                .Replace(NewLine, " ");

            string log = SbForDBLog.ToString();

            logger.Info(log, LogType.Sql, result: result);
        }


        /// <summary>
        /// Readerが完了した時に呼ばれる。(Select文用のログ) 
        /// </summary>
        /// <param name="reader"></param>
        public void ReaderFinish(System.Data.IDataReader reader)
        {
            stopwatch.Stop();
            SbForDBLog.Clear();

            SetCommonLogString(SbForDBLog, SqlExecuteType.Reader).Replace(NewLine, " ");
            string log = SbForDBLog.ToString();

            //logger.LogInformation(log);
            //AuditLogHelper.ExecutedSqlLog(log);
            logger.Info(log, LogType.Sql);
        }

        public bool IsActive
        {
            get { return true; }
        }

        public void OnError(System.Data.IDbCommand profiledDbCommand, SqlExecuteType executeType, System.Exception exception)
        {
            SbForDBLog.Clear();

            SbForDBLog.Append("エラー発生SQL  ");
            SetCommonLogString(SbForDBLog, executeType)
                .AppendLine()
                .AppendLine(exception.ToString())
                .Replace(NewLine, " ");

            string log = SbForDBLog.ToString();
            logger.LogError(log);
        }

        /// <summary>
        /// ログ出力用のパラメータ情報文字列を作成する。
        /// </summary>
        /// <param name="each"></param>
        /// <param name="sb"></param>
        /// <returns></returns>
        private string CreateParametersText(IDataParameterCollection parameters, StringBuilder sb)
        {
            //foreach(IDataParameter each in parameters)
            //    sb.Append("{ ").Append(each.ParameterName).Append(" = [").Append(each.Value.ToString()).Append("]  DBType=[")
            //        .Append(each.DbType).Append("] }");

            sb.Append("{ ");
            foreach (IDataParameter each in parameters)
                sb.Append(each.ParameterName).Append("=[").Append(each.Value.ToString()).Append("] ");

            sb.Append("}");

            return sb.ToString(); //パラメータ
        }

        /// <summary>
        /// ログに書き出す際の共通フォーマットを出力する。
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        StringBuilder SetCommonLogString(StringBuilder sb, SqlExecuteType sqlType)
        {
            sb.Append("実行SQLログ【CommandType】").Append(sqlType).Append("【SQL】").Append(commandText)
                    .Append("【パラメータ】").Append(parametersText).Append("【実行時間(ミリ秒)】").Append(stopwatch.ElapsedMilliseconds);

            return sb;
        }
    }

}