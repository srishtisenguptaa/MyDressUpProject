using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMVCProject.DataModel;
using MyMVCProject.Models;

namespace MyMVCProject.Controllers
{
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> Add([FromBody] WishlistRequest request)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId) || userId <= 0)
            {
                // User not logged in
                return Json(new { success = false, redirectToLogin = true, message = "Please log in first." });
            }

            if (request == null)
            {
                return Json(new { success = false, message = "Request body is null" });
            }

            bool exists = await _context.WishlistItems
                .AnyAsync(w => w.UserId == userId && w.ProductId == request.ProductId);

            if (!exists)
            {
                _context.WishlistItems.Add(new WishListItem
                {
                    UserId = userId,
                    ProductId = request.ProductId
                });

                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }


        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] WishlistRequest request)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null) return Json(new { success = false, data = new object[0] });

            int userId = int.Parse(userIdClaim);


            if (userId <= 0)
                return Json(new { success = false, message = "Please log in first." });
            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == request.ProductId);

            if (item != null)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetUserWishlist()
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Json(new { success = false, message = "Please log in first." });
            }

            var items = await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Id) // 👈 sort by WishlistItems.Id descending
                .Join(_context.Products,
                      w => w.ProductId,
                      p => p.ProductId,
                      (w, p) => new
                      {
                          p.ProductId,
                          p.Title,
                          p.Price,
                          p.ImageUrl
                      })
                .ToListAsync();

            return Json(new
            {
                success = true,
                data = items
            });
        }


        public class WishlistRequest
        {
            public int ProductId { get; set; }
        }
    }
}
