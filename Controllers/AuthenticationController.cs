using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PumpPalace.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PumpPalace.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;

namespace PumpPalace.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly PumpPalaceDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly EmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public AuthenticationController(PumpPalaceDbContext dbContext, IMapper mapper, ILogger<AuthenticationController> logger, EmailSender emailSender,IConfiguration configuration)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _emailSender = emailSender;
            _configuration = configuration;
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

            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Email == model.Email);

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

        // ForgotPassword - Strona widoku
        [HttpGet]
        public IActionResult ForgotPasswordPage()
        {
            return View();
        }

        // ForgotPassword - Przetwarzanie formularza
        [HttpPost]
        public async Task<IActionResult> ForgotPasswordPage(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await SendPasswordResetEmail(model.Email))
            {
                TempData["SuccessMessage"] = "Na Twój adres e-mail wysłano link do resetowania hasła.";
            }
            else
            {
                TempData["ErrorMessage"] = "Wystąpił problem podczas wysyłania e-maila. Spróbuj ponownie.";
            }

            return RedirectToAction("ForgotPasswordPage");
        }


        // Resetowanie hasła - Strona widoku
        [HttpGet]
        public IActionResult ResetPasswordPage(string token)
        {
            var user = _dbContext.Customers.FirstOrDefault(c => c.PasswordResetToken == token && c.PasswordResetTokenExpiry > DateTime.UtcNow);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Token jest nieprawidłowy lub wygasł.";
                return RedirectToAction("ForgotPasswordPage");
            }

            var model = new ResetPasswordViewModel { Token = token };
            return View(model);
        }

        // Resetowanie hasła - Przetwarzanie formularza
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ResetPasswordPage", model);
            }

            // Sprawdzamy, czy oba hasła są zgodne
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Hasła nie są zgodne.");
                return View("ResetPasswordPage", model);
            }

            var user = _dbContext.Customers.FirstOrDefault(c => c.PasswordResetToken == model.Token && c.PasswordResetTokenExpiry > DateTime.UtcNow);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Token jest nieprawidłowy lub wygasł.";
                return RedirectToAction("ForgotPasswordPage");
            }

            // Hashujemy nowe hasło
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            _dbContext.SaveChanges();

            TempData["SuccessMessage"] = "Twoje hasło zostało zresetowane! Możesz się teraz zalogować.";
            return RedirectToAction("LoginPage");
        }

        // Send Password Reset Email
        private async Task<bool> SendPasswordResetEmail(string email)
        {
            var user = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);

            if (user == null)
            {
                return false; // Użytkownik nie istnieje w bazie
            }

            var resetToken = Guid.NewGuid().ToString();
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            await _dbContext.SaveChangesAsync();

            var resetLink = Url.Action("ResetPasswordPage", "Authentication", new { token = resetToken }, Request.Scheme);

            try
            {
                // Wyślij e-mail z linkiem do resetowania hasła
                await SendEmail(
                    email,
                    "Resetowanie hasła - PumpPalace",
                    $@"Kliknij poniższy link, aby zresetować swoje hasło:
            <a href='{resetLink}'>Zresetuj hasło</a>"
                );
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas wysyłania wiadomości e-mail.");
                return false;
            }
        }


        private async Task SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                var fromEmail = _configuration["SendGridSettings:FromEmail"];
                var fromName = _configuration["SendGridSettings:FromName"];
                var apiKey = _configuration["SendGridSettings:ApiKey"];

                var client = new SendGridClient(apiKey);

                var from = new EmailAddress(fromEmail, fromName);
                var to = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);

                // Wysłanie e-maila
                var response = await client.SendEmailAsync(msg);

                // Sprawdzenie odpowiedzi
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    // Logowanie szczegółów odpowiedzi, aby sprawdzić, co się stało
                    _logger.LogError($"Błąd przy wysyłaniu e-maila. Status: {response.StatusCode}, Body: {await response.Body.ReadAsStringAsync()}");

                    // Możesz też sprawdzić inne szczegóły odpowiedzi
                    var responseBody = await response.Body.ReadAsStringAsync();
                    _logger.LogError($"Błąd: {responseBody}");

                    // Możesz również dodać obsługę specyficznych kodów błędów, np. 400, 500
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        _logger.LogError("Zły request, sprawdź dane wejściowe.");
                    }
                    else if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        _logger.LogError("Błąd wewnętrzny serwera SendGrid.");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Wystąpił problem podczas wysyłania wiadomości e-mail.";
                _logger.LogError(ex, "Błąd podczas wysyłania wiadomości e-mail.");
            }
        }

        // Wylogowanie
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
