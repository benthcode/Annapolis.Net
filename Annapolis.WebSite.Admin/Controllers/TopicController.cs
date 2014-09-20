using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Annapolis.Web.Attribute;
using Annapolis.WebSite.Admin.Models;
using Annapolis.WebSite.Admin.ViewModels;
using PagedList;

namespace Annapolis.WebSite.Admin.Controllers
{

    public class TopicController : Controller
    {
        private AdminDbEntities db = new AdminDbEntities();

        private static int PageSize = 10;
        //
        // GET: /Topic/

        public ActionResult Index(int? page)
        {
            int pageNumber = 1;
            if (page.HasValue && page.Value > 1) { pageNumber = page.Value; }

            var contentTopics = from topic in db.ContentTopics.Include(c => c.ContentThread).Include(c => c.MemberUser)
                                join comment in db.ContentComments on topic.FirstCommentId equals comment.Id
                                orderby topic.LastUpdateTime
                                select new TopicViewModel
                                {
                                    Topic = topic,
                                    Post = comment
                                };

            ViewBag.SearchKey = string.Empty;
            return View("Index", contentTopics.ToPagedList(pageNumber, PageSize));
        }

        [HttpPost]
        public ActionResult Search(string searchKey, int? page)
        {
            int pageNumber = 1;
            if (page.HasValue && page.Value > 1) { pageNumber = page.Value; }

            if(string.IsNullOrWhiteSpace(searchKey)) {  return Index(page); }
            
            var lowerSearchKey = searchKey.ToLower();
            var contenttopics = from topic in db.ContentTopics.Include(c => c.ContentThread).Include(c => c.MemberUser)
                                join comment in db.ContentComments on topic.FirstCommentId equals comment.Id
                                where topic.Title.ToLower().Contains(lowerSearchKey) 
                                orderby topic.LastUpdateTime
                                select new TopicViewModel
                                {
                                    Topic = topic,
                                    Post = comment
                                };

            ViewBag.SearchKey = searchKey;
            return View("Index", contenttopics.ToPagedList(pageNumber, PageSize));
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult TopicItem(TopicViewModel topicViewModel)
        {
            ContentTopic topic = db.ContentTopics.Find(topicViewModel.Topic.Id);
            ContentComment post = db.ContentComments.Find(topic.FirstCommentId);

            topic.IsHidden = topicViewModel.Topic.IsHidden;
            topic.IsSticky = topicViewModel.Topic.IsSticky;
            topic.IsLocked = topicViewModel.Topic.IsLocked;

            db.SaveChanges();

            topicViewModel.Topic = topic;
            topicViewModel.Post = post;

            return PartialView("_TopicItem", topicViewModel);
        }

        //
        // GET: /Topic/Details/5

        public ActionResult Details(Guid? id = null)
        {
            if (!id.HasValue) { return HttpNotFound(); }

            ContentTopic contentTopic = db.ContentTopics.Include(c => c.ContentThread)
                                                    .Include(c => c.MemberUser)
                                                    .Include(c => c.ContentComments)
                                                    .Where(c => c.Id == id.Value).SingleOrDefault();

            if (contentTopic == null)  {  return HttpNotFound(); }

            var viewModel = new TopicCommentsViewModel() { Topic = contentTopic };
            viewModel.Post = contentTopic.ContentComments.SingleOrDefault(x => x.Id == viewModel.Topic.FirstCommentId);
            viewModel.Comments = contentTopic.ContentComments.Where(x => x.Id != viewModel.Topic.FirstCommentId)
                                             .OrderBy(x => x.CreateTime)
                                             .ToList();
            
            return View(viewModel);
        }

        //
        // GET: /Topic/Create

        public ActionResult Create()
        {
            ViewBag.ThreadId = new SelectList(db.ContentThreads, "Id", "Name");
            ViewBag.UserId = new SelectList(db.MemberUsers, "Id", "UserName");
            return View();
        }

        //
        // POST: /Topic/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContentTopic contenttopic)
        {
            if (ModelState.IsValid)
            {
                contenttopic.Id = Guid.NewGuid();
                db.ContentTopics.Add(contenttopic);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ThreadId = new SelectList(db.ContentThreads, "Id", "Name", contenttopic.ThreadId);
            ViewBag.UserId = new SelectList(db.MemberUsers, "Id", "UserName", contenttopic.UserId);
            return View(contenttopic);
        }

        //
        // GET: /Topic/Edit/5

        public ActionResult Edit(Guid? id = null)
        {
            if (!id.HasValue) { return HttpNotFound(); }

            ContentTopic contentTopic = db.ContentTopics.Include(c => c.ContentThread)
                                                    .Include(c => c.MemberUser)
                                                    .Include(c => c.ContentComments)
                                                    .Where(c => c.Id == id.Value).SingleOrDefault();

            if (contentTopic == null) { return HttpNotFound(); }

            var viewModel = new TopicCommentsViewModel() { Topic = contentTopic };
            viewModel.Post = contentTopic.ContentComments.SingleOrDefault(x => x.Id == viewModel.Topic.FirstCommentId);
            viewModel.Comments = contentTopic.ContentComments.Where(x => x.Id != viewModel.Topic.FirstCommentId).ToList();

            ViewBag.Threads = new SelectList(db.ContentThreads, "Id", "Name", viewModel.Topic.ThreadId);
            ViewBag.Users = new SelectList(db.MemberUsers, "Id", "UserName", viewModel.Topic.UserId);
            return View(viewModel);
        }

        //
        // POST: /Topic/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(TopicCommentsViewModel viewModel)
        {
            var post = db.ContentComments.Single(x => x.Id == viewModel.Topic.FirstCommentId);
            post.Content = viewModel.Post.Content;
            post.OriginalContent = viewModel.Post.OriginalContent;
            viewModel.Post = post;
            if (ModelState.IsValid)
            {
                db.Entry(viewModel.Topic).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Topic.ThreadId = new SelectList(db.ContentThreads, "Id", "Name", viewModel.Topic.ThreadId);
            ViewBag.Topic.UserId = new SelectList(db.MemberUsers, "Id", "UserName", viewModel.Topic.UserId);
            return View(viewModel);
        }

        //
        // GET: /Topic/Delete/5

        public ActionResult Delete(Guid? id = null)
        {
            ContentTopic contenttopic = db.ContentTopics.Find(id);
            if (contenttopic == null)
            {
                return HttpNotFound();
            }
            return View(contenttopic);
        }

        //
        // POST: /Topic/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ContentTopic contenttopic = db.ContentTopics.Find(id);
            db.ContentTopics.Remove(contenttopic);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}