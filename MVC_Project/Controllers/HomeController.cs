using System.Web.Mvc;

namespace MVC_Project.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        //[AllowAnonymous]
        public ActionResult Index()
        {
           
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}