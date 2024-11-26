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
                    DiscountPrice = item.Product.DiscountPrice,
                    Quantity = item.Quantity,
                    InStock = item.Product.InStock // Add stock information
                }).ToList();

                return View(cartViewModel);
            }

            return View(new List<CartItemViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginPage", "Authentication");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Find product in the database to check available stock
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                return NotFound();
            }

            if (quantity > product.InStock) // Check if quantity is greater than stock
            {
                TempData["ErrorMessage"] = $"Cannot add more than {product.InStock} units of {product.Name} to the cart.";
                return RedirectToAction("Cart");
            }

            // Get or create the cart
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

            // Check if the product is already in the cart
            var existingCartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingCartItem != null)
            {
                int newQuantity = existingCartItem.Quantity + quantity;

                if (newQuantity > product.InStock)
                {
                    TempData["ErrorMessage"] = $"Cannot add more than {product.InStock} units of {product.Name} to the cart.";
                    return RedirectToAction("Cart");
                }

                existingCartItem.Quantity = newQuantity;
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

            var cart = await _dbContext.Carts
                                       .Include(c => c.Items)
                                       .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                var cartItem = cart.Items.FirstOrDefault(item => item.CartItemId == cartItemId);

                if (cartItem != null)
                {
                    cart.Items.Remove(cartItem);
                    await _dbContext.SaveChangesAsync();
                }
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginPage", "Authentication");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var cart = await _dbContext.Carts
                                       .Include(c => c.Items)
                                       .FirstOrDefaultAsync(c => c.UserId == userId);

            var cartItem = cart?.Items.FirstOrDefault(item => item.CartItemId == cartItemId);
            if (cartItem != null)
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);
                if (product == null)
                {
                    return NotFound();
                }

                if (quantity > product.InStock) // Ensure quantity does not exceed available stock
                {
                    TempData["ErrorMessage"] = $"Cannot set quantity to {quantity} for {product.Name}. Only {product.InStock} units are available.";
                    return RedirectToAction("Cart");
                }

                cartItem.Quantity = quantity;
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Cart");
        }

        public IActionResult OrderSummary(int orderId)
        {
            return View();
        }

        public IActionResult Payment(int orderId)
        {
            return View();
        }
    }



}
