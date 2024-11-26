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
                return RedirectToAction("LoginPage", "Authentication");
            }

            int userId;
            if (!int.TryParse(value, out userId))
            {
                return RedirectToAction("LoginPage", "Authentication");
            }

            var cart = await _dbContext.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product) // Załadowanie produktów z koszyka
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                var cartViewModel = cart.Items.Select(item => new CartItemViewModel
                {
                    CartItemId = item.CartItemId,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Price = item.Product.Price,
                    DiscountPrice = item.Product.DiscountPrice, // Załadowanie ceny po rabacie
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

            var existingCartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    CartId = cart.Id
                };
                cart.Items.Add(cartItem);
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginPage", "Authentication");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Find the cart for the logged-in user
            var cart = await _dbContext.Carts
                                       .Include(c => c.Items)
                                       .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                // Find the cart item to remove
                var cartItem = cart.Items.FirstOrDefault(item => item.CartItemId == cartItemId);

                if (cartItem != null)
                {
                    // Remove the item from the cart
                    cart.Items.Remove(cartItem);

                    // Save changes to the database
                    await _dbContext.SaveChangesAsync();
                }
            }

            // Redirect to the Cart page after removal
            return RedirectToAction("Cart");
        }

    }
}
