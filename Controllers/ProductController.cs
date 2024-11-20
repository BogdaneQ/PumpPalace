using Microsoft.AspNetCore.Mvc;
using PumpPalace.Models;
using System.Linq;

namespace PumpPalace.Controllers
{
    public class ProductsController : Controller
    {
        private readonly PumpPalaceDbContext _context;

        public ProductsController(PumpPalaceDbContext context)
        {
            _context = context;
        }

        // GET: AddProduct
        public IActionResult AddProduct()
        {
            return View();
        }

        // POST: AddProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct(Product model)
        {
            if (ModelState.IsValid)
            {
                // Logika dodawania produktu do bazy danych
                _context.Products.Add(model);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Product added successfully!";
                return RedirectToAction("Products"); // Przekierowanie na stronę produktów
            }

            return View(model); // Jeśli wystąpił błąd, pozostajemy na tej samej stronie
        }

        // GET: Products
        public IActionResult Products()
        {
            var products = _context.Products.ToList(); // Pobierz produkty z bazy danych
            return View(products);
        }
    }
}
