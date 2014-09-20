using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Annapolis.Manager;
using Annapolis.Shared.Model;
using Annapolis.WebSite.Application.Base;
using Annapolis.WebSite.ClientModels;
using Annapolis.WebSite.Drivers.Abstract;
using Annapolis.Web.Client;
using Annapolis.WebSite.ViewModels;
using Annapolis.Web.Attribute;
using Annapolis.Web.Mvc;

namespace Annapolis.WebSite.Controllers
{
    public class AccountController : BaseSiteMvcController
    {

        public ActionResult LoginOnPanel()
        {
            if (ControllerContext.IsChildAction || Request.IsAjaxRequest())
            {
                return View();
            }
            return HttpNotFound();
        }

        private void SaveTicketToCookie(TokenUser tokenUser, bool isPersistent)
        {
            
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2, tokenUser.UserName, 
                                                    DateTime.Now, DateTime.Now.Add(FormsAuthentication.Timeout), isPersistent, tokenUser.Token);
            string hashticket = FormsAuthentication.Encrypt(ticket);
            HttpCookie usercookie = new HttpCookie(FormsAuthentication.FormsCookieName, hashticket);
            Response.Cookies.Add(usercookie);

          
            Session[AnnapolisHttpApplication.Session_UserName_Key] = tokenUser.UserName;
        }

        [HttpPost]
        public ClientJsonResult Register(UserRegistrationClient registerUser)
        {
            if (registerUser != null)
            {
                try
                {
                    registerUser.ClearNotifications();
                    IAccountDriver driver = DependencyResolver.Current.GetService<IAccountDriver>();
                    TokenUser tokenUser;
                    OperationStatus status = driver.TryRegister(registerUser, out tokenUser);
                    if (status == OperationStatus.RegisterSuccess)
                    {
                        SaveTicketToCookie(tokenUser, false);
                        TokenUserClient clientTokenUser = new TokenUserClient(tokenUser);
                        clientTokenUser.AddSuccessNotification(MessageManager.GetMessage(status));
                        return ClientJson(clientTokenUser);
                    }
                    else
                    {
                        registerUser.ServerStatus = false;
                        registerUser.AddErrorNotification(MessageManager.GetMessage(status));
                    }
                }
                catch (Exception ex)
                {
                    registerUser.ServerStatus = false;
                    LoggingWork.Error(ex);
                    registerUser.AddErrorNotification(MessageManager.GetMessage(OperationStatus.GenericError));
                }
            }
            else
            {
                registerUser = new UserRegistrationClient() { ServerStatus = false };
                registerUser.AddErrorNotification(MessageManager.GetMessage(OperationStatus.DataFormatError));
            }

            return ClientJson(registerUser);
        }

        [HttpPost]
        public ClientJsonResult SignIn(UserSignInClient signInUser)
        {
            if (signInUser != null)
            {
                try
                {
                    signInUser.ClearNotifications();
                    IAccountDriver driver = DependencyResolver.Current.GetService<IAccountDriver>();
                    TokenUser tokenUser;
                    OperationStatus status = driver.TrySignIn(signInUser, out tokenUser);
                    if (status == OperationStatus.SignInSuccess)
                    {
                        SaveTicketToCookie(tokenUser, signInUser.IsCookiePersistent);
                        TokenUserClient clientTokenUser = new TokenUserClient(tokenUser);
                        clientTokenUser.AddSuccessNotification(MessageManager.GetMessage(status));
                        return ClientJson(clientTokenUser);
                    }
                    else
                    {
                        signInUser.ServerStatus = false;
                        signInUser.AddErrorNotification(MessageManager.GetMessage(status));
                    }
                }
                catch (Exception ex)
                {
                    signInUser.ServerStatus = false;
                    LoggingWork.Error(ex);
                    signInUser.AddErrorNotification(MessageManager.GetMessage(OperationStatus.GenericError));
                }
            }
            else
            {
                signInUser = new UserSignInClient() { ServerStatus = false };
                signInUser.AddErrorNotification(MessageManager.GetMessage(OperationStatus.DataFormatError));
            }

            return ClientJson(signInUser);
        }

        [HttpPost]
        public ClientJsonResult SignOut(TokenUserClient tokenUser)
        {
            
            if (tokenUser != null)
            {
                IAccountDriver driver = DependencyResolver.Current.GetService<IAccountDriver>();
                driver.TrySignOut(tokenUser);
                FormsAuthentication.SignOut();
                tokenUser.AddSuccessNotification(MessageManager.GetMessage(OperationStatus.SignOutSuccess));
            }
            else
            {
                tokenUser = new TokenUserClient();
                tokenUser.AddErrorNotification(MessageManager.GetMessage(OperationStatus.DataFormatError));
            }
            return ClientJson(tokenUser);

        }

        [HttpGet]
        public ActionResult LoginOn()
        {

            UserAuthenticationViewModel viewModel = new UserAuthenticationViewModel();
            viewModel.UserRegister = new UserRegisterViewModel();
            viewModel.UserSignIn = new UserSignInViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UserRegister(UserRegisterViewModel registerUser)
        {
            string errorMessage = null;
            if (registerUser == null)
            {
                registerUser = new UserRegisterViewModel();
                errorMessage = MessageManager.GetMessage(OperationStatus.DataFormatError);
            }
            else
            {
                try
                {
                    IAccountDriver driver = DependencyResolver.Current.GetService<IAccountDriver>();
                    TokenUser tokenUser;
                    UserRegistrationClient client = new UserRegistrationClient()
                                                    {
                                                        UserName = registerUser.UserName,
                                                        RegisterEmail = registerUser.RegisterEmail,
                                                        Password = registerUser.Password
                                                    };
                    OperationStatus status = driver.TryRegister(client, out tokenUser);
                    if (status == OperationStatus.RegisterSuccess)
                    {
                        SaveTicketToCookie(tokenUser, false);
                        string returnUrl = FormsAuthentication.GetRedirectUrl(string.Empty, false);
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        errorMessage = MessageManager.GetMessage(status);
                    }
                }
                catch
                {
                    errorMessage = MessageManager.GetMessage(OperationStatus.GenericError);
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ViewBag.ErrorMessage = errorMessage;
            }
            return PartialView("_UserRegister", registerUser);
        }

        [HttpPost]
        public ActionResult UserSignIn(UserSignInViewModel signInUser)
        {
            string errorMessage = null;
            if (signInUser == null)
            {
                signInUser = new UserSignInViewModel();
                errorMessage = MessageManager.GetMessage(OperationStatus.DataFormatError);
            }
            else
            {
                try
                {
                    IAccountDriver driver = DependencyResolver.Current.GetService<IAccountDriver>();
                    TokenUser tokenUser;
                    UserSignInClient client = new UserSignInClient() 
                                                {
                                                    Identifier = signInUser.Identifier,
                                                    Password = signInUser.Password
                                                };
                    OperationStatus status = driver.TrySignIn(client, out tokenUser);
                    if (status == OperationStatus.SignInSuccess)
                    {
                        SaveTicketToCookie(tokenUser, signInUser.IsCookiePersistent);
                        string returnUrl = FormsAuthentication.GetRedirectUrl(string.Empty, false);
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        errorMessage = MessageManager.GetMessage(status);
                    }
                }
                catch (Exception ex)
                {
                    LoggingWork.Error(ex);
                    errorMessage = MessageManager.GetMessage(OperationStatus.GenericError);
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ViewBag.ErrorMessage = errorMessage;
            }
            return PartialView("_UserSignIn", signInUser);
        }
        
    }
}
