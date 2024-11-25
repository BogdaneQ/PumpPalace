using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PumpPalace.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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

        // Rejestracja - Strona widoku
        [HttpGet]
        public IActionResult RegisterPage()
        {
            return View();
        }

        // Rejestracja - Przetwarzanie formularza
        [HttpPost]
        public IActionResult RegisterPage(CustomerRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Sprawdzenie, czy użytkownik z takim emailem już istnieje
            if (_dbContext.Customers.Any(c => c.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Podany adres email jest już zajęty.");
                return View(model);
            }

            // Sprawdzenie, czy użytkownik z takim username już istnieje
            if (_dbContext.Customers.Any(c => c.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Podana nazwa użytkownika jest już zajęta.");
                return View(model);
            }

            // Mapowanie modelu do klasy Customer
            var customer = _mapper.Map<Customer>(model);
            customer.Password = BCrypt.Net.BCrypt.HashPassword(model.Password); // Hashowanie hasła


            // Dodanie nowego użytkownika do bazy
            _dbContext.Customers.Add(customer);
            _dbContext.SaveChanges();

            // Po udanej rejestracji, przekierowanie na stronę logowania
            return RedirectToAction("LoginPage");
        }

        // Logowanie - Strona widoku
        [HttpGet]
        public IActionResult LoginPage()
        {
            return View();
        }

        // Logowanie - Przetwarzanie formularza
        [HttpPost]
        public async Task<IActionResult> LoginPage(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customer = _dbContext.Customers.FirstOrDefault(c => c.Email == model.Email);

            if (customer == null || !BCrypt.Net.BCrypt.Verify(model.Password, customer.Password))
            {
                ModelState.AddModelError(string.Empty, "Nieprawidłowy email lub hasło.");
                return View(model);
            }

            // Tworzenie listy claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, customer.Username),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim(ClaimTypes.Role, customer.IsAdmin ? "Admin" : "User") // Rola użytkownika
            };

            // Tworzenie identity i principal
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Zalogowanie użytkownika i zapisanie danych do cookies
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            // Przekierowanie na stronę główną po zalogowaniu
            return RedirectToAction("Index", "Home");
        }

        // Zapomniane hasło - Strona widoku
        [HttpGet]
        public IActionResult ForgotPasswordPage()
        {
            return View();
        }

        // Wylogowanie
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ChangePasswordPage()
        {
            var viewModel = new ChangePasswordViewModel();
            return View(viewModel);
        }
    }
}