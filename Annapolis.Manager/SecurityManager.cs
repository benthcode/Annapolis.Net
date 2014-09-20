using Annapolis.Abstract.Work;
using Annapolis.IoC;
using Annapolis.Shared.Model;
using Microsoft.Practices.Unity;

namespace Annapolis.Manager
{
    public class SecurityManager 
    {
      
        public static ISecurityWorker Service { get; set; }

        static SecurityManager()
        {
            Service = UnityMVC.Container.Resolve<ISecurityWorker>();
        }

        public static TokenUser ExistingTokenUser(string userName)
        {

            return Service.ExistingTokenUser(userName);
        }

        public static void AddOrUpdateTokenUser(TokenUser user)
        {
            Service.AddOrUpdateTokenUser(user);
        }

        public static void AddOrUpdateCurrentTokenUser(TokenUser user)
        {
            Service.AddOrUpdateCurrentTokenUser(user);
        }

        public static bool VerifyToken(string userName, string token)
        {
            return Service.VerifyToken(userName, token);
        }

        //[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public static bool VerifyToken(string userName, string token, out TokenUser user)
        {
            return Service.VerifyToken(userName, token, out user);
        }

        /// <summary>
        /// Verify token user based on current HttpContext User
        /// </summary>
        public static bool VerifyToken()
        {
            return Service.VerifyToken();
        }

        /// <summary>
        /// Remove token user based on current HttpContext User
        /// </summary>
        public static void RemoveTokenUser()
        {
            Service.RemoveTokenUser();
        }

        public static void RemoveTokenUser(TokenUser user)
        {
            Service.RemoveTokenUser();
        }

        public static void RemoveTokenUser(string userName)
        {
            Service.RemoveTokenUser(userName);
        }

        public static void Clear()
        {
            Service.Clear();
        }

        public static TokenUser CurrentUser
        {
            get { return Service.CurrentUser; }
        }

        public static bool IsCurrentUserValid()
        {
            return Service.IsCurrentUserValid(); 
        }

    }
}
