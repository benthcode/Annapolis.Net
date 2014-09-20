using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Entity
{
    public class ContentTagCategoryOnThread : BaseEntity
    {
        public Guid TagCategoryId { get; set; }
        public ContentTagCategory TagCategory { get; set; }
        public Guid ThreadId { get; set; }
        public ContentThread Thread { get; set; }

        public bool OnlyShowHotTag { get; set; }
        public bool IncludeAll { get; set; }
        public bool IncludeOther { get; set; }

        public bool IsTicked { get; set; }

    }
}
