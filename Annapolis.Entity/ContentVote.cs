using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Entity
{
    public class ContentVote : BaseOwnerEntity
    {
        public short Amount { get; set; }
        public DateTime VoteDate { get; set; }

        public Guid CommentId { get; set; }
        public virtual ContentComment Comment { get; set; }

    }
}
