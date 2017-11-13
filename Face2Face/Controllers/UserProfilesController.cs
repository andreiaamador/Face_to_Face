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

        private UserManager<ApplicationUser, int> userManager = new UserManager<ApplicationUser, int>(new CustomUserStore(new ApplicationDbContext()));

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
            ViewBag.Roles = new SelectList(db.AspNetRoles, "Name", "Name");
            return View();
        }

        // POST: UserProfiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model, string roleName)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, roleName);
                    UserProfile newUser = new UserProfile();
                    newUser.UserID = user.Id;
                    db.UserProfile.Add(newUser);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }

            }

            ViewBag.Roles = new SelectList(db.AspNetRoles, "Name", "Name");
            return View(model);
        }

        // GET: UserProfiles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //UserProfile userProfile = db.UserProfile.Find(id);
            UserProfile userProfile = db.UserProfile.Include("AspNetUsers.AspNetRoles").FirstOrDefault(u => u.UserID == id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }

            //esta linha é para ir buscar o Role actual da pessoa, para ficar seleccionado na dropdown
            //senão sempre que entrasses na página ia estar o valor Admin escolhido, em vez do actual da conta
            var userRole = userProfile.AspNetUsers.AspNetRoles.First();

            ViewBag.Roles = new SelectList(db.AspNetRoles, "Name", "Name", userRole.Name);
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", userProfile.UserID);
            return View(userProfile);
        }

        // POST: UserProfiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,Nationality,Name,Age,Photo")] UserProfile userProfile, String roleName)
        {
            if (ModelState.IsValid)
            {
                //alterar o role do utilizador
                var role = db.AspNetRoles.FirstOrDefault(r => r.Name == roleName);
                if (role == null)
                {
                    return HttpNotFound();
                }
                var userRoles = userManager.GetRoles(userProfile.UserID);
                foreach (string r in userRoles)
                {
                    userManager.RemoveFromRole(userProfile.UserID, r);
                }
                userManager.AddToRole(userProfile.UserID, role.Name);

                db.Entry(userProfile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Roles = new SelectList(db.AspNetRoles, "Name", "Name");
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
            db.UserProfile.Remove(userProfile);
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

        public ActionResult GetProfileClassificationAsync(int? id)
        {
            DbHelper helper = new DbHelper();
            var classification = helper.GetProfileClassificationAsync(id);
            return View("ClassificationPartial", classification);
        }

        // GET: UserProfiles
        public ActionResult GetOwnProfile()
        {
            int userLog = Convert.ToInt32(User.Identity.GetUserId());
            return View("ProfilePartial", db.UserProfile.Find(userLog));
        }

        public ActionResult GetProfileTopBar()
        {
            int userLog = Convert.ToInt32(User.Identity.GetUserId());
            return View("_ProfileTopBar", db.UserProfile.Find(userLog));
        }
    }

    public class DbHelper
    {
        private Face2FaceEntities1 db = new Face2FaceEntities1();

        public double? GetProfileClassificationAsync(int? id)
        {
            ObjectParameter x = new ObjectParameter("x", typeof(double));
            db.sp_ProfileClassification(id, x);

            if (!x.Value.Equals(System.DBNull.Value)) {
                return Convert.ToDouble(x.Value);
            }

            else {
                return null;
            }
        }
    }
}
