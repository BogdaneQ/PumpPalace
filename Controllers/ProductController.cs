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

        public IActionResult ProductDetails(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                VAT = product.VAT,
                Currency = product.Currency,
                PictureUrl = product.PictureUrl,
                InStock = product.InStock,
                IsNew = product.IsNew,
                IsPromotion = product.IsPromotion
            };

            return View(viewModel);
        }

        public IActionResult ForHimPage()
        {
            int categoryIdForHim = _context.Categories.FirstOrDefault(c => c.Name == "ForHim")?.Id ?? 0;
            var products = _context.Products
                .Where(p => p.CategoryId == categoryIdForHim)
                .ToList();
            return View(products);
        }

        public IActionResult ForHerPage()
        {
            int categoryIdForHer = _context.Categories.FirstOrDefault(c => c.Name == "ForHer")?.Id ?? 0;
            var products = _context.Products
                .Where(p => p.CategoryId == categoryIdForHer)
                .ToList();
            return View(products);
        }

        public IActionResult AccessoriesPage()
        {
            int categoryIdAccessories = _context.Categories.FirstOrDefault(c => c.Name == "Accessories")?.Id ?? 0;
            var products = _context.Products
                .Where(p => p.CategoryId == categoryIdAccessories)
                .ToList();
            return View(products);
        }

        public IActionResult NewDropPage()
        {
            var newProducts = _context.Products
                .Where(p => p.IsNew && p.NewUntil.HasValue && p.NewUntil.Value > DateTime.UtcNow)
                .ToList();

            return View(newProducts);
        }


    }
}
