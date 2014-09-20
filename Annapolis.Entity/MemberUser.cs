using System;
using System.Linq;
using System.Collections.Generic;

namespace Annapolis.Entity   
{

    public class MemberUser : BaseEntity
    {
        
        public string UserName { get; set; }
        public string RegisterEmail { get; set; }
        public string ContactEmail { get; set; }

        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string Token { get; set; }
        public DateTime TokenGeneratedTime { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public int? FailedPasswordAttemptCount { get; set; }
        public int? FailedPasswordAnswerAttemptCount { get; set; }
 
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public DateTime? LastLockoutDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LoginIdExpiresDate { get; set; }
        public DateTime? BirthDay { get; set; }

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

        public Guid RoleId { get; set; }
        public virtual MemberRole Role { get; set; }

        public virtual ICollection<Setting> Settings { get; set; }

        public virtual ICollection<ContentTopic> Topics { get; set; }
        public virtual ICollection<ContentComment> Comments { get; set; }
        public virtual ICollection<UploadFile> Files { get; set; }
        public virtual ICollection<ContentVote> Votes { get; set; }
       
    }
}
