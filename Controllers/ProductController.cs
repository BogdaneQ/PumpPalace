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

        public IActionResult ProductList(ProductFilterViewModel filters)
        {
            // Jeśli filtr jest pusty, inicjalizujemy nowy obiekt
            if (filters == null)
            {
                filters = new ProductFilterViewModel();
            }

            // Pobieramy wszystkie produkty
            var products = _context.Products.AsQueryable();

            // Filtracja po cenie minimalnej
            if (filters.MinPrice.HasValue)
            {
                products = products.Where(p => p.Price >= filters.MinPrice.Value);
            }

            // Filtracja po cenie maksymalnej
            if (filters.MaxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= filters.MaxPrice.Value);
            }

            // Filtracja po promocji 
            if (filters.OnDiscount.HasValue)
            {
                products = products.Where(p => p.DiscountPrice.HasValue);
            }

            // Filtracja po dostępności
            if (filters.InStock.HasValue && filters.InStock.Value)
            {
                products = products.Where(p => p.InStock > 0);
            }

            // Ustawiamy przefiltrowane produkty w modelu
            filters.Products = products.ToList();

            // Przekazujemy model do widoku
            return View(filters);
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
