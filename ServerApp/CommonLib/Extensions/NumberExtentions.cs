using System;
using System.Collections.Generic;
using System.Linq;


public static class NumberExtentions
{

    /// <summary>
    /// 通貨形式の文字列に変換する。(\1,000など)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToCurrencyStr(this decimal value)
    {
        return value.ToString("#,0");
        //return value.ToString("C");
    }

    /// <summary>
    /// 通貨形式の文字列に変換する。(1,000など)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToCurrencyStr(this decimal? value)
    {
        if (value == null)
            return "";

        return value.Value.ToCurrencyStr();
    }


    /// <summary>
    /// 通貨形式の文字列に変換する。(1,000.00など)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToCurrencyPointString(this decimal value)
    {
        return value.ToString("#,#0.00");
    }

    /// <summary>
    /// 通貨形式の文字列に変換する。(1,000.00など)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToCurrencyPointString(this decimal? value)
    {
        if (value == null)
            return "";

        return value.Value.ToCurrencyPointString();
    }


    /// <summary>
    /// 通貨形式の文字列に変換する。(1,000など)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToCurrencyStr(this int value)
    {
        return value.ToString("#,0");
    }

    /// <summary>
    /// 通貨形式の文字列に変換する。(1,000など)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToCurrencyStr(this int? value)
    {
        if (value == null)
            return "";

        return value.Value.ToCurrencyStr();
    }

    /// <summary>
    /// 秒数からTimespanを作成する。
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TimeSpan? ToTimeSpanFromSeconds(this decimal? value)
    {
        if (value == null)
            return null;

        return value.Value.ToTimeSpanFromSeconds();
    }


    /// <summary>
    /// 秒数からTimespanを作成する。
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TimeSpan ToTimeSpanFromSeconds(this decimal value)
    {
        //int num = (int)Math.Truncate(value.Value);

        var result = TimeSpan.FromSeconds((double)value);
        return result;
    }


    /// <summary>
    /// 秒数からTimespanを作成する。
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TimeSpan? ToTimeSpanFromSeconds(this int? value)
    {
        if (value == null)
            return null;

        return value.Value.ToTimeSpanFromSeconds();
    }


    /// <summary>
    /// 秒数からTimespanを作成する。
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TimeSpan ToTimeSpanFromSeconds(this int value)
    {
        //int num = (int)Math.Truncate(value.Value);

        var result = TimeSpan.FromSeconds((double)value);
        return result;
    }

}
