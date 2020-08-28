using Shwallak.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace Shwallak.Controllers
{
    public class HomeController : Controller
    {
        private static int tries = 0;
        private OurDB db = new OurDB();
        public ActionResult Index()
        {
            return View(db.Articles.Include(a => a.Comments).Include(a => a.Writer).ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            ViewBag.priv = 0;
            ViewBag.pub = 0;
            foreach(var a in db.Articles.GroupBy(x => x.SubscribersOnly).Select(x => new { Key = x.Key, Count = x.Count() }).ToList())
            {
                if(a.Key)
                {
                    ViewBag.priv = a.Count;
                }
                else
                {
                    ViewBag.pub = a.Count;
                }
            }
            
            return View();
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
                    return tryChecker();
            }
            Subscriber wanted = subscribers.First(); //user name supposed to be key
            if (!wanted.Password.Equals(password))
            {
                message = "invalid password";
                MessageBox.Show(message, message, MessageBoxButtons.OK);
                return tryChecker();
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
                return tryChecker();
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
                        return tryChecker();
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

        private ActionResult tryChecker()
        {
            tries++;
            if (tries == 3)
            {
                string Message = "out of tries";
                MessageBox.Show(Message, "Limited tries");
                tries = 0;
                return RedirectToAction("Index");
            }
            MessageBox.Show("got " + (3 - tries) + " left", "Limited tries");
            return RedirectToAction("LoginBy");
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