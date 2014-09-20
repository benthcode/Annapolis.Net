using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{
    public class ContentThreadMapping : BaseEntityTypeConfiguration<ContentThread>
    {
        public ContentThreadMapping()
        {
            Property(x => x.Name).HasMaxLength(512);
            Property(x => x.Description).HasMaxLength(1024);
            Property(t => t.Key).HasMaxLength(64);

            //Self Join
            HasOptional(x => x.ParentThread).WithMany(t => t.SubThreads).HasForeignKey(t => t.ParentThreadId).WillCascadeOnDelete(false);

            //Thread <= Topic
            HasMany(x => x.Topics).WithRequired(t => t.Thread).HasForeignKey(t => t.ThreadId).WillCascadeOnDelete(false) ;

            //Thread <= TagThreadMap
            HasMany(x => x.TagCategoryMaps).WithRequired(m => m.Thread).HasForeignKey(m => m.ThreadId).WillCascadeOnDelete(false);
        }
    }
}
