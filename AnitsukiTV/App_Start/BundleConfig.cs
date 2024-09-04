using System.Web;
using System.Web.Optimization;

namespace AnitsukiTV
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/swiper").Include(
                      "~/swiper/swiper-bundle.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/splidejs").Include(
                      "~/splidejs/splide/dist/js/splide.min.js",
                      "~/js/splide.js"));

            bundles.Add(new ScriptBundle("~/bundles/toastr").Include(
                      "~/Scripts/toastr.js"));

            bundles.Add(new ScriptBundle("~/bundles/video").Include(
                      "~/js/video.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapbundle").Include(
                      "~/Scripts/bootstrap.bundle.js",
                      "~/Scripts/bootstrap.bundle.min.js",
                      "~/Scripts/bootstrap.esm.js",
                      "~/Scripts/bootstrap.esm.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerycore").Include(
                      "~/Scripts/jquery-3.7.1.js",
                      "~/Scripts/jquery-3.7.1.min.js",
                      "~/Scripts/jquery-3.7.1.slim.js",
                      "~/Scripts/jquery-3.7.1.slim.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryvalidate").Include(
                      "~/Scripts/jquery.validate.js",
                      "~/Scripts/jquery.validate.min.js",
                      "~/Scripts/jquery.validate.unobtrusive.js",
                      "~/Scripts/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap-grid.css",
                      "~/Content/bootstrap-grid.min.css",
                      "~/Content/bootstrap-grid.rtl.css",
                      "~/Content/bootstrap-grid.rtl.min.css",
                      "~/Content/bootstrap-reboot.css",
                      "~/Content/bootstrap-reboot.min.css",
                      "~/Content/bootstrap-reboot.rtl.css",
                      "~/Content/bootstrap-reboot.rtl.min.css",
                      "~/Content/bootstrap-utilities.css",
                      "~/Content/bootstrap-utilities.min.css",
                      "~/Content/bootstrap-utilities.rtl.css",
                      "~/Content/bootstrap-utilities.rtl.min.css",
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap.min.css",
                      "~/Content/bootstrap.rtl.css",
                      "~/Content/bootstrap.rtl.min.css",
                      "~/Content/PagedList.css",
                      "~/Content/Site.css",
                      "~/Content/toastr.css",
                      "~/Content/toastr.min.css"));


            bundles.Add(new StyleBundle("~/css").Include(
                      "~/css/About1.css",
                      "~/css/AnimeDetail.css",
                      "~/css/Category.css",
                      "~/css/Home.css",
                      "~/css/PrivacyandPolitic.css",
                      "~/css/User.css",
                      "~/css/VideoDetail.css",
                      "~/css/video.css"));

            bundles.Add(new StyleBundle("~/splidejs").Include(
                      "~/splidejs/splide/dist/css/themes/splide-skyblue.min.css",
                      "~/splidejs/splide/dist/css/splide.min.css"));

            bundles.Add(new StyleBundle("~/swiper").Include(
                      "~/swiper/swiper-bundle.min.css"));
        }
    }
}
