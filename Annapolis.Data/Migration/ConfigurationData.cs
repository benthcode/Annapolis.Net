using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;
using Annapolis.Shared.Model;
using System.Xml;
using System.Xml.Linq;

namespace Annapolis.Data.Migration
{
    internal sealed class ConfigurationData
    {
        internal void AddExternalSites(AnnapolisDbContext context)
        {
          
        }

        internal void AddSettings(AnnapolisDbContext context)
        {
           
            var adminRole = new MemberRole() {  RoleName = "Adminstrator", IsAdmin = true };
            adminRole.GenerateId();
            context.Roles.Add(adminRole);
            

            var standardRole = new MemberRole() { RoleName = "StandardUser", IsAdmin = false };
            standardRole.GenerateId();
            context.Roles.Add(standardRole);

            var adminUser = new MemberUser()
            {
                UserName = "Admin",
                PasswordSalt = "r68KUmM1jVG36xXWE9a4C533ciIV/dFZ",
                Password = "87alJXOYXukJqDOXFhMxEgHskNwc7BXxNEYGKAH4Ozo=",
                CreateDate = DateTime.UtcNow, 
                LastLoginDate = DateTime.UtcNow,
                TokenGeneratedTime = DateTime.UtcNow,
                Token = Guid.NewGuid().ToString(),
                RegisterEmail = "admin@test.com", Role = adminRole 
            };
            adminUser.GenerateId();
            context.Users.Add(adminUser);

            //context.SaveChanges();

            var language_default = context.Languages.Where(x => x.Culture == "en-US").Single();

            var defaultSetting = new Setting()
            {
                Name = "Default",
                Theme = "Ninja",
                Skin = "jquery.ui.flick",
                IsDefault = true,
                Language = language_default,
                NewMemberStartRole = standardRole,
                SuperAdminUser = adminUser,
                AutoApproveNewMember = true,
                EnableSocialLogins = false,
                IsClosed = false,

                TopicsPerPage = 10,
                CommentsPerPage = 10,
                DefaultTopicThumbnailFile = "/Content/chesapeakebay/images/topic_default_thumbnail.png",

                UploadFileRootPath = "/Upload/",
                UpdoadFilePathHashLevel = 4,

                UploadImageFileExtension = "gif,jpg,jpeg,png,bmp",
                UploadImageFileOrginalMaxByteSize = 5242880, //5M
                UploadImageFileMaxByteSize = 204800, //200K
                UploadImageFileMaxWidth = 512,
                UploadImageFileMaxHeight = 768,

                UploadDocumentFileExtension = "pdf,doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2",
                UploadDocumentFileMaxByteSize = 1048576, //1M

                UploadFlashFileExtension = "swf,flv",
                UploadFlashFileMaxByteSize = 1048576, //1M

                UploadMediaFileExtension = "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb",
                UploadMediaFileMaxByteSize = 1048576, //1M

                UploadThumbnailPath = "Thumb/",
                UploadThumbnailFileMaxByteSize = 1048576, //5M
                UploadThumbnailHeight = 128,
                UploadThumbnailWidth = 128,

                BannedWordReplaceHolder = "*",

                EmailVerifyOnNewUser = false,
                EmailSendOnNewUser = false
            };
            defaultSetting.GenerateId();
            context.Settings.Add(defaultSetting);

            var setting = new Setting()
            {
                Name = "Cupertino",
                Theme = "Ninja",
                Skin = "jquery.ui.cupertino",
                IsDefault = false,
                Language = language_default,
                NewMemberStartRole = standardRole,
                SuperAdminUser = adminUser,
                EnableSocialLogins = false,
                IsClosed = false
            };
            setting.GenerateId();
            context.Settings.Add(setting);

            context.SaveChanges();



            var testUser = new MemberUser()
            {
                UserName = "Htaoliu",
                PasswordSalt = "XQMcflD7Y+NwgdlASlsvC2IQRe5fNTJS",
                Password = "AMuoEp1oIUGpF71BD34DjQmoBuAtORfV+qgeMDP4RtQ=",
                IsApproved = true,
                IsLockedOut = false,
                CreateDate = DateTime.UtcNow,
                LastLoginDate = DateTime.UtcNow,
                TokenGeneratedTime = DateTime.UtcNow,
                RegisterEmail = "Htaoliu@test.com",
                Token = Guid.NewGuid().ToString(),
                Role = standardRole
            };
            testUser.GenerateId();
            context.Users.Add(testUser);
            context.SaveChanges();

            testUser = new MemberUser()
            {
                UserName = "frank",
                PasswordSalt = "/KVi5l4QwyEZqiBSEqDo3VBP1b3ssFTH",
                Password = "3bQCXLa7X9qw0XvFMGaKbWOI8satqI/V+OUNcmbH0R0=",
                IsApproved = true,
                IsLockedOut = false,
                CreateDate = DateTime.UtcNow,
                LastLoginDate = DateTime.UtcNow,
                TokenGeneratedTime = DateTime.UtcNow,
                RegisterEmail = "a@test.com",
                Token = Guid.NewGuid().ToString(),
                Role = standardRole
            };
            testUser.GenerateId();
            context.Users.Add(testUser);
            context.SaveChanges();


            testUser = new MemberUser()
            {
                UserName = "rebecca",
                PasswordSalt = "dL4VMJRaAmiRmM2TeGLjw2HKgJevJn0K",
                Password = "96cnojho85jOb7m8vGrE4UFZpuHd2eg97ZqlXAy8iE0=",
                IsApproved = true,
                IsLockedOut = false,
                CreateDate = DateTime.UtcNow,
                LastLoginDate = DateTime.UtcNow,
                TokenGeneratedTime = DateTime.UtcNow,
                RegisterEmail = "b@test.com",
                Token = Guid.NewGuid().ToString(),
                Role = standardRole
            };
            testUser.GenerateId();
            context.Users.Add(testUser);
            context.SaveChanges();

            //ContentTopic topic = new ContentTopic();
            //topic.Id = Guid.NewGuid();
            //topic.LastUpdateTime = DateTime.Now;

            //context.Topics.Add(topic);
            //context.SaveChanges();

            //ContentComment post = new ContentComment();
            //post.Id = Guid.NewGuid();
            //post.CreateTime = DateTime.Now;
            //post.LastUpdateTime = DateTime.Now;
            //post.User = adminUser;

            //post.Topic = topic;


            //context.Comments.Add(post);
            //context.SaveChanges();
        }

        
    }
}
