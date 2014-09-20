using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{
    public class UploadFileMapping : BaseOwnerEntityTypeConfiguration<UploadFile>
    {
        public UploadFileMapping()
        {
            Property(x => x.FilePath).HasMaxLength(256);
           
            HasRequired(x => x.User).WithMany(u => u.Files).HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
            HasRequired(x => x.Category).WithMany(g => g.Files).HasForeignKey(x => x.CategoryId).WillCascadeOnDelete(false);
        }
    }
}
