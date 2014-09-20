using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{
    public class PermissionOnThreadMapping : BaseEntityTypeConfiguration<PermissionOnThread>
    {
        public PermissionOnThreadMapping()
        {
            HasRequired(x => x.Role).WithMany().HasForeignKey(x => x.RoleId).WillCascadeOnDelete(false);
            HasRequired(x => x.Permission).WithMany().HasForeignKey(x => x.PermissionId).WillCascadeOnDelete(false);
            HasRequired(x => x.Thread).WithMany().HasForeignKey(x => x.ThreadId).WillCascadeOnDelete(false);
        }
    }
}
