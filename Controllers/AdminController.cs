﻿using AutoMapper;
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
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("ManageProducts");
            }
            
            return View(product); // Wróć do widoku z błędami walidacji
        }

        public IActionResult Statistics()
        {
            return View(); // Wyświetlamy widok raportu sprzedaży
        }

        public IActionResult OrderManagement()
        {
            var orders = _context.Orders
                .Include(o => o.CustomerId) // Dołączenie danych klienta
                .ToList();

            return View(orders);
        }
    }
}