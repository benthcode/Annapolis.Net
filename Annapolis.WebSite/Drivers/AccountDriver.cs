using System;
using System.Linq;
using Annapolis.Abstract.Work;
using Annapolis.Entity;
using Annapolis.Manager;
using Annapolis.Shared.Model;
using Annapolis.WebSite.ClientModels;
using Annapolis.WebSite.Drivers.Abstract;
using Annapolis.WebSite.Drivers.Base;

namespace Annapolis.WebSite.Drivers
{
    public class AccountDriver : BaseDriver, IAccountDriver
    {

        private readonly IMemberUserWork _userWork;

        private readonly IMemberRoleWork _roleWork;

        public AccountDriver(IMemberUserWork userWork, IMemberRoleWork roleWork)
        {
            _userWork = userWork;
            _roleWork = roleWork;
        }
      
        public OperationStatus TryRegister(UserRegistrationClient registerUser, out TokenUser tokenUser)
        {

            if (registerUser == null) { tokenUser = null; return OperationStatus.DataFormatError; }

            tokenUser = null;
            registerUser.ServerStatus = false;
            OperationStatus status = OperationStatus.None;
            
            try
            {
                var circleUser = _userWork.Create();
                circleUser.UserName = registerUser.UserName;
                circleUser.RegisterEmail = registerUser.RegisterEmail;
                circleUser.Password = registerUser.Password;
                status = _userWork.Save(circleUser);

                if (status == OperationStatus.Success)
                {
                    MemberRole role = _roleWork.AllCacheItems.Where(x => x.Id == circleUser.RoleId).SingleOrDefault();
                    tokenUser = new TokenUser(circleUser, role);
                    SecurityManager.AddOrUpdateCurrentTokenUser(tokenUser);
                    status = OperationStatus.RegisterSuccess;
                    registerUser.ServerStatus = true;
                    return status;
                }
            }
            catch (Exception ex)
            {
                tokenUser = null;
                status = OperationStatus.GenericError;
                registerUser.ServerStatus = false;
                LoggingWork.Error(ex);
            }
            
           
            tokenUser = null;
            return status;
        }

        public OperationStatus TrySignIn(UserSignInClient signInUser, out TokenUser tokenUser)
        {
            if (signInUser == null) { tokenUser = null; return OperationStatus.DataFormatError; }

            signInUser.ServerStatus = false;
            OperationStatus status = OperationStatus.None;
            try
            {
                MemberUser user = null;
                if (_userWork.ValidateUser(signInUser.Identifier, signInUser.Password, out user))
                {
                    tokenUser = new TokenUser(user);
                    SecurityManager.AddOrUpdateCurrentTokenUser(tokenUser);
                    status = OperationStatus.SignInSuccess;
                    signInUser.ServerStatus = true;
                    return OperationStatus.SignInSuccess;
                }
                else
                {
                    status = OperationStatus.NoUseOrWrongPassword;
                }
            }
            catch(Exception ex)
            {
                tokenUser = null;
                signInUser.ServerStatus = false;
                status = OperationStatus.GenericError;
                LoggingWork.Error(ex);
            }

            tokenUser = null;
            return status;
        }

        public OperationStatus TrySignOut(TokenUserClient clientTokenUser)
        {
            try
            {
                if (clientTokenUser == null) return OperationStatus.NotValidUser;
                if (SecurityManager.VerifyToken(clientTokenUser.UserName, clientTokenUser.Token))
                {
                    SecurityManager.RemoveTokenUser(clientTokenUser.UserName);
                }
                else
                {
                    return OperationStatus.NotValidUser;
                }
                clientTokenUser.Reset();
                clientTokenUser.ServerStatus = true;
                return OperationStatus.SignOutSuccess;
            }
            catch
            {
                clientTokenUser.ServerStatus = false;
                return OperationStatus.GenericError;
            }
        }

        public OperationStatus TryUpdateUserPassword(UserPasswordUpdteClient userPasswordUpdate)
        {
            try
            {
                OperationStatus status = _userWork.UpdatePassword(userPasswordUpdate.UserName, userPasswordUpdate.OldPassword, userPasswordUpdate.NewPassword);
                userPasswordUpdate.ServerStatus = true;
                return status;
            }
            catch
            {
                userPasswordUpdate.ServerStatus = false;
                return OperationStatus.GenericError;
            }
        }

    }
}