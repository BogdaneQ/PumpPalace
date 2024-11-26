using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PumpPalace.Models;

namespace PumpPalace.Controllers
{

    public class CartController : Controller
    {
        private readonly PumpPalaceDbContext _dbContext;

        public CartController(PumpPalaceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Cart()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginPage", "Authentication");
            }

            string value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(value))
            {
                Console.WriteLine("User ID is missing.");
                return RedirectToAction("LoginPage", "Authentication");
            }

            int userId;
            if (!int.TryParse(value, out userId))
            {
                Console.WriteLine($"Failed to parse User ID: {value}");
                return RedirectToAction("LoginPage", "Authentication");
            }

            var cart = await _dbContext.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                var cartViewModel = cart.Items.Select(item => new CartItemViewModel
                {
                    CartItemId = item.CartItemId,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Price = item.Product.Price,
                    Quantity = item.Quantity
                }).ToList();

                return View(cartViewModel);
            }

            return View(new List<CartItemViewModel>());
        }



        public IActionResult OrderSummary(int orderId)
        {
            return View();
        }
        public IActionResult Payment(int orderId)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginPage", "Authentication");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Pobranie koszyka użytkownika
            var cart = await _dbContext.Carts.Include(c => c.Items)
                                             .FirstOrDefaultAsync(c => c.UserId == userId);

            // Jeżeli koszyk nie istnieje, tworzymy nowy
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Items = new List<CartItem>()
                };
                _dbContext.Carts.Add(cart);
                await _dbContext.SaveChangesAsync();
            }

            // Sprawdzanie, czy produkt już jest w koszyku
            var existingCartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingCartItem != null)
            {
                // Jeśli produkt już jest w koszyku, aktualizujemy ilość
                existingCartItem.Quantity += quantity;
            }
            else
            {
                // Dodawanie nowego produktu do koszyka
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    CartId = cart.Id
                };
                cart.Items.Add(cartItem);
            }

            // Zapisanie zmian w bazie danych
            await _dbContext.SaveChangesAsync();

            // Po dodaniu przekierowanie do widoku koszyka
            return RedirectToAction("Cart");
        }
    }
}
