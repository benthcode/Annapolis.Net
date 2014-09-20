using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{
    public class MemberRoleMapping : BaseEntityTypeConfiguration<MemberRole>
    {
        public MemberRoleMapping()
        {
            Property(x => x.RoleName).HasMaxLength(64);
  
        }
    }
}
