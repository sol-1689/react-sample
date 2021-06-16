using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public static class ExceptionExtension
{
    public static string GetMessages(this Exception ex, string splitText = null)
    {
        if (ex is AggregateException ae)
        {
            return GetMessages(ae, splitText);
        }
        else
            return ex.ToString();
    }

    public static string GetMessages(this AggregateException ae, string splitText = null)
    {
        var messages = ae?.InnerExceptions.Select(a => a.ToString());

        if (messages.IsNullOrEmpty())
            return "";
        else
        {
            splitText = splitText ?? Environment.NewLine;
            return string.Join(splitText, messages);
        }
    }
}