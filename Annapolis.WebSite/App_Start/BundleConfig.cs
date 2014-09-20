using System.Web.Optimization;

namespace Annapolis.WebSite.App
{
    public class BundleConfig
    {
        
        public static void RegisterBundles(BundleCollection bundles)
        {
            

            bundles.Clear();
            bundles.IgnoreList.Clear();

            string cultureName = WebSiteConfig.DefaultSetting.Language.Culture.ToLower();

       
            var preLibraryScriptBundle = new ScriptBundle("~/bundles/pre-libraryScript").Include(
                           "~/Scripts/library/jquery/jquery-1.7.2.min.js",
                           "~/Scripts/library/amplify/amplify.min.js", 
                           "~/Scripts/library/moment/moment.min.js",
                           "~/Scripts/library/knockout/knockout-3.0.0.js",
                           "~/Scripts/chesapeakebay/cpkb.js"
                         
                );
            if (cultureName != "en-us")
            {
                preLibraryScriptBundle.Include(string.Format("~/Scripts/library/moment/lang/{0}.js", cultureName));

            }
            
            bundles.Add(preLibraryScriptBundle);

            var postLibraryScriptBundle = new ScriptBundle("~/bundles/post-libraryScript").Include(
                           "~/Scripts/library/json/json2.min.js",
                       
                           "~/Scripts/library/jquery/simplePagination/jquery.simplePagination.js",
                           "~/Scripts/library/jquery/scrollUp/jquery.scrollUp.min.js",
                           "~/Scripts/library/noty/jquery.noty.js",
                           "~/Scripts/library/noty/layouts/top.js",
                           "~/Scripts/library/noty/layouts/center.js",
                           "~/Scripts/library/noty/layouts/bottom.js",
                           "~/Scripts/library/noty/themes/default.js",

                           "~/Scripts/library/bootstrap/bootstrap-v2.3.2.min.js",

                           "~/Scripts/library/knockout/knockout.mapping.js",
                           "~/Scripts/library/knockout/knockout.validation.min.js",
                           "~/Scripts/library/knockout/ko.editables.js",
            
                           //chesapeakebay
                           "~/Scripts/chesapeakebay/extend.js",
                           "~/Scripts/chesapeakebay/utility.js",
                           
                           //lang
                            string.Format("~/Scripts/chesapeakebay/lang/{0}.js", cultureName),

                           //model
                           "~/Scripts/chesapeakebay/model/model.js",
                           "~/Scripts/chesapeakebay/model/model.user.js",
                           "~/Scripts/chesapeakebay/model/model.content.js",

                           //site
                           "~/Scripts/chesapeakebay/site/site.js",
                           "~/Scripts/chesapeakebay/site/site.user.js",
                           "~/Scripts/chesapeakebay/site/site.layout.js",  
                           "~/Scripts/chesapeakebay/site/site.content.js"
                        );
            postLibraryScriptBundle.Include("~/Scripts/library/kindeditor/kindeditor-min.js");

            string kindEditorLangFileName;
            switch (cultureName)
            { 
                case "en-us" :
                    kindEditorLangFileName = "en"; break;
                case "zh-cn" :
                    kindEditorLangFileName = "zh-CN"; break;
                default:
                    kindEditorLangFileName = "en"; break;
            }
            postLibraryScriptBundle.Include(string.Format("~/Scripts/library/kindeditor/lang/{0}.js", kindEditorLangFileName));
            bundles.Add(postLibraryScriptBundle);


            bundles.Add(new StyleBundle("~/Content/siteCss")
                                        .Include("~/Content/bootstrap/css/bootstrap-v2.3.2.css")
                                        .Include("~/Content/library/simplePagination/simplePagination.css")
                                        .Include("~/Content/library/scrollUp/css/labs.css")
                                        .Include("~/Content/library/scrollUp/css/themes/image.css")
                                        .Include("~/Content/site.css")
                                        .Include("~/Content/chesapeakebay/cpkb.css"));
          
        }
    }
}