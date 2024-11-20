using Microsoft.AspNetCore.Mvc;
using PumpPalace.Models;
using System.Linq;

namespace PumpPalace.Controllers
{
    public class ProductController : Controller
    {
        
        public IActionResult ProductList()
        {
            return View();
        }
    }
}
