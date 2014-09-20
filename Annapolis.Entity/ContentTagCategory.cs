using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Entity
{
    public class ContentTagCategory : BaseEntity
    {
        public string Name { get; set; } 
        public string Description { get; set; }
        public int SortOrder { get; set; }

        public bool IsLocked { get; set; }
        public bool IsHidden { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateTime { get; set; }

        public ICollection<ContentTag> Tags { get; set; }
        public ICollection<ContentTagCategoryOnThread> ThreadMaps { get; set; }

    }
}
