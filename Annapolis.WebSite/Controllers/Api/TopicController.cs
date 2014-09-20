using System;
using System.Web.Http;
using System.Web.Mvc;
using Annapolis.Manager;
using Annapolis.Shared.Model;
using Annapolis.WebSite.Application.Base;
using Annapolis.WebSite.ClientModels;
using Annapolis.WebSite.Drivers.Abstract;
using Annapolis.Web.Client;
using Annapolis.WebSite.Application.Attribute;

namespace Annapolis.WebSite.Controllers.Api
{
   
    public class TopicController : BaseApiController
    {

        private readonly ITopicDriver _topicDriver;

        public TopicController()
        {
            _topicDriver = DependencyResolver.Current.GetService<ITopicDriver>();
        }

        [System.Web.Http.HttpPost]
        public ClientModel TopicsByFilter([FromBody]PageTopicFilterClient fitler)
        {
            var pageTopic = _topicDriver.PagingClientTopics(DefaultSetting.TopicsPerPage, fitler);
            return pageTopic;
        }

        [System.Web.Http.HttpGet]
        public ClientModel GetCommentsOnPage(Guid key, int? page)
        {
            int pageNumber = GetPageNumber(page);
            var pageComment = _topicDriver.GetPageComments(key, pageNumber, DefaultSetting.TopicsPerPage);
            return pageComment;
        }

        private void SaveTopic(TopicClient topic)
        {
            var status = _topicDriver.Save(topic);
            if (status == OperationStatus.Success)
            {
                topic.AddSuccessNotification(MessageManager.GetMessage(status));
            }
            else
            {
                topic.AddErrorNotification(MessageManager.GetMessage(status));
            }
        }

        [System.Web.Http.HttpPost]
        [AnnaApiAuthorize]
        public ClientModel Create([FromBody]TopicClient topic)
        {
            SaveTopic(topic);
            return topic;
        }

        [System.Web.Http.HttpPut]
        [AnnaApiAuthorize]
        public ClientModel Update(string key, TopicClient topic)
        {
            SaveTopic(topic);
            return topic;
        }

        [System.Web.Http.HttpDelete]
        [AnnaApiAuthorize]
        public void Delete(string key)
        {
        }

    }
}
