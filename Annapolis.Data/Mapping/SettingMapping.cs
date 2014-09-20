using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{
    public class SettingMapping : BaseEntityTypeConfiguration<Setting>
    {
        public SettingMapping()
        {
            Property(x => x.Name).IsRequired().HasMaxLength(16);
            Property(x => x.DefaultTopicThumbnailFile).HasMaxLength(1024);

            HasRequired(x => x.Language).WithMany(l => l.Settings).HasForeignKey(x => x.LanguageId).WillCascadeOnDelete(false); //.Map(m => m.MapKey("LanguageId"));
            HasRequired(x => x.NewMemberStartRole).WithMany(r => r.Settings).HasForeignKey(x => x.NewMemberStartRoleId).WillCascadeOnDelete(false); //Map(m => m.MapKey("NewMemberStartRoleId"));
            HasRequired(x => x.SuperAdminUser).WithMany(r => r.Settings).HasForeignKey(x => x.SuperAdminUserId).WillCascadeOnDelete(false);
            
        }
    }
}
