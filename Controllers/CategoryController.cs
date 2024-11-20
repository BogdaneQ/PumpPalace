using Microsoft.AspNetCore.Mvc;

namespace PumpPalace.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult NewDropPage()
        {
            return View();
        }

        public IActionResult AccesoriesPage()
        {
            return View();
        }

        public IActionResult ForHimPage()
        {
            return View();
        }

        public IActionResult ForHerPage()
        {
            return View();
        }
    }
}
