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
    
    public partial class ContentTagCategoryOnThread
    {
        public System.Guid Id { get; set; }
        public System.Guid TagCategoryId { get; set; }
        public System.Guid ThreadId { get; set; }
        public bool OnlyShowHotTag { get; set; }
        public bool IncludeAll { get; set; }
        public bool IncludeOther { get; set; }
        public bool IsTicked { get; set; }
        public string Key { get; set; }
    
        public virtual ContentTagCategory ContentTagCategory { get; set; }
        public virtual ContentThread ContentThread { get; set; }
    }
}