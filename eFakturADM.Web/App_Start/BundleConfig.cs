using System.Web;
using System.Web.Optimization;

namespace eFakturADM.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/bundles/css/plugins").Include(
                "~/Content/plugins/uniform/css/uniform.default.css",
                "~/Content/plugins/bootstrap-switch/css/bootstrap-switch.css",
                "~/Content/plugins/font-awesome/css/font-awesome.css",
                "~/Content/plugins/simple-line-icons/simple-line-icons.css",
                "~/Content/plugins/bootstrap/css/bootstrap.css"));

            bundles.Add(new StyleBundle("~/bundles/css/theme").Include(
                 "~/Content/theme/components.css",                 
                 "~/Content/theme/default.css",
                 "~/Content/theme/layout.css",
                 "~/Content/theme/light.css",
                 "~/Content/theme/datatable.css",
                 "~/Content/theme/short.css"
                 ));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/metronic.js",
                "~/Scripts/layout.js",
                "~/Scripts/demo.js"));

            bundles.Add(new ScriptBundle("~/bundles/ui").Include(
                "~/Content/ui/eFakturADM.ui.core.js"));                
        }
    }
}