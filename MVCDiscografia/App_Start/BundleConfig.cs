using System;
using System.Web.Optimization;
using System.Linq;
using System.Xml.Linq;

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
                //obtenemos la ubicación de packages.config para leer la versión utilizada de cada librería en el proyecto
                string packagesConfigFile = System.Web.HttpContext.Current.Server.MapPath("~/packages.config");
                XDocument configXML = XDocument.Load(packagesConfigFile);

                //obtenemos los ids de los paquetes y sus versiones. Sólo los que tienen un CDN asociado.
                var versionPackage = (from p in configXML.Descendants("package")
                                      where (new[] {
                                          "jQuery",
                                          "jQuery.Validation",
                                          "Microsoft.jQuery.Unobtrusive.Validation",
                                          "Modernizr",
                                          "bootstrap",
                                          "Respond" }).ToList().Contains(p.Attribute("id").Value)
                                      select new
                                      {
                                          id = p.Attribute("id").Value,
                                          version = p.Attribute("version").Value
                                      }).ToList();
                
                //jQuery
                ScriptBundle bndljQuery = new ScriptBundle("~/bundles/jquery",
                   Properties.Settings.Default.jQueryCdn.Replace("#version#",   //reemplazamos con el número de versión actual
                   versionPackage.Where(p => p.id == "jQuery").FirstOrDefault().version
                ));
                bndljQuery.Include("~/Scripts/jquery-{version}.js");
                bndljQuery.CdnFallbackExpression = "window.jQuery";
                bundles.Add(bndljQuery);

                //jQuery.validate
                ScriptBundle bndljQueryValidate = new ScriptBundle("~/bundles/jqueryval",
                    Properties.Settings.Default.jQueryValidateCdn.Replace("#version#",  //reemplazamos con el número de versión actual
                    versionPackage.Where(p => p.id == "jQuery.Validation").FirstOrDefault().version
                ));
                bndljQueryValidate.Include("~/Scripts/jquery.validate.min.js");
                bndljQueryValidate.CdnFallbackExpression = "$.validator";
                bundles.Add(bndljQueryValidate);

                //jQuery.validate Unobtrusive
                ScriptBundle bndlValidateUnobtrusive = new ScriptBundle("~/bundles/unobtrusive",
                    Properties.Settings.Default.ValidateUnobtrusiveCdn.Replace("#version#", //reemplazamos con el número de versión actual
                    versionPackage.Where(p => p.id == "Microsoft.jQuery.Unobtrusive.Validation").FirstOrDefault().version
                ));
                bndlValidateUnobtrusive.Include("~/Scripts/jquery.validate.unobtrusive.min.js");
                bndlValidateUnobtrusive.CdnFallbackExpression = "$.validator";  //typeof $().validate
                bundles.Add(bndlValidateUnobtrusive);

                //Modernizr
                // Use the development version of Modernizr to develop with and learn from. Then, when you're
                // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
                ScriptBundle bndlModernizr = new ScriptBundle("~/bundles/modernizr",
                    Properties.Settings.Default.ModernizrCdn.Replace("#version#",   //reemplazamos con el número de versión actual
                    versionPackage.Where(p => p.id == "Modernizr").FirstOrDefault().version
                ));
                bndlModernizr.Include("~/Scripts/modernizr-*");
                bndlModernizr.CdnFallbackExpression = "window.Modernizr";
                bundles.Add(bndlModernizr);

                //BootstrapJS
                ScriptBundle bndlBootstrapJs = new ScriptBundle("~/bundles/bootstrap",
                    Properties.Settings.Default.BootstrapJsCdn.Replace("#version#", //reemplazamos con el número de versión actual
                    versionPackage.Where(p => p.id == "bootstrap").FirstOrDefault().version
                    ));
                bndlBootstrapJs.Include(
                    "~/Scripts/bootstrap.js",
                    "~/Scripts/popper.js"
                );
                bndlBootstrapJs.CdnFallbackExpression = "$.fn.modal";
                bundles.Add(bndlBootstrapJs);

                //respondJS
                ScriptBundle bndlRespondJs = new ScriptBundle("~/bundles/respond",
                    Properties.Settings.Default.RespondCdn.Replace("#version#", //reemplazamos con el número de versión actual
                    versionPackage.Where(p => p.id == "Respond").FirstOrDefault().version
                    ));
                bndlRespondJs.Include("~/Scripts/respond.js");
                bndlRespondJs.CdnFallbackExpression = "window.respond";
                bundles.Add(bndlRespondJs);

                //BootstrapCss
                StyleBundle bndlBootstrapCss = new StyleBundle("~/Content/cssbootstrap",
                    Properties.Settings.Default.BootstrapCssCdn.Replace("#version#",    //reemplazamos con el número de versión actual
                    versionPackage.Where(p => p.id == "bootstrap").FirstOrDefault().version
                    ));
                bndlBootstrapCss.IncludeFallback("~/Content/bootstrap.css", "sr-only", "width", "1px");
                bundles.Add(bndlBootstrapCss);

                //Estilos locales
                bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/Site.css"));
            }catch(Exception)
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
