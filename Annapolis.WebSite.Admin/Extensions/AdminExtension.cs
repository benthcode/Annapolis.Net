using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Annapolis.WebSite.Admin.Extensions
{
    public static class AdminExtension
    {

        public static MvcHtmlString AdminActionLink(this HtmlHelper htmlHelper, string linkText, string actionName)
        {
            return htmlHelper.ActionLink(linkText, actionName);
        }

        public static MvcHtmlString AdminAction(this HtmlHelper htmlHelper, string actionName)
        {
            return htmlHelper.Action(actionName);
        }

        public static MvcHtmlString AdminActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValue)
        {
            return htmlHelper.ActionLink(linkText, actionName, routeValue); 
        }

        public static MvcHtmlString AdminAction(this HtmlHelper htmlHelper, string actionName, object routeValue)
        {
            return htmlHelper.Action(actionName, routeValue); 
        }

        public static MvcHtmlString AdminActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string adminControllerName)
        {
            return htmlHelper.ActionLink(linkText, actionName, adminControllerName); 
        }

        public static MvcHtmlString AdminAction(this HtmlHelper htmlHelper, string actionName, string adminControllerName)
        {
            return htmlHelper.Action(actionName, adminControllerName); 
        }

        public static MvcHtmlString AdminActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string adminControllerName, object htmlAttributes)
        {
            return htmlHelper.ActionLink(linkText, actionName, adminControllerName, htmlAttributes);
        }

        public static MvcHtmlString AdminActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string adminControllerName, object routeValues, object htmlAttributes)
        {
            return htmlHelper.ActionLink(linkText, actionName, adminControllerName, routeValues, htmlAttributes); 
        }


        public static MvcHtmlString AdminAction(this HtmlHelper htmlHelper, string actionName, string adminControllerName, object htmlAttributes)
        {
            return htmlHelper.Action(actionName, adminControllerName, htmlAttributes); 
        }



        public static string AdminAction(this UrlHelper urlHelper, string actionName)
        {
            return urlHelper.Action(actionName);
        }

        public static string AdminAction(this UrlHelper urlHelper, string actionName, object routeValue)
        {
            return urlHelper.Action(actionName, routeValue);
        }

        public static string AdminAction(this UrlHelper urlHelper, string actionName, string adminControllerName)
        {
            return urlHelper.Action(actionName, adminControllerName);
        }

        public static string AdminAction(this UrlHelper urlHelper, string actionName, string adminControllerName, object routeValue)
        {
            return urlHelper.Action(actionName, adminControllerName, routeValue);
        }

    }
}