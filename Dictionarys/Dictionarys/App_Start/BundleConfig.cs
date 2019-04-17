using System.Web;
using System.Web.Optimization;

namespace Dictionarys
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/Scripts/site.js")
            );

            bundles.Add(new StyleBundle("~/Content/css").Include(
                 "~/Content/menu.css",
                 "~/Content/site.css"));
        }
    }
}