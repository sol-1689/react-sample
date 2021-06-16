using System;
using System.Collections.Generic;
using System.Text;
using Domain.Util;

public static class ByteExtentions
{

    public static string ToBase64String(this byte[] value)
    {
        if (value.IsNullOrEmpty())
            return "";

        return Convert.ToBase64String(value);
    }
}