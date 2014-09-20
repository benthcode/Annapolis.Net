using System.Collections.Generic;
using System.Web.Mvc;
using Annapolis.Abstract.Work;
using Annapolis.Data;
using Annapolis.Entity;
using Annapolis.WebSite.App;
using Autofac;
using DevTrends.MvcDonutCaching;
using DevTrends.MvcDonutCaching.Annotations;
using Annapolis.Web.Script;
using Annapolis.Shared.Model;
using System.Text;
using Annapolis.Web.Client;
using Annapolis.Web.Mvc;

namespace Annapolis.WebSite.Application.Base
{
    public abstract class BaseMvcController : System.Web.Mvc.Controller
    {
        protected readonly Setting DefaultSetting;
        protected readonly Dictionary<string, string> LocaleResources;

        protected readonly ILoggingWork LoggingWork;
      
        public BaseMvcController()
        {
            DefaultSetting = WebSiteConfig.DefaultSetting;
            LocaleResources = WebSiteConfig.LocaleResources;

            LoggingWork = DependencyResolver.Current.GetService<ILoggingWork>();
           
        }

        protected  ClientJsonResult ClientJson(ClientModel model)
        {
            return new ClientJsonResult() { Model = model };
        }

        protected  ClientJsonResult ClientJson(ClientModel model, string contentType , Encoding contentEncoding)
        {
            return new ClientJsonResult()
            {
                ContentEncoding = contentEncoding,
                ContentType = contentType,
                Model = model
            };
        }


        public ILifetimeScope LifetimeScope
        {
            get;
            set;
        }

        public OutputCacheManager OutputCacheManager
        {
            get;
            [UsedImplicitly]
            set;
        }

        protected int GetPageNumber(int? page)
        {
            return page.HasValue && page.Value > PageConstants.FirstPageNumber ? page.Value : PageConstants.FirstPageNumber;
        }
    }

    public abstract class BaseSiteMvcController : BaseMvcController
    {
        protected JavascriptRegister ScriptRegister;

        public BaseSiteMvcController()
        {
            if (System.Web.HttpContext.Current.Items["JavascriptRegister"] == null)
            {
                System.Web.HttpContext.Current.Items["JavascriptRegister"] = new JavascriptRegister();
            }
            ScriptRegister = System.Web.HttpContext.Current.Items["JavascriptRegister"] as JavascriptRegister;
            //ViewBag.JavascriptScriptRegisterationHelper = ScriptRegister;
        }

        
    }

    public abstract class BaseAdminSiteController : BaseMvcController
    {
        protected AnnapolisDbContext DbContext;

        public BaseAdminSiteController()
        {
            DbContext = new AnnapolisDbContext();
        }
    }
}