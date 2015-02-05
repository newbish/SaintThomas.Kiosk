using BundleTransformer.Core.Transformers;
using System.Web;
using System.Web.Optimization;

namespace SaintThomas.Kiosk
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            var cssTransformer = new StyleTransformer();

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ext").Include(
                "~/Scripts/jquery.oembed.js",
                "~/Scripts/jquery.fancybox.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/kiosk").Include(
                "~/Scripts/kiosk.js"
                ));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            var fontawesome = new Bundle("~/Content/font-awesome").Include(
                "~/Content/fontawesome/font-awesome.less"
                );
            fontawesome.Transforms.Add(cssTransformer);
            bundles.Add(fontawesome);
            var less = new Bundle("~/Content/Kiosk").Include(
                "~/Content/Kiosk.less",
                "~/Content/jquery.fancybox.css"
                );
            less.Transforms.Add(cssTransformer);
            bundles.Add(less);

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }
    }
}
