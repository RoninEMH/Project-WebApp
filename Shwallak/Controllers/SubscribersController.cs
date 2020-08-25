using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Shwallak.Models;

namespace Shwallak.Controllers
{
    public class SubscribersController : Controller
    {
        private OurDB db = new OurDB();

        // GET: Subscribers
        public ActionResult Index()
        {
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
            return View(subscriber);
        }

        // GET: Subscribers/Create
        public ActionResult Create()
        {
            if (!Session["type"].Equals("none"))
                return RedirectToAction("Index");
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
                    if(sub.Nickname.Equals(subscriber.Nickname))
                        return View(subscriber);
                    if(sub.Email.Equals(subscriber.Email))
                        return View(subscriber);
                }
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
            return View(subscriber);
        }

        // POST: Subscribers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Subscriber subscriber = db.Subscribers.Find(id);
            db.Subscribers.Remove(subscriber);
            db.SaveChanges();
            return RedirectToAction("Details/" + subscriber.SubscriberID, "Subscribers");
        }

        public ActionResult Search(string userName, int? age)
        {
            if (userName == null)
                return RedirectToAction("Index");
            List<Subscriber> subscribers = new List<Subscriber>();
            if (age == null)
            {
                foreach (Subscriber s in db.Subscribers)
                {
                    if (s.Nickname.Equals(userName))
                        subscribers.Add(s);
                }
                return View(subscribers);
            }
            else
            {
                foreach (Subscriber s in db.Subscribers)
                {
                    if (s.Nickname.Equals(userName) && s.Age >= age)
                        subscribers.Add(s);
                }
                return View(subscribers);
            }
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
    }
}
