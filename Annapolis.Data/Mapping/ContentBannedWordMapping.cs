using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{
    public class ContentBannedWordMapping : BaseEntityTypeConfiguration<ContentBannedWord>
    {
        public ContentBannedWordMapping()
        {
            Property(x => x.Word).HasMaxLength(256);
        }
    }
}
