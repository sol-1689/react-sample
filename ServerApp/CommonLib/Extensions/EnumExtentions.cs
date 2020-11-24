using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

/// <summary>
/// Enumの拡張メソッド
/// </summary>
public static class EnumExtentions
{
    /// <summary>
    /// [Description]属性の値を取得する。
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string GetDescription(this Enum source)
    {
        if (source == null)
            return "";
    
        FieldInfo fi = source.GetType().GetField(source.ToString());
        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

        string result = (attributes.IsNullOrEmpty()) ? source.ToString() : attributes[0].Description;
        return result;
    }

}