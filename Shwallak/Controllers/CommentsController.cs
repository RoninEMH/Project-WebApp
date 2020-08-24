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
    public class CommentsController : Controller
    {
        private OurDB db = new OurDB();

        // GET: Comments
        public ActionResult Index()
        {
            var commants = db.Commants.Include(c => c.Article);
            return View(commants.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<Comment> comments = new List<Comment>();
            comments.AddRange(db.Commants.Include(x => x.Article).Include(x=>x.Article.Comments).Where(x => x.CommentID == id));
            if (comments.Count == 0)
            {
                return HttpNotFound();
            }
            if (db.Commants.Find(id).Article.SubscribersOnly && Session["type"].Equals("none"))
                return RedirectToAction("LoginBy", "Home");

            db.Commants.Find(id).Watches = db.Commants.Find(id).Watches + 1;
            db.SaveChanges();
            return View(comments.ElementAt(0));
        }

        // GET: Comments/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
                return HttpNotFound();
            Article article = db.Articles.Find(id);
            if (article == null)
                return HttpNotFound();

            if (article.SubscribersOnly && Session["type"].Equals("none"))
                return RedirectToAction("LoginBy", "Home");

            ViewBag.id = id;
            ViewBag.name = article.Title;
            ViewBag.ArticleID = new SelectList(db.Articles, "ArticleID", "Title");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CommentID,Author,Year,Month,Day,Hour,Minute,Content,ArticleID")] Comment comment, int? id)
        {
            if (ModelState.IsValid)
            {
                db.Commants.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Details/"+comment.CommentID);
            }

            if (id == null)
                return HttpNotFound();
            Article article = db.Articles.Find(id);
            if (article == null)
                return HttpNotFound();

            ViewBag.id = id;
            ViewBag.name = article.Title;
            ViewBag.ArticleID = new SelectList(db.Articles, "ArticleID", "Title", comment.ArticleID);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Commants.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArticleID = new SelectList(db.Articles, "ArticleID", "Title", comment.ArticleID);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommentID,Author,Year,Month,Day,Hour,Minute,Content,ArticleID")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ArticleID = new SelectList(db.Articles, "ArticleID", "Title", comment.ArticleID);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Commants.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Commants.Find(id);
            db.Commants.Remove(comment);
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

        public ActionResult Results(string author, string contnet, string date)
        {
            List<Comment> results = new List<Comment>();
            List<Comment> temp = new List<Comment>();

            if ((author == null || author.Equals("")) && (contnet == null || contnet.Equals("")) && (date == null || date.Equals("")))
            {
                return RedirectToAction("Search");
            }

            foreach (Comment comment in db.Commants.Include(x=>x.Article).ToList())
            {
                results.Add(comment);
            }


            if (author != null && !author.Equals(""))
            {
                temp.AddRange(results);
                foreach (Comment comment in temp)
                {
                    if (comment.Author == null)
                        results.Remove(comment);
                    else if (!comment.Author.ToLower().Contains(author.ToLower()))
                        results.Remove(comment);
                }
                temp.Clear();
            }

            if (contnet != null && !contnet.Equals(""))
            {
                temp.AddRange(results);
                foreach (Comment comment in temp)
                {
                    if (comment.Content == null)
                        results.Remove(comment);
                    else if (!comment.Content.ToLower().Contains(contnet.ToLower()))
                        results.Remove(comment);
                }
                temp.Clear();
            }

            if (date != null && !date.Equals(""))
            {
                try
                {
                    string[] numbers = date.Split('-');
                    if (numbers.Length != 3)
                        return RedirectToAction("Search");

                    int year = int.Parse(numbers[0]);
                    int month = int.Parse(numbers[1]);
                    int day = int.Parse(numbers[2]);

                    temp.AddRange(results);
                    foreach (Comment comment in temp)
                    {
                        if (comment.Year != year)
                            results.Remove(comment);
                    }
                    temp.Clear();


                    temp.AddRange(results);
                    foreach (Comment comment in temp)
                    {
                        if (comment.Month != month)
                            results.Remove(comment);
                    }
                    temp.Clear();

                    temp.AddRange(results);
                    foreach (Comment comment in temp)
                    {
                        if (comment.Day != day)
                            results.Remove(comment);
                    }
                    temp.Clear();
                }
                catch (FormatException e)
                {
                    return RedirectToAction("Search");
                }
            }

                return View(results);
        }
    }
}
