using Boardy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Boardy.Controllers
{
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return Redirect("/");
        }

        [HttpGet]
        public ActionResult View(int id)
        {
            var category = db.Categories.Find(id);
            var discussions = from discussion in category.Discussions select discussion;

            if (category != null)
            {
                ViewBag.category = category;
                ViewBag.discussions = discussions;

                return View();
            }
            else
            {
                return Redirect("/");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Post(int id)
        {
            var category = db.Categories.Find(id);

            if (category != null)
            {
                ViewBag.category = category;

                return View();
            }
            else
            {
                return Redirect("/");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Post(int id, string title, string text)
        {
            var category = db.Categories.Find(id);
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (category != null && user != null)
            {
                var discussion = new Discussion(title, text, category, user, DateTime.Now);
                db.Discussions.Add(discussion);
                db.SaveChanges();

                return Redirect("/Categories/View/" + id);
            }
            else
            {
                return Redirect("/");
            }
        }
    }
}