using System;
using Annapolis.Entity;
using Annapolis.Shared.Model;
using Annapolis.WebSite.ClientModels;
using Annapolis.WebSite.Drivers.Base;
using Annapolis.WebSite.ViewModels;
using Annapolis.Web.Client;

namespace Annapolis.WebSite.Drivers.Abstract
{
    public interface ITopicDriver : IEntityConvertableDriver<ContentTopic, TopicClient>, 
        ISavableDriver<ContentTopic, TopicClient>
    {
        ModelListClient<TopicClient> GetStickyTopics(string threadKey);

        PageListClient<TopicClient> PagingClientTopics(int pageSize, PageTopicFilterClient filter);
        PageListClient<TopicClient> PagingMyClientTopics(int pageNumber, int pageSize, string sort);

        TopicWithComments GetTopicWithComments(Guid topicId, int pageNumber, int CommentsPerPage);
        TopicWithCommentsClient ToTopicWithCommentsClient(TopicWithComments topicWithComments);

        PageListClient<CommentClient> GetPageComments(Guid id, int pageNumber, int pageSize);

        OperationStatus HasPermission(EntityPermission permission, ContentTopic contentTopic = null, Guid? threadId = null);
    }
}
