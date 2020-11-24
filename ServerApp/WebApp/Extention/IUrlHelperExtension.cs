using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class IUrlHelperExtension
{


    public static string GetAreaName(this ActionContext context)
    {
        return context.RouteData.GetAreaName();
    }

    public static string GetAreaName(this RouteData routeData)
    {
        var values = routeData.Values;

        return values.TryGetValue("area", out var area)
            ? area.ToString()
            : null;

        // RouteData.Values["area"]
    }


    public static bool IsApiRoute(this RouteData routeData)
    {
        return routeData.GetAreaName() == "api";

        // RouteData.Values["area"]
    }
}