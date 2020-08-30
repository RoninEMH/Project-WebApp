using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Shwallak.Migrations;
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
            if (article.SubscribersOnly && (Session["type"]==null ||Session["type"].Equals("none")))
                return RedirectToAction("LoginBy", "Home");

            if(Session["type"]!=null && Session["type"].Equals("subscriber"))
            {
                addToFavorite((int)Session["id"], article.Section);
            }
            article.Watches = article.Watches + 1;
            db.SaveChanges();
            article.Writer = db.Writers.Find(article.WriterID);
            return View(article);
        }

        // GET: Articles/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
                return HttpNotFound();
            if(Session["type"]!=null && Session["type"].Equals("writer"))
            {
                if (Session["id"] == null || !Session["id"].Equals(id))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
                return RedirectToAction("Index", "Home");

            Writer writer = db.Writers.Find(id);
            if (writer == null)
                return HttpNotFound();

           

            ViewBag.id = id;
            ViewBag.name = writer.FullName;
            ViewBag.WriterID = new SelectList(db.Writers, "WriterID", "FullName");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ArticleID,Title,Content,Year,Month,Day,SubscribersOnly,Section,WriterID")] Article article, int? id)
        {
            if (id == null)
                return HttpNotFound();
            if (Session["type"] != null && Session["type"].Equals("writer"))
            {
                if (Session["id"] == null || !Session["id"].Equals(id))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                db.Articles.Add(article);
                db.SaveChanges();
                return RedirectToAction("Facebook/" + article.ArticleID);
            }

            if (id == null)
                return HttpNotFound();
            Writer writer = db.Writers.Find(id);
            if (writer == null)
                return HttpNotFound();

            ViewBag.id = id;
            ViewBag.name = writer.FullName;

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
            if(Session["type"]!=null && Session["type"].Equals("writer"))
            {
                if (Session["id"] == null || !Session["id"].Equals(db.Articles.Find(id).WriterID))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            List<Article> articles = new List<Article>();
            List<Article> results = new List<Article>();

            articles.AddRange(db.Articles.Include(x => x.Writer));
            results.AddRange(articles.Where(x => x.ArticleID == id));

            if (results.Count != 1)
            {
                return HttpNotFound();
            }
            ViewBag.id = results.First().WriterID;
            ViewBag.name = results.First().Writer.FullName;

            ViewBag.WriterID = new SelectList(db.Writers, "WriterID", "FullName", results.First().WriterID);
            return View(results.First());
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ArticleID,Title,Content,Year,Month,Day,SubscribersOnly,Section,WriterID")] Article article)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (Session["type"] != null && Session["type"].Equals("writer"))
            {
                if (Session["id"] == null || !Session["id"].Equals(db.Articles.Find(id).WriterID))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MyArea/"+article.WriterID, "Writers");
            }
            List<Article> articles = new List<Article>();
            List<Article> results = new List<Article>();

            articles.AddRange(db.Articles.Include(x => x.Writer));
            results.AddRange(articles.Where(x => x.ArticleID == article.ArticleID));

            if (results.Count != 1)
            {
                return HttpNotFound();
            }
            ViewBag.id = results.First().WriterID;
            ViewBag.name = results.First().Writer.FullName;

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

            List<Article> articles = new List<Article>();
            List<Article> results = new List<Article>();
            articles.AddRange(db.Articles.Include(x => x.Writer));
            results.AddRange(articles.Where(x => x.ArticleID == id));
            if (results.Count != 1)
            {
                return HttpNotFound();
            }
            if(Session["type"]!=null && Session["type"].Equals("writer"))
            {
                if (Session["id"] == null || !Session["id"].Equals(results.First().WriterID))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (Session["type"] == null || !Session["type"].Equals("admin"))
            {
                return RedirectToAction("Index", "Home");

            }

            return View(results.First());
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Article article = db.Articles.Find(id);

            if (Session["type"] != null && Session["type"].Equals("writer"))
            {
                if (Session["id"] == null || !Session["id"].Equals(article.WriterID))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (Session["type"] == null || !Session["type"].Equals("admin"))
            {
                return RedirectToAction("Index", "Home");

            }

            db.Articles.Remove(article);
            db.SaveChanges();

            if(Session["type"]!=null && Session["type"].Equals("writer"))
                 return RedirectToAction("MyArea/" + article.WriterID, "Writers");
            return RedirectToAction("Index", "Home");
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

            foreach (Article article in db.Articles.Include(x => x.Writer).ToList())
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
                catch (FormatException e)
                {
                    return RedirectToAction("Search");
                }
            }

            return View(results);
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

        //public ActionResult SortBy() => View();

        public ActionResult Sort(string sortBy)
        {
            if (sortBy == null)
                return RedirectToAction("Index");
            List<Article> list = new List<Article>();
            foreach (Article a in db.Articles.ToList())
            {
                a.Writer = db.Writers.Find(a.WriterID);
                list.Add(a);
            }
            if (sortBy.Equals("title"))
                list.Sort((x, y) => string.Compare(x.Title, y.Title));
            else if (sortBy.Equals("writer"))
                list.Sort((x, y) => string.Compare(x.Writer.FullName, y.Writer.FullName));
            /*
            else if (sortBy.Equals("section"))
                list.Sort((x, y) => string.Compare(x.Section.ToString(), y.Section.ToString()));
            */
            else if (sortBy.Equals("watches"))
                list.Sort((x, y) => y.Watches - x.Watches);
            else //by date
            {
                list.Sort(delegate (Article x, Article y)
                {
                    DateTime dateX = new DateTime(x.Year, x.Month, x.Day);
                    DateTime dateY = new DateTime(y.Year, y.Month, y.Day);
                    return DateTime.Compare(dateY, dateX);
                });
            }
            return View(list);
        }
        public ActionResult Facebook(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(Session["type"]==null || !Session["type"].Equals("writer"))
            {
                return RedirectToAction("Index", "Home");
            }

            List<Article> articles = new List<Article>();
            List<Article> results = new List<Article>();
            articles.AddRange(db.Articles.Include(x => x.Writer));
            results.AddRange(articles.Where(x => x.ArticleID == id));
            if (results.Count != 1)
            {
                return HttpNotFound();
            }
            if (Session["id"] == null || !Session["id"].Equals(results.First().WriterID))
            {
                return RedirectToAction("Index", "Home");
            }
            db.SaveChanges();
            return View(results.First());
        }

        private void addToFavorite(int id, Section section)
        {
            int index = 0;
            switch (section)
            {
                case Section.Sport:
                    index = 0; break;
                case Section.Business:
                    index = 1; break;
                case Section.Culture:
                    index = 2; break;
                case Section.Food:
                    index = 3; break;
                case Section.Celebs:
                    index = 4; break;
                case Section.Fashion:
                    index = 5; break;
                case Section.Health:
                    index = 6; break;
                case Section.Tourism:
                    index = 7; break;
                case Section.Other:
                    index = 8; break;
            }

            string[] favorite = db.Subscribers.Find(id).Favorite.Split(',');
            favorite[index] = (int.Parse(favorite[index]) + 1).ToString();
            StringBuilder sb = new StringBuilder();
            sb.Append(favorite[0]);
            for(int i=1; i<9;i++)
            {
                sb.Append(",");
                sb.Append(favorite[i]);
            }
            db.Subscribers.Find(id).Favorite = sb.ToString();
            db.SaveChanges();
        }
    }
}
