using System.Text;
using System.Web;
using System.Web.Mvc;
using Annapolis.Shared.Utility;
using Annapolis.Web.Script;
using Annapolis.Web.Client;

namespace Annapolis.Web.Extension
{
    public static class Extensions
    {
       
        public delegate string MvcCacheCallback(HttpContextBase context);

        public static object Substitute(this HtmlHelper html, MvcCacheCallback cb)
        {
            html.ViewContext.HttpContext.Response.WriteSubstitution(
                c => cb(html.ViewContext.HttpContext)
                );
            return null;
        }

        public static MvcHtmlString KoRadioButtonList(this HtmlHelper htmlHelper, ISelectableListClient selectModel)
        {
            string groupName = StringUtility.GenerateAlphabet(12);
            string divBinderName = StringUtility.GenerateAlphabet(12);
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<div id='{0}'>", divBinderName);
            sb.AppendFormat("<div  data-bind='foreach: models'>");
          
            sb.AppendFormat("<input type='radio' data-bind='value: uniqueId, checked: $parent.selectedValue, attr: {{id: uniqueId}}' name='{0}' /><label data-bind='text: text, uniqueFor: uniqueId' />",
                          groupName);
            sb.Append("</div></div>");
           
            var scriptView = htmlHelper.ViewDataContainer as IScriptView;

            if (scriptView != null)
            {
                string koUnqiueId = StringUtility.GenerateAlphabet(12);
                scriptView.ViewScriptRegister.AddJson(koUnqiueId, selectModel);
                scriptView.ViewScriptRegister.AddKnockOutBinding(koUnqiueId, "#" + divBinderName);
                selectModel.UniqueId = koUnqiueId;
            }
            selectModel.MultiSelect = false;

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString KoRadioButtonSetList(this HtmlHelper htmlHelper, ISelectableListClient selectModel)
        {
            string groupName = StringUtility.GenerateAlphabet(12);
            string divBinderName = StringUtility.GenerateAlphabet(12);
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<div id='{0}'>", divBinderName);
            sb.Append("<div data-bind='foreach: models' class='btn-group'>");
            sb.AppendFormat("<button type='button' data-bind='text: text, value:text, buttonSetValue: uniqueId, rootElementId:\"{0}\"' class='btn'></button>", divBinderName);
            sb.Append("</div></div>");
           
            var scriptView = htmlHelper.ViewDataContainer as IScriptView;

            if (scriptView != null)
            {
                string koUnqiueId = StringUtility.GenerateAlphabet(12);
                scriptView.ViewScriptRegister.AddJson(koUnqiueId, selectModel);
                scriptView.ViewScriptRegister.AddKnockOutBinding(koUnqiueId, "#" + divBinderName);
                selectModel.UniqueId = koUnqiueId;
            }
            selectModel.MultiSelect = false;

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString KoCheckBoxList(this HtmlHelper htmlHelper, ISelectableListClient selectModel)
        {
            string groupName = StringUtility.GenerateAlphabet(12);
            string divBinderName = StringUtility.GenerateAlphabet(12);
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<div id='{0}'>", divBinderName);
            sb.AppendFormat("<div  data-bind='foreach: models'>");
            sb.AppendFormat("<input type='checkbox' data-bind='value: uniqueId, checked: $parent.selectedValue, attr: {{id: uniqueId}}' name='{0}' /><span data-bind='text: text'></span>",
                            groupName);
            sb.Append("</div></div>");
            var scriptView = htmlHelper.ViewDataContainer as IScriptView;
            

            if (scriptView != null)
            {
                string koUnqiueId = StringUtility.GenerateAlphabet(12);
                scriptView.ViewScriptRegister.AddJson(koUnqiueId, selectModel);
                scriptView.ViewScriptRegister.AddKnockOutBinding(koUnqiueId, "#" + divBinderName);
                selectModel.UniqueId = koUnqiueId;
            }
            selectModel.MultiSelect = true;

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString KoDropDownList(this HtmlHelper htmlHelper, ISelectableListClient selectModel, bool multiSelect = false)
        {
           
            string divBinderName = StringUtility.GenerateAlphabet(12);
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<div id='{0}'>", divBinderName);
            sb.Append("<select");
            if (multiSelect)
            {
                sb.Append(" multiple='multiple'");    
            }
            sb.Append(" data-bind=\"options:models, optionsText:'text', optionsValue:'uniqueId', value: selectedValue \"></select></div>");
            var scriptView = htmlHelper.ViewDataContainer as IScriptView;

            if (scriptView != null)
            {
                string koUnqiueId = StringUtility.GenerateAlphabet(12);
                scriptView.ViewScriptRegister.AddJson(koUnqiueId, selectModel);
                scriptView.ViewScriptRegister.AddKnockOutBinding(koUnqiueId, "#" + divBinderName);
                selectModel.UniqueId = koUnqiueId;
            }
            selectModel.MultiSelect = multiSelect;

            return new MvcHtmlString(sb.ToString());
        } 
    }
}