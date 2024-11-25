using Microsoft.AspNetCore.Mvc;

namespace PumpPalace.Controllers
{
    public class CustomerController : Controller
    {
        // Akcja do wyświetlania strony "Moje konto"
        public IActionResult MyAccount()
        {
            return View(); // Wyświetlamy widok MyAccount
        }

        // Akcja do wyświetlania historii zamówień
        public IActionResult OrderHistory()
        {
            return View(); // Wyświetlamy widok OrderHistory
        }

        // Akcja do wyświetlania ustawień profilu
        public IActionResult ProfileSettings()
        {
            return View(); // Wyświetlamy widok ProfileSettings
        }

        // Akcja do wyświetlania listy życzeń
        public IActionResult Wishlist()
        {
            return View(); // Wyświetlamy widok Wishlist
        }
    }
}
