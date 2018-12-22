using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Boardy.Models;
using System.Collections.Generic;

namespace Boardy.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
          
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        [Authorize]
        public ActionResult Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            var userId = User.Identity.GetUserId();
            var roles = UserManager.GetRoles(userId);

            var cuser = db.Users.Find(userId);

            ViewBag.user = cuser;

            if(roles.Contains("Administrator"))
            {
                ViewBag.isAdministrator = true;
            }
            else
            {
                ViewBag.isAdministrator = false;
            }

            ViewBag.categories = db.Categories;

            string adminRole = "", moderatorRole = "";

            var allRoles = from role in db.Roles select role;
            foreach( var role in allRoles )
            {
                if(role.Name == "Administrator")
                {
                    adminRole = role.Id;
                }

                if(role.Name == "Moderator")
                {
                    moderatorRole = role.Id;
                }
            }

            List<ApplicationUser> admins = new List<ApplicationUser>();
            List<ApplicationUser> moderators = new List<ApplicationUser>();

            var allUsers = from user in db.Users select user;
            ViewBag.Error = allUsers.Count();

            foreach(var user in allUsers)
            {
                var role = user.Roles.FirstOrDefault();
                if(role.RoleId == adminRole)
                {
                    admins.Insert(0, user);
                } 
                else if(role.RoleId == moderatorRole)
                {
                    moderators.Insert(0, user);
                }
            }

            ViewBag.admins = admins;    
            ViewBag.moderators = moderators;

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateProfile(string camp1)
        {
            var uid = User.Identity.GetUserId();
            var user = db.Users.Find(uid);

            try
            {
                if (TryUpdateModel(user))
                {
                    user.Camp1 = camp1;

                    db.SaveChanges();
                }
            }
            catch (Exception) { }

            return Redirect("/Manage");
        }

        //
        // POST: /Manage/AddAdministrator
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult AddAdministrator(string email)
        {
            var user = UserManager.FindByEmail(email);
        
            try
            {
                if (TryUpdateModel(user))
                {
                    UserManager.RemoveFromRoles(user.Id, UserManager.GetRoles(user.Id).ToArray());
                    UserManager.AddToRole(user.Id, "Administrator");
                    db.SaveChanges();
                }
            }
            catch (Exception) { }

            return Redirect("/Manage");
        }

        //
        // DELETE: /Manage/DeleteAdministrator

        [Authorize(Roles = "Administrator")]
        [HttpDelete]
        public ActionResult DeleteAdministrator(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            if(user != null)
            {
                UserManager.RemoveFromRole(user.Id, "Administrator");
                UserManager.AddToRole(user.Id, "User");
            }

            return Redirect("/Manage");
        }

        //
        // POST: /Manage/AddModerator
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult AddModerator(string email)
        {
            var user = UserManager.FindByEmail(email);

            try
            {
                if (TryUpdateModel(user))
                {
                    UserManager.RemoveFromRoles(user.Id, UserManager.GetRoles(user.Id).ToArray());
                    UserManager.AddToRole(user.Id, "Moderator");
                    db.SaveChanges();
                }
            }
            catch (Exception) { }

            return Redirect("/Manage");
        }

        //
        // DELETE: /Manage/DeleteModerator
        [Authorize(Roles = "Administrator")]
        [HttpDelete]
        public ActionResult DeleteModerator(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            if (user != null)
            {
                UserManager.RemoveFromRole(user.Id, "Moderator");
                UserManager.AddToRole(user.Id, "User");
            }

            return Redirect("/Manage");
        }

        //
        // POST: /Manage/AddCategory
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult AddCategory(string name)
        {
            try
            {
                var category = new Category(name: name);

                db.Categories.Add(category);
                db.SaveChanges();
            }
            catch (Exception _) { }

            return Redirect("/Manage");
        }

        //
        // POST: /Manage/UpdateCategory
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult UpdateCategory(int id, string name)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Category category = db.Categories.Find(id);

                    if (TryUpdateModel(category))
                    {
                        category.Name = name;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e) { }

            return Redirect("/Manage");
        }

        //
        // DELETE: /Manage/DeleteCategory
        [Authorize(Roles = "Administrator")]
        [HttpDelete]
        public ActionResult DeleteCategory(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();

            return Redirect("/Manage");
        }

        //
        // GET: /Manage/ChangePassword
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            Error
        }

#endregion
    }
}