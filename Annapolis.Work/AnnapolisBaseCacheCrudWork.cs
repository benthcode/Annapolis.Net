using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Abstract.Work;
using Annapolis.Entity;
using System.Linq.Expressions;
using Annapolis.Abstract.Repository;
using Annapolis.Shared.Extension;
using Microsoft.Practices.Unity;

namespace Annapolis.Work
{
    public abstract class AnnapolisBaseCacheCrudWork<T> : AnnapolisBaseCrudWork<T>, IAnnapolisBaseCacheCrudWork<T>  where T : BaseEntity, new()
    {
        [Dependency]
        public ICacheWork CacheManager { get; set; }
        protected static string All_CacheItems_Key;
        protected static string All_CacheDictionaryItems_Key;
        

        static AnnapolisBaseCacheCrudWork()
        {
            All_CacheItems_Key = Guid.NewGuid().ToString();
            All_CacheDictionaryItems_Key = Guid.NewGuid().ToString();

        }

        public virtual List<T> AllCacheItems
        {
            get
            {
                if (!CacheManager.Contains(All_CacheItems_Key))
                {
                    CacheManager.AddOrUpdate(All_CacheItems_Key, All.ToList());
                }
                return CacheManager.GetData<List<T>>(All_CacheItems_Key);
            }
        }

        public virtual Dictionary<Guid, T> AllDictionary
        {
            get
            {
                if (!CacheManager.Contains(All_CacheDictionaryItems_Key))
                {
                    CacheManager.AddOrUpdate(All_CacheDictionaryItems_Key, AllCacheItems.ToDictionary(x => x.Id));
                }
                return CacheManager.GetData<Dictionary<Guid, T>>(All_CacheDictionaryItems_Key);
            }
        }

        #region Query

        public override IQueryable<T> Query(Expression<Func<T, bool>> predicate, string orderBy = null, string[] includeSelectors = null)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                return AllCacheItems.AsQueryable().Where(predicate);
            }
            else
            {
                return AllCacheItems.AsQueryable().Where(predicate).OrderBy(orderBy);
            }
        }

        public override IQueryable<T> Query(string predicate, string orderBy = null, string[] includeSelectors = null)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                return AllCacheItems.AsQueryable().Where(predicate);
            }
            else
            {
                return AllCacheItems.AsQueryable().Where(predicate).OrderBy(orderBy);
            }
        }

        #endregion

        public override T Single(Expression<Func<T, bool>> expression, string[] includeSelectors = null)
        {
            return AllCacheItems.AsQueryable().Where(expression).SingleOrDefault();
        }

        public override bool Contains(T item)
        {
            return AllDictionary.ContainsKey(item.Id);
        }

        public override bool Contains(Expression<Func<T, bool>> predicate)
        {
            return AllCacheItems.AsQueryable().Count(predicate) > 0;
        }

        public override T Create()
        {
            return new T();
        }

        public override T Get(Guid id, string[] includeSelectors = null)
        {
            if (AllDictionary.ContainsKey(id)) return AllDictionary[id];
            return null;
        }

        protected override void MarkPersistentDataChanged()
        {
            CacheManager.Flush();
        }


    }
}
