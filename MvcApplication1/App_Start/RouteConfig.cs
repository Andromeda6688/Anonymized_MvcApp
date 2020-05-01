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
               name: "Administration",
               url: "Admin/{action}/{Id}",
               defaults: new { controller = "Administration", action = "PageList", Id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "Account",
               url: "Account/{action}/{Id}",
               defaults: new { controller = "Account", action = "UserList", Id = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "level_1",
                url: "{PageName}",
                defaults: new { controller = "Home", action = "Index", PageName = UrlParameter.Optional }
            );
            
            routes.MapRoute(
                name: "level_2",
                url: "{ParentName}/{PageName}",
                defaults: new { controller = "Home", action = "Index", ParentName = UrlParameter.Optional }
            );

            routes.MapRoute(
             name: "Default",
             url: "{controller}/{action}/{param}",
             defaults: new { controller = "Home", action = "Index", param = UrlParameter.Optional }
         );

          
        }
    }
}
