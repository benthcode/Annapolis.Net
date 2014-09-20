using System;
using System.Linq;
using System.Linq.Expressions;
using Annapolis.Abstract.Work;
using Annapolis.Entity;
using Annapolis.Manager;
using Annapolis.Shared.Model;
using Annapolis.WebSite.Application;
using Annapolis.WebSite.ClientModels;
using Annapolis.WebSite.Drivers.Abstract;
using Annapolis.WebSite.Drivers.Base;
using LinqDynamicKit;
using Annapolis.Web.Client;

namespace Annapolis.WebSite.Drivers
{
    public class CommentDriver : OwnerableEntityDriver<ContentComment, CommentClient>, ICommentDriver 
    {
        private readonly ICommentWork _commentWork;
        private readonly IThreadWork _threadWordk;


        public CommentDriver(ICommentWork commentWork, IThreadWork threadWork)
            : base(commentWork)
        {
            _commentWork = commentWork;
            _threadWordk = threadWork;
        }


        public override CommentClient CreateClient(string[] includeProperties = null)
        {
            var commentClient = base.CreateClient(includeProperties);

            if (includeProperties!= null && includeProperties.Contains("Vote"))
            {
                commentClient.Vote = new VoteClient();
                if (SecurityManager.CurrentUser != null)
                {
                    commentClient.Vote.UserName = SecurityManager.CurrentUser.UserName;
                }
            }

            return commentClient;
        }

        public override CommentClient ToClient(ContentComment entity, CommentClient c = null, string[] excludeProperties = null, bool serverStatus = true)
        {
            if (entity == null) return null;

            CommentClient commentClient = base.ToClient(entity, c, excludeProperties, serverStatus);

            commentClient.Content = entity.Content;
            commentClient.TopicId = entity.TopicId;
            commentClient.CreateTime = entity.CreateTime;
            commentClient.LastUpdateTime = entity.LastUpdateTime;
            commentClient.IsLocked = entity.IsLocked || entity.IsHidden;

            if (excludeProperties == null || !excludeProperties.Contains("Vote"))
            {
                if (commentClient.Vote == null) { commentClient.Vote = new VoteClient(); }

                commentClient.Vote.UserName = entity.UserName;
                commentClient.Vote.VoteUpCount = entity.VoteUpCount;
                commentClient.Vote.VoteDownCount = entity.VoteDownCount;
                commentClient.Vote.CommentId = entity.Id;
            }

            if (excludeProperties == null || !excludeProperties.Contains("UploadPermission"))
            {
                commentClient.CanUploadDocument = HasPermission(EntityPermission.UploadDocument, entity) == OperationStatus.Granted;
                commentClient.CanUploadImage = HasPermission(EntityPermission.UploadImage, entity) == OperationStatus.Granted;
            }

            if(entity.Topic != null)
            {
                commentClient.ThreadId = entity.Topic.ThreadId;
                commentClient.TopicTitle = entity.Topic.Title;
                if (excludeProperties == null || !excludeProperties.Contains("Topic.SubTitle"))
                {
                    commentClient.TopicSubTitle = entity.Topic.SubTitle;
                }
                commentClient.TopicThumbnail = entity.Topic.Thumbnail;
                if (entity.Topic.IsLocked || entity.Topic.IsHidden)
                {
                    commentClient.IsLocked = true;
                }
                else
                {
                    if (_threadWordk.GetThread(entity.Topic.ThreadId).IsLocked) { commentClient.IsLocked = true; }
                }

            }
           

            return commentClient;
        }

        public override ContentComment FromClient(ContentComment m, CommentClient c, string[] includeProperties = null)
        {
            ContentComment comment = base.FromClient(m, c, includeProperties);
            if (comment == null) return null;

            comment.Content = c.Content;
            if (comment.IsNew())
            {
                comment.TopicId = c.TopicId;
            }

            if (includeProperties!=null && includeProperties.Contains("Vote"))
            {
                comment.VoteUpCount = c.Vote.VoteUpCount;
                comment.VoteDownCount = c.Vote.VoteDownCount;
            }

            _commentWork.ParseContentFile(comment, WebConstants.File_Category_Content);

            return comment;
        }

        public override OperationStatus Save(CommentClient c, string[] includeProperties = null, string[] excludeProperties = null)
        {
            return base.Save(c, new string[]{"Topic", "Topic.Thread"}, excludeProperties);
        }

        public OperationStatus HasPermission(EntityPermission permission, ContentComment contentComment = null, Guid? threadId = null)
        {
            return _commentWork.HasPermission(permission, contentComment, threadId);
        }

        public PageListClient<CommentClient> PagingMyClientComments(int pageNumber, int pageSize, string sort)
        {
            Expression<Func<ContentComment, bool>> predicate = PredicateBuilder.True<ContentComment>();
            predicate = predicate.And(x => x.UserId == SecurityManager.CurrentUser.UserId);
            predicate = predicate.Expand();

            var pageComments = _commentWork.Paging(pageNumber, pageSize, sort, predicate, new string[] { "Topic" });
            PageListClient<CommentClient> page = new PageListClient<CommentClient>(pageComments, pageComments.Count);
            foreach (var comment in pageComments)
            {
                page.Add(ToClient(comment, excludeProperties: new string[] { "Vote", "Topic.SubTitle" }));
            }

            return page;
        }

    }
}