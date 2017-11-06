using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Face2Face.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;

namespace Face2Face.Controllers
{
    public class UserProfilesController : Controller
    {
        private Face2FaceEntities1 db = new Face2FaceEntities1();

        // GET: UserProfiles
        public ActionResult Index()
        {
            var userProfile = db.UserProfile.Include(u => u.AspNetUsers);
            return View(userProfile.ToList());
        }

        // GET: UserProfiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProfile userProfile = db.UserProfile.Find(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            return View(userProfile);
        }

        // GET: UserProfiles/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: UserProfiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,Nationality,Name,Age,Photo")] UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                db.UserProfile.Add(userProfile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", userProfile.UserID);
            return View(userProfile);
        }

        // GET: UserProfiles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProfile userProfile = db.UserProfile.Find(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", userProfile.UserID);
            return View(userProfile);
        }

        // POST: UserProfiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,Nationality,Name,Age,Photo")] UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userProfile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", userProfile.UserID);
            return View(userProfile);
        }

        // GET: UserProfiles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProfile userProfile = db.UserProfile.Find(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            return View(userProfile);
        }

        // POST: UserProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserProfile userProfile = db.UserProfile.Find(id);
            AspNetUsers aspNetUser = db.AspNetUsers.Find(id);
            db.UserProfile.Remove(userProfile);
            db.AspNetUsers.Remove(aspNetUser);
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

        public async Task<ActionResult> GetProfileClassificationAsync(int? id)
        {
            DbHelper helper = new DbHelper();
            var classification = await helper.GetProfileClassificationAsync(id);
            return View("ClassificationPartial", classification);
        }

        // GET: UserProfiles
        public ActionResult GetOwnProfile()
        {
            int userLog = Convert.ToInt32(User.Identity.GetUserId());
            return View("ProfilePartial", db.UserProfile.Find(userLog));
        }
    }

    public class DbHelper
    {
        private Face2FaceEntities1 db = new Face2FaceEntities1();

        public async Task<double?> GetProfileClassificationAsync(int? id)
        {
            ObjectParameter x = new ObjectParameter("x", typeof(double));
            db.sp_ProfileClassification(id,  x);

            if (!x.Value.Equals(System.DBNull.Value)) {
                return Convert.ToDouble(x.Value);
            }

            else {
                return null;
            }
            db.sp_ProfileClassification(13,  x);
            var y = x.Value;


            return Convert.ToDouble(x.Value);

            //double classification = 0;
            //int count = 0;

            //foreach (var evenT in db.EventTable)
            //{
            //    if (evenT.UserID == id)
            //    {
            //        foreach (var review in evenT.ReviewTable)
            //        {
            //            classification = classification + review.Classification;
            //            count++;
            //        }
            //    }
            //}
            //if (count != 0)
            //{
            //    return (double)classification / count;
            //}
            //else
            //{
            //    return (double)0;
            //}
>>>>>>> origin/Andreia
        }
    }
}
