using System.Collections.Generic;
using Annapolis.WebSite.Admin.Models;

namespace Annapolis.WebSite.Admin.ViewModels
{
    public class AdminNotificationViewModel
    {
        public List<MemberUser> NewUsers { get; set; }
        public List<ContentTopic> NewTopics { get; set; }
        public List<ContentComment> NewComments { get; set; }
    }
}