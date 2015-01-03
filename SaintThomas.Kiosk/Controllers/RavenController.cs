using System;
using System.Linq;
using System.Web.Mvc;
using Raven.Client;
using Raven.Client.FileSystem;
using Microsoft.AspNet.Identity;
using SaintThomas.Kiosk.Models;

namespace SaintThomas.Kiosk.Controllers
{
    public class RavenController : Controller, IActionFilter
    {
        /// <remarks>
        /// Paging code
        /// http://ben.onfabrik.com/posts/paging-with-ravendb-and-aspnet-mvc
        /// </remarks>
        public const int PageSize = 10;
        public const int DefaultPage = 1;

        public IDocumentSession RavenSession { get; set; }
        public IFilesStore RavenFS { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) 
        {
            RavenSession = MvcApplication.Store.OpenSession();
            RavenFS = MvcApplication.Store.FilesStore;
            //RavenSession.Advanced.UseOptimisticConcurrency = true;
            base.OnActionExecuting(filterContext);
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
            base.OnActionExecuted(filterContext);
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