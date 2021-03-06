﻿using System;
using System.Collections.Generic;
using System.Linq;

using System.Web.Mvc;
using System.Web.Routing;

namespace TuSalud
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "LoadBTree",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Pharmacy", action = "LoadBTree", id = UrlParameter.Optional }
            );
        }
    }
}
