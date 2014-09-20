using Annapolis.WebSite.ClientModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Annapolis.WebSite.ViewModels
{

    public class UserRegisterViewModel
    {
        [Required]
        [MinLength(4)]
        public string UserName { get; set; }
        [EmailAddress]
        public string RegisterEmail { get; set; }
        [MinLength(6)]
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }

    public class UserSignInViewModel
    {
        [Required]
        [MinLength(4)]
        public string Identifier { get; set; }
        [MinLength(6)]
        public string Password { get; set; }
        public bool IsCookiePersistent { get; set; }
    }

    public class UserAuthenticationViewModel
    {
        public UserRegisterViewModel UserRegister { get; set; }
        public UserSignInViewModel UserSignIn { get; set; }
    }
}