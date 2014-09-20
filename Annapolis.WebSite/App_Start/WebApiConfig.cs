using System.Web.Http;
using System.Web.Http.ModelBinding;
using Annapolis.Web.Client;
using Annapolis.Web.Http;
using Annapolis.WebSite.Application;
using Annapolis.WebSite.ClientModels;

namespace Annapolis.WebSite.App
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
           
            config.Routes.MapHttpRoute(
              name: "UserActionApi",
              routeTemplate: "api/user/{action}",
              defaults: new { controller = "User" }
            );

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{key}",
                defaults: new { key = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{key}",
                defaults: new { key = RouteParameter.Optional }
            );


            config.Formatters.Insert(0, new JsonNetMediaTypeFormatter());
        }
    }
}
