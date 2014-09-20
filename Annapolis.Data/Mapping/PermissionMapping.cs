using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Data.Mapping;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{
    public class PermissionMapping : BaseEntityTypeConfiguration<Permission>
    {
        public PermissionMapping()
        {
            Property(x => x.Name).HasMaxLength(64);
            Property(x => x.Description).HasMaxLength(256);
        }
    }
}
