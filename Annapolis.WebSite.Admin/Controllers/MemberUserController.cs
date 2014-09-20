using System;
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
    public class MemberUserController : Controller
    {
        private AdminDbEntities db = new AdminDbEntities();

        private static int PageSize = 50;
        //
        // GET: /MemberUser/

        public ActionResult Index(int? page)
        {
            int pageNumber = 1;
            if (page.HasValue && page.Value > 1) { pageNumber = page.Value; }

            var memberusers = db.MemberUsers.Include(m => m.MemberRole).OrderBy(m => m.UserName);

            ViewBag.SearchKey = string.Empty;
            return View("Index", memberusers.ToPagedList(pageNumber, PageSize));
        }


        [HttpPost]
        public ActionResult Search(string searchKey, int? page)
        { 
            int pageNumber = 1;
            if (page.HasValue && page.Value > 1) { pageNumber = page.Value; }
            
            if(string.IsNullOrWhiteSpace(searchKey)) return Index(pageNumber);

            var lowerSearchKey = searchKey.ToLower();
            var memberusers = db.MemberUsers.Include(m => m.MemberRole)
                            .Where(m => m.UserName.ToLower().Contains(lowerSearchKey) || m.RegisterEmail.ToLower().Contains(lowerSearchKey))
                            .OrderBy(m => m.UserName);
            ViewBag.SearchKey = searchKey;
            return View("Index", memberusers.ToPagedList(pageNumber, PageSize));
        }


        [HttpPost]
        [AjaxOnly]
        public ActionResult UserItem(MemberUser user)
        {
            MemberUser saveUser = db.MemberUsers.Find(user.Id);
            saveUser.IsApproved = user.IsApproved;
            saveUser.IsLockedOut = user.IsLockedOut;
            db.SaveChanges();

            return View("_UserItem", saveUser);
        }


        public ActionResult UserTopics(Guid? Id = null, int? page = null)
        {
            if(Id == null) { return HttpNotFound(); }

            int pageNumber = 1;
            if (page.HasValue && page.Value > 1) { pageNumber = page.Value; }

            var contenttopics = from topic in db.ContentTopics.Include(c => c.ContentThread).Include(c => c.MemberUser)
                                where topic.UserId == Id.Value
                                join comment in db.ContentComments on topic.FirstCommentId equals comment.Id
                                orderby topic.LastUpdateTime
                                select new TopicViewModel
                                {
                                    Topic = topic,
                                    Post = comment
                                };
            ViewBag.UserId = Id.Value;
            return View(contenttopics.ToPagedList(pageNumber, 20));
        }

        //
        // GET: /MemberUser/Details/5

        public ActionResult Details(Guid? id = null)
        {
            MemberUser memberuser = db.MemberUsers.Find(id);
            if (memberuser == null)
            {
                return HttpNotFound();
            }
            return View(memberuser);
        }

        //
        // GET: /MemberUser/Create

        public ActionResult Create()
        {
            ViewBag.RoleId = new SelectList(db.MemberRoles, "Id", "RoleName");
            return View();
        }

        //
        // POST: /MemberUser/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MemberUser memberuser)
        {
            if (ModelState.IsValid)
            {
                memberuser.Id = Guid.NewGuid();
                db.MemberUsers.Add(memberuser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RoleId = new SelectList(db.MemberRoles, "Id", "RoleName", memberuser.RoleId);
            return View(memberuser);
        }

        //
        // GET: /MemberUser/Edit/5

        public ActionResult Edit(Guid? id = null)
        {
            MemberUser memberuser = db.MemberUsers.Find(id);
            if (memberuser == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.MemberRoles, "Id", "RoleName", memberuser.RoleId);
            return View(memberuser);
        }

        //
        // POST: /MemberUser/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MemberUser memberuser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(memberuser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(db.MemberRoles, "Id", "RoleName", memberuser.RoleId);
            return View(memberuser);
        }

        //
        // GET: /MemberUser/Delete/5

        public ActionResult Delete(Guid? id = null)
        {
            MemberUser memberuser = db.MemberUsers.Find(id);
            if (memberuser == null)
            {
                return HttpNotFound();
            }
            return View(memberuser);
        }

        //
        // POST: /MemberUser/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            MemberUser memberuser = db.MemberUsers.Find(id);
            db.MemberUsers.Remove(memberuser);
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