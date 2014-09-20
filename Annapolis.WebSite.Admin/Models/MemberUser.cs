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
    
    public partial class MemberUser
    {
        public MemberUser()
        {
            this.ContentComments = new HashSet<ContentComment>();
            this.ContentTopics = new HashSet<ContentTopic>();
            this.ContentVotes = new HashSet<ContentVote>();
            this.Settings = new HashSet<Setting>();
            this.UploadFiles = new HashSet<UploadFile>();
        }
    
        public System.Guid Id { get; set; }
        public string UserName { get; set; }
        public string RegisterEmail { get; set; }
        public string ContactEmail { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string Token { get; set; }
        public System.DateTime TokenGeneratedTime { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public Nullable<int> FailedPasswordAttemptCount { get; set; }
        public Nullable<int> FailedPasswordAnswerAttemptCount { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
        public Nullable<System.DateTime> LastPasswordChangedDate { get; set; }
        public Nullable<System.DateTime> LastLockoutDate { get; set; }
        public Nullable<System.DateTime> LastActivityDate { get; set; }
        public Nullable<System.DateTime> LoginIdExpiresDate { get; set; }
        public Nullable<System.DateTime> BirthDay { get; set; }
        public string Avatar { get; set; }
        public string Comment { get; set; }
        public string Signature { get; set; }
        public string Location { get; set; }
        public string PersonalWebsite { get; set; }
        public int Credit { get; set; }
        public bool IsExternalAccount { get; set; }
        public string ExternalAccessId { get; set; }
        public string OpenId { get; set; }
        public string ExternalAccessToken { get; set; }
        public bool DisableEmailNotifications { get; set; }
        public bool DisableComment { get; set; }
        public bool DisablePrivateMessages { get; set; }
        public bool DisableFileUploads { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public System.Guid RoleId { get; set; }
        public string Key { get; set; }
    
        public virtual ICollection<ContentComment> ContentComments { get; set; }
        public virtual ICollection<ContentTopic> ContentTopics { get; set; }
        public virtual ICollection<ContentVote> ContentVotes { get; set; }
        public virtual MemberRole MemberRole { get; set; }
        public virtual ICollection<Setting> Settings { get; set; }
        public virtual ICollection<UploadFile> UploadFiles { get; set; }
    }
}
