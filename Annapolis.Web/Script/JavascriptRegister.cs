using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Annapolis.Manager;
using Annapolis.Web.Client;
using Annapolis.Shared.Utility;

namespace Annapolis.Web.Script
{
    public class JavascriptRegister
    {
        private class JsonScriptObject
        {
            internal IClientModel JsonModel { get; set; }

            internal bool RequiredDefineOnClientSide { get; set; }
           
        }

        private class VariableScriptObject
        {
            internal string ObjectString { get; set; }
            internal bool RequiredQuote { get; set; }
            internal bool RequiredVarDefine { get; set; }
        }


        //Locale, JsonObject is used frequently
        private Dictionary<string, string> _localeDictionary = new Dictionary<string, string>();
        private Dictionary<string, JsonScriptObject> _jsonObjectDictionary = new Dictionary<string, JsonScriptObject>();
        //not very often
        private Dictionary<string, VariableScriptObject> _variableDictionary;
        private Dictionary<string, string> _koBindingDictionary;

        public string GenerateClientUniqueId()
        {
            return StringUtility.GenerateAlphabet(12);
        }

        [Conditional("DEBUG")]
        private void InsertSpaceTag(StringBuilder sb)
        {
           
        }

        #region Add Objects

        public void AddLocale(string name, string localeValue)
        {
            _localeDictionary.Add(name, localeValue);
        }

        public void AddLocale(string name)
        {
            string localValue = string.Empty;
            if (MessageManager.Resources.ContainsKey(name))
            {
                localValue = MessageManager.Resources[name];
            }
            _localeDictionary.Add(name, localValue);
        }

        public void AddVariable(string name, string value, bool requiredQuote = true, bool requireVarDefine = true)
        {
            if (_variableDictionary == null)
            {
                _variableDictionary = new Dictionary<string, VariableScriptObject>();
            }
            _variableDictionary.Add(name, new VariableScriptObject() { ObjectString = value, RequiredQuote = requiredQuote, RequiredVarDefine = requireVarDefine });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="jsonString"></param>
        /// <param name="clientType"></param>
        /// <param name="requireDefinedOnClient">Define var on js</param>
        /// <param name="nameSpace">null: use namespace defined in client viewModel; empty: not use namespace;  otherwise use targetNameSpace</param>
        public void AddJson(string name, IClientModel model, bool requireDefinedOnClient = true,
                                string targetJsonClassName = null, string targetJsonNameSpace = null)
        {
            _jsonObjectDictionary.Add(name,
                new JsonScriptObject()
                {
                    JsonModel = model,
                    RequiredDefineOnClientSide = requireDefinedOnClient//,
                    //TargetJsonNameSpace = targetJsonClassName,
                    // TargetJsonClassName = targetJsonNameSpace
                });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">json object name</param>
        /// <param name="viewIdentifier">view identifier</param>
        public void AddKnockOutBinding(string name, string viewIdentifier)
        {
            if (_koBindingDictionary == null)
            {
                _koBindingDictionary = new Dictionary<string, string>();
            }

            _koBindingDictionary.Add(name, viewIdentifier);
        }

        #endregion

        #region Help Methods

        private void AppendScriptBeginTag(StringBuilder sb)
        {
            sb.Append("<script type=\"text/javascript\">");
        }

        private void AppendScriptEndTag(StringBuilder sb)
        {
            sb.Append("</script>");
        }

        private void AppendScriptDomReadyBeginTag(StringBuilder sb)
        {
            sb.Append("$(document).ready(function () {");
        }

        private void AppendScriptDomReadyEndTag(StringBuilder sb)
        {
            sb.Append("});");
        }

        #endregion

        #region Register Locale

        private void ConcatenateLocale(StringBuilder sb)
        {
            if (sb == null || _localeDictionary == null || _localeDictionary.Count == 0) return;

            foreach (var item in _localeDictionary)
            {
                InsertSpaceTag(sb);
                sb.AppendFormat("{0}[\"{1}\"]=\"{2}\";", ClientModel.DefaultLocaleNameSpace, item.Key, item.Value);
                InsertSpaceTag(sb);
            }
        }

        public IHtmlString RegisterLocaleOnClient(bool requireScriptTag = true)
        {

            if (_localeDictionary == null || _localeDictionary.Count == 0) return new HtmlString(string.Empty);

            StringBuilder sb = new StringBuilder();

            if (requireScriptTag)
            {
                AppendScriptBeginTag(sb);
            }

            ConcatenateLocale(sb);

            if (requireScriptTag)
            {
                AppendScriptEndTag(sb);
            }

            return new HtmlString(sb.ToString());

        }

        public IHtmlString RegisterLocaleOnClientNow(string key, string value)
        {
            StringBuilder sb = new StringBuilder();
            InsertSpaceTag(sb);
            sb.AppendFormat("{0}[\"{1}\"]=\"{2}\";", ClientModel.DefaultLocaleNameSpace, key, value);
            InsertSpaceTag(sb);
            return new HtmlString(sb.ToString());
        }

        #endregion

        #region Register Variable

        private void ConcatenateVariable(StringBuilder sb)
        {
            if (sb == null || _variableDictionary == null || _variableDictionary.Count == 0) return;

            foreach (var item in _variableDictionary)
            {
                InsertSpaceTag(sb);
                if (string.IsNullOrWhiteSpace(item.Value.ObjectString)) continue;
                if (item.Value.RequiredVarDefine) { sb.Append("var "); }
                if (item.Value.RequiredQuote)
                {
                    sb.AppendFormat("{0}='{1}';", item.Key, item.Value.ObjectString);
                }
                else
                {
                    sb.AppendFormat("{0}={1};", item.Key, item.Value.ObjectString);
                }
                InsertSpaceTag(sb);
            }
        }

        public IHtmlString RegisterVariableOnClient(bool requireScriptTag = true)
        {
            if (_variableDictionary != null && _variableDictionary.Count == 0) return new HtmlString(string.Empty);

            StringBuilder sb = new StringBuilder();

            if (requireScriptTag) { AppendScriptBeginTag(sb); }

            ConcatenateVariable(sb);

            if (requireScriptTag) { AppendScriptEndTag(sb); }

            return new HtmlString(sb.ToString());
        }

        public IHtmlString RegisterVariableOnClientNow(string name, string value, bool requireVarDefine = true, bool requiredQuote = true,
                                                      bool requireScriptTag = true, bool requireDomReadyBlock = true)
        {
            StringBuilder sb = new StringBuilder();

            if (requireScriptTag) { AppendScriptBeginTag(sb); }
            if (requireDomReadyBlock) { AppendScriptDomReadyBeginTag(sb); }

            InsertSpaceTag(sb);
            if (requireVarDefine) { sb.Append("var "); }
            if (requiredQuote)
            {
                sb.AppendFormat("{0}='{1}';", name, value);
            }
            else
            {
                sb.AppendFormat("{0}={1};", name, value);
            }
            InsertSpaceTag(sb);

            if (requireDomReadyBlock) { AppendScriptDomReadyEndTag(sb); }
            if (requireScriptTag) { AppendScriptEndTag(sb); }

            return new HtmlString(sb.ToString());
        }

        #endregion

        #region Register Json

        private void ConcatenateJson(StringBuilder sb)
        {
            if (sb == null || _jsonObjectDictionary == null || _jsonObjectDictionary.Count == 0) return;

            foreach (var item in _jsonObjectDictionary)
            {
                InsertSpaceTag(sb);
                if (item.Value.RequiredDefineOnClientSide)
                {
                    sb.Append("var ");
                }

                sb.AppendFormat("{0}=new {1}.{2}({3});", item.Key, item.Value.JsonModel.TargetJsonModelNameSpace,
                                       item.Value.JsonModel.TargetJsonModelName, item.Value.JsonModel.ToJson());

                //string jsonModelNameSpace = item.Value.TargetJsonNameSpace == null ? item.Value.JsonModel.TargetJsonModelNameSpace : item.Value.TargetJsonNameSpace;
                //string jsonModelClassName = string.IsNullOrEmpty(item.Value.TargetJsonClassName) ? item.Value.JsonModel.TargetJsonModelName : item.Value.TargetJsonClassName;

                //if (jsonModelNameSpace == string.Empty) //use namespace defined in ClientModel
                //{
                //    //sb.AppendFormat("{0}=new {1}(JSON.parse('{2}'));", item.Key, jsonModelClassName, item.Value.JsonModel.ToJson()); 
                //    sb.AppendFormat("{0}=new {1}({2});", item.Key, jsonModelClassName, item.Value.JsonModel.ToJson());
                //}
                //else
                //{
                //   sb.AppendFormat("{0}=new {1}.{2}(JSON.parse('{3}'));", item.Key, jsonModelNameSpace, jsonModelClassName, item.Value.JsonModel.ToJson());  
                //}
                InsertSpaceTag(sb);
            }
        }

        public IHtmlString RegisterJsonOnClient(bool requireScriptTag = true)
        {
            if (_jsonObjectDictionary == null || _jsonObjectDictionary.Count == 0) return new HtmlString(string.Empty);

            StringBuilder sb = new StringBuilder();

            if (requireScriptTag)
            {
                AppendScriptBeginTag(sb);
            }

            ConcatenateJson(sb);

            if (requireScriptTag)
            {
                AppendScriptEndTag(sb);
            }

            return new HtmlString(sb.ToString());
        }

        public IHtmlString RegisterJsonOnClientNow(string name, IClientModel clientModel, bool requireVarDefine = true,
                                                     bool requireScriptTag = true, bool requireDomReadyBlock = true)
        {
            StringBuilder sb = new StringBuilder();

            if (requireScriptTag) { AppendScriptBeginTag(sb); }
            if (requireDomReadyBlock) { AppendScriptDomReadyBeginTag(sb); }

            InsertSpaceTag(sb);
            if (requireVarDefine) { sb.Append("var "); }
            sb.AppendFormat("{0}=new {1}.{2}({3});", name, clientModel.TargetJsonModelNameSpace, clientModel.TargetJsonModelName, clientModel.ToJson());
            InsertSpaceTag(sb);

            if (requireDomReadyBlock) { AppendScriptDomReadyEndTag(sb); }
            if (requireScriptTag) { AppendScriptEndTag(sb); }

            return new HtmlString(sb.ToString());
        }

        #endregion

        #region Register KO Binding

        private void ConcatenateKoBinding(StringBuilder sb)
        {
            if (sb == null || _koBindingDictionary == null || _koBindingDictionary.Count == 0) return;

            foreach (var name in _koBindingDictionary.Keys)
            {
                InsertSpaceTag(sb);
                string[] viewIdnetifiers = _koBindingDictionary[name].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var vMark in viewIdnetifiers)
                {
                    sb.AppendFormat("ko.applyBindings({0}, $('{1}')[0]);", name, vMark.Trim());
                }
                InsertSpaceTag(sb);
            }
        }

        public IHtmlString RegisterKoBinding(bool requireDomReadyBlock)
        {
            if (_koBindingDictionary == null || _koBindingDictionary.Count == 0) return new HtmlString(string.Empty);

            StringBuilder sb = new StringBuilder();

            if (requireDomReadyBlock)
            {
                AppendScriptDomReadyBeginTag(sb);
            }

            ConcatenateKoBinding(sb);

            if (requireDomReadyBlock)
            {
                AppendScriptDomReadyEndTag(sb);
            }

            return new HtmlString(sb.ToString());
        }

        public IHtmlString RegisterKoBindingNow(string name, string viewIdentifiers,
                                                    bool requireScriptTag = true, bool requireDomReadyBlock = true)
        {
            StringBuilder sb = new StringBuilder();

            if (requireScriptTag) { AppendScriptBeginTag(sb); }
            if (requireDomReadyBlock) { AppendScriptDomReadyBeginTag(sb); }

            string[] viewMarks = viewIdentifiers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var vMark in viewMarks)
            {
                InsertSpaceTag(sb);
                sb.AppendFormat("ko.applyBindings({0}, $('{1}')[0]);", name, vMark.Trim());
                InsertSpaceTag(sb);
            }

            if (requireDomReadyBlock) { AppendScriptDomReadyEndTag(sb); }
            if (requireScriptTag) { AppendScriptEndTag(sb); }

            return new HtmlString(sb.ToString());
        }

        #endregion

        public IHtmlString RegisterAllScriptOnClient(bool requireScriptTag = true, bool requireDomReadyBlock = true)
        {
            int count = 0;
            if (_variableDictionary != null) { count += _variableDictionary.Count; }
            if (_koBindingDictionary != null) { count += _koBindingDictionary.Count; }
            if (_jsonObjectDictionary != null) { count += _jsonObjectDictionary.Count; }
            if (_localeDictionary != null) { count += _localeDictionary.Count; }
            if (count == 0) return new HtmlString(string.Empty);

            StringBuilder sb = new StringBuilder();

            if (requireScriptTag) { AppendScriptBeginTag(sb); }

            ConcatenateLocale(sb);
            ConcatenateVariable(sb);
            ConcatenateJson(sb);

            if (_koBindingDictionary != null && _koBindingDictionary.Count > 0)
            {
                if (requireDomReadyBlock) { AppendScriptDomReadyBeginTag(sb); }
                ConcatenateKoBinding(sb);
                if (requireDomReadyBlock) { AppendScriptDomReadyEndTag(sb); }
            }

            if (requireScriptTag) { AppendScriptEndTag(sb); }

            return new HtmlString(sb.ToString());
        }

    }
}
