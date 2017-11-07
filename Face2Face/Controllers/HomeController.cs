using Face2Face.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Microsoft.AspNet.Identity;

namespace Face2Face.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            if (User.Identity.GetUserId() == null)
            {
                return View();
            }
            else {
                return RedirectToAction("EventsList", "EventTables");
            }
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
         
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}