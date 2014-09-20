using System.Collections.Generic;
using Annapolis.WebSite.Admin.Models;

namespace Annapolis.WebSite.Admin.ViewModels
{
    public class TopicViewModel
    {
        public ContentTopic Topic { get; set; }
        public ContentComment Post { get; set; }
    }

    public class TopicCommentsViewModel
    {
        public TopicCommentsViewModel() { }

        public ContentTopic Topic { get; set; }
        public ContentComment Post { get; set; }
        public List<ContentComment> Comments { get; set; }
    }
}