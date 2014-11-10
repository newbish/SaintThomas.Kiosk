using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Database.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SaintThomas.Kiosk
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static DocumentStore Store;
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
                //IndexCreation.CreateIndexes(typeof(KnowledgeIndex).Assembly, Store);
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
    }
}
