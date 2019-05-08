using System.Web.Mvc;

namespace MVCDiscografia.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Más información sobre este sitio.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Sugerencias y soporte.";

            return View();
        }
    }
}