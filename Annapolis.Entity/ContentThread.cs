using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Entity
{
    public class ContentThread : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsLocked { get; set; }
        public bool IsHidden { get; set; }

      
        public bool IsAbstract { get; set; }
        public bool IsDefault { get; set; }
        public int SortOrder { get; set; }
        public DateTime DateCreated { get; set; }

        public Guid? ParentThreadId { get; set; }
        public ContentThread ParentThread { get; set; }

        public virtual ICollection<ContentTagCategoryOnThread> TagCategoryMaps { get; set; }
        public virtual ICollection<ContentThread> SubThreads { get; set; }
        public virtual ICollection<ContentTopic> Topics { get; set; }
    }
}
