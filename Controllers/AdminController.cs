using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PumpPalace.Models;

namespace PumpPalace.Controllers
{
    public class AdminController : Controller
    {
        private readonly PumpPalaceDbContext _context;

        public AdminController(PumpPalaceDbContext context)
        {
            _context = context;
        }
        public IActionResult AdminPanel()
        {
            return View();
        }

        public IActionResult ManageProducts()
        {
            var products = _context.Products.ToList();
            return View(products);
        }


        [HttpPost]
        public IActionResult ManageProducts(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("ManageProducts");
            }
            return View(); // Jeśli coś poszło nie tak
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageProducts");
        }


        public IActionResult Statistics()
        {
            return View();
        }
    }
}
