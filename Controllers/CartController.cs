using Microsoft.AspNetCore.Mvc;

namespace PumpPalace.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
