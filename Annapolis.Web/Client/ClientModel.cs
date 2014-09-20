using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Annapolis.Shared.Utility;
using Annapolis.Web.Script;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Web.Mvc;
using System.IO;
using System.Dynamic;
using IModelBinder = System.Web.Mvc.IModelBinder;
using ModelBindingContext = System.Web.Mvc.ModelBindingContext;

namespace Annapolis.Web.Client
{
    
    public abstract class ClientModel : IClientModel
    {
        protected static readonly JsonSerializerSettings DefaultJsonSerializerSetting;

        public static readonly string DefaultLocaleNameSpace = "$cl"; 

        static ClientModel()
        {
            DefaultJsonSerializerSetting = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
              
                Error = delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                {
                    args.ErrorContext.Handled = true;
                }
            };
            var enumConverter = new StringEnumConverter();
            enumConverter.CamelCaseText = true;
            DefaultJsonSerializerSetting.Converters.Add(enumConverter);

        }

        public ClientModel()
        {
            ServerStatus = true;
        }

        #region Target Model Namespace & Name

        public static readonly string DefaultTargetJsonModelNameSpace = "$cm"; //"circle.viewModel";
        private static Dictionary<Type, string> _modelTargetNameSpaces = new Dictionary<Type, string>();
        private static Dictionary<Type, string> _modelTargetClassNames = new Dictionary<Type, string>();

        protected static void RegisterModelTargetNameSpace(Type type, string ns)
        {
            if (string.IsNullOrEmpty(ns)) return;
            if (_modelTargetNameSpaces.ContainsKey(type))
            {
                throw new InvalidOperationException("The target json namespace for this type has been existed!");
            }
            else
            {
                _modelTargetNameSpaces.Add(type, ns);
            }
        }

        protected static void RegisterModelTargetClassName(Type type, string modelName)
        {
            if (string.IsNullOrEmpty(modelName)) return;
            if (_modelTargetClassNames.ContainsKey(type))
            {
                throw new InvalidOperationException("The target json model name for this type has been existed!");
            }
            else
            {
                _modelTargetClassNames.Add(type, modelName);
            }
        }

        public static string GetTargetModelNameSapce(Type type)
        {
            if (_modelTargetNameSpaces.ContainsKey(type)) return _modelTargetNameSpaces[type];
            return DefaultTargetJsonModelNameSpace;
        }

        public static string GetTargetModelName(Type type)
        {
            if (_modelTargetClassNames.ContainsKey(type)) return _modelTargetClassNames[type];
            throw new SystemException("No target model class name matched!");
        }

        [JsonIgnore]
        public string TargetJsonModelNameSpace
        {
            get
            {
                Type type = this.GetType();
                return GetTargetModelNameSapce(type);
            }
        }

        [JsonIgnore]
        public string TargetJsonModelName
        {
            get
            {
                Type type = this.GetType();
                return GetTargetModelName(type);
            }
        }

        #endregion

        #region Notification

        [JsonProperty]
        private List<Notification> ServerNotifications { get; set; }

        public void AddNotification(Notification notification)
        {
            if (ServerNotifications == null) ServerNotifications = new List<Notification>();
            ServerNotifications.Add(notification);
        }

        public void AddNotification(string message, NotificationType notificationType = NotificationType.Information,
                                    int timeOut = Notification.ShortTime, bool isModel = false, bool isVisible = true)
        {
            AddNotification(new Notification(message, notificationType, timeOut, isModel, isVisible));
        }

        public void AddErrorNotification(string errorMessage, int timeOut = Notification.Stick, bool isModal = true, bool isVisible = true)
        {
            AddNotification(new Notification(errorMessage, NotificationType.Error, timeOut, isModal, isVisible));
        }

        public void AddSuccessNotification(string successMessage, int timeOut = Notification.ShortTime, bool isModal = false, bool isVisible = true)
        {
            AddNotification(new Notification(successMessage, NotificationType.Success, timeOut, isModal, isVisible));
        }

        public void AddInformationNotification(string infoMessage, int timeOut = Notification.MediumTime, bool isModal = false, bool isVisible = true)
        {
            AddNotification(new Notification(infoMessage, NotificationType.Information, timeOut, isModal, isVisible));
        }

        public void AddAlertNotification(string alertMessage, int timeOut = Notification.MediumTime, bool isModal = true, bool isVisible = true)
        {
            AddNotification(new Notification(alertMessage, NotificationType.Alert, timeOut, isModal, isVisible));
        }

        public void AddWarningNotification(string warningMessage, int timeOut = Notification.LongTime, bool isModal = false, bool isVisible = true)
        {
            AddNotification(new Notification(warningMessage, NotificationType.Warning, timeOut, isModal, isVisible));
        }

        public void AddConfirmNotification(string confirmMessage, int timeOut = Notification.Stick, bool isModal = true, bool isVisible = true)
        {
            AddNotification(new Notification(confirmMessage, NotificationType.Confirmation, timeOut, isModal, isVisible));
        }

        public void ClearNotifications()
        {
            if (ServerNotifications != null)
            {
                ServerNotifications.Clear();
            }
        }

        #endregion

        #region Json Interation

        public virtual string ToJson(JsonSerializerSettings setting = null)
        {
            var str = JsonConvert.SerializeObject(this,  setting ?? DefaultJsonSerializerSetting);
            return str;
        }

        public static string ToJson(object obj, JsonSerializerSettings setting = null)
        {
            var x = DefaultJsonSerializerSetting.Converters;
            return JsonConvert.SerializeObject(obj, setting ?? DefaultJsonSerializerSetting);
        }

        public static object FromJson(string jsonString, JsonSerializerSettings setting = null)
        {
            if (jsonString == null) return null;
            var model = JsonConvert.DeserializeObject(jsonString, setting ?? DefaultJsonSerializerSetting);
            return model;
        }

        public static object FromJson(string jsonString, Type type, JsonSerializerSettings setting = null)
        {
            if (jsonString == null) return null;
            var model = JsonConvert.DeserializeObject(jsonString, type, setting ?? DefaultJsonSerializerSetting);
            return model;
        }

        public static T FromJson<T>(string jsonString, JsonSerializerSettings setting = null) where T : ClientModel
        {
            if (jsonString == null) return null;
            var model = JsonConvert.DeserializeObject<T>(jsonString, setting ?? DefaultJsonSerializerSetting);
            if (model != null)
            {
                model.ServerStatus = false;
                model.ClearNotifications();
            }
            return model;
        }

        public virtual void RegisterToClient(string name, JavascriptRegister jsRegister, bool requireDefinedOnClient = true)
        {
            jsRegister.AddJson(name, this, requireDefinedOnClient);
        }

        #endregion

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool ServerStatus { get; set; }


        public string UniqueId { get; set; }
        public string ServerActionKey { get; set; }

        public void GenerateUniqueId()
        {
            UniqueId = StringUtility.GenerateAlphabet(12);
        }

    }

    public abstract class IdenticalClientModel : ClientModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// It is for empty viewModel. 
        /// it will be created on memory and also used for communication with client side, but cannot be added to server
        /// </summary>
        //public bool CanAdd { get; set; }

        //public bool UpdateApproved { get; set; }

        //public bool DeleteApproved { get; set; }

        public bool IsNew()
        {
            return Id == Guid.Empty;
        }

    }

    public abstract class OwnerClientModel : IdenticalClientModel
    {
        public string UserName { get; set; }
    }
}
