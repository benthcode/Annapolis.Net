using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{
    public class LocaleLanguageMapping : BaseEntityTypeConfiguration<LocaleLanguage>
    {
        public LocaleLanguageMapping()
        {
            Property(x => x.Culture).HasMaxLength(16);
            Property(x => x.LanguageImageFileName).HasMaxLength(255);
            Property(x => x.Name).HasMaxLength(64);

        }
    }
}
