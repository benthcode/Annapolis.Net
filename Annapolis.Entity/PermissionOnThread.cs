using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Entity
{
    public class PermissionOnThread : BaseEntity
    {

       
        public bool IsGranted { get; set; }

        public Guid RoleId { get; set; }
        public virtual MemberRole Role { get; set; }

        public Guid ThreadId { get; set; }
        public virtual ContentThread Thread { get; set; }

        public Guid PermissionId { get; set; }
        public virtual Permission Permission { get; set; }
       
    }
}
