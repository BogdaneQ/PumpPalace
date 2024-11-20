using Microsoft.AspNetCore.Mvc;

namespace PumpPalace.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Cart()
        {
            return View();
        }
    }
}
