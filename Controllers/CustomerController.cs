using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PumpPalace.Models;

namespace PumpPalace.Controllers
{
    public class CustomerController : Controller
    {

        private readonly PumpPalaceDbContext _dbContext;

        public CustomerController(PumpPalaceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult MyAccount()
        {
            var user = GetLoggedInUser();
            if (user == null)
            {
                return RedirectToAction("LoginPage", "Authentication");
            }

            var model = new ProfileSettingsViewModel
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address
            };

            return View(model);
        }

        public IActionResult OrderHistory()
        {
            return View();
        }

        public IActionResult ProfileSettings()
        {
            var user = GetLoggedInUser();
            if (user == null)
            {
                return RedirectToAction("LoginPage", "Authentication");
            }

            var model = new ProfileSettingsViewModel
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateProfile(ProfileSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ProfileSettings", model);
            }

            var user = GetLoggedInUser();
            if (user == null)
            {
                return RedirectToAction("LoginPage", "Authentication");
            }

            // Aktualizacja danych użytkownika w bazie danych
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.Address = model.Address;

            _dbContext.SaveChanges();

            TempData["SuccessMessage"] = "Profil został zaktualizowany!";
            return RedirectToAction("MyAccount");
        }

        public IActionResult Wishlist()
        {
            return View(); // Wyświetlamy widok Wishlist
        }

        private Customer GetLoggedInUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            return _dbContext.Customers.FirstOrDefault(c => c.Email == email);
        }
    }
}
