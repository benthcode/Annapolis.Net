using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;
using Annapolis.Abstract.Work;
using System.Data.Entity;

namespace Annapolis.Work
{
    public class ThreadWork : AnnapolisBaseCacheCrudWork<ContentThread>, IThreadWork
    {
        private readonly ITagCategoryWork _tagCategoryWork;

        public ThreadWork(ITagCategoryWork tagCategoryWork)
        {
            _tagCategoryWork = tagCategoryWork;
        }

        public override IQueryable<ContentThread> All
        {
            get
            {
                return base.All.Include(x => x.TagCategoryMaps);
            }
        }

        private Dictionary<string, ContentThread> AliasDictionary
        {
            get
            {
                if (!CacheManager.Contains("ThreadService_AliasDictionary"))
                {
                    var dict = AllCacheItems.Where(x => x.Key != null).ToDictionary(x => x.Key);
                    CacheManager.AddOrUpdate("ThreadService_AliasDictionary", dict);
                }
                return CacheManager.GetData<Dictionary<string, ContentThread>>("ThreadService_AliasDictionary");
            }
        }

        public ContentThread RootThread
        {
            get
            {
                if (!CacheManager.Contains("ThreadService_RootThread"))
                {
                    var dict = AllCacheItems.Where(x => !x.ParentThreadId.HasValue).SingleOrDefault();
                    CacheManager.AddOrUpdate("ThreadService_RootThread", dict);
                }
                return CacheManager.GetData<ContentThread>("ThreadService_RootThread");
            }
        }

        public int TotalThreadCount
        {
            get { return AllCacheItems.Count; }
        }

        public ContentThread GetThread(string key)
        {
            if (key == null) return null;
            if (AliasDictionary.ContainsKey(key))
            {
                return AliasDictionary[key];
            }
            return null;
        }

        public ContentThread GetThread(Guid id)
        {
            if (AllDictionary.ContainsKey(id))
            {
                return AllDictionary[id];
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadId"></param>
        /// <param name="topLevel">0 for itself, 1 for its direct children</param>
        /// <param name="bottomLevel"></param>
        /// <returns></returns>
        public List<ContentThread> GetSubThreads(Guid threadId, int topLevel = 0, int bottomLevel = int.MaxValue)
        {
            if (threadId == Guid.Empty || !AllDictionary.ContainsKey(threadId)) return null;
            ContentThread parentThread = AllDictionary[threadId];
            if (parentThread == null) return null;
            if (topLevel<0 || bottomLevel<0 || topLevel > bottomLevel) return null;

            List<ContentThread> childThreads = new List<ContentThread>();
            Queue<ContentThread> currentThreads = new Queue<ContentThread>();

            currentThreads.Enqueue(parentThread);
            currentThreads.Enqueue(null);
            int currentLevel = 0;

            while (currentThreads.Count > 0)
            {
                ContentThread currentThread = currentThreads.Dequeue();
                if (currentThread == null)
                {
                    currentLevel++;
                    if (currentLevel > bottomLevel || currentThreads.Count == 0 || currentThreads.Last() == null) break;
                    currentThreads.Enqueue(null);
                    continue;
                }

                if (currentLevel >= topLevel && currentLevel <= bottomLevel)
                {
                    childThreads.Add(currentThread);
                }
                if (currentThread.SubThreads != null && currentThread.SubThreads.Count > 0)
                {
                    foreach (var subTag in currentThread.SubThreads)
                    {
                        currentThreads.Enqueue(subTag);
                    }
                }
            }

            return childThreads;
        }

        public List<ContentTagCategoryOnThread> GetTagCategories(Guid threadId)
        {
            ContentThread thread = Get(threadId);
            if (thread == null) return null;


            foreach (var map in thread.TagCategoryMaps)
            {
                map.TagCategory = _tagCategoryWork.AllDictionary[map.TagCategoryId];
            }

            return thread.TagCategoryMaps.ToList();
        }

    }
}
