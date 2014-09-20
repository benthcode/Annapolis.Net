using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{
    public class ContentCommentMapping : BaseOwnerEntityTypeConfiguration<ContentComment>
    {
        public ContentCommentMapping()
        {
            Property(p => p.Content).HasMaxLength(8192);
            Property(p => p.OriginalContent).HasMaxLength(8192);

            HasRequired(p => p.Topic).WithMany(t => t.Comments).HasForeignKey(p => p.TopicId).WillCascadeOnDelete();

            //Post => User
            HasRequired(p=> p.User).WithMany(u => u.Comments).HasForeignKey(p => p.UserId).WillCascadeOnDelete(false);

            //Post <= File
            HasMany(x => x.Files).WithOptional().HasForeignKey(f => f.AttachId).WillCascadeOnDelete(false);
            
            //Post <= ContentVote
            HasMany(x => x.Votes).WithRequired(v => v.Comment).HasForeignKey(v => v.CommentId).WillCascadeOnDelete(false);
        }
    }
}
