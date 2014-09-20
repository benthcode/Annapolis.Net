using System.Collections.Generic;
using Annapolis.Entity;
using Annapolis.Shared.Model;
using Annapolis.WebSite.App;

namespace Annapolis.WebSite.Application.Base
{
    public abstract class BaseApiController : System.Web.Http.ApiController 
    {
        protected readonly Setting DefaultSetting;
        protected readonly Dictionary<string, string> LocaleResources;

        protected static readonly string Token_UserName_Key = "cpkbUserName";
        protected static readonly string Token_Header_Key = "cpkbToken";

        public BaseApiController()
        {
            DefaultSetting = WebSiteConfig.DefaultSetting;
            LocaleResources = WebSiteConfig.LocaleResources;
        }

        protected int GetPageNumber(int? page)
        {
            return page.HasValue && page.Value > PageConstants.FirstPageNumber ? page.Value : PageConstants.FirstPageNumber;
        }
    }
}
