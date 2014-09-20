using System;
using System.Collections.Generic;
using Annapolis.WebSite.Application;
using Annapolis.Web.Client;

namespace Annapolis.WebSite.ClientModels
{
    public class TagClient : SelectableItemClient
    {
        static TagClient()
        {
            RegisterModelTargetClassName(typeof(TagClient), "Tag");
        }
        
    }

    public class ThreadClient : SelectableItemClient
    {
        static ThreadClient()
        {
            RegisterModelTargetClassName(typeof(ThreadClient), "Thread");
        }
    }

    public class VoteClient : OwnerClientModel
    {
        static VoteClient()
        {
            RegisterModelTargetClassName(typeof(VoteClient), "Vote");
        }

        public Guid CommentId { get; set; }
        public int VoteUpCount { get; set; }
        public int VoteDownCount { get; set; }
    }

    public class TopicClient : OwnerClientModel
    {

        static TopicClient()
        {
            RegisterModelTargetClassName(typeof(TopicClient), "Topic");
        }

        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Content { get; set; }
        public bool IsSticky { get; set; }
        public bool IsLocked { get; set; }
        public string Thumbnail { get; set; }
        public Guid FirstCommentId { get; set; }
        public Guid ThreadId { get; set; }
        public string ThreadKey { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateTime { get; set; }

        public VoteClient Vote { get; set; }

        public bool CanUploadDocument { get; set; }
        public bool CanUploadImage { get; set; }

        public ICollection<CommentClient> Comments { get; set; }

        public ModelListClient<TagOptionClient> TagOptions { get; set; }
    }

    public class CommentClient : OwnerClientModel
    {
        static CommentClient()
        {
            RegisterModelTargetClassName(typeof(CommentClient), "Comment");
        }

        public string Content { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateTime { get; set; }

        public Guid TopicId { get; set; }
        public Guid ThreadId { get; set; }
        public bool IsLocked { get; set; }

        public VoteClient Vote { get; set; }
        public string TopicTitle { get; set; }
        public string TopicSubTitle { get; set; }
        public string TopicThumbnail { get; set; }

        public bool CanUploadDocument { get; set; }
        public bool CanUploadImage { get; set; }
    }

    public class TagOptionClient : ClientModel
    {
        static TagOptionClient()
        {
            RegisterModelTargetClassName(typeof(TagOptionClient), "TagOption");
        }

        public string CategoryName { get; set; }
        public string IdStrs { get; set; }

        public List<Guid> ToTagIds()
        {
            if (string.IsNullOrEmpty(IdStrs)) return null;
            string[] tagIdStrs = IdStrs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> tagIds = new List<Guid>();
            foreach (var tagIdStr in tagIdStrs)
            {
                if (string.Compare(tagIdStr.Trim(), WebConstants.Tag_Specifier_All) == 0) continue;
                tagIds.Add(Guid.Parse(tagIdStr));
            }
            return tagIds;
        }
    }

    public class TagListClient : SelectableListClient<TagClient>
    {
        public static readonly string Tag_Target_Key = "Tag";

        static TagListClient()
        {
            RegisterModelTargetClassName(typeof(TagListClient), "TagList");
        }

        public TagListClient()
        {
            Target = Tag_Target_Key;
        }
    }

    public class ThreadListCLient : SelectableListClient<ThreadClient>
    { 
    public static readonly string Thread_Target_Key = "Thread";

        static ThreadListCLient()
        {
            RegisterModelTargetClassName(typeof(ThreadListCLient), "ThreadList");
        }

        public ThreadListCLient()
        {
            Target = Thread_Target_Key;
        }
    }

    public class TopicWithCommentsClient : ClientModel
    {
        static TopicWithCommentsClient()
        {
            RegisterModelTargetClassName(typeof(TopicWithCommentsClient), "TopicWithPageComments");
        }
        public TopicClient Topic { get; set; }
        public PageListClient<CommentClient> PageComments { get; set; }
    }

    public class PageTopicClient : ClientModel
    {
        static PageTopicClient()
        {
            RegisterModelTargetClassName(typeof(PageTopicClient), "PageTopic");
        }

        public PageTopicClient()
        {
            Filter = new PageTopicFilterClient();
        }

        public PageListClient<TopicClient> Page { get; set; }

        public PageTopicFilterClient Filter { get; set; }
    }

    public class PageTopicFilterClient : ClientModel
    {
        static PageTopicFilterClient()
        {
            RegisterModelTargetClassName(typeof(PageTopicFilterClient), "PageTopicFilter");
        }

        public PageTopicFilterClient()
        {
            TagOptions = new ModelListClient<TagOptionClient>();
            TagOptions.Models = new List<TagOptionClient>();
        }

        /// <summary>
        /// 1-based page index
        /// </summary>
        public int PageNumber { get; set; }

        public string Sort { get; set; }

        public string ThreadKey { get; set; } 

        public ModelListClient<TagOptionClient> TagOptions { get; set; }
    }
}