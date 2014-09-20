using System.Web.Http;
using System.Web.Mvc;
using Annapolis.Manager;
using Annapolis.Shared.Model;
using Annapolis.WebSite.Application.Base;
using Annapolis.WebSite.ClientModels;
using Annapolis.WebSite.Drivers.Abstract;
using Annapolis.Web.Client;
using Annapolis.WebSite.Application.Attribute;

namespace Annapolis.WebSite.Controllers.Api
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountDriver _accountDriver;

        public AccountController()
        {
            _accountDriver = DependencyResolver.Current.GetService<IAccountDriver>();
        }

        [System.Web.Http.HttpPost]
        [AnnaApiAuthorize]
        public ClientModel UpdatePassword([FromBody]UserPasswordUpdteClient user)
        {
            if (user == null) return null;

            OperationStatus status = _accountDriver.TryUpdateUserPassword(user);
            if (status == OperationStatus.PasswordUpdateSuccess) { user.AddSuccessNotification(MessageManager.GetMessage(status)); }
            else { user.AddErrorNotification(MessageManager.GetMessage(status)); }

            return user;
        }
    }
}
