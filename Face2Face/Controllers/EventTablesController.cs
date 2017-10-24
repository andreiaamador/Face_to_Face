﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Face2Face.Models;
using Microsoft.AspNet.Identity;

namespace Face2Face.Controllers
{
    public class EventTablesController : Controller
    {
        private Face2FaceEntities1 db = new Face2FaceEntities1();
        ReviewTable reviewTable = new ReviewTable();

        // GET: EventTables
        public ActionResult Index()
        {
            var eventTable = db.EventTable.Include(e => e.LanguagesTable).Include(e => e.UserProfile);
            return View(eventTable.ToList());
        }

        // GET: EventTables/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventTable eventTable = db.EventTable.Find(id);
            if (eventTable == null)
            {
                return HttpNotFound();
            }
            return View(eventTable);
        }

        // GET: EventTables/Create
        public ActionResult Create()
        {
            ViewBag.LanguageID = new SelectList(db.LanguagesTable, "LanguageID", "Language");
            ViewBag.UserID = new SelectList(db.UserProfile, "UserID", "Nationality");
            return View();
        }
        
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID,LanguageID,UserID,Name,Photo,Date,Summary,EndSignUpDate,MaxUsers,Budget,Address")] EventTable eventTable)
        {
            if (ModelState.IsValid)
            {
                db.EventTable.Add(eventTable);
                reviewTable.UserID = Convert.ToInt32(User.Identity.GetUserId());
                reviewTable.EventID = eventTable.EventID;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LanguageID = new SelectList(db.LanguagesTable, "LanguageID", "Language", eventTable.LanguageID);
            ViewBag.UserID = new SelectList(db.UserProfile, "UserID", "Nationality", eventTable.UserID);
            return View(eventTable);
        }

        // GET: EventTables/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventTable eventTable = db.EventTable.Find(id);
            if (eventTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.LanguageID = new SelectList(db.LanguagesTable, "LanguageID", "Language", eventTable.LanguageID);
            ViewBag.UserID = new SelectList(db.UserProfile, "UserID", "Nationality", eventTable.UserID);
            return View(eventTable);
        }

        // POST: EventTables/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventID,LanguageID,UserID,Name,Photo,Date,Summary,EndSignUpDate,MaxUsers,Budget,Address")] EventTable eventTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LanguageID = new SelectList(db.LanguagesTable, "LanguageID", "Language", eventTable.LanguageID);
            ViewBag.UserID = new SelectList(db.UserProfile, "UserID", "Nationality", eventTable.UserID);
            return View(eventTable);
        }

        //GET: EventTables/Add Review
        public ActionResult AddReview([Bind(Include = "Review")] ReviewTable reviewTable)
        {
            if (ModelState.IsValid)
            {
            db.Entry(reviewTable).State = EntityState.Added;
            db.SaveChanges();
            return RedirectToAction("Index");
            }
            return View();
        }

        // GET: EventTables/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventTable eventTable = db.EventTable.Find(id);
            if (eventTable == null)
            {
                return HttpNotFound();
            }
            return View(eventTable);
        }

        // POST: EventTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventTable eventTable = db.EventTable.Find(id);
            db.EventTable.Remove(eventTable);
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
