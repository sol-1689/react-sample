using Domain.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public static class StringExtensions
{
    static readonly Regex numericRegex = new Regex("^[0-9]+$");
    static readonly Regex regAtmark = new Regex(@"^[\@|\＠]");
    
    /// <summary>
    /// 文字列が数値のみかをチェックする。
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsNumeric(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return false;

        return numericRegex.IsMatch(str);

    }

    public static string AddSlashToDateString(this string str)
    {
        return CommonUtil.AddSlashToDateString(str);
    }

    /// <summary>
    /// byte配列に変換する。
    /// </summary>
    /// <param name="str"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static byte[] ToByte(this string str, Encoding encoding = null )
    {
        if (string.IsNullOrEmpty(str))
            return null;

        encoding = encoding ?? Encoding.UTF8;
        return encoding.GetBytes(str);
    }

    public static bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    //public static bool IsTrue(this string str)
    //{
    //    return str == CommonStrings.FLAG_TRUE;
    //}
}
