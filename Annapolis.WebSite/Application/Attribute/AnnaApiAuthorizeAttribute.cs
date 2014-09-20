using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Annapolis.Abstract.Work;
using Annapolis.Manager;
using Annapolis.Shared.Model;

namespace Annapolis.WebSite.Application.Attribute
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AnnaApiAuthorizeAttribute : AuthorizeAttribute
    {
        private static readonly string Token_UserName_Key = "cpkbUserName";
        private static readonly string Token_Header_Key = "cpkbToken";
       
        private string[] _authorizedRoles;

        public AnnaApiAuthorizeAttribute()
        {
           
        }

        public AnnaApiAuthorizeAttribute(string roleName)
        {
            _authorizedRoles = roleName.Split(new char[] { '|' });
     
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            
            bool isAuthorized = false;

            if (actionContext.Request.Headers.Contains(Token_UserName_Key) && actionContext.Request.Headers.Contains(Token_Header_Key))
            {
                string cpkbUserName = actionContext.Request.Headers.GetValues(Token_UserName_Key).FirstOrDefault();
                string cpkbToken = actionContext.Request.Headers.GetValues(Token_Header_Key).FirstOrDefault();
                TokenUser tokenUser = null;
                if (SecurityManager.VerifyToken(cpkbUserName, cpkbToken, out tokenUser))
                {
                    if(tokenUser != null)
                    {
                        if (_authorizedRoles != null && _authorizedRoles.Length > 0)
                        {
                            isAuthorized = _authorizedRoles.Contains(tokenUser.RoleName);
                        }
                        else
                        {
                            isAuthorized = true;
                        }
                    }
                }
                
            }

            if (!isAuthorized)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

        }

    }
}