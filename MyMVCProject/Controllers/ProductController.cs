using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMVCProject.DataModel;
using System.Linq;
using System.Threading.Tasks;

namespace MyMVCProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products.Where(x => x.InStock == true).ToList();
            return Ok(products);
        }

        // ✅ FIXED ENDPOINT (no duplicate "api/product")
        [HttpGet("categories")]
        public async Task<IActionResult> GetDistinctCategories()
        {
            var categories = await _context.Products
                .Select(p => p.Type)
                .Distinct()
                .ToListAsync();

            return Ok(categories);
        }
    }
}
