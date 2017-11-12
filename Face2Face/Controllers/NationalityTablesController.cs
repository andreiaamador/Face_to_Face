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
    public class NationalityTablesController : Controller
    {
        private Face2FaceEntities1 db = new Face2FaceEntities1();

        
        // GET: NationalityTables
        public ActionResult Index()
        {
            return View(db.NationalityTable.ToList());
        }

        
        // GET: NationalityTables/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NationalityTable nationalityTable = db.NationalityTable.Find(id);
            if (nationalityTable == null)
            {
                return HttpNotFound();
            }
            return View(nationalityTable);
        }

        // GET: NationalityTables/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NationalityTables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NationalityID,Nationality")] NationalityTable nationalityTable)
        {
            if (ModelState.IsValid)
            {
                db.NationalityTable.Add(nationalityTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nationalityTable);
        }

        // GET: NationalityTables/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NationalityTable nationalityTable = db.NationalityTable.Find(id);
            if (nationalityTable == null)
            {
                return HttpNotFound();
            }
            return View(nationalityTable);
        }

        // POST: NationalityTables/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NationalityID,Nationality")] NationalityTable nationalityTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nationalityTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nationalityTable);
        }
        // GET: NationalityTables/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NationalityTable nationalityTable = db.NationalityTable.Find(id);
            if (nationalityTable == null)
            {
                return HttpNotFound();
            }
            return View(nationalityTable);
        }
        // POST: NationalityTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            NationalityTable nationalityTable = db.NationalityTable.Find(id);
            db.NationalityTable.Remove(nationalityTable);
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
