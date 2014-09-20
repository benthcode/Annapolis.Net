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
    
    public partial class ContentTopic
    {
        public ContentTopic()
        {
            this.ContentComments = new HashSet<ContentComment>();
            this.ContentTags = new HashSet<ContentTag>();
        }
    
        public System.Guid Id { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string SubTitle { get; set; }
        public string OriginalSubTitle { get; set; }
        public string TitleUrl { get; set; }
        public string Thumbnail { get; set; }
        public bool IsSticky { get; set; }
        public bool IsLocked { get; set; }
        public System.DateTime CreateTime { get; set; }
        public System.DateTime LastUpdateTime { get; set; }
        public System.Guid ThreadId { get; set; }
        public System.Guid UserId { get; set; }
        public string Key { get; set; }
        public Nullable<System.Guid> FirstCommentId { get; set; }
        public Nullable<System.Guid> LastCommentId { get; set; }
        public bool IsHidden { get; set; }
    
        public virtual ICollection<ContentComment> ContentComments { get; set; }
        public virtual ContentThread ContentThread { get; set; }
        public virtual MemberUser MemberUser { get; set; }
        public virtual ICollection<ContentTag> ContentTags { get; set; }
    }
}