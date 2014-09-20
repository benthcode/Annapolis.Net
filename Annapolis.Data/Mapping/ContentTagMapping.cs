using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{

    public class ContentTagMapping : BaseEntityTypeConfiguration<ContentTag>
    {
        public ContentTagMapping()
        {
            Property(x => x.Name).HasMaxLength(256);
            Property(x => x.Description).HasMaxLength(512);

            //Self Join
            HasOptional(x => x.ParentTag).WithMany(t => t.SubTags).HasForeignKey(x => x.ParentTagId).WillCascadeOnDelete(false);

            //Tag <=> TagCategory
            HasOptional(t => t.Category).WithMany(g => g.Tags).HasForeignKey(t => t.CategoryId).WillCascadeOnDelete(false);

        }
    }
}
