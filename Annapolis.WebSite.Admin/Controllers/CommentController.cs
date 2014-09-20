using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Annapolis.WebSite.Admin.Models;
using PagedList;
using Annapolis.Web.Attribute;
using Annapolis.WebSite.Admin.ViewModels;

namespace Annapolis.WebSite.Admin.Controllers
{
    public class CommentController : Controller
    {
        private AdminDbEntities db = new AdminDbEntities();

        private static int PageSize = 10;

        //
        // GET: /Comment/

        public ActionResult Index(int? page)
        {
            int pageNumber = 1;
            if (page.HasValue && page.Value > 1) { pageNumber = page.Value; }

            var contentcomments = from comment in db.ContentComments.Include(c => c.ContentTopic).Include(c => c.MemberUser)
                                    .Where(c => !c.IsAttachedToTopic)
                                  join topic in db.ContentTopics on comment.TopicId equals topic.Id
                                  orderby topic.Title, comment.CreateTime descending
                                  select new TopicViewModel
                                    {
                                        Topic = topic,
                                        Post = comment
                                    };
            ViewBag.SearchKey = string.Empty;
            return View("Index", contentcomments.ToPagedList(pageNumber, PageSize));
        }

        [HttpPost]
        public ActionResult Search(string searchKey, int? page)
        {
            int pageNumber = 1;
            if (page.HasValue && page.Value > 1) { pageNumber = page.Value; }

            if (string.IsNullOrWhiteSpace(searchKey)) { return Index(page); }

            var lowerSearchKey = searchKey.ToLower();
            var contenttopics = from comment in db.ContentComments.Include(c => c.ContentTopic).Include(c => c.MemberUser)
                                    .Where(c => !c.IsAttachedToTopic && c.Content.ToLower().Contains(lowerSearchKey))
                                join topic in db.ContentTopics on comment.TopicId equals topic.Id
                                orderby topic.Title, comment.CreateTime descending
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
        public ActionResult CommentItem(TopicViewModel commentViewModel)
        {

            ContentComment commentItem = db.ContentComments.Find(commentViewModel.Post.Id);

            commentItem.IsHidden = commentViewModel.Post.IsHidden;
            commentItem.IsSpam = commentViewModel.Post.IsSpam;
            commentItem.IsLocked = commentViewModel.Post.IsLocked;
            commentItem.IsSolution = commentViewModel.Post.IsSolution;

            db.SaveChanges();

            commentViewModel.Post = commentItem;
            ContentTopic topic = db.ContentTopics.Find(commentViewModel.Post.TopicId);
            commentViewModel.Topic = topic;

            return PartialView("_CommentItem", commentViewModel);
        }

        //
        // GET: /Comment/Details/5

        public ActionResult Details(Guid? id)
        {
            if (!id.HasValue) { return HttpNotFound(); }

            ContentComment contentcomment = db.ContentComments.Find(id.Value);
            if (contentcomment == null)
            {
                return HttpNotFound();
            }
            return View(contentcomment);
        }

        //
        // GET: /Comment/Create

        public ActionResult Create()
        {
            ViewBag.TopicId = new SelectList(db.ContentTopics, "Id", "Title");
            ViewBag.UserId = new SelectList(db.MemberUsers, "Id", "UserName");
            return View();
        }

        //
        // POST: /Comment/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContentComment contentcomment)
        {
            if (ModelState.IsValid)
            {
                contentcomment.Id = Guid.NewGuid();
                db.ContentComments.Add(contentcomment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TopicId = new SelectList(db.ContentTopics, "Id", "Title", contentcomment.TopicId);
            ViewBag.UserId = new SelectList(db.MemberUsers, "Id", "UserName", contentcomment.UserId);
            return View(contentcomment);
        }

        //
        // GET: /Comment/Edit/5

        public ActionResult Edit(Guid? id = null)
        {
            if (!id.HasValue) { return HttpNotFound(); }

            ContentComment contentcomment = db.ContentComments.Find(id.Value);
            if (contentcomment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TopicId = new SelectList(db.ContentTopics, "Id", "Title", contentcomment.TopicId);
            ViewBag.UserId = new SelectList(db.MemberUsers, "Id", "UserName", contentcomment.UserId);
            return View(contentcomment);
        }

        //
        // POST: /Comment/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ContentComment contentcomment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contentcomment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TopicId = new SelectList(db.ContentTopics, "Id", "Title", contentcomment.TopicId);
            ViewBag.UserId = new SelectList(db.MemberUsers, "Id", "UserName", contentcomment.UserId);
            return View(contentcomment);
        }

        //
        // GET: /Comment/Delete/5

        public ActionResult Delete(Guid? id = null)
        {
            if (!id.HasValue) { return HttpNotFound(); }

            ContentComment contentcomment = db.ContentComments.Find(id.Value);
            if (contentcomment == null)
            {
                return HttpNotFound();
            }
            return View(contentcomment);
        }

        //
        // POST: /Comment/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid? id)
        {
            if (!id.HasValue) { return HttpNotFound(); }

            ContentComment contentcomment = db.ContentComments.Find(id.Value);
            db.ContentComments.Remove(contentcomment);
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