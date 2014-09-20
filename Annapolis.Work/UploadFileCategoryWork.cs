using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Abstract.Work;
using Annapolis.Entity;

namespace Annapolis.Work
{
    public class UploadFileCategoryWork : AnnapolisBaseCacheCrudWork<UploadFileCategory>, IUploadFileCategoryWork
    {
        private Dictionary<string, UploadFileCategory> FileCategoryByName
        {
            get
            {
                if (!CacheManager.Contains("UploadFileCategoryService_FileCategoryByName"))
                {
                    var dict = AllCacheItems.ToDictionary(x => x.Name);
                    CacheManager.AddOrUpdate("UploadFileCategoryService_FileCategoryByName", dict);
                }
                return CacheManager.GetData<Dictionary<string, UploadFileCategory>>("UploadFileCategoryService_FileCategoryByName");
            }
        }

        public UploadFileCategory GetFileCategoryByName(string name)
        {
            return FileCategoryByName[name];
        }

    }
}
