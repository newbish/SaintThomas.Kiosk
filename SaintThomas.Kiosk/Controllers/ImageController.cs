using Raven.Client;
using Raven.Json.Linq;
using SaintThomas.Kiosk.Controllers.Extensions;
using SaintThomas.Kiosk.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SaintThomas.Kiosk.Controllers
{
    public class ImageController : RavenController
    {
        string contentFormat = "images/{0}";
        // GET: Content
        public ActionResult Index()
        {
            RavenQueryStatistics stats;
            var content = RavenSession.Query<Models.Image>()
                .Statistics(out stats)
                .ToList();
            return View(content);
        }

        // GET: Content/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Content/Create
        [HttpPost]
        public ActionResult Create(ImageCreateModel model)
        {
            try
            {
                //var bmp = new System.Drawing.Bitmap(model.ImageContent.InputStream);
                var image = new Image
                {
                    Body = model.Body,
                    Video = model.Video,
                    Position = model.Position,
                    Active = true,
                };
                RavenSession.Store(image);
                using (var filesSession = RavenFS.OpenAsyncSession())
                {
                    var metadata = new RavenJObject {
                        { "Image", image.PrimaryKey },
                        { "Name", model.ImageContent.FileName }
                    };
                    filesSession.RegisterUpload(string.Format(contentFormat, image.PrimaryKey), model.ImageContent.InputStream, metadata);
                    filesSession.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Content/Edit/5
        public ActionResult Edit(int id)
        {
            var image = RavenSession.Load<Image>(id);
            return View(image);
        }

        // POST: Content/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Content/Delete/5
        public ActionResult Delete(int id)
        {
            var image = RavenSession.Load<Image>(id);
            return View(image);
        }

        // POST: Content/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public async Task<ActionResult> Download(int id)
        {
            try
            {
                using (var filesSession = RavenFS.OpenAsyncSession())
                {
                    var file = await filesSession.LoadFileAsync(string.Format(contentFormat, id));
                    var filename = file.Metadata["Name"].ToString();
                    var stream = await filesSession.DownloadAsync("images" + file.Name);
                    return new FileStreamResult(stream, filename.GetContentType())
                    {
                        FileDownloadName = filename
                    };
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
