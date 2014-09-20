using System;
using System.Collections.Generic;
using System.Linq;
using Annapolis.Abstract.Work;
using Annapolis.Entity;
using Annapolis.WebSite.ClientModels;
using Annapolis.WebSite.Drivers.Abstract;
using Annapolis.WebSite.Drivers.Base;

namespace Annapolis.WebSite.Drivers
{
    public class ThreadDriver : SavableEntityDriver<ContentThread, ThreadClient>, IThreadDriver
    {
        private readonly IThreadWork _threadServic;

        public ThreadDriver(IThreadWork threadWork)
            : base(threadWork)
        {
            _threadServic = threadWork;
        }

        public override ThreadClient ToClient(ContentThread entity, ThreadClient c = null, string[] excludeProperties = null, bool serverStatus = true)
        {
            if (entity == null) return null;

            ThreadClient tagClient = base.ToClient(entity, c, excludeProperties, serverStatus);

            tagClient.Text = entity.Name;
            tagClient.UniqueId = entity.Id.ToString();
            return tagClient;
        }

        public override ContentThread FromClient(ContentThread entity, ThreadClient c, string[] includeProperties = null)
        {
            if (entity == null) return null;

            entity.Name = c.Text;

            return entity;
        }


        public ContentThread RootThread
        {
            get { return _threadServic.RootThread; }
        }

        public List<ContentTagCategoryOnThread> GetTagCategories(Guid threadId)
        {
            return _threadServic.GetTagCategories(threadId);
        }

        public ContentThread GetThreadByKey(string key)
        {
            return _threadServic.GetThread(key);
        }

        /// <summary>
        /// child threads are the direct sub threads, and not include itself
        /// </summary>
        /// <param name="threadId"></param>
        /// <returns></returns>
        public List<ContentThread> GetChildThreads(Guid threadId)
        {
            if (_threadServic.AllCacheItems.Count <= 1) { return null; }
            List<ContentThread> contentThreads = _threadServic.GetSubThreads(threadId, 1, 1);
            var threads = contentThreads.Where(x => x.IsHidden).ToList();
            return threads;
        }

    }
}
