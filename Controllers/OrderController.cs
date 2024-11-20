using Microsoft.AspNetCore.Mvc;

namespace PumpPalace.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
