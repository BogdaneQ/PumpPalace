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

    }
}
