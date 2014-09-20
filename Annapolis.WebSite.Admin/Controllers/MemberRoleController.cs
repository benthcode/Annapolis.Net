using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Annapolis.WebSite.Admin.Models;

namespace Annapolis.WebSite.Admin.Controllers
{
    public class MemberRoleController : Controller
    {
        private AdminDbEntities db = new AdminDbEntities();

        //
        // GET: /MemberRole/

        public ActionResult Index()
        {
            return View(db.MemberRoles.ToList());
        }

        //
        // GET: /MemberRole/Details/5

        public ActionResult Details(Guid? id = null)
        {
            MemberRole memberrole = db.MemberRoles.Find(id);
            if (memberrole == null)
            {
                return HttpNotFound();
            }
            return View(memberrole);
        }

        //
        // GET: /MemberRole/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /MemberRole/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MemberRole memberrole)
        {
            if (ModelState.IsValid)
            {
                memberrole.Id = Guid.NewGuid();
                db.MemberRoles.Add(memberrole);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(memberrole);
        }

        //
        // GET: /MemberRole/Edit/5

        public ActionResult Edit(Guid? id = null)
        {
            MemberRole memberrole = db.MemberRoles.Find(id);
            if (memberrole == null)
            {
                return HttpNotFound();
            }
            return View(memberrole);
        }

        //
        // POST: /MemberRole/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MemberRole memberrole)
        {
            if (ModelState.IsValid)
            {
                db.Entry(memberrole).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(memberrole);
        }

        //
        // GET: /MemberRole/Delete/5

        public ActionResult Delete(Guid? id = null)
        {
            MemberRole memberrole = db.MemberRoles.Find(id);
            if (memberrole == null)
            {
                return HttpNotFound();
            }
            return View(memberrole);
        }

        //
        // POST: /MemberRole/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            MemberRole memberrole = db.MemberRoles.Find(id);
            db.MemberRoles.Remove(memberrole);
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