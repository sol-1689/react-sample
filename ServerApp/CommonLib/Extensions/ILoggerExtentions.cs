using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

/// <summary>
/// 記録内容種別
/// </summary>
public enum LogType
{
    [Description("リクエスト開始")]
    RequestStart,
    [Description("SQL発行")]
    Sql,
    [Description("リクエスト終了")]
    RequestEnd,
    [Description("パラメータエラー")]
    ParameterError,
    [Description("エラー")]
    Error,
    [Description("例外")]
    Exception,

    [Description("バッチ開始")]
    BatchStart,

    [Description("バッチ終了")]
    BatchEnd,

    [Description("バッチエラー")]
    BatchError,



    [Description("その他")]
    Other
}


public static class ILoggerExtentions
{

    public static void Info(this Microsoft.Extensions.Logging.ILogger logger, string message = "", LogType logType = LogType.Other, 
        object result = null, string errorMessage = "", Exception ex = null)
    {
        //var defaultEvent = new LogEventInfo(NLog.LogLevel.Info, "", message);

        //defaultEvent.Properties["logType"] = logType.GetDescription();
        //defaultEvent.Properties["result"] = result;
        //defaultEvent.Properties["errormessage"] = errorMessage;

        var logEvent = CreateLogEvent(message, logType, result, errorMessage);
        logger.Log(Microsoft.Extensions.Logging.LogLevel.Information,
              default, logEvent, ex, CustomLogEvent.Formatter);
    }


    public static void Warn(this Microsoft.Extensions.Logging.ILogger logger, string message = "", LogType logType = LogType.Other,
        object result = null, string errorMessage = "", Exception ex = null)
    {

        var logEvent = CreateLogEvent(message, logType, result, errorMessage);
        logger.Log(Microsoft.Extensions.Logging.LogLevel.Warning,
              default, logEvent, ex, CustomLogEvent.Formatter);
    }


    public static void Error(this Microsoft.Extensions.Logging.ILogger logger, string message = "", LogType logType = LogType.Error,
        object result = null, string errorMessage = "", Exception ex = null)
    {

        var logEvent = CreateLogEvent(message, logType, result, errorMessage);
        logger.Log(Microsoft.Extensions.Logging.LogLevel.Error,
              default, logEvent, ex, CustomLogEvent.Formatter);
    }

    public static void Trace(this Microsoft.Extensions.Logging.ILogger logger, string message = "", LogType logType = LogType.Other,
        object result = null, string errorMessage = "", Exception ex = null)
    {

        var logEvent = CreateLogEvent(message, logType, result, errorMessage);
        logger.Log(Microsoft.Extensions.Logging.LogLevel.Trace,
              default, logEvent, ex, CustomLogEvent.Formatter);
    }

    public static void Debug(this Microsoft.Extensions.Logging.ILogger logger, string message = "", LogType logType = LogType.Other,
        object result = null, string errorMessage = "", Exception ex = null)
    {

        var logEvent = CreateLogEvent(message, logType, result, errorMessage);
        logger.Log(Microsoft.Extensions.Logging.LogLevel.Debug,
              default, logEvent, ex, CustomLogEvent.Formatter);
    }


    static CustomLogEvent CreateLogEvent(string message, LogType logType, object result, string errorMessage)
    {
        CustomLogEvent logEvent = new CustomLogEvent(message)
            .AddProp("logType", logType.GetDescription())
            .AddProp("result", result)
            .AddProp("errormessage", errorMessage);

        return logEvent;
    }



}

class CustomLogEvent : IEnumerable<KeyValuePair<string, object>>
{
    List<KeyValuePair<string, object>> _properties = new List<KeyValuePair<string, object>>();

    public string Message { get; }

    public CustomLogEvent(string message)
    {
        Message = message;
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return _properties.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator(){ return GetEnumerator(); }

    public CustomLogEvent AddProp(string name, object value)
    {
        _properties.Add(new KeyValuePair<string, object>(name, value));
        return this;
    }


    public static Func<CustomLogEvent, Exception, string> Formatter { get; } = (l, e) => l.Message;
}