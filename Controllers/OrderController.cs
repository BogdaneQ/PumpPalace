using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PumpPalace.Models;

namespace PumpPalace.Controllers
{
    public class OrderController : Controller
    {
        private readonly PumpPalaceDbContext _dbContext;

        public OrderController(PumpPalaceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CancelOrder(int orderId)
        {
            // Pobierz zamówienie na podstawie ID
            var order = _dbContext.Orders.FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("OrderHistory");
            }

            // Sprawdź, czy zamówienie można anulować
            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Canceled)
            {
                TempData["ErrorMessage"] = "Order cannot be canceled as it is already completed or canceled.";
                return RedirectToAction("OrderHistory");
            }

            // Zmień status zamówienia na Canceled
            order.Status = OrderStatus.Canceled;

            // Zapisz zmiany w bazie danych
            _dbContext.SaveChanges();

            TempData["SuccessMessage"] = "Order has been canceled successfully.";
            return RedirectToAction("OrderHistory");
        }

        public async Task<IActionResult> OrderHistory()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginPage", "Authentication");
            }

            // Pobierz userId
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Pobierz historię zamówień użytkownika
            var orders = await _dbContext.Orders
                .Where(o => o.CustomerId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderHistoryViewModel
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.Status.ToString(),
                    TotalPrice = o.TotalPrice
                })
                .ToListAsync();

            var viewModel = new OrderHistoryListViewModel
            {
                Orders = orders ?? new List<OrderHistoryViewModel>()
            };

            return View(viewModel);
        }

    }
}
