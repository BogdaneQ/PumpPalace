using Microsoft.AspNetCore.Mvc;

namespace PumpPalace.Controllers
{
    public class NewsletterControlle : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
