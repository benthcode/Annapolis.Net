using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Annapolis.IoC;
using Annapolis.Manager;
using Annapolis.Shared.Model;
using Annapolis.WebSite.App;
using Annapolis.WebSite.ClientModels;
using Microsoft.Practices.Unity;
using Annapolis.Web.Client;
using Annapolis.WebSite.Application;
using System.Collections.Generic;

namespace Annapolis.WebSite
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class AnnapolisHttpApplication : System.Web.HttpApplication
    {
        public static readonly string Session_UserName_Key = "circleUserName"; 

        protected void Application_Start()
        {

            IUnityContainer container= UnityMVC.Build();
            IocConfig.Register(container);
            UnityMVC.Start(container);
           
            WebSiteConfig.Load();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var allClientModelTypes = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(a => !a.IsAbstract && !a.IsGenericType && a.IsSubclassOf(typeof(ClientModel))).ToList();
            foreach (var clientModelType in allClientModelTypes)
            {
                ModelBinders.Binders.Add(clientModelType, new ClientModelMvcModelBinder());
            }

            ForceCallingStaticContructor();

        }

        private void ForceCallingStaticContructor()
        {
            TagOptionClient to = new TagOptionClient();

            var allClientModelTypes = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(a => !a.IsAbstract && !a.IsGenericType && a.IsSubclassOf(typeof(ClientModel))).OrderBy(x => x.IsAssignableFrom(typeof(IModelList))).ToList();
            foreach (var clientModelType in allClientModelTypes)
            {
                var obj = Activator.CreateInstance(clientModelType);
            }
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
           
            if (HttpContext.Current.User != null)
            {
                FormsIdentity formIdentity = Context.User.Identity as FormsIdentity;
                
                TokenUser user;
                if (SecurityManager.VerifyToken(formIdentity.Name, formIdentity.Ticket.UserData, out user))
                {
                    if (user != null)
                    {
                        HttpContext.Current.User = new CirclePrincipal(user);
                    }
                }
            }
        
        }

        protected void Session_OnEnd(object sender, EventArgs e)
        {
            if (Session[Session_UserName_Key] != null)
            {
                string userName = Session[Session_UserName_Key] as string;
            }
        }
       
    }
}