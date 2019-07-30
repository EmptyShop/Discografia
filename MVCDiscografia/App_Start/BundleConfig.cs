using System;
using System.Web.Optimization;
using System.Configuration;

namespace MVCDiscografia
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            try
            {
                //jQuery
                ScriptBundle bndljQuery = new ScriptBundle("~/bundles/jquery",
                   Properties.Settings.Default.jQueryCdn    //@"//code.jquery.com/jquery-3.4.1.min.js"
                );
                bndljQuery.Include("~/Scripts/jquery-{version}.js");
                bndljQuery.CdnFallbackExpression = "window.jQuery";
                bundles.Add(bndljQuery);

                //jQuery.validate
                ScriptBundle bndljQueryValidate = new ScriptBundle("~/bundles/jqueryval",
                    Properties.Settings.Default.jQueryValidateCdn   //@"//cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.17.0/jquery.validate.min.js"
                );
                bndljQueryValidate.Include("~/Scripts/jquery.validate.min.js");
                bndljQueryValidate.CdnFallbackExpression = "$.validator";
                bundles.Add(bndljQueryValidate);

                //jQuery.validate Unobtrusive
                ScriptBundle bndlValidateUnobtrusive = new ScriptBundle("~/bundles/unobtrusive",
                    Properties.Settings.Default.ValidateUnobtrusiveCdn   //@"//cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/3.2.11/jquery.validate.unobtrusive.min.js"
                );
                bndlValidateUnobtrusive.Include("~/Scripts/jquery.validate.unobtrusive.min.js");
                bndlValidateUnobtrusive.CdnFallbackExpression = "$.validator";  //typeof $().validate
                bundles.Add(bndlValidateUnobtrusive);

                //Modernizr
                // Use the development version of Modernizr to develop with and learn from. Then, when you're
                // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
                ScriptBundle bndlModernizr = new ScriptBundle("~/bundles/modernizr",
                    Properties.Settings.Default.ModernizrCdn    //@"//cdnjs.cloudflare.com/ajax/libs/modernizr/2.8.3/modernizr.min.js"
                );
                bndlModernizr.Include("~/Scripts/modernizr-*");
                bndlModernizr.CdnFallbackExpression = "window.Modernizr";
                bundles.Add(bndlModernizr);

                //BootstrapJS
                ScriptBundle bndlBootstrapJs = new ScriptBundle("~/bundles/bootstrap",
                    Properties.Settings.Default.BootstrapJsCdn  //@"//stackpath.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"
                    );
                bndlBootstrapJs.Include(
                    "~/Scripts/bootstrap.js",
                    "~/Scripts/popper.js"
                );
                bndlBootstrapJs.CdnFallbackExpression = "$.fn.modal";
                bundles.Add(bndlBootstrapJs);

                //respondJS
                ScriptBundle bndlRespondJs = new ScriptBundle("~/bundles/respond",
                    Properties.Settings.Default.RespondCdn  //@"//cdnjs.cloudflare.com/ajax/libs/respond.js/1.4.0/respond.min.js"
                    );
                bndlRespondJs.Include("~/Scripts/respond.js");
                bndlRespondJs.CdnFallbackExpression = "window.respond";
                bundles.Add(bndlRespondJs);

                //BootstrapCss
                StyleBundle bndlBootstrapCss = new StyleBundle("~/Content/cssbootstrap",
                    Properties.Settings.Default.BootstrapCssCdn //@"//stackpath.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css"
                    );
                bndlBootstrapCss.IncludeFallback("~/Content/bootstrap.css", "sr-only", "width", "1px");
                bundles.Add(bndlBootstrapCss);

                //Estilos locales
                bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/Site.css"));
            }catch(Exception e)
            {
                /*Si ocurriera algún error al hacer el bundling, realizamos un bundling seguro (el bundling por default).*/
                bundles.Clear();

                bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

                bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                            "~/Scripts/jquery.validate.js"));

                bundles.Add(new ScriptBundle("~/bundles/unobtrusive").Include(
                            "~/Scripts/jquery.validate.unobtrusive.js"));

                // Use the development version of Modernizr to develop with and learn from. Then, when you're
                // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
                bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                            "~/Scripts/modernizr-*"));

                bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                          "~/Scripts/bootstrap.js"));
                bundles.Add(new ScriptBundle("~/bundles/respond").Include(
                          "~/Scripts/respond.js"));

                bundles.Add(new StyleBundle("~/Content/cssbootstrap").Include(
                          "~/Content/bootstrap.css"));
                bundles.Add(new StyleBundle("~/Content/css").Include(
                          "~/Content/site.css"));
            }

            BundleTable.EnableOptimizations = true;
        }
    }
}
