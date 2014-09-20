using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;
using Annapolis.Abstract.Repository;
using Annapolis.Abstract.Work;

namespace Annapolis.Work
{
    internal class ServiceHelper
    {
        internal static void Save<T>(IRepository<T> repository, T item) where T : BaseEntity
        {
            if (item.IsNew())
            {
                repository.Add(item);
            }
            else
            {
                repository.Update(item);
            }
        }

        internal static void Delete<T>(IRepository<T> repository, T item) where T : BaseEntity
        {
            repository.Delete(item);
        }

        internal static void ClearCache(ICacheWork cacheManagerWork)
        {
            cacheManagerWork.Flush();
        }

    }
}
