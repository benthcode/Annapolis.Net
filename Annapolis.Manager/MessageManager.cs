using System.Collections.Generic;
using Annapolis.Abstract.Work;
using Annapolis.IoC;
using Annapolis.Shared.Model;
using Microsoft.Practices.Unity;

namespace Annapolis.Manager
{
    public class MessageManager 
    {
        private static readonly string Message_Manager_CacheKey = "Message_Manager_CacheKey";
        private static readonly string Message_Manager_Operation_Status_CacheKey = "Message_Manager_Operation_Status_CacheKey";

        private static ICacheWork _cacheManager;
        private static ILanguageWork _languageWork;
        private static ISettingWork _settingWork;

        private static Dictionary<OperationStatus, string> _operationStatus;
        private static Dictionary<string, string> _defaultLocaleResources;

        static MessageManager()
        {
            _cacheManager = UnityMVC.Container.Resolve<ICacheWork>();
            _languageWork = UnityMVC.Container.Resolve<ILanguageWork>();
            _settingWork = UnityMVC.Container.Resolve<ISettingWork>();

            Reload();

            _cacheManager.AddOrUpdate(Message_Manager_Operation_Status_CacheKey, new Dictionary<OperationStatus, string>());
            _operationStatus = _cacheManager.GetData<Dictionary<OperationStatus, string>>(Message_Manager_Operation_Status_CacheKey);
            LoadOperationStatusKey();
        }

        public static void Reload()
        {
            _cacheManager.Remove(Message_Manager_CacheKey);
            _cacheManager.AddOrUpdate(Message_Manager_CacheKey, new Dictionary<string, string>());
            _defaultLocaleResources = _cacheManager.GetData<Dictionary<string, string>>(Message_Manager_CacheKey);

            foreach (var resourceKey in _languageWork.AllResourceKeyCacheItems)
            {
                var resourceValue = _languageWork.GetResourceValue(_settingWork.GetDefaultSetting().Language, resourceKey);
                _defaultLocaleResources.Add(resourceKey.ResourceKey, resourceValue.ResourceValue);
            }
        }

        private static void LoadOperationStatusKey()
        {
            _operationStatus.Add(OperationStatus.Success, "App.OperationStatus.Success");
            _operationStatus.Add(OperationStatus.NoPermission, "App.OperationStatus.NoPermission");
             _operationStatus.Add(OperationStatus.GenericError, "App.OperationStatus.GenericError");
             _operationStatus.Add(OperationStatus.DataFormatError, "App.OperationStatus.DataFormatError");
            

            _operationStatus.Add(OperationStatus.SignInSuccess, "Membership.OperationStatus.UserSignInSuccess");
            _operationStatus.Add(OperationStatus.SignOutSuccess, "Membership.OperationStatus.UserSignOutSuccess");
            _operationStatus.Add(OperationStatus.RegisterSuccess, "Membership.OperationStatus.UserRegisterSuccess");
            _operationStatus.Add(OperationStatus.NoUseOrWrongPassword, "Membership.OperationStatus.NoUserOrWrongPassword");
            _operationStatus.Add(OperationStatus.NotValidUser, "Membership.OperationStatus.NotValidUser");
            
            _operationStatus.Add(OperationStatus.InvalidUserName, "Membership.OperationStatus.InvalidUserName");
            _operationStatus.Add(OperationStatus.InvalidEmail, "Membership.OperationStatus.InvalidEmail");
            _operationStatus.Add(OperationStatus.InvalidPassword, "Membership.OperationStatus.InvalidPassword");
            _operationStatus.Add(OperationStatus.DuplicatedUserName, "Membership.OperationStatus.DuplicatedUserName");
            _operationStatus.Add(OperationStatus.DuplicatedRegisterEmail, "Membership.OperationStatus.DuplicatedRegisterEmail");
            _operationStatus.Add(OperationStatus.DuplicatedContactEmail, "Membership.OperationStatus.DuplicatedContactEmail");
            _operationStatus.Add(OperationStatus.InvalidQuestion, "Membership.OperationStatus.InvalidQuestion");
            _operationStatus.Add(OperationStatus.InvalidAnswer, "Membership.OperationStatus.InvalidAnswer");
            _operationStatus.Add(OperationStatus.UserRejected, "Membership.OperationStatus.UserRejected");
            _operationStatus.Add(OperationStatus.Verification, "Membership.OperationStatus.Verification");
            _operationStatus.Add(OperationStatus.PasswordUpdateSuccess, "Membership.OperationStatus.PasswordUpdateSuccess");
            _operationStatus.Add(OperationStatus.PasswordUpdateFail, "Membership.OperationStatus.PasswordUpdateFail");

            _operationStatus.Add(OperationStatus.VoteHasExisted, "Vote.VoteHasExisted");
            _operationStatus.Add(OperationStatus.VoteCannotForYourSelf, "Vote.VoteCannotForYourSelf");
        }

        public static Dictionary<string, string> Resources
        {
            get { return _defaultLocaleResources; }
        }

        public static string GetMessage(OperationStatus status)
        {
            if (_operationStatus.ContainsKey(status))
            {
                var statusKey = _operationStatus[status];
                if (_defaultLocaleResources.ContainsKey(statusKey))
                {
                    return _defaultLocaleResources[_operationStatus[status]];
                }
                return statusKey;
            }
            return status.ToString();
        }

        public static string GetMessage(string key)
        {
            return _defaultLocaleResources[key];
        }
    } 
}
