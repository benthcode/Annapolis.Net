using System;
using Annapolis.Shared.Model;
using Newtonsoft.Json;
using Annapolis.Web.Client;
using System.ComponentModel;

namespace Annapolis.WebSite.ClientModels
{
    public class UserRegistrationClient : ClientModel
    {

        static UserRegistrationClient()
        {
            RegisterModelTargetClassName(typeof(UserRegistrationClient), "UserRegistration");
        }

      
        public string UserName { get; set; }
        public string RegisterEmail { get; set; }

        public string Password { get; set; }

   
    }

    public class UserSignInClient : ClientModel
    {

        static UserSignInClient()
        {
            RegisterModelTargetClassName(typeof(UserSignInClient), "UserSignIn");
        }

        public string Identifier { get; set; }
        public string Password { get; set; }
        public bool IsCookiePersistent { get; set; }

    }

    public class TokenUserClient : ClientModel
    {

        static TokenUserClient()
        {
            RegisterModelTargetClassName(typeof(TokenUserClient), "TokenUser");
        }

        public TokenUserClient()
        {
        }

        public TokenUserClient(TokenUser tokenUser):this()
        {
            if (tokenUser == null) { throw new Exception("TokenUser cannot be null!"); }

            Id = tokenUser.UserId;
            RegisterEmail = tokenUser.RegisterEmail;
            UserName = tokenUser.UserName;
            RoleName = tokenUser.RoleName;
            Token = tokenUser.Token;
            IsAdmin = tokenUser.IsAdmin;
            IsApproved = tokenUser.IsApproved;
            IsLocked = tokenUser.IsLockedOut;
        }

       
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string RegisterEmail { get; set; }
        public string Token { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLocked { get; set; }
     
        public bool IsAdmin { get; set; }

        [JsonIgnore]
        public bool IsAuthenticated
        {
            get { return IsApproved && !IsLocked; }
        }

        [JsonIgnore]
        public Guid Id { get; set; }

        public void Reset()
        {
            Id = Guid.Empty;
            RegisterEmail = string.Empty;
            UserName = string.Empty;
            RoleName = string.Empty;
            Token = string.Empty;
            IsApproved = false;
            IsLocked = false;
            IsAdmin = false;
            ClearNotifications();
        }
    }

    public class UserPasswordUpdteClient : ClientModel
    {
        static UserPasswordUpdteClient()
        {
            RegisterModelTargetClassName(typeof(UserPasswordUpdteClient), "UserPasswordUpdate");
        }

        public UserPasswordUpdteClient()
        {
        }

        public UserPasswordUpdteClient(TokenUser user)
        {
            this.UserName = user.UserName;
            this.RegisterEmail = user.RegisterEmail;
        }

        public string UserName { get; set; }
        public string RegisterEmail { get; set; }   
     
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

    }

}