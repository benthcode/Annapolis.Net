using System;
using System.Collections.Generic;
using Annapolis.Entity;
using Annapolis.WebSite.ClientModels;
using Annapolis.WebSite.Drivers.Base;

namespace Annapolis.WebSite.Drivers.Abstract
{
    public interface IThreadDriver : IEntityConvertableDriver<ContentThread, ThreadClient>,
          ISavableDriver<ContentThread, ThreadClient>
    {
        ContentThread RootThread { get; }

        ContentThread GetThreadByKey(string key);

        List<ContentTagCategoryOnThread> GetTagCategories(Guid threadId);

        List<ContentThread> GetChildThreads(Guid threadId);

    }
}
