using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Entity
{
    public class ContentTopic : BaseOwnerEntity
    {
        public ContentTopic()
        {
            Tags = new List<ContentTag>();
            Comments = new List<ContentComment>();
        }

        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string SubTitle { get; set; }
        public string OriginalSubTitle { get; set; }
        public string TitleUrl { get; set; }

        public string Thumbnail { get; set; }
       
        public bool IsSticky { get; set; }
        public bool IsLocked { get; set; }
        public bool IsHidden { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateTime { get; set; }

        public Guid ThreadId { get; set; }
        public ContentThread Thread { get; set; }

        public virtual ContentComment FirstComment { get; set; }
        public virtual ContentComment LastComment { get; set; }

        public virtual ICollection<ContentComment> Comments { get; set; }
        public virtual ICollection<ContentTag> Tags { get; set; }
      
    }
}
