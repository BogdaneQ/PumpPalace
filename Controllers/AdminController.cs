using Microsoft.AspNetCore.Mvc;

namespace PumpPalace.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult AdminPanel()
        {
            return View();
        }
    }
}
