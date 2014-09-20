using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Entity
{
    public class ContentTag : BaseEntity
    {
        public ContentTag()
        {
            Topics = new List<ContentTopic>();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public int SortOrder { get; set; }
        public bool IsHot { get; set; }
        public bool IsLocked { get; set; }
        public bool IsHidden { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime? LastUpdateTime { get; set; }

        public Guid? CategoryId { get; set; }
        public ContentTagCategory Category { get; set; }

        public Guid? ParentTagId { get; set; }
        public ContentTag ParentTag { get; set; }

        public ICollection<ContentTopic> Topics { get; set; }
        public ICollection<ContentTag> SubTags { get; set; }
    }
}
