using System;
using System.Collections.Generic;
using System.Text;
using Domain.Util;

public static class BoolExtensions
{
    public static int AsFlag(this bool value)
    {
       return value ? 1 : 0;
    }

    public static bool AsFlag(this int? value)
    {
       return value == null ? false : AsFlag(value.Value);
    }

    public static bool AsFlag(this int value)
    {
       return value == 1 ? true : false;
    }


}