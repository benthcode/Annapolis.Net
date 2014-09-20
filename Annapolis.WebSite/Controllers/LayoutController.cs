using System.Web.Mvc;
using Annapolis.WebSite.Application.Base;

namespace Annapolis.WebSite.Controllers
{
    public class LayoutController : BaseSiteMvcController
    {

        public LayoutController()
        { 
        }

        [ChildActionOnly]
        public ActionResult TopMenu()
        {
            return View();
        }

    }
}
