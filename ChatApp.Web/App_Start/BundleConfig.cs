using System.Web;
using System.Web.Optimization;

namespace ChatApp.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/signalr").Include("~/Scripts/jquery.signalR-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/main").Include("~/Scripts/Main.js"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/uploader").Include("~/Scripts/jquery.uploader.js","~/Scripts/UploaderHelpers.js"/*,"~/Scripts/CryptoJS/core-min.js", "~/Scripts/CryptoJS/md5-min.js"*/));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular-timeago-master-core").Include(
                "~/Scripts/angular-timeago-master/dist/angular-timeago-core.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/angular-timeago-master").Include(
                "~/Scripts/angular-timeago-master/dist/angular-timeago.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                "~/Scripts/angular/angular.js"));
            bundles.Add(new ScriptBundle("~/bundles/angular-animate").Include(
                "~/Scripts/angular-animate/angular-animate.js"));

            bundles.Add(new ScriptBundle("~/bundles/lodash").Include(
                "~/Scripts/lodash/lodash.js"));
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                    "~/Scripts/app.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                     
                      "~/Content/style.css"));


        }
    }
}
