using Microsoft.AspNetCore.Mvc;

namespace TchaComBack.Controllers
{
    public class LandingPageController : Controller
    {
        public IActionResult Index()
        {
            return View("LandingPage");
        }
    }
}
