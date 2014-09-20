using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Annapolis.Web.Attribute;
using Annapolis.WebSite.Application.Attribute;
using Annapolis.WebSite.Application.Base;
using Annapolis.WebSite.ClientModels;
using Annapolis.WebSite.Drivers.Abstract;
using Annapolis.Web.Client;
using Annapolis.Shared.Model;

namespace Annapolis.WebSite.Controllers
{
    public class TopicController : BaseSiteMvcController
    {
     
        private readonly ITopicDriver _topicDriver;
        private readonly ICommentDriver _commentDriver;
        private readonly ITagDriver _tagDriver;
        private readonly IThreadDriver _threadDriver;

        public TopicController(ITopicDriver topicDriver, ICommentDriver commentDriver, 
            ITagDriver tagDirver,  IThreadDriver threadDriver)
        {
            _topicDriver = topicDriver;
            _commentDriver = commentDriver;
            _tagDriver = tagDirver;
            _threadDriver = threadDriver;
        }

        [HttpGet]
        public ActionResult Thread(string key, int? page)
        {
  
            var currentThread = string.IsNullOrEmpty(key) ? _threadDriver.RootThread : _threadDriver.GetThreadByKey(key);
            if (currentThread == null) { return HttpNotFound(); }

            var tagCategoryMaps = _threadDriver.GetTagCategories(currentThread.Id);
            List<TagListClient> tagLists = new List<TagListClient>();
            foreach (var map in tagCategoryMaps)
            {
                var tagList = _tagDriver.GetTagsByCategory(map.TagCategory.Name, map.OnlyShowHotTag, map.IncludeAll, map.IncludeOther);
                tagLists.Add(tagList);
            }
            ViewBag.TagLists = tagLists;


            PageTopicFilterClient topicFilter = new PageTopicFilterClient();
            topicFilter.PageNumber = GetPageNumber(page);;
            topicFilter.ThreadKey = currentThread.Key;
            topicFilter.Sort = " CreateTime DESC";
            PageListClient<TopicClient> pageList = _topicDriver.PagingClientTopics(DefaultSetting.TopicsPerPage, topicFilter);
            PageTopicClient pageTopic = new PageTopicClient() { Page = pageList, Filter = topicFilter };

            ViewBag.CurrentThread = currentThread;
         
            pageTopic.Page.GenerateUniqueId();
            return View("Thread", pageTopic);
        }

        public ActionResult StickyTopic(string key)
        {
            var stickyTopics = _topicDriver.GetStickyTopics(key);

            return View(stickyTopics);
        }

        [HttpGet]
        public ActionResult Show(Guid key)
        {
            var commentsPerTopic = _topicDriver.GetTopicWithComments(key, PageConstants.FirstPageNumber, DefaultSetting.CommentsPerPage);
            if (commentsPerTopic == null) { return HttpNotFound(); }

            ScriptRegister.AddJson("commentsPerTopic", _topicDriver.ToTopicWithCommentsClient(commentsPerTopic));
            
            var newComment = _commentDriver.Create();
            newComment.TopicId = commentsPerTopic.Topic.Id;
            newComment.Topic = commentsPerTopic.Topic;
            ScriptRegister.AddJson("newComment", _commentDriver.ToClient(newComment));

            ViewBag.CurrentThread = _threadDriver.Get(commentsPerTopic.Topic.ThreadId);

            return View();
        }

        [HttpGet]
        [AnnaMvcAuthorize]
        public ActionResult New(string key)
        {
            var currentThread = string.IsNullOrEmpty(key) ? _threadDriver.RootThread : _threadDriver.GetThreadByKey(key);
            if (currentThread == null) { return HttpNotFound(); }

            var topic = _topicDriver.Create();
            topic.Thread = currentThread;
            topic.ThreadId = currentThread.Id;
         
            var topicClient = _topicDriver.ToClient(topic);
            ProcessTagOption(topicClient);
            ScriptRegister.AddJson("topic", topicClient);

            return View("Record", topicClient);
        }

        [HttpGet]
        [AnnaMvcAuthorize]
        public ActionResult Edit(Guid key)
        {
            var topic = _topicDriver.Get(key, new string[]{"Tags", "Tags.Category", "Thread", "Thread.TagCategoryMaps"});
            if (topic != null && !topic.IsNew())
            {
                TopicClient topicClient = _topicDriver.ToClient(topic);
                ProcessTagOption(topicClient);
                ScriptRegister.AddJson("topic", topicClient);
                return View("Record", topicClient);
            }
            return HttpNotFound();
        }

        private void ProcessTagOption(TopicClient topic)
        {
            if (topic == null) return;
         
            var tagCategoryMaps = _threadDriver.GetTagCategories(topic.ThreadId);
            List<TagListClient> tagLists = new List<TagListClient>();
            foreach (var map in tagCategoryMaps)
            {
                var tagList = _tagDriver.GetTagsByCategory(map.TagCategory.Name, map.OnlyShowHotTag, false, map.IncludeOther);
                if (topic.IsNew() && tagList.Count > 0)
                {
                    topic.TagOptions.Add(new TagOptionClient() { CategoryName = map.TagCategory.Name, IdStrs = tagList[0].Id.ToString()});
                }
                TagOptionClient tagOption = topic.TagOptions.SingleOrDefault(x => x.CategoryName == map.TagCategory.Name);
                if (tagOption != null)
                {
                    tagList.SelectedValue = tagOption.IdStrs;
                }
                tagLists.Add(tagList);
            }
            ViewBag.TagLists = tagLists;
        }

        [ChildActionOnly]
        public PartialViewResult RenderTagOption(TopicClient topic)
        {
            ProcessTagOption(topic);
            return PartialView("TagOption");
        }

        public ContentResult Test()
        {
            return Content("test");
        }
    }
}
