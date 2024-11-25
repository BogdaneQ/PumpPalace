using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PumpPalace.Models;
using System.Linq;

namespace PumpPalace.Controllers
{
    public class ProductController : Controller
    {
        private readonly PumpPalaceDbContext _context;
        public ProductController(PumpPalaceDbContext context)
        {
            _context = context;
        }

        public IActionResult ProductList()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        public IActionResult ProductDetails()
        {
            return View();
        }
    }
}
