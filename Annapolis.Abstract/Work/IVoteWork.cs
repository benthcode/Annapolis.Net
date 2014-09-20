using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;
using Annapolis.Shared.Model;

namespace Annapolis.Abstract.Work
{
    public interface IVoteWork : IAnnapolisBaseCrudWork<ContentVote>
    {
        OperationStatus VoteUp(Guid commentId);

        OperationStatus VoteDown(Guid commentId);
    }
}
