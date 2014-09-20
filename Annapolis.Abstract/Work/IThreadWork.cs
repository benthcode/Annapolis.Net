using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Abstract.Work
{
    public interface IThreadWork : IAnnapolisBaseCacheCrudWork<ContentThread>
    {
        ContentThread RootThread { get; }
        int TotalThreadCount { get; }

        ContentThread GetThread(string key);
        ContentThread GetThread(Guid id);

        List<ContentThread> GetSubThreads(Guid threadId, int topLevel = 0, int bottomLevel = int.MaxValue);
        
        List<ContentTagCategoryOnThread> GetTagCategories(Guid threadId);

    }
}
