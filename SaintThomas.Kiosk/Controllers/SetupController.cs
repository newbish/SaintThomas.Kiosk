using Raven.Client;
using SaintThomas.Kiosk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RavenDB.AspNet.Identity;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SaintThomas.Kiosk.Controllers
{
    public class SetupController : RavenController
    {
        public SetupController()
        {
            var ustore = new UserStore<ApplicationUser>(() => this.RavenSession);
            this.UserManager = new UserManager<ApplicationUser>(ustore);
        }

        public SetupController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        public async Task<ActionResult> Index()
        {
            RavenQueryStatistics stats;
            var users = RavenSession.Query<Models.ApplicationUser>()
                .Statistics(out stats)
                .ToList();
            if (users.Count != 0)
                return RedirectToAction("Index", "Home");
            var model = new {Email = "admin@stthom.edu", Password = "admin123"};
            var user = new ApplicationUser {UserName = model.Email, Email = model.Email};
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var roleClaim = new Claim(ClaimTypes.Role, "User");
                await UserManager.AddClaimAsync(user.Id, roleClaim);
                roleClaim = new Claim(ClaimTypes.Role, "Admin");
                await UserManager.AddClaimAsync(user.Id, roleClaim);
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}