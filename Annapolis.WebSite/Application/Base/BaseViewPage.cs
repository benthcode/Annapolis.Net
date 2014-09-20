using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Annapolis.Entity;
using Annapolis.WebSite.App;
using Annapolis.Web.Script;

namespace Annapolis.WebSite.Application.Base
{

    public abstract class BaseViewPage : WebViewPage
    {
        protected readonly Setting DefaultSetting;
        protected readonly Dictionary<string, string> LocaleResources;

        public BaseViewPage()
        {
            DefaultSetting = WebSiteConfig.DefaultSetting;
            LocaleResources = WebSiteConfig.LocaleResources;
        }
    }

    public abstract class BaseViewPage<TModel> : WebViewPage<TModel>
    {
        protected readonly Setting DefaultSetting;
        protected readonly Dictionary<string, string> LocaleResources;

        public BaseViewPage()
        {
            DefaultSetting = WebSiteConfig.DefaultSetting;
            LocaleResources = WebSiteConfig.LocaleResources;
        }
    }

    public abstract class BaseSiteViewPage : BaseViewPage, IScriptView
    {
        protected JavascriptRegister ScriptRegister;

        public JavascriptRegister ViewScriptRegister
        {
            get { return ScriptRegister; }
        }
    
        public override void InitHelpers()
        {
            base.InitHelpers();
            if (System.Web.HttpContext.Current.Items["JavascriptRegister"] == null)
            {
                System.Web.HttpContext.Current.Items["JavascriptRegister"] = new JavascriptRegister();
            }
            ScriptRegister = System.Web.HttpContext.Current.Items["JavascriptRegister"] as JavascriptRegister;
        }

    }

    public abstract class BaseSiteViewPage<TModel> : BaseViewPage<TModel>, IScriptView
    {
        protected JavascriptRegister ScriptRegister;

        public JavascriptRegister ViewScriptRegister
        {
            get { return ScriptRegister; }
        }

        public override void InitHelpers()
        {
            base.InitHelpers();
            if (System.Web.HttpContext.Current.Items["JavascriptRegister"] == null)
            {
                System.Web.HttpContext.Current.Items["JavascriptRegister"] = new JavascriptRegister();
            }
            ScriptRegister = System.Web.HttpContext.Current.Items["JavascriptRegister"] as JavascriptRegister;
        }


    }
}