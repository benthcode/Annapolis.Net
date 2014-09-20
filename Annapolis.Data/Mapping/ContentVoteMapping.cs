using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{
    public class ContentVoteMapping : BaseOwnerEntityTypeConfiguration<ContentVote>
    {
        public ContentVoteMapping()
        {
            HasRequired(x => x.User).WithMany(u => u.Votes).HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
        }
    }
}
