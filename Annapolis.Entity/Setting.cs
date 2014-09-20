using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace Annapolis.Entity
{
    public class Setting : BaseEntity
    {

        public string Name { get; set; }
        public bool IsDefault { get; set; }

        public string HomePageController { get; set; }
        public string HomePageAction { get; set; }

        //Forum feature
        public string ForumName { get; set; }
        public string ForumUrl { get; set; }
        public bool IsClosed { get; set; }
      
        //Forum Skin
        public string Theme { get; set; }
        public string Skin { get; set; }

        // SMTPHost
        public string SMTPHost { get; set; }
        public int? SMTPPort { get; set; }
        public string SMTPUsername { get; set; }
        public string SMTPPassword { get; set; }
        public bool? SMTPEnableSSL { get; set; }
        
        public string AdminEmailAddress { get; set; }
        public bool EmailSendOnNewUser { get; set; }
        public bool EmailVerifyOnNewUser { get; set; }

        //User
        public bool EnableSocialLogins { get; set; }
        public bool AutoApproveNewMember { get; set; }

        //Content
        public int TopicsPerPage { get; set; }
        public int CommentsPerPage { get; set; }
        public string DefaultTopicThumbnailFile { get; set; }
        
        //File
        public string UploadFileRootPath { get; set; }
      
        public int UpdoadFilePathHashLevel { get; set; }

        public string UploadImageFileExtension { get; set; }
        public int UploadImageFileOrginalMaxByteSize { get; set; }
        public int UploadImageFileMaxByteSize { get; set; }
        public int UploadImageFileMaxWidth { get; set; }
        public int UploadImageFileMaxHeight { get; set; }

        public string UploadDocumentFileExtension { get; set; }
        public int UploadDocumentFileMaxByteSize { get; set; }

        public string UploadFlashFileExtension { get; set; }
        public int UploadFlashFileMaxByteSize { get; set; }

        public string UploadMediaFileExtension { get; set; }
        public int UploadMediaFileMaxByteSize { get; set; }

        public string UploadThumbnailPath { get; set; }
        public int UploadThumbnailFileMaxByteSize { get; set; }
        public int UploadThumbnailWidth { get; set; }
        public int UploadThumbnailHeight { get; set; }
        

        //BannedWord
        public string BannedWordReplaceHolder { get; set; }

        public Guid SuperAdminUserId { get; set; }
        public virtual MemberUser SuperAdminUser { get; set; }

        public Guid NewMemberStartRoleId { get; set; }
        public virtual MemberRole NewMemberStartRole { get; set; }

        public Guid LanguageId { get; set; }
        public virtual LocaleLanguage Language { get; set; }

    }
}
