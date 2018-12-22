using Boardy.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Boardy.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Users
        public ActionResult Index()
        {
            return Redirect("/");
        }

        [HttpGet]
        public ActionResult Show(string id)
        {
            var user = db.Users.Find(id);

            if (user != null)
            {
                ViewBag.user = user;
                return View();
            }
            else
            {
                return Redirect("/");
            }
        }
    }
}