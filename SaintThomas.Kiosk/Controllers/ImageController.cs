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
        [Authorize(Roles = "Admin,User")]
        // GET: Content
        public ActionResult Index()
        {
            RavenQueryStatistics stats;
            var content = RavenSession.Query<Models.Image>()
                .Statistics(out stats)
                .ToList();
            return View(content);
        }

        [Authorize(Roles = "Admin")]
        // GET: Content/Create
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: Content/Create
        [HttpPost]
        public ActionResult Create(ImageCreateEditModel model)
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

        [Authorize(Roles = "Admin")]
        // GET: Content/Edit/5
        public ActionResult Edit(int id)
        {
            var image = AutoMapper.Mapper.Map<ImageCreateEditModel>(RavenSession.Load<Image>(id));
            return View(image);
        }

        [Authorize(Roles = "Admin")]
        // POST: Content/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, ImageCreateEditModel model)
        {
            try
            {
                var image = RavenSession.Load<Image>(model.PrimaryKey);
                image.Body = model.Body;
                image.Video = model.Video;
                RavenSession.Store(image);
                if (model.ImageContent != null)
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
                var image = AutoMapper.Mapper.Map<ImageCreateEditModel>(RavenSession.Load<Image>(id));
                return View(image);
            }
        }

        [Authorize(Roles = "Admin")]
        // GET: Content/Delete/5
        public ActionResult Delete(int id)
        {
            var image = RavenSession.Load<Image>(id);
            return View(image);
        }

        [Authorize(Roles = "Admin")]
        // POST: Content/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id, FormCollection collection)
        {
            try
            {
                RavenSession.Delete<Image>(RavenSession.Load<Image>(id));
                using (var filesSession = RavenFS.OpenAsyncSession())
                {
                    filesSession.RegisterFileDeletion(string.Format(contentFormat, id));
                    await filesSession.SaveChangesAsync();
                }
                RavenSession.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [Authorize(Roles = "Admin,User")]
        // GET: Content/Edit/5
        public ActionResult Details(int id)
        {
            var image = RavenSession.Load<Image>(id);
            return View(image);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public string Sort(List<long> list)
        {
            if (list != null && list.Count > 0)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var image = RavenSession.Load<Image>(list[i]);
                    image.Position = i;
                    RavenSession.Store(image);
                }
                RavenSession.SaveChanges();
                return "true";
            }
            return "false";
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
