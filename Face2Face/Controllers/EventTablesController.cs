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
        //internal static object cs;

        // GET: EventTables
        public ActionResult EventsList()
        {
            //var eventTable = db.EventTable.Include(e => e.LanguagesTable).Include(e => e.UserProfile);
            //,eventTable.ToList()


            List<string> allList = new List<String>();
            foreach (var lang in db.LanguagesTable) {
                allList.Add(lang.Language);
            }

            ViewBag.allLanguages = allList;
            return View("EventsList");
        }

        public ActionResult GetAllEventsList()
        {
            var eventTable = db.EventTable.Include(e => e.LanguagesTable).Include(e => e.UserProfile).OrderBy(e => e.Date);
            return View("_allEvents", eventTable.ToList());
        }

        public PartialViewResult MyOwnEvents()
        {
            int userID = Convert.ToInt32(User.Identity.GetUserId());
            string sqlQuery = string.Format("{0}{1}", "select * from EventTable where UserID=", userID);
            var eventTable = db.EventTable.SqlQuery(sqlQuery);
            return PartialView("_MyOwnEvents", eventTable.OrderBy(e => e.Date).ToList());
        }

        public PartialViewResult MyNextEvents()
        {
            int userID = Convert.ToInt32(User.Identity.GetUserId());

            var eventTable = db.EventTable;

            //////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////
            List<EventTable> eventos = new List<EventTable>();
            foreach (var evnt in eventTable)
            {
                foreach (var part in evnt.UserProfile1)
                {
                    if (part.UserID == userID)
                    {
                        eventos.Add(evnt);
                        break;
                    }
                }
            }

            return PartialView("_MyOwnEvents", eventos.OrderBy(e => e.Date).ToList());
        }

        // POST:
        [HttpPost]
        public PartialViewResult Filter(string Location, string Date, string Language, string keyWord)
        {
            //List<EventTable> eventTable = new List<EventTable>();
            string sqlQuery;
            int languageID = 0;

            if (Location != "" || Date != "" || (Language != "--Select--" && Language !=null) || keyWord != "")
            {
                sqlQuery = "select * from EventTable where";
                if (Location != "")
                {
                    sqlQuery = string.Format("{0} {1}", sqlQuery, "lower(Address) like lower('%" + Location + "%')");
                }

                if (Language != "--Select--" && Language != null)
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
                    if (Location == "" && Language == "--Select--" && Language != null)
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
                    if (Location == "" && Language == "--Select--" && Language != null && Date == "")
                    {
                        sqlQuery = string.Format("{0} {1}", sqlQuery, "lower(Name) like lower('%" + keyWord + "%') or lower(Summary) like lower('%" + keyWord + "%')");
                    }
                    else
                    {
                        sqlQuery = string.Format("{0} {1}", sqlQuery, "and lower(Name) like lower('%" + keyWord + "%') or lower(Summary) like lower('%" + keyWord + "%')");
                    }
                }

                var eventTable = db.EventTable.SqlQuery(sqlQuery);


                if (Date == "")
                {
                    return PartialView("_allEvents", eventTable.OrderBy(e => e.Date).ToList());
                }
                else {
                    return PartialView("_allEvents", eventTable);
                }
            }
            else
            {
                return PartialView("_allEvents", db.EventTable.OrderBy(e => e.Date).ToList());
            }
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

            int userLog = Convert.ToInt32(User.Identity.GetUserId());
            ViewBag.userLog = userLog;
            ViewBag.userInEvent = eventTable.UserProfile1.Contains(db.UserProfile.Find(userLog));
            ViewBag.isOnReviews = false;


            ViewBag.isOnReviews = false;
            foreach (var rev in db.ReviewTable) {
                if (rev.UserID== userLog) {
                    ViewBag.isOnReviews = true;
                }

            }
           

            //if (db.ReviewTable.Find(id, userLog) != null)
            //{
            //    ViewBag.isOnReviews = true;
            //}
            //else
            //{
            //    ViewBag.isOnReviews = false;
            //}
            ViewBag.Chat = db.MessageTable.Where(c => c.EventID == id).ToList();

            return View(eventTable);
        }

        public ActionResult GetParticipants(int eventID)
        {
            return View("_ParticipantsDetails", db.EventTable.Find(eventID));
        }

        public PartialViewResult GoToEvent(int EventID)
        {
            int userLog = Convert.ToInt32(User.Identity.GetUserId());
            var eventTable = db.EventTable.Find(EventID);
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

            return PartialView("_ParticipantsDetails", eventTable);
        }

        public ActionResult GetReviews(int eventID)
        {
            ViewBag.userLog = Convert.ToInt32(User.Identity.GetUserId());
            return View("_ReviewsPartial", db.EventTable.Find(eventID));
        }

        [HttpPost]
        public PartialViewResult AddReviews(int EventID, int classification, string review)
        {
            int userLog = Convert.ToInt32(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                ReviewTable reviewTable = new ReviewTable
                {
                    EventID = EventID,
                    UserID = userLog,
                    Classification = classification,
                    Review = review
                };

                db.ReviewTable.Add(reviewTable);
                db.Entry(reviewTable).State = EntityState.Added;
                db.SaveChanges();
            }

            EventTable eventTable = db.EventTable.Find(EventID);
            ViewBag.userLog = userLog;
            ViewBag.userInEvent = eventTable.UserProfile1.Contains(db.UserProfile.Find(userLog));

            ViewBag.isOnReviews = true;
            //if (db.ReviewTable.Find(EventID, userLog) != null)
            //{
            //    ViewBag.isOnReviews = true;
            //}
            //else
            //{
            //    ViewBag.isOnReviews = false;
            //}
            return PartialView("_ReviewsPartial", eventTable);
        }

        public ActionResult RemoveReviews(int eventID, int userID)
        {
            if (ModelState.IsValid)
            {

                int userLog = Convert.ToInt32(User.Identity.GetUserId());

                //if (!db.EventTable.Find(eventID).UserProfile1.Contains(db.UserProfile.Find(userLog)))
                //{

                //var reviewTable = db.ReviewTable.Find(eventID, userID);
                //db.ReviewTable.Remove(reviewTable);

                ViewBag.isOnReviews = true;
                foreach (var rev in db.ReviewTable) {
                    if (rev.UserID==userLog) {
                        db.ReviewTable.Remove(rev);
                        ViewBag.isOnReviews = false;
                        break;
                    }
                }

                //db.Entry(db.ReviewTable).State = EntityState.Deleted;
                db.SaveChanges();
                //}

                ViewBag.userLog = userLog;
                ViewBag.userInEvent = db.EventTable.Find(eventID).UserProfile1.Contains(db.UserProfile.Find(userLog));


                //if (db.ReviewTable.Find(eventID, userLog) != null)
                //{
                //    ViewBag.isOnReviews = true;
                //}
                //else
                //{
                //    ViewBag.isOnReviews = false;
                //}

            }
            return View("Details", db.EventTable.Find(eventID));
        }

        //GET
        public ActionResult ChatView(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MessageTable messageTable = db.MessageTable.Find(id);
            if (messageTable == null)
            {
                return HttpNotFound();
            }

            int userLog = Convert.ToInt32(User.Identity.GetUserId());
            ViewBag.userLog = userLog;
            ViewBag.userInEvent = db.EventTable.Find(id).UserProfile1.Contains(db.UserProfile.Find(userLog));

            if (db.ReviewTable.Find(id, userLog) != null)
            {
                ViewBag.isOnReviews = true;
            }
            else
            {
                ViewBag.isOnReviews = false;
            }

            return View("Details", db.EventTable.Find(id));
        }

        [HttpPost]
        public ActionResult ChatView([Bind(Include = "MessageID,EventID,UserID,Message")] MessageTable messageTable, string message, int? id)
        {
            if (ModelState.IsValid)
            {
                messageTable.UserID = Convert.ToInt32(User.Identity.GetUserId());
                messageTable.EventID = (int)id;

                db.MessageTable.Add(messageTable);
                db.Entry(messageTable).State = EntityState.Added;
                db.SaveChanges();

                ViewBag.user = db.UserProfile.Find(Convert.ToInt32(User.Identity.GetUserId())).Name;
            }

            int userLog = Convert.ToInt32(User.Identity.GetUserId());
            ViewBag.userLog = userLog;
            ViewBag.userInEvent = db.EventTable.Find(id).UserProfile1.Contains(db.UserProfile.Find(userLog));

            if (db.ReviewTable.Find(id, userLog) != null)
            {
                ViewBag.isOnReviews = true;
            }
            else
            {
                ViewBag.isOnReviews = false;
            }

            return View("Details", db.EventTable.Find(id));
            //return PartialView("_ChatView", db.MessageTable.Where(c => c.EventID == userLoggedIn).ToList());
        }
        
        //[HttpPost]
        //public ActionResult ChatView(int id, string message)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        int userLog = Convert.ToInt32(User.Identity.GetUserId());
        //        ChatTable chatTable = new ChatTable
        //        {
        //            EventID = id,
        //            UserID = userLog,
        //            ChatEntry = message,
        //        };

        //        db.ChatTable.Add(chatTable);
        //        db.Entry(chatTable).State = EntityState.Added;
        //        db.SaveChanges();

        //        ViewBag.user = userLog;
        //        return RedirectToAction("ChatView", db.ChatTable);
        //    }
        //    else
        //    {
        //        return View("EventList", db.EventTable);
        //    }
        //}

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

        public ActionResult Create([Bind(Include = "EventID,LanguageID,Name,Date,Summary,EndSignUpDate,MaxUsers,Budget,Address")] EventTable eventTable, HttpPostedFileBase photo, string releaseDate, string endSignUpDate, string Address, string hour)
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

                //eventTable.Date = Convert.ToDateTime(releaseDate+" "+hour+":00.00");
                eventTable.Date = Convert.ToDateTime(releaseDate);
                eventTable.EndSignUpDate = Convert.ToDateTime(endSignUpDate);

                eventTable.Address = Address;

                eventTable.UserProfile1.Add(db.UserProfile.Find(Convert.ToInt32(User.Identity.GetUserId())));
                db.EventTable.Add(eventTable);

                db.SaveChanges();
                return RedirectToAction("EventsList");
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

        public ActionResult Edit([Bind(Include = "EventID,LanguageID,UserID,Name,Date,Summary,EndSignUpDate,MaxUsers,Budget,Address")] EventTable eventTable, HttpPostedFileBase photo, string releaseDate, string endSignUpDate, string Address)

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
                eventTable.Address = Address;

                db.Entry(eventTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("EventsList");
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


            db.EventTable.Find(id).UserProfile1.Clear();
            db.EventTable.Remove(eventTable);


            db.SaveChanges();
            return RedirectToAction("EventsList");
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

        public EventTable ArrangeEvents(EventTable eventTable)
        {
            return eventTable;
        }

        public double GetEventClassification(int? id)
        {

            var reviewTable = db.EventTable.Find(id).ReviewTable;
            double eventReviews = 0;
            int count = 0;
            if (reviewTable.Count != 0)
            {
                foreach (var rev in reviewTable)
                {
                    eventReviews = eventReviews + rev.Classification;
                    count++;
                }
                return (double)eventReviews / count;
            }

            else
            {
                return (double)0;
            }
        }
    }
}
