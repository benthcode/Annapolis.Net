using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;
using Annapolis.Abstract.Work;
using Annapolis.Shared.Model;

namespace Annapolis.Work
{
    public class VoteWork : AnnapolisBaseOwnerCrudWork<ContentVote>, IVoteWork
    {
        private ICommentWork _commentWork;

        public VoteWork(ICommentWork commentWork)
        {
            _commentWork = commentWork;
        }

        public override ContentVote Create()
        {
            var vote = base.Create();
            vote.VoteDate = DateTime.UtcNow;
            return vote;
        }

        private OperationStatus MakeVote(Guid commentId, short amount)
        {
            if (!Security.IsCurrentUserValid()) return OperationStatus.NotValidUser;
            var comment = _commentWork.Get(commentId, new string[] {"Topic"});
            if (comment == null) return OperationStatus.GenericError;
            if (comment.UserId == Security.CurrentUser.UserId) return OperationStatus.VoteCannotForYourSelf;
            var vote = Single(x => x.CommentId == commentId && x.UserId == Security.CurrentUser.UserId);
            if (vote != null)
            {
                return OperationStatus.VoteHasExisted;
            }
            if (comment.IsLocked || comment.Topic.IsLocked)
            {
                return OperationStatus.CommentHasLocked;
            }
            if (comment.IsHidden || comment.Topic.IsHidden)
            {
                return OperationStatus.CommentHasHidden;
            }

            using (var worker = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    if (amount > 0)
                    {
                        comment.VoteUpCount += amount;
                    }
                    else
                    {
                        comment.VoteDownCount += Math.Abs(amount);
                    }
                    OperationStatus status = _commentWork.SaveMark(comment, false);
                    if (status != OperationStatus.Success) { return status; }

                    vote = Create();
                    vote.Amount = amount;
                    vote.CommentId = commentId;
                    status = SaveMark(vote);
                    if (status != OperationStatus.Success) { return status; }

                    worker.Commit();
                    return OperationStatus.Success;
                }
                catch
                {
                    worker.Rollback();
                    return OperationStatus.GenericError;
                }
            }
        }

        public OperationStatus VoteUp(Guid commentId)
        {
            return MakeVote(commentId, 1);
        }

        public OperationStatus VoteDown(Guid commentId)
        {
           return MakeVote(commentId, -1);
        }
    }
}
