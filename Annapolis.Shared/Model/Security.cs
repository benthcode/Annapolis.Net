using System;
using System.Security.Principal;
using Annapolis.Entity;

namespace Annapolis.Shared.Model
{
    public class CirclePrincipal : IPrincipal
    {
        private CircleIdentity _identity;
        private string _roleName;
        private bool _isAdmin;


        public CirclePrincipal(TokenUser tokenUser)
        {
            _identity = new CircleIdentity(tokenUser);
            _roleName = tokenUser.RoleName;
            _isAdmin = tokenUser.IsAdmin;
        }

        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
           
            return _isAdmin || string.Compare(_roleName, role, true) == 0;
        }

    }

    public class CircleIdentity : IIdentity
    {

        private bool _isAuthenticated;
        private string _name;

        public CircleIdentity(TokenUser tokenUser)
        {
            _name = tokenUser.UserName;
            _isAuthenticated = tokenUser.IsAuthenticated;
        }

        public string AuthenticationType
        {
            get { return "CircleAuth"; }
        }

        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
        }

        public string Name
        {
            get { return _name; }
        }
        
    }

    public class TokenUser
    {
        private bool _isAuthenticated;
        private string _userName;
        private string _roleName;
        private string _registerEmail;

        private string _token;
        private Guid _userId;
        private Guid _roleId;
        private bool _isAdmin;
        private bool _isApproved;
        private bool _isLockedOut;


        public TokenUser(MemberUser user):this(user, user.Role)
        {
           
        }

        public TokenUser(MemberUser user, MemberRole role)
        {
            if (user == null || role == null) throw new Exception("User and Role cannot be null");

            _userName = user.UserName;
            _roleName = role.RoleName;
            _registerEmail = user.RegisterEmail;

            _token = user.Token;
            _userId = user.Id;
            _roleId = user.RoleId;
            _isApproved = user.IsApproved;
            _isLockedOut = user.IsLockedOut;
            _isAdmin = role.IsAdmin;

        }


        public bool IsAuthenticated
        {
            get { return _isApproved&& !_isLockedOut; }
        }

        public string UserName
        {
            get { return _userName; }
        }

        public string RoleName
        {
            get { return _roleName; }
        }

        public string RegisterEmail
        {
            get { return _registerEmail; }
        }


        public string Token
        {
            get { return _token; }
        }

        public Guid UserId 
        {
            get { return _userId; }
        }

        public Guid RoleId
        {
            get { return _roleId; }
        }

        public bool IsApproved
        {
            get { return _isApproved; }
        }

        public bool IsLockedOut
        {
            get { return _isLockedOut; }
        }

        public bool IsAdmin
        {
            get { return _isAdmin; }
        }
    }
     
}