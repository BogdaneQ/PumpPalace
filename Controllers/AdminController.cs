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


        // Zarządzanie zamówieniami
        public IActionResult OrderManagement()
        {
            return View(); // Wyświetlamy widok zarządzania zamówieniami
        }

        // Zarządzanie użytkownikami
        public IActionResult UserManagement()
        {
            return View(); // Wyświetlamy widok zarządzania użytkownikami
        }

        // Zarządzanie rabatami
        public IActionResult Discounts()
        {
            return View(); // Wyświetlamy widok zarządzania rabatami
        }

        // Zarządzanie stanem magazynowym
        public IActionResult InventoryManagement()
        {
            return View(); // Wyświetlamy widok zarządzania stanem magazynowym
        }

        // Statystyki użytkowników
        public IActionResult UserStatistics()
        {
            return View(); // Wyświetlamy widok statystyk użytkowników
        }

        // Raport sprzedaży
        public IActionResult SalesReport()
        {
            return View(); // Wyświetlamy widok raportu sprzedaży
        }
    }
}