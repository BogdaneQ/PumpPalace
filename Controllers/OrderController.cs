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

        public async Task<IActionResult> OrderDetails(int orderId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginPage", "Authentication");
            }

            // Pobierz userId
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Pobierz zamówienie
            var order = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == userId);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("OrderHistory");
            }

            // Utwórz ViewModel dla szczegółów zamówienia
            var orderSummary = new OrderSummaryViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                OrderStatus = order.Status.ToString(),
                TotalPrice = order.TotalPrice,
                ShippingAddress = _dbContext.Customers.FirstOrDefault(c => c.Id == userId)?.Address ?? "Unknown",
                BillingAddress = _dbContext.Customers.FirstOrDefault(c => c.Id == userId)?.Address ?? "Unknown",
                OrderItems = order.OrderItems.Select(item => new OrderItemViewModel
                {
                    ProductName = item.Product?.Name ?? "Unknown Product",
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            };

            return View(orderSummary);
        }


    }
}
