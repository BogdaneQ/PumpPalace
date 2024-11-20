using Microsoft.AspNetCore.Mvc;

namespace PumpPalace.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
