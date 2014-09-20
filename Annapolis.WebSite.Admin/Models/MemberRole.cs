//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Annapolis.WebSite.Admin.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MemberRole
    {
        public MemberRole()
        {
            this.MemberUsers = new HashSet<MemberUser>();
            this.PermissionOnThreads = new HashSet<PermissionOnThread>();
            this.Settings = new HashSet<Setting>();
        }
    
        public System.Guid Id { get; set; }
        public string RoleName { get; set; }
        public bool IsAdmin { get; set; }
        public string Key { get; set; }
    
        public virtual ICollection<MemberUser> MemberUsers { get; set; }
        public virtual ICollection<PermissionOnThread> PermissionOnThreads { get; set; }
        public virtual ICollection<Setting> Settings { get; set; }
    }
}
