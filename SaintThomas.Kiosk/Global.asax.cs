using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.FileSystem;
using Raven.Client.Indexes;
using Raven.Database.Server;
using SaintThomas.Kiosk.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SaintThomas.Kiosk.Indexes;

namespace SaintThomas.Kiosk
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static EmbeddableDocumentStore Store;
        public static string RavenFilestore = "Kiosk";
        static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(MvcApplication));

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Init raven store
            //Raven.Database.Server.NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080);
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080, false);
            Store = new EmbeddableDocumentStore
            {
                ConnectionStringName = "RavenDB",
                UseEmbeddedHttpServer = true
            };
            Store.Initialize();
            FilesStore fileStore = (FilesStore)Store.FilesStore;
            fileStore.DefaultFileSystem = RavenFilestore;
            fileStore.Initialize(true);
            fileStore.AsyncFilesCommands.Admin.CreateOrUpdateFileSystemAsync(
                new Raven.Abstractions.FileSystem.FileSystemDocument() {
                    Settings = { { "Raven/FileSystem/DataDir", string.Format("~/App_Data/FileSystem/{0}", RavenFilestore) } }
                },
                RavenFilestore);
            CreateObjectMaps();
            TryCreatingIndexesOrRedirectToErrorPage();
        }

        /// <summary>
        /// We do this so we can basically hijack the ajax request and handle authentication on ajax calls ourselves
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/questions/9608929/unauthorized-result-in-ajax-requests
        /// </remarks>
        protected void Application_EndRequest()
        {
            // Any AJAX request that ends in a redirect should get mapped to an unauthorized request
            // since it should only happen when the request is not authorized and gets automatically
            // redirected to the login page.
            var context = new HttpContextWrapper(Context);
            if (context.Response.StatusCode == 302 && context.Request.IsAjaxRequest())
            {
                context.Response.Clear();
                Context.Response.StatusCode = 401;
            }
        }
        
        static void TryCreatingIndexesOrRedirectToErrorPage()
        {
            try
            {
                //new RavenDB.AspNet.Identity.User_ByUserName<ApplicationUser>().Execute(Store);
                //var catalog = new CompositionContainer(new AssemblyCatalog(typeof(RavenDB.AspNet.Identity.User_ByUserName<ApplicationUser>).Assembly));
                //IndexCreation.CreateIndexes(catalog, Store.DatabaseCommands.ForSystemDatabase(), Store.Conventions);
                //IndexCreation.CreateIndexes(typeof(KnowledgeIndex).Assembly, Store);
                IndexCreation.CreateIndexes(typeof(PositionIndex).Assembly, Store);
            }
            catch (WebException e)
            {
                var socketException = e.InnerException as SocketException;
                if (socketException == null)
                    throw;

                switch (socketException.SocketErrorCode)
                {
                    case SocketError.AddressNotAvailable:
                    case SocketError.NetworkDown:
                    case SocketError.NetworkUnreachable:
                    case SocketError.ConnectionAborted:
                    case SocketError.ConnectionReset:
                    case SocketError.TimedOut:
                    case SocketError.ConnectionRefused:
                    case SocketError.HostDown:
                    case SocketError.HostUnreachable:
                    case SocketError.HostNotFound:
                        HttpContext.Current.Response.Redirect("~/Error.html?raven=failed");
                        break;
                    default:
                        throw;
                }
            }
        }
        static void CreateObjectMaps()
        {
            AutoMapper.Mapper.CreateMap<Image, ImageCreateEditModel>();
        }
    }
}
