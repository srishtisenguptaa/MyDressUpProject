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
            var userId = HttpContext.Session.GetInt32("UserId");
            

            if (userId == null)
                return Json(new { success = false, message = "Please log in first." });
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
                    UserId = userId.Value,
                    ProductId = request.ProductId
                });

                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] WishlistRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Json(new { success = false });

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
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Please log in first." });
            }

            var items = await _context.WishlistItems
                .Where(w => w.UserId == userId)
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
