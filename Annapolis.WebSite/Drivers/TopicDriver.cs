using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using Annapolis.WebSite.ViewModels;
using LinqDynamicKit;
using Annapolis.Web.Client;
using PagedList;

namespace Annapolis.WebSite.Drivers
{
    public class TopicDriver : OwnerableEntityDriver<ContentTopic, TopicClient>, ITopicDriver 
    {

        private readonly ITopicWork _topicWork;
        private readonly ICommentWork _commentWork;
        private readonly ICommentDriver _commentDriver;
        private readonly ITagWork _tagWork;
        private readonly IThreadWork _threadWork;

        public TopicDriver(ITopicWork contentTopicWork, ICommentWork commentWork, 
            ICommentDriver commentDriver, ITagWork tagWork, IThreadWork threadWork)
            : base(contentTopicWork)
        {
            _topicWork = contentTopicWork;
            _commentWork = commentWork;
            _commentDriver = commentDriver;
            _tagWork = tagWork;
            _threadWork = threadWork;
        }

        #region IConvertable

        public override TopicClient ToClient(ContentTopic entity, TopicClient c = null, string[] excludeProperties = null, bool serverStatus = true)
        {
            if (entity == null) return null;

            TopicClient topicClient = base.ToClient(entity, c, excludeProperties, serverStatus);

            topicClient.Title = entity.Title;
            topicClient.SubTitle = entity.SubTitle;
            topicClient.IsSticky = entity.IsLocked;
            topicClient.IsLocked = entity.IsLocked || entity.IsHidden;
            topicClient.CreateTime = entity.CreateTime;
            topicClient.LastUpdateTime = entity.LastUpdateTime;
            topicClient.Thumbnail = entity.Thumbnail;
            topicClient.ThreadId = entity.ThreadId;
            if (entity.Thread != null)
            {
                topicClient.ThreadKey = entity.Thread.Key;
                if (!topicClient.IsLocked && entity.Thread.IsLocked) { entity.IsLocked = true; }
            }

            if (excludeProperties==null || !excludeProperties.Contains("FirstComment"))
            {
                topicClient.FirstCommentId = entity.FirstComment.Id;
                topicClient.Content = entity.FirstComment.Content;
            }

            if (excludeProperties == null || !excludeProperties.Contains("Vote"))
            {
                if (topicClient.Vote == null) { topicClient.Vote = new VoteClient(); }

                topicClient.Vote.CommentId = entity.FirstComment.Id;
                topicClient.Vote.UserName = entity.UserName;
                topicClient.Vote.VoteUpCount = entity.FirstComment.VoteUpCount;
                topicClient.Vote.VoteDownCount = entity.FirstComment.VoteDownCount;
            }

            if (excludeProperties == null || !excludeProperties.Contains("Comments"))
            {
                if (topicClient.Comments == null) { topicClient.Comments = new List<CommentClient>(); }
                if (entity.Comments != null && entity.Comments.Count > 0)
                {
                    foreach (var comment in entity.Comments)
                    {
                        topicClient.Comments.Add(_commentDriver.ToClient(comment));
                    }
                }
            }

            if (excludeProperties == null || !excludeProperties.Contains("Tags"))
            {
                if (topicClient.TagOptions == null) {  topicClient.TagOptions = new ModelListClient<TagOptionClient>(); }
                if (entity.Tags != null && entity.Tags.Count > 0)
                {
                    topicClient.TagOptions.Clear();
                    var groupTags = entity.Tags.GroupBy(x => x.Category).ToList();
                    foreach (var tagCategory in groupTags)
                    {
                        string tagIds = string.Join(",", tagCategory.Select(x => x.Id));
                        topicClient.TagOptions.Add(new TagOptionClient() { CategoryName = tagCategory.Key.Name, IdStrs = tagIds });
                    }
                }
            }

            if (excludeProperties == null || !excludeProperties.Contains("UploadPermission"))
            {
                topicClient.CanUploadDocument = HasPermission(EntityPermission.UploadDocument, entity) == OperationStatus.Granted;
                topicClient.CanUploadImage = HasPermission(EntityPermission.UploadImage,  entity) == OperationStatus.Granted;
            }

            return topicClient;
        }

        public override ContentTopic FromClient(ContentTopic m, TopicClient c, string[] includeProperties = null)
        {
            ContentTopic topic = base.FromClient(m, c, includeProperties);
            if (topic == null) return null;

            topic.Title = c.Title;
            topic.SubTitle = c.SubTitle;
            topic.FirstComment.Content = c.Content;
            topic.FirstComment.IsAttachedToTopic = true;
            topic.Thumbnail = c.Thumbnail;
            topic.ThreadId = c.ThreadId;

            #region Tags

            if (c.TagOptions != null && c.TagOptions.Models!= null && c.TagOptions.Count > 0)
            {
                var tagCategoryMaps = _threadWork.GetTagCategories(c.ThreadId).ToDictionary(x => x.TagCategory.Name);
                for (int i = c.TagOptions.Count - 1; i >= 0; i-- )
                {
                    if (!tagCategoryMaps.ContainsKey(c.TagOptions[i].CategoryName))
                    {
                        c.TagOptions.RemoveAt(i);
                    }
                }
                
                var targetTagIds = c.TagOptions.SelectMany(x => x.ToTagIds()).ToList();
                var currentTags = topic.Tags.ToList();
                for (int i = topic.Tags.Count - 1; i >= 0; i--)
                {
                    if (!targetTagIds.Contains(currentTags[i].Id))
                    {
                        topic.Tags.Remove(currentTags[i]);
                    }
                }

                var toAddTagIds = targetTagIds.Where(x => !currentTags.Select(t => t.Id).Contains(x)).ToList();
                var toAddTags = _tagWork.All.Include(x => x.Category).Where(x => toAddTagIds.Contains(x.Id)).ToList();
                foreach (var tag in toAddTags)
                {
                    topic.Tags.Add(tag);
                }
            }

            #endregion

            _commentWork.ParseContentFile(topic.FirstComment, WebConstants.File_Category_Content);

            return topic;
    
        }
       

        #endregion

        public override OperationStatus Save(TopicClient c, string[] includeProperties = null, string[] excludeProperties = null)
        {
            return base.Save(c, new string[]{"FirstComment", "Tags", "Tags.Category"}, new string[]{"Vote"});
        }

        public ModelListClient<TopicClient> GetStickyTopics(string threadKey)
        { 
            Expression<Func<ContentTopic, bool>> predicate = PredicateBuilder.True<ContentTopic>();
            if (_threadWork.TotalThreadCount > 1)
            {
                var currentThread = string.IsNullOrEmpty(threadKey) ? _threadWork.RootThread : _threadWork.GetThread(threadKey);
                predicate = predicate.And(x => x.ThreadId == currentThread.Id);
            }

            predicate = predicate.And(x => x.IsSticky && !x.IsHidden);
            var query = _topicWork.Query(predicate, "CreateTime");
            ModelListClient<TopicClient> modelList = new ModelListClient<TopicClient>();
            foreach (var topic in query)
            {
                modelList.Add(ToClient(topic, excludeProperties: new string[] { "Tags", "FirstComment", "Comments", "UploadPermission" }));
            }

            return modelList;
        }

        //get topics on paging
        public PageListClient<TopicClient> PagingClientTopics(int pageSize, PageTopicFilterClient filter)
        {
            Expression<Func<ContentTopic, bool>> predicate = PredicateBuilder.True<ContentTopic>(); // null; // = x => x.IsVisible == false;

            predicate = predicate.And(x => !x.IsHidden);

            //Ignore it, if there is only one thread in the system
            if (_threadWork.TotalThreadCount > 1)
            {
                ContentThread thread = _threadWork.GetThread(filter.ThreadKey);
                Expression<Func<ContentTopic, bool>> threadPredicate = PredicateBuilder.False<ContentTopic>();
                threadPredicate = threadPredicate.Or(x => x.ThreadId == thread.Id);
                predicate = predicate.And(threadPredicate.Expand());
            }

            foreach (var tagOption in filter.TagOptions)
            {
                if (string.IsNullOrEmpty(tagOption.IdStrs) || tagOption.IdStrs.Contains(WebConstants.Tag_Specifier_All)) continue;
                List<Guid> tagIds = tagOption.ToTagIds();
                if (tagIds.Count > 0)
                {
                    Expression<Func<ContentTopic, bool>> singleTagPredicate = PredicateBuilder.False<ContentTopic>();
                    foreach (var tagId in tagIds)
                    {
                        singleTagPredicate = singleTagPredicate.Or(x => x.Tags.Select(t => t.Id).Contains(tagId));
                    }

                    if (singleTagPredicate != null)
                    {
                        predicate = predicate.And(singleTagPredicate.Expand()); //call expand() to fix not bound error
                    }
                }
            }

            var pageTopic = _topicWork.Paging(filter.PageNumber, pageSize, filter.Sort, predicate, new string[]{"Tags"});
            PageListClient<TopicClient> page = new PageListClient<TopicClient>(pageTopic, pageTopic.Count);
            foreach (var topic in pageTopic)
            {
                page.Add(ToClient(topic, excludeProperties: new string[] {"Tags", "FirstComment", "Comments", "UploadPermission"}));
            }
         
            return page;
        }

        public PageListClient<TopicClient> PagingMyClientTopics(int pageNumber, int pageSize, string sort)
        {
            Expression<Func<ContentTopic, bool>> predicate = PredicateBuilder.True<ContentTopic>();
            predicate = predicate.And(x => x.UserId == SecurityManager.CurrentUser.UserId);
            predicate = predicate.Expand();

            var pageTopic = _topicWork.Paging(pageNumber, pageSize, sort, predicate);
            PageListClient<TopicClient> page = new PageListClient<TopicClient>(pageTopic, pageTopic.Count);
            foreach (var topic in pageTopic)
            {
                page.Add(ToClient(topic, excludeProperties: new string[] {"Tags", "Comments", "UploadPermission"}));
            }

            return page;
        }

        public OperationStatus HasPermission(EntityPermission permission, ContentTopic contentTopic = null, Guid? threadId = null)
        {
            return _topicWork.HasPermission(permission, contentTopic, threadId);
        }

        //get single topic with its posts on paging
        public TopicWithComments GetTopicWithComments(Guid topicId, int pageNumber, int CommentsPerPage)
        {

            TopicWithComments topicWithComments = new TopicWithComments();

            if (topicId == Guid.Empty) return null;
            Expression<Func<ContentComment, bool>> predicate = x => x.TopicId == topicId && !x.IsHidden && !x.IsSpam;
            topicWithComments.Comments = _commentWork.Paging(pageNumber, CommentsPerPage, "CreateTime desc", predicate, new string[] { "Topic", "Topic.User", "Topic.FirstComment", "Topic.Tags", "Topic.Tags.Category" });

            if (topicWithComments.Comments.Count > 0)
            {
                topicWithComments.Topic = topicWithComments.Comments[0].Topic;
            }
            else
            {
                topicWithComments.Topic = Get(topicId, new string[]{"Tags", "Tags.Category"});
            }

           

            return topicWithComments;
        }

        public TopicWithCommentsClient ToTopicWithCommentsClient(TopicWithComments topicWithComments)
        {

            TopicWithCommentsClient topicWithCommentsClient = new TopicWithCommentsClient();
            topicWithCommentsClient.PageComments = new PageListClient<CommentClient>(topicWithComments.Comments, topicWithComments.Comments.Count);

            foreach (var comment in topicWithComments.Comments)
            {
                topicWithCommentsClient.PageComments.Models.Add(_commentDriver.ToClient(comment));
            }
            topicWithCommentsClient.Topic = ToClient(topicWithComments.Topic, excludeProperties: new string[]{"Comments"});

            return topicWithCommentsClient;
        }

        public PageListClient<CommentClient> GetPageComments(Guid topicId, int pageNumber, int pageSize)
        {
            if (topicId == Guid.Empty) return null;
            Expression<Func<ContentComment, bool>> predicate = x => x.TopicId == topicId && !x.IsHidden && !x.IsSpam;
            var comments = _commentWork.Paging(pageNumber, pageSize, "CreateTime desc", predicate);

            PageListClient<CommentClient> pageComments = new PageListClient<CommentClient>(comments, comments.Count);
            pageComments.Models = new List<CommentClient>();
            foreach (var comment in comments)
            {
                pageComments.Add(_commentDriver.ToClient(comment, excludeProperties: new string[] { "Tags" }));
            }

            return pageComments;
        }
        
    }
}