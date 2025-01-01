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
            if (filters == null)
            {
                filters = new ProductFilterViewModel();
            }

            var products = ApplyFilters(_context.Products.AsQueryable(), filters);
            filters.Products = products.ToList();

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

        public IActionResult ForHimPage(ProductFilterViewModel filters)
        {
            if (filters == null)
            {
                filters = new ProductFilterViewModel();
            }

            int categoryIdForHim = _context.Categories.FirstOrDefault(c => c.Name == "ForHim")?.Id ?? 0;
            var products = ApplyFilters(_context.Products.Where(p => p.CategoryId == categoryIdForHim), filters);
            filters.Products = products.ToList();

            return View(filters);
        }

        public IActionResult ForHerPage(ProductFilterViewModel filters)
        {
            if (filters == null)
            {
                filters = new ProductFilterViewModel();
            }

            int categoryIdForHer = _context.Categories.FirstOrDefault(c => c.Name == "ForHer")?.Id ?? 0;
            var products = ApplyFilters(_context.Products.Where(p => p.CategoryId == categoryIdForHer), filters);
            filters.Products = products.ToList();

            return View(filters);
        }

        public IActionResult AccessoriesPage(ProductFilterViewModel filters)
        {
            if (filters == null)
            {
                filters = new ProductFilterViewModel();
            }

            int categoryIdAccessories = _context.Categories.FirstOrDefault(c => c.Name == "Accessories")?.Id ?? 0;
            var products = ApplyFilters(_context.Products.Where(p => p.CategoryId == categoryIdAccessories), filters);
            filters.Products = products.ToList();

            return View(filters);
        }

        public IActionResult NewDropPage(ProductFilterViewModel filters)
        {
            if (filters == null)
            {
                filters = new ProductFilterViewModel();
            }

            var products = ApplyFilters(_context.Products.Where(p => p.IsNew && p.NewUntil.HasValue && p.NewUntil.Value > DateTime.UtcNow), filters);
            filters.Products = products.ToList();

            return View(filters);
        }

        private IQueryable<Product> ApplyFilters(IQueryable<Product> products, ProductFilterViewModel filters)
        {
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                string searchTerm = filters.SearchTerm.ToLower();
                products = products.Where(p => p.Name.ToLower().Contains(searchTerm));
            }

            if (filters.MinPrice.HasValue)
            {
                products = products.Where(p =>
                    (p.DiscountPrice.HasValue ? p.DiscountPrice.Value : p.Price) >= filters.MinPrice.Value);
            }

            if (filters.MaxPrice.HasValue)
            {
                products = products.Where(p =>
                    (p.DiscountPrice.HasValue ? p.DiscountPrice.Value : p.Price) <= filters.MaxPrice.Value);
            }

            if (filters.OnDiscount.HasValue && filters.OnDiscount.Value)
            {
                products = products.Where(p => p.DiscountPrice.HasValue);
            }

            if (filters.InStock.HasValue && filters.InStock.Value)
            {
                products = products.Where(p => p.InStock > 0);
            }
            if (filters.MinPrice == null)
            {
                filters.MinPrice = 0;
            }

            if (filters.MaxPrice == null)
            {
                filters.MaxPrice = 10000;
            }

            return products;
        }
    }
}
