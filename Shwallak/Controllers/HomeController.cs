using Shwallak.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shwallak.Controllers
{
    public class HomeController : Controller
    {
        private OurDB db = new OurDB();
        public ActionResult Index()
        {
            return View(db.Articles.Include(a => a.Comments).Include(a => a.Writer).ToList());
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