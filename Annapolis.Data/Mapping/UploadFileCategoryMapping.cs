using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{
    public class UploadFileCategoryMapping : BaseEntityTypeConfiguration<UploadFileCategory>
    {
        public UploadFileCategoryMapping()
        {
            Property(x => x.Name).HasMaxLength(128);
            Property(x => x.Description).HasMaxLength(256);
        }
    }
}
