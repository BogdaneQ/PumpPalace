using Microsoft.AspNetCore.Mvc;

namespace PumpPalace.Controllers
{
    public class SkinController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
