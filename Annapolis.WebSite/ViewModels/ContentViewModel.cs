using Annapolis.Entity;
using Annapolis.Shared.Model;
using PagedList;

namespace Annapolis.WebSite.ViewModels
{
    public class TopicWithComments : ViewModel
    {
        public ContentTopic Topic { get; set; }
        public IPagedList<ContentComment> Comments { get; set; }
    }
}