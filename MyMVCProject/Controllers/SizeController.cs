using Microsoft.AspNetCore.Mvc;
using MyMVCProject.DataModel;
using MyMVCProject.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MyMVCProject.Controllers
{
    public class SizeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SizeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /Size/SaveSelectedSize
        [HttpPost]
        public async Task<IActionResult> SaveSelectedSize([FromBody] SelectedSize request)
        {
            if (request == null || string.IsNullOrEmpty(request.Size))
                return Json(new { success = false, message = "Invalid size selection." });

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
                return Json(new { success = false, message = "User not logged in." });

            request.UserId = int.Parse(userIdClaim);

            _context.SelectedSizes.Add(request);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Size saved successfully!" });
        }

        // GET: /Size/GetSelectedSize?productId=10
        [HttpGet]
        public IActionResult GetSelectedSize(int productId)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
                return Json(new { success = false, message = "User not logged in." });

            int userId = int.Parse(userIdClaim);

            var size = _context.SelectedSizes
                .FirstOrDefault(s => s.ProductId == productId && s.UserId == userId);

            if (size == null)
                return Json(new { success = false, message = "No size selected." });

            return Json(new { success = true, size = size.Size });
        }
    }
}
