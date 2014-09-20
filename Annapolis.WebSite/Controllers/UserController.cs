using System.Web.Mvc;
using Annapolis.WebSite.Application.Attribute;
using Annapolis.WebSite.Application.Base;
using Annapolis.WebSite.Drivers.Abstract;

namespace Annapolis.WebSite.Controllers
{
    public class UserController : BaseSiteMvcController
    {
        private ITopicDriver _topicDriver;
        private ICommentDriver _commentDriver;

        public UserController(ITopicDriver topicDriver, ICommentDriver commentDriver)
        {
            _topicDriver = topicDriver;
            _commentDriver = commentDriver;
        }

        [HttpGet]
        [AnnaMvcAuthorize]
        public ActionResult Topic(int? page)
        {
            int pageNumber = GetPageNumber(page);
            var pageTopics = _topicDriver.PagingMyClientTopics(pageNumber, DefaultSetting.TopicsPerPage, "CreateTime DESC");
            return View(pageTopics);
        }

        [HttpGet]
        [AnnaMvcAuthorize]
        public ActionResult Comment(int? page)
        {
            int pageNumber = GetPageNumber(page);
            var pageComments = _commentDriver.PagingMyClientComments(pageNumber, DefaultSetting.CommentsPerPage, "Topic.Title,CreateTime DESC");
            return View(pageComments);
        }

        [HttpGet]
        [AnnaMvcAuthorize]
        public ActionResult Setting()
        {
            return View();
        }

        [HttpGet]
        [AnnaMvcAuthorize]
        public ActionResult SettingUpdatePassword()
        {
            return View();
        }

    }
}
