using System.Web;
using System.Web.Optimization;

namespace Repair.Web.Site
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //            "~/Scripts/bootstrap-3.3.5-dist/bootstrap.js"));

            //bundles.Add(new ScriptBundle("~/bundles/AjaxFormPost").Include(
            //            "~/Scripts/lzy/AjaxFormPost.js"));

            //bundles.Add(new ScriptBundle("~/bundles/LayerFunction").Include(
            //           "~/Scripts/lzy/LayerFunction.js"));

            //bundles.Add(new ScriptBundle("~/bundles/layer").Include(
            //          "~/Scripts/layer/layer.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryform").Include(
            //           "~/Scripts/jquery/jquery.form.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //           "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/WdatePicker").Include(
            //          "~/Content/My97DatePicker/WdatePicker.js"));

            //bundles.Add(new StyleBundle("~/Content/css/lzy").Include("~/Content/css/Style.css"));

            //bundles.Add(new StyleBundle("~/Content/css/bootstrap").Include(
            //    "~/Content/bootstrap-3.3.5-dist/css/bootstrap.css",
            //    "~/Content/bootstrap-3.3.5-dist/css/bootstrap-theme.css"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap-3.3.5-dist/fileinput.js"));
            bundles.Add(new StyleBundle("~/Content/css/bootstrap").Include(
                "~/Content/bootstrap-3.3.5-dist/css/fileinput.css"));
        }
    }
}