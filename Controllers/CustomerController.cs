using Microsoft.AspNetCore.Mvc;

namespace PumpPalace.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
