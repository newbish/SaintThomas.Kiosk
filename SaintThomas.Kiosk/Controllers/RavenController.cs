using System;
using System.Linq;
using System.Web.Mvc;
using Raven.Client;
using Microsoft.AspNet.Identity;
using SaintThomas.Kiosk.Models;

namespace SaintThomas.Kiosk.Controllers
{
    public class RavenController : Controller
    {
        /// <remarks>
        /// Paging code
        /// http://ben.onfabrik.com/posts/paging-with-ravendb-and-aspnet-mvc
        /// </remarks>
        public const int PageSize = 10;
        public const int DefaultPage = 1;

        public IDocumentSession RavenSession { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RavenSession = MvcApplication.Store.OpenSession();
            //RavenSession.Advanced.UseOptimisticConcurrency = true;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

            using (RavenSession)
            {
                if (filterContext.Exception != null)
                    return;

                if (RavenSession == null)
                    return;

                if (Server.GetLastError() != null)
                    return;

                RavenSession.SaveChanges();
            }
        }

        /// <summary>
        /// Method to get the user obejct
        /// </summary>
        /// <returns></returns>
        protected ApplicationUser GetUser()
        {
            return !Request.IsAuthenticated ? null : RavenSession.Load<ApplicationUser>(User.Identity.GetUserId());
        }
    }
}