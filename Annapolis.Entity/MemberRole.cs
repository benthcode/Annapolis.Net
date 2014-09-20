using System;
using System.Collections.Generic;

namespace Annapolis.Entity
{
    public class MemberRole : BaseEntity
    {
        public string RoleName { get; set; }
        public bool IsAdmin { get; set; }
        public virtual ICollection<MemberUser> Users { get; set; }
        public virtual ICollection<Setting> Settings { get; set; }
    }
}
