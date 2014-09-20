using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Entity
{
    public class ContentComment : BaseOwnerEntity
    {
        public ContentComment()
        {
            Files = new List<UploadFile>();
        }

        public string Content { get; set; }
        public string OriginalContent { get; set; }

        public bool IsSpam { get; set; }
        public bool IsLocked { get; set; }
        public bool IsHidden { get; set; }
        public bool IsSolution { get; set; }
        public bool IsAttachedToTopic { get; set; }
       
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateTime { get; set; }

        public Guid TopicId { get; set; }
        public virtual ContentTopic Topic { get; set; }
        public virtual ICollection<UploadFile> Files { get; set; }

        public int VoteUpCount { get; set; }
        public int VoteDownCount { get; set; }
        public virtual ICollection<ContentVote> Votes { get; set; }

    }
}
