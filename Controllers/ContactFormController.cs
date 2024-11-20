using Microsoft.AspNetCore.Mvc;

namespace PumpPalace.Controllers
{
    public class ContactFormController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
