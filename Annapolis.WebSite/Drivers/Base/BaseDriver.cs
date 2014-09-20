using System.Collections.Generic;
using System.Web.Mvc;
using Annapolis.Abstract.UnitOfWork;
using Annapolis.Abstract.Work;
using Annapolis.Entity;
using Annapolis.WebSite.App;

namespace Annapolis.WebSite.Drivers.Base
{

    
    public abstract class BaseDriver : IBaseDriver
    {
        protected readonly Setting DefaultSetting;
        protected readonly Dictionary<string, string> LocaleResources;

        protected readonly ILoggingWork LoggingWork;
        protected readonly IUnitOfWorkManager UnitOfWorkManager;

        public BaseDriver()
        {
            DefaultSetting = WebSiteConfig.DefaultSetting;
            LocaleResources = WebSiteConfig.LocaleResources;
  
            LoggingWork = DependencyResolver.Current.GetService<ILoggingWork>();
            UnitOfWorkManager = DependencyResolver.Current.GetService<IUnitOfWorkManager>();
        }

    }

    public abstract class EntityDriver : BaseDriver
    { 
    
    }

}