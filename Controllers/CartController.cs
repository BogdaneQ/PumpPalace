using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

            // Pobierz dane użytkownika (adres i profil)
            var user = await _dbContext.Customers.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("LoginPage", "Authentication");
            }

            // Oblicz wartości zamówienia
            decimal totalNetPrice = cart.Items.Sum(item =>
                (item.Product.DiscountPrice ?? item.Product.Price) * item.Quantity);
            decimal totalPrice = totalNetPrice;

            // Utwórz zamówienie w bazie
            var order = new Order
            {
                CustomerId = userId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.New,
                TotalNetPrice = totalNetPrice,
                TotalPrice = totalPrice,
                OrderItems = cart.Items.Select(item => new OrderItem
                {
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity,
                    Price = item.Product.DiscountPrice ?? item.Product.Price,
                    VATAmount = (item.Product.DiscountPrice ?? item.Product.Price) * 0.23m // VAT od każdego produktu
                }).ToList()
            };

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            // Wyczyść koszyk
            _dbContext.Carts.Remove(cart);
            await _dbContext.SaveChangesAsync();

            // Utwórz ViewModel dla OrderSummary
            var orderSummary = new OrderSummaryViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                OrderStatus = order.Status.ToString(),
                TotalPrice = order.TotalPrice,
                ShippingAddress = user.Address,
                BillingAddress = user.Address,
                OrderItems = order.OrderItems.Select(item => new OrderItemViewModel
                {
                    ProductName = _dbContext.Products.Find(item.ProductId)?.Name ?? "Unknown Product",
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            };

            return View(orderSummary);
        }


        public IActionResult ProceedToPayment()
        {
                return RedirectToAction("PaymentPage", "Cart");
        }
        public IActionResult PaymentPage()
        {
            
            return View();
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


    }



}
