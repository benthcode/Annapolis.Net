using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{
    public class ContentTopicMapping : BaseOwnerEntityTypeConfiguration<ContentTopic>
    {
        public ContentTopicMapping()
        {
            Property(t => t.Title).HasMaxLength(512);
            Property(t => t.OriginalTitle).HasMaxLength(512);
            Property(t => t.SubTitle).HasMaxLength(1024);
            Property(t => t.OriginalSubTitle).HasMaxLength(1024);
            Property(t => t.Thumbnail).HasMaxLength(512);

            //Topic => User
            HasRequired(t => t.User).WithMany(u => u.Topics).HasForeignKey(t => t.UserId).WillCascadeOnDelete(false);

            //Topic <=> Tag
            HasMany(x => x.Tags).WithMany(t => t.Topics).Map(m =>
                                                                {
                                                                    m.MapLeftKey("TopicId");
                                                                    m.MapRightKey("TagId");
                                                                    m.ToTable("Content_MapTopicTag");
                                                                }
                                                            );

            //FirstComment
            HasOptional(x => x.FirstComment).WithOptionalDependent().Map(m => m.MapKey("FirstCommentId"));
            //LastPost
            HasOptional(x => x.LastComment).WithOptionalDependent().Map(m => m.MapKey("LastCommentId"));

          
        }
    }
}
