using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PumpPalace.Models;
using System.Linq;
using System.Collections.Generic;

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
                if (product.VAT == 0)
                {
                    product.VAT = 0.23M;
                }
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("ManageProducts"); // Odśwież widok po dodaniu
            }

            // Jeśli walidacja nie przeszła, przekazujemy obecne produkty do widoku
            var products = _context.Products.ToList();
            ViewData["Products"] = products;
            return View(products);
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            // Znajdujemy produkt na podstawie Id
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            return RedirectToAction("ManageProducts"); // Powrót do listy produktów
        }

        public IActionResult EditProduct(int id)
        {
            // Pobieramy produkt do edycji
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(); // Zwróć 404, jeśli produkt nie istnieje
            }

            return View(product); // Przekazujemy produkt do widoku edycji
        }

        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                // Aktualizujemy produkt
                _context.Products.Update(product);
                _context.SaveChanges();
                return RedirectToAction("ManageProducts"); // Powrót do widoku zarządzania
            }

            return View(product); // Powrót do widoku edycji w przypadku błędów
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