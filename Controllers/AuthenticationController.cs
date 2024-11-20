using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PumpPalace.Models;
using System.Linq;

namespace PumpPalace.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly PumpPalaceDbContext _dbContext;
        private readonly IMapper _mapper;

        public AuthenticationController(PumpPalaceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult RegisterPage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterPage(CustomerRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_dbContext.Customers.Any(c => c.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Podany adres email jest już zajęty.");
                return View(model);
            }

            if (_dbContext.Customers.Any(c => c.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Podana nazwa użytkownika jest już zajęta.");
                return View(model);
            }

            var customer = _mapper.Map<Customer>(model);
            customer.Password = BCrypt.Net.BCrypt.HashPassword(model.Password); // Hashowanie hasła

            _dbContext.Customers.Add(customer);
            _dbContext.SaveChanges();

            return RedirectToAction("LoginPage");
        }

        [HttpGet]
        public IActionResult LoginPage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPasswordPage()
        {
            return View();
        }
    }
}
