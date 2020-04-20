using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcApplication1
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute(
                name: "Solutions",
                url: "Solutions/{Id}",
                defaults: new { controller = "Solutions", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
               name: "Administration",
               url: "Admin/{PageName}",
               defaults: new { controller = "Administration", action = "Index", PageName = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "Login",
               url: "Account/{action}",
               defaults: new { controller = "Account" }
           );

           

          /*  routes.MapRoute(
                name: "Test",
                url: "Test",
                defaults: new { controller = "Home", action = "Test", PageName = UrlParameter.Optional }
            );*/

            routes.MapRoute(
                name: "Page",
                url: "{PageName}",
                defaults: new { controller = "Home", action = "Index", PageName = UrlParameter.Optional }
            );

            routes.MapRoute(
             name: "Default",
             url: "{controller}/{action}",
             defaults: new { controller = "Home", action="Index" }
         );

          
        }
    }
}
