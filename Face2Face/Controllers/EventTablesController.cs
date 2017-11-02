using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Face2Face.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using System.Data.SqlClient;
using System.IO;

namespace Face2Face.Controllers
{
    public class EventTablesController : Controller
    {
        private Face2FaceEntities1 db = new Face2FaceEntities1();

        // GET: EventTables
        public ActionResult EventsList()
        {
            var eventTable = db.EventTable.Include(e => e.LanguagesTable).Include(e => e.UserProfile);
            return View(eventTable.ToList());
        }

        public ActionResult MyOwnEvents()
        {
            int userID = Convert.ToInt32(User.Identity.GetUserId());

            string sqlQuery = string.Format("{0}{1}", "select * from EventTable where UserID=", userID);
            var eventTable = db.EventTable.SqlQuery(sqlQuery);

            return View("EventsList", eventTable.ToList());
        }

        public ActionResult NextEvents() {
            int userID = Convert.ToInt32(User.Identity.GetUserId());

            var eventTable = db.EventTable;

            //////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////
            List<EventTable> eventos= new List<EventTable>();
            foreach (var evnt in eventTable)
            {
                foreach (var part in evnt.UserProfile1) {
                    if (part.UserID == userID) {
                        eventos.Add(evnt);
                        break;
                    }
                }
            }

            return View("EventsList", eventos.ToList());
        }

        // POST:
        [HttpPost]
        public ActionResult Filter(string Location, string Date, string Language, string keyWord)
        {
            //List<EventTable> eventTable = new List<EventTable>();
            string sqlQuery;
            int languageID = 0;

            if (Location != "" || Date != "" || Language != "--Select--" || keyWord != "")
            {
                sqlQuery = "select * from EventTable where";
                if (Location != "")
                {
                    sqlQuery = string.Format("{0} {1}", sqlQuery, "lower(Address) like lower('%" + Location + "%')");
                }

                if (Language != "--Select--")
                {
                    foreach (var item in db.LanguagesTable)
                    {
                        if (item.Language == Language)
                        {
                            languageID = item.LanguageID;
                            break;
                        }
                    }

                    if (Location == "")
                    {
                        sqlQuery = string.Format("{0} {1}", sqlQuery, "LanguageID = " + languageID + "");
                    }
                    else
                    {
                        sqlQuery = string.Format("{0} {1}", sqlQuery, "and LanguageID = " + languageID + "");
                    }
                }

                if (Date != "")
                {
                    if (Location == "" && Language == "--Select--")
                    {
                        sqlQuery = string.Format("{0} {1}", sqlQuery, "lower(Date) like lower('%" + Date + "%')");
                    }
                    else
                    {
                        sqlQuery = string.Format("{0} {1}", sqlQuery, "and lower(Date) like lower('%" + Date + "%')");
                    }
                }

                if (keyWord != "")
                {
                    if (Location == "" && Language == "--Select--" && Date == "")
                    {
                        sqlQuery = string.Format("{0} {1}", sqlQuery, "lower(Name) like lower('%" + keyWord + "%') or lower(Summary) like lower('%" + keyWord + "%')");
                    }
                    else
                    {
                        sqlQuery = string.Format("{0} {1}", sqlQuery, "and lower(Name) like lower('%" + keyWord + "%') or lower(Summary) like lower('%" + keyWord + "%')");
                    }
                }

                var eventTable = db.EventTable.SqlQuery(sqlQuery);

                return View("EventsList", eventTable.ToList());
            }
            else
            {
                return View("EventsList", db.EventTable.ToList());
            }
        }


        public ActionResult GoToEvent(int id)
        {
            int userLog = Convert.ToInt32(User.Identity.GetUserId());
            var eventTable = db.EventTable.Find(id);
            UserProfile Profile = db.UserProfile.Find(userLog);

            if (eventTable.UserProfile1.Contains(Profile))
            {
                eventTable.UserProfile1.Remove(Profile);
            }
            else
            {
                eventTable.UserProfile1.Add(Profile);
            }

            db.Entry(eventTable).State = EntityState.Modified;
            db.SaveChanges();

            //ViewBag.userLog = userLog;
            //ViewBag.userInEvent = IsThisUserInEvent(eventTable.UserProfile1, ViewBag.userLog);
      
            return View("EventsList", db.EventTable);
        }

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

           
            ViewBag.userLog = Convert.ToInt32(User.Identity.GetUserId());
            ViewBag.userInEvent = IsThisUserInEvent(eventTable.UserProfile1, ViewBag.userLog);

            return View(eventTable);
        }

        // GET: EventTables/Create
        public ActionResult Create()
        {
            ViewBag.LanguageID = new SelectList(db.LanguagesTable, "LanguageID", "Language");
            ViewBag.UserID = new SelectList(db.UserProfile, "UserID", "Nationality");
            return View();
        }

        // POST: EventTables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID,LanguageID,Name,Date,Summary,EndSignUpDate,MaxUsers,Budget,Address")] EventTable eventTable, HttpPostedFileBase photo, string releaseDate, string endSignUpDate)
        {
            if (ModelState.IsValid)
            {
                eventTable.UserID = Convert.ToInt32(User.Identity.GetUserId());
                if (photo != null && photo.ContentLength > 0)
                    try
                    {
                        string path = Path.Combine(Server.MapPath("~/UploadedFiles/"), Path.GetFileName(photo.FileName));
                        photo.SaveAs(path);
                        eventTable.Photo = "/UploadedFiles/" + Path.GetFileName(photo.FileName);
                        eventTable.Date = Convert.ToDateTime(releaseDate);
                        eventTable.EndSignUpDate = Convert.ToDateTime(endSignUpDate);


                        db.Entry(eventTable).State = EntityState.Modified;
                        db.SaveChanges();
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                db.EventTable.Add(eventTable);
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
        public ActionResult Edit([Bind(Include = "EventID,LanguageID,UserID,Name,Date,Summary,EndSignUpDate,MaxUsers,Budget,Address")] EventTable eventTable, HttpPostedFileBase photo, string releaseDate, string endSignUpDate)
        {
            if (ModelState.IsValid)
            {
                eventTable.UserID = Convert.ToInt32(User.Identity.GetUserId());

                if (photo != null && photo.ContentLength > 0)
                    try
                    {
                        string path = Path.Combine(Server.MapPath("~/UploadedFiles/"), Path.GetFileName(photo.FileName));
                        photo.SaveAs(path);
                        eventTable.Photo = "/UploadedFiles/" + Path.GetFileName(photo.FileName);

                        db.Entry(eventTable).State = EntityState.Modified;
                        db.SaveChanges();
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                eventTable.Date = Convert.ToDateTime(releaseDate);
                eventTable.EndSignUpDate = Convert.ToDateTime(endSignUpDate);

                db.Entry(eventTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LanguageID = new SelectList(db.LanguagesTable, "LanguageID", "Language", eventTable.LanguageID);
            ViewBag.UserID = new SelectList(db.UserProfile, "UserID", "Nationality", eventTable.UserID);
            return View(eventTable);
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

        public ActionResult UploadFile()
        {
            return View();
        }


        public bool IsThisUserInEvent(ICollection<UserProfile> users, int userLog) {

            bool isIn = false;
            foreach (var user in users)
            {
                if (user.UserID == userLog)
                {
                    isIn = true;
                    break;
                }
            }

            return isIn;
        }

        public EventTable ArrangeEvents(EventTable eventTable) {

         
            return eventTable;
        }
    }
}
