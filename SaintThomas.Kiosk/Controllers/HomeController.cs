using Raven.Client;
using Raven.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SaintThomas.Kiosk.Controllers
{
    public class HomeController : RavenController
    {
        public ActionResult Index()
        {
            RavenQueryStatistics stats;
            var content = RavenSession.Query<Models.Image>("PositionIndex")
                .Statistics(out stats)
                .Take(5)
                .ToList();
            return View(content);
        }

        public ActionResult Kiosk()
        {
            RavenQueryStatistics stats;
            var content = RavenSession.Query<Models.Image>("PositionIndex")
                .Statistics(out stats)
                .Take(5)
                .ToList();
            return View(content);
        }
    }
}