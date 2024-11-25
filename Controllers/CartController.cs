using Microsoft.AspNetCore.Mvc;
using PumpPalace.Models;

namespace PumpPalace.Controllers
{
    public class CartController : Controller
    {
        // Akcja do wyświetlania koszyka
        public IActionResult Cart()
        {
            return View(); // Po prostu wyświetlamy widok Cart, bez logiki
        }

        // Akcja do wyświetlania podsumowania zamówienia
        public IActionResult OrderSummary(int orderId)
        {
            return View(); // Po prostu wyświetlamy widok OrderSummary, bez logiki
        }

        // Akcja do wyświetlania płatności
        public IActionResult Payment(int orderId)
        {
            return View(); // Po prostu wyświetlamy widok Payment, bez logiki
        }
    }
}
