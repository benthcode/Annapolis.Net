using System.Web.Mvc;
using Annapolis.WebSite.Application.Base;
using Annapolis.WebSite.Drivers.Abstract;

namespace Annapolis.WebSite.Controllers
{
    public class TagController : BaseSiteMvcController
    {
        ITagDriver _tagDriver;

        public TagController(ITagDriver tagDriver)
        {
            _tagDriver = tagDriver;
        }
        
        [ChildActionOnly]
        public ActionResult TagsByCategory(string categoryName, 
                                           bool isMultiSelection = false,
                                           string selectedValue = null,
                                           string uniqueId = null,
                                           bool onlyHot = true,
                                           bool includeAll = true,
                                           bool includeOther = true)
        {

            if (!onlyHot) { includeOther = false; }
            if (isMultiSelection) { includeAll = false; }

            var tagList = _tagDriver.GetTagsByCategory(categoryName, onlyHot, includeAll, includeOther);
            tagList.MultiSelect = isMultiSelection;
            tagList.Group = categoryName;
            tagList.SelectedValue = selectedValue;
            tagList.UniqueId = uniqueId;
            return PartialView(tagList);
        }

    }
}
