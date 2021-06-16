using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class ModelStateDictionaryExtentions
{
    public static IEnumerable<KeyValuePair<string, ModelStateEntry>> GetAllErrorKv(this ModelStateDictionary modelState)
    {
        return modelState.Where(kv => kv.Value.Errors.Any());
    }

    public static IEnumerable<ModelError> GetAllError(this ModelStateDictionary modelState)
    {
        return modelState.Values.SelectMany(v => v.Errors);
    }

    public static IEnumerable<string> GetAllErrorMessges(this ModelStateDictionary modelState)
    {
        return GetAllError(modelState).Select(e => e.ErrorMessage);
    }

    /// <summary>
    /// ModelStateのエラーをRequestErrorInfoのリストとして取得する。
    /// </summary>
    /// <param name="modelState"></param>
    /// <returns></returns>
    public static List<RequestErrorInfo> GetAllAsErrorInfo(this ModelStateDictionary modelState)
    {
        return GetAllErrorKv(modelState).SelectMany(kv => kv.Value.Errors.Select(e => new RequestErrorInfo(e.ErrorMessage, kv.Key))).ToList();
    }

    public static string GetAllErrorMessge(this ModelStateDictionary modelState, string separator = ",")
    {
        return string.Join(separator, GetAllErrorMessges(modelState));
    }
}