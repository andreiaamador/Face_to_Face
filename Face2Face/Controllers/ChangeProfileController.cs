using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Face2Face.Models;
using System.Net;

namespace Face2Face.Controllers
{
    public class ChangeProfileController : Controller
    {

        private Face2FaceEntities1 db = new Face2FaceEntities1();

        // GET: ChangeProfile
        public ActionResult Index()
        {
            return View();
        }

        // GET: Edit1
        public ActionResult EditUserProfile(int? id)
        {
            UserProfile userProfile = db.UserProfile.Find(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            return View(userProfile);
        }

        // POST : Edit1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserProfile([Bind(Include = "Name, Age, Photo,Nationality, PhoneNumber, Email, NativeLanguage, FluentLanguage, InterestedLanguage")] ChangeProfile changeProfile)
        {
            if (ModelState.IsValid)
            {

                db.Entry(changeProfile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(changeProfile);
        }





    }

}