using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PumpPalace.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();

            ViewBag.Categories = categories;

            var currencies = _context.Currencies
                .Select(c => new SelectListItem
                {
                    Value = c.Code,
                    Text = c.Code
                })
                .ToList();

            ViewBag.Currencies = currencies;

            return View(products);
        }



        [HttpPost]
        public async Task<IActionResult> ManageProducts(Product product)
        {
            if (ModelState.IsValid)
            {
                // Walidacja kategorii
                if (!_context.Categories.Any(c => c.Id == product.CategoryId))
                {
                    ModelState.AddModelError("CategoryId", "Invalid category selected.");
                }

                // Walidacja waluty
                if (!_context.Currencies.Any(c => c.Code == product.Currency))
                {
                    ModelState.AddModelError("Currency", "Invalid currency selected.");
                }

                // Jeżeli model zawiera błędy, wróć do widoku
                if (ModelState.ErrorCount > 0)
                {
                    var products = await _context.Products.ToListAsync();

                    ViewBag.Categories = _context.Categories
                        .Select(c => new SelectListItem
                        {
                            Value = c.Id.ToString(),
                            Text = c.Name
                        })
                        .ToList();

                    ViewBag.Currencies = _context.Currencies
                        .Select(c => new SelectListItem
                        {
                            Value = c.Code,
                            Text = c.Code
                        })
                        .ToList();

                    return View(products);
                }

                // Ustaw domyślną wartość VAT, jeśli jest zero
                if (product.VAT == 0)
                {
                    product.VAT = 0.23M;
                }

                if (product.IsNew)
                {
                    product.NewUntil = DateTime.UtcNow.AddDays(3);
                }
                else
                {
                    product.NewUntil = null;
                }

                // Dodanie produktu
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ManageProducts));
            }

            // Przy błędnym modelu, wypełnij dane do widoku
            var productsError = await _context.Products.ToListAsync();

            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();

            ViewBag.Currencies = _context.Currencies
                .Select(c => new SelectListItem
                {
                    Value = c.Code,
                    Text = c.Code
                })
                .ToList();

            return View(productsError);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManageProducts");
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Przygotowanie listy kategorii i walut
            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();

            ViewBag.Currencies = _context.Currencies
                .Select(c => new SelectListItem
                {
                    Value = c.Code,
                    Text = c.Code
                })
                .ToList();

            return View(product);
        }



        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                // Walidacja kategorii
                if (!_context.Categories.Any(c => c.Id == product.CategoryId))
                {
                    ModelState.AddModelError("CategoryId", "Invalid category selected.");
                }

                // Walidacja waluty
                if (!_context.Currencies.Any(c => c.Code == product.Currency))
                {
                    ModelState.AddModelError("Currency", "Invalid currency selected.");
                }

                // Jeżeli model zawiera błędy, wróć do widoku
                if (ModelState.ErrorCount > 0)
                {
                    ViewBag.Categories = _context.Categories
                        .Select(c => new SelectListItem
                        {
                            Value = c.Id.ToString(),
                            Text = c.Name
                        })
                        .ToList();

                    ViewBag.Currencies = _context.Currencies
                        .Select(c => new SelectListItem
                        {
                            Value = c.Code,
                            Text = c.Code
                        })
                        .ToList();

                    return View(product);
                }

                // Ustaw domyślną wartość VAT, jeśli jest zero
                if (product.VAT == 0)
                {
                    product.VAT = 0.23M;
                }

                // Zaktualizowanie danych produktu
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ManageProducts));
            }

            // Przy błędnym modelu, wypełnij dane do widoku
            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();

            ViewBag.Currencies = _context.Currencies
                .Select(c => new SelectListItem
                {
                    Value = c.Code,
                    Text = c.Code
                })
                .ToList();

            return View(product);
        }


        public IActionResult Statistics()
        {
            return View(); // Wyświetlamy widok raportu sprzedaży
        }

        public IActionResult OrderManagement()
        {
            return View();
        }

        public IActionResult UserManagement()
        {
            return View();
        }

        public IActionResult Discounts()
        {
            return View();
        }

        public IActionResult InventoryManagement()
        {
            return View();
        }

        public IActionResult UserStatistics()
        {
            return View();
        }
        public IActionResult SalesReport()
        {
            return View();
        }
    }
}