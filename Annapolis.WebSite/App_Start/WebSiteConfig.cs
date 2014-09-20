using Annapolis.Abstract.UnitOfWork;
using Annapolis.Abstract.Work;
using Annapolis.Entity;
using Annapolis.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Annapolis.WebSite.App
{
    public static class WebSiteConfig
    {

        private static Setting _defaultSetting;
        private static Dictionary<string, string> _localeResources;

        private static void LoadDefaultSetting()
        {
            using (IUnitOfWorkManager manager = DependencyResolver.Current.GetService<IUnitOfWorkManager>())
            {
                ISettingWork settingWork = DependencyResolver.Current.GetService<ISettingWork>();
                _defaultSetting = settingWork.GetDefaultSetting();
                if (_defaultSetting == null)
                {
                    throw new Exception("Default setting is required!");
                }
            }
        }


        public static Setting DefaultSetting
        {
            get { return _defaultSetting; }
        }

        public static Dictionary<string, string> LocaleResources
        {
            get { return _localeResources; }
        }

        private static string _layoutFileLocation;

        public static string LayoutFileLocation
        {
            get { return _layoutFileLocation; }
        }

        public static void Load()
        {
            LoadDefaultSetting();
            _localeResources = MessageManager.Resources;

            LoadLayout();
        }

        private static void LoadLayout()
        {
            string layoutDirectory = HttpContext.Current.Server.MapPath(string.Format("/Themes/{0}/layout", WebSiteConfig.DefaultSetting.Theme));
            DirectoryInfo directoryInfo = new DirectoryInfo(layoutDirectory);
            var fileName = directoryInfo.GetFiles("*.cshtml").FirstOrDefault();
            if (fileName != null)
            {
                _layoutFileLocation = string.Format("~/Themes/{0}/layout/{1}", WebSiteConfig.DefaultSetting.Theme, fileName);
            }
        }

        public static List<string> GetThemes()
        {
            string themeDirectory = HttpContext.Current.Server.MapPath("/Themes");
            DirectoryInfo directoryInfo = new DirectoryInfo(themeDirectory);
            var topSubDirectories = directoryInfo.GetDirectories().Select(d => d.Name);
            return topSubDirectories.ToList();
        }

        public static List<string> GetSkins(string theme)
        {
            string skinDirectory = HttpContext.Current.Server.MapPath(string.Format("/Themes/{0}/skin", theme));
            DirectoryInfo directoryInfo = new DirectoryInfo(skinDirectory);
            var topSubDirectories = directoryInfo.GetDirectories().Select(d => d.Name);
            return topSubDirectories.ToList();
        }

    }
}