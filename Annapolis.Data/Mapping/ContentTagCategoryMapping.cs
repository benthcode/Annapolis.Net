using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{

    public class ContentTagCategoryMapping : BaseEntityTypeConfiguration<ContentTagCategory>
    {
        public ContentTagCategoryMapping()
        {
            Property(x => x.Name).HasMaxLength(128);
            Property(x => x.Description).HasMaxLength(256);

            //TagCategory <= TagThreadMap
            HasMany(x => x.ThreadMaps).WithRequired(m => m.TagCategory).HasForeignKey(m => m.TagCategoryId).WillCascadeOnDelete(false);
        }
    }
}
