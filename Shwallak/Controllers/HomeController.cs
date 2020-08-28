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

        public ActionResult Statistics()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View(db.Articles.Include(a => a.Comments).Include(a => a.Writer).ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

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


            var csv = new StringBuilder();

            csv.AppendLine("section,count");
            foreach(var obj in db.Articles.GroupBy(x => x.Section).Select(x => new { Count = x.Count(), x.Key }).ToList())
            {
                var first =(int) obj.Key;
                var second = obj.Count;
                var newLine = string.Format("{0},{1}", first, second);
                csv.AppendLine(newLine);
            }

            string path = HttpRuntime.AppDomainAppPath;

            int a = path.IndexOf("\\Shwallak");
            path = path.Substring(0, a) + "\\Shwallak\\Content\\data.csv";

            System.IO.File.WriteAllText(path, csv.ToString());


            return View(db.Articles);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private ActionResult LoginAsSubscriber(string userName, string password)
        {
            List<Subscriber> subscribers = new List<Subscriber>();
            foreach (Subscriber s in db.Subscribers)
            {
                if (s.Nickname.Equals(userName))
                    subscribers.Add(s);
            }
            string message;
            if (subscribers.Count() != 1)
            {
                message = "invalid username";
                if (subscribers.Count() == 0)
                    message = "username not exist, consider sign up";

                DialogResult result = MessageBox.Show(message, message, MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                    return RedirectToAction("Create", "Subscribers");
                else
                    return RedirectToAction("LoginBy");
            }
            Subscriber wanted = subscribers.First(); //user name supposed to be key
            if (!wanted.Password.Equals(password))
            {
                message = "invalid password";
                MessageBox.Show(message, message, MessageBoxButtons.OK);
                return RedirectToAction("LoginBy");
            }

            Session["id"] = wanted.SubscriberID;
            Session["username"] = wanted.Nickname;
            Session["type"] = "subscriber";
            return RedirectToAction("Index", "Home");
        }


        private ActionResult LoginAsWriter(string userName, string password)
        {
            List<Writer> writers = new List<Writer>();
            foreach (Writer w in db.Writers)
            {
                if (w.FullName.Equals(userName))
                    writers.Add(w);
            }
            string message;
            if (writers.Count() != 1)
            {
                message = "invalid username";
                DialogResult result = MessageBox.Show(message, message, MessageBoxButtons.OK);
                    return RedirectToAction("LoginBy");
            }
            Writer wanted = writers.First(); //user name supposed to be key
            if (!wanted.Password.Equals(password))
            {
                message = "invalid password";
                MessageBox.Show(message, message, MessageBoxButtons.OK);
                return RedirectToAction("LoginBy");
            }
            Session["id"] = wanted.WriterID;
            Session["username"] = wanted.FullName;
            Session["type"] = "writer";
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Login(string userName, string password, int? type)
        {
            if (userName == null || userName.Equals(""))
            {
                string messege = "you must enter your username";
                DialogResult result = MessageBox.Show(messege, messege, MessageBoxButtons.OK);
                return RedirectToAction("LoginBy");
            }
            if (password == null || password.Equals(""))
            {
                string messege = "you must enter your password";
                DialogResult result = MessageBox.Show(messege, messege, MessageBoxButtons.OK);
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
                        string message = "invalid password";
                        MessageBox.Show(message, message, MessageBoxButtons.OK);
                        return RedirectToAction("LoginBy");
                    }

                }    
                else
                {
                    string message = "invalid username";
                    DialogResult result = MessageBox.Show(message, message, MessageBoxButtons.OK);
                    return RedirectToAction("LoginBy");
                }

            }
            else
                return RedirectToAction("LoginBy");
        }

        public ActionResult LoginBy()
        {
            if (!Session["type"].Equals("none"))
                return RedirectToAction("Index");
            return View();
        }

        public ActionResult LogOut()
        {
            Session["username"] = "guest";
            Session["type"] = "none";
            Session["id"] = null;
            return RedirectToAction("Index");
        }
    }
}