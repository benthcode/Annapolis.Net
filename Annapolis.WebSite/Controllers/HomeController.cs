using System;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;

namespace Annapolis.WebSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Simple");
        }


        [DonutOutputCache(Duration = 24 * 3600)]
        public ActionResult Simple()
        {
            return View(DateTime.Now);
        }

        [ChildActionOnly, DonutOutputCache(Duration = 60, Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
        public ActionResult SimpleDonutOne()
        {
            return PartialView(DateTime.Now);
        }

        [ChildActionOnly] 
        public ActionResult NestedDonutOne()
        {
            return PartialView(DateTime.Now);
        }

        [ChildActionOnly]
        public ActionResult SimpleDonutTwo()
        {
            return PartialView(DateTime.Now);
        }

        public ActionResult ExpireSimpleDonutCache()
        {
         
            return Content("OK", "text/plain");
        }

        public ActionResult ExpireSimpleDonutOneCache()
        {
         
            return Content("OK", "text/plain");
        }
    }
}
