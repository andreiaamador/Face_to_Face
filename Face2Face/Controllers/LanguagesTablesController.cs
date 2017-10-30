using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Face2Face.Models;

namespace Face2Face.Controllers
{
    //[Authorize(Roles = "User")]
    public class LanguagesTablesController : Controller
    {
        private Face2FaceEntities1 db = new Face2FaceEntities1();

        // GET: LanguagesTables
        public ActionResult Index()
        {
            return View(db.LanguagesTable.ToList());
        }

        // GET: LanguagesTables/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LanguagesTable languagesTable = db.LanguagesTable.Find(id);
            if (languagesTable == null)
            {
                return HttpNotFound();
            }
            return View(languagesTable);
        }

        // GET: LanguagesTables/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LanguagesTables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LanguageID,Language")] LanguagesTable languagesTable)
        {
            if (ModelState.IsValid)
            {
                db.LanguagesTable.Add(languagesTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(languagesTable);
        }

        // GET: LanguagesTables/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LanguagesTable languagesTable = db.LanguagesTable.Find(id);
            if (languagesTable == null)
            {
                return HttpNotFound();
            }
            return View(languagesTable);
        }

        // POST: LanguagesTables/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LanguageID,Language")] LanguagesTable languagesTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(languagesTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(languagesTable);
        }

        // GET: LanguagesTables/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LanguagesTable languagesTable = db.LanguagesTable.Find(id);
            if (languagesTable == null)
            {
                return HttpNotFound();
            }
            return View(languagesTable);
        }

        // POST: LanguagesTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LanguagesTable languagesTable = db.LanguagesTable.Find(id);
            db.LanguagesTable.Remove(languagesTable);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
