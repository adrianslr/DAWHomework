using Boardy.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Boardy.Controllers
{
    public class DiscussionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return Redirect("/");
        }

        [HttpGet]
        public ActionResult View(int id)
        {
            var discussion = db.Discussions.Find(id);

            if (discussion != null)
            {
                ViewBag.discussion = discussion;
                ViewBag.author = discussion.Author;

                ViewBag.category = discussion.Category;

                return View();
            }
            else
            {
                return Redirect("/");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Reply(int id, string text)
        {
            var discussion = db.Discussions.Find(id);
            var uid = User.Identity.GetUserId();
            var user = db.Users.Find(uid);

            if (discussion != null)
            {
                var reply = new Reply(text, discussion, user, DateTime.Now);
                db.Replies.Add(reply);
                db.SaveChanges();

                return Redirect("/Discussions/View/" + discussion.DiscussionID);
            }
            else
            {
                return Redirect("/");
            }
        }
    }
}