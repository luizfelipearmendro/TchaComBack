using Microsoft.AspNetCore.Mvc;

namespace TCBSistemaDeControle.Controllers
{
    public class LandingPageController : Controller
    {
        public IActionResult Index()
        {
            return View("LandingPage");
        }
    }
}
