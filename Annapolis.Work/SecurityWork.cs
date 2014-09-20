using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Annapolis.Abstract.Work;
using Annapolis.Shared.Model;
using Microsoft.Practices.Unity;
using System.Security.Principal;
using Annapolis.Entity;

namespace Annapolis.Work
{

    public class SecurityWork :AnnapolisBaseWork, ISecurityWorker
    {

        private static readonly string Security_Service_CacheKey = "Security_Service_CacheKey";
       
        private static Dictionary<string, TokenUser> _dictToken;

        
        private ICacheWork _cacheManager { get; set; }
        private IMemberUserWork _userWorker { get; set; }

        public SecurityWork(ICacheWork cacheWork, IMemberUserWork userWorker)
        {
            _userWorker = userWorker;
            _cacheManager = cacheWork;
            if (!_cacheManager.Contains(Security_Service_CacheKey))
            {
                _cacheManager.AddOrUpdate(Security_Service_CacheKey, new Dictionary<string, TokenUser>());
            }
            _dictToken = _cacheManager.GetData<Dictionary<string, TokenUser>>(Security_Service_CacheKey);
        }

        public TokenUser ExistingTokenUser(string userName)
        {

            if (_dictToken.ContainsKey(userName)) 
            {
                return _dictToken[userName];
            }

            return null;
        }

        public void AddOrUpdateTokenUser(TokenUser user)
        {

            if (_dictToken.ContainsKey(user.UserName))
            {
                _dictToken[user.UserName] = user;
            }
            else
            {
                _dictToken.Add(user.UserName, user);
            }
        }

        public void AddOrUpdateCurrentTokenUser(TokenUser user)
        {

            if (CurrentUser != null)
            {
                RemoveTokenUser(user.UserName);
            }

            AddOrUpdateTokenUser(user);
            ContextUser = new CirclePrincipal(user);
        }

        public bool VerifyToken(string userName, string token)
        {
            try
            {
                TokenUser user;
                return VerifyToken(userName, token, out user);
            }
            catch
            {

            }

            return false;
        }

        public bool VerifyToken(string userName, string token, out TokenUser user)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(token))
                { 
                    user = null; 
                    return false;
                }

                if (!_dictToken.ContainsKey(userName))
                {
                    MemberUser memberUser;
                    if (_userWorker.ValidateToken(userName, token, out memberUser) == true)
                    {
                        _dictToken.Add(userName, new TokenUser(memberUser));
                    }
                }

                if (_dictToken.ContainsKey(userName))
                {
                    if (_dictToken[userName].Token == token)
                    {
                        user = _dictToken[userName];
                        return true;
                    }
                }

            }
            catch
            {

            }

            user = null;
            return false;
        }

        /// <summary>
        /// Verify token item based on current HttpContext User
        /// </summary>
        public bool VerifyToken()
        {
            if (CurrentUser != null)
            {
                return VerifyToken(CurrentUser.UserName, CurrentUser.Token);
            }

            return false;
        }

        /// <summary>
        /// Remove token item based on current HttpContext User
        /// </summary>
        public void RemoveTokenUser()
        {
            if(CurrentUser != null)
            {
               RemoveTokenUser(CurrentUser);
            }
        }

        public void RemoveTokenUser(TokenUser user)
        {
            if (user != null)
            {
                RemoveTokenUser(user.UserName);
            }
        }

        public void RemoveTokenUser(string userName)
        {
            if (_dictToken.ContainsKey(userName))
            {
                _dictToken.Remove(userName);
            }

            if(CurrentUser!= null && CurrentUser.UserName == userName)
            {
                ContextUser = null;
            }
        }

        public void Clear()
        {
            _dictToken.Clear();
        }

        public bool IsCurrentUserValid()
        {
            return (CurrentUser != null) && CurrentUser.IsAuthenticated;
        }

        public TokenUser CurrentUser
        {
            get 
            {
                if (ContextUser != null && ContextUser.Identity != null)
                {
                    if (_dictToken.ContainsKey(ContextUser.Identity.Name)) 
                    {
                        return _dictToken[ContextUser.Identity.Name];
                    }
                }

                return null;
            }
        }

        private IPrincipal ContextUser
        {
            get { return HttpContext.Current.User; }
            set { HttpContext.Current.User = value; }
        }

    }
}