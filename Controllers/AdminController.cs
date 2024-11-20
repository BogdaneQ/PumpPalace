using Microsoft.AspNetCore.Mvc;

namespace PumpPalace.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult AdminPanel()
        {
            return View();
        }

        public IActionResult ManageProducts()
        {
            return View();
        }

        public IActionResult Statistics()
        {
            return View();
        }
    }
}
