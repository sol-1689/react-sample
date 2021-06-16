
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;



/// <summary>
/// サーバとの通信用処理を保持したクラス。
/// </summary>
public class WebConnectService : IDisposable
{
    //クッキー用設定。
    HttpClient _client;

    /// <summary>
    /// HttpClient
    /// </summary>
    public HttpClient Client {
        get
        {
            if (_client == null)
                _client = CreateHttpClient();

            return _client;
        }
    }

    /// <summary>HttpClientHandler </summary>
    public HttpClientHandler HttpHandler { get; set; }

    /// <summary>クッキー用 </summary>
    public Func<CookieContainer> CookieGetter;
    public CookieContainer CurrentCookieContainer { get { return CookieGetter(); } }

    /// <summary> クライアント側のリクエストのタイムアウト時間。</summary>
    public TimeSpan RequestTimeout { get; set; } = new TimeSpan(0, 2, 0);

    ILogger<WebConnectService> logger;


    public WebConnectService(ILogger<WebConnectService> logger)
    {
        this.logger = logger;
    }


    /// <summary>
    /// WebConnectServiceのインスタンスを作成する。
    /// </summary>
    HttpClient CreateHttpClient()
    {
        //認証処理について、ログイン成功時に得たクッキーをstaticな変数に保持しておき、
        //リクエスト毎に添付して送る形にしようかと思っております。
        //（＝ブラウザと同じ様な挙動にする）
        //なので認証処理自体はサーバー側におまかせになる。

        HttpClientHandler handler = HttpHandler;

        if (handler == null)
        {
            handler = new HttpClientHandler();
            //gzip用。これつけるとResponseヘッダーのContent-EncodingとContent-Lengthがログに出ない。HttpClient内で勝手に消される仕様とのこと。
            handler.AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate);

            //https接続時に自己証明書を許可。
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };
        }

        HttpClient client = new HttpClient(handler);
        client.Timeout = RequestTimeout;
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate"); //gzip圧縮可。

        return client;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void AddHeaderIfNotExists(string name, string value)
    {
        if(!Client.DefaultRequestHeaders.Contains(name))
            Client.DefaultRequestHeaders.TryAddWithoutValidation(name, value); //gzip圧縮可。
    }


    #region Jsonで返ってくる場合


    /// <summary>
    /// サーバーへリクエストを送り、結果を取得する。
    /// RequestResult以外の型が返ってくることがあれば使えるように一応作っとく。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serverUri"></param>
    /// <param name="postParm"></param>
    /// <returns></returns>
    public async Task<T> SendRequest<T>(Uri serverUri)
    {
        var jsonString = await GetJsonString(serverUri, (object)null);
        return ConvertJsonTo<T>(jsonString);
    }

    /// <summary>
    /// サーバーへリクエストを送り、結果を取得する。
    /// RequestResult以外の型が返ってくることがあれば使えるように一応作っとく。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serverUri"></param>
    /// <param name="postParm"></param>
    /// <returns></returns>
    public async Task<T> SendRequest<T>(Uri serverUri, object postParm)
    {
        var jsonString = await GetJsonString(serverUri, postParm);
        return ConvertJsonTo<T>(jsonString);
    }

    /// <summary>
    /// サーバーへリクエストを送り、結果を取得する。
    /// RequestResult以外の型が返ってくることがあれば使えるように一応作っとく。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serverUri"></param>
    /// <param name="postParm"></param>
    /// <returns></returns>
    public async Task<T> SendRequest<T>(Uri serverUri, string postBody)
    {
        var jsonString = await GetJsonString(serverUri, postBody);
        return ConvertJsonTo<T>(jsonString);
    }

    /// <summary>
    /// サーバーへリクエストを送り、結果を取得する。
    /// RequestResult以外の型が返ってくることがあれば使えるように一応作っとく。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serverUri"></param>
    /// <param name="postParm"></param>
    /// <returns></returns>
    public async Task<T> SendRequest<T>(Uri serverUri, Dictionary<string, string> postParams)
    {
        var jsonString = await GetJsonString(serverUri, postParams);
        return ConvertJsonTo<T>(jsonString);
    }



    /// <summary>
    /// サーバーへリクエストを送り、結果をJson文字列として取得する。
    /// 直には呼ばないかも。
    /// </summary>
    /// <param name="serverUri"></param>
    /// <param name="postParm"></param>
    /// <returns></returns>
    public async Task<string> GetJsonString(Uri serverUri, object postParm = null)
    {
        HttpResponseMessage responce = await GetResponse(serverUri, postParm);

        if (responce != null)
            return await responce.Content.ReadAsStringAsync();

        return null;
    }

    /// <summary>
    /// サーバーへリクエストを送り、結果をJson文字列として取得する。
    /// 直には呼ばないかも。
    /// </summary>
    /// <param name="serverUri"></param>
    /// <param name="postParm"></param>
    /// <returns></returns>
    public async Task<string> GetJsonString(Uri serverUri, string bodyJson)
    {
        HttpResponseMessage responce = await GetResponse(serverUri,  bodyJson);

        if (responce != null)
            return await responce.Content.ReadAsStringAsync();

        return null;
    }

    /// <summary>
    /// サーバーへリクエストを送り、結果をJson文字列として取得する。
    /// 直には呼ばないかも。
    /// </summary>
    /// <param name="serverUri"></param>
    /// <param name="postParm"></param>
    /// <returns></returns>
    public async Task<string> GetJsonString(Uri serverUri, Dictionary<string,string> postParams)
    {     
        HttpResponseMessage responce = await GetResponse(serverUri, postParams);

        if (responce != null)
            return await responce.Content.ReadAsStringAsync();

        return null;
    }

    #endregion


    #region byteで返ってくる場合

    ///// <summary>
    ///// サーバーへリクエストを送り、結果を取得する。
    ///// RequestResult以外の型が返ってくることがあれば使えるように一応作っとく。
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="serverUri"></param>
    ///// <param name="postParm"></param>
    ///// <returns></returns>
    //public async Task<byte[]> SendRequestByteAsync(Uri serverUri, object postParm = null)
    //{
    //    return await GetByteAsync(serverUri, postParm);
    //}

    /// <summary>
    /// サーバーへリクエストを送り、結果をbyteとして取得する。
    /// 直には呼ばないかも。
    /// </summary>
    /// <param name="serverUri"></param>
    /// <param name="postParm"></param>
    /// <returns></returns>
    public async Task<byte[]> GetByte(Uri serverUri, object postParm = null)
    {
        HttpResponseMessage responce = await GetResponse(serverUri, postParm);

        if (responce != null)
            return await responce.Content.ReadAsByteArrayAsync();

        return null;
    }

    #endregion





    /// <summary>
    /// サーバーへリクエストを送り、結果をHttpResponseMessageとして取得する。
    /// 直には呼ばないかも。
    /// </summary>
    /// <param name="serverUri"></param>
    /// <param name="postParm"></param>
    /// <returns></returns>
    async Task<HttpResponseMessage> GetResponse(Uri serverUri, object postParm = null)
    {
        string jsonObj = JsonConvert.SerializeObject(postParm);
        HttpContent content = new StringContent(jsonObj, System.Text.Encoding.UTF8, "application/json");
        var responce = await Post(serverUri, content);
        return responce;
    }

    /// <summary>
    /// サーバーへリクエストを送り、結果をHttpResponseMessageとして取得する。
    /// 直には呼ばないかも。
    /// </summary>
    /// <param name="serverUri"></param>
    /// <param name="postParm"></param>
    /// <returns></returns>
    async Task<HttpResponseMessage> GetResponse(Uri serverUri, string bodyJson)
    {
        HttpContent content = new StringContent(bodyJson ?? "", System.Text.Encoding.UTF8, "application/json");
        var responce = await Post(serverUri, content);
        return responce;
    }

    /// <summary>
    /// サーバーへリクエストを送り、結果をHttpResponseMessageとして取得する。
    /// 直には呼ばないかも。
    /// </summary>
    /// <param name="serverUri"></param>
    /// <param name="postParm"></param>
    /// <returns></returns>
    async Task<HttpResponseMessage> GetResponse(Uri serverUri, Dictionary<string, string> postParams)
    {
        HttpContent encodedContent = new FormUrlEncodedContent(postParams);
        var responce = await Post(serverUri, encodedContent);
        return responce;
    }

    async Task<HttpResponseMessage> Post(Uri serverUri, HttpContent content)
    {
        logger.LogInformation($"Http通信開始【{serverUri}】");
        HttpResponseMessage response = null;

        try
        {
            response = await Client.PostAsync(serverUri, content); //ここでレスポンス設定
            logger.LogInformation($"Http通信終了【{serverUri}】【{response.StatusCode}】");
            
        }
        catch (AggregateException ex)
        {
            logger.LogError(ex, $"Http通信でエラー【{serverUri}】【{ex.GetMessages()}】");
        }
        return response;
    }


    /// <summary>
    /// Jsonオブジェクトを指定した型に変換する。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    public T ConvertJsonTo<T>(string jsonString)
    {
        logger.LogInformation($"Http結果【{jsonString}】");
        return JsonConvert.DeserializeObject<T>(jsonString);
    }


    public void Dispose()
    {
        Client?.Dispose();
    }
}


///// <summary>
///// サーバーへリクエストを送り、結果をHttpResponseMessageとして取得する。
///// 直には呼ばないかも。
///// </summary>
///// <param name="serverUri"></param>
///// <param name="postParm"></param>
///// <returns></returns>
//public bool GetResponseWithCheck(Uri serverUri, ref HttpResponseMessage responce,
//    object postParm = null)
//{

//    try
//    {
//        responce = GetResponseAsync(serverUri, postParm); //ここでレスポンス設定
//    }
//    catch (AggregateException ex)
//    {
//        //サーバー立ち上がってなくて、404すら返ってこない場合もここにくる。
//        var innerException = ex.InnerException;
//        if (innerException is HttpRequestException)
//        {
//            throw;
//        }
//        else
//        {
//            throw;
//        }
//    }

//    return true;
//}

///// <summary>
///// Json→RequestResult変換時の処理保持したクラス。
///// </summary>
//public class RequestResultSerializer : JsonConverter
//{
//    /// <summary>
//    /// Json→RequestResult変換時の処理を少し拡張。
//    /// </summary>
//    /// <param name="reader"></param>
//    /// <param name="objectType"></param>
//    /// <param name="existingValue"></param>
//    /// <param name="serializer"></param>
//    /// <returns></returns>
//    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//    {
//        //まずデフォルトの変換を行う。
//        JObject jsonObject = JObject.Load(reader);
//        RequestResult result = jsonObject.ToObject(objectType) as RequestResult;

//        //ResultValueのJson文字列はすぐにデシリアライズせず、文字列のまま保持しておく。
//        //RequestResultのResultValueAsメソッドが呼ばれたタイミングでデシリアライズする。

//        //サーバー側:ResultValue →　クライアント側:ResultValueJson　への付け替え処理。
//        JProperty prop = jsonObject.Properties().Where(m => m.Name == "ResultValue").FirstOrDefault(); 
//        result.ResultValueJson = (prop == null ? "" : prop.Value.ToString());

//        return result;
//    }

//    public override bool CanConvert(Type objectType)
//    {
//        return typeof(RequestResult).IsAssignableFrom(objectType); //RequestResultに対してのConverter。
//    }

//    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//    {
//        return; //クライアントではJsonに書き込まないと思うので何もしない。
//    }
//}
