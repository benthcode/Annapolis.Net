using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Annapolis.WebSite.Admin.Models;

namespace Annapolis.WebSite.Admin.Controllers
{
    public class NotificationController : Controller
    {
        private AdminDbEntities db = new AdminDbEntities();

        //
        // GET: /Notification/

        public ActionResult Index()
        {
            var settings = db.Settings.Include(s => s.LocaleLanguage).Include(s => s.MemberRole).Include(s => s.MemberUser);
            return View(settings.ToList());
        }

        //
        // GET: /Notification/Details/5

        public ActionResult Details(Guid? id = null)
        {
            Setting setting = db.Settings.Find(id);
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        //
        // GET: /Notification/Create

        public ActionResult Create()
        {
            ViewBag.LanguageId = new SelectList(db.LocaleLanguages, "Id", "Name");
            ViewBag.NewMemberStartRoleId = new SelectList(db.MemberRoles, "Id", "RoleName");
            ViewBag.SuperAdminUserId = new SelectList(db.MemberUsers, "Id", "UserName");
            return View();
        }

        //
        // POST: /Notification/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Setting setting)
        {
            if (ModelState.IsValid)
            {
                setting.Id = Guid.NewGuid();
                db.Settings.Add(setting);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LanguageId = new SelectList(db.LocaleLanguages, "Id", "Name", setting.LanguageId);
            ViewBag.NewMemberStartRoleId = new SelectList(db.MemberRoles, "Id", "RoleName", setting.NewMemberStartRoleId);
            ViewBag.SuperAdminUserId = new SelectList(db.MemberUsers, "Id", "UserName", setting.SuperAdminUserId);
            return View(setting);
        }

        //
        // GET: /Notification/Edit/5

        public ActionResult Edit(Guid? id = null)
        {
            Setting setting = db.Settings.Find(id);
            if (setting == null)
            {
                return HttpNotFound();
            }
            ViewBag.LanguageId = new SelectList(db.LocaleLanguages, "Id", "Name", setting.LanguageId);
            ViewBag.NewMemberStartRoleId = new SelectList(db.MemberRoles, "Id", "RoleName", setting.NewMemberStartRoleId);
            ViewBag.SuperAdminUserId = new SelectList(db.MemberUsers, "Id", "UserName", setting.SuperAdminUserId);
            return View(setting);
        }

        //
        // POST: /Notification/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Setting setting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(setting).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LanguageId = new SelectList(db.LocaleLanguages, "Id", "Name", setting.LanguageId);
            ViewBag.NewMemberStartRoleId = new SelectList(db.MemberRoles, "Id", "RoleName", setting.NewMemberStartRoleId);
            ViewBag.SuperAdminUserId = new SelectList(db.MemberUsers, "Id", "UserName", setting.SuperAdminUserId);
            return View(setting);
        }

        //
        // GET: /Notification/Delete/5

        public ActionResult Delete(Guid? id = null)
        {
            Setting setting = db.Settings.Find(id);
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        //
        // POST: /Notification/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Setting setting = db.Settings.Find(id);
            db.Settings.Remove(setting);
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