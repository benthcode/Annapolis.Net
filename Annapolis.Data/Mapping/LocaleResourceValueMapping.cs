using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{
    public class LocaleResourceValueMapping : BaseEntityTypeConfiguration<LocaleResourceValue>
    {
        public LocaleResourceValueMapping()
        {
            Property(x => x.Note).HasMaxLength(255);
            Property(x => x.ResourceValue).HasMaxLength(1024);

            HasRequired(x => x.Language).WithMany(x => x.LocaleValues).HasForeignKey(x => x.LanguageId).WillCascadeOnDelete(false);
            HasRequired(x => x.ResourceKey).WithMany(x => x.ResourceValues).HasForeignKey(x => x.ResourceKeyId).WillCascadeOnDelete(false);
        }
    }
}
