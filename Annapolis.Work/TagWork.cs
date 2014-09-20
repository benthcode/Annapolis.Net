using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Annapolis.Abstract.Work;
using Annapolis.Entity;

namespace Annapolis.Work
{
   
    public class TagWork : AnnapolisBaseCacheCrudWork<ContentTag>, ITagWork
    {
        public override IQueryable<ContentTag> All
        {
            get { return Repository.All.OrderBy(x => x.SortOrder); }
        }

        private Dictionary<Guid, ContentTag> RootTags
        {
            get
            {
                if (!CacheManager.Contains("TagService_RootTags"))
                {
                    var dict = AllCacheItems.Where(x => !x.ParentTagId.HasValue && x.CategoryId.HasValue).ToDictionary(x => x.CategoryId.Value);
                    CacheManager.AddOrUpdate("TagService_RootTags", dict);
                }
                return CacheManager.GetData<Dictionary<Guid, ContentTag>>("TagService_RootTags");
            }
        }

        public List<ContentTag> GetSubTags(Guid tagId, int topLevel = 0, int bottomLevel = int.MaxValue)
        {
            if (tagId == Guid.Empty || !AllDictionary.ContainsKey(tagId)) return null;
            ContentTag parentTag = AllDictionary[tagId];
            if (parentTag == null) return null;
            if (topLevel < 0 || bottomLevel < 0 || topLevel > bottomLevel) return null;

            List<ContentTag> childTags = new List<ContentTag>();
            Queue<ContentTag> currentTags = new Queue<ContentTag>();

            currentTags.Enqueue(parentTag);
            currentTags.Enqueue(null);
            int currentLevel = 0;

            while (currentTags.Count > 0)
            {
                ContentTag tag = currentTags.Dequeue();
                if (tag == null)
                {
                    currentLevel++;
                    if (currentLevel > bottomLevel || currentTags.Count == 0 || currentTags.Last() == null) break;
                    currentTags.Enqueue(null);
                    continue;
                }

                if (currentLevel >= topLevel && currentLevel <= bottomLevel)
                {
                    childTags.Add(tag);
                }
                if (tag.SubTags != null && tag.SubTags.Count > 0)
                {
                    foreach (var subTag in tag.SubTags)
                    {
                        currentTags.Enqueue(subTag);
                    }
                }
            }

            return childTags;
        }

        public List<ContentTag> GetTagsByCategory(ContentTagCategory tagCategory, int depth = int.MaxValue)
        {
            ContentTag tag = RootTags[tagCategory.Id];
            if (tag == null) return null;
            return GetSubTags(tag.Id, 0, depth);
        }
    }
}
