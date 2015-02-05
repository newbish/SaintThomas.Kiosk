using Raven.Client;
using SaintThomas.Kiosk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RavenDB.AspNet.Identity;

namespace SaintThomas.Kiosk.Controllers
{
    public class UserController : RavenController
    {
        // GET: Admin
        [Authorize(Roles = "Admin,User")]
        public ActionResult Index()
        {
            RavenQueryStatistics stats;
            var users = RavenSession.Query<Models.ApplicationUser>()
                .Statistics(out stats)
                .ToList();
            return View(users);
        }

        // GET: Admin/Details/5
        [Authorize(Roles = "Admin,User")]
        public ActionResult Details(int id)
        {
            var user = RavenSession.Load<ApplicationUser>(id);
            var isUser = user.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.ClaimValue == "User");
            var isAdmin = user.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.ClaimValue == "Admin");
            ViewBag.IsUser = isUser != null;
            ViewBag.IsAdmin = isAdmin != null;
            return View(user);
        }

        // GET: Admin/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var user = RavenSession.Load<ApplicationUser>(id);
            var isUser = user.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.ClaimValue == "User");
            var isAdmin = user.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.ClaimValue == "Admin");
            ViewBag.IsUser = isUser != null;
            ViewBag.IsAdmin = isAdmin != null ;
            return View(user);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, FormCollection collection)
        {
            ViewBag.IsUser = bool.Parse(collection["User"].Split(',')[0]);
            ViewBag.IsAdmin = bool.Parse(collection["Admin"].Split(',')[0]);
            try
            {
                var dbitem = RavenSession.Load<ApplicationUser>(id);
                dbitem.FirstName = collection["FirstName"];
                dbitem.LastName = collection["LastName"];
                dbitem.PhoneNumber = collection["PhoneNumber"];
                dbitem.Claims.Clear();
                if (ViewBag.IsUser)
                    dbitem.Claims.Add(new IdentityUserClaim { ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", ClaimValue = "User" });
                if (ViewBag.IsAdmin)
                    dbitem.Claims.Add(new IdentityUserClaim { ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", ClaimValue = "Admin" });
                RavenSession.Store(dbitem);
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Edit", id);
            }
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            var user = RavenSession.Load<ApplicationUser>(id);
            var isUser = user.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.ClaimValue == "User");
            var isAdmin = user.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.ClaimValue == "Admin");
            ViewBag.IsUser = isUser != null;
            ViewBag.IsAdmin = isAdmin != null;
            return View(user);
        }

        // POST: Admin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var ustore = new UserStore<ApplicationUser>(() => this.RavenSession);
                var userManager = new Microsoft.AspNet.Identity.UserManager<ApplicationUser>(ustore);
                var user = RavenSession.Load<ApplicationUser>(id);
                userManager.DeleteAsync(user);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
