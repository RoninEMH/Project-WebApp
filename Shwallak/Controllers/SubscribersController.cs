using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using Shwallak.Models;

namespace Shwallak.Controllers
{
    public class SubscribersController : Controller
    {
        private OurDB db = new OurDB();

        // GET: Subscribers
        public ActionResult Index()
        {
            if (Session["type"] == null || !Session["type"].Equals("admin"))
                return RedirectToAction("Index", "Home");
            return View(db.Subscribers.ToList());
        }

        // GET: Subscribers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscriber subscriber = db.Subscribers.Find(id);
            if (subscriber == null)
            {
                return HttpNotFound();
            }
            if (Session["type"] != null && Session["type"].Equals("subscriber"))
            {
                if (Session["id"] == null || !Session["id"].Equals(id))
                    return RedirectToAction("Index", "Home");
            }
            else if (Session["type"] == null || !Session["type"].Equals("admin"))
                return RedirectToAction("Index", "Home");
            return View(subscriber);
        }

        // GET: Subscribers/Create
        public ActionResult Create()
        {
            if(Session["type"] == null)
                return View();
            if (!Session["type"].Equals("none"))
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: Subscribers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SubscriberID,Age,Gender,Email,Nickname,Password")] Subscriber subscriber)
        {
            if (ModelState.IsValid)
            {
                foreach(Subscriber sub in db.Subscribers.ToList())
                {
                    if (sub.Nickname.Equals(subscriber.Nickname))
                    {
                        ViewBag.messege = "this nickname is taken";
                        return View(subscriber);
                    }
                    if (sub.Email.Equals(subscriber.Email))
                    {
                        ViewBag.messege = "this email is allready in use";
                        return View(subscriber);
                    }
                }
                subscriber.Favorite = "0,0,0,0,0,0,0,0,0";
                db.Subscribers.Add(subscriber);
                db.SaveChanges();

                Session["username"] = subscriber.Nickname;
                Session["type"] = "subscriber";
                Session["id"] = subscriber.SubscriberID;
                return Redirect("~/Home/Index");
            }

            return View(subscriber);
        }

        // GET: Subscribers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscriber subscriber = db.Subscribers.Find(id);
            if (subscriber == null)
            {
                return HttpNotFound();
            }

            if (Session["type"] != null && Session["type"].Equals("subscriber"))
            {
                if (Session["id"] == null || !Session["id"].Equals(id))
                    return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Home");
            
            return View(subscriber);
        }

        // POST: Subscribers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SubscriberID,Age,Gender,Email,Nickname,Password")] Subscriber subscriber)
        {
            if (ModelState.IsValid)
            {
                foreach (Subscriber sub in db.Subscribers.AsNoTracking().ToList())
                {
                    if (sub.SubscriberID == subscriber.SubscriberID)
                        continue;
                    if (sub.Nickname.Equals(subscriber.Nickname))
                    {
                        ViewBag.messege = "this nickname is taken";
                        return View(subscriber);
                    }
                    if (sub.Email.Equals(subscriber.Email))
                    {
                        ViewBag.messege = "this email is allready in use";
                        return View(subscriber);
                    }
                }
                db.Entry(subscriber).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details/" + subscriber.SubscriberID);
            }
            return View(subscriber);
        }

        // GET: Subscribers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscriber subscriber = db.Subscribers.Find(id);
            if (subscriber == null)
            {
                return HttpNotFound();
            }
            if (Session["type"] == null || !Session["type"].Equals("admin"))
                return RedirectToAction("Index", "Home");

            return View(subscriber);
        }

        // POST: Subscribers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["type"] == null || !Session["type"].Equals("admin"))
                return RedirectToAction("Index", "Home");
            Subscriber subscriber = db.Subscribers.Find(id);
            db.Subscribers.Remove(subscriber);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Search()
        {
            if (Session["type"] == null || !Session["type"].Equals("admin"))
                return RedirectToAction("Index", "Home");
            return View();
        }

        public ActionResult Results(string nickname, int? age)
        {
            if (Session["type"] == null || !Session["type"].Equals("admin"))
                return RedirectToAction("Index", "Home");
            List<Subscriber> results = new List<Subscriber>();
            List<Subscriber> temp = new List<Subscriber>();

            if ((nickname == null || nickname.Equals("")) && age==null)
            {
                return RedirectToAction("Search");
            }

            foreach (Subscriber subscriber in db.Subscribers.ToList())
            {
                results.Add(subscriber);
            }


            if (nickname != null && !nickname.Equals(""))
            {
                temp.AddRange(results);
                foreach (Subscriber subscriber in temp)
                {
                    if (subscriber.Nickname == null)
                        results.Remove(subscriber);
                    else if (!subscriber.Nickname.ToLower().Contains(nickname.ToLower()))
                        results.Remove(subscriber);
                }
                temp.Clear();
            }


            if(age!=null)
            {
                temp.AddRange(results);
                foreach (Subscriber subscriber in temp)
                {
                    if (subscriber.Age!=age)
                        results.Remove(subscriber);
                }
                temp.Clear();
            }

            return View(results);
        }

        private int CompareGenderMale(Subscriber x, Subscriber y)
        {
            if (x.Gender.ToString().Equals(y.Gender))
                return 0;
            if (x.Gender.ToString().Equals("Male"))
                return -1;
            else if (x.Gender.ToString().Equals("Female"))
            {
                if (y.Gender.ToString().Equals("Male"))
                    return 1;
                else
                    return -1;
            }
            else if (x.Gender.ToString().Equals("Other"))
                return 1;
            return 1;
        }
        private int CompareGenderFemale(Subscriber x, Subscriber y)
        {
            if (x.Gender.ToString().Equals(y.Gender))
                return 0;
            if (x.Gender.ToString().Equals("Female"))
                return -1;
            else if (x.Gender.ToString().Equals("Male"))
            {
                if (y.Gender.ToString().Equals("Female"))
                    return 1;
                else
                    return -1;
            }
            else if (x.Gender.ToString().Equals("Other"))
                return 1;
            return 1;
        }

        private int CompareGender(Subscriber x, Subscriber y, Gender gender)
        {
            if (gender.Equals(Gender.Male))
                return CompareGenderMale(x, y);
            else
                return CompareGenderFemale(x, y);
        }

        public ActionResult Sort(string sortBy, int? gender)
        {
            if (Session["type"] == null || !Session["type"].Equals("admin"))
                return RedirectToAction("Index", "Home");
            if (sortBy == null)
                return RedirectToAction("Index");
            List<Subscriber> list = new List<Subscriber>();
            foreach (Subscriber s in db.Subscribers)
                list.Add(s);
            if (sortBy.Equals("age"))
                list.Sort((x, y) => x.Age - y.Age);
            else if (sortBy.Equals("nickname"))
                list.Sort((x, y) => string.Compare(x.Nickname, y.Nickname));
            else if (sortBy.Equals("gender"))
                if (gender == 1)
                    list.Sort((x, y) => CompareGender(x, y, Gender.Male));
                else
                    list.Sort((x, y) => CompareGender(x, y, Gender.Female));
            return View(list);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult ChangePassword(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscriber subscriber = db.Subscribers.Find(id);
            if (subscriber == null)
            {
                return HttpNotFound();
            }
            if (Session["type"] != null && Session["type"].Equals("subscriber"))
            {
                if (Session["id"] == null || !Session["id"].Equals(id))
                    return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Home");

            return View(subscriber);
        }

        // POST: Subscribers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword([Bind(Include = "SubscriberID,Age,Gender,Email,Nickname,Password")] Subscriber subscriber)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subscriber).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details/" + subscriber.SubscriberID);
            }
            return View(subscriber);
        }
    }
}
