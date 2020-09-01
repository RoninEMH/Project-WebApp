using Newtonsoft.Json;
using Shwallak.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace Shwallak.Controllers
{
    public class HomeController : Controller
    {
        private OurDB db = new OurDB();
        private string messege;
        public ActionResult Statistics()
        {
            var csv1 = new StringBuilder();

            csv1.AppendLine("section,count");
            foreach (var obj in db.Articles.GroupBy(x => x.Section).Select(x => new { Count = x.Count(), x.Key }).ToList())
            {
                var first = (int)obj.Key;
                var second = obj.Count;
                var newLine = string.Format("{0},{1}", first, second);
                csv1.AppendLine(newLine);
            }

            string path1 = HttpRuntime.AppDomainAppPath;

            int a1 = path1.IndexOf("\\Shwallak");
            path1 = path1.Substring(0, a1) + "\\Shwallak\\Content\\data1.csv";

            System.IO.File.WriteAllText(path1, csv1.ToString());

            int max = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            int last = 0;

            var csv2 = new StringBuilder();

            csv2.AppendLine("day,count");
            foreach (var obj in db.Articles.Where(x=>x.Month==DateTime.Now.Month).GroupBy(x => x.Day).Select(x => new { Count = x.Count(), x.Key }).ToList())
            {
                if((int)obj.Key != last+1)
                {
                    for(int i = last+1;i< (int)obj.Key;i++)
                    {
                        csv2.AppendLine(string.Format("{0},{1}", i, 0));
                    }
                }
                last = (int)obj.Key;
                var first = (int)obj.Key;
                var second = obj.Count;
                var newLine = string.Format("{0},{1}", first, second);
                csv2.AppendLine(newLine);
            }
            for(int i=last+1;i<=max;i++)
            {
                csv2.AppendLine(string.Format("{0},{1}", i, 0));
            }

            string path2 = HttpRuntime.AppDomainAppPath;

            int a2 = path2.IndexOf("\\Shwallak");
            path2 = path2.Substring(0, a2) + "\\Shwallak\\Content\\data2.csv";

            System.IO.File.WriteAllText(path2, csv2.ToString());

            int month = DateTime.Now.Month - 1;
            if (month == 0)
                month = 12;
            int max2 = DateTime.DaysInMonth(DateTime.Now.Year, month);
            int last2 = 0;

            var csv3 = new StringBuilder();

            csv3.AppendLine("day,count");
            foreach (var obj in db.Articles.Where(x => x.Month == month).GroupBy(x => x.Day).Select(x => new { Count = x.Count(), x.Key }).ToList())
            {
                if ((int)obj.Key != last2 + 1)
                {
                    for (int i = last2 + 1; i < (int)obj.Key; i++)
                    {
                        csv3.AppendLine(string.Format("{0},{1}", i, 0));
                    }
                }
                last2 = (int)obj.Key;
                var first2 = (int)obj.Key;
                var second2 = obj.Count;
                var newLine2 = string.Format("{0},{1}", first2, second2);
                csv3.AppendLine(newLine2);
            }
            for (int i = last2 + 1; i <= max2; i++)
            {
                csv3.AppendLine(string.Format("{0},{1}", i, 0));
            }

            string path3 = HttpRuntime.AppDomainAppPath;

            int a3 = path3.IndexOf("\\Shwallak");
            path3 = path3.Substring(0, a3) + "\\Shwallak\\Content\\data3.csv";

            System.IO.File.WriteAllText(path3, csv3.ToString());
            return View();
        }
        public ActionResult Index()
        {
            if (Session["type"] != null && Session["type"].Equals("subscriber"))
                ViewBag.favorite = db.Subscribers.Find(Session["id"]).Favor();

            return View(db.Articles.Include(a => a.Comments).Include(a => a.Writer).ToList());
        }

        public ActionResult About()
        {
            ViewBag.priv = 0;
            ViewBag.pub = 0;
            foreach(var obj in db.Articles.GroupBy(x => x.SubscribersOnly).Select(x => new { Key = x.Key, Count = x.Count() }).ToList())
            {
                if(obj.Key)
                {
                    ViewBag.priv = obj.Count;
                }
                else
                {
                    ViewBag.pub = obj.Count;
                }
            }
            return View(db.Articles);
        }

        public ActionResult Contact()
        {
            return View();
        }

        private ActionResult LoginAsSubscriber(string userName, string password)
        {
            if (Session["type"] != null && !Session["type"].Equals("none"))
                return RedirectToAction("Index", "Home");
            List<Subscriber> subscribers = new List<Subscriber>();
            foreach (Subscriber s in db.Subscribers)
            {
                if (s.Nickname.Equals(userName))
                    subscribers.Add(s);
            }
            if (subscribers.Count() != 1)
            {
                messege = "invalid username";
                if (subscribers.Count() == 0)
                    messege = "username not exist, consider sign up";

                TempData["messege"] = messege;
                return RedirectToAction("LoginBy");
            }
            Subscriber wanted = subscribers.First(); //user name supposed to be key
            if (!wanted.Password.Equals(password))
            {
                messege = "invalid password";
                TempData["messege"] = messege;
                return RedirectToAction("LoginBy");
            }

            Session["id"] = wanted.SubscriberID;
            Session["username"] = wanted.Nickname;
            Session["type"] = "subscriber";
            return RedirectToAction("Index", "Home");
        }


        private ActionResult LoginAsWriter(string userName, string password)
        {
            if (Session["type"] != null && !Session["type"].Equals("none"))
                return RedirectToAction("Index", "Home");
            List<Writer> writers = new List<Writer>();
            foreach (Writer w in db.Writers)
            {
                if (w.FullName.Equals(userName))
                    writers.Add(w);
            }
            if (writers.Count() != 1)
            {
                messege = "invalid username";
                TempData["messege"] = messege;
                return RedirectToAction("LoginBy");
            }
            Writer wanted = writers.First(); //user name supposed to be key
            if (!wanted.Password.Equals(password))
            {
                messege = "invalid password";
                TempData["messege"] = messege;
                return RedirectToAction("LoginBy");
            }
            Session["id"] = wanted.WriterID;
            Session["username"] = wanted.FullName;
            Session["type"] = "writer";
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Login(string userName, string password, int? type)
        {
            if (Session["type"] != null && !Session["type"].Equals("none"))
                return RedirectToAction("Index", "Home");
            if (userName == null || userName.Equals(""))
            {
                messege = "you must enter username";
                TempData["messege"] = messege;
                return RedirectToAction("LoginBy");
            }
            if (password == null || password.Equals(""))
            {
                messege = "you must enter password";
                TempData["messege"] = messege;
                return RedirectToAction("LoginBy");
            }
            /*if (userName.Contains(" "))
                userName = userName.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray().ToString();
            if (password.Contains(" "))
                password = password.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray().ToString();*/

            if (type == 1)
                return LoginAsSubscriber(userName, password);
            else if (type == 2)
                return LoginAsWriter(userName, password);
            else if(type==3)
            {
                if(userName.Equals("Igor"))
                {
                    if(password.Equals("webApp"))
                    {
                        Session["type"] = "admin";
                        Session["username"] = "Igor";
                        Session["id"] = 0;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["messege"] = messege;
                        return RedirectToAction("LoginBy");
                    }

                }    
                else
                {
                    TempData["messege"] = messege;
                    return RedirectToAction("LoginBy");
                }

            }
            else
                return RedirectToAction("LoginBy");
        }

        public ActionResult LoginBy()
        {
            if (Session["type"] != null && !Session["type"].Equals("none"))
                return RedirectToAction("Index", "Home");

            ViewBag.messege = TempData["messege"];
            return View();
        }

        public ActionResult LogOut()
        {
            if (Session["type"] == null)
                return RedirectToAction("Index", "Home");
            if (Session["type"].Equals("none"))
                return RedirectToAction("Index", "Home");
            
            Session["username"] = "guest";
            Session["type"] = "none";
            Session["id"] = null;
            return RedirectToAction("Index");
        }


        public ActionResult ShowTemp() => View();
    }
}