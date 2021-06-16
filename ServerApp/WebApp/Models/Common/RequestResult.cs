using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WebApp
{
    /// <summary>
    /// リクエスト通信の戻り値を表すクラス
    /// </summary>
    public class RequestResult
    {
        #region Createメソッド

        /// <summary>
        /// 処理成功時の戻り値を作成する。
        /// </summary>
        /// <returns>処理成功時の戻り値</returns>
        internal static RequestResult CreateSuccessResult()
        {
            RequestResult result = new RequestResult();
            return result;
        }

        /// <summary>
        /// JavaScript側に返すオブジェクトを保持した、処理成功時の戻り値を作成する。
        /// </summary>
        /// <typeparam name="T">JavaScript側に返すオブジェクトのType</typeparam>
        /// <param name="resultValue">JavaScript側に返すオブジェクト</param>
        /// <returns>処理成功時の戻り値</returns>
        internal static RequestResult CreateSuccessResult(object resultValue)
        {
            RequestResult result = new RequestResult();
            result.Result = resultValue;

            return result;
        }

        /// <summary>
        /// リダイレクトしたい際の戻り値
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static RequestResult CreateRedirectResult(string url)
        {
            RequestResult result = new RequestResult();
            result.RedirectUrl = url;
            return result;
        }

        /// <summary>
        /// 認証に失敗した際の戻り値を作成する。
        /// </summary>
        /// <param name="errorMessages">エラーメッセージ</param>
        /// <returns>処理失敗時の戻り値</returns>
        public static RequestResult CreateAuthFailedResult(string redirectUrl, params string[] errorMessages)
        {
            var result = RequestResult.CreateErrorResult(errorMessages);
            result.IsAuthenticationFailed = true;

            result.RedirectUrl = redirectUrl;

            return result;
        }

        /// <summary>
        /// エラーメッセージを保持した、処理失敗時の戻り値を作成する。
        /// </summary>
        /// <param name="errorMessages">エラーメッセージ</param>
        /// <returns>処理失敗時の戻り値</returns>
        internal static RequestResult CreateErrorResult(params string[] errorMessages)
        {
            RequestResult result = new RequestResult();
            result.ErrorMessages = errorMessages?.Select(m => new RequestErrorInfo(m)).ToList();

            return result;
        }

        internal static RequestResult CreateErrorResult(ModelStateDictionary modelState)
        {
            RequestResult result = new RequestResult();
            result.ErrorMessages = modelState.GetAllAsErrorInfo();

            return result;
        }


        /// <summary>
        /// エラーメッセージを保持した、処理失敗時の戻り値を作成する。
        /// </summary>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>処理失敗時の戻り値</returns>
        internal static RequestResult CreateErrorResult(IEnumerable<string> errorMessages)
        {
            RequestResult result = new RequestResult();
            result.ErrorMessages = errorMessages?.Select(m => new RequestErrorInfo(m)).ToList();

            return result;
        }

        /// <summary>
        /// エラーメッセージを保持した、処理失敗時の戻り値を作成する。
        /// </summary>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>処理失敗時の戻り値</returns>
        internal static RequestResult CreateErrorResult(Exception ex)
        {
            RequestResult result = new RequestResult();
            result.ErrorMessages = new List<RequestErrorInfo>(){new RequestErrorInfo(ex.Message)};
            result.IsException = true;
            return result;
        }


        /// <summary>
        /// エラーメッセージ付きでリダイレクトしたい際の戻り値
        /// </summary>
        /// <param name="url"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static RequestResult CreateErrorRedirectResult(string url, IEnumerable<string> messages)
        {
            RequestResult result = new RequestResult();
            result.ErrorMessages = messages?.Select(m => new RequestErrorInfo(m)).ToList();
            result.RedirectUrl = url;
            return result;
        }

        /// <summary>
        /// セッションがタイムアウトした際の戻り値
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        internal static RequestResult CreateSessionTimeOutResult(params string[] messages)
        {
            RequestResult result = new RequestResult();
            result.ErrorMessages = messages?.Select(m => new RequestErrorInfo(m)).ToList();
            result.IsSessionTimeOut = true;
            return result;
        }

        /// <summary>
        /// セッションがタイムアウトした際の戻り値
        /// </summary>
        /// <param name="url"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        internal static RequestResult CreateSessionTimeOutResult(string url, IEnumerable<string> messages)
        {
            RequestResult result = new RequestResult();
            result.ErrorMessages = messages?.Select(m => new RequestErrorInfo(m)).ToList();
            result.IsSessionTimeOut = true;
            result.UrlWhenSessionTimeOut = url;
            return result;
        }


        #endregion


        #region プロパティ

        /// <summary>
        /// JavaScript側に返すオブジェクトを取得・設定する
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// 処理が成功したかを取得・設定する
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                return ErrorMessages.IsNullOrEmpty();
            }
        }

        /// <summary>
        /// リダイレクトのの遷移先を取得・設定する。
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// 例外が発生したかを取得・設定する
        /// </summary>
        public bool IsException { get; set; }

        /// <summary>
        /// セッションがタイムアウトしたかを取得・設定する。
        /// </summary>
        public bool IsSessionTimeOut { get; set; }

        /// <summary>
        /// 認証処理に失敗したかを取得・設定する。
        /// </summary>
        public bool IsAuthenticationFailed { get; set; }

        /// <summary>
        /// セッションがタイムアウトした際の遷移先を取得・設定する。
        /// </summary>
        public string UrlWhenSessionTimeOut { get; set; }

        /// <summary>
        /// エラーメッセージを取得・設定する。
        /// </summary>
        public List<RequestErrorInfo> ErrorMessages { get; set; }

        public string AllErrorMessage { get
        {
            return ErrorMessages.IsNullOrEmpty() ? "":
                string.Join(Environment.NewLine, ErrorMessages.Select(e => e.Message));
        }}

        #endregion



    }
}

public class RequestErrorInfo
{
    public RequestErrorInfo() { }

    public RequestErrorInfo(string message, string code = "") 
    {
        Message = message;
        Code = code;
    }

    /// <summary>エラーコード。任意。 </summary>
    public string Code { get; set; }
    public string Message { get; set; }
}
