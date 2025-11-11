using Microsoft.AspNetCore.Mvc;
using MyMVCProject.DataModel;
using PlayDressUp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PlayDressUp.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartItem request)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
                return Json(new { success = false, message = "Please log in first." });

            int userId = int.Parse(userIdClaim);

            // Check if item already exists (same product + size)
            var existingItem = _context.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == request.ProductId && c.Size == request.Size);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                var newItem = new CartItem
                {
                    UserId = userId,
                    ProductId = request.ProductId,
                    Size = request.Size,
                    Quantity = request.Quantity
                };
                _context.CartItems.Add(newItem);
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Item added to cart!" });
        }

        // GET: /Cart/GetUserCart
        [HttpGet]
        public IActionResult GetUserCart()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
                return Json(new { success = false, data = new object[0] });

            int userId = int.Parse(userIdClaim);

            var items = from c in _context.CartItems
                        join p in _context.Products on c.ProductId equals p.ProductId
                        where c.UserId == userId
                        select new
                        {
                            p.ProductId,
                            p.Title,
                            p.ImageUrl,
                            p.Price,
                            c.Size,
                            c.Quantity
                        };

            return Json(new { success = true, data = items.ToList() });
        }

        // POST: /Cart/Remove
        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] int productId)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
                return Json(new { success = false, message = "Please log in first." });

            int userId = int.Parse(userIdClaim);

            var item = _context.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }
    }
}
