using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GDYCMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "MainPageEditor",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Admin", action = "CentralPageEditor", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "TestRoute",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Admin", action = "Test", id = UrlParameter.Optional }
            );

        }
    }
}