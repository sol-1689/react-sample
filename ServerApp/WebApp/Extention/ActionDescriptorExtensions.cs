using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class ActionDescriptorExtensions
{
    public static ControllerActionDescriptor AsControllerActionDescriptor(this ActionDescriptor actionDescriptor)
    {
        var result = (actionDescriptor as ControllerActionDescriptor);
        return result;
    }

    public static bool HasAttribute<T>(this ActionDescriptor d)
        where T : Attribute
    {
        return d.AsControllerActionDescriptor().HasAttribute<T>();
    }

    public static bool HasAttribute<T>(this ControllerActionDescriptor d) 
        where T : Attribute
    {
        bool result = d.ControllerTypeInfo.GetCustomAttributes(typeof(T), true).Any()
                || d.MethodInfo.GetCustomAttributes(typeof(T), true).Any();

        return result;
    }

    public static T GetAttribute<T>(this ActionDescriptor d)
    where T : Attribute
    {

        return d.AsControllerActionDescriptor().GetAttribute<T>();
    }

    public static T GetAttribute<T>(this ControllerActionDescriptor d)
        where T : Attribute
    {

        var result = d.ControllerTypeInfo.GetCustomAttributes(typeof(T), true).FirstOrDefault();
        if (result == null)
            result = d.MethodInfo.GetCustomAttributes(typeof(T), true).FirstOrDefault();

        return result as T;
    }

}