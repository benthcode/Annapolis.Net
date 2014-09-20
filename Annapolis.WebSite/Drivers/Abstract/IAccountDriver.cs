using Annapolis.Shared.Model;
using Annapolis.WebSite.ClientModels;
using Annapolis.WebSite.Drivers.Base;

namespace Annapolis.WebSite.Drivers.Abstract
{
    public interface IAccountDriver : IBaseDriver
    {
        OperationStatus TryRegister(UserRegistrationClient registerUser, out TokenUser tokenUser);
        OperationStatus TrySignIn(UserSignInClient signInUser, out TokenUser tokenUser);
        OperationStatus TrySignOut(TokenUserClient clientTokenUser);
        OperationStatus TryUpdateUserPassword(UserPasswordUpdteClient userPasswordUpdate);
    }
}
