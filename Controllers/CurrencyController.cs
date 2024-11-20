using Microsoft.AspNetCore.Mvc;

namespace PumpPalace.Controllers
{
    public class CurrencyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
