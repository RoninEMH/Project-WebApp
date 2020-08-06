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
    public class WritersController : Controller
    {
        private OurDB db = new OurDB();

        // GET: Writers
        public ActionResult Index()
        {
            return View(db.Writers.ToList());
        }

        // GET: Writers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Writer writer = db.Writers.Find(id);
            if (writer == null)
            {
                return HttpNotFound();
            }
            return View(writer);
        }

        // GET: Writers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Writers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WriterID,FullName,Gender,Email,Year,Password")] Writer writer)
        {
            if (ModelState.IsValid)
            {
                db.Writers.Add(writer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(writer);
        }

        // GET: Writers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Writer writer = db.Writers.Find(id);
            if (writer == null)
            {
                return HttpNotFound();
            }
            return View(writer);
        }

        // POST: Writers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WriterID,FullName,Gender,Email,Year,Password")] Writer writer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(writer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(writer);
        }

        // GET: Writers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Writer writer = db.Writers.Find(id);
            if (writer == null)
            {
                return HttpNotFound();
            }
            return View(writer);
        }

        // POST: Writers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Writer writer = db.Writers.Find(id);
            db.Writers.Remove(writer);
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

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult Results(string name, string email, int? year)
        {
            List<Writer> results = new List<Writer>();
            List<Writer> temp = new List<Writer>();

            if ((name == null || name.Equals("")) && (email == null || email.Equals("")) && year == null)
            {
                return RedirectToAction("Index");
            }

            foreach (Writer writer in db.Writers.ToList())
            {
                results.Add(writer);
            }


            if (name != null && !name.Equals(""))
            {
                temp.AddRange(results);
                foreach (Writer writer in temp)
                {
                    if (writer.FullName == null)
                        results.Remove(writer);
                    else if (!writer.FullName.Equals(name))
                        results.Remove(writer);
                }
                temp.Clear();
            }

            if (email != null && !email.Equals(""))
            {
                temp.AddRange(results);
                foreach (Writer writer in temp)
                {
                    if (writer.Email == null)
                        results.Remove(writer);
                    else if (!writer.Email.Equals(email))
                        results.Remove(writer);
                }
                temp.Clear();
            }

            if (year != null)
            {
                temp.AddRange(results);
                foreach (Writer writer in temp)
                {
                    if (writer.Year != year)
                        results.Remove(writer);
                }
                temp.Clear();
            }

            return View(results);
        }

        public ActionResult SortByDate()
        {
            List<Writer> writers = db.Writers.ToList();
            writers.Sort((x, y) => x.Year - y.Year);
            return View(writers);
        }
    }
}
