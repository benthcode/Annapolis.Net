using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Abstract.Work;
using Annapolis.Entity;

namespace Annapolis.Work
{
    public class TagCategoryWork : AnnapolisBaseCacheCrudWork<ContentTagCategory>, ITagCategoryWork
    {
        private Dictionary<string, ContentTagCategory> TagCategoryByName
        {
            get
            {
                if(!CacheManager.Contains("TagCategoryService_TagCategoryByName"))
                {
                    var dict = AllCacheItems.ToDictionary(x => x.Name);
                    CacheManager.AddOrUpdate("TagCategoryService_TagCategoryByName", dict);
                }
                return CacheManager.GetData<Dictionary<string, ContentTagCategory>>("TagCategoryService_TagCategoryByName");
            }
        }

        public ContentTagCategory GetTagCategoryByName(string name)
        {
            return TagCategoryByName[name];
        }
    }
}
