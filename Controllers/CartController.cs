using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Web_QLNhaHang.Data;
using Web_QLNhaHang.Models;

namespace Web_QLNhaHang.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "Cart";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken] // Allow AJAX requests without token
        public async Task<IActionResult> AddToCart(int dishId, int quantity = 1, string? notes = null)
        {
            var dish = await _context.Dishes.FindAsync(dishId);
            if (dish == null || !dish.IsAvailable)
            {
                return Json(new { success = false, message = "Món ăn không tồn tại hoặc đã hết." });
            }

            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(c => c.DishId == dishId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                if (!string.IsNullOrEmpty(notes))
                {
                    existingItem.Notes = notes;
                }
            }
            else
            {
                cart.Add(new CartItem
                {
                    DishId = dish.DishId,
                    DishName = dish.DishName,
                    Price = dish.Price,
                    Quantity = quantity,
                    ImageUrl = dish.ImageUrl,
                    Notes = notes
                });
            }

            SaveCart(cart);
            return Json(new { success = true, cartCount = cart.Sum(c => c.Quantity) });
        }

        [HttpPost]
        public IActionResult UpdateCart(int dishId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.DishId == dishId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int dishId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.DishId == dishId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToAction("Index");
        }

        public IActionResult GetCartCount()
        {
            var cart = GetCart();
            return Json(new { count = cart.Sum(c => c.Quantity) });
        }

        [HttpPost]
        public IActionResult UpdateCartNotes(int dishId, string? notes)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.DishId == dishId);

            if (item != null)
            {
                item.Notes = notes ?? string.Empty;
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }
    }
}

