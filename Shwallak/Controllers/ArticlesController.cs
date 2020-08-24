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
    public class ArticlesController : Controller
    {
        private OurDB db = new OurDB();

        // GET: Articles
        public ActionResult Index()
        {
            var articles = db.Articles.Include(a => a.Writer);
            return View(articles.ToList());
        }

        // GET: Articles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<Article> articles = new List<Article>();
            articles.AddRange(db.Articles.Include(x => x.Comments));
            Article article = articles.Find(x => x.ArticleID == id);
            if (article == null)
            {
                return HttpNotFound();
            }
            if (article.SubscribersOnly && Session["type"].Equals("none"))
                return RedirectToAction("LoginBy", "Home");

            article.Watches = article.Watches + 1;
            db.SaveChanges();
            article.Writer = db.Writers.Find(article.WriterID);
            return View(article);
        }

        // GET: Articles/Create
        public ActionResult Create()
        {
            ViewBag.WriterID = new SelectList(db.Writers, "WriterID", "FullName");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ArticleID,Title,Content,Year,Month,Day,SubscribersOnly,Section,WriterID")] Article article)
        {
            if (ModelState.IsValid)
            {
                db.Articles.Add(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.WriterID = new SelectList(db.Writers, "WriterID", "FullName", article.WriterID);
            return View(article);
        }

        // GET: Articles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            ViewBag.WriterID = new SelectList(db.Writers, "WriterID", "FullName", article.WriterID);
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ArticleID,Title,Content,Year,Month,Day,SubscribersOnly,Section,WriterID")] Article article)
        {
            if (ModelState.IsValid)
            {
                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.WriterID = new SelectList(db.Writers, "WriterID", "FullName", article.WriterID);
            return View(article);
        }

        // GET: Articles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
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

        public ActionResult Results(string title, string date)
        {
            List<Article> results = new List<Article>();
            List<Article> temp = new List<Article>();

            if ((title == null || title.Equals("")) && (date == null || date.Equals("")))
            {
                return RedirectToAction("Search");
            }

            foreach (Article article in db.Articles.Include(x=>x.Writer).ToList())
            {
                results.Add(article);
            }


            if (title != null && !title.Equals(""))
            {
                temp.AddRange(results);
                foreach (Article article in temp)
                {
                    if (article.Title == null)
                        results.Remove(article);
                    else if (!article.Title.ToLower().Contains(title.ToLower()))
                        results.Remove(article);
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
                    foreach (Article article in temp)
                    {
                        if (article.Year != year)
                            results.Remove(article);
                    }
                    temp.Clear();


                    temp.AddRange(results);
                    foreach (Article article in temp)
                    {
                        if (article.Month != month)
                            results.Remove(article);
                    }
                    temp.Clear();

                    temp.AddRange(results);
                    foreach (Article article in temp)
                    {
                        if (article.Day != day)
                            results.Remove(article);
                    }
                    temp.Clear();
                }
                catch(FormatException e)
                {
                    return RedirectToAction("Search");
                }
            }

            return View(results);
        }

        public ActionResult SortByDate()
        {
            List<Article> articles = new List<Article>();

            foreach (Article article in db.Articles.ToList())
            {
                article.Writer = db.Writers.Find(article.WriterID);
                articles.Add(article);
            }

            articles.Sort(delegate (Article x, Article y)
            {
                if (x.Year == y.Year)
                {
                    if (x.Month == y.Month)
                    {
                        return x.Day - y.Day;
                    }
                    else
                        return x.Month - y.Month;
                }
                else
                    return x.Year - y.Year;
            });
            return View(articles);
        }

        public ActionResult SectionList(string section)
        {
            List<Article> articles = db.Articles.Include(x => x.Comments).ToList();
            List<Article> results = new List<Article>();
            Section sec;
            if (section == null)
                return HttpNotFound();
            switch (section)
            {
                case "Sport":
                    sec = Section.Sport;
                    break;
                case "Business":
                    sec = Section.Business;
                    break;
                case "Culture":
                    sec = Section.Culture;
                    break;
                case "Food":
                    sec = Section.Food;
                    break;
                case "Fashion":
                    sec = Section.Fashion;
                    break;
                case "Tourism":
                    sec = Section.Tourism;
                    break;
                case "Celebs":
                    sec = Section.Celebs;
                    break;
                case "Health":
                    sec = Section.Health;
                    break;
                case "Other":
                    sec = Section.Other;
                    break;
                default:
                    return HttpNotFound();


            }
            ViewBag.Section = section;
            results.AddRange(articles.Where(x => x.Section == sec));
            return View(results);
        }
    }
}
