using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FlexiLearn_JeffreyOdogba
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Members", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Login",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Members", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Delete",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Members", action = "Delete", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "CoursesRequest",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Members", action = "CoursesRequest", id = UrlParameter.Optional }
            );
        }
    }
}
