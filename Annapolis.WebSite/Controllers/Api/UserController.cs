using System.Web.Mvc;
using Annapolis.WebSite.Application.Base;
using Annapolis.WebSite.Drivers.Abstract;
using Annapolis.WebSite.Application.Attribute;
using Annapolis.Web.Client;

namespace Annapolis.WebSite.Controllers.Api
{
    public class UserController : BaseApiController
    {
        private ITopicDriver _topicDriver;
        private ICommentDriver _commentDriver;

        public UserController()
        {
            
        }

        [System.Web.Http.HttpGet]
        [AnnaApiAuthorize]
        public ClientModel Topic(int? page)
        {
            _topicDriver = DependencyResolver.Current.GetService<ITopicDriver>();
            int pageNumber = GetPageNumber(page);
            var pageTopics = _topicDriver.PagingMyClientTopics(pageNumber, DefaultSetting.TopicsPerPage, "CreateTime DESC");
            return pageTopics;
        }

        [System.Web.Http.HttpGet]
        [AnnaApiAuthorize]
        public ClientModel Comment(int? page)
        {
            _commentDriver  = DependencyResolver.Current.GetService<ICommentDriver>();
            int pageNumber = GetPageNumber(page);
            var pageComments = _commentDriver.PagingMyClientComments(pageNumber, DefaultSetting.CommentsPerPage, "CreateTime DESC");
            return pageComments;
        }

    }
}
