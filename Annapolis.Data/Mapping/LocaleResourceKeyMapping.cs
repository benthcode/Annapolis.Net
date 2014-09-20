using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{
    public class LocaleResourceKeyMapping : BaseEntityTypeConfiguration<LocaleResourceKey>
    {
        public LocaleResourceKeyMapping()
        {
            Property(x => x.ResourceKey).HasMaxLength(255);
            Property(x => x.Note).HasMaxLength(255);
        
        }
    }
}
