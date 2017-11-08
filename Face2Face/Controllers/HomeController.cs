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
    public class HomeController : Controller
    {
        private Face2FaceEntities1 db = new Face2FaceEntities1();
        public ActionResult Index()
        {

            if (User.Identity.GetUserId() == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("EventsList", "EventTables");
            }

        }

        public ActionResult About()
        {

            return View();
        }

        public ActionResult Contact()
        {
            //ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}