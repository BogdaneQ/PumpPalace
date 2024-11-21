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
            return View(new Product()); // Przekazujemy pusty model, aby formularz działał poprawnie
        }

        [HttpPost]
        public IActionResult ManageProducts(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("AdminPanel", "Admin");
            }
            
            return View(product); // Wróć do widoku z błędami walidacji
        }

        public IActionResult Statistics()
        {
            return View();
        }
    }
}
