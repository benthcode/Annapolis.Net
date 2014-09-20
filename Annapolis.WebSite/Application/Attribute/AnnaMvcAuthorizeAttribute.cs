using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Annapolis.Abstract.Work;
using Annapolis.Manager;

namespace Annapolis.WebSite.Application.Attribute
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AnnaMvcAuthorizeAttribute : AuthorizeAttribute
    {

        private string[] _authorizedRoles;
        private readonly ISecurityWorker SecurityService;

        public AnnaMvcAuthorizeAttribute()
        {
        }

        public AnnaMvcAuthorizeAttribute(string roleName)
        {
            _authorizedRoles = roleName.Split(new char[] { '|' });

            SecurityService = DependencyResolver.Current.GetService<ISecurityWorker>();
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            base.AuthorizeCore(httpContext);

            var user = httpContext.User;
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated) return false;
            if (SecurityManager.ExistingTokenUser(user.Identity.Name) == null) return false;
            
            if (_authorizedRoles != null && _authorizedRoles.Length > 0)
            {
                return _authorizedRoles.Any(role => user.IsInRole(role));
            }
            else
            {
                return true;
            }
        }
    }
}