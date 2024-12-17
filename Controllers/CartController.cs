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

            if (quantity > product.InStock)
            {
                TempData["ErrorMessage"] = $"Cannot add more than {product.InStock} units of {product.Name} to the cart.";
                return RedirectToAction("ProductList"); // Powrót do listy produktów
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
                    return RedirectToAction("ProductList", "Product"); // Powrót do listy produktów
                }

                existingCartItem.Quantity = newQuantity;
                TempData["SuccessMessage"] = $"{product.Name} quantity updated in the cart.";
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
                TempData["SuccessMessage"] = $"{product.Name} has been added to the cart.";
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("ProductList", "Product"); // Powrót do listy produktów
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
                    TempData["SuccessMessage"] = "Product removed from the cart successfully.";
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

                if (quantity > product.InStock)
                {
                    TempData["ErrorMessage"] = $"Cannot set quantity to {quantity} for {product.Name}. Only {product.InStock} units are available.";
                    return RedirectToAction("Cart");
                }

                cartItem.Quantity = quantity;
                await _dbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = $"{product.Name} quantity updated successfully.";
            }

            return RedirectToAction("Cart");
        }


        public async Task<IActionResult> OrderSummary()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginPage", "Authentication");
            }

            // Pobierz userId
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Pobierz koszyk użytkownika
            var cart = await _dbContext.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Cart");
            }

            // Pobierz dane użytkownika (na potrzeby adresu i profilu)
            var user = await _dbContext.Customers.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("LoginPage", "Authentication");
            }

            // Oblicz łączną cenę
            decimal totalPrice = cart.Items.Sum(item =>
                (item.Product.DiscountPrice.HasValue && item.Product.DiscountPrice.Value < item.Product.Price
                    ? item.Product.DiscountPrice.Value
                    : item.Product.Price) * item.Quantity
            );

            // Utwórz ViewModel dla OrderSummary
            var orderSummary = new OrderSummaryViewModel
            {
                OrderId = new Random().Next(10000, 99999), // Tymczasowe generowanie numeru zamówienia
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                TotalPrice = totalPrice,
                ShippingAddress = user.Address, // Pobierz adres użytkownika
                BillingAddress = user.Address,
                OrderItems = cart.Items.Select(item => new OrderItemViewModel
                {
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    Price = item.Product.DiscountPrice ?? item.Product.Price // Uwzględnij cenę promocyjną
                }).ToList()
            };

            return View(orderSummary);
        }

        public IActionResult ProceedToPayment(OrderSummaryViewModel order)
        {
            if (ModelState.IsValid)
            {
                // Przetwarzanie danych zamówienia (zapis do bazy, generowanie zamówienia itp.)
                return RedirectToAction("PaymentPage", "Cart", order);
            }

            return View("OrderSummary", order);
        }

    }



}
