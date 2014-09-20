using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{
    public class MemberUserMapping : BaseEntityTypeConfiguration<MemberUser>
    {
        public MemberUserMapping()
        {
            Property(x => x.UserName).HasMaxLength(64);
            Property(x => x.PasswordSalt).HasMaxLength(64);
            Property(x => x.Password).HasMaxLength(128);
            Property(x => x.PasswordQuestion).HasMaxLength(256);
            Property(x => x.PasswordAnswer).HasMaxLength(64);
            Property(x => x.Signature).HasMaxLength(256);
            
            
            Property(x => x.RegisterEmail).HasMaxLength(64);
            Property(x => x.ContactEmail).HasMaxLength(64);

            Property(x => x.Avatar).HasMaxLength(256);
            Property(x => x.Comment).HasMaxLength(1024);
            Property(x => x.PersonalWebsite).HasMaxLength(256);

            Property(x => x.OpenId).HasMaxLength(128);
            Property(x => x.ExternalAccessId).HasMaxLength(128);
            Property(x => x.ExternalAccessToken).HasMaxLength(128);
            Property(x => x.Latitude).HasMaxLength(128);
            Property(x => x.Longitude).HasMaxLength(128);
            Property(x => x.Location).HasMaxLength(256);

            HasRequired(x => x.Role).WithMany(r => r.Users).HasForeignKey(x => x.RoleId).WillCascadeOnDelete(false);
         
        }
    }
}
