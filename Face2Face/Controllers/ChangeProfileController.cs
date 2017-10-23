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

       /* // GET:Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserProfile/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Nationality, Name, Age, Photo")] UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                db.UserProfile.Add(userProfile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userProfile);
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Email,PhoneNumber")] AspNetUsers user)
        {
            if (ModelState.IsValid)
            {
                db.AspNetUsers.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        } */


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
        public ActionResult EditUserProfile([Bind(Include = "Nationality, Name, Age, Photo")] UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {

                




                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userProfile);
        }





    }

}