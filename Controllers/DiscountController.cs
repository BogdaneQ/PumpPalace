using Microsoft.AspNetCore.Mvc;

namespace PumpPalace.Controllers
{
    public class DiscountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
