using System.Web.Mvc;
using System.Web.Routing;

namespace Annapolis.WebSite.App
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            routes.LowercaseUrls = true;
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "UserUrl",
                url: "user/{action}",
                defaults: new { controller = "User", action = "Topic" }
            );

            routes.MapRoute(
                name: "TopicUrl",
                url: "topic/{action}/{key}",
                defaults: new { controller = "Topic", action = "Thread" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{key}",
                defaults: new { controller = "Topic", action = "Thread", key = UrlParameter.Optional }
            );
        }
    }
}